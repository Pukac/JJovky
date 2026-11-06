namespace JJPoems.Services;

public interface IFavoritesService
{
	Task<HashSet<string>> GetFavoriteIdsAsync();
	Task<bool> IsFavoriteAsync(string poemId);
	Task<bool> ToggleFavoriteAsync(string poemId);
}
