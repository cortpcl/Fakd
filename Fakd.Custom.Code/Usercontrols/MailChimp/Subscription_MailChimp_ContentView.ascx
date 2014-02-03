<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscription_MailChimp_ContentView.ascx.cs"
    Inherits="Ontranet.Subscription_MailChimp_ContentView" %>
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
        <asp:Panel ID="pnlMain" runat="server">
            <div class="form-comments">
                <fieldset>
                    <div class="row">
                        <div class="col">

                            <asp:Label ID="lblListHeader" runat="server" />
                            <asp:CheckBoxList ID="CheckBoxList1" runat="server">
                            </asp:CheckBoxList>
                            <asp:CustomValidator ID="CheckBoxList1_Check" runat="server" Display="Dynamic" EnableClientScript="true"
                                ClientIDMode="Static" ClientValidationFunction="CheckItem" CssClass="NoListSelected"></asp:CustomValidator>
                        </div>
                    </div>
                    <asp:Panel ID="pnlInputs" runat="server">
                        <div class="row">
                            <div class="col">
                                <asp:Label ID="lblFirstName" runat="server" AssociatedControlID="txtFirstname" />
                                <asp:TextBox ID="txtFirstname" runat="server" ValidationGroup="subscription"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="subscription" runat="server"
                                    Display="Dynamic" ControlToValidate="txtFirstname"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col">
                            <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" />

                                
                                <asp:TextBox ID="txtLastName" ValidationGroup="subscription" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                    Display="Dynamic" ControlToValidate="txtLastName" ValidationGroup="subscription"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col">

                                <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" />
                                <asp:TextBox ID="txtEmail" ValidationGroup="subscription" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    Display="Dynamic" ValidationGroup="subscription" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                    Display="Dynamic" ControlToValidate="txtEmail" ValidationGroup="subscription" runat="server" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Button ID="btn" runat="server" OnClick="btn_Click" ValidationGroup="subscription" />
                    <p>
                        <asp:Label ID="lbError" runat="server" Visible="false"></asp:Label></p>
                </fieldset>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlUserResponse" runat="server" Visible="false">
            <p>
                <asp:Label ID="lblSubscriptionSuccess" runat="server"></asp:Label></p>
        </asp:Panel>
        </section>
    </ContentTemplate>
</asp:UpdatePanel>
