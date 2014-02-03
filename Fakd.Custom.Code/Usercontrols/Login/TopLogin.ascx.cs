using System;
using umbraco.cms.businesslogic.member;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using MyControls.Html5;
using umbraco.NodeFactory;

namespace Ontranet
{
    public partial class TopLogin : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Visible = false;


            if (Page.User.Identity.IsAuthenticated)
            {
                System.Web.UI.WebControls.HyperLink lnkEditProfile = (System.Web.UI.WebControls.HyperLink)UmbracoLoginView.FindControl("lnkEditProfile");
                lnkEditProfile.Text = umbraco.library.GetDictionaryItem("Login.Edit.Profile.Link.Text");

                var node = new Node(1693).NiceUrl;
                lnkEditProfile.NavigateUrl = node;

                System.Web.UI.WebControls.LoginStatus LoginStatus1 = (System.Web.UI.WebControls.LoginStatus)UmbracoLoginView.FindControl("LoginStatus1");
                LoginStatus1.LogoutText = umbraco.library.GetDictionaryItem("Label.Logout");
            }
            else
            {

                System.Web.UI.WebControls.Label lblLoginTitle = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("lblLoginTitle");
                lblLoginTitle.Text = umbraco.library.GetDictionaryItem("Label.Login.Title");

                Html5TextBox UserName = (Html5TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserName");
                UserName.Placeholder = umbraco.library.GetDictionaryItem("Label.Email");

                Html5TextBox Password = (Html5TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("Password");
                Password.Placeholder = umbraco.library.GetDictionaryItem("Label.Password");

                System.Web.UI.WebControls.RequiredFieldValidator UserNameRequired = (System.Web.UI.WebControls.RequiredFieldValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserNameRequired");
                UserNameRequired.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Email.Empty");

                System.Web.UI.WebControls.CustomValidator custUserApproved = (System.Web.UI.WebControls.CustomValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("custUserApproved");
                custUserApproved.ErrorMessage = umbraco.library.GetDictionaryItem("Label.User.Not.Authorised");


                System.Web.UI.WebControls.RegularExpressionValidator regEmail = (System.Web.UI.WebControls.RegularExpressionValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("regEmail");
                regEmail.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Email.Invalid");

                System.Web.UI.WebControls.RequiredFieldValidator PasswordRequired = (System.Web.UI.WebControls.RequiredFieldValidator)UmbracoLoginView.FindControl("ctlLogin").FindControl("PasswordRequired");
                PasswordRequired.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Password.Empty");


                System.Web.UI.WebControls.Button LoginButton = (System.Web.UI.WebControls.Button)UmbracoLoginView.FindControl("ctlLogin").FindControl("LoginButton");
                LoginButton.Text = umbraco.library.GetDictionaryItem("Submit");

                System.Web.UI.WebControls.HyperLink lnkNew = (System.Web.UI.WebControls.HyperLink)UmbracoLoginView.FindControl("lnkNew");
                var lnkNewnode = new Node(1418).NiceUrl;
                lnkNew.NavigateUrl = lnkNewnode;
                lnkNew.Text = umbraco.library.GetDictionaryItem("Label.New.User");

                System.Web.UI.WebControls.HyperLink lnkForgotPassword = (System.Web.UI.WebControls.HyperLink)UmbracoLoginView.FindControl("lnkForgotPassword");
                var lnkForgotPasswordnode = new Node(1693).NiceUrl;
                lnkForgotPassword.NavigateUrl = lnkForgotPasswordnode;
                lnkForgotPassword.Text = umbraco.library.GetDictionaryItem("Label.Forgot.Password");

            }

        }
        protected void OnLoginError(object sender, EventArgs e)
        {
            lblError.Visible = true;
            lblError.Text = umbraco.library.GetDictionaryItem("Login.Error");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "timeout", "$('#header').addClass('active');$('#header').addClass('active');", true);

        }

        protected void OnLoggingIn(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    System.Web.UI.WebControls.TextBox txtUserName = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("UserName");
                    System.Web.UI.WebControls.TextBox Password = (System.Web.UI.WebControls.TextBox)UmbracoLoginView.FindControl("ctlLogin").FindControl("Password");

                    FormsAuthentication.Authenticate(txtUserName.Text, Password.Text);
                    if (Page.User.Identity.IsAuthenticated)
                    {
                        FormsAuthentication.SetAuthCookie(txtUserName.Text, false);

                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.Text = umbraco.library.GetDictionaryItem("Login.Error");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "timeout", "$('#header').addClass('active');$('#header').addClass('active');", true);
                    }

                }
                catch
                {
                    lblError.Visible = true;
                    lblError.Text = umbraco.library.GetDictionaryItem("Login.Error");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "timeout", "$('#header').addClass('active');$('#header').addClass('active');", true);
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "timeout", "$('#header').addClass('active');$('#header').addClass('active');", true);

                }
                else
                {
                    args.IsValid = true;
                }
            }



        }

    }
}