using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Threading;
using System.Windows.Input;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using WonderLab.Services.UI;

namespace WonderLab.Views.Controls;

[PseudoClasses(":ispanelopen", ":ispanelclose")]
public sealed class NavigationView : SelectingItemsControl {
    private ContentPresenter _PART_ContentPresenter;
    private Border _PART_Border;

    private bool _isRunPanelAnimation;

    [Obsolete("弃用回调逻辑.")]
    public event EventHandler GoBacked;

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(Content));

    public static readonly StyledProperty<object> MainContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(MainContent));

    public static readonly StyledProperty<object> FooterContentProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(FooterContent));

    public static readonly StyledProperty<bool> CanGoBackProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(CanGoBack), false);

    public static readonly StyledProperty<bool> IsOpenBackgroundPanelProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsOpenBackgroundPanel));

    [Content]
    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool CanGoBack {
        get => GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    public object MainContent {
        get => GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }

    public object FooterContent {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    public bool IsOpenBackgroundPanel {
        get => GetValue(IsOpenBackgroundPanelProperty);
        set => SetValue(IsOpenBackgroundPanelProperty, value);
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        App.GetService<WindowService>().HandlePropertyChanged(BoundsProperty, () => {
            _PART_Border.Width = _PART_ContentPresenter.Bounds.Width - 10;
            _PART_Border.Height = _PART_ContentPresenter.Bounds.Height -10;
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_ContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        _PART_Border = e.NameScope.Find<Border>("PART_Border");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenBackgroundPanelProperty) {
            var @bool = change.GetNewValue<bool>();
            _isRunPanelAnimation = @bool;

            Dispatcher.UIThread.Post(() => {
                _PART_ContentPresenter.Opacity = @bool ? 0 : 1;
                _PART_Border.Margin = new(14, 5, 14, @bool ? 15 : -_PART_ContentPresenter.Bounds.Height);
            }, DispatcherPriority.Render);
        }
    }
}

public sealed class NavigationViewItem : ListBoxItem, ICommandSource {
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<NavigationViewItem, string>(nameof(Icon));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<NavigationViewItem, ICommand>(nameof(Command));

    public static readonly StyledProperty<object> CommandParameterProperty =
        AvaloniaProperty.Register<NavigationView, object>(nameof(CommandParameter));

    public string Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        e.NameScope.Find<Button>("ButtonLayout")!.Click += (sender, args) => {
            IsSelected = IsSelected ? IsSelected : !IsSelected;
        };
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) {
        throw new NotImplementedException();
    }
}