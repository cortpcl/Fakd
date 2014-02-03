using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.propertytype;
using umbraco.interfaces;
using System.Collections;
using umbraco.cms.businesslogic.property;
using System.Web.Security;
using MailChimp;
using System.Net.Mail;


namespace Ontranet
{

    public partial class RegisterMember : System.Web.UI.UserControl
    {
        private string _beforeRow = "<tr>";
        private string _afterRow = "</tr>";
        private string _beforeCell = "<td  class=\"cellcontent\">";
        private string _beforeCellReq = "<td  class=\"cellcontentReq\">";
        private string _beforeCellSmallReq = "<td class=\"cellsmallreq\">";
        private string _beforeCellSmall = "<td class=\"cellsmall\">";
        private string _afterCell = "</td>";
        private string _beforeCellHeader = "<div class=\"signupheader\">";
        private string _endCellHeader = "</div>";
        private string _beforeCellRight = "<td class=\"firstcellsignup\">";
        private string _startTable = "<table class=\"osMemberProfile\">";
        private string _endTable = "</table>";
        private ArrayList _dataFields = new ArrayList();
        private int _redirect;
        private string _memberTypeAlias;
        private string _membergroup;
        private bool _displaywizardsidebar;
        private Wizard wzrd1;
        private string _adminemailaddress;
        private string _mailChimpApiKey;
        private int items = 3;

        MailChimp.ApiWrapper api = new MailChimp.ApiWrapper();

        Label HeaderLabel = new Label();
        CheckBoxList newsletters = new CheckBoxList();
        Panel pnlResp = new Panel();

        public int RedirectOnSuccess
        {
            get { return _redirect; }
            set { _redirect = value; }
        }

        public string MemberTypeAlias
        {
            get { return _memberTypeAlias; }
            set { _memberTypeAlias = value; }
        }

        public bool DisplayWizardSideBay
        {
            get { return _displaywizardsidebar; }
            set { _displaywizardsidebar = value; }
        }

        public string AddToMemberGroup
        {
            get { return _membergroup; }
            set { _membergroup = value; }
        }
        public string AdminEmailAddress
        {
            get { return _adminemailaddress; }
            set { _adminemailaddress = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Show notification is user allready is loggedin, logut to create a new user
                System.Web.UI.WebControls.LoginStatus LoginStatus1 = (System.Web.UI.WebControls.LoginStatus)UmbracoLoginView.FindControl("LoginStatus1");
                LoginStatus1.LogoutText = umbraco.library.GetDictionaryItem("Label.Logout");

                System.Web.UI.WebControls.Label lblLoggedIn = (System.Web.UI.WebControls.Label)UmbracoLoginView.FindControl("lblLoggedIn");
                lblLoggedIn.Text = umbraco.library.GetDictionaryItem("Register.Loggedin.Description");
            }
            else
            {
                _mailChimpApiKey = System.Configuration.ConfigurationManager.AppSettings["mailChimpApiKey"].ToString();
                api.setCurrentApiKey(_mailChimpApiKey);

                ////get the properties of the member type
                MemberType mt = MemberType.GetByAlias(_memberTypeAlias);
                IEnumerable<PropertyType> ptypes;
                ptypes = mt.PropertyTypes;

                ptypes = ptypes.Where(g => g.TabId != 0);

                #region Wizard

                wzrd1 = new Wizard();
                wzrd1.CssClass = "RegisterForm";
                wzrd1.SideBarStyle.CssClass = "sidebar";
                wzrd1.StepStyle.CssClass = "stepcontent";
                wzrd1.SkipLinkText = " ";

                wzrd1.NavigationStyle.HorizontalAlign = HorizontalAlign.Left;

                wzrd1.StepNextButtonText = "Næste";
                wzrd1.StepPreviousButtonText = "Forrige";
                wzrd1.NavigationStyle.CssClass = "buttons";
                wzrd1.StartNextButtonText = "Næste";

                wzrd1.FinishPreviousButtonText = "Forrige";
                wzrd1.FinishCompleteButtonText = "Færdig - Opret mig";
                wzrd1.StepStyle.CssClass = "stepcontent";

                wzrd1.DisplaySideBar = _displaywizardsidebar;

                WizardStep ws = new WizardStep();
                ws.ID = "999";
                ws.Title = "Login oplysninger";

                ws.Controls.Add(new LiteralControl(_beforeCellHeader));
                ws.Controls.Add(new LiteralControl("<h3>" + ws.Title + "</h3>"));
                ws.Controls.Add(new LiteralControl(_endCellHeader));
                ws.Controls.Add(new LiteralControl(_startTable));

                foreach (Control c in CommonControls())
                {
                    ws.Controls.Add(c);
                }

                wzrd1.WizardSteps.Add(ws);

                foreach (umbraco.cms.businesslogic.ContentType.TabI t in mt.getVirtualTabs)
                {
                    WizardStep wsT = new WizardStep();
                    wsT.ID = t.Id.ToString();
                    wsT.Title = t.Caption;
                    wsT.Controls.Add(new LiteralControl(_beforeCellHeader));
                    wsT.Controls.Add(new LiteralControl("<h3>" + t.Caption + "</h3>"));
                    wsT.Controls.Add(new LiteralControl(_endCellHeader));
                    wsT.Controls.Add(new LiteralControl(_startTable));
                    wzrd1.WizardSteps.Add(wsT);
                }

                foreach (PropertyType pt in ptypes)
                {
                    if (mt.ViewOnProfile(pt) == true && pt.Alias.ToString() != "medlemsnummerMember")
                    {
                        WizardStep wsFind = new WizardStep();

                        foreach (WizardStep p in wzrd1.WizardSteps)
                        {
                            if (p.ID.ToString() == pt.TabId.ToString())
                            {
                                wsFind = p;
                            }
                        }

                        wsFind.Controls.Add(new LiteralControl(_beforeRow));
                        wsFind.Controls.Add(new LiteralControl(_beforeCellRight));

                        IDataType dt = pt.DataTypeDefinition.DataType;
                        dt.DataEditor.Editor.ID = pt.Alias;
                        ((System.Web.UI.WebControls.WebControl)dt.DataEditor.Editor).CssClass = "formItem";

                        if (mt.MemberCanEdit(pt))
                        {
                            _dataFields.Add(dt);
                        }
                        else
                        {
                            try
                            {
                                ((System.Web.UI.WebControls.WebControl)dt.DataEditor.Editor).Enabled = false;
                            }
                            catch
                            {
                            }
                        }

                        Label l = new Label();
                        l.ID = "lbl" + pt.Alias;
                        l.AssociatedControlID = dt.DataEditor.Editor.ID;
                        l.CssClass = "memberFormLabels";

                        l.Text = pt.Name;

                        if (pt.Description != "")
                        {
                            if (pt.Description.ToString().Contains("[f]"))
                            {
                                l.Text = l.Text + "<br/><span class=\"fielddesc\">(" + pt.Description.Replace("[f]", "").Trim() + ")</span>";
                            }
                        }

                        wsFind.Controls.Add(l);
                        wsFind.Controls.Add(new LiteralControl(_afterCell));

                        if (pt.Mandatory)
                        {
                            wsFind.Controls.Add(new LiteralControl(_beforeCellSmallReq));
                            wsFind.Controls.Add(new LiteralControl(""));
                            wsFind.Controls.Add(new LiteralControl(_afterCell));
                            wsFind.Controls.Add(new LiteralControl(_beforeCellReq));
                            wsFind.Controls.Add(dt.DataEditor.Editor);
                        }
                        else
                        {
                            wsFind.Controls.Add(new LiteralControl(_beforeCellSmall));
                            wsFind.Controls.Add(new LiteralControl(""));
                            wsFind.Controls.Add(new LiteralControl(_afterCell));
                            wsFind.Controls.Add(new LiteralControl(_beforeCell));
                            wsFind.Controls.Add(dt.DataEditor.Editor);
                        }

                        // Validation
                        if (pt.Mandatory)
                        {
                            try
                            {
                                RequiredFieldValidator rq = new RequiredFieldValidator();
                                rq.ID = dt.DataEditor.Editor.ID + "_RFV";
                                rq.ControlToValidate = dt.DataEditor.Editor.ID;

                                rq.ValidationGroup = "osMemberControlsValidate";

                                rq.ErrorMessage = pt.Name + " " + umbraco.library.GetDictionaryItem("Error.Field.Empty") ;
                                rq.CssClass = "pErrorMessage";
                                rq.Display = ValidatorDisplay.Dynamic;
                                rq.EnableClientScript = true;
                                wsFind.Controls.Add(rq);
                            }
                            catch
                            {
                            }
                        }
                        // RegExp Validation
                        if (!string.IsNullOrEmpty(pt.ValidationRegExp))
                        {
                            try
                            {
                                RegularExpressionValidator rv = new RegularExpressionValidator();
                                rv.ID = dt.DataEditor.Editor.ID + "_REV";
                                rv.ControlToValidate = dt.DataEditor.Editor.ID;
                                rv.ValidationExpression = pt.ValidationRegExp;

                                rv.CssClass = "pErrorMessage";
                                rv.ValidationGroup = "osMemberControlsValidate";
                                if (pt.Description != "")
                                {
                                    rv.ErrorMessage = "Indtast: " + pt.Description.Replace("[f]", "").Trim();
                                }
                                else
                                {
                                    rv.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Wrong.Text");
                                }
                                rv.Display = ValidatorDisplay.Dynamic;
                                rv.EnableClientScript = true;

                                wsFind.Controls.Add(rv);
                            }
                            catch
                            {
                            }
                        }

                        wsFind.Controls.Add(new LiteralControl(_afterCell));
                        wsFind.Controls.Add(new LiteralControl(_afterRow));
                    }
                }

                WizardStep wsNews = new WizardStep();

                wsNews.ID = "9999";
                wsNews.Title = "Nyhedsbreve";
                wsNews.Controls.Add(new LiteralControl(_beforeCellHeader));
                wsNews.Controls.Add(new LiteralControl("<h3>" + wsNews.Title + "</h3>"));
                wsNews.Controls.Add(new LiteralControl(_endCellHeader));
                wsNews.Controls.Add(new LiteralControl(_startTable));

                foreach (Control c in News())
                {
                    wsNews.Controls.Add(c);
                }

                wzrd1.WizardSteps.Add(wsNews);

                foreach (WizardStep wsEnd in wzrd1.WizardSteps)
                {
                    wsEnd.Controls.Add(new LiteralControl(_endTable));
                }

                wzrd1.FinishButtonClick += new WizardNavigationEventHandler(WizardFinish_Click);
                wzrd1.PreviousButtonClick += new WizardNavigationEventHandler(Wizard_NextButtonClick);
                wzrd1.NextButtonClick += new WizardNavigationEventHandler(Wizard_NextButtonClick);
                wzrd1.ActiveStepChanged += new EventHandler(wzrd1_ActiveStepChanged);
                wzrd1.SideBarButtonClick += new WizardNavigationEventHandler(Wizard_NextButtonClick);

                foreach (Control c in wzrd1.Controls)
                {
                    AssignValidationGroup(c);
                }

                ph1.Controls.Add(wzrd1);


                #endregion 'wizard'
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public List<Control> News()
        {

            List<Control> l = new List<Control>();

            newsletters.ID = "newslist";
            newsletters.Width = 400;

            newsletters.Items.Clear();
            bindNewsletters();

            Label newsLabel = new Label();
            newsLabel.ID = "newsLabel";
            newsLabel.AssociatedControlID = newsletters.ID.ToString();
            newsLabel.Text = umbraco.library.GetDictionaryItem("Label.Newslist.Header");

            l.Add(new LiteralControl(_beforeRow));
            l.Add(new LiteralControl(_beforeCellRight));
            l.Add(newsLabel);
            l.Add(new LiteralControl(_afterCell));
            l.Add(new LiteralControl(_beforeCell));
            l.Add(newsletters);

            l.Add(new LiteralControl(_afterCell));
            l.Add(new LiteralControl(_afterRow));

            pnlResp.CssClass = "responseNewsletterSubscription";
            pnlResp.Visible = false;

            Label responseLabel = new Label();
            responseLabel.ID = "responseLabel";
            responseLabel.CssClass = "responseNewsletterLabel";
            responseLabel.Text = umbraco.library.GetDictionaryItem("Success.Description.Text");

            pnlResp.Controls.Add(responseLabel);

            l.Add(new LiteralControl(_beforeRow));
            l.Add(new LiteralControl("<td colspan=\"2\">"));
            l.Add(pnlResp);
            l.Add(new LiteralControl(_afterCell));
            l.Add(new LiteralControl(_afterRow));

            return l;

        }
        private void bindNewsletters()
        {

            MCList[] lists = api.lists();
            foreach (MCList list in lists)
            {
                ListItem i = new ListItem();
                i.Value = list.id;
                i.Text = list.name;
                newsletters.Items.Add(i);

            }
        }



        public void list_subscribe(string Email, string FirstName, string LastName, string listId)
        {

            pnlResp.Visible = false;
            HeaderLabel.Visible = true;
            string email_type = "text";

            MCMergeVar[] merges = new MCMergeVar[2];
            merges[0].tag = "FNAME";
            merges[0].val = FirstName;
            merges[1].tag = "LNAME";
            merges[1].val = LastName;

            bool MemberExists = false;

            MCListMember[] lists = api.listMembers(listId);

            foreach (MCListMember list in lists)
            {
                if (list.email.ToString().Equals(Email))
                {
                    MemberExists = true;
                }
            }
            if (!MemberExists)
            {
                try
                {
                    api.listSubscribe(listId, Email, merges, email_type, true);
                    pnlResp.Visible = true;
                    HeaderLabel.Visible = false;
                }
                catch
                {
                    lblError.Text = umbraco.library.GetDictionaryItem("Error.Subscribe");
                }
            }
        }


        public void AssignValidationGroup(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                string i = c.GetType().ToString();
                if (c.GetType().ToString() == "System.Web.UI.WebControls.Button")
                {
                    ((Button)c).ValidationGroup = "osMemberControlsValidate";
                }
                if (c.Controls.Count > 0)
                {
                    foreach (Control cc in c.Controls)
                    {
                        AssignValidationGroup(cc);
                    }
                }
            }
        }


        public List<Control> CommonControls()
        {
            List<Control> l = new List<Control>();

            Label UserNameLabel = new Label();

            Label PasswordLabel = new Label(), EmailLabel = new Label(), ConfirmPasswordLabel = new Label();
            TextBox txtUserName = new TextBox(), txtEmail = new TextBox(), txtPassword = new TextBox(), txtConfirmPassword = new TextBox();
            RequiredFieldValidator UserNameRequired = new RequiredFieldValidator(), ConfirmPasswordRequired = new RequiredFieldValidator(), PasswordRequired = new RequiredFieldValidator(), EmailRequired = new RequiredFieldValidator();
            CompareValidator PasswordCompare = new CompareValidator();
            RegularExpressionValidator EmailRegEx = new RegularExpressionValidator();

            //txtUserName.ID = "txtUserName";

            //UserNameLabel.ID = "UserNameLabel";
            //UserNameLabel.AssociatedControlID = txtUserName.ID.ToString();
            //UserNameLabel.Text = umbraco.library.GetDictionaryItem("Label.UserName");

            //UserNameRequired.ID = "UserNameRequired";
            //UserNameRequired.ControlToValidate = txtUserName.ID.ToString();

            //UserNameRequired.ErrorMessage = umbraco.library.GetDictionaryItem("Error.UserName.Empty");
            //UserNameRequired.ValidationGroup = "osMemberControlsValidate";
            //UserNameRequired.CssClass = "pErrorMessage";
            //UserNameRequired.Display = ValidatorDisplay.Dynamic;

            //List<Control> UserNameValidators = new List<Control>();
            //UserNameValidators.Add(UserNameRequired);


            //foreach (Control c in WrapControls(UserNameLabel, txtUserName, UserNameValidators))
            //{
            //    l.Add(c);
            //}

            txtEmail.ID = "txtEmail";
            txtEmail.CssClass = "formItem";

            EmailLabel.ID = "EmailLabel";
            EmailLabel.AssociatedControlID = txtEmail.ID.ToString();
            EmailLabel.Text = umbraco.library.GetDictionaryItem("Label.Email");

            EmailLabel.Text += "<br/><span class=\"fielddesc\">(Bruges ved login)</span>";


            EmailRequired.ID = "EmailRequired";
            EmailRequired.ControlToValidate = txtEmail.ID.ToString();

            EmailRequired.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Email.Empty"); //Error.Email.Empty

            EmailRequired.ValidationGroup = "osMemberControlsValidate";
            EmailRequired.CssClass = "pErrorMessage";
            EmailRequired.Display = ValidatorDisplay.Dynamic;

            EmailRegEx.ID = "EmailRegex";
            EmailRegEx.ControlToValidate = txtEmail.ID.ToString();

            EmailRegEx.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Email.Invalid");
            EmailRegEx.EnableClientScript = true;
            EmailRegEx.ValidationExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            EmailRegEx.ValidationGroup = "osMemberControlsValidate";
            EmailRegEx.CssClass = "pErrorMessage";
            EmailRegEx.Display = ValidatorDisplay.Dynamic;

            List<Control> EmailValidators = new List<Control>();
            EmailValidators.Add(EmailRequired);
            EmailValidators.Add(EmailRegEx);

            foreach (Control c in WrapControls(EmailLabel, txtEmail, EmailValidators, true))
            {
                l.Add(c);
            }

            txtPassword.ID = "txtPassword";
            txtPassword.TextMode = TextBoxMode.Password;
            txtPassword.CssClass = "formItem";

            PasswordLabel.ID = "PasswordLabel";
            PasswordLabel.AssociatedControlID = txtPassword.ID.ToString();
            PasswordLabel.Text = umbraco.library.GetDictionaryItem("Label.Password");
            PasswordRequired.ID = "PasswordRequired";
            PasswordRequired.ControlToValidate = txtPassword.ID.ToString();
            PasswordRequired.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Password.Empty");
            PasswordRequired.ValidationGroup = "osMemberControlsValidate";
            PasswordRequired.CssClass = "pErrorMessage";
            PasswordRequired.Display = ValidatorDisplay.Dynamic;

            List<Control> PasswordValidators = new List<Control>();
            PasswordValidators.Add(PasswordRequired);

            foreach (Control c in WrapControls(PasswordLabel, txtPassword, PasswordValidators, true))
            {
                l.Add(c);
            }

            txtConfirmPassword.ID = "txtConfirmPassword";
            txtConfirmPassword.TextMode = TextBoxMode.Password;
            txtConfirmPassword.CssClass = "formItem";

            ConfirmPasswordLabel.ID = "ConfirmPassword";
            ConfirmPasswordLabel.AssociatedControlID = txtConfirmPassword.ID.ToString();
            ConfirmPasswordLabel.Text = umbraco.library.GetDictionaryItem("Label.Password.Confirm");
            ConfirmPasswordRequired.ID = "ConfirmPasswordRequired";
            ConfirmPasswordRequired.ControlToValidate = txtConfirmPassword.ID.ToString();
            ConfirmPasswordRequired.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Password.Confirm");
            ConfirmPasswordRequired.ValidationGroup = "osMemberControlsValidate";
            ConfirmPasswordRequired.CssClass = "pErrorMessage";
            ConfirmPasswordRequired.Display = ValidatorDisplay.Dynamic;

            PasswordCompare.ID = "PasswordCompare";
            PasswordCompare.ControlToCompare = txtPassword.ID.ToString();
            PasswordCompare.ControlToValidate = txtConfirmPassword.ID.ToString();
            PasswordCompare.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Passwords.Mismatch");
            PasswordCompare.ValidationGroup = "osMemberControlsValidate";
            PasswordCompare.CssClass = "pErrorMessage";
            PasswordCompare.Display = ValidatorDisplay.Dynamic;

            List<Control> PasswordCompareValidators = new List<Control>();
            PasswordCompareValidators.Add(ConfirmPasswordRequired);
            PasswordCompareValidators.Add(PasswordCompare);

            foreach (Control c in WrapControls(ConfirmPasswordLabel, txtConfirmPassword, PasswordCompareValidators, true))
            {
                l.Add(c);
            }

            return l;
        }

        public List<Control> WrapControls(Control LabelControl, Control NonLabelControl, List<Control> Validators, bool Mandatory)
        {
            List<Control> l = new List<Control>();
            l.Add(new LiteralControl(_beforeRow));
            l.Add(new LiteralControl(_beforeCellRight));
            l.Add(LabelControl);
            l.Add(new LiteralControl(_afterCell));
            if (Mandatory)
            {
                l.Add(new LiteralControl(_beforeCellSmallReq));
                l.Add(new LiteralControl(""));
                l.Add(new LiteralControl(_afterCell));
                l.Add(new LiteralControl(_beforeCellReq));
            }
            else
            {
                l.Add(new LiteralControl(_beforeCellSmall));
                l.Add(new LiteralControl(""));
                l.Add(new LiteralControl(_afterCell));
                l.Add(new LiteralControl(_beforeCell));
            }



            l.Add(NonLabelControl);

            foreach (Control c in Validators)
            {
                l.Add(c);
            }
            l.Add(new LiteralControl(_afterCell));
            l.Add(new LiteralControl(_afterRow));

            return l;
        }



        protected void Wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            Page.Validate("osMemberControlsValidate");
            // lblError.Text = "";

            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }
        }

        protected void wzrd1_ActiveStepChanged(object sender, EventArgs e)
        {
            if (wzrd1.ActiveStepIndex == 1)
            {
                ViewState["password"] = ((TextBox)wzrd1.WizardSteps[0].FindControl("txtPassword")).Text;
            }

            TextBox txtEmail = (TextBox)wzrd1.WizardSteps[0].FindControl("txtEmail");

            string tempEmail = txtEmail.Text;

            if (Member.GetMemberFromEmail(tempEmail) != null & Member.GetMemberFromLoginName(tempEmail) != null)
            {
                wzrd1.ActiveStepIndex = 0;

                lblError.Text = umbraco.library.GetDictionaryItem("Error.Email.InUse").Replace("[email]", "<b>" + tempEmail + "</b> ");
                txtEmail.Text = tempEmail;
            }
            else
            {
                lblError.Text = "";
            }
        }

        protected void WizardFinish_Click(object sender, WizardNavigationEventArgs e)
        {

            Page.Validate("osMemberControlsValidate");

            Member newMember = null;
            if (Page.IsValid)
            {
                string fname = ((TextBox)wzrd1.WizardSteps[1].FindControl("fornavnMember")).Text;
                string lname = ((TextBox)wzrd1.WizardSteps[1].FindControl("efternavnMember")).Text;
                string fullname = fname + " " + lname;
                string sEmail = ((TextBox)wzrd1.WizardSteps[0].FindControl("txtEmail")).Text;
                string spassword = ViewState["password"].ToString();
                MemberType mt = MemberType.GetByAlias(_memberTypeAlias);
                newMember = Member.MakeNew(sEmail, mt, new umbraco.BusinessLogic.User(0));

                Roles.AddUserToRole(sEmail, _membergroup);

                newMember.Email = sEmail;
                newMember.Password = spassword;
                newMember.LoginName = sEmail;
                newMember.Text = fullname;
                newMember.Save();

                //get the properties of the member type
                Member m = Member.GetMemberFromLoginName(sEmail);

                foreach (umbraco.interfaces.IDataType df in _dataFields)
                {
                    try
                    {
                        Property p = m.getProperty(df.DataEditor.Editor.ID);
                        df.Data.PropertyId = p.Id;
                        df.DataEditor.Save();
                        items = 1;
                    }
                    catch
                    {

                    }
                }

                //m.getProperty("forumUserPosts").Value = 0;
                //m.getProperty("forumUserLastPrivateMessage").Value = DateTime.Now;
                //m.getProperty("forumUserIsAuthorised").Value = 1;
                //m.getProperty("forumUserKarma").Value = 1;
                m.getProperty("memberAthorized").Value = 0;

                string f = fname;
                string l = lname;

                foreach (ListItem li in newsletters.Items)
                {
                    string listid = li.Value.ToString();

                    if (li.Selected)
                    {
                        list_subscribe(sEmail, f, l, listid);
                    }
                }

                MailAddress sfrom = new MailAddress("post@diaetist.dk");

                //Admin mail
                MailMessage mms = new MailMessage();
                SmtpClient ss = new SmtpClient();

                string HTMLTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\new-user-created.html";
                string TextTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\new-user-created.txt";
                string HTMLBody = "";
                string TextBody = "";
                HTMLBody = System.IO.File.ReadAllText(HTMLTemplatePath);
                TextBody = System.IO.File.ReadAllText(TextTemplatePath);
                HTMLBody = HTMLBody.Replace("<%UserName%>", m.Text);
                HTMLBody = HTMLBody.Replace("<%Email%>", m.Email);
                HTMLBody = HTMLBody.Replace("<%Host%>", Request.Url.Host);
                TextBody = TextBody.Replace("<%UserName%>", m.Text);
                TextBody = TextBody.Replace("<%Email%>", m.Email);
                TextBody = TextBody.Replace("<%Host%>", Request.Url.Host);

                mms.From = new MailAddress(sfrom.Address, "Fakd");
                mms.To.Add(_adminemailaddress);
                mms.Subject = "Ny bruger af typen " + _memberTypeAlias + " har tilmeldt sig. ";
                AlternateView plain = AlternateView.CreateAlternateViewFromString(TextBody, new System.Net.Mime.ContentType("text/plain"));
                AlternateView html = AlternateView.CreateAlternateViewFromString(HTMLBody, new System.Net.Mime.ContentType("text/html"));
                mms.AlternateViews.Add(plain);
                mms.AlternateViews.Add(html);

                ss.Send(mms);

                //User mail
                MailMessage mmsClient = new MailMessage();
                SmtpClient ssClient = new SmtpClient();

                string HTMLTemplatePathClient = @"D:\WEB\fakd.dk\www\umbraco\developer\new-user-created-client-mail.html";
                string TextTemplatePathClient = @"D:\WEB\fakd.dk\www\umbraco\developer\new-user-created-client-mail.txt";
                string HTMLBodyClient = "";
                string TextBodyClient = "";
                HTMLBodyClient = System.IO.File.ReadAllText(HTMLTemplatePathClient);
                TextBodyClient = System.IO.File.ReadAllText(TextTemplatePathClient);
                HTMLBodyClient = HTMLBodyClient.Replace("<%UserName%>", m.Email);
                HTMLBodyClient = HTMLBodyClient.Replace("<%Password%>", spassword);
                HTMLBodyClient = HTMLBodyClient.Replace("<%Host%>", Request.Url.Host);

                TextBodyClient = TextBodyClient.Replace("<%UserName%>", m.Email);
                TextBodyClient = TextBodyClient.Replace("<%Password%>", spassword);
                TextBodyClient = TextBodyClient.Replace("<%Host%>", Request.Url.Host);

                mmsClient.From = new MailAddress(sfrom.Address, "Fakd");
                mmsClient.To.Add(sEmail);
                mmsClient.Subject = "Du er oprettet som bruger på diaetist.dk";
                AlternateView plainClient = AlternateView.CreateAlternateViewFromString(TextBodyClient, new System.Net.Mime.ContentType("text/plain"));
                AlternateView htmlClient = AlternateView.CreateAlternateViewFromString(HTMLBodyClient, new System.Net.Mime.ContentType("text/html"));
                mmsClient.AlternateViews.Add(plainClient);
                mmsClient.AlternateViews.Add(htmlClient);

                ssClient.Send(mmsClient);

                m.getProperty("administratorUnderrettetOmoprettelse").Value = 1;

                // FormsAuthentication.SetAuthCookie(sEmail, false);

                Session.Add("aspnetforumUserName", sEmail);
                m.Save();

                Response.Redirect(umbraco.library.NiceUrl(_redirect));
            }
            else
            {
                lblError.Text = umbraco.library.GetDictionaryItem("Error.Member.Signup.Generel.Failure");
            }
        }
    }
}