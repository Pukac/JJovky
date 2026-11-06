using JJPoems.Services;
using JJPoems.ViewModels;

namespace JJPoems;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IPoetryRepository, JsonPoetryRepository>();
		builder.Services.AddSingleton<IUserAccountService, UserAccountService>();
		builder.Services.AddSingleton<IFavoritesService, FavoritesService>();
		builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
		builder.Services.AddSingleton<IOnboardingService, OnboardingService>();
		builder.Services.AddSingleton<IUserWorkspaceService, UserWorkspaceService>();
		builder.Services.AddSingleton<IQuickCaptureDraftService, QuickCaptureDraftService>();
		builder.Services.AddSingleton<IThemeService, ThemeService>();
		builder.Services.AddSingleton<AppShell>();

		builder.Services.AddTransient<OnboardingViewModel>();
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<PoemsViewModel>();
		builder.Services.AddTransient<PoemDetailViewModel>();
		builder.Services.AddTransient<FavoritesViewModel>();
		builder.Services.AddTransient<AboutViewModel>();
		builder.Services.AddTransient<StudioViewModel>();
		builder.Services.AddTransient<TimelineViewModel>();
		builder.Services.AddTransient<UserWritingEditorViewModel>();
		builder.Services.AddTransient<UserWritingDetailViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();

		return builder.Build();
	}
}
