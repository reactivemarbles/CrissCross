using ReactiveUI;
using System.Reactive.Disposables;

namespace CrissCross.XamForms.Test.Views
{
    public partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.NavigateBack, v => v.btnBack).DisposeWith(d);
            });
        }
    }
}