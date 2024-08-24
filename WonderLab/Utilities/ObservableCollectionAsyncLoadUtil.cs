using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace WonderLab.Utilities;

public sealed class ObservableCollectionAsyncLoadUtil<T> {
    public event Action Loaded;

    public void Pause() {
        bPause = true;
    }

    public async void Continue() {
        bPause = false;
        await AddItemAsync();
    }

    private int count;
    private bool bPause;

    private ObservableCollection<T> _sourceData;
    private ObservableCollection<T> _targetData;

    public void Load(ObservableCollection<T> targetData, ObservableCollection<T> sourceData) {
        if (sourceData == null || targetData == null)
            return;

        _sourceData = new ObservableCollection<T>(sourceData);
        _targetData = targetData;

        Load();
    }

    private async void Load() {
        count = 0;
        await AddItemAsync();
    }

    private async Task AddItemAsync() {
        if (bPause)
            return;

        if (count < _sourceData.Count) {
            await Task.Delay(1);
            _targetData.Add(_sourceData[count++]);
            await AddItemAsync();
        } else {
            _sourceData = null;
            Loaded?.Invoke();
        }
    }
}