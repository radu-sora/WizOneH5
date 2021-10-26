<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="DocumentPayment.aspx.cs" Inherits="WizOne.AvansXDecont.DocumentPayment" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentStateId', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentId', GoToIstoric);
                    break;
            }
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
                            OnMotivRespingere()
                        
                    }
                });
            }
        }

        function OnMotivRespingere() {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '') {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Nu ati completat motivul refuzului pentru respingere documente!"),
                    type: "warning"
                });
            }
            else {
                popUpMotiv.Hide();
                grDate.PerformCallback("btnDelete;" + txtMtv.GetText());
                txtMtv.SetText('');
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


    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnSave"  runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {                       
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 
    <br /> 
    <table width="60%">   
        <tr>
            <td id="divRol" runat="server">
                <label id="lblStatusDoc" runat="server" style="display:inline-block;">Status documente</label>
                <dx:ASPxComboBox ID="cmbDocState" ClientInstanceName="cmbDocState" ClientIDMode="Static" runat="server" Width="250px" ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="true" OnSelectedIndexChanged="cmbDocState_SelectedIndexChanged" >
                       <ClientSideEvents  SelectedIndexChanged="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />                
                </dx:ASPxComboBox>
            </td>
            <td align="left">
                <label id="lblActiune" runat="server" style="display:inline-block;">Actiune</label>
                <dx:ASPxComboBox ID="cmbOperationSign" ClientInstanceName="cmbOperationSign" ClientIDMode="Static" runat="server" Width="250px" ValueField="OperationSignId" TextField="OperationSign" ValueType="System.Int32" AutoPostBack="true" OnSelectedIndexChanged="cmbOperationSign_SelectedIndexChanged" >                            
                       <ClientSideEvents  SelectedIndexChanged="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />  
                </dx:ASPxComboBox>
            </td>
            <td align="left">
                 <label id="lblData" runat="server" style="display:inline-block;">Data platii</label>
                <dx:ASPxDateEdit ID="txtPaymentDate" runat="server" Width="100%" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" PickerDisplayMode="Auto" meta:resourcekey="txtPaymentDate" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>            
            </td>
            <td align="left">
                <label id="lblModPlata" runat="server" style="display:inline-block;">Modalitate plata</label>
                <dx:ASPxComboBox ID="cmbPaymentMethod" runat="server" ClientInstanceName="cmbPaymentMethod" ClientIDMode="Static" Width="215px" ValueField="DictionaryItemId" DropDownWidth="200" 
                    TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="true" OnSelectedIndexChanged="cmbPaymentMethod_SelectedIndexChanged" >   
                       <ClientSideEvents  SelectedIndexChanged="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />                      
                </dx:ASPxComboBox>
            </td>   
            <td align="left" valign="bottom">
                <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" 
                    OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" 
                    OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" OnDataBound="grDate_DataBound" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>    
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="DocumentId" Name="DocumentId" Caption="Nr. document" ReadOnly="true" Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Angajat" ReadOnly="true" Width="200px"  />
                        <dx:GridViewDataTextColumn FieldName="DocumentState" Name="DocumentState" Caption="Stare document" ReadOnly="true" Width="100px" />
				        <dx:GridViewDataTextColumn FieldName="DocumentTypeName" Name="DocumentTypeName" Caption="Tip document" ReadOnly="true"  Width="200px" />
					    <dx:GridViewDataDateColumn FieldName="DocumentDate" Name="DocumentDate" Caption="Data document" ReadOnly="true"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>					    	
                        <dx:GridViewDataTextColumn FieldName="TotalAmount" Name="TotalAmount" Caption="Val. document" ReadOnly="true" Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="CurrencyCode" Name="CurrencyCode" Caption="CurrencyCode" ReadOnly="true" Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="UnconfRestAmount" Name="UnconfRestAmount" Caption="Total plata"  Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="BankName" Name="BankName" Caption="Banca" ReadOnly="true" Width="200px"  />
                        <dx:GridViewDataTextColumn FieldName="SucursalaName" Name="SucursalaName" Caption="Sucursala" ReadOnly="true" Width="200px"  />
                        <dx:GridViewDataTextColumn FieldName="TotalComissionPayment" Name="TotalComissionPayment" Caption="Comision bancar" ReadOnly="true" Width="100px"  />
                        <dx:GridViewDataTextColumn FieldName="PaymentCurrencyCode" Name="PaymentCurrencyCode" Caption="Valuta plata" ReadOnly="true" Width="100px"  />
                        <dx:GridViewDataTextColumn FieldName="BankNamePlatitor" Name="BankNamePlatitor" Caption="Banca platitor" ReadOnly="true" Width="200px"  />
					    <dx:GridViewDataDateColumn FieldName="PaymentDate" Name="PaymentDate" Caption="Data plata" Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>

                        <dx:GridViewDataTextColumn FieldName="DocumentStateId" Name="DocumentStateId" Caption="DocumentStateId" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="DocumentStateIdUpdate" Name="DocumentStateIdUpdate" Caption="DocumentStateIdUpdate" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="DocumentTypeId" Name="DocumentTypeId" Caption="DocumentTypeId" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="DocumentTypeCode" Name="DocumentTypeCode" Caption="DocumentTypeCode" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="CurrencyId" Name="CurrencyId" Caption="CurrencyId" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="BankId" Name="BankId" Caption="BankId" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="SucursalaId" Name="SucursalaId" Caption="SucursalaId" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="PaymentCurrencyId" Name="PaymentCurrencyId" Caption="PaymentCurrencyId" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="BankIdPlatitor" Name="BankIdPlatitor" Caption="BankIdPlatitor" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="BankCodePlatitor" Name="BankCodePlatitor" Caption="BankCodePlatitor" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="IBANPlatitor" Name="IBANPlatitor" Caption="IBANPlatitor" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="SrcDocId" Name="SrcDocId" Caption="SrcDocId" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="50px" Visible="false" />
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Anulare"  Image-ToolTip="Anulare">
                            <Styles>
                                <Style Font-Bold="true" />
                            </Styles>
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>
                    </SettingsCommandButton>                    
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table> 
</asp:Content>
