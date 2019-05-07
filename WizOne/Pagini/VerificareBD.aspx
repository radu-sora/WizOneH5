<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="VerificareBD.aspx.cs" Inherits="WizOne.Pagini.VerificareBD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
 
    
</script>

    <body>
        <table width="100%">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
                </tr>
        </table>  
         <table width="100%">
          <tr>    
              <td>             
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%">
                        <Settings ShowFilterRow="True" ShowGroupPanel="False" VerticalScrollBarMode="Visible" VerticalScrollableHeight="500" />
                        <SettingsSearchPanel Visible="False" />
                        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                        <ClientSideEvents ContextMenu="ctx" />
                        <Columns>
                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" />                         
                            <dx:GridViewDataTextColumn FieldName="TABLE_NAME_REF" Name="TABLE_NAME_REF" Caption="Tabela REF" />                                 
                            <dx:GridViewDataTextColumn FieldName="COLUMN_NAME_REF" Name="COLUMN_NAME_REF" Caption="Coloana REF" />
                            <dx:GridViewDataTextColumn FieldName="COLUMN_DEFAULT_REF" Name="COLUMN_DEFAULT_REF" Caption="Val. Implicita REF" />
                            <dx:GridViewDataTextColumn FieldName="IS_NULLABLE_REF" Name="IS_NULLABLE_REF" Caption="Accepta NULL REF" />
                            <dx:GridViewDataTextColumn FieldName="DATA_TYPE_REF" Name="DATA_TYPE_REF" Caption="Tip data REF" />
                            <dx:GridViewDataTextColumn FieldName="CHARACTER_MAXIMUM_LENGTH_REF" Name="CHARACTER_MAXIMUM_LENGTH_REF" Caption="Lungime sir REF" />
                            <dx:GridViewDataTextColumn FieldName="NUMERIC_PRECISION_REF" Name="NUMERIC_PRECISION_REF" Caption="Nr. cifre REF" />
                            <dx:GridViewDataTextColumn FieldName="NUMERIC_SCALE_REF" Name="NUMERIC_SCALE_REF" Caption="Nr. zecimale REF" />

                            <dx:GridViewDataTextColumn FieldName="TABLE_NAME_CLIENT" Name="TABLE_NAME_CLIENT" Caption="Tabela CLIENT" />                                 
                            <dx:GridViewDataTextColumn FieldName="COLUMN_NAME_CLIENT" Name="COLUMN_NAME_CLIENT" Caption="Coloana CLIENT" />
                            <dx:GridViewDataTextColumn FieldName="COLUMN_DEFAULT_CLIENT" Name="COLUMN_DEFAULT_CLIENT" Caption="Val. Implicita CLIENT" />
                            <dx:GridViewDataTextColumn FieldName="IS_NULLABLE_CLIENT" Name="IS_NULLABLE_CLIENT" Caption="Accepta NULL CLIENT" />
                            <dx:GridViewDataTextColumn FieldName="DATA_TYPE_CLIENT" Name="DATA_TYPE_CLIENT" Caption="Tip data CLIENT" />
                            <dx:GridViewDataTextColumn FieldName="CHARACTER_MAX_LENGTH_CLIENT" Name="CHARACTER_MAX_LENGTH_CLIENT" Caption="Lungime sir CLIENT" />
                            <dx:GridViewDataTextColumn FieldName="NUMERIC_PRECISION_CLIENT" Name="NUMERIC_PRECISION_CLIENT" Caption="Nr. cifre CLIENT" />
                            <dx:GridViewDataTextColumn FieldName="NUMERIC_SCALE_CLIENT" Name="NUMERIC_SCALE_CLIENT" Caption="Nr. zecimale CLIENT" />                    
                                
                        </Columns>
                    </dx:ASPxGridView>   
                  </td>           
	    </tr>
    </table>  

    </body>

</asp:Content>