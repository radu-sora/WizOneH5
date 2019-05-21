<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Test2.ascx.cs" Inherits="WizOne.Personal.Test2" %>



<script type="text/javascript">

    //function OnTextChangedHandlerCtr(s) {
    //    if (s.name == "deUltimaZiLucr") {
    //        var DateTo = new Date(s.GetDate());
    //        var DateAng = new Date(deDataAng.GetDate());

    //        if (DateTo < DateAng) {
    //            swal({
    //                title: "Atentie !", text: "Data plecarii este mai mica decat data angajarii",
    //                type: "warning"
    //            });
    //        }
    //        else {
    //            DateTo.setDate(DateTo.getDate() + 1);
    //            deDataPlecarii.SetDate(DateTo);
    //        }
    //    }

    //    if (s.name == "deDataPlecarii")
    //    {
    //        var DateTo = new Date(s.GetDate());
    //        var DateMax = new Date(2100, 1, 1);
    //        var DateAng = new Date(deDataAng.GetDate());

    //        if (DateTo < DateAng) {
    //            swal({
    //                title: "Atentie !", text: "Data plecarii este mai mica decat data angajarii",
    //                type: "warning"
    //            });
    //        }
    //        else {
    //            if (DateTo.getTime() != DateMax.getTime()) {
    //                DateTo.setDate(DateTo.getDate() - 1);
    //                deUltimaZiLucr.SetDate(DateTo);
    //            }
    //        }
    //    }

    //    pnlCtlContract.PerformCallback(s.name + ";" +s.GetText());
    //}
    //function OnValueChangedHandlerCtr(s) {
    //    pnlCtlContract.PerformCallback(s.name + ";" + s.GetValue());
    //}

    //function OnClickCtr(s) {
    //    pnlLoading.Show();
    //    pnlCtlContract.PerformCallback(s.name);
    //}

    function GoToIstoricCtr(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    //function OnClickCautaCOR(s) {
    //    window.open('CautaCOR.aspx', '', 'height=400,width=600,left=' + (window.outerWidth / 2 + window.screenX - 300) + ', top=' + (window.outerHeight / 2 + window.screenY - 200));
    //}


    //window.CompleteazaCOR = function () {
    //    pnlCtlContract.PerformCallback("btnCautaCOR");
    //}

    function OnEndCallbackCtr(s, e) {
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
				


   <dx:ASPxCallbackPanel ID = "Test2_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlContract" runat="server" OnCallback="pnlCtlContract_Callback" SettingsLoadingPanel-Enabled="false">
       <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackCtr(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList  ID="Test2_DataList" runat="server">  
        <ItemTemplate>
			<div>
            <tr>
             <td>
			  <fieldset class="fieldset-auto-width">
				<legend class="legend-font-size">Contract</legend>
				<table width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrCtrInt" Width="100" runat="server"  Text="Nr. ctr. intern" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrCtrInt"  Width="100" runat="server" Text='<%# Eval("F100985") %>'   >
                                
							</dx:ASPxTextBox >
						</td>
                        <td >
                            <dx:ASPxButton ID="btnCtrInt" ClientInstanceName="btnCtrInt" ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)"  >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"  PaddingRight="10px"/>
                            </dx:ASPxButton>
						</td>	
						<td >
                            <dx:ASPxButton ID="btnCtrIntIst" ClientInstanceName="btnCtrIntIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px" />
                            </dx:ASPxButton>
                        </td>
					</tr>	
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDataCtrInt" runat="server"  Text="Data ctr. intern"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDataCtrInt" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100986") %>'    >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                               
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>
						<td>		
							<dx:ASPxLabel  ID="lblDataAng" runat="server"  Text="Data angajarii"></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxDateEdit  ID="deDataAng" ClientIDMode="Static" ClientInstanceName="deDataAng" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10022") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
                        <td>
                            <dx:ASPxButton ID="btnDataAng" ClientInstanceName="btnDataAng"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                            
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnDataAngIst" ClientInstanceName="btnDataAngIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblTermenRevisal" runat="server"  Text="Termen depunere Revisal"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deTermenRevisal" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" ReadOnly="true"  Enabled="false"    >
                                <CalendarProperties FirstDayOfWeek="Monday" />                               
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblTipCtrMunca" runat="server"  Text="Tip contract munca"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxComboBox DataSourceID="dsTCM"  Value='<%#Eval("F100984") %>'   ID="cmbTipCtrMunca" Width="100"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32" >
                                
							</dx:ASPxComboBox>
						</td>
					</tr>	
                    
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblDurCtr" runat="server"  Text="Durata contract" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsDC"  Value='<%#Eval("F1009741") %>'  ID="cmbDurCtr"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F08903" ValueField="F08902" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDeLaData" runat="server"  Text="De la data"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDeLaData" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100933") %>'    >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblLaData" runat="server"  Text="La data"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deLaData" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100934") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrLuni" Width="100" runat="server"  Text="Nr. luni" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrLuni"  Width="100" runat="server" ReadOnly="true"  ></dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrZile" Width="100" runat="server"  Text="Nr. zile" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrZile"  Width="100" runat="server" ReadOnly="true"   ></dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblPrel" runat="server"  Text="Prelungire contract" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsPC"  Value='<%#Eval("F100938") %>' ID="cmbPrel"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32" >
                                
							</dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblExcIncet" runat="server"  Text="Exceptie incetare" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsEI"  Value='<%#Eval("F100929") %>'  ID="cmbExcIncet"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F09403" ValueField="F09402" ValueType="System.Int32">
                               
							</dx:ASPxComboBox>
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCASSAngajat" runat="server"  Text="CASS angajat" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCASS"  Value='<%#Eval("F1003900") %>' ID="cmbCASSAngajat"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F06303" ValueField="F06302" ValueType="System.Int32" >
                            
							</dx:ASPxComboBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnCASS" ClientInstanceName="btnCASS"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCASSIst" ClientInstanceName="btnCASSIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCASSAngajator" runat="server"  Text="CASS Angajator" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCASS"  Value='<%#Eval("F1003907") %>' ID="cmbCASSAngajator"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F06303" ValueField="F06302" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblSalariu" Width="100" runat="server"  Text="Salariu" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtSalariu"  Width="100" runat="server"  Text='<%# Eval("F100699") %>'  DisplayFormatString="N0" oncontextMenu="ctx(this,event)"  >
                                
							</dx:ASPxTextBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnSalariu" ClientInstanceName="btnSalariu"  ClientIDMode="Static"  Width="5" Height="5" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" runat="server" oncontextMenu="ctx(this,event)"  >
                              
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnSalariuIst" ClientInstanceName="btnSalariuIst"  ClientIDMode="Static"  Width="5" Height="5" Font-Size="1px"  RenderMode="Link" ToolTip="Istoric modificari" runat="server" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifSa" runat="server"  Text="Data modificare salariu"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifSal" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Bind("F100991") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCategAng1" runat="server"  Text="Categorie angajat 1" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCategAng_61"  Value='<%#Eval("F10061") %>' ID="cmbCategAng1"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F72404" ValueField="F72402" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCategAng2" runat="server"  Text="Categorie angajat 2" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCategAng_62"  Value='<%#Eval("F10062") %>' ID="cmbCategAng2"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F72404" ValueField="F72402" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblLocAnt" Width="100" runat="server"  Text="Loc anterior" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtLocAnt"  Width="100" runat="server" Text='<%# Eval("F10078") %>'   >
                                
							</dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblLocatieInt" Width="100" runat="server"  Text="Locatie interna" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsLocatieInt"  Value='<%#Eval("F100966") %>' ID="cmbLocatieInt"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="LOCATIE" ValueField="NUMAR" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					             				
				</table>
                <asp:ObjectDataSource runat="server" ID="dsTCM" TypeName="WizOne.Module.General" SelectMethod="GetTipContract" />
                <asp:ObjectDataSource runat="server" ID="dsDC"  TypeName="WizOne.Module.General" SelectMethod="GetDurataContract" />
                <asp:ObjectDataSource runat="server" ID="dsPC"  TypeName="WizOne.Module.General" SelectMethod="GetPrelungireContract" />
                <asp:ObjectDataSource runat="server" ID="dsEI"  TypeName="WizOne.Module.General" SelectMethod="GetExceptieIncetare" />
                <asp:ObjectDataSource runat="server" ID="dsCASS"  TypeName="WizOne.Module.General" SelectMethod="GetCASS" />
                <asp:ObjectDataSource runat="server" ID="dsCategAng_61"  TypeName="WizOne.Module.General" SelectMethod="GetCategAng_61" />
                <asp:ObjectDataSource runat="server" ID="dsCategAng_62"  TypeName="WizOne.Module.General" SelectMethod="GetCategAng_62" />
                <asp:ObjectDataSource runat="server" ID="dsLocatieInt"  TypeName="WizOne.Module.General" SelectMethod="GetLocatieInt" />
			  </fieldset>
			  <fieldset class="fieldset-auto-width">
				<legend class="legend-font-size">Tip munca</legend>
				<table width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblTipAng" Width="100" runat="server"  Text="Tip angajat" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsTA"  Value='<%#Eval("F10010") %>' ID="cmbTipAng" Width="130"   runat="server" DropDownStyle="DropDown"  TextField="F71604" ValueField="F71602"  ValueType="System.Int32">
                               
							</dx:ASPxComboBox >
						</td>
					</tr>
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblTimpPartial"  Width="100" runat="server"  Text="Timp partial" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsTP"  Value='<%#Eval("F10043") %>' ID="cmbTimpPartial" Width="100" runat="server"  TextField="Denumire" ValueField="Id"    ValueType="System.Int32" >
                                
							</dx:ASPxComboBox>
						</td>
                    </tr>
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblNorma"  Width="100" runat="server"  Text="Norma" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsN"  Value='<%#Eval("F100973") %>' ID="cmbNorma" Width="100" runat="server"   TextField="Denumire" ValueField="Id"   ValueType="System.Int32" >
                                
							</dx:ASPxComboBox>
						</td>
						<td>	
                            <dx:ASPxButton ID="btnNorma" ClientInstanceName="btnNorma" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>									
						</td>
                        <td>
                            <dx:ASPxButton ID="btnNormaIst" ClientInstanceName="btnNormaIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                    </tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifNorma" runat="server"  Text="Data modificare norma"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifNorma" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Bind("F100955") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblTipNorma" runat="server"  Text="Tip norma" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsTN"  Value='<%#Eval("F100926") %>' ID="cmbTipNorma"  Width="130" runat="server" DropDownStyle="DropDown"  TextField="F09203" ValueField="F09202" ValueType="System.Int32" >
                                
							</dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblDurTimpMunca" runat="server"  Text="Durata timp munca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsDTM"  Value='<%#Eval("F100927") %>'  ID="cmbDurTimpMunca" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F09103" ValueField="F09102" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>   
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblRepTimpMunca" runat="server"  Text="Repartizare timp munca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsRTM"  Value='<%#Eval("F100928") %>'  ID="cmbRepTimpMunca" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F09303" ValueField="F09302" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>    
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblIntervRepTimpMunca" runat="server"  Text="Interval repartizare timp munca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsIRTM"  Value='<%#Eval("F100939") %>' ID="cmbIntervRepTimpMunca" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F09603" ValueField="F09602"  ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>     
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrOre" Width="100" runat="server"  Text="Nr ore pe luna/saptamana" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrOre"  Width="75" runat="server" Text='<%# Bind("F100964") %>'   >
                               
							</dx:ASPxTextBox >
						</td>
					</tr>  
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCOR" runat="server"  Text="COR" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCOR"  ID="cmbCOR"   Enabled="false" Width="130"  runat="server" DropDownStyle="DropDown" DropDownWidth ="700"  TextField="F72204" ValueField="F72202" ValueType="System.Int32" >
                                <Columns>
                                    <dx:ListBoxColumn FieldName="F72202" Caption="Cod COR" Width="100px" />
                                    <dx:ListBoxColumn FieldName="F72204" Caption="Descriere" Width="600px" />
                                </Columns>
							</dx:ASPxComboBox>	
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCautaCOR" ClientInstanceName="btnCautaCOR" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Cauta cod COR" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/lupa.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>				
						</td>
						<td>	
                            <dx:ASPxButton ID="btnCOR" ClientInstanceName="btnCOR" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>									
						</td>
                        <td>
                            <dx:ASPxButton ID="btnCORIst" ClientInstanceName="btnCORIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr> 
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifCOR" runat="server"  Text="Data modificare COR"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifCOR" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100956") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>  
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblFunctie" runat="server"  Text="Functie" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsFunctie"  Value='<%#Eval("F10071") %>' ID="cmbFunctie" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F71804" ValueField="F71802" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnFunc" ClientInstanceName="btnFunc"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnFuncIst" ClientInstanceName="btnFuncIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                               
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>  
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifFunctie" runat="server"  Text="Data modificare functie"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifFunctie" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100992") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr> 
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblMeserie" runat="server"  Text="Meserie" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsMeserie"  Value='<%#Eval("F10029") %>' ID="cmbMeserie" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F71704" ValueField="F71702" ValueType="System.Int32">
                              
							</dx:ASPxComboBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnMeserie" ClientInstanceName="btnMeserie"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)"  >
                               
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnMeserieIst" ClientInstanceName="btnMeserieIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>  
                    <tr>
                        <td>
                            <dx:ASPxCheckBox ID="chkFunctieBaza"  runat="server" Width="150" Text="Functie de baza" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"F10048").ToString()=="1"%>' ClientInstanceName="chkbx4" >
                                
                            </dx:ASPxCheckBox>
                        </td>

				    </tr>                                                                                                                                                                                       				
				</table>
                <asp:ObjectDataSource runat="server" ID="dsTA" TypeName="WizOne.Module.General" SelectMethod="GetTipAngajat" />
                <asp:ObjectDataSource runat="server" ID="dsTP" TypeName="WizOne.Module.General" SelectMethod="GetTimpPartial"/>
                <asp:ObjectDataSource runat="server" ID="dsN" TypeName="WizOne.Module.General" SelectMethod="GetNorma"/>
                <asp:ObjectDataSource runat="server" ID="dsTN" TypeName="WizOne.Module.General" SelectMethod="GetTipNorma"/>
                <asp:ObjectDataSource runat="server" ID="dsDTM" TypeName="WizOne.Module.General" SelectMethod="GetDurataTimpMunca"/>
                <asp:ObjectDataSource runat="server" ID="dsRTM" TypeName="WizOne.Module.General" SelectMethod="GetRepartizareTimpMunca"/>
                <asp:ObjectDataSource runat="server" ID="dsIRTM" TypeName="WizOne.Module.General" SelectMethod="GetIntervalRepartizareTimpMunca"/>
                <asp:ObjectDataSource runat="server" ID="dsCOR" TypeName="WizOne.Module.General" SelectMethod="GetCOR"/>
                <asp:ObjectDataSource runat="server" ID="dsFunctie" TypeName="WizOne.Module.General" SelectMethod="GetFunctie"/>
                <asp:ObjectDataSource runat="server" ID="dsMeserie" TypeName="WizOne.Module.General" SelectMethod="GetMeserie"/>
			  </fieldset>
            </td>
           <td valign="top" width="310">      
			      <fieldset >
				    <legend class="legend-font-size">Perioada</legend>
				    <table width="60%">	
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblPerioadaProba" width="125" runat="server"  Text="Perioada de proba" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
                                <dx:ASPxLabel  ID="lblZL" runat="server"  Text="zile lucratoare" ></dx:ASPxLabel >
                            </td>
                            <td align="right">
							    <dx:ASPxTextBox  ID="txtPerProbaZL" Width="20"  runat="server" Text='<%# Eval("F1001063") %>'  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblTest1" runat="server"  Text=" " ></dx:ASPxLabel >	
						    </td>
						    <td align="right>
                                <dx:ASPxLabel  ID="lblZC" runat="server"  Text="zile calendaristice" ></dx:ASPxLabel >
                            </td>
                            <td align="right">
							    <dx:ASPxTextBox  ID="txtPerProbaZC" Width="20"  runat="server" Text='<%# Eval("F100975") %>'  >
                                   
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblNrZilePreavizDemisie" runat="server"  Text="Nr zile preaviz demisie" ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtNrZilePreavizDemisie" Width="75"  runat="server" Text='<%# Eval("F1009742") %>'  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblNrZilePreavizConc" runat="server"  Text="Nr zile preaviz concediere" ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtNrZilePreavizConc" Width="75"  runat="server" Text='<%# Eval("F100931") %>'  >
                                  
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
				    </table>
			        </fieldset>
			        <fieldset >
				    <legend class="legend-font-size">Data incetare</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblUltimaZiLucr" runat="server"  Text="Ultima zi lucrata"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deUltimaZiLucr" ClientIDMode="Static" ClientInstanceName="deUltimaZiLucr" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10023") %>'   >
                                        <CalendarProperties FirstDayOfWeek="Monday" /> 
                                       
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblMotivPlecare" Width="100" runat="server"  Text="Motiv plecare" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsMP"  Value='<%#Eval("F10025") %>' ID="cmbMotivPlecare"  Width="100"  runat="server" DropDownStyle="DropDown"  TextField="F72104" ValueField="F72102"   ValueType="System.Int32" >
                                         
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnMotivPl" ClientInstanceName="btnMotivPl"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                                        
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnMotivPlIst" ClientInstanceName="btnMotivPlIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                        
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td>
							        <dx:ASPxLabel  ID="lblDataPlecarii" runat="server"  Text="Data plecarii"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataPlecarii" ClientIDMode="Static" ClientInstanceName="deDataPlecarii" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100993") %>'   >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                       
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataReintegr" runat="server"  Text="Data reintegrare"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataReintegr" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100930") %>'   >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                       
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblGradInvalid" Width="100" runat="server"  Text="Grad invaliditate" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsGI"  Value='<%#Eval("F10027") %>' ID="cmbGradInvalid" Width="100"  runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id"  ValueType="System.Int32" >
                                       
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataValabInvalid" runat="server"  Text="Data valabilitate invaliditate"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataValabInvalid" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100271") %>'   >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chkScutitImp" runat="server" Width="150" Text="Scutit impozit" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"F10026").ToString()=="1"%>' ClientInstanceName="chkbx1">
                                    
                                </dx:ASPxCheckBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chkBifaPensionar" runat="server" Width="150" Text="Bifa pensionar" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"F10037").ToString()=="1"%>' ClientInstanceName="chkbx2" >
                                    
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chkBifaDetasat"  runat="server" Width="150" Text="Bifa detasat de la alt angajator" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"F100954").ToString()=="1"%>' ClientInstanceName="chkbx3" >
                                   
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
				        </table>
                        <asp:ObjectDataSource runat="server" ID="dsMP" TypeName="WizOne.Module.General" SelectMethod="GetMotivPlecare"/>
                        <asp:ObjectDataSource runat="server" ID="dsGI" TypeName="WizOne.Module.General" SelectMethod="ListaMP_GradInvaliditate"/>
			        </fieldset>           
             </td>
              <td valign="top">   
			      <fieldset class="fieldset-auto-width">
				    <legend class="legend-font-size">Situatie CO</legend>
				    <table width="60%">	
				       <tr>				
						   <td >
							    <dx:ASPxLabel  ID="lblVechimeComp" width="150" runat="server"  Text="Vechime in companie" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
                                <dx:ASPxLabel  ID="lblVechCompAni"  runat="server"  Text="ani" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCompAni" Width="25"  runat="server"  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblTest2" width="150" runat="server"  Text=" " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
                                <dx:ASPxLabel  ID="lblVechCompLuni" runat="server"  Text="luni" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCompLuni" Width="25"  runat="server"  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblVechimeCarteMunca" width="150" runat="server"  Text="Vechime pe cartea de munca" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
                                <dx:ASPxLabel  ID="lblVechCarteMuncaAni" runat="server"  Text="ani" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCarteMuncaAni" Width="25"  runat="server"  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblTest3" width="150"  runat="server"  Text=" " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
                                 <dx:ASPxLabel  ID="lblVechCarteMuncaLuni" runat="server"  Text="luni" ></dx:ASPxLabel >                                
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCarteMuncaLuni" Width="25"  runat="server"  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblGrila" runat="server"  Text="Grila" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtGrila" Width="75"  runat="server" Text='<%# Eval("F10072") %>'  >
                                   
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOFidel" runat="server"  Text="Zile CO fidelitate" ></dx:ASPxLabel >	
						    </td>
                            <td></td>	                            	
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOFidel" Width="75"  runat="server" Text='<%# Eval("F100640") %>'  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOAnAnt" runat="server"  Text="Zile CO an anterior" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOAnAnt" Width="75"  runat="server" Text='<%# Eval("F100641") %>'  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOCuvAnCrt" runat="server"  Text="Zile CO cuvenite an curent" ></dx:ASPxLabel >	
						    </td>	
                            <td></td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOCuvAnCrt" Width="75"  runat="server" Text='<%# Eval("F100642") %>'  >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZLP" runat="server"  Text="Zile libere platite" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtZLP" Width="75"  runat="server"  >
                                   
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZLPCuv" runat="server"  Text="Zile libere platite cuvenite" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtZLPCuv" Width="75"  runat="server"   >
                                   
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td>		
							    <dx:ASPxLabel  ID="lblDataPrimeiAng" runat="server"  Text="Data primei angajari"></dx:ASPxLabel >	
						    </td>
                            <td></td>	
						    <td>	
							    <dx:ASPxDateEdit  ID="deDataPrimeiAng" Width="85" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F1001049") %>'   >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                   
							    </dx:ASPxDateEdit>										
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