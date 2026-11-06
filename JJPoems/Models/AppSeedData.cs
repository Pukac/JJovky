namespace JJPoems.Models;

public class AppSeedData
{
	public AuthorProfile Author { get; set; } = new();
	public List<Poem> Poems { get; set; } = [];
}
