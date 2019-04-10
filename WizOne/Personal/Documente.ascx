<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Documente.ascx.cs" Inherits="WizOne.Personal.Documente" %>



<script type="text/javascript">

    function OnTextChangedHandlerDoc(s) {
        pnlCtlDocumente.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerDoc(s) {
        pnlCtlDocumente.PerformCallback(s.name + ";" + s.GetValue());
    }
    function OnClickDoc(s) {
        pnlLoading.Show();
        pnlCtlDocumente.PerformCallback(s.name);
    }

    function GoToIstoricDoc(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    function OnEndCallbackDoc(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
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
				

   <dx:ASPxCallbackPanel ID = "pnlCtlDocumente" ClientIDMode="Static" ClientInstanceName="pnlCtlDocumente" runat="server" OnCallback="pnlCtlDocumente_Callback" SettingsLoadingPanel-Enabled="false">
       <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackDoc(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top" width="310">
			      <fieldset >
				        <legend class="legend-font-size">Nationalitate</legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTara" Width="100" runat="server"  Text="Tara/Nationalitate" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTN" Width="150"  Value='<%#Eval("F100987") %>' ID="cmbTara"   runat="server" DropDownStyle="DropDown"  TextField="F73304" ValueField="F73302" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblCetatenie" Width="100" runat="server"  Text="Cetatenie" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsCet" Width="150" Value='<%#Eval("F100981") %>' ID="cmbCetatenie"   runat="server" DropDownStyle="DropDown"  TextField="F73204" ValueField="F73202" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTipAutMunca" Width="100" runat="server"  Text="Tip autorizatie munca" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTAM" Width="150"  Value='<%#Eval("F100911") %>' ID="cmbTipAutMunca"   runat="server" DropDownStyle="DropDown"  TextField="F08803" ValueField="F08802" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataInc" runat="server"  Text="Data inceput"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataInc" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100912") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataSf" runat="server"  Text="Data sfarsit"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataSf" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100913") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsTN" TypeName="WizOne.Module.General" SelectMethod="GetF733"/>
                      <asp:ObjectDataSource runat="server" ID="dsCet" TypeName="WizOne.Module.General" SelectMethod="GetCetatenie"/>
                      <asp:ObjectDataSource runat="server" ID="dsTAM" TypeName="WizOne.Module.General" SelectMethod="GetTipAutMunca"/>
			        </fieldset>
			        <fieldset >
				    <legend class="legend-font-size">Document identitate</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNumeMama"  Width="100"  runat="server"  Text="Nume mama"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeMama" Width="150"  runat="server" Text='<%# Eval("F100988") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNumeTata" Width="100" runat="server"  Text="Nume tata"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeTata" Width="150"  runat="server" Text='<%# Eval("F100989") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblTipDoc" Width="100" runat="server"  Text="Tip document(BI/CI)"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150"  Value='<%#Eval("F100983") %>' ID="cmbTipDoc"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnDocId" ClientInstanceName="btnDocId"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"   RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ OnClickDoc(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnDocIdIst" ClientInstanceName="btnDocIdIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricDoc(s); }" />
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
							        <dx:ASPxTextBox  ID="txtSerieNr" Width="150"  runat="server" Text='<%# Eval("F10052") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblEmisDe" Width="100" runat="server"  Text="Emis de"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtEmisDe" Width="150"  runat="server" Text='<%# Eval("F100521") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLocNastere" Width="100" runat="server"  Text="Loc nastere"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtLocNastere" Width="150"  runat="server" Text='<%# Eval("F100980") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr> 					        
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataELib" Width="100" runat="server"  Text="Data eliberarii"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataElib" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100522") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>					        
					        <tr>
						        <td>		
							        <dx:ASPxLabel  ID="lblDataExp" Width="100" runat="server"  Text="Data expirarii"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataExp" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100963") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>                                                                                                                               					       
				        </table>
			        </fieldset>
                 </td> 
            <td  valign="top" width="310">
			      <fieldset >
				        <legend class="legend-font-size">Detalii contract</legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblNrPermisMunca" Width="100" runat="server"  Text="Numar permis munca" ></dx:ASPxLabel >	
						        </td>							        
						        <td>
							        <dx:ASPxTextBox  ID="txtNrPermisMunca" Width="150"  runat="server" Text='<%# Eval("F100982") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblDataPermisMunca" Width="100" runat="server"  Text="Data permis munca" ></dx:ASPxLabel >	
						        </td>	
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataPermisMunca" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100994") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblNrCtrIntVechi" Width="100" runat="server"  Text="Numar contract intern vechi" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtNrCtrIntVechi" Width="150"  runat="server" Text='<%# Eval("F100940") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataCtrIntVechi" runat="server"  Text="Data contract intern vechi"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtDataCtrIntVechi" Width="150"  runat="server" Text='<%# Eval("F100941") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDetaliiCtrAngajat" runat="server"  Text="Detalii contract angajat"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtDetaliiCtrAngajat" Width="150"  runat="server" Text='<%# Eval("F100942") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
				        </table>
			        </fieldset>
			        <fieldset >
				    <legend class="legend-font-size">Permis auto</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblCateg"  Width="100"  runat="server"  Text="Categorie"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsCateg" Value='<%#Eval("F10028") %>' ID="cmbCateg"   runat="server" DropDownStyle="DropDown"  TextField="F71404" ValueField="F71402" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnPermis" ClientInstanceName="btnPermis"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ OnClickDoc(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnPermisIst" ClientInstanceName="btnPermisIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricDoc(s); }" />
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
							        <dx:ASPxDateEdit  ID="deDataEmitere" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10024") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataExpirare" Width="100" runat="server"  Text="Data expirare"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataExpirare" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F1001000")%> '  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNr" Width="100" runat="server"  Text="Numar"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNr" Width="150"  runat="server" Text='<%# Eval("F1001001") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblPermisEmisDe" Width="100" runat="server"  Text="Emis de"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtPermisEmisDe" Width="150"  runat="server" Text='<%# Eval("F1001002") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>					                                                                                                                                     					       
				        </table>
                        <asp:ObjectDataSource runat="server" ID="dsCateg" TypeName="WizOne.Module.General" SelectMethod="GetCategPermis"/>
			        </fieldset>
                 </td> 

            <td  valign="top" >
			      <fieldset >
				        <legend></legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblStudii" Width="100" runat="server"  Text="Studii" ></dx:ASPxLabel >	
						        </td>							        
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsStudii" Value='<%#Eval("F10050") %>' ID="cmbStudiiDoc"   runat="server" DropDownStyle="DropDown"  TextField="F71204" ValueField="F71202" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnStudii" ClientInstanceName="btnStudii"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ OnClickDoc(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnStudiiIst" ClientInstanceName="btnStudiiIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricDoc(s); }" />
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
							        <dx:ASPxTextBox  ID="txtCalif1" Width="150"  runat="server" Text='<%# Eval("F100903") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblCalif2" Width="100" runat="server"  Text="Calificare 2" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtCalif2" Width="150"  runat="server" Text='<%# Eval("F100904") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblTitluAcademic" runat="server"  Text="Titlu academic"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox Width="150" DataSourceID="dsTitluAcad" Value='<%#Eval("F10051") %>' ID="cmbTitluAcademic"   runat="server" DropDownStyle="DropDown"  TextField="F71304" ValueField="F71302" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnTitluAcad" ClientInstanceName="btnTitluAcad"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ OnClickDoc(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnTitluAcadIst" ClientInstanceName="btnTitluAcadIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricDoc(s); }" />
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
							        <dx:ASPxComboBox Width="150" DataSourceID="dsDedSomaj" Value='<%#Eval("F10073") %>' ID="cmbDedSomaj"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDoc(s); }" />
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNrCarteMunca" runat="server"  Text="Numar carte munca"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNrCarteMunca" Width="150"  runat="server" Text='<%# Eval("F10012") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblSerieCarteMunca" runat="server"  Text="Serie carte munca"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtSerieCarteMunca" Width="150"  runat="server" Text='<%# Eval("F10013") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataCarteMunca" Width="100" runat="server"  Text="Data carte munca"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataCarteMunca" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%#Eval("FX1")%>'   AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsStudii" TypeName="WizOne.Module.General" SelectMethod="GetStudii"/>
                      <asp:ObjectDataSource runat="server" ID="dsTitluAcad" TypeName="WizOne.Module.General" SelectMethod="GetTitluAcademic"/>
                      <asp:ObjectDataSource runat="server" ID="dsDedSomaj" TypeName="WizOne.Module.General" SelectMethod="ListaMP_DeduceriSomaj"/>
			        </fieldset>
			        <fieldset >
				    <legend class="legend-font-size">Stagiu militar</legend>
				        <table width="60%">	
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLivret" runat="server"  Text="Livret militar(serie/numar)"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtLivret" Width="150"  runat="server" Text='<%# Eval("F100571") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblElibDe" runat="server"  Text="Eliberat de"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtElibDe" Width="150"  runat="server" Text='<%# Eval("F100572") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>                               			
						        <td>		
							        <dx:ASPxLabel  ID="lblDeLaData" Width="100" runat="server"  Text="De la data"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDeLaDataLivMil" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100573") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>                            
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLaData" Width="100" runat="server"  Text="La data"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deLaDataLivMil" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100574")%>'   AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>                            
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblGrad" runat="server"  Text="Grad"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtGrad" Width="150"  runat="server" Text='<%# Eval("F100575") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
                            <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblOrdin" runat="server"  Text="Ordin"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtOrdin" Width="150"  runat="server" Text='<%# Eval("F100576") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDoc(s); }" />
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
</body>