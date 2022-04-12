<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Contr4.aspx.cs" Inherits="WizOne.Pagini.Contr4" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        
        function OnEndCallbackSupervizori(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
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
                <td>
                    <div style="float:left; padding-right:15px; padding-bottom:10px;">
                        <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" SelectInputTextOnClick="true"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div> 
                </td>
                    <td>
                        <div style="float:left; padding:0px 15px;">
                            <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                                <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                <ClientSideEvents Click="function(s, e) {
                                                pnlLoading.Show();
                                                e.processOnServer = true;
                                            }" />
                            </dx:ASPxButton>
                        </div>
                </td>
                </tr>
            </table>
            <br />
            <table style="width:100%;">
                <tr>
                    <td >

     <dx:ASPxGridView ID="grDateSupervizori" runat="server" ClientInstanceName="grDateSupervizori" ClientIDMode="Static" Width="75%" AutoGenerateColumns="false"  OnDataBinding="grDateSupervizori_DataBinding" OnInitNewRow="grDateSupervizori_InitNewRow"
        OnRowInserting="grDateSupervizori_RowInserting" OnRowUpdating="grDateSupervizori_RowUpdating" OnRowDeleting="grDateSupervizori_RowDeleting"  OnCommandButtonInitialize="grDateSupervizori_CommandButtonInitialize">
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents CustomButtonClick="function(s, e) { grDateSupervizori_CustomButtonClick(s, e); }" ContextMenu="ctx" EndCallback="OnEndCallbackSupervizori"/> 
        <SettingsEditing Mode="Inline" />       
        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
        <Columns>
            <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca"  Width="75px" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Nume complet"  Width="150px" Visible="false"/>
            <dx:GridViewDataComboBoxColumn FieldName="IdSuper" Name="IdSuper" Caption="Supervizor"  Width="250px" >
                <Settings SortMode="DisplayText" />
                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataComboBoxColumn FieldName="IdUser" Name="IdUser" Caption="Utilizator"  Width="250px" >
                <Settings SortMode="DisplayText" />
                <PropertiesComboBox TextField="F70104" ValueField="F70102" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn> 
            <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px" >  
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>                                                                                      
 
            <dx:GridViewDataCheckColumn FieldName="Modificabil" Name="Modificabil" Caption="Modificabil" ReadOnly="true"  Visible="false"  Width="70px"  />
            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO"  Width="75px" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME"  Width="75px" Visible="false"/>                         
              
        </Columns>
        <SettingsCommandButton>
            <UpdateButton ButtonType="Link" Text="Actualizeaza">
                <Styles>
                    <Style Paddings-PaddingRight="10" Paddings-PaddingTop="20">
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




    

</asp:Content>
