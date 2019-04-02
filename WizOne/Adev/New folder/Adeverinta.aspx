<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Adeverinta.aspx.cs" Inherits="WizOne.Adev.Adeverinta" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

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
        //popGenCIC.SetContentUrl(strUrl);
        popGenCIC.Show();
    }

    function OnValueChangedHandler(s) {
        if (s.name == "cmbAdev")
        {
            if (cmbAdev.GetValue() == 2) {
                //swal({ title: 'Atentie!', text: 'Pentru adeverinta CIC este necesara completarea prealabila a paginii "Date Adeverinta CIC"!', type: 'warning' });
            }
        }
        else
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
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }

    function OnGenerare(s,e)
    {
        if (cmbAdev.GetValue() == 2) {
            OnClickCIC();
            e.processOnServer = false;
        }
        else
            e.processOnServer = true;
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

                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }"  />
          <PanelCollection>
            <dx:PanelContent>
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

                    <table style="width:100%;">
                        <tr>
                            <td style="float:right; text-align:right;">
                                <dx:ASPxButton ID="btnCreaza" ClientInstanceName="btnCreaza" ClientIDMode="Static" runat="server" Text="Genereaza" OnClick="btnCreaza_Click" oncontextMenu="ctx(this,event)">                         
                                    <Image Url="~/Fisiere/Imagini/Icoane/finalizare.png"></Image>
                                    <ClientSideEvents Click="function (s,e) { popGenCIC.Hide(); }" />
                                </dx:ASPxButton>
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align:top;">
                                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnInitNewRow="grDate_InitNewRow" 
                                    OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting">
                                    <SettingsBehavior  AllowFocusedRow="true"  />
                                    <Settings ShowFilterRow="False" ShowColumnHeaders="true" />
                                    <SettingsSearchPanel Visible="False" />   
                                    <SettingsEditing Mode="Inline" />    
                                    <Columns>	
                                        <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                                        <dx:GridViewDataTextColumn FieldName="MARCA" Name="MARCA" Caption="Marca" Width="75px" ReadOnly="true" />
                                        <dx:GridViewDataTextColumn FieldName="NUME" Name="NUME" Caption="Nume si prenume copil"  Width="180px" />
                                        <dx:GridViewDataDateColumn FieldName="DATA_NASTERE" Name="DATA_NASTERE" Caption="Data nastere copil"  Width="100px" >
                                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                        </dx:GridViewDataDateColumn>
                                        <dx:GridViewDataDateColumn FieldName="MATERNITATE_DELA" Name="MATERNITATE_DELA" Caption="Indemnizatie maternitate in perioada de la"  Width="100px" >
                                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                        </dx:GridViewDataDateColumn>
                                        <dx:GridViewDataDateColumn FieldName="MATERNITATE_PANALA" Name="MATERNITATE_PANALA" Caption="Indemnizatie maternitate in perioada pana la"  Width="100px" >
                                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                        </dx:GridViewDataDateColumn>
                                        <dx:GridViewDataDateColumn FieldName="INDEMNIZATIE_DELA" Name="INDEMNIZATIE_DELA" Caption="Indemnizatie copil in perioada de la"  Width="100px" >
                                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                        </dx:GridViewDataDateColumn>
                                        <dx:GridViewDataDateColumn FieldName="INDEMNIZATIE_PANALA" Name="INDEMNIZATIE_PANALA" Caption="Indemnizatie copil in perioada pana la"  Width="100px" >
                                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                        </dx:GridViewDataDateColumn>
                                        <dx:GridViewDataDateColumn FieldName="DATA_APROBARE" Name="DATA_APROBARE" Caption="Data aprobare CIC"  Width="100px" >
                                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                        </dx:GridViewDataDateColumn>
						            </Columns>
                                      <SettingsCommandButton>
                                            <UpdateButton ButtonType="Link" Text="Actualizeaza">
                                                <Styles>
                                                    <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
                                                    </Style>
                                                </Styles>
                                            </UpdateButton>
                                            <CancelButton ButtonType="Link" Text="Renunta">
                                            </CancelButton>

                                            <EditButton Image-ToolTip="Edit">
                                                <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                                                <Styles>
                                                    <Style Paddings-PaddingRight="5px" />
                                                </Styles>
                                            </EditButton>
                                            <DeleteButton Image-ToolTip="Sterge">
                                                <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                                            </DeleteButton>
                                            <NewButton Image-ToolTip="Rand nou">
                                                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                                <Styles>
                                                    <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                                </Styles>
                                            </NewButton>
                                        </SettingsCommandButton>
                                </dx:ASPxGridView>
                            </td>
                        </tr>
                    </table>


                </dx:PopupControlContentControl>
            </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>