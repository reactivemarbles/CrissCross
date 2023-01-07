// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross
{
    /// <summary>
    /// interface for RxBase.
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public interface IRxObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, INotifiyRoutableViewModel, ICancelable
    {
    }
}