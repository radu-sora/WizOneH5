<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateIdentificare.ascx.cs" Inherits="WizOne.Personal.DateIdentificare" %>


<script type="text/javascript">

    function VerifMarca(s) {
        pnlCtlDateIdent.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnClickDI(s) {
        pnlLoading.Show();
        pnlCtlDateIdent.PerformCallback(s.name);
    }

    function OnConfirm() {
        pnlCtlDateIdent.PerformCallback("PreluareDate");
    }

    function StartUploadDI() {
        pnlLoading.Show();
    }

    function EndUploadDI(s) {
        pnlLoading.Hide();
        pnlCtlDateIdent.PerformCallback("img");
    }

    function GoToIstoricDI(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    function OnEndCallbackDI(s, e) {
        pnlLoading.Hide();
        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }

        if (s.cp_InfoMessage != null) {
            swal({
                title: "Informare", text: s.cp_InfoMessage,
                type: "info", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da!", cancelButtonText: "Nu", closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    pnlCtlDateIdent.PerformCallback("PreluareDate");
                }
            });
            s.cp_InfoMessage = null;
        }
    }

    function ValidareCNP(s, e) {
        if (<%=Session["MP_NuPermiteCNPInvalid"] %> == 1) {
            var cnp = s.GetText();
            if (cnp != "") {
                if (verifCnp(cnp)) {

                    var strZiua = "2000-01-01";
                    switch (cnp.substr(0, 1)) {
                        case '1':
                        case '2':
                            an = "19";
                            break;
                        case '3':
                        case '4':
                            an = "18";
                            break;
                        case '5':
                        case '6':
                            an = "20";
                            break;
                    }
                    strZiua = an + cnp.substr(1, 2) + "-" + cnp.substr(3, 2) + "-" + cnp.substr(5, 2);
                    var ziua = new Date(strZiua);
                    deDataNasterii.SetValue(ziua);

                    var azi = new Date();
                    var varsta = dateDiffInDays(ziua, azi);
                    txtVarsta.SetValue(varsta);

                    var idSex = 0;
                    if ((parseInt(cnp.substr(0, 1)) % 2) != 0)
                        idSex = 1;
                    else
                        idSex = 2;
                    rbSex.SetValue(idSex);

                    if (varsta < 16) {
                        swal({ title: "Atentie !", text: "Nu puteti angaja o persoana cu varsta mai mica de 16 ani!", type: "warning" });
                    }
                    else {
                        swal({ title: "Va rugam asteptati !", text: "Se verifica CNP-ul", type: "warning" });
                        pnlLoading.Show();
                        pnlCtlDateIdent.PerformCallback(s.name + ";" + s.GetText());
                    }
                }
                else {
                    swal({ title: "Atentie !", text: "CNP invalid", type: "warning" });
                }
            }
        }
    }

    function map(fn, arr) {
        var ret = [];
        for (var x = 0; x < arr.length; x++)
            ret.push(fn(arr[x]));
        return ret;
    }

    function reduce(fn, arr, initial) {
        ret = initial;
        for (var i = 0; i < arr.length; i++) { ret = fn(arr[i], ret); }
        return ret;
    }

    function sum(arr) { return reduce(function (x, y) { return x + y; }, arr, 0); }

    function verifCnp(cnp) {
        if (cnp.length != 13)
            return false;

        cnp = map(parseInt, cnp.split(''));

        var coefs = [2, 7, 9, 1, 4, 6, 3, 5, 8, 2, 7, 9];
        var idx = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

        var s = map(function (x) { return coefs[x] * cnp[x]; }, idx);
        s = sum(s) % 11;

        return (s < 10 && s == cnp[12]) || (s == 10 && cnp[12] == 1);
    }

    function dateDiffInDays(a, b) {
        const _MS_PER_DAY = 1000 * 60 * 60 * 24 * 365;

        // Discard the time and time-zone information.
        const utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
        const utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());

        return Math.floor((utc2 - utc1) / _MS_PER_DAY);
    }

    function CalcVarsta() {
        var azi = new Date();
        //var ziua = new Date(strZiua);
        var ziua = deDataNasterii.GetValue();
        var varsta = dateDiffInDays(ziua, azi);
        txtVarsta.SetValue(varsta);
    }

</script>



<body>
	<style type="text/css">
		.fieldset-auto-width {
				display: inline-block;                         
		}
        .legend-font-size
        {
            font-size:15px;
            font-weight:bold;
        }
	</style>

    <dx:ASPxCallbackPanel ID = "DateIdentificare_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlDateIdent" runat="server" OnCallback="pnlCtlDateIdent_Callback"  SettingsLoadingPanel-Enabled="false">
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackDI(s,e); }" />
        <PanelCollection>
            <dx:PanelContent>

                        <table>
                            <tr align="left">

                                <td valign="bottom">
			                      <fieldset class="fieldset-auto-width">
				                    <legend id="lgFoto" runat="server" class="legend-font-size">Fotografie</legend>
				                    <table width="200" height="200"  valign="bottom">
                                        <tr>
                                            <td align="left"  valign="bottom">
                                                <img  Height="180" HorizontalAlignment="Center" ID="img" runat="server"  VerticalAlignment="Center" Width="170" />
                                            </td>
                                        </tr>
                                        <tr style="display:inline-block;" halign="right" valign="bottom">
					                        <td >
                                                <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="false" HorizontalAlignment="Center"
                                                    BrowseButton-Text="Incarca"  FileUploadMode="OnPageLoad" UploadMode="Advanced"  AutoStartUpload="true"  ToolTip="Incarca fotografie" ShowTextBox="false"
                                                    ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                    <BrowseButton>
                                                        <Image Url="../Fisiere/Imagini/Icoane/incarca.png" Width="16px" Height="16px"></Image>                                    
                                                    </BrowseButton>
                                                    <ClientSideEvents FilesUploadStart="StartUploadDI" FileUploadComplete="function(s,e) { EndUploadDI(s); }" />
                                                </dx:ASPxUploadControl>
                                            </td>   
                                            <td >
                                                <dx:ASPxButton ID="btnSterge" runat="server" ToolTip="Sterge fotografie" HorizontalAlignment="Center" Text="Sterge" AutoPostBack="false"  Height="28">
                                                    <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px"></Image>
                                                    <Paddings PaddingLeft="0px" PaddingRight="0px" />
                                                    <ClientSideEvents Click="function(s,e) { pnlCtlDateIdent.PerformCallback('btnSterge'); }" />
                                                </dx:ASPxButton>

					                        </td>
                                        </tr>	                    
				                    </table>
			                      </fieldset>                    	
                                </td>
                 
                                <td style="padding:0px 15px;"></td>

                                <td valign="top">

                                    <asp:DataList ID="DateIdentificare_DataList" runat="server">            
                                        <ItemTemplate> 

			                        <fieldset class="fieldset-auto-width">
				                    <legend id="lgIdent" runat="server" class="legend-font-size">Date unice de identificare</legend>
				                    <table width="40%">	
					                    <tr>				
						                    <td >
							                    <dx:ASPxLabel  ID="lblMarca" runat="server"  Text="Marca" ></dx:ASPxLabel >	
						                    </td>	
						                    <td>
							                    <dx:ASPxTextBox  ID="txtMarcaDI" runat="server" Text='<%# Eval("F10003") %>' AutoPostBack="false" >
                                                    <ClientSideEvents TextChanged="function(s,e){ VerifMarca(s); }" />
							                    </dx:ASPxTextBox >
						                    </td>
					                    </tr>	
					                    <tr>				
						                    <td >
							                    <dx:ASPxLabel  ID="lblCNP" runat="server"  Text="CNP/CUI" ></dx:ASPxLabel>
						                    </td>	
						                    <td>
							                    <dx:ASPxTextBox  ID="txtCNPDI" runat="server" Text='<%# Eval("F10017") %>'  AutoPostBack="false" >
                                                    <ClientSideEvents TextChanged="function(s,e){ ValidareCNP(s,e); }" />
							                    </dx:ASPxTextBox >
						                    </td>
					                    </tr>
					                    <tr>				
						                    <td>		
							                    <dx:ASPxLabel  ID="lblMarcaUnica" runat="server"  Text="Marca unica"></dx:ASPxLabel >	
						                    </td>
						                    <td>	
							                    <dx:ASPxTextBox  ID="txtMarcaUnica" runat="server" Text='<%# Eval("F1001036") %>' ReadOnly="true" AutoPostBack="false"  ></dx:ASPxTextBox>										
						                    </td>
					                    </tr>
					                    <tr>				
						                    <td>		
							                    <dx:ASPxLabel  ID="lblEID" runat="server"  Text="EID"></dx:ASPxLabel >	
						                    </td>
						                    <td>	
							                    <dx:ASPxTextBox  ID="txtEIDDI" runat="server" Text='<%# Eval("F100901") %>'  AutoPostBack="false" >
							                    </dx:ASPxTextBox >										
						                    </td>
					                    </tr>					
				                    </table>
			                        </fieldset>

			                        <fieldset class="fieldset-auto-width">
				                    <legend id="lgSex" runat="server" class="legend-font-size">Data nasterii si Sex</legend>
				                    <table width="60%">	
					                    <tr>				
						                    <td>
							                    <dx:ASPxLabel  ID="lblDataNasterii" Width="100" runat="server"  Text="Data nasterii" ></dx:ASPxLabel>	
						                    </td>	
						                    <td>
							                    <dx:ASPxDateEdit  ID="deDataNasterii" ClientInstanceName="deDataNasterii"  Enabled="true" Width="100" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" runat="server" Value='<%# Eval("F10021") %>'  AutoPostBack="false" >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                                    <ClientSideEvents DateChanged="function(s,e){ CalcVarsta(); }" />
							                    </dx:ASPxDateEdit>
						                    </td>
                                        </tr>
                                        <tr>
						                    <td>
							                    <dx:ASPxLabel  ID="lblVarsta"  Width="100" runat="server"  Text="Varsta" ></dx:ASPxLabel>	
						                    </td>	
						                    <td>
							                    <dx:ASPxTextBox  ID="txtVarsta" ClientInstanceName="txtVarsta" Width="75" ClientEnabled="false" runat="server" AutoPostBack="false" ></dx:ASPxTextBox >
						                    </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblSex" runat="server" Text="Sex"></dx:ASPxLabel>
                                            </td>
                                            <td>
                                                <dx:ASPxRadioButtonList ID="rbSex" runat="server" ClientInstanceName="rbSex" RepeatDirection="Horizontal">
                                                    <Items>
                                                        <dx:ListEditItem Text="Masculin" Value="1" />
                                                        <dx:ListEditItem Text="Feminin" Value="2" />
                                                    </Items>
                                                </dx:ASPxRadioButtonList>
                                            </td>

					                    </tr>						
				                    </table>
			                        </fieldset>
          
        
	
			                        <fieldset class="fieldset-auto-width">
				                    <legend id="lgNume" runat="server" class="legend-font-size">Nume si prenume</legend>
				                    <table width="40%">	
					                    <tr>				
						                    <td>	
							                    <dx:ASPxLabel  ID="lblNume" runat="server" Text="Nume"></dx:ASPxLabel >	
						                    </td>
						                    <td>		
							                    <dx:ASPxTextBox  ID="txtNume" runat="server" Text='<%# Eval("F10008") %>'  AutoPostBack="false" >
							                    </dx:ASPxTextBox >						
						                    </td>
                                            <td>
                                                <dx:ASPxButton ID="btnNume" ClientInstanceName="btnNume"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  ToolTip="Modificari contract"  RenderMode="Link" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                                    <ClientSideEvents Click="function(s,e){ OnClickDI(s); }" />
                                                    <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                                    <Paddings PaddingLeft="10px"/>
                                                </dx:ASPxButton>
                                            </td>
                                            <td>
                                                <dx:ASPxButton ID="btnNumeIst" ClientInstanceName="btnNumeIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                                    <ClientSideEvents Click="function(s,e){ GoToIstoricDI(s); }" />
                                                    <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                                    <Paddings PaddingLeft="10px"/>
                                                </dx:ASPxButton>
                                            </td>
					                    </tr>
					                    <tr>				
						                    <td>		
							                    <dx:ASPxLabel  ID="lblPrenume" runat="server"  Text="Prenume"></dx:ASPxLabel >	
						                    </td>
						                    <td>	
							                    <dx:ASPxTextBox  ID="txtPrenume" runat="server" ClientInstanceName="txtPrenume"  Text='<%# Eval("F10009") %>'  AutoPostBack="false"  >
							                    </dx:ASPxTextBox >										
						                    </td>
                                            <td>
                                                <dx:ASPxButton ID="btnPrenume" ClientInstanceName="btnPrenume" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                                    <ClientSideEvents Click="function(s,e){ OnClickDI(s); }" />
                                                    <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                                    <Paddings PaddingLeft="10px"/>
                                                </dx:ASPxButton>
                                            </td>
                                            <td>
                                                <dx:ASPxButton ID="btnPrenumeIst" ClientInstanceName="btnPrenumeIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                                    <ClientSideEvents Click="function(s,e){ GoToIstoricDI(s); }" />
                                                    <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                                    <Paddings PaddingLeft="10px"/>
                                                </dx:ASPxButton>
                                            </td>
					                    </tr>
					                    <tr>				
						                    <td>	
							                    <dx:ASPxLabel  ID="lblNumeAnt" runat="server" Text="Nume anterior"></dx:ASPxLabel >	
						                    </td>
						                    <td>		
							                    <dx:ASPxTextBox  ID="txtNumeAnt" runat="server" Text='<%# Eval("F100905") %>'  AutoPostBack="false" >
							                    </dx:ASPxTextBox >						
						                    </td>
					                    </tr>
					                    <tr>				
						                    <td>		
							                    <dx:ASPxLabel  ID="lblDataModifNume" runat="server"  Text="Data modificare nume"></dx:ASPxLabel >	
						                    </td>
						                    <td>	
							                    <dx:ASPxDateEdit  ID="deDataModifNume" Width="100" runat="server"  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100906") %>' AutoPostBack="false"  >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />
							                    </dx:ASPxDateEdit>										
						                    </td>
					                    </tr>   
					                    <tr>				
						                    <td >
							                    <dx:ASPxLabel  ID="lblStareCivila" Width="100" runat="server"  Text="Stare civila" ></dx:ASPxLabel >	
						                    </td>	
						                    <td>
							                    <dx:ASPxComboBox DataSourceID="dsStareCivila"   Value='<%#Eval("F10046") %>' ID="cmbStareCivila"   runat="server" DropDownStyle="DropDown"  TextField="F71004" ValueField="F71002" AutoPostBack="false"  ValueType="System.Int32" >
							                    </dx:ASPxComboBox>
						                    </td>
					                    </tr>                                     	
                    				
				                    </table>
                                    <asp:ObjectDataSource runat="server" ID="dsStareCivila" TypeName="WizOne.Module.General" SelectMethod="GetStareCivila"/>
			                        </fieldset>
               
                                        </ItemTemplate>
                                    </asp:DataList>
                                </td> 
                            </tr>
			            </table>

            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
</body>		













