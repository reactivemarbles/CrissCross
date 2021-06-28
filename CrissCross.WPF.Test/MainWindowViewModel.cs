using CrissCross.WPF.Test.Views;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.Test
{
    public class MainWindowViewModel : RxObject
    {
        public MainWindowViewModel()
        {
            Locator.CurrentMutable.RegisterConstant<MainViewModel>(new());
            Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());

            Locator.CurrentMutable.RegisterConstant<FirstViewModel>(new());
            Locator.CurrentMutable.Register<IViewFor<FirstViewModel>>(() => new FirstView());
            var s = new SecondWindow();
            s.Show();
        }
    }
}