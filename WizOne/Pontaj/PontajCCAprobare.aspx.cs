using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajCCAprobare : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                
                foreach (GridViewColumn c in grCC.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtAnLuna.Value = DateTime.Now;

                string strAng = $@"SELECT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS ""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                                FROM F100 A
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607";
                DataTable dtAng = General.IncarcaDT(strAng, null);
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

                DataTable dtDpt = General.IncarcaDT(General.SelectDepartamente(), null);
                //GridViewDataComboBoxColumn colDpt = (grCC.Columns["IdDept"] as GridViewDataComboBoxColumn);
                //colDpt.PropertiesComboBox.DataSource = dtDpt;
                cmbDpt.DataSource = dtDpt;
                cmbDpt.DataBind();

                DataTable dtCC = General.IncarcaDT("SELECT F06204, F06205 FROM F062", null);
                //GridViewDataComboBoxColumn colCC = (grCC.Columns["F06204"] as GridViewDataComboBoxColumn);
                //colCC.PropertiesComboBox.DataSource = dtCC;
                cmbCC.DataSource = dtCC;
                cmbCC.DataBind();

                DataTable dtPro = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""tblProiecte"" WHERE ""IdResponsabil""=" + Session["UserId"], null);
                //GridViewDataComboBoxColumn colPro = (grCC.Columns["IdProiect"] as GridViewDataComboBoxColumn);
                //colPro.PropertiesComboBox.DataSource = dtPro;
                cmbPro.DataSource = dtPro;
                cmbPro.DataBind();

                //DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                //GridViewDataComboBoxColumn colStari = (grCC.Columns["IdStare"] as GridViewDataComboBoxColumn);
                //colStari.PropertiesComboBox.DataSource = dtStari;


                if (!IsPostBack)
                {
                    DataTable dtAd = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAdminCC"" ", null);
                    Session["Ptj_tblAdminCC"] = dtAd;
                    for (int i = 0; i < dtAd.Rows.Count; i++)
                    {
                        grCC.Columns[dtAd.Rows[i]["Camp"].ToString()].Visible = Convert.ToBoolean(dtAd.Rows[i]["Vizibil"]);
                    }
                }
                else
                {
                    grCC.DataSource = Session["InformatiaCurenta"];
                    grCC.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltruSterge_Click(object sender, EventArgs e)
        {
            try
            {
                cmbAng.Value = null;
                cmbDpt.Value = null;
                cmbPro.Value = null;
                cmbCC.Value = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                Actiune(1);
                //string msg = "";
                //string ids = "";
                //List<object> lst = grCC.GetSelectedFieldValues(new string[] { "IdAuto", "IdStare", "NumeAngajat" });
                //if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                //for (int i = 0; i < lst.Count(); i++)
                //{
                //    object[] arr = lst[i] as object[];
                //    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                //    {
                //        case 3:
                //            msg += "Pontajul pt " + arr[2] + " " + Dami.TraduCuvant("este deja aprobat") + System.Environment.NewLine;
                //            continue;
                //    }

                //    ids += ", " + arr[0];
                //}

                //if (ids != "")
                //    General.ExecutaNonQuery($@"UPDATE ""Ptj_CC"" SET ""IdStare""= 3 WHERE ""IdAuto"" IN ({ids})", null);

                //grCC.JSProperties["cpAlertMessage"] = msg;
                //grCC.DataBind();

                //grCC.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRespinge_Click(object sender, EventArgs e)
        {
            try
            {
                Actiune(2);
                //string msg = "";
                //string ids = "";
                //List<object> lst = grCC.GetSelectedFieldValues(new string[] { "IdAuto", "IdStare", "NumeAngajat" });
                //if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                //for (int i = 0; i < lst.Count(); i++)
                //{
                //    object[] arr = lst[i] as object[];
                //    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                //    {
                //        case -1:
                //            msg += "Pontajul pt " + arr[2] + " " + Dami.TraduCuvant("este deja respinsa") + System.Environment.NewLine;
                //            continue;
                //    }

                //    ids += ", " + arr[0];
                //}

                //if (ids != "")
                //    General.ExecutaNonQuery($@"UPDATE ""Ptj_CC"" SET ""IdStare""= 0 WHERE ""IdAuto"" IN ({ids})", null);

                //grCC.JSProperties["cpAlertMessage"] = msg;
                //grCC.DataBind();

                //grCC.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void Actiune(int tip)
        {
            //tip = 1  Aprobare
            //tip = 2  Respingere

            try
            {
                string msg = "";
                string ids = "";
                string idStare = "3";
                if (tip == 2) idStare = "0";

                List<object> lst = grCC.GetSelectedFieldValues(new string[] { "IdAuto", "IdStare", "NumeComplet" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case 0:
                            msg += "Pontajul pt " + arr[2] + " " + Dami.TraduCuvant("este deja respins") + System.Environment.NewLine;
                            break;
                        case 3:
                            msg += "Pontajul pt " + arr[2] + " " + Dami.TraduCuvant("este deja aprobat") + System.Environment.NewLine;
                            break;
                        default:
                            if (tip == 1)
                                msg += "Pontajul pt " + arr[2] + " " + Dami.TraduCuvant("a fost aprobat") + System.Environment.NewLine;
                            else
                                msg += "Pontajul pt " + arr[2] + " " + Dami.TraduCuvant("a fost respins") + System.Environment.NewLine;
                            ids += ", " + arr[0];
                            break;
                    }
                }

                if (ids != "")
                    General.ExecutaNonQuery($@"UPDATE ""Ptj_CC"" SET ""IdStare""= {idStare} WHERE ""IdAuto"" IN ({ids.Substring(1)})", null);

                grCC.JSProperties["cpAlertMessage"] = msg;
                //MessageBox.Show(msg, MessageBox.icoInfo, "Pontajul pe centrii de cost");

                IncarcaGrid();
                grCC.Selection.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                string filtru = "";

                DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                filtru = $@" AND {General.ToDataUniv(ziua.Year, ziua.Month)} <= {General.TruncateDate("A.Ziua")} AND {General.TruncateDate("A.Ziua")} <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}";
                if (cmbAng.Value != null) filtru += " AND A.F10003=" + Convert.ToInt32(cmbAng.Value ?? -99);
                if (cmbCC.Value != null) filtru += " AND A.F06204=" + Convert.ToInt32(cmbCC.Value ?? -99);
                if (cmbDpt.Value != null) filtru += " AND A.IdDept=" + Convert.ToInt32(cmbDpt.Value ?? -99);
                if (cmbPro.Value != null) filtru += " AND A.IdProiect=" + Convert.ToInt32(cmbPro.Value ?? -99);

                ////proiectele la care sunt responsabil
                ////union
                ////proiectele care nu au responsabil, deci sunt comune tuturor

                //string strSql = $@"SELECT A.* FROM ""Ptj_CC"" A 
                //                INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                //                WHERE B.""IdResponsabil""= {Session["UserId"]} {filtru}
                //                UNION 
                //                SELECT * FROM ""Ptj_CC"" WHERE ""IdProiect"" IS NULL ";


                string strSql = $@"SELECT A.""IdAuto"", F.F10008 + ' ' + F.F10009 AS ""NumeComplet"", CONVERT(nvarchar(10), A.""Ziua"", 103) AS ""Ziua"", 
                                C.F06205 AS ""F06204"", D.""Denumire"" As ""IdProiect"", E.F00608 As ""IdDept"", 
                                CONVERT(nvarchar(5), A.""De"", 108) AS ""De"", CONVERT(nvarchar(5), A.""La"", 108) AS ""La"", A.""NrOre"", A.""IdStare"", G.""Denumire"" AS ""Stare"", G.""Culoare""
                                FROM ""Ptj_CC"" A 
                                INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                LEFT JOIN F062 C ON A.F06204 = C.F06204
                                LEFT JOIN ""tblProiecte"" D ON A.""IdProiect"" = D.""Id""
                                LEFT JOIN F006 E ON A.IdDept = E.F00607
                                LEFT JOIN F100 F ON A.F10003 = F.F10003
                                LEFT JOIN ""Ptj_tblStari"" G ON A.""IdStare"" = G.""Id""
                                WHERE B.""IdResponsabil"" = {Session["UserId"]} {filtru}";

                dt = General.IncarcaDT(strSql, null);
                Session["InformatiaCurenta"] = dt;
                grCC.KeyFieldName = "IdAuto";
                grCC.DataSource = dt;
                grCC.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
            }
        }


        protected void grCC_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "Stare")
                {
                    e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(e.GetValue("Culoare").ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    switch (str)
                    {
                        case "btnAproba":
                            Actiune(1);
                            break;
                        case "btnRespinge":
                            Actiune(2);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}