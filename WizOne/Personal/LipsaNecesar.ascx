<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LipsaNecesar.ascx.cs" Inherits="WizOne.Personal.LipsaNecesar" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateNecesar" runat="server" ClientInstanceName="grDateNecesar" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateNecesar_DataBinding"  >
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />         
                    <ClientSideEvents ContextMenu="ctx" />                                                 
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Nume Obiect Lipsa" Width="490px" >
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="ValoareEstimata" Name="ValoareEstimata" Caption="Valoare Estimata"  Width="490px" />                                                       
                    </Columns>
                </dx:ASPxGridView>                    
            </td>
        </tr>
    </table> 

</body>


