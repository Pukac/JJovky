using JJPoems.Models;
using Microsoft.Maui.Controls.Shapes;

namespace JJPoems.Services;

public class ThemeService : IThemeService
{
	private readonly IUserAccountService _accountService;
	private readonly IUserWorkspaceService _workspaceService;
	private string _currentPaletteKey = "sand";
	private bool _isInitialized;

	public ThemeService(IUserAccountService accountService, IUserWorkspaceService workspaceService)
	{
		_accountService = accountService;
		_workspaceService = workspaceService;
	}

	public void Initialize()
	{
		if (_isInitialized)
		{
			return;
		}

		_isInitialized = true;
		if (Application.Current is not null)
		{
			Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
		}
	}

	public async Task ApplyCurrentUserThemeAsync()
	{
		var account = await _accountService.GetCurrentAccountAsync();
		if (account is null)
		{
			ApplyPalette("sand");
			return;
		}

		var profile = await _workspaceService.GetProfileAsync();
		ApplyPalette(profile.PreferredPaletteKey);
	}

	public void ApplyPalette(string? paletteKey)
	{
		_currentPaletteKey = string.IsNullOrWhiteSpace(paletteKey) ? "sand" : paletteKey;
		ThemePaletteState.CurrentPaletteKey = _currentPaletteKey;

		var palette = PaletteCatalog.Get(_currentPaletteKey);
		var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
		var resources = Application.Current?.Resources;
		if (resources is null)
		{
			return;
		}

		resources["ThemePrimaryBackground"] = Color.FromArgb(isDark ? palette.DarkPrimaryBackgroundHex : palette.PrimaryBackgroundHex);
		resources["ThemeSecondaryBackground"] = Color.FromArgb(isDark ? palette.DarkSecondaryBackgroundHex : palette.SecondaryBackgroundHex);
		resources["ThemePrimaryText"] = Color.FromArgb(isDark ? palette.DarkPrimaryTextHex : palette.PrimaryTextHex);
		resources["ThemeSecondaryText"] = Color.FromArgb(isDark ? palette.DarkSecondaryTextHex : palette.SecondaryTextHex);
		resources["ThemeAccent"] = Color.FromArgb(isDark ? palette.DarkAccentHex : palette.AccentHex);
		resources["ThemeAccentSoft"] = Color.FromArgb(isDark ? palette.DarkAccentHex : palette.AccentSoftHex);
		resources["ThemeBorderDivider"] = Color.FromArgb(isDark ? palette.DarkBorderDividerHex : palette.BorderDividerHex);
		resources["ThemeCard"] = Color.FromArgb(isDark ? palette.DarkCardHex : palette.CardHex);
		resources["ThemePanel"] = Color.FromArgb(isDark ? palette.DarkPanelHex : palette.PanelHex);
		resources["ThemeHeroBody"] = Color.FromArgb(isDark ? palette.DarkHeroBodyHex : palette.HeroBodyHex);
		resources["ThemeButtonText"] = Color.FromArgb(isDark ? palette.DarkPrimaryBackgroundHex : palette.CardHex);
		resources["ThemeWarmGlowBrush"] = CreateRadialBrush(
			isDark ? palette.DarkWarmGlowInnerHex : palette.WarmGlowInnerHex,
			isDark ? palette.DarkWarmGlowOuterHex : palette.WarmGlowOuterHex);
		resources["ThemeAccentGlowBrush"] = CreateRadialBrush(
			isDark ? palette.DarkAccentGlowHex : palette.AccentGlowHex,
			"#00000000",
			0.8);
		resources["ThemeHeroSurfaceBrush"] = CreateLinearBrush(
			isDark ? palette.DarkHeroSurfaceStartHex : palette.HeroSurfaceStartHex,
			isDark ? palette.DarkHeroSurfaceEndHex : palette.HeroSurfaceEndHex);
	}

	private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
	{
		ApplyPalette(_currentPaletteKey);
	}

	private static RadialGradientBrush CreateRadialBrush(string innerHex, string outerHex, double radius = 0.75)
	{
		return new RadialGradientBrush
		{
			Center = new Point(0.5, 0.5),
			Radius = radius,
			GradientStops =
			[
				new GradientStop(Color.FromArgb(innerHex), 0.0f),
				new GradientStop(Color.FromArgb(outerHex), 1.0f)
			]
		};
	}

	private static LinearGradientBrush CreateLinearBrush(string startHex, string endHex)
	{
		return new LinearGradientBrush
		{
			StartPoint = new Point(0, 0),
			EndPoint = new Point(1, 1),
			GradientStops =
			[
				new GradientStop(Color.FromArgb(startHex), 0.0f),
				new GradientStop(Color.FromArgb(endHex), 1.0f)
			]
		};
	}
}
