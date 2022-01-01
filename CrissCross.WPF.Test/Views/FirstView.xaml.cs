using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace CrissCross.WPF.Test.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class FirstView
    {
        public FirstView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                ViewModel ??= Locator.Current.GetService<FirstViewModel>();
                this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoMain).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
            });
        }
    }
}