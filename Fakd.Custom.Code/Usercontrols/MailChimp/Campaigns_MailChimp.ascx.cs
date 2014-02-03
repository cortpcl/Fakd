using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CookComputing.XmlRpc;
using MailChimp;

namespace Ontranet
{
    public partial class Campaigns_MailChimp : System.Web.UI.UserControl
    {


       

        protected void Page_Load(object sender, EventArgs e)
        {

             
             MailChimp.ApiWrapper api = new MailChimp.ApiWrapper();
               api.setCurrentApiKey(System.Configuration.ConfigurationManager.AppSettings["mailChimpApiKey"].ToString());


            if (Request["campaignid"] != null)
            {

                MCCampaignContent cam = api.campaignContent(Request["campaignid"].ToString());

                Literal1.Text = cam.html;
            }




        }
    }
}