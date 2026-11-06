using System.Text.Json;
using JJPoems.Models;

namespace JJPoems.Services;

public class UserWorkspaceService : IUserWorkspaceService
{
    private readonly SemaphoreSlim _writeGate = new(1, 1);
    private readonly SemaphoreSlim _initializationGate = new(1, 1);
    private readonly IUserAccountService _accountService;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public UserWorkspaceService(IUserAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task EnsureWorkspaceAsync()
    {
        await _initializationGate.WaitAsync();
        try
        {
            var account = await GetRequiredAccountAsync();
            var path = GetWorkspacePath(account.Id);
            if (File.Exists(path))
            {
                return;
            }

            var data = new UserWorkspaceData
            {
                Profile = new UserProfile
                {
                    DisplayName = account.DisplayName,
                    Tagline = account.Tagline,
                    PreferredPaletteKey = account.PreferredPaletteKey,
                    Bio = "Tvoj osobny priestor pre basnicky, sny, vylevy hlavy, drobne uvedomenia a vsetko, co si pyta byt zapisane."
                }
            };

            await SaveAsync(path, data);
        }
        finally { _initializationGate.Release(); }
    }

    public async Task DeleteWritingAsync(string writingId)
    {
        await _writeGate.WaitAsync();
        try
        {
            var path = await GetRequiredWorkspacePathAsync();
            var data = await LoadAsync(path);
            data.Writings.RemoveAll(item => item.Id.Equals(writingId, StringComparison.OrdinalIgnoreCase));
            await SaveAsync(path, data);
        }
        finally { _writeGate.Release(); }
    }

    public async Task<UserProfile> GetProfileAsync()
    {
        var path = await GetRequiredWorkspacePathAsync();
        return (await LoadAsync(path)).Profile;
    }

    public async Task<UserWriting?> GetWritingByIdAsync(string writingId)
    {
        var path = await GetRequiredWorkspacePathAsync();
        return (await LoadAsync(path)).Writings
            .OrderByDescending(item => item.UpdatedAtUtc)
            .FirstOrDefault(item => item.Id.Equals(writingId, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<UserWriting>> GetWritingsAsync()
    {
        var path = await GetRequiredWorkspacePathAsync();
        return (await LoadAsync(path)).Writings
            .OrderByDescending(item => item.UpdatedAtUtc)
            .ToList();
    }

    public async Task SaveProfileAsync(UserProfile profile)
    {
        await _writeGate.WaitAsync();
        try
        {
            var path = await GetRequiredWorkspacePathAsync();
            var data = await LoadAsync(path);
            data.Profile = profile;
            await SaveAsync(path, data);
        }
        finally { _writeGate.Release(); }
    }

    public async Task SaveWritingAsync(UserWriting writing)
    {
        await _writeGate.WaitAsync();
        try
        {
            var path = await GetRequiredWorkspacePathAsync();
            var data = await LoadAsync(path);
            var existing = data.Writings.FirstOrDefault(item => item.Id.Equals(writing.Id, StringComparison.OrdinalIgnoreCase));

            writing.Content = writing.Content.Trim();
            writing.Title = writing.Title.Trim();
            writing.Mood = writing.Mood.Trim();
            writing.Section = string.IsNullOrWhiteSpace(writing.Section) ? "Basnicky" : writing.Section.Trim();
            writing.UpdatedAtUtc = DateTime.UtcNow;

            if (existing is null)
            {
                writing.CreatedAtUtc = DateTime.UtcNow;
                data.Writings.Add(writing);
            }
            else
            {
                existing.Title = writing.Title;
                existing.Section = writing.Section;
                existing.Mood = writing.Mood;
                existing.Content = writing.Content;
                existing.PaletteKey = writing.PaletteKey;
                existing.UpdatedAtUtc = writing.UpdatedAtUtc;
            }

            await SaveAsync(path, data);
        }
        finally { _writeGate.Release(); }
    }

    public async Task<string> ExportAsync()
    {
        var data = await LoadAsync(await GetRequiredWorkspacePathAsync());
        return JsonSerializer.Serialize(data, _serializerOptions);
    }

    public async Task<int> ImportAsync(string json)
    {
        var imported = JsonSerializer.Deserialize<UserWorkspaceData>(json, _serializerOptions)
            ?? throw new JsonException("Invalid backup.");
        if (imported.Writings is null || imported.Writings.Count > 10000 ||
            imported.Writings.Any(w => w is null || string.IsNullOrWhiteSpace(w.Id) || w.Content is null || w.Title is null || w.Section is null || w.Mood is null))
            throw new JsonException("Invalid writings.");
        await _writeGate.WaitAsync();
        try
        {
            var path = await GetRequiredWorkspacePathAsync();
            var data = await LoadAsync(path);
            var ids = data.Writings.Select(w => w.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var added = imported.Writings.Where(w => ids.Add(w.Id)).ToList();
            data.Writings.AddRange(added);
            await SaveAsync(path, data);
            return added.Count;
        }
        finally { _writeGate.Release(); }
    }

    private async Task<UserWorkspaceData> LoadAsync(string storagePath)
    {
        if (!File.Exists(storagePath))
        {
            return new UserWorkspaceData();
        }

        return await SafeJsonStorage.ReadAsync<UserWorkspaceData>(storagePath) ?? new UserWorkspaceData();
    }

    private async Task SaveAsync(string storagePath, UserWorkspaceData data)
    {
        await SafeJsonStorage.WriteAsync(storagePath, data);
    }

    private string GetWorkspacePath(string accountId)
    {
        return Path.Combine(FileSystem.AppDataDirectory, "workspaces", $"{accountId}.json");
    }

    private async Task<UserAccount> GetRequiredAccountAsync()
    {
        var account = await _accountService.GetCurrentAccountAsync();
        return account ?? throw new InvalidOperationException("No active account is available.");
    }

    private async Task<string> GetRequiredWorkspacePathAsync()
    {
        var account = await GetRequiredAccountAsync();
        await EnsureWorkspaceAsync();
        return GetWorkspacePath(account.Id);
    }
}
