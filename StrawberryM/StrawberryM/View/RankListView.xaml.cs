using StrawberryM.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StrawberryM.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RankView : ContentPage
    {
        public RankView()
        {
            InitializeComponent();
            BindingContext = new RankListViewModel();
        }
    }
}