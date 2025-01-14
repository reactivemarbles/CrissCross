// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace CrissCross.WPF.UI.AutomationPeers;

internal class CardControlAutomationPeer(CardControl owner) : FrameworkElementAutomationPeer(owner)
{
    /// <summary>
    /// Gets the control pattern for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />.
    /// </summary>
    /// <param name="patternInterface">A value from the enumeration.</param>
    /// <returns>
    /// An object that implements the <see cref="T:System.Windows.Automation.Provider.ISynchronizedInputProvider" /> interface if <paramref name="patternInterface" /> is <see cref="F:System.Windows.Automation.Peers.PatternInterface.SynchronizedInput" />; otherwise, null.
    /// </returns>
    public override object GetPattern(PatternInterface patternInterface)
    {
        if (patternInterface == PatternInterface.ItemContainer)
        {
            return this;
        }

        return base.GetPattern(patternInterface);
    }

    protected override string GetClassNameCore() => "CardControl";

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Pane;

    protected override AutomationPeer GetLabeledByCore()
    {
        if (owner.Header is UIElement element)
        {
            return CreatePeerForElement(element);
        }

        return base.GetLabeledByCore();
    }

    protected override string GetNameCore()
    {
        var result = base.GetNameCore() ?? string.Empty;

        if (result?.Length == 0)
        {
            result = AutomationProperties.GetName(owner);
        }

        if (result?.Length == 0 && owner.Header is DependencyObject d)
        {
            result = AutomationProperties.GetName(d);
        }

        if (result?.Length == 0 && owner.Header is string s)
        {
            result = s;
        }

        return result!;
    }
}
