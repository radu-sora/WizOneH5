<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajSpecial.aspx.cs" Inherits="WizOne.Pontaj.PontajSpecial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        var __name_text_box = "";
        var __name_label = "";

        function ArataPopUp(s, e) {
            if (e.htmlEvent.keyCode == 24 || e.htmlEvent.keyCode == 107 || e.htmlEvent.keyCode == 187) {
                popUpModif.Show();
                ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
                __name_text_box = s.name;
                __name_label = s.name.replace("txt", "lbl");
            }
        }

        function OnModif(s, e) {   
            popUpModif.Hide();      
            txtValuri.Set(__name_text_box, "");
            var texts = "";
            if (cmbTipAbs.GetText() != "")
                texts = cmbTipAbs.GetText();
            else {
                $('#<% =pnlValuri.ID %> input[type="text"]').each(function () {
                    
                    if ($(this).val() != '' && $(this).val() != '0') {
                        var tmp = $(this).attr('id').replace('_I', '').replace('flo1', '');
                        var lista = tmp.split("_");
                        //texts += "/" + $(this).val() + $(this).attr('id').replace('_I', '').replace('flo1', '');
                        texts += "/" + $(this).val() + lista[1]; 
                        var valoare = txtValuri.Get(__name_text_box);                     
                        if (valoare.length > 0)
                            valoare = valoare + ";";
                        else
                            valoare = "";
                        txtValuri.Set(__name_text_box, valoare + lista[0] + "=" + $(this).val());
                    }
                });
             
                txtValuri.Set("IdProgram_" + __name_text_box, cmbProgr.GetValue());
                if (texts != "")
                    texts = texts.substring(1);               
                    
            }

            //if (texts != "")
            //{
                var txt = ASPxClientControl.GetControlCollection().GetByName(__name_text_box);
                txt.SetText(texts);             
                var lbl = ASPxClientControl.GetControlCollection().GetByName(__name_label);
               
                if (cmbProgr.GetValue() == null) {
                    lbl.SetText("-");
                    lbl.GetMainElement().title = "";
                }
                else {
                    //lbl.SetText(cmbProgr.GetValue());
                    lbl.SetText("-");
                    var tipProgr = "<%=Session["PtjSpecial_ProgrameJS"] %>";
                    var resProgr = tipProgr.split(";");
                    for (var i = 0; i < resProgr.length; i++) {
                        var linieProgr = resProgr[i].split(",");
                        if (linieProgr[0] == cmbProgr.GetValue()) {
                            lbl.SetText(linieProgr[2]);
                            lbl.GetMainElement().title = linieProgr[1];
                        }
                }


            }

            //}
            __name_text_box = "";
            __name_label = "";
            EmptyVal();
            cmbTipAbs.SetValue(null);
            cmbProgr.SetValue(null);
        }

        function EmptyVal() {
            $('#<% =pnlValuri.ID %> input[type="text"]').val('');
        }

        function EmptyCmbAbs(s, e) {
            cmbTipAbs.SetValue(null);
        }

        function EmptyFields(s, e) {
            //cmbAng.SetValue(null);
            //cmbCtr.SetValue(null);
            //cmbStare.SetValue(null);

            //cmbSub.SetValue(null);
            //cmbSec.SetValue(null);
            //cmbFil.SetValue(null);
            //cmbDept.SetValue(null);
            //cmbSubDept.SetValue(null);
            //cmbBirou.SetValue(null);
            //cmbCateg.SetValue(null);

            pnlCtl.PerformCallback('EmptyFields');
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
            pnlLoading.Hide();
        }

    </script>

</asp:Content>

 

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">  
        .centerText input {  
            text-align: center;  
        }  
    </style>

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">  
                <dx:ASPxButton ID="btnPtjEch" ClientInstanceName="btnPtjEch" ClientIDMode="Static" runat="server" Text="Pontajul echipei" AutoPostBack="true" PostBackUrl="../Pontaj/PontajEchipa.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/preluare.png"></Image>
                </dx:ASPxButton>  
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Initializare" AutoPostBack="true" OnClick="btnInit_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>  
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
                    <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <table style="margin-left:15px;">
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Rol</label>
                                            <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdRol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRol'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                            <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" SelectInputTextOnClick="true"
                                                        CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                                </Columns>
                                            </dx:ASPxComboBox>
                                        </div>                               
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSub" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Subcomp.</label>
                                            <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblFil" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Filiala</label>
                                            <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Sectie</label>
                                            <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Dept.</label>
                                            <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSubDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:80px;">Subdept.</label>
                                            <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblBirou" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Birou</label>
                                            <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                             </dx:ASPxComboBox>
                                        </div>   
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblCtr" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Contract</label>
                                            <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" />
                                        </div>    
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblCateg" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Categorie</label>
                                            <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.String" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />                                
                                        </div>                                        
                                    </td> 
                                 </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;  padding-bottom:10px;" >
                                            <label id="lblDeLa" runat="server" style="display:inline-block; float:left;  min-width:54px; width:80px;">De la</label>
						                    <dx:ASPxDateEdit  ID="dtDataStart" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
							                </dx:ASPxDateEdit>	
                                         </div>  
                                         <div style="float:left; padding-right:15px;">
                                             <label id="lblLa" runat="server" style="display:inline-block; float:left;   width:70px;">La</label>
						                    <dx:ASPxDateEdit  ID="dtDataSfarsit" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
							                </dx:ASPxDateEdit>
                                         </div>
                                         <div style="float:left; padding-right:15px;">
                                            <label id="lblSablon" runat="server" style="display:inline-block; float:left;  min-width:54px; width:60px;">Sablon</label>
                                            <dx:ASPxComboBox ID="cmbSablon" ClientInstanceName="cmbSablon" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSablon'); }" />                                 
                                            </dx:ASPxComboBox>
                                         </div>
                                     </td>   
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblNumeSablon" runat="server" style="display:inline-block; float:left;  min-width:75px; width:80px;">Nume sablon</label>
							                <dx:ASPxTextBox  ID="txtNumeSablon" style="display:inline-block; float:left; width:100px;" runat="server"  AutoPostBack="false" />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblNrZileSablon" runat="server" style="display:inline-block; float:left;  min-width:54px; width:70px;">Nr. zile</label>
                                            <dx:ASPxComboBox ID="cmbNrZileSablon" ClientInstanceName="cmbNrZileSablon" ClientIDMode="Static" runat="server" Width="50px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbNrZileSablon'); }" />                                  
                                            </dx:ASPxComboBox>
                                         </div> 
                                        <div style="float:left;padding-right:15px;">
                                            <dx:ASPxButton ID="btnSablon" runat="server"  RenderMode="Link" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                                <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback('btnSablon'); }" />
                                            </dx:ASPxButton>
                                        </div>  
                                        <div style="float:left;padding-right:15px;">
                                            <dx:ASPxButton ID="btnSterge" runat="server"  RenderMode="Link" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                                <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                                                <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback('btnSterge'); }" />
                                            </dx:ASPxButton>
                                        </div> 
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;padding-bottom:10px;" >
							                <dx:ASPxTextBox ID="txtZiua1" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua2" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua3" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua4" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua5" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua6" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua7" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua8" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua9" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua10" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                       </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;" >
							                <dx:ASPxTextBox ID="lblZiua1" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua2" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua3" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />                                           
							                <dx:ASPxTextBox ID="lblZiua4" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua5" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua6" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua7" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua8" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua9" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua10" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;padding-bottom:10px;" >
							                <dx:ASPxTextBox ID="txtZiua11" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua12" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua13" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua14" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua15" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua16" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua17" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua18" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua19" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua20" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                       </div>
                                    </td>
                                </tr>
                               <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;" >
							                <dx:ASPxTextBox ID="lblZiua11" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua12" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua13" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />                                           
							                <dx:ASPxTextBox ID="lblZiua14" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua15" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua16" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua17" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua18" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua19" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua20" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         <div style="float:left; padding-right:15px;padding-bottom:10px;" >
							                <dx:ASPxTextBox ID="txtZiua21" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua22" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua23" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua24" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua25" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua26" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua27" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua28" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua29" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua30" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                            <dx:ASPxTextBox ID="txtZiua31" ReadOnly="true" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false" AutoPostBack="false" ClientSideEvents-KeyDown="function(s, e) { ArataPopUp(s,e) }" />
                                       </div>                                   
                                    </td>
                                </tr>
                               <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;" >
							                <dx:ASPxTextBox ID="lblZiua21" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua22" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua23" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />                                           
							                <dx:ASPxTextBox ID="lblZiua24" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua25" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua26" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua27" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua28" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua29" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua30" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />
							                <dx:ASPxTextBox ID="lblZiua31" ReadOnly="true" Text="" Border-BorderStyle="None" style="display:inline-block; float:left; width:75px;" CssClass="centerText" runat="server" Visible="false" AutoPostBack="false"  />

                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                         <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblBife" runat="server" style="display:inline-block; float:left;  width:150px; padding-bottom:10px; vertical-align:text-bottom;">Initializarea sa includa:</label>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <dx:ASPxCheckBox ID="chkS"  runat="server" style="display:inline-block; float:left;   width:100px; padding-bottom:10px; vertical-align:text-bottom;" Text="Sambata"  TextAlign="Left" ClientInstanceName="chkbx1" />                                    
                                            <dx:ASPxCheckBox ID="chkD"  runat="server" style="display:inline-block; float:left;  width:100px; padding-bottom:10px; vertical-align:text-bottom;" Text="Duminca"  TextAlign="Left" ClientInstanceName="chkbx2" />   
                                            <dx:ASPxCheckBox ID="chkSL"  runat="server" style="display:inline-block; float:left;  width:200px; padding-bottom:10px; vertical-align:text-bottom;" Text="Sarbatori legale"  TextAlign="Left" ClientInstanceName="chkbx3" />   
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <dx:ASPxCheckBox ID="chkDecalare"  runat="server" style="display:inline-block; float:left; font-weight:bold;   width:150px; padding-bottom:10px; vertical-align:text-bottom;" Text="Decalare pontaj"  TextAlign="Left" ClientInstanceName="chkbx4" />                                    
                                       </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <dx:ASPxCheckBox ID="chkPontare"  runat="server" style="display:inline-block; float:left;   width:150px; padding-bottom:10px; vertical-align:text-bottom;" Text="Pentru pontare"  TextAlign="Left" ClientInstanceName="chkbx5" oncontextMenu="ctx(this,event)"/>                                    
                                            <dx:ASPxCheckBox ID="chkPlanif"  runat="server" style="display:inline-block; float:left;  width:150px; padding-bottom:10px; vertical-align:text-bottom;" Text="Pentru planificare"  TextAlign="Left" ClientInstanceName="chkbx6" oncontextMenu="ctx(this,event)"/>   
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblZiStart" runat="server" style="display:inline-block; float:left;   min-width:75px; width:200px;">Initializarea sa inceapa cu ziua</label>
							                <dx:ASPxTextBox  ID="txtZiStart" style="display:inline-block; float:left; width:50px;" runat="server"  AutoPostBack="false" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>


                <div style="float:left; padding:0px 15px;">
                    <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        <ClientSideEvents Click="function(s, e) {
                                        pnlLoading.Show();
                                        e.processOnServer = true;
                                    }" />
                    </dx:ASPxButton>
                </div>

                <div style="float:left;">
                    <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        <ClientSideEvents Click="EmptyFields" />
                    </dx:ASPxButton>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <br />
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" />                   
                    <ClientSideEvents ContextMenu="ctx"  />
                    <Columns>
                        
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true" FixedStyle="Left" VisibleIndex="3" Width="250px" Settings-AutoFilterCondition="Contains"/>
              
                        <dx:GridViewDataTextColumn FieldName="Companie" Caption="Companie" ReadOnly="true"  Width="200" />
                        <dx:GridViewDataTextColumn FieldName="Subcompanie" Caption="Subcompanie" ReadOnly="true"  Width="200"/>
                        <dx:GridViewDataTextColumn FieldName="Filiala" Caption="Filiala" ReadOnly="true" Width="200" />
                        <dx:GridViewDataTextColumn FieldName="Sectie" Caption="Sectie" ReadOnly="true" Width="200"/>
                        <dx:GridViewDataTextColumn FieldName="Dept" Caption="Dept" ReadOnly="true" Width="200"/>
                        <dx:GridViewDataTextColumn FieldName="Subdept" Caption="Subdept" ReadOnly="true" Width="100" />
                        <dx:GridViewDataTextColumn FieldName="Birou" Caption="Birou" ReadOnly="true" Width="100" />
                        <dx:GridViewDataTextColumn FieldName="Categorie" Caption="Categorie" ReadOnly="true" Width="100" />
                    </Columns>
                    
                </dx:ASPxGridView>

                <br />
    
            </td>
        </tr>
    </table>

    <dx:ASPxPopupControl ID="popUpModif" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpModifArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="550px" Height="200px" HeaderText="Modificare pontaj"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpModif" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <div class="row">
                        <div class="col-sm-12">
                            <div style="display:inline-table; float:right;">
                                <dx:ASPxButton ID="btnModif" runat="server" Text="Salveaza" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnModif(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <span style="font-weight:bold; font-size:14px;" id="modifAbsZi" runat="server">Absente de tip zi</span>
                                <br />
                                <br />
                            </div>
                        </div>
                        <div class="row" style="text-align:center;">
                            <div class="col-md-12">
                                <div style="display:inline-table;">
                                    <dx:ASPxComboBox ID="cmbTipAbs" runat="server" ClientIDMode="Static" ClientInstanceName="cmbTipAbs" Width="200px" DropDownWidth="350px" ValueField="Id" TextField="DenumireScurta" AutoPostBack="false" TextFormatString="{0}">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="DenumireScurta" Caption="Id" Width="50" />
                                            <dx:ListBoxColumn FieldName="Denumire" Caption="Denumire" Width="200" />
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) {
                                                    e.processOnServer = false;
                                                    EmptyVal();
                                                }" />
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <br /><br /><br />
                                <span style="font-weight:bold; font-size:14px;" id="modifAbsOra" runat="server">Absente de tip ora</span>
                            </div>
                        </div>
                        <div class="row" id="pnlValuri" runat="server" style="margin:20px 50px 50px 50px;">
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <span style="font-weight:bold; font-size:14px;" id="modifPrgLucru" runat="server">Program de lucru</span>
                                <br />
                                <br />
                            </div>
                        </div>
                        <div class="row" style="text-align:center;">
                            <div class="col-md-12">
                                <div style="display:inline-table;">
                                    <dx:ASPxComboBox ID="cmbProgr" runat="server" ClientIDMode="Static" ClientInstanceName="cmbProgr" Width="200px" DropDownWidth="350px" ValueField="Id" TextField="Denumire" AutoPostBack="false" TextFormatString="{0}"/>                                                                                     
                                </div>
                            </div>
                        </div>
                    </div>
                    <dx:ASPxHiddenField ID="txtValuri" runat="server"></dx:ASPxHiddenField>         
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>



</asp:Content>
