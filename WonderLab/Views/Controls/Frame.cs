using Avalonia;
using Avalonia.Controls;
using Avalonia.Animation;
using Avalonia.Threading;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using System.Linq;
using System.Collections.Generic;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Views.Controls.Media.Transitions;

namespace WonderLab.Views.Controls;

/// <summary>
/// 页面导航控件
/// </summary>
/// <remarks>
/// 将 <see cref="UserControl"/> 处理并显示出来
/// </remarks>
public sealed class Frame : ContentControl {
    private ContentPresenter PART_ContentPresenter;

    private Stack<PageStackData> _pageStack = new();

    public bool CanGoBack => _pageStack.Count > 0;

    public void GoBack() {
        if (CanGoBack && _pageStack.TryPop(out _)) {
            SetContentAndAnimate(_pageStack.Peek());
        }
    }

    public void Navigate(Control control, NavigationTransition transition = null) {
        _pageStack.Push(new(transition) {
            Instance = control,
        });

        if (PART_ContentPresenter is not null) {
            SetContentAndAnimate(_pageStack.Peek());
        }
    }

    private PageStackData GetPageInStack(Control userControl) {
        return _pageStack.FirstOrDefault(x => nameof(x.Instance) == nameof(userControl));
    }

    private void SetContentAndAnimate(PageStackData pageStackData) {
        var pageStack = GetPageInStack(pageStackData.Instance) ?? pageStackData;
        Content = pageStack.Instance;

        if (PART_ContentPresenter is not null) {
            PART_ContentPresenter.Opacity = 0;

            Dispatcher.UIThread.Post(() => {
                pageStack.NavigationTransition.RunAnimation(PART_ContentPresenter, default);
            }, DispatcherPriority.Render);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        PART_ContentPresenter = e.NameScope.Find<ContentPresenter>(nameof(PART_ContentPresenter));
    }
}