// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace CrissCross.WPF.UI.Styles.Controls;

/// <summary>
/// Overwrites ContextMenu-Style for some UIElements (like RichTextBox) that don't take the default ContextMenu-Style by default.
/// <para>The code inside this CodeBehind-Class forces this ContextMenu-Style on these UIElements through Reflection (because it is only accessible through Reflection it is also only possible through CodeBehind and not XAML).</para>
/// This Code is based on a StackOverflow-Answer: https://stackoverflow.com/a/56736232/9759874.
/// </summary>
public partial class ContextMenu : ResourceDictionary
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenu"/> class.
    /// Registers editing <see cref="ContextMenu"/> styles with <see cref="Dispatcher"/>.
    /// </summary>
    public ContextMenu() =>
        //// Run OnResourceDictionaryLoaded asynchronously to ensure other ResourceDictionary are already loaded before adding new entries
        Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            new Action(OnResourceDictionaryLoaded));

    private void OnResourceDictionaryLoaded()
    {
        var currentAssembly = typeof(Application).Assembly;

        AddEditorContextMenuDefaultStyle(currentAssembly);
    }

    private void AddEditorContextMenuDefaultStyle(Assembly currentAssembly)
    {
        var editorContextMenuType = Type.GetType(
            "System.Windows.Documents.TextEditorContextMenu+EditorContextMenu, " + currentAssembly);

        if (editorContextMenuType == null || this["UiContextMenu"] is not Style contextMenuStyle)
        {
            return;
        }

        var editorContextMenuStyle = new Style(editorContextMenuType, contextMenuStyle);
        Add(editorContextMenuType, editorContextMenuStyle);
    }
}
