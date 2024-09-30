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
using Avalonia.Media;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System.Threading.Tasks;
using System.Linq;
using Waher.Script.Units.BaseQuantities;

namespace WonderLab.Views.Controls;

[PseudoClasses(":ispanelopen", ":ispanelclose")]
public sealed class NavigationView : SelectingItemsControl {
    private readonly Rotate3DTransform _rotate3DTransform = new() {
        Depth = 500,
        AngleX = 0,
        AngleY = 0,
        Transitions = [
            new DoubleTransition {
                Easing = new ExponentialEaseOut(),
                Duration = TimeSpan.FromSeconds(0.3d),
                Property = Rotate3DTransform.AngleXProperty
            },
            new DoubleTransition {
                Easing = new ExponentialEaseOut(),
                Duration = TimeSpan.FromSeconds(0.3d),
                Property = Rotate3DTransform.AngleYProperty
            }
        ]
    };

    private ContentPresenter _PART_ContentPresenter;
    private ScrollViewer _PART_ScrollViewer;
    private Border _PART_Border;

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

    public static readonly StyledProperty<bool> IsCenterProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsCenter), false);

    public static readonly StyledProperty<bool> IsOpenBackgroundPanelProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsOpenBackgroundPanel));

    [Content]
    public object Content {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsCenter {
        get => GetValue(IsCenterProperty);
        set => SetValue(IsCenterProperty, value);
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

    private double GetAllItemWidth() {
        var result = 0d;
        foreach (var item in Items.Cast<Visual>()) {
            result += item.Bounds.Width + 5;
        }

        return result;
    }

    private async void RunAnimation(bool isCenter, bool isDelay = true) {
        double totalItemsWidth = GetAllItemWidth();
        double offset = (_PART_ScrollViewer.Bounds.Width - 16 - totalItemsWidth) / 2;
        //252.39999999999998 243.59999999999997
        //260.4
        foreach (var item in (isCenter ? Items.Reverse() : Items).Cast<NavigationViewItem>()) {
            item.GoCenter(isCenter ? offset : 0);
            await Task.Delay(TimeSpan.FromMilliseconds(isDelay ? 50 : 0));
        }
    }

    protected override async void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_ContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        _PART_ScrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        _PART_Border = e.NameScope.Find<Border>("PART_Border");

        _PART_Border.RenderTransform = _rotate3DTransform;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenBackgroundPanelProperty) {
            var @bool = change.GetNewValue<bool>();

            Dispatcher.UIThread.Post(() => {
                _PART_Border.Opacity = @bool ? 1 : 0;
                _PART_Border.IsHitTestVisible = @bool;
                _PART_ContentPresenter.Opacity = @bool ? 0 : 1;
                
            }, DispatcherPriority.Render);
        }

        if (change.Property == IsCenterProperty) {
            RunAnimation(change.GetNewValue<bool>());
        }
    }
}

public sealed class NavigationViewItem : ListBoxItem, ICommandSource {
    private readonly TranslateTransform _defaultAnimation = new() {
        X = 0,
        Y = 0,
        Transitions = [
            new DoubleTransition {
                Easing = new ExponentialEaseOut(),
                Duration = TimeSpan.FromSeconds(0.5d),
                Property = TranslateTransform.XProperty
            },
            new DoubleTransition {
                Easing = new ExponentialEaseOut(),
                Duration = TimeSpan.FromSeconds(0.5d),
                Property = TranslateTransform.YProperty
            }
        ]
    };

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

    internal void GoCenter(double offset) {
        _defaultAnimation.X = offset;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        if (Parent is not NavigationView) {
            _defaultAnimation.Y = 0;
        }

        RenderTransform = _defaultAnimation;
        e.NameScope.Find<Button>("ButtonLayout")!.Click += (sender, args) => {
            IsSelected = IsSelected ? IsSelected : !IsSelected;
        };
    }

    void ICommandSource.CanExecuteChanged(object sender, EventArgs e) {
        throw new NotImplementedException();
    }
}