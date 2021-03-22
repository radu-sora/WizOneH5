<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CursuriView.aspx.cs" Inherits="WizOne.Curs.CursuriView" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnArata":
                    grDate.GetRowValues(e.visibleIndex, 'IdCurs', GoToArataMode);
                    break;
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

        function GoToArataMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=17&id=' + Value, '_blank ');
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

    <table width="20%">
            <tr>
                <td>
                    <label id="lblCurs" runat="server" style="display:inline-block;">De la</label>
                    <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataIncResource1" >
                        <CalendarProperties FirstDayOfWeek="Monday" />
                    </dx:ASPxDateEdit>
                </td>
                <td>
                    <label id="lblCurs2" runat="server" style="display:inline-block;">Pana la</label>
                    <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataSfResource1" >
                        <CalendarProperties FirstDayOfWeek="Monday" />
                    </dx:ASPxDateEdit>                    
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
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>      
                        <dx:GridViewCommandColumn Width="125px" VisibleIndex="0" ButtonType="Image"  Caption=" " Name="butoaneGrid" >  
                            <CustomButtons>                     
                                <dx:GridViewCommandColumnCustomButton ID="btnArata">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>                                
                            </CustomButtons>                                               
                        </dx:GridViewCommandColumn> 
                        <dx:GridViewDataTextColumn FieldName="Curs" Name="Curs" Caption="Curs" ReadOnly="true" Width="200px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Sesiune" Name="Sesiune" Caption="Sesiune nivel1" ReadOnly="true" Width="150px"  />
                        <dx:GridViewDataTextColumn FieldName="Categ_Niv1Nume" Name="Categ_Niv1Nume" Caption="Categ_Niv1" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Categ_Niv2Nume" Name="Categ_Niv2Nume" Caption="Categ_Niv2" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Categ_Niv3Nume" Name="Categ_Niv3Nume" Caption="Categ_Niv3" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  ReadOnly="true" Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  ReadOnly="true" Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora inceput"  ReadOnly="true" Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="HH:mm"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora sfarsit"  ReadOnly="true" Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="HH:mm"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="Tematica" Name="Tematica" Caption="Tematica" ReadOnly="true" Width="150px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Organizator" Name="Organizator" Caption="Organizator" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Trainer" Name="Trainer" Caption="Trainer" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataTextColumn FieldName="Locatie" Name="Locatie" Caption="Locatie" ReadOnly="true" Width="100px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="FinalizareCurs" Name="FinalizareCurs" Caption="FinalizareCurs" ReadOnly="true" Width="100px">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii"  Width="200px"  >                       
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataTextColumn FieldName="StareSesiune" Name="StareSesiune" Caption="Stare sesiune"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="IdCurs" Name="IdCurs" Caption="IdCurs"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdSesiune" Name="IdSesiune" Caption="IdSesiune"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                    </Columns>                     
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>    

    

</asp:Content>
