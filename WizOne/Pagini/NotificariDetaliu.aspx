<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotificariDetaliu.aspx.cs" Inherits="WizOne.Pagini.NotificariDetaliu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notificari Detaliu</title>

    <link type="text/css" rel="stylesheet" href="../fisiere/css/style.css" />
    <link type="text/css" rel="stylesheet" href="../fisiere/css/teme.css" />
    <link type="text/css" rel="stylesheet" href="../fisiere/css/widgets.css" />
    <script type="text/javascript" src="../Fisiere/js/utils.js"></script>

    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

    <script type="text/javascript">
        function cmbCol_SelectedIndexChanged_Client(s, e) {
            var lstCond1 = ["<>", ">", ">=", "<", "<=", "=", "intre", "in", "not in",  "fara valoare", "cu valoare"];
            var lstCond2 = ["incepe cu", "contine", "se termina cu"];

            cmbCond.ClearItems();
            if (s.GetSelectedItem().GetColumnText('TipData').toLowerCase() == 'string') {
                for (var i = 0; i < lstCond2.length; ++i) {
                        cmbCond.AddItem(lstCond2[i], lstCond2[i]);
                }
            }
            else
                for (var i = 0; i < lstCond1.length; ++i) {
                        cmbCond.AddItem(lstCond1[i], lstCond1[i]);
                }

            txtVal.SetEnabled(false);
            cmbVal.SetEnabled(false);
            cmbValData.SetEnabled(false);
            txtZile.SetEnabled(false);
            txtVal2.SetEnabled(false);
            cmbVal2.SetEnabled(false);
            cmbValData2.SetEnabled(false);
            txtZile2.SetEnabled(false);
        }

        var __focusedCtl = null;
        function onGotFocus(s, e) {
            __focusedCtl = s;
        }

        function btnAddCmp_Click_Client()
        {
            if (!__focusedCtl) return;
            if (cmbAddCmp.GetText() == '') return;
            switch (__focusedCtl.name)
            {
                case "pnlCorp_txtContinut":
                    txtContinut.ExecuteCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, "#$" + cmbAddCmp.GetText() + "$#");
                    break;
                case "pnlAtasament_txtNume":
                    txtNume.SetText(txtNume.GetText() + "#$" + cmbAddCmp.GetText() + "$#");
                    break;
                case "pnlAtasament_txtAtt":
                    txtAtt.ExecuteCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, "#$" + cmbAddCmp.GetText() + "$#");
                    break;
                case "pnlXLS_txtExcel":
                    txtExcel.SetText(txtExcel.GetText() + "#$" + cmbAddCmp.GetText() + "$#");
                    break;
                case "txtSubiect":
                    txtSubiect.SetText(txtSubiect.GetText() + "#$" + cmbAddCmp.GetText() + "$#");
                    break;
            }
        }

        function cmbVal_SelectedIndexChanged_Client()
        {
            txtVal.SetText(cmbVal.GetText());
        }

        function cmbVal2_SelectedIndexChanged_Client() {
            txtVal2.SetText(cmbVal2.GetText());
        }

        function cmbValData_SelectedIndexChanged_Client() {
            txtVal.SetText(cmbValData.GetText());
        }

        function cmbValData2_SelectedIndexChanged_Client() {
            txtVal2.SetText(cmbValData2.GetText());
        }


        function cmbCond_SelectedIndexChanged_Client() {
            switch(cmbCol.GetSelectedItem().GetColumnText('TipData').toLowerCase())
            {
                case "string":
                    {
                        txtVal.SetEnabled(true);
                        cmbVal.SetEnabled(true);
                        cmbValData.SetEnabled(false);
                        txtZile.SetEnabled(false);
                        txtVal2.SetEnabled(false);
                        cmbVal2.SetEnabled(false);
                        cmbValData2.SetEnabled(false);
                        txtZile2.SetEnabled(false);
                    }
                    break;
                case "datetime":
                    {
                        switch (cmbCond.GetText().toLowerCase()) {
                            case "se modifica":
                            case "fara valoare":
                            case "cu valoare":
                                {
                                    txtVal.SetEnabled(false);
                                    cmbVal.SetEnabled(false);
                                    cmbValData.SetEnabled(false);
                                    txtZile.SetEnabled(false);
                                    txtVal2.SetEnabled(false);
                                    cmbVal2.SetEnabled(false);
                                    cmbValData2.SetEnabled(false);
                                    txtZile2.SetEnabled(false);
                                }
                                break;
                            case "intre":
                                {
                                    txtVal.SetEnabled(true);
                                    cmbVal.SetEnabled(true);
                                    cmbValData.SetEnabled(true);
                                    txtZile.SetEnabled(true);
                                    txtVal2.SetEnabled(true);
                                    cmbVal2.SetEnabled(true);
                                    cmbValData2.SetEnabled(true);
                                    txtZile2.SetEnabled(true);
                                }
                                break;
                            default:
                                {
                                    txtVal.SetEnabled(true);
                                    cmbVal.SetEnabled(true);
                                    cmbValData.SetEnabled(true);
                                    txtZile.SetEnabled(true);
                                    txtVal2.SetEnabled(false);
                                    cmbVal2.SetEnabled(false);
                                    cmbValData2.SetEnabled(false);
                                    txtZile2.SetEnabled(false);
                                }
                                break;
                        }
                    }
                    break;
                case "int":
                    {
                        switch (cmbCond.GetText().toLowerCase())
                        {
                            case "se modifica":
                            case "fara valoare":
                            case "cu valoare":
                                {
                                    txtVal.SetEnabled(false);
                                    cmbVal.SetEnabled(false);
                                    cmbValData.SetEnabled(false);
                                    txtZile.SetEnabled(false);
                                    txtVal2.SetEnabled(false);
                                    cmbVal2.SetEnabled(false);
                                    cmbValData2.SetEnabled(false);
                                    txtZile2.SetEnabled(false);
                                }
                                break;
                            case "intre":
                                {
                                    txtVal.SetEnabled(true);
                                    cmbVal.SetEnabled(true);
                                    cmbValData.SetEnabled(false);
                                    txtZile.SetEnabled(false);
                                    txtVal2.SetEnabled(true);
                                    cmbVal2.SetEnabled(true);
                                    cmbValData2.SetEnabled(false);
                                    txtZile2.SetEnabled(false);
                                }
                                break;
                            default:
                                {
                                    txtVal.SetEnabled(true);
                                    cmbVal.SetEnabled(true);
                                    cmbValData.SetEnabled(false);
                                    txtZile.SetEnabled(false);
                                    txtVal2.SetEnabled(false);
                                    cmbVal2.SetEnabled(false);
                                    cmbValData2.SetEnabled(false);
                                    txtZile2.SetEnabled(false);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

    </script>


</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="sm1" runat="server"></asp:ScriptManager>

        <div style="width:100%; text-align:right; margin-bottom:10px;">
            <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" >
                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
            </dx:ASPxButton>
        </div>



        <table style="width:100%;">
            <tr style="padding:20px !important;">
                <td><label id="lblId" runat="server" style="padding-left:0;">Id</label></td>
                <td><dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false"></dx:ASPxTextBox></td>
                <td><label id="lblDenumire" runat="server">Denumire</label></td>
                <td><dx:ASPxTextBox ID="txtDenumire" runat="server" Width="275px"></dx:ASPxTextBox></td>
                <td> <label id="lblActiv" runat="server">Activ</label></td>
                <td><dx:ASPxCheckBox ID="chkActiv" runat="server" Checked="true" AllowGrayed="false" EnableViewState="false"></dx:ASPxCheckBox></td>
                <td><label id="lblAddCmp" runat="server">Camp</label></td>
                <td><dx:ASPxComboBox ID="cmbAddCmp" runat="server" ClientInstanceName="cmbAddCmp" AutoPostBack="false" CssClass="cmbTip" Width="170px" ValueField="Id" ValueType="System.Int32" TextField="Denumire" /></td>
                <td><label style="padding-left:0; padding-right:0;">&nbsp;</label></td>
                <td>
                <dx:ASPxButton ID="btnAddCmp" runat="server" Width="30px" AutoPostBack="false" ToolTip="Adauga camp in continutul notificarii">
                    <Image Url="~/Fisiere/Imagini/Icoane/adauga.png"></Image>
                    <ClientSideEvents Click="btnAddCmp_Click_Client" />
                </dx:ASPxButton>
                    </td>
                <td><label style="padding-left:0; padding-right:0;">&nbsp;</label></td>
            </tr>
        </table>
        <br />
        

        <dx:ASPxRoundPanel ID="pnlMail" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Trimite mail la" CssClass="pnlAlign indentright20" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>

                    <div class="div_ver">

                        <dx:ASPxGridView ID="grDateMail" runat="server" ClientInstanceName="grDateMail" Width="100%" AutoGenerateColumns="false" OnRowDeleting="grDateMail_RowDeleting" OnRowUpdating="grDateMail_RowUpdating" OnCustomCallback="grDateMail_CustomCallback" OnInitNewRow="grDateMail_InitNewRow" OnRowInserting="grDateMail_RowInserting" OnHtmlEditFormCreated="grDateMail_HtmlEditFormCreated" >
                            <SettingsBehavior AllowFocusedRow="true" />
                            <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                            <SettingsSearchPanel Visible="False" />
                            <Columns>
                                <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" Visible="false" />
                                <dx:GridViewDataTextColumn FieldName="MailTip" Name="Tip" Caption="Tip" Width="80px" />
                                <dx:GridViewDataTextColumn FieldName="MailAdresaId" Name="AdresaId" Caption="Adresa Id" Width="200px" Visible="false" />
                                <dx:GridViewDataTextColumn FieldName="MailAdresaText" Name="Adresa" Caption="Adresa" Width="200px" />
                                <dx:GridViewDataTextColumn FieldName="MailDestinatie" Name="Destinatie" Caption="Des" Width="50px" />
                                <dx:GridViewDataCheckColumn FieldName="IncludeLinkAprobare" Name="Link" Caption=" " Width="25px" />
                                <dx:GridViewCommandColumn Width="50px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="4" ButtonType="Image" Caption=" " />
                            </Columns>

                            <SettingsCommandButton>
                                <UpdateButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                    <Styles>
                                        <Style Paddings-PaddingRight="5px" />
                                    </Styles>
                                </UpdateButton>
                                <CancelButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                </CancelButton>

                                <EditButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
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


                            <Templates>
                                <EditForm>
                                    <div style="padding: 4px 3px 4px">
                                        <table>
                                            <tr>
                                                <td><label id="lblTip" runat="server">Tip</label> </td>
                                                <td><label id="lblAdr" runat="server">Adresa</label> </td>
                                                <td><label id="lblDes" runat="server">Destinatie</label> </td>
                                                <td><label id="lblIncLink" runat="server">Link</label></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dx:ASPxComboBox ID="cmbTip" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="120px">
                                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { grDateMail.PerformCallback('cmbTip'); }" />
                                                        <Items>
                                                            <dx:ListEditItem Text = "mail" Value = "mail" Selected = "true" />
                                                            <dx:ListEditItem Text = "user" Value = "user" />
                                                            <dx:ListEditItem Text = "angajat" Value = "angajat" />
                                                            <dx:ListEditItem Text = "grup user" Value = "grup user" />
                                                            <dx:ListEditItem Text = "grup angajati" Value = "grup angajati" />
                                                            <dx:ListEditItem Text = "coloana tabel" Value = "coloana tabel" />
                                                        </Items>
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td>
                                                    <dx:ASPxTextBox ID="txtAdr" runat="server" Width="215px" />
                                                    <dx:ASPxComboBox ID="cmbAdr" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="215px" ValueField="Id" ValueType="System.Int32" TextField="Denumire" Visible="false" />
                                                </td>
                                                <td>
                                                    <dx:ASPxComboBox ID="cmbDes" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="60px" >
                                                        <Items>
                                                            <dx:ListEditItem Text="TO" Value="TO" Selected="true" />
                                                            <dx:ListEditItem Text="CC" Value="CC" />
                                                            <dx:ListEditItem Text="BCC" Value="BCC" />
                                                        </Items>
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td>
                                                    <dx:ASPxCheckBox ID="chkLink" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                        <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                        <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                    </div>
                                </EditForm>
                            </Templates>


                        </dx:ASPxGridView>

                    </div>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>

        <dx:ASPxRoundPanel ID="pnlMesaj" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" Visible="false" Width="65%" HeaderText="Permite validarea" CssClass="pnlAlign indentright20" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>

                    <div class="div_hor">
                        <label id="lblArata" runat="server">Arata</label>
                        <dx:ASPxComboBox ID="cmbMesaj" runat="server" AutoPostBack="false" CssClass="cmbTip indentBottom10" Width="220px" >
                            <Items>
                                <dx:ListEditItem Text="mesaj de eroare" Value="mesaj de eroare" Selected = "true"  />
                                <dx:ListEditItem Text="avertisment" Value="avertisment" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>

        <dx:ASPxRoundPanel ID="pnlValid" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Valid pentru" CssClass="pnlAlign" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>

                    <asp:UpdatePanel ID="up2" runat="server" OnUnload="up2_Unload">
                        <ContentTemplate>

                            <div class="div_ver">
                                <dx:ASPxComboBox ID="cmbValid" runat="server" AutoPostBack="true" CssClass="cmbTip indentBottom10" Width="220px" OnSelectedIndexChanged="cmbValid_SelectedIndexChanged">
                                    <Items>
                                        <dx:ListEditItem Text="Toti users" Value="0" Selected = "true"  />
                                        <dx:ListEditItem Text="Grup users" Value="1" />
                                        <dx:ListEditItem Text="User" Value="2" />
                                    </Items>
                                </dx:ASPxComboBox>
                                <dx:ASPxComboBox ID="cmbValidValoare" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="220px" ValueField="Id" ValueType="System.Int32" TextField="Denumire" Enabled="false" />
                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cmbValid" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>

        <dx:ASPxRoundPanel ID="pnlCorp" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" Width="100%" HeaderText="Text mail" CssClass="pnlAlign indentTop10" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>
                    <table>
                        <tr>
                            <td><label id="lblSubiect" runat="server">Subiect</label></td>
                            <td>
                                <dx:ASPxTextBox ID="txtSubiect" runat="server" ClientInstanceName="txtSubiect" ClientIDMode="Static" Width="390px">
                                    <ClientSideEvents GotFocus="onGotFocus" />    
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                    </table>

                    <dx:ASPxHtmlEditor ID="txtContinut" runat="server" ClientInstanceName="txtContinut" Height="370px" Width="100%">
                        <ClientSideEvents GotFocus="onGotFocus" />
                        <SettingsDialogs>
                            <InsertImageDialog>
                                <SettingsImageUpload UploadFolder="~/UploadFiles/Images/">
                                    <ValidationSettings AllowedFileExtensions=".jpe,.jpeg,.jpg,.gif,.png" MaxFileSize="500000">
                                    </ValidationSettings>
                                </SettingsImageUpload>
                            </InsertImageDialog>
                        </SettingsDialogs>
                    </dx:ASPxHtmlEditor>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>

        <dx:ASPxRoundPanel ID="pnlFiltru" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Filtru dupa" Width="100%" CssClass="pnlAlign indentTop10" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>

                    <div class="div_ver">

                        <dx:ASPxGridView ID="grDateCond" runat="server" ClientInstanceName="grDateCond" Width="100%" AutoGenerateColumns="false" OnRowDeleting="grDateCond_RowDeleting" OnRowUpdating="grDateCond_RowUpdating" OnCustomCallback="grDateCond_CustomCallback" OnInitNewRow="grDateCond_InitNewRow" OnRowInserting="grDateCond_RowInserting" OnHtmlRowCreated="grDateCond_HtmlRowCreated" >
                            <SettingsBehavior AllowFocusedRow="true" />
                            <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowColumnHeaders="true" />
                            <SettingsSearchPanel Visible="False" />
                            <Columns>
                                <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" Visible="false" />
                                <dx:GridViewDataTextColumn FieldName="Coloana" Name="Coloana" Caption="Coloana" Width="90px" />
                                <dx:GridViewDataTextColumn FieldName="Operator" Name="Operator" Caption="Operator" Width="110px" />
                                <dx:GridViewDataTextColumn FieldName="Valoare1" Name="Valoare1" Caption="Valoare" Width="120px" />
                                <dx:GridViewDataTextColumn FieldName="NrZile1" Name="NrZile1" Caption="Nr zile" Width="30px" />
                                <dx:GridViewDataTextColumn FieldName="Valoare2" Name="Valoare2" Caption="Valoare 2" Width="120px" />
                                <dx:GridViewDataTextColumn FieldName="NrZile2" Name="NrZile2" Caption="Nr zile 2" Width="30px" />
                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" />
                                <dx:GridViewCommandColumn Width="24px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" ButtonType="Image" Caption=" " >
                                </dx:GridViewCommandColumn>
                            </Columns>

                            <SettingsCommandButton>

                                <UpdateButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                    <Styles>
                                        <Style Paddings-PaddingRight="5px" />
                                    </Styles>
                                </UpdateButton>
                                <CancelButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                </CancelButton>

                                <EditButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
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

                            <Templates>
                                <EditForm>
                                    <div style="padding: 4px 3px 4px">
                                        <table>
                                            <tr>
                                                <td><label id="Label1" runat="server">Camp</label> </td>
                                                <td><label id="Label2" runat="server">Operator</label> </td>
                                                <td><label id="Label3" runat="server">Prima Valoare</label> </td>
                                                <td><label id="Label4" runat="server">A Doua Valoare</label> </td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dx:ASPxComboBox ID="cmbCol" ClientInstanceName="cmbCol" runat="server" Width="120px" DropDownStyle="DropDownList" ValueField="Id" ValueType="System.Int32" TextFormatString="{0}" >
                                                        <ItemStyle Border-BorderWidth="0px" />
                                                        <ClientSideEvents SelectedIndexChanged="cmbCol_SelectedIndexChanged_Client" />
                                                        <Columns>
                                                            <dx:ListBoxColumn FieldName="Denumire" Caption="Camp"  />
                                                            <dx:ListBoxColumn FieldName="TipData" Caption="Tip data" />
                                                            <dx:ListBoxColumn FieldName="Id" Visible="false"/>
                                                        </Columns>
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td>
                                                    <dx:ASPxComboBox ID="cmbCond" ClientInstanceName="cmbCond" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="140px" >
                                                        <ClientSideEvents SelectedIndexChanged="cmbCond_SelectedIndexChanged_Client" />
                                                    </dx:ASPxComboBox>
                                                </td>
                                                <td>
                                                    <div class="div_hor" style="margin-bottom:0;">
                                                        <dx:ASPxTextBox ID="txtVal" ClientInstanceName="txtVal" runat="server" Width="120px" />
                                                        <dx:ASPxComboBox ID="cmbVal" runat="server" ClientInstanceName="cmbVal" AutoPostBack="false" CssClass="cmbTip" Width="8px" DropDownStyle="DropDownList" ValueField="Id" ValueType="System.Int32" TextField="Denumire" TextFormatString="{0}" CallbackPageSize="15" EnableCallbackMode="true" >
                                                            <ClientSideEvents SelectedIndexChanged="cmbVal_SelectedIndexChanged_Client" />
                                                            <Columns>
                                                                <dx:ListBoxColumn FieldName="Denumire" Caption="Camp"  />
                                                                <dx:ListBoxColumn FieldName="TipData" Caption="Tip data" />
                                                                <dx:ListBoxColumn FieldName="Id" Visible="false"/>
                                                            </Columns>
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxComboBox ID="cmbValData" ClientInstanceName="cmbValData" DropDownStyle="DropDownList" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="18px">
                                                            <Items>
                                                                <dx:ListEditItem Text="ieri" Value="ieri" />
                                                                <dx:ListEditItem Text="astazi" Value="astazi" />
                                                                <dx:ListEditItem Text="maine" Value="maine" />
                                                                <dx:ListEditItem Text="prima zi din saptamana" Value="prima zi din saptamana" />
                                                                <dx:ListEditItem Text="ultima zi din saptamana" Value="ultima zi din saptamana" />
                                                                <dx:ListEditItem Text="prima zi din luna" Value="prima zi din luna" />
                                                                <dx:ListEditItem Text="ultima zi din luna" Value="ultima zi din luna" />
                                                                <dx:ListEditItem Text="prima zi din an" Value="prima zi din an" />
                                                                <dx:ListEditItem Text="ultima zi din an" Value="ultima zi din an" />
                                                            </Items>
                                                            <ClientSideEvents SelectedIndexChanged="cmbValData_SelectedIndexChanged_Client" />
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxSpinEdit ID="txtZile" ClientInstanceName="txtZile" runat="server" Width="50px" MinValue="-200" MaxValue="200" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="div_hor" style="margin-bottom:0;">
                                                        <dx:ASPxTextBox ID="txtVal2" ClientInstanceName="txtVal2" runat="server" Width="120px" />
                                                        <dx:ASPxComboBox ID="cmbVal2" runat="server" ClientInstanceName="cmbVal2" AutoPostBack="false" CssClass="cmbTip" Width="18px" ValueField="Id" ValueType="System.Int32" TextField="Denumire" >
                                                            <ClientSideEvents SelectedIndexChanged="cmbVal2_SelectedIndexChanged_Client" />
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxComboBox ID="cmbValData2" ClientInstanceName="cmbValData2" runat="server" AutoPostBack="false" CssClass="cmbTip" Width="18px">
                                                            <Items>
                                                                <dx:ListEditItem Text="ieri" Value="ieri" />
                                                                <dx:ListEditItem Text="astazi" Value="astazi" />
                                                                <dx:ListEditItem Text="maine" Value="maine" />
                                                                <dx:ListEditItem Text="prima zi din saptamana" Value="prima zi din saptamana" />
                                                                <dx:ListEditItem Text="ultima zi din saptamana" Value="ultima zi din saptamana" />
                                                                <dx:ListEditItem Text="prima zi din luna" Value="prima zi din luna" />
                                                                <dx:ListEditItem Text="ultima zi din luna" Value="ultima zi din luna" />
                                                                <dx:ListEditItem Text="prima zi din an" Value="prima zi din an" />
                                                                <dx:ListEditItem Text="ultima zi din an" Value="ultima zi din an" />
                                                            </Items>
                                                            <ClientSideEvents SelectedIndexChanged="cmbValData2_SelectedIndexChanged_Client" />
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxSpinEdit ID="txtZile2" ClientInstanceName="txtZile2" runat="server" Width="40px" MinValue="-200" MaxValue="200" />
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                        <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                        <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                    </div>

                                </EditForm>
                            </Templates>

                        </dx:ASPxGridView>

                    </div>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>

        <dx:ASPxRoundPanel ID="pnlAtasament" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" Width="100%" HeaderText="Atasament" CssClass="pnlAlign indentTop10" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>

<%--                    <asp:UpdatePanel ID="up5" runat="server">
                        <ContentTemplate>
--%>
                            <table>
                                <tr>
                                    <td><label id="lblNume" runat="server">Nume</label></td>
                                    <td>
                                        <dx:ASPxTextBox ID="txtNume" runat="server" ClientInstanceName="txtNume" Width="250px">
                                            <ClientSideEvents GotFocus="onGotFocus" />
                                        </dx:ASPxTextBox></td>
                                    <td style="padding:0px 15px;"><dx:ASPxCheckBox ID="chkDisc" runat="server" Text="Salvare in disc" TextAlign="Left" /></td>
                                    <td style="padding-right:15px !important;"><dx:ASPxCheckBox ID="chkBaza" runat="server" Text="Salvare in BD" TextAlign="Left"></dx:ASPxCheckBox></td>
                                    <td style="padding-right:15px;"><dx:ASPxCheckBox ID="chkTrimite" runat="server" Text="Trimite pe mail" TextAlign="Left"></dx:ASPxCheckBox></td>
                                </tr>
                            </table>

                            <dx:ASPxHtmlEditor ID="txtAtt" runat="server" ClientInstanceName="txtAtt" Height="370px" Width="100%">
                                <ClientSideEvents GotFocus="onGotFocus" />
                                <SettingsDialogs>
                                    <InsertImageDialog>
                                        <SettingsImageUpload UploadFolder="~/UploadFiles/Images/">
                                            <ValidationSettings AllowedFileExtensions=".jpe,.jpeg,.jpg,.gif,.png" MaxFileSize="500000">
                                            </ValidationSettings>
                                        </SettingsImageUpload>
                                    </InsertImageDialog>
                                </SettingsDialogs>
                            </dx:ASPxHtmlEditor>

<%--                        </ContentTemplate>
                        <Triggers>
                            
                        </Triggers>
                    </asp:UpdatePanel>--%>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>


        <dx:ASPxRoundPanel ID="pnlXLS" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" Width="100%" HeaderText="Export Excel" CssClass="pnlAlign indentTop10" >
            <HeaderStyle Font-Bold="true" />
            <PanelCollection>
                <dx:PanelContent>

                    <div class="div_ver">
                        <dx:ASPxCheckBox ID="chkExcel" runat="server" Text="Export excel" TextAlign="Left" />
                        <dx:ASPxMemo ID="txtExcel" ClientInstanceName="txtExcel" runat="server" Width="100%" Height="150px" >
                            <ClientSideEvents GotFocus="onGotFocus" />
                        </dx:ASPxMemo>
                    </div>

                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxRoundPanel>


    </form>
</body>
</html>
