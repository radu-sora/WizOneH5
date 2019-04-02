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
    public partial class Cartele : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateCartele.DataBind();
            //grDateCartele.AddNewRow();
            foreach (dynamic c in grDateCartele.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateCartele.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateCartele.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateCartele.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateCartele.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateCartele.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
        }

        protected void grDateCartele_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"F100Cartele\" WHERE F10003 = " + Session["Marca"].ToString();
            if (Constante.tipBD == 2)
                sqlFinal = "SELECT " + General.SelectListaCampuriOracle("F100Cartele", "F10003") + " FROM \"F100Cartele\" WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100Cartele"))
            {
                dt = ds.Tables["F100Cartele"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Cartele";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCartele.KeyFieldName = "IdAuto";
            grDateCartele.DataSource = dt;

            DataTable dtAng = General.GetF100NumeComplet();
            GridViewDataComboBoxColumn colAng = (grDateCartele.Columns["F10003"] as GridViewDataComboBoxColumn);
            colAng.PropertiesComboBox.DataSource = dtAng;



        }

        protected void grDateCartele_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Cartele"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100Cartele");
            }
            catch (Exception ex)
            {

            }
        }



        protected void grDateCartele_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["F100Cartele"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Cartele"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F10003":
                                row[x] = Session["Marca"];
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Cartele"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x]= Dami.NextId("F100Cartele"); 
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

                ds.Tables["F100Cartele"].Rows.Add(row);
                e.Cancel = true;
                grDateCartele.CancelEdit();
                grDateCartele.DataSource = ds.Tables["F100Cartele"];
                grDateCartele.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCartele_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Cartele"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Cartele"].Columns)
                {
                    if (!col.AutoIncrement && grDateCartele.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateCartele.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCartele.DataSource = ds.Tables["F100Cartele"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCartele_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Cartele"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCartele.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCartele.DataSource = ds.Tables["F100Cartele"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}