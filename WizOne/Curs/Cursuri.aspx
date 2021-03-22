<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Cursuri.aspx.cs" Inherits="WizOne.Curs.Cursuri" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDoc":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDoc);   
                    break;
                case "btnComp":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToComp); 
                    break;
                case "btnTitl":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToTitl);
                    break;
                case "btnDept":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDept);
                    break;
                case "btnAng":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAng);
                    break;
                case "btnCursAnt":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCursAnt);
                    break;
                case "btnArata":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToArataMode);
                    break;
            }
        }


        function GoToArataMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=17&id=' + Value, '_blank ');
        }

        function GoToComp(Value) {
            var tipFunctii = parseInt("<%=Session["TipFunctiiCurs"] %>");

            if (parseInt(tipFunctii) == 1) {
                strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=1&qwe=" + Value;
                popGen.SetHeaderText("Competente");
                popGen.SetContentUrl(strUrl);
                popGen.Show();
            }
            else {
                strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=2&qwe=" + Value;
                popGen.SetHeaderText("Competente");
                popGen.SetContentUrl(strUrl);
                popGen.Show();
            }
        }

        function GoToAng(Value) {
            strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=3&qwe=" + Value;
            popGen.SetHeaderText("Angajati");
            popGen.SetContentUrl(strUrl);
            popGen.Show();   
        }

        function GoToTitl(Value) {
            var tipFunctii = parseInt("<%=Session["TipFunctiiCurs"] %>");

            if (parseInt(tipFunctii) == 1) {
                strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=4&qwe=" + Value;
                popGen.SetHeaderText("Functii");
                popGen.SetContentUrl(strUrl);
                popGen.Show();
            }
            else if (parseInt(tipFunctii) == 0) {
                strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=5&qwe=" + Value;
                popGen.SetHeaderText("Functii");
                popGen.SetContentUrl(strUrl);
                popGen.Show();
            }
            else {
                strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=6&qwe=" + Value;
                popGen.SetHeaderText("Functii");
                popGen.SetContentUrl(strUrl);
                popGen.Show();
            }
        }

        function GoToDept(Value) {
            strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=7&qwe=" + Value;
            popGen.SetHeaderText("Departamente");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCursAnt(Value) {
            strUrl = getAbsoluteUrl + "Curs/relCurs.aspx?tip=8&qwe=" + Value;
            popGen.SetHeaderText("Cursuri anterioare");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToDoc(Value) {
            if (parseInt(Value) <= 0) {
                swal({
                    title: "Atentie", text: "Lipsesc date: curs!",
                    type: "warning"
                });
            }
            else {
                Value = Value + ',-99';
                strUrl = getAbsoluteUrl + "Curs/relSesiuneDocumente.aspx?tip=1&qwe=" + Value;
                popGen.SetHeaderText("Documente");
                popGen.SetContentUrl(strUrl);
                popGen.Show();
            }
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
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnRowInserting="grDate_RowInserting" OnRowDeleting="grDate_RowDeleting" OnDataBinding="grDate_DataBinding"  OnInitNewRow="grDate_InitNewRow"  >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="125px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" ShowDeleteButton="true" ShowNewButtonInHeader="true" Caption=" " Name="butoaneGrid" >  
                            <CustomButtons>                     
                                <dx:GridViewCommandColumnCustomButton ID="btnArata">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>                                
                            </CustomButtons>                                               
                        </dx:GridViewCommandColumn>
                        <dx:GridViewCommandColumn Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Documente">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDoc" Visibility="BrowsableRow">
                                    <Image ToolTip="Documente" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>                       
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" Width="50px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire"  Width="150px" >       
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii"   Width="200px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Gradul" Name="Gradul" Caption="Gradul" Visible="false" Width="100px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Categ_Niv1Id" Name="Categ_Niv1Id" Caption="Categ nivel 1" Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Categ_Niv2Id" Name="Categ_Niv2Id" Caption="Categ nivel 2" Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Categ_Niv3Id" Name="Categ_Niv3Id" Caption="Categ nivel 3" Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewCommandColumn Name="colCompetente" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Competente">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnComp" Visibility="BrowsableRow">
                                    <Image ToolTip="Competente" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>  
                        <dx:GridViewCommandColumn Name="colTitluri" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Titluri">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnTitl" Visibility="BrowsableRow">
                                    <Image ToolTip="Titluri" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewCommandColumn Width="100px" Name="colDept" ButtonType="Image" ShowEditButton="false"  Caption="Departamente">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDept" Visibility="BrowsableRow">
                                    <Image ToolTip="Departamente" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewCommandColumn Name="colAngajati" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Angajati">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAng" Visibility="BrowsableRow">
                                    <Image ToolTip="Angajati" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewCommandColumn Name="colCursAnt" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Cursuri anterioare">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnCursAnt" Visibility="BrowsableRow">
                                    <Image ToolTip="Cursuri anterioare" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="Buget" Name="Buget" Caption="Buget"  Width="50px" />
                        <dx:GridViewDataTextColumn FieldName="FinalizareCurs" Name="FinalizareCurs" Caption="Finalizare curs"  Width="200px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="OrganizatorId" Name="OrganizatorId" Caption="Organizator" Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataCheckColumn FieldName="OrganizatorNume" Name="OrganizatorNume" Caption="Organizator Nume" Visible="false" ShowInCustomizationForm="false" Width="70px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataCheckColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdCategorieNota" Name="IdCategorieNota" Caption="Categorie"  Width="100px" >
                            <PropertiesComboBox TextField="DenumireCategorie" ValueField="IdCategorie" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                      
                        <dx:GridViewDataComboBoxColumn FieldName="TemplateDiploma" Name="TemplateDiploma" Caption="Diploma participare"  Width="150px" >
                            <PropertiesComboBox TextField="DescrDocument" ValueField="IdDocument" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataCheckColumn FieldName="Activ" Name="Activ" Caption="Activ"  Width="50px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataCheckColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="User_TrainerId" Name="User_TrainerId" Caption="Trainer"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="IdUser" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Recurenta" Name="Recurenta" Caption="Recurenta"  Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataCheckColumn FieldName="DateAditLaPlanificare" Name="DateAditLaPlanificare" Caption="Date aditionale"  Width="100px" Visible="false" ShowInCustomizationForm="false"  />
                        <dx:GridViewDataCheckColumn FieldName="Importat" Name="Importat" Caption="Anterior"  Width="50px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="MetodaLivrare" Name="MetodaLivrare" Caption="Metoda livrare curs"  Width="75px" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
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
