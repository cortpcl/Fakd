<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginNetworkGroups.ascx.cs" Inherits="Ontranet.LoginNetworkGroups" %>
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
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            ValidationGroup="login"  Display="Dynamic" CssClass="Validators"></asp:RequiredFieldValidator>
                              
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
     
    </AnonymousTemplate>

  
</asp:LoginView>
<asp:Label ID="lblError" runat="server" CssClass="pErrorMessage"></asp:Label>

