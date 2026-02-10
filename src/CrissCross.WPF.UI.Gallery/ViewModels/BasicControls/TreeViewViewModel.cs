// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using CP.Reactive.Collections;
using CrissCross.WPF.UI.Controls;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// Represents the view model for a tree view control, providing data binding and state management for hierarchical data
/// structures in a user interface.
/// </summary>
/// <remarks>This view model is typically used in MVVM architectures to facilitate interaction between a tree view
/// UI component and the underlying data. It inherits from RxObject to support reactive property change
/// notifications.</remarks>
public class TreeViewViewModel : RxObject
{
    private ReactiveTreeItem? _selectedItem;
    private string? _newName;
    private string? _petName;
    private string? _selectedElement;
    private string? _lastSelectedElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewViewModel"/> class with a default family tree and sets up commands for.
    /// managing tree items.
    /// </summary>
    /// <remarks>This constructor creates an initial family tree structure and configures reactive commands
    /// for adding persons or pets, expanding or collapsing tree nodes, and removing items. It also establishes
    /// subscriptions to update selection-related properties based on user interactions. The default tree includes two
    /// root persons, each with a child, to provide an example structure for immediate use.</remarks>
    public TreeViewViewModel()
    {
        var cliffordPulman = new Person("Clifford Pulman", [new Pet("Kitty")]);
        cliffordPulman.DisposeWith(Disposables);
        var clifford = new Person("Clifford", [cliffordPulman]);
        clifford.DisposeWith(Disposables);
        var clarencePulman = new Person("Clarence Pulman");
        clarencePulman.DisposeWith(Disposables);
        var clarence = new Person("Clarence", [clarencePulman]);
        clarence.DisposeWith(Disposables);
        Family = new ReactiveList<ReactiveTreeItem>([clifford, clarence]);

        AddPerson = ReactiveCommand.Create(() => { });
        AddPerson.Subscribe(_ =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            var p = new Person(NewName);
            SelectedItem.AddChild(p);
            p.IsSelected = true;
            p.ExpandPath();
        });
        AddPet = ReactiveCommand.Create(() => { });
        AddPet.Subscribe(_ =>
        {
            if (SelectedItem == null)
            {
                return;
            }

            var p = new Pet(PetName);
            SelectedItem.AddChild(p);
            p.IsSelected = true;
            p.ExpandPath();
        });
        Collapse = ReactiveCommand.Create(() => { });
        Collapse.Subscribe(_ => SelectedItem?.CollapsePath());
        Expand = ReactiveCommand.Create(() => { });
        Expand.Subscribe(_ => SelectedItem?.ExpandPath());
        Remove = ReactiveCommand.Create(() => { });
        Remove.Subscribe(_ => SelectedItem?.RemoveChild());
        var isAnimalOrPerson = Family.CurrentItems.FlattenAndSelect(
            rti =>
            {
                if (rti is Person person)
                {
                    return rti.WhenAnyValue(vs => vs.IsSelected).Select(x => (x, person.DisplayName));
                }
                else if (rti is Pet pet)
                {
                    return rti.WhenAnyValue(vs => vs.IsSelected).Select(x => (x, pet.DisplayName));
                }
                else
                {
                    return rti.WhenAnyValue(vs => vs.IsSelected).Select(x => (x, (string?)"NoName"));
                }
            });
        isAnimalOrPerson.Subscribe(x =>
        {
            if (x.x)
            {
                SelectedElement = x.Item2;
            }
            else
            {
                LastSelectedElement = x.Item2;
            }
        });
    }

    /// <summary>
    /// Gets a read-only collection containing the child items related to this tree node.
    /// </summary>
    /// <remarks>The collection reflects changes to the underlying set of child items in real time.
    /// Modifications to the collection must be performed through the appropriate methods on the parent object; direct
    /// changes to the collection are not permitted.</remarks>
    public ReactiveList<ReactiveTreeItem> Family { get; }

    /// <summary>
    /// Gets the add person.
    /// </summary>
    /// <value>
    /// The add person.
    /// </value>
    public ReactiveCommand<Unit, Unit> AddPerson { get; }

    /// <summary>
    /// Gets the add pet.
    /// </summary>
    /// <value>
    /// The add pet.
    /// </value>
    public ReactiveCommand<Unit, Unit> AddPet { get; }

    /// <summary>
    /// Gets the remove.
    /// </summary>
    /// <value>
    /// The remove.
    /// </value>
    public ReactiveCommand<Unit, Unit> Remove { get; }

    /// <summary>
    /// Gets the collapse.
    /// </summary>
    /// <value>
    /// The collapse.
    /// </value>
    public ReactiveCommand<Unit, Unit> Collapse { get; }

    /// <summary>
    /// Gets the expand.
    /// </summary>
    /// <value>
    /// The expand.
    /// </value>
    public ReactiveCommand<Unit, Unit> Expand { get; }

    /// <summary>
    /// Gets or sets creates new name.
    /// </summary>
    /// <value>
    /// The new name.
    /// </value>
    public string? NewName
    {
        get => _newName;
        set => this.RaiseAndSetIfChanged(ref _newName, value);
    }

    /// <summary>
    /// Gets or sets the name of the pet.
    /// </summary>
    /// <value>
    /// The name of the pet.
    /// </value>
    public string? PetName
    {
        get => _petName;
        set => this.RaiseAndSetIfChanged(ref _petName, value);
    }

    /// <summary>
    /// Gets or sets the selected element.
    /// </summary>
    /// <value>
    /// The selected element.
    /// </value>
    public string? SelectedElement
    {
        get => _selectedElement;
        set => this.RaiseAndSetIfChanged(ref _selectedElement, value);
    }

    /// <summary>
    /// Gets or sets the last selected element.
    /// </summary>
    /// <value>
    /// The last selected element.
    /// </value>
    public string? LastSelectedElement
    {
        get => _lastSelectedElement;
        set => this.RaiseAndSetIfChanged(ref _lastSelectedElement, value);
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    /// <value>
    /// The selected item.
    /// </value>
    public ReactiveTreeItem? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the control and optionally releases the managed resources.
    /// </summary>
    /// <remarks>This method overrides Dispose to ensure that all managed resources associated with the
    /// control are properly disposed when disposing is true. Call this method when you are finished using the control
    /// to free resources promptly.</remarks>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Family.Dispose();
            AddPerson.Dispose();
            AddPet.Dispose();
            Remove.Dispose();
            Collapse.Dispose();
            Expand.Dispose();
        }

        base.Dispose(disposing);
    }
}
