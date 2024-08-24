using WonderLab.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WonderLab.Extensions;

public static class CollectionExtension {
    public static ObservableCollection<T> ToObservableList<T>(this IEnumerable<T> values) {
        return new(values);
    }

    public static ObservableCollectionAsyncLoadUtil<T> Load<T>(this ObservableCollection<T> targetData, ObservableCollection<T> sourceData, bool isClear = true) {
        if (isClear) {
            targetData.Clear();
        }

        var helper = new ObservableCollectionAsyncLoadUtil<T>();
        helper.Load(targetData, sourceData);
        return helper;
    }
}