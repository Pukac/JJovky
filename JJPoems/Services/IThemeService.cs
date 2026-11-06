namespace JJPoems.Services;

public interface IThemeService
{
	void Initialize();
	Task ApplyCurrentUserThemeAsync();
	void ApplyPalette(string? paletteKey);
}
