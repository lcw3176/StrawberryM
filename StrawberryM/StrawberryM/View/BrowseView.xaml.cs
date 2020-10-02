using StrawberryM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StrawberryM.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowseView : ContentPage
    {
        public BrowseView()
        {
            InitializeComponent();
            BindingContext = new BrowseViewModel();
        }
    }
}