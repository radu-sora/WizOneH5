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
    public partial class CursuriSesiuni : System.Web.UI.Page
    {

        public class metaUploadFile
        {
            public byte[] UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

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

                btnDoc.Image.ToolTip = Dami.TraduCuvant("btnDoc", "Documente");
                btnNomenclatorTraineri.Image.ToolTip = Dami.TraduCuvant("btnNomenclatorTraineri", "Trainer");
                btnNomenclatorCosturiEstimat.Image.ToolTip = Dami.TraduCuvant("btnNomenclatorCosturiEstimat", "Cost estimat");
                btnNomenclatorCosturiEfectiv.Image.ToolTip = Dami.TraduCuvant("btnNomenclatorCosturiEfectiv", "Cost efectiv");

                lblCurs.InnerText = Dami.TraduCuvant("Curs");
                

                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn coloana = c as GridViewDataColumn;
                            coloana.Caption = Dami.TraduCuvant(coloana.FieldName ?? coloana.Caption, coloana.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                int tip = -1;
                bool isTrainer = false;
                bool isTrainerHR = false;
                bool userTrainerId = false;

                DataTable dtDrept = GetDrepturi(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()));
                if (dtDrept != null && dtDrept.Rows.Count > 0)
                {
                    if (dtDrept.Select("Id = 8") != null && dtDrept.Select("Id = 8").Count() > 0)
                    {
                        tip = 3;
                        isTrainer = true;
                    }
                    if (dtDrept.Select("Id = 8") != null && dtDrept.Select("Id = 9").Count() > 0)
                    {
                        tip = 5;
                        isTrainerHR = true;
                    }
                    if (isTrainer && !isTrainerHR) // not HR
                    {
                        userTrainerId = true;
                    }
                }

                int hasNomenclatorCosturi = Convert.ToInt32(Dami.ValoareParam("hasNomenclatorCosturi", "0"));
                int hasNomenclatorTraineri = Convert.ToInt32(Dami.ValoareParam("hasNomenclatorTraineri", "0"));
                int afisareOrganizatorCursuri = Convert.ToInt32(Dami.ValoareParam("afisareOrganizatorCursuri", "0"));


                if (!IsPostBack)
                {
                    DataTable dtSes = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\"", null);
                    Session["CursInreg_Sesiuni"] = dtSes;
                }

                //DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblCateg_Niv1"" ", null);
                //GridViewDataComboBoxColumn col = (grDate.Columns["Categ_Niv1Id"] as GridViewDataComboBoxColumn);
                //col.PropertiesComboBox.DataSource = dt;

                //dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblCateg_Niv2"" ", null);
                //col = (grDate.Columns["Categ_Niv2Id"] as GridViewDataComboBoxColumn);
                //col.PropertiesComboBox.DataSource = dt;

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblTipSesiuni"" ", null);
                GridViewDataComboBoxColumn col = (grDate.Columns["TipSesiune"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblCategSesiuni"" ", null);
                col = (grDate.Columns["CategSesiune"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblTematici"" ", null);
                col = (grDate.Columns["TematicaId"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblCateg_Niv1"" ", null);
                col = (grDate.Columns["InternExtern"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblOrganizatori"" ", null);
                col = (grDate.Columns["OrganizatorId"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblTraineri"" ", null);
                col = (grDate.Columns["TrainerId"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblLocatii"" ", null);
                col = (grDate.Columns["Locatie"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""tblMonede"" ", null);
                col = (grDate.Columns["IdMoneda"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblStariSesiune"" ", null);
                col = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Eval_Quiz"" WHERE ""TipQuiz"" = 4 ", null);
                col = (grDate.Columns["IdQuiz"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                DataTable dtCurs = General.IncarcaDT("SELECT * FROM \"Curs_tblCurs\"", null);
                cmbCurs.DataSource = dtCurs;
                cmbCurs.DataBind();

                grDate.Columns["colNomenclatorCosturi"].Visible = (hasNomenclatorCosturi == 1) ? true : false;
                grDate.Columns["CostEstimat"].Visible = (hasNomenclatorCosturi == 0) ? true : false;
                grDate.Columns["colNomenclatorTrainer"].Visible = (hasNomenclatorTraineri == 1) ? true : false;
                grDate.Columns["TrainerId"].Visible = (hasNomenclatorTraineri == 0) ? true : false;
                grDate.Columns["OrganizatorId"].Visible = (afisareOrganizatorCursuri == 1) ? true : false;
                grDate.Columns["OrganizatorNume"].Visible = (afisareOrganizatorCursuri == 1) ? true : false;

                if (!IsPostBack)
                    Session["CursuriSesiuni_Grid"] = null;
                else
                    IncarcaGrid();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetDrepturi(int IdUser, int F10003 = -99)
        {
            try
            {
                DataTable q = null;
                q = General.IncarcaDT("SELECT a.\"IdGrup\" as \"Id\" FROM \"relGrupUser\" a WHERE a.\"IdUser\" = " + IdUser, null);    
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
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
                Session["CursuriSesiuni_Grid"] = null;
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

                //IncarcaGrid();
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
                int curs = -99;
                if (cmbCurs.Value != null)
                {
                    curs = Convert.ToInt32(cmbCurs.Value);
                }

                if (Session["CursuriSesiuni_Grid"] == null)
                {
                    grDate.KeyFieldName = "IdAuto";

                    dt = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\" WHERE \"IdCurs\" = " + curs, null);
                    //dt = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\" a LEFT JOIN \"Curs_tblCurs\" on a.\"IdCurs\" = b.\"Id\" WHERE a.\"IdCurs\" = " + curs, null);

                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["CursuriSesiuni_Grid"] = dt;
                }
                else
                {
                    grDate.KeyFieldName = "IdAuto";

                    dt = Session["CursuriSesiuni_Grid"] as DataTable;
                    //dt = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\" a LEFT JOIN \"Curs_tblCurs\" on a.\"IdCurs\" = b.\"Id\" WHERE a.\"IdCurs\" = " + curs, null);

                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
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

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
   
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
                    DataRow[] lst = dt.Select("Id=" + idStare);
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }






        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
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


        protected void grDate_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
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

        protected void grDate_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
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



        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
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
          
               


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable dt = Session["CursuriSesiuni_Grid"] as DataTable;

                General.SalveazaDate(dt, "Curs_tblCursSesiune");

                MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        /*LeonardM 01.09.2015 - verificare daca angajatul a terminat cursurile anterioare pentru a participa la cursul prezent*/
        public DataTable GetVerificareAbsolvireCursAnterior(int F10003, int idCurs)
        {
            DataTable result = null;

            try
            {
                string sql = @"select count(*)
                               from ""Curs_relCursAnterior"" ant
                               join ""Curs_tblCurs"" curs on ant.""IdCursAnterior"" = curs.""Id""
                               join ""Curs_tblCursSesiune"" ses on curs.""Id"" = ses.""IdCurs""          
                               left join ""Curs_Inregistrare"" inreg on curs.""Id"" = inreg.""IdCurs""   
                               and ses.""Id"" = inreg.""IdSesiune""
                               and inreg.""IdStare"" = 5
                               and inreg.""F10003"" = {0}
                               where ant.""IdCurs"" =  {1}
                               and inreg.""Id"" is null
                            ";
                sql = string.Format(sql, F10003, idCurs);

                result = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return result;
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["CursuriSesiuni_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                if (e.NewValues["Denumire"] == null || e.NewValues["Denumire"].ToString().Length <= 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste denumirea!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;

                        if (col.ColumnName.ToUpper() == "DATAEXPIRARE")
                        {
                            DateTime dataInceput = Convert.ToDateTime((e.NewValues["DataInceput"] ?? "01/01/2100").ToString());
                            int recurenta = 0;
                            if (Session["CursuriSesiuni_Recurenta"] != null)
                                recurenta = Convert.ToInt32(Session["CursuriSesiuni_Recurenta"].ToString());
                            DateTime dataExpirare = dataInceput.AddMonths(recurenta);
                            e.NewValues["DataExpirare"] = dataExpirare;
                        }
                        if (col.ColumnName.ToUpper() == "TOTALORE")
                        {
                            double TotalOre;
                            GetTotalHoursSession(e, out TotalOre);
                            e.NewValues["TotalOre"] = TotalOre;
                        }
                    }

                }

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["CursuriSesiuni_Grid"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }      
    
        

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["CursuriSesiuni_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;

                int idSesMax = 1;
                string sql = "SELECT MAX(a.\"Id\") + 1 FROM \"Curs_tblCursSesiune\" a LEFT JOIN \"Curs_tblCurs\" b on a.\"IdCurs\" = b.\"Id\" ";
                DataTable dtTmp = General.IncarcaDT(sql, null);
                if (dtTmp != null & dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null)
                    idSesMax = Convert.ToInt32(dtTmp.Rows[0][0].ToString()) + 1;

                int? categ1 = null;
                string categ3 = null;
                int? categ2_id = null;

                DataTable dtCurs = GetCursRestransFiltru((int)cmbCurs.Value);

                if (dtCurs != null && dtCurs.Rows.Count > 0)
                {
                    categ1 = dtCurs.Rows[0]["Categ_Niv1Id"] == DBNull.Value ? 0 : Convert.ToInt32(dtCurs.Rows[0]["Categ_Niv1Id"].ToString());
                    categ3 = dtCurs.Rows[0]["Categ_Niv3Nume"] == DBNull.Value ? "" : dtCurs.Rows[0]["Categ_Niv3Nume"].ToString();
                    if (categ3 != null && categ3.Length > 20)
                    {
                        categ3 = categ3.Substring(0, 20);
                    }
                    categ2_id = dtCurs.Rows[0]["Categ_Niv2Id"] == DBNull.Value ? 0 : Convert.ToInt32(dtCurs.Rows[0]["Categ_Niv2Id"].ToString());
                    Session["CursuriSesiuni_Recurenta"] = dtCurs.Rows[0]["Recurenta"] == DBNull.Value ? 0 : Convert.ToInt32(dtCurs.Rows[0]["Recurenta"].ToString());
                }

                e.NewValues["Id"] = idSesMax;
                e.NewValues["IdCurs"] = (int)cmbCurs.Value;
                e.NewValues["IdStare"] = 1;
 

                e.NewValues["InternExtern"] = categ1;
                e.NewValues["CodBuget"] = categ3 == null ? null : categ3.ToString();
                //e.NewValues["Categ_Niv1Id] = categ1;
                //e.NewValues["Categ_Niv2Id"] = categ2_id;

                e.NewValues["DataInceputAprobare"] = new DateTime(1900, 1, 1);
                e.NewValues["DataSfarsitAprobare"] = new DateTime(2100, 1, 1);


                if (Constante.tipBD == 1)
                {
                    if (dt.Columns["IdAuto"] != null)
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                            e.NewValues["IdAuto"] = max;
                        }
                        else
                            e.NewValues["IdAuto"] = 1;
                    }
                }
                else
                    e.NewValues["IdAuto"] = Dami.NextId("Curs_tblCursSesiune");

                if (e.NewValues["Denumire"] == null || e.NewValues["Denumire"].ToString().Length <= 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste denumirea!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "DATAEXPIRARE":
                                DateTime dataInceput = Convert.ToDateTime((e.NewValues["DataInceput"] ?? "01/01/2100").ToString());
                                int recurenta = 0;
                                if (Session["CursuriSesiuni_Recurenta"] != null)
                                    recurenta = Convert.ToInt32(Session["CursuriSesiuni_Recurenta"].ToString());
                                DateTime dataExpirare = dataInceput.AddMonths(recurenta);
                                e.NewValues["DataExpirare"] = dataExpirare;
                                break;
                            case "TOTALORE":                             
                                double TotalOre;
                                GetTotalHoursSession(e, out TotalOre);
                                e.NewValues["TotalOre"] = TotalOre;
                                
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["CursuriSesiuni_Grid"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["CursuriSesiuni_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["CursuriSesiuni_Grid"] = dt;
                grDate.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
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

 

        public DataTable GetCursRestransFiltru(int idCurs)
        {
            DataTable q = null;

            try
            {
                string sql = "SELECT a.\"IdAuto\", a.\"Id\", a.\"Denumire\", a.\"Categ_Niv1Id\",  b.\"Denumire\" as \"Categ_Niv1Nume\", a.\"Categ_Niv2Id\", c.\"Denumire\" as \"Categ_Niv2Nume\", "
                    + " a.\"Categ_Niv3Id,  d.\"Denumire\" as \"Categ_Niv3Nume\" , a.\"Recurenta\" FROM "
                    + " \"Curs_tblCurs\" a "
                    + " LEFT JOIN \"Curs_tblCateg_Niv1\" b on a.\"Categ_Niv1Id\" = b.\"Id\" "
                    + " LEFT JOIN \"Curs_tblCateg_Niv2\" c on a.\"Categ_Niv2Id\" = c.\"Id\" "
                    + " LEFT JOIN \"Curs_tblCateg_Niv3\" d on a.\"Categ_Niv3Id\" = d.\"Id\" "
                    + " WHERE a.\"Id\" = " + idCurs;

                q = General.IncarcaDT(sql, null);
              
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;

        }

        private void GetTotalHoursSession(DevExpress.Web.Data.ASPxDataInsertingEventArgs e, out double TotalHours)
        {
            TotalHours = -99;
            try
            {               
                DateTime dataSfarsit = Convert.ToDateTime((e.NewValues["DataSfarsit"] ?? "01/01/2100").ToString());

                DateTime dataInceput = Convert.ToDateTime((e.NewValues["DataInceput"] ?? "01/01/2100").ToString());
                int days = dataSfarsit.Subtract(dataInceput).Days + 1;

                DateTime tmpInceput = Convert.ToDateTime((e.NewValues["OraInceput"] ?? "01/01/2100").ToString());
                TimeSpan ts;
                DateTime tmpSfarsit = Convert.ToDateTime((e.NewValues["OraSfarsit"] ?? "01/01/2100").ToString());

                int orePauzaMasa = Convert.ToInt32((e.NewValues["OrePauzaMasa"] ?? "0").ToString());
                if (tmpSfarsit != null)
                {
                    ts = tmpSfarsit.Subtract(tmpInceput);
                    TotalHours = (ts.Hours - Convert.ToInt32(orePauzaMasa) + (ts.Minutes / 60.0)) * days;
                }                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GetTotalHoursSession(DevExpress.Web.Data.ASPxDataUpdatingEventArgs e, out double TotalHours)
        {
            TotalHours = -99;
            try
            {
                DateTime dataSfarsit = Convert.ToDateTime((e.NewValues["DataSfarsit"] ?? "01/01/2100").ToString());

                DateTime dataInceput = Convert.ToDateTime((e.NewValues["DataInceput"] ?? "01/01/2100").ToString());
                int days = dataSfarsit.Subtract(dataInceput).Days + 1;

                DateTime tmpInceput = Convert.ToDateTime((e.NewValues["OraInceput"] ?? "01/01/2100").ToString());
                TimeSpan ts;
                DateTime tmpSfarsit = Convert.ToDateTime((e.NewValues["OraSfarsit"] ?? "01/01/2100").ToString());

                int orePauzaMasa = Convert.ToInt32((e.NewValues["OrePauzaMasa"] ?? "0").ToString());
                if (tmpSfarsit != null)
                {
                    ts = tmpSfarsit.Subtract(tmpInceput);
                    TotalHours = (ts.Hours - Convert.ToInt32(orePauzaMasa) + (ts.Minutes / 60.0)) * days;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }


            /*            <dx:GridViewDataComboBoxColumn FieldName = "Categ_Niv1Id" Name="Categ_Niv1Id" Caption="Categ nivel1" Width="150px">
                            <PropertiesComboBox TextField = "Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode = "DisplayText" />
                        </ dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName = "Categ_Niv2Id" Name="Categ_Niv2Id" Caption="Categ nivel2"  Width="150px">
                            <PropertiesComboBox TextField = "Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode = "DisplayText" />
                        </ dx:GridViewDataComboBoxColumn> 
            */



}