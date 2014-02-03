using System;
using umbraco.cms.businesslogic.member;
using System.Data.SqlClient;
using System.Text;
using System.Net.Mail;

namespace Ontranet
{
    public partial class studentCheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd = new SqlCommand("dbo.getStudentEndDate", con);
            SqlDataReader reader;

            con.Open();

            StringBuilder sb = new StringBuilder();

            int iYear = -1;
            int iMonth = -1;
            bool blnSendMail = false;

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            using (reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int memberId = int.Parse(reader.GetSqlValue(1).ToString());
                        if (reader.GetSqlValue(4) != null)
                        {
                            string loginName = reader.GetSqlValue(4).ToString();

                            Member m = Member.GetMemberFromLoginName(loginName);
                            string sName = string.Empty;
                            if (m != null)
                            {
                                sName = m.Text;
                                string MemberFieldAlias = reader.GetSqlValue(3).ToString();

                                if (reader.GetSqlValue(5) != null )
                                {    
                                    if (reader.GetSqlValue(5).ToString().Contains("/"))
                                    {
                                        if (MemberFieldAlias.Equals("forventetFraediguddannetMember"))
                                        {
                                            string tempDT = reader.GetSqlValue(5).ToString();

                                            string[] dt = tempDT.Split('/');

                                            iYear = int.Parse(dt[1].ToString());
                                            iMonth = int.Parse(dt[0].ToString());
                                        }

                                        if ((iYear == currentYear) && (iMonth == currentMonth))
                                        {
                                            blnSendMail = true;
                                            sb.Append("<tr><td>Name: </td><td><strong>" + sName + "</strong></td></tr>");
                                            sb.Append("<tr><td>Login: </td><td>" + loginName + "</td></tr>");
                                            sb.Append("<tr><td>Slutter udd.: </td><td>" + iMonth + "/" + iYear + "</td></tr>");
                                            sb.Append("<tr><td colspan=\"2\">&nbsp;</td></tr>");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (blnSendMail)
            {
                try
                {
                    MailAddress sfrom = new MailAddress("noreply@fakd.dk");

                    MailMessage mms = new MailMessage();
                    SmtpClient ss = new SmtpClient();

                    string HTMLTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\student-end-this-year.html";
                    //  string TextTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\student-end-this-year.txt";
                    string HTMLBody = "";
                    // string TextBody = "";
                    HTMLBody = System.IO.File.ReadAllText(HTMLTemplatePath);
                    //  TextBody = System.IO.File.ReadAllText(TextTemplatePath);
                    HTMLBody = HTMLBody.Replace("<%users%>", sb.ToString());
                    HTMLBody = HTMLBody.Replace("<%Host%>", Request.Url.Host);
                    // TextBody = TextBody.Replace("<%users%>", sb.ToString());
                    // TextBody = TextBody.Replace("<%Host%>", Request.Url.Host);

                    string strEmailTo = umbraco.library.GetDictionaryItem("PliableForm.defaultToAddress");

                    mms.From = new MailAddress(sfrom.Address, "Fakd");
                    mms.To.Add(strEmailTo);
                    mms.Subject = "Studenter der slutter i denne måned";
                    mms.IsBodyHtml = true;
                    //   AlternateView plain = AlternateView.CreateAlternateViewFromString(TextBody, new System.Net.Mime.ContentType("text/plain"));
                    AlternateView html = AlternateView.CreateAlternateViewFromString(HTMLBody, new System.Net.Mime.ContentType("text/html"));
                    mms.AlternateViews.Add(html);
                    //mms.AlternateViews.Add(plain);
                    ss.Send(mms);
                }
                catch
                {
       }
            }
       }
    }
}