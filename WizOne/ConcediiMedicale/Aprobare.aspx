<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Aprobare.aspx.cs" Inherits="WizOne.ConcediiMedicale.Aprobare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {    
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id;IdStare', GoToEditMode);
                    break;
                case "btnArata":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToArataMode);
                    break;
            }
        }

        function GoToArataMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=20&id=' + Value, '_blank ');
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }

        function GoToDeleteMode(Value) {
            if (Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Nu puteti anula o cerere deja anulata",
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
                <dx:ASPxButton ID="btnRapCM" ClientInstanceName="btnRapCM" ClientIDMode="Static" runat="server" Text="Rapoarte" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        strUrl = getAbsoluteUrl + 'Personal/ListaDocumente.aspx?qwe=' + s.name;
                        popRap.SetHeaderText('Rapoarte');
                        popRap.SetContentUrl(strUrl);
                        popRap.Show();                       
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/arata.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnCalcul" runat="server" Text="Calcul medie CM" OnClick="btnCalcul_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAnulare" runat="server" Text="Sterge" OnClick="btnAnulare_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                </dx:ASPxButton>  
                <dx:ASPxButton ID="btnTransfera" runat="server" Text="Transfera" OnClick="btnTransfera_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/m5.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAdauga" runat="server" Text="Adauga CM" OnClick="btnAdauga_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/view.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 
    <br /> 
    <table width="20%">   
        <tr>
            <td align="left">
                <label id="lblAngFiltru" runat="server" visible="false" style="display:inline-block;">Angajat</label>
                <dx:ASPxComboBox ID="cmbAngFiltru" ClientInstanceName="cmbAngFiltru" Visible="false" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
                    </Columns>                            
                </dx:ASPxComboBox>
            </td>   

            <td align="left">
                <dx:ASPxButton ID="btnFiltru" Visible="false" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>
                   	
        </tr>
    </table>
    <br />
     <table width="80%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Modificare" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>     
                                <dx:GridViewCommandColumnCustomButton ID="btnArata">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton> 
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="150px" VisibleIndex="2" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="F30003" Name="F30003" Caption="Angajat" ReadOnly="true" Width="300px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown" >
                                <Columns>
                                    <dx:ListBoxColumn FieldName="F30003" Caption="Marca" Width="130px" />
                                    <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                </Columns>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="F300601" Name="F300601" Caption="Serie CM" ReadOnly="true" Width="100px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="F300602" Name="F300602" Caption="Numar CM" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="F300606" Name="F300606" Caption="Serie CM initial" ReadOnly="true" Width="120px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="F300608" Name="F300608" Caption="Numar CM initial" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
			            <dx:GridViewDataTextColumn FieldName="F300619" Name="F300619" Caption="Cod diagnostic" ReadOnly="true"  Width="120px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataDateColumn FieldName="F30037" Name="F30037" Caption="Data inceput" ReadOnly="true"  Width="120px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
					    <dx:GridViewDataDateColumn FieldName="F30038" Name="F30038" Caption="Data sfarsit" ReadOnly="true"  Width="100px" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>	
                        <dx:GridViewDataCheckColumn FieldName="Document" Name="Document" Caption="Document" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataCheckColumn>
                    
                        <dx:GridViewDataTextColumn FieldName="F300612" Name="F300612" Caption="BazaCalculCM" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />                        
                        <dx:GridViewDataTextColumn FieldName="F300613" Name="F300613" Caption="ZileBazaCalculCM" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F300620" Name="F300620" Caption="MedieZileBazaCalcul" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F300614" Name="F300614" Caption="MedieZilnicaCM" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F300624" Name="F300624" Caption="ModifManuala" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F30014" Name="F30014" Caption="Procent" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F300623" Name="F300623" Caption="Optiune" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="F30052" Name="F30052" Caption="Id" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false" />
                    </Columns>
                    <SettingsCommandButton>
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

        <dx:ASPxPopupControl ID="popRap" runat="server" AllowDragging="True" AllowResize="True"
            CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Middle"
            EnableViewState="False" PopupElementID="popupArea" PopupHorizontalAlign="WindowCenter"
            PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="False" Width="550px" MinHeight="500px"
            Height="100%" FooterText=" " CloseOnEscape="false" ClientInstanceName="popRap" EnableHierarchyRecreation="True">                
                <ContentCollection>
                    <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
                    </dx:PopupControlContentControl>
                </ContentCollection>
        </dx:ASPxPopupControl>

</asp:Content>
