using JJPoems.Models;

namespace JJPoems.Services;

public interface IUserAccountService
{
	bool HasActiveAccount();
	bool HasAnyAccount();
	Task<IReadOnlyList<UserAccount>> GetAccountsAsync();
	Task<UserAccount?> GetCurrentAccountAsync();
	Task<UserAccount> CreateOrActivateAccountAsync(string displayName, string email, string tagline, string preferredPaletteKey);
	Task SwitchAccountAsync(string accountId);
	Task UpdateCurrentAccountAsync(UserAccount account);
	Task SignOutAsync();
}
