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
            string valMin = "100000";
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                valMin = dtParam.Rows[0][0].ToString();

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Cartele\" a WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100Cartele2"))
            {
                dt = ds.Tables["F100Cartele2"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Cartele2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCartele.KeyFieldName = "IdAuto";
            grDateCartele.DataSource = dt;         
        }        
        

        protected void grDateCartele_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Cartele2"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100Cartele2");

                e.NewValues["Modificabil"] = 1;
                e.NewValues["DataInceput"] = DateTime.Now;

                //Florin 2019.09.26
                DataTable dtF100 = ds.Tables["F100"];
                if (dtF100 != null && dtF100.Rows.Count > 0)
                {
                    e.NewValues["DataInceput"] = dtF100.Rows[0]["F10022"];
                    e.NewValues["DataSfarsit"] = new DateTime(2100, 1, 1);
                }
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

                //if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //{
                //    try
                //    {
                //        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                //        {
                //            grDateCartele.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                //            e.Cancel = true;
                //            grDateCartele.CancelEdit();
                //            return;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

                //for (int i = 0; i <= grDateCartele.VisibleRowCount - 1; i++)
                //{
                //    object[] obj = grDateCartele.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                //    DateTime dtInc = Convert.ToDateTime(obj[0]);
                //    DateTime dtSf = Convert.ToDateTime(obj[1]);

                //    if (dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //    {
                //        try
                //        {
                //            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                //            {
                //                grDateCartele.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                //                e.Cancel = true;
                //                grDateCartele.CancelEdit();
                //                return;
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}

                object[] row = new object[ds.Tables["F100Cartele2"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Cartele2"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Cartele2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x]= Dami.NextId("F100Cartele2"); 
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

                ds.Tables["F100Cartele2"].Rows.Add(row);
                e.Cancel = true;
                grDateCartele.CancelEdit();
                grDateCartele.DataSource = ds.Tables["F100Cartele2"];
                grDateCartele.KeyFieldName = "IdAuto";            
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

                //if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //{
                //    try
                //    {
                //        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                //        {
                //            grDateCartele.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                //            e.Cancel = true;
                //            grDateCartele.CancelEdit();
                //            return;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

                //for (int i = 0; i <= grDateCartele.VisibleRowCount - 1; i++)
                //{
                //    object[] obj = grDateCartele.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                //    DateTime dtInc = Convert.ToDateTime(obj[0]);
                //    DateTime dtSf = Convert.ToDateTime(obj[1]);

                //    if (grDateCartele.EditingRowVisibleIndex != i && dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //    {
                //        try
                //        {
                //            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                //            {
                //                grDateCartele.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                //                e.Cancel = true;
                //                grDateCartele.CancelEdit();
                //                return;
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Cartele2"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Cartele2"].Columns)
                {
                    if (!col.AutoIncrement && grDateCartele.Columns[col.ColumnName] != null && grDateCartele.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateCartele.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCartele.DataSource = ds.Tables["F100Cartele2"];
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

                DataRow row = ds.Tables["F100Cartele2"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCartele.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCartele.DataSource = ds.Tables["F100Cartele2"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCartele_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDateCartele.GetRow(e.VisibleIndex) as DataRowView;

                if (values != null)
                {
                    string modif = values.Row["Modificabil"].ToString();

                    if (modif == "0")
                    {
                        if (e.ButtonType == ColumnCommandButtonType.Edit || e.ButtonType == ColumnCommandButtonType.Delete)

                            e.Visible = false;

                    }
                }
            }
        }


    }
}