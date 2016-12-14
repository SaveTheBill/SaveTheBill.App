using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalNotifications.Plugin;
using LocalNotifications.Plugin.Abstractions;
using Newtonsoft.Json;
using SaveTheBill.Infrastructure;
using SaveTheBill.Model;
using SaveTheBill.Resources;

namespace SaveTheBill.ViewModel
{
    public class BillDetailPageViewModel
    {
        private readonly IList<Bill> _billList;
        private readonly FileSaver _fileSaver;

        public BillDetailPageViewModel()
        {
            _billList = new List<Bill>();
            _fileSaver = new FileSaver();
        }

        public async Task Save_OnClicked(string title, string ammount, string currenyValue, int currencyIndex, string detail, bool hasGuarantee,
            DateTime guaranteeDatePicker, DateTime buyDate, string location, string mediaFile, Bill oldBill = null)
        {
            var list = JsonConvert.DeserializeObject<IEnumerable<Bill>>(await _fileSaver.ReadContentFromLocalFileAsync());
            var id = 1;
            if ((list != null) && list.Any())
            {
                foreach (var item in list)
                    _billList.Add(item);
                id = list.Count() + 1;
            }            

            var bill = new Bill
            {
                Title = title,
                Amount = ammount,
                Currency = new Currency
                {
                    CurrencyIndex = currencyIndex,
                    CurrencyValue = currenyValue
                },
                Detail = detail,
                HasGuarantee = hasGuarantee,
                GuaranteeExpireDate = guaranteeDatePicker,
                ScanDate = buyDate,
                Location = location,
                ImageSource = mediaFile
            };

            if (oldBill != null)
            {
                bill.Id = oldBill.Id;
                await RemoveItemFromListAsync(bill);
            }
            else
            {
                bill.Id = id;
            }

            _billList.Add(bill);

            await _fileSaver.SaveContentToLocalFileAsync(_billList);

            if (bill.HasGuarantee)
            {
                SetLocalNotification(bill);
            }
        }

        public void SetLocalNotification(Bill bill)
        {
            var not = CrossLocalNotifications.CreateLocalNotifier();           

            not.Notify(new LocalNotification
            {
                Id = bill.Id,
                NotifyTime = bill.GuaranteeExpireDate,
                Text = "Ihre Garantie zum Produkt " + bill.Title + "läuft ab",
                Title = "Garantie läuft in " + (bill.GuaranteeExpireDate - DateTime.Now) + "ab"
            });
        }

        public async Task RemoveItemFromListAsync(Bill bill)
        {
            var list = JsonConvert.DeserializeObject<IEnumerable<Bill>>(await _fileSaver.ReadContentFromLocalFileAsync());
            if ((list != null) && list.Any())
                foreach (var listItem in list)
                    _billList.Add(listItem);
            var item = _billList.First(x => x.Id == bill.Id);

            if (item != null)
            {
                _billList.Remove(item);
                var not = CrossLocalNotifications.CreateLocalNotifier();
                not.Cancel(item.Id);
            }

            await _fileSaver.SaveContentToLocalFileAsync(_billList);
                     

            _billList.Clear();
        }
    }
}