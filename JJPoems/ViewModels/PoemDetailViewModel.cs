using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class PoemDetailViewModel : BaseViewModel
{
	private readonly IFavoritesService _favoritesService;
	private readonly INavigationService _navigationService;
	private readonly IPoetryRepository _poetryRepository;
	private IReadOnlyList<Poem> _poems = [];
	private Poem? _currentPoem;
	private bool _isFavorite;

	public PoemDetailViewModel(IPoetryRepository poetryRepository, IFavoritesService favoritesService, INavigationService navigationService)
	{
		_poetryRepository = poetryRepository;
		_favoritesService = favoritesService;
		_navigationService = navigationService;
		ToggleFavoriteCommand = new Command(async () => await ToggleFavoriteAsync(), () => CurrentPoem is not null);
		GoToPreviousCommand = new Command(async () => await GoToAdjacentPoemAsync(-1), () => HasPrevious);
		GoToNextCommand = new Command(async () => await GoToAdjacentPoemAsync(1), () => HasNext);
	}

	public Poem? CurrentPoem
	{
		get => _currentPoem;
		private set
		{
			if (SetProperty(ref _currentPoem, value))
			{
				Title = value?.Title ?? "Detail básne";
				OnPropertyChanged(nameof(HasPrevious));
				OnPropertyChanged(nameof(HasNext));
				OnPropertyChanged(nameof(FavoriteButtonText));
				((Command)ToggleFavoriteCommand).ChangeCanExecute();
				((Command)GoToPreviousCommand).ChangeCanExecute();
				((Command)GoToNextCommand).ChangeCanExecute();
			}
		}
	}

	public bool IsFavorite
	{
		get => _isFavorite;
		private set
		{
			if (SetProperty(ref _isFavorite, value))
			{
				OnPropertyChanged(nameof(FavoriteButtonText));
			}
		}
	}

	public bool HasPrevious => GetCurrentIndex() > 0;

	public bool HasNext => GetCurrentIndex() >= 0 && GetCurrentIndex() < _poems.Count - 1;

	public string FavoriteButtonText => IsFavorite ? "Odobrať z obľúbených" : "Pridať medzi obľúbené";

	public ICommand ToggleFavoriteCommand { get; }

	public ICommand GoToPreviousCommand { get; }

	public ICommand GoToNextCommand { get; }

	public async Task LoadPoemAsync(string poemId)
	{
		IsBusy = true;
		try
		{
			_poems = await _poetryRepository.GetPoemsAsync();
			CurrentPoem = _poems.FirstOrDefault(poem => poem.Id.Equals(poemId, StringComparison.OrdinalIgnoreCase));
			if (CurrentPoem is not null)
			{
				IsFavorite = await _favoritesService.IsFavoriteAsync(CurrentPoem.Id);
			}
		}
		finally
		{
			IsBusy = false;
		}
	}

	private int GetCurrentIndex()
	{
		if (CurrentPoem is null)
		{
			return -1;
		}

		return _poems
			.Select((poem, index) => new { poem, index })
			.FirstOrDefault(item => item.poem.Id.Equals(CurrentPoem.Id, StringComparison.OrdinalIgnoreCase))
			?.index ?? -1;
	}

	private async Task GoToAdjacentPoemAsync(int direction)
	{
		var currentIndex = GetCurrentIndex();
		if (currentIndex < 0)
		{
			return;
		}

		var nextIndex = currentIndex + direction;
		if (nextIndex < 0 || nextIndex >= _poems.Count)
		{
			return;
		}

		await _navigationService.GoToPoemDetailAsync(_poems[nextIndex].Id);
	}

	private async Task ToggleFavoriteAsync()
	{
		if (CurrentPoem is null)
		{
			return;
		}

		IsFavorite = await _favoritesService.ToggleFavoriteAsync(CurrentPoem.Id);
	}
}
