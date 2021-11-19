<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="Document.aspx.cs" Inherits="WizOne.AvansXDecont.Document" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentId', GoToEditMode);
                    break;			
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentId', GoToIstoric);
                    break;
            }
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }		
		
        function GoToDeleteMode(Value) {
            if (Value == 0 || Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Nu puteti anula o cerere deja anulata sau respinsa",
                    type: "warning"
                });
            }
            else {
                swal({
                    title: "Sunteti sigur/a ?", text: "Cererea va fi anulata !",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, anuleaza!", cancelButtonText: "Renunta", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete;" + Value);
                    }
                });
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=10&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }


        function CloseDeferedWindow() {
            popUpDivide.Hide();
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnMotivRespingere(s, e) {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '') {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Pentru a putea respinge este nevoie de un motiv"),
                    type: "warning"
                });
            }
            else {
                popUpMotiv.Hide();
                grDate.PerformCallback('btnRespinge;' + txtMtv.GetText());
                txtMtv.SetText('');
            }
        }

    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" runat="server" Text="Respinge" OnClick="btnRespinge_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" runat="server" Text="Aproba" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNou"  runat="server" Text="Adauga" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpNou.Show(); pnlCtl.PerformCallback(cmbDocumentType.GetValue()); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/new.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDuplicare"  runat="server" Text="Duplicare" OnClick="btnDuplicare_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/clone.png"></Image>
                </dx:ASPxButton>				
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 
    <br /> 
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared"  OnCustomButtonInitialize="grDate_CustomButtonInitialize"  >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" ShowEditButton="false" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Modifica" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>                        							
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>              
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        
                        <dx:GridViewDataTextColumn FieldName="DocumentId" Name="DocumentId" Caption="Nr. document" ReadOnly="true" Width="105px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
						<dx:GridViewDataTextColumn FieldName="DocumentTypeName" Name="DocumentTypeName" Caption="Tip document" ReadOnly="true" Width="150px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataDateColumn FieldName="DocumentDate" Name="DocumentDate" Caption="Data document" ReadOnly="true"  Width="110px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>						
						<dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Angajat" ReadOnly="true"  Width="300px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
						<dx:GridViewDataTextColumn FieldName="TotalAmountDocument" Name="TotalAmountDocument" Caption="Valoare" ReadOnly="true"  Width="100px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
						<dx:GridViewDataTextColumn FieldName="CurrencyCodeDocument" Name="CurrencyCodeDocument" Caption="Moneda" ReadOnly="true"  Width="75px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
						<dx:GridViewDataTextColumn FieldName="RefuseReason" Name="RefuseReason" Caption="Motiv refuz" ReadOnly="true"  Width="500px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
						<dx:GridViewDataTextColumn FieldName="SrcDocId" Name="SrcDocId" Caption="Doc. Sursa" ReadOnly="true"  Width="100px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
						
						<dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="CurrencyIdDocument" Name="CurrencyIdDocument" Caption="CurrencyIdDocument" ReadOnly="true" Width="75px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="SumaDocument" Name="SumaDocument" Caption="Valoare" ReadOnly="true" Width="75px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="DocumentTypeId" Name="DocumentTypeId" Caption="DocumentTypeId" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Culoare" Name="Culoare" Caption="Culoare" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdCircuit" Name="IdCircuit" Caption="IdCircuit" ReadOnly="true" Width="50px" Visible="false" />						
						<dx:GridViewDataTextColumn FieldName="IsBudgetOwnerForDocument" Name="IsBudgetOwnerForDocument" Caption="IsBudgetOwnerForDocument" ReadOnly="true" Width="75px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="User" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="Data" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="canBeRefused" Name="canBeRefused" Caption="canBeRefused" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie" ReadOnly="true" Width="50px" Visible="false" />
                    </Columns>                   
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table> 
	 
	 
	 
    <dx:ASPxPopupControl ID="popUpNou" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpNouArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Selectare tip document"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpNou" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">

                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server"  OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
                    <ClientSideEvents EndCallback="function (s,e) { pnlLoading.Hide(); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <asp:Panel ID="Panel1" runat="server">
                                <table style="width:100%;">
                                    <tr>
                                        <td align="right">
                                            <dx:ASPxButton ID="btnSave" runat="server" Text="Salvare" AutoPostBack="true" OnClick="btnSalvare_Click" >
                                                <ClientSideEvents Click="function(s, e) { popUpNou.Hide(); e.processOnServer = true; }" />
                                                <Image Url="~/Fisiere/Imagini/Icoane/save.png"></Image>
                                            </dx:ASPxButton>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:100%;">
                                            <label id="lblFormNou" runat="server" >Document</label>
                                            <br />
                                            <dx:ASPxComboBox ID="cmbDocumentType" runat="server" ClientIDMode="Static" ClientInstanceName="cmbDocumentType" Width="200px" DropDownWidth="200px" ValueField="DocumentTypeId" TextField="DocumentTypeName" AutoPostBack="false"  AllowNull="true" CssClass="aspxComboBox_center">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(s.GetValue()); }" />
                                            </dx:ASPxComboBox>  
											<br/>
											<label id="lblDataDocument" runat="server"  >Data document</label>
                                            <br />
				                            <dx:ASPxDateEdit  ID="deDataDoc" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" ClientInstanceName="deDataDoc"  AutoPostBack="false" CssClass="aspxComboBox_center" >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
				                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>	

    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv respingere"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRespingeMtv" runat="server" Text="Respinge" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnMotivRespingere(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxMemo ID="txtMtv" runat="server" ClientIDMode="Static" ClientInstanceName="txtMtv" Width="630px" Height="180px"></dx:ASPxMemo>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>	
</asp:Content>
