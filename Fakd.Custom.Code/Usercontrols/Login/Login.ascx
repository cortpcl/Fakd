<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="Ontranet.Login" %>
<asp:LoginView ID="UmbracoLoginView" runat="server">
    <AnonymousTemplate>
        <asp:Label ID="lblNotLoggedindescription" runat="server"></asp:Label>

        <asp:Login RenderOuterTable="false" ID="ctlLogin" runat="server" RememberMeSet="True"
            Visible="true" OnLoginError="OnLoginError" OnLoggingIn="OnLoggingIn" VisibleWhenLoggedIn="False">
            <LayoutTemplate>
                <div id="login" class="form-comments">
                    <fieldset>
                        <div class="row">
                            <div class="col">

                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"></asp:Label>

                                <asp:TextBox ID="UserName" runat="server" ValidationGroup="login"></asp:TextBox>
                                      <asp:RegularExpressionValidator ID="regEmail" runat="server" ControlToValidate="UserName"
                                    Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="login" CssClass="Validators"  EnableClientScript="true"></asp:RegularExpressionValidator>
                             <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            ValidationGroup="login"  Display="Dynamic" CssClass="Validators"  EnableClientScript="true"></asp:RequiredFieldValidator>
                              <asp:CustomValidator ID="custUserApproved" runat="server" Display="dynamic" ValidationGroup="login" ControlToValidate="UserName" OnServerValidate="custUserApproved_ServerValidate"></asp:CustomValidator>
                          
                            </div>

                           <div class="col">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"></asp:Label>

                                <asp:TextBox ID="Password" runat="server" ValidationGroup="login" TextMode="Password"></asp:TextBox>
                            
                                 <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            ValidationGroup="login"  Display="Dynamic" CssClass="Validators"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="login" />
                    </fieldset>
                </div>
            </LayoutTemplate>
        </asp:Login>
        <p>
            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="passrecovery" CausesValidation="false"></asp:LinkButton>
        </p>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <asp:Label ID="lblLoggedindescription" runat="server"></asp:Label>
        <br />
        <br />
        <asp:HyperLink ID="lnkEditProfile" runat="server"></asp:HyperLink><br />
        <asp:LoginStatus ID="LoginStatus1" runat="server" />
    </LoggedInTemplate>
</asp:LoginView>
<asp:Label ID="lblError" runat="server" CssClass="pErrorMessage"></asp:Label>
<asp:PasswordRecovery ID="PasswordRecovery1" runat="server" Visible="false" FailureTextStyle-HorizontalAlign="Left"
    MailDefinition-IsBodyHtml="true" MailDefinition-BodyFileName="~/umbraco/developer/password-recovery-email-text.html"
    RenderOuterTable="false">
    <UserNameTemplate>

        <asp:Label ID="lblPassRecoveryText" runat="server"></asp:Label>
        <div class="form-comments">
            <fieldset>
                <div class="row">
                    <div class="col">
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"></asp:Label>
                        <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="regEmail2" runat="server" ControlToValidate="UserName"
                                    Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="ctl00$PasswordRecovery1" CssClass="Validators" EnableClientScript="true"></asp:RegularExpressionValidator>

                        <asp:RequiredFieldValidator ID="UserNameRequired2" runat="server" ControlToValidate="UserName"
                            ValidationGroup="ctl00$PasswordRecovery1"  Display="Dynamic" CssClass="Validators" EnableClientScript="true"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" ValidationGroup="ctl00$PasswordRecovery1" />
            </fieldset>
        </div>
        <p>
            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
        </p>
    </UserNameTemplate>
</asp:PasswordRecovery>
