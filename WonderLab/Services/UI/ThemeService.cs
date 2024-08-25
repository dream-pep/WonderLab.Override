using System;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Media.Imaging;
using WonderLab.Extensions;

namespace WonderLab.Services.UI;

public sealed class ThemeService {
    public static readonly Lazy<Bitmap> OldMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.old_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> ReleaseMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.release_minecraft.png".ToBitmap());
    public static readonly Lazy<Bitmap> SnapshotMinecraftIcon = new("resm:WonderLab.Assets.Images.Icons.snapshot_minecraft.png".ToBitmap());

    private readonly string DINPro = "resm:WonderLab.Assets.Fonts.DinPro.ttf?assembly=WonderLab#DIN Pro";

    public void SetCurrentTheme(int index) {
        Application.Current.RequestedThemeVariant = index switch {
            0 => ThemeVariant.Light,
            1 => ThemeVariant.Dark,
            2 => ThemeVariant.Default,
            _ => ThemeVariant.Default,
        };
    }

    public void ApplyDefaultFont() {
        Application.Current.Resources["DefaultFontFamily"] = $"{DINPro}";
    }
}