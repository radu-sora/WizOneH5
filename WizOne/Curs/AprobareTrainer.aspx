<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="AprobareTrainer.aspx.cs" Inherits="WizOne.Curs.AprobareTrainer" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">       
 

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnChangingSession(s, e) {
            lblStare.value = "";
            if (s.GetValue != null)
            {
                var idStare = 0;
                var ses = "<%=Session["AprobareTrainer_SesiuniSir"] %>" ;
                var res = ses.split(";");               
                for (var i = 0; i < res.length; i++) {
                    var linie = res[i].split(",");
                    if (linie[0] == s.GetValue()) {            
                        idStare = Number(linie[1]);
                    }
                }
               
                var txt = "";
                switch (idStare)
                {
                    case 1:
                        txt = "initiata";
                        break;
                    case 2:
                        txt = "deschisa pt inscrieri";
                        break;
                    case 3:
                        txt = "inchisa pt inscrieri";
                        break;
                    case 4:
                        txt = "finalizata";
                        break;
                    case 5:
                        txt = "anulata";
                        break;
                }

                lblStare.value = "Sesiune " + txt;
                
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
                <dx:ASPxButton ID="btnPrint" runat="server" Text="Imprima" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" > 
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />                      
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnFinalizare" runat="server" Text="Finalizare sesiune" OnClick="btnFinalizare_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
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
    <table width="35%">   
        <tr>
            <td id="divRol" runat="server">
                <label id="lblCurs" runat="server" style="display:inline-block;">Curs</label>
                <dx:ASPxComboBox ID="cmbCurs" ClientInstanceName="cmbCurs" ClientIDMode="Static" runat="server"  Width="200px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                    <ClientSideEvents SelectedIndexChanged="function(s, e) {                      
                         cmbSesiune.PerformCallback(s.GetValue()); }" />
                </dx:ASPxComboBox>
            </td>
 
             <td runat="server">
                <label id="lblSesiune" runat="server" style="display:inline-block;">Sesiune</label>
                <dx:ASPxComboBox ID="cmbSesiune" ClientInstanceName="cmbSesiune" ClientIDMode="Static" runat="server" Width="200px" ValueField="Id" OnCallback="cmbSesiune_Callback" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                    <ClientSideEvents SelectedIndexChanged="OnChangingSession" />
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
			<td align="left">		
                <label id="lblStare" runat="server"  style="display:inline-block;"></label>	
            </td>           
                               	
        </tr>
    </table>
    <br />
     <table width="50%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnDataBinding="grDate_DataBinding"  OnRowUpdating="grDate_RowUpdating">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>     
                        <dx:GridViewCommandColumn Width="125px" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" />                                            
                        <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajatul" ReadOnly="true" Width="180px" >
                            <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
					    <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii" ReadOnly="true"  Width="350px" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdCategValoareNota" Name="IdCategValoareNota" Caption="Nota" ReadOnly="true" Width="150px"  >
                            <PropertiesComboBox TextField="DenumireValoare" ValueField="IdCategValoare" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataCheckColumn FieldName="PrezentCurs" Name="PrezentCurs" Caption="Prezent"  Width="100px"  />

                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="75px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" ReadOnly="true" Width="75px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="IdStare" Name="IdStare" Caption="IdStare" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false" />
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
