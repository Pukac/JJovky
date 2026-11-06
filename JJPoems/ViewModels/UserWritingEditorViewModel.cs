using System.Collections.ObjectModel;
using System.Windows.Input;
using JJPoems.Models;
using JJPoems.Services;

namespace JJPoems.ViewModels;

public class UserWritingEditorViewModel : BaseViewModel
{
    private readonly IQuickCaptureDraftService _quickCaptureDraftService;
    private readonly IUserWorkspaceService _workspaceService;
    private CancellationTokenSource? _autosaveCts;
    private readonly SemaphoreSlim _draftGate = new(1, 1);
    private bool _loading;
    private bool _finished;
    private string _draftStatus = "Koncept sa uklada automaticky.";
    public string DraftStatus { get => _draftStatus; private set => SetProperty(ref _draftStatus, value); }
    private bool _isQuickCapture;
    private string? _writingId;
    private string _titleText = string.Empty;
    private string _section = "Basnicky";
    private string _mood = string.Empty;
    private string _content = string.Empty;
    private PaletteOption _selectedPaletteOption = PaletteCatalog.Get("sand");
    private bool _isExisting;

    public UserWritingEditorViewModel(IUserWorkspaceService workspaceService, IQuickCaptureDraftService quickCaptureDraftService)
    {
        _workspaceService = workspaceService;
        _quickCaptureDraftService = quickCaptureDraftService;
        Title = "Novy blok";
        PaletteOptions = new ObservableCollection<PaletteOption>(PaletteCatalog.CreateOptions(_selectedPaletteOption.Key));
        SectionOptions = ["Basnicky", "Vylev hlavy", "Sny", "Dennik", "Napady"];
        SaveCommand = new Command(async () => await SaveAsync(), CanSave);
        DeleteCommand = new Command(async () => await DeleteAsync(), () => IsExisting);
    }

    public ObservableCollection<PaletteOption> PaletteOptions { get; }

    public IReadOnlyList<string> SectionOptions { get; }

    public ICommand SaveCommand { get; }

    public ICommand DeleteCommand { get; }

    public string TitleText
    {
        get => _titleText;
        set
        {
            if (SetProperty(ref _titleText, value))
            {
                RefreshCommandStates();
                ScheduleAutosaveIfNeeded();
            }
        }
    }

    public string Section
    {
        get => _section;
        set
        {
            if (SetProperty(ref _section, value))
            {
                OnPropertyChanged(nameof(SectionPreviewAccentHex));
                OnPropertyChanged(nameof(SectionPreviewSurfaceHex));
                OnPropertyChanged(nameof(SectionPreviewBorderHex));
                ScheduleAutosaveIfNeeded();
            }
        }
    }

    public string Mood
    {
        get => _mood;
        set
        {
            if (SetProperty(ref _mood, value))
            {
                ScheduleAutosaveIfNeeded();
            }
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (SetProperty(ref _content, value))
            {
                RefreshCommandStates();
                ScheduleAutosaveIfNeeded();
            }
        }
    }

    public PaletteOption SelectedPaletteOption
    {
        get => _selectedPaletteOption;
        set
        {
            if (SetProperty(ref _selectedPaletteOption, value))
            {
                OnPropertyChanged(nameof(SectionPreviewAccentHex));
                OnPropertyChanged(nameof(SectionPreviewSurfaceHex));
                OnPropertyChanged(nameof(SectionPreviewBorderHex));
                ScheduleAutosaveIfNeeded();
            }
        }
    }

    public bool IsQuickCapture
    {
        get => _isQuickCapture;
        private set
        {
            if (SetProperty(ref _isQuickCapture, value))
            {
                OnPropertyChanged(nameof(HeaderTitle));
                OnPropertyChanged(nameof(HeaderBody));
                OnPropertyChanged(nameof(PrimaryButtonText));
                OnPropertyChanged(nameof(TitlePlaceholder));
                OnPropertyChanged(nameof(ContentPlaceholder));
                RefreshCommandStates();
            }
        }
    }

    public string HeaderTitle => IsQuickCapture ? "Rychly zachyt myslienky" : "Novy priestor pre myslienky";

    public string HeaderBody => IsQuickCapture
        ? "Len otvor, napis a zavri. Appka draft priebezne podrzi, aby sa nestratila ani krehka myslienka."
        : "Pis surovo alebo jemne. Dolezite je, aby to bolo tvoje, zive a pravdive.";

    public string TitlePlaceholder => IsQuickCapture ? "Nazov je volitelny" : "Nazov textu";

    public string ContentPlaceholder => IsQuickCapture
        ? "Co ti teraz beha hlavou?"
        : "Zacni pisat basen, sen, poznamku alebo myslienkovy pochod...";

    public string PrimaryButtonText => IsQuickCapture ? "Ulozit zachyt" : "Ulozit";

    public string SectionPreviewAccentHex => SectionStyleCatalog.GetAccentHex(Section, SelectedPaletteOption.Key);

    public string SectionPreviewSurfaceHex => SectionStyleCatalog.GetSurfaceHex(Section, SelectedPaletteOption.Key);

    public string SectionPreviewBorderHex => SectionStyleCatalog.GetBorderHex(Section, SelectedPaletteOption.Key);

    public bool IsExisting
    {
        get => _isExisting;
        private set
        {
            if (SetProperty(ref _isExisting, value))
            {
                ((Command)DeleteCommand).ChangeCanExecute();
            }
        }
    }

    public async Task FlushDraftAsync()
    {
        _autosaveCts?.Cancel();
        if (!_loading && !_finished) await TrySaveDraftAsync();
    }

    public async Task LoadAsync(string? writingId, string? mode = null)
    {
        _autosaveCts?.Cancel();
        _loading = true;
        _finished = false;
        try
        {
            _writingId = writingId;
            IsQuickCapture = string.Equals(mode, "quick", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(writingId))
            {
                Title = IsQuickCapture ? "Rychly zachyt" : "Novy blok";
                IsExisting = false;
                TitleText = string.Empty;
                Section = IsQuickCapture ? "Vylev hlavy" : "Basnicky";
                Mood = string.Empty;
                Content = string.Empty;
                SelectedPaletteOption = PaletteCatalog.Get("sand");

                await LoadDraftAsync();

                return;
            }

            var writing = await _workspaceService.GetWritingByIdAsync(writingId);
            if (writing is null)
            {
                return;
            }

            Title = "Upravit blok";
            IsExisting = true;
            TitleText = writing.Title;
            Section = string.IsNullOrWhiteSpace(writing.Section) ? "Basnicky" : writing.Section;
            Mood = writing.Mood;
            Content = writing.Content;
            SelectedPaletteOption = PaletteCatalog.Get(writing.PaletteKey);
            await LoadDraftAsync();
        }
        finally { _loading = false; }
    }

    private bool CanSave()
    {
        return IsQuickCapture
            ? !string.IsNullOrWhiteSpace(Content)
            : !string.IsNullOrWhiteSpace(TitleText) && !string.IsNullOrWhiteSpace(Content);
    }

    private async Task DeleteAsync()
    {
        if (string.IsNullOrWhiteSpace(_writingId))
        {
            return;
        }

        if (!await Shell.Current.DisplayAlertAsync("Vymazat text?", "Tento text bude odstraneny. Pred vymazanim si mozes vytvorit zalohu v profile.", "Vymazat", "Zrusit")) return;
        try
        {
            await _workspaceService.DeleteWritingAsync(_writingId);
            _finished = true;
            _autosaveCts?.Cancel();
            await _draftGate.WaitAsync();
            try { await _quickCaptureDraftService.ClearDraftAsync(_writingId); }
            finally { _draftGate.Release(); }
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { await Shell.Current.DisplayAlertAsync("Nepodarilo sa vymazat", "Skus to znova. Text zostava v aplikacii.", "OK"); }
    }

    private async Task LoadDraftAsync()
    {
        var draft = await _quickCaptureDraftService.GetDraftAsync(_writingId);
        if (draft is null)
        {
            return;
        }

        TitleText = draft.Title;
        Section = string.IsNullOrWhiteSpace(draft.Section) ? "Vylev hlavy" : draft.Section;
        Mood = draft.Mood;
        Content = draft.Content;
        SelectedPaletteOption = PaletteCatalog.Get(draft.PaletteKey);
        DraftStatus = "Obnoveny rozpisany koncept.";
    }

    private void RefreshCommandStates()
    {
        ((Command)SaveCommand).ChangeCanExecute();
    }

    private async Task SaveAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var title = string.IsNullOrWhiteSpace(TitleText)
                ? BuildTitleFromContent()
                : TitleText.Trim();

            var writing = new UserWriting
            {
                Id = string.IsNullOrWhiteSpace(_writingId) ? Guid.NewGuid().ToString("N") : _writingId,
                Title = title,
                Section = Section,
                Mood = Mood,
                Content = Content,
                PaletteKey = SelectedPaletteOption.Key
            };

            await _workspaceService.SaveWritingAsync(writing);
            _finished = true;
            _autosaveCts?.Cancel();
            await _draftGate.WaitAsync();
            try { await _quickCaptureDraftService.ClearDraftAsync(_writingId); }
            finally { _draftGate.Release(); }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { await Shell.Current.DisplayAlertAsync("Text sa nepodarilo ulozit", "Tvoj text ostava v editore. Skontroluj volne miesto a skus ulozit znova.", "OK"); }
        finally { IsBusy = false; }
    }

    private async Task TrySaveDraftAsync()
    {
        await _draftGate.WaitAsync();
        try
        {
            if (_finished || _loading) return;
            await SaveDraftAsync();
            DraftStatus = "Koncept ulozeny v telefone.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { DraftStatus = "Koncept sa neulozil. Skontroluj volne miesto a skus Ulozit."; }
        finally { _draftGate.Release(); }
    }

    private async Task SaveDraftAsync()
    {
        var hasAnything = !string.IsNullOrWhiteSpace(Content)
            || !string.IsNullOrWhiteSpace(TitleText)
            || !string.IsNullOrWhiteSpace(Mood);

        if (!hasAnything)
        {
            await _quickCaptureDraftService.ClearDraftAsync(_writingId);
            return;
        }

        await _quickCaptureDraftService.SaveDraftAsync(new QuickCaptureDraft
        {
            WritingId = _writingId,
            Title = TitleText,
            Section = Section,
            Mood = Mood,
            Content = Content,
            PaletteKey = SelectedPaletteOption.Key
        });
    }

    private void ScheduleAutosaveIfNeeded()
    {
        if (_loading || _finished)
        {
            return;
        }

        _autosaveCts?.Cancel();
        _autosaveCts = new CancellationTokenSource();
        var token = _autosaveCts.Token;

        _ = AutosaveAsync(token);
    }

    private async Task AutosaveAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(900, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(TrySaveDraftAsync);
        }
        catch (TaskCanceledException)
        {
        }
    }

    private string BuildTitleFromContent()
    {
        var line = Content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item));

        if (string.IsNullOrWhiteSpace(line))
        {
            return "Neulozena myslienka";
        }

        return line.Length <= 38 ? line : $"{line[..35]}...";
    }
}
