using ReactiveUI;

namespace CrissCross.WPF.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.NavigateToView<MainViewModel>();
            });
        }
    }
}