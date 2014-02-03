<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopLogin.ascx.cs" Inherits="Ontranet.TopLogin" %>

<%@ Register Assembly="MyControls.Html5" Namespace="MyControls.Html5" TagPrefix="cc2" %>

<asp:LoginView ID="UmbracoLoginView" runat="server">
    <AnonymousTemplate>
        <fieldset>
            <div class="form-holder">
                <strong class="title">
                    <asp:Label ID="lblLoginTitle" runat="server"></asp:Label></strong>
                <div class="field">
                    <asp:Login RenderOuterTable="false" ID="ctlLogin" runat="server" RememberMeSet="True"
                        Visible="true" OnLoginError="OnLoginError" OnLoggingIn="OnLoggingIn" VisibleWhenLoggedIn="False">
                        <LayoutTemplate>
                            <div class="col">
                                <cc2:Html5TextBox ID="UserName" runat="server" ValidationGroup="toplogin" CssClass="email"></cc2:Html5TextBox>
                                <asp:RegularExpressionValidator ID="regEmail" runat="server" ControlToValidate="UserName"
                                    Display="none" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="toplogin"></asp:RegularExpressionValidator>
                                     <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            ValidationGroup="toplogin"  Display="none" EnableClientScript="true"></asp:RequiredFieldValidator>
                                <cc2:Html5TextBox ID="Password" runat="server" ValidationGroup="toplogin" CssClass="password" ClientIDMode="Static" TextMode="Password"></cc2:Html5TextBox>
                               <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            ValidationGroup="toplogin"  Display="none" EnableClientScript="true"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="custUserApproved" runat="server" Display="None" ValidationGroup="toplogin" ControlToValidate="UserName" OnServerValidate="custUserApproved_ServerValidate"></asp:CustomValidator>
                            </div>
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="toplogin" />
                        </LayoutTemplate>
                    </asp:Login>
                </div>
            </div>
            <ul>
                <li>
                    <asp:HyperLink ID="lnkForgotPassword" runat="server"></asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="lnkNew" runat="server"></asp:HyperLink></li>
            </ul>
        </fieldset>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <asp:HyperLink ID="lnkEditProfile" runat="server"></asp:HyperLink><br />
        <asp:LoginStatus ID="LoginStatus1" runat="server" />
    </LoggedInTemplate>
</asp:LoginView>
<asp:Label ID="lblError" runat="server" CssClass="Validators2"></asp:Label>
<asp:ValidationSummary ID="valSummary" runat="server" CssClass="Validators2" DisplayMode="List" EnableClientScript="true" ShowMessageBox="false" ShowSummary="true" ValidationGroup="toplogin"/>
