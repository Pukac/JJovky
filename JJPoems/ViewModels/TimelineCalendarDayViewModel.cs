using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JJPoems.ViewModels;

public class TimelineCalendarDayViewModel : INotifyPropertyChanged
{
	private bool _isSelected;

	public event PropertyChangedEventHandler? PropertyChanged;

	public required DateOnly Date { get; init; }

	public required int EntryCount { get; init; }

	public required bool IsCurrentMonth { get; init; }

	public string DayNumber => Date.Day.ToString();

	public bool HasEntries => EntryCount > 0;

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if (_isSelected == value)
			{
				return;
			}

			_isSelected = value;
			OnPropertyChanged();
		}
	}

	public string EntryCountLabel => HasEntries ? $"{EntryCount}x" : string.Empty;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
