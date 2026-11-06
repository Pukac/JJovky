using JJPoems.Views;

namespace JJPoems.Services;

public class ShellNavigationService : INavigationService
{
	public Task GoToAboutAsync()
	{
		return Shell.Current.GoToAsync(nameof(AboutPage));
	}

	public Task GoToPoemsAsync()
	{
		return Shell.Current.GoToAsync("//poems");
	}

	public Task GoToPoemDetailAsync(string poemId)
	{
		return Shell.Current.GoToAsync($"{nameof(PoemDetailPage)}?poemId={Uri.EscapeDataString(poemId)}");
	}

	public Task GoToProfileAsync()
	{
		return Shell.Current.GoToAsync("//profile");
	}

	public Task GoToStudioAsync()
	{
		return Shell.Current.GoToAsync("//studio");
	}

	public Task GoToTimelineAsync()
	{
		return Shell.Current.GoToAsync(nameof(TimelinePage));
	}

	public Task GoToWritingEditorAsync(string? writingId = null, bool quickCapture = false)
	{
		var route = string.IsNullOrWhiteSpace(writingId)
			? nameof(UserWritingEditorPage)
			: $"{nameof(UserWritingEditorPage)}?writingId={Uri.EscapeDataString(writingId)}";

		if (quickCapture)
		{
			route = route.Contains('?')
				? $"{route}&mode=quick"
				: $"{route}?mode=quick";
		}

		return Shell.Current.GoToAsync(route);
	}

	public Task GoToWritingDetailAsync(string writingId)
	{
		return Shell.Current.GoToAsync($"{nameof(UserWritingDetailPage)}?writingId={Uri.EscapeDataString(writingId)}");
	}
}
