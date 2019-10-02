<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Documente.ascx.cs" Inherits="WizOne.Personal.Documente" %>



<script type="text/javascript">

    function GoToIstoricDoc(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    function OnEndCallbackDoc(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }

    function dtActiv_DateChanged(s) {
        var dtInc = new Date(deDataInc.GetDate());
        var dtSf = new Date(deDataSf.GetDate());

        if (dtSf < dtInc) {
            swal({title: "", text: "Data start este ulterioara celei de final!",type: "warning"});
        }
    }

    function dtDoc_DateChanged(s) {
        var dtInc = new Date(deDataElib.GetDate());
        var dtSf = new Date(deDataExp.GetDate());
        var msg = "";

        if (dtSf < dtInc) {
            msg = "Data eliberarii BI/CI este ulterioara datei expirarii!";
        }

        var azi = new Date();
        if (dtInc > azi) {
            msg = msg + " Data eliberarii BI/CI este ulterioara zilei curente!";
        }

        if (msg != "")
            swal({ title: "", text: msg, type: "warning" });
    }

    function dtPermis_DateChanged(s) {
        var dtInc = new Date(deDataEmitere.GetDate());
        var dtSf = new Date(deDataExpirare.GetDate());

        if (dtSf < dtInc) {
            swal({ title: "", text: "Data expirarii este anterioara datei emiterii!", type: "warning" });
        }
    }

    function Livret_DateChanged(s) {
        var dtInc = new Date(deDeLaDataLivMil.GetDate());
        var dtSf = new Date(deLaDataLivMil.GetDate());

        if (dtSf < dtInc) {
            swal({ title: "", text: "Data start este ulterioara celei de final!", type: "warning" });
        }
    }

    function cmbTara_SelectedIndexChanged(s) {
        //pnlCtlDocumente.PerformCallback(s.name + ";" + s.GetValue());
        hfTara.Set('Tara', s.GetValue());
        cmbTipDoc.ClearItems();
        var tara = 0;
        var tipDoc = "<%=Session["MP_ComboTipDoc"] %>";
        var res = tipDoc.split(";");
        for (var i = 0; i < res.length; i++) {
            var linie = res[i].split(",");
            if (linie[2] == cmbTara.GetValue()) {         
                cmbTipDoc.AddItem(linie[1], Number(linie[0]));
                tara = Number(linie[3]);
            }
        }
        cmbTipDoc.SetSelectedIndex(0);
        cmbCetatenie.SetSelectedIndex(tara);

        if (tara == 3) {
            cmbTipAutMunca.SetEnabled(true);
            deDataInc.SetEnabled(true);
            deDataSf.SetEnabled(true);
        }
        else {
            cmbTipAutMunca.SetEnabled(false);
            deDataInc.SetEnabled(false);
            deDataSf.SetEnabled(false);
        }
    }
</script>
<body>
    <table width="100%">
		<tr>
			<td align="left">					
			</td>			
		
		</tr>			
	</table>
				

   <dx:ASPxCallbackPanel ID = "Documente_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlDocumente" runat="server" OnCallback="Documente_pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
       <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackDoc(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="Documente_DataList" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top" width="310">
			      <fieldset >
				        <legend id="lgNat" runat="server" class="legend-font-size">Nationalitate</legend>
				        <table id="lgNatTable" runat="server" width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTara" Width="100" runat="server"  Text="Tara/Nationalitate" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTN" Width="150"  Value='<%#Eval("F100987") %>' ID="cmbTara" TabIndex="1" ClientInstanceName="cmbTara"  runat="server" DropDownStyle="DropDown"  TextField="F73304" ValueField="F73302" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ cmbTara_SelectedIndexChanged(s); }" />
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblCetatenie" Width="100" runat="server"  Text="Cetatenie" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsCet" Width="150" Value='<%#Eval("F100981") %>' ID="cmbCetatenie" ClientInstanceName="cmbCetatenie"  runat="server" DropDownStyle="DropDown"  TextField="F73204" ValueField="F73202" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTipAutMunca" Width="100" runat="server"  Text="Tip autorizatie munca" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTAM" Width="150"  Value='<%#Eval("F100911") %>' ID="cmbTipAutMunca" TabIndex="2" ClientInstanceName="cmbTipAutMunca"  runat="server" DropDownStyle="DropDown"  TextField="F08803" ValueField="F08802" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataInc" runat="server"  Text="Data inceput"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataInc" ClientInstanceName="deDataInc" Width="100" runat="server" TabIndex="3"  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100912") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ dtActiv_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataSf" runat="server"  Text="Data sfarsit"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataSf" ClientInstanceName="deDataSf" Width="100" runat="server" TabIndex="4" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100913") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ dtActiv_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsTN" TypeName="WizOne.Module.General" SelectMethod="GetF733"/>
                      <asp:ObjectDataSource runat="server" ID="dsCet" TypeName="WizOne.Module.General" SelectMethod="GetCetatenie"/>
                      <asp:ObjectDataSource runat="server" ID="dsTAM" TypeName="WizOne.Module.General" SelectMethod="GetTipAutMunca"/>
			        </fieldset>
			        <fieldset >
				    <legend id="lgDocID" runat="server" class="legend-font-size">Document identitate</legend>
				        <table id="lgDocIDTable" runat="server" width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNumeMama"  Width="100"  runat="server"  Text="Nume mama"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeMama" Width="150"  runat="server" Text='<%# Eval("F100988") %>' TabIndex="5" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNumeTata" Width="100" runat="server"  Text="Nume tata"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeTata" Width="150"  runat="server" Text='<%# Eval("F100989") %>' TabIndex="6" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblTipDoc" Width="100" runat="server"  Text="Tip document(BI/CI)"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150"  Value='<%#Eval("F100983") %>' ID="cmbTipDoc" TabIndex="7" ClientInstanceName="cmbTipDoc" runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnDocId" ClientInstanceName="btnDocId"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"   RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnDocIdIst" ClientInstanceName="btnDocIdIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblSerieNr" Width="100" runat="server"  Text="Serie si numar"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtSerieNr" Width="150"  runat="server" TabIndex="8" Text='<%# Eval("F10052") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblEmisDe" Width="100" runat="server"  Text="Emis de"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtEmisDe" Width="150"  runat="server" TabIndex="9" Text='<%# Eval("F100521") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLocNastere" Width="100" runat="server"  Text="Loc nastere"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtLocNastere" Width="150"  runat="server" TabIndex="10" Text='<%# Eval("F100980") %>' AutoPostBack="false" >

							        </dx:ASPxTextBox>
						        </td>
					        </tr> 					        
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataELib" Width="100" runat="server"  Text="Data eliberarii"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataElib" ClientInstanceName="deDataElib" TabIndex="11" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100522") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ dtDoc_DateChanged(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>					        
					        <tr>
						        <td>		
							        <dx:ASPxLabel  ID="lblDataExp" Width="100" runat="server"  Text="Data expirarii"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataExp" ClientInstanceName="deDataExp" Width="100" TabIndex="12" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100963") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ dtDoc_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>                                                                                                                               					       
				        </table>
			        </fieldset>
                 </td> 
            <td  valign="top" width="310">
			      <fieldset >
				        <legend id="lgDetCtr" runat="server" class="legend-font-size">Detalii contract</legend>
				        <table id="lgDetCtrTable" runat="server" width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblNrPermisMunca" Width="100" runat="server"  Text="Numar permis munca" ></dx:ASPxLabel >	
						        </td>							        
						        <td>
							        <dx:ASPxTextBox  ID="txtNrPermisMunca" Width="150"  runat="server" TabIndex="13" Text='<%# Eval("F100982") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblDataPermisMunca" Width="100" runat="server"  Text="Data permis munca" ></dx:ASPxLabel >	
						        </td>	
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataPermisMunca" Width="100" runat="server" TabIndex="14" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100994") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblNrCtrIntVechi" Width="100" runat="server"  Text="Numar contract intern vechi" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtNrCtrIntVechi" Width="150"  runat="server" TabIndex="15" Text='<%# Eval("F100940") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataCtrIntVechi" runat="server"  Text="Data contract intern vechi"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtDataCtrIntVechi" Width="150"  runat="server" TabIndex="16" Text='<%# Eval("F100941") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDetaliiCtrAngajat" runat="server"  Text="Detalii contract angajat"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtDetaliiCtrAngajat" Width="150"  runat="server" TabIndex="17" Text='<%# Eval("F100942") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
				        </table>
			        </fieldset>
			        <fieldset >
				    <legend id="lgPermis" runat="server" class="legend-font-size">Permis auto</legend>
				        <table id="lgPermisTable" runat="server" width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblCateg"  Width="100"  runat="server"  Text="Categorie"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsCateg" Value='<%#Eval("F10028") %>' ID="cmbCateg" TabIndex="18"   runat="server" DropDownStyle="DropDown"  TextField="F71404" ValueField="F71402" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnPermis" ClientInstanceName="btnPermis"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnPermisIst" ClientInstanceName="btnPermisIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataEmitere" Width="100" runat="server"  Text="Data emitere"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataEmitere" ClientInstanceName="deDataEmitere" Width="100" TabIndex="19" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10024") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ dtPermis_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataExpirare" Width="100" runat="server"  Text="Data expirare"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataExpirare" ClientInstanceName="deDataExpirare" Width="100" TabIndex="20" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F1001000")%> '  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ dtPermis_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNr" Width="100" runat="server"  Text="Numar"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNr" Width="150"  runat="server" TabIndex="21" Text='<%# Eval("F1001001") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblPermisEmisDe" Width="100" runat="server"  Text="Emis de"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtPermisEmisDe" Width="150"  runat="server" TabIndex="22" Text='<%# Eval("F1001002") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>					                                                                                                                                     					       
				        </table>
                        <asp:ObjectDataSource runat="server" ID="dsCateg" TypeName="WizOne.Module.General" SelectMethod="GetCategPermis"/>
			        </fieldset>
                 </td> 

            <td  valign="top" >
			      <fieldset >
				        <legend id="lgStudii" runat="server"></legend>
				        <table id="lgStudiiTable" runat="server" width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblStudii" Width="100" runat="server"  Text="Studii" ></dx:ASPxLabel >	
						        </td>							        
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsStudii" Value='<%#Eval("F10050") %>' ID="cmbStudiiDoc" TabIndex="23"  runat="server" DropDownStyle="DropDown"  TextField="F71204" ValueField="F71202" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnStudii" ClientInstanceName="btnStudii"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnStudiiIst" ClientInstanceName="btnStudiiIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblCalif1" Width="100" runat="server"  Text="Calificare 1" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtCalif1" Width="150"  runat="server" Text='<%# Eval("F100903") %>' TabIndex="24" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblCalif2" Width="100" runat="server"  Text="Calificare 2" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtCalif2" Width="150"  runat="server" Text='<%# Eval("F100904") %>' TabIndex="25" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblTitluAcademic" runat="server"  Text="Titlu academic"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsTitluAcad" Value='<%#Eval("F10051") %>' ID="cmbTitluAcademic" TabIndex="26"  runat="server" DropDownStyle="DropDown"  TextField="F71304" ValueField="F71302" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnTitluAcad" ClientInstanceName="btnTitluAcad"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnTitluAcadIst" ClientInstanceName="btnTitluAcadIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDedSomaj" runat="server"  Text="Deduceri somaj"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsDedSomaj" Value='<%#Eval("F10073") %>' ID="cmbDedSomaj" TabIndex="27"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNrCarteMunca" runat="server"  Text="Numar carte munca"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNrCarteMunca" Width="150"  runat="server" Text='<%# Eval("F10012") %>' TabIndex="28" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblSerieCarteMunca" runat="server"  Text="Serie carte munca"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtSerieCarteMunca" Width="150"  runat="server" Text='<%# Eval("F10013") %>' TabIndex="29" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataCarteMunca" Width="100" runat="server"  Text="Data carte munca"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataCarteMunca" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="30" EditFormatString="dd.MM.yyyy" Value='<%#Eval("FX1")%>'   AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsStudii" TypeName="WizOne.Module.General" SelectMethod="GetStudii"/>
                      <asp:ObjectDataSource runat="server" ID="dsTitluAcad" TypeName="WizOne.Module.General" SelectMethod="GetTitluAcademic"/>
                      <asp:ObjectDataSource runat="server" ID="dsDedSomaj" TypeName="WizOne.Module.General" SelectMethod="ListaMP_DeduceriSomaj"/>
			        </fieldset>
			        <fieldset >
				    <legend id="lgStagiu" runat="server" class="legend-font-size">Stagiu militar</legend>
				        <table id="lgStagiuTable" runat="server" width="60%">	
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLivret" runat="server"  Text="Livret militar(serie/numar)"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtLivret" Width="150"  runat="server" Text='<%# Eval("F100571") %>' TabIndex="31" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblElibDe" runat="server"  Text="Eliberat de"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtElibDe" Width="150"  runat="server" Text='<%# Eval("F100572") %>' TabIndex="32" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>                               		
						        <td>
							        <dx:ASPxLabel  ID="lblDeLaData" Width="100" runat="server"  Text="De la data"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDeLaDataLivMil" ClientInstanceName="deDeLaDataLivMil" Width="100" runat="server" TabIndex="33" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100573") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                         <ClientSideEvents DateChanged="function(s,e){ Livret_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>                            
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLaData" Width="100" runat="server"  Text="La data"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deLaDataLivMil" ClientInstanceName="deLaDataLivMil" Width="100" runat="server" TabIndex="34" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100574")%>'   AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                         <ClientSideEvents DateChanged="function(s,e){ Livret_DateChanged(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>                            
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblGrad" runat="server"  Text="Grad"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtGrad" Width="150"  runat="server" Text='<%# Eval("F100575") %>' TabIndex="35" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblOrdin" runat="server"  Text="Ordin"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtOrdin" Width="150"  runat="server" Text='<%# Eval("F100576") %>' TabIndex="36" AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>                                                                                    					                                                                                                                                     					       
				        </table>
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
      <dx:ASPxHiddenField runat="server" ID="hfTara" ClientInstanceName="hfTara" />
</body>