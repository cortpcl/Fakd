<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegisterMember.ascx.cs"
    Inherits="Ontranet.RegisterMember" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
          <asp:PlaceHolder ID="ph1" runat="server" />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />
    </ContentTemplate>
</asp:UpdatePanel>

<asp:PlaceHolder ID="PlaceHolder1" runat="server">
    <asp:LoginView ID="UmbracoLoginView" runat="server">
        <AnonymousTemplate>
    </AnonymousTemplate>
        <LoggedInTemplate>
         <asp:Label ID="lblLoggedIn" runat="server"></asp:Label>
         <br />
            <asp:LoginName ID="LoginName1" runat="server" />
            <br />
                <asp:LoginStatus ID="LoginStatus1" runat="server" />
        </LoggedInTemplate>
    </asp:LoginView>
</asp:PlaceHolder>
<br />
<br />
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
    <ProgressTemplate>
        <asp:Panel ID="Panel1" CssClass="overlay" runat="server">
            <asp:Panel ID="Panel2" CssClass="loader" runat="server">
                <img src="/images/loading.gif" />
            </asp:Panel>
        </asp:Panel>
    </ProgressTemplate>
</asp:UpdateProgress>

