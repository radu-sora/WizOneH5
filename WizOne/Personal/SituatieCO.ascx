<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SituatieCO.ascx.cs" Inherits="WizOne.Personal.SituatieCO" %>




<body>
    <td>
        <tr halign="center">
            <dx:ASPxButton ID="btnCalc" ClientInstanceName="btnCalc" ClientIDMode="Static" runat="server" Text="Calcul CO" OnClick="btnCalc_Click" AutoPostBack="true"  >
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnCalcSI" ClientInstanceName="btnCalcSI" ClientIDMode="Static" runat="server" Text="Calcul SI" OnClick="btnCalcSI_Click" AutoPostBack="true"  >
            </dx:ASPxButton>
        </tr>
        <tr>
        <dx:ASPxGridView ID="grDateSituatieCO" runat="server" ClientInstanceName="grDateSituatieCO" ClientIDMode="Static" Width="43%" AutoGenerateColumns="false"  OnDataBinding="grDateSituatieCO_DataBinding" >
            <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
            <Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"  />                
                <Columns>            
                    <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca"  Width="75px" Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="An" Name="An" Caption="Anul"  Width="75px" />
                    <dx:GridViewDataTextColumn FieldName="RamaseAnterior" Name="RamaseAnterior" Caption="Ramase anterior"  Width="75px"  HeaderStyle-Wrap="True" />
                    <dx:GridViewDataTextColumn FieldName="Cuvenite" Name="Cuvenite" Caption="Cuvenite"  Width="75px"  HeaderStyle-Wrap="True"/>
                    <dx:GridViewDataTextColumn FieldName="Total" Name="Total" Caption="Total cuvenite"  Width="75px"  HeaderStyle-Wrap="True"/>
                    <dx:GridViewDataTextColumn FieldName="Aprobate" Name="Aprobate" Caption="Aprobate"  Width="75px"  HeaderStyle-Wrap="True"/>
                    <dx:GridViewDataTextColumn FieldName="Ramase" Name="Ramase" Caption="Ramase curent"  Width="75px"  HeaderStyle-Wrap="True"/>
                    <dx:GridViewDataTextColumn FieldName="Solicitate" Name="Solicitate" Caption="Solicitate"  Width="75px"  HeaderStyle-Wrap="True"/>
                    <dx:GridViewDataTextColumn FieldName="Planificate" Name="Planificate" Caption="Planificate"  Width="75px"  HeaderStyle-Wrap="True"/>
                    <dx:GridViewDataTextColumn FieldName="RamaseDePlanificat" Name="RamaseDePlanificat" Caption="Ramase de planificat"  Width="75px"  HeaderStyle-Wrap="True"/>
                </Columns>
        </dx:ASPxGridView>
        </tr>
    </td>

</body>