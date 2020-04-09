using DevExpress.Web;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Programe
{
    public partial class Lista : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
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
                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_Programe"" ORDER BY ""Id""", null);

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

                    int idPrg = Convert.ToInt32(arr[1]);

                    switch (arr[0])
                    {
                        case "btnEdit":
                            {
                                string url = "~/Programe/Detalii.aspx";
                                if (url != "")
                                {
                                    Session["InformatiaCurenta"] = null;
                                    Session["IdProgram"] = arr[1];
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
                                        INSERT INTO ""Ptj_ProgrameTrepte""(""IdProgram"", ""TipInOut"", ""OraInceput"", ""OraSfarsit"", ""Valoare"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Programe""),0) + 1, ""TipInOut"", ""OraInceput"", ""OraSfarsit"", ""Valoare"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_ProgrameTrepte"" WHERE ""IdProgram""=@1;
                                        INSERT INTO ""Ptj_ProgramePauza""(""IdProgram"", ""OraInceput"", ""OraInceputDeLa"", ""OraInceputLa"", ""OraSfarist"", ""OraSfaristDeLa"", ""OraSfaristLa"", ""TimpDedus"", ""TimpMin"", ""TimpMax"", ""FaraMarja"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Programe""),0) + 1, ""OraInceput"", ""OraInceputDeLa"", ""OraInceputLa"", ""OraSfarsit"", ""OraSfarsitDeLa"", ""OraSfarsitLa"", ""TimpDedus"", ""TimpMin"", ""TimpMax"", ""FaraMarja"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_ProgramePauza"" WHERE ""IdPRogram""=@1;
                                        INSERT INTO ""Ptj_ProgrameOreNoapte""(""IdProgram"", ""Rotunjire"", ""OraINceput"", ""OraSfarsit"", ""ValMin"", ""ValMax"", ""ValFixa"", ""Multiplicator"", ""Camp"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Programe""),0) + 1, ""Rotunjire"", ""OraInceput"", ""OraSfarsit"", ""ValMin"", ""ValMax"", ""ValFixa"", ""Multiplicator"", ""Camp"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_ProgrameOreNoapte"" WHERE ""IdPRogram""=@1;
                                        INSERT INTO ""Ptj_ProgrameAlteOre""(""IdProgram"", ""Rotunjire"", ""OraINceput"", ""OraSfarsit"", ""ValMin"", ""ValMax"", ""ValFixa"", ""Multiplicator"", ""Camp"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Programe""),0) + 1, ""Rotunjire"", ""OraInceput"", ""OraSfarsit"", ""ValMin"", ""ValMax"", ""ValFixa"", ""Multiplicator"", ""Camp"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_ProgrameAlteOre"" WHERE ""IdPRogram""=@1;
                                        INSERT INTO ""Ptj_Programe""(""Id"", ""Denumire"", ""DataInceput"", ""DataSfarsit"", ""OraIntrare"", ""OraIesire"", ""Norma"", ""TipPontare"", ""ONRotunjire"", ""ONCamp"", ""OSRotunjire"", ""OSValMin"", ""OSValMax"", ""OSCamp"", ""OSCampSub"", ""OSCampPeste"", ""INSubDiferentaRaportare"", ""INSubMinPlata"", ""INSubMaxPlata"", ""INSubCampPlatit"", ""INSubCampNeplatit"", ""INPesteDiferentaRaportare"", ""INPesteDiferentaPlata"", ""INPesteCampPlatit"", ""INPesteCampNeplatit"", ""OUTSubDiferentaRaportare"", ""OUTSubDiferentaPlata"", ""OUTSubCampPlatit"", ""OUTSubCampNeplatit"", ""OUTPesteDiferentaRaportare"", ""OUTPesteMinPlata"", ""OUTPesteMaxPlata"", ""OUTPesteCampPlatit"", ""OUTPesteCampNeplatit"", ""PauzaTimp"", ""PauzaDedusa"", ""DeNoapte"", ""Flexibil"", ""PauzaMin"", ""ONValStr"", ""VAL2"", ""VAL1"", ""VAL4"", ""VAL3"", ""VAL0"", ""VAL4ROT"", ""VAL3ROT"", ""Val2Rot"", ""VAL0ROT"", ""VAL1ROT"", ""PauzaScutita"", ""OreLucrateMin"", ""TipRaportareOreNoapte"", ""DenumireScurta"", USER_NO, TIME)
                                        SELECT COALESCE((SELECT MAX(""Id"") FROM ""Ptj_Programe""),0) + 1, ""Denumire"", ""DataInceput"", ""DataSfarsit"", ""OraIntrare"", ""OraIesire"", ""Norma"", ""TipPontare"", ""ONRotunjire"", ""ONCamp"", ""OSRotunjire"", ""OSValMin"", ""OSValMax"", ""OSCamp"", ""OSCampSub"", ""OSCampPeste"", ""INSubDiferentaRaportare"", ""INSubMinPlata"", ""INSubMaxPlata"", ""INSubCampPlatit"", ""INSubCampNeplatit"", ""INPesteDiferentaRaportare"", ""INPesteDiferentaPlata"", ""INPesteCampPlatit"", ""INPesteCampNeplatit"", ""OUTSubDiferentaRaportare"", ""OUTSubDiferentaPlata"", ""OUTSubCampPlatit"", ""OUTSubCampNeplatit"", ""OUTPesteDiferentaRaportare"", ""OUTPesteMinPlata"", ""OUTPesteMaxPlata"", ""OUTPesteCampPlatit"", ""OUTPesteCampNeplatit"", ""PauzaTimp"", ""PauzaDedusa"", ""DeNoapte"", ""Flexibil"", ""PauzaMin"", ""ONValStr"", ""VAL2"", ""VAL1"", ""VAL4"", ""VAL3"", ""VAL0"", ""VAL4ROT"", ""VAL3ROT"", ""Val2Rot"", ""VAL0ROT"", ""VAL1ROT"", ""PauzaScutita"", ""OreLucrateMin"", ""TipRaportareOreNoapte"", ""DenumireScurta"", {Session["UserId"]}, {General.CurrentDate()} FROM ""Ptj_Programe"" WHERE ""Id""=@1;
                                    END;", new object[] { idPrg });
                                IncarcaGrid();
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces finalizat cu succes");
                            }
                            break;
                        case "btnSterge":
                            {
                                int are = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(""IdProgram"") FROM ""Ptj_Intrari"" WHERE ""IdProgram""=@1", new object[] { idPrg }));
                                if (are > 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Acest program a fost deja folosit in pontaj." + Environment.NewLine + "Nu se mai poate sterge.");
                                    return;
                                }

                                string strSql = $@"SELECT CONVERT(nvarchar(10),""IdContract"") + ',' FROM ""Ptj_ContracteSchimburi"" WHERE ""IdProgram""=@1 FOR XML PATH ('')";
                                if (Constante.tipBD == 2)
                                    strSql = $@"SELECT LISTAGG(""IdContract"", ',') WITHIN GROUP (ORDER BY ""IdContract"") AS Contracte FROM ""Ptj_ContracteSchimburi"" WHERE ""IdProgram""=@1";

                                string ctrs = General.Nz(General.ExecutaScalar(strSql, new object[] { idPrg }), "").ToString();
                                if (ctrs != "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Urmatoarele contracte au atribut acest program:" + Environment.NewLine + ctrs);
                                    return;
                                }

                                General.ExecutaNonQuery(
                                    $@"BEGIN
                                        DELETE FROM ""Ptj_ProgrameAlteOre"" WHERE ""IdProgram""=@1;
                                        DELETE FROM ""Ptj_ProgrameOreNoapte"" WHERE ""IdProgram""=@1;
                                        DELETE FROM ""Ptj_ProgramePauza"" WHERE ""IdProgram""=@1;
                                        DELETE FROM ""Ptj_ProgrameTrepte"" WHERE ""IdProgram""=@1;
                                        DELETE FROM ""Ptj_Programe"" WHERE ""Id""=@1;
                                    END;", new object[] { idPrg });
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
                string url = "~/Programe/Detalii.aspx";
                if (url != "")
                {
                    Session["IdProgram"] = -99;
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