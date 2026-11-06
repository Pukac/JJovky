namespace JJPoems.Services;

public interface INavigationService
{
	Task GoToPoemsAsync();
	Task GoToAboutAsync();
	Task GoToPoemDetailAsync(string poemId);
	Task GoToStudioAsync();
	Task GoToProfileAsync();
	Task GoToWritingEditorAsync(string? writingId = null, bool quickCapture = false);
	Task GoToWritingDetailAsync(string writingId);
	Task GoToTimelineAsync();
}
