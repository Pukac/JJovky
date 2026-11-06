using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class TimelinePage : ContentPage
{
	private readonly TimelineViewModel _viewModel;

	public TimelinePage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<TimelineViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.RefreshAsync();
	}
}
