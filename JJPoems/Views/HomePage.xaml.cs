using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class HomePage : ContentPage
{
	private readonly HomeViewModel _viewModel;

	public HomePage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<HomeViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}
}
