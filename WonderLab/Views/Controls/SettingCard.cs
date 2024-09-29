using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SixLabors.Fonts;
using System.Diagnostics;

namespace WonderLab.Views.Controls;

public sealed class SettingCard : ItemsControl {
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SettingCard, string>(nameof(Header), "Hello Title");

    public static readonly StyledProperty<string> GlyphProperty =
        AvaloniaProperty.Register<SettingCard, string>(nameof(Glyph));

    public string Header {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string Glyph {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }
}