<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmallLogin.ascx.cs" Inherits="Ontranet.SmallLogin" %>

<asp:LoginView ID="UmbracoLoginView" runat="server">
    <AnonymousTemplate>
        <asp:Login ID="Login1" runat="server" RenderOuterTable="false" OnLoggingIn="OnLoggingIn">
            <LayoutTemplate>
                <fieldset>
                    <div class="form-holder">
                        <strong class="title">Log ind</strong>
                        <div class="field">
                            <div class="col">

                                <asp:TextBox ID="UserName" runat="server" ClientIDMode="Static" placeholder="email" CssClass="email" />
                                <asp:TextBox ID="Password" runat="server" CssClass="password" ClientIDMode="Static" placeholder="Kode" TextMode="Password" />
                            </div>
                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Login" ValidationGroup="ctl00$ctl00$Login1" ClientIDMode="Static" />
                        </div>
                    </div>
                    <ul>
                        <li>
                            <asp:HyperLink ID="lnkForgotten" runat="server" CssClass="link">Glemt adgangskode?</asp:HyperLink></li>
                        <li>
                            <asp:HyperLink ID="lnkNew" runat="server" CssClass="link">Førstegangsbruger?</asp:HyperLink></li>
                    </ul>
                </fieldset>
            </LayoutTemplate>
        </asp:Login>
    </AnonymousTemplate>
    <LoggedInTemplate>

        <asp:LoginStatus ID="ctlLogin" runat="server" LoginText="Login" LogoutText="Log ud" />
        <br />
        <asp:HyperLink ID="lnkProfile" runat="server">Ret min
            profil</asp:HyperLink>
    </LoggedInTemplate>
</asp:LoginView>
