using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class UserWritingDetailViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IUserWorkspaceService _workspaceService;
	private IReadOnlyList<UserWriting> _writings = [];
	private UserWriting? _currentWriting;

	public UserWritingDetailViewModel(IUserWorkspaceService workspaceService, INavigationService navigationService)
	{
		_workspaceService = workspaceService;
		_navigationService = navigationService;
		Title = "Tvoj text";
		EditCommand = new Command(async () => await EditAsync(), () => CurrentWriting is not null);
		GoToPreviousCommand = new Command(async () => await GoToAdjacentWritingAsync(-1), () => HasPrevious);
		GoToNextCommand = new Command(async () => await GoToAdjacentWritingAsync(1), () => HasNext);
	}

	public UserWriting? CurrentWriting
	{
		get => _currentWriting;
		private set
		{
			if (SetProperty(ref _currentWriting, value))
			{
				Title = value?.Title ?? "Tvoj text";
				OnPropertyChanged(nameof(HasPrevious));
				OnPropertyChanged(nameof(HasNext));
				((Command)EditCommand).ChangeCanExecute();
				((Command)GoToPreviousCommand).ChangeCanExecute();
				((Command)GoToNextCommand).ChangeCanExecute();
			}
		}
	}

	public bool HasPrevious => GetCurrentIndex() > 0;

	public bool HasNext => GetCurrentIndex() >= 0 && GetCurrentIndex() < _writings.Count - 1;

	public ICommand EditCommand { get; }

	public ICommand GoToPreviousCommand { get; }

	public ICommand GoToNextCommand { get; }

	public async Task LoadWritingAsync(string writingId)
	{
		IsBusy = true;
		try
		{
			await _workspaceService.EnsureWorkspaceAsync();
			_writings = await _workspaceService.GetWritingsAsync();
			CurrentWriting = _writings.FirstOrDefault(item => item.Id.Equals(writingId, StringComparison.OrdinalIgnoreCase));
		}
		finally
		{
			IsBusy = false;
		}
	}

	private int GetCurrentIndex()
	{
		if (CurrentWriting is null)
		{
			return -1;
		}

		return _writings
			.Select((writing, index) => new { writing, index })
			.FirstOrDefault(item => item.writing.Id.Equals(CurrentWriting.Id, StringComparison.OrdinalIgnoreCase))
			?.index ?? -1;
	}

	private async Task EditAsync()
	{
		if (CurrentWriting is null)
		{
			return;
		}

		await _navigationService.GoToWritingEditorAsync(CurrentWriting.Id);
	}

	private async Task GoToAdjacentWritingAsync(int direction)
	{
		var currentIndex = GetCurrentIndex();
		if (currentIndex < 0)
		{
			return;
		}

		var nextIndex = currentIndex + direction;
		if (nextIndex < 0 || nextIndex >= _writings.Count)
		{
			return;
		}

		await _navigationService.GoToWritingDetailAsync(_writings[nextIndex].Id);
	}
}
