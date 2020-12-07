using DevExpress.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class EvalMultiplu : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //DevExpress.Web.ASPxWebControl.GlobalThemeBaseColor = "#F78119";
            //DevExpress.Web.ASPxWebControl.GlobalTheme = "MaterialCompact";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();
                Session["PaginaWeb"] = "Eval.EvalMultiplu";

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LagSelectorPopup") >= 0)
                    Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                
                #endregion

                if (Request["pp"] != null)
                    txtTitlu.Text = Dami.TraduCuvant("Prima Pagina - Evaluare");
                else
                    txtTitlu.Text = General.VarSession("Titlu").ToString();


                DataTable dt = General.IncarcaDT(
                    $@"SELECT DISTINCT B.""SelectQuery""
                    FROM ""Eval_relGrupSetAngajati"" A
                    INNER JOIN ""Eval_SetAngajati"" B ON A.""IdSetAngajati"" = B.""IdSetAng""
                    INNER JOIN ""relGrupUser"" C ON A.""IdGrup"" = C.""IdGrup""
                    WHERE C.""IdUser""={Session["UserId"]}");

                string strSql = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (General.Nz(dt.Rows[i]["SelectQuery"], "").ToString() != "")
                        strSql += " UNION " + dt.Rows[i]["SelectQuery"];
                }

                if (strSql != "")
                {
                    DataTable dtAng = General.IncarcaDT(strSql.Substring(7));
                    cmbAng.DataSource = dtAng;
                    cmbAng.DataBind();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        #region Varianta A

        protected void pnlSec_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                DataTable dt = General.IncarcaDT(
                    $@"SELECT B.""Sectiune"", B.""Subsectiune"", comp.""IdCategorie"", comp.""DenCategorie"", compDet.""IdCompetenta"", compDet.""DenCompetenta"", B.""IdCalificativ"", D.""Nota"", D.""Valoare""
                    FROM ""Eval_CategCompetente"" comp
                    INNER JOIN ""Eval_CategCompetenteDet"" compDet on compDet.""IdCategorie"" = comp.""IdCategorie""
                    INNER JOIN ""Eval_CompXSetAng"" compDetAng on compDetAng.""IdCategorie"" = comp.""IdCategorie""
                    INNER JOIN ""Eval_SetAngajati"" setAng ON setAng.""IdSetAng"" = compDetAng.""IdSetAng""
                    INNER JOIN ""Eval_SetAngajatiDetail"" setAngDet ON   setAng.""IdSetAng"" = setAngDet.""IdSetAng""
                    LEFT JOIN ""Eval_CategCompetente"" B ON comp.""IdCategorie"" = B.""IdCategorie""
                    LEFT JOIN ""Eval_tblTipValori"" C ON B.""IdCalificativ"" = C.""Id""
                    LEFT JOIN ""Eval_tblTipValoriLinii"" D ON C.""Id"" = D.""Id""
                    WHERE setAngDet.""Id"" = {General.Nz(cmbAng.Value, -99)}
                    GROUP BY B.""Sectiune"", B.""Subsectiune"", comp.""IdCategorie"", comp.""DenCategorie"", compDet.""IdCompetenta"", compDet.""DenCompetenta"", B.""IdCalificativ"", D.""Nota"", D.""Valoare"" ");
                Session["Eval_Multiplu_Angajati"] = dt;

                ctlSec.DataSource = dt.DefaultView.ToTable(true, "Sectiune");
                ctlSec.DataBind();
                ctlSub.DataSource = null;
                ctlSub.DataBind();
                ctlCtg.DataSource = null;
                ctlCtg.DataBind();
                ctlCom.DataSource = null;
                ctlCom.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlSub_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                DataTable dt = Session["Eval_Multiplu_Angajati"] as DataTable;

                ctlSub.DataSource = dt.Select("Sectiune='" + General.Nz(e.Parameter, "-99") + "'").CopyToDataTable().DefaultView.ToTable(true, "Subsectiune");
                ctlSub.DataBind();
                ctlCtg.DataSource = null;
                ctlCtg.DataBind();
                ctlCom.DataSource = null;
                ctlCom.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtg_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                string[] arr = e.Parameter.Split(';');
                if (arr.Length != 2) return;

                DataTable dt = Session["Eval_Multiplu_Angajati"] as DataTable;

                ctlCtg.DataSource = dt.Select("Sectiune='" + General.Nz(arr[0], "-99") + "' AND Subsectiune='" + General.Nz(arr[1], "-99") + "'").CopyToDataTable().DefaultView.ToTable(true, "IdCategorie", "DenCategorie");
                ctlCtg.DataBind();
                ctlCom.DataSource = null;
                ctlCom.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCom_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                string[] arr = e.Parameter.Split(';');
                if (arr.Length < 3) return;
                string filtru = "Sectiune='" + General.Nz(arr[0], "-99") + "' AND Subsectiune='" + General.Nz(arr[1], "-99") + "' AND IdCategorie=" + General.Nz(arr[2], -99);
                DataTable dt = Session["Eval_Multiplu_Angajati"] as DataTable;

                ctlCom.DataSource = dt.Select(filtru).CopyToDataTable().DefaultView.ToTable(true, "IdCompetenta", "DenCompetenta");
                ctlCom.DataBind();

                ctlCal.DataSource = dt.Select(filtru).CopyToDataTable().DefaultView.ToTable(true, "Valoare", "Nota");
                ctlCal.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion


        protected void pmlCallback_Callback(object source, CallbackEventArgs e)
        {
            try
            {
                var obj = JObject.Parse(e.Parameter) as dynamic;

                if ((int?)obj.ang == null || (string)obj.sec == null || (string)obj.sub == null || (int?)obj.ctg == null || (int?)obj.com == null || (string)obj.cal == null)
                {
                    cmbAng.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Date incomplete"); 
                }
                else
                {
                    General.ExecutaNonQuery($@"INSERT INTO ""Eval_RaspunsMultiplu""(F10003, Data, Sectiune, Subsectiune, CategorieCompetentaId, CategorieCompetentaDenumire, CompetentaId, CompetentaDenumire, CalificativId, CalificativDenumire, FunctiaId, FunctiaDenumire, Observatii, IdUser, USER_NO, TIME)
                                            VALUES(@1, {General.CurrentDate()}, @2, @3, @4, @5, @6, @7, @8, @9, (SELECT F10071 FROM F100 WHERE F10003=@1), (SELECT F71804 FROM F100 A INNER JOIN F718 B ON A.F10071=B.F71802 WHERE F10003=@1), @10, {Session["UserId"]}, {Session["UserId"]}, {General.CurrentDate()})",
                                            new object[] { (int?)obj.ang, (string)obj.sec, (string)obj.sub, (int?)obj.ctg, (string)obj.ctgDen, (int?)obj.com, (string)obj.comDen, (string)obj.cal, (string)obj.calDen, (string)obj.obs });
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}