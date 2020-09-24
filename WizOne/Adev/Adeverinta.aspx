<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Adeverinta.aspx.cs" Inherits="WizOne.Adev.Adeverinta" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function OnClick(s) {   
        pnlCtl.PerformCallback(s.name);   
    }

    function OnClickD112(s) {
        strUrl = getAbsoluteUrl + "Adev/ConfigD112.aspx";
        popGenD112.SetHeaderText("Date declaratie lunara - Adeverinta somaj");
        popGenD112.SetContentUrl(strUrl);
        popGenD112.Show();
    }

    function OnClickCIC() {
        if (cmbAng.GetValue() == null) {
            swal({ title: '', text: 'Nu ati selectat niciun angajat!', type: 'warning' });
            return;
        }
        strUrl = getAbsoluteUrl + "Adev/ConfigCIC.aspx";
        popGenCIC.SetHeaderText("Date Adeverinta CIC");
        popGenCIC.SetContentUrl(strUrl);
        popGenCIC.Show();
    }

    function OnValueChangedHandler(s) {
        pnlCtl.PerformCallback(s.name + ";" + s.GetValue());
    }

    function OnTextChangedHandler(s) {
        pnlCtl.PerformCallback(s.name + ";" + s.GetValue());
    }


    //function OnGenerare(s, e) {
    //    if (cmbAdev.GetValue() == 2) {
    //        swal({
    //            title: 'Sunteti sigur/a ?', text: 'Pentru adeverinta CIC este necesara completarea prealabila a paginii "Date Adeverinta CIC".\n Sunteti sigur/a ca ati completat si vreti sa continuati procesul de generare ?',
    //            type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
    //        }, function (isConfirm) {
    //            if (isConfirm) {
    //                pnlCtl.PerformCallback(s.name);
    //            }
    //        });
    //    }
    //    else {
    //        pnlCtl.PerformCallback(s.name);
    //    }
    //}

    function OnEndCallback(s, e) {
        pnlLoading.Hide();
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }

    function OnGenerare(s,e)
    {
        if (cmbAdev.GetValue() == 3) {
            OnClickCIC();
            e.processOnServer = false;
        }
        else
            e.processOnServer = true;
    }

    function HidePopupAndShowInfo() {
        popGenCIC.Hide();
        btnCreaza.DoClick();
    }

    function HidePopup() {
        popGenCIC.Hide();
    }

    function EmptyFields(s, e) {
        cmbAngBulk.SetValue(null);
        cmbCtr.SetValue(null);     

        cmbSub.SetValue(null);
        cmbSec.SetValue(null);
        cmbFil.SetValue(null);
        cmbDept.SetValue(null);
        cmbSubDept.SetValue(null);
        cmbBirou.SetValue(null);
        cmbCateg.SetValue(null);

        
    }

</script>
	<style type="text/css">
        .legend-border
        {
             border: 0;
        }
	</style>
    <body>
        <table width="100%">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnGenerare" ClientInstanceName="btnGenerare" ClientIDMode="Static" runat="server" Text="Genereaza" OnClick="btnGen_Click" oncontextMenu="ctx(this,event)">                         
                            <Image Url="~/Fisiere/Imagini/Icoane/finalizare.png"></Image>
                            <ClientSideEvents Click="function(s, e) { OnGenerare(s, e); }" />
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnCreaza" ClientInstanceName="btnCreaza" ClientIDMode="Static" runat="server" ClientVisible="false" OnClick="btnGen_Click" AutoPostBack="true"/>
                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }"  />
          <PanelCollection>
            <dx:PanelContent>
                <table style="margin-left:15px;">
                    <tr id="bulk1" runat="server">
                        <td>
                            <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                <label id="lblAngBulk" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                <dx:ASPxComboBox ID="cmbAngBulk" ClientInstanceName="cmbAngBulk" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"
                                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                    </Columns>
                                </dx:ASPxComboBox>
                            </div>                               
                            <div style="float:left; padding-right:15px;">
                                <label id="lblSub" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Subcomp.</label>
                                <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                </dx:ASPxComboBox>
                            </div>
                            <div style="float:left; padding-right:15px;">
                                <label id="lblFil" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Filiala</label>
                                <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                </dx:ASPxComboBox>
                            </div>
                            <div style="float:left; padding-right:15px;">
                                <label id="lblSec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Sectie</label>
                                <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                </dx:ASPxComboBox>
                            </div>
                            <div style="float:left; padding-right:15px;">
                                <label id="lblCateg" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Categorie</label>
                                <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="150px" ValueField="F72402" TextField="F72404" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                            </div>
                        </td>
                    </tr>
                    <tr id="bulk2" runat="server">
                        <td>
                            <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                <label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Dept.</label>
                                <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                                </dx:ASPxComboBox>
                            </div>
                            <div style="float:left; padding-right:15px;">
                                <label id="lblSubDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:80px;">Subdept.</label>
                                <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                            </div>
                            <div style="float:left; padding-right:15px;">
                                <label id="lblBirou" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Birou</label>
                                <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                    </dx:ASPxComboBox>
                            </div>   
                            <div style="float:left; padding-right:15px;">
                                <label id="lblCtr" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Contract</label>
                                <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" />
                            </div>                                                                                                                                                            
                        </td> 
                        </tr>
                        <tr>
                            <td>
                                <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                    <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                        <ClientSideEvents Click="function(s, e) {
                                                        pnlLoading.Show();
                                                        e.processOnServer = true;
                                                    }" />
                                    </dx:ASPxButton>
                         
                                    <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                                        <ClientSideEvents Click="EmptyFields" />
                                    </dx:ASPxButton>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">

                                <br />
                                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  >
                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" />                   
                                    <ClientSideEvents ContextMenu="ctx"  />
                                    <Columns>
                        
                                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />

                                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains" />
                                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true" FixedStyle="Left" VisibleIndex="3" Width="150px" Settings-AutoFilterCondition="Contains"/>
              
                                        <dx:GridViewDataTextColumn FieldName="Companie" Caption="Companie" ReadOnly="true"  Width="150" />
                                        <dx:GridViewDataTextColumn FieldName="Subcompanie" Caption="Subcompanie" ReadOnly="true"  Width="150"/>
                                        <dx:GridViewDataTextColumn FieldName="Filiala" Caption="Filiala" ReadOnly="true" Width="150" />
                                        <dx:GridViewDataTextColumn FieldName="Sectie" Caption="Sectie" ReadOnly="true" Width="150"/>
                                        <dx:GridViewDataTextColumn FieldName="Dept" Caption="Dept" ReadOnly="true" Width="100"/>
                                        <dx:GridViewDataTextColumn FieldName="Subdept" Caption="Subdept" ReadOnly="true" Width="100" />
                                        <dx:GridViewDataTextColumn FieldName="Birou" Caption="Birou" ReadOnly="true" Width="100" />

                                    </Columns>
                    
                                </dx:ASPxGridView>

                                <br />
    
                            </td>
                        </tr>
                </table>
            <table style="margin-left:15px;">
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblTipGen" runat="server" Text="Tip generare:" >                                
                        </dx:ASPxLabel>
                        <dx:ASPxRadioButton ID="rbTipGen1" Width="175" runat="server" RepeatDirection="Horizontal"  Text="intr-un singur fisier" Enabled="true"  ClientInstanceName="rbTipGen1"
                                GroupName="TipGen">                                                                
                        </dx:ASPxRadioButton>
                        <dx:ASPxRadioButton ID="rbTipGen2"  Width="175" runat="server" RepeatDirection="Horizontal"  Text="in fisiere separate" Enabled="true" ClientInstanceName="rbTipGen2" 
                                GroupName="TipGen">                              
                        </dx:ASPxRadioButton>
                    </td>
                </tr>
            </table>
			<div>
                <tr align="left">
                 <td   valign="top">
                   <fieldset  >
                    <legend class="legend-font-size">Adeverinte</legend>         
                    <table width="10%" >  
                        <tr>
                            <td>
                                <label id="lblAdev" runat="server" style="display:inline-block;">Tip adeverinta</label>
                                <dx:ASPxComboBox ID="cmbAdev" runat="server" ClientInstanceName="cmbAdev" ClientIDMode="Static" Width="175px" ValueField="Id" DropDownWidth="175" 
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >   
                                    <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />  
                                </dx:ASPxComboBox>
                            </td>
                        </tr>  
                        <tr>
                            <td>
                                <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="175px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                        CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >     
                                    <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />                               
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
                                    </Columns>                            
                                </dx:ASPxComboBox>
                            </td>
                        </tr>


                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chkVenit"  runat="server" Text="Angajati cu venituri in ..." TextAlign="Left" ClientInstanceName="chkVenit" >
                                    <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                </dx:ASPxCheckBox>
                            </td>
                            <td>
                                <dx:ASPxCheckBox ID="chkCIC"  runat="server"  Text="CIC" TextAlign="Left" ClientInstanceName="chkCIC" >
                                    <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                </dx:ASPxCheckBox>
                            </td>
                            <td>
                                <dx:ASPxCheckBox ID="chkActivi"  runat="server"  Text="Activi" TextAlign="Left"  ClientInstanceName="chkActivi" >
                                    <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                </dx:ASPxCheckBox>
                            </td>
                        </tr>
  
                    </table>
                  </fieldset >
                   <fieldset border="0">                     
                    <legend class="legend-border"></legend>   
                    <table width="10%" >
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnConfig" ClientInstanceName="btnConfig" ClientIDMode="Static" runat="server" Text="Configurare" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)">                                
                                    <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>    
                    </table>
                  </fieldset >
                </td> 
                 <td id="config" runat="server" style="display:none" valign="top">
                   <fieldset >                     
                    <legend class="legend-font-size">Setari generale</legend> 
                    <table width="10%" >
                        <tr>
                            <td align="left">                             
                                    <dx:ASPxCheckBox ID="chkRep1"  runat="server" Width="180"  Text="Apare reprezentant legal 1?" TextAlign="Left" ClientInstanceName="chkRep1" >
                                    </dx:ASPxCheckBox>   
                             </td>  
                            <td align="right">                               
                                    <dx:ASPxCheckBox ID="chkRep2"  runat="server" Width="180"  Text="Apare reprezentant legal 2?" TextAlign="Left" ClientInstanceName="chkRep2" >
                                    </dx:ASPxCheckBox>                                
                            </td>
                        </tr>    
                        <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblNumeRL1" Width="100" runat="server"  Text="Nume: " ></dx:ASPxLabel >	
							    <dx:ASPxTextBox  ID="txtNumeRL1"  Width="100" runat="server" AutoPostBack="false" >         
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                          
							    </dx:ASPxTextBox >
						    </td>
						    <td >
							    <dx:ASPxLabel  ID="lblNumeRL2" Width="100" runat="server"  Text="Nume: " ></dx:ASPxLabel >	
							    <dx:ASPxTextBox  ID="txtNumeRL2"  Width="100" runat="server"  AutoPostBack="false" >  
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                                  
							    </dx:ASPxTextBox >
						    </td>
                        </tr>
                        <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblFunctieRL1" Width="100" runat="server"  Text="Functie: " ></dx:ASPxLabel >	
							    <dx:ASPxTextBox  ID="txtFunctieRL1"  Width="100" runat="server"  AutoPostBack="false" >  
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                                  
							    </dx:ASPxTextBox >
						    </td>
						    <td >
							    <dx:ASPxLabel  ID="lblFunctieRL2" Width="100" runat="server"  Text="Functie: " ></dx:ASPxLabel >	
							    <dx:ASPxTextBox  ID="txtFunctieRL2"  Width="100" runat="server"   AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox >
						    </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta sanatate</legend> 
                    <table width="10%" >
                        <tr>
                            <td>
                                <dx:ASPxLabel ID="lblInterval" runat="server" Text="Interval:" >                                
                                </dx:ASPxLabel>
                                <dx:ASPxRadioButton ID="rbInterval1" Width="75" runat="server" RepeatDirection="Horizontal"  Text="12 luni" Enabled="true"  ClientInstanceName="rbInterval1"
                                     GroupName="Interval"> 
                                    <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                    
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbInterval2"  Width="175" runat="server" RepeatDirection="Horizontal"  Text="24 luni" Enabled="true" ClientInstanceName="rbInterval2" 
                                     GroupName="Interval">
                                    <ClientSideEvents  CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                  
                                </dx:ASPxRadioButton>
                            </td>
                            <td>
                                <dx:ASPxLabel ID="lblEmitent" runat="server" Text="Emitent:" >                                
                                </dx:ASPxLabel> 
                                <dx:ASPxRadioButton ID="rbEmitent1" Width="75" runat="server" RepeatDirection="Horizontal" Text="Angajator" Enabled="true"  ClientInstanceName="rbEmitent1"
                                     GroupName="Emitent">                                    
                                     <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbEmitent2"  Width="125" runat="server" RepeatDirection="Horizontal" Text="Casa de sanatate" Enabled="true" ClientInstanceName="rbEmitent2" 
                                     GroupName="Emitent">     
                                     <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                            
                                </dx:ASPxRadioButton>
                            </td>
                        </tr>
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblCoduri" Width="300" runat="server"  Text="Coduri indemnizatii excluse din document" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtCoduri"  Width="200" runat="server" AutoPostBack="false" >     
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                              
							    </dx:ASPxTextBox >
						    </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta venituri anuale</legend> 
                    <table width="10%" >
                        <tr>
                            <td align="left">                             
                                    <dx:ASPxCheckBox ID="chkSalNet"  runat="server" Width="150"  Text="Include salariul net?" TextAlign="Left" ClientInstanceName="chkSalNet" >
                                         <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                    </dx:ASPxCheckBox>   
                             </td> 
                        </tr> 
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblAnul" Width="100" runat="server"  Text="Anul " ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxComboBox ID="cmbAnul"  Width="75"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id"  ValueType="System.Int32">
                                     <ClientSideEvents   SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							    </dx:ASPxComboBox >
						    </td>
					    </tr>                          
                        <tr>
                            <td>
                                <dx:ASPxLabel ID="lblSumeCtr" runat="server" Text="Sume contract:" >                                
                                </dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxRadioButton ID="rbSumeContract1" Width="75" runat="server" Text="pe Marca" Enabled="true"  ClientInstanceName="rbSumeContract1"
                                     GroupName="SumeCtr">                                    
                                     <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                </dx:ASPxRadioButton>
                            </td>
                            <td>
                                <dx:ASPxRadioButton ID="rbSumeContract2"  Width="75" runat="server" Text="pe CNP" Enabled="true" ClientInstanceName="rbSumeContract2" 
                                     GroupName="SumeCtr">           
                                     <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                      
                                </dx:ASPxRadioButton>
                            </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta CIC</legend> 
                    <table width="10%" >
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblVarstaCopil" Width="175" runat="server"  Text="Varsta copil pentru filtrare" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtVarstaCopil"  Width="50" runat="server" AutoPostBack="false" >      
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                             
							    </dx:ASPxTextBox >
						    </td>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblVenit" Width="100" runat="server"  Text="Venit net realizat" ></dx:ASPxLabel >	
						    </td>	
						    <td align="right">
							    <dx:ASPxTextBox  ID="txtVenit"  Width="200" runat="server"  AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox >
						    </td>
                        </tr>    
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblZileLucrate" Width="100" runat="server"  Text="Zile lucrate" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtZileLucrate"  Width="200" runat="server" AutoPostBack="false" >   
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                                
							    </dx:ASPxTextBox >
						    </td>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblZileAbsente" Width="100" runat="server"  Text="Zile absente" ></dx:ASPxLabel >	
						    </td>	
						    <td align="right">
							    <dx:ASPxTextBox  ID="txtZileAbsente"  Width="200" runat="server"  AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox >
						    </td>
                        </tr> 
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblZileCO" Width="100" runat="server"  Text="Zile CO" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtZileCO"  Width="200" runat="server" AutoPostBack="false" >    
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                               
							    </dx:ASPxTextBox >
						    </td>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblZileCM" Width="100" runat="server"  Text="Zile CM" ></dx:ASPxLabel >	
						    </td>	
						    <td align="right">
							    <dx:ASPxTextBox  ID="txtZileCM"  Width="200" runat="server"  AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox >
						    </td>
                        </tr> 
                        <tr>
						    <td align="left">
                                <dx:ASPxButton ID="btnCIC" ClientInstanceName="btnCIC"  ClientIDMode="Static"  Width="10" Height="10" runat="server"  Text="Date Adeverinta CIC" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                                    <ClientSideEvents Click="function(s,e){ OnClickCIC(s); }" />
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
						    </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta somaj</legend> 
                    <table width="10%" >
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblTitlu" Width="100" runat="server"  Text="Titlu" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtTitlu"  Width="200" runat="server" AutoPostBack="false" > 
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                                  
							    </dx:ASPxTextBox >
						    </td>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblCompartiment" Width="100" runat="server"  Text="Compartimentul" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtCompartiment"  Width="200" runat="server"  AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox >
						    </td>
                        </tr>    
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel   Width="100" runat="server"  Text="     " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
							    <dx:ASPxLabel   Width="275" runat="server"  Text="Nume, Functie (se ia Reprezentant Legal 1)" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxLabel   Width="100" runat="server"  Text="     " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
							    <dx:ASPxLabel   Width="275" runat="server"  Text="Nume, Functie (se ia Reprezentant Legal 2)" ></dx:ASPxLabel >	
						    </td>	
                        </tr> 
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblZileSusp" Width="100" runat="server"  Text="Zile suspendare" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtZileSusp"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
						    </td>
						    <td align="left">
							    <dx:ASPxLabel   Width="100" runat="server"  Text="     " ></dx:ASPxLabel >	
						    </td>                            	
						    <td align="left">
                                <dx:ASPxButton ID="btnD112" ClientInstanceName="btnD112"  ClientIDMode="Static"  Width="10" Height="10" runat="server"  Text="Nr. inreg. D112" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                                    <ClientSideEvents Click="function(s,e){ OnClickD112(s); }" />
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
						    </td>
                        </tr>
                        <tr>
						    <td align="left">
							    <dx:ASPxLabel   Width="100" runat="server"  Text="     " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
							    <dx:ASPxLabel   Width="275" runat="server"  Text="Coduri tranzactii separate prin + (plus)" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
							    <dx:ASPxLabel   Width="100" runat="server"  Text="     " ></dx:ASPxLabel >	
						    </td>
                            <td align="left">                             
                                    <dx:ASPxCheckBox ID="chkDIS"  runat="server" Width="300"  Text="Doar suspendarile din intervalul raportat" TextAlign="Right" ClientInstanceName="chkDIS" >
                                         <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                                    </dx:ASPxCheckBox>   
                             </td>                             
                        </tr> 
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta vechime</legend> 
                    <table width="10%" >
                        <tr>
                            <td>
                                <dx:ASPxLabel ID="lblFunctie" runat="server" Text="Functia angajatului:" >                                
                                </dx:ASPxLabel>
                                <dx:ASPxRadioButton ID="rbFunc1" Width="75" runat="server" RepeatDirection="Horizontal"  Text="COR" Enabled="true"  ClientInstanceName="rbFunc1"
                                     GroupName="Functie">    
                                     <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbFunc2"  Width="175" runat="server" RepeatDirection="Horizontal"  Text="Interna" Enabled="true" ClientInstanceName="rbFunc2" 
                                     GroupName="Functie">        
                                     <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                         
                                </dx:ASPxRadioButton>
                            </td>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblNEM" Width="100" runat="server"  Text="Zile NEM" ></dx:ASPxLabel >	
                                <dx:ASPxLabel  ID="lblCFP" Width="100" runat="server"  Text="Zile CFP" ></dx:ASPxLabel >
						    </td>
						    <td align="left">
							    <dx:ASPxTextBox  ID="txtNEM"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
							    <dx:ASPxTextBox  ID="txtCFP"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
						    </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta stagiu</legend> 
                    <table width="10%" >
                        <tr>
                            <td>
                                <dx:ASPxLabel ID="lblIntervalStagiu" runat="server" Text="Interval:" >                                
                                </dx:ASPxLabel>
                                <dx:ASPxRadioButton ID="rbIntervalStagiu1" Width="75" runat="server" RepeatDirection="Horizontal"  Text="6 luni" Enabled="true"  ClientInstanceName="rbIntervalStagiu1"
                                     GroupName="IntervalStagiu"> 
                                    <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                    
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbIntervalStagiu2"  Width="175" runat="server" RepeatDirection="Horizontal"  Text="12 luni" Enabled="true" ClientInstanceName="rbIntervalStagiu2" 
                                     GroupName="IntervalStagiu">
                                    <ClientSideEvents  CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                  
                                </dx:ASPxRadioButton>
                            </td>
						    <td align="left">
							    <dx:ASPxLabel  ID="lblBCM" Width="100" runat="server"  Text="Baza calcul CM" ></dx:ASPxLabel >	
						    </td>	
						    <td align="right">
							    <dx:ASPxTextBox  ID="txtBCM"  Width="200" runat="server"  AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox >
						    </td>
                        </tr>                   
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Setari Adeverinta deplasare</legend> 
                    <table width="10%" >   
                        <tr>                 
						    <td align="left">
							    <dx:ASPxLabel  ID="lblSubsem" Width="100" runat="server"  Text="Subsemnatul" ></dx:ASPxLabel >
                             </td>
                             <td align="left">
							    <dx:ASPxTextBox  ID="txtSubsem"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
                             </td>
                             <td align="left">
                                <dx:ASPxLabel  ID="lblDom" Width="200" runat="server"  Text="Domeniul activitatii profesionale" ></dx:ASPxLabel >
                            </td>
                             <td align="left">
                                 <dx:ASPxTextBox  ID="txtDom"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
                            </td>
                        </tr>
                        <tr>
                              <td align="left">
                                <dx:ASPxLabel  ID="lblFunc" Width="100" runat="server"  Text="Functia" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtFunc"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
                            </td>
                              <td align="left">
							    <dx:ASPxLabel  ID="lbllLoc" Width="200" runat="server"  Text="Locul de desfasurare al activitatii profesionale" ></dx:ASPxLabel >
						    </td>
                            <td align="right">
							    <dx:ASPxTextBox  ID="txtLoc"  Width="200" runat="server" AutoPostBack="false" >        
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />                           
							    </dx:ASPxTextBox >
						    </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset border="0">                     
                    <legend class="legend-border"></legend>   
                    <table width="40%" >
                        <tr align="center">
                            <td >
                                <dx:ASPxButton ID="btnSalvare" ClientInstanceName="btnSalvare" ClientIDMode="Static" runat="server" Text="Salvare" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)">                                
                                    <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                            <td >
                                <dx:ASPxButton ID="btnAnulare" ClientInstanceName="btnAnulare" ClientIDMode="Static" runat="server" Text="Anulare" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)" >                                
                                    <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>
  
                    </table>
                  </fieldset >
                 </td>
            </tr>      
		</div>
            </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>

    <dx:ASPxPopupControl ID="popGenCIC" runat="server" AllowDragging="True" AllowResize="True" Modal="true"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Middle"
        EnableViewState="False" PopupElementID="popupArea" PopupHorizontalAlign="WindowCenter" ShowCloseButton="false"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="False" Width="1000px" MinHeight="500px"
        Height="100%" FooterText=" " CloseOnEscape="false" ClientInstanceName="popGenCIC" EnableHierarchyRecreation="True">                
            <ContentCollection>
                <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">

                </dx:PopupControlContentControl>
            </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>