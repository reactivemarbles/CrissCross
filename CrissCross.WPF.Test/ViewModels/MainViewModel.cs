using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Input;

namespace CrissCross.WPF.Test
{
    public class MainViewModel : RxObject
    {
        public MainViewModel()
        {
            GotoFirst = ReactiveCommand.Create(() =>
            {
                this.NavigateToView<MainViewModel>("secondWindow");
                this.NavigateToView<FirstViewModel>("mainWindow");
            });
        }

        public ICommand GotoFirst { get; }

        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
        {
            Debug.WriteLine($"{DateTime.Now} Navigated To: {e.To?.Name} From: {e.From?.Name} with Host {e.HostName}");
            base.WhenNavigatedTo(e, disposables);
        }

        public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now} Navigated From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
            base.WhenNavigatedFrom(e);
        }

        public override void WhenNavigating(IViewModelNavigatingEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now} Navigating From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
            base.WhenNavigating(e);
        }
    }
}