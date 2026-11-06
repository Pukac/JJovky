namespace JJPoems;

public partial class App : Application
{
	private readonly AppShell _appShell;
	private readonly Services.IUserAccountService _accountService;
	private readonly Services.IOnboardingService _onboardingService;
	private readonly Services.IThemeService _themeService;

	public App(
		AppShell appShell,
		Services.IOnboardingService onboardingService,
		Services.IUserAccountService accountService,
		Services.IThemeService themeService)
	{
		InitializeComponent();
		_appShell = appShell;
		_onboardingService = onboardingService;
		_accountService = accountService;
		_themeService = themeService;
		_themeService.Initialize();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var page = _onboardingService.HasCompletedOnboarding() && _accountService.HasActiveAccount()
			? (Page)_appShell
			: new Views.OnboardingPage();

		var window = new Window(page);
		window.Created += async (_, _) =>
		{
			try
			{
				await _themeService.ApplyCurrentUserThemeAsync();
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
			{
				await page.DisplayAlertAsync("Profil sa nepodarilo nacitat", "Skontroluj volne miesto. Ak mas exportovanu zalohu, mozes ju obnovit v profile.", "OK");
			}
		};
		return window;
	}
}
