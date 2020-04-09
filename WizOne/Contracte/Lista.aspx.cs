using DevExpress.Web;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Contracte
{
    public partial class Lista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                txtTitlu.Text = (Session["Titlu"] ?? "").ToString();

                if (!IsPostBack)
                {
                    IncarcaGrid();
                }
                else
                {
                    if (grDate.IsCallback)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_Contracte"" ORDER BY ""Id"" ", null);

                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["InformatiaCurenta"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    {                        
                        return;
                    }

                    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "Denumire" }) as object[];

                    int idCtr = Convert.ToInt32(arr[1]);

                    switch (arr[0])
                    {
                        case "btnEdit":
                            {
                                string url = "~/Contracte/Detalii.aspx";
                                if (url != "")
                                {
                                    Session["InformatiaCurenta"] = null;
                                    Session["IdContract"] = idCtr;
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                }
                            }
                            break;
                        case "btnDuplica":
                            {
                                General.ExecutaNonQuery(
                                    $@"BEGIN
                                        INSERT INTO ""Ptj_ContracteSchimburi""(""IdContract"", ""TipSchimb"", ""Denumire"", ""IdProgram"", ""OraInceput"", ""OraInceputDeLa"", ""OraInceputLa"", ""OraSfarsit"", ""OraSfarsitDeLa"", ""OraSfarsitLa"", ""ModVerificare"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Contracte""),0) + 1, ""TipSchimb"", ""Denumire"", ""IdProgram"", ""OraInceput"", ""OraInceputDeLa"", ""OraInceputLa"", ""OraSfarsit"", ""OraSfarsitDeLa"", ""OraSfarsitLa"", ""ModVerificare"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_ContracteSchimburi"" WHERE ""IdContract""=@1;
                                        INSERT INTO ""Ptj_ContracteAbsente""(""IdContract"", ""IdAbsenta"", ZL, SL, S, D, ""InPontajAnual"", ""Camp"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Contracte""),0) + 1, ""IdAbsenta"", ZL, SL, S, D, ""InPontajAnual"", ""Camp"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_ContracteAbsente"" WHERE ""IdContract""=@1;
                                        INSERT INTO ""Ptj_Contracte""(""Id"", ""Denumire"", ""TipContract"", ""OraInSchimbare"", ""OraOutSchimbare"", ""TipSchimb0"", ""Program0"", ""TipSchimb1"", ""Program1"", ""TipSchimb2"", ""Program2"", ""TipSchimb3"", ""Program3"",""TipSchimb4"", ""Program4"", ""TipSchimb5"", ""Program5"", ""TipSchimb6"", ""Program6"", ""TipSchimb7"", ""Program7"", ""TipSchimb8"", ""Program8"", ""OreSup"", ""Afisare"", ""TipRaportareOreNoapte"", ""PontareAutomata"", ""OraInInitializare"", ""OraOutInitializare"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Contracte""),0) + 1, ""Denumire"" {Dami.Operator()} ' - Copie', ""TipContract"", ""OraInSchimbare"", ""OraOutSchimbare"", ""TipSchimb0"", ""Program0"", ""TipSchimb1"", ""Program1"", ""TipSchimb2"", ""Program2"", ""TipSchimb3"", ""Program3"",""TipSchimb4"", ""Program4"", ""TipSchimb5"", ""Program5"", ""TipSchimb6"", ""Program6"", ""TipSchimb7"", ""Program7"", ""TipSchimb8"", ""Program8"", ""OreSup"", ""Afisare"", ""TipRaportareOreNoapte"", ""PontareAutomata"", ""OraInInitializare"", ""OraOutInitializare"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_Contracte"" WHERE ""Id""=@1;
                                    END;", new object[] { idCtr});
                                IncarcaGrid();
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces finalizat cu succes");
                            }
                            break;
                        case "btnSterge":
                            {
                                int are = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(""IdContract"") FROM ""Ptj_Intrari"" WHERE ""IdContract""=@1", new object[] { idCtr }));
                                if (are > 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Acest contract a fost deja folosit in pontaj." + Environment.NewLine + "Nu se mai poate sterge.");
                                    return;
                                }

                                string strSql = $@"SELECT CONVERT(nvarchar(10),F10003) + ',' FROM ""F100Contracte"" WHERE ""IdContract""=@1 AND CAST(""DataInceput"" AS DATE) <= {General.CurrentDate()} AND {General.CurrentDate()} <= CAST(""DataSfarsit"" AS DATE) FOR XML PATH ('')";
                                if (Constante.tipBD == 2)
                                    strSql = $@"SELECT LISTAGG(F10003, ',') WITHIN GROUP (ORDER BY F10003) AS Marci FROM ""F100Contracte"" WHERE ""IdContract""=@1 AND CAST(""DataInceput"" AS DATE) <= {General.CurrentDate()} AND {General.CurrentDate()} <= CAST(""DataSfarsit"" AS DATE)";

                                string marci = General.Nz(General.ExecutaScalar(strSql, new object[] { idCtr }), "").ToString();
                                if (marci != "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Urmatoarele marci au atribut acest contract:" + Environment.NewLine + marci);
                                    return;
                                }

                                General.ExecutaNonQuery(
                                    $@"BEGIN
                                        DELETE FROM ""F100Contracte2"" WHERE ""IdContract""=@1;
                                        DELETE FROM ""Ptj_ContracteAbsente"" WHERE ""IdContract""=@1;
                                        DELETE FROM ""Ptj_ContracteSchimburi"" WHERE ""IdContract""=@1;
                                        DELETE FROM ""Ptj_Contracte"" WHERE ""Id""=@1;
                                    END;", new object[] { idCtr });
                                IncarcaGrid();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "~/Contracte/Detalii.aspx";
                if (url != "")
                {
                    Session["IdContract"] = -99;
                    Session["InformatiaCurenta"] = null;
                    if (Page.IsCallback)
                        ASPxWebControl.RedirectOnCallback(url);
                    else
                        Response.Redirect(url, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

    }
}