<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Adresa.ascx.cs" Inherits="WizOne.Personal.Adresa" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<script>
    function grDate_CustomButtonClick(s, e) 
    {
        switch (e.buttonID) {
            case "btnGasit":
                pnlLoading.Show();
                grDateCautaAdresa.GetRowValues(e.visibleIndex, 'Artera;SirutaJudet;Judet;SirutaOras;MunOraCom;SirutaSat;LocSatSect', OnClickGasit);
                break;
        }
    }

    function OnClickGasit(values)
    {
        grDateAdresa.SetEditValue("Strada", values[0]);
        grDateAdresa.SetEditValue("Judet", values[2]);
        grDateAdresa.SetEditValue("Oras", values[4]);
        grDateAdresa.SetEditValue("Sat", values[6]);

        hfSiruta.Set("SirutaJudet", values[1]);
        hfSiruta.Set("SirutaOras", values[3]);
        hfSiruta.Set("SirutaSat", values[5]);

        pnlLoading.Hide();
        popUpCauta.Hide();
    }

    function DamiAdresa()
    {
        lblAdresa.SetText(grDateAdresa.cp_AdresaCurenta);
    }

    function OnEndCallback(s, e) {
        
        if (s.cp_Mesaj != null) {
            swal({
                title: "Atentie !", text: s.cp_Mesaj,
                type: "warning"
            });
            s.cp_Mesaj = null;
        }
    }

</script>

<table width="100%">
    <tr>
        <td align="left">
            <br />
            <span style="float:left; font-size:12px; margin-left:15px;">Adresa curenta - &nbsp;</span>
            <dx:ASPxLabel ID="lblAdresa" ClientIDMode="Static" ClientInstanceName="lblAdresa" runat="server" Text="" Font-Size="12px" Font-Bold="true" ForeColor="#00578a"/>
            <br /><br />
        </td>
    </tr>
    <tr>
        <td>
            <dx:ASPxHiddenField ID="hfSiruta" runat="server" ClientInstanceName="hfSiruta" ClientIDMode="Static"/>
            <dx:ASPxGridView ID="grDateAdresa" runat="server" ClientInstanceName="grDateAdresa" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false" OnDataBinding="grDateAdresa_DataBinding" OnInitNewRow="grDateAdresa_InitNewRow"
                OnRowInserting="grDateAdresa_RowInserting" OnRowUpdating="grDateAdresa_RowUpdating" OnRowDeleting="grDateAdresa_RowDeleting">
                <SettingsBehavior AllowFocusedRow="true" />
                <Settings ShowFilterRow="False" ShowColumnHeaders="true" />
                <ClientSideEvents ContextMenu="ctx" CustomButtonClick="function(s,e){ popUpCauta.Show(); }" EndCallback="DamiAdresa" />
                <SettingsEditing Mode="Inline" />
                <Columns>
                    <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                    <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Angajat"  Width="75px" Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                    <dx:GridViewDataComboBoxColumn FieldName="IdTipAdresa" Name="IdTipAdresa" Caption="Tip adresa" Width="150px">
                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewCommandColumn Width="75px" ButtonType="Image" ShowEditButton="false"  Caption=" ">
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="btnCauta" Visibility="EditableRow">
                                <Image ToolTip="Cauta" Url="~/Fisiere/Imagini/Icoane/lupa.png" />
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                        
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="IdTipStrada" Name="IdTipStrada" Caption="Tip artera" Visible="false" Width="150px" />
                    <dx:GridViewDataTextColumn FieldName="Judet" Name="Judet" Caption="Judet"  ReadOnly="true" Width="150px" />
                    <dx:GridViewDataTextColumn FieldName="SirutaJudet" Name="SirutaJudet" Caption="Judet" Visible="false" Width="200px" />
                    <dx:GridViewDataTextColumn FieldName="Oras" Name="Oras" Caption="Mun/Oras/Comuna" ReadOnly="true" Width="200px" />
                    <dx:GridViewDataTextColumn FieldName="SirutaOras" Name="SirutaOras" Caption="Oras" Visible="false" Width="200px" />
                    <dx:GridViewDataTextColumn FieldName="Sat" Name="Sat" Caption="Localitate/Sector" ReadOnly="true" Width="200px" />
                    <dx:GridViewDataTextColumn FieldName="SirutaSat" Name="SirutaSat" Caption="Sat" Visible="false" Width="200px" />
                    <dx:GridViewDataTextColumn FieldName="Strada" Name="Strada" Caption="Strada" Width="200px" />
                    <dx:GridViewDataTextColumn FieldName="Numar" Name="Numar" Caption="Numar" Width="75px" />
                    <dx:GridViewDataTextColumn FieldName="Bloc" Name="Bloc" Caption="Bloc" Width="75px" />
                    <dx:GridViewDataTextColumn FieldName="Scara" Name="Scara" Caption="Scara" Width="75px" />
                    <dx:GridViewDataTextColumn FieldName="Etaj" Name="Etaj" Caption="Etaj" Width="75px" />
                    <dx:GridViewDataTextColumn FieldName="Apartament" Name="Apartament" Caption="Apartament" Width="80px" />
                    <dx:GridViewDataTextColumn FieldName="CodPostal" Name="CodPostal" Caption="Cod postal" Width="80px" />
                    <dx:GridViewDataDateColumn FieldName="DataModif" Name="DataModif" Caption="Data modif." Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                    </dx:GridViewDataDateColumn>
                    <dx:GridViewDataCheckColumn FieldName="Principal" Name="Principal" Caption="Principal"  Width="70px"  />
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


<dx:ASPxPopupControl ID="popUpCauta" runat="server" HeaderText="Cauta adresa" ClientInstanceName="popUpCauta" OnWindowCallback="popUpCauta_WindowCallback"
     AllowDragging="False" AllowResize="False" ClientIDMode="Static" CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpModifArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="900px" Height="400px" 
        FooterText=" " CloseOnEscape="True" EnableHierarchyRecreation="false">
    <ClientSideEvents CloseUp="function(s, e) { txtLoc.SetText(''); txtArt.SetText(''); }" EndCallback="OnEndCallback" />
    <ContentCollection>
        <dx:PopupControlContentControl ID="Popupcontrolcontentcontrol1" runat="server">

            <div class="row">
                <div class="col-md-12">
                    <div style="display:inline-table; float:right;">
                        <dx:ASPxButton ID="btnCloseCautaAdr" ClientInstanceName="btnCloseCautaAdr" ClientIDMode="Static" runat="server" Text="Inchide" AutoPostBack="false">
                            <ClientSideEvents Click="function(s, e){ popUpCauta.Hide();}" />
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>
                        <br /><br />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-1">
                    <dx:ASPxLabel ID="lblLoc" runat="server" Text="Localitate" />	
                </div>
                <div class="col-md-4">
                    <dx:ASPxTextBox ID="txtLoc" runat="server" ClientInstanceName="txtLoc" ClientIDMode="Static" Width="200" AutoPostBack="false" />
                </div>
                 <div class="col-md-1">
                    <dx:ASPxLabel ID="lblArt" runat="server" Text="Artera" />
                </div>
                <div class="col-md-4">
                    <dx:ASPxTextBox ID="txtArt" runat="server" ClientInstanceName="txtArt" ClientIDMode="Static" Width="200" AutoPostBack="false" />
                </div>
                <div class="col-md-1">
                    <dx:ASPxButton ID="btnFiltruAdresa" runat="server" AutoPostBack="false">
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        <ClientSideEvents Click="function(s, e){ popUpCauta.PerformWindowCallback(popUpCauta.GetWindow(0),'Filtru') }" />
                    </dx:ASPxButton>
                </div>
            </div>
            <br />
            <dx:ASPxGridView ID="grDateCautaAdresa" runat="server" ClientInstanceName="grDateCautaAdresa" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" >
                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                <Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"/>
                <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" />
                <Columns>
                    <dx:GridViewCommandColumn Width="40px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid">
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="btnGasit">
                                <Image ToolTip="Adresa gasita" Url="~/Fisiere/Imagini/Icoane/validare.png" />
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="Judet" Name="Judet" Caption="Judet" Width="200" />
                    <dx:GridViewDataTextColumn FieldName="MunOraCom" Name="MunOraCom" Caption="Mun/Oras/Com" Width="200" />
                    <dx:GridViewDataTextColumn FieldName="LocSatSect" Name="LocSatSect" Caption="Localitate" Width="200" />
                    <dx:GridViewDataTextColumn FieldName="Artera" Name="Artera" Caption="Artera" Width="200" />

                    <dx:GridViewDataTextColumn FieldName="SirutaSat" Name="SirutaSat" Caption="SirutaSat"  Width="125px"  Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="SirutaOras" Name="SirutaOras" Caption="SirutaOras"  Width="125px" Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="SirutaJudet" Name="SirutaJudet" Caption="SirutaJudet"  Width="125px"  Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="125px"  Visible="false"/>
                </Columns>
            </dx:ASPxGridView>

        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>


