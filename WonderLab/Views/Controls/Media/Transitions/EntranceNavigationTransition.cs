using Avalonia;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using System;
using System.Threading;

namespace WonderLab.Views.Controls.Media.Transitions;

/// <summary>
/// Specifies the animation to run when a user navigates forward in a logical hierarchy, 
/// like from a master list to a detail page.
/// </summary>
public class EntranceNavigationTransition : NavigationTransition {
    public async override void RunAnimation(Animatable ctrl, CancellationToken cancellationToken = default) {
        var animation = new Animation {
            Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
            Children = {
                new KeyFrame {
                    Setters = {
                        new Setter(Visual.OpacityProperty, 0.0),
                        new Setter(TranslateTransform.YProperty, 100.0d),
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame {
                    Setters = {
                        new Setter(Visual.OpacityProperty, 1d),
                        new Setter(TranslateTransform.YProperty, 0.0d),
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromSeconds(0.5d),
            FillMode = FillMode.Forward
        };

        await animation.RunAsync(ctrl, cancellationToken);
    }
}