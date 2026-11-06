namespace JJPoems.Models;

public class UserAccount
{
	public string Id { get; set; } = Guid.NewGuid().ToString("N");
	public string Email { get; set; } = string.Empty;
	public string DisplayName { get; set; } = "Moje tiche miesto";
	public string Tagline { get; set; } = "Osobny priestor pre basne, sny a vnutorne zapisky.";
	public string PreferredPaletteKey { get; set; } = "sand";
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime LastUsedAtUtc { get; set; } = DateTime.UtcNow;
}
