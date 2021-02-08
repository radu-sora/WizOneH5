<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CursuriSesiuni.aspx.cs" Inherits="WizOne.Curs.CursuriSesiuni" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDoc":
                    grDate.GetRowValues(e.visibleIndex, 'IdCurs;Id', GoToDoc);
                    break;
                case "btnNomenclatorTraineri":
                    grDate.GetRowValues(e.visibleIndex, 'IdCurs;Id', GoToTrainer);
                    break;
                case "btnNomenclatorCosturiEstimat":
                    grDate.GetRowValues(e.visibleIndex, 'IdCurs;Id', GoToCostEstimat);
                    break;
                case "btnNomenclatorCosturiEfectiv":
                    grDate.GetRowValues(e.visibleIndex, 'IdCurs;Id', GoToCostEfectiv);
                    break;
   
            }
        }

        function GoToDoc(Value) {
            strUrl = getAbsoluteUrl + "Curs/relSesiuneDocumente.aspx?tip=1&qwe=" + Value;
            popGen.SetHeaderText("Documente");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToTrainer(Value) {
            strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=9&qwe=" + Value;
            popGen.SetHeaderText("Traineri");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        } 

        function GoToCostEfectiv(Value) {
            strUrl = getAbsoluteUrl + "Curs/relSesiuneCosturi.aspx?tip=1&qwe=" + Value;
            popGen.SetHeaderText("Cost efectiv");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }   

        function GoToCostEstimat(Value) {
            strUrl = getAbsoluteUrl + "Curs/relSesiuneCosturi.aspx?tip=0&qwe=" + Value;
            popGen.SetHeaderText("Cost estimat");
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
                <dx:ASPxButton ID="btnSave"  runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
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


    <table width="20%">   
        <tr>           
            <td align="left">
                <label id="lblCurs" runat="server" style="display:inline-block;">Selectare curs</label>
                <dx:ASPxComboBox ID="cmbCurs" runat="server" ClientInstanceName="cmbCurs" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >                           
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
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize"
                    OnRowUpdating="grDate_RowUpdating" OnRowInserting="grDate_RowInserting" OnRowDeleting="grDate_RowDeleting" OnInitNewRow="grDate_InitNewRow">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true"  SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="125px" VisibleIndex="1" ButtonType="Image" Caption=" " ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDoc" Visibility="BrowsableRow">
                                    <Image ToolTip="Documente" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons> 
                        </dx:GridViewCommandColumn>





                       
                        <dx:GridViewDataComboBoxColumn FieldName="TipSesiune" Name="TipSesiune" Caption="Tip sesiune"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire"   Width="200px"  />
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px" >         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px" >         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceputAprobare" Name="DataInceputAprobare" Caption="Data inc. aprobare"  Width="100px" >         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsitAprobare" Name="DataSfarsitAprobare" Caption="Data sf. aprobare"  Width="100px" >         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="CategSesiune" Name="CategSesiune" Caption="Categorie sesiune"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DataExpirare" Name="DataExpirare" Caption="Data expirare"   Width="100px" >         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora inceput"   Width="100px" >  
                        </dx:GridViewDataTimeEditColumn>
                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora sfarsit"   Width="100px" >   
                        </dx:GridViewDataTimeEditColumn>
                        <dx:GridViewDataTextColumn FieldName="OrePauzaMasa" Name="OrePauzaMasa" Caption="Ore pauza masa"   Width="50px"  />
                        <dx:GridViewDataTextColumn FieldName="TotalOre" Name="TotalOre" Caption="Total ore"   Width="50px"  />
                        <dx:GridViewDataTextColumn FieldName="NrMin" Name="NrMin" Caption="Nr min"   Width="50px"  />
                        <dx:GridViewDataTextColumn FieldName="NrMax" Name="NrMax" Caption="Nr max"   Width="50px"  />
                        <dx:GridViewDataComboBoxColumn FieldName="TematicaId" Name="TematicaId" Caption="Tematica"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="TematicaNume" Name="TematicaNume" Caption="Tematica nume"   Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataComboBoxColumn FieldName="InternExtern" Name="InternExtern" Caption="Intern/Extern"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="OrganizatorId" Name="OrganizatorId" Caption="Organizator"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="OrganizatorNume" Name="OrganizatorNume" Caption="Organizator nume"   Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataComboBoxColumn FieldName="TrainerId" Name="TrainerId" Caption="Trainer"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="IdUser" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="TrainerNume" Name="TrainerNume" Caption="Trainer nume"   Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewCommandColumn Name="colNomenclatorTrainer" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Trainer">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnNomenclatorTraineri" Visibility="BrowsableRow">
                                    <Image ToolTip="Trainer" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Locatie" Name="Locatie" Caption="Locatie"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataCheckColumn FieldName="Bugetat" Name="Bugetat" Caption="Bugetat"  Width="50px"  />   
                        <dx:GridViewDataTextColumn FieldName="Buget" Name="Buget" Caption="Buget"   Width="50px"  />
                        <dx:GridViewDataTextColumn FieldName="CodBuget" Name="CodBuget" Caption="Cod buget"   Width="100px"  />                        
                        <dx:GridViewDataTextColumn FieldName="CostEstimat" Name="CostEstimat" Caption="Cost estimat"   Width="100px"  /> 
                        <dx:GridViewCommandColumn Name="colNomenclatorCosturi" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Cost estimat">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnNomenclatorCosturiEstimat" Visibility="BrowsableRow">
                                    <Image ToolTip="Cost estimat" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdMoneda" Name="IdMoneda" Caption="Moneda"  Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="CostEfectiv" Name="CostEfectiv" Caption="Cost efectiv"   Width="100px"  />  
                        <dx:GridViewCommandColumn Name="colNomenclatorCosturiEfectiv" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Cost efectiv">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnNomenclatorCosturiEfectiv" Visibility="BrowsableRow">
                                    <Image ToolTip="Cost efectiv" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii"   Width="200px"  />   
                       <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare"  Width="100px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Materiale" Name="Materiale" Caption="Materiale training"   Width="200px"  />   
                       <dx:GridViewDataComboBoxColumn FieldName="IdQuiz" Name="IdQuiz" Caption="Chestionar feedback"  Width="100px">
                            <PropertiesComboBox TextField="Titlu" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdCurs" Name="IdCurs" Caption="IdCurs"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false"  ShowInCustomizationForm="false"/>
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
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                        </DeleteButton>
                        <NewButton Image-ToolTip="Rand nou">
                        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                        <Styles>
                            <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                        </Styles>
                       </NewButton>
                    </SettingsCommandButton> 
                  
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>    


</asp:Content>
