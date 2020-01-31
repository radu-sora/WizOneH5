<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigCIC.aspx.cs" Inherits="WizOne.Adev.ConfigCIC" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Date Adeverinta CIC</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

    <script type="text/javascript">
        function OnBtnClientClick(s, e) {
            window.parent.HidePopupAndShowInfo();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                    <dx:ASPxButton ID="btnGenerare" ClientInstanceName="btnGenerare" ClientIDMode="Static" runat="server" AutoPostBack="false" Text="Genereaza" OnClick="btnGen_Click" oncontextMenu="ctx(this,event)">                         
                        <Image Url="~/Fisiere/Imagini/Icoane/finalizare.png"></Image>
                        <ClientSideEvents Click="OnBtnClientClick" />
                    </dx:ASPxButton>
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnInitNewRow="grDate_InitNewRow" 
                        OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting">
                        <SettingsBehavior  AllowFocusedRow="true"  />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true" />
                        <SettingsSearchPanel Visible="False" />   
                        <SettingsEditing Mode="Inline" />    
                        <Columns>	
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                            <dx:GridViewDataTextColumn FieldName="MARCA" Name="MARCA" Caption="Marca" Width="75px" ReadOnly="true" />
                            <dx:GridViewDataTextColumn FieldName="NUME" Name="NUME" Caption="Nume si prenume copil"  Width="180px" />
                            <dx:GridViewDataDateColumn FieldName="DATA_NASTERE" Name="DATA_NASTERE" Caption="Data nastere copil"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="MATERNITATE_DELA" Name="MATERNITATE_DELA" Caption="Indemnizatie maternitate in perioada de la"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="MATERNITATE_PANALA" Name="MATERNITATE_PANALA" Caption="Indemnizatie maternitate in perioada pana la"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="INDEMNIZATIE_DELA" Name="INDEMNIZATIE_DELA" Caption="Indemnizatie copil in perioada de la"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="INDEMNIZATIE_PANALA" Name="INDEMNIZATIE_PANALA" Caption="Indemnizatie copil in perioada pana la"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="DATA_APROBARE" Name="DATA_APROBARE" Caption="Data aprobare CIC"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
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


    </form>
</body>
</html>
