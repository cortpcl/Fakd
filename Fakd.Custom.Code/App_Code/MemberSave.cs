using System;
using System.Net.Mail;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;

namespace Ontranet
{
    public class MemberSave : ApplicationBase
    {
        public MemberSave()
        {
            Member.AfterSave += new Member.SaveEventHandler(MemberAfterSave);
        }

        private void MemberAfterSave(Member sender, umbraco.cms.businesslogic.SaveEventArgs asgs)
        {

            //sender mail ToString member, when they are approveed, nut only once   
            try
            {
                string dateMemberAthorized = sender.getProperty("dateMemberAthorized").Value.ToString();
                string memberAthorized = sender.getProperty("memberAthorized").Value.ToString();

                if (memberAthorized == "1" && dateMemberAthorized == string.Empty)
                {
                    MailAddress sfrom = new MailAddress("noreply@fakd.dk");

                    MailMessage mms = new MailMessage();
                    SmtpClient ss = new SmtpClient();

                    string HTMLTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\user-profile-approved.html";
                    string HTMLBody = "";

                    HTMLBody = System.IO.File.ReadAllText(HTMLTemplatePath);
                    HTMLBody = HTMLBody.Replace("<%UserName%>", sender.Email);
                    HTMLBody = HTMLBody.Replace("<%Host%>", HttpContext.Current.Request.Url.Host);
             

                    mms.From = new MailAddress(sfrom.Address, "Fakd");
                    mms.To.Add(sender.Email);
                    mms.Subject = "Dit medlemsskab nu er godkendt på fakd.dk";
                    AlternateView html = AlternateView.CreateAlternateViewFromString(HTMLBody, new System.Net.Mime.ContentType("text/html"));
                    mms.AlternateViews.Add(html);
                    ss.Send(mms);

                    sender.getProperty("dateMemberAthorized").Value = DateTime.Now.ToString();
                    sender.Save();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
        }
    }
}