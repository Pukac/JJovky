using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class OnboardingPage : ContentPage
{
	private readonly OnboardingViewModel _viewModel;

	public OnboardingPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<OnboardingViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.Completed -= OnCompleted;
		_viewModel.Completed += OnCompleted;
		await _viewModel.RefreshAsync();
	}

	private void OnCompleted(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = ServiceHelper.GetRequiredService<AppShell>();
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.Completed -= OnCompleted;
	}
}
