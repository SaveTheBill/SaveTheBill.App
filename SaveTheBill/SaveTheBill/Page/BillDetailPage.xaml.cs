using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SaveTheBill.Infrastructure;
using SaveTheBill.Model;
using SaveTheBill.ViewModel;
using Xamarin.Forms;

namespace SaveTheBill.Page
{
    public partial class BillDetailPage : ContentPage
    {
        private readonly BillDetailPageViewModel _viewModel;        
        private readonly IList<Bill> _billList;
        private readonly FileSaver _fileSaver;
        private readonly Bill _bill;
        public BillDetailPage(Bill bill = null)
        {
            _bill = bill;
            _viewModel = new BillDetailPageViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_bill != null)
            {
                TitleEntry.Text = _bill.Title;
            }

        }

        private void Save_OnClicked(object sender, EventArgs e)
        {
            _viewModel.Save_OnClicked(TitleEntry.Text, AmoundEntry.Text, DetailEntry.Text, GuaranteeSwitch.IsToggled, GuaranteeDatePicker.Date, BuyDateEntry.Date, LocationEntry.Text);
            Navigation.PushAsync(new HomePage());
        }

        private void GuaranteeSwitch_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (GuaranteeDatePicker != null)
            {
                var guaranteeSwitch = (Switch)sender;

                GuaranteeDatePicker.IsEnabled = guaranteeSwitch.IsToggled;
            }
        }

        private void Image_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var image = (Image) sender;
            if (image.Source != null)
                image.IsVisible = true;
            else
                image.IsVisible = false;
        }

        private async void AddPhoto_OnClicked(object sender, EventArgs e)
        {
            MediaFile file;
            var indicator = new ActivityIndicator {Color = new Color(.5)};
                        
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Fehler", "Kamera nicht verfügbar.", "OK");
                return;
            }

            var action = await DisplayActionSheet("Wählen Sie eine Quelle", "Abbrechen", null, "Kamera", "Gallerie");

            if (action.Equals("Gallerie"))
            {
                file = await CrossMedia.Current.PickPhotoAsync();

                indicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");

                if (file == null)
                    return;
            }
            else
            {
                file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "LocalData",
                    Name = "bill_" + DateTime.Now + ".jpg"
                });

                indicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");

                if (file == null)
                    return;
            }
            ImageEntry.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });

            indicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");
        }
    }
}
