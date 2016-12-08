using System;
using System.Collections.Generic;
using System.ComponentModel;
using LocalNotifications.Plugin;
using LocalNotifications.Plugin.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SaveTheBill.Model;
using SaveTheBill.Resources;
using SaveTheBill.ViewModel;
using Xamarin.Forms;

namespace SaveTheBill.Page
{
    public partial class BillDetailPage : ContentPage
    {
        private readonly Bill _bill;        
        private readonly BillDetailPageViewModel _viewModel;
        private string _mediaFile;

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
                FillDetailPage(_bill);
        }

        private async void Save_OnClicked(object sender, EventArgs e)
        {
            await _viewModel.Save_OnClicked(TitleEntry.Text, AmoundEntry.Text, DetailEntry.Text, GuaranteeSwitch.IsToggled,
                GuaranteeDatePicker.Date, BuyDateEntry.Date, LocationEntry.Text, _mediaFile, _bill);           
            await Navigation.PopAsync(true);
        }

        private void FillDetailPage(Bill bill)
        {
            TitleEntry.Text = bill.Title;
            AmoundEntry.Text = bill.Amount.ToString();
            DetailEntry.Text = bill.Detail;
            GuaranteeSwitch.IsToggled = bill.HasGuarantee;
            GuaranteeDatePicker.Date = bill.GuaranteeExpireDate;
            BuyDateEntry.Date = bill.ScanDate;
            LocationEntry.Text = bill.Location;
            ImageEntry.Source = ImageSource.FromFile(bill.ImageSource);
        }

        private void SetNotifications()
        {
            var notification = new LocalNotification
            {
                Title = NotificationResources.NotificationTitle,
                Text = NotificationResources.NotificationText,
                Id = 1,
                NotifyTime = _bill.GuaranteeExpireDate
            };
            var notifier = CrossLocalNotifications.CreateLocalNotifier();
            notifier.Notify(notification);
        }

        private void GuaranteeSwitch_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (GuaranteeDatePicker != null)
            {
                var guaranteeSwitch = (Switch) sender;

                GuaranteeDatePicker.IsEnabled = guaranteeSwitch.IsToggled;
            }
        }

        private void Image_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var image = (Image) sender;
            image.IsVisible = image.Source != null;
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
                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
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
                _mediaFile = file.Path;

                file.Dispose();
                return stream;
            });

            indicator.SetBinding(ActivityIndicator.IsRunningProperty, "IsLoading");
        }
    }
}