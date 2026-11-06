namespace JJPoems.Models;

public class Poem
{
	public string Id { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string Theme { get; set; } = string.Empty;
	public string Mood { get; set; } = string.Empty;
	public string Preview { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public bool IsFeatured { get; set; }
	public int ReadingTimeMinutes { get; set; }

	public string ReadingTimeLabel => $"{ReadingTimeMinutes} min čítania";
}
