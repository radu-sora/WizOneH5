using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.Diagnostics;
using System.IO;

namespace WizOne.Personal
{
    public partial class Suspendari : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

                #region Traducere
                foreach (dynamic c in grDateSuspendari.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }

                lblMtvSusp.Text = Dami.TraduCuvant(lblMtvSusp.Text);
                lblDataInceputSusp.Text = Dami.TraduCuvant(lblDataInceputSusp.Text);
                lblDataSfarsitSusp.Text = Dami.TraduCuvant(lblDataSfarsitSusp.Text);
                lblDataIncetareSusp.Text = Dami.TraduCuvant(lblDataIncetareSusp.Text);

                grDateSuspendari.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDateSuspendari.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDateSuspendari.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDateSuspendari.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDateSuspendari.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

                #endregion

                //General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));
                string sql = @"SELECT * FROM F090";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F090", "F09002");
                DataTable dtSusp = General.IncarcaDT(sql, null);
                GridViewDataComboBoxColumn colSusp = (grDateSuspendari.Columns["F11104"] as GridViewDataComboBoxColumn);
                colSusp.PropertiesComboBox.DataSource = dtSusp;

                Session["MP_SuspNomen"] = dtSusp;

                cmbMotivSuspendare.DataSource = dtSusp;
                cmbMotivSuspendare.DataBind();

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables[0];
                if (!IsPostBack)
                {//Radu 27.11.2019
                    Session["MP_SuspMotiv"] = -99;           
                    ActualizareSusp(1);
                }
                else
                    ActualizareSusp(2);


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
                IncarcaGrid();

                if (IsPostBack)
                {
                    int motiv = Convert.ToInt32(Session["MP_SuspMotiv"].ToString());
                    if (motiv != -99)
                    {
                        cmbMotivSuspendare.Value = motiv;
                        deDataInceputSusp.Value = Convert.ToDateTime(Session["MP_SuspDataIncp"].ToString());
                        deDataSfarsitSusp.Value = Convert.ToDateTime(Session["MP_SuspDataSf"].ToString());
                        deDataIncetareSusp.Value = Convert.ToDateTime(Session["MP_SuspDataInct"].ToString());
                    }
                }              

                cmbMotivSuspendare.ClientEnabled = false;
                deDataInceputSusp.ClientEnabled = false;
                deDataSfarsitSusp.ClientEnabled = false;
                deDataIncetareSusp.ClientEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = General.IncarcaDT("SELECT * FROM F111 WHERE F11103 = " + Session["Marca"].ToString() + " ORDER BY F11105", null);
                grDateSuspendari.KeyFieldName = "F11103;F11104;F11105;F11106;F11107";
                grDateSuspendari.DataSource = dt;
                grDateSuspendari.DataBind();
                Session["MP_Suspendari"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnSalveaza_Click()
        {
            try
            {
                DateTime dt = new DateTime(2100, 1, 1);

                if (Convert.ToInt32(cmbMotivSuspendare.Value ?? -99) <= 0 || Convert.ToDateTime(deDataInceputSusp.Value ?? dt) == dt)
                {
                    pnlCtlSusp.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu este nicio suspendare de salvat!");
                    return;
                }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                ds.Tables[0].Rows[0]["F100925"] = Convert.ToInt32(cmbMotivSuspendare.Value);
                ds.Tables[0].Rows[0]["F100922"] = deDataInceputSusp.Date;
                ds.Tables[0].Rows[0]["F100923"] = deDataSfarsitSusp.Date;
                ds.Tables[0].Rows[0]["F100924"] = deDataIncetareSusp.Date;
                ds.Tables[1].Rows[0]["F100925"] = Convert.ToInt32(cmbMotivSuspendare.Value);
                ds.Tables[1].Rows[0]["F100922"] = deDataInceputSusp.Date;
                ds.Tables[1].Rows[0]["F100923"] = deDataSfarsitSusp.Date;
                ds.Tables[1].Rows[0]["F100924"] = deDataIncetareSusp.Date;
                //Radu 17.01.2020
                DataTable dtSuspNomen = Session["MP_SuspNomen"] as DataTable;
                DataRow[] drSusp = dtSuspNomen.Select("F09002 = " + Convert.ToInt32(cmbMotivSuspendare.Value).ToString());
                if (drSusp[0]["F09004"].ToString() != "Art52Alin1LiteraC")
                {
                    ds.Tables[0].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                    ds.Tables[0].Rows[0]["F1001102"] = deDataInceputSusp.Date.AddDays(-1);
                    ds.Tables[2].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                    ds.Tables[2].Rows[0]["F1001102"] = deDataInceputSusp.Date.AddDays(-1);
                }
                Session["InformatiaCurentaPersonal"] = ds;

                AdaugaIstoricSuspendare(Convert.ToInt32(Session["Marca"].ToString()), Convert.ToInt32(cmbMotivSuspendare.Value ?? -99), deDataInceputSusp.Date, deDataSfarsitSusp.Date, deDataIncetareSusp.Date, Convert.ToInt32(Session["UserId"].ToString()));

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void pnlCtlSusp_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                //btnSalveaza_Click();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string[] param = e.Parameter.Split(';');
                int motiv = Convert.ToInt32(Session["MP_SuspMotiv"].ToString());
                switch (param[0])
                {
                    case "ActSusp":
                        if (motiv != -99)
                        {
                            cmbMotivSuspendare.Value = motiv;
                            deDataInceputSusp.Value = Convert.ToDateTime(Session["MP_SuspDataIncp"].ToString());
                            deDataSfarsitSusp.Value = Convert.ToDateTime(Session["MP_SuspDataSf"].ToString());
                            deDataIncetareSusp.Value = Convert.ToDateTime(Session["MP_SuspDataInct"].ToString());
                            ds.Tables[0].Rows[0]["F100925"] = motiv;
                            ds.Tables[0].Rows[0]["F100922"] = Convert.ToDateTime(Session["MP_SuspDataIncp"].ToString());
                            ds.Tables[0].Rows[0]["F100923"] = Convert.ToDateTime(Session["MP_SuspDataSf"].ToString());
                            ds.Tables[0].Rows[0]["F100924"] = Convert.ToDateTime(Session["MP_SuspDataInct"].ToString());
                            ds.Tables[1].Rows[0]["F100925"] = motiv;
                            ds.Tables[1].Rows[0]["F100922"] = Convert.ToDateTime(Session["MP_SuspDataIncp"].ToString());
                            ds.Tables[1].Rows[0]["F100923"] = Convert.ToDateTime(Session["MP_SuspDataSf"].ToString());
                            ds.Tables[1].Rows[0]["F100924"] = Convert.ToDateTime(Session["MP_SuspDataInct"].ToString());
                        }
                        break;
                }
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Radu 22.01.2020 - nu se mai foloseste
        public void AdaugaIstoricSuspendare(int f10003, int idMotiv, DateTime dtInceput, DateTime dtSfarsit, DateTime dtIncetare, int idUser)
        {
            try
            {
                if (f10003 == -99) return;

                bool modif = false;

                string data = "";
                if (Constante.tipBD == 1)
                {
                    data = "COALESCE(A.F11105, '2100-01-01') AS F11105, " 
                         + "COALESCE(A.F11106, '2100-01-01') AS F11106, "
                         + "COALESCE(A.F11107, '2100-01-01') AS F11107 ";
                }
                else
                {
                    data = "COALESCE(A.F11105,TO_DATE('01-01-2100', 'DD-MM-YYYY')) AS F11105, "
                         + "COALESCE(A.F11106,TO_DATE('01-01-2100', 'DD-MM-YYYY')) AS F11106, "
                         + "COALESCE(A.F11107,TO_DATE('01-01-2100', 'DD-MM-YYYY')) AS F11107 ";
                }

                DataTable dt = General.IncarcaDT("SELECT " + data + ", a.* FROM F111 a WHERE F11103 = " + Session["Marca"].ToString() + " ORDER BY TIME DESC", null);


                if (dt == null || dt.Rows.Count == 0)
                {
                    modif = true;
                }
                else
                {
                    if (idMotiv != Convert.ToInt32(dt.Rows[0]["F11104"].ToString()) || 
                        dtInceput.Date != Convert.ToDateTime(dt.Rows[0]["F11105"]).Date || 
                        dtSfarsit.Date != Convert.ToDateTime(dt.Rows[0]["F11106"]).Date || 
                        dtIncetare.Date != Convert.ToDateTime(dt.Rows[0]["F11107"]).Date) modif = true;
                }

                if (modif)
                {
    
                    DateTime dtLuc = General.DamiDataLucru();
                    DataTable dtF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + f10003.ToString(), null);

                    if (dtF100 != null && dtF100.Rows.Count > 0)
                    {
                        string sql =
                            $@"INSERT INTO F111 (F11101, F11102, F11103, F11104, F11105, F11106, F11107, YEAR, MONTH, USER_NO, TIME)
                               VALUES (111, '{General.Nz(dtF100.Rows[0]["F10017"], "")}', {f10003}, {idMotiv},{General.ToDataUniv(dtInceput)}, {General.ToDataUniv(dtSfarsit)}, {General.ToDataUniv(dtIncetare)},
                               {dtLuc.Year}, {dtLuc.Month}, {Session["UserId"]}, {General.CurrentDate()})";

                        General.IncarcaDT(sql, null);

                        try
                        {
                            General.CalculCO(dtInceput.Year, f10003);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSuspendari_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                if (e.NewValues["F11104"] == null || Convert.ToInt32(e.NewValues["F11104"].ToString()) <= 0)
                {
                    e.Cancel = true;
                    grDateSuspendari.CancelEdit();
                    return;
                }

                if (e.NewValues["F11105"] == null || e.NewValues["F11105"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11105"].ToString()) == new DateTime(2100, 1, 1))
                {
                    e.Cancel = true;
                    grDateSuspendari.CancelEdit();
                    return;
                }

                if (e.NewValues["F11106"] == null || e.NewValues["F11106"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11106"].ToString()) == new DateTime(2100, 1, 1))
                {
                    e.Cancel = true;
                    grDateSuspendari.CancelEdit();
                    return;
                }


                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }                

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dtSusp = Session["MP_Suspendari"] as DataTable;
                DataRow row = dtSusp.Rows.Find(keys);

                foreach (DataColumn col in dtSusp.Columns)
                {
                    if (!col.AutoIncrement && grDateSuspendari.Columns[col.ColumnName] != null && grDateSuspendari.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    if (col.ColumnName.ToUpper() == "USER_NO")                     
                        row[col.ColumnName] = Convert.ToInt32(Session["UserId"].ToString());
                    if (col.ColumnName.ToUpper() == "TIME")
                        row[col.ColumnName] = DateTime.Now;

                }

                e.Cancel = true;
                grDateSuspendari.CancelEdit();
               
                grDateSuspendari.DataSource = dtSusp;
                Session["MP_Suspendari"] = dtSusp;

                //ds.Tables[0].Rows[0]["F100925"] = dtSusp.Rows[0]["F11104"];
                //ds.Tables[0].Rows[0]["F100922"] = dtSusp.Rows[0]["F11105"];
                //ds.Tables[0].Rows[0]["F100923"] = dtSusp.Rows[0]["F11106"];
                //ds.Tables[0].Rows[0]["F100924"] = dtSusp.Rows[0]["F11107"];
                //ds.Tables[1].Rows[0]["F100925"] = dtSusp.Rows[0]["F11104"];
                //ds.Tables[1].Rows[0]["F100922"] = dtSusp.Rows[0]["F11105"];
                //ds.Tables[1].Rows[0]["F100923"] = dtSusp.Rows[0]["F11106"];
                //ds.Tables[1].Rows[0]["F100924"] = dtSusp.Rows[0]["F11107"];
                ////Radu 17.01.2020
                //ds.Tables[0].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                //ds.Tables[0].Rows[0]["F1001102"] = Convert.ToDateTime(dtSusp.Rows[0]["F11105"].ToString()).Date.AddDays(-1);
                //ds.Tables[2].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                //ds.Tables[2].Rows[0]["F1001102"] = Convert.ToDateTime(dtSusp.Rows[0]["F11105"].ToString()).Date.AddDays(-1);

                if (Convert.ToInt32(row["F11104"].ToString()) == 11 && Convert.ToDateTime(row["F11107"]) != new DateTime(2100, 1, 1))  //revenire din CIC
                {
                    ds.Tables[0].Rows[0]["F10076"] = row["F11105"];
                    ds.Tables[0].Rows[0]["F10077"] = Convert.ToDateTime(row["F11107"].ToString()).AddDays(-1);
                    ds.Tables[1].Rows[0]["F10076"] = row["F11105"];
                    ds.Tables[1].Rows[0]["F10077"] = Convert.ToDateTime(row["F11107"].ToString()).AddDays(-1);
                }

                Session["InformatiaCurentaPersonal"] = ds;

                
                General.SalveazaDate(dtSusp, "F111");

                //transfer
                DataTable dtSuspNomen = Session["MP_SuspNomen"] as DataTable;
                if (dtSuspNomen != null && dtSuspNomen.Rows.Count > 0)
                {
                    DataRow[] dr = dtSuspNomen.Select("F09002 = " + e.NewValues["F11104"]);
                    if (dr != null && dr.Count() > 0)
                    {
                        int cod = (dr[0]["CodTranzactie"] as int?) ?? 0;
                        if (dr[0]["TransferTranzactii"] != null && dr[0]["TransferTranzactii"].ToString().Length > 0 && Convert.ToInt32(dr[0]["TransferTranzactii"].ToString()) == 1)
                            General.TransferTranzactii(Session["Marca"].ToString(), cod.ToString(),
                                Convert.ToDateTime(e.NewValues["F11105"].ToString()), Convert.ToDateTime(e.NewValues["F11106"].ToString()), Convert.ToDateTime(e.NewValues["F11107"].ToString()));
                        if (dr[0]["TransferPontaj"] != null && dr[0]["TransferPontaj"].ToString().Length > 0 && Convert.ToInt32(dr[0]["TransferPontaj"].ToString()) == 1)
                            General.TransferPontaj(Session["Marca"].ToString(), Convert.ToDateTime(e.NewValues["F11105"].ToString()), 
                                Convert.ToDateTime(e.NewValues["F11106"].ToString()), Convert.ToDateTime(e.NewValues["F11107"].ToString()),
                                (dr[0]["DenumireScurta"] as string));

                    }
                }

                try
                {
                    General.CalculCO(Convert.ToDateTime(dtSusp.Rows[0]["F11105"]).Year, Convert.ToInt32(dtSusp.Rows[0]["F11103"].ToString()));
                }
                catch (Exception) { }

                ActualizareSusp(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }

        }

        protected void grDateSuspendari_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            //try
            //{
            //    object[] keys = new object[e.Keys.Count];
            //    for (int i = 0; i < e.Keys.Count; i++)
            //    { keys[i] = e.Keys[i]; }

            //    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            //    DataTable dtSusp = Session["MP_Suspendari"] as DataTable;
            //    DataRow row = dtSusp.Rows.Find(keys);

            //    row.Delete();

            //    e.Cancel = true;
            //    grDateSuspendari.CancelEdit();
            //    Session["InformatiaCurentaPersonal"] = ds;
            //    grDateSuspendari.DataSource = dtSusp;
            //    Session["MP_Suspendari"] = dtSusp;
            //    ActualizareSusp(2);
            //    General.SalveazaDate(dtSusp, "F111");

            //    try
            //    {
            //        General.CalculCO(Convert.ToDateTime(dtSusp.Rows[0]["F11105"]).Year, Convert.ToInt32(dtSusp.Rows[0]["F11103"].ToString()));
            //    }
            //    catch (Exception) { }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            //}
        }


        protected void grDateSuspendari_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DateTime dtLuc = General.DamiDataLucru();
                e.NewValues["F11101"] = 111;
                e.NewValues["F11102"] = ds.Tables[0].Rows[0]["F10017"];
                e.NewValues["F11103"] = Convert.ToInt32(Session["Marca"].ToString());
                e.NewValues["F11104"] = 0;
                e.NewValues["F11105"] = new DateTime(2100, 1, 1);
                e.NewValues["F11106"] = new DateTime(2100, 1, 1);
                e.NewValues["F11107"] = new DateTime(2100, 1, 1);
                e.NewValues["YEAR"] = dtLuc.Year;
                e.NewValues["MONTH"] = dtLuc.Month;
                e.NewValues["USER_NO"] = Convert.ToInt32(Session["UserId"].ToString());
                e.NewValues["TIME"] = DateTime.Now;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSuspendari_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                bool err = false;
                if (e.NewValues["F11104"] == null || Convert.ToInt32(e.NewValues["F11104"].ToString()) <= 0)   
                    err= true;                

                if (e.NewValues["F11105"] == null || e.NewValues["F11105"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11105"].ToString()) == new DateTime(2100, 1, 1))
                    err = true;

                if (e.NewValues["F11106"] == null || e.NewValues["F11106"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11106"].ToString()) == new DateTime(2100, 1, 1))
                    err = true;

                if (!err)
                {
                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                    DataTable dtSusp = Session["MP_Suspendari"] as DataTable;
                    DateTime dtLuc = General.DamiDataLucru();
                    object[] row = new object[dtSusp.Columns.Count];
                    int x = 0;
                    foreach (DataColumn col in dtSusp.Columns)
                    {
                        if (!col.AutoIncrement)
                        {
                            switch (col.ColumnName.ToUpper())
                            {
                                case "F11101":
                                    row[x] = 111;
                                    break;
                                case "F11102":
                                    row[x] = ds.Tables[0].Rows[0]["F10017"];
                                    break;
                                case "F11103":
                                    row[x] = Convert.ToInt32(Session["Marca"].ToString());
                                    break;
                                case "YEAR":
                                    row[x] = dtLuc.Year;
                                    break;
                                case "MONTH":
                                    row[x] = dtLuc.Month;
                                    break;
                                case "USER_NO":
                                    row[x] = Convert.ToInt32(Session["UserId"].ToString());
                                    break;
                                case "TIME":
                                    row[x] = DateTime.Now;
                                    break;
                                default:
                                    row[x] = e.NewValues[col.ColumnName];
                                    break;
                            }
                        }
                        x++;
                    }

                    dtSusp.Rows.Add(row);
                    e.Cancel = true;
                    grDateSuspendari.CancelEdit();
                    grDateSuspendari.DataSource = dtSusp;
                    //grDateSuspendari.KeyFieldName = "IdAuto";
                    Session["MP_Suspendari"] = dtSusp;


                    General.SalveazaDate(dtSusp, "F111");

                    try
                    {
                        General.CalculCO(Convert.ToDateTime(dtSusp.Rows[0]["F11105"]).Year, Convert.ToInt32(dtSusp.Rows[0]["F11103"].ToString()));
                    }
                    catch (Exception) { }


                    //ds.Tables[0].Rows[0]["F100925"] = dtSusp.Rows[0]["F11104"];
                    //ds.Tables[0].Rows[0]["F100922"] = dtSusp.Rows[0]["F11105"];
                    //ds.Tables[0].Rows[0]["F100923"] = dtSusp.Rows[0]["F11106"];
                    //ds.Tables[0].Rows[0]["F100924"] = dtSusp.Rows[0]["F11107"];
                    //ds.Tables[1].Rows[0]["F100925"] = dtSusp.Rows[0]["F11104"];
                    //ds.Tables[1].Rows[0]["F100922"] = dtSusp.Rows[0]["F11105"];
                    //ds.Tables[1].Rows[0]["F100923"] = dtSusp.Rows[0]["F11106"];
                    //ds.Tables[1].Rows[0]["F100924"] = dtSusp.Rows[0]["F11107"];
                    ////Radu 17.01.2020
                    //ds.Tables[0].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                    //ds.Tables[0].Rows[0]["F1001102"] = Convert.ToDateTime(dtSusp.Rows[0]["F11105"].ToString()).Date.AddDays(-1);
                    //ds.Tables[2].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                    //ds.Tables[2].Rows[0]["F1001102"] = Convert.ToDateTime(dtSusp.Rows[0]["F11105"].ToString()).Date.AddDays(-1);
                    Session["InformatiaCurentaPersonal"] = ds;
                    ActualizareSusp(2);

                    //transfer
                    DataTable dtSuspNomen = Session["MP_SuspNomen"] as DataTable;
                    if (dtSuspNomen != null && dtSuspNomen.Rows.Count > 0)
                    {
                        DataRow[] dr = dtSuspNomen.Select("F09002 = " + e.NewValues["F11104"]);
                        if (dr != null && dr.Count() > 0)
                        {
                            int cod = (dr[0]["CodTranzactie"] as int?) ?? 0;
                            if (dr[0]["TransferTranzactii"] != null && dr[0]["TransferTranzactii"].ToString().Length > 0 && Convert.ToInt32(dr[0]["TransferTranzactii"].ToString()) == 1)                            
                                General.TransferTranzactii(Session["Marca"].ToString(), cod.ToString(),
                                    Convert.ToDateTime(e.NewValues["F11105"].ToString()), Convert.ToDateTime(e.NewValues["F11106"].ToString()), Convert.ToDateTime(e.NewValues["F11107"].ToString()));
                            if (dr[0]["TransferPontaj"] != null && dr[0]["TransferPontaj"].ToString().Length > 0 && Convert.ToInt32(dr[0]["TransferPontaj"].ToString()) == 1)
                                General.TransferPontaj(Session["Marca"].ToString(), Convert.ToDateTime(e.NewValues["F11105"].ToString()), Convert.ToDateTime(e.NewValues["F11106"].ToString()), Convert.ToDateTime(e.NewValues["F11107"].ToString()),
                                    (dr[0]["DenumireScurta"] as string));

                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                    grDateSuspendari.CancelEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void ActualizareSusp(int param)
        {
            DataTable dtSuspAng = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataTable dt = Session["MP_Suspendari"] as DataTable;
            if (param == 1)
            {
                dtSuspAng = General.IncarcaDT("select * from f111 Where F11103 = " + Session["Marca"].ToString() + " AND (F11107 IS NULL OR F11107 = "
                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ")  ORDER BY F11105", null);

            }
            if (param == 2)
            {               
                string data = General.ToDataUniv(2100, 1, 1);
                if (dt.Select("F11107 IS NULL OR F11107 = " + data, "F11105 ASC") != null && dt.Select("F11107 IS NULL OR F11107 = " + data, "F11105 ASC").Count() > 0)
                    dtSuspAng = dt.Select("F11107 IS NULL OR F11107 = " + data, "F11105 ASC").CopyToDataTable();
            }
            if (dtSuspAng != null && dtSuspAng.Rows.Count > 0)
            {
                cmbMotivSuspendare.Value = dtSuspAng.Rows[0]["F11104"];
                deDataInceputSusp.Value = dtSuspAng.Rows[0]["F11105"];
                deDataSfarsitSusp.Value = dtSuspAng.Rows[0]["F11106"];
                deDataIncetareSusp.Value = dtSuspAng.Rows[0]["F11107"];
                if (param == 2)
                {
                    Session["MP_SuspMotiv"] = dtSuspAng.Rows[0]["F11104"];
                    Session["MP_SuspDataIncp"] = dtSuspAng.Rows[0]["F11105"];
                    Session["MP_SuspDataSf"] = dtSuspAng.Rows[0]["F11106"];
                    Session["MP_SuspDataInct"] = dtSuspAng.Rows[0]["F11107"];
                }
                ds.Tables[0].Rows[0]["F100925"] = dtSuspAng.Rows[0]["F11104"];
                ds.Tables[0].Rows[0]["F100922"] = dtSuspAng.Rows[0]["F11105"];
                ds.Tables[0].Rows[0]["F100923"] = dtSuspAng.Rows[0]["F11106"];
                ds.Tables[0].Rows[0]["F100924"] = dtSuspAng.Rows[0]["F11107"];
                ds.Tables[1].Rows[0]["F100925"] = dtSuspAng.Rows[0]["F11104"];
                ds.Tables[1].Rows[0]["F100922"] = dtSuspAng.Rows[0]["F11105"];
                ds.Tables[1].Rows[0]["F100923"] = dtSuspAng.Rows[0]["F11106"];
                ds.Tables[1].Rows[0]["F100924"] = dtSuspAng.Rows[0]["F11107"];
                //Radu 17.01.2020
                DataTable dtSuspNomen = Session["MP_SuspNomen"] as DataTable;
                DataRow[] drSusp = dtSuspNomen.Select("F09002 = " + dtSuspAng.Rows[0]["F11104"].ToString());
                if (drSusp[0]["F09004"].ToString() != "Art52Alin1LiteraC")
                {
                    ds.Tables[0].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                    ds.Tables[0].Rows[0]["F1001102"] = Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Date.AddDays(-1);
                    ds.Tables[2].Rows[0]["F1001101"] = ds.Tables[0].Rows[0]["F10022"];
                    ds.Tables[2].Rows[0]["F1001102"] = Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Date.AddDays(-1);
                }
                if (Convert.ToInt32(dtSuspAng.Rows[0]["F11104"].ToString()) == 11 && Convert.ToDateTime(ds.Tables[0].Rows[0]["F100924"]) == new DateTime(2100, 1, 1))  //introducere CIC
                {
                    ds.Tables[0].Rows[0]["F10076"] = ds.Tables[0].Rows[0]["F100922"];
                    ds.Tables[0].Rows[0]["F10077"] = ds.Tables[0].Rows[0]["F100923"];
                    ds.Tables[1].Rows[0]["F10076"] = ds.Tables[0].Rows[0]["F100922"];
                    ds.Tables[1].Rows[0]["F10077"] = ds.Tables[0].Rows[0]["F100923"];
                }
            }
            else
            {
                if (param == 1)
                {
                    cmbMotivSuspendare.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["F100925"] as int? ?? 0);
                    deDataInceputSusp.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100922"] as DateTime? ?? new DateTime(2100, 1, 1));
                    deDataSfarsitSusp.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100923"] as DateTime? ?? new DateTime(2100, 1, 1));
                    deDataIncetareSusp.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100924"] as DateTime? ?? new DateTime(2100, 1, 1));
                }
                else
                {
                    //Radu 21.01.2020
                    DataTable dtSuspNomen = Session["MP_SuspNomen"] as DataTable;
                    DataRow[] drSusp = dtSuspNomen.Select("F09002 = " + dtSuspAng.Rows[0]["F11104"].ToString());
                    if (drSusp[0]["F09004"].ToString() != "Art52Alin1LiteraC")
                    {
                        ds.Tables[0].Rows[0]["F1001101"] = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100924"]) == new DateTime(2100, 1, 1) ? ds.Tables[0].Rows[0]["F10022"] : ds.Tables[0].Rows[0]["F100924"];
                        ds.Tables[0].Rows[0]["F1001102"] = new DateTime(2100, 1, 1);
                        ds.Tables[2].Rows[0]["F1001101"] = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100924"]) == new DateTime(2100, 1, 1) ? ds.Tables[0].Rows[0]["F10022"] : ds.Tables[0].Rows[0]["F100924"];
                        ds.Tables[2].Rows[0]["F1001102"] = new DateTime(2100, 1, 1);
                    }
                    if (dt.Select("1 = 1", "F11105 DESC") != null && dt.Select("1 = 1", "F11105 DESC").Count() > 0 && Convert.ToInt32(dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11104"].ToString()) == 11 && Convert.ToDateTime(dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11107"]) != new DateTime(2100, 1, 1))  //revenire din CIC
                    {
                        ds.Tables[0].Rows[0]["F10076"] = dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11105"];
                        ds.Tables[0].Rows[0]["F10077"] = Convert.ToDateTime(dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11107"].ToString()).AddDays(-1);
                        ds.Tables[1].Rows[0]["F10076"] = dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11105"];
                        ds.Tables[1].Rows[0]["F10077"] = Convert.ToDateTime(dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11107"].ToString()).AddDays(-1);
                    }

                    if (dt.Select("1 = 1", "F11105 DESC") != null && dt.Select("1 = 1", "F11105 DESC").Count() > 0)
                    {
                        Session["MP_SuspMotiv"] = dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11104"];
                        Session["MP_SuspDataIncp"] = dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11105"];
                        Session["MP_SuspDataSf"] = dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11106"];
                        Session["MP_SuspDataInct"] = dt.Select("1 = 1", "F11105 DESC").CopyToDataTable().Rows[0]["F11107"];
                    }
                }
                //if (param == 2)
                //{
                //    Session["MP_SuspMotiv"] = 0;
                //    Session["MP_SuspDataIncp"] = new DateTime(2100, 1, 1);
                //    Session["MP_SuspDataSf"] = new DateTime(2100, 1, 1);
                //    Session["MP_SuspDataInct"] = new DateTime(2100, 1, 1);
                //}


            }
        }


    }


    //    <asp:UpdatePanel ID = "UpdatePanel1" runat="server">
    //    <ContentTemplate>
    //         <asp:HiddenField ID = "hfMotiv" runat="server" />
    //        <asp:HiddenField ID = "hfDataIncp" runat="server" />
    //        <asp:HiddenField ID = "hfDataSf" runat="server" />
    //        <asp:HiddenField ID = "hfDataInct" runat="server" />  
    //    </ContentTemplate>
    //</asp:UpdatePanel>

    //var motiv = parseInt("<%=Session["MP_SuspMotiv"] %>");
    //var dataIncp = "<%=Session["MP_SuspDataIncp"] %>";
    //var dataSf = "<%=Session["MP_SuspDataSf"] %>";
    //var dataInct = "<%=Session["MP_SuspDataInct"] %>";

    //var motiv = parseInt($('#<%=hfMotiv.ClientID %>').val());
    //var dataIncp = $('#<%=hfDataIncp.ClientID %>').val();
    //var dataSf = $('#<%=hfDataSf.ClientID %>').val();
    //var dataInct = $('#<%=hfDataInct.ClientID %>').val();

    //var motiv = parseInt(hfSusp.Get("Motiv"));
    //var dataIncp = hfSusp.Get("DataIncp");
    //var dataSf = hfSusp.Get("DataSf");
    //var dataInct = hfSusp.Get("DataInct");
}