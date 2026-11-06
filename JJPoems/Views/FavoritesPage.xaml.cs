using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class FavoritesPage : ContentPage
{
	private readonly FavoritesViewModel _viewModel;

	public FavoritesPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<FavoritesViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.RefreshAsync();
	}
}
