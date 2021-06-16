<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="AtribuireDocument.aspx.cs" Inherits="WizOne.Pagini.AtribuireDocument" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">  

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDoc":
                    pnlLoading.Show();
                    grDate.GetRowValues(e.visibleIndex, 'IdAuto', GoToFisierAtrMode);
                    break;
            }
        }

        function GoToFisierAtrMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=12&id=' + Value, '_blank ')
            pnlLoading.Hide();
        }

        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        var __focusedCtl = null;
        function onGotFocus(s, e) {
            __focusedCtl = s;
        }

    </script>

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
    </table>
    

        <table style="width:55%;">
            <tr>
                <td >
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  OnCustomCallback="grDate_CustomCallback" 
                         OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated">
                        <SettingsBehavior AllowFocusedRow="true"  />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true" />  
                        <SettingsEditing Mode="EditFormAndDisplayRow" />  
                        <ClientSideEvents CustomButtonClick="function(s, e) { grDate_CustomButtonClick(s, e); }"  ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />           
                        <Columns>           
                        <dx:GridViewCommandColumn Width="50px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >       
                        </dx:GridViewCommandColumn>                            
						    <dx:GridViewDataTextColumn FieldName="Entitate" Name="Entitate" Caption="Entitate" Width="100px" VisibleIndex="1" />
                            <dx:GridViewDataTextColumn FieldName="Inregistrare" Name="Inregistrare" Caption="Inregistrare"  Width="250px" VisibleIndex="2" />
                            <dx:GridViewCommandColumn Width="100px" ButtonType="Image"  Caption="Document" Visible="false" ShowInCustomizationForm="false" >
                                <CustomButtons>
                                    <dx:GridViewCommandColumnCustomButton ID="btnDoc" Visibility="BrowsableRow">
                                        <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                </CustomButtons>                        
                            </dx:GridViewCommandColumn>    
						    <dx:GridViewDataTextColumn FieldName="IdEntitate" Name="IdEntitate" Caption="IdEntitate" Width="50px"  Visible="false" ShowInCustomizationForm="false"/>
                            <dx:GridViewDataTextColumn FieldName="IdInregistrare" Name="IdInregistrare" Caption="IdInregistrare"  Width="50px" Visible="false" ShowInCustomizationForm="false"/> 
                            <dx:GridViewDataTextColumn FieldName="Document" Name="Document" Caption="Document"  Width="50px" Visible="false" ShowInCustomizationForm="false"/> 
					        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                            <dx:GridViewDataDateColumn FieldName="TIME" Name="Time" Caption="Time" Visible="false" ShowInCustomizationForm="false" />
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
                        <Templates>
                            <EditForm>
                                <div style="padding: 4px 3px 4px">
                                    <table class="auto-style8">
                                        <tr>
                                            <td style="padding-left:10px !important;">Entitate</td>
                                            <td style="padding-left:10px !important;">Inregistrare</td>
                                        </tr>
                                        <tr>
                                            <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbEntitate" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdEntitate") %>' >
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { grDate.PerformCallback(s.GetValue()); }"  />
                                                                                 </dx:ASPxComboBox>
                                            <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbInreg" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdInregistrare") %>' />
                                                                                       
                                        </tr>
                                        <tr>
                                            <td style="padding:10px !important;">Document</td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="vertical-align:top;padding:10px;" class="auto-style3">
                                                <dx:ASPxHtmlEditor ID="txtDoc" runat="server" ClientInstanceName="txtDoc" Height="370px" Width="800">
                                                    <ClientSideEvents GotFocus="onGotFocus" />
                                                    <Settings AllowHtmlView ="false" />
                                                    <SettingsDialogs>
                                                        <InsertImageDialog>
                                                            <SettingsImageUpload UploadFolder="~/UploadFiles/Images/">
                                                                <ValidationSettings AllowedFileExtensions=".jpe,.jpeg,.jpg,.gif,.png" MaxFileSize="500000">
                                                                </ValidationSettings>
                                                            </SettingsImageUpload>
                                                        </InsertImageDialog>
                                                    </SettingsDialogs>
                                                </dx:ASPxHtmlEditor>
                                            </td>
                                        </tr> 
                                        <tr>
                                            <td class="auto-style3" style="padding:10px;">
                                                <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                    <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" runat="server" ReplacementType="EditFormUpdateButton" />
                                                    <dx:ASPxGridViewTemplateReplacement ID="CancelButton" runat="server" ReplacementType="EditFormCancelButton" />
                                                </div>
                                            </td>
                                        </tr>                                        
                                    </table>
                                </div>
                            </EditForm>
                        </Templates>
                    </dx:ASPxGridView>
                </td>
            </tr>
        </table>
    

</asp:Content>
