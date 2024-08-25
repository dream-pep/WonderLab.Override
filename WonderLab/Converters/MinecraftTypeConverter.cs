using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WonderLab.Converters;

public sealed class MinecraftTypeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string type = value.ToString();
        if (string.IsNullOrEmpty(type)) {
            return "我上早八";
        }

        return type switch {
            "release" => "正式版",
            "snapshot" => "快照版",
            "old_beta" => "远古 Beta 版",
            "old_alpha" => "远古 Alpha 版",
            _ => "我上早八"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}