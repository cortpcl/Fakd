using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;

using umbraco;
using umbraco.NodeFactory;

namespace Ontranet
{
    public partial class Login : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;
            if (Page.User.Identity.IsAuthenticated)
            {
                System.Web.UI.WebControls.Label lblLoggedindescription = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("lblLoggedindescription");
                lblLoggedindescription.Text = umbraco.library.GetDictionaryItem("Login.Loggedin.Description");

                System.Web.UI.WebControls.LoginStatus LoginStatus1 = (System.Web.UI.WebControls.LoginStatus)UmbracoLoginView.FindControl("LoginStatus1");
                LoginStatus1.LogoutText = umbraco.library.GetDictionaryItem("Label.Logout");

                System.Web.UI.WebControls.HyperLink lnkEditProfile = (System.Web.UI.WebControls.HyperLink)UmbracoLoginView.FindControl("lnkEditProfile");
                lnkEditProfile.Text = umbraco.library.GetDictionaryItem("Login.Edit.Profile.Link.Text");

                var node = new Node(1693).NiceUrl;
                lnkEditProfile.NavigateUrl = node;
            }
            else
            {
                System.Web.UI.WebControls.Label lblNotLoggedindescription = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("lblNotLoggedindescription");
                lblNotLoggedindescription.Text = umbraco.library.GetDictionaryItem("Login.NotLoggedin.Description");

                System.Web.UI.WebControls.RequiredFieldValidator UserNameRequired = (System.Web.UI.WebControls.RequiredFieldValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserNameRequired");
                UserNameRequired.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.Email.Empty");

                System.Web.UI.WebControls.CustomValidator custUserApproved = (System.Web.UI.WebControls.CustomValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("custUserApproved");
                custUserApproved.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Label.User.Not.Authorised");

                System.Web.UI.WebControls.RegularExpressionValidator regEmail = (System.Web.UI.WebControls.RegularExpressionValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("regEmail");
                regEmail.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.Email.Invalid");

                System.Web.UI.WebControls.RequiredFieldValidator PasswordRequired = (System.Web.UI.WebControls.RequiredFieldValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("PasswordRequired");
                PasswordRequired.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.Password.Empty");

                System.Web.UI.WebControls.Label UserNameLabel = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserNameLabel");
                UserNameLabel.Text = umbraco.library.GetDictionaryItem("Label.Email") + "*";

                System.Web.UI.WebControls.Label PasswordLabel = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("ctlLogin").FindControl("PasswordLabel");
                PasswordLabel.Text = umbraco.library.GetDictionaryItem("Label.Password") + "*";

                System.Web.UI.WebControls.Button LoginButton = (System.Web.UI.WebControls.Button)UmbracoLoginView.FindControl("ctlLogin").FindControl("LoginButton");
                LoginButton.Text = umbraco.library.GetDictionaryItem("Submit");

                System.Web.UI.WebControls.LinkButton LinkButton1 = (System.Web.UI.WebControls.LinkButton)UmbracoLoginView.FindControl("LinkButton1");
                LinkButton1.Text = umbraco.library.GetDictionaryItem("Label.Forgot.Password");
            }
        }

        protected void passrecovery(object sender, EventArgs e)
        {
            PasswordRecovery1.Visible = true;

            System.Web.UI.WebControls.Label lblPassRecoveryText = (System.Web.UI.WebControls.Label)PasswordRecovery1.UserNameTemplateContainer.FindControl("lblPassRecoveryText");
            lblPassRecoveryText.Text = umbraco.library.GetDictionaryItem("Login.Password.Recovery.Description");

            System.Web.UI.WebControls.Label PasswordRecoveryUserNameLabel = (System.Web.UI.WebControls.Label)PasswordRecovery1.UserNameTemplateContainer.FindControl("UserNameLabel");
            PasswordRecoveryUserNameLabel.Text = umbraco.library.GetDictionaryItem("Label.Email") + "*";

            System.Web.UI.WebControls.RequiredFieldValidator UserNameRequired2 = (System.Web.UI.WebControls.RequiredFieldValidator)PasswordRecovery1.UserNameTemplateContainer.FindControl("UserNameRequired2");
            UserNameRequired2.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.Email.Empty");

            System.Web.UI.WebControls.RegularExpressionValidator regEmail = (System.Web.UI.WebControls.RegularExpressionValidator)PasswordRecovery1.UserNameTemplateContainer.FindControl("regEmail2");
            regEmail.ErrorMessage = "<br/>" + umbraco.library.GetDictionaryItem("Error.Email.Invalid");

            System.Web.UI.WebControls.Button PasswordRecoveryLoginButton = (System.Web.UI.WebControls.Button)PasswordRecovery1.UserNameTemplateContainer.FindControl("SubmitButton");
            PasswordRecoveryLoginButton.Text = umbraco.library.GetDictionaryItem("Submit");

            PasswordRecovery1.SuccessText = umbraco.library.GetDictionaryItem("Login.Password.Recovery.Success.Description");
            PasswordRecovery1.GeneralFailureText = umbraco.library.GetDictionaryItem("Label.PasswordRecovery.GeneralFailureText");
            PasswordRecovery1.UserNameFailureText = umbraco.library.GetDictionaryItem("Label.PasswordRecovery.UserNameFailureText");
            PasswordRecovery1.MailDefinition.Subject = umbraco.library.GetDictionaryItem("Label.PasswordRecovery.MailDefinition.Subject");
            PasswordRecovery1.UserNameRequiredErrorMessage = umbraco.library.GetDictionaryItem("Label.PasswordRecoveryUserNameRequiredErrorMessage");
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
                    var m = Member.GetMemberFromLoginName(txtUserName.Text);

                    FormsAuthentication.Authenticate(txtUserName.Text, Password.Text);
                    if (Page.User.Identity.IsAuthenticated)
                    {
                        FormsAuthentication.SetAuthCookie(txtUserName.Text, false);
                        Response.Redirect(string.Concat(CurrentPage, "?m=", "loggedin"));
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

        public string CurrentPage
        {
            get
            {
                return Request.Url.AbsolutePath;
            }
        }

        protected void custUserApproved_ServerValidate(object source, ServerValidateEventArgs args)
        {
            System.Web.UI.WebControls.TextBox txtUserName = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserName");
            System.Web.UI.WebControls.TextBox Password = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("Password");

            txtUserName.Text = txtUserName.Text.Trim();

            if (Member.GetMemberFromLoginName(txtUserName.Text) != null)
            {
                var m = Member.GetMemberFromLoginName(txtUserName.Text);
                if (m.getProperty("memberAthorized").Value.ToString() != "1")
                {
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }
        }
    }
}