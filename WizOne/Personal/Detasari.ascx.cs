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
    public partial class Detasari : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            DataTable dt = ds.Tables[0];
            if (!IsPostBack)
                ActualizareDet(1);            
            else
                ActualizareDet(2);


            Mutare_DataList.DataSource = table;
            Mutare_DataList.DataBind();

            grDateDetasari.DataBind();
            foreach (dynamic c in grDateDetasari.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateDetasari.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateDetasari.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateDetasari.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateDetasari.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateDetasari.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");


            lblNumeAngajator.Text = Dami.TraduCuvant(lblNumeAngajator.Text);
            lblCUI.Text = Dami.TraduCuvant(lblCUI.Text);
            lblNationalitate.Text = Dami.TraduCuvant(lblNationalitate.Text);
            lblDataInceputDet.Text = Dami.TraduCuvant(lblDataInceputDet.Text);
            lblDataSfarsitDet.Text = Dami.TraduCuvant(lblDataSfarsitDet.Text);
            lblDataIncetareDet.Text = Dami.TraduCuvant(lblDataIncetareDet.Text);

            //string[] etichete = new string[6] { "lblNumeAngajator", "lblCUI", "lblNationalitate", "lblDataInceputDet", "lblDataSfarsitDet", "lblDataIncetareDet"};
            //for (int i = 0; i < etichete.Count(); i++)
            //{
            //    ASPxLabel lbl = Detasari_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
            //    lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            //}
            //General.SecuritatePersonal(Detasari_DataList, Convert.ToInt32(Session["UserId"].ToString()));

            txtNumeAngajator.ClientEnabled = false;
            txtCUI.ClientEnabled = false;
            cmbNationalitate.ClientEnabled = false;
            deDataInceputDet.ClientEnabled = false;
            deDataSfarsitDet.ClientEnabled = false;
            deDataIncetareDet.ClientEnabled = false;

            chk1.ClientEnabled = false;
            chk2.ClientEnabled = false;
            chk3.ClientEnabled = false;
            chk4.ClientEnabled = false;
            chk5.ClientEnabled = false;    

            string sql = @"SELECT * FROM F733 ORDER BY F73304";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F733", "F73302") + " ORDER BY F73304";
            DataTable dtDet = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colDet = (grDateDetasari.Columns["F11206"] as GridViewDataComboBoxColumn);
            colDet.PropertiesComboBox.DataSource = dtDet;

            cmbNationalitate.DataSource = dtDet;
            cmbNationalitate.DataBind();

            string[] etichete = new string[9] {  "lblNumeAngExp", "lblCUIExp", "lblMutareExp", "lblTemeiLegal", "lblDataMutare",
                                                "lblNumeAngPrel", "lblCUIPrel", "lblMutarePrel", "lblDataPrel"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Mutare_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }
            General.SecuritatePersonal(Mutare_DataList, Convert.ToInt32(Session["UserId"].ToString()));

            IncarcaGrid();

            if (IsPostBack)
            {
                txtNumeAngajator.Text = Session["MP_DetNumeAng"] == DBNull.Value  ? "" : (Session["MP_DetNumeAng"] ?? "").ToString();
                txtCUI.Text = Session["MP_DetCUI"] == DBNull.Value ? "" : (Session["MP_DetCUI"] ?? "").ToString();
                cmbNationalitate.Value = Convert.ToInt32((Session["MP_DetNation"] == DBNull.Value ? "0" : (Session["MP_DetNation"] ?? "0").ToString()).ToString());
                deDataInceputDet.Value = Convert.ToDateTime((Session["MP_DetDataIncp"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataIncp"] ?? "01/01/2100").ToString()).ToString());
                deDataSfarsitDet.Value = Convert.ToDateTime((Session["MP_DetDataSf"]  == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataSf"] ?? "01/01/2100").ToString()).ToString());
                deDataIncetareDet.Value = Convert.ToDateTime((Session["MP_DetDataInct"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataInct"] ?? "01/01/2100").ToString()).ToString());

                chk1.Checked = Convert.ToInt32((Session["MP_Det1"] == DBNull.Value ? "0" : (Session["MP_Det1"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                chk2.Checked = Convert.ToInt32((Session["MP_Det2"] == DBNull.Value ? "0" : (Session["MP_Det2"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                chk3.Checked = Convert.ToInt32((Session["MP_Det3"] == DBNull.Value ? "0" : (Session["MP_Det3"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                chk4.Checked = Convert.ToInt32((Session["MP_Det4"] == DBNull.Value ? "0" : (Session["MP_Det4"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                chk5.Checked = Convert.ToInt32((Session["MP_Det5"] == DBNull.Value ? "0" : (Session["MP_Det5"] ?? "0").ToString()).ToString()) == 1 ? true : false;

            }
        }

        protected void grDateDetasari_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGrid()
        {
            string sqlFinal = "SELECT * FROM F112 WHERE F11203 = " + HttpContext.Current.Session["Marca"].ToString() + " ORDER BY F11207";
            DataTable dt = new DataTable();
            dt = General.IncarcaDT(sqlFinal, null);
            grDateDetasari.KeyFieldName = "IdAuto";
            grDateDetasari.DataSource = dt;
            grDateDetasari.DataBind();

            Session["MP_Detasari"] = dt;    

        }

        protected void grDateDetasari_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                bool err = false;
                DataTable dtDet = Session["MP_Detasari"] as DataTable;

                if (e.NewValues["F11207"] == null || e.NewValues["F11207"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11207"].ToString()) == new DateTime(2100, 1, 1))
                    err = true;

                if (e.NewValues["F11208"] == null || e.NewValues["F11208"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11208"].ToString()) == new DateTime(2100, 1, 1))
                    err = true;

                if (dtDet != null && dtDet.Rows.Count > 0)
                {
                    DataTable dtTemp = dtDet.Select("1 = 1").OrderByDescending(x => x["F11207"]).CopyToDataTable();
                    if (Convert.ToDateTime(dtTemp.Rows[0]["F11209"] == DBNull.Value ? "01/01/2100" : dtTemp.Rows[0]["F11209"].ToString()) == new DateTime(2100, 1, 1))
                    {
                        grDateDetasari.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul are o detasare activa!");
                        err = true;
                    }
                    else
                    {
                        if (Convert.ToDateTime(dtTemp.Rows[0]["F11209"].ToString()) > Convert.ToDateTime(e.NewValues["F11207"].ToString()))
                        {
                            grDateDetasari.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Detasarea se suprapune cu cea precedenta!");
                            err = true;
                        }
                    }
                }

                if (!err)
                {
                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                    DateTime dtLuc = General.DamiDataLucru();
                    e.NewValues["F11201"] = 112;
                    e.NewValues["F11202"] = ds.Tables[0].Rows[0]["F10017"];
                    e.NewValues["F11203"] = Convert.ToInt32(Session["Marca"].ToString());
                    e.NewValues["YEAR"] = dtLuc.Year;
                    e.NewValues["MONTH"] = dtLuc.Month;
                    e.NewValues["USER_NO"] = Convert.ToInt32(Session["UserId"].ToString());
                    e.NewValues["TIME"] = DateTime.Now;                    
                    object[] row = new object[dtDet.Columns.Count];
                    int x = 0;
                    foreach (DataColumn col in dtDet.Columns)
                    {
                        if (!col.AutoIncrement)
                        {
                            switch (col.ColumnName.ToUpper())
                            {
                                case "F11201":
                                    row[x] = 111;
                                    break;
                                case "F11202":
                                    row[x] = ds.Tables[0].Rows[0]["F10017"];
                                    break;
                                case "F11203":
                                    row[x] = Convert.ToInt32(Session["Marca"].ToString());
                                    break;
                                case "F11209":
                                    if (e.NewValues["F11209"] == null)
                                        row[x] =new DateTime(2100, 1, 1);
                                    else
                                        row[x] = e.NewValues[col.ColumnName];
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

                    dtDet.Rows.Add(row);
                    e.Cancel = true;
                    grDateDetasari.CancelEdit();
                    grDateDetasari.DataSource = dtDet;
                    Session["MP_Detasari"] = dtDet;


                    General.SalveazaDate(dtDet, "F112");               

           
                    Session["InformatiaCurentaPersonal"] = ds;
                    ActualizareDet(2);
                }
                else
                {
                    e.Cancel = true;
                    grDateDetasari.CancelEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateDetasari_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
 
                if (e.NewValues["F11207"] == null || e.NewValues["F11207"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11207"].ToString()) == new DateTime(2100, 1, 1))
                {
                    e.Cancel = true;
                    grDateDetasari.CancelEdit();
                    return;
                }

                if (e.NewValues["F11208"] == null || e.NewValues["F11208"].ToString().Length < 10 || Convert.ToDateTime(e.NewValues["F11208"].ToString()) == new DateTime(2100, 1, 1))
                {
                    e.Cancel = true;
                    grDateDetasari.CancelEdit();
                    return;
                }


                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dtDet = Session["MP_Detasari"] as DataTable;
                DataRow row = dtDet.Rows.Find(keys);

                DateTime dtIncetareVeche = new DateTime(2100, 1, 1);

                foreach (DataColumn col in dtDet.Columns)
                {
                    if (!col.AutoIncrement && grDateDetasari.Columns[col.ColumnName] != null && grDateDetasari.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        if (col.ColumnName == "F11209" && row[col.ColumnName] != DBNull.Value)
                            dtIncetareVeche = Convert.ToDateTime(row[col.ColumnName].ToString());
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    if (col.ColumnName.ToUpper() == "USER_NO")
                        row[col.ColumnName] = Convert.ToInt32(Session["UserId"].ToString());
                    if (col.ColumnName.ToUpper() == "TIME")
                        row[col.ColumnName] = DateTime.Now;

                }

                e.Cancel = true;
                grDateDetasari.CancelEdit();

                grDateDetasari.DataSource = dtDet;
                Session["MP_Detasari"] = dtDet; 

                Session["InformatiaCurentaPersonal"] = ds;

                General.SalveazaDate(dtDet, "F112");
       

                ActualizareDet(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }


        private void ActualizareDet(int param)
        {
            DataTable dtDetAng = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataTable dt = Session["MP_Detasari"] as DataTable;
            if (param == 1)
            {
                dtDetAng = General.IncarcaDT("select * from f112 Where F11203 = " + Session["Marca"].ToString() + " AND (F11209 IS NULL OR F11209 = "
                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ")  ORDER BY F11207", null);

            }
            if (param == 2)
            {
                if (dt.Select("F11209 IS NULL OR F11209 = '2100-01-01'", "F11207 ASC") != null && dt.Select("F11209 IS NULL OR F11209 = '2100-01-01'", "F11207 ASC").Count() > 0)
                    dtDetAng = dt.Select("F11209 IS NULL OR F11209 = '2100-01-01'", "F11207 ASC").CopyToDataTable();
            }
            if (dtDetAng != null && dtDetAng.Rows.Count > 0)
            {
                txtNumeAngajator.Text = dtDetAng.Rows[0]["F11204"] == DBNull.Value ? "" : dtDetAng.Rows[0]["F11204"].ToString();
                txtCUI.Text = dtDetAng.Rows[0]["F11205"] == DBNull.Value ? "" : dtDetAng.Rows[0]["F11205"].ToString();
                cmbNationalitate.Value = dtDetAng.Rows[0]["F11206"];
                deDataInceputDet.Value = dtDetAng.Rows[0]["F11207"];
                deDataSfarsitDet.Value = dtDetAng.Rows[0]["F11208"];
                deDataIncetareDet.Value = dtDetAng.Rows[0]["F11209"];

                chk1.Checked = dtDetAng.Rows[0]["F11210"] == DBNull.Value ? false : (Convert.ToInt32(dtDetAng.Rows[0]["F11210"].ToString()) == 1 ? true : false);
                chk2.Checked = dtDetAng.Rows[0]["F11211"] == DBNull.Value ? false : (Convert.ToInt32(dtDetAng.Rows[0]["F11211"].ToString()) == 1 ? true : false);
                chk3.Checked = dtDetAng.Rows[0]["F11212"] == DBNull.Value ? false : (Convert.ToInt32(dtDetAng.Rows[0]["F11212"].ToString()) == 1 ? true : false);
                chk4.Checked = dtDetAng.Rows[0]["F11213"] == DBNull.Value ? false : (Convert.ToInt32(dtDetAng.Rows[0]["F11213"].ToString()) == 1 ? true : false);
                chk5.Checked = dtDetAng.Rows[0]["F11214"] == DBNull.Value ? false : (Convert.ToInt32(dtDetAng.Rows[0]["F11214"].ToString()) == 1 ? true : false);

                if (param == 2)
                {
                    Session["MP_DetNumeAng"] = dtDetAng.Rows[0]["F11204"];
                    Session["MP_DetCUI"] = dtDetAng.Rows[0]["F11205"];
                    Session["MP_DetNation"] = dtDetAng.Rows[0]["F11206"];
                    Session["MP_DetDataIncp"] = dtDetAng.Rows[0]["F11207"];
                    Session["MP_DetDataSf"] = dtDetAng.Rows[0]["F11208"];
                    Session["MP_DetDataInct"] = dtDetAng.Rows[0]["F11209"];

                    Session["MP_Det1"] = dtDetAng.Rows[0]["F11210"];
                    Session["MP_Det2"] = dtDetAng.Rows[0]["F11211"];
                    Session["MP_Det3"] = dtDetAng.Rows[0]["F11212"];
                    Session["MP_Det4"] = dtDetAng.Rows[0]["F11213"];
                    Session["MP_Det5"] = dtDetAng.Rows[0]["F11214"];
                }
                ds.Tables[0].Rows[0]["F100915"] = dtDetAng.Rows[0]["F11207"];
                ds.Tables[0].Rows[0]["F100916"] = dtDetAng.Rows[0]["F11208"];
                ds.Tables[0].Rows[0]["F100917"] = dtDetAng.Rows[0]["F11209"];
                ds.Tables[0].Rows[0]["F100918"] = dtDetAng.Rows[0]["F11204"];
                ds.Tables[0].Rows[0]["F100919"] = dtDetAng.Rows[0]["F11205"];
                ds.Tables[0].Rows[0]["F100920"] = dtDetAng.Rows[0]["F11206"];
                ds.Tables[0].Rows[0]["F1001125"] = dtDetAng.Rows[0]["F11210"];
                ds.Tables[0].Rows[0]["F1001126"] = dtDetAng.Rows[0]["F11211"];
                ds.Tables[0].Rows[0]["F1001127"] = dtDetAng.Rows[0]["F11212"];
                ds.Tables[0].Rows[0]["F1001128"] = dtDetAng.Rows[0]["F11213"];
                ds.Tables[0].Rows[0]["F1001129"] = dtDetAng.Rows[0]["F11214"];


                ds.Tables[1].Rows[0]["F100915"] = dtDetAng.Rows[0]["F11207"];
                ds.Tables[1].Rows[0]["F100916"] = dtDetAng.Rows[0]["F11208"];
                ds.Tables[1].Rows[0]["F100917"] = dtDetAng.Rows[0]["F11209"];
                ds.Tables[1].Rows[0]["F100918"] = dtDetAng.Rows[0]["F11204"];
                ds.Tables[1].Rows[0]["F100919"] = dtDetAng.Rows[0]["F11205"];
                ds.Tables[1].Rows[0]["F100920"] = dtDetAng.Rows[0]["F11206"];

                ds.Tables[2].Rows[0]["F1001125"] = dtDetAng.Rows[0]["F11210"];
                ds.Tables[2].Rows[0]["F1001126"] = dtDetAng.Rows[0]["F11211"];
                ds.Tables[2].Rows[0]["F1001127"] = dtDetAng.Rows[0]["F11212"];
                ds.Tables[2].Rows[0]["F1001128"] = dtDetAng.Rows[0]["F11213"];
                ds.Tables[2].Rows[0]["F1001129"] = dtDetAng.Rows[0]["F11214"];

            }
            else
            {
                if (param == 1)
                {
                    txtNumeAngajator.Text = ds.Tables[0].Rows[0]["F100918"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["F100918"].ToString();
                    txtCUI.Text = ds.Tables[0].Rows[0]["F100919"] == DBNull.Value ? "" : ds.Tables[0].Rows[0]["F100919"].ToString();
                    cmbNationalitate.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["F100920"] as int? ?? 0);
                    deDataInceputDet.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100915"] as DateTime? ?? new DateTime(2100, 1, 1));
                    deDataSfarsitDet.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100916"] as DateTime? ?? new DateTime(2100, 1, 1));
                    deDataIncetareDet.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["F100917"] as DateTime? ?? new DateTime(2100, 1, 1));

                    chk1.Checked = ds.Tables[0].Rows[0]["F1001125"] == DBNull.Value ? false : (Convert.ToInt32(ds.Tables[0].Rows[0]["F1001125"].ToString()) == 1 ? true : false);
                    chk2.Checked = ds.Tables[0].Rows[0]["F1001126"] == DBNull.Value ? false : (Convert.ToInt32(ds.Tables[0].Rows[0]["F1001126"].ToString()) == 1 ? true : false);
                    chk3.Checked = ds.Tables[0].Rows[0]["F1001127"] == DBNull.Value ? false : (Convert.ToInt32(ds.Tables[0].Rows[0]["F1001127"].ToString()) == 1 ? true : false);
                    chk4.Checked = ds.Tables[0].Rows[0]["F1001128"] == DBNull.Value ? false : (Convert.ToInt32(ds.Tables[0].Rows[0]["F1001128"].ToString()) == 1 ? true : false);
                    chk5.Checked = ds.Tables[0].Rows[0]["F1001129"] == DBNull.Value ? false : (Convert.ToInt32(ds.Tables[0].Rows[0]["F1001129"].ToString()) == 1 ? true : false);
                }
                else
                {  

                    if (dt.Select("1 = 1", "F11207 DESC") != null && dt.Select("1 = 1", "F11207 DESC").Count() > 0)
                    {     

                        Session["MP_DetNumeAng"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11204"];
                        Session["MP_DetCUI"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11205"];
                        Session["MP_DetNation"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11206"];
                        Session["MP_DetDataIncp"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11207"];
                        Session["MP_DetDataSf"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11208"];
                        Session["MP_DetDataInct"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11209"];

                        Session["MP_Det1"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11210"];
                        Session["MP_Det2"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11211"];
                        Session["MP_Det3"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11212"];
                        Session["MP_Det4"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11213"];
                        Session["MP_Det5"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11214"];

                        ds.Tables[0].Rows[0]["F100915"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11207"];
                        ds.Tables[0].Rows[0]["F100916"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11208"];
                        ds.Tables[0].Rows[0]["F100917"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11209"];
                        ds.Tables[0].Rows[0]["F100918"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11204"];
                        ds.Tables[0].Rows[0]["F100919"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11205"];
                        ds.Tables[0].Rows[0]["F100920"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11206"];
                        ds.Tables[0].Rows[0]["F1001125"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11210"];
                        ds.Tables[0].Rows[0]["F1001126"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11211"];
                        ds.Tables[0].Rows[0]["F1001127"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11212"];
                        ds.Tables[0].Rows[0]["F1001128"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11213"];
                        ds.Tables[0].Rows[0]["F1001129"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11214"];


                        ds.Tables[1].Rows[0]["F100915"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11207"];
                        ds.Tables[1].Rows[0]["F100916"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11208"];
                        ds.Tables[1].Rows[0]["F100917"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11209"];
                        ds.Tables[1].Rows[0]["F100918"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11204"];
                        ds.Tables[1].Rows[0]["F100919"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11205"];
                        ds.Tables[1].Rows[0]["F100920"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11206"];

                        ds.Tables[2].Rows[0]["F1001125"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11210"];
                        ds.Tables[2].Rows[0]["F1001126"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11211"];
                        ds.Tables[2].Rows[0]["F1001127"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11212"];
                        ds.Tables[2].Rows[0]["F1001128"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11213"];
                        ds.Tables[2].Rows[0]["F1001129"] = dt.Select("1 = 1", "F11207 DESC").CopyToDataTable().Rows[0]["F11214"];


                    }
                }  


            }
        }

        protected void pnlCtlDet_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string[] param = e.Parameter.Split(';');
                switch (param[0])
                {
                    case "ActDet":
                        txtNumeAngajator.Text = Session["MP_DetNumeAng"] == DBNull.Value ? "" : (Session["MP_DetNumeAng"] ?? "").ToString();
                        txtCUI.Text = Session["MP_DetCUI"] == DBNull.Value ? "" : (Session["MP_DetCUI"] ?? "").ToString();
                        cmbNationalitate.Value = Convert.ToInt32(Session["MP_DetNation"] == DBNull.Value ? "0" : (Session["MP_DetNation"] ?? "0").ToString());
                        deDataInceputDet.Value = Convert.ToDateTime(Session["MP_DetDataIncp"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataIncp"] ?? "01/01/2100").ToString());
                        deDataSfarsitDet.Value = Convert.ToDateTime(Session["MP_DetDataSf"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataSf"] ?? "01/01/2100").ToString());
                        deDataIncetareDet.Value = Convert.ToDateTime(Session["MP_DetDataInct"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataInct"] ?? "01/01/2100").ToString());

                        chk1.Checked = Convert.ToInt32((Session["MP_Det1"] == DBNull.Value ? "0" : (Session["MP_Det1"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                        chk2.Checked = Convert.ToInt32((Session["MP_Det2"] == DBNull.Value ? "0" : (Session["MP_Det2"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                        chk3.Checked = Convert.ToInt32((Session["MP_Det3"] == DBNull.Value ? "0" : (Session["MP_Det3"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                        chk4.Checked = Convert.ToInt32((Session["MP_Det4"] == DBNull.Value ? "0" : (Session["MP_Det4"] ?? "0").ToString()).ToString()) == 1 ? true : false;
                        chk5.Checked = Convert.ToInt32((Session["MP_Det5"] == DBNull.Value ? "0" : (Session["MP_Det5"] ?? "0").ToString()).ToString()) == 1 ? true : false;

                        ds.Tables[0].Rows[0]["F100915"] = Convert.ToDateTime(Session["MP_DetDataIncp"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataIncp"] ?? "01/01/2100").ToString());
                        ds.Tables[0].Rows[0]["F100916"] = Convert.ToDateTime(Session["MP_DetDataSf"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataSf"] ?? "01/01/2100").ToString());
                        ds.Tables[0].Rows[0]["F100917"] = Convert.ToDateTime(Session["MP_DetDataInct"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataInct"] ?? "01/01/2100").ToString());
                        ds.Tables[0].Rows[0]["F100918"] = Session["MP_DetNumeAng"] == DBNull.Value ? "" : (Session["MP_DetNumeAng"] ?? "").ToString();
                        ds.Tables[0].Rows[0]["F100919"] = Session["MP_DetCUI"] == DBNull.Value ? "" : (Session["MP_DetCUI"] ?? "").ToString();
                        ds.Tables[0].Rows[0]["F100920"] = Convert.ToInt32(Session["MP_DetNation"] == DBNull.Value ? "0" : (Session["MP_DetNation"] ?? "").ToString());
                        ds.Tables[0].Rows[0]["F1001125"] = Convert.ToInt32((Session["MP_Det1"] == DBNull.Value ? "0" : (Session["MP_Det1"] ?? "0").ToString()).ToString());
                        ds.Tables[0].Rows[0]["F1001126"] = Convert.ToInt32((Session["MP_Det2"] == DBNull.Value ? "0" : (Session["MP_Det2"] ?? "0").ToString()).ToString());
                        ds.Tables[0].Rows[0]["F1001127"] = Convert.ToInt32((Session["MP_Det3"] == DBNull.Value ? "0" : (Session["MP_Det3"] ?? "0").ToString()).ToString());
                        ds.Tables[0].Rows[0]["F1001128"] = Convert.ToInt32((Session["MP_Det4"] == DBNull.Value ? "0" : (Session["MP_Det4"] ?? "0").ToString()).ToString());
                        ds.Tables[0].Rows[0]["F1001129"] = Convert.ToInt32((Session["MP_Det5"] == DBNull.Value ? "0" : (Session["MP_Det5"] ?? "0").ToString()).ToString());


                        ds.Tables[1].Rows[0]["F100915"] = Convert.ToDateTime(Session["MP_DetDataIncp"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataIncp"] ?? "01/01/2100").ToString());
                        ds.Tables[1].Rows[0]["F100916"] = Convert.ToDateTime(Session["MP_DetDataSf"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataSf"] ?? "01/01/2100").ToString());
                        ds.Tables[1].Rows[0]["F100917"] = Convert.ToDateTime(Session["MP_DetDataInct"] == DBNull.Value ? "01/01/2100" : (Session["MP_DetDataInct"] ?? "01/01/2100").ToString());
                        ds.Tables[1].Rows[0]["F100918"] = Session["MP_DetNumeAng"] == DBNull.Value ? "" : (Session["MP_DetNumeAng"] ?? "").ToString();
                        ds.Tables[1].Rows[0]["F100919"] = Session["MP_DetCUI"] == DBNull.Value ? "" : (Session["MP_DetCUI"] ?? "").ToString();
                        ds.Tables[1].Rows[0]["F100920"] = Convert.ToInt32(Session["MP_DetNation"] == DBNull.Value ? "0" : (Session["MP_DetNation"] ?? "0").ToString());

                        ds.Tables[2].Rows[0]["F1001125"] = Convert.ToInt32((Session["MP_Det1"] == DBNull.Value ? "0" : (Session["MP_Det1"] ?? "0").ToString()).ToString());
                        ds.Tables[2].Rows[0]["F1001126"] = Convert.ToInt32((Session["MP_Det2"] == DBNull.Value ? "0" : (Session["MP_Det2"] ?? "0").ToString()).ToString());
                        ds.Tables[2].Rows[0]["F1001127"] = Convert.ToInt32((Session["MP_Det3"] == DBNull.Value ? "0" : (Session["MP_Det3"] ?? "0").ToString()).ToString());
                        ds.Tables[2].Rows[0]["F1001128"] = Convert.ToInt32((Session["MP_Det4"] == DBNull.Value ? "0" : (Session["MP_Det4"] ?? "0").ToString()).ToString());
                        ds.Tables[2].Rows[0]["F1001129"] = Convert.ToInt32((Session["MP_Det5"] == DBNull.Value ? "0" : (Session["MP_Det5"] ?? "0").ToString()).ToString());               
                        break;
                }
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateDetasari_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            var grid = sender as ASPxGridView;
            e.Editor.ReadOnly = false;
            if (e.Column.FieldName == "F11206")
            {
                var tb = e.Editor as ASPxComboBox;
                tb.ClientSideEvents.SelectedIndexChanged = "OnChangedTaraDet";
            }
            if (e.Column.FieldName == "F11211")
            {
                var tb = e.Editor as ASPxCheckBox;
                tb.ClientSideEvents.CheckedChanged = "OnChangedChk2";
            }
            if (e.Column.FieldName == "F11213")
            {
                var tb = e.Editor as ASPxCheckBox;
                tb.ClientSideEvents.CheckedChanged = "OnChangedChk4";
            }
        }

    }

    //Checked='<%#Convert.ToInt32(DataBinder.GetPropertyValue(Container.DataItem,"F1001125"))==1%>'
}