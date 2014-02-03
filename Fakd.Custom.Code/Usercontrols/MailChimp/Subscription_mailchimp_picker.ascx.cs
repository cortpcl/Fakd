using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CookComputing.XmlRpc;
using MailChimp;
using System.Collections;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.propertytype;

using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.property;
using AjaxControlToolkit;
using umbraco.cms.businesslogic;

namespace Ontranet
{
    public partial class Subscription_mailchimp_picker : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {
        MailChimp.ApiWrapper api = new MailChimp.ApiWrapper();
        Hashtable arrUserResponse = new Hashtable();
        public string umbracoValue;

        private string email;
        private string firstName;

        private string lastName;

        private void Page_Init(object sender, EventArgs args)
        {



            int memberId = int.Parse(Request["id"]);

            Member m = new Member(memberId);


            firstName = m.getProperty("fornavnMember").Value.ToString();
            lastName = m.getProperty("efternavnMember").Value.ToString();


            email = m.Email;

            CheckBoxList2.Items.Clear();

            api.setCurrentApiKey(System.Configuration.ConfigurationManager.AppSettings["mailChimpApiKey"].ToString());

            MCList[] lists = api.lists();
            foreach (MCList list in lists)
            {
                ListItem i = new ListItem();
                i.Value = list.id;
                i.Text = list.name;
                CheckBoxList2.Items.Add(i);


                MCListMember[] listsMembers = api.listMembers(list.id);

                foreach (MCListMember listm in listsMembers)
                {

                    if (listm.email != null && email == listm.email) i.Selected = true;

                }

            }
         }




        protected void Page_Load(object sender, EventArgs e)
        {






            if (Page.IsPostBack)
            {


            }

       


        }


      

       
      
        public object value
        {
            get
            {
                return umbracoValue;
            }
            set
            {
                umbracoValue = value.ToString();
            }
        }
    }
}