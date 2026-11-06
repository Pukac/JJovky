using JJPoems.ViewModels;

namespace JJPoems.Views;

public partial class ProfilePage : ContentPage
{
	private readonly ProfileViewModel _viewModel;

	public ProfilePage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<ProfileViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.SignedOut -= OnSignedOut;
		_viewModel.SignedOut += OnSignedOut;
		await _viewModel.RefreshAsync();
	}

	private void OnSignedOut(object? sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new OnboardingPage();
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		_viewModel.SignedOut -= OnSignedOut;
	}
}
