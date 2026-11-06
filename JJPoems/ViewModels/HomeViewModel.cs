using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class HomeViewModel : BaseViewModel
{
	private readonly IUserAccountService _accountService;
	private readonly INavigationService _navigationService;
	private readonly IPoetryRepository _poetryRepository;
	private readonly IUserWorkspaceService _workspaceService;
	private string _librarySummary = string.Empty;
	private string _welcomeName = "Tvoje miesto";
	private string _spaceTagline = string.Empty;
	private string _personalSummary = string.Empty;
	private string _recentWritingTitle = string.Empty;
	private string _recentWritingPreview = string.Empty;
	private string _recentWritingId = string.Empty;
	private bool _hasRecentWriting;

	public HomeViewModel(
		IPoetryRepository poetryRepository,
		INavigationService navigationService,
		IUserWorkspaceService workspaceService,
		IUserAccountService accountService)
	{
		_poetryRepository = poetryRepository;
		_navigationService = navigationService;
		_workspaceService = workspaceService;
		_accountService = accountService;
		Title = "JJ Poems";
		FeaturedPoems = [];
		GoToPoemsCommand = new Command(async () => await _navigationService.GoToPoemsAsync());
		GoToAboutCommand = new Command(async () => await _navigationService.GoToAboutAsync());
		GoToStudioCommand = new Command(async () => await _navigationService.GoToStudioAsync());
		GoToQuickCaptureCommand = new Command(async () => await _navigationService.GoToWritingEditorAsync(quickCapture: true));
		GoToTimelineCommand = new Command(async () => await _navigationService.GoToTimelineAsync());
		OpenRecentWritingCommand = new Command(async () => await OpenRecentWritingAsync(), () => HasRecentWriting);
		OpenPoemCommand = new Command<Poem>(async poem =>
		{
			if (poem is not null)
			{
				await _navigationService.GoToPoemDetailAsync(poem.Id);
			}
		});
	}

	public ObservableCollection<Poem> FeaturedPoems { get; }

	public ICommand GoToPoemsCommand { get; }

	public ICommand GoToAboutCommand { get; }

	public ICommand GoToStudioCommand { get; }

	public ICommand GoToQuickCaptureCommand { get; }

	public ICommand GoToTimelineCommand { get; }

	public ICommand OpenRecentWritingCommand { get; }

	public ICommand OpenPoemCommand { get; }

	public string LibrarySummary
	{
		get => _librarySummary;
		private set => SetProperty(ref _librarySummary, value);
	}

	public string WelcomeName
	{
		get => _welcomeName;
		private set => SetProperty(ref _welcomeName, value);
	}

	public string SpaceTagline
	{
		get => _spaceTagline;
		private set => SetProperty(ref _spaceTagline, value);
	}

	public string PersonalSummary
	{
		get => _personalSummary;
		private set => SetProperty(ref _personalSummary, value);
	}

	public string RecentWritingTitle
	{
		get => _recentWritingTitle;
		private set => SetProperty(ref _recentWritingTitle, value);
	}

	public string RecentWritingPreview
	{
		get => _recentWritingPreview;
		private set => SetProperty(ref _recentWritingPreview, value);
	}

	public bool HasRecentWriting
	{
		get => _hasRecentWriting;
		private set
		{
			if (SetProperty(ref _hasRecentWriting, value))
			{
				((Command)OpenRecentWritingCommand).ChangeCanExecute();
			}
		}
	}

	public async Task InitializeAsync()
	{
		IsBusy = true;
		try
		{
			var account = await _accountService.GetCurrentAccountAsync();
			await _workspaceService.EnsureWorkspaceAsync();
			var profile = await _workspaceService.GetProfileAsync();
			var writings = await _workspaceService.GetWritingsAsync();
			var allPoems = await _poetryRepository.GetPoemsAsync();
			var poems = allPoems.Where(poem => poem.IsFeatured).Take(4).ToList();

			FeaturedPoems.Clear();
			foreach (var poem in poems)
			{
				FeaturedPoems.Add(poem);
			}

			WelcomeName = account?.DisplayName ?? profile.DisplayName;
			SpaceTagline = profile.Tagline;
			PersonalSummary = writings.Count == 0
				? "Tvoj priestor je pripraveny. Skus prvu basnicku, sen, napad alebo volny myslienkovy tok."
				: $"{writings.Count} vlastnych zapisov rozlozenych medzi sekcie pre jemne citanie aj tvorbu.";
			LibrarySummary = $"{allPoems.Count} textov vo vyvojovej kolekcii sluzi ako inspiracia a literarny sprievod.";

			var recentWriting = writings.FirstOrDefault();
			HasRecentWriting = recentWriting is not null;
			RecentWritingTitle = recentWriting?.Title ?? string.Empty;
			RecentWritingPreview = recentWriting?.Preview ?? string.Empty;
			_recentWritingId = recentWriting?.Id ?? string.Empty;
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task OpenRecentWritingAsync()
	{
		if (string.IsNullOrWhiteSpace(_recentWritingId))
		{
			return;
		}

		await _navigationService.GoToWritingDetailAsync(_recentWritingId);
	}
}
