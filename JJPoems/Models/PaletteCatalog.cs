namespace JJPoems.Models;

public static class PaletteCatalog
{
	private static readonly IReadOnlyList<PaletteOption> SourceOptions =
	[
		new()
		{
			Key = "sand",
			Name = "Piesok",
			AccentHex = "#A8825B",
			DarkAccentHex = "#D3AE84",
			PrimaryBackgroundHex = "#F7F1E8",
			SecondaryBackgroundHex = "#EFE4D1",
			PrimaryTextHex = "#2F2924",
			SecondaryTextHex = "#6E6257",
			AccentSoftHex = "#C9AE92",
			BorderDividerHex = "#D8C8B3",
			CardHex = "#FFFDFC",
			PanelHex = "#FFFCF7",
			HeroBodyHex = "#5F5349",
			WarmGlowInnerHex = "#FFF8EE",
			WarmGlowOuterHex = "#E9D8C3",
			AccentGlowHex = "#D6BC9F",
			HeroSurfaceStartHex = "#FFFDFC",
			HeroSurfaceEndHex = "#F3E7D8",
			DarkPrimaryBackgroundHex = "#161311",
			DarkSecondaryBackgroundHex = "#211C19",
			DarkPrimaryTextHex = "#F2E9DC",
			DarkSecondaryTextHex = "#B9A99A",
			DarkBorderDividerHex = "#4A3F38",
			DarkCardHex = "#1B1714",
			DarkPanelHex = "#201A17",
			DarkHeroBodyHex = "#C5B7A7",
			DarkWarmGlowInnerHex = "#3A2F28",
			DarkWarmGlowOuterHex = "#1A1613",
			DarkAccentGlowHex = "#7A6048",
			DarkHeroSurfaceStartHex = "#241E1B",
			DarkHeroSurfaceEndHex = "#171311"
		},
		new()
		{
			Key = "rose",
			Name = "Ruza",
			AccentHex = "#B87476",
			DarkAccentHex = "#E2A0A2",
			PrimaryBackgroundHex = "#FAF0EE",
			SecondaryBackgroundHex = "#F3E0DD",
			PrimaryTextHex = "#352725",
			SecondaryTextHex = "#7A5F5B",
			AccentSoftHex = "#D5A2A4",
			BorderDividerHex = "#E0C4C0",
			CardHex = "#FFFDFA",
			PanelHex = "#FFF8F6",
			HeroBodyHex = "#6D5552",
			WarmGlowInnerHex = "#FDEDEA",
			WarmGlowOuterHex = "#EACBCC",
			AccentGlowHex = "#DEB0B2",
			HeroSurfaceStartHex = "#FFFCFB",
			HeroSurfaceEndHex = "#F5E3E1",
			DarkPrimaryBackgroundHex = "#191312",
			DarkSecondaryBackgroundHex = "#251B1A",
			DarkPrimaryTextHex = "#F5E9E6",
			DarkSecondaryTextHex = "#C6AAA5",
			DarkBorderDividerHex = "#543E3C",
			DarkCardHex = "#201716",
			DarkPanelHex = "#261B1A",
			DarkHeroBodyHex = "#D8B8B4",
			DarkWarmGlowInnerHex = "#3B2827",
			DarkWarmGlowOuterHex = "#1D1514",
			DarkAccentGlowHex = "#8A5C5D",
			DarkHeroSurfaceStartHex = "#2A1E1D",
			DarkHeroSurfaceEndHex = "#191312"
		},
		new()
		{
			Key = "sage",
			Name = "Salvia",
			AccentHex = "#7E9776",
			DarkAccentHex = "#AFC7A4",
			PrimaryBackgroundHex = "#F2F4EE",
			SecondaryBackgroundHex = "#E2E8DB",
			PrimaryTextHex = "#283028",
			SecondaryTextHex = "#5D6959",
			AccentSoftHex = "#B6C6AF",
			BorderDividerHex = "#CED7C8",
			CardHex = "#FDFFFB",
			PanelHex = "#F7FAF2",
			HeroBodyHex = "#536051",
			WarmGlowInnerHex = "#F1F7EA",
			WarmGlowOuterHex = "#D7E2D1",
			AccentGlowHex = "#B5C7AC",
			HeroSurfaceStartHex = "#FCFEFB",
			HeroSurfaceEndHex = "#E8F0E2",
			DarkPrimaryBackgroundHex = "#131714",
			DarkSecondaryBackgroundHex = "#1B211D",
			DarkPrimaryTextHex = "#EAF0E7",
			DarkSecondaryTextHex = "#AAB7A6",
			DarkBorderDividerHex = "#3B473D",
			DarkCardHex = "#171C18",
			DarkPanelHex = "#1D241F",
			DarkHeroBodyHex = "#C2CEC0",
			DarkWarmGlowInnerHex = "#2A342C",
			DarkWarmGlowOuterHex = "#151A16",
			DarkAccentGlowHex = "#657A61",
			DarkHeroSurfaceStartHex = "#202721",
			DarkHeroSurfaceEndHex = "#141915"
		},
		new()
		{
			Key = "sky",
			Name = "Obloha",
			AccentHex = "#6E8FA8",
			DarkAccentHex = "#9FBFD6",
			PrimaryBackgroundHex = "#EEF4F7",
			SecondaryBackgroundHex = "#DDE8EF",
			PrimaryTextHex = "#243038",
			SecondaryTextHex = "#5D6E7A",
			AccentSoftHex = "#A9C0D1",
			BorderDividerHex = "#C7D7E1",
			CardHex = "#FCFEFF",
			PanelHex = "#F5FAFD",
			HeroBodyHex = "#52636F",
			WarmGlowInnerHex = "#EAF5FC",
			WarmGlowOuterHex = "#D7E5F1",
			AccentGlowHex = "#B5CBD9",
			HeroSurfaceStartHex = "#FEFFFF",
			HeroSurfaceEndHex = "#E5EFF6",
			DarkPrimaryBackgroundHex = "#11161A",
			DarkSecondaryBackgroundHex = "#182027",
			DarkPrimaryTextHex = "#E7EFF4",
			DarkSecondaryTextHex = "#A6B7C4",
			DarkBorderDividerHex = "#3C4C59",
			DarkCardHex = "#141B21",
			DarkPanelHex = "#1A232B",
			DarkHeroBodyHex = "#C2D0DB",
			DarkWarmGlowInnerHex = "#25313B",
			DarkWarmGlowOuterHex = "#13191F",
			DarkAccentGlowHex = "#5F7A90",
			DarkHeroSurfaceStartHex = "#1C252D",
			DarkHeroSurfaceEndHex = "#12181E"
		},
		new()
		{
			Key = "lavender",
			Name = "Levandula",
			AccentHex = "#8A7BAF",
			DarkAccentHex = "#B7A7D8",
			PrimaryBackgroundHex = "#F4F1F8",
			SecondaryBackgroundHex = "#E8E0F0",
			PrimaryTextHex = "#2F2835",
			SecondaryTextHex = "#695D74",
			AccentSoftHex = "#BDAED9",
			BorderDividerHex = "#D6CBE2",
			CardHex = "#FEFCFF",
			PanelHex = "#F9F5FC",
			HeroBodyHex = "#5E5569",
			WarmGlowInnerHex = "#F2ECF9",
			WarmGlowOuterHex = "#E3D7EF",
			AccentGlowHex = "#C6B6DE",
			HeroSurfaceStartHex = "#FFFFFF",
			HeroSurfaceEndHex = "#ECE2F5",
			DarkPrimaryBackgroundHex = "#15121A",
			DarkSecondaryBackgroundHex = "#1D1924",
			DarkPrimaryTextHex = "#EEE7F4",
			DarkSecondaryTextHex = "#B7ABC6",
			DarkBorderDividerHex = "#473D56",
			DarkCardHex = "#1A1621",
			DarkPanelHex = "#211B2A",
			DarkHeroBodyHex = "#D0C4DD",
			DarkWarmGlowInnerHex = "#2E2639",
			DarkWarmGlowOuterHex = "#17131D",
			DarkAccentGlowHex = "#75668F",
			DarkHeroSurfaceStartHex = "#241D2D",
			DarkHeroSurfaceEndHex = "#16121B"
		}
	];

	public static IReadOnlyList<PaletteOption> Options => CreateOptions();

	public static PaletteOption Get(string? key)
	{
		return SourceOptions.FirstOrDefault(option => option.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
			?.Clone() ?? SourceOptions[0].Clone();
	}

	public static List<PaletteOption> CreateOptions(string? selectedKey = null)
	{
		return SourceOptions
			.Select(option =>
			{
				var clone = option.Clone();
				clone.IsSelected = clone.Key.Equals(selectedKey, StringComparison.OrdinalIgnoreCase);
				return clone;
			})
			.ToList();
	}

	public static void MarkSelected(IEnumerable<PaletteOption> options, string? selectedKey)
	{
		foreach (var option in options)
		{
			option.IsSelected = option.Key.Equals(selectedKey, StringComparison.OrdinalIgnoreCase);
		}
	}
}
