using System.Text.Json;
using JJPoems.Models;
using Microsoft.Maui.Storage;

namespace JJPoems.Services;

public class UserAccountService : IUserAccountService
{
	private const string CurrentAccountPreferenceKey = "current-account-id";


	private readonly string _storagePath = Path.Combine(FileSystem.AppDataDirectory, "user-accounts.json");

	public bool HasActiveAccount()
	{
		return !string.IsNullOrWhiteSpace(Preferences.Default.Get(CurrentAccountPreferenceKey, string.Empty));
	}

	public bool HasAnyAccount()
	{
		return LoadStore().Accounts.Count > 0;
	}

	public Task<IReadOnlyList<UserAccount>> GetAccountsAsync()
	{
		var accounts = LoadStore().Accounts
			.OrderByDescending(account => account.LastUsedAtUtc)
			.ToList();

		return Task.FromResult<IReadOnlyList<UserAccount>>(accounts);
	}

	public Task<UserAccount?> GetCurrentAccountAsync()
	{
		var store = LoadStore();
		var currentId = GetCurrentAccountId();
		var account = store.Accounts.FirstOrDefault(item => item.Id.Equals(currentId, StringComparison.OrdinalIgnoreCase));
		return Task.FromResult(account);
	}

	public Task<UserAccount> CreateOrActivateAccountAsync(string displayName, string email, string tagline, string preferredPaletteKey)
	{
		var normalizedEmail = NormalizeEmail(email);
		var safeDisplayName = string.IsNullOrWhiteSpace(displayName) ? "Moje tiche miesto" : displayName.Trim();
		var safeTagline = string.IsNullOrWhiteSpace(tagline)
			? "Osobny priestor pre verse, sny a myslienkove pochody."
			: tagline.Trim();

		var store = LoadStore();
		var existing = store.Accounts.FirstOrDefault(item => item.Email.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase));

		if (existing is null)
		{
			existing = new UserAccount
			{
				Id = Guid.NewGuid().ToString("N"),
				Email = normalizedEmail,
				DisplayName = safeDisplayName,
				Tagline = safeTagline,
				PreferredPaletteKey = preferredPaletteKey,
				CreatedAtUtc = DateTime.UtcNow,
				LastUsedAtUtc = DateTime.UtcNow
			};

			store.Accounts.Add(existing);
		}
		else
		{
			existing.DisplayName = safeDisplayName;
			existing.Tagline = safeTagline;
			existing.PreferredPaletteKey = preferredPaletteKey;
			existing.LastUsedAtUtc = DateTime.UtcNow;
		}

		store.CurrentAccountId = existing.Id;
		SaveStore(store);
		return Task.FromResult(existing);
	}

	public Task SwitchAccountAsync(string accountId)
	{
		var store = LoadStore();
		var account = store.Accounts.FirstOrDefault(item => item.Id.Equals(accountId, StringComparison.OrdinalIgnoreCase));
		if (account is null)
		{
			return Task.CompletedTask;
		}

		account.LastUsedAtUtc = DateTime.UtcNow;
		store.CurrentAccountId = account.Id;
		SaveStore(store);
		return Task.CompletedTask;
	}

	public Task UpdateCurrentAccountAsync(UserAccount account)
	{
		var store = LoadStore();
		var currentId = GetCurrentAccountId();
		var existing = store.Accounts.FirstOrDefault(item => item.Id.Equals(currentId, StringComparison.OrdinalIgnoreCase));
		if (existing is null)
		{
			return Task.CompletedTask;
		}

		existing.DisplayName = string.IsNullOrWhiteSpace(account.DisplayName) ? existing.DisplayName : account.DisplayName.Trim();
		existing.Email = NormalizeEmail(account.Email);
		existing.Tagline = string.IsNullOrWhiteSpace(account.Tagline) ? existing.Tagline : account.Tagline.Trim();
		existing.PreferredPaletteKey = string.IsNullOrWhiteSpace(account.PreferredPaletteKey) ? existing.PreferredPaletteKey : account.PreferredPaletteKey;
		existing.LastUsedAtUtc = DateTime.UtcNow;

		SaveStore(store);
		return Task.CompletedTask;
	}

	public Task SignOutAsync()
	{
		var store = LoadStore();
		store.CurrentAccountId = null;
		SaveStore(store);
		return Task.CompletedTask;
	}

	private string GetCurrentAccountId()
	{
		return Preferences.Default.Get(CurrentAccountPreferenceKey, string.Empty);
	}

	private UserAccountStore LoadStore()
	{
		if (!File.Exists(_storagePath))
		{
			return new UserAccountStore
			{
				CurrentAccountId = GetCurrentAccountId()
			};
		}

		var store = SafeJsonStorage.ReadAsync<UserAccountStore>(_storagePath).GetAwaiter().GetResult() ?? new UserAccountStore();
		store.CurrentAccountId = string.IsNullOrWhiteSpace(store.CurrentAccountId) ? GetCurrentAccountId() : store.CurrentAccountId;
		return store;
	}

	private void SaveStore(UserAccountStore store)
	{
		SafeJsonStorage.WriteAsync(_storagePath, store).GetAwaiter().GetResult();
		Preferences.Default.Set(CurrentAccountPreferenceKey, store.CurrentAccountId ?? string.Empty);
	}

	private static string NormalizeEmail(string email)
	{
		return email.Trim().ToLowerInvariant();
	}
}
