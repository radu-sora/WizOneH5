﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="CereriTactil.aspx.cs" Inherits="WizOne.Tactil.CereriTactil" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <script>        
        function StartUpload() {
            //pnlLoading.Show();
        }

        function EndUpload(s) {
            //pnlLoading.Hide();
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function ShowAbsDesc(tip) {
            if (tip == 1) {
                document.getElementById('txtAbsDesc').style.visibility = "visible";
            }
            else {
                document.getElementById('txtAbsDesc').style.visibility = "hidden";
            }
        }

        function DelayedCallback(strCallbackName) {
            pnlCtl.PerformCallback(1);
        }

        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }
    </script>

    <table style="width:100%;">
        <tr>
            <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgBack.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left"><Label runat="server"  id="lblMarca" style="font-weight: bold;font-size:20px">MARCA: </Label> </td>
            <td align="center"><Label  runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</Label></td>
            <td align="right">
                <dx:ASPxButton ID="btnLogOut" ClientInstanceName="btnLogOut" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Deconectare" AutoPostBack="true" OnClick="btnLogOut_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    

    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" meta:resourcekey="pnlCtlResource1" >
        <SettingsLoadingPanel Enabled="False"></SettingsLoadingPanel>
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
        <PanelCollection>
            <dx:PanelContent meta:resourcekey="PanelContentResource1">
                <table style="width:100%;" >
                    <tr>
                        <td width="130"></td>
                        <td width="275" align="left">
                                        <dx:ASPxRadioButton ID="rbMotiv1" runat="server" Width="200px"  Height="75"  HorizontalAlign="Center"   style="font-size:30px;"  Visible="false"
                                                RepeatDirection="Horizontal"  Text="Personal" Enabled="true"   ClientInstanceName="rbMotiv1"
                                                GroupName="Motiv">                                                                         
                                        </dx:ASPxRadioButton>
                        </td>
                        <td width="120"></td>
                        <td width="300" align="left" id="td1" runat="server">
                                        <dx:ASPxRadioButton ID="rbMotiv2" runat="server" Width="250px"  Height="75"  HorizontalAlign="Center"  style="font-size:30px;"  Visible="false"
                                                RepeatDirection="Horizontal"  Text="Interes serviciu" Enabled="true"  ClientInstanceName="rbMotiv2"
                                                GroupName="Motiv">                                                                         
                                        </dx:ASPxRadioButton> 
                        </td>
                        <td width="350"></td>
                    </tr>
                </table>
                <table style="width:100%;" >
                    <tr>
                        <td width="130"></td>
                        <td width="700" align="left" id="tdSelAbs" runat="server">
                                <dx:ASPxComboBox ID="cmbSelAbs" runat="server" ClientInstanceName="cmbSelAbs" ClientIDMode="Static" Width="600" ValueField="Id" DropDownWidth="600" DropDownHeight="500"  style="font-size:30px;" Height="75" 
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbAbsResource1" ButtonStyle-Width="75"   >
                                    <ItemStyle Font-Size="XX-Large" />
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(8); }" />                            
                                </dx:ASPxComboBox>
                        </td>  
                            <td width="300">
                    </tr>
                </table>

                <div class="row text-center align-center">     
                    <div class="Absente_divOuter">

                        <div class="Absente_Cereri_CampuriSup" id="divRol" runat="server">
                            <label id="lblRol" runat="server" style="display:inline-block;">Rol</label>
                            <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="250px" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbRolResource1" >
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(7); }" />
                            </dx:ASPxComboBox>
                        </div>

                        <div class="Absente_Cereri_CampuriSup">
                            <label id="lblTip" runat="server" style="display:inline-block;">Tip Cerere</label>
                            <dx:ASPxComboBox ID="cmbAbs" runat="server" ClientInstanceName="cmbAbs" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                                TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbAbsResource1" >
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(2); }" />
                            </dx:ASPxComboBox>
                        </div>


                        <div class="col-sm-4">
                            <div class="badgeTactil">
                                <h3>Data Inceput</h3>                           

                                                  
                            </div>
                        </div>
                    
                        <div class="col-sm-4"  id="lblDataSf" runat="server">
                            <div class="badgeTactil">
                                <h3>Data Sfarsit</h3>
                                
                  
                                
                            </div>
                        </div>    
                    
                        <div class="col-sm-4">
                            <div class="badgeTactil" id="lblNrOre" runat="server" visible="false">
                                <h3  id ="H3" runat="server">Nr. ore</h3>
                           </div>
                        </div>                                      

                    </div>
                </div>
                <table style="width:100%;" >
                    <tr>
                        <td width="130"></td>
                        <td width="275" align="left">
                                    <dx:ASPxDateEdit ID="txtDataInc"   runat="server"  style="font-size:30px;left:auto" Width="250px" HorizontalAlign="Center"   ButtonStyle-Width="75" Height="75"  DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataIncResource1" >
                             
                                        <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(3); }" />
                                
                                        <CalendarProperties>                                
                                            <MonthGridPaddings Padding="30px" />
                                            <DayStyle Font-Size="30px">
                                                <Paddings Padding="30px" />
                                            </DayStyle>
                                        </CalendarProperties>

                                    </dx:ASPxDateEdit> 
                        </td>
                        <td width="120"></td>
                        <td width="300" align="left" id="tdDataSf" runat="server">
                                    <dx:ASPxDateEdit ID="txtDataSf" runat="server"  style="font-size:30px;" Width="250px" HorizontalAlign="Center"     ButtonStyle-Width="75" Height="75"  DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataSfResource1" >
                               
                                        <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(6); }" />   
                                       <CalendarProperties>                                
                                            <MonthGridPaddings Padding="30px" />
                                            <DayStyle Font-Size="30px">
                                                <Paddings Padding="30px" />
                                            </DayStyle>
                                        </CalendarProperties>                               
                                    </dx:ASPxDateEdit>  
                        </td>
                        <td width="250" align="right"  id="tdNrOre" runat="server"  visible="false">
                                    <dx:ASPxSpinEdit ID="txtNrOre" runat="server" Width="200px"  Height="75"  HorizontalAlign="Center" ButtonStyle-Width="75"  style="font-size:30px;"  Visible="false"/>                                    
 
                        </td>
                        <td width="100"></td>
                    </tr>
                </table>

                <div class="row text-center align-center">     
                        <div class="Absente_divOuter">
                      
                            <div class="col-sm-4">
                                <div class="badgeTactil" id="lblZile" runat="server">
                                    <h3  id ="H1" runat="server">Nr. zile</h3>
                               </div>
                            </div>

                            <div class="col-sm-4">
                                <div class="badgeTactil" id="lblZileRamase" runat="server">
                                    <h3  id ="H2" runat="server">Nr. zile ramase</h3>
                               </div>
                            </div>


                        </div>

                </div>

                <table style="width:100%;" >
                    <tr>
                        <td width="130"></td>

                        <td width="275" align="left"  id="tdNrZile" runat="server">
                                    <dx:ASPxTextBox ID="txtNrZile" runat="server" Width="200px"  Height="75"  HorizontalAlign="Center" ButtonStyle-Width="75"  style="font-size:30px;"  Enabled="false"/>                                    
 
                        </td>
                        <td width="120"></td>
                        <td width="300" align="left"  id="tdNrZileRamase" runat="server">
                                    <dx:ASPxTextBox ID="txtNrZileRamase" runat="server" Width="200px"  Height="75"  HorizontalAlign="Center" ButtonStyle-Width="75"  style="font-size:30px;"  Enabled="false"/>                                    
 
                        </td>
                         <td width="350"></td>

                    </tr>
                </table>

                <table style="width:100%;" >
                    <tr>
                        <td>
                        <label id="lbl1" runat="server" style="display:inline-block;">&nbsp;</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <label id="lbl2" runat="server" style="display:inline-block;">&nbsp;</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <label id="lbl3" runat="server" style="display:inline-block;">&nbsp;</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <label id="lbl4" runat="server" style="display:inline-block;">&nbsp;</label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        <label id="lbl5" runat="server" style="display:inline-block;">&nbsp;</label>
                        </td>
                    </tr>
                </table>

                <div class="row text-center align-center">

                    <div class="col-sm-4">
                        <div class="badgeTactil">
                            <asp:LinkButton runat="server" ID="lnkCereri">
                                <div>
                        
                                </div>
                            </asp:LinkButton>                  
                        </div>
                    </div>

                    <div class="col-sm-4">
                        <div class="badgeTactil">
                            <asp:LinkButton runat="server" ID="lnkSave" OnClick="lnkSave_Click"  >      
                                <div>
                                    <img src ="../Fisiere/Imagini/bdgAprobare.jpg" alt = "Trimite la aprobare" />
                                </div>
                            </asp:LinkButton>
                            <h3>Trimite la aprobare</h3>
                        </div>
                    </div>

                </div>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>    

</asp:Content>

