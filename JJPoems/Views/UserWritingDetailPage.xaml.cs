using JJPoems.ViewModels;

namespace JJPoems.Views;

[QueryProperty(nameof(WritingId), "writingId")]
public partial class UserWritingDetailPage : ContentPage
{
	private readonly UserWritingDetailViewModel _viewModel;
	private string? _writingId;

	public UserWritingDetailPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<UserWritingDetailViewModel>();
	}

	public string? WritingId
	{
		get => _writingId;
		set
		{
			_writingId = string.IsNullOrWhiteSpace(value) ? null : Uri.UnescapeDataString(value);
			_ = LoadAsync();
		}
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadAsync();
	}

	private async Task LoadAsync()
	{
		if (!string.IsNullOrWhiteSpace(_writingId))
		{
			await _viewModel.LoadWritingAsync(_writingId);
		}
	}
}
