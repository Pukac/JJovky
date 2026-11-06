using System.Text.Json;
using Microsoft.Maui.Storage;

namespace JJPoems.Services;

public class FavoritesService : IFavoritesService
{
	private readonly IUserAccountService _accountService;

	public FavoritesService(IUserAccountService accountService)
	{
		_accountService = accountService;
	}

	public async Task<HashSet<string>> GetFavoriteIdsAsync()
	{
		var json = Preferences.Default.Get(await GetFavoritesKeyAsync(), string.Empty);
		if (string.IsNullOrWhiteSpace(json))
		{
			return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		var ids = JsonSerializer.Deserialize<List<string>>(json) ?? [];
		return new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase);
	}

	public async Task<bool> IsFavoriteAsync(string poemId)
	{
		return (await GetFavoriteIdsAsync()).Contains(poemId);
	}

	public async Task<bool> ToggleFavoriteAsync(string poemId)
	{
		var ids = await GetFavoriteIdsAsync();
		var isFavorite = !ids.Remove(poemId);
		if (isFavorite)
		{
			ids.Add(poemId);
		}

		Preferences.Default.Set(await GetFavoritesKeyAsync(), JsonSerializer.Serialize(ids.OrderBy(id => id).ToArray()));
		return isFavorite;
	}

	private async Task<string> GetFavoritesKeyAsync()
	{
		var account = await _accountService.GetCurrentAccountAsync();
		var accountId = account?.Id ?? "guest";
		return $"favorite-poem-ids-{accountId}";
	}
}
