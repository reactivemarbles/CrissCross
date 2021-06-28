using ReactiveUI;
using Splat;
using System.Reactive.Disposables;

namespace CrissCross.WPF.Test.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                ViewModel ??= Locator.Current.GetService<MainViewModel>();
                this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirst).DisposeWith(d);
            });
        }
    }
}