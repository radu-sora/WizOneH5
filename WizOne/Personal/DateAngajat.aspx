<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="DateAngajat.aspx.cs" Inherits="WizOne.Personal.DateAngajat" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>
        function OnTabClick(s,e)
        {
            if (e.tab.name == "Documente")
            {
                pnlLoading.Show();
                s.PerformCallback();
                pnlLoading.Hide();
            }
        }
        //function OnActiveTabChanging(s, e) {
        //    UpdateGrid();
        //}

        //function UpdateGrid() {
        //    if (modifDosar && typeof grDateDosar !== 'undefined' && ASPxClientUtils.IsExists(grDateDosar)) {
        //        grDateDosar.UpdateEdit();
        //        modifDosar = false;
        //    }

        //    if (typeof grDateBeneficii !== 'undefined' && ASPxClientUtils.IsExists(grDateBeneficii)) {
        //        //grDateBeneficii.Refresh();
        //    }
        //}

        function OnActiveTabChanged(s, e) {
            //switch (s.GetActiveTab().name) {
            //    case "Beneficii":
            //        if (typeof grDateBeneficii !== 'undefined' && ASPxClientUtils.IsExists(grDateBeneficii)) {
            //            grDateBeneficii.Refresh();
            //        }
            //        break;
            //    case "Dosar":
            //        if (typeof grDateDosar !== 'undefined' && ASPxClientUtils.IsExists(grDateDosar)) {
            //            grDateDosar.Refresh();
            //        }
            //        break;
            //        //if (modifDosar && typeof grDateDosar !== 'undefined' && ASPxClientUtils.IsExists(grDateDosar)) {
            //        //    grDateDosar.UpdateEdit();
            //        //    modifDosar = false;
            //        //}
            //        //break;
            //}
        }
    </script>

	<table width="100%">
		<tr>
			<td align="left"></td>			
			<td align="right">
                <dx:ASPxButton ID="btnDocPersonal" ClientInstanceName="btnDocPersonal" ClientIDMode="Static" runat="server" Text="Document" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                         strUrl = getAbsoluteUrl + 'Personal/ListaDocumente.aspx?qwe=' + s.name;
                        popGenDoc.SetHeaderText('Lista documente');
                        popGenDoc.SetContentUrl(strUrl);
                        popGenDoc.Show();                       
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>	
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        //UpdateGrid();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>				
				<dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="Lista.aspx" >
					<Image Url="../Fisiere/Imagini/Icoane/iesire.png"></Image>
				</dx:ASPxButton>
			</td>
		</tr>
	</table>
				
    <div>
        <dx:ASPxLabel  ID="lblDateAngajat" runat="server"  Text="" ></dx:ASPxLabel >	
        <dx:ASPxPageControl ID="ASPxPageControl2" runat="server" Width="100%" TabSpacing="0px" CssClass="pcTemplates" SkinID="None" EnableViewState="false" EnableHierarchyRecreation="false">
            <ClientSideEvents TabClick="OnTabClick" ActiveTabChanged="function(s,e) { OnActiveTabChanged(s,e); }" />
                <TabPages>
                </TabPages>     
            <Paddings Padding="0px" PaddingLeft="12px" />
            <ContentStyle Font-Names="Tahoma" Font-Overline="False" Font-Size="11px">
                <Paddings Padding="0px" />
                <Border BorderColor="#6DA0E7" BorderStyle="Solid" BorderWidth="1px" />
            </ContentStyle>
        </dx:ASPxPageControl>
    </div>


        <dx:ASPxPopupControl ID="popGenDoc" runat="server" AllowDragging="True" AllowResize="True"
            CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Middle"
            EnableViewState="False" PopupElementID="popupArea" PopupHorizontalAlign="WindowCenter"
            PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="False" Width="550px" MinHeight="500px"
            Height="100%" FooterText=" " CloseOnEscape="false" ClientInstanceName="popGenDoc" EnableHierarchyRecreation="True">                
                <ContentCollection>
                    <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
                    </dx:PopupControlContentControl>
                </ContentCollection>
        </dx:ASPxPopupControl>
  <dx:ASPxHiddenField ID="hfDate" runat="server" ClientInstanceName="hfDate" ClientIDMode="Static"/>
</asp:Content>	