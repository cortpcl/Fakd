<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyProfile.ascx.cs" Inherits="Ontranet.MyProfile" %>
<div id="myprofile">
    <span>
        <img src="/images/fakd/user_blue.png" alt="Profil" /></span> <a href="#">Vis mine profil
            muligheder</a>
</div>
<div id="profileOptions" style="display: none">
    <ul>
        <li><a href="/forside/medlemsservice/ret-medlemsoplysninger/min-profil.aspx">Ret min
            profil</a></li>
        <li>
            <asp:LoginStatus ID="ctlLogin" runat="server" LoginText="Login" LogoutText="Log ud" />
        </li>
    </ul>
</div>
