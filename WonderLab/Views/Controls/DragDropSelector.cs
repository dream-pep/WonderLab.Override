using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Collections;

namespace WonderLab.Views.Controls;

/// <summary>
/// 拖放选择器
/// </summary>
public sealed class DragDropSelector : TemplatedControl {
    private Border PRAT_SelectReceiver;

    public static readonly StyledProperty<IEnumerable> ItemSourceProperty =
        AvaloniaProperty.Register<DragDropSelector, IEnumerable>(nameof(ItemSource));

    public IEnumerable ItemSource {
        get => GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        //DragDrop.SetAllowDrop(PRAT_SelectReceiver, true);
    }
}