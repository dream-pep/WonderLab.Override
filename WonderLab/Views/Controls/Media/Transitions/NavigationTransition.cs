using Avalonia;
using System.Threading;
using Avalonia.Animation;

namespace WonderLab.Views.Controls.Media.Transitions;

public abstract class NavigationTransition : AvaloniaObject {
    public abstract void RunAnimation(Animatable ctrl, CancellationToken cancellationToken = default);
}