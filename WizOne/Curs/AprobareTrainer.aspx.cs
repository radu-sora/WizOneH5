using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
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

namespace WizOne.Curs
{
    public partial class AprobareTrainer : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");                
                btnFinalizare.Text = Dami.TraduCuvant("btnFinalizare", "Finalizare sesiune");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
             
                
                foreach (GridViewColumn c in grDate.Columns)
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

                txtTitlu.Text = General.VarSession("Titlu").ToString();


                DataTable dtCurs = GetCursTraineri(Convert.ToInt32(Session["UserId"].ToString()));
                cmbCurs.DataSource = dtCurs;
                cmbCurs.DataBind();     
         

                DataTable dtNote = GetCurs_ValoriNoteXCategorie(Convert.ToInt32(cmbCurs.Value ?? -99), Convert.ToInt32(cmbSesiune.Value ?? -99));
                GridViewDataComboBoxColumn colNote = (grDate.Columns["IdCategValoareNota"] as GridViewDataComboBoxColumn);
                colNote.PropertiesComboBox.DataSource = dtNote;

                DataTable dtAng = General.GetF100NumeComplet();
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                if (IsPostBack)
                {
                    DataTable dt = Session["AprobareTrainer_Grid"] as DataTable;
                    grDate.DataSource = dt;
                    grDate.DataBind();
                 
                }
                else
                {
                    DataTable dtSes = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\"", null);
                    Session["AprobareTrainer_Sesiuni"] = dtSes;                   

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //IncarcaGrid();
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
                if (cmbCurs.Value == null || cmbSesiune.Value == null)
                {
                    MessageBox.Show("Lipsesc date !", MessageBox.icoInfo, "Atentie !");
                    return;
                }

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
                cmbCurs.Value = null;
                cmbSesiune.Value = null;

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbCurs.Value == null || cmbSesiune.Value == null)
                {
                    MessageBox.Show("Lipsesc date !", MessageBox.icoInfo, "Atentie !");
                    return;
                }
                DataTable dt = Session["AprobareTrainer_Grid"] as DataTable;
                General.SalveazaDate(dt, "Curs_Inregistrare");
                
                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AprobareTrainer_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["AprobareTrainer_Grid"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnFinalizare_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbCurs.Value == null || cmbSesiune.Value == null)
                {
                    MessageBox.Show("Lipsesc date !", MessageBox.icoInfo, "Atentie !");
                    return;
                }

                DataTable dtSes = Session["AprobareTrainer_Sesiuni"] as DataTable;
                int idStare = 0;
                if (dtSes != null)
                {
                    for (int i = 0; i < dtSes.Select("IdCurs = " + Convert.ToInt32(cmbCurs.Value ?? -99)).Count(); i++)
                    {
                        if (Convert.ToInt32(dtSes.Select("IdCurs = " + Convert.ToInt32(cmbCurs.Value ?? -99))[i]["Id"].ToString()) == Convert.ToInt32(cmbSesiune.Value ?? -99))
                            idStare = Convert.ToInt32(dtSes.Select("IdCurs = " + Convert.ToInt32(cmbCurs.Value ?? -99))[i]["IdStare"].ToString());                          
                    }
                    switch (idStare)
                    {
                        case 4:
                            MessageBox.Show(Dami.TraduCuvant("Sesiune finalizata !"), MessageBox.icoInfo, "Atentie !");
                            return;
                        case 5:
                            MessageBox.Show(Dami.TraduCuvant("Sesiune anulata !"), MessageBox.icoInfo, "Atentie !");
                            return;
                    }
                }

                string strSql = @"UPDATE ""Curs_tblCursSesiune"" SET ""IdStare""=4 WHERE ""IdCurs""={0} AND ""Id""={1};";
                strSql += @"UPDATE ""Curs_tblCurs"" SET ""Activ""=0 WHERE (SELECT Count(*) FROM ""Curs_tblCursSesiune"" WHERE ""IdCurs""={0} AND ""IdStare"" NOT IN (4,5)) = 0 AND Id = {0};";
                strSql = "BEGIN " + strSql + " END;";

                strSql = string.Format(strSql, Convert.ToInt32(cmbCurs.Value ?? -99), Convert.ToInt32(cmbSesiune.Value ?? -99));
                General.ExecutaNonQuery(strSql, null);

                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo);
                lblStare.InnerText = "Sesiune finalizata";

                IncarcaGrid();

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                
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
                grDate.KeyFieldName = "IdAuto";
                
                dt = GetCurs_InregistrareAprobare(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbCurs.Value ?? -99), Convert.ToInt32(cmbSesiune.Value ?? -99));

                DataTable dtNote = GetCurs_ValoriNoteXCategorie(Convert.ToInt32(cmbCurs.Value ?? -99), Convert.ToInt32(cmbSesiune.Value ?? -99));
                GridViewDataComboBoxColumn colNote= (grDate.Columns["IdCategValoareNota"] as GridViewDataComboBoxColumn);
                colNote.PropertiesComboBox.DataSource = dtNote;

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["AprobareTrainer_Grid"] = dt;
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



        public DataTable GetCurs_InregistrareAprobare(int idUser, int idCurs, int idSesiune)
        {
            /*
             * join b in this.ObjectContext.Curs_tblCursSesiune on a.IdSesiune equals b.Id into all
                         from al in all.DefaultIfEmpty()*/
            /*LeonardM 01.09.2015
             * pentru feedback trainer
             * se afiseaza doar inregistrarile care au eListaAsteptare = 0 , adica doar cei care nu sunt in lista asteptare*/
            DataTable q = null;
            DataTable dtParam = General.IncarcaDT("SELECT * FROM \"tblParametrii\" WHERE \"Nume\" = 'hasNomenclatorTraineri'", null);
            if (dtParam != null && dtParam.Rows.Count > 0)
            {
                if (dtParam.Rows[0]["Valoare"].ToString() == "1")
                {
                    string sql = "SELECT a.* FROM \"Curs_Inregistrare\" a "
                                + " JOIN \"Curs_tblCursSesiune\" b on a.\"IdCurs\" = b.\"IdCurs\" and a.\"IdSesiune\" = b.\"Id\" "
                                + " JOIN \"Curs_relSesiuneTrainer\" c on a.\"IdCurs\" = c.\"IdCurs\" and a.\"IdSesiune\" = b.\"Id\"  "
                                + " WHERE c.\"IdTrainer\" = " + idUser + " and a.\"IdCurs\" = " + idCurs + " and a.\"IdSesiune\" = " + idSesiune + " and a.\"eListaAsteptare\" = 0 and a.\"IdStare\" > 0 ORDER BY a.F10003";

                    q = General.IncarcaDT(sql, null);
                }
            }
            else
            {
                string sql = "SELECT a.* FROM \"Curs_Inregistrare\" a "
                            + " JOIN \"Curs_tblCursSesiune\" b on a.\"IdCurs\" = b.\"IdCurs\" and a.\"IdSesiune\" = b.\"Id\" "
                            + " WHERE b.\"TrainerId\" = " + idUser + " and a.\"IdCurs\" = " + idCurs + " and a.\"IdSesiune\" = " + idSesiune + " and a.\"eListaAsteptare\" = 0 and a.\"IdStare\" > 0 ORDER BY a.F10003";

                q = General.IncarcaDT(sql, null);
            }
            return q;
        }

        public DataTable GetCurs_ValoriNoteXCategorie(int idCurs, int idSesiune)
        {
            try
            {
                string sql = "SELECT d.* FROM \"Curs_tblCursSesiune\" a "
                            + " JOIN \"Curs_tblCurs\" b on a.\"IdCurs\" = b.\"Id\" "
                            + " LEFT JOIN \"Curs_tblCategoriiNote\" c on b.\"IdCategorieNota\" = c.\"IdCategorie\" "
                            + " LEFT JOIN \"Curs_tblCategNoteValori\" d on c.\"IdCategorie\" = d.\"IdCategorie\" "
                            + " WHERE a.\"IdCurs\" = " + idCurs + " AND a.\"Id\" = " + idSesiune + " ORDER BY d.\"DenumireValoare\"";

                DataTable q = General.IncarcaDT(sql, null);

                if (q == null || q.Rows.Count == 0)
                    return null;
                else
                    return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }




      

        public DataTable GetCursTraineri(int IdUser)
        {
            DataTable q = null;

            try
            {
                int hasNomenclatorTraineri = 0;
                hasNomenclatorTraineri = Convert.ToInt32(Dami.ValoareParam("hasNomenclatorTraineri", "0"));

                string strSql = string.Empty;
                if (hasNomenclatorTraineri == 0)
                    strSql = @"SELECT distinct A.* FROM ""Curs_tblCurs"" A
                                INNER JOIN ""Curs_tblCursSesiune"" B ON A.""Id"" = B.""IdCurs""
                                WHERE  COALESCE(A.""Activ"",0)=1 AND B.""IdStare"" <> 5 AND B.""TrainerId""={0}";
                else
                    strSql = @"SELECT distinct A.* FROM ""Curs_tblCurs"" A
                                INNER JOIN ""Curs_tblCursSesiune"" B ON A.""Id"" = B.""IdCurs""
                                INNER JOIN ""Curs_relSesiuneTrainer"" C ON A.""Id"" = C.""IdCurs""
                                                                       AND B.""Id"" = C.""IdSesiune""
                                WHERE  COALESCE(A.""Activ"",0)=1 AND B.""IdStare"" <> 5 AND C.""IdTrainer""={0}";

                strSql = string.Format(strSql, IdUser);

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        protected void cmbSesiune_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                lblStare.InnerText = "";
                DataTable dtSes = Session["AprobareTrainer_Sesiuni"] as DataTable;
                if (dtSes != null)
                {
                    if (e.Parameter != null && e.Parameter.Length > 0)
                    {
                        cmbSesiune.DataSource = dtSes.Select("IdCurs = " + e.Parameter) == null || dtSes.Select("IdCurs = " + e.Parameter).Count() <= 0 ? null : dtSes.Select("IdCurs = " + e.Parameter).CopyToDataTable();
                        cmbSesiune.DataBind();

                        string ses = "";
                        if (dtSes.Select("IdCurs = " + e.Parameter) != null)
                        {
                            for (int i = 0; i < dtSes.Select("IdCurs = " + e.Parameter).Count(); i++)
                            {
                                ses += dtSes.Select("IdCurs = " + e.Parameter).CopyToDataTable().Rows[i]["Id"].ToString() + "," + dtSes.Select("IdCurs = " + e.Parameter).CopyToDataTable().Rows[i]["IdStare"].ToString();
                                if (i < dtSes.Select("IdCurs = " + e.Parameter).Count() - 1)
                                    ses += ";";
                            }
                            Session["AprobareTrainer_SesiuniSir"] = ses;
                        }
                    }
                    else
                    {
                        cmbSesiune.DataSource = null;
                        cmbSesiune.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}