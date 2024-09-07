using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WonderLab.Views.Controls;

/// <summary>
/// 拖放选择器
/// </summary>
public sealed class DragDropSelector : TemplatedControl {
    public const string DEFAULT_DRAG_DATAFORMAT = "DefaultDragUserCard";

    private bool _captured;
    private Point _dragInitialPoint;
    private PointerEventArgs _dragEventArgs;

    private ListBox _PART_ListBox;
    private FontIcon _PART_FontIcon;
    private Button _PART_CancelButton;
    private Border _PRAT_SelectReceiver;
    private TextBlock _PART_TextBlock;
    private TextBlock _PART_SubTextBlock;
    private TextBlock _PART_SubTitleTextBlock;

    public static readonly StyledProperty<IEnumerable> ItemSourceProperty =
        AvaloniaProperty.Register<DragDropSelector, IEnumerable>(nameof(ItemSource));

    public IEnumerable ItemSource {
        get => GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (_PRAT_SelectReceiver is not null) {
            DragDrop.SetAllowDrop(_PRAT_SelectReceiver, true);
        }

        _PRAT_SelectReceiver?.AddHandler(DragDrop.DropEvent, OnDragItemDrop, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        foreach (var item in _PART_ListBox.ItemsSource ?? _PART_ListBox.Items) {
            (item as Control)?.AddHandler(PointerMovedEvent, OnDragItemPointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            (item as Control)?.AddHandler(PointerPressedEvent, OnDragItemPointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_ListBox = e.NameScope.Find<ListBox>("PART_ListBox");
        _PART_FontIcon = e.NameScope.Find<FontIcon>("PART_FontIcon");
        _PART_TextBlock = e.NameScope.Find<TextBlock>("PART_TextBlock");
        _PART_SubTextBlock = e.NameScope.Find<TextBlock>("PART_SubTextBlock");
        _PART_SubTitleTextBlock = e.NameScope.Find<TextBlock>("PART_SubTitleTextBlock");
        _PART_CancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        _PRAT_SelectReceiver = e.NameScope.Find<Border>("PRAT_SelectReceiver");

        _PART_CancelButton.Click += (_, _) => {
            HideReceiveInfo(true);
            HideListBox(true);
        };
    }

    private void HideListBox(bool isReverse = false) {
        _PART_ListBox.Opacity = isReverse ? 1 : 0;
        _PART_ListBox.Margin = isReverse ? new(0, 0, 0, 0) : new(0, 0, 0, 10);
    }

    private void HideReceiveInfo(bool isReverse = false) {
        _PART_FontIcon.Opacity = isReverse ? 1 : 0;
        _PART_TextBlock.Opacity = isReverse ? 1 : 0;
        _PART_FontIcon.Margin = isReverse ? new(0, 0, 0, 0) : new(0, 0, 0, -10);
        _PART_TextBlock.Margin = isReverse ? new(0, 8, 0, 0) : new(0, 0, 0, 10);

        _PART_SubTextBlock.Opacity = isReverse ? 0 : 1;
        _PART_SubTitleTextBlock.Opacity = isReverse ? 0 : 1;
    }

    private void OnDragItemDrop(object sender, DragEventArgs args) {
        _captured = true;
        //_PART_TextBlock.Text = (args.Data.Get(DEFAULT_DRAG_DATAFORMAT) as ListBoxItem).Content.ToString();
        HideListBox();
        HideReceiveInfo();
    }

    private async void OnDragItemPointerMoved(object sender, PointerEventArgs e) {
        var properties = e.GetCurrentPoint(sender as Visual).Properties;

        if (properties.IsLeftButtonPressed) {
            await DoDragDropAsync(_dragEventArgs, sender as Visual);
            if (!_captured) {
                _PART_TextBlock.Text = "将档案拖至此处";
            } else {
                _captured = false;
            }
        }
    }

    private void OnDragItemPointerPressed(object sender, PointerPressedEventArgs e) {
        var properties = e.GetCurrentPoint(sender as Visual).Properties;
        if (properties.IsLeftButtonPressed) {
            if (e.Source is Control control) {
                if ((control as ISelectable ?? control.Parent as ISelectable ?? control.FindLogicalAncestorOfType<ISelectable>())?.IsSelected ?? false) {
                    e.Handled = true;
                }

                //_captured = true;
                _dragEventArgs = e;
                _dragInitialPoint = e.GetPosition(null);
            }
        }
    }

    private static async Task DoDragDropAsync(PointerEventArgs triggerEvent, object value) {
        var data = new DataObject();
        data.Set(DEFAULT_DRAG_DATAFORMAT, value!);

        await DragDrop.DoDragDrop(triggerEvent, data, DragDropEffects.Move);
    }
}