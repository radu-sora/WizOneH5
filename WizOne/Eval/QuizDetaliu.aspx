<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="QuizDetaliu.aspx.cs" Inherits="WizOne.Eval.QuizDetaliu" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    


    <table width="100%">
        <tr>
            <td align="left" />
            <td align="right" >
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { OnBtnSaveClick(s,e); }" />
                    <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="QuizLista.aspx">
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <div>


    <dx:ASPxPageControl SkinID="None" Width="100%"  EnableViewState="false" ID="ASPxPageControl2" ClientInstanceName="pageCtl"
        runat="server" ActiveTabIndex="0" TabSpacing="0px" CssClass="pcTemplates" EnableHierarchyRecreation="true">
        <ClientSideEvents EndCallback="function(s,e) { onGridEndCallback(s); }" />
        <Paddings Padding="30px" PaddingLeft="12px" />
        <ContentStyle Font-Names="Tahoma" Font-Overline="False" Font-Size="11px">
            <Paddings Padding="30px" />
            <Border BorderColor="#6DA0E7" BorderStyle="Solid" BorderWidth="1px" />
        </ContentStyle>
    </dx:ASPxPageControl>
   
    </div>

    <script>
        function OnBtnSaveClick(s, e) {
            pnlLoading.Show();
            if (typeof grDateOrdonare !== 'undefined')
                grDateOrdonare.UpdateEdit();
            
            pageCtl.PerformCallback();
        }

        function onGridEndCallback(s) {
            pnlLoading.Hide();
            if (s.cpAlertMessage) {
                swal({
                    title: "Atentie",
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }
        }
    </script>
</asp:Content>