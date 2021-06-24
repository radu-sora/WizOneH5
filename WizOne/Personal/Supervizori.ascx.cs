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
    public partial class Supervizori : System.Web.UI.UserControl
    {
        //DataTable dt = new DataTable();
        protected void Page_Init(object sender, EventArgs e)
        {

            
            grDateSupervizori.DataBind();
            //grDateSupervizori.AddNewRow();
            foreach (dynamic c in grDateSupervizori.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateSupervizori.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateSupervizori.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateSupervizori.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateSupervizori.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateSupervizori.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateSupervizori);
        }

        protected void grDateSupervizori_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Supervizori\" a WHERE F10003 = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100Supervizori2"))
            {
                dt = ds.Tables["F100Supervizori2"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Supervizori2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }

            
            grDateSupervizori.KeyFieldName = "IdAuto";
            grDateSupervizori.DataSource = dt;
            //grDateSupervizori.DataBind();

            DataTable dtSuper = General.IncarcaDT(@"SELECT CAST(""Id"" AS INT) AS ""Id"", CASE WHEN ""Alias"" IS NULL THEN ""Denumire"" ELSE ""Alias"" END AS ""Denumire"" FROM ""tblSupervizori"" ORDER BY ""Denumire""", null);
            GridViewDataComboBoxColumn colSuper = (grDateSupervizori.Columns["IdSuper"] as GridViewDataComboBoxColumn);
            colSuper.PropertiesComboBox.DataSource = dtSuper;

            string sql = @"SELECT * FROM USERS ORDER BY F70104";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("USERS", "F70102") + " ORDER BY F70104";
            DataTable dtUser = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colUser = (grDateSupervizori.Columns["IdUser"] as GridViewDataComboBoxColumn);
            colUser.PropertiesComboBox.DataSource = dtUser;

            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;
        }

        protected void grDateSupervizori_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Supervizori2"];

                //string sqlFinal = "SELECT * FROM \"F100Supervizori2\" WHERE F10003 = " + Session["Marca"].ToString();
                //dt = General.IncarcaDT(sqlFinal, null);
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100Supervizori2");

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
            catch (Exception)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

   
        protected void grDateSupervizori_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
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
                //            grDateSupervizori.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                //            e.Cancel = true;
                //            grDateSupervizori.CancelEdit();
                //            return;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

                //for (int i = 0; i <= grDateSupervizori.VisibleRowCount - 1; i++)
                //{
                //    object[] obj = grDateSupervizori.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                //    DateTime dtInc = Convert.ToDateTime(obj[0]);
                //    DateTime dtSf = Convert.ToDateTime(obj[1]);

                //    if (grDateSupervizori.EditingRowVisibleIndex != i && dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //    {
                //        try
                //        {
                //            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                //            {
                //                grDateSupervizori.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                //                e.Cancel = true;
                //                grDateSupervizori.CancelEdit();
                //                return;
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                
                DataRow row = ds.Tables["F100Supervizori2"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Supervizori2"].Columns)
                {
                    if (!col.AutoIncrement && grDateSupervizori.Columns[col.ColumnName] != null && grDateSupervizori.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateSupervizori.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateSupervizori.DataSource = ds.Tables["F100Supervizori2"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void grDateSupervizori_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
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
                //            grDateSupervizori.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                //            e.Cancel = true;
                //            grDateSupervizori.CancelEdit();
                //            return;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

                //for (int i = 0; i <= grDateSupervizori.VisibleRowCount - 1; i++)
                //{
                //    object[] obj = grDateSupervizori.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                //    DateTime dtInc = Convert.ToDateTime(obj[0]);
                //    DateTime dtSf = Convert.ToDateTime(obj[1]);

                //    if (dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //    {
                //        try
                //        {
                //            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                //            {
                //                grDateSupervizori.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                //                e.Cancel = true;
                //                grDateSupervizori.CancelEdit();
                //                return;
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}

                //string sqlFinal = "SELECT * FROM \"F100Supervizori2\" WHERE F10003 = " + Session["Marca"].ToString();
                //dt = General.IncarcaDT(sqlFinal, null);
                object[] row = new object[ds.Tables["F100Supervizori2"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Supervizori2"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F10003":
                                row[x] = Session["Marca"];
                                break;
                            case "NUMECOMPLET":
                                row[x] = ds.Tables["F100"].Rows[0]["F10008"].ToString() + " " + ds.Tables["F100"].Rows[0]["F10009"].ToString();
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Supervizori2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F100Supervizori2");
                                break;
                            case "DATAINCEPUT":
                                //row[x] = new DateTime(1900, 1, 1);
                                break;
                            case "DATASFARSIT":
                                //row[x] = new DateTime(2100, 1, 1);
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

                ds.Tables["F100Supervizori2"].Rows.Add(row);
                e.Cancel = true;
                grDateSupervizori.CancelEdit();
                //Session["DateAngajat"] = dt;
                grDateSupervizori.DataSource = ds.Tables["F100Supervizori2"];
                grDateSupervizori.KeyFieldName = "IdAuto";
                //grDateSupervizori.DataBind();
                //grDateSupervizori.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSupervizori_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Supervizori2"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateSupervizori.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateSupervizori.DataSource = ds.Tables["F100Supervizori2"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSupervizori_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDateSupervizori.GetRow(e.VisibleIndex) as DataRowView;

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

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //<dx:GridViewCommandColumn Width = "75px" VisibleIndex="0" ButtonType="Image" ShowEditButton="true" Caption=" " >
        //    <CustomButtons>
        //        <dx:GridViewCommandColumnCustomButton ID = "btnEdit" >
        //            < Image ToolTip="Modifica" Url="~/Fisiere/Imagini/Icoane/edit.png" />
        //        </dx:GridViewCommandColumnCustomButton>
        //    </CustomButtons>
        //    <CustomButtons>
        //        <dx:GridViewCommandColumnCustomButton ID = "btnDelete" >
        //            < Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
        //        </dx:GridViewCommandColumnCustomButton>
        //    </CustomButtons>
        //</dx:GridViewCommandColumn> 

    }
}