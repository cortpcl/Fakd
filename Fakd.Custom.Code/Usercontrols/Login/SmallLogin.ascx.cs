using System;
using umbraco;
using umbraco.cms.businesslogic.member;
using System.Web.Security;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.NodeFactory;


namespace Ontranet
{
    public partial class SmallLogin : System.Web.UI.UserControl
    {

       

        protected void Page_Load(object sender, EventArgs e)
        {

            var node = new Node(1693).NiceUrl;

            if (Request.IsAuthenticated)
            {
                HyperLink lnkProfile = (HyperLink)UmbracoLoginView.FindControl("lnkProfile");
                lnkProfile.NavigateUrl = node;
            }
           
            if (!Request.IsAuthenticated) {
              
               var nodeNewUser = new Node(1418).NiceUrl;
            
                HyperLink lnkNew = (HyperLink)UmbracoLoginView.FindControl("lnkNew");
                if (lnkNew != null) {
                lnkNew.NavigateUrl = nodeNewUser;
                }

            //    HyperLink lnkForgotten = (HyperLink)UmbracoLoginView.FindControl("lnkForgotten");
            //    lnkForgotten.NavigateUrl = node;
            }

        }
        protected void OnLoggingIn(object sender, EventArgs e)
        {

            var node = new Node(1693).NiceUrl;
           
            try
            {
                System.Web.UI.WebControls.TextBox txtUserName = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("Login1").FindControl("UserName");
                txtUserName.Text = txtUserName.Text.Trim();
                var m = Member.GetMemberFromLoginName(txtUserName.Text);

             if (m.getProperty("memberAthorized").Value.ToString() != "1")
                {
                    Response.Redirect(string.Concat(node, "?m=", "notauthorised"));
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(txtUserName.Text, false);
                    Response.Redirect(string.Concat(node, "?m=", "loggedin"), false);
                }
            }
            catch
            {

                Response.Redirect(string.Concat(node, "?m=", "error"));
            }



        }
    }
}