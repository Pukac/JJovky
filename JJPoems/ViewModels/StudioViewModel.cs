using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class StudioViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IUserWorkspaceService _workspaceService;
	private readonly List<UserWriting> _allWritings = [];
	private bool _hasEntries;
	private string _selectedSectionFilter = "Vsetko";
	private string _profileHeadline = "Tvoja tvorba";

	public StudioViewModel(IUserWorkspaceService workspaceService, INavigationService navigationService)
	{
		_workspaceService = workspaceService;
		_navigationService = navigationService;
		Title = "Tvorba";
		Writings = [];
		SectionFilters = ["Vsetko", "Basnicky", "Vylev hlavy", "Sny", "Dennik", "Napady"];
		CreateNewCommand = new Command(async () => await _navigationService.GoToWritingEditorAsync());
		QuickCaptureCommand = new Command(async () => await _navigationService.GoToWritingEditorAsync(quickCapture: true));
		GoToTimelineCommand = new Command(async () => await _navigationService.GoToTimelineAsync());
		OpenWritingCommand = new Command<UserWriting>(async writing =>
		{
			if (writing is not null)
			{
				await _navigationService.GoToWritingDetailAsync(writing.Id);
			}
		});
	}

	public ObservableCollection<UserWriting> Writings { get; }

	public IReadOnlyList<string> SectionFilters { get; }

	public ICommand CreateNewCommand { get; }

	public ICommand QuickCaptureCommand { get; }

	public ICommand GoToTimelineCommand { get; }

	public ICommand OpenWritingCommand { get; }

	public string SelectedSectionFilter
	{
		get => _selectedSectionFilter;
		set
		{
			if (SetProperty(ref _selectedSectionFilter, value))
			{
				ApplyFilter();
			}
		}
	}

	public string ProfileHeadline
	{
		get => _profileHeadline;
		private set => SetProperty(ref _profileHeadline, value);
	}

	public bool HasEntries
	{
		get => _hasEntries;
		private set => SetProperty(ref _hasEntries, value);
	}

	public async Task RefreshAsync()
	{
		IsBusy = true;
		try
		{
			await _workspaceService.EnsureWorkspaceAsync();
			var profile = await _workspaceService.GetProfileAsync();
			var writings = await _workspaceService.GetWritingsAsync();
			ProfileHeadline = profile.DisplayName;
			_allWritings.Clear();
			_allWritings.AddRange(writings);
			ApplyFilter();
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ApplyFilter()
	{
		var filtered = SelectedSectionFilter == "Vsetko"
			? _allWritings
			: _allWritings.Where(item => item.Section.Equals(SelectedSectionFilter, StringComparison.OrdinalIgnoreCase)).ToList();

		Writings.Clear();
		foreach (var writing in filtered)
		{
			Writings.Add(writing);
		}

		HasEntries = Writings.Count > 0;
	}
}
