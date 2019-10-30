<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Evaluare.ascx.cs" Inherits="WizOne.Personal.Evaluare" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateEvaluare" runat="server" ClientInstanceName="grDateEvaluare" ClientIDMode="Static" Width="80%" AutoGenerateColumns="False"  OnDataBinding="grDateEvaluare_DataBinding"  OnInitNewRow="grDateEvaluare_InitNewRow"
                    OnRowInserting="grDateEvaluare_RowInserting" OnRowUpdating="grDateEvaluare_RowUpdating" OnRowDeleting="grDateEvaluare_RowDeleting">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateEvaluare_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>

<SettingsPopup>
<HeaderFilter MinHeight="140px"></HeaderFilter>
</SettingsPopup>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="3" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false" VisibleIndex="0"/>
                        <dx:GridViewDataTextColumn FieldName="An" Name="An" Caption="An"  Width="75px" VisibleIndex="4" />
                        <dx:GridViewDataTextColumn FieldName="Punctaj_max" Name="Punctaj_max" Caption="Punctaj maxim"  Width="100px" VisibleIndex="5" />
                        <dx:GridViewDataTextColumn FieldName="Punctaj" Name="Punctaj" Caption="Punctaj" VisibleIndex="8" Width="100px" />                        
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" VisibleIndex="1" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" VisibleIndex="2" />
                        <dx:GridViewDataTextColumn Caption="Observatii" FieldName="Observatii" Name="Observatii" VisibleIndex="9" Width="250px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Recomandari" FieldName="Recomandari" Name="Recomandari" VisibleIndex="10" Width="250px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Info" FieldName="Info" Name="Info" VisibleIndex="11" Width="250px">
                        </dx:GridViewDataTextColumn>
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
<Paddings PaddingTop="10px" PaddingRight="10px"></Paddings>
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Renunta">
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" >
<Paddings PaddingRight="5px"></Paddings>
                                </Style>
                            </Styles>
                        </EditButton>
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                        </DeleteButton>
                        <NewButton Image-ToolTip="Rand nou">
                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                            <Styles>
                                <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" >
<Paddings PaddingLeft="5px" PaddingRight="5px"></Paddings>
                                </Style>
                            </Styles>
                        </NewButton>
                    </SettingsCommandButton>
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table> 



</body>