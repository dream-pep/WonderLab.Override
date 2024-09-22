using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using System;
using WonderLab.Classes.Enums;

namespace WonderLab.Behaviors;

public sealed class TaskStepProgressBarBehavior : Behavior<ProgressBar> {
    public static readonly StyledProperty<ProgressBarState> ProgressStateProperty =
        AvaloniaProperty.Register<TaskStepProgressBarBehavior, ProgressBarState>(nameof(ProgressState), ProgressBarState.Normal);

    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<TaskStepProgressBarBehavior, bool>(nameof(IsIndeterminate), true);

    public bool IsIndeterminate {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    public ProgressBarState ProgressState {
        get => GetValue(ProgressStateProperty);
        set => SetValue(ProgressStateProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ProgressStateProperty) {
            AssociatedObject.Foreground = change.GetNewValue<ProgressBarState>() switch {
                ProgressBarState.Normal => Application.Current.Resources["PrimaryBrush500"] as IBrush,
                ProgressBarState.Pause => Application.Current.Resources["WarningBrush500"] as IBrush,
                ProgressBarState.Error => Application.Current.Resources["DangerColor500"] as IBrush,
                _ => throw new NotImplementedException(),
            };
        }

        if (change.Property == IsIndeterminateProperty) {
            AssociatedObject.IsIndeterminate = change.GetNewValue<bool>();
        }
    }
}