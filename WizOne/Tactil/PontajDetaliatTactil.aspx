<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajDetaliatTactil.aspx.cs" Inherits="WizOne.Tactil.PontajDetaliatTactil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>        
        function OnControlsInitialized(s, e) {          
            AdjustSize();      
        }

        function AdjustSize() {
            grDate.SetHeight(850);
            grDateTotaluri.SetHeight(850);
        }
    </script>
        <table style="width:100%;">
            <tr>
                <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
            </tr>
        </table>

        <table style="width:100%;">
            <tr>
                <td align="left">
                    <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="../Fisiere/Imagini/bdgBack.png"></Image>
                    </dx:ASPxButton>
                </td>
                <td align="left"><Label runat="server" id="lblMarca" style="font-weight: bold;font-size:20px">MARCA: </Label> </td>
                <td align="center"><Label runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</Label></td>
                <td align="right">
                    <dx:ASPxButton ID="btnLogOut" ClientInstanceName="btnLogOut" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Deconectare" AutoPostBack="true" OnClick="btnLogOut_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>

      <table width="30%">
            <tr>            
                <td>
                     <Label runat="server">Luna/An: </Label> 
                    <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="200px" DisplayFormatString="MM/yyyy" ButtonStyle-Width="75"  style="font-size:20px;" Height="30"  PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" >                           
                            <CalendarProperties FirstDayOfWeek="Monday" >                            
                       
                            </CalendarProperties>
                    </dx:ASPxDateEdit>
                </td>
                <td valign="bottom">
					<dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
						<Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>			
					</dx:ASPxButton>
                </td>
            </tr>
        </table>

    <table   >
        <tr>
            <td  align="center" valign="top" width="675">               
                <dx:ASPxHiddenField ID="hfRowIndex" runat="server" ClientInstanceName="hfRowIndex" ClientIDMode="Static"></dx:ASPxHiddenField>
                <dx:ASPxGridView ID="grDate"  runat="server" ClientInstanceName="grDate" ClientIDMode="Static" AutoGenerateColumns="false" 
                    OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlRowPrepared="grDate_HtmlRowPrepared" 
                    OnDataBound="grDate_DataBound" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomJSProperties="grDate_CustomJSProperties"   style="font-size:15px;left:auto">
                    <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="false" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="false" ColumnResizeMode="Control"  />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                    <SettingsSearchPanel Visible="false"  />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx"/>
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="Cheia" Caption=" " ReadOnly="true" Visible="true" ShowInCustomizationForm="true" FixedStyle="Left" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="Angajat" Caption="Angajat" ReadOnly="true" Width="150px" VisibleIndex="3" Visible="false" ShowInCustomizationForm="false" PropertiesTextEdit-ClientSideEvents-ValueChanged="" />

                        <dx:GridViewDataTextColumn FieldName="ZiLibera" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiLiberaLegala" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiSapt" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="F10022" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F10023" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdStare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Afisare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ValActive" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                    </Columns>
                    
                </dx:ASPxGridView>
            </td>
            <td  align="left" valign="top" id="tdTotaluri" runat="server" Width="300">
                 <dx:ASPxGridView ID="grDateTotaluri"  runat="server" ClientInstanceName="grDateTotaluri" ClientIDMode="Static" AutoGenerateColumns="false" style="font-size:15px;left:auto">
                    <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="false" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="false" ColumnResizeMode="Control"  />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                    <SettingsSearchPanel Visible="false"  />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx"/>
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="Id" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TextAfisare" Caption="Denumire" ReadOnly="true"  Width="200px" Visible="true" ShowInCustomizationForm="true" FixedStyle="Left" />
                        <dx:GridViewDataTextColumn FieldName="Valoare" Name="Valoare" Caption="Valoare" ReadOnly="true" Width="100px"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="An" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Luna" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                    </Columns>
                    
                </dx:ASPxGridView> 
            </td>
        </tr>
    </table>



    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>

</asp:Content>
