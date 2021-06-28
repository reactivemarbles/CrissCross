using ReactiveUI;
using System.Reactive.Disposables;

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