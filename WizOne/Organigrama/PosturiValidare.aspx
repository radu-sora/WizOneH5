<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PosturiValidare.aspx.cs" Inherits="WizOne.Organigrama.PosturiValidare" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
            <table width="100%">
                <tr>
                    <td align="left">
                        <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
                    </td>
                    <td align="right">
                        <dx:ASPxButton ID="btnValidare" runat="server" Text="Validare" OnClick="btnValidare_Click" oncontextMenu="ctx(this,event)">
                            <ClientSideEvents Click="function(s, e) {
                            pnlLoading.Show();
                            e.processOnServer = true;
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/validare.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                            <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajat</label>
                            <div style="float:left; padding-right:15px;">
                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                            CallbackPageSize="15" AllowNull="true" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                    </Columns>
                                </dx:ASPxComboBox>
                            </div>
                            <label id="lblSup" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Superior administrativ</label>
                            <div style="float:left; padding-right:15px;">    
                                <dx:ASPxComboBox ID="cmbSup" runat="server" Width="300px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" 
                                            IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Denumire" Caption="Post" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NivelIerarhic" Caption="Nivel" Width="50px" />
                                        <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompanie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                    </Columns>
                                </dx:ASPxComboBox>
                            </div>
                            <div style="float:left;">
                                <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                </dx:ASPxButton>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" OnCustomButtonCallback="grDate_CustomButtonCallback">
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
                            <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                            <SettingsSearchPanel Visible="True" />
                            <ClientSideEvents ContextMenu="ctx" />

                            <Columns>
                                <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />

                                <dx:GridViewCommandColumn Width="160px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                                    <CustomButtons>
                                        <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                            <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                        </dx:GridViewCommandColumnCustomButton>
                                        <dx:GridViewCommandColumnCustomButton ID="btnPrint">
                                            <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/print.png" />
                                        </dx:GridViewCommandColumnCustomButton>
                                    </CustomButtons>
                                </dx:GridViewCommandColumn>

                                <dx:GridViewDataComboBoxColumn FieldName="F10003" Caption="Marca" ReadOnly="true"/>
						        <dx:GridViewDataComboBoxColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true"/>
						        <dx:GridViewDataComboBoxColumn FieldName="IdPost" Caption="Id Post" ReadOnly="true"/>
                                <dx:GridViewDataComboBoxColumn FieldName="NumePost" Caption="Nume Post" ReadOnly="true"/>
                                <dx:GridViewDataComboBoxColumn FieldName="DataReferinta" Caption="Data Referinta" Width="100px" ReadOnly="true"/>
                                <dx:GridViewDataComboBoxColumn FieldName="Locatie" Caption="Locatie" ReadOnly="true"/>

                                <dx:GridViewDataCheckColumn FieldName="modifStructura" Caption="Structura" ReadOnly="true" />
                                <dx:GridViewDataCheckColumn FieldName="modifFunctie" Caption="Functia" ReadOnly="true" />
                                <dx:GridViewDataCheckColumn FieldName="modifCOR" Caption="COR" ReadOnly="true" />
                                <dx:GridViewDataCheckColumn FieldName="modifSalariu" Caption="Salariul" ReadOnly="true" />  
                                
                                <dx:GridViewDataComboBoxColumn FieldName="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
			                    <dx:GridViewDataComboBoxColumn FieldName="CodCOR" Caption="CodCOR" Visible="false" ShowInCustomizationForm="false"/>
                                <dx:GridViewDataComboBoxColumn FieldName="Nume" Caption="Nume"  Visible="false" ShowInCustomizationForm="false"/>
                                <dx:GridViewDataComboBoxColumn FieldName="Structura" Caption="Structura" Visible="false" ShowInCustomizationForm="false"/>
                            </Columns>
                        </dx:ASPxGridView>
                    </td>
                </tr>
            </table>



</asp:Content>
