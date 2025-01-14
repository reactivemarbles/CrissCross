// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extends <see cref="System.Windows.Controls.GridView"/> to use Wpf.Ui custom styles.
/// </summary>
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
public class GridView : System.Windows.Controls.GridView
{
    static GridView()
    {
        ResourceDictionary resourceDict =
            new()
            {
                Source = new Uri("pack://application:,,,/CrissCross.WPF.UI;component/Controls/GridView/GridViewColumnHeader.xaml")
            };

        var defaultStyle = (Style)resourceDict["UiGridViewColumnHeaderStyle"];

        ColumnHeaderContainerStyleProperty.OverrideMetadata(
            typeof(GridView),
            new FrameworkPropertyMetadata(defaultStyle));
    }
}
