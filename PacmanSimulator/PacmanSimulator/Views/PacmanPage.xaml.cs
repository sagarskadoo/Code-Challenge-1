using System.ComponentModel;
using Xamarin.Forms;

namespace PacmanSimulator.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class PacmanPage : ContentPage
    {
        public PacmanPage()
        {
            InitializeComponent();
            var viewModel = new PacmanViewModel();

            // Setting up the ViewModel.
            BindingContext = viewModel;
        }
    }
}
