// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Work in progress.
/// </summary>
public class TreeGridHeader : FrameworkElement
{
    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(TreeGridHeader),
        new PropertyMetadata(string.Empty, OnTitleChanged));

    /// <summary>
    /// Property for <see cref="Group"/>.
    /// </summary>
    public static readonly DependencyProperty GroupProperty = DependencyProperty.Register(
        nameof(Group),
        typeof(string),
        typeof(TreeGridHeader),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the title that will be displayed.
    /// </summary>
    public string Title
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <summary>
    /// Gets or sets the column group name.
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    [MergableProperty(false)]
    public string Group
    {
        get => (string)GetValue(GroupProperty);
        set => SetValue(GroupProperty, value);
    }

    /// <summary>
    /// This virtual method is called when <see cref="Title"/> is changed.
    /// </summary>
    protected virtual void OnTitleChanged()
    {
        var title = Title;

        if (!string.IsNullOrEmpty(Group) || string.IsNullOrEmpty(title))
        {
            return;
        }

        Group = title.ToLower().Trim();
    }

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeGridHeader header)
        {
            return;
        }

        header.OnTitleChanged();
    }
}
