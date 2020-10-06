<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Diagrama.aspx.cs" Inherits="WizOne.Organigrama.Diagrama" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxHiddenField ID="hf" runat="server" ClientIDMode="Static" ClientInstanceName="hf" />
    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" Text="Exporta document" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) { OnExport(s,e); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnModifStruc" ClientInstanceName="btnModifStruc" ClientIDMode="Static" runat="server" Text="Modifica" AutoPostBack="true" OnClick="btnModif_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/schimba.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNou" ClientInstanceName="btnNou" ClientIDMode="Static" runat="server" Text="Adauga" OnClick="btnNou_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                    <label id="lblDtVig" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Data selectie</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit id="txtDtVig" ClientIDMode="Static" ClientInstanceName="txtDtVig" runat="server" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" Width="100px" />
                    </div>

                    <label id="lblActiv" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Post activ</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxCheckBox id="chkActiv" runat="server" TextAlign="Left" />
                    </div>

                    <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajati</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbAng" runat="server" Width="130px">
                            <Items>
                                <dx:ListEditItem Text="Toti" Value="4" />
                                <dx:ListEditItem Text="Activi" Value="1" Selected="true" />
                                <dx:ListEditItem Text="Activi suspendati" Value="2" />
                                <dx:ListEditItem Text="Inactivi" Value="3" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>


                    <label id="lblParinte" runat="server" style="display:inline-block; float:left; padding-right:15px;">Superior</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbParinte" runat="server" Width="130px" ValueField="Camp" TextField="Denumire" />
                    </div>


                    <div style="float:left;">
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExpand" runat="server" Text="Expand" OnClick="btnExpand_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/stare.png"></Image>
                        </dx:ASPxButton>
                    </div>
                    <div style="float:left; line-height:16px; vertical-align:middle; margin-top:5px;">
                        <label style="display:inline-block; float:left; padding:0px 15px;">Legenda angajati: Activ</label>
                        <div style="width:16px; height:16px; background-color:#c8ffc8;float:left; margin-left:0px; border:solid 2px #e6e6e6;"></div>
                        <label style="display:inline-block; float:left; padding:0px 15px;">Activ suspendat</label>
                        <div style="width:16px; height:16px; background-color:#ffffc8;float:left; margin-left:0px; border:solid 2px #e6e6e6;"></div>
                        <label style="display:inline-block; float:left; padding:0px 15px;">Inactiv</label>
                        <div style="width:16px; height:16px; background-color:#ffc8c8;float:left; margin-left:0px; border:solid 2px #e6e6e6;"></div>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <dx:ASPxPanel ID="pnlCont" runat="server">
                    
                </dx:ASPxPanel>

            </td>
        </tr>
    </table>



</asp:Content>
