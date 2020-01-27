<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="IntroDetaliu.aspx.cs" Inherits="WizOne.Pagini.IntroDetaliu" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function ShowWidgetPanel(widgetPanelUID) {     
            var panel = dockManager.GetPanelByUID(widgetPanelUID);
            var button = ASPxClientControl.GetControlCollection().GetByName('widgetButton_' + widgetPanelUID);
            if (button != null) {
                var currentClass = button.GetMainElement().className;
                if (currentClass.indexOf("disabled") == -1) {
                    panel.left = 150;
                    panel.top = 500;
                    panel.Show();
                }
                else
                    panel.Hide();
            }
            else
                panel.Show();
        }

        function SetWidgetButtonVisible(widgetName, visible) {        
            var button = ASPxClientControl.GetControlCollection().GetByName('widgetButton_' + widgetName);
            if (button != null) {
                var currentClass = button.GetMainElement().className;
                var newClass = ASPxClientUtils.Trim(visible ? currentClass.replace('disabled', '') : currentClass + ' disabled');
                button.GetMainElement().className = newClass;
            }
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
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <dx:ASPxPanel ID="ASPxPanel1" Collapsible="true" runat="server">
        <PanelCollection>
            <dx:PanelContent>
                <div class="div_hor">
                    <label id="lblId" runat="server" style="padding-left:0;">Id</label>
                    <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false"></dx:ASPxTextBox>
                    <label id="lblDenumire" runat="server">Denumire</label>
                    <dx:ASPxTextBox ID="txtDenumire" runat="server" Width="350px"></dx:ASPxTextBox>
                    <label id="lblActiv" runat="server">Activ</label>
                    <dx:ASPxCheckBox ID="chkActiv" runat="server" Checked="true" AllowGrayed="false" EnableViewState="false"></dx:ASPxCheckBox>
                </div>

                <div class="widgetPanel">
                    <dx:ASPxComboBox ID="cmbTip" runat="server" AutoPostBack="true" CssClass="cmbTip" OnSelectedIndexChanged="cmbTip_SelectedIndexChanged" />
                    <asp:DataList ID="lst" runat="server" RepeatDirection="Horizontal" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <div style="float:left; padding-right:0px; width:100%; text-align:center; width:100px;">
                                <dx:ASPxImage runat="server" ImageUrl='<%# string.Format("~/Fisiere/Imagini/Widgets/{0}.png", DataBinder.Eval(Container.DataItem,"RutaImg")) %>' Cursor="pointer"
                                    ClientInstanceName='<%# "widgetButton_" + DataBinder.Eval(Container.DataItem,"Nume") %>' ToolTip='<%# DataBinder.Eval(Container.DataItem,"Eticheta") %>'
                                    ClientSideEvents-Click='<%# GetClientButtonClickHandler(Container) %>' >
                                </dx:ASPxImage>
                                <label style="width:100%; display:inline-block;"><%# DataBinder.Eval(Container.DataItem,"Eticheta") %></label>
                            </div>
                        </ItemTemplate>
                    </asp:DataList>
                </div>
            </dx:PanelContent>
        </PanelCollection>
        <ExpandButtonTemplate>
            <div style="border-bottom:solid 1px #898989; vertical-align:bottom; line-height:25px;">
                <label id="lblCfg" runat="server" style="font-size:14px;">Date de configurare</label>
                <img src="../Fisiere/Imagini/Icoane/sgCls.png" style="display:inline-block; vertical-align:middle;" alt="ico"  />
            </div>
        </ExpandButtonTemplate>
        <SettingsCollapsing ExpandEffect="Slide" ExpandButton-Position="Far" ExpandButton-Visible="true" ExpandButton-GlyphType="ArrowBottom" AnimationType="Slide"></SettingsCollapsing>
    </dx:ASPxPanel>


    <dx:ASPxDockManager runat="server" ID="ASPxDockManager" ClientInstanceName="dockManager" OnClientLayout="dockManager_ClientLayout">
        <ClientSideEvents
            PanelShown="function(s, e) { SetWidgetButtonVisible(e.panel.panelUID, false) }"
            PanelCloseUp="function(s, e) { SetWidgetButtonVisible(e.panel.panelUID, true) }" />
    </dx:ASPxDockManager>

    <div id="divPanel" class="divPanel cuZIndex" runat="server"></div>


</asp:Content>
