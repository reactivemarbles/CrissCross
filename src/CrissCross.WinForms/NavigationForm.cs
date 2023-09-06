// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reactive.Linq;

namespace CrissCross.WinForms;

/// <summary>
/// NavigationForm.
/// </summary>
/// <seealso cref="Form" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
public partial class NavigationForm : Form, ISetNavigation, IUseNavigation, IAmBuilt
{
    private DockStyle _navigationFrameDock = DockStyle.Fill;
    private bool _navigateBackIsEnabled = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationForm"/> class.
    /// </summary>
    public NavigationForm() => InitializeComponent();

    /// <summary>
    /// Gets or sets a value indicating whether [navigate back is enabled].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    [Category("CrissCross")]
    [Description("A value indicating if Navigating back is enabled.")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true)]
    public bool NavigateBackIsEnabled
    {
        get => _navigateBackIsEnabled;
        set
        {
            _navigateBackIsEnabled = value;
            NavigationFrame.NavigateBackIsEnabled = _navigateBackIsEnabled;
        }
    }

    /// <summary>
    /// Gets or sets the navigation frame dock.
    /// </summary>
    /// <value>
    /// The navigation frame dock.
    /// </value>
    [Category("CrissCross")]
    [Description("A value indicating the dock style.")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true)]
    public DockStyle NavigationFrameDock
    {
        get => _navigationFrameDock;
        set
        {
            _navigationFrameDock = value;
            NavigationFrame.Dock = _navigationFrameDock;
        }
    }

    /// <summary>
    /// Gets the can navigate back.
    /// </summary>
    /// <value>
    /// The can navigate back.
    /// </value>
    public IObservable<bool> CanNavigateBack => NavigationFrame.CanNavigateBackObservable.Select(x => x == true);

    /// <summary>
    /// Gets the navigation frame.
    /// </summary>
    /// <value>
    /// The navigation frame.
    /// </value>
    public ViewModelRoutedViewHost NavigationFrame { get; } = new();

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
        SuspendLayout();
        NavigationFrame.HostName = Name;
        if (!DesignMode)
        {
            this.SetMainNavigationHost(NavigationFrame);
        }

        NavigationFrame.NavigateBackIsEnabled = NavigateBackIsEnabled;
        NavigationFrame.Dock = NavigationFrameDock;
        Controls.Add(NavigationFrame);
        ResumeLayout();
        base.OnLoad(e);
    }
}
