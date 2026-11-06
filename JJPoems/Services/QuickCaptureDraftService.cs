using System.Text.Json;
using JJPoems.Models;

namespace JJPoems.Services;

public class QuickCaptureDraftService : IQuickCaptureDraftService
{
    private readonly IUserAccountService _accountService;

    public QuickCaptureDraftService(IUserAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<QuickCaptureDraft?> GetDraftAsync(string? writingId = null)
    {
        var path = await GetDraftPathAsync(writingId);
        if (!File.Exists(path))
        {
            return null;
        }

        return await SafeJsonStorage.ReadAsync<QuickCaptureDraft>(path);
    }

    public async Task SaveDraftAsync(QuickCaptureDraft draft)
    {
        var path = await GetDraftPathAsync(draft.WritingId);
        draft.UpdatedAtUtc = DateTime.UtcNow;
        await SafeJsonStorage.WriteAsync(path, draft);
    }

    public async Task ClearDraftAsync(string? writingId = null)
    {
        var path = await GetDraftPathAsync(writingId);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        if (File.Exists(path + ".bak")) File.Delete(path + ".bak");
    }

    private async Task<string> GetDraftPathAsync(string? writingId)
    {
        var account = await _accountService.GetCurrentAccountAsync()
            ?? throw new InvalidOperationException("No active account is available.");

        var suffix = string.IsNullOrEmpty(writingId) ? "quick-capture" : Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(writingId)));
        return Path.Combine(FileSystem.AppDataDirectory, "drafts", $"{account.Id}-{suffix}.json");
    }
}
