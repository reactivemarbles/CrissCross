using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace CrissCross
{
    public class ShellViewModel<TViewModel> : ShellContent, IUseHostedNavigation, IActivatableView
        where TViewModel : class, IRxObject, new()
    {
        public static readonly BindableProperty ContractProperty = BindableProperty.Create(
            nameof(Contract),
            typeof(string),
            typeof(ShellViewModel<TViewModel>),
            null);

        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ShellViewModel<TViewModel>),
            default(TViewModel),
            BindingMode.Default,
            propertyChanged: ViewModelChanged);

        private static void ViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShellViewModel<TViewModel> svm)
            {
                var view = svm.ViewLocator?.ResolveView(newValue as TViewModel, svm.Contract);
                svm.ContentTemplate = new DataTemplate(() => view);
            }
        }

        public ShellViewModel()
        {
            ViewLocator = Locator.Current.GetService<IViewLocator>();
            ViewModel = new TViewModel();
        }

        public IRxObject ViewModel
        {
            get => (IRxObject)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public string Contract
        {
            get => (string)GetValue(ContractProperty);
            set => SetValue(ContractProperty, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator? ViewLocator { get; set; }
    }
}