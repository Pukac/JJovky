namespace JJPoems.Models;

public class UserWriting
{
	public string Id { get; set; } = Guid.NewGuid().ToString("N");
	public string Title { get; set; } = string.Empty;
	public string Section { get; set; } = "Basnicky";
	public string Mood { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public string PaletteKey { get; set; } = "sand";
	public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

	public string Preview
	{
		get
		{
			var preview = Content.Replace("\r", string.Empty).Trim();
			if (preview.Length <= 120)
			{
				return preview;
			}

			return $"{preview[..117]}...";
		}
	}

	public int ReadingTimeMinutes => Math.Max(1, (int)Math.Ceiling(GetWordCount() / 170d));

	public string ReadingTimeLabel => $"{ReadingTimeMinutes} min citania";

	public string AccentHex => PaletteCatalog.Get(PaletteKey).AccentHex;

	public string SectionAccentHex => SectionStyleCatalog.GetAccentHex(Section, ThemePaletteState.CurrentPaletteKey);

	public string SectionSurfaceHex => SectionStyleCatalog.GetSurfaceHex(Section, ThemePaletteState.CurrentPaletteKey);

	public string SectionBorderHex => SectionStyleCatalog.GetBorderHex(Section, ThemePaletteState.CurrentPaletteKey);

	public string SectionTagHex => SectionStyleCatalog.GetTagHex(Section, ThemePaletteState.CurrentPaletteKey);

	public string DisplayDate => UpdatedAtUtc.ToLocalTime().ToString("dd.MM.yyyy");

	private int GetWordCount()
	{
		return Content
			.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries)
			.Length;
	}
}
