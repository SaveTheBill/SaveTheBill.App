using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Plugin.Messaging;
using SaveTheBill.Infrastructure;
using SaveTheBill.Model;
using SaveTheBill.Resources;
using Xamarin.Forms;

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

        public async Task Save_OnClicked(string title, string ammount, string detail, bool hasGuarantee,
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

            double cost;
            double.TryParse(ammount, out cost);

            var bill = new Bill
            {
                Title = title,
                Amount = cost,
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
        }

        public async Task RemoveItemFromListAsync(Bill bill)
        {
            var list = JsonConvert.DeserializeObject<IEnumerable<Bill>>(await _fileSaver.ReadContentFromLocalFileAsync());
            if ((list != null) && list.Any())
            {
                foreach (var listItem in list)
                    _billList.Add(listItem);
            }

            var item = _billList.SingleOrDefault(x => x.Id == bill.Id);
            if (item != null) _billList.Remove(item);

            await _fileSaver.SaveContentToLocalFileAsync(_billList);
        }
    }
}