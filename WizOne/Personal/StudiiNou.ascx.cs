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
    public partial class StudiiNou : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateStudii.DataBind();
            foreach (dynamic c in grDateStudii.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateStudii.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateStudii.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateStudii.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateStudii.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateStudii.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateStudii);
        }

        protected void grDateStudii_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"F100Studii\" WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100Studii"))
            {
                dt = ds.Tables["F100Studii"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Studii";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateStudii.KeyFieldName = "IdAuto";
            grDateStudii.DataSource = dt;

            DataTable dtTipInv = General.IncarcaDT("Select * FROM \"tblTipInvatamant\"", null);
            GridViewDataComboBoxColumn colTipInv = (grDateStudii.Columns["IdTipInvatamant"] as GridViewDataComboBoxColumn);
            colTipInv.PropertiesComboBox.DataSource = dtTipInv;

            DataTable dtNiv = General.IncarcaDT("Select * FROM \"tblNivelStudii\"", null);
            GridViewDataComboBoxColumn colNiv = (grDateStudii.Columns["IdNivel"] as GridViewDataComboBoxColumn);
            colNiv.PropertiesComboBox.DataSource = dtNiv;

            DataTable dtTipInst = General.IncarcaDT("Select * FROM \"tblTipInstitutie\"", null);
            GridViewDataComboBoxColumn colTipInst = (grDateStudii.Columns["IdTipInstitutie"] as GridViewDataComboBoxColumn);
            colTipInst.PropertiesComboBox.DataSource = dtTipInst;

            DataTable dtProfil = General.IncarcaDT("Select * FROM \"tblProfilStudii\"", null);
            GridViewDataComboBoxColumn colProfil = (grDateStudii.Columns["IdProfil"] as GridViewDataComboBoxColumn);
            colProfil.PropertiesComboBox.DataSource = dtProfil;

            DataTable dtDom = General.IncarcaDT("Select * FROM \"tblDomeniuStudii\"", null);
            GridViewDataComboBoxColumn colDom = (grDateStudii.Columns["IdDomeniu"] as GridViewDataComboBoxColumn);
            colDom.PropertiesComboBox.DataSource = dtDom;

            DataTable dtLoc = General.IncarcaDT("select a.siruta, a.denloc as \"Nivel3\", b.denloc as \"Nivel2\", c.denloc as \"Nivel1\" from localitati a "
                                            + "left  join Localitati b on a.sirsup = b.siruta "
                                            + "left join Localitati c on b.SIRSUP = c.SIRUTA "
                                            + "where a.niv = 3", null);
            GridViewDataComboBoxColumn colLoc = (grDateStudii.Columns["SirutaLocalitate"] as GridViewDataComboBoxColumn);
            colLoc.PropertiesComboBox.DataSource = dtLoc;


        }

        protected void grDateStudii_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Studii"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100Studii");

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        

        protected void grDateStudii_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["F100Studii"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Studii"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Studii"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F100Studii");
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

                ds.Tables["F100Studii"].Rows.Add(row);
                e.Cancel = true;
                grDateStudii.CancelEdit();
                grDateStudii.DataSource = ds.Tables["F100Studii"];
                grDateStudii.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateStudii_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Studii"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Studii"].Columns)
                {
                    if (!col.AutoIncrement && grDateStudii.Columns[col.ColumnName] != null && grDateStudii.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateStudii.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateStudii.DataSource = ds.Tables["F100Studii"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateStudii_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Studii"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateStudii.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateStudii.DataSource = ds.Tables["F100Studii"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateStudii_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Perioada")
            {
                int nrAni = 0, nrLuni = 0;
                string perioada = " {0} {1} {2} {3} ";
                DateTime dataStart = DateTime.Now, dataSfarsit = DateTime.Now;

                string dtStart = e.GetListSourceFieldValue("DeLaData").ToString();
                string dtSfarsit = e.GetListSourceFieldValue("LaData").ToString();

                dataStart = new DateTime(Convert.ToInt32(dtStart.Substring(6, 4)), Convert.ToInt32(dtStart.Substring(3, 2)), Convert.ToInt32(dtStart.Substring(0, 2)));
                dataSfarsit = new DateTime(Convert.ToInt32(dtSfarsit.Substring(6, 4)), Convert.ToInt32(dtSfarsit.Substring(3, 2)), Convert.ToInt32(dtSfarsit.Substring(0, 2)));

                CalculPerioada(dataStart, dataSfarsit, out nrAni, out nrLuni);

                perioada = string.Format(perioada, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                 (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""));

                e.Value = perioada;
            }
        }



        private void CalculPerioada(DateTime dataStart, DateTime dataSfarsit, out int nrAni, out int nrLuni)
        {
            DateTime odtT1, odtT2, odtD;
            List<int> arNrZileInLuna = new List<int>();

            int nrZile = 0;

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