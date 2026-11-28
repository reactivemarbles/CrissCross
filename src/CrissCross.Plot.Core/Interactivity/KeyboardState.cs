// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Tracks which keyboard keys are currently pressed.</summary>
public class KeyboardState
{
    /// <summary>The names of the keys that are currently pressed.</summary>
    private readonly HashSet<string> _pressedKeyNames = [];

    /// <summary>Gets the number of currently pressed keys.</summary>
    public int PressedKeyCount => _pressedKeyNames.Count;

    /// <summary>Gets a snapshot of the names of the currently pressed keys.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "PSH1017", Justification = "The public property returns a snapshot array for compatibility with existing callers.")]
    public string[] GetPressedKeyNames
    {
        get
        {
            string[] pressedKeyNames = new string[_pressedKeyNames.Count];
            _pressedKeyNames.CopyTo(pressedKeyNames);
            return pressedKeyNames;
        }
    }

    /// <summary>Clears the current pressed-key state.</summary>
    public void Reset() => _pressedKeyNames.Clear();

    /// <summary>Adds a key to the pressed-key state.</summary>
    /// <param name="key">The key to add.</param>
    public void Add(Key key) => _ = _pressedKeyNames.Add(key.Name);

    /// <summary>Removes a key from the pressed-key state.</summary>
    /// <param name="key">The key to remove.</param>
    public void Remove(Key key) => _ = _pressedKeyNames.Remove(key.Name);

    /// <summary>Determines whether the specified key is pressed.</summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> when the key is pressed.</returns>
    public bool IsPressed(Key key) => IsPressed(key.Name);

    /// <summary>Determines whether any of the specified keys are pressed.</summary>
    /// <param name="keys">The keys to check.</param>
    /// <returns><see langword="true"/> when at least one key is pressed.</returns>
    public bool IsPressed(IEnumerable<Key> keys)
    {
        foreach (var key in keys)
        {
            if (IsPressed(key.Name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Determines whether the key with the specified name is pressed.</summary>
    /// <param name="keyName">The key name to check.</param>
    /// <returns><see langword="true"/> when the named key is pressed.</returns>
    public bool IsPressed(string keyName) => _pressedKeyNames.Contains(keyName);

    /// <summary>Returns a string describing the currently pressed keys.</summary>
    /// <returns>A description of the current keyboard state.</returns>
    public override string ToString() => (_pressedKeyNames.Count == 0)
        ? "KeyState with 0 pressed key"
        : $"KeyState with {_pressedKeyNames.Count} pressed keys: " + string.Join(", ", _pressedKeyNames);
}
