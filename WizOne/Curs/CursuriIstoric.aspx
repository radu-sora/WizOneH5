<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CursuriIstoric.aspx.cs" Inherits="WizOne.Curs.CursuriIstoric" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnNomenclatorTraineri":
                    grDate.GetRowValues(e.visibleIndex, 'IdCurs;IdSesiune', GoToTrainer); 
                    break;
            }
        }

        function GoToTrainer(Value) {
            strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=9&qwe=" + Value;
            popGen.SetHeaderText("Traineri");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
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
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 

    <br /> 

    <table width="50%">
        <tr>
            <td>
                <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="215px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" 
                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource1" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource2" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource3" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource4" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource5" />
                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource6" />
                    </Columns>
                    <ClientSideEvents />
                </dx:ASPxComboBox>
            </td>
            <td>
                <label id="lblCurs" runat="server" style="display:inline-block;">Curs</label>
                <dx:ASPxComboBox ID="cmbCurs" runat="server" ClientInstanceName="cmbCurs" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  >
                    <ClientSideEvents SelectedIndexChanged="function(s, e) {
                         cmbSesiune.PerformCallback(s.GetValue()); }" />
                </dx:ASPxComboBox>
            </td>
            <td>
                <label id="lblSesiune" runat="server" style="display:inline-block;">Sesiune</label>
                <dx:ASPxComboBox ID="cmbSesiune" runat="server" ClientInstanceName="cmbSesiune" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  OnCallback="cmbSesiune_Callback" >
                </dx:ASPxComboBox>
            </td>
        </tr>
            <tr>
               <td>
                    <label id="lblCateg_Niv1" runat="server" style="display:inline-block;">Categ niv1</label>
                    <dx:ASPxComboBox ID="cmbCateg_Niv1" runat="server" ClientInstanceName="cmbCateg_Niv1" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                        TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  >
                    </dx:ASPxComboBox>
                </td>
                <td>
                    <label id="lblCateg_Niv2" runat="server" style="display:inline-block;">Categ niv2</label>
                    <dx:ASPxComboBox ID="cmbCateg_Niv2" runat="server" ClientInstanceName="cmbCateg_Niv2" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                        TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  >
                    </dx:ASPxComboBox>
                </td>
                  <td>
                    <label id="lblDepartament" runat="server" style="display:inline-block;">Departament</label>
                    <dx:ASPxComboBox ID="cmbDepartament" runat="server" ClientInstanceName="cmbDepartament" ClientIDMode="Static" Width="215px" ValueField="F00607" DropDownWidth="200" 
                        TextField="F00608" ValueType="System.Int32" AutoPostBack="false"  >
                    </dx:ASPxComboBox>
                </td>
                <td align="left">
                    <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                    </dx:ASPxButton>
                </td>
                <td align="left">
                    <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltruSterge_Click" >                    
                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                    </dx:ASPxButton>
                </td>  
            </tr>
    </table>

    <br />
     <table width="80%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>                  
                        <dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="200px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Categ_Niv1" Name="Categ_Niv1" Caption="Categ nivel1" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Categ_Niv2" Name="Categ_Niv2" Caption="Categ nivel2" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Departament" Name="Departament" Caption="Departament" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Curs" Name="Curs" Caption="Curs" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Sesiune" Name="Sesiune" Caption="Sesiune" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Organizator" Name="Organizator" Caption="Organizator" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Trainer" Name="Trainer" Caption="Trainer" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewCommandColumn Name="colNomenclatorTrainer" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Trainer">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnNomenclatorTraineri" Visibility="AllDataRows">
                                    <Image ToolTip="Trainer" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataTextColumn FieldName="Certificat" Name="Certificat" Caption="Certificat" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Stare" Name="Stare" Caption="Stare" ReadOnly="true" Width="100px">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Nota" Name="Nota" Caption="Nota"  Width="100px"  >                       
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataTextColumn FieldName="Planificare_CostRONcuTVA" Name="Planificare_CostRONcuTVA" Caption="CostRONcuTVA"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Planificare_Denumire" Name="Planificare_Denumire" Caption="Denumire"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Planificare_Organizator" Name="Planificare_Organizator" Caption="Organizator2"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="IdCurs" Name="IdCurs" Caption="IdCurs"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdSesiune" Name="IdSesiune" Caption="IdSesiune"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                    </Columns>                     
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>    

    

</asp:Content>
