// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.CompilerServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WinForms;
#else
namespace CrissCross.WinForms;
#endif

/// <summary>Hosts WinForms navigation content.</summary>
/// <seealso cref="Form" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IUseNavigation" />
public partial class NavigationForm : Form, ISetNavigation, IUseNavigation
{
    /// <summary>Stores the navigation Host Name value.</summary>
    private string? _navigationHostName;

    /// <summary>Stores the navigation Frame Dock value.</summary>
    private DockStyle _navigationFrameDock = DockStyle.Fill;

    /// <summary>Stores the navigate Back Is Enabled value.</summary>
    private bool _navigateBackIsEnabled = true;

    /// <summary>Initializes a new instance of the <see cref="NavigationForm"/> class.</summary>
    public NavigationForm() => InitializeComponent();

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
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

    /// <summary>Gets or sets the navigation frame dock.</summary>
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

    /// <summary>Gets the can navigate back.</summary>
    /// <value>
    /// The can navigate back.
    /// </value>
    public IObservable<bool> CanNavigateBack => NavigationFrame.CanNavigateBackObservable.Select(static x => x == true);

    /// <summary>Gets or sets the stable routed navigation host name.</summary>
    [Category("CrissCross")]
    [Description("The logical navigation host name.")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Localizable(true)]
    public string? HostName
    {
        get => _navigationHostName ?? Name;
        set
        {
            _navigationHostName = string.IsNullOrWhiteSpace(value) ? null : value;
            NavigationFrame.HostName = ResolveNavigationHostName();
        }
    }

    /// <summary>Gets the navigation frame.</summary>
    /// <value>
    /// The navigation frame.
    /// </value>
    public ViewModelRoutedViewHost NavigationFrame { get; } = new();

    /// <inheritdoc/>
    string? ISetNavigation.Name => HostName;

    /// <inheritdoc/>
    string? IUseNavigation.Name => HostName;

    /// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.</summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
        SuspendLayout();
        var hostName = ResolveNavigationHostName();
        _navigationHostName = hostName;
        NavigationFrame.HostName = hostName;
        if (string.IsNullOrWhiteSpace(NavigationFrame.Name))
        {
            NavigationFrame.Name = hostName;
        }

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

    /// <summary>Runs the resolve Navigation Host Name operation.</summary>
    /// <returns>The resolved host name.</returns>
    private string ResolveNavigationHostName()
    {
        if (!string.IsNullOrWhiteSpace(_navigationHostName))
        {
            return _navigationHostName!;
        }

        return !string.IsNullOrWhiteSpace(Name)
            ? Name
            : $"__crisscross_navhost_{nameof(NavigationForm)}_{RuntimeHelpers.GetHashCode(this):X8}";
    }
}
