using System.Text.Json;
using JJPoems.Models;

namespace JJPoems.Services;

public class JsonPoetryRepository : IPoetryRepository
{
	private readonly Lazy<Task<AppSeedData>> _seedDataLoader;

	public JsonPoetryRepository()
	{
		_seedDataLoader = new Lazy<Task<AppSeedData>>(LoadSeedDataAsync);
	}

	public async Task<AuthorProfile> GetAuthorAsync()
	{
		return (await _seedDataLoader.Value).Author;
	}

	public async Task<IReadOnlyList<Poem>> GetFeaturedPoemsAsync()
	{
		return (await GetPoemsAsync()).Where(poem => poem.IsFeatured).ToList();
	}

	public async Task<Poem?> GetPoemByIdAsync(string poemId)
	{
		return (await GetPoemsAsync()).FirstOrDefault(poem => poem.Id.Equals(poemId, StringComparison.OrdinalIgnoreCase));
	}

	public async Task<IReadOnlyList<Poem>> GetPoemsAsync()
	{
		return (await _seedDataLoader.Value).Poems;
	}

	private static async Task<AppSeedData> LoadSeedDataAsync()
	{
		await using var stream = await FileSystem.OpenAppPackageFileAsync("poetry-seed.json");
		using var reader = new StreamReader(stream);
		var json = await reader.ReadToEndAsync();

		var data = JsonSerializer.Deserialize<AppSeedData>(json, new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		}) ?? new AppSeedData();

		foreach (var poem in data.Poems)
		{
			poem.Content = poem.Content.Replace("\r\n", "\n").Trim();
			poem.Preview = poem.Preview.Replace("\r\n", "\n").Trim();
			poem.ReadingTimeMinutes = Math.Max(1, (int)Math.Ceiling(GetWordCount(poem.Content) / 170d));
		}

		return data;
	}

	private static int GetWordCount(string text)
	{
		return text
			.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
			.Length;
	}
}
