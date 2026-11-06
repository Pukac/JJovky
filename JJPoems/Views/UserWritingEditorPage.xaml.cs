using JJPoems.ViewModels;

namespace JJPoems.Views;

[QueryProperty(nameof(WritingId), "writingId")]
[QueryProperty(nameof(Mode), "mode")]
public partial class UserWritingEditorPage : ContentPage
{
	private readonly UserWritingEditorViewModel _viewModel;
	private string? _mode;
	private string? _writingId;

	public UserWritingEditorPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<UserWritingEditorViewModel>();
	}

	public string? WritingId
	{
		get => _writingId;
		set
		{
			_writingId = string.IsNullOrWhiteSpace(value) ? null : Uri.UnescapeDataString(value);
		}
	}

	public string? Mode
	{
		get => _mode;
		set
		{
			_mode = string.IsNullOrWhiteSpace(value) ? null : Uri.UnescapeDataString(value);
		}
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadAsync(_writingId, _mode);
	}

	protected override async void OnDisappearing()
	{
		base.OnDisappearing();
		await _viewModel.FlushDraftAsync();
	}
}
