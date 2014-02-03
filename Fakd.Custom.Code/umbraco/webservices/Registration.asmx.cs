using System.ComponentModel;
using System.Web.Services;
using System.Net.Mail;
using System.Globalization;
using System.Web.Script.Services;




namespace Ontranet
{


    /// <summary>
    /// Summary description for Registration
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService] // To allow this Web Service to be called from script, (using ASP.NET AJAX or JQuery), and return json formatted data.
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Registration : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendRegistration(string MemberNumber,
			string Name,
			string Phone,
            string Email, string EventName, string EventDate)
        {
         

         string strMemberNumber = "";
			string strName = "";
			string strPhone = "";
            string strEmail = "";
            string strEventName = "";
            string strEventDate = "";



            if (MemberNumber != "") { strMemberNumber = MemberNumber.TrimEnd(); }
            if (Name != "") {strName = Name;}
            if (Phone != "") {strPhone = Phone;}
            if (Email != "") {strEmail = Email;}
            if (EventName != "") { strEventName = EventName; }

            if (EventDate != "") { strEventDate = EventDate; }


            string strSubject = "Ny registrering"; //GetDictionaryItemByCulture("contactGroupSubject", GetLanguageIdFromNodeId(icurrentDoc));
            string strfromAddress = "post@diaetist.dk"; //GetDictionaryItemByCulture("PliableForm.fromAddress", GetLanguageIdFromNodeId(icurrentDoc));

            if (strEmail != string.Empty)
            {
                try
                {
                    var cultureInfo = new CultureInfo("da-DK");

                    MailAddress sfrom = new MailAddress(strfromAddress);

                    string HTMLTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\new-registration.html";

            
                    string HTMLBody = "";

                    HTMLBody = System.IO.File.ReadAllText(HTMLTemplatePath);

                 
                    if (strMemberNumber != "") { HTMLBody = HTMLBody.Replace("<%MemberNumber%>", strMemberNumber); }
                    
                    HTMLBody = HTMLBody.Replace("<%Name%>", strName);
                    HTMLBody = HTMLBody.Replace("<%Email%>", strEmail);
                    HTMLBody = HTMLBody.Replace("<%Phone%>", strPhone);
                    HTMLBody = HTMLBody.Replace("<%EventName%>", strEventName);
                    HTMLBody = HTMLBody.Replace("<%EventDate%>", strEventDate);
                   

                    MailMessage mmsClient = new MailMessage();
                    mmsClient.BodyEncoding = System.Text.Encoding.GetEncoding("windows-1255");
                    SmtpClient ssClient = new SmtpClient();

                    mmsClient.From = new MailAddress(sfrom.Address);
                    mmsClient.To.Add(strEmail);
                    mmsClient.Subject = strSubject;
                    AlternateView htmlClient = AlternateView.CreateAlternateViewFromString(HTMLBody, mmsClient.SubjectEncoding = System.Text.Encoding.Default, "text/html");

                    mmsClient.AlternateViews.Add(htmlClient);
                    ssClient.Send(mmsClient);

                }
                catch
                {
                }
            }
        }
    }
}
