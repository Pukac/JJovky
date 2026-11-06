using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class OnboardingViewModel : BaseViewModel
{
	private readonly IUserAccountService _accountService;
	private readonly IOnboardingService _onboardingService;
	private readonly IThemeService _themeService;
	private readonly IUserWorkspaceService _workspaceService;
	private string _displayName = string.Empty;
	private string _email = string.Empty;
	private string _tagline = "Moj jemny priestor pre verse, sny a vnutorne zapisovanie.";
	private PaletteOption _selectedPaletteOption = PaletteCatalog.Get("sand");
	private bool _hasExistingAccounts;

	public OnboardingViewModel(
		IOnboardingService onboardingService,
		IUserAccountService accountService,
		IUserWorkspaceService workspaceService,
		IThemeService themeService)
	{
		_onboardingService = onboardingService;
		_accountService = accountService;
		_workspaceService = workspaceService;
		_themeService = themeService;
		Title = "Vitaj v JJ Poems";
		ExistingAccounts = [];
		PaletteOptions = new ObservableCollection<PaletteOption>(PaletteCatalog.CreateOptions(_selectedPaletteOption.Key));
		ContinueCommand = new Command(async () => await ContinueAsync(), CanContinue);
		UseExistingAccountCommand = new Command<UserAccount>(async account => await UseExistingAccountAsync(account));
		SelectPaletteCommand = new Command<PaletteOption>(SelectPalette);
	}

	public event EventHandler? Completed;

	public ObservableCollection<UserAccount> ExistingAccounts { get; }

	public ObservableCollection<PaletteOption> PaletteOptions { get; }

	public ICommand ContinueCommand { get; }

	public ICommand UseExistingAccountCommand { get; }

	public ICommand SelectPaletteCommand { get; }

	public string DisplayName
	{
		get => _displayName;
		set
		{
			if (SetProperty(ref _displayName, value))
			{
				((Command)ContinueCommand).ChangeCanExecute();
			}
		}
	}

	public string Email
	{
		get => _email;
		set
		{
			if (SetProperty(ref _email, value))
			{
				((Command)ContinueCommand).ChangeCanExecute();
			}
		}
	}

	public string Tagline
	{
		get => _tagline;
		set => SetProperty(ref _tagline, value);
	}

	public PaletteOption SelectedPaletteOption
	{
		get => _selectedPaletteOption;
		set
		{
			if (SetProperty(ref _selectedPaletteOption, value))
			{
				PaletteCatalog.MarkSelected(PaletteOptions, value.Key);
				_themeService.ApplyPalette(value.Key);
			}
		}
	}

	public bool HasExistingAccounts
	{
		get => _hasExistingAccounts;
		private set => SetProperty(ref _hasExistingAccounts, value);
	}

	public async Task RefreshAsync()
	{
		var accounts = await _accountService.GetAccountsAsync();
		ExistingAccounts.Clear();
		foreach (var account in accounts)
		{
			ExistingAccounts.Add(account);
		}

		HasExistingAccounts = ExistingAccounts.Count > 0;
		if (!HasExistingAccounts)
		{
			_themeService.ApplyPalette(SelectedPaletteOption.Key);
			return;
		}

		var recent = ExistingAccounts[0];
		if (string.IsNullOrWhiteSpace(DisplayName))
		{
			DisplayName = recent.DisplayName;
		}

		if (string.IsNullOrWhiteSpace(Email))
		{
			Email = recent.Email;
		}

		SelectedPaletteOption = PaletteOptions.First(option => option.Key.Equals(recent.PreferredPaletteKey, StringComparison.OrdinalIgnoreCase));
	}

	private bool CanContinue()
	{
		return !string.IsNullOrWhiteSpace(DisplayName)
			&& !string.IsNullOrWhiteSpace(Email)
			&& Email.Contains('@')
			&& Email.Contains('.');
	}

	private async Task ContinueAsync()
	{
		IsBusy = true;
		try
		{
			await _accountService.CreateOrActivateAccountAsync(DisplayName, Email, Tagline, SelectedPaletteOption.Key);
			await _workspaceService.EnsureWorkspaceAsync();
			_themeService.ApplyPalette(SelectedPaletteOption.Key);
			_onboardingService.CompleteOnboarding();
			Completed?.Invoke(this, EventArgs.Empty);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void SelectPalette(PaletteOption? option)
	{
		if (option is null)
		{
			return;
		}

		SelectedPaletteOption = option;
	}

	private async Task UseExistingAccountAsync(UserAccount? account)
	{
		if (account is null)
		{
			return;
		}

		IsBusy = true;
		try
		{
			await _accountService.SwitchAccountAsync(account.Id);
			await _workspaceService.EnsureWorkspaceAsync();
			_themeService.ApplyPalette(account.PreferredPaletteKey);
			_onboardingService.CompleteOnboarding();
			Completed?.Invoke(this, EventArgs.Empty);
		}
		finally
		{
			IsBusy = false;
		}
	}
}
