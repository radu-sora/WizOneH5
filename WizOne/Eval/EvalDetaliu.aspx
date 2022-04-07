<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Cadru.Master" CodeBehind="EvalDetaliu.aspx.cs" Inherits="WizOne.Eval.EvalDetaliu" %>


<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server" >
    <script language="javascript" type="text/javascript">



        function OnClickLabelSectiune(s) {
            var index = s.name.substring(4);
            txtNrSectiune.SetText(index);
            hf.Set("IdSec", index);
            pnlSectiune.PerformCallback(s.name);
        }

        function OnClickAccesSectiune(s) {
            hf.Set("IdSec", txtNrSectiune.GetText());
            pnlSectiune.PerformCallback(s.name);
        }

        function OnTextChangedHandlerCtr(s, e) {
            pnlSectiune.PerformCallback(s.name + "#$" + s.GetText());
        }

        function OnCMBTipChanged(s, e) {
            pnlSectiune.PerformCallback(s.name + "#$" + s.GetText());
        }
        function OnCHKChanged(s, e) {
            pnlSectiune.PerformCallback(s.name + "#$" + s.GetValue());
        }
        
        function OnDateChangedHandlerCtr(s, e) {
            pnlSectiune.PerformCallback(s.name + "#$" + s.GetText());
        }

        var currentEditingIndex;
        var lastObjective;
        var isCustomCascadingCallback = false;

        function RefreshData(objectiveValue) {
            hfObiectiv.Set("CurrentObjective", objectiveValue);  
            ActivityEditor.PerformCallback();
        }

        function cmbObiectiv_SelectedIndexChanged(s, e) {
            lastObjective = s.GetValue();
            isCustomCascadingCallback = true;
            RefreshData(lastObjective);
        }

        function OnBatchEditStartEditing(s, e) {
            currentEditingIndex = e.visibleIndex;
            var currentObjective = grid.BatchEditApi.GetCellValue(currentEditingIndex, "IdObiectiv");
            if (currentObjective != lastObjective && e.focusedColumn.fieldName == "IdActivitate" && currentObjective != null) {
                lastObjective = currentObjective;
                RefreshData(currentObjective);
            }
        }

        function OnFinalizare(s, e) {
            swal({
                title: 'Sunteti sigur/a ca vreti sa finalizati?', text: 'Nu veti mai putea modifica dupa finalizare!',
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    pnlLoading.Show();
                    pnlSectiune.PerformCallback("btnFinalizare");
                }
            });
        }

        function OnTabSelectionChanged(s) {
            pnlSectiune.PerformCallback(s.name);
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
            pnlLoading.Hide();
        }

        function OnLuareLaCunostinta(s, e) {
            swal({
                title: 'Acord', text: 'Mentionati daca sunteti sau nu de acord cu rezultatele acestui formular!',
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da', cancelButtonText: 'Nu', closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    pnlSectiune.PerformCallback("btnLuatCunostinta#$1");
                }
                else
                {
                    pnlSectiune.PerformCallback("btnLuatCunostinta#$0");
                }                
            });
        }

    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

         <dx:ASPxCallbackPanel ID="pnlSectiune" ClientIDMode="Static" ClientInstanceName="pnlSectiune" ScrollBars="Vertical" runat="server" OnCallback="pnlSectiune_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }"/>
        <PanelCollection>
            <dx:PanelContent>
    <table width="100%">
        <tr>
            <td align="left" />
            <td align="right">
                <dx:ASPxButton ID="btnLuatCunostinta" ClientInstanceName="btnLuatCunostinta"  ClientIDMode="Static" runat="server" Visible="false" Text="Luare la cunostinta" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e){
                        OnLuareLaCunostinta(s,e);
                        }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/user.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnFinalizare" runat="server" ClientInstanceName="btnFinalizare" Text="Finalizare" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e){
                        OnFinalizare(s,e);
                        }" />
                    <Image Url="../Fisiere/Imagini/Icoane/finalizare.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        pnlSectiune.PerformCallback('btnSave');
                        }" />
                    <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="EvalLista.aspx?q=56">
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

                <div style="width:100%; display:inline; text-align:left;">
                    <span id="lblEvaluat" runat="server" style="font-size:14px; margin:15px 15px 25px 0px; padding:5px; font-weight:bold; background-color:#dadada;"></span>
                </div>

                <div class="pnlTaburiEval">
                    <ul id="ulCtn" runat="server">

                    </ul>
                </div>

                <table width="100%" style="visibility:hidden;">
                    <tr aria-orientation="horizontal" width="100%">
                        <td width="100%" aria-orientation="horizontal">
                            <table>
                                <tr>
                                    <td>
                                        <dx:ASPxLabel ID="btnInapoi" ClientInstanceName="btnInapoi" runat="server" Text="0" ForeColor="Transparent" CssClass="badge">
                                            <ClientSideEvents Click="function(s, e) {
                                                                OnClickAccesSectiune(s); 
                                                                }" />
                                            <BackgroundImage ImageUrl ="../Fisiere/Imagini/Icoane/btnInapoi.png" />
                                        </dx:ASPxLabel>
                                    </td>
                                    <td> </td>
                                    <td>
                                        <dx:ASPxLabel ID="txtNrSectiune" ClientIDMode="Static" ClientInstanceName="txtNrSectiune" Text="1" runat="server" Font-Bold="true" Font-Size="Medium" />
                                    </td>
                                    <td> </td>
                                    <td>
                                        <dx:ASPxLabel ID="txtDinSectiune" ClientIDMode="Static" ClientInstanceName="txtDinSectiune" Text="din" runat="server" Font-Bold="true" Font-Size="Medium" />
                                    </td>
                                    <td> </td>
                                    <td>
                                        <dx:ASPxLabel ID="txtNrTotalSectiune" ClientIDMode="Static" ClientInstanceName="txtNrTotalSectiune" Text="35" runat="server" Font-Bold="true" Font-Size="Medium" />
                                    </td>
                                    <td> </td>
                                    <td>
                                        <dx:ASPxLabel ID="btnInainte" ClientInstanceName="btnInainte" runat="server" Text="0" ForeColor="Transparent" CssClass="badge">
                                            <ClientSideEvents Click="function(s, e) {
                                                                    OnClickAccesSectiune(s); 
                                                                    }" />
                                            <BackgroundImage ImageUrl="../Fisiere/Imagini/Icoane/btnInainte.png" />
                                        </dx:ASPxLabel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <dx:ASPxTabControl ID="tabSuper" runat="server">
                    <ClientSideEvents ActiveTabChanged="function(s, e) { OnTabSelectionChanged(s); }" />
                    <Tabs>
                    </Tabs>
                </dx:ASPxTabControl>
                
                <br /><br />

                <dx:ASPxPanel id="divIntrebari" runat="server" style="height:50vh;"></dx:ASPxPanel>
                
            </dx:PanelContent>

        </PanelCollection>
    </dx:ASPxCallbackPanel>
    <dx:ASPxHiddenField ID="hf" ClientIDMode="Static" runat="server" ClientInstanceName="hf" />
    <dx:ASPxHiddenField ID="hfObiectiv" ClientIDMode="Static" runat="server" ClientInstanceName="hfObiectiv" />
    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="grid" />

     
    
    <script>
        function OnGridBatchEditStartEditing(s, e) {
            var col = e.focusedColumn.fieldName;
            if (s.cp_ColoaneRO != null && s.cp_ColoaneRO.indexOf(col + ";") >= 0)
                e.cancel = true; 
        }
    </script>

    
    
          
    
</asp:Content>