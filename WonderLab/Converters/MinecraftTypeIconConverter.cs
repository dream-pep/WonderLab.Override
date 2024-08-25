using Avalonia.Data.Converters;
using System;
using System.Globalization;
using WonderLab.Services.UI;

namespace WonderLab.Converters;

public sealed class MinecraftTypeIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string type = value.ToString();
        if (string.IsNullOrEmpty(type)) {
            return null;
        }

        return type switch {
            "old_beta" => ThemeService.OldMinecraftIcon.Value,
            "old_alpha" => ThemeService.OldMinecraftIcon.Value,
            "release" => ThemeService.ReleaseMinecraftIcon.Value,
            "snapshot" => ThemeService.SnapshotMinecraftIcon.Value,
            _ => null
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}