using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class StudioPage : ContentPage
{
	private readonly StudioViewModel _viewModel;

	public StudioPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<StudioViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.RefreshAsync();
	}
}
