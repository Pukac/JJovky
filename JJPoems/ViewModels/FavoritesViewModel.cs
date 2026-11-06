using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class FavoritesViewModel : BaseViewModel
{
	private readonly IFavoritesService _favoritesService;
	private readonly INavigationService _navigationService;
	private readonly IPoetryRepository _poetryRepository;
	private bool _hasFavorites;

	public FavoritesViewModel(IPoetryRepository poetryRepository, IFavoritesService favoritesService, INavigationService navigationService)
	{
		_poetryRepository = poetryRepository;
		_favoritesService = favoritesService;
		_navigationService = navigationService;
		Title = "Obľúbené";
		FavoritePoems = [];
		OpenPoemCommand = new Command<Poem>(async poem =>
		{
			if (poem is not null)
			{
				await _navigationService.GoToPoemDetailAsync(poem.Id);
			}
		});
	}

	public ObservableCollection<Poem> FavoritePoems { get; }

	public ICommand OpenPoemCommand { get; }

	public bool HasFavorites
	{
		get => _hasFavorites;
		private set => SetProperty(ref _hasFavorites, value);
	}

	public async Task RefreshAsync()
	{
		IsBusy = true;
		try
		{
			var favoriteIds = await _favoritesService.GetFavoriteIdsAsync();
			var poems = await _poetryRepository.GetPoemsAsync();
			var favorites = poems.Where(poem => favoriteIds.Contains(poem.Id)).ToList();

			FavoritePoems.Clear();
			foreach (var poem in favorites)
			{
				FavoritePoems.Add(poem);
			}

			HasFavorites = FavoritePoems.Count > 0;
		}
		finally
		{
			IsBusy = false;
		}
	}
}
