﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.Contracte.Detalii" %>




<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width:100%">
	    <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
		    <td class="pull-right">
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
			    <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="Contract.aspx" >
				    <Image Url="../Fisiere/Imagini/Icoane/iesire.png"></Image>
			    </dx:ASPxButton>
		    </td>
	    </tr>
        <tr>
            <td colspan="2">
                <div  style="margin:15px 0px; display:inline-block;">
                    <div class="ctl_inline">
                        <label id="lblId" runat="server">Id</label>
                        <dx:ASPxTextBox ID="txtId" Width="50" runat="server" ClientEnabled="false"/>
                    </div>
                    <div class="ctl_inline">
                        <label id="lblDenumire" runat="server">Denumire</label>
                        <dx:ASPxTextBox ID="txtDenumire" Width="400" runat="server" AutoPostBack="false"/>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <dx:ASPxPageControl ID="tabCtr" ClientInstanceName="tabCtr" runat="server" Width="100%" Height="100%" CssClass="dxtcFixed" ActiveTabIndex="0">
                    <TabPages>
                        <dx:TabPage Text="Absente">
                            <ContentCollection>
                                <dx:ContentControl ID="ContentControl1" runat="server">
                                    <div  style="margin:15px 0px; display:inline-block;">
                                        <div class="ctl_inline">
                                            <label id="lblOraSchIn" runat="server">Ora Schimbare In</label>
							                <dx:ASPxTimeEdit ID="txtOraSchIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblOraSchOut" runat="server">Ora Schimbare Out</label>
							                <dx:ASPxTimeEdit ID="txtOraSchOut" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblOreSup" runat="server">Ore suplimentare</label>
                                            <dx:ASPxCheckBox ID="chkOreSup" runat="server" AutoPostBack="false"/>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblAfisare" runat="server">Afisare ore</label>
							                <dx:ASPxComboBox ID="cmbAfisare" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="150">
                                                <Items>
                                                    <dx:ListEditItem Text="Trunchiere la ore" Value="1" />
                                                    <dx:ListEditItem Text="Cu minute" Value="2" />
                                                    <dx:ListEditItem Text="Cu zecimale" Value="3" />
                                                </Items>
							                </dx:ASPxComboBox>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblRap" runat="server">Tip raportare ore noapte</label>
    							            <dx:ASPxComboBox ID="cmbRap" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="150">
                                                <Items>
                                                    <dx:ListEditItem Text="Pe inceput de schimb" Value="1" />
                                                    <dx:ListEditItem Text="Pe sfarsit de schimb" Value="2" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblPontareAuto" runat="server">Initializare automata</label>
                                            <dx:ASPxCheckBox ID="chkPontareAuto" runat="server" AutoPostBack="false"/>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblOraInInit" runat="server">Ora intrare</label>
                                            <dx:ASPxTimeEdit  ID="txtOraIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                        </div>
                                        <div class="ctl_inline">
                                            <label id="lblOraOut" runat="server">Ora iesire</label>
                                            <dx:ASPxTimeEdit  ID="txtOraOut" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                        </div>
                                    </div>
                                    
                                    <dx:ASPxGridView ID="grDateAbs" runat="server" ClientInstanceName="grDateAbs" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDateAbs_BatchUpdate" SettingsPager-PageSize="50">
                                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                        <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                                        <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                                        <SettingsSearchPanel Visible="false" />
                                        <SettingsLoadingPanel Mode="ShowAsPopup" />
                                        <ClientSideEvents ContextMenu="ctx" />
                                        <Columns>
                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                            <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" Width="250px" >
                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                            </dx:GridViewDataComboBoxColumn>   
                                            <dx:GridViewDataCheckColumn FieldName="ZL" Name="ZL" Caption="Zile Lucr." Width="60px"  />
                                            <dx:GridViewDataCheckColumn FieldName="S" Name="S" Caption="Sambata" Width="60px"  />
                                            <dx:GridViewDataCheckColumn FieldName="D" Name="D" Caption="Duminica" Width="60px"  />
                                            <dx:GridViewDataCheckColumn FieldName="SL" Name="SL" Caption="Sarb. Legale" Width="60px" />
                                            <dx:GridViewDataCheckColumn FieldName="InPontajAnual" Name="InPontajAnual" Caption="Istoric extins" Width="60px"  />

                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/> 
                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>      
                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />              
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
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <dx:TabPage Text="Programe">
                            <ContentCollection>
                                <dx:ContentControl ID="ContentControl2" runat="server">
                                    <dx:ASPxImage runat="server" ID="dxImage_NewYork" ImageUrl="~/Content/TabControl/Images/Cities/BrooklynBridge.jpg" CssClass="dxtc-image-newyork" />
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                    </TabPages>
                </dx:ASPxPageControl>
            </td>
        </tr>
    </table>

    <script>

    </script>

</asp:Content>