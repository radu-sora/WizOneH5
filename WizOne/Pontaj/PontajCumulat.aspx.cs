using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajCumulat : System.Web.UI.Page
    {


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Radu 28.11.2019 - se inlocuieste Ptj_AliasF cu Ptj_tblAdmin
                    //DataTable dt = General.IncarcaDT(@"SELECT CS.*, COALESCE(AF.ALIAS,CS.COLOANA) ""Caption"" 
                    //                                    FROM ""Ptj_CumulatSetari"" CS LEFT JOIN ""Ptj_AliasF"" AF ON CS.""Coloana"" = AF.""Denumire""
                    //                                    ORDER BY CS.""Ordine""  ", null);
                    DataTable dt = General.IncarcaDT(@"SELECT CS.*, COALESCE(AF.""Alias"",CS.""Coloana"") ""Caption"",  coalesce(af.""AliasToolTip"", coalesce(AF.""Alias"",CS.""Coloana"")) ""ToolTip"" 
                                                        FROM ""Ptj_CumulatSetari"" CS LEFT JOIN ""Ptj_tblAdmin"" AF ON CS.""Coloana"" = AF.""Coloana""
                                                        ORDER BY CS.""Ordine""  ", null);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GridViewDataSpinEditColumn c = new GridViewDataSpinEditColumn();
                        c.Name = "col" + i;
                        c.FieldName = dt.Rows[i]["Coloana"].ToString();
                        c.Caption = Dami.TraduCuvant(dt.Rows[i]["Caption"].ToString());
                        c.ToolTip = Dami.TraduCuvant(dt.Rows[i]["ToolTip"].ToString());
                        c.ReadOnly = true;
                        //c.Width = Unit.Pixel(100);
                        c.VisibleIndex = 100 + i;

                        //Florin 2019.06.24
                        ////c.ReadOnly = Convert.ToBoolean(dt.Rows[i]["Editabil"] ?? 0);
                        //c.ReadOnly = Convert.ToBoolean(dt.Rows[i]["Editabil"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[i]["Editabil"].ToString()));
                        c.ReadOnly = !Convert.ToBoolean(General.Nz(dt.Rows[i]["Editabil"], 0));

                        c.PropertiesSpinEdit.MaxLength = 10;
                        c.PropertiesSpinEdit.NumberFormat = SpinEditNumberFormat.Number;
                        c.PropertiesSpinEdit.DisplayFormatString = "N0";
                        c.PropertiesSpinEdit.DisplayFormatInEditMode = true;

                        grDate.Columns.Add(c);

                        ASPxSummaryItem s = new ASPxSummaryItem();
                        s.FieldName = dt.Rows[i]["Coloana"].ToString();
                        s.SummaryType = DevExpress.Data.SummaryItemType.Sum;

                        grDate.TotalSummary.Add(s);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                //Radu 09.12.2019
                lblAnLuna.InnerText = Dami.TraduCuvant("Luna/An");
                lblRol.InnerText = Dami.TraduCuvant("Roluri");
                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblCtr.InnerText = Dami.TraduCuvant("Contract");
                lblSub.InnerText = Dami.TraduCuvant("Subcomp.");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectie");
                lblDept.InnerText = Dami.TraduCuvant("Dept.");
                lblSubDept.InnerText = Dami.TraduCuvant("Subdept.");
                lblBirou.InnerText = Dami.TraduCuvant("Birou");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;

                    txtAnLuna.Value = DateTime.Now;

                    string sqlRol = $@"select a.""Id"", a.""Denumire"", case when a.""PoateInitializa"" = 1 then 1 else 0 end as ""PoateInitializa"", case when a.""PoateSterge"" = 1 then 1 else 0 end as ""PoateSterge"", COALESCE(a.""TipMesaj"",1) AS ""TipMesaj"" 
                                 from ""Ptj_tblRoluri"" a 
                                 inner join ""Ptj_relGrupSuper"" b on a.""Id"" = b.""IdRol"" 
                                 where b.""IdSuper"" =  {Session["UserId"]}
                                 group by a.""Id"", a.""Denumire"", a.""PoateInitializa"",a.""PoateSterge"", a.""TipMesaj"" 
                                 union 
                                 select a.""Id"", a.""Denumire"", case when a.""PoateInitializa"" = 1 then 1 else 0 end as ""PoateInitializa"", case when a.""PoateSterge"" = 1 then 1 else 0 end as ""PoateSterge"", COALESCE(a.""TipMesaj"",1) AS ""TipMesaj"" 
                                 from ""Ptj_tblRoluri"" a 
                                 inner join ""Ptj_relGrupSuper"" b on a.""Id"" = b.""IdRol"" 
                                 inner join ""relGrupAngajat"" c on b.""IdGrup""  = c.""IdGrup"" 
                                 inner join ""F100Supervizori"" d on c.F10003 = d.F10003 and (-1 * b.""IdSuper"")= d.""IdSuper"" 
                                 where d.""IdUser"" =  {Session["UserId"]}
                                 group by a.""Id"", a.""Denumire"", a.""PoateInitializa"",a.""PoateSterge"", a.""TipMesaj"" ";
                    DataTable dtRol = General.IncarcaDT(sqlRol, null);
                    cmbRol.DataSource = dtRol;
                    cmbRol.DataBind();
                    if (dtRol != null && dtRol.Rows.Count > 0) cmbRol.Value = dtRol.Rows[0]["Id"];

                }
                else
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();

                }

                cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003", null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                cmbDept.DataBind();
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT("SELECT F00809, F00810 FROM F008", null);
                cmbBirou.DataBind();

                cmbStare.DataSource = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblStariPontaj"" ", null);
                cmbStare.DataBind();

                cmbCtr.DataSource = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Contracte"" ", null);
                cmbCtr.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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

        //Florin 2019.12.10 - rescrisa
        protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                string mesaj = "";

                grDate.CancelEdit();

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                string ids = "";

                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;

                    string msg = Notif.TrimiteNotificare("Pontaj.PontajCumulat", (int)Constante.TipNotificare.Validare, @"SELECT * FROM ""Ptj_Cumulat"" WHERE F10003= " + upd.NewValues["F10003"] + @" AND ""An""=" + txtAnLuna.Date.Year + @" AND ""Luna""=" + txtAnLuna.Date.Month, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                    if (msg != "" && msg.Substring(0, 1) == "2")
                    {
                        mesaj += msg.Substring(2) + Environment.NewLine;
                        continue;
                    }
                    else
                        mesaj += upd.NewValues["F10003"] + " - proces realizat cu succes" + Environment.NewLine;

                    object[] keys = new object[] { upd.Keys[0] };

                    DataRow row = dt.Rows.Find(keys);
                    if (row == null) continue;

                    row["USER_NO"] = Session["UserId"];
                    row["TIME"] = DateTime.Now;

                    foreach (DictionaryEntry de in upd.NewValues)
                    {
                        var ert = de.Key.ToString();
                        if (Constante.lstFuri.IndexOf(de.Key.ToString() + ";") >= 0)
                        {
                            if (upd.NewValues[de.Key.ToString()] != null)
                                row[de.Key.ToString()] = Convert.ToDecimal(upd.NewValues[de.Key.ToString()]);
                            else
                                row[de.Key.ToString()] = DBNull.Value;
                        }
                    }

                    ids += upd.NewValues["F10003"] + ";";
                }

                if (dt.GetChanges() != null && ((DataTable)dt.GetChanges()).Rows.Count > 0)
                {
                    General.SalveazaDate(dt, "Ptj_Cumulat");

                    string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };

                    string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        General.CalculFormuleCumulat(Convert.ToInt32(arr[i]), txtAnLuna.Date.Year, txtAnLuna.Date.Month);

                        Notif.TrimiteNotificare("Pontaj.PontajCumulat", (int)Constante.TipNotificare.Notificare, @"SELECT * FROM ""Ptj_Cumulat"" WHERE F10003= " + Convert.ToInt32(arr[i]) + @" AND ""An""=" + txtAnLuna.Date.Year + @" AND ""Luna""=" + txtAnLuna.Date.Month, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99) );
                    }

                    grDate.JSProperties["cpAlertMessage"] = mesaj;

                    Session["InformatiaCurenta"] = dt;
                }
                else
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista modificari");

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        //{
        //    try
        //    {
        //        grDate.CancelEdit();

        //        DataTable dt = Session["InformatiaCurenta"] as DataTable;
        //        string ids = "";

        //        for (int x = 0; x < e.UpdateValues.Count; x++)
        //        {
        //            ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
        //            object[] keys = new object[] { upd.Keys[0] };

        //            DataRow row = dt.Rows.Find(keys);
        //            if (row == null) continue;

        //            row["USER_NO"] = Session["UserId"];
        //            row["TIME"] = DateTime.Now;

        //            foreach (DictionaryEntry de in upd.NewValues)
        //            {
        //                if (Constante.lstFuri.IndexOf(de.Key.ToString() + ";") >= 0)
        //                {
        //                    row[de.Key.ToString()] = upd.NewValues[de.Key.ToString()] ?? DBNull.Value;
        //                }
        //            }

        //            ids += upd.NewValues["F10003"] + ";";
        //        }

        //        if (dt.GetChanges() != null && ((DataTable)dt.GetChanges()).Rows.Count > 0)
        //        {
        //            General.SalveazaDate(dt, "Ptj_Cumulat");

        //            string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        //            for (int i = 0; i < arr.Length; i++)
        //            {
        //                General.CalculFormuleCumulat(Convert.ToInt32(arr[i]), txtAnLuna.Date.Year, txtAnLuna.Date.Month);
        //            }

        //            MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
        //        }
        //        else
        //            MessageBox.Show("Nu exista modificari", MessageBox.icoInfo);


        //        e.Handled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                switch (e.Parameter)
                {
                    case "cmbSub":
                        cmbFil.Value = null;
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbFil":
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbSec":
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbDept":
                        cmbSubDept.Value = null;
                        break;
                }

                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                cmbDept.DataBind();
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                //Actualizam nume complet
                General.ExecutaNonQuery($@"UPDATE ""Ptj_Cumulat"" 
                                        SET ""NumeComplet""=(SELECT X.F10008 {Dami.Operator()} ' ' {Dami.Operator()} X.F10009 FROM F100 X WHERE X.F10003=""Ptj_Cumulat"".F10003) 
                                        WHERE ""NumeComplet"" IS NULL", null);

                grDate.KeyFieldName = "IdAuto";

                DataTable dt = General.IncarcaDT(SelectGrid(), null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string SelectGrid()
        {
            string strSql = "";

            try
            {
                string strFiltru = "";

                int an = Convert.ToDateTime(txtAnLuna.Value).Year;
                int luna = Convert.ToDateTime(txtAnLuna.Value).Month;

                //se obtin angajatii din subordine in acelasi mod ca in pontajul echipei (functia srvPontaj.PontajAfiseaza)
                strFiltru += @" AND C.""An""=" + an;
                strFiltru += @" AND C.""Luna""=" + luna;
                if (Convert.ToInt32(cmbStare.Value ?? -99) != -99) strFiltru += @" AND COALESCE(C.""IdStare"",1) = " + cmbStare.Value;
                if (Convert.ToInt32(cmbCtr.Value ?? -99) != -99) strFiltru += @" AND D.""IdContract"" = " + cmbCtr.Value;

                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;
                if (Convert.ToInt32(cmbDept.Value ?? -99) != -99) strFiltru += " AND A.F10007 = " + cmbDept.Value;
                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99) strFiltru += " AND B.F100958 = " + cmbSubDept.Value;
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99) strFiltru += " AND B.F100959 = " + cmbBirou.Value;

                strFiltru += General.GetF10003Roluri(Convert.ToInt32(Session["UserId"]), an, luna, 0, -99, Convert.ToInt32(cmbRol.Value ?? -99), 0, Convert.ToInt32(cmbDept.Value ?? -99), -99);

                //Florin 2019.12.09 - adaugam drepturile
                int idRol = Convert.ToInt32(General.Nz(cmbRol.Value, 1));
                strSql = $@"SELECT C.*, 
                        CASE WHEN ({idRol} = 0 AND (C.""IdStare"" = 1 OR C.""IdStare"" = 4)) OR 
					    ({idRol} = 1 AND (C.""IdStare"" = 1 OR C.""IdStare"" = 4)) OR 
					    ({idRol} = 2 AND (C.""IdStare"" = 1 OR C.""IdStare"" = 2 OR C.""IdStare"" = 4 OR C.""IdStare"" = 6)) OR 
					    {idRol} = 3 THEN 1 ELSE 0 END ""DrepturiModif""
                        FROM ""Ptj_Cumulat"" C
                        INNER JOIN F100 A ON C.F10003=A.F10003
                        INNER JOIN F1001 B ON A.F10003=B.F10003
                        LEFT JOIN (SELECT X.F10003, MAX(X.""IdContract"") AS ""IdContract"" FROM ""F100Contracte"" X WHERE {General.TruncateDate("X.DataInceput")} <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= {General.TruncateDate("X.DataSfarsit")} GROUP BY X.F10003) D ON A.F10003=D.F10003
                        WHERE 1=1 {strFiltru}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void grDate_DataBound(object sender, EventArgs e)
        {
            try
            {
                var lstDrepturi = new Dictionary<int, int>();

                var grid = sender as ASPxGridView;
                for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
                {
                    var rowValues = grid.GetRowValues(i, new string[] { "IdAuto", "DrepturiModif" }) as object[];
                    lstDrepturi.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), Convert.ToInt32(rowValues[1] ?? 0));
                }

                grid.JSProperties["cp_cellsDrepturi"] = lstDrepturi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}