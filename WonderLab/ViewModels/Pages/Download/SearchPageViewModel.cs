using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Extensions;
using WonderLab.Services.Game;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Pages.Download;

public sealed partial class SearchPageViewModel : ViewModelBase {
    private readonly LanguageService _languageService;
    private readonly GameNewsService _gameNewsService;

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

    public SearchPageViewModel(LanguageService languageService, GameNewsService gameNewsService) {
        _languageService = languageService;
        _gameNewsService = gameNewsService;

        PossibleSearches = languageService.ModDatas;
    }

    public async void OnLoaded() {
        await Task.Run(async () => {
            JavaPatchNotes = (await _gameNewsService.GetJavaPatchNotesAsync()).ToImmutableArray();
            CurrentJavaPatchNote = new(JavaPatchNotes.First(), JavaPatchNotes.ElementAt(1));
        });
    }
}