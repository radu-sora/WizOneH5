<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Structura.ascx.cs" Inherits="WizOne.Personal.Structura" %>



<script type="text/javascript">


    function OnValueChangedHandlerStruct(s) {    
        if (s.name != "cmbStru") {
            if (s.name == "deDataModifStr") {
                var DataModif = new Date(deDataModifStr.GetDate());
                pnlCtlStruct.PerformCallback(s.name + ";" + DataModif.getDate() + "/" + (DataModif.getMonth() + 1) + "/" + DataModif.getFullYear());
            }
            else
                pnlCtlStruct.PerformCallback(s.name + ";" + s.GetValue());
        }
        else {     
            var item = cmbStru.GetSelectedItem();        
            hfCC.Set('CC', item.GetColumnText("CC"));
            cmbCC.ClearItems();
            var tipCC = "<%=Session["MP_ComboCC"] %>";
            var resCC = tipCC.split(";");
            for (var i = 0; i < resCC.length; i++) {
                var linieCC = resCC[i].split(",");
                if (item.GetColumnText("CC") == "9999")
                    cmbCC.AddItem(linieCC[1], Number(linieCC[0]));
                else {                    
                    if (linieCC[0] == Number(item.GetColumnText("CC"))) 
                        cmbCC.AddItem(linieCC[1], Number(linieCC[0]));
                }
            }
            pnlCtlStruct.PerformCallback(s.name);   

        }
    }
    function OnClickStr(s) {
        pnlLoading.Show();
        pnlCtlStruct.PerformCallback(s.name);
    }

    function GoToIstoricStr(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

</script>

				
    <dx:ASPxCallbackPanel ID = "pnlCtlStruct" ClientIDMode="Static" ClientInstanceName="pnlCtlStruct" runat="server" OnCallback="pnlCtlStruct_Callback"  SettingsLoadingPanel-Enabled="false">
        <PanelCollection>
            <dx:PanelContent>

                <legend id="lgStruc" runat="server" class="legend-font-size" style="width:250px; margin:20px 0px 30px 20px;">Structura organizatorica</legend>

                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblStru" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Structura</label>
				    <dx:ASPxComboBox ID="cmbStru" runat="server" DropDownStyle="DropDown" ValueField="IdAuto" ValueType="System.Int32" TabIndex="1" AutoPostBack="false" Width="250" ItemStyle-Wrap="True" >
                        <Columns>
                            <dx:ListBoxColumn FieldName="F00204" Caption="Companie" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00202" Caption="IdCompanie" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="F00305" Caption="Subcompanie" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00304" Caption="IdSubcompanie" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="F00406" Caption="Filiala" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00405" Caption="IdFiliala" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="F00507" Caption="Sectie" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00506" Caption="IdSectie" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="F00608" Caption="Departament" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00607" Caption="IdDepartament" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="F00709" Caption="Subdepartament" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00708" Caption="IdSubdepartament" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="F00810" Caption="Birou" Width="130px" />
                            <dx:ListBoxColumn FieldName="F00809" Caption="IdBirou" Width="130px" Visible="false"/>
                            <dx:ListBoxColumn FieldName="CC" Caption="Centru cost" Width="130px"/>
                            <dx:ListBoxColumn FieldName="IdAuto" Caption="NrCrt" Width="130px" Visible="false"/>
                        </Columns>
                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				    </dx:ASPxComboBox>
                </div>
                
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblCom" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Companie</label>
                    <dx:ASPxTextBox ID="txtCom" runat="server" Width="250" ReadOnly="true" TabIndex="2" CssClass="txtEnabled" />
                </div>
						
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblSub" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Subcompanie</label>
                    <dx:ASPxTextBox ID="txtSub" runat="server" Width="250" ReadOnly="true" TabIndex="3" CssClass="txtEnabled" />
                </div>
						
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblFil" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Filiala</label>
                    <dx:ASPxTextBox ID="txtFil" runat="server" Width="250" ReadOnly="true" TabIndex="4" CssClass="txtEnabled" />
                </div>

                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblSec" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Sectie</label>
                    <dx:ASPxTextBox ID="txtSec" runat="server" Width="250" ReadOnly="true" TabIndex="5" CssClass="txtEnabled" />
                </div>

                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblDept" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Departament</label>
                    <dx:ASPxTextBox ID="txtDept" runat="server" Width="250" ReadOnly="true" TabIndex="6" CssClass="txtEnabled" />
                </div>
            			
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblSubdept" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Subdepartament</label>
                    <dx:ASPxTextBox ID="txtSubdept" runat="server" Width="250" ReadOnly="true" TabIndex="7" CssClass="txtEnabled" />
                </div>
            			
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblBir" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Birou/Echipa</label>
				    <dx:ASPxComboBox ID="cmbBir" runat="server" DropDownStyle="DropDown" TextField="F00810" TabIndex="8" ValueField="F00809" ValueType="System.Int32" Width="250">
                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				    </dx:ASPxComboBox>
                </div>

                 <div style="width:100%; margin-bottom:10px;">
                    <label id="lblDataModif" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Data modificarii</label>
                    <table>
                        <tr>
                            <td>
							    <dx:ASPxDateEdit  ID="deDataModifStr" ClientInstanceName="deDataModifStr" Width="100" TabIndex="9"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                    <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
							    </dx:ASPxDateEdit>					        
                            </td>    
                        </tr>
                    </table>
                </div>           			
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblCC" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Centru cost</label>
                    <table>
                        <tr>
                            <td>
				                <dx:ASPxComboBox ID="cmbCC" runat="server" DropDownStyle="DropDown" TabIndex="10" TextField="F06205" ValueField="F06204" ValueType="System.Int32" Width="200">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				                </dx:ASPxComboBox>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btnCC" ClientInstanceName="btnCC"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                    <ClientSideEvents Click="function(s,e){ OnClickStr(s); }" />
                                    <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                    <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                </dx:ASPxButton>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btnCCIst" ClientInstanceName="btnCCIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                    <ClientSideEvents Click="function(s,e){ GoToIstoricStr(s); }" />
                                    <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                    <Paddings PaddingLeft="10px"/>
                                </dx:ASPxButton>
                            </td>
                        </tr>
                    </table>
                </div>		
			
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblPL" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Punct de lucru</label>
                    <table>
                        <tr>
                            <td>
				                <dx:ASPxComboBox ID="cmbPL" runat="server" DropDownStyle="DropDown" TabIndex="11" TextField="F08003" ValueField="F08002" ValueType="System.Int32" Width="200">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				                </dx:ASPxComboBox>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btnPL" ClientInstanceName="btnPL" ClientIDMode="Static" Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                    <ClientSideEvents Click="function(s,e){ OnClickStr(s); }" />
                                    <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                    <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                </dx:ASPxButton>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btnPLIst" ClientInstanceName="btnPLIst" ClientIDMode="Static" Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                    <ClientSideEvents Click="function(s,e){ GoToIstoricStr(s); }" />
                                    <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                    <Paddings PaddingLeft="10px"/>
                                </dx:ASPxButton>
                            </td>
                        </tr>
                    </table>
                </div>	

                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblLocatie" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Locatie munca</label>
                    <table>
                        <tr>
                            <td>
				                <dx:ASPxComboBox ID="cmbLocatie" runat="server" DropDownStyle="DropDown" TabIndex="12" TextField="LOCATIE_MUNCA" ValueField="ID_LOCATIE" ValueType="System.Int32" Width="200">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				                </dx:ASPxComboBox>
                            </td>               
                        </tr>
                    </table>
                </div>	

                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblCAEN" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Cod CAEN</label>
                    <table>
                        <tr>
                            <td>
				                <dx:ASPxComboBox ID="cmbCAEN" runat="server" DropDownStyle="DropDown" TabIndex="13" TextField="F80104" ValueField="F80103" ValueType="System.Int32" Width="200">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				                </dx:ASPxComboBox>
                            </td>    
                        </tr>
                    </table>
                </div>
                <div style="width:100%; margin-bottom:10px;">
                    <label id="lblUnitStat" runat="server" style="display:inline-block; float:left; padding:0px 15px; width:150px;">Unitate locala statistica</label>
                    <table>
                        <tr>
                            <td>
				                <dx:ASPxComboBox ID="cmbUnitStat" runat="server" DropDownStyle="DropDown" TabIndex="14" TextField="F80304" ValueField="F80303" ValueType="System.Int32" Width="200">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerStruct(s); }" />
				                </dx:ASPxComboBox>
                            </td>    
                        </tr>
                    </table>
                </div>

            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>	

  <dx:ASPxHiddenField runat="server" ID="hfCC" ClientInstanceName="hfCC" />