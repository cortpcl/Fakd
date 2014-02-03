using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;

using umbraco;
using umbraco.presentation.nodeFactory;

namespace Ontranet
{
    public partial class LoginNetworkGroups : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (!Page.User.Identity.IsAuthenticated)
            {
                System.Web.UI.WebControls.Label lblNotLoggedindescription = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("lblNotLoggedindescription");
                lblNotLoggedindescription.Text = umbraco.library.GetDictionaryItem("Login.NotLoggedin.Description");

                System.Web.UI.WebControls.RequiredFieldValidator UserNameRequired = (System.Web.UI.WebControls.RequiredFieldValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserNameRequired");
                UserNameRequired.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.UserName.Empty");

                System.Web.UI.WebControls.RequiredFieldValidator PasswordRequired = (System.Web.UI.WebControls.RequiredFieldValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("PasswordRequired");
                PasswordRequired.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.Password.Empty");

                System.Web.UI.WebControls.Label UserNameLabel = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserNameLabel");
                UserNameLabel.Text = umbraco.library.GetDictionaryItem("Label.UserName") + "*";

                System.Web.UI.WebControls.Label PasswordLabel = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("ctlLogin").FindControl("PasswordLabel");
                PasswordLabel.Text = umbraco.library.GetDictionaryItem("Label.Password") + "*";

                System.Web.UI.WebControls.Button LoginButton = (System.Web.UI.WebControls.Button)UmbracoLoginView.FindControl("ctlLogin").FindControl("LoginButton");
                LoginButton.Text = umbraco.library.GetDictionaryItem("Submit");


            }
        }

   

        protected void OnLoginError(object sender, EventArgs e)
        {
            lblError.Visible = true;
            lblError.Text = umbraco.library.GetDictionaryItem("Login.Error");
        }

        protected void OnLoggingIn(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    System.Web.UI.WebControls.TextBox txtUserName = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserName");
                    System.Web.UI.WebControls.TextBox Password = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("Password");
                    txtUserName.Text = txtUserName.Text.Trim();
                  
              

                    FormsAuthentication.Authenticate(txtUserName.Text, Password.Text);
                    if (Page.User.Identity.IsAuthenticated)
                    {
                        FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
                      
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.Text = umbraco.library.GetDictionaryItem("Login.Error");
                    }
                }
                catch
                {
                    lblError.Visible = true;
                    lblError.Text = umbraco.library.GetDictionaryItem("Login.Error");
                }
            }
        }

        

      
    }
}