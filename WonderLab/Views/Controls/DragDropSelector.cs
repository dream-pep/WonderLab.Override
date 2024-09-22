using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Extensions;

namespace WonderLab.Views.Controls;

/// <summary>
/// 拖放选择器
/// </summary>
public sealed class DragDropSelector : TemplatedControl {
    public const string DEFAULT_DRAG_DATAFORMAT = "DefaultDragUserCard";

    private bool _captured;
    private Point _dragInitialPoint;
    private PointerEventArgs _dragEventArgs;

    private FontIcon _PART_FontIcon;
    private Button _PART_AfreshButton;
    private Button _PART_CancelButton;
    private Button _PART_ConfirmButton;
    private Border _PART_SelectReceiver;
    private TextBlock _PART_TextBlock;
    private TextBlock _PART_SubTextBlock;
    private TextBlock _PART_SubTitleTextBlock;
    private PageSwitcher _PART_PageSwitcher;

    public static readonly StyledProperty<AccountViewData> SelectedAccountProperty =
        AvaloniaProperty.Register<DragDropSelector, AccountViewData>(nameof(SelectedAccount));

    public static readonly StyledProperty<IEnumerable> ItemSourceProperty =
        AvaloniaProperty.Register<DragDropSelector, IEnumerable>(nameof(ItemSource));

    public static readonly StyledProperty<ICommand> ConfirmCommandProperty =
        AvaloniaProperty.Register<DragDropSelector, ICommand>(nameof(ConfirmCommand));

    public static readonly StyledProperty<ICommand> CancelCommandProperty =
        AvaloniaProperty.Register<DragDropSelector, ICommand>(nameof(CancelCommand));

    public AccountViewData SelectedAccount {
        get => GetValue(SelectedAccountProperty);
        set => SetValue(SelectedAccountProperty, value);
    }

    public IEnumerable ItemSource {
        get => GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public ICommand ConfirmCommand {
        get => GetValue(ConfirmCommandProperty);
        set => SetValue(ConfirmCommandProperty, value);
    }

    public ICommand CancelCommand {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (_PART_SelectReceiver is not null) {
            DragDrop.SetAllowDrop(_PART_SelectReceiver, true);
        }

        _PART_SelectReceiver?.AddHandler(DragDrop.DropEvent, OnDragItemDrop, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        _PART_PageSwitcher.ItemsSource = null;
        _PART_PageSwitcher.ItemsSource = (ItemSource as IEnumerable<object>)
            .Select(x => new ListBoxItem() { DataContext = x })
            .ToObservableList();

        foreach (var item in _PART_PageSwitcher.ItemsSource) {
            (item as Control)?.AddHandler(PointerMovedEvent, OnDragItemPointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
            (item as Control)?.AddHandler(PointerPressedEvent, OnDragItemPointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_PageSwitcher = e.NameScope.Find<PageSwitcher>("PART_PageSwitcher");
        _PART_FontIcon = e.NameScope.Find<FontIcon>("PART_FontIcon");
        _PART_TextBlock = e.NameScope.Find<TextBlock>("PART_TextBlock");
        _PART_SubTextBlock = e.NameScope.Find<TextBlock>("PART_SubTextBlock");
        _PART_SubTitleTextBlock = e.NameScope.Find<TextBlock>("PART_SubTitleTextBlock");
        _PART_CancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        _PART_AfreshButton = e.NameScope.Find<Button>("PART_AfreshButton");
        _PART_ConfirmButton = e.NameScope.Find<Button>("PART_ConfirmButton");
        _PART_SelectReceiver = e.NameScope.Find<Border>("PRAT_SelectReceiver");

        _PART_AfreshButton.Click += (_, _) => {
            SelectedAccount = null;

            ControlButtonGroup(false);
            HideReceiveInfo(true);
            HideListBox(true);
        };
    }

    private void ControlButtonGroup(bool isShow = true) {
        _PART_SelectReceiver.Width = isShow ? 260 : 180;
        _PART_CancelButton.Width = isShow ? 0 : 180;
        _PART_ConfirmButton.Width = isShow ? 172 : 0;

        _PART_AfreshButton.IsVisible = isShow;
        _PART_AfreshButton.Width = isShow ? 80 : 0;
    }

    private void HideListBox(bool isReverse = false) {
        _PART_PageSwitcher.Opacity = isReverse ? 1 : 0;
        _PART_PageSwitcher.Margin = isReverse ? new(16) : new(0, 0, 0, 26);
    }

    private async void HideReceiveInfo(bool isReverse = false) {
        _PART_SubTextBlock.Opacity = isReverse ? 0 : 1;
        _PART_SubTitleTextBlock.Opacity = isReverse ? 0 : 1;

        if (isReverse) {
            await Task.Delay(TimeSpan.FromSeconds(0.5));
        }

        _PART_FontIcon.Opacity = isReverse ? 1 : 0;
        _PART_TextBlock.Opacity = isReverse ? 1 : 0;
        _PART_FontIcon.Margin = isReverse ? new(0, 0, 0, 0) : new(0, 0, 0, 0);
        _PART_TextBlock.Margin = isReverse ? new(0, 8, 0, 0) : new(0, 0, 0, 10);
    }

    private void OnDragItemDrop(object sender, DragEventArgs args) {
        _captured = true;
        SelectedAccount = (args.Data.Get(DEFAULT_DRAG_DATAFORMAT) as ListBoxItem).DataContext as AccountViewData;

        //_PART_TextBlock.Text = (args.Data.Get(DEFAULT_DRAG_DATAFORMAT) as ListBoxItem).Content.ToString();
        HideListBox();
        HideReceiveInfo();
        ControlButtonGroup();
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