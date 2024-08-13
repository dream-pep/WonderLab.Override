using Avalonia;
using System.Threading;
using Avalonia.Animation;

namespace WonderLab.Views.Controls.Media.Transitions;

public abstract class NavigationTransition : AvaloniaObject {
    /// <summary>
    /// Executes a predefined animation on the desired object
    /// </summary>
    /// <param name="ctrl">The object to animate</param>
    /// <param name="cancellationToken"></param>
    public abstract void RunAnimation(Animatable ctrl, CancellationToken cancellationToken);
}
