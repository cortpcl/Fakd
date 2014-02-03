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
using System.Net.Mail;
using MailChimp;
using System.Text;
using System.Data.SqlClient;



namespace Ontranet
{
    public partial class EditMember : System.Web.UI.UserControl
    {
        private string _beforeRow = "<tr>";
        private string _afterRow = "</tr>";
        private string _beforeCell = "<td>";
        private string _afterCell = "</td>";
        private string _beforeCellHeader = "<div class=\"signupheader\">";
        private string _endCellHeader = "</div>";
        private string _beforeCellRight = "<td class=\"firstcellsignup\">";
        private string _startTable = "<table class=\"osMemberProfile\">";
        private string _endTable = "</table>";
        private ArrayList _dataFields = new ArrayList();
        private int _redirectWhenNotLoggeedIn;
        private bool _displaywizardsidebar;
        private Wizard wzrd1;
        private string _adminemailaddress;
        private string _mailChimpApiKey;
        public Hashtable hs = new Hashtable();

        public int RedirectWhenNotLoggeedIn
        {
            get { return _redirectWhenNotLoggeedIn; }
            set { _redirectWhenNotLoggeedIn = value; }
        }

        public string MailChimpApiKey
        {
            get { return _mailChimpApiKey; }
            set { _mailChimpApiKey = value; }
        }

        public bool DisplayWizardSideBay
        {
            get { return _displaywizardsidebar; }
            set { _displaywizardsidebar = value; }
        }

        public string AdminEmailAddress
        {
            get { return _adminemailaddress; }
            set { _adminemailaddress = value; }
        }

        Member m;
        MailChimp.ApiWrapper api = new MailChimp.ApiWrapper();

        CheckBoxList newsletters = new CheckBoxList();
        ChangePassword chgPass = new ChangePassword();
        Panel pnlResp = new Panel();
        Label HeaderLabel = new Label();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.User.Identity.IsAuthenticated)
            {
                _mailChimpApiKey = System.Configuration.ConfigurationManager.AppSettings["mailChimpApiKey"].ToString();
                api.setCurrentApiKey(_mailChimpApiKey);


                m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name);
                umbraco.cms.businesslogic.member.MemberType mt = new umbraco.cms.businesslogic.member.MemberType(m.ContentType.Id);
                IEnumerable<PropertyType> ptypes;

                ptypes = mt.PropertyTypes;
                ptypes = ptypes.Where(g => g.TabId != 0);

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
                wzrd1.FinishCompleteButtonText = "Gem og afslut";
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

                WizardStep wsPass = new WizardStep();
                wsPass.ID = "99";
                wsPass.Title = "Password";
                wsPass.Controls.Add(new LiteralControl(_beforeCellHeader));
                wsPass.Controls.Add(new LiteralControl("<h3>" + wsPass.Title + "</h3>"));
                wsPass.Controls.Add(new LiteralControl(_endCellHeader));
                foreach (Control c in PasswordControls())
                {
                    wsPass.Controls.Add(c);
                }
                wzrd1.WizardSteps.Add(wsPass);

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
                    if (mt.ViewOnProfile(pt) == true)
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
                        dt.Data.Value = m.getProperty(pt.Alias.ToString()).Value;

                        if (mt.MemberCanEdit(pt))
                        {
                            _dataFields.Add(dt);
                        }
                        else
                        {
                            try
                            {
                                if (pt.Alias == "medlemsnummerMember")
                                {
                                    string v = (String)m.getProperty(pt.Alias.ToString()).Value;

                                    if (v.Equals(String.Empty))
                                    {
                                        dt.Data.Value = m.getProperty(pt.Alias.ToString()).Value = pt.Description;
                                    }
                                }
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
                        wsFind.Controls.Add(new LiteralControl(_beforeCell));
                        wsFind.Controls.Add(dt.DataEditor.Editor);

                        // Validation
                        if (pt.Mandatory)
                        {
                            try
                            {
                                RequiredFieldValidator rq = new RequiredFieldValidator();
                                rq.ID = dt.DataEditor.Editor.ID + "_RFV";
                                rq.ControlToValidate = dt.DataEditor.Editor.ID;
                                rq.ValidationGroup = "osMemberControlsValidate";
                                rq.ErrorMessage = l.Text + " " + umbraco.library.GetDictionaryItem("Error.Field.Empty") ;
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
                                    rv.ErrorMessage = umbraco.library.GetDictionaryItem("Error.Wrong.Text") ;
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
                    if (wsEnd.ID != "99")
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
           }
            else
            {
                Response.Redirect(umbraco.library.NiceUrl(_redirectWhenNotLoggeedIn));
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
        public List<Control> PasswordControls()
        {
            List<Control> l = new List<Control>();
            chgPass.ID = "passChange";
            chgPass.ChangePasswordTitleText = "";
            chgPass.ConfirmNewPasswordLabelText = "Bekræft password";
            chgPass.InstructionText = "Vælg selv et nyt password";
            chgPass.InstructionTextStyle.HorizontalAlign = HorizontalAlign.Left;
            chgPass.InstructionTextStyle.Font.Bold = true;
            System.Drawing.Color myColor = System.Drawing.ColorTranslator.FromHtml("#1c84c2");
            chgPass.InstructionTextStyle.ForeColor = myColor;
            chgPass.TextBoxStyle.CssClass = "passInput";
            chgPass.ControlStyle.CssClass = "passControls";
            chgPass.ChangePasswordButtonText = "Nyt password " + chgPass.CurrentPassword;
            chgPass.ChangePasswordButtonStyle.CssClass = "leftalign";
            chgPass.CancelButtonStyle.CssClass = "hide";
            chgPass.CancelButtonText = "Fortryd";
            chgPass.ContinueButtonStyle.CssClass = "hide";
            chgPass.LabelStyle.CssClass = "firstcellsignup";
            chgPass.LabelStyle.HorizontalAlign = HorizontalAlign.Left;
            chgPass.NewPasswordLabelText = "Nyt password";
            chgPass.PasswordLabelText = "Nuværende password";
            chgPass.ChangePasswordFailureText = umbraco.library.GetDictionaryItem("Error.Passwords.Mismatch");
            chgPass.MailDefinition.BodyFileName = "~/umbraco/developer/password-change-email-text.html";
            chgPass.MailDefinition.Subject = "Ændret password til diaetist.dk";
            chgPass.MailDefinition.IsBodyHtml = true;
            chgPass.SuccessText = "Dit nye password er sendt til din email";
            chgPass.SuccessTitleText = "Passwordet blev ændret";
            chgPass.ConfirmPasswordCompareErrorMessage = umbraco.library.GetDictionaryItem("Error.Passwords.Mismatch");
            l.Add(chgPass);

            PasswordRecovery PassRecovery = new PasswordRecovery();
            PassRecovery.ID = "passrecov";
            PassRecovery.UserNameLabelText = m.Email;
            PassRecovery.ControlStyle.CssClass = "passControls";
            PassRecovery.TextBoxStyle.CssClass = "passInput";
            PassRecovery.UserNameTitleText = "Få et nyt password";
            PassRecovery.TitleTextStyle.HorizontalAlign = HorizontalAlign.Left;
            PassRecovery.TitleTextStyle.Font.Bold = true;
            PassRecovery.TitleTextStyle.ForeColor = myColor;
            PassRecovery.UserNameInstructionText = "Det nye password vil blive sendt til ";
            PassRecovery.InstructionTextStyle.HorizontalAlign = HorizontalAlign.Left;
            PassRecovery.UserName = HttpContext.Current.User.Identity.Name;
            PassRecovery.LabelStyle.CssClass = "firstcellsignup";
            PassRecovery.LabelStyle.HorizontalAlign = HorizontalAlign.Left;
            PassRecovery.SubmitButtonText = "Modtag";
            PassRecovery.SuccessText = "Dit nye password er sendt til din email";
            PassRecovery.SubmitButtonStyle.CssClass = "leftalign";
            PassRecovery.MailDefinition.BodyFileName = "~/umbraco/developer/password-recovery-email-text.html";
            PassRecovery.MailDefinition.Subject = "Nyt password til diaetist.dk";
            PassRecovery.MailDefinition.IsBodyHtml = true;

            TextBox tb = (TextBox)PassRecovery.UserNameTemplateContainer.FindControl("UserName");
            tb.Enabled = false;
            tb.Visible = false;

            l.Add(PassRecovery);

            return l;
        }

        protected void sendEmail(System.Net.Mail.MailMessage fileMsg)
        {
            System.Net.Mail.MailMessage msg = fileMsg;
            SmtpClient sc = new SmtpClient();
            sc.Send(msg);
        }

        public List<Control> News()
        {
            List<Control> l = new List<Control>();
            newsletters.ID = "newslist";
            newsletters.Width = 400;

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
            newsletters.Items.Clear();
            Member m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name);

            try
            {

                MCList[] lists = api.lists();
                foreach (MCList list in lists)
                {
                    ListItem i = new ListItem();
                    i.Value = list.id;
                    i.Text = list.name;
                    newsletters.Items.Add(i);

                    try
                    {
                        MCListMember[] listsMembers = api.listMembers(list.id);

                        foreach (MCListMember listm in listsMembers)
                        {
                            if (listm.email.Trim().ToLower().Equals(m.Email.Trim().ToLower())) i.Selected = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        sendErrorMail(ex, "Err retrieving members from mailchimp");
                    }
                }
            }
            catch (Exception ex)
            {
                sendErrorMail(ex, "Err retrieving newwsletters from mailchimp");
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
                catch (Exception ex)
                {
                    lblError.Text = umbraco.library.GetDictionaryItem("Error.Subscribe");
                    sendErrorMail(ex, umbraco.library.GetDictionaryItem("Error.Subscribe"));
                }
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
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = umbraco.library.GetDictionaryItem("Error.Unsubscribe") + " " + ex.Message;
                sendErrorMail(ex, umbraco.library.GetDictionaryItem("Error.Unsubscribe"));
            }
        }

        public List<Control> CommonControls()
        {
            List<Control> l = new List<Control>();

            Label UserNameLabel = new Label(), PasswordLabel = new Label(), EmailLabel = new Label(), ConfirmPasswordLabel = new Label();
            TextBox txtUserName = new TextBox(), txtEmail = new TextBox(), txtPassword = new TextBox(), txtConfirmPassword = new TextBox();
            RequiredFieldValidator UserNameRequired = new RequiredFieldValidator(), ConfirmPasswordRequired = new RequiredFieldValidator(), PasswordRequired = new RequiredFieldValidator(), EmailRequired = new RequiredFieldValidator();
            CompareValidator PasswordCompare = new CompareValidator();
            RegularExpressionValidator EmailRegEx = new RegularExpressionValidator();

            //txtUserName.ID = "txtUserName";
            //txtUserName.Text = HttpContext.Current.User.Identity.Name;


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
            txtEmail.Text = m.Email;

            EmailLabel.ID = "EmailLabel";
            EmailLabel.AssociatedControlID = txtEmail.ID.ToString();
            EmailLabel.Text = umbraco.library.GetDictionaryItem("Label.Email");

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

            foreach (Control c in WrapControls(EmailLabel, txtEmail, EmailValidators))
            {
                l.Add(c);
            }

            return l;
        }

        public List<Control> WrapControls(Control LabelControl, Control NonLabelControl, List<Control> Validators)
        {
            List<Control> l = new List<Control>();
            l.Add(new LiteralControl(_beforeRow));
            l.Add(new LiteralControl(_beforeCellRight));
            l.Add(LabelControl);
            l.Add(new LiteralControl(_afterCell));
            l.Add(new LiteralControl(_beforeCell));
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
            lblError.Text = "";
            Page.Validate("osMemberControlsValidate");

            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                foreach (umbraco.interfaces.IDataType df in _dataFields)
                {
                    Property p = m.getProperty(df.DataEditor.Editor.ID);

                    if (!hs.ContainsKey(p.Id))
                    {
                        hs.Add(p.Id, df.Data.Value.ToString());
                    }
                }
                Save();
            }
        }
        protected void Password_Change(object sender, LoginCancelEventArgs e)
        {
        }

        protected void wzrd1_ActiveStepChanged(object sender, EventArgs e)
        {
            lblError.Text = "";
            Member m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name);

            foreach (umbraco.interfaces.IDataType df in _dataFields)
            {
                Property p = m.getProperty(df.DataEditor.Editor.ID);

                if (!hs.ContainsKey(p.Id))
                {
                    hs.Add(p.Id, df.Data.Value.ToString());
                }
            }
            // try
            // {
            //lblError.Text = "";

            //    TextBox txtUserName = (TextBox)wzrd1.WizardSteps[0].FindControl("txtUserName");
            //    string tempUserName = txtUserName.Text;
            //    if (m.LoginName != txtUserName.Text)
            //    {
            //        if (Member.IsMember(txtUserName.Text))
            //        {
            //            wzrd1.ActiveStepIndex = 0;
            //            lblError.Text = umbraco.library.GetDictionaryItem("Error.UserName.InUse").Replace("[username]", "<b>" + tempUserName + "</b> ");
            //            txtUserName.Text = m.LoginName;
            //        }
            //        else
            //        {
            //            try
            //            {
            //                m.LoginName = txtUserName.Text;
            //                m.Save();
            //            }
            //            catch (Exception ex)
            //            {
            //                sendErrorMail(ex, "Err saving username");
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    sendErrorMail(ex, "Err validating username");
            //}


            try
            {
                TextBox txtEmail = (TextBox)wzrd1.WizardSteps[0].FindControl("txtEmail");
                string tempEmail = txtEmail.Text;
                if (m.Email != txtEmail.Text)
                {
                    if (Member.GetMemberFromEmail(txtEmail.Text) != null)
                    {
                        wzrd1.ActiveStepIndex = 0;
       
                        lblError.Text = umbraco.library.GetDictionaryItem("Error.Email.InUse").Replace("[email]", "<b>" + tempEmail + "</b> ");
                      
                        txtEmail.Text = m.Email;
                    }
                    else
                    {
                        foreach (ListItem li in newsletters.Items)
                        {
                            string listid = li.Value.ToString();
                            list_unsubscribe(m.Email, listid);
                        }
                        try
                        {
                            if (!hs.ContainsKey(m.Id + 1))
                            {
                                hs.Add(m.Id + 1, "Email");
                                SaveMemberChangesToDb(m.Id, "Email", m.Email, txtEmail.Text, "", -1);
                            }

                            m.Email = txtEmail.Text;
                            m.Save();

                        }
                        catch (Exception ex)
                        {
                            sendErrorMail(ex, "Err saving email");
                        }

                        bindNewsletters();
                        pnlResp.Visible = false;
                        HeaderLabel.Visible = true;
                    }
                }

            }
            catch (Exception ex)
            {
                sendErrorMail(ex, "Err validating email");
            }
        }

        protected void WizardFinish_Click(object sender, WizardNavigationEventArgs e)
        {

            Member m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name);

            try
            {
                string f = m.getProperty("fornavnMember").Value.ToString();
                string l = m.getProperty("efternavnMember").Value.ToString();

                foreach (ListItem li in newsletters.Items)
                {
                    string listid = li.Value.ToString();

                    if (li.Selected)
                    {
                        list_subscribe(m.Email, f, l, listid);
                    }
                    else
                    {
                        list_unsubscribe(m.Email, listid);
                    }
                }
            }
            catch
            {
            }

           
            //Admin mail
            if (GetMemberChanges(m.Id).ToString() != "")
            {
                MailAddress sfrom = new MailAddress("post@diaetist.dk");

                MailMessage mms = new MailMessage();
                SmtpClient ss = new SmtpClient();

                string HTMLTemplatePath = @"D:\WEB\fakd.dk\www\umbraco\developer\user-profile-change.html";

                string HTMLBody = "";

                HTMLBody = System.IO.File.ReadAllText(HTMLTemplatePath);

                HTMLBody = HTMLBody.Replace("<%UserName%>", m.getProperty("fornavnMember").Value.ToString() + " " + m.getProperty("efternavnMember").Value.ToString());
                HTMLBody = HTMLBody.Replace("<%Host%>", Request.Url.Host);

                HTMLBody = HTMLBody.Replace("<%changes%>", GetMemberChanges(m.Id).ToString());

                mms.From = new MailAddress(sfrom.Address, "Fakd");
                mms.To.Add(_adminemailaddress);
                mms.Subject = "Brugeren har tilpasset sin profil, overfør oplysninger til winkas";
                AlternateView html = AlternateView.CreateAlternateViewFromString(HTMLBody, new System.Net.Mime.ContentType("text/html"));
                mms.AlternateViews.Add(html);
                ss.Send(mms);

                DeleteMemberChanges(m.Id);
            }

            lblError.Text = "Ændringerne er gemt...";

        }

        protected void sendErrorMail(Exception ex, string errString)
        {

            MailAddress sfrom = new MailAddress("post@diaetist.dk");
            MailMessage mm = new MailMessage();
            SmtpClient s = new SmtpClient();

            mm.From = new MailAddress(sfrom.Address, "Fakd");
            mm.To.Add("pcl@pcl.dk");
            mm.Subject = errString;
            mm.Body += ex.Message.ToString();

            mm.Body += ex.Source.ToString();
            mm.Body += ex.StackTrace.ToString();

            mm.IsBodyHtml = true;
            s.Send(mm);
        }

        protected void Save()
        {
            pnlResp.Visible = false;
            HeaderLabel.Visible = true;

            try
            {
                Page.Validate("osMemberControlsValidate");
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                sendErrorMail(ex, "Validating page");
            }

            Member m = Member.GetMemberFromLoginName(HttpContext.Current.User.Identity.Name);
            if (Page.IsValid)
            {
                try
                {
                    foreach (umbraco.interfaces.IDataType df in _dataFields)
                    {
                        Property p = m.getProperty(df.DataEditor.Editor.ID);

                        df.Data.PropertyId = p.Id;
                        df.DataEditor.Save();


                        string val = (string)hs[p.Id];
                        if (val != p.Value.ToString())
                        {
                            SaveMemberChangesToDb(m.Id, p.PropertyType.Name.ToString(), val, df.Data.Value.ToString(), p.PropertyType.DataTypeDefinition.DataType.DataTypeName, p.PropertyType.DataTypeDefinition.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    sendErrorMail(ex, "Getting properties");
                }

                try
                {
                    m.Save();
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    sendErrorMail(ex, "Fakd: Error updating new user");
                }

                try
                {
                    FormsAuthentication.SetAuthCookie(m.LoginName, true);
                    Session.Add("aspnetforumUserName", m.LoginName);
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    sendErrorMail(ex, "Fakd: Error updating new user");
                }
            }
        }


        private void SaveMemberChangesToDb(int id, string PropertyName, string oldvalue, string newvalue, string DataTypeDefinition, int DataTypeDefinitionId)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            string sql = string.Empty;

            sql = "INSERT INTO MemberProfileChanges (memberId,PropertyName,OldValue,NewValue,DataTypeDefinition,DataTypeDefinitionId) VALUES (" + id + ",'" + PropertyName + "','" + oldvalue + "','" + newvalue + "','" + DataTypeDefinition + "'," + DataTypeDefinitionId + ")";
            SqlCommand cmd = new SqlCommand(sql, con);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        private StringBuilder GetMemberChanges(int id)
        {
            StringBuilder sb = new StringBuilder();
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            SqlCommand cmd = new SqlCommand("SELECT * FROM  MemberProfileChanges WHERE memberId=" + id, con);

            con.Open();
            SqlDataReader r = cmd.ExecuteReader();

            string PropertyName = string.Empty;
            string OldValue = string.Empty;
            string NewValue = string.Empty;
            string DataTypeDefinition = string.Empty;
            int DataTypeDefinitionId = -1;
            try
            {
                while (r.Read())
                {
                    PropertyName = (string)r["PropertyName"];
                    OldValue = (string)r["OldValue"];
                    NewValue = (string)r["NewValue"];
                    DataTypeDefinition = (string)r["DataTypeDefinition"];
                    DataTypeDefinitionId = (int)r["DataTypeDefinitionId"];

                    if (DataTypeDefinition == "Checkbox list")
                    {
                        SortedList ddDataList = umbraco.cms.businesslogic.datatype.PreValues.GetPreValues(DataTypeDefinitionId);

                        string v = null;
                        string o = null;
                        foreach (umbraco.cms.businesslogic.datatype.PreValue pv in ddDataList.GetValueList())
                        {
                            if (NewValue.Contains(pv.Id.ToString()))
                            {
                                v += pv.Value + ",";
                            }

                            if (OldValue.Contains(pv.Id.ToString()))
                            {
                                o += pv.Value + ",";
                            }
                        }
                        //if (v.LastIndexOf(",") != -1)
                            NewValue = "" + v; //.Remove(v.Length - 1);
                        //if (o.LastIndexOf(",") != -1)
                            OldValue = "" + o; //.Remove(v.Length - 1);
                    }

                    if (DataTypeDefinition == "Radiobutton list" || DataTypeDefinition == "Dropdown list")
                    {
                        SortedList typesOfUserrd = umbraco.cms.businesslogic.datatype.PreValues.GetPreValues(DataTypeDefinitionId);

                        foreach (umbraco.cms.businesslogic.datatype.PreValue pv in typesOfUserrd.GetValueList())
                        {
                            if (NewValue.Equals(pv.Id.ToString()))
                            {
                                NewValue = pv.Value;
                            }

                            if (OldValue.Equals(pv.Id.ToString()))
                            {
                                OldValue = pv.Value;
                            }
                        }
                    }

                    if (OldValue.Equals(string.Empty))
                    {
                       OldValue = "Var ikke udfyldt tidligere";
                    }
                    if (NewValue.Equals(string.Empty))
                    {
                        NewValue = "Intet valgt eller udfyldt";
                    }
                
                    sb.Append("<tr><td>" + PropertyName + ":</td><td><strong>" + NewValue + "</strong> (" + OldValue + ")</td></tr>");
                }
            }
            finally
            {
                r.Close();
                con.Close();
            }
            return sb;
        }

        private void DeleteMemberChanges(int id)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["umbracoDbDSN"].ToString());
            string sql = "DELETE FROM  MemberProfileChanges WHERE memberId=" + id;
            SqlCommand cmd = new SqlCommand(sql, con);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }
    }

}