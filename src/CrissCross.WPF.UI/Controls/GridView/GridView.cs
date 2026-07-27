// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extends <see cref="System.Windows.Controls.GridView"/> to use Wpf.Ui custom styles.</summary>
/// <example>
/// To use this enhanced GridView in a ListView:
/// <code lang="xml">
/// &lt;ListView&gt;
///     &lt;ListView.View&gt;
///         &lt;local:GridView&gt;
///             &lt;GridViewColumn Header="First Name" DisplayMemberBinding="{Binding FirstName}"/&gt;
///             &lt;GridViewColumn Header="Last Name" DisplayMemberBinding="{Binding LastName}"/&gt;
///         &lt;/local:GridView&gt;
///     &lt;/ListView.View&gt;
/// &lt;/ListView&gt;
/// </code>
/// </example>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class GridView : System.Windows.Controls.GridView
{
#if REACTIVE_SHIM
    /// <summary>The pack URI for the column-header style dictionary.</summary>
    private const string GridViewColumnHeaderUri =
        "pack://application:,,,/CrissCross.WPF.UI.Reactive;component/Controls/GridView/GridViewColumnHeader.xaml";
#else
    /// <summary>The pack URI for the column-header style dictionary.</summary>
    private const string GridViewColumnHeaderUri =
        "pack://application:,,,/CrissCross.WPF.UI;component/Controls/GridView/GridViewColumnHeader.xaml";
#endif

    /// <summary>Provides the GridView member.</summary>
    static GridView()
    {
        ResourceDictionary resourceDict = new() { Source = new(GridViewColumnHeaderUri), };

        var defaultStyle = (Style)resourceDict["UiGridViewColumnHeaderStyle"];

        ColumnHeaderContainerStyleProperty.OverrideMetadata(
            typeof(GridView),
            new FrameworkPropertyMetadata(defaultStyle));
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
