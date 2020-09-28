<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Dosar.ascx.cs" Inherits="WizOne.Personal.Dosar" %>



<body>
    <script>

        var modifDosar = false;

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=11&id=' + Value, '_blank ')
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({ title: "", text: s.cpAlertMessage, type: "warning" });
                s.cpAlertMessage = null;
            }
        }

        function grDateDosar_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnSterge":
                    grDateDosar.PerformCallback('btnSterge;' + s.GetRowKey(e.visibleIndex));
                    break;
            }
        }

        function RiseFlag() {
            modifDosar = true;
        }

    </script>

    <dx:ASPxButton ID="btnSolNoua" ClientInstanceName="btnSolNoua" ClientIDMode="Static" runat="server" Text="Solicitare noua" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
        <ClientSideEvents Click="function(s, e) { grDateDosar.UpdateEdit(); }" />
    </dx:ASPxButton>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateDosar" runat="server" ClientInstanceName="grDateDosar" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" 
                    OnBatchUpdate="grDateDosar_BatchUpdate" OnInitNewRow="grDateDosar_InitNewRow" OnCustomButtonInitialize="grDateDosar_CustomButtonInitialize"
                    OnCustomCallback="grDateDosar_CustomCallback" OnHtmlRowCreated="grDateDosar_HtmlRowCreated">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" CustomButtonClick="grDateDosar_CustomButtonClick" BatchEditStartEditing="RiseFlag" ContextMenu="ctx" /> 
                    <Columns>
                        <dx:GridViewCommandColumn Width="60px" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" ">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnSterge">
                                    <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataColumn Caption=" " VisibleIndex="0" Width="50px"  Settings-ShowEditorInBatchEditMode="false" CellStyle-HorizontalAlign="Center">
                            <DataItemTemplate>
                                <img id="imgExista" runat="server" title="Indica daca are sau nu are fisier" src='<%# GetImagePath(Eval("AreFisier")) %>' />
                            </DataItemTemplate>
                        </dx:GridViewDataColumn>
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Angajat"  Width="75px" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Denumire" Width="250px" >
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" Width="100%"/>
                        <dx:GridViewDataTextColumn FieldName="FisierNume" Name="FisierNume" Caption="Nume"  Width="250px" Settings-ShowEditorInBatchEditMode="false" />
                        <dx:GridViewDataTextColumn FieldName="FisierExtensie" Name="FisierExtensie" Caption="Extensie"  Width="250px" Settings-ShowEditorInBatchEditMode="false" />

                        <dx:GridViewBandColumn Caption="Atasamente" HeaderStyle-HorizontalAlign="Center" Name="colAtas">
                            <Columns>
                                <dx:GridViewDataColumn Width="100px" Caption="Incarca" CellStyle-HorizontalAlign="Center" Name="colUpload" Settings-ShowEditorInBatchEditMode="false" BatchEditModifiedCellStyle-HorizontalAlign="Center">
                                    <DataItemTemplate>
                                        <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ShowProgressPanel="true" Height="28px"
                                            BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                            OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                            <BrowseButton>
                                                <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                            </BrowseButton>
                                            <ValidationSettings ShowErrors="False"></ValidationSettings>
                                            <ClientSideEvents FileUploadComplete="function(s,e) { grDateDosar.PerformCallback('btnDocUpload'); }" />
                                        </dx:ASPxUploadControl>
                                    </DataItemTemplate>
                                </dx:GridViewDataColumn>
                        
                                <dx:GridViewDataColumn Width="100px" Caption="Vizualizare" CellStyle-HorizontalAlign="Center" Name="colArata" Settings-ShowEditorInBatchEditMode="false">
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnArata" runat="server" Text="" AutoPostBack="false" ToolTip="Arata document" HorizontalAlign="Center" oncontextMenu="ctx(this,event)" >
                                            <Image Url="~/Fisiere/Imagini/Icoane/arata.png"></Image>
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataColumn>
                                
                                <dx:GridViewDataColumn Width="100px" Caption="Stergere" CellStyle-HorizontalAlign="Center" Name="colSterge" Settings-ShowEditorInBatchEditMode="false">
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnStergeFisier" runat="server" Text="" AutoPostBack="false" ToolTip="Sterge document" oncontextMenu="ctx(this,event)" >
                                            <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataColumn>
                            </Columns>
                        </dx:GridViewBandColumn>

                        <dx:GridViewDataDateColumn FieldName="AreFisier" Name="AreFisier" Caption="AreFisier" Visible="false" ShowInCustomizationForm="false" /> 
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />                                              
                    </Columns>
                    <SettingsCommandButton>
                        <NewButton>
                            <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                        </NewButton>
                        <DeleteButton>
                            <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                        </DeleteButton>
                    </SettingsCommandButton>
                </dx:ASPxGridView>                    
            </td>
        </tr>
    </table> 



</body>