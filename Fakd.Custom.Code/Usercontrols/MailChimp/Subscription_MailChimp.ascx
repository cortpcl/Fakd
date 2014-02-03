<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscription_MailChimp.ascx.cs"
    Inherits="Ontranet.Subscription_MailChimp" %>
<%@ Register Assembly="MyControls.Html5" Namespace="MyControls.Html5" TagPrefix="cc1" %>
<asp:UpdatePanel ID="pnl" runat="server">
    <ContentTemplate>
        <script type="text/javascript" language="javascript">
            function CheckItem(sender, args) {
                var chkControlId = '<%=CheckBoxList1.ClientID%>';
                var options = document.getElementById(chkControlId).getElementsByTagName('input');
                var ischecked = false;
                args.IsValid = false;
                for (i = 0; i < options.length; i++) {
                    var opt = options[i];
                    if (opt.type == "checkbox") {
                        if (opt.checked) {
                            ischecked = true;
                            args.IsValid = true;
                        }
                    }
                }
            }
        </script>
        <section class="box subscribe">
            <h2>
                <asp:Label ID="lblMainHeader" runat="server"></asp:Label></h2>
            <asp:Panel ID="pnlMain" runat="server">
                <p>
                    <asp:Label ID="lblListHeader" runat="server" />
                    <asp:CheckBoxList ID="CheckBoxList1" runat="server">
                    </asp:CheckBoxList>
                    <asp:CustomValidator ID="CheckBoxList1_Check" runat="server" Display="Dynamic" EnableClientScript="true"
                        ClientIDMode="Static" ClientValidationFunction="CheckItem" CssClass="NoListSelected"></asp:CustomValidator>
                </p>
                <div class="form-subscribe">
                    <fieldset>
                        <div class="col">
                            <asp:Panel ID="pnlInputs" runat="server">
                                <cc1:Html5TextBox ID="txtFirstname" runat="server" ValidationGroup="subscription"></cc1:Html5TextBox>
                                
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="subscription" runat="server"
                                    Display="Dynamic" ControlToValidate="txtFirstname"></asp:RequiredFieldValidator>
                               <cc1:Html5TextBox ID="txtLastName" runat="server" ValidationGroup="subscription"></cc1:Html5TextBox>
                               
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    Display="Dynamic" ControlToValidate="txtLastName" ValidationGroup="subscription"></asp:RequiredFieldValidator>
                                
                                <cc1:Html5TextBox ID="txtEmail" runat="server" ValidationGroup="subscription"></cc1:Html5TextBox>
                        
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    Display="Dynamic" ValidationGroup="subscription" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                    Display="Dynamic" ControlToValidate="txtEmail" ValidationGroup="subscription" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </asp:Panel>
                        </div>
                        <asp:Button ID="btn" runat="server" OnClick="btn_Click" ValidationGroup="subscription" />
                        <asp:Label ID="lbError" runat="server" Visible="false"></asp:Label>
                    </fieldset>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlUserResponse" runat="server" Visible="false">
                <asp:Label ID="lblSubscriptionSuccess" runat="server"></asp:Label>
            </asp:Panel>
        </section>
    </ContentTemplate>
</asp:UpdatePanel>
