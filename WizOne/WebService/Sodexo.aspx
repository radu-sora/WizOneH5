<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Sodexo.aspx.cs" Inherits="WizOne.WebService.Sodexo" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">



</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function OnValueChangedHandler(s) {            
            pnlCtl.PerformCallback(s.name + ";" + s.GetValue());
            if (s.name == "cmbCompanie") {
                grDateAdrese.PerformCallback(s.GetValue());
                grDate.PerformCallback(s.GetValue());
            }
        }

        function OnClick(s) {
            pnlCtl.PerformCallback(s.name);
        }

        function OnClickDetalii() {
            c1 = -1; c2 = -1; c3 = -1;
            if (cmbCriteriu1.lastSuccessValue != null)
                c1 = cmbCriteriu1.lastSuccessValue;
            if (cmbCriteriu2.lastSuccessValue != null)
                c2 = cmbCriteriu2.lastSuccessValue;
            if (cmbCriteriu3.lastSuccessValue != null)
                c3 = cmbCriteriu3.lastSuccessValue;
            strUrl = getAbsoluteUrl + "WebService/Detalii.aspx?comp=" + cmbCompanie.inputElement.value + "&c1=" + c1 + "&c2=" + c2 + "&c3=" + c3;
            popGenDetalii.SetHeaderText("Detalii ordin");
            popGenDetalii.SetContentUrl(strUrl);
            popGenDetalii.Show();
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
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
                        <dx:ASPxButton ID="btnDetalii" ClientInstanceName="btnDetalii" ClientIDMode="Static" runat="server" Text="Detalii ordin" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/info.png"></Image>
                            <ClientSideEvents Click="function(s,e){ OnClickDetalii(); }" /> 
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Trimite ordin" AutoPostBack="true" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
           <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" />
          <PanelCollection>
            <dx:PanelContent>

			<div>
              <tr>
              <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Date conectare</legend>
				        <table width="40%">	
					        <tr>	
						        <td >
							        <dx:ASPxLabel  ID="lblUtilizator"  runat="server"  Text="Utilizator: " ></dx:ASPxLabel >					
							        <dx:ASPxTextBox  ID="txtUtilizator" Width="150"  runat="server"  AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>  
						        <td >
							        <dx:ASPxLabel  ID="lblParola"  runat="server"  Text="Parola: " ></dx:ASPxLabel >					
							        <dx:ASPxTextBox  ID="txtParola" Width="150"  runat="server"  AutoPostBack="false" Password="true"  >
							        </dx:ASPxTextBox>
						        </td>
						        <td >
							        <dx:ASPxLabel  ID="lblEMail"  runat="server"  Text="E-mail: " ></dx:ASPxLabel >					
							        <dx:ASPxTextBox  ID="txtEMail" Width="150"  runat="server"  AutoPostBack="false"   >
							        </dx:ASPxTextBox>
						        </td>
                                <td align="left" valign="bottom">
                                    <dx:ASPxButton ID="btnConectare" ClientInstanceName="btnConectare" ClientIDMode="Static" runat="server" Text="Conectare" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)">
                                        <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                                        <Paddings Padding="0px" />
                                    </dx:ASPxButton>
                                </td>
                            </tr> 
				        </table>
			        </fieldset>
               <fieldset  >
                <legend class="legend-font-size"></legend>
                <table width="40%" >
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblCompanie"  runat="server"  Text="Compania: " ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="150" ID="cmbCompanie"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							</dx:ASPxComboBox>
						</td>
                    </tr>    
                </table>
              </fieldset >

               <fieldset  >
                <legend class="legend-font-size"></legend>
                <table width="40%" >
                    <tr>
                        <td>
                            <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="50%" AutoGenerateColumns="false" OnDataBinding="grDate_DataBinding" OnInitNewRow="grDate_InitNewRow" OnCustomCallback="grDate_CustomCallback"
                                OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting">
                                <SettingsBehavior AllowFocusedRow="true" />
                                <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />                                   
                                <SettingsEditing Mode="Inline" />                                      
                                <Columns>
                                    <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />                                    
                                    <dx:GridViewDataComboBoxColumn FieldName="Tichete" Name="Tichete" Caption="Tichete" Width="150px" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.String" DropDownStyle="DropDown" >
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>                                   
                                    <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                                    <dx:GridViewDataTextColumn FieldName="Companie" Name="Companie" Caption="Companie"  Width="75px" Visible="false"/>
                                    <dx:GridViewDataTextColumn FieldName="Adresa" Name="Adresa" Caption="Adresa"  Width="75px" Visible="false"/>
                                    <dx:GridViewDataTextColumn FieldName="NumeTichet" Name="NumeTichet" Caption="NumeTichet"  Width="120px" Visible="false"/>
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
              </fieldset >
              <fieldset  >
                <legend class="legend-font-size">Criterii sortare</legend>
                  <table width="40%" >
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblCriteriu1"  runat="server"  Text="Criteriul 1: " ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="150" ID="cmbCriteriu1"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >							
                                <ClientSideEvents  SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }"/>
							</dx:ASPxComboBox>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblCriteriu2"  runat="server"  Text="Criteriul 2: " ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="150" ID="cmbCriteriu2"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >							
                                 <ClientSideEvents  SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }"/>
							</dx:ASPxComboBox>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblCriteriu3"  runat="server"  Text="Criteriul 3: " ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="150" ID="cmbCriteriu3"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >							
                                 <ClientSideEvents  SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }"/>
							</dx:ASPxComboBox>
						</td>
                    </tr>
                   </table>
              </fieldset >
               <fieldset  >
                <legend class="legend-font-size">Adrese</legend>
                <table width="60%" >
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblTipAdresa"  runat="server"  Text="Tip adresa: " ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="150" ID="cmbTipAdresa"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							</dx:ASPxComboBox>
						</td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxGridView ID="grDateAdrese" runat="server" ClientInstanceName="grDateAdrese" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDateAdrese_CustomCallback"
                                OnDataBinding="grDateAdrese_DataBinding" OnInitNewRow="grDateAdrese_InitNewRow" OnRowInserting="grDateAdrese_RowInserting" OnRowUpdating="grDateAdrese_RowUpdating" OnRowDeleting="grDateAdrese_RowDeleting">                               
                                <SettingsBehavior AllowFocusedRow="true" />
                                <Settings ShowFilterRow="false" ShowColumnHeaders="true"  />  
                                <Columns>  
                                    <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />                                                                          
                                     <dx:GridViewDataTextColumn FieldName="TipAdresa" Name="TipAdresa" Caption="TipAdresa"  Width="20px" Visible="false" ReadOnly="true"/>    
                                     <dx:GridViewDataComboBoxColumn FieldName="IdAdresaWiz" Name="IdAdresaWiz" Caption="Adresa Wiz" Width="200px" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>  
                                    <dx:GridViewDataComboBoxColumn FieldName="IdAdresaSodexo" Name="IdAdresaSodexo" Caption="Adresa Sodexo" Width="250px" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn> 
                                    <dx:GridViewDataTextColumn FieldName="Tichete" Name="Tichete" Caption="Tichete"  Width="200px" ReadOnly="true"/>        
                                    <dx:GridViewDataComboBoxColumn FieldName="PersContact" Name="PersContact" Caption="Persoana contact" Width="150px" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>                                                                 
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
              </fieldset >
                </td> 
            </tr>      
		</div>
            </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>

</asp:Content>