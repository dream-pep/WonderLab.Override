using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using WonderLab.Services.UI;
using WonderLab.Classes.Datas;
using WonderLab.Services.Game;
using WonderLab.Classes.Datas.ViewData;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class SearchPageViewModel : ViewModelBase {
    private readonly LanguageService _languageService;
    private readonly GameNewsService _gameNewsService;
    private readonly ILogger<SearchPageViewModel> _logger;

    [ObservableProperty] public string _searchText;
    [ObservableProperty] public bool _isDropDownOpen;
    [ObservableProperty] public ImmutableArray<ModData> _possibleSearches;
    [ObservableProperty] public ImmutableArray<JavaPatchNoteViewData> _javaPatchNotes;
    [ObservableProperty] public Tuple<JavaPatchNoteViewData, JavaPatchNoteViewData> _currentJavaPatchNote;

    public AutoCompleteFilterPredicate<object> ModFilter {
        get {
            return (searchText, obj) =>
                (obj as ModData).ChineseName.Contains(searchText)
                || (obj as ModData).ModrinthId.Contains(searchText)
                || (obj as ModData).CurseforgeId.Contains(searchText);
        }
    }

    public SearchPageViewModel(LanguageService languageService, GameNewsService gameNewsService, ILogger<SearchPageViewModel> logger) {
        _logger = logger;
        _languageService = languageService;
        _gameNewsService = gameNewsService;

        PossibleSearches = languageService.ModDatas;
    }

    public async void OnLoaded() {
        try {
            await Task.Run(async () => {
                JavaPatchNotes = (await _gameNewsService.GetJavaPatchNotesAsync()).ToImmutableArray();
                CurrentJavaPatchNote = new(JavaPatchNotes.First(), JavaPatchNotes.ElementAt(1));
            });
        } catch (Exception ex) {
            _logger.LogError("遭遇了错误，完整信息堆栈：{Trace}", ex.ToString());
        }
    }
}