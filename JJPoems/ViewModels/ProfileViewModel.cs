using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly IUserAccountService _accountService;
    private readonly IFavoritesService _favoritesService;
    private readonly INavigationService _navigationService;
    private readonly IPoetryRepository _poetryRepository;
    private readonly IThemeService _themeService;
    private readonly IUserWorkspaceService _workspaceService;
    private string _displayName = string.Empty;
    private string _email = string.Empty;
    private string _tagline = string.Empty;
    private string _bio = string.Empty;
    private PaletteOption _selectedPaletteOption = PaletteCatalog.Get("sand");
    private bool _hasFavorites;

    public ProfileViewModel(
        IUserWorkspaceService workspaceService,
        IPoetryRepository poetryRepository,
        IFavoritesService favoritesService,
        INavigationService navigationService,
        IUserAccountService accountService,
        IThemeService themeService)
    {
        _workspaceService = workspaceService;
        _poetryRepository = poetryRepository;
        _favoritesService = favoritesService;
        _navigationService = navigationService;
        _accountService = accountService;
        _themeService = themeService;
        Title = "Profil";
        FavoritePoems = [];
        PaletteOptions = new ObservableCollection<PaletteOption>(PaletteCatalog.CreateOptions(_selectedPaletteOption.Key));
        SaveProfileCommand = new Command(async () => await SaveProfileAsync());
        OpenFavoriteCommand = new Command<Poem>(async poem =>
        {
            if (poem is not null)
            {
                await _navigationService.GoToPoemDetailAsync(poem.Id);
            }
        });
        GoToAboutCommand = new Command(async () => await _navigationService.GoToAboutAsync());
        SwitchProfileCommand = new Command(async () => await SwitchProfileAsync());
        SelectPaletteCommand = new Command<PaletteOption>(SelectPalette);
        ExportCommand = new Command(async () => await ExportAsync());
        ImportCommand = new Command(async () => await ImportAsync());
    }

    public ICommand ExportCommand { get; }
    public ICommand ImportCommand { get; }

    private async Task ExportAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var json = await _workspaceService.ExportAsync();
            var path = Path.Combine(FileSystem.CacheDirectory, $"JJPoems-zaloha-{DateTime.Now:yyyyMMdd-HHmmss}.json");
            await File.WriteAllTextAsync(path, json);
            await Share.Default.RequestAsync(new ShareFileRequest { Title = "Ulozit zalohu JJ Poems", File = new ShareFile(path, "application/json") });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        { await Shell.Current.DisplayAlertAsync("Zaloha sa nepodarila", "Skontroluj volne miesto a skus to znova.", "OK"); }
        finally { IsBusy = false; }
    }

    private async Task ImportAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Vyber JSON zalohu JJ Poems" });
            if (file is null) return;
            await using var stream = await file.OpenReadAsync();
            // Limit memory use before parsing a user-selected document.
            using var buffer = new MemoryStream();
            var bytes = new byte[8192];
            int read;
            while ((read = await stream.ReadAsync(bytes)) > 0)
            {
                if (buffer.Length + read > 20 * 1024 * 1024) throw new System.Text.Json.JsonException("Backup too large.");
                await buffer.WriteAsync(bytes.AsMemory(0, read));
            }
            if (!await Shell.Current.DisplayAlertAsync("Obnovit texty?", "Chybajuce texty sa pridaju do aktualneho profilu. Existujuce texty ani tvoja tema sa neprepisu.", "Obnovit", "Zrusit")) return;
            var count = await _workspaceService.ImportAsync(System.Text.Encoding.UTF8.GetString(buffer.ToArray()));
            await Shell.Current.DisplayAlertAsync("Obnova dokoncena", $"Pridane texty: {count}. Najdes ich vo svojom studiu.", "OK");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        { await Shell.Current.DisplayAlertAsync("Zalohu sa nepodarilo obnovit", "Vyber platnu JSON zalohu JJ Poems (najviac 20 MB).", "OK"); }
        finally { IsBusy = false; }
    }

    public event EventHandler? SignedOut;

    public ObservableCollection<Poem> FavoritePoems { get; }

    public ObservableCollection<PaletteOption> PaletteOptions { get; }

    public ICommand SaveProfileCommand { get; }

    public ICommand OpenFavoriteCommand { get; }

    public ICommand GoToAboutCommand { get; }

    public ICommand SwitchProfileCommand { get; }

    public ICommand SelectPaletteCommand { get; }

    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Tagline
    {
        get => _tagline;
        set => SetProperty(ref _tagline, value);
    }

    public string Bio
    {
        get => _bio;
        set => SetProperty(ref _bio, value);
    }

    public PaletteOption SelectedPaletteOption
    {
        get => _selectedPaletteOption;
        set
        {
            if (SetProperty(ref _selectedPaletteOption, value))
            {
                PaletteCatalog.MarkSelected(PaletteOptions, value.Key);
                OnPropertyChanged(nameof(SelectedPaletteName));
                OnPropertyChanged(nameof(ProfileAccentHex));
                _themeService.ApplyPalette(value.Key);
            }
        }
    }

    public string SelectedPaletteName => SelectedPaletteOption.Name;

    public string ProfileAccentHex => SelectedPaletteOption.AccentHex;

    public bool HasFavorites
    {
        get => _hasFavorites;
        private set => SetProperty(ref _hasFavorites, value);
    }

    public async Task RefreshAsync()
    {
        IsBusy = true;
        try
        {
            await _workspaceService.EnsureWorkspaceAsync();
            var account = await _accountService.GetCurrentAccountAsync();
            var profile = await _workspaceService.GetProfileAsync();
            DisplayName = profile.DisplayName;
            Email = account?.Email ?? string.Empty;
            Tagline = profile.Tagline;
            Bio = profile.Bio;
            SelectedPaletteOption = PaletteOptions.First(option => option.Key.Equals(profile.PreferredPaletteKey, StringComparison.OrdinalIgnoreCase));

            var favoriteIds = await _favoritesService.GetFavoriteIdsAsync();
            var poems = await _poetryRepository.GetPoemsAsync();
            var favorites = poems.Where(poem => favoriteIds.Contains(poem.Id)).ToList();

            FavoritePoems.Clear();
            foreach (var poem in favorites)
            {
                FavoritePoems.Add(poem);
            }

            HasFavorites = FavoritePoems.Count > 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveProfileAsync()
    {
        var currentAccount = await _accountService.GetCurrentAccountAsync();
        var profile = new UserProfile
        {
            DisplayName = string.IsNullOrWhiteSpace(DisplayName) ? "Moje tiche miesto" : DisplayName.Trim(),
            Tagline = string.IsNullOrWhiteSpace(Tagline) ? "Priestor pre vlastne verse a jemne myslienkove stopy." : Tagline.Trim(),
            Bio = string.IsNullOrWhiteSpace(Bio) ? "Sem si mozes ukladat vlastne basne, poznamky, sny, napady aj vnutorne pochody." : Bio.Trim(),
            PreferredPaletteKey = SelectedPaletteOption.Key
        };

        await _workspaceService.SaveProfileAsync(profile);
        if (currentAccount is not null)
        {
            currentAccount.DisplayName = profile.DisplayName;
            currentAccount.Email = string.IsNullOrWhiteSpace(Email) ? currentAccount.Email : Email.Trim();
            currentAccount.Tagline = profile.Tagline;
            currentAccount.PreferredPaletteKey = profile.PreferredPaletteKey;
            await _accountService.UpdateCurrentAccountAsync(currentAccount);
        }

        _themeService.ApplyPalette(profile.PreferredPaletteKey);
    }

    private void SelectPalette(PaletteOption? option)
    {
        if (option is null)
        {
            return;
        }

        SelectedPaletteOption = option;
    }

    private async Task SwitchProfileAsync()
    {
        await _accountService.SignOutAsync();
        _themeService.ApplyPalette("sand");
        SignedOut?.Invoke(this, EventArgs.Empty);
    }
}
