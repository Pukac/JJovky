using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class PoemsPage : ContentPage
{
	private readonly PoemsViewModel _viewModel;

	public PoemsPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<PoemsViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.InitializeAsync();
	}
}
