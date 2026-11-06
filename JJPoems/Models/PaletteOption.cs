using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JJPoems.Models;

public class PaletteOption : INotifyPropertyChanged
{
	private bool _isSelected;

	public event PropertyChangedEventHandler? PropertyChanged;

	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string AccentHex { get; set; } = string.Empty;
	public string DarkAccentHex { get; set; } = string.Empty;
	public string PrimaryBackgroundHex { get; set; } = string.Empty;
	public string SecondaryBackgroundHex { get; set; } = string.Empty;
	public string PrimaryTextHex { get; set; } = string.Empty;
	public string SecondaryTextHex { get; set; } = string.Empty;
	public string AccentSoftHex { get; set; } = string.Empty;
	public string BorderDividerHex { get; set; } = string.Empty;
	public string CardHex { get; set; } = string.Empty;
	public string PanelHex { get; set; } = string.Empty;
	public string HeroBodyHex { get; set; } = string.Empty;
	public string WarmGlowInnerHex { get; set; } = string.Empty;
	public string WarmGlowOuterHex { get; set; } = string.Empty;
	public string AccentGlowHex { get; set; } = string.Empty;
	public string HeroSurfaceStartHex { get; set; } = string.Empty;
	public string HeroSurfaceEndHex { get; set; } = string.Empty;
	public string DarkPrimaryBackgroundHex { get; set; } = string.Empty;
	public string DarkSecondaryBackgroundHex { get; set; } = string.Empty;
	public string DarkPrimaryTextHex { get; set; } = string.Empty;
	public string DarkSecondaryTextHex { get; set; } = string.Empty;
	public string DarkBorderDividerHex { get; set; } = string.Empty;
	public string DarkCardHex { get; set; } = string.Empty;
	public string DarkPanelHex { get; set; } = string.Empty;
	public string DarkHeroBodyHex { get; set; } = string.Empty;
	public string DarkWarmGlowInnerHex { get; set; } = string.Empty;
	public string DarkWarmGlowOuterHex { get; set; } = string.Empty;
	public string DarkAccentGlowHex { get; set; } = string.Empty;
	public string DarkHeroSurfaceStartHex { get; set; } = string.Empty;
	public string DarkHeroSurfaceEndHex { get; set; } = string.Empty;

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
			OnPropertyChanged(nameof(SelectionStrokeHex));
			OnPropertyChanged(nameof(SelectionStrokeThickness));
		}
	}

	public string SelectionStrokeHex => IsSelected ? AccentHex : BorderDividerHex;

	public double SelectionStrokeThickness => IsSelected ? 2 : 1;

	public string PreviewChipHex => AccentSoftHex;

	public PaletteOption Clone()
	{
		return (PaletteOption)MemberwiseClone();
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
