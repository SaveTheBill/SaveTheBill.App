using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaveTheBill.Infrastructure;
using SaveTheBill.Model;
using Xamarin.Forms;

namespace SaveTheBill.Page
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly FileSaver _fileSaver;
        private ObservableCollection<Bill> _billList;
        private bool _noEntries = true;

        public HomePage()
        {
            _fileSaver = new FileSaver();
            BillList = new ObservableCollection<Bill>();
            BindingContext = this;
            InitializeComponent();
        }


        public ObservableCollection<Bill> BillList
        {
            get { return _billList; }
            set
            {
                _billList = value;                
                RaisePropertyChanged();
            }
        }

        public bool NoEntries
        {
            get { return _noEntries; }
            set
            {
                if (_noEntries == value) return;
                _noEntries = value;
                OnPropertyChanged();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (Device.OS == TargetPlatform.iOS)
            {
                ToolbarItems.Add(null);
            }

            await FillListViewAsync();
            StackLayoutNoEntries.IsVisible = _billList.Count == 0;
        }

        public async Task FillListViewAsync()
        {
            var json = await _fileSaver.ReadContentFromLocalFileAsync();

            if (json.Length > 0)
                BillList = new ObservableCollection<Bill>(JsonConvert.DeserializeObject<IEnumerable<Bill>>(json));
        }

        public async void OnDeleteItem(object sender, EventArgs e)
        {
            var mi = (MenuItem) sender;
            var item = (Group.BillItem) mi.CommandParameter;
            item.Title = "(null)";
            await _fileSaver.SaveContentToLocalFileAsync(BillList);
        }

        private void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (ListView) sender;
            var item = ((ListView) sender).SelectedItem;

            if (item == null) return;

            Navigation.PushAsync(new BillDetailPage((Bill)item));
            listView.SelectedItem = null;

        }

        private void AddGroupItem_OnClicked(object sender, EventArgs e)
        {
            var result = (ToolbarItem) sender;
            if (result != null)
                Navigation.PushAsync(new BillDetailPage());
        }

        #region INotifyPropertyChanged

        public new event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

