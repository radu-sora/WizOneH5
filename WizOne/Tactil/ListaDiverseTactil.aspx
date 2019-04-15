<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ListaDiverseTactil.aspx.cs" Inherits="WizOne.Tactil.ListaDiverseTactil" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        $(function () {
            $("body").on('click keypress', function () {
                ResetThisSession();
            });
        });

        var timeOutSecunde = <%= Session["TimeOutSecunde"] %>;
        var timeInSecondsAfterSessionOut = 30;
        var secondTick = 0;

        function ResetThisSession() {
            secondTick = 0;
        }

        function StartThisSessionTimer() {
            secondTick++;
            if (timeOutSecunde != null)
                timeInSecondsAfterSessionOut = timeOutSecunde;
            var timeLeft = ((timeInSecondsAfterSessionOut - secondTick) / 60).toFixed(0); // in minutes
            timeLeft = timeInSecondsAfterSessionOut - secondTick;
            $("#spanTimeLeft").html(timeLeft);

            if (secondTick >= timeInSecondsAfterSessionOut) {
                clearTimeout(tick);
                window.location = "../DefaultTactil.aspx";
                return;
            }
            tick = setTimeout("StartThisSessionTimer()", 1000);
        }

        StartThisSessionTimer();

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
                case "btnCerere":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCerereMode);
                    break;
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                    break;
            }
        }


        function GoToDeleteMode(Value) {

            if (Value == 0 || Value == -1) {
                swal({
                    title: trad_string(limba, 'Operatie nepermisa'), text: trad_string(limba, 'Nu puteti anula o cerere deja anulata sau respinsa'),
                    type: "warning"
                });
            }
            else {
                swal({
                    title: trad_string(limba, "Sunteti sigur/a ?"), text: trad_string(limba, "Cererea va fi anulata !"),
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: trad_string(limba, "Da, continua!"), cancelButtonText: trad_string(limba, "Renunta"), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete;" + Value);
                    }
                });
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=3&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCerereMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=1&tbl=1&id=' + Value, '_blank ')
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=2&id=' + Value, '_blank ')
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, "Atentie !"), text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnRespinge(s, e)
        {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de respingere ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
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
                    title: trad_string(limba, "Atentie !"), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function OnAproba(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de aprobare ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAproba;6");
                    }
                });
            }
            else {
                swal({
                    title: trad_string(limba, "Atentie !"), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function EmptyFields(s, e) {
            cmbViz.SetValue(null);
            cmbTip.SetValue(null);
            cmbRol.SetValue(null);

            txtDataInc.SetValue(null);
            txtDataSf.SetValue(null);
            cmbStare.SetValue(null);

            //pnlCtl.PerformCallback('EmptyFields');
        }

    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <table style="width:100%;">
            <tr>
                <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
            </tr>
        </table>
    <table width="100%">
        <tr>
            <td align="left"><Label runat="server"  id="lblMarca" style="font-weight: bold;">MARCA: </Label> </td>
            <td align="center"><Label  runat="server" id="lblNume" style="font-weight: bold;">NUME:</Label></td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgback.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr align="center">
            <td colspan="2">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnRowUpdating="grDate_RowUpdating" OnCustomCallback="grDate_CustomCallback" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="true"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />                 
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>                  

                        <dx:GridViewCommandColumn Width="50px" VisibleIndex="1" ButtonType="Image"   ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>           
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Imprima atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

						<dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="70px" VisibleIndex="2"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="170px" VisibleIndex="3" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
						<dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="200px" VisibleIndex="4" Settings-AutoFilterCondition="Contains" />
						<dx:GridViewDataTextColumn FieldName="TipCerere" Name="TipCerere" Caption="Tip Cerere" ReadOnly="true" Width="150px" VisibleIndex="5" />
						<dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Cerere" ReadOnly="true" Width="350px" VisibleIndex="6" />
						<dx:GridViewDataTextColumn FieldName="Raspuns" Name="Raspuns" Caption="Raspuns" ReadOnly="true" Width="350px" VisibleIndex="7" />
						
						
                    </Columns>                   
        
                                  
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>

    

</asp:Content>
