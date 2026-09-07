# Avalonia navigation property ownership

`NavigationWindow.ViewModelProperty` and `NavigationUserControl.ViewModelProperty`
are now registered on the non-generic base classes as `StyledProperty<object?>`.
All closed generic navigation types share those property registrations. This
makes the properties addressable from XAML and removes the AVP1002 suppressions.

`NavigationWindow<TViewModel>.ViewModel` and
`NavigationUserControl<TViewModel>.ViewModel` remain strongly typed. Ordinary
view-model assignment and `IViewFor<TViewModel>` usage require no changes.

Code that explicitly declares the property handle as
`StyledProperty<TViewModel?>` must use `StyledProperty<object?>` instead. Code
reading a value through the untyped Avalonia property API must cast the result,
or use the strongly typed `ViewModel` property. This public handle-type change
was approved for this update and should be called out in the next release notes.
