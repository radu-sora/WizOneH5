<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuizCreate.ascx.cs" Inherits="WizOne.Eval.QuizCreate" %>

<script type="text/javascript">

    function OnValueChanged(s) {
        switch(s.name)
        {
            case "txtDenumireQuiz":
                lstValori.Set('Denumire', s.GetValue());
                break;
            case "chkQuizActiv":
                lstValori.Set('Activ', s.GetValue());
                break;
            case "dtDataInceput":
                lstValori.Set('DataInceput', s.GetValue());
                break;
            case "dtDataSfarsit":
                lstValori.Set('DataSfarsit', s.GetValue());
                break;
            case "cmbPerioada":
                lstValori.Set('Anul', s.GetValue());
                break;
            case "txtTitlu":
                lstValori.Set('Titlu', s.GetValue());
                break;
            case "chkPreluareAutomataRaspunsuri":
                lstValori.Set('Preluare', s.GetValue());
                break;
            case "cmbCateg":
                lstValori.Set('CategorieQuiz', s.GetValue());
                break;
            case "chkLuatLaCunostinta":
                lstValori.Set('LuatLaCunostinta', s.GetValue());
                break;
            case "txtNrZileLuatLaCunostinta":
                lstValori.Set('NrZileLuatLaCunostinta', s.GetValue());
                break;
            case "cmbIdRaport":
                lstValori.Set('IdRaport', s.GetValue());
                break;
            case "chkSinc":
                lstValori.Set('Sincronizare', s.GetValue());
                break;
        }
    }


    function OnCMBTipChanged(s, e) {
        hf1.Set("Id", "3");
        panel2.PerformCallback(s.name + ";" + s.GetValue() + ";" + hf.Get("Id"));
    }
    function OnCHKChanged(s, e) {
        hf1.Set("Id", "3");
        panel2.PerformCallback(s.name + ";" + s.GetValue() + ";" + hf.Get("Id"));
    }

    function OnTXTChanged(s, e) {
        hf1.Set("Id", "3");
        panel2.PerformCallback(s.name + ";" + s.GetText() + ";" + hf.Get("Id"));
    }

    function AddSectiune(s, e) {
        hf1.Set("Id", "2");
        panel2.PerformCallback(s.name);
    }

    function DeleteSectiuneSauIntrebare(s, e) {
        if (grDateIntrebari.GetNodeState(grDateIntrebari.GetFocusedNodeKey()) != "Child") {
            swal({
                title: "Operatie nepermisa", text: "Pentru a putea sterge, nodul nu trebuie sa aiba subnivele",
                type: "warning"
            });
        }
        else {
            swal({
                title: 'Sunteti sigur/a ?', text: 'Vreti sa stergeti sectiunea curenta din chestionar ?',
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    hf1.Set("Id", "-1");
                    panel2.PerformCallback(s.name);
                }
            });
        }
    }

    function AddIntrebare(s, e) {
        hf1.Set("Id", "1");
        panel2.PerformCallback(s.name);
    }

    //function DeleteIntrebare(s, e) {

    //    swal({
    //        title: 'Sunteti sigur/a ?', text: 'Vreti sa stergeti intrebarea curenta din chestionar ?',
    //        type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
    //    }, function (isConfirm) {
    //        if (isConfirm) {
    //            hf1.value = "-1";
    //            panel2.PerformCallback(s.name);
    //        }
    //    });
    //}


    //LeonardM 22.11.2017
    //pentru completare obiective in functie de alegere lista
    var currentEditingIndex;
    var lastTipLista;
    var isCustomCascadingCallback = false;

    function RefreshData(TipListaValue, ColumnNameValue) {
        hfObiectiv.Set("CurrentTipLista", TipListaValue);
        IdNomenclatorEditor.PerformCallback();
    }

    function RefreshDataTipListaValue(TipListaValue) {
        hfObiectiv.Set("CurrentTipLista", TipListaValue);
        IdNomenclatorEditor.PerformCallback();
    }

    // Value array contains "EmployeeID" and "Notes" field values returned from the server 
    function OnGetRowValuesGridObiective(values) {
        hfObiectivColName.Set("CurrentColumnNameObiectiv", values[1]);

        RefreshDataTipListaValue(lastTipLista);
    }

    function cmbObiectivTipLista_SelectedIndexChanged(s, e) {
        //LeonardM 25.11.2017
        //eveniment ce lanseaza modificarea listei Unnamed_ItemsRequestedByFilterCondition
        //In interiorul metodei OnGetRowValuesGridObiective apelam RefreshDataTipListaValue pentru a fi sigur
        //ca se actualizeaza ok CurrentTipLista si CurrentColumnNameObiectiv
        lastTipLista = s.GetValue();
        grDateObiective.GetRowValues(grDateObiective.GetFocusedRowIndex(),
                            'Id;ColumnName', OnGetRowValuesGridObiective);

        isCustomCascadingCallback = true;
        //old LeonardM 25.11.2017
        //RefreshData(lastTipLista, 'Obiectiv');
        //RefreshDataTipListaValue(lastTipLista);
    }

    function cmbNomenclator_EndCallBack(s, e) {
        if (isCustomCascadingCallback) {
            if (s.GetItemCount() > 0)
                grDateObiective.BatchEditApi.SetCellValue(currentEditingIndex, "IdNomenclator", s.GetItem(0).value);
            isCustomCascadingCallback = false;
        }
    }

    function OnBatchEditStartEditing(s, e) {
        currentEditingIndex = e.visibleIndex;
        var currentTipLista = grDateObiective.BatchEditApi.GetCellValue(currentEditingIndex, "TipValoare");
        var currentColumnName = grDateObiective.BatchEditApi.GetCellValue(currentEditingIndex, "ColumnName");
        
        if (currentTipLista != lastTipLista && e.focusedColumn.fieldName == "IdNomenclator" && currentTipLista != null) {
            lastTipLista = currentTipLista;
            RefreshData(currentTipLista, currentColumnName);
        }
    }

    function FocusedRowChangedObiectiv(s, e) {
        currentEditingIndex = s.GetRowKey(s.GetFocusedRowIndex());
        if (grDateObiective && grDateObiective.BatchEditApi) {
            var currentColumnName = grDateObiective.BatchEditApi.GetCellValue(currentEditingIndex, "ColumnName");
            hfObiectivColName.Set("CurrentColumnNameObiectiv", ColumnNameValue);
            IdNomenclatorEditor.PerformCallback();
        }
    }

    //LeonardM 06.12.2017
    //completare competente
    var lastTipListaCompetente;
    var currentEditingIndexCompetente;
    var isCustomCascadingCompetenteCallback = false;


    function RefreshDataTipListaValueCompetente(TipListaValue) {
        hfCompetente.Set("CurrentTipListaCompetenta", TipListaValue);
        IdNomenclatorCompetenteEditor.PerformCallback();
    }

    function OnGetRowValuesGridCompetente(values) {
        hfCompetenteColName.Set("CurrentColumnNameCompetente", values[1]);

        RefreshDataTipListaValueCompetente(lastTipListaCompetente);
    }

    function OnBatchEditStartEditingCompetente(s, e) {
        currentEditingIndexCompetente = e.visibleIndex;
        var currentColumnName = grDateCompetente.BatchEditApi.GetCellValue(currentEditingIndexCompetente, "ColumnName");
        hfCompetenteColName.Set("CurrentColumnNameCompetente", currentColumnName);
        //if (currentTipLista != lastTipLista && e.focusedColumn.fieldName == "IdNomenclator" && currentTipLista != null) {
        //    lastTipLista = currentTipLista;
        //    RefreshData(currentTipLista, currentColumnName);
        //}
    }
    function FocusedRowChangedCompetente(s, e) {
        currentEditingIndexCompetente = s.GetRowKey(s.GetFocusedRowIndex());
        isCustomCascadingCompetenteCallback = true;
        //var currentColumnName = grDateCompetente.BatchEditApi.GetCellValue(currentEditingIndexCompetente, "ColumnName");
        //var currentColumnName = "Competenta";
        //var currentColumnName = currentEditingIndexCompetente;
        if (currentEditingIndexCompetente == 1)
        {
            hfCompetenteColName.Set("CurrentColumnNameCompetente", "Competenta");
        }
        else if (currentEditingIndexCompetente == 3)
        {
            hfCompetenteColName.Set("CurrentColumnNameCompetente", "Calificativ");
        }
        else
            hfCompetenteColName.Set("CurrentColumnNameCompetente", "");

        //hfCompetenteColName.Set("CurrentColumnNameCompetente", currentColumnName);
        if (typeof IdNomenclatorCompetenteEditor !== "undefined" && ASPxClientUtils.IsExists(IdNomenclatorCompetenteEditor)) {
            IdNomenclatorCompetenteEditor.PerformCallback();
        }   
    }

    function cmbNomenclatorCompetente_EndCallBack(s, e) {
        if (isCustomCascadingCompetenteCallback) {
            if (s.GetItemCount() > 0)
                grDateCompetente.BatchEditApi.SetCellValue(currentEditingIndexCompetente, "IdNomenclator", s.GetItem(0).value);
            isCustomCascadingCompetenteCallback = false;
        }
    }

    //end LeonardM 22.11.2017

    function Ordonare() {
        if (typeof grDateOrdonare !== 'undefined' && (hf1.Get("Id") == -1 || hf1.Get("Id") == 1 || hf1.Get("Id") == 2)) {
            grDateOrdonare.UpdateEdit();
            grDateOrdonare.PerformCallback();
        }
    }

</script>


<body>
    <table style="margin-left:20px;">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblFirstDelimitator" runat="server" Width="5" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxFormLayout runat="server" ID="partGeneral" >
                                <Items>
                                    <dx:LayoutGroup TabImage-AlternateText="Date Generale" Caption="Date Generale" >
                                        <Items>
                                            <dx:LayoutItem Caption="">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer>
                                                        <asp:DataList ID="dtList1" runat="server">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <tr>
                                                                        <fieldset class="fieldset-auto-width">
                                                                            <table width="30%" >
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblDenumireQuiz" Width="100" runat="server" Text="Denumire" />
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxTextBox ID="txtDenumireQuiz" ClientInstanceName="txtDenumireQuiz" ClientIDMode="Static" Width="400" runat="server" Text='<%# Eval("Denumire") %>' AutoPostBack="false">
                                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnValueChanged(s); }"  />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td style="margin-left:50px;">
                                                                                        <dx:ASPxCheckBox ID="chkQuizActiv" runat="server" Text="Activ" TextAlign="Left"
                                                                                            Checked='<%#  Eval("Activ") == DBNull.Value ? false : Convert.ToBoolean(Eval("Activ"))%>'
                                                                                            ClientInstanceName="chkQuizActiv" ClientIDMode="Static">
                                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxCheckBox>

                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblCateg" Width="60" runat="server" Text="Categorie" />
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox DataSourceID="dsCategorie" Value='<%# Eval("CategorieQuiz") %>' ID="cmbCateg" runat="server" Width="100px"
                                                                                            DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false"
                                                                                            ValueType="System.Int32" ClientInstanceName="cmbCateg" ClientIDMode="Static">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxComboBox>
                                                                                        <asp:ObjectDataSource runat="server" ID="dsCategorie" TypeName="WizOne.Module.Evaluare" SelectMethod="GetEval_tblCategorie" />                                                                                    
                                                                                    </td>
                                                                                </tr>
                                                                            </table>

                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblIntre1" Width="100" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                    
                                                                            <table width="30%">
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblDataInceput" Width="100" runat="server" Text="Data Inceput" />
                                                                                    </td>
                                                                                    <td width="150">
                                                                                        <dx:ASPxDateEdit ID="dtDataInceput" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" 
                                                                                            EditFormatString="dd.MM.yyyy" ClientInstanceName="dtDataInceput" ClientIDMode="Static"
                                                                                            Value='<%# Eval("DataInceput") %>' AutoPostBack="false" >
                                                                                            <CalendarProperties FirstDayOfWeek="Monday" />
                                                                                            <ClientSideEvents DateChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxDateEdit>
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblDataSfarsit" Width="70" runat="server" Text="Data Sfarsit" />
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxDateEdit ID="dtDataSfarsit" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" 
                                                                                            EditFormatString="dd.MM.yyyy" ClientInstanceName="dtDataSfarsit" ClientIDMode="Static"
                                                                                            Value='<%# Eval("DataSfarsit") %>' AutoPostBack="false" >
                                                                                            <CalendarProperties FirstDayOfWeek="Monday" />
                                                                                            <ClientSideEvents DateChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxDateEdit>
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblPerioada" runat="server" Text="Perioada" />
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox DataSourceID="dsPerioada" Value='<%# Eval("Anul") %>' ID="cmbPerioada" runat="server"
                                                                                            DropDownStyle="DropDown" TextField="DenPerioada" ValueField="IdPerioada" AutoPostBack="false"
                                                                                            ValueType="System.Int32" ClientInstanceName="cmbPerioada" ClientIDMode="Static">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxComboBox>
                                                                                        <asp:ObjectDataSource runat="server" ID="dsPerioada" TypeName="WizOne.Module.Evaluare" SelectMethod="GetEval_Perioada" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>

                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblIntre2" Width="100" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>

                                                                            <table width="30">
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblTitlu" Width="100" runat="server" Text="Titlu" />
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxTextBox ID="txtTitlu" Width="400" ClientInstanceName="txtTitlu" ClientIDMode="Static" runat="server" Text='<%# Eval("Titlu") %>' AutoPostBack="false">
                                                                                            <ClientSideEvents TextChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td> 
                                                                                    <td>
                                                                                        <dx:ASPxCheckBox ID="chkPreluareAutomataRaspunsuri" Width="220" runat="server" Text="Preluare automata a raspunsurilor"
                                                                                            TextAlign="Left" Checked='<%#  Eval("Preluare") == DBNull.Value ? false : Convert.ToBoolean(Eval("Preluare"))%>'
                                                                                            ClientInstanceName="chkPreluareAutomataRaspunsuri" ClientIDMode="Static">
                                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxCheckBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <br />
                                                                            <table width="30">
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblLuatLaCunostinta" Width="100" runat="server" Text="Luat la cunostinta" />
                                                                                    </td>
                                                                                   <td>
                                                                                        <dx:ASPxCheckBox ID="chkLuatLaCunostinta" Width="20" runat="server"
                                                                                            TextAlign="Left" Checked='<%#  Eval("LuatLaCunostinta") == DBNull.Value ? false : Convert.ToBoolean(Eval("LuatLaCunostinta"))%>'
                                                                                            ClientInstanceName="chkLuatLaCunostinta" ClientIDMode="Static">
                                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxCheckBox>
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblNrZile" Width="160" runat="server" Text="Numar zile luat la cunostinta" />
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxTextBox ID="txtNrZileLuatLaCunostinta" Width="30" ClientInstanceName="txtNrZileLuatLaCunostinta" ClientIDMode="Static" runat="server" Text='<%# Eval("NrZileLuatLaCunostinta") %>' AutoPostBack="false">
                                                                                            <ClientSideEvents TextChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblIdRaport" Width="50" runat="server" Text="Raport" />
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox ID="cmbIdRaport" ClientInstanceName="cmbIdRaport" ClientIDMode="Static" runat="server" DropDownStyle="DropDown" Value='<%# Eval("IdRaport") %>' TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" DataSourceID="dsRaps" >
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxComboBox>
                                                                                        <asp:ObjectDataSource runat="server" ID="dsRaps" TypeName="WizOne.Module.Evaluare" SelectMethod="GetRapoarte" />
                                                                                    </td>
                                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                                    <td>
                                                                                        <dx:ASPxLabel ID="lblSinc" Width="144" runat="server" Text="Sincronizare raspunsuri" />
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxCheckBox ID="chkSinc" Width="20" runat="server" ClientInstanceName="chkSinc" ClientIDMode="Static" Checked='<%#  Eval("Sincronizare") == DBNull.Value ? false : Convert.ToBoolean(Eval("Sincronizare"))%>' >
                                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnValueChanged(s); }" />
                                                                                        </dx:ASPxCheckBox>
                                                                                    </td>
                                                                                 </tr>
                                                                            </table>
                                                                                                                        
                                                                        </fieldset>
                                                                    </tr>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:DataList>
                                                    </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:LayoutGroup>
                                </Items>
                            </dx:ASPxFormLayout>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblBetweenZones" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxCallbackPanel ID="panel2" ClientInstanceName="panel2" runat="server" Width="200px" OnCallback="panel2_Callback" SettingsLoadingPanel-Enabled="false">
                                <ClientSideEvents BeginCallback="function (s,e) { pnlLoading.Show(); }" EndCallback="function (s,e) { pnlLoading.Hide(); Ordonare(); }" />
                                <PanelCollection>
                                    <dx:PanelContent runat="server">

                                        <dx:ASPxFormLayout runat="server" ID="partGrid">
                                            <Items>
                                                <dx:LayoutGroup Caption="Rezumat">
                                                    <Items>
                                                        <dx:LayoutItem Caption="">
                                                            <LayoutItemNestedControlCollection>
                                                                <dx:LayoutItemNestedControlContainer>
                                                                     <table>
                                                                         <tr>
                                                                            <td align="right">
                                                                                <dx:ASPxButton ID="btnDeleteSectiune" runat="server" ClientIDMode="Static" ClientInstanceName="btnDeleteSectiune" Text="" AutoPostBack="false">
                                                                                    <ClientSideEvents Click="function(s, e){ DeleteSectiuneSauIntrebare(s); }" />
                                                                                    <Image Url="../Fisiere/Imagini/Icoane/sterge.png" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                         </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxTreeList ID="grDateIntrebari" ClientInstanceName="grDateIntrebari" ClientIDMode="Static" runat="server" AutoGenerateColumns="false"
                                                                                    KeyFieldName="Id" ParentFieldName="Parinte" Width="780px" >
                                                                                    <Settings GridLines="Both" HorizontalScrollBarMode="Visible" ShowRoot="true" />
                                                                                    <SettingsBehavior AutoExpandAllNodes="true" AllowFocusedNode="true" FocusNodeOnLoad="false" ProcessFocusedNodeChangedOnServer="True" />
                                                                                    <SettingsEditing Mode="Inline"/>
                                                                                    <ClientSideEvents FocusedNodeChanged="function(s, e) {
	                                                                                            hf.Set('Id',s.GetFocusedNodeKey());
                                                                                                hf1.Set('Id','');
                                                                                                panel2.PerformCallback();
                                                                                            }" />
                                                                                    <Columns>
                                                                                        <dx:TreeListTextColumn FieldName="Id" Visible="false" />
                                                                                        <dx:TreeListTextColumn FieldName="Descriere" Caption="Descriere" Visible="true" VisibleIndex="0" Width="100%" ReadOnly="true" />
                                                                                        <dx:TreeListTextColumn FieldName="Parinte" Visible="false" />
                                                                                    </Columns>
                                                                                </dx:ASPxTreeList>
                                                                            </td>
                                                                        </tr>         
                                                                    </table>
                                                                </dx:LayoutItemNestedControlContainer>
                                                            </LayoutItemNestedControlCollection>
                                                        </dx:LayoutItem>
                                                    </Items>
                                                </dx:LayoutGroup>
                                            </Items>
                                        </dx:ASPxFormLayout>


                                        <dx:ASPxFormLayout runat="server" ID="partSectiune" >
                                            <Items>
                                                <dx:LayoutGroup TabImage-AlternateText="Sectiune" Caption="Sectiune" >
                                                    <Items>
                                                        <dx:LayoutItem Caption="">
                                                            <LayoutItemNestedControlCollection>
                                                                <dx:LayoutItemNestedControlContainer>
                                                                    <table>  
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxLabel ID="lblDenSectiune" Text="Descriere" runat="server" Width="100" />
                                                                            </td>
                                                                            <td> 
                                                                                <dx:ASPxTextBox ID="txtDenSectiune" runat="server" Width="620" AutoPostBack="false" ClientInstanceName="txtDenSectiune" >
                                                                                    
                                                                                </dx:ASPxTextBox>
                                                                            </td>
                                                                            <td>&nbsp;&nbsp;&nbsp;</td>
                                                                            <td>
                                                                                <dx:ASPxButton ID="btnAddSectiune" runat="server" ClientInstanceName="btnAddSectiune" ClientIDMode="Static" Text="" AutoPostBack="false" >
                                                                                    <ClientSideEvents Click="function(s, e){ AddSectiune(s); }" />
                                                                                    <Image Url="../Fisiere/Imagini/Icoane/adauga.png" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                        </tr>
                                                                     </table>
                                                                </dx:LayoutItemNestedControlContainer>
                                                             </LayoutItemNestedControlCollection>
                                                        </dx:LayoutItem>
                                                    </Items>
                                                </dx:LayoutGroup>
                                            </Items>
                                        </dx:ASPxFormLayout>


                                        <dx:ASPxFormLayout runat="server" ID="partIntrebari">
                                            <Items>
                                                <dx:LayoutGroup TabImage-AlternateText="Intrebari" Caption="Intrebari" >
                                                    <Items>

                                                <dx:LayoutItem Caption="">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer>
                                                            <table>
                                                            </table>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblTipIntrebare" Text="Tip" runat="server" Width="100" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbTip" ClientInstanceName="cmbTip" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" >
                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblTipIntrebareObiect" Text="Obiect" runat="server" Width="90" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbTipObiect" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" ClientInstanceName="cmbTipObiect" >
                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblPerioadaObi" Width="70" runat="server" Text="Perioada:" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbPerioadaObi" runat="server"
                                                                            DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false"
                                                                            ValueType="System.Int32" ClientInstanceName="cmbPerioadaObi" >
                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblPerioadaComp" Width="70" runat="server" Text="Perioada:" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbPerioadaComp" runat="server"
                                                                            DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false"
                                                                            ValueType="System.Int32" ClientInstanceName="cmbPerioadaComp" >
                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblOrientare" runat="server" Text="Orientare" Width="70" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxRadioButton ID="chkOrizontal" runat="server" Width="75" Text="Orizontal" ClientInstanceName="chkOrizontal" GroupName="Orientare" AutoPostBack="false" >
                                                                             <ClientSideEvents CheckedChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxRadioButton ID="chkVertical" runat="server" Width="75" Text="Vertical" ClientInstanceName="chkVertical" GroupName="Orientare" AutoPostBack="false" >
                                                                            <ClientSideEvents CheckedChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblSecPart5" Width="5" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblGrup" runat="server" Width="40" Text="Grup" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbGrup" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" CssClass="margin_rightt15"
                                                                            AutoPostBack="false" ValueType="System.Int32" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblSursaDate" runat="server" Width="100" Text="Sursa date" ClientVisible="false" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbSursaDate" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" CssClass="margin_rightt15"
                                                                            AutoPostBack="false" ValueType="System.Int32" ClientInstanceName="cmbSursaDate" ClientVisible="false">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td></td>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblCategObi" runat="server" Width="100" Text="Categ. obiectiv" ClientVisible="false" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbCategObi" ClientInstanceName="cmbCategObi" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" ClientVisible="false" CssClass="margin_rightt15"/>
                                                                    </td>                                                                    
                                                                    <td></td>
                                                                    <td>
                                                                        <dx:ASPxCheckBox ID="chkObligatoriu" runat="server" Text="Obligatoriu" TextAlign="Left" ClientInstanceName="chkObligatoriu" AutoPostBack="false" Width="100px" CssClass="margin_rightt15">
                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                        </dx:ASPxCheckBox>
                                                                    </td>
                                                                    <td></td>
                                                                     <td>
                                                                        <dx:ASPxLabel ID="lblPrelObi" runat="server" Text="Preluare obiective:" Width="100" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxRadioButton ID="chkNomenclator" runat="server" Width="80" Text="Nomenclator" ClientInstanceName="chkNomenclator" GroupName="PrelObi" AutoPostBack="false" >
                                                                             <ClientSideEvents CheckedChanged="function(s, e){ 
                                                                                                                    OnCHKChanged(s); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxRadioButton ID="chkObiective" runat="server" Width="150" Text="Obiective individuale" ClientInstanceName="chkObiective" GroupName="PrelObi" AutoPostBack="false" >
                                                                            <ClientSideEvents CheckedChanged="function(s, e){ 
                                                                                                                    OnCHKChanged(s); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </td>         

                                                                     <td>
                                                                        <dx:ASPxLabel ID="lblPrelComp" runat="server" Text="Preluare competente:" Width="120" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxRadioButton ID="chkNomenclatorComp" runat="server" Width="80" Text="Nomenclator" ClientInstanceName="chkNomenclatorComp" GroupName="PrelComp" AutoPostBack="false" >
                                                                             <ClientSideEvents CheckedChanged="function(s, e){ 
                                                                                                                    OnCHKChanged(s); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxRadioButton ID="chkCompetente" runat="server" Width="150" Text="Competente" ClientInstanceName="chkCompetente" GroupName="PrelComp" AutoPostBack="false" >
                                                                            <ClientSideEvents CheckedChanged="function(s, e){ 
                                                                                                                    OnCHKChanged(s); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </td>                                                                    

                                                                    <td>
                                                                        <dx:ASPxCheckBox ID="chkSalvareOb" runat="server" Text="Salvare obiective" TextAlign="Left" ClientInstanceName="chkSalvareOb" AutoPostBack="false" Width="120px" CssClass="margin_rightt15">
                                                                            <ClientSideEvents ValueChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                        </dx:ASPxCheckBox>
                                                                    </td>

                                                                </tr>
                                                            </table>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblSecPart8" runat="server" Width="5" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxLabel ID="lblDesIntrebare" runat="server" Text="Descriere" Width="100" />
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxTextBox ID="txtDescIntrebare" runat="server" Width="620" ClientInstanceName="txtDescIntrebare" ClientIDMode="Static" AutoPostBack="false">
                                                                            
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td>&nbsp;&nbsp;&nbsp;</td>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnAddIntrebare" runat="server" ClientIDMode="Static" ClientInstanceName="btnAddIntrebare" Text="" AutoPostBack="false">
                                                                            <ClientSideEvents Click="function(s, e){ AddIntrebare(s); }" />
                                                                            <Image Url="../Fisiere/Imagini/Icoane/adauga.png" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                        
                                                    </Items>
                                                </dx:LayoutGroup>
                                            </Items>

                                        </dx:ASPxFormLayout>


                                        <dx:ASPxFormLayout runat="server" ID="partGridObiective">
                                            <Items>
                                                <dx:LayoutGroup Caption="Obiective">
                                                    <Items>
                                                        <dx:LayoutItem Caption="">
                                                            <LayoutItemNestedControlCollection>
                                                                <dx:LayoutItemNestedControlContainer>
                                                                    <table cellspacing="20">
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxLabel ID="lblTemplateObiective" Text="Template&nbsp;&nbsp;&nbsp;" runat="server" ClientInstanceName="lblTemplateObiective" ClientIDMode="Static" />
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxComboBox ID="cmbTemplateObiective" runat="server" DropDownStyle="DropDown" ValueField="TemplateId" TextField="TemplateName" ValueType="System.Int32" AutoPostBack="false" >
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                                </dx:ASPxComboBox>
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxLabel ID="lblMinObiective" Text="Obiective - minim&nbsp;&nbsp;&nbsp;" runat="server" ClientInstanceName="lblMinObiective" ClientIDMode="Static" Width="110px" />
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxTextBox ID="txtMinObiective" runat="server" ClientInstanceName="txtMinObiective" ClientIDMode="Static" AutoPostBack="false" Width="50px">
                                                                                    <MaskSettings Mask="<0..999g>" />
                                                                                    <ClientSideEvents TextChanged="function(s, e){ OnTXTChanged(s); }" />
                                                                                    <ValidationSettings Display="Dynamic" />
                                                                                </dx:ASPxTextBox>
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxLabel ID="lblMaxObiective" Text="maxim&nbsp;&nbsp;&nbsp;" runat="server" ClientInstanceName="lblMaxObiective" ClientIDMode="Static" />
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxTextBox ID="txtMaxObiective" runat="server" ClientInstanceName="txtMaxObiective" ClientIDMode="Static" AutoPostBack="false" Width="50px">
                                                                                    <MaskSettings Mask="<0..999g>" />
                                                                                    <ClientSideEvents TextChanged="function(s, e){ OnTXTChanged(s); }" />
                                                                                    <ValidationSettings Display="Dynamic" />
                                                                                </dx:ASPxTextBox>
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxCheckBox ID="chkCanAddObiective" Width="100px" runat="server" Text="poate adauga" TextAlign="Left" ClientInstanceName="chkCanAddObiective" AutoPostBack="false" >
                                                                                    <ClientSideEvents ValueChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxCheckBox ID="chkCanDeleteObiective" Width="100px" runat="server" Text="poate sterge" TextAlign="Left" ClientInstanceName="chkCanDeleteObiective" AutoPostBack="false" >
                                                                                    <ClientSideEvents ValueChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxCheckBox ID="chkCanEditObiective" Width="100px" runat="server" Text="poate edita" TextAlign="Left" ClientInstanceName="chkCanAddObiective" AutoPostBack="false" >
                                                                                    <ClientSideEvents ValueChanged="function(s, e){ OnCHKChanged(s); }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <br />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="9">
                                                                                <dx:ASPxHiddenField runat="server" ID="hfObiectiv" ClientInstanceName="hfObiectiv" />
                                                                                <dx:ASPxHiddenField runat="server" ID="hfObiectivColName" ClientInstanceName="hfObiectivColName" />
                                                                                <dx:ASPxGridView ID="grDateObiective" runat="server" SettingsPager-PageSize="50" ClientInstanceName="grDateObiective" ClientIDMode="Static" 
                                                                                    Width="1000px" AutoGenerateColumns="false" 
                                                                                    OnAutoFilterCellEditorInitialize="grDateObiective_AutoFilterCellEditorInitialize" 
                                                                                    OnCellEditorInitialize="grDateObiective_CellEditorInitialize"
                                                                                    OnRowUpdating="grDateObiective_RowUpdating" 
                                                                                    OnCustomErrorText="grDateObiective_CustomErrorText">
                                                                                    <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" ColumnResizeMode="NextColumn" />
                                                                                    <Settings ShowFilterRow="false" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" />
                                                                                    <SettingsSearchPanel Visible="false" />
                                                                                    <ClientSideEvents ContextMenu="ctx" BatchEditStartEditing="OnBatchEditStartEditing" FocusedRowChanged="FocusedRowChangedObiectiv" />
                                                                                    <SettingsEditing Mode="Inline" />
                                                                                    <Columns>
                                                                                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image"
                                                                                            Caption=" " />

                                                                                        <dx:GridViewDataTextColumn FieldName="Id" Name ="Id" Caption="Id" Visible="false" />
                                                                                        <dx:GridViewDataTextColumn FieldName="TemplateId" Name="TemplateId" Caption="TemplateId" Visible="false" />
                                                                                        <dx:GridViewDataTextColumn FieldName="ColumnName" Name="ColumnName" Caption="ColumnName" Visible="true" Width="150" VisibleIndex="1" />
                                                                                        <dx:GridViewDataTextColumn FieldName="Width" Name="Width" Caption="Latime" Width="80" VisibleIndex="2" >
                                                                                            <PropertiesTextEdit DisplayFormatString="n0" >
                                                                                                <MaskSettings Mask="<0..999g>" />
                                                                                            </PropertiesTextEdit>
                                                                                        </dx:GridViewDataTextColumn>
                                                                                        <dx:GridViewDataCheckColumn FieldName="Obligatoriu" Name="Obligatoriu" Caption="Obligatoriu"  Width="70px" VisibleIndex="2"  />
                                                                                        <dx:GridViewDataCheckColumn FieldName="Citire" Name="Citire" Caption="Citire"  Width="70px" VisibleIndex="3" />
                                                                                        <dx:GridViewDataCheckColumn FieldName="Editare" Name="Editare" Caption="Editare"  Width="70px" VisibleIndex="4" />
                                                                                        <dx:GridViewDataCheckColumn FieldName="Vizibil" Name="Vizibil" Caption="Vizibil"  Width="70px" VisibleIndex="5" />
                                                                                        <dx:GridViewDataComboBoxColumn FieldName="TipValoare" Name="TipValoare" Caption="Tip Valoare" Width="150" VisibleIndex="6" >
                                                                                            <PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                                                <ValidationSettings RequiredField-IsRequired="true" Display="None" />
                                                                                                <ClientSideEvents SelectedIndexChanged="cmbObiectivTipLista_SelectedIndexChanged" />
                                                                                            </PropertiesComboBox>
                                                                                        </dx:GridViewDataComboBoxColumn>
                                                                                        <dx:GridViewDataComboBoxColumn FieldName="IdNomenclator" Name="IdNomenclator" Caption="Nomenclator" Width="150" VisibleIndex="7">
                                                                                            <PropertiesComboBox TextField="DenNomenclator" ValueField="IdNomenclator" ValueType="System.Int32" DropDownStyle="DropDown" EnableCallbackMode="true" OnItemRequestedByValue="Unnamed_ItemRequestedByValue" OnItemsRequestedByFilterCondition="Unnamed_ItemsRequestedByFilterCondition" />
                                                                                        </dx:GridViewDataComboBoxColumn>
                                                                                        <dx:GridViewDataSpinEditColumn FieldName="Ordine" Name ="Ordine" Caption="Ordine" VisibleIndex="9" PropertiesSpinEdit-SpinButtons-ShowIncrementButtons="false" PropertiesSpinEdit-MinValue="1" PropertiesSpinEdit-MaxValue="99"/>
                                                                                        <dx:GridViewDataTextColumn FieldName="FormulaSql" Name="FormulaSql" Caption="FormulaSql" VisibleIndex="10" />
                                                                                        <dx:GridViewDataTextColumn FieldName="Alias" Name="Alias" Caption="Alias" VisibleIndex="12" />
                                                                                        <dx:GridViewDataComboBoxColumn FieldName="TotalColoana" Name="TotalColoana" Caption="Total Coloana" Width="150" VisibleIndex="14" PropertiesComboBox-AllowNull="true">
                                                                                            <PropertiesComboBox>
                                                                                                <Items>
                                                                                                    <dx:ListEditItem Value="1" Text="Suma fara zecimale" />
                                                                                                    <dx:ListEditItem Value="2" Text="Suma cu 2 zecimale" />
                                                                                                    <dx:ListEditItem Value="3" Text="Medie fara zecimale" />
                                                                                                    <dx:ListEditItem Value="4" Text="Medie cu 2 zecimale" />
                                                                                                    <dx:ListEditItem Value="5" Text="Valoare minima" />
                                                                                                    <dx:ListEditItem Value="6" Text="Valoare maxima" />
                                                                                                </Items>
                                                                                            </PropertiesComboBox>
                                                                                        </dx:GridViewDataComboBoxColumn>
                                                                                    </Columns>
                                                                                    <SettingsCommandButton>
                                                                                        <EditButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                                                                            <Styles>
                                                                                                <Style Paddings-PaddingRight="5px" />
                                                                                            </Styles>
                                                                                        </EditButton>
                                                                                        <UpdateButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                                                                            <Styles>
                                                                                                <Style Paddings-PaddingRight="5px" />
                                                                                            </Styles>
                                                                                        </UpdateButton>
                                                                                        <CancelButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                                                                        </CancelButton>
                                                                                    </SettingsCommandButton>
                                                                                </dx:ASPxGridView>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </dx:LayoutItemNestedControlContainer>
                                                            </LayoutItemNestedControlCollection>
                                                        </dx:LayoutItem>
                                                    </Items>
                                                </dx:LayoutGroup>
                                            </Items>
                                        </dx:ASPxFormLayout>


                                        <dx:ASPxFormLayout ID="partGridCompetente" runat="server">
                                            <Items>
                                                <dx:LayoutGroup Caption="Competente">
                                                    <Items>
                                                        <dx:LayoutItem Caption="">
                                                            <LayoutItemNestedControlCollection>
                                                                <dx:LayoutItemNestedControlContainer>
                                                                    <table cellspacing="20">
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxLabel ID="lblTemplateCompetente" Text="Template:" runat="server" ClientInstanceName="lblTemplateCompetente" ClientIDMode="Static" />
                                                                            </td>
                                                                            <td>
                                                                                <dx:ASPxComboBox ID="cmbTemplateCompetente" runat="server" DropDownStyle="DropDown" ValueField="TemplateId" ValueType="System.Int32" TextField="TemplateName" AutoPostBack="false">
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s, e){ OnCMBTipChanged(s); }" />
                                                                                </dx:ASPxComboBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <dx:ASPxHiddenField runat="server" ID="hfCompetente" ClientInstanceName="hfCompetente" />
                                                                                <dx:ASPxHiddenField runat="server" ID="hfCompetenteColName" ClientInstanceName="hfCompetenteColName" />
                                                                                <dx:ASPxGridView ID="grDateCompetente" runat="server" SettingsPager-PageSize="50" ClientInstanceName="grDateCompetente" ClientIDMode="Static"
                                                                                    Width="1100px" AutoGenerateColumns="false"
                                                                                    OnAutoFilterCellEditorInitialize="grDateCompetente_AutoFilterCellEditorInitialize"
                                                                                    OnCellEditorInitialize="grDateCompetente_CellEditorInitialize"
                                                                                    OnRowUpdating="grDateCompetente_RowUpdating"
                                                                                    OnCustomErrorText="grDateCompetente_CustomErrorText">
                                                                                    <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true"
                                                                                        AllowSelectByRowClick="true" ColumnResizeMode="NextColumn" />
                                                                                    <Settings ShowFilterRow="false" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" />
                                                                                    <SettingsSearchPanel Visible="false" />
                                                                                    <ClientSideEvents ContextMenu="ctx" BatchEditStartEditing="OnBatchEditStartEditingCompetente" FocusedRowChanged="FocusedRowChangedCompetente" />
                                                                                    <SettingsEditing Mode="Inline" />
                                                                                    <Columns>
                                                                                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />

                                                                                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" Visible="false" />
                                                                                        <dx:GridViewDataTextColumn FieldName="TemplateId" Name="TemplateId" Caption="TemplateId" Visible="false" />
                                                                                        <dx:GridViewDataTextColumn FieldName="ColumnName" Name="ColumnName" Caption="ColumnName" Visible="true" Width="150" VisibleIndex="1" />
                                                                                        <dx:GridViewDataTextColumn FieldName="Width" Name="Width" Caption="Latime" Width="100" VisibleIndex="2" >
                                                                                            <PropertiesTextEdit DisplayFormatString="n0" >
                                                                                                <MaskSettings Mask="<0..999g>" />
                                                                                            </PropertiesTextEdit>
                                                                                        </dx:GridViewDataTextColumn>
                                                                                        <dx:GridViewDataCheckColumn FieldName="Obligatoriu" Name="Obligatoriu" Caption="Obligatoriu"  Width="70px" VisibleIndex="2"  />
                                                                                        <dx:GridViewDataCheckColumn FieldName="Citire" Name="Citire" Caption="Citire"  Width="70px" VisibleIndex="3" />
                                                                                        <dx:GridViewDataCheckColumn FieldName="Editare" Name="Editare" Caption="Editare"  Width="70px" VisibleIndex="4" />
                                                                                        <dx:GridViewDataCheckColumn FieldName="Vizibil" Name="Vizibil" Caption="Vizibil"  Width="70px" VisibleIndex="5" />
                                                                                        <dx:GridViewDataComboBoxColumn FieldName="IdNomenclator" Name="IdNomenclator" Caption="Nomenclator" Width="150" VisibleIndex="7">
                                                                                            <PropertiesComboBox TextField="DenNomenclator" ValueField="IdNomenclator" ValueType="System.Int32" DropDownStyle="DropDown" EnableCallbackMode="true" OnItemRequestedByValue="Unnamed_ItemRequestedByValue1" OnItemsRequestedByFilterCondition="Unnamed_ItemsRequestedByFilterCondition1"/>
                                                                                        </dx:GridViewDataComboBoxColumn>
                                                                                        <dx:GridViewDataSpinEditColumn FieldName="Ordine" Name ="Ordine" Caption="Ordine" VisibleIndex="9" PropertiesSpinEdit-SpinButtons-ShowIncrementButtons="false" PropertiesSpinEdit-MinValue="1" PropertiesSpinEdit-MaxValue="99"/>
                                                                                        <dx:GridViewDataTextColumn FieldName="FormulaSql" Name="FormulaSql" Caption="FormulaSql" VisibleIndex="10" />
                                                                                        <dx:GridViewDataTextColumn FieldName="Alias" Name="Alias" Caption="Alias" VisibleIndex="12" />
                                                                                        <dx:GridViewDataComboBoxColumn FieldName="TotalColoana" Name="TotalColoana" Caption="Total Coloana" Width="150" VisibleIndex="14" PropertiesComboBox-AllowNull="true">
                                                                                            <PropertiesComboBox>
                                                                                                <Items>
                                                                                                    <dx:ListEditItem Value="1" Text="Suma fara zecimale" />
                                                                                                    <dx:ListEditItem Value="2" Text="Suma cu 2 zecimale" />
                                                                                                    <dx:ListEditItem Value="3" Text="Medie fara zecimale" />
                                                                                                    <dx:ListEditItem Value="4" Text="Medie cu 2 zecimale" />
                                                                                                    <dx:ListEditItem Value="5" Text="Valoare minima" />
                                                                                                    <dx:ListEditItem Value="6" Text="Valoare maxima" />
                                                                                                </Items>
                                                                                            </PropertiesComboBox>
                                                                                        </dx:GridViewDataComboBoxColumn>
                                                                                    </Columns>
                                                                                    <SettingsCommandButton>
                                                                                        <EditButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                                                                            <Styles>
                                                                                                <Style Paddings-PaddingRight="5px" />
                                                                                            </Styles>
                                                                                        </EditButton>
                                                                                        <UpdateButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                                                                            <Styles>
                                                                                                <Style Paddings-PaddingRight="5px" />
                                                                                            </Styles>
                                                                                        </UpdateButton>
                                                                                        <CancelButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                                                                        </CancelButton>
                                                                                    </SettingsCommandButton>
                                                                                </dx:ASPxGridView>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </dx:LayoutItemNestedControlContainer>
                                                            </LayoutItemNestedControlCollection>
                                                        </dx:LayoutItem>
                                                    </Items>
                                                </dx:LayoutGroup>
                                            </Items>
                                        </dx:ASPxFormLayout>

                                        <dx:ASPxFormLayout ID="pnlConfigTipTabela" runat="server">
                                            <Items>
                                                <dx:LayoutGroup Caption="Configurare tip control tabela">
                                                    <Items>
                                                        <dx:LayoutItem Caption="">
                                                            <LayoutItemNestedControlCollection>
                                                                <dx:LayoutItemNestedControlContainer>
                                                                    <table cellspacing="20">
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <dx:ASPxGridView ID="grDateTabela" runat="server" SettingsPager-PageSize="50" ClientInstanceName="grDateTabela" ClientIDMode="Static" AutoGenerateColumns="false" KeyFieldName="IdQuiz;IdLinie;Coloana"
                                                                                     OnRowInserting="grDateTabela_RowInserting" OnRowUpdating="grDateTabela_RowUpdating" OnRowDeleting="grDateTabela_RowDeleting" OnInitNewRow="grDateTabela_InitNewRow">
                                                                                    <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" ColumnResizeMode="NextColumn" />
                                                                                    <Settings ShowFilterRow="false" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" />
                                                                                    <SettingsSearchPanel Visible="false" />
                                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                                    <SettingsEditing Mode="Inline" />
                                                                                    <Columns>
                                                                                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />

                                                                                        <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" Visible="false" ShowInCustomizationForm="false" />
                                                                                        <dx:GridViewDataTextColumn FieldName="IdLinie" Name="IdLinie" Caption="IdLinie" Visible="false" ShowInCustomizationForm="false" />
                                                                                        <dx:GridViewDataComboBoxColumn FieldName="Coloana" Name="Coloana" Caption="Coloana" Width="120">
                                                                                            <PropertiesComboBox>
                                                                                                <Items>
                                                                                                    <dx:ListEditItem Value="1" Text="Coloana 1" />
                                                                                                    <dx:ListEditItem Value="2" Text="Coloana 2" />
                                                                                                    <dx:ListEditItem Value="3" Text="Coloana 3" />
                                                                                                    <dx:ListEditItem Value="4" Text="Coloana 4" />
                                                                                                    <dx:ListEditItem Value="5" Text="Coloana 5" />
                                                                                                    <dx:ListEditItem Value="6" Text="Coloana 6" />
                                                                                                </Items>
                                                                                            </PropertiesComboBox>
                                                                                        </dx:GridViewDataComboBoxColumn>
                                                                                        <dx:GridViewDataSpinEditColumn FieldName="Lungime" Name="Lungime" Caption="Lungime" Width="100" PropertiesSpinEdit-MinValue="10" PropertiesSpinEdit-MaxValue="1000" PropertiesSpinEdit-SpinButtons-ShowIncrementButtons="false"/>
                                                                                        <dx:GridViewDataTextColumn FieldName="Alias" Name="Alias" Caption="Alias" Width="250"/>
                                                                                        
                                                                                    </Columns>
                                                                                    <SettingsCommandButton>
                                                                                        <EditButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                                                                            <Styles>
                                                                                                <Style Paddings-PaddingRight="5px" />
                                                                                            </Styles>
                                                                                        </EditButton>
						                                                                <DeleteButton>
							                                                                <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
						                                                                </DeleteButton>
                                                                                        
                                                                                        <UpdateButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                                                                            <Styles>
                                                                                                <Style Paddings-PaddingRight="5px" />
                                                                                            </Styles>
                                                                                        </UpdateButton>
                                                                                        <CancelButton>
                                                                                            <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                                                                        </CancelButton>

                                                                                        <NewButton>
                                                                                            <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                                                                                        </NewButton>
                                                                                    </SettingsCommandButton>
                                                                                </dx:ASPxGridView>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </dx:LayoutItemNestedControlContainer>
                                                            </LayoutItemNestedControlCollection>
                                                        </dx:LayoutItem>
                                                    </Items>
                                                </dx:LayoutGroup>
                                            </Items>
                                        </dx:ASPxFormLayout>


                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxCallbackPanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblEndDelimitator" runat="server" Width="5" />
                            <dx:ASPxHiddenField ID="hf" runat="server" ClientIDMode="Static" ClientInstanceName="hf" />
                            <dx:ASPxHiddenField ID="hf1" runat="server" ClientIDMode="Static" ClientInstanceName="hf1" />
                            <dx:ASPxHiddenField ID="lstValori" runat="server" ClientIDMode="Static" ClientInstanceName="lstValori" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
   </table>
</body>