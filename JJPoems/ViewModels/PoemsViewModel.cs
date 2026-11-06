using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class PoemsViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IPoetryRepository _poetryRepository;
	private readonly List<Poem> _allPoems = [];
	private string _searchText = string.Empty;
	private bool _isInitialized;

	public PoemsViewModel(IPoetryRepository poetryRepository, INavigationService navigationService)
	{
		_poetryRepository = poetryRepository;
		_navigationService = navigationService;
		Title = "Inspiracia";
		FilteredPoems = [];
		OpenPoemCommand = new Command<Poem>(async poem =>
		{
			if (poem is not null)
			{
				await _navigationService.GoToPoemDetailAsync(poem.Id);
			}
		});
	}

	public ObservableCollection<Poem> FilteredPoems { get; }

	public ICommand OpenPoemCommand { get; }

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (SetProperty(ref _searchText, value))
			{
				ApplyFilter();
			}
		}
	}

	public async Task InitializeAsync()
	{
		if (_isInitialized)
		{
			return;
		}

		IsBusy = true;
		try
		{
			_allPoems.Clear();
			_allPoems.AddRange(await _poetryRepository.GetPoemsAsync());
			ApplyFilter();
			_isInitialized = true;
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ApplyFilter()
	{
		var term = SearchText.Trim();
		var filtered = string.IsNullOrWhiteSpace(term)
			? _allPoems
			: _allPoems.Where(poem =>
				poem.Title.Contains(term, StringComparison.CurrentCultureIgnoreCase) ||
				poem.Theme.Contains(term, StringComparison.CurrentCultureIgnoreCase) ||
				poem.Mood.Contains(term, StringComparison.CurrentCultureIgnoreCase)).ToList();

		FilteredPoems.Clear();
		foreach (var poem in filtered)
		{
			FilteredPoems.Add(poem);
		}
	}
}
