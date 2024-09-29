using Avalonia;
using Avalonia.Controls;
using System.Collections;
using WonderLab.Services.UI;
using Avalonia.Interactivity;
using System.Collections.Generic;
using WonderLab.Classes.Interfaces;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using WonderLab.Classes.Datas.ViewData;

namespace WonderLab.Views.Controls;

public sealed class TaskListPanel : ItemsControl {
    private Border _PART_LayoutBorder;
    //private ListBox _taskListBox;
    private Border _PART_ContentLayout;
    private TextBlock _PART_TaskListTip;
    private WindowService _windowService;

    public bool IsPaneOpen {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<TaskListPanel, bool>(nameof(IsPaneOpen), false);

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        if (Design.IsDesignMode) {
            return;
        }

        _windowService = App.ServiceProvider.GetService<WindowService>();
        _PART_ContentLayout.RenderTransform = TransformOperations.Parse($"translateX({_PART_ContentLayout.Bounds.Width + 15}px)");

        Items.CollectionChanged += (o, args) => {
            switch (args.Action) {
                case NotifyCollectionChangedAction.Add:
                    _PART_TaskListTip.Opacity = 0;
                    if (ItemCount == 1) {
                        _PART_ContentLayout.Height = _PART_LayoutBorder.Bounds.Height - 5;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (ItemCount == 0) {
                        _PART_TaskListTip.Opacity = 1;
                        _PART_ContentLayout.Height = 130;
                    }
                    break;
            }
        };

        _windowService.HandlePropertyChanged(BoundsProperty, () => {
            if (ItemCount > 0) {
                _PART_ContentLayout.Height = _PART_LayoutBorder.Bounds.Height - 5;
            }

            _PART_ContentLayout.MaxHeight = _PART_LayoutBorder.Bounds.Height - 5;
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_ContentLayout = e.NameScope.Find<Border>("PART_ContentLayout");
        _PART_LayoutBorder = e.NameScope.Find<Border>("PART_LayoutBorder");
        _PART_TaskListTip = e.NameScope.Find<TextBlock>("PART_TaskListTip");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == IsPaneOpenProperty) {
            var px = change.GetNewValue<bool>() ? 0 : _PART_ContentLayout.Bounds.Width + 15;
            _PART_ContentLayout.RenderTransform = TransformOperations.Parse($"translateX({px}px)");
        }
    }
}