// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using Microsoft.Extensions.Logging;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>
/// A StateTracker is an object responsible for tracking the specified properties of the specified target objects.
/// Tracking means persisting the values of the specified object properties, and restoring this data when appropriate.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Tracker"/> class.
/// Creates a new instance of the state tracker with the specified storage.
/// </remarks>
/// <param name="store">The factory that will create an IStore for each tracked object's data.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Tracker(IStore store)
{
    /// <summary>Configurations for types.</summary>
    private readonly Dictionary<Type, TrackingConfiguration> _typeConfigurations = [];

    /// <summary>Weak reference dictionary.</summary>
    private readonly ConditionalWeakTable<object, TrackingConfiguration> _configurationsDict = new();

    /// <summary>Provides the _trackedObjects member.</summary>
    private readonly List<WeakReference> _trackedObjects = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Tracker"/> class.
    /// Creates a StateTracker that uses json files in a per-user folder to store the data.
    /// </summary>
    public Tracker()
        : this(new JsonFileStore()) { }

    /// <summary>Gets or sets the object that is used to store and retrieve tracked data.</summary>
    public IStore Store { get; set; } = store;

    /// <summary>Gets or sets the logger used to report state tracking failures.</summary>
    public ILogger Logger { get; set; } = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    // todo: allow caller to configure via action argument
    /// <summary>
    /// Track a target object. This will apply any previously stored state to the target and
    /// start listening for events that indicate persisting new data is required.
    /// </summary>
    /// <param name="target">The target.</param>
    public void Track(object target) => Track(target, Configure(target));

    /// <summary>Apply any previously stored data to the target object.</summary>
    /// <param name="target">The target.</param>
    public void Apply(object target) => Configure(target).Apply(target);

    /// <summary>Apply specified defaults to the tracked properties of the target object.</summary>
    /// <param name="target">The target.</param>
    public void ApplyDefaults(object target) => Configure(target).ApplyDefaults(target);

    /// <summary>Forget any saved state for the object with the specified id.</summary>
    /// <param name="id">The identifier.</param>
    public void Forget(string id) => Store.ClearData(id);

    /// <summary>Forget any saved state for the target object.</summary>
    /// <param name="target">The target.</param>
    public void Forget(object target)
    {
        var id = Configure(target).GetStoreId(target);
        Forget(id);
    }

    /// <summary>Forget all saved state.</summary>
    public void ForgetAll() => Store.ClearAll();

    /// <summary>Gets or creates a tracking configuration for the target object.</summary>
    /// <param name="target">The target.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration Configure(object target)
    {
        ThrowHelper.ThrowIfNull(target, nameof(target));

        TrackingConfiguration config;
        if (_configurationsDict.TryGetValue(target, out var cfg))
        {
            config = cfg;
        }
        else
        {
            config = Configure(target.GetType());

            // if the object or the caller want to customize the config for this type, copy the config so they don't
            // mess with the config for the type
            if (target is ITrackingAware)
            {
                config = new(config, target.GetType());

                // allow the object to adjust the configuration
                if (target is ITrackingAware ita)
                {
                    ita.ConfigureTracking(config);
                }
            }

            _configurationsDict.Add(target, config);
        }

        return config;
    }

    /// <summary>
    /// Gets or creates a tracking configuration for the specified type. Objects of the
    /// specified type will be tracked according to the settings that are defined in the
    /// configuration object.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="request">The typed tracking request.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration<T> Configure<T>(TrackingRequest<T> request) => new(Configure(request.TargetType));

    /// <summary>
    /// Gets or creates a tracking configuration for the specified type. Objects of the
    /// specified type will be tracked according to the settings that are defined in the
    /// configuration object.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Tracking Configuration.</returns>
    public TrackingConfiguration Configure(Type type)
    {
        ThrowHelper.ThrowIfNull(type, nameof(type));

#if NET6_0_OR_GREATER
        ref var configuration = ref CollectionsMarshal.GetValueRefOrAddDefault(_typeConfigurations, type, out var exists);
        if (exists)
        {
            return configuration!;
        }
#else
        if (_typeConfigurations.TryGetValue(type, out var configuration))
        {
            return configuration;
        }
#endif

        // todo: we should make a config for each base type recursively, in case at a later point we add config for
        // a base type
        // tbd : should configurations delegate work to base classes, rather than copying their config data?
        // if a config for this exact type does not exist, copy from base type's config or create a blank one
        var baseConfig = FindConfiguration(type);
        configuration = baseConfig is not null ? new(baseConfig, type) : new(this, type);
#if !NET6_0_OR_GREATER
        _typeConfigurations.Add(type, configuration);
#endif

        return configuration!;
    }

    /// <summary>
    /// Stop tracking the target object. This prevents the persisting
    /// the target's properties when PersistAll is called on the tracker.
    /// It is used to prevent saving invalid data when the target object
    /// still exists but is in an invalid state (e.g. disposed forms).
    /// </summary>
    /// <param name="target">The target.</param>
    public void StopTracking(object target)
    {
        if (!_configurationsDict.TryGetValue(target, out var cfg))
        {
            return;
        }

        cfg.StopTracking(target);
    }

    /// <summary>Persists the tracked properties of the target object.</summary>
    /// <param name="target">The target.</param>
    public void Persist(object target) => Configure(target).Persist(target);

    /// <summary>Runs a global persist for all objects that are still alive and tracked.</summary>
    public void PersistAll()
    {
        foreach (var trackedObject in _trackedObjects)
        {
            if (!trackedObject.IsAlive || trackedObject.Target is not { } target)
            {
                continue;
            }

            if (_configurationsDict.TryGetValue(target, out var configuration))
            {
                configuration.Persist(target);
            }
        }
    }

    /// <summary>This is internal to allow TrackingConfiguration to call it so we can avoid the extra lookup (finding
    /// the configuration).</summary>
    /// <param name="target">The target object.</param>
    /// <param name="config">The config value.</param>
    internal void Track(object target, TrackingConfiguration config)
    {
        ThrowHelper.ThrowIfNull(target, nameof(target));

        // apply any previously stored data
        config.Apply(target);

        // listen for persist requests
        config.StartTracking(target);

        // add to list of objects to track
        _trackedObjects.Add(new(target));
    }

    /// <summary>Provides the RemoveFromList member.</summary>
    /// <param name="target">The target object.</param>
    internal void RemoveFromList(object target)
    {
        _ = _configurationsDict.Remove(target);
        _ = _trackedObjects.RemoveAll(t => t.Target == target);
    }

    /// <summary>Provides the FindConfiguration member.</summary>
    /// <param name="type">The type value.</param>
    /// <returns>The result.</returns>
    private TrackingConfiguration? FindConfiguration(Type type)
    {
        if (_typeConfigurations.TryGetValue(type, out var value))
        {
            return value;
        }

        return type == typeof(object) || type.BaseType is null ? null : FindConfiguration(type.BaseType);
    }
}
