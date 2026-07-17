// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the CancellationExtensions member.</summary>
internal static class CancellationExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    extension(CancellationToken cancellationToken)
    {
        /// <summary>Provides the WhenCanceled member.</summary>
        /// <returns>The result.</returns>
        public Task WhenCanceled()
        {
            var tcs = new TaskCompletionSource<int>();
            var registration = default(CancellationTokenRegistration);
            registration = cancellationToken.Register(
                o =>
                {
                    if (o is TaskCompletionSource<int> tcs)
                    {
                        _ = tcs.TrySetCanceled();
                    }

                    // ReSharper disable once AccessToModifiedClosure
                    registration.Dispose();
                },
                tcs);
            return tcs.Task;
        }
    }

    /// <summary>Provides extension members.</summary>
    /// <param name="task">The task value.</param>
    extension(Task task)
    {
        /// <summary>Provides the WithCancellationToken member.</summary>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public async Task WithCancellationToken(CancellationToken cancellationToken) =>
            await Task.WhenAny(task, cancellationToken.WhenCanceled());
    }

    /// <summary>Provides extension members.</summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="task">The task value.</param>
    extension<T>(Task<T> task)
    {
        /// <summary>Provides the WithCancellationToken member.</summary>
        /// <param name="cancellationToken">The cancellationToken value.</param>
        /// <returns>The result.</returns>
        public async Task<T> WithCancellationToken(CancellationToken cancellationToken)
        {
            var firstTaskToFinish = await Task.WhenAny(task, cancellationToken.WhenCanceled());
            if (firstTaskToFinish == task)
            {
                return await task;
            }

            await firstTaskToFinish;

            // Will never be reached because the previous statement will throw, but necessary to satisfy the compiler
            throw new OperationCanceledException(cancellationToken);
        }
    }
}
