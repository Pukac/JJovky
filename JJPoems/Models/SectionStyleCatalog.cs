namespace JJPoems.Models;

public static class SectionStyleCatalog
{
	public static string GetAccentHex(string? section, string? paletteKey)
	{
		var palette = PaletteCatalog.Get(paletteKey);
		return Normalize(section) switch
		{
			"vylevhlavy" => ColorBlendHelper.Blend(palette.AccentHex, "#C07A7B", 0.45),
			"sny" => ColorBlendHelper.Blend(palette.AccentHex, "#98B6D0", 0.52),
			"dennik" => ColorBlendHelper.Blend(palette.AccentHex, "#8E735A", 0.35),
			"napady" => ColorBlendHelper.Blend(palette.AccentHex, "#C9A35C", 0.48),
			_ => ColorBlendHelper.Blend(palette.AccentHex, palette.PrimaryTextHex, 0.12)
		};
	}

	public static string GetSurfaceHex(string? section, string? paletteKey)
	{
		var palette = PaletteCatalog.Get(paletteKey);
		var accent = GetAccentHex(section, paletteKey);
		return ColorBlendHelper.Blend(palette.CardHex, accent, 0.16);
	}

	public static string GetBorderHex(string? section, string? paletteKey)
	{
		var palette = PaletteCatalog.Get(paletteKey);
		var accent = GetAccentHex(section, paletteKey);
		return ColorBlendHelper.Blend(palette.BorderDividerHex, accent, 0.34);
	}

	public static string GetTagHex(string? section, string? paletteKey)
	{
		var palette = PaletteCatalog.Get(paletteKey);
		var accent = GetAccentHex(section, paletteKey);
		return ColorBlendHelper.Blend(palette.SecondaryBackgroundHex, accent, 0.24);
	}

	private static string Normalize(string? value)
	{
		return new string((value ?? string.Empty)
			.Where(char.IsLetterOrDigit)
			.ToArray())
			.ToLowerInvariant();
	}
}
