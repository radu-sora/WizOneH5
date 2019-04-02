<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractZilnic.ascx.cs" Inherits="WizOne.ContracteLucru.ContractZilnic" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<script type="text/javascript">

    function OnValueChangedHandlerCtrZilnic0(s) {
        pnlCtlCtrZilnic0.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb0")
        {
            if (s.GetValue() == 1) {
                cmbProg0.SetVisible(true);
                grDate0.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg0.SetVisible(false);
                grDate0.SetVisible(true);
            }

        }
    }
      
    function OnValueChangedHandlerCtrZilnic1(s) {
        pnlCtlCtrZilnic1.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb1") {
            if (s.GetValue() == 1) {
                cmbProg1.SetVisible(true);
                grDate1.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg1.SetVisible(false);
                grDate1.SetVisible(true);
            }

        }
    }

    function OnValueChangedHandlerCtrZilnic2(s) {
        pnlCtlCtrZilnic2.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb2") {
            if (s.GetValue() == 1) {
                cmbProg2.SetVisible(true);
                grDate2.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg2.SetVisible(false);
                grDate2.SetVisible(true);
            }

        }
    }

    function OnValueChangedHandlerCtrZilnic3(s) {
        pnlCtlCtrZilnic3.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb3") {
            if (s.GetValue() == 1) {
                cmbProg3.SetVisible(true);
                grDate3.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg3.SetVisible(false);
                grDate3.SetVisible(true);
            }

        }
    }

    function OnValueChangedHandlerCtrZilnic4(s) {
        pnlCtlCtrZilnic4.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb4") {
            if (s.GetValue() == 1) {
                cmbProg4.SetVisible(true);
                grDate4.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg4.SetVisible(false);
                grDate4.SetVisible(true);
            }

        }
    }

    function OnValueChangedHandlerCtrZilnic5(s) {
        pnlCtlCtrZilnic5.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb5") {
            if (s.GetValue() == 1) {
                cmbProg5.SetVisible(true);
                grDate5.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg5.SetVisible(false);
                grDate5.SetVisible(true);
            }

        }
    }
 
    function OnValueChangedHandlerCtrZilnic6(s) {
        pnlCtlCtrZilnic6.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb6") {
            if (s.GetValue() == 1) {
                cmbProg6.SetVisible(true);
                grDate6.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg6.SetVisible(false);
                grDate6.SetVisible(true);
            }

        }
    }
 
    function OnValueChangedHandlerCtrZilnic7(s) {
        pnlCtlCtrZilnic7.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb7") {
            if (s.GetValue() == 1) {
                cmbProg7.SetVisible(true);
                grDate7.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg7.SetVisible(false);
                grDate7.SetVisible(true);
            }

        }
    }

    function OnValueChangedHandlerCtrZilnic8(s) {
        pnlCtlCtrZilnic8.PerformCallback(s.name + ";" + s.GetValue());
        if (s.name == "cmbSchimb8") {
            if (s.GetValue() == 1) {
                cmbProg8.SetVisible(true);
                grDate8.SetVisible(false);
            }
            if (s.GetValue() == 2) {
                cmbProg8.SetVisible(false);
                grDate8.SetVisible(true);
            }

        }
    }

    function Init0() {
        if (cmbSchimb0.GetValue() == 1 || cmbSchimb0.GetValue() == null) {
            cmbProg0.SetVisible(true);
            grDate0.SetVisible(false);
        }
        if (cmbSchimb0.GetValue() == 2) {
            cmbProg0.SetVisible(false);
            grDate0.SetVisible(true);
        }
    }

    function Init1() {

        if (cmbSchimb1.GetValue() == 1 || cmbSchimb1.GetValue() == null) {
            cmbProg1.SetVisible(true);
            grDate1.SetVisible(false);
        }
        if (cmbSchimb1.GetValue() == 2) {
            cmbProg1.SetVisible(false);
            grDate1.SetVisible(true);
        }
    }

    function Init2() {
        if (cmbSchimb2.GetValue() == 1 || cmbSchimb2.GetValue() == null) {
            cmbProg2.SetVisible(true);
            grDate2.SetVisible(false);
        }
        if (cmbSchimb2.GetValue() == 2) {
            cmbProg2.SetVisible(false);
            grDate2.SetVisible(true);
        }
    }

    function Init3() {
        if (cmbSchimb3.GetValue() == 1 || cmbSchimb3.GetValue() == null) {
            cmbProg3.SetVisible(true);
            grDate3.SetVisible(false);
        }
        if (cmbSchimb3.GetValue() == 2) {
            cmbProg3.SetVisible(false);
            grDate3.SetVisible(true);
        }
    }

    function Init4() {
        if (cmbSchimb4.GetValue() == 1 || cmbSchimb4.GetValue() == null) {
            cmbProg4.SetVisible(true);
            grDate4.SetVisible(false);
        }
        if (cmbSchimb4.GetValue() == 2) {
            cmbProg4.SetVisible(false);
            grDate4.SetVisible(true);
        }
    }

    function Init5() {
        if (cmbSchimb5.GetValue() == 1 || cmbSchimb5.GetValue() == null) {
            cmbProg5.SetVisible(true);
            grDate5.SetVisible(false);
        }
        if (cmbSchimb5.GetValue() == 2) {
            cmbProg5.SetVisible(false);
            grDate5.SetVisible(true);
        }
    }

    function Init6() {
        if (cmbSchimb6.GetValue() == 1 || cmbSchimb6.GetValue() == null) {
            cmbProg6.SetVisible(true);
            grDate6.SetVisible(false);
        }
        if (cmbSchimb6.GetValue() == 2) {
            cmbProg6.SetVisible(false);
            grDate6.SetVisible(true);
        }
    }

    function Init7() {
        if (cmbSchimb7.GetValue() == 1 || cmbSchimb7.GetValue() == null) {
            cmbProg7.SetVisible(true);
            grDate7.SetVisible(false);
        }
        if (cmbSchimb7.GetValue() == 2) {
            cmbProg7.SetVisible(false);
            grDate7.SetVisible(true);
        }
    }

    function Init8() {
        if (cmbSchimb8.GetValue() == 1 || cmbSchimb8.GetValue() == null) {
            cmbProg8.SetVisible(true);
            grDate8.SetVisible(false);
        }
        if (cmbSchimb8.GetValue() == 2) {
            cmbProg8.SetVisible(false);
            grDate8.SetVisible(true);
        }
    }


</script>
<body>

	<table width="100%">
		<tr>
		<th valign="top" class="legend-font-size">Schimburi</th>
		</tr>			
	</table>
				
  <table>
   <tr>   
     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic0" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic0" runat="server" OnCallback="pnlCtlCtrZilnic0_Callback" SettingsLoadingPanel-Enabled="false">
         <ClientSideEvents Init="function(s,e){ Init0(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList0" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Standard</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch0"  Value='<%#Eval("TipSchimb0") %>' ID="cmbSchimb0"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic0(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg0"  Value='<%#Eval("Program0") %>' ID="cmbProg0"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic0(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch0" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg0" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate0" runat="server" ClientInstanceName="grDate0" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDate0_DataBinding" OnInitNewRow="grDate0_InitNewRow"
                OnRowInserting="grDate0_RowInserting" OnRowUpdating="grDate0_RowUpdating" OnRowDeleting="grDate0_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate0_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
    <tr>   
     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic1" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic1" runat="server" OnCallback="pnlCtlCtrZilnic1_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init1(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Luni</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch1"  Value='<%#Eval("TipSchimb1") %>' ID="cmbSchimb1"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic1(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg1"  Value='<%#Eval("Program1") %>' ID="cmbProg1"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic1(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch1" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg1" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate1" runat="server" ClientInstanceName="grDate1" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDate1_DataBinding" OnInitNewRow="grDate1_InitNewRow"
                OnRowInserting="grDate1_RowInserting" OnRowUpdating="grDate1_RowUpdating" OnRowDeleting="grDate1_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate1_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" ReadOnly="true" /> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
    <tr>   
     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic2" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic2" runat="server" OnCallback="pnlCtlCtrZilnic2_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init2(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList2" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Marti</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch2"  Value='<%#Eval("TipSchimb2") %>' ID="cmbSchimb2"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic2(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg2"  Value='<%#Eval("Program2") %>' ID="cmbProg2"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic2(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch2" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg2" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate2" runat="server" ClientInstanceName="grDate2" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false" OnDataBinding="grDate2_DataBinding" OnInitNewRow="grDate2_InitNewRow"
                OnRowInserting="grDate2_RowInserting" OnRowUpdating="grDate2_RowUpdating" OnRowDeleting="grDate2_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate2_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                 <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn> 
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
    <tr>   
     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic3" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic3" runat="server" OnCallback="pnlCtlCtrZilnic3_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init3(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList3" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Miercuri</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch3"  Value='<%#Eval("TipSchimb3") %>' ID="cmbSchimb3"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic3(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg3"  Value='<%#Eval("Program3") %>' ID="cmbProg3"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic3(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch3" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg3" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate3" runat="server" ClientInstanceName="grDate3" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"   OnDataBinding="grDate3_DataBinding" OnInitNewRow="grDate3_InitNewRow"
                OnRowInserting="grDate3_RowInserting" OnRowUpdating="grDate3_RowUpdating" OnRowDeleting="grDate3_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate3_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn> 
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
   <tr>   

     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic4" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic4" runat="server" OnCallback="pnlCtlCtrZilnic4_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init4(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList4" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Joi</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch4"  Value='<%#Eval("TipSchimb4") %>' ID="cmbSchimb4"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic4(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg4"  Value='<%#Eval("Program4") %>' ID="cmbProg4"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic4(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch4" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg4" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate4" runat="server" ClientInstanceName="grDate4" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"   OnDataBinding="grDate4_DataBinding" OnInitNewRow="grDate4_InitNewRow"
                OnRowInserting="grDate4_RowInserting" OnRowUpdating="grDate4_RowUpdating" OnRowDeleting="grDate4_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate4_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn> 
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
   <tr>   

     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic5" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic5" runat="server" OnCallback="pnlCtlCtrZilnic5_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init5(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList5" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Vineri</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch5"  Value='<%#Eval("TipSchimb5") %>' ID="cmbSchimb5"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic5(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg5"  Value='<%#Eval("Program5") %>' ID="cmbProg5"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic5(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch5" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg5" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate5" runat="server" ClientInstanceName="grDate5" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"   OnDataBinding="grDate5_DataBinding" OnInitNewRow="grDate5_InitNewRow"
                OnRowInserting="grDate5_RowInserting" OnRowUpdating="grDate5_RowUpdating" OnRowDeleting="grDate5_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate5_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
     <tr>   
     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic6" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic6" runat="server" OnCallback="pnlCtlCtrZilnic6_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init6(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList6" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Sambata</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch6"  Value='<%#Eval("TipSchimb6") %>' ID="cmbSchimb6"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic6(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg6"  Value='<%#Eval("Program6") %>' ID="cmbProg6"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic6(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch6" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg6" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate6" runat="server" ClientInstanceName="grDate6" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"   OnDataBinding="grDate6_DataBinding" OnInitNewRow="grDate6_InitNewRow"
                OnRowInserting="grDate6_RowInserting" OnRowUpdating="grDate6_RowUpdating" OnRowDeleting="grDate6_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate6_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
    <tr>   
     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic7" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic7" runat="server" OnCallback="pnlCtlCtrZilnic7_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init7(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList7" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Duminica</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch7"  Value='<%#Eval("TipSchimb7") %>' ID="cmbSchimb7"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic7(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg7"  Value='<%#Eval("Program7") %>' ID="cmbProg7"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic7(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch7" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg7" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td> 
        <dx:ASPxGridView ID="grDate7" runat="server" ClientInstanceName="grDate7" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"   OnDataBinding="grDate7_DataBinding"  OnInitNewRow="grDate7_InitNewRow"
                OnRowInserting="grDate7_RowInserting" OnRowUpdating="grDate7_RowUpdating" OnRowDeleting="grDate7_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate7_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" ReadOnly="true" /> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
    <tr>   

     <td valign="top">
     <dx:ASPxCallbackPanel ID = "pnlCtlCtrZilnic8" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrZilnic8" runat="server" OnCallback="pnlCtlCtrZilnic8_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents Init="function(s,e){ Init8(); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DataList8" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">S. Legala</legend>
				        <table width="25%">	
					        <tr>				
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsSch8"  Value='<%#Eval("TipSchimb8") %>' ID="cmbSchimb8"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic8(s); }" />
							        </dx:ASPxComboBox >
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsProg8"  Value='<%#Eval("Program8") %>' ID="cmbProg8"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrZilnic8(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table> 
                      <asp:ObjectDataSource runat="server" ID="dsSch8" TypeName="WizOne.Module.General" SelectMethod="ListaTipSchimburi"/>
                      <asp:ObjectDataSource runat="server" ID="dsProg8" TypeName="WizOne.Module.General" SelectMethod="GetPrograme"/>        
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
     </td>
     <td>
        <dx:ASPxGridView ID="grDate8" runat="server" ClientInstanceName="grDate8" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"   OnDataBinding="grDate8_DataBinding" OnInitNewRow="grDate8_InitNewRow"
                OnRowInserting="grDate8_RowInserting" OnRowUpdating="grDate8_RowUpdating" OnRowDeleting="grDate8_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDate8_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="100px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb"  Width="100px" Visible="false"/>    
                <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume"  Width="200px" />                
                <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="140px">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="Denumire" />
                            <dx:ListBoxColumn FieldName="OraIntrare1" Caption="Ora inceput" />
                            <dx:ListBoxColumn FieldName="OraIesire1" Caption="Ora Sfarsit" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn> 
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sf"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"  ReadOnly="true"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sf &gt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sf &lt;"   Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm"/> 
                <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="100px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  

                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
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
</body>