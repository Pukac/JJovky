using JJPoems.Models;

namespace JJPoems.Services;

public interface IPoetryRepository
{
	Task<IReadOnlyList<Poem>> GetPoemsAsync();
	Task<IReadOnlyList<Poem>> GetFeaturedPoemsAsync();
	Task<Poem?> GetPoemByIdAsync(string poemId);
	Task<AuthorProfile> GetAuthorAsync();
}
