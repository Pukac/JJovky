using JJPoems.Views;

namespace JJPoems;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(PoemDetailPage), typeof(PoemDetailPage));
		Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
		Routing.RegisterRoute(nameof(UserWritingEditorPage), typeof(UserWritingEditorPage));
		Routing.RegisterRoute(nameof(UserWritingDetailPage), typeof(UserWritingDetailPage));
		Routing.RegisterRoute(nameof(TimelinePage), typeof(TimelinePage));
	}
}
