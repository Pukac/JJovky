using System.Collections.ObjectModel;

namespace JJPoems.Models;

public class TimelineDayGroup : ObservableCollection<UserWriting>
{
	public TimelineDayGroup(DateOnly date, IEnumerable<UserWriting> writings)
		: base(writings)
	{
		Date = date;
	}

	public DateOnly Date { get; }

	public string Title
	{
		get
		{
			var today = DateOnly.FromDateTime(DateTime.Today);
			if (Date == today)
			{
				return "Dnes";
			}

			if (Date == today.AddDays(-1))
			{
				return "Vcera";
			}

			return Date.ToString("dd. MM. yyyy");
		}
	}

	public string Subtitle => $"{Count} zapisov v tomto dni";
}
