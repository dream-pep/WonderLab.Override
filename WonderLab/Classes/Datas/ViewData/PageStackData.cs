using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WonderLab.Views.Controls.Media.Transitions;

namespace WonderLab.Classes.Datas.ViewData;
public class PageStackData {
    /// <summary>
    /// Initializes a new instance of the PageStackEntry class.
    /// </summary>
    /// <param name="sourcePageType">The type of page associated with the navigation entry, as a type reference</param>
    /// <param name="parameter">The navigation parameter associated with the navigation entry.</param>
    /// <param name="navigationTransitionInfo">Info about the animated transition associated with the navigation entry.</param>
    public PageStackData(NavigationTransition navigationTransitionInfo) {
        NavigationTransition = navigationTransitionInfo ?? new EntranceNavigationTransition();
    }

    /// <summary>
    /// Gets a value that indicates the animated transition associated with the navigation entry.
    /// </summary>
    public NavigationTransition NavigationTransition { get; }

    internal Control Instance { get; set; }

}
