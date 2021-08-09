<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Lista.aspx.cs" Inherits="WizOne.Absente.Lista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    
    <script>

        var limba = "<%= Session["IdLimba"] %>";

        function grDate_CustomButtonClick(s, e) {
            if (cmbViz.GetValue() == 4) {
                //NOP
            }
            else {
                switch (e.buttonID) {
                    case "btnEdit":
                        grDate.StartEditRow(e.visibleIndex);
                        break;
                    case "btnDelete":
                        grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                        break;
                    case "btnPlanif":
                        grDate.PerformCallback("btnPlanif;0");
                        break;
                    case "btnIstoric":
                        grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                        break;
                    case "btnDivide":
                        grDate.GetRowValues(e.visibleIndex, 'Id;IdStare;DataInceput;DataSfarsit', GoToDivide);
                        //popUpDivide.Show();
                        break;
                    case "btnCerere":
                        grDate.GetRowValues(e.visibleIndex, 'Id', GoToCerereMode);
                        break;
                    case "btnAtasament":
                        grDate.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                        break;
                }
            }
        }


        function OnMotivRespingere(s,e)
        {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '')
            {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Pentru a putea respinge este nevoie de un motiv"),
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
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=1&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCerereMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=1&tbl=1&id=' + Value, '_blank ')
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=1&id=' + Value, '_blank ')
        }

        function CloseDeferedWindow() {
            popUpDivide.Hide();
        }


        function OnEndCallback(s, e) {         
            if (s.cpAlertMessage != null) { 
                if (s.cpSuccessMessage != null) 
                    swal({
                        title: trad_string(limba, ""), text: s.cpAlertMessage,
                        type: "success"
                    });
                else
                    swal({
                        title: trad_string(limba, ""), text: s.cpAlertMessage,
                        type: "warning"
                    });
                s.cpAlertMessage = null;
                s.cpSuccessMessage = null;
            }
        }

        function GoToDivide(values) {
            if (values[1] != 3) {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Se pot diviza numai cererile in starea aprobat"),
                    type: "warning"
                });
            }
            else {
                if (Math.round(Math.abs((values[3].getTime() - values[2].getTime()) / (24 * 60 * 60 * 1000))) + 1 < 2) {
                    swal({
                        title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Intervalul ales trebuie sa fie de minim 2 zile"),
                        type: "warning"
                    });
                }
                else {
                    txtDataInc.value = values[2].getTime();
                    txtDataSf.value = values[3].getTime();
                    //txtDataDivide.value = txtDataInc.value;
                    txtDataDivide.SetValue(null);
                    popUpDivide.Show();
                }
            }
        }

        function OKDivideClick(s, e) {
            if (txtDataDivide.GetDate() == null) {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Lipseste data cu care se divide cererea"),
                    type: "warning"
                });
                 e.processOnServer = false;
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
                        title: trad_string(limba, ""), text: trad_string(limba, "Data nu este in intervalul din cerere"),
                        type: "warning"
                    });

                    e.processOnServer = false;
                }
            }            
        }

        function SetComboViz() {
            if (typeof cmbRol !== 'undefined') {
                if (cmbViz.GetValue() == 3) {
                    cmbRol.SetEnabled(false);
                    cmbRol.SetValue(-1);
                }
                else {
                    cmbRol.SetEnabled(true);
                }
            }
        }

        function SetEnabled() {            
            var esteActiv = cmbViz.GetValue() !== 4;            

            typeof btnRespinge !== 'undefined' && btnRespinge.SetEnabled(esteActiv);
            typeof btnAproba !== 'undefined' && btnAproba.SetEnabled(esteActiv);            
            btnSolNoua.SetEnabled(esteActiv);
            cmbAng.SetEnabled(esteActiv);
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
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function OnAnulare(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de anulare ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAnulare; ");
                    }
                });
            }
            else {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
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
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }               
    </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-content">
        <div class="page-content-header">
            <div>
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </div>
            <div>
                <dx:ASPxButton ID="btnSolNoua" ClientInstanceName="btnSolNoua" Font-Bold="true" ClientIDMode="Static" runat="server" Text="Solicitare noua" OnClick="btnNew_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnIstoricExtins" ClientInstanceName="btnIstoricExtins" ClientIDMode="Static" runat="server" Text="Istoric Extins" CssClass="hidden-xs hidden-sm" OnClick="btnIstoricExtins_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/istoric.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAnulare" ClientInstanceName="btnAnulare" ClientIDMode="Static" runat="server" Text="Anulare" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                    <ClientSideEvents Click="function(s, e) { OnAnulare(s, e); }" />                    
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespinge" ClientInstanceName="btnRespinge" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                    <ClientSideEvents Click="function(s, e) { OnRespinge(s, e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function(s, e) { OnAproba(s, e); }" />                    
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" PostBackUrl="~/Pagini/MainPage.aspx" CssClass="hidden-xs hidden-sm" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </div>            
        </div>
        <div>
            <div class="row row-fix">
                <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                    <dx:ASPxLabel ID="lblViz" runat="server" AssociatedControlID="cmbViz" Text="Vizualizare" Font-Bold="true" />
                    <dx:ASPxComboBox ID="cmbViz" ClientInstanceName="cmbViz" ClientIDMode="Static" runat="server" Width="100%" AutoPostBack="false"
                        ValueType="System.Int32">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                        <ClientSideEvents Init="function(s,e) { SetComboViz(); }" SelectedIndexChanged="function(s, e) { SetComboViz(); SetEnabled(); pageControl.onFilterChange(true); }" />
                    </dx:ASPxComboBox>
                </div>
                <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                    <dx:ASPxLabel ID="lblRol" runat="server" AssociatedControlID="cmbRol" Text="Rol" Font-Bold="true" />
                    <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="100%" AutoPostBack="false"
                        ValueField="Id" TextField="Denumire" ValueType="System.Int32">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                        <ClientSideEvents SelectedIndexChanged="function(s, e) { pageControl.onFilterChange(true); }" />
                    </dx:ASPxComboBox>
                </div>
                <div class="col-lg-1 col-md-4 col-sm-4 col-xs-6">                        
                    <dx:ASPxLabel ID="lblStare" runat="server" AssociatedControlID="cmbStare" Text="Stare" Font-Bold="true" />
                    <dx:ASPxDropDownEdit ID="cmbStare" ClientInstanceName="cmbStare" runat="server" Width="100%">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" ModalDropDownCaption="Selectie stari" />
                        <DropDownApplyButton Visibility="ModalDropDown" />                        
                        <DropDownWindowStyle CssClass="dx-dropdownwindow-adaptive" />
                        <DropDownWindowTemplate>
                            <dx:ASPxListBox ID="lstStare" ClientInstanceName="lstStare" runat="server" SelectionMode="CheckColumn" Width="100%" Height="170px" 
                                EnableSelectAll="true" SelectAllText="(Selectie toate)" ValueType="System.Int32">                                
                                <Items>                                
                                    <dx:ListEditItem Text="Solicitat" Value="1" />
                                    <dx:ListEditItem Text="In Curs" Value="2" />
                                    <dx:ListEditItem Text="Aprobat" Value="3" />
                                    <dx:ListEditItem Text="Respins" Value="0" />
                                    <dx:ListEditItem Text="Anulat" Value="-1" />
                                    <dx:ListEditItem Text="Planificat" Value="4" />
                                </Items>
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { updateComboBoxText(cmbStare, lstStare); }" />
                            </dx:ASPxListBox>
                            <dx:ASPxButton ID="btnInchide" runat="server" AutoPostBack="False" Text="Inchide" CssClass="pull-right hidden-xs hidden-sm" style="margin:7px">
                                <ClientSideEvents Click="function(s, e) { cmbStare.HideDropDown(); pageControl.onFilterChange(); }" />
                            </dx:ASPxButton>                                                
                        </DropDownWindowTemplate>
                        <ClientSideEvents 
                            TextChanged="function(s, e) { updateListBoxValues(cmbStare, lstStare); }" 
                            DropDown="function(s, e) { updateListBoxValues(cmbStare, lstStare); }" 
                            DropDownCommandButtonClick="function(s, e) { cmbStare.HideDropDown(); pageControl.onFilterChange(); }" />
                    </dx:ASPxDropDownEdit>
                </div>
                <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                    <dx:ASPxLabel ID="lblDtInc" runat="server" AssociatedControlID="txtDtInc" Text="Data Inceput" Font-Bold="true" />
                    <dx:ASPxDateEdit ID="txtDtInc" ClientInstanceName="txtDtInc" runat="server" Width="100%" 
                        DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" PickerDisplayMode="Auto" AllowNull="false">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                        <CalendarProperties FirstDayOfWeek="Monday" />
                        <ClientSideEvents ValueChanged="function(s, e) { pageControl.onFilterChange(); }" />
                    </dx:ASPxDateEdit>                        
                </div>
                <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                    <dx:ASPxLabel ID="lblDtSf" runat="server" AssociatedControlID="txtDtSf" Text="Data Sfarsit" Font-Bold="true" />
                    <dx:ASPxDateEdit ID="txtDtSf" ClientInstanceName="txtDtSf" runat="server" Width="100%" 
                        DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" PickerDisplayMode="Auto" AllowNull="false">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                        <CalendarProperties FirstDayOfWeek="Monday" />
                        <ClientSideEvents ValueChanged="function(s, e) { pageControl.onFilterChange(); }" />
                    </dx:ASPxDateEdit>
                </div>
                <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                    <dx:ASPxLabel ID="lblAng" runat="server" AssociatedControlID="cmbAng" Text="Angajat" Font-Bold="true" />
                    <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="100%" AutoPostBack="false"                        
                        ValueField="F10003" TextField="NumeAngajat" ValueType="System.Int32" AllowNull="true" OnCallback="cmbAng_Callback">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />                        
                    </dx:ASPxComboBox>
                </div>
                <div class="col-lg-1 col-md-12 col-sm-12 col-xs-12">
                    <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        <ClientSideEvents Click="function(s, e) { pageControl.onFilterButtonClick(); }" />
                    </dx:ASPxButton>
                </div>
            </div>            
        </div>
        <div class="page-content-data invisible">
            <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"
                CssClass="dx-grid-adaptive dx-grid-adaptive-hide-group dx-grid-adaptive-hide-header dx-grid-adaptive-scale-commandcolumn dx-grid-adaptive-fullscreen-popup"
                OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCustomUnboundColumnData="grDate_CustomUnboundColumnData">
                <Settings ShowFilterRow="True" ShowFilterRowMenu="true" ShowGroupPanel="True" HorizontalScrollBarMode="Auto" VerticalScrollBarMode="Auto" />
                <SettingsAdaptivity AdaptivityMode="HideDataCellsWindowLimit" AdaptiveDetailColumnCount="1" HideDataCellsAtWindowInnerWidth="1024" />
                <SettingsEditing Mode="PopupEditForm" />
                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                <SettingsSearchPanel Visible="true" />                
                <SettingsPopup>
                    <EditForm Modal="true" HorizontalAlign="WindowCenter" VerticalAlign="WindowCenter">
                        <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchAtWindowInnerWidth="1024" MinWidth="100%" MinHeight="100%" />
                    </EditForm>
                </SettingsPopup>
                <SettingsCommandButton>
                    <UpdateButton ButtonType="Button" Text="Actualizeaza">
                        <Styles>
                            <Style Font-Bold="true" />
                        </Styles>
                    </UpdateButton>
                    <CancelButton ButtonType="Button" Text="Renunta">
                        <Styles>
                            <Style Font-Bold="true" />
                        </Styles>
                    </CancelButton>                    
                </SettingsCommandButton>                
                <Columns>

                    <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />

                    <dx:GridViewCommandColumn Width="160px" VisibleIndex="1" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                <Image ToolTip="Modificare" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                            </dx:GridViewCommandColumnCustomButton>
                            <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                            </dx:GridViewCommandColumnCustomButton>
                            <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                            </dx:GridViewCommandColumnCustomButton>
                            <dx:GridViewCommandColumnCustomButton ID="btnDivide">
                                <Image ToolTip="Divide" Url="~/Fisiere/Imagini/Icoane/divide.png" />
                            </dx:GridViewCommandColumnCustomButton>
                            <dx:GridViewCommandColumnCustomButton ID="btnCerere">
                                <Image ToolTip="Arata cerere" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                            </dx:GridViewCommandColumnCustomButton>
                            <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                            </dx:GridViewCommandColumnCustomButton>
                            <dx:GridViewCommandColumnCustomButton ID="btnPlanif">
                                <Image ToolTip="Transforma in solicitat" Url="~/Fisiere/Imagini/Icoane/notif.png" />
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                    </dx:GridViewCommandColumn>

                    <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="250px" VisibleIndex="2" >
                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        <Settings FilterMode="DisplayText" />
                    </dx:GridViewDataComboBoxColumn>

                    <dx:GridViewDataTextColumn FieldName="Actiune" Name="Actiune" Caption="Actiune" ReadOnly="true" Width="150px" Visible="false" ShowInCustomizationForm="false" />
                    <dx:GridViewDataTextColumn FieldName="Rol" Name="Rol" Caption="Rol" ReadOnly="true" Width="150px" Visible="false" ShowInCustomizationForm="false" />

                    <dx:GridViewDataTextColumn FieldName="F10003" Name="Marca" Caption="Marca" ReadOnly="true" Width="100px" VisibleIndex="3" Settings-AutoFilterCondition="Equals" />
                    <dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="250px" VisibleIndex="4" Settings-AutoFilterCondition="Contains" />
                    <dx:GridViewDataTextColumn FieldName="Subcompania" Name="Subcompania" Caption="Subcompania" ReadOnly="true" Width="250px" VisibleIndex="5" Settings-AutoFilterCondition="Contains" />
                    <dx:GridViewDataTextColumn FieldName="EID" Name="EID" Caption="EID" ReadOnly="true" Width="100px" VisibleIndex="6" Settings-AutoFilterCondition="Equals" />

                    <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" ReadOnly="true" Width="250px" VisibleIndex="7">
                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        <Settings FilterMode="DisplayText" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput" ReadOnly="true" Width="100px" VisibleIndex="8" >
                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                    </dx:GridViewDataDateColumn>
                    <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit" ReadOnly="true" Width="100px" VisibleIndex="9" >
                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                    </dx:GridViewDataDateColumn>
                    <dx:GridViewDataTextColumn FieldName="NrZile" Name="NrZile" Caption="Nr. zile" ReadOnly="true" Width="70px" VisibleIndex="10" />
                    <dx:GridViewDataTextColumn FieldName="NumarOre" Name="NumarOre" Caption="Nr. ore" ReadOnly="true"  Width="100px" UnboundType="String" VisibleIndex="11"/>
                    <dx:GridViewDataTextColumn FieldName="NrOre" Name="NrOre" ReadOnly="true" Width="70px" Visible="false"  />
                    <dx:GridViewDataMemoColumn FieldName="Observatii" Name="Observatii" Caption="Observatii" Width="250px" VisibleIndex="12">
                        <PropertiesMemoEdit Width="100%" Height="60px" />
                    </dx:GridViewDataMemoColumn>                    
                    <dx:GridViewDataComboBoxColumn FieldName="NumeInlocuitor" Name="NumeInlocuitor" Caption="Inlocuitor" Width="250px" ShowInCustomizationForm="false" VisibleIndex="13">
                        <PropertiesComboBox DropDownStyle="DropDownList" ValueField="NumeComplet" ValueType="System.String" TextField="NumeComplet" Width="100%">
                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                        </PropertiesComboBox>
                    </dx:GridViewDataComboBoxColumn>

                    <dx:GridViewDataComboBoxColumn FieldName="TrimiteLa" Name="TrimiteLa" Caption="Tip aditional solicitare" Width="250px" VisibleIndex="14">
                        <PropertiesComboBox DropDownStyle="DropDownList" ValueField="Denumire" ValueType="System.String" TextField="Denumire" Width="100%">
                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                        </PropertiesComboBox>
                    </dx:GridViewDataComboBoxColumn>                                           
                    <dx:GridViewDataMemoColumn FieldName="Comentarii" Name="Comentarii" Caption="Comentarii" Width="250px" VisibleIndex="15">
                        <PropertiesMemoEdit Width="100%" Height="60px" />
                    </dx:GridViewDataMemoColumn>
                    <dx:GridViewDataTextColumn FieldName="DateConcatenate" Name="DateConcatenate" Caption="Informatii aditionale" ReadOnly="true" Width="250px" VisibleIndex="16" />
                        
                    <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="17" />
                    <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="18" />
                    <dx:GridViewDataTextColumn FieldName="Inlocuitor" Name="Inlocuitor" Caption="Inlocuitor" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="19" />

                    <dx:GridViewDataTextColumn FieldName="Compensare" Name="Compensare" Caption="Trimite la" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="20" />
                    <dx:GridViewDataTextColumn FieldName="CompensarePlata" Name="CompensarePlata" Caption="Trimite la plata" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="21" />
                    <dx:GridViewDataTextColumn FieldName="CompensarePlataDenumire" Name="CompensarePlataDenumire" Caption="Trimite la plata" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="22" />
                    <dx:GridViewDataTextColumn FieldName="CompensareBanca" Name="CompensareBanca" Caption="Trimite la banca" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="23" />
                    <dx:GridViewDataTextColumn FieldName="CompensareBancaDenumire" Name="CompensareBancaDenumire" Caption="Trimite la banca" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="24" />

                    <dx:GridViewDataTextColumn FieldName="AdaugaAtasament" Name="AdaugaAtasament" Caption="Adauga Atasament" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="25" />

                    <dx:GridViewDataTextColumn FieldName="Anulare_Valoare" Name="Anulare_Valoare" Caption="Anulare - tip" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="26" />
                    <dx:GridViewDataTextColumn FieldName="Anulare_NrZile" Name="Anulare_NrZile" Caption="Anulare - nr zile" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="27" />

                    <dx:GridViewDataCheckColumn FieldName="CampBifa" Name="CampBifa" Caption="CampBifa" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="28">
                        <EditFormSettings Visible="true" />
                    </dx:GridViewDataCheckColumn>

                </Columns>                
                <Templates>
                    <EditForm>      
                        <div class="panel panel-slim panel-adaptive-fullscreen">
                          <div class="panel-body">
                            <div class="row row-fix">
                                <div class="col-sm-6 col-xs-12">
                                    <dx:ASPxLabel ID="ObservatiiTemplateLabel" runat="server" AssociatedControlID="ObservatiiTemplate" Text="Observatii" Font-Bold="true" />
                                    <dx:ASPxGridViewTemplateReplacement ID="ObservatiiTemplate" runat="server" ReplacementType="EditFormCellEditor" ColumnID="Observatii" />                                            
                                </div>
                                <div class="col-sm-6 col-xs-12">
                                    <dx:ASPxLabel ID="ComentariiTemplateLabel" runat="server" AssociatedControlID="ComentariiTemplate" Text="Comentarii" Font-Bold="true" />
                                    <dx:ASPxGridViewTemplateReplacement ID="ComentariiTemplate" runat="server" ReplacementType="EditFormCellEditor" ColumnID="Comentarii" />                                        
                                </div>
                            </div>
                            <div class="row row-fix">
                                <div id="InlocuitorEditContainer" runat="server" class="col-sm-6 col-xs-12">
                                    <dx:ASPxLabel ID="InlocuitorTemplateLabel" runat="server" AssociatedControlID="InlocuitorTemplate" Text="Inlocuitor" Font-Bold="true" />
                                    <dx:ASPxGridViewTemplateReplacement ID="InlocuitorTemplate" runat="server" ReplacementType="EditFormCellEditor" ColumnID="NumeInlocuitor" />                                    
                                </div>
                                <div id="SolicitareEditContainer" runat="server" class="col-sm-6 col-xs-12">
                                    <dx:ASPxLabel ID="SolicitareTemplateLabel" runat="server" AssociatedControlID="SolicitareTemplate" Text="Tip aditional solicitare" Font-Bold="true" />
                                    <dx:ASPxGridViewTemplateReplacement ID="SolicitareTemplate" runat="server" ReplacementType="EditFormCellEditor" ColumnID="TrimiteLa" />                                    
                                </div>
                            </div>
                            <div class="row row-fix">
                                <div id="UploadEditContainer" runat="server" class="col-xs-6">
                                    <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                        BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                        ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocUploadResource1">
                                        <BrowseButton>
                                            <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                        </BrowseButton>
                                        <ValidationSettings ShowErrors="False"></ValidationSettings>                                        
                                    </dx:ASPxUploadControl>                                    
                                </div>
                                <div id="CampBifaEditContainer" runat="server" class="col-xs-6">
                                    <dx:ASPxLabel ID="CampBifaTemplateLabel" runat="server" AssociatedControlID="CampBifaTemplate" Text="Camp bifa" Font-Bold="true" CssClass="label-inline" />                                    
                                    <dx:ASPxGridViewTemplateReplacement ID="CampBifaTemplate" runat="server" ReplacementType="EditFormCellEditor" ColumnID="CampBifa" />
                                </div>
                            </div>                            
                          </div>
                          <div class="panel-footer panel-footer-commandbox">
                              <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" runat="server" ReplacementType="EditFormUpdateButton" />
                              <dx:ASPxGridViewTemplateReplacement ID="CancelButton" runat="server" ReplacementType="EditFormCancelButton" />
                          </div>
                        </div>                                                             
                    </EditForm>                    
                </Templates>
                <ClientSideEvents 
                    CustomButtonClick="grDate_CustomButtonClick" 
                    ContextMenu="ctx" 
                    EndCallback="function(s,e) { OnEndCallback(s, e); SetEnabled(); }" />
            </dx:ASPxGridView>
        </div>
    </div>
    
    <dx:ASPxPopupControl ID="popUpDivide" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpDivideArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="300px" Height="200px" HeaderText="Alege data de divizare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpDivide" EnableHierarchyRecreation="false">
            <ContentCollection>
                <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
                    <dx:ASPxPanel ID="pnlDivide" runat="server" DefaultButton="btnOKDivide">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <table width="100%">
                                    <tr style="float:right; text-align:right; margin-bottom:30px;">
                                        <td>
                                            <dx:ASPxButton ID="btnOKDivide" runat="server" Text="Divide" AutoPostBack="false" OnClick="btnOKDivide_Click" HorizontalAlign="Right" oncontextMenu="ctx(this,event)" >
                                                <Image Url="~/Fisiere/Imagini/Icoane/divide.png"></Image>
                                                <ClientSideEvents Click="function (s,e) { OKDivideClick(s,e); }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                    <tr align="center">
                                        <td>
                                            <dx:ASPxDateEdit ID="txtDataDivide" ClientIDMode="Static" ClientInstanceName="txtDataDivide" runat="server" Width="120px">
                                                <CalendarProperties FirstDayOfWeek="Monday" />
                                            </dx:ASPxDateEdit>
                                            <br />
                                            <dx:ASPxCheckBox ID="chkAnulare" ClientInstanceName="chkAnulare" runat="server" Text="Anulare concediu incepand cu ziua urmatoare acestei date" TextAlign="Right" />
                                            <dx:ASPxHiddenField ID="txtDataSf" runat="server" ClientIDMode="Static" ClientInstanceName="txtDataSf" />
                                            <dx:ASPxHiddenField ID="txtDataInc" runat="server" ClientIDMode="Static" ClientInstanceName="txtDataInc" />
                                        </td>
                                    </tr>
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>
            </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv respingere"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRespingeMtv" runat="server" Text="Respinge" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnMotivRespingere(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxMemo ID="txtMtv" runat="server" ClientIDMode="Static" ClientInstanceName="txtMtv" Width="630px" Height="180px"></dx:ASPxMemo>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>   

    <script>       
        var textSeparator = ',';
        
        function updateComboBoxText(comboBox, listBox) {            
            comboBox.SetText(getSelectedItemsText(listBox));
        }
        function updateListBoxValues(comboBox, listBox) {
            listBox.UnselectAll();
            listBox.SelectValues(getValuesByTexts(comboBox, listBox));            
        }
        function getSelectedItemsText(listBox) {
            var items = listBox.GetSelectedItems();
            var texts = [];

            for (var i = 0; i < items.length; i++) {
                texts.push(items[i].text);
            }

            return texts.join(textSeparator);
        }
        function getValuesByTexts(comboBox, listBox) {
            var texts = comboBox.GetText().split(textSeparator);
            var values = [];
            var item;

            for (var i = 0; i < texts.length; i++) {
                (item = listBox.FindItemByText(texts[i])) && values.push(item.value);                
            }

            return values;
        }     

        /* Page control */
        var pageControl = {
            /* Data */
            pageContent: null,
            /* Interface */
            init: function () {
                var self = this;

                self.pageContent = $('.page-content');
                ASPxClientControl.GetControlCollection().ControlsInitialized.AddHandler(function (s, e) {
                    self.onControlsInitialized(e);
                });
            },
            /* Events */
            onControlsInitialized: function (e) {
                if (!e.isCallback) { // Validate document ready                    
                    this.pageContent.find('> div[class*="invisible"]').removeClass('invisible'); // To hide DX controls UI init issues.
                }
            },
            onFilterChange: function (updateResult) {                
                cmbAng.PerformCallback(JSON.stringify({
                    viz: cmbViz.GetValue(),
                    rol: typeof cmbRol !== 'undefined' ? cmbRol.GetValue() : null,
                    stare: lstStare.GetSelectedValues(),
                    dtInc: this.getLocalDate(txtDtInc.GetDate()),
                    dtSf: this.getLocalDate(txtDtSf.GetDate()),
                    ang: cmbAng.GetValue()
                }));
                updateResult && this.onFilterButtonClick();
            },
            onFilterButtonClick: function () {
                grDate.PerformCallback('btnFiltru;' + JSON.stringify({
                    viz: cmbViz.GetValue(),
                    rol: typeof cmbRol !== 'undefined' ? cmbRol.GetValue() : null,
                    stare: lstStare.GetSelectedValues(),
                    dtInc: this.getLocalDate(txtDtInc.GetDate()),
                    dtSf: this.getLocalDate(txtDtSf.GetDate()),
                    ang: cmbAng.GetValue()
                }));
            },            
            /* Internal */
            getLocalDate: function (date) {
                var localDate = null;

                if (date instanceof Date) {
                    localDate = new Date();
                    localDate.setTime(date.getTime() + (date.getTimezoneOffset() * (-1) * 60 * 1000));
                }

                return localDate;
            }
        }

        pageControl.init();
    </script>

</asp:Content>
