using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class AboutViewModel : BaseViewModel
{
	private readonly IPoetryRepository _poetryRepository;
	private AuthorProfile _author = new();
	private bool _isInitialized;

	public AboutViewModel(IPoetryRepository poetryRepository)
	{
		_poetryRepository = poetryRepository;
		Title = "O kolekcii";
	}

	public AuthorProfile Author
	{
		get => _author;
		private set => SetProperty(ref _author, value);
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
			Author = await _poetryRepository.GetAuthorAsync();
			_isInitialized = true;
		}
		finally
		{
			IsBusy = false;
		}
	}
}
