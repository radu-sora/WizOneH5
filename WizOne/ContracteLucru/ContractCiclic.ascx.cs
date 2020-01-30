using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ContracteLucru
{
    public partial class ContractCiclic : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {       
            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();

            grDateCtrCiclic.DataBind();           

        }

        protected void grDateCtrCiclic_DataBinding(object sender, EventArgs e)
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
            //AdminContracte();

            string sqlFinal = "SELECT * FROM \"Ptj_ContracteCiclice\" WHERE \"IdContract\" = " + Session["IdContract"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            if (ds.Tables.Contains("Ptj_ContracteCiclice"))
            {
                dt = ds.Tables["Ptj_ContracteCiclice"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Ptj_ContracteCiclice";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCtrCiclic.KeyFieldName = "IdAuto";
            grDateCtrCiclic.DataSource = dt;

            DataTable dtCic = General.IncarcaDT(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""TipContract"" = 1 AND ""Id"" <>  " + Session["IdContract"].ToString(), null);
            GridViewDataComboBoxColumn colCic = (grDateCtrCiclic.Columns["IdContractZilnic"] as GridViewDataComboBoxColumn);
            colCic.PropertiesComboBox.DataSource = dtCic;
            Session["InformatiaCurentaContracte"] = ds;

        }

        protected void pnlCtlCtrCiclic_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;            
            switch (param[0])
            {
                case "deDataInc":
                    string[] data = param[1].Split(' ');
                    ds.Tables["Ptj_ContracteCiclice"].Rows[0]["CicluDataInceput"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "txtLung":
                    ds.Tables["Ptj_ContracteCiclice"].Rows[0]["CicluLungime"] = param[1];
                    Session["InformatiaCurentaContracte"] = ds;
                    AdminContracte();
                    break;                
            }
        }

        protected void grDateCtrCiclic_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteCiclice"];
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCtrCiclic_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteCiclice"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteCiclice"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteCiclice"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "IDCONTRACTZILNIC":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste contractul zilnic. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                if (valid)
                {
                    ds.Tables["Ptj_ContracteCiclice"].Rows.Add(row);
                    e.Cancel = true;
                    grDateCtrCiclic.CancelEdit();
                    grDateCtrCiclic.DataSource = ds.Tables["Ptj_ContracteCiclice"];
                    grDateCtrCiclic.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrCiclic_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteCiclice"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteCiclice"].Columns)
                {
                    if (!col.AutoIncrement && grDateCtrCiclic.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDCONTRACTZILNIC":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste contractul zilnic. ";
                            }
                            break;

                    }

                } 

                if (valid)
                {
                    e.Cancel = true;
                    grDateCtrCiclic.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    grDateCtrCiclic.DataSource = ds.Tables["Ptj_ContracteCiclice"];
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrCiclic_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteCiclice"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCtrCiclic.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                grDateCtrCiclic.DataSource = ds.Tables["Ptj_ContracteCiclice"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void AdminContracte()
        {
            try
            {
                int i = 1;
                int idAuto = 0;
                ASPxTextBox txtLung = DataList1.Items[0].FindControl("txtLung") as ASPxTextBox;
                int lung = Convert.ToInt32(txtLung.Value ?? 0);
                if (lung == 0) return;

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                int total = ds.Tables["Ptj_ContracteCiclice"].Rows.Count;

                for (i = 0; i < total; i++)
                {
                    string sql = "SELECT * FROM \"Ptj_ContracteCiclice\" WHERE \"ZiCiclu\" = " + i.ToString();
                    DataTable dt = General.IncarcaDT(sql, null);

                    if (i > lung && dt != null && dt.Rows.Count > 0)
                    {
                        sql = "DELETE FROM \"Ptj_ContracteCiclice\" WHERE \"IdAuto\" = " + dt.Rows[0]["IdAuto"].ToString();
                        General.ExecutaNonQuery(sql, null);
                    }
                }                


                for (int x = i; x <= lung; x++)
                {
                    idAuto -= 1;
                    string sql = "INSERT INTO \"Ptj_ContracteCiclice\" (\"IdContract\", \"ZiCiclu\", \"IdContractZilnic\", \"SuprascrierePontaj\", \"IdAuto\", USER_NO, TIME) VALUES "
                        + " ({0}, {1}, NULL, 0, {2}, {3}, {4})";
                    sql = string.Format(sql, Session["IdContract"].ToString() , x, idAuto, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "getdate()" : "sysdate"));
                    General.IncarcaDT(sql, null);
                }

                string sqlFinal = "SELECT * FROM \"Ptj_ContracteCiclice\" WHERE \"IdContract\" = " + Session["IdContract"].ToString();
                DataTable table = new DataTable();
                table = General.IncarcaDT(sqlFinal, null);
                ds.Tables.Remove(ds.Tables["Ptj_ContracteCiclice"]);
                table.TableName = "Ptj_ContracteCiclice";
                table.PrimaryKey = new DataColumn[] { table.Columns["IdAuto"] };
                ds.Tables.Add(table);
                Session["InformatiaCurentaContracte"] = ds;

                DataList1.DataSource = table;
                DataList1.DataBind();

                grDateCtrCiclic.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void ArataMesaj(string mesaj)
        {
            pnlCtlCtrCiclic.Controls.Add(new LiteralControl());
            WebControl script = new WebControl(HtmlTextWriterTag.Script);
            pnlCtlCtrCiclic.Controls.Add(script);
            script.Attributes["id"] = "dxss_123456";
            script.Attributes["type"] = "text/javascript";
            script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        }

    }
}