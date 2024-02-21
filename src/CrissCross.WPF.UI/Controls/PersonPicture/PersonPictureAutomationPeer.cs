// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Automation.Peers;

namespace CrissCross.WPF.UI.Controls
{
    /// <summary>
    /// PersonPictureAutomationPeer.
    /// </summary>
    /// <seealso cref="System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="PersonPictureAutomationPeer"/> class.
    /// </remarks>
    /// <param name="owner">The owner.</param>
    public class PersonPictureAutomationPeer(PersonPicture owner) : FrameworkElementAutomationPeer(owner)
    {
        /// <summary>
        /// Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>
        /// The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;

        /// <summary>
        /// Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>
        /// An <see cref="F:System.String.Empty" /> string.
        /// </returns>
        protected override string GetClassNameCore() => nameof(PersonPicture);
    }
}
