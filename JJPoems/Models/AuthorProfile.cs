namespace JJPoems.Models;

public class AuthorProfile
{
	public string Name { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;
	public string Biography { get; set; } = string.Empty;
	public string WritingStyle { get; set; } = string.Empty;
	public string Inspirations { get; set; } = string.Empty;
	public List<string> Highlights { get; set; } = [];
	public string FavoriteQuote { get; set; } = string.Empty;
	public string ContactLabel { get; set; } = string.Empty;
	public string ContactUrl { get; set; } = string.Empty;
	public string PortraitImage { get; set; } = string.Empty;
}
