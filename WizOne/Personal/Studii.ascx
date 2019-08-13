<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Studii.ascx.cs" Inherits="WizOne.Personal.Studii" %>



<script type="text/javascript">

    function GoToIstoricStudii(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }
</script>
<body>

    <table width="100%">
		<tr>
			<td align="left">					
			</td>			
	
		</tr>			
	</table>			

   <dx:ASPxCallbackPanel ID = "Studii_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlStudii" runat="server" OnCallback="pnlCtlStudii_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="Studii_DataList" runat="server">
        <ItemTemplate>
			<div>

			  <fieldset class="fieldset-auto-width">
				<legend id="lgStud" runat="server" class="legend-font-size">Studii</legend>
				<table id="lgStudTable" runat="server" width="80%">	
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblStudii" Width="100" runat="server"  Text="Studii"></dx:ASPxLabel >	
						</td>
						<td>	
						<dx:ASPxComboBox DataSourceID="dsStudii" Width="150"  Value='<%#Eval("F10050") %>' ID="cmbStudiiSt"   runat="server" DropDownStyle="DropDown"  TextField="F71204" ValueField="F71202" AutoPostBack="false"  ValueType="System.Int32" >
							</dx:ASPxComboBox>
						</td>
                        <td>
                            <dx:ASPxButton ID="btnStudii" ClientInstanceName="btnStudii"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"   RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickStudii(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnStudiiIst" ClientInstanceName="btnStudiiIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricStudii(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblInstit"  runat="server"  Text="Institutia de invatamant" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtInstit"  Width="100" runat="server" Text='<%# Bind("F1001085") %>'  AutoPostBack="false" >
							</dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblSpec" Width="100" runat="server"  Text="Specializarea" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtSpec"  Width="100" runat="server" Text='<%# Bind("F1001086") %>'  AutoPostBack="false" >
							</dx:ASPxTextBox >
						</td>
					</tr>
                    
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDataInceputSt" runat="server"  Text="Data inceput studii"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDataInceputSt" Width="75" runat="server" Value='<%# Bind("F1001087") %>' DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  > 
                                <CalendarProperties FirstDayOfWeek="Monday" /> 
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDataSfarsitSt" runat="server"  Text="Data sfarsit studii"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDataSfarsitSt" Width="75" runat="server" Value='<%# Bind("F1001088") %>'  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDataDiploma" runat="server"  Text="Data diploma"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDataDiploma" Width="75" runat="server" Value='<%# Bind("F1001089") %>'  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
							</dx:ASPxDateEdit>					
						</td>
					</tr>                                                            
                    
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblObs" Width="100" runat="server"  Text="Observatii" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtObs"  Width="100" runat="server" Text='<%# Bind("F1001090") %>' AutoPostBack="false" >
							</dx:ASPxTextBox >
						</td>
					</tr>                                                            
                    					             				
				</table>
                  <asp:ObjectDataSource runat="server" ID="dsStudii" TypeName="WizOne.Module.General" SelectMethod="GetStudii"/>
			  </fieldset>
              </div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
</body>