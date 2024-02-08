// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace CrissCross.WPF.UI.AutomationPeers;

internal class CardControlAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly CardControl _owner;

    public CardControlAutomationPeer(CardControl owner)
        : base(owner)
    {
        _owner = owner;
    }

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
        if (_owner.Header is UIElement element)
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
            result = AutomationProperties.GetName(_owner);
        }

        if (result?.Length == 0 && _owner.Header is DependencyObject d)
        {
            result = AutomationProperties.GetName(d);
        }

        if (result?.Length == 0 && _owner.Header is string s)
        {
            result = s;
        }

        return result!;
    }
}
