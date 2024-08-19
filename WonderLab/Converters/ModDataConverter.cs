using System;
using System.Globalization;
using WonderLab.Classes.Datas;
using Avalonia.Data.Converters;

namespace WonderLab.Converters;
public sealed class ModDataConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var modData = value as ModData;

        if (modData is null) {
            return "default";
        }

        return modData.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}