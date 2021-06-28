using ReactiveUI;
using System.Reactive.Disposables;

namespace CrissCross.WPF.Test
{
    /// <summary>
    /// Interaction logic for SecondWindow.xaml
    /// </summary>
    public partial class SecondWindow : IUseNavigation
    {
        public SecondWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.NavigateToView<FirstViewModel>();
                NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack).DisposeWith(d);
            });
        }
    }
}