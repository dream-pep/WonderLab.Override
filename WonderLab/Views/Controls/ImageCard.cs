using Avalonia;
using Avalonia.Media;
using Avalonia.Metadata;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using AsyncImageLoader;

namespace WonderLab.Views.Controls;

public sealed class ImageCard : TemplatedControl {
    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<ImageCard, string>(nameof(Source));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ImageCard, string>(nameof(Title), "Loading...");

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<ImageCard, string>(nameof(Description), "Loading...");

    public static readonly StyledProperty<string> DateProperty =
        AvaloniaProperty.Register<ImageCard, string>(nameof(Date), "Loading...");

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<ImageCard, ICommand>(nameof(Command));

    public string Source {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public string Date {
        get => GetValue(DateProperty);
        set => SetValue(DateProperty, value); 
    }

    public string Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public ICommand Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}