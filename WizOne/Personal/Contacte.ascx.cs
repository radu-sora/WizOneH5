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
    public partial class Contacte : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateContacte.DataBind();
            //grDateContacte.AddNewRow();
            foreach (dynamic c in grDateContacte.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateContacte.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateContacte.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateContacte.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateContacte.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateContacte.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

        }

        protected void grDateContacte_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"F100Contacte\" WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100Contacte"))
            {
                dt = ds.Tables["F100Contacte"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Contacte";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateContacte.KeyFieldName = "IdAuto";
            grDateContacte.DataSource = dt;

            string sql = @"SELECT * FROM ""tblContacte"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("tblContacte", "Id");
            DataTable dtContact = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colContact = (grDateContacte.Columns["IdContact"] as GridViewDataComboBoxColumn);
            colContact.PropertiesComboBox.DataSource = dtContact;

            sql = @"SELECT * FROM LOCATII ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("LOCATII", "NUMAR");
            DataTable dtLocatii = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colLocatii = (grDateContacte.Columns["IdLocatie"] as GridViewDataComboBoxColumn);
            colLocatii.PropertiesComboBox.DataSource = dtLocatii;

            GridViewDataComboBoxColumn colPrincipal = (grDateContacte.Columns["EstePrincipal"] as GridViewDataComboBoxColumn);
            colPrincipal.PropertiesComboBox.DataSource = General.ListaContacteF100();



        }

        protected void grDateContacte_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Contacte"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100Contacte");
            }
            catch (Exception ex)
            {

            }
        }



        protected void grDateContacte_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string result = "", valoare = "";
                object[] row = new object[ds.Tables["F100Contacte"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Contacte"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Contacte"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x]= Dami.NextId("F100Contacte");
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "VALOARE":
                                valoare = e.NewValues[col.ColumnName].ToString();
                                char[] trimedChars = { '.', ' ', '\'', '@', '-', '*', '#', '(', ')', '+', '/' };
                                string originalString = e.NewValues[col.ColumnName].ToString();
                                string[] splitedChars = originalString.Split(trimedChars);                                
                                foreach (string s in splitedChars)
                                {
                                    result += s;
                                }
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                x = 0;
                int estePrincipal = 0, idContact = 0;
                foreach (DataColumn col in ds.Tables["F100Contacte"].Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDLOCATIE":
                            if (row[x] != null && row[x].ToString() != "0")                            
                                ds.Tables[2].Rows[0]["F100966"] = row[x];                            
                            break;
                        case "INTERIOR":
                            if (row[x] != null && row[x].ToString().Length > 0)                            
                                ds.Tables[2].Rows[0]["F100965"] = row[x];                            
                            break;
                        case "ESTEPRINCIPAL":
                            if (row[x] != null && row[x].ToString().Length > 0)
                                estePrincipal = Convert.ToInt32(row[x].ToString());
                            break;
                        case "IDCONTACT":
                            idContact = Convert.ToInt32(row[x].ToString());
                            break;
                        case "VALOARESTRING":
                            row[x] = result;
                            break;
                        case "PAROLAPDF":
                            if (row[x] != null && row[x].ToString().Length > 0)
                                ds.Tables[1].Rows[0]["F10016"] = row[x].ToString();
                            break;
                    }
                    x++;
                }

                switch (estePrincipal)
                {
                    // Telefon 1
                    case 1:                        
                            // daca nu e email...
                            if (idContact != 2 && idContact != 0)
                            {
                                // telefon 1
                                if (result.Length > 0)
                                    ds.Tables[1].Rows[0]["F10088"] = result;
                            }  
                        break;
                    // Telefon 2
                    case 2:                       
                            // daca nu e email...
                            if (idContact != 2 && idContact != 0)
                            {
                                // telefon 2
                                if (result.Length > 0)
                                    ds.Tables[1].Rows[0]["F10089"] = result;
                            }                        
                        break;
                    // EMail
                    case 3:
                        {
                            if (idContact == 2)
                            {
                                // e-mail
                                if (result.Length > 0)
                                    ds.Tables[1].Rows[0]["F100894"] = valoare;
                            }
                        }
                        break;
                }


                ds.Tables["F100Contacte"].Rows.Add(row);
                e.Cancel = true;
                grDateContacte.CancelEdit();
                grDateContacte.DataSource = ds.Tables["F100Contacte"];
                grDateContacte.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateContacte_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                string result = "", valoare = "";
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Contacte"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Contacte"].Columns)
                {
                    if (!col.AutoIncrement && grDateContacte.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "VALOARE":
                            valoare = e.NewValues[col.ColumnName].ToString();
                            char[] trimedChars = { '.', ' ', '\'', '@', '-', '*', '#', '(', ')', '+', '/' };
                            string originalString = e.NewValues[col.ColumnName].ToString();
                            string[] splitedChars = originalString.Split(trimedChars);
                            foreach (string s in splitedChars)
                            {
                                result += s;
                            }
                            break;
                    }

                }

                int x = 0;
                int estePrincipal = 0, idContact = 0;
                foreach (DataColumn col in ds.Tables["F100Contacte"].Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDLOCATIE":
                            if (row[x] != null && row[x].ToString() != "0")                            
                                ds.Tables[2].Rows[0]["F100966"] = row[x];                            
                            break;
                        case "INTERIOR":
                            if (row[x] != null && row[x].ToString().Length > 0)                            
                                ds.Tables[2].Rows[0]["F100965"] = row[x];                            
                            break;
                        case "ESTEPRINCIPAL":
                            if (row[x] != null && row[x].ToString().Length > 0)
                                estePrincipal = Convert.ToInt32(row[x].ToString());
                            break;
                        case "IDCONTACT":
                            idContact = Convert.ToInt32(row[x].ToString());
                            break;
                        case "VALOARESTRING":
                            row[x] = result;
                            break;
                        case "PAROLAPDF":
                            if (row[x] != null && row[x].ToString().Length > 0)
                                ds.Tables[1].Rows[0]["F10016"] = row[x].ToString();
                            break;
                    }
                    x++;
                }

                switch (estePrincipal)
                {
                    // Telefon 1
                    case 1:
                        // daca nu e email...
                        if (idContact != 2 && idContact != 0)
                        {
                            // telefon 1
                            if (result.Length > 0)
                                ds.Tables[1].Rows[0]["F10088"] = result;
                        }
                        break;
                    // Telefon 2
                    case 2:
                        // daca nu e email...
                        if (idContact != 2 && idContact != 0)
                        {
                            // telefon 2
                            if (result.Length > 0)
                                ds.Tables[1].Rows[0]["F10089"] = result;
                        }
                        break;
                    // EMail
                    case 3:
                        {
                            if (idContact == 2)
                            {
                                // e-mail
                                if (result.Length > 0)
                                    ds.Tables[1].Rows[0]["F100894"] = valoare;
                            }
                        }
                        break;
                }

                e.Cancel = true;
                grDateContacte.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateContacte.DataSource = ds.Tables["F100Contacte"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateContacte_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Contacte"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateContacte.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateContacte.DataSource = ds.Tables["F100Contacte"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}