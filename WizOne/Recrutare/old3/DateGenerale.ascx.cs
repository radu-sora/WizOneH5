using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;

namespace WizOne.Recrutare
{
    public partial class DateGenerale : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtCan = GeneralRec.IncarcaDT("SELECT * FROM Rec_tblCanale", null);
            cmbCanal.DataSource = dtCan;
            cmbCanal.DataBind();

            frmGen.DataSource = (DataTable)Session["InformatiaCurenta"];
            frmGen.DataBind();

            //grDateExperienta.DataBind();
            ////grDateExperienta.AddNewRow();
            //foreach (dynamic c in grDateExperienta.Columns)
            //{
            //    try
            //    {
            //        c.Caption = Dami.TraduCuvant(c.Caption);
            //    }
            //    catch (Exception) { }
            //}
            //grDateExperienta.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            //grDateExperienta.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            //grDateExperienta.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            //grDateExperienta.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            //grDateExperienta.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
        }

        //protected void Page_Unload(object sender, EventArgs e)
        //{
        //    DataTable dt22 = (DataTable)Session["InformatiaCurenta"];
        //    dt22.Rows[0]["Localitate"] = txtLoc.Value;
        //    Session["InformatiaCurenta"] = dt22;
        //}

        protected void cmbCanal_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Rec_tblCanale""", null);
                cmbCanal.DataSource = dt;
                cmbCanal.DataBind();
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
            //grDateExperienta.KeyFieldName = "IdAuto";
            //grDateExperienta.DataSource = dt;

            //DataTable dtExp = General.GetObiecteDinArie("ArieTabExperientaDinPersonal");
            //GridViewDataComboBoxColumn colExp = (grDateExperienta.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            //colExp.PropertiesComboBox.DataSource = dtExp;



        }

        protected void grDateExperienta_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Experienta"];
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
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Experienta"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
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
                //grDateExperienta.CancelEdit();
                //grDateExperienta.DataSource = ds.Tables["Admin_Experienta"];
                //grDateExperienta.KeyFieldName = "IdAuto";
                ////grDateBeneficii.AddNewRow();
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

                //foreach (DataColumn col in ds.Tables["Admin_Experienta"].Columns)
                //{
                //    if (!col.AutoIncrement && grDateExperienta.Columns[col.ColumnName].Visible)
                //    {
                //        var edc = e.NewValues[col.ColumnName];
                //        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                //    }

                //}

                //e.Cancel = true;
                //grDateExperienta.CancelEdit();
                //Session["InformatiaCurentaPersonal"] = ds;
                //grDateExperienta.DataSource = ds.Tables["Admin_Experienta"];
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
                //grDateExperienta.CancelEdit();
                //Session["InformatiaCurentaPersonal"] = ds;
                //grDateExperienta.DataSource = ds.Tables["Admin_Experienta"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void btnUpdate_Click(object sender, EventArgs e)
        {
            DataTable dt22 = (DataTable)Session["InformatiaCurenta"];
            dt22.Rows[0]["Localitate"] = txtLoc.Value;
            Session["InformatiaCurenta"] = dt22;
        }


    }
}