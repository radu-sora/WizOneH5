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
    public partial class Experienta : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateExperienta.DataBind();
            //grDateExperienta.AddNewRow();
            foreach (dynamic c in grDateExperienta.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateExperienta.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateExperienta.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateExperienta.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateExperienta.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateExperienta.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
        }

        protected void grDateExperienta_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"Admin_Experienta\" WHERE \"Marca\" = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Experienta"))
            {
                dt = ds.Tables["Admin_Experienta"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Experienta";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateExperienta.KeyFieldName = "IdAuto";
            grDateExperienta.DataSource = dt;

            DataTable dtExp = General.GetObiecteDinArie("ArieTabExperientaDinPersonal");
            GridViewDataComboBoxColumn colExp = (grDateExperienta.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            colExp.PropertiesComboBox.DataSource = dtExp;



        }

        protected void grDateExperienta_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Experienta"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Experienta");

                e.NewValues["DataInceput"] = DateTime.Now;
            }
            catch (Exception ex)
            {

            }
        }

        

        protected void grDateExperienta_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["Admin_Experienta"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Admin_Experienta"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Experienta"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("Admin_Experienta");
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
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

                ds.Tables["Admin_Experienta"].Rows.Add(row);
                e.Cancel = true;
                grDateExperienta.CancelEdit();
                grDateExperienta.DataSource = ds.Tables["Admin_Experienta"];
                grDateExperienta.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateExperienta_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Experienta"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Admin_Experienta"].Columns)
                {
                    if (!col.AutoIncrement && grDateExperienta.Columns[col.ColumnName] != null && grDateExperienta.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateExperienta.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateExperienta.DataSource = ds.Tables["Admin_Experienta"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateExperienta_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Experienta"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateExperienta.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateExperienta.DataSource = ds.Tables["Admin_Experienta"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateExperienta_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Vechime")
            {
                int nrAni = 0, nrLuni = 0, nrZile = 0;
                string vechime = " {0} {1} {2} {3} {4} {5} ";
                DateTime dataStart = DateTime.Now, dataSfarsit = DateTime.Now;

                string dtStart = e.GetListSourceFieldValue("DataInceput").ToString();
                string dtSfarsit = e.GetListSourceFieldValue("DataSfarsit").ToString();

                dataStart = new DateTime(Convert.ToInt32(dtStart.Substring(6, 4)), Convert.ToInt32(dtStart.Substring(3, 2)), Convert.ToInt32(dtStart.Substring(0, 2)));
                dataSfarsit = new DateTime(Convert.ToInt32(dtSfarsit.Substring(6, 4)), Convert.ToInt32(dtSfarsit.Substring(3, 2)), Convert.ToInt32(dtSfarsit.Substring(0, 2)));

                CalculVechime(dataStart, dataSfarsit, out nrAni, out nrLuni, out nrZile);

                vechime = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                 (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                 (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : ""));

                e.Value = vechime;
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