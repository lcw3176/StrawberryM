using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StrawberryM.Model;
using StrawberryM.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StrawberryM.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NowPlayingView : ContentPage
    {
        public NowPlayingView()
        {
            InitializeComponent();
            BindingContext = NowPlayingViewModel.GetInstance();
        }
    }
}