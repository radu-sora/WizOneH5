<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="NomenBeneficii.aspx.cs" Inherits="WizOne.Beneficii.NomenBeneficii" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">  

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDoc":
                    pnlLoading.Show();
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToFisierMode);
                    break;
            }
        }

        function GoToFisierMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=19&id=' + Value, '_blank ')
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

        function EndUpload(s) {
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
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
                                <CustomButtons>
                                    <dx:GridViewCommandColumnCustomButton ID="btnDoc" Visibility="BrowsableRow">
                                        <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                </CustomButtons>                        
                            </dx:GridViewCommandColumn>                              
						    <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" Width="200px" VisibleIndex="1" >
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                <SettingsHeaderFilter Mode="CheckedList" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataDateColumn FieldName="DeLaData" Name="DeLaData" Caption="Data inceput"  Width="100px" VisibleIndex="2">         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                <SettingsHeaderFilter Mode="CheckedList" />
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="LaData" Name="LaData" Caption="Data sfarsit"  Width="100px" VisibleIndex="3">         
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                <SettingsHeaderFilter Mode="CheckedList" />
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere"  Width="250px" VisibleIndex="4" >  
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                <SettingsHeaderFilter Mode="CheckedList" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataComboBoxColumn FieldName="IdGrup" Name="IdGrup" Caption="Grup beneficii" Width="100px" VisibleIndex="4">
                                <Settings SortMode="DisplayText" />
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            </dx:GridViewDataComboBoxColumn>
						    <dx:GridViewDataTextColumn FieldName="RON" Name="RON" Caption="RON" Width="70px" VisibleIndex="5" >
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                <SettingsHeaderFilter Mode="CheckedList" />
                            </dx:GridViewDataTextColumn>
						    <dx:GridViewDataTextColumn FieldName="EURO" Name="EURO" Caption="EURO" Width="70px" VisibleIndex="6" >
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                <SettingsHeaderFilter Mode="CheckedList" />
                            </dx:GridViewDataTextColumn>

						    <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" Width="50px"  Visible="false" ShowInCustomizationForm="false"/>
                            <dx:GridViewDataTextColumn FieldName="IdCategorie" Name="IdCategorie" Caption="IdCategorie" Width="50px"  Visible="false" ShowInCustomizationForm="false"/>
                             <dx:GridViewDataTextColumn FieldName="ValoareEstimata" Name="ValoareEstimata" Caption="ValoareEstimata" Width="50px"  Visible="false" ShowInCustomizationForm="false"/>
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
                                            <td id="lblDen" runat="server" style="padding-left:10px !important;">Denumire</td>
                                            <td id="lblDeLa" runat="server" style="padding-left:10px !important;">Data inceput</td>
                                            <td id="lblLa" runat="server" style="padding-left:10px !important;">Data sfarsit</td>
                                        </tr>
                                        <tr>
                                            <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtDen" runat="server" Width="250px" Value='<%# Bind("Denumire") %>' />      
                                            <td style="padding:10px !important;"><dx:ASPxDateEdit ID="deDeLaData" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DeLaData") %>' /></td>
                                            <td style="padding:10px !important;"><dx:ASPxDateEdit ID="deLaData" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("LaData") %>' /></td> 
                                        </tr>
                                        <tr>
                                            <td id="lblDesc" runat="server" style="padding:10px !important;">Descriere</td>
                                        </tr>
                                        <tr>
                                            <td rowspan="3" style="vertical-align:top;padding:10px !important;"><dx:ASPxMemo ID="txtDesc" runat="server" Width="500px" Height="150" Text='<%# Bind("Descriere") %>' /></td>
                                       </tr>
                                        <tr>
                                            <td id="lblRON" runat="server" style="padding-left:10px !important;">RON</td>
                                            <td id="lblEURO" runat="server" style="padding-left:10px !important;">EURO</td>
                                            <td id="lblGrup" runat="server" style="padding-left:10px !important;">Grup beneficii</td>
                                        </tr>
                                        <tr>
                                            <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtRON" runat="server" Width="70px" Value='<%# Bind("RON") %>' /> 
                                            <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtEURO" runat="server" Width="70px" Value='<%# Bind("EURO") %>' /> 
                                             <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbGrup" runat="server" Width="100px" ValueField="Id" DropDownWidth="100" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdGrup") %>' />
                                        </tr>
                                    <tr>
                                        <td style="padding:10px !important;" colspan="3">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadBen" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadBen" OnFileUploadComplete="btnDocUploadBen_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                                <ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                            </dx:ASPxUploadControl>
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
