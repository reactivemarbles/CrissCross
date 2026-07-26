# CrissCross Navigation Modes

This executable demonstrates the platform-neutral, bidirectional navigation API in `CrissCross` without coupling it to a UI framework.

Run it from the repository root:

```powershell
dotnet run --project src/samples/NavigationModes/CrissCross.NavigationModes.Example.csproj -c Release
```

The sample covers:

- Typed ViewModel-first and View-first resolution.
- Interface and runtime lookup keys.
- Contracts, host metadata, and strongly typed parameters.
- Supplied ViewModel and View identity preservation.
- Cancellation before factory invocation.
- Platform-neutral journal record, back, forward, and clear transitions.

Platform hosts can consume the resulting `NavigationResolution` and apply it to their own routed view host. The registry itself deliberately creates an observable resolution and does not mutate UI state.
