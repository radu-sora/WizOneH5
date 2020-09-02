<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajBulk.aspx.cs" Inherits="WizOne.Pontaj.PontajBulk" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />
				
                <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                    <label id="lblInc" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Data Inceput</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>
                    <label id="lblSf" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Sfarsit</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>
                    <div style="float:left;">
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </div>
                </div>
                <br /><br />
            </td>
        </tr>
		<tr>
			<td colspan="2">
				<dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" OnRowDeleting="grCC_RowDeleting" Width="100%" OnCellEditorInitialize="grDate_CellEditorInitialize"
					OnRowInserting="grCC_RowInserting" OnRowUpdating="grCC_RowUpdating" OnInitNewRow="grCC_InitNewRow" OnCustomErrorText="grDate_CustomErrorText">
					<SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
					<Settings ShowFilterRow="True" ShowGroupPanel="True" />
					<SettingsSearchPanel Visible="True" />
					<ClientSideEvents ContextMenu="ctx" />
					<SettingsEditing Mode="Inline" />
                    <ClientSideEvents EndCallback="function(s,e) { OnGridEndCallback(s); }" ContextMenu="ctx" />

					<Columns>
						<dx:GridViewCommandColumn ShowSelectCheckbox="false" ShowClearFilterButton="true" VisibleIndex="0" SelectAllCheckboxMode="None" Width="50px" ShowDeleteButton="true" 
                            ShowEditButton="true" ShowNewButtonInHeader="true" ButtonType="Image" Caption=" " />

                        <dx:GridViewDataDateColumn FieldName="Ziua" Name="Ziua" Caption="Ziua" Width="100px" VisibleIndex="1" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
						<dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Contract/Proiect" VisibleIndex="2" >
							<PropertiesComboBox TextField="F06205" ValueField="F06204" ValueType="System.Int32" DropDownStyle="DropDown">
							</PropertiesComboBox>
						</dx:GridViewDataComboBoxColumn>
						<dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" VisibleIndex="3">
							<PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
							</PropertiesComboBox>
						</dx:GridViewDataComboBoxColumn>
						<dx:GridViewDataSpinEditColumn FieldName="NrOre1" Name="NrOre" Caption="Nr. Ore" VisibleIndex="4" />
                        <dx:GridViewDataSpinEditColumn FieldName="NrOre10" Name="NrAng" Caption="Nr. Ang" VisibleIndex="5" />
                        <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii" VisibleIndex="6" PropertiesTextEdit-MaxLength="500" />
                        
						<dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajat" ReadOnly="true" Width="250px" Visible="false" VisibleIndex="7" ShowInCustomizationForm="false" >
							<PropertiesComboBox TextField="Nume" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown">
							</PropertiesComboBox>
						</dx:GridViewDataComboBoxColumn>

					</Columns>

					<SettingsCommandButton>
						<UpdateButton>
							<Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
							<Styles>
								<Style Paddings-PaddingRight="5px" />
							</Styles>
						</UpdateButton>
						<CancelButton>
							<Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
						</CancelButton>

						<EditButton>
							<Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
							<Styles>
								<Style Paddings-PaddingRight="5px" />
							</Styles>
						</EditButton>
						<DeleteButton>
							<Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
						</DeleteButton>

                        <NewButton>
                            <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                        </NewButton>

					</SettingsCommandButton>
				</dx:ASPxGridView>
			
			</td>
		</tr>
    </table>

    <script>
        function OnGridEndCallback(s) {
            if (s.cpAlertMessage) {
                swal({
                    title: "",
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }
        }
    </script>
</asp:Content>
