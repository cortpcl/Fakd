<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditMember.ascx.cs" Inherits="Ontranet.EditMember" %>



<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:PlaceHolder ID="ph1" runat="server" />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" />
    </ContentTemplate>
</asp:UpdatePanel>
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

