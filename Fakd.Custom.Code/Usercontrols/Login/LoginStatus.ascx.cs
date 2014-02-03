using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco;
using umbraco.cms.businesslogic.member;
using System.Data;
using umbraco.DataLayer;
using umbraco.BusinessLogic;
using System.Globalization;
using System.Collections;

namespace Ontranet
{
    public partial class LoginStatus : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.User.Identity.IsAuthenticated)
            {

                Label lblLogout = UmbracoLoginView.FindControl("lblLogout") as Label;
                lblLogout.Text = umbraco.library.GetDictionaryItem("Label.Logout");

            }
            else
            {

                Label lblLogin = UmbracoLoginView.FindControl("lblLogin") as Label;
                lblLogin.Text = umbraco.library.GetDictionaryItem("Label.Login");

            }

        }
    }
}