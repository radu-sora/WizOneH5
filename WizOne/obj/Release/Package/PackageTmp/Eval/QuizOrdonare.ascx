<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuizOrdonare.ascx.cs" Inherits="WizOne.Eval.QuizOrdonare" %>

<body>
    <table>
        <tr>
            <td>
                <dx:ASPxTreeList ID="grDateOrdonare" ClientInstanceName="grDateOrdonare" ClientIDMode="Static" runat="server" AutoGenerateColumns="false" KeyFieldName="Id" ParentFieldName="Parinte" Width="780px" OnBatchUpdate="grDateOrdonare_BatchUpdate" OnCustomCallback="grDateOrdonare_CustomCallback">
                    <Settings GridLines="Both" HorizontalScrollBarMode="Visible" ShowRoot="true" />
                    <SettingsBehavior AutoExpandAllNodes="true" AllowFocusedNode="true" FocusNodeOnLoad="false" ProcessFocusedNodeChangedOnServer="True" />
                     <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <ClientSideEvents BatchEditStartEditing="function(s,e) { onBatchEditStartEditing(s,e); }" />
                    <Columns>
                        <dx:TreeListTextColumn FieldName="Id" Visible="false" />
                        <dx:TreeListTextColumn FieldName="Descriere" Caption="Descriere" Visible="true" VisibleIndex="0" Width="100%" ReadOnly="true" />
                        <dx:TreeListTextColumn FieldName="OrdineAfisare" VisibleIndex="2"/>
                        <dx:TreeListTextColumn FieldName="Parinte" Visible="false" />
                    </Columns>
                </dx:ASPxTreeList>
            </td>
        </tr>
    </table>

    <script>
        function onBatchEditStartEditing(s, e) {
            if (e.focusedColumn.fieldName == "Descriere")
                e.cancel = true;
        }
    </script>
</body>