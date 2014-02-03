<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoginStatus.ascx.cs" Inherits="Ontranet.LoginStatus" %>


<asp:LoginView ID="UmbracoLoginView" runat="server">
    <AnonymousTemplate>
       <asp:Label ID="lblLogin" runat="server"></asp:Label>
    </AnonymousTemplate>
    <LoggedInTemplate>
   <asp:Label ID="lblLogout" runat="server"></asp:Label>
    </LoggedInTemplate>
</asp:LoginView>
