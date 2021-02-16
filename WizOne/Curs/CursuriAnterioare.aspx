<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CursuriAnterioare.aspx.cs" Inherits="WizOne.Curs.CursuriAnterioare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">
        function grDate_CustomButtonClick(s, e) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=14&id=' + s.GetRowKey(s.GetFocusedRowIndex()), '_blank ')
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function EndUpload(s) {
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnSave"  runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 
    <br />
     <table width="70%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnRowInserting="grDate_RowInserting" OnRowDeleting="grDate_RowDeleting" OnDataBinding="grDate_DataBinding"  OnInitNewRow="grDate_InitNewRow"  >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="90px" VisibleIndex="0" ButtonType="Image" ShowEditButton="true" ShowDeleteButton="true" ShowNewButtonInHeader="true" Caption=" " Name="butoaneGrid" >  
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata fisierul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>                          
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="CursNume" Name="CursNume" Caption="Denumire curs"  Width="200px" >                         
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataTextColumn FieldName="SesiuneNume" Name="SesiuneNume" Caption="Tip curs"   Width="200px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Organizator" Name="Organizator" Caption="Organizator"  Width="200px" >                         
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataTextColumn FieldName="Certificat" Name="Certificat" Caption="Specializare"   Width="200px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii"   Width="200px" >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdStare" Name="IdStare" Caption="Stare" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="areFisier" Name="areFisier" Caption="areFisier" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Anulare"  Image-ToolTip="Anulare">
                            <Styles>
                                <Style Font-Bold="true" />
                            </Styles>
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
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
                                <table>
                                    <tr>
                                        <td id="lblObs" runat="server" style="padding-left:10px !important;">Observatii</td>
                                        <td id="lblDataInc" runat="server" style="padding:10px !important;">Data inceput</td>
                                        <td id="lblDataSf" runat="server" style="padding:10px !important;">Data sfarsit</td>
                                    </tr>
                                    <tr>
                                        <td rowspan="5" style="vertical-align:top;padding:10px !important;"><dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="150" Text='<%# Bind("Observatii") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataInc" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataInceput") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataSf" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataSfarsit") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td id="lblNumeCurs" runat="server" style="padding-left:10px !important;" >Denumire curs</td>
                                        <td id="lblNumeSesiune" runat="server" style="padding-left:10px !important;" >Tip curs</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNumeCurs" runat="server" Width="250px" Value='<%# Bind("CursNume") %>' /></td>    
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNumeSesiune" runat="server" Width="250px" Value='<%# Bind("SesiuneNume") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td id="lblOrg" runat="server" style="padding-left:10px !important" >Organizator</td>
                                        <td id="lblSpec" runat="server" style="padding-left:10px !important">Specializare</td>                              
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtOrg" runat="server" Width="250px" Value='<%# Bind("Organizator") %>' /></td>    
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtSpec" runat="server" Width="250px" Value='<%# Bind("Certificat") %>' /></td>  
                                    </tr>                                    
                                    <tr>
                                        <td style="padding:10px !important;" colspan="2">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUpload" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                                <ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                            </dx:ASPxUploadControl>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">
                                            <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                                <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
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
