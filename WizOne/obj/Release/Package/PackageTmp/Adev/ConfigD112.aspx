<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigD112.aspx.cs" Inherits="WizOne.Adev.ConfigD112" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Date declaratie lunara - Adeverinta somaj</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                </td>
            </tr>
            <tr>
                <td>
                     <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting">
                        <SettingsBehavior  AllowFocusedRow="true"  />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true" />
                        <SettingsSearchPanel Visible="False" />   
                        <SettingsEditing Mode="Inline" />  
                        <Columns>
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                            <dx:GridViewDataTextColumn FieldName="AN" Name="AN" Caption="Anul"  Width="75px" />                             
                            <dx:GridViewDataTextColumn FieldName="LUNA" Name="LUNA" Caption="Luna"  Width="75px" />                                                                                    	
                            <dx:GridViewDataTextColumn FieldName="DECLARATIE" Name="DECLARATIE" Caption="Nr. de inregistrare a declaratiei validate" Width="150px" />
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
