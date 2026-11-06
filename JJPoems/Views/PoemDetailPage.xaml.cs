using JJPoems.ViewModels;

namespace JJPoems.Views;

[QueryProperty(nameof(PoemId), "poemId")]
public partial class PoemDetailPage : ContentPage
{
	private readonly PoemDetailViewModel _viewModel;
	private string _poemId = string.Empty;

	public PoemDetailPage()
	{
		InitializeComponent();
		BindingContext = _viewModel = ServiceHelper.GetRequiredService<PoemDetailViewModel>();
	}

	public string PoemId
	{
		get => _poemId;
		set
		{
			_poemId = Uri.UnescapeDataString(value ?? string.Empty);
			_ = LoadAsync();
		}
	}

	private async Task LoadAsync()
	{
		if (!string.IsNullOrWhiteSpace(_poemId))
		{
			await _viewModel.LoadPoemAsync(_poemId);
		}
	}
}
