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
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Beneficii
{
    public partial class Aprobare : System.Web.UI.Page
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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");


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

                if (!IsPostBack)
                {
                    string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                    DataTable dtHR = General.IncarcaDT("SELECT COUNT(*) FROM F100Supervizori WHERE IdUser = " + Session["UserId"].ToString() + " AND IdSuper IN (" + idHR + ")", null);
                    if (dtHR != null && dtHR.Rows.Count > 0 && Convert.ToInt32(dtHR.Rows[0][0].ToString()) > 0)                                         
                        Session["BenAprobare_HR"] = 1;                    
                    else                  
                        Session["BenAprobare_HR"] = 0;

                    txtDataInc.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    txtDataSf.Value = new DateTime(2100, 1, 1);

                    ASPxListBox nestedListBox = checkComboBoxStare.FindControl("listBox") as ASPxListBox;
                    for (int i = 0; i < nestedListBox.Items.Count; i++)
                    {
                        if (Convert.ToInt32(nestedListBox.Items[i].Value) == 2)
                            nestedListBox.Items[i].Selected = true;
                    }
                }

                if (Session["BenAprobare_HR"] != null && Convert.ToInt32(Session["BenAprobare_HR"].ToString()) == 1)                
                    btnSave.Visible = false;                
                else
                {
                    btnAproba.Visible = false;
                    btnRespinge.Visible = false;
                    cmbAngFiltru.Value = Convert.ToInt32(Session["User_Marca"].ToString());
                    cmbAngFiltru.ClientEnabled = false;
                }

                DataTable dtSes = General.IncarcaDT(SelectSesiuni(), null);
                GridViewDataComboBoxColumn colSes = (grDate.Columns["IdSesiune"] as GridViewDataComboBoxColumn);
                colSes.PropertiesComboBox.DataSource = dtSes;

                DataTable dtBen = General.IncarcaDT(SelectBeneficii(), null);
                GridViewDataComboBoxColumn colBen = (grDate.Columns["IdBeneficiu"] as GridViewDataComboBoxColumn);
                colBen.PropertiesComboBox.DataSource = dtBen;
                Session["BenAprobare_Ben"] = dtBen;

                cmbSesiuneFiltru.DataSource = dtSes;
                cmbSesiuneFiltru.DataBind();

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ben_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;
      

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

                //if (IsPostBack)
                //{
                //    DataTable dt = Session["BeneficiiAprob_Grid"] as DataTable;
                //    grDate.KeyFieldName = "IdAuto";
                //    grDate.DataSource = dt;
                //    grDate.DataBind();

                //}
                //else
                //    IncarcaGrid();



                if (!IsPostBack)
                    Session["BeneficiiAprob_Grid"] = null;

                IncarcaGrid();


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
                Session["BeneficiiAprob_Grid"] = null;
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
                cmbAngFiltru.Value = null;
                cmbSesiuneFiltru.Value = null;
                checkComboBoxStare.Value = null;
                Session["BeneficiiAprob_Grid"] = null;
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
                DataTable dt = Session["BeneficiiAprob_Grid"] as DataTable;

                //if (dt != null)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //        General.ExecutaNonQuery("UPDATE \"Ben_Sesiuni\" SET \"Descriere\" = '" + dt.Rows[i]["Descriere"].ToString() + "', IdBeneficiu = " + dt.Rows[i]["IdBeneficiu"].ToString() 
                //            + ", IdStare = 2, DataInceputBen = " + General.ToDataUniv(Convert.ToDateTime(dt.Rows[i]["DataInceputBen"])) + ", DataSfarsitBen = " + General.ToDataUniv(Convert.ToDateTime(dt.Rows[i]["DataSfarsitBen"])) + " WHERE \"IdSesiune\" = " + dt.Rows[i]["IdSesiune"].ToString() + " AND F10003 = " + dt.Rows[i]["F10003"].ToString(), null);
                //    IncarcaGrid();
                //}
                General.SalveazaDate(dt, "Ben_Sesiuni");
                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);
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
                MetodeCereri(1);
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

                //DataTable dt = SelectGrid();
                //grDate.KeyFieldName = "IdAuto";
                //grDate.DataSource = dt;
                //grDate.DataBind();
                //Session["BeneficiiAprob_Grid"] = dt;

                if (Session["BeneficiiAprob_Grid"] == null)
                {
                    dt = SelectGrid();
                    grDate.DataSource = dt;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataBind();
                    Session["BeneficiiAprob_Grid"] = dt;
                }
                else
                {
                    grDate.KeyFieldName = "IdAuto";

                    dt = Session["BeneficiiAprob_Grid"] as DataTable;
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Insuficienti parametrii");
                        return;
                    }

                    switch (arr[0])
                    {   
                        case "btnRespinge":
                            MetodeCereri(2, arr[1].Trim());
                            IncarcaGrid();
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
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



        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["BeneficiiAprob_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);
                DataTable dtBen = Session["BenAprobare_Ben"] as DataTable;      

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        if (col.ColumnName != "Descriere" && col.ColumnName != "DataInceputBen" && col.ColumnName != "DataSfarsitBen")
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;                       

                        if (col.ColumnName == "IdBeneficiu" && e.NewValues[col.ColumnName] != null)
                        {
                            DataRow dr = dtBen.Select("Id = " + e.NewValues[col.ColumnName].ToString()).FirstOrDefault();
                            row["Descriere"] = dr["Descriere"]; 
                            row["DataInceputBen"] = dr["DeLaData"]; 
                            row["DataSfarsitBen"] = dr["LaData"];
                        }    


                    }

                }

                row["IdStare"] = 2;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["BeneficiiAprob_Grid"] = dt;
                grDate.DataSource = dt;
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
                if (e.VisibleIndex >= 0)
                {              
                    if (e.ButtonID == "btnArata")
                    {
                        object[] obj = grDate.GetRowValues(e.VisibleIndex, new string[] { "IdStare", "IdBeneficiu" }) as object[];
                        if (obj[1] == null || obj[1].ToString().Length <= 0)
                            e.Visible = DefaultBoolean.False;                        
                    }                    
                }
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
                if (e.VisibleIndex == -1) return;

                if (e.ButtonType == ColumnCommandButtonType.Edit)
                {
                    if (Session["BenAprobare_HR"] != null && Convert.ToInt32(Session["BenAprobare_HR"].ToString()) == 1)                        
                        e.Visible = false;                        
                    else
                    {
                        object[] obj = grDate.GetRowValues(e.VisibleIndex, new string[] { "IdStare", "IdBeneficiu" }) as object[];
                        int idStare = Convert.ToInt32(obj[0].ToString());
                        if (idStare > 2)
                            e.Visible = false;
                    }
                }
               
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



        private void MetodeCereri(int tipActiune, string motiv = "")
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere

            try
            {
                int nrSel = 0;
                string ids = "", lstMarci = "", lstAuto= "";
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "IdSesiune", "F10003", "IdStare", "DataInceput", "DataSfarsit", "IdAuto" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
             
                    grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? dataInc = arr[3] as DateTime?;
                    DateTime? dataSf = arr[4] as DateTime?;

                    if (Convert.ToInt32(General.Nz(arr[2], 0)) != 2 || (dataInc ?? new DateTime(2100, 1, 1)) > DateTime.Now.Date || (dataSf ?? new DateTime(1900, 1, 1)) < DateTime.Now.Date)
                        continue;        
                 
                    ids += Convert.ToInt32(General.Nz(arr[0], 0)) + ";";
                    lstMarci += Convert.ToInt32(General.Nz(arr[1], 0)) + ";";
                    lstAuto += Convert.ToInt32(General.Nz(arr[5], 0)) + ";";
                    nrSel++;

                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                    {
   
                            grDate.JSProperties["cpAlertMessage"] = "Nu exista cereri valide";

                    }
                    else
                    {
          
                            grDate.JSProperties["cpAlertMessage"] = msg;
                    }
                    grDate.Selection.UnselectAll();
                    return;
                }

                msg = msg + AprobaCerere(Convert.ToInt32(Session["UserId"].ToString()), ids, lstMarci, lstAuto, nrSel, tipActiune, motiv);
  
        
                    grDate.JSProperties["cpAlertMessage"] = msg;

                Session["BeneficiiAprob_Grid"] = null;
                IncarcaGrid();

                grDate.Selection.UnselectAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string AprobaCerere(int idUser, string ids, string lstMarci, string lstAuto, int total, int actiune, string motiv = "")
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere


            string msg = "";
            string msgValid = "";

            try
            {
                if (ids == "") return "Nu exista cereri pentru aceasta actiune !";
     
                int nr = 0;
                string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrMarci = lstMarci.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrAuto = lstAuto.Split(new string[] { ";" }, StringSplitOptions.None);

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i] != "")
                    {
                        int id = -99;

                        try
                        {
                            id = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (id != -99)
                        {
                            General.ExecutaNonQuery("UPDATE \"Ben_Sesiuni\" SET \"IdStare\" = " + (actiune == 1 ? "3" : "4") + ", Motiv = '" + motiv + "' WHERE \"IdSesiune\" = " + id + " AND F10003 = " + arrMarci[i], null);                            

                            #region  Notificare start

                            string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };                
                            int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);

                            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                            {
                                NotifAsync.TrimiteNotificare("Beneficii.Aprobare", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 2 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM Ben_Sesiuni Z WHERE IdAuto=" + arrAuto[i], "Ben_Sesiuni", Convert.ToInt32(arrAuto[i]), idUser, marcaUser, arrParam);
                            });

                            #endregion

                            nr++;
                            
                        }
                    }
                }

                if (nr > 0)
                {
                    if (actiune == 1)
                        msg = "S-au aprobat " + nr.ToString() + " cereri din " + total + " !";
                    else
                        msg = "S-au respins " + nr.ToString() + " cereri din " + total + " !";

                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
                }
                else
                {
                    if (msgValid != "")
                        msg = msgValid;
                    else
                        msg = "Nu exista cereri pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

        }



        private string SelectAngajati()
        {
            string strSql = "";

            try
            {     
                strSql = $@"select F10003, ""NumeComplet"", ""Functia"", ""Subcompanie"", ""Filiala"", ""Sectie"", ""Departament""
                        from
                        (SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"",
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                        FROM F100 A 
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" FF on A.f10003 = FF.f10003 
                         ) T order by T.""NumeComplet"" ";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

      

        private string SelectSesiuni()
        {
            string strSql = "";

            try
            {
                strSql = $@"SELECT ""Id"", ""Denumire"" FROM  Ben_tblSesiuni  ORDER BY ""Denumire"" ";


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string SelectBeneficii()
        {
            string strSql = "";

            try
            {
                strSql = @"select CAST (a.""Id"" AS INT) as ""Id"", a.""Denumire"", a.""DeLaData"", a.""LaData"", a.""Descriere""
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal') ORDER BY a.""Denumire""";


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string DamiStari()
        {
            string val = "";

            try
            {
                if (General.Nz(checkComboBoxStare.Value, "").ToString() != "")
                {
                    val = checkComboBoxStare.Value.ToString().ToLower().Replace("solicitat", "1").Replace("in curs", "2").Replace("aprobat", "3").Replace("respins", "0").Replace("anulat", "-1").Replace("planificat", "4");
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

        public DataTable SelectGrid()
        {

            DataTable q = null;

            try
            {
                string strSql = "";
                string filtru = "";


                if (Convert.ToInt32(cmbAngFiltru.Value ?? -99) != -99) filtru += " AND F10003 = " + Convert.ToInt32(cmbAngFiltru.Value ?? -99);
                if (Convert.ToInt32(cmbSesiuneFiltru.Value ?? -99) != -99) filtru += " AND \"IdSesiune\" = " + Convert.ToInt32(cmbSesiuneFiltru.Value ?? -99);
                if (checkComboBoxStare.Value != null) filtru += @" AND ""IdStare"" IN (" + DamiStari() + ")";

                strSql = "SELECT IdAuto, F10003, IdSesiune, IdBeneficiu, DataInceput, DataSfarsit, IdStare, DataInceputBen, DataSfarsitBen, Descriere, USER_NO, TIME FROM Ben_Sesiuni WHERE 1=1 " + filtru ;     

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }




    }
}