namespace JJPoems.Models;

public class UserAccountStore
{
	public string? CurrentAccountId { get; set; }
	public List<UserAccount> Accounts { get; set; } = [];
}
