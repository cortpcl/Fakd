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
using System.Threading;


namespace Ontranet
{
    public partial class Subscription_MailChimp : System.Web.UI.UserControl
    {

        MailChimp.ApiWrapper api = new MailChimp.ApiWrapper();
        Hashtable arrUserResponse = new Hashtable();
        private string email;
        private string firstName;

        private string lastName;
        bool memberLoggedOn = false;
        private string _mailChimpApiKey;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated) { memberLoggedOn = true; }

            _mailChimpApiKey = System.Configuration.ConfigurationManager.AppSettings["mailChimpApiKey"].ToString();
            api.setCurrentApiKey(_mailChimpApiKey);

            //Node current = Node.GetCurrent();
            //string strHideNesletter = "";
            //if (current.GetProperty("hideNewsletter") != null) {
            //    strHideNesletter = current.GetProperty("hideNewsletter").ToString();
            //    if (strHideNesletter == "1") { pnl.Visible = false; }
            //}

     if (memberLoggedOn)
            {

                Member m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name.ToString());

                pnlInputs.Visible = true;

                if (m.getProperty("fornavnMember") != null) {
                firstName = m.getProperty("fornavnMember").Value.ToString();
                lastName = m.getProperty("efternavnMember").Value.ToString();

                email = m.Email;

                if (firstName != "") { txtFirstname.Text = firstName; }
                if (lastName != "") { txtLastName.Text = lastName; }
                if (email != "") { txtEmail.Text = email; }
                }
            }

            if (!Page.IsPostBack)
            {

                mailChimp_Lists();
                txtFirstname.Placeholder = umbraco.library.GetDictionaryItem("Label.Firstname");
                txtLastName.Placeholder = umbraco.library.GetDictionaryItem("Label.Lastname");
                txtEmail.Placeholder = umbraco.library.GetDictionaryItem("Label.Email");
                lblListHeader.Text = umbraco.library.GetDictionaryItem("Label.Newslist.Header");
                lblMainHeader.Text = umbraco.library.GetDictionaryItem("Frontpage.Title.Nyhedsbrev");
                RequiredFieldValidator1.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Email.Empty") + "<br/>";
                RegularExpressionValidator1.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Email.Invalid") + "<br/";
                CheckBoxList1_Check.ErrorMessage = umbraco.library.GetDictionaryItem("Error.NoListSelected") + "<br/>";
                RequiredFieldValidator2.ErrorMessage = umbraco.library.GetDictionaryItem("Error.FirstName.Empty") + "<br/>"; ;
                RequiredFieldValidator3.ErrorMessage = umbraco.library.GetDictionaryItem("Error.LastName.Empty") + "<br/>"; ;
            }
        }

        private void mailChimp_Lists()
        {
            MCList[] lists = api.lists();

            int countLists = lists.Count();
            bool blnSelected = false;
            if (countLists == 1) {blnSelected = true;}

        
            foreach (MCList list in lists)
            {
                ListItem i = new ListItem();
                i.Value = list.id;
                i.Text = list.name;
                if (blnSelected) {
                i.Selected = true;
                }
                CheckBoxList1.Items.Add(i);

                if (memberLoggedOn)
                {
                    MCListMember[] listsMembers = api.listMembers(list.id);

                    foreach (MCListMember listm in listsMembers)
                    {
                        if (listm.email != null && email == listm.email) i.Selected = true;
                    }
                }
            }
        }

        public void list_subscribe(string Email, string FirstName, string LastName, string listId)
        {
            string email_type = "text";

            MCMergeVar[] merges = new MCMergeVar[2];
            merges[0].tag = "FNAME";
            merges[0].val = FirstName;
            merges[1].tag = "LNAME";
            merges[1].val = LastName;

            try
            {
                api.listSubscribe(listId, Email, merges, email_type, true);
                arrUserResponse.Add(listId, GetListName(listId));
            }
            catch
            {
                lbError.Text = umbraco.library.GetDictionaryItem("Error.Subscribe");
            }
        }

        public void list_unsubscribe(string Email, string listId)
        {
            try
            {
                MCListMember[] lists = api.listMembers(listId);

                foreach (MCListMember list in lists)
                {
                    if (list.email.ToString().Equals(Email))
                    {
                        api.listUnsubscribe(listId, Email);
                        arrUserResponse.Add(listId, GetListName(listId));
                    }
                }
            }
            catch
            {
                lbError.Text = umbraco.library.GetDictionaryItem("Error.Unsubscribe");
            }
        }

        private string GetListName(string listid)
        {
            string ListName = string.Empty;

            foreach (ListItem li in CheckBoxList1.Items)
            {
                if (li.Value == listid) ListName = li.Text;
            }
            return ListName;

        }

        protected void btn_Click(object sender, EventArgs e)
        {

            if (Page.IsValid)
            {

                arrUserResponse.Clear();
                lblSubscriptionSuccess.Text = string.Empty;

                foreach (ListItem li in CheckBoxList1.Items)
                {
                    if (li.Selected)
                    {
                        if (memberLoggedOn) { list_subscribe(email, firstName, lastName, li.Value); }
                        else { list_subscribe(txtEmail.Text, txtFirstname.Text, txtLastName.Text, li.Value); }

                    }
                    else
                    {
                        if (memberLoggedOn) { list_unsubscribe(email, li.Value); }
                        else { list_unsubscribe(txtEmail.Text, li.Value); }
                    }
                }

                lblSubscriptionSuccess.Text = umbraco.library.GetDictionaryItem("Success.Subscribe.Text") + "<br/>";
                foreach (ListItem li in CheckBoxList1.Items)
                {
                    if (li.Selected)
                    {
                        lblSubscriptionSuccess.Text += " - " + GetListName(li.Value) + "<br/>";
                    }
                }
                lblSubscriptionSuccess.Text += "<br/>" + umbraco.library.GetDictionaryItem("Success.Description.Text");

                if (lblSubscriptionSuccess.Text != string.Empty)
                {
                    pnlMain.Visible = false;
                    pnlUserResponse.Visible = true;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "scrollTop", "$('html, body').animate({ scrollTop: $('.subscribe').offset().top }, 1000);", true);
   
                }
            }
        }
    }
}