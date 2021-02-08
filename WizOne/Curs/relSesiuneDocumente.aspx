<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="relSesiuneDocumente.aspx.cs" Inherits="WizOne.Curs.relSesiuneDocumente" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="lblTitlu" runat="server"></title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" >

    </script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

    <script language="javascript" type="text/javascript">


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
            grDate.PerformCallback();
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                    <dx:ASPxUploadControl ID="btnIncarca" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                        BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                        ClientInstanceName="UploadImage" OnFileUploadComplete="btnIncarca_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocResource1">
                        <BrowseButton>
                            <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                        </BrowseButton>
                        <ValidationSettings ShowErrors="False"></ValidationSettings>
                        <ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
                    </dx:ASPxUploadControl>
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%"  OnRowDeleting="grDate_RowDeleting"   OnCustomCallback="grDate_CustomCallback" >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="false" />
                        <SettingsEditing Mode="EditFormAndDisplayRow" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />
                         <ClientSideEvents  ContextMenu="ctx"  EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                        <Columns>
                            <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                            <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" Caption=" " ShowDeleteButton="true" ShowNewButtonInHeader="true" Name="butoaneGrid" >
 
                            </dx:GridViewCommandColumn>
                                          
                            <dx:GridViewDataTextColumn FieldName="FisierNume" Name="FisierNume" Caption="Document"  Width="300px"/>                           

                            <dx:GridViewDataTextColumn FieldName="Tabela" Name="Tabela" Caption="Tabela" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                            <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                            <dx:GridViewDataTextColumn FieldName="EsteCerere" Name="EsteCerere" Caption="EsteCerere" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
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
                   
                            <DeleteButton Image-ToolTip="Sterge">
                                <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                            </DeleteButton>                    
                        </SettingsCommandButton> 
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
