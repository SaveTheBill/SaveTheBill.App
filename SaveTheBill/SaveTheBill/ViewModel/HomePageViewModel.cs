using Plugin.Messaging;
using SaveTheBill.Model;
using SaveTheBill.Resources;

namespace SaveTheBill.ViewModel
{
    public class HomePageViewModel
    {
        public void SendEmail(Bill bill)
        {
            var emailMessenger = CrossMessaging.Current.EmailMessenger;
            if (emailMessenger.CanSendEmail)
            {
                var billText = "<b>Beschreibung: </b>" + bill.Title + "<br/>" +
                               "<br/><b>Betrag: <b/> " + "<b>Garantie läuft ab: </b>" +
                               (bill.HasGuarantee ? bill.GuaranteeExpireDate.ToString() : "Keine Garantie erfasst") +
                               bill.Amount + "<br/><b>Kaufort: <b/> " +
                               bill.Location + "<br/><b>Eingescannt am: <b/> " + bill.ScanDate;

                if (bill.ImageSource == null)
                {
                    var email = new EmailMessageBuilder()
                        .Subject("SaveTheBill Quittung Nummer #" + bill.Id)
                        .BodyAsHtml(EmailResources.EmailStart + "<br/><br/>" + EmailResources.EmailText + "<br/><br/>" +
                                    billText + "<br/><br/>" +
                                    EmailResources.EmailEnd + "<br/>" + EmailResources.Signature)
                        .Build();
                    emailMessenger.SendEmail(email);
                }
                else
                {
                    var email = new EmailMessageBuilder()
                        .Subject("SaveTheBill Quittung Nummer #" + bill.Id)
                        .BodyAsHtml(EmailResources.EmailStart + "<br/><br/>" + EmailResources.EmailText + "<br/><br/>" +
                                    billText + "<br/><br/>" +
                                    EmailResources.EmailEnd + "<br/>" + EmailResources.Signature)
                        .WithAttachment(bill.ImageSource, "image/jpeg")
                        .Build();
                    emailMessenger.SendEmail(email);
                }
            }
        }
    }
}