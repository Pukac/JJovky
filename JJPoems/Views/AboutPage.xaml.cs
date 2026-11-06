using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class AboutPage : ContentPage
{
	private readonly AboutViewModel _viewModel;

	public AboutPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<AboutViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}
}
