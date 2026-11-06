namespace JJPoems.Models;

public class QuickCaptureDraft
{
	public string? WritingId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Section { get; set; } = "Vylev hlavy";
	public string Mood { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public string PaletteKey { get; set; } = "sand";
	public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
