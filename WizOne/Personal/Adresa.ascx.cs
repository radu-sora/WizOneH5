using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;

namespace WizOne.Personal
{
    public partial class Adresa : System.Web.UI.UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
         try
            {
                grDateAdresa.DataBind();

                if (!IsPostBack)
                {
                    Session["MP_CautaAdresa"] = null;
                    Session["MP_TipArtera"] = General.IncarcaDT("SELECT * FROM \"tblTipStrada\"", null);               
                }

                btnCauta.Visibility = GridViewCustomButtonVisibility.EditableRow;

                foreach (dynamic c in grDateAdresa.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }
                grDateAdresa.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDateAdresa.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDateAdresa.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDateAdresa.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDateAdresa.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

                DataTable dtAdr = Session["MP_CautaAdresa"] as DataTable;
                grDateCautaAdresa.DataSource = dtAdr;
                grDateCautaAdresa.KeyFieldName = "IdAuto";
                grDateCautaAdresa.DataBind();

                if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateCautaAdresa);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAdresa_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string sqlFinal = "SELECT * FROM \"F100Adrese\" WHERE F10003 = " + Session["Marca"].ToString();
                if (Constante.tipBD == 2)
                    sqlFinal = General.SelectOracle("F100Adrese", "IdAuto") + " WHERE F10003 = " + Session["Marca"].ToString();
                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("F100Adrese"))
                {
                    dt = ds.Tables["F100Adrese"];
                }
                else
                {
                    dt = General.IncarcaDT(sqlFinal, null);
                    dt.TableName = "F100Adrese";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }
                grDateAdresa.KeyFieldName = "IdAuto";
                grDateAdresa.DataSource = dt;

                string sql = @"SELECT * FROM ""tblTipAdresa"" ";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("tblTipAdresa", "Id");
                DataTable dtTipAdr = General.IncarcaDT(sql, null);
                GridViewDataComboBoxColumn colTipAdr = (grDateAdresa.Columns["IdTipAdresa"] as GridViewDataComboBoxColumn);
                colTipAdr.PropertiesComboBox.DataSource = dtTipAdr;

                sql = @"SELECT * FROM ""tblTipStrada"" ";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("tblTipStrada", "Id");
                DataTable dtTipArt = General.IncarcaDT(sql, null);
                GridViewDataComboBoxColumn colTipArt = (grDateAdresa.Columns["IdTipStrada"] as GridViewDataComboBoxColumn);
                colTipArt.PropertiesComboBox.DataSource = dtTipArt;

                if (!IsPostBack)
                {
                    lblAdresa.Text = DamiAdresa(dt);
                    DataTable dtLoc = General.IncarcaDT("SELECT * FROM LOCALITATI", null);
                    Session["MP_AdresaLocalitati"] = dtLoc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAdresa_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Adrese"];
                //if (dt.Columns["IdAuto"] != null)
                //{
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        //int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                //        int max = Dami.NextId("F100Adrese");
                //        e.NewValues["IdAuto"] = max;
                //    }
                //    else
                //        e.NewValues["IdAuto"] = 1;
                //}    

                e.NewValues["F10003"] = Session["Marca"];
                e.NewValues["Principal"] = 0;
                if (Constante.tipBD == 1)
                    e.NewValues["IdAuto"] = Convert.ToInt32(General.Nz(ds.Tables["F100Adrese"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    e.NewValues["IdAuto"] = Dami.NextId("F100Adrese");
                e.NewValues["USER_NO"] = Session["UserId"];
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["DataModif"] = DateTime.Now;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAdresa_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dtLoc = Session["MP_AdresaLocalitati"] as DataTable;
                int idAuto = 0;
                object[] row = new object[ds.Tables["F100Adrese"].Columns.Count];
                int x = 0;
                int tipArtera = 0;

                if (Convert.ToInt32(e.NewValues["IdTipAdresa"].ToString()) == 1)
                    e.NewValues["Principal"] = 1;
                else
                    e.NewValues["Principal"] = 0;

                foreach (DataColumn col in ds.Tables["F100Adrese"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F10003":
                                row[x] = Session["Marca"];
                                break;
                            case "IDTIPSTRADA":
                                DataTable dtArtera = Session["MP_TipArtera"] as DataTable;
                                if (dtArtera != null && dtArtera.Rows.Count > 0)
                                {
                                    bool gasit = false;
                                    for (int i = 0; i < dtArtera.Rows.Count; i++)
                                    {
                                        if (dtArtera.Rows[i]["Denumire"].ToString().Length > 0 && e.NewValues["Strada"].ToString().ToUpper().Contains(dtArtera.Rows[i]["Denumire"].ToString().ToUpper()))
                                        {
                                            row[x] = Convert.ToInt32(dtArtera.Rows[i]["Id"].ToString());
                                            tipArtera = Convert.ToInt32(dtArtera.Rows[i]["Id"].ToString());
                                            e.NewValues["Strada"] = e.NewValues["Strada"].ToString().ToUpper().Replace(dtArtera.Rows[i]["Denumire"].ToString().ToUpper(), "");
                                            gasit = true;
                                            break;
                                        }
                                    }
                                    if (!gasit)
                                    {
                                        row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                                        tipArtera = Convert.ToInt32((row[x] == DBNull.Value ? "0" : row[x].ToString()));
                                    }
                                }
                                break;
                            case "SIRUTANIVEL1":
                                if (hfSiruta.Contains("SirutaNivel1"))
                                    row[x] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel1"], -99));
                                break;
                            case "TIPNIVEL1":
                                row[x] = Convert.ToInt32(dtLoc.Select("SIRUTA=" + General.Nz(hfSiruta["SirutaNivel1"], -99))[0]["TIP"].ToString());
                                break;
                            case "SIRUTANIVEL2":
                                if (hfSiruta.Contains("SirutaNivel2"))
                                    row[x] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel2"], -99));
                                break;
                            case "TIPNIVEL2":
                                row[x] = Convert.ToInt32(dtLoc.Select("SIRUTA=" + General.Nz(hfSiruta["SirutaNivel2"], -99))[0]["TIP"].ToString());
                                break;
                            case "SIRUTANIVEL3":
                                if (hfSiruta.Contains("SirutaNivel3"))
                                    row[x] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel3"], -99));
                                break;
                            case "TIPNIVEL3":
                                row[x] = Convert.ToInt32(dtLoc.Select("SIRUTA=" + General.Nz(hfSiruta["SirutaNivel3"], -99))[0]["TIP"].ToString());
                                break;
                            case "PRINCIPAL":
                                row[x] = e.NewValues[col.ColumnName] ?? 0;
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)                                
                                    idAuto = Convert.ToInt32(General.Nz(ds.Tables["F100Adrese"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    idAuto = Dami.NextId("F100Adrese");
                                row[x] = idAuto;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                                break;
                        }
                    }

                    x++;
                }             

                ds.Tables["F100Adrese"].Rows.Add(row);

                if (General.Nz(e.NewValues["Principal"], 0).ToString() == "1")
                {
                    ds.Tables[2].Rows[0]["F1001021"] = tipArtera;

                    if (e.NewValues.Contains("NumeNivel1"))
                    {
                        ds.Tables[1].Rows[0]["F100891"] = e.NewValues["NumeNivel1"] ?? DBNull.Value;
                        DataTable dtJudet = General.IncarcaDT("SELECT OASP FROM JUDETE WHERE DENJUD = '" + ds.Tables[1].Rows[0]["F100891"].ToString().Replace("MUNICIPIUL ", "").Replace("JUDETUL ", "") + "'", null);
                        if (dtJudet != null && dtJudet.Rows.Count > 0 && dtJudet.Rows[0][0] != null)
                            ds.Tables[1].Rows[0]["F100895"] = dtJudet.Rows[0][0].ToString();
                    }

                    if (e.NewValues["NumeNivel2"] != null && (e.NewValues["NumeNivel2"].ToString().IndexOf("ORAS") == 0 || e.NewValues["NumeNivel2"].ToString().IndexOf("MUNICIPIU") == 0))
                    {
                        ds.Tables[1].Rows[0]["F10081"] = e.NewValues["NumeNivel2"] ?? "";
                        ds.Tables[1].Rows[0]["F100907"] = DBNull.Value;
                    }
                    else
                    {
                        ds.Tables[1].Rows[0]["F100907"] = e.NewValues["NumeNivel2"] ?? DBNull.Value;
                        ds.Tables[1].Rows[0]["F10081"] = "";
                    }
                    if (e.NewValues.Contains("NumeNivel3"))
                    {
                        if (e.NewValues["NumeNivel3"] != null && e.NewValues["NumeNivel3"].ToString().Contains("SECTORUL"))
                            ds.Tables[1].Rows[0]["F10082"] = e.NewValues["NumeNivel3"] ?? DBNull.Value;
                        else
                            ds.Tables[1].Rows[0]["F100908"] = e.NewValues["NumeNivel3"] ?? DBNull.Value;
                    }
                    ds.Tables[1].Rows[0]["F10083"] = e.NewValues["Strada"] ?? DBNull.Value;
                    ds.Tables[1].Rows[0]["F10084"] = e.NewValues["Numar"] ?? DBNull.Value;
                    ds.Tables[1].Rows[0]["F10085"] = e.NewValues["Bloc"] ?? DBNull.Value;
                    ds.Tables[1].Rows[0]["F10086"] = e.NewValues["Apartament"] ?? DBNull.Value;
                    ds.Tables[1].Rows[0]["F100892"] = e.NewValues["Scara"] ?? DBNull.Value;
                    ds.Tables[1].Rows[0]["F100893"] = e.NewValues["Etaj"] ?? DBNull.Value;
                    ds.Tables[1].Rows[0]["F10087"] = e.NewValues["CodPostal"] ?? DBNull.Value;

                    if (hfSiruta.Contains("SirutaNivel1"))
                        ds.Tables[1].Rows[0]["F100921"] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel1"], -99));
                    if (hfSiruta.Contains("SirutaNivel2"))
                        ds.Tables[1].Rows[0]["F100914"] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel2"], -99));
                    if (hfSiruta.Contains("SirutaNivel3"))
                        ds.Tables[1].Rows[0]["F100897"] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel3"], -99));

                    for (int i = 0; i < ds.Tables["F100Adrese"].Rows.Count; i++)
                    {
                        if (idAuto.ToString() != ds.Tables["F100Adrese"].Rows[i]["IdAuto"].ToString())
                            ds.Tables["F100Adrese"].Rows[i]["Principal"] = 0;
                    }
                }

                grDateAdresa.JSProperties["cp_AdresaCurenta"] = DamiAdresa(ds.Tables["F100Adrese"]);

                e.Cancel = true;
                grDateAdresa.CancelEdit();
                grDateAdresa.DataSource = ds.Tables["F100Adrese"];
                grDateAdresa.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAdresa_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataRow row = ds.Tables["F100Adrese"].Rows.Find(keys);
                DataTable dtLoc = Session["MP_AdresaLocalitati"] as DataTable;
                int tipArtera = 0;

                if (Convert.ToInt32(e.NewValues["IdTipAdresa"].ToString()) == 1)
                    e.NewValues["Principal"] = 1;
                else
                    e.NewValues["Principal"] = 0;

                foreach (DataColumn col in ds.Tables["F100Adrese"].Columns)
                {
                    if (e.NewValues.Contains(col.ColumnName))
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "SIRUTANIVEL1":
                                if (hfSiruta.Contains("SirutaNivel1"))
                                    row[col.ColumnName] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel1"], -99));
                                break;
                            case "TIPNIVEL1":
                                row[col.ColumnName] = Convert.ToInt32(dtLoc.Select("SIRUTA=" + General.Nz(hfSiruta["SirutaNivel1"], -99))[0]["TIP"].ToString());
                                break;                
                            case "SIRUTANIVEL2":
                                if (hfSiruta.Contains("SirutaNivel2"))
                                    row[col.ColumnName] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel2"], -99));
                                break;
                            case "TIPNIVEL2":
                                row[col.ColumnName] = Convert.ToInt32(dtLoc.Select("SIRUTA=" + General.Nz(hfSiruta["SirutaNivel2"], -99))[0]["TIP"].ToString());
                                break;                          
                            case "SIRUTANIVEL3":
                                if (hfSiruta.Contains("SirutaNivel3"))
                                    row[col.ColumnName] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel3"], -99));
                                break;
                            case "TIPNIVEL3":
                                row[col.ColumnName] = Convert.ToInt32(dtLoc.Select("SIRUTA=" + General.Nz(hfSiruta["SirutaNivel3"], -99))[0]["TIP"].ToString());
                                break;
                            case "IDTIPSTRADA":  
                                DataTable dtArtera = Session["MP_TipArtera"] as DataTable;
                                if (dtArtera != null && dtArtera.Rows.Count > 0)
                                {
                                    bool gasit = false;
                                    for (int i = 0; i < dtArtera.Rows.Count; i++)
                                    {
                                        if (dtArtera.Rows[i]["Denumire"].ToString().Length > 0 && e.NewValues["Strada"].ToString().ToUpper().Contains(dtArtera.Rows[i]["Denumire"].ToString().ToUpper()))
                                        {
                                            row[col.ColumnName] = Convert.ToInt32(dtArtera.Rows[i]["Id"].ToString());
                                            tipArtera = Convert.ToInt32(dtArtera.Rows[i]["Id"].ToString());
                                            e.NewValues["Strada"] = e.NewValues["Strada"].ToString().ToUpper().Replace(dtArtera.Rows[i]["Denumire"].ToString().ToUpper(), "");
                                            gasit = true;
                                            break;
                                        }
                                    }
                                    if (!gasit)
                                    {
                                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                                        tipArtera = Convert.ToInt32((row[col.ColumnName] == DBNull.Value ? "0" : row[col.ColumnName].ToString()));
                                    }
                                }
                                break; 
                            default:
                                row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                                break;
                        }
                    }
                }

                                     
                row["USER_NO"] = Session["UserId"];               
                row["TIME"] = DateTime.Now;
              

                if (General.Nz(e.NewValues["Principal"],0).ToString() == "1")
                {
                    ds.Tables[2].Rows[0]["F1001021"] = tipArtera;

                    if (e.NewValues.Contains("NumeNivel1"))
                    {
                        ds.Tables[1].Rows[0]["F100891"] = e.NewValues["NumeNivel1"] ?? DBNull.Value;
                        DataTable dtJudet = General.IncarcaDT("SELECT OASP FROM JUDETE WHERE DENJUD = '" + ds.Tables[1].Rows[0]["F100891"].ToString().Replace("MUNICIPIUL ", "").Replace("JUDETUL ", "") + "'", null);
                        if (dtJudet != null && dtJudet.Rows.Count > 0 && dtJudet.Rows[0][0] != null)
                            ds.Tables[1].Rows[0]["F100895"] = dtJudet.Rows[0][0].ToString();
                    }
                    if (e.NewValues.Contains("NumeNivel2"))
                    {
                        if (e.NewValues["NumeNivel2"] != null && (e.NewValues["NumeNivel2"].ToString().IndexOf("ORAS") == 0 || e.NewValues["NumeNivel2"].ToString().IndexOf("MUNICIPIU") == 0))
                        {
                            ds.Tables[1].Rows[0]["F10081"] = e.NewValues["NumeNivel2"] ?? "";
                            ds.Tables[1].Rows[0]["F100907"] = DBNull.Value;
                        }
                        else    //if (e.NewValues.Contains("Comuna"))
                        {
                            ds.Tables[1].Rows[0]["F100907"] = e.NewValues["NumeNivel2"] ?? DBNull.Value;
                            ds.Tables[1].Rows[0]["F10081"] = "";
                        }
                    }
                    if (e.NewValues.Contains("NumeNivel3"))
                    {
                        if (e.NewValues["NumeNivel3"] != null && e.NewValues["NumeNivel3"].ToString().Contains("SECTORUL"))
                            ds.Tables[1].Rows[0]["F10082"] = e.NewValues["NumeNivel3"] ?? DBNull.Value;
                        else
                            ds.Tables[1].Rows[0]["F100908"] = e.NewValues["NumeNivel3"] ?? DBNull.Value;
                    }
                    if (e.NewValues.Contains("Strada"))
                        ds.Tables[1].Rows[0]["F10083"] = e.NewValues["Strada"] ?? DBNull.Value;
                    if (e.NewValues.Contains("Numar"))
                        ds.Tables[1].Rows[0]["F10084"] = e.NewValues["Numar"] ?? DBNull.Value;
                    if (e.NewValues.Contains("Bloc"))
                        ds.Tables[1].Rows[0]["F10085"] = e.NewValues["Bloc"] ?? DBNull.Value;
                    if (e.NewValues.Contains("Apartament"))
                        ds.Tables[1].Rows[0]["F10086"] = e.NewValues["Apartament"] ?? DBNull.Value;
                    if (e.NewValues.Contains("Scara"))
                        ds.Tables[1].Rows[0]["F100892"] = e.NewValues["Scara"] ?? DBNull.Value;
                    if (e.NewValues.Contains("Etaj"))
                        ds.Tables[1].Rows[0]["F100893"] = e.NewValues["Etaj"] ?? DBNull.Value;
                    if (e.NewValues.Contains("CodPostal"))
                        ds.Tables[1].Rows[0]["F10087"] = e.NewValues["CodPostal"] ?? DBNull.Value;


                    if (hfSiruta.Contains("SirutaNivel1"))
                        ds.Tables[1].Rows[0]["F100921"] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel1"], -99));
                    if (hfSiruta.Contains("SirutaNivel2"))
                        ds.Tables[1].Rows[0]["F100914"] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel2"], -99));
                    if (hfSiruta.Contains("SirutaNivel3"))
                        ds.Tables[1].Rows[0]["F100897"] = Convert.ToInt32(General.Nz(hfSiruta["SirutaNivel3"], -99));

                    for (int i = 0; i < ds.Tables["F100Adrese"].Rows.Count; i++)
                    {
                        if (row["IdAuto"].ToString() != ds.Tables["F100Adrese"].Rows[i]["IdAuto"].ToString())
                            ds.Tables["F100Adrese"].Rows[i]["Principal"] = 0;
                    }
                }

                grDateAdresa.JSProperties["cp_AdresaCurenta"] = DamiAdresa(ds.Tables["F100Adrese"]);

                e.Cancel = true;
                grDateAdresa.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateAdresa.DataSource = ds.Tables["F100Adrese"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAdresa_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Adrese"].Rows.Find(keys);

                if (General.Nz(e.Values["Principal"], 0).ToString() == "1")
                {
                    ds.Tables[2].Rows[0]["F1001021"] = DBNull.Value;

                    ds.Tables[1].Rows[0]["F100891"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100895"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10081"] = "";
                    ds.Tables[1].Rows[0]["F100907"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100908"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10082"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10083"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10084"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10085"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10086"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100892"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100893"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F10087"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100921"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100914"] = DBNull.Value;
                    ds.Tables[1].Rows[0]["F100897"] = DBNull.Value;
                }

                row.Delete();

                grDateAdresa.JSProperties["cp_AdresaCurenta"] = DamiAdresa(ds.Tables["F100Adrese"]);

                e.Cancel = true;
                grDateAdresa.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateAdresa.DataSource = ds.Tables["F100Adrese"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }


        protected void popUpCauta_WindowCallback(object source, PopupWindowCallbackArgs e)
        {
            try
            {
                string param = e.Parameter;

                switch(param)
                {
                    case "Filtru":
                        {
                            if (txtJud.Text.Trim() != "" && txtJud.Text.Trim().Length < 3)
                            {
                                popUpCauta.JSProperties["cp_Mesaj"] = Dami.TraduCuvant("Introduceti minim 3 caractere la numele judetului!");
                                return;
                            }

                            if (txtLoc.Text.Trim() == "" && txtArt.Text.Trim() == "")
                            {
                                if (txtJud.Text.Trim() != "")
                                {
                                    popUpCauta.JSProperties["cp_Mesaj"] = Dami.TraduCuvant("Introduceti minim 3 caractere si la numele localitatii pentru o cautare mai rapida!");
                                    return;
                                }
                                else
                                {
                                    popUpCauta.JSProperties["cp_Mesaj"] = Dami.TraduCuvant("Introduceti minim 3 caractere la numele judetului si al localitatii!");
                                    return;
                                }
                            }

                            if (txtLoc.Text.Trim() == "" && txtJud.Text.Trim() == "")
                            {
                                popUpCauta.JSProperties["cp_Mesaj"] = Dami.TraduCuvant("Introduceti minim 3 caractere la numele judetului si al localitatii!");
                                return;
                            }

                            if (txtArt.Text.Trim() != "" && txtArt.Text.Trim().Length < 3)
                            {
                                popUpCauta.JSProperties["cp_Mesaj"] = Dami.TraduCuvant("Introduceti minim 3 caractere la numele arterei!");
                                return;
                            }

                            if (txtLoc.Text.Trim() != "" && txtLoc.Text.Trim().Length < 3)
                            {
                                popUpCauta.JSProperties["cp_Mesaj"] = Dami.TraduCuvant("Introduceti minim 3 caractere la numele localitatii!");
                                return;
                            }


                            DataTable dtAdr = GetAdresa(txtArt.Text, txtLoc.Text, txtJud.Text);
                            grDateCautaAdresa.DataSource = dtAdr;
                            grDateCautaAdresa.KeyFieldName = "IdAuto";
                            grDateCautaAdresa.DataBind();
                            Session["MP_CautaAdresa"] = dtAdr;
                        }
                        break;
                    case "Stergere":
                        grDateCautaAdresa.DataSource = null;
                        grDateCautaAdresa.DataBind();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        public DataTable GetAdresa(string artera, string localitate, string judet)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            int nr = 0;

            string sql = "";
            if (Constante.tipBD == 1)
                sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('CODURIPOSTALE')";
            else
                sql = "SELECT COUNT(*) FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('CODURIPOSTALE')";

            dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                if (dt.Rows[0][0].ToString().Length > 0)
                {
                    nr = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (nr > 0)
                    {
                        nr = 0;
                        sql = "SELECT COUNT(*) FROM CODURIPOSTALE";
                        dt1 = General.IncarcaDT(sql, null);
                        if (dt1 != null && dt1.Rows.Count > 0)
                            if (dt1.Rows[0][0].ToString().Length > 0)
                                nr = Convert.ToInt32(dt1.Rows[0][0].ToString());
                    }
                }

            if (artera == "" || nr == 0)
            {
                string strSql = "SELECT ROW_NUMBER() over(order by a.DENLOC, b.DENLOC, c.DENLOC) AS \"IdAuto\", " +
                         " ' ' AS \"Artera\", a.DENLOC AS \"NumeNivel3\", MAX(a.SIRUTA) AS \"SirutaNivel3\", " +
                         " b.DENLOC AS \"NumeNivel2\", MAX(b.SIRUTA) AS \"SirutaNivel2\", c.DENLOC AS \"NumeNivel1\", MAX(c.SIRUTA) AS \"SirutaNivel1\" from LOCALITATI a, LOCALITATI b, LOCALITATI c " +
                         " WHERE UPPER(a.DENLOC) LIKE UPPER('%" + localitate + "%') AND a.NIV = 3 AND a.SIRSUP = b.SIRUTA AND " +
                         " b.NIV = 2 AND b.SIRSUP = c.SIRUTA AND c.NIV = 1 AND UPPER(c.DENLOC) LIKE UPPER('%" + judet + "%') GROUP BY a.DENLOC, b.DENLOC, " +
                         " c.DENLOC ORDER BY \"NumeNivel1\", \"NumeNivel2\", \"NumeNivel3\", \"Artera\"";

                dt = General.IncarcaDT(strSql, null);
            }
            else
            {
                string cityName = "";
                if (localitate != "")
                    cityName = " AND UPPER(CITY_NAME) LIKE UPPER('%" + localitate + "%') ";

                string numeJudet = "";
                if (judet != "")
                    numeJudet = " AND UPPER(c.DENLOC) LIKE UPPER('%" + judet + "%') ";

                string op = " + ";
                if (Constante.tipBD == 2) op = " || ";


                string strSql = "SELECT MAX(\"IdAuto\") AS \"IdAuto\", STREET_I_T AS \"Artera\", a.DENLOC AS \"NumeNivel3\", MAX(a.SIRUTA) AS \"SirutaNivel3\", b.DENLOC AS \"NumeNivel2\", " +
                                "MAX(b.SIRUTA) AS \"SirutaNivel2\", c.DENLOC AS \"NumeNivel1\", MAX(c.SIRUTA) AS \"SirutaNivel1\" from CODURIPOSTALE, LOCALITATI a, LOCALITATI b, LOCALITATI c WHERE UPPER(STREET_I_T) LIKE UPPER('%" + artera + "%') AND a.NIV = 3 " +
                                "AND a.SIRSUP = b.SIRUTA AND b.NIV = 2 AND b.SIRSUP = c.SIRUTA AND c.NIV = 1 AND CITY_NAME = " +
                                "a.DENLOC " + cityName + " AND (c.DENLOC = 'JUDETUL ' " + op + " REGION_NAM OR c.DENLOC = 'MUN ' " + op + " REGION_NAM OR c.DENLOC = 'MUNICIPIUL ' " + op + " REGION_NAM) " + numeJudet +
                                "GROUP BY  STREET_I_T, a.DENLOC, b.DENLOC, c.DENLOC " +
                                "ORDER BY \"NumeNivel1\", \"NumeNivel2\", \"NumeNivel3\", \"Artera\"";

                dt = General.IncarcaDT(strSql, null);
            }

            return dt;
        }

        private string DamiAdresa(DataTable dt)
        {
            string txt = "";

            List<int> lstSate = new List<int> { 11, 19, 22, 23 };
            try
            {
                DataRow[] arr = dt.Select("Principal=1");
                if (arr.Length > 0)
                {
                    DataTable dtArtera = Session["MP_TipArtera"] as DataTable;
                    string strada = General.Nz(arr[0]["Strada"], "").ToString() != "" ? General.Nz(arr[0]["Strada"], "").ToString() : "";
                    if (dtArtera != null && dtArtera.Rows.Count > 0 && strada.Length > 0)
                    {
                        bool gasit = false;
                        for (int i = 0; i < dtArtera.Rows.Count; i++)
                        {
                            if (strada.ToUpper().Contains(dtArtera.Rows[i]["Denumire"].ToString().ToUpper()))
                            {
                                gasit = true;
                                break;
                            }
                        }
                        if (!gasit)
                            strada = "str. " + strada;
                    }
                    string numar = (General.Nz(arr[0]["Numar"], "").ToString() != "") ? " nr. " + General.Nz(arr[0]["Numar"], "").ToString() + "," : "";
                    string bloc = (General.Nz(arr[0]["Bloc"], "").ToString() != "" ) ? " bloc " + General.Nz(arr[0]["Bloc"], "").ToString() + "," : "";
                    string scara = (General.Nz(arr[0]["Scara"], "").ToString() != "") ? " sc. " + General.Nz(arr[0]["Scara"], "").ToString() + "," : "";
                    string etaj = "";
                    if (General.Nz(arr[0]["Etaj"], "").ToString() != "") {
                        etaj = " etaj " + General.Nz(arr[0]["Etaj"], "").ToString() + ",";
                        if (General.Nz(arr[0]["Etaj"], "").ToString() == "0")
                            etaj = " parter,";
                        if (General.Nz(arr[0]["Etaj"], "").ToString().Any(x => char.IsLetter(x)))
                            etaj = General.Nz(arr[0]["Etaj"], "").ToString() + ","; 
                        if (General.Nz(arr[0]["Etaj"], "").ToString().Contains("-"))
                            etaj = " subsol" + General.Nz(arr[0]["Etaj"], "").ToString().Replace('-', ' ') + ",";
                    }
                    string apartament = (General.Nz(arr[0]["Apartament"], "").ToString() != "") ? " ap. " + General.Nz(arr[0]["Apartament"], "").ToString() + "," : "";
                    string nivel3 = "";
                    if (General.Nz(arr[0]["NumeNivel3"], "").ToString() != "")
                    {
                        if (General.Nz(arr[0]["TipNivel3"], "").ToString() != "" && lstSate.Contains(Convert.ToInt32(General.Nz(arr[0]["TipNivel3"], "").ToString())))
                            nivel3 = " sat " + General.Nz(arr[0]["NumeNivel3"], "").ToString() + ",";
                        else if (General.Nz(arr[0]["TipNivel3"], "").ToString() != "" && General.Nz(arr[0]["TipNivel3"], "").ToString() == "6" && !General.Nz(arr[0]["NumeNivel3"], "").ToString().ToUpper().Contains("SECTOR"))
                            nivel3 = " sector " + General.Nz(arr[0]["NumeNivel3"], "").ToString() + ",";
                        else
                            nivel3 = " " + General.Nz(arr[0]["NumeNivel3"], "").ToString() + ", ";
                    }
                    string nivel2 = "";
                    if (General.Nz(arr[0]["NumeNivel2"], "").ToString() != "")
                    {
                        if (General.Nz(arr[0]["TipNivel2"], "").ToString() != "" && (General.Nz(arr[0]["TipNivel2"], "").ToString() == "1" || General.Nz(arr[0]["TipNivel2"], "").ToString() == "4") 
                            && !General.Nz(arr[0]["NumeNivel2"], "").ToString().ToUpper().Contains("MUNICIPIU"))
                            nivel2 = " municipiul " + General.Nz(arr[0]["NumeNivel2"], "").ToString() + ",";
                        else if (General.Nz(arr[0]["TipNivel2"], "").ToString() != "" && General.Nz(arr[0]["TipNivel2"], "").ToString() == "2" && !General.Nz(arr[0]["NumeNivel2"], "").ToString().ToUpper().Contains("ORAS"))
                            nivel2 = " orasul " + General.Nz(arr[0]["NumeNivel2"], "").ToString() + ",";
                        else if (General.Nz(arr[0]["TipNivel2"], "").ToString() != "" && General.Nz(arr[0]["TipNivel2"], "").ToString() == "3" && !General.Nz(arr[0]["NumeNivel2"], "").ToString().ToUpper().Contains("COMUNA"))
                            nivel2 = " comuna " + General.Nz(arr[0]["NumeNivel2"], "").ToString() + ",";
                        else
                            nivel2 = " " + General.Nz(arr[0]["NumeNivel2"], "").ToString() + ", ";
                    }
                    string nivel1 = "";
                    if (General.Nz(arr[0]["NumeNivel1"], "").ToString() != "")
                    {
                        if (!General.Nz(arr[0]["NumeNivel1"], "").ToString().ToUpper().Contains("MUNICIPIU") && !General.Nz(arr[0]["NumeNivel1"], "").ToString().ToUpper().Contains("JUDET"))
                        {
                            if (General.Nz(arr[0]["NumeNivel1"], "").ToString().ToUpper().Contains("BUCURESTI"))
                                nivel1 = " municipiul " + General.Nz(arr[0]["NumeNivel1"], "").ToString();
                            else
                                nivel1 = " judetul " + General.Nz(arr[0]["NumeNivel1"], "").ToString();
                        }
                        else
                            nivel1 = " " + General.Nz(arr[0]["NumeNivel1"], "").ToString();
                    }


                    txt = strada + numar + bloc + scara + etaj + apartament + nivel3 + nivel2 + nivel1;
                    //((General.Nz(arr[0]["Strada"], "").ToString() != "" && General.Nz(arr[0]["Strada"], "").ToString() != "0") ? "str. " + General.Nz(arr[0]["Strada"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Numar"], "").ToString() != "" && General.Nz(arr[0]["Numar"], "").ToString() != "0") ? "nr. " + General.Nz(arr[0]["Numar"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Bloc"], "").ToString() != "" && General.Nz(arr[0]["Bloc"], "").ToString() != "0") ? "bloc " + General.Nz(arr[0]["Bloc"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Scara"], "").ToString() != "" && General.Nz(arr[0]["Scara"], "").ToString() != "0") ? "sc. " + General.Nz(arr[0]["Scara"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Etaj"], "").ToString() != "" && General.Nz(arr[0]["Etaj"], "").ToString() != "0") ? "etaj " + General.Nz(arr[0]["Etaj"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Apartament"], "").ToString() != "" && General.Nz(arr[0]["Apartament"], "").ToString() != "0") ? "ap. " + General.Nz(arr[0]["Apartament"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["NumeNivel3"], "").ToString() != "" && General.Nz(arr[0]["NumeNivel3"], "").ToString() != "0") ?
                    //    (General.Nz(arr[0]["TipNivel3"], "").ToString() != "" && General.Nz(arr[0]["TipNivel3"], "").ToString() != "0" && lstSate.Contains(Convert.ToInt32(General.Nz(arr[0]["TipNivel3"], "").ToString()))) ? "sat " + General.Nz(arr[0]["NumeNivel3"], "").ToString() + ", " : General.Nz(arr[0]["NumeNivel3"], "").ToString() + ", " : "");

                    //((General.Nz(arr[0]["Comuna"], "").ToString() != "" && General.Nz(arr[0]["Comuna"], "").ToString() != "0") ? General.Nz(arr[0]["Comuna"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Sat"], "").ToString() != "" && General.Nz(arr[0]["Sat"], "").ToString() != "0") ? General.Nz(arr[0]["Sat"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Oras"], "").ToString() != "" && General.Nz(arr[0]["Oras"], "").ToString() != "0") ? General.Nz(arr[0]["Oras"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Sector"], "").ToString() != "" && General.Nz(arr[0]["Sector"], "").ToString() != "0") ? General.Nz(arr[0]["Sector"], "").ToString() + ", " : "") +
                    //((General.Nz(arr[0]["Judet"], "").ToString() != "" && General.Nz(arr[0]["Judet"], "").ToString() != "0") ? General.Nz(arr[0]["Judet"], "").ToString() + ", " : "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }

            return txt;
        }

    }
}