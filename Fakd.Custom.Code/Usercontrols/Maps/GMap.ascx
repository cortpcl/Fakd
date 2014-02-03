<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GMap.ascx.cs" Inherits="Ontranet.GMap" %>
<%@ Register Assembly="GMaps" Namespace="Subgurim.Controles" TagPrefix="cc1" %>


<!--<div>
    <div style="float: left">
        Sorter efter region &nbsp;
    </div>
    <div style="float: left">-->
<asp:DropDownList ID="DropDownList2" runat="server"
    AutoPostBack="True" Visible="false">
</asp:DropDownList>
<!--</div>
</div>

<br />
<br />-->



<cc1:GMap ID="GMap1" runat="server" Height="450px" Width="650px" ajaxUpdateProgressMessage="Henter kort..."
    enableHookMouseWheelToZoom="True" enableStore="False" enableContinuousZoom="False"
    enableDoubleClickZoom="False" />


<asp:Panel ID="memberslist" runat="server" Visible="false">

    <div class="members">
        <asp:Literal ID="member" runat="server"></asp:Literal>
    </div>
</asp:Panel>


