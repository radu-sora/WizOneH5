using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.IO;
using System.Diagnostics;

namespace WizOne.Personal
{
    public partial class Echipamente : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateEchipamente.DataBind();
            //grDateEchipamente.AddNewRow();

            foreach (dynamic c in grDateEchipamente.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateEchipamente.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateEchipamente.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateEchipamente.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateEchipamente.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateEchipamente.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateEchipamente);
        }

        protected void grDateEchipamente_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGrid()
        {

            string sqlFinal = "SELECT * FROM \"Admin_Echipamente\" WHERE \"Marca\" = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Echipamente"))
            {
                dt = ds.Tables["Admin_Echipamente"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Echipamente";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateEchipamente.KeyFieldName = "IdAuto";
            grDateEchipamente.DataSource = dt;

            DataTable dtEchip = General.GetObiecteDinArie("ArieTabEchipamenteDinPersonal");
            GridViewDataComboBoxColumn colEchip = (grDateEchipamente.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            colEchip.PropertiesComboBox.DataSource = dtEchip;

            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;

        }

        protected void grDateEchipamente_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Echipamente"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Echipamente");

                e.NewValues["DataPrimire"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDateEchipamente_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["Admin_Echipamente"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Admin_Echipamente"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "MARCA":
                                row[x] = Session["Marca"];
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Echipamente"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("Admin_Echipamente");
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DURATAUTILIZARE":
                                int nrAni = 0, nrLuni = 0, nrZile = 0;
                                CalculVechime(Convert.ToDateTime(e.NewValues["DataPrimire"]).Date, Convert.ToDateTime(e.NewValues["DataExpirare"]).Date, out nrAni, out nrLuni, out nrZile);
                                string vechime = " {0} {1} {2} {3} {4} {5} ";
                                vechime = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                                 (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                                 (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : ""));
                                row[x] = vechime;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                ds.Tables["Admin_Echipamente"].Rows.Add(row);
                e.Cancel = true;
                grDateEchipamente.CancelEdit();
                grDateEchipamente.DataSource = ds.Tables["Admin_Echipamente"];
                grDateEchipamente.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateEchipamente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Echipamente"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Admin_Echipamente"].Columns)
                {
                    if (!col.AutoIncrement)
                    {      
                        if (col.ColumnName.ToUpper() == "DURATAUTILIZARE")
                        {
                            int nrAni = 0, nrLuni = 0, nrZile = 0;
                            CalculVechime(Convert.ToDateTime(e.NewValues["DataPrimire"]).Date, Convert.ToDateTime(e.NewValues["DataExpirare"]).Date, out nrAni, out nrLuni, out nrZile);
                            string vechime = " {0} {1} {2} {3} {4} {5} ";
                            vechime = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                             (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                             (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : ""));
                            row[col.ColumnName] = vechime;
                        }
                        else
                        {
                            var edc = e.NewValues[col.ColumnName];
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                        }
                    }

                }

                e.Cancel = true;
                grDateEchipamente.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateEchipamente.DataSource = ds.Tables["Admin_Echipamente"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateEchipamente_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Echipamente"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateEchipamente.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateEchipamente.DataSource = ds.Tables["Admin_Echipamente"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateEchipamente_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "DurataUtil")
            {
                int nrAni = 0, nrLuni = 0, nrZile = 0;
                string vechime = " {0} {1} {2} {3} {4} {5} ";
                DateTime dataStart = DateTime.Now, dataSfarsit = DateTime.Now;

                if (e.GetListSourceFieldValue("DataPrimire") != null && e.GetListSourceFieldValue("DataExpirare") != null && e.GetListSourceFieldValue("DataPrimire").ToString().Length > 0 && e.GetListSourceFieldValue("DataExpirare").ToString().Length > 0)
                {
                    DateTime dtStart = Convert.ToDateTime(e.GetListSourceFieldValue("DataPrimire").ToString());
                    DateTime dtSfarsit = Convert.ToDateTime(e.GetListSourceFieldValue("DataExpirare").ToString());

                    dataStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
                    dataSfarsit = new DateTime(dtSfarsit.Year, dtSfarsit.Month, dtSfarsit.Day);

                    CalculVechime(dataStart, dataSfarsit, out nrAni, out nrLuni, out nrZile);

                    vechime = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                        (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                        (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : ""));

                    e.Value = vechime;
                    
                }
            }
        }

        private void CalculVechime(DateTime dataStart, DateTime dataSfarsit, out int nrAni, out int nrLuni, out int nrZile)
        {
            DateTime odtT1, odtT2, odtD;
            List<int> arNrZileInLuna = new List<int>();

            // determin nr zile calendaristice in luna:
            odtT1 = new DateTime(dataStart.Year, dataStart.Month, 1, 0, 0, 0);
            odtT2 = new DateTime(dataSfarsit.Year, dataSfarsit.Month, 1, 0, 0, 0);
            for (DateTime odtDt = odtT1; odtDt <= odtT2;)
            {
                odtD = new DateTime(
                    odtDt.Month == 12 ? odtDt.Year + 1 : odtDt.Year,
                    odtDt.Month == 12 ? 1 : odtDt.Month + 1, 1, 0, 0, 0);

                int odtsDf = (int)(odtD - odtDt).TotalDays;
                arNrZileInLuna.Add((int)(odtsDf));
                odtDt = odtD;
            }

            nrLuni = 0;
            nrZile = (int)(dataSfarsit - dataStart).TotalDays + 1;

            for (int nI = 0; nI < arNrZileInLuna.Count && nrZile >= arNrZileInLuna[nI]; nI++)
            {
                nrZile -= arNrZileInLuna[nI];
                nrLuni++;
            }

            nrAni = 0;
            if (nrLuni > 12)
            {
                nrAni = nrLuni / 12;
                nrLuni = nrLuni % 12;
            }
        }

    }
}