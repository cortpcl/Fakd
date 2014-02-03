using System;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Ontranet
{


    public partial class addJobRssToDatabase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // ReadFeed("http://www.ofir.dk/RSS.aspx?querydesc=dj0xLjJ8aT0xOTIuMTY4LjE5My4yLEpPQlMsMC4xLERLLGRrX29maXJfMDIsLGRhLURLLG13b2ZpcmRhc3BwdWJsaXNoZWR8Yz1uLHBhZ2VudW1iZXIsLDEsO24scGFnZXNpemUsLDEwLHxvPW4sZnJlZXRleHQsLGRpw6Z0aXN0LDtuLGFkcmVmZXJlbmNlNCwsZGnDpnRpc3QsO24sYWRyZWZlcmVuY2U1LCxkacOmdGlzdCw7bixhZHJlZmVyZW5jZTYsLGRpw6Z0aXN0LHxsPVRydWV8eT1UcnVlfHg9RmFsc2U7MDs7fHQ9VHJ1ZTs7MDs7Ozt8cz1yYW5rMSxk");

            ReadFeed("http://www.ofir.dk/RSS.aspx?querydesc=dj0yLjB8aT0sSk9CUywwLjEsREssREtfT0ZJUl8wMiwsZGEtREssbXdvZmlyZGFzcHB1Ymxpc2hlZHxjPW4scGFnZW51bWJlciwsMSw7bixwYWdlc2l6ZSwsMTAsO24sbG9jYXRpb250ZXh0cyxBbmQsRGFubWFyayx8bz1uLGZyZWV0ZXh0LCxkacOmdGlzdCw7bixhZHJlZmVyZW5jZTQsLGRpw6Z0aXN0LDtuLGFkcmVmZXJlbmNlNSwsZGnDpnRpc3QsO24sYWRyZWZlcmVuY2U2LCxkacOmdGlzdCx8cz1UcnVlO1RydWV8eD0xO213YWRyZWZlcmVuY2UyNWRhdGU7bXdhZHJlZmVyZW5jZTI2ZGF0ZTs1MDAwfHM9cmFuazEsZA==");
        }


        /// <summary>
        /// Reads the relevant Rss feed and returns a list off RssFeedItems
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public void ReadFeed(string url)
        {
            try
            {
                //create an http request which will be used to retrieve the rss feed
                HttpWebRequest rssFeed = (HttpWebRequest)WebRequest.Create(url);

                //use a dataset to retrieve the rss feed
                using (DataSet rssData = new DataSet())
                {
                    //read the xml from the stream of the web request
                    rssData.ReadXmlSchema("http://www.thearchitect.co.uk/schemas/rss-2_0.xsd"); //avoids <atom:link> throwing an error
                    rssData.ReadXml(rssFeed.GetResponse().GetResponseStream());

                    //loop through the rss items in the dataset and populate the list of rss feed items
                    string Description, Link, Title, jobid, PublishDate;

                    foreach (DataRow dataRow in rssData.Tables["item"].Rows)
                    {
                        Description = Convert.ToString(dataRow["description"].ToString().Replace("'","''"));
                        Link = Convert.ToString(dataRow["link"]);
                        string[] _j = Link.Split('=');
                        jobid = _j[3];
                        PublishDate = Convert.ToString(dataRow["pubDate"]); // Convert.ToDateTime(dataRow["pubDate"]);
                        Title = Convert.ToString(dataRow["title"].ToString().Replace("'", "''"));
                        SaveItem(jobid, Title, PublishDate, Link, Description);

                    }
                }
            }
            catch (Exception ex)
            {
                MailAddress sfrom = new MailAddress("noreply@fakd.dk");

                //Admin mail
                MailMessage mms = new MailMessage();
                SmtpClient ss = new SmtpClient();

                mms.From = new MailAddress(sfrom.Address, "Fakd");
                mms.To.Add("domain@pcl.dk");
                mms.Subject = "Fejl: i import af jobs";
                mms.Body = ex.Message;
                ss.Send(mms);

            }


        }

        private void SaveItem(string id, string title, string dt, string link, string desc)
        {

            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd = new SqlCommand("exec dbo.AddJobs '" + id + "','" + title + "','" + dt + "','" + link + "','" + desc + "'", con);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                // m.delete();
                cmd.Dispose();
                con.Dispose();
                con.Close();
            }


        }

    }
}