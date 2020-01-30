<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="IstoricExtins.aspx.cs" Inherits="WizOne.Absente.IstoricExtins" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function LoadCtl(s)
        {
            var id = cmbViz.GetSelectedItem().value;
            switch (id) {
                case 1:
                    grDate.SetVisible(true);
                    grView.SetVisible(false);
                    document.getElementById('grAnual').style.display = "none";
                    grLunar.SetVisible(false);
                    //document.getElementById('lblLeg').style.display = "none";
                    //pnlLeg.SetVisible(false);
                    document.getElementById('<%= lblLuna.ClientID %>').style.display = "none";
                    txtLuna.SetVisible(false);
                    document.getElementById('<%= lblFil.ClientID %>').style.display = "none";
                    cmbFil.SetVisible(false);
                    btnLoad.SetVisible(false);
                    btnExport.SetVisible(false);
                    grLeg.MoveColumn(0, 0);
                    grLeg.SetVisible(false);
                    pnlLoading.Hide();
                    document.getElementById('<%= lblAn.ClientID %>').style.display = "none";
                    cmbAn.SetVisible(false);
                    break;
                case 2:
                    grDate.SetVisible(false);
                    grView.SetVisible(true);
                    document.getElementById('grAnual').style.display = "none";
                    grLunar.SetVisible(false);
                    //document.getElementById('lblLeg').style.display = "none";
                    //pnlLeg.SetVisible(false);
                    document.getElementById('<%= lblLuna.ClientID %>').style.display = "none";
                    txtLuna.SetVisible(false);
                    document.getElementById('<%= lblFil.ClientID %>').style.display = "none";
                    cmbFil.SetVisible(false);
                    btnLoad.SetVisible(false);
                    btnExport.SetVisible(false);
                    grLeg.MoveColumn(0, 0);
                    grLeg.SetVisible(false);
                    pnlLoading.Hide();
                    document.getElementById('<%= lblAn.ClientID %>').style.display = "none";
                    cmbAn.SetVisible(false);
                    break;
                case 3:
                    grDate.SetVisible(false);
                    grView.SetVisible(false);
                    document.getElementById('grAnual').style.display = "inline-block";
                    grLunar.SetVisible(false);
                    //document.getElementById('lblLeg').style.display = "inline-block";
                    //pnlLeg.SetVisible(true);
                    document.getElementById('<%= lblLuna.ClientID %>').style.display = "none";
                    txtLuna.SetVisible(false);
                    document.getElementById('<%= lblFil.ClientID %>').style.display = "none";
                    cmbFil.SetVisible(false);
                    btnLoad.SetVisible(true);
                    btnExport.SetVisible(false);
                    grLeg.MoveColumn(0,0);
                    grLeg.SetVisible(true);
                    pnlLoading.Hide();
                    document.getElementById('<%= lblAn.ClientID %>').style.display = "inline-block";
                    cmbAn.SetVisible(true);
                    break;
                case 4:
                    grDate.SetVisible(false);
                    grView.SetVisible(false);
                    document.getElementById('grAnual').style.display = "none";
                    grLunar.SetVisible(true);
                    //document.getElementById('lblLeg').style.display = "none";
                    //pnlLeg.SetVisible(false);
                    document.getElementById('<%= lblLuna.ClientID %>').style.display = "inline-block";
                    txtLuna.SetVisible(true);
                    document.getElementById('<%= lblFil.ClientID %>').style.display = "inline-block";
                    cmbFil.SetVisible(true);
                    btnLoad.SetVisible(true);
                    btnExport.SetVisible(true);
                    grLeg.MoveColumn(0);
                    grLeg.SetVisible(true);
                    pnlLoading.Hide();
                    document.getElementById('<%= lblAn.ClientID %>').style.display = "none";
                    cmbAn.SetVisible(false);
                    break;
                default:
                    pnlLoading.Hide();
                    break;
            }
        }
    </script>



    <script type="text/javascript">

        function OnGetRowValues(values) {
            pnlLoading.Show();

            var arr = document.getElementsByClassName("tag_" + values[0]);
            for (var idx = 0; idx < arr.length; ++idx)
            {
                if (arr[idx] != null)
                {
                    if (grLeg.IsRowSelectedOnPage(grLeg.GetFocusedRowIndex()))
                        arr[idx].style.background = values[1];
                    else
                        arr[idx].style.background = "#FFFFFF";
                }
            }

            var arr = document.getElementsByClassName("crs_" + values[0]);
            for (var idx = 0; idx < arr.length; ++idx) {
                if (arr[idx] != null) {
                    if (grLeg.IsRowSelectedOnPage(grLeg.GetFocusedRowIndex()))
                        arr[idx].style.background = "repeating-linear-gradient(45deg, #000000, #000000 1px," + values[1] + " 1px, " + values[1] + " 5px)";
                    else
                        arr[idx].style.background = "#FFFFFF";
                }
            }

            pnlLoading.Hide();
        }

    </script>








</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    
    <div style="width:100%; float:left; margin:20px 0px 0px 10px;">
        <dx:ASPxLabel ID="lblAngajat" runat="server" Font-Size="14px" Font-Bold="true" Font-Underline="false" Font-Italic="true" />    
    </div>

    <div class="Absente_divOuter">

        <div class="Absente_Cereri_CampuriSup">
            <label id="lblViz" runat="server" style="display:inline-block;">Vizualizare</label>
            <dx:ASPxComboBox ID="cmbViz" ClientInstanceName="cmbViz" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                <ClientSideEvents 
                    Init="function(s, e) { LoadCtl(s); }" 
                    SelectedIndexChanged="function(s, e) { 
                    pnlLoading.Show();
                    LoadCtl(s); 
                }" />
            </dx:ASPxComboBox>
        </div>


        <div class="Absente_Cereri_CampuriSup">
            <label id="lblLuna" runat="server" style="display:none;">Luna</label>
            <dx:ASPxDateEdit ID="txtLuna" ClientInstanceName="txtLuna" ClientIDMode="Static" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" ClientVisible="false" >
                <CalendarProperties FirstDayOfWeek="Monday" />
            </dx:ASPxDateEdit>
        </div>

        <div class="Absente_Cereri_CampuriSup">
            <label id="lblAn" runat="server" style="display:none;">Anul</label>
            <dx:ASPxComboBox ID="cmbAn" runat="server" ClientInstanceName="cmbAn" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100" 
                TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" ClientVisible="false" >
            </dx:ASPxComboBox>
        </div>

        <div class="Absente_Cereri_CampuriSup">
            <label id="lblFil" runat="server" style="display:none;">Filtru</label>
            <dx:ASPxComboBox ID="cmbFil" runat="server" ClientInstanceName="cmbFil" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" ClientVisible="false" >            
            </dx:ASPxComboBox>
        </div>

        <div id="pnlLoad" style="display:inline-block; margin-top:27px;">
            <dx:ASPxButton ID="btnLoad" ClientInstanceName="btnLoad" ClientIDMode="Static" runat="server" Text="Incarca" OnClick="btnLoad_Click" ClientVisible="false" oncontextMenu="ctx(this,event)">
                <ClientSideEvents Click="function(s, e) {
                    pnlLoading.Show();
                    e.processOnServer = true;
                }" />
                <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" Text="Export" AutoPostBack="true" ClientVisible="false" OnClick="btnExport_Click" oncontextMenu="ctx(this,event)" >
                <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
            </dx:ASPxButton>
        </div>

    </div>

    <br /><br />

    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="true" ClientVisible="false" >
        <Settings ShowGroupPanel="false" />
        <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="false" AllowSelectSingleRowOnly="false" />
        <ClientSideEvents ContextMenu="ctx" />
    </dx:ASPxGridView>

    <dx:ASPxGridView ID="grView" runat="server" ClientInstanceName="grView" ClientIDMode="Static" Width="100%" AutoGenerateColumns="true" ClientVisible="false" >
        <Settings ShowGroupPanel="false" />
        <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="false" AllowSelectSingleRowOnly="false" />
        <ClientSideEvents ContextMenu="ctx" />
    </dx:ASPxGridView>
    
    <table id="grAnual" runat="server" clientidmode="Static" style="display:none; border:none;">
    </table>

    <dx:ASPxGridView ID="grLunar" runat="server" ClientInstanceName="grLunar" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" ClientVisible="false" >
        <Settings ShowFilterRow="True" HorizontalScrollBarMode="Auto" />
        <Columns>
            <dx:GridViewDataTextColumn FieldName="Marca" Width="60px" />
            <dx:GridViewDataTextColumn FieldName="Angajat" Width="250px" Settings-AutoFilterCondition="Contains" />
            <dx:GridViewDataColorEditColumn FieldName="1" Caption="1" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="2" Caption="2" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="3" Caption="3" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="4" Caption="4" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="5" Caption="5" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="6" Caption="6" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="7" Caption="7" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="8" Caption="8" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="9" Caption="9" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="10" Caption="10" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="11" Caption="11" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="12" Caption="12" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="13" Caption="13" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="14" Caption="14" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %> "></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="15" Caption="15" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="16" Caption="16" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="17" Caption="17" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="18" Caption="18" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="19" Caption="19" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="20" Caption="20" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="21" Caption="21" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="22" Caption="22" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="23" Caption="23" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="24" Caption="24" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="25" Caption="25" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="26" Caption="26" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="27" Caption="27" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn FieldName="28" Caption="28" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn Caption="29" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn Caption="30" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColorEditColumn Caption="31" Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
        </Columns>
    </dx:ASPxGridView>
    
    <br />
    <br />

    <dx:ASPxGridView ID="grLeg" runat="server" ClientInstanceName="grLeg" ClientIDMode="Static" Width="500px" AutoGenerateColumns="true" ClientVisible="false" Caption="Legenda" >
        <Settings ShowGroupPanel="false" VerticalScrollBarMode="Visible" VerticalScrollableHeight="230" />
        <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" AllowSort="false" />
        <ClientSideEvents ContextMenu="ctx" />
        <SettingsPager Mode="ShowAllRecords" />
        <ClientSideEvents SelectionChanged="function(s, e) { grLeg.GetRowValues(grLeg.GetFocusedRowIndex(), 'Id;Culoare', OnGetRowValues); }" />
        <Columns>
            <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="None" CellStyle-CssClass="hide_column" />
            <dx:GridViewDataColorEditColumn FieldName="Culoare" Caption=" " Width="30px">
                <dataitemtemplate>
                    <div style="width:15px; height:15px; border: #9f9f9f 1px solid; background:<%#Container.Text %>"></div>
                </dataitemtemplate>
            </dx:GridViewDataColorEditColumn>
            <dx:GridViewDataColumn FieldName="Denumire" Caption="Absenta" Name="Denumire" /> 
            <dx:GridViewDataColumn FieldName="Id" Caption="Id" Name="Id" Visible="false" /> 
        </Columns>
    </dx:ASPxGridView>


    <dx:ASPxGridViewExporter GridViewID="grLunar" ID="ExportGrid" runat="server" />



</asp:Content>
