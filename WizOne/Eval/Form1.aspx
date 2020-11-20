<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Form1.aspx.cs" Inherits="WizOne.Eval.Form1" %>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

         <dx:ASPxCallbackPanel ID="pnlSectiune" ClientIDMode="Static" ClientInstanceName="pnlSectiune" ScrollBars="Vertical" runat="server" OnCallback="pnlSectiune_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents EndCallback="function (s,e) { pnlSectiune.PerformCallback('btnSave'); }"/>
        <PanelCollection>
            <dx:PanelContent>


                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" />
                    <ClientSideEvents Click="function(s,e) { grTabela_WXY_1008.UpdateEdit(); }" />
                </dx:ASPxButton>

    <dx:ASPxPanel id="divIntrebari" runat="server" style="height:50vh;"></dx:ASPxPanel>
            </dx:PanelContent>

        </PanelCollection>
    </dx:ASPxCallbackPanel>

</asp:Content>
