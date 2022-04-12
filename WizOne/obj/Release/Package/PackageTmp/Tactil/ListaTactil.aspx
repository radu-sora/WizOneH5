<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ListaTactil.aspx.cs" Inherits="WizOne.Tactil.ListaTactil" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>        
        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;    
                case "btnPrint":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToPrintMode);
                    break;
            }
        }

        function OnMotivRespingere(s,e)
        {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '')
            {
                swal({
                    title: "Operatie nepermisa", text: "Pentru a putea respinge este nevoie de un motiv",
                    type: "warning"
                });
            }
            else
            {
                popUpMotiv.Hide();
                grDate.PerformCallback('btnRespinge;' + txtMtv.GetText());
                txtMtv.SetText('');
            }
        }

        function GoToDeleteMode(Value) {
            if (Value == 0 || Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Nu puteti anula o cerere deja anulata sau respinsa",
                    type: "warning"
                });
            }
            else {
                swal({
                    title: "Sunteti sigur/a ?", text: "Cererea va fi anulata !",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, anuleaza!", cancelButtonText: "Renunta", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete;" + Value);
                    }
                });
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCerereMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=1&tbl=1&id=' + Value, '_blank ')
        }

        function GoToPrintMode(Value) {
            grDate.PerformCallback("btnPrint;" + Value);
        }

        function CloseDeferedWindow() {
            popUpDivide.Hide();
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

        function GoToDivide(values) {
            if (values[1] != 3) {
                swal({
                    title: "Operatie nepermisa", text: "Se pot diviza numai cererile in starea aprobat",
                    type: "warning"
                });
            }
            else {
                if (Math.round(Math.abs((values[3].getTime() - values[2].getTime()) / (24 * 60 * 60 * 1000))) + 1 < 2) {
                    swal({
                        title: "Operatie nepermisa", text: "Intervalul ales trebuie sa fie de minim 2 zile",
                        type: "warning"
                    });
                }
                else {
                    txtDataInc.value = values[2].getTime();
                    txtDataSf.value = values[3].getTime();
                    txtDataDivide.value = txtDataInc.value;
                    popUpDivide.Show();
                }
            }
        }

        function OKDivideClick(s, e) {
            if (txtDataDivide.GetDate() == null) {
                swal({
                    title: "Atentie", text: "Lipseste data cu care se divide cererea",
                    type: "warning"
                });
            }
            else {
                var txtDiv = txtDataDivide.GetDate().getTime();
                if (txtDataInc.value <= txtDiv && txtDiv <= txtDataSf.value)
                {
                    txtDataInc.value = null;
                    txtDataSf.value = null;
                    txtDataDivide.value = null;
                    popUpDivide.Hide();
                    e.processOnServer = true;
                }
                else
                {
                    swal({
                        title: "Atentie", text: "Data nu este in intervalul din cerere",
                        type: "warning"
                    });

                    e.processOnServer = false;
                }
            }            
        }

        function SetComboViz()
        {
            if (typeof cmbRol !== 'undefined') {
                if (cmbViz.GetValue() == 3)
                    cmbRol.SetEnabled(false);
                else
                    cmbRol.SetEnabled(true);
            }
        }

        function OnRespinge(s, e)
        {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de respingere ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        if (grDate.cpParamMotiv == "1")
                            popUpMotiv.Show();
                        else
                            grDate.PerformCallback("btnRespinge; ");
                    }
                });
            }
            else
            {
                swal({
                    title: "Atentie", text: "Nu exista linii selectate",
                    type: "warning"
                });
            }
        }

        function OnAproba(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de aprobare ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAproba;6");
                    }
                });
            }
            else {
                swal({
                    title: "Atentie", text: "Nu exista linii selectate",
                    type: "warning"
                });
            }
        }

    </script>

    <table style="width:100%;">
        <tr>
            <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgBack.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left"><Label runat="server"  id="lblMarca" style="font-weight: bold;">MARCA: </Label> </td>
            <td align="center"><Label  runat="server" id="lblNume" style="font-weight: bold;">NUME:</Label></td>
            <td align="right">
                <dx:ASPxButton ID="btnLogOut" ClientInstanceName="btnLogOut" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Deconectare" AutoPostBack="true" OnClick="btnLogOut_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr align="center">
            <td colspan="3">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="90%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="False"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="160px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnPrint">
                                    <Image ToolTip="Imprima" Url="~/Fisiere/Imagini/Icoane/print.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="175px" VisibleIndex="2" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="350px" Visible="false" VisibleIndex="5" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" ReadOnly="true" Width="595px" VisibleIndex="6">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput" ReadOnly="true" Width="100px" VisibleIndex="7" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit" ReadOnly="true" Width="100px" VisibleIndex="8" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="NrZile" Name="NrZile" Caption="Nr. zile" ReadOnly="true" Width="70px" VisibleIndex="9" />                        
                        <dx:GridViewDataTextColumn FieldName="NrOre" Name="NrOre" Caption="Nr. ore" ReadOnly="true" Width="70px" VisibleIndex="10" />
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="11" />
                    </Columns>                        
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>  

</asp:Content>
