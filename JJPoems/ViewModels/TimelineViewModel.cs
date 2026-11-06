using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class TimelineViewModel : BaseViewModel
{
	private readonly INavigationService _navigationService;
	private readonly IUserWorkspaceService _workspaceService;
	private readonly CultureInfo _culture = new("sk-SK");
	private readonly List<UserWriting> _allWritings = [];
	private bool _hasEntries;
	private bool _hasSelectedDayEntries;
	private DateOnly _displayMonth;
	private DateOnly? _selectedDate;
	private string _currentMonthLabel = string.Empty;
	private string _monthSummary = string.Empty;
	private string _selectedDayTitle = string.Empty;
	private string _selectedDaySubtitle = string.Empty;
	private string _summary = string.Empty;

	public TimelineViewModel(IUserWorkspaceService workspaceService, INavigationService navigationService)
	{
		_workspaceService = workspaceService;
		_navigationService = navigationService;
		Title = "Casova stopa";
		DayGroups = [];
		CalendarDays = [];
		SelectedDayEntries = [];
		OpenWritingCommand = new Command<UserWriting>(async writing =>
		{
			if (writing is not null)
			{
				await _navigationService.GoToWritingDetailAsync(writing.Id);
			}
		});
		SelectCalendarDayCommand = new Command<TimelineCalendarDayViewModel>(day =>
		{
			if (day is not null)
			{
				SelectDate(day.Date);
			}
		});
		GoToQuickCaptureCommand = new Command(async () => await _navigationService.GoToWritingEditorAsync(quickCapture: true));
		PreviousMonthCommand = new Command(() =>
		{
			_displayMonth = _displayMonth.AddMonths(-1);
			BuildCalendar();
		});
		NextMonthCommand = new Command(() =>
		{
			_displayMonth = _displayMonth.AddMonths(1);
			BuildCalendar();
		});
	}

	public ObservableCollection<TimelineDayGroup> DayGroups { get; }

	public ObservableCollection<TimelineCalendarDayViewModel> CalendarDays { get; }

	public ObservableCollection<UserWriting> SelectedDayEntries { get; }

	public ICommand OpenWritingCommand { get; }

	public ICommand SelectCalendarDayCommand { get; }

	public ICommand GoToQuickCaptureCommand { get; }

	public ICommand PreviousMonthCommand { get; }

	public ICommand NextMonthCommand { get; }

	public bool HasEntries
	{
		get => _hasEntries;
		private set => SetProperty(ref _hasEntries, value);
	}

	public bool HasSelectedDayEntries
	{
		get => _hasSelectedDayEntries;
		private set => SetProperty(ref _hasSelectedDayEntries, value);
	}

	public string CurrentMonthLabel
	{
		get => _currentMonthLabel;
		private set => SetProperty(ref _currentMonthLabel, value);
	}

	public string MonthSummary
	{
		get => _monthSummary;
		private set => SetProperty(ref _monthSummary, value);
	}

	public string SelectedDayTitle
	{
		get => _selectedDayTitle;
		private set => SetProperty(ref _selectedDayTitle, value);
	}

	public string SelectedDaySubtitle
	{
		get => _selectedDaySubtitle;
		private set => SetProperty(ref _selectedDaySubtitle, value);
	}

	public string Summary
	{
		get => _summary;
		private set => SetProperty(ref _summary, value);
	}

	public async Task RefreshAsync()
	{
		IsBusy = true;
		try
		{
			await _workspaceService.EnsureWorkspaceAsync();
			var writings = await _workspaceService.GetWritingsAsync();
			_allWritings.Clear();
			_allWritings.AddRange(writings.OrderByDescending(item => item.UpdatedAtUtc));

			var groups = _allWritings
				.GroupBy(writing => DateOnly.FromDateTime(writing.UpdatedAtUtc.ToLocalTime()))
				.OrderByDescending(group => group.Key)
				.Select(group => new TimelineDayGroup(group.Key, group.OrderByDescending(item => item.UpdatedAtUtc)))
				.ToList();

			DayGroups.Clear();
			foreach (var group in groups)
			{
				DayGroups.Add(group);
			}

			HasEntries = DayGroups.Count > 0;
			Summary = HasEntries
				? $"{_allWritings.Count} zapisov rozprestretych napriec tvojim casom a naladami."
				: "Zatial tu nic nie je. Prvy zachyt vytvori aj tvoju prvu casovu stopu.";

			var anchorDate = DayGroups.FirstOrDefault()?.Date ?? DateOnly.FromDateTime(DateTime.Today);
			_displayMonth = new DateOnly(anchorDate.Year, anchorDate.Month, 1);
			_selectedDate = DayGroups.FirstOrDefault()?.Date;
			BuildCalendar();
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void BuildCalendar()
	{
		var monthStart = new DateOnly(_displayMonth.Year, _displayMonth.Month, 1);
		var monthEnd = monthStart.AddMonths(1).AddDays(-1);
		var firstVisible = monthStart.AddDays(-GetLeadingOffset(monthStart));
		var countsByDate = _allWritings
			.GroupBy(writing => DateOnly.FromDateTime(writing.UpdatedAtUtc.ToLocalTime()))
			.ToDictionary(group => group.Key, group => group.Count());

		CalendarDays.Clear();
		for (var index = 0; index < 42; index++)
		{
			var date = firstVisible.AddDays(index);
			CalendarDays.Add(new TimelineCalendarDayViewModel
			{
				Date = date,
				EntryCount = countsByDate.TryGetValue(date, out var count) ? count : 0,
				IsCurrentMonth = date.Month == monthStart.Month && date.Year == monthStart.Year,
				IsSelected = _selectedDate == date
			});
		}

		if (_selectedDate is null || (_selectedDate.Value.Month != monthStart.Month || _selectedDate.Value.Year != monthStart.Year))
		{
			_selectedDate = countsByDate.Keys
				.Where(date => date.Month == monthStart.Month && date.Year == monthStart.Year)
				.OrderByDescending(date => date)
				.Cast<DateOnly?>()
				.FirstOrDefault() ?? monthStart;
		}

		foreach (var day in CalendarDays)
		{
			day.IsSelected = _selectedDate == day.Date;
		}

		var monthWritings = _allWritings
			.Where(writing =>
			{
				var localDate = DateOnly.FromDateTime(writing.UpdatedAtUtc.ToLocalTime());
				return localDate >= monthStart && localDate <= monthEnd;
			})
			.ToList();

		CurrentMonthLabel = _culture.TextInfo.ToTitleCase(monthStart.ToString("MMMM yyyy", _culture));
		MonthSummary = monthWritings.Count > 0
			? $"{monthWritings.Count} zapisov v tomto mesiaci. Dotkni sa dna a otvor svoju stopu."
			: "V tomto mesiaci zatial nie je ziaden zapis. Mozno je cas na novu myslienku.";

		SelectDate(_selectedDate ?? monthStart, false);
	}

	private void SelectDate(DateOnly date, bool updateCalendarSelection = true)
	{
		_selectedDate = date;

		if (updateCalendarSelection)
		{
			foreach (var day in CalendarDays)
			{
				day.IsSelected = day.Date == date;
			}
		}

		var items = _allWritings
			.Where(writing => DateOnly.FromDateTime(writing.UpdatedAtUtc.ToLocalTime()) == date)
			.OrderByDescending(writing => writing.UpdatedAtUtc)
			.ToList();

		SelectedDayEntries.Clear();
		foreach (var writing in items)
		{
			SelectedDayEntries.Add(writing);
		}

		HasSelectedDayEntries = SelectedDayEntries.Count > 0;
		SelectedDayTitle = GetDateTitle(date);
		SelectedDaySubtitle = HasSelectedDayEntries
			? $"{SelectedDayEntries.Count} zapisov pre tento den"
			: "V tento den si si este nic neulozil.";
	}

	private static int GetLeadingOffset(DateOnly date)
	{
		var dayOfWeek = (int)date.DayOfWeek;
		return dayOfWeek == 0 ? 6 : dayOfWeek - 1;
	}

	private string GetDateTitle(DateOnly date)
	{
		var today = DateOnly.FromDateTime(DateTime.Today);
		if (date == today)
		{
			return "Dnes";
		}

		if (date == today.AddDays(-1))
		{
			return "Vcera";
		}

		return date.ToString("dddd, d. MMMM yyyy", _culture);
	}
}
