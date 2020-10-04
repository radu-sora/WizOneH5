using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;

namespace WizOne.Pagini
{
    public partial class MeniuDetaliu : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";
        int max = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();


                txtTitlu.Text = General.VarSession("Titlu").ToString();

                foreach (var c in grDate.Columns)
                {
                    try
                    {
                        TreeListDataColumn col = (TreeListDataColumn)c;
                        col.Caption = Dami.TraduCuvant(col.Caption ?? col.FieldName);
                    }
                    catch (Exception){}
                }


                //DataTable dtCmb = General.IncarcaDT(@"SELECT * FROM ""tblMeniuri"" ", null);
                //TreeListComboBoxColumn colPag = grDate.Columns["IdNomen"] as TreeListComboBoxColumn;
                //colPag.PropertiesComboBox.DataSource = null;
                //colPag.PropertiesComboBox.DataSource = dtCmb;
                //colPag.PropertiesComboBox.ValueField = "Id";
                ////colPag.PropertiesComboBox.ValueType = GetType(System.String);
                //colPag.PropertiesComboBox.TextField = "Pagina";

                //TreeListComboBoxColumn colIco = grDate.Columns["Imagine"] as TreeListComboBoxColumn;
                //colIco.PropertiesComboBox.DataSource = IncarcaIcoane();
                //colIco.PropertiesComboBox.ValueField = "Denumire";
                //colIco.PropertiesComboBox.TextField = "Denumire";
                //colIco.PropertiesComboBox.ImageUrlField = "CaleImg";


                if (!IsPostBack)
                {
                    //int max = Convert.ToInt32(General.ExecutaScalar("SELECT COALESCE(MAX(IdMeniu),0) FROM MeniuLinii", null)) + 1;
                    max = Convert.ToInt32(General.ExecutaScalar("SELECT COALESCE(MAX(\"IdMeniu\"),0) FROM \"MeniuLinii\"", null)) + 1;
                    txtMax["Max"] = max;
                    txtMax2.Value = max.ToString();
                    Session["IdMaxValue"] = max;                                       

                    int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);                
                    DataTable dtTemp = new DataTable();
                    DataTable dt = new DataTable();
                    
                    dt.Columns.Add("Stare", typeof(bool));
                    dt.Columns.Add("StareMobil", typeof(bool));

                    if (Session["Sablon_TipActiune"].ToString() == "Clone")
                        dtTemp = General.IncarcaDT(@"SELECT * FROM ""MeniuLinii"" WHERE ""Id""=@1 ", new string[] { "-99" });
                    else
                        dtTemp = General.IncarcaDT(@"SELECT * FROM ""MeniuLinii"" WHERE ""Id""=@1 ", new string[] { id.ToString() });                    
                    
                    dt.Load(dtTemp.CreateDataReader(), LoadOption.OverwriteChanges);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdMeniu"] };

                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                //incarcam header-ul
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""MeniuLista"" WHERE ""Id""=@1 ", new string[] { id.ToString() });
                                if (dtHead.Rows.Count > 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["Id"].ToString();
                                    txtDenumire.Text = dtHead.Rows[0]["Denumire"].ToString();
                                    txtDesc.Text = dtHead.Rows[0]["Descriere"].ToString();
                                    chkActiv.Value = Convert.ToBoolean(dtHead.Rows[0]["Activ"]);
                                }

                                //incarcam liniile
                                if (Session["Sablon_TipActiune"].ToString() == "Clone")
                                {
                                    DataTable dtOri = General.IncarcaDT(@"SELECT * FROM ""MeniuLinii"" WHERE ""Id""=@1", new string[] { id.ToString() });

                                    foreach (DataRow dr in dtOri.Rows)
                                    {
                                        DataRow drDes = dt.NewRow();
                                        drDes["Id"] = -99;
                                        drDes["IdMeniu"] = dr["IdMeniu"];
                                        drDes["Parinte"] = dr["Parinte"];
                                        drDes["IdNomen"] = dr["IdNomen"];
                                        drDes["Ordine"] = dr["Ordine"];
                                        drDes["Stare"] = dr["Stare"];
                                        drDes["Nume"] = dr["Nume"];
                                        drDes["Descriere"] = dr["Descriere"];
                                        drDes["Imagine"] = dr["Imagine"];
                                        drDes["USER_NO"] = Session["UserId"];
                                        drDes["TIME"] = DateTime.Now;
                                        drDes["StareMobil"] = dr["StareMobil"];
                                        drDes["NumeMobil"] = dr["NumeMobil"];
                                        drDes["OrdineMobil"] = dr["OrdineMobil"];
                                        dt.Rows.Add(drDes);
                                    }
                                }
                            }
                            break;
                    }

                    Session["Icons"] = IncarcaIcoane();
                    Session["Menus"] = General.IncarcaDT(@"SELECT * FROM ""tblMeniuri"" ", null);
                    Session["InformatiaCurenta"] = dt;                    
                }                

                (grDate.Columns["Imagine"] as TreeListComboBoxColumn).PropertiesComboBox.DataSource = Session["Icons"];
                (grDate.Columns["IdNomen"] as TreeListComboBoxColumn).PropertiesComboBox.DataSource = Session["Menus"];
                grDate.DataSource = Session["InformatiaCurenta"];
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""MeniuLista"" WHERE ""Id""=@1 ", new string[] { id.ToString() });

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) FROM ""MeniuLista"" ", null) ?? 0) + 1;
                            DataRow drHead = dtHead.NewRow();
                            drHead["Id"] = id;
                            drHead["Denumire"] = txtDenumire.Text;
                            drHead["Descriere"] = txtDesc.Text;
                            drHead["Activ"] = chkActiv.Value;
                            dtHead.Rows.Add(drHead);

                            int idMeniu = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""IdMeniu""),0) FROM ""MeniuLinii"" ", null) ?? 0);
                            foreach (DataRow dr in dt.Rows)
                            {
                                dr["Id"] = id;
                                dr["IdMeniu"] = Convert.ToInt32(General.Nz(dr["IdMeniu"],0)) + idMeniu;
                                if (Convert.ToInt32(General.Nz(dr["Parinte"], 0)) != 0) dr["Parinte"] = Convert.ToInt32(General.Nz(dr["Parinte"], 0)) + idMeniu;
                            }
                        }
                        break;
                    case "Edit":
                        {
                            dtHead.Rows[0]["Denumire"] = txtDenumire.Text;
                            dtHead.Rows[0]["Descriere"] = txtDesc.Text;
                            dtHead.Rows[0]["Activ"] = chkActiv.Value;

                            //string key = grDate.Nodes[0].Key;
                            int idMeniu = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""IdMeniu""),0) FROM ""MeniuLinii"" ", null) ?? 0);
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr.RowState == DataRowState.Added)
                                {
                                    //dr["Id"] = id;
                                    dr["IdMeniu"] = Convert.ToInt32(General.Nz(dr["IdMeniu"], 0)) + idMeniu;
                                    //if (Convert.ToInt32(General.Nz(dr["Parinte"], 0)) != 0) dr["Parinte"] = Convert.ToInt32(General.Nz(dr["Parinte"], 0)) + idMeniu;
                                }
                            }
                        }
                        break;
                }


                //if (dtHead.Rows.Count == 0)
                //{
                //    id = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) FROM ""MeniuLista"" ", null) ?? 0) + 1;
                //    DataRow drHead = dtHead.NewRow();
                //    drHead["Id"] = id;
                //    drHead["Denumire"] = txtDenumire.Text;
                //    drHead["Descriere"] = txtDesc.Text;
                //    dtHead.Rows.Add(drHead);

                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        dr["Id"]= id;
                //    }
                //}
                //else
                //{
                //    dtHead.Rows[0]["Denumire"] = txtDenumire.Text;
                //    dtHead.Rows[0]["Descriere"] = txtDesc.Text;
                //}

                //SqlDataAdapter daHead = new SqlDataAdapter();
                //daHead.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""MeniuLista"" ", null);
                //SqlCommandBuilder cbHead = new SqlCommandBuilder(daHead);
                //daHead.Update(dtHead);

                //daHead.Dispose();
                //daHead = null;
                General.SalveazaDate(dtHead, "MeniuLista");



                //SqlDataAdapter da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""MeniuLinii"" ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;
                General.SalveazaDate(dt, "MeniuLinii");

                HttpContext.Current.Session["Sablon_Tabela"] = "MeniuLista";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);

                //MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_NodeDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow found = dt.Rows.Find(e.Keys[0]);
                found.Delete();

                e.Cancel = true;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_NodeInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                ASPxTreeList grDate = sender as ASPxTreeList;

                int id = -99;
                if (Session["Sablon_TipActiune"].ToString() == "Edit") id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                object[] row = new object[dt.Columns.Count];
                int x = 0;
                //int max = Convert.ToInt32(General.ExecutaScalar("SELECT COALESCE(MAX(IdMeniu),0) FROM MeniuLinii", null)) + 1;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToLower())
                        {
                            case "id":
                                row[x] = id;
                                break;
                            case "idmeniu":
                                //este un id virtual; adevaratul id se pune in butonul de salvare
                                //row[x] = Convert.ToInt32(General.Nz(dt.Compute("max(\"IdMeniu\")", "[IdMeniu] IS NOT NULL"), 0)) + 1;
                                row[x] = Convert.ToInt32(General.Nz(dt.Compute("MAX(IdMeniu)", "[IdMeniu] IS NOT NULL"), 0)) + 1;
                                break;
                            case "parinte":
                                row[x] = Convert.ToInt32(grDate.NewNodeParentKey == "" ? "0" : grDate.NewNodeParentKey);
                                //TreeListNode ert2 = grDate.FindNodeByKeyValue(row[x].ToString()) as TreeListNode;
                                //TreeListNode nd = grDate.Nodes[(int)row[x]] as TreeListNode;
                                //string ert1 = "";
                                break;
                            //case "IDAUTO":
                            //    //row[x] = dt.AsEnumerable().Max(p => p.Field<int>("IdAuto")) + 1;
                            //    row[x] = max;
                            //    break;
                            case "user_no":
                                row[x] = Session["UserId"];
                                break;
                            case "time":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_NodeUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow row = dt.Rows.Find(e.Keys[0]);

                foreach (DictionaryEntry col in e.NewValues)
                {
                    if (cmp.IndexOf(col.Key.ToString().ToUpper() + ",") < 0)
                    {
                        row[col.Key.ToString()] = col.Value ?? DBNull.Value;
                    }
                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewNode(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                int id = -99;
                if (Session["Sablon_TipActiune"].ToString() == "Edit") id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);

                e.NewValues["Id"] = id;
                //e.NewValues["IdMeniu"] = id;
                e.NewValues["Stare"] = 1;
                
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                //if (dt.Columns["IdMeniu"] != null)
                //{
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        int max = dt.AsEnumerable().Max(p => p.Field<int?>("IdMeniu")) ?? 0;
                //        e.NewValues["IdMeniu"] = max + 1;
                //    }
                //    else
                //        e.NewValues["IdMeniu"] = 1;
                //}
                if (dt.Columns["Ordine"] != null)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int max = dt.AsEnumerable().Max(p => p.Field<int?>("Ordine")) ?? 0;
                        e.NewValues["Ordine"] = max + 1;
                    }
                    else
                        e.NewValues["Ordine"] = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CellEditorInitialize(object sender, TreeListColumnEditorEventArgs e)
        {
            try
            {
                int level = 1;
                ASPxTreeList tl = sender as ASPxTreeList;
                if (tl.IsNewNodeEditing)
                {
                    if (tl.NewNodeParentKey != "")
                    {
                        TreeListNode node = tl.FindNodeByKeyValue(tl.NewNodeParentKey);
                        if (node != null) level = (node.Level + 1);
                    }
                }
                else
                {
                    if (e.NodeKey != null)
                    {
                        TreeListNode node = tl.FindNodeByKeyValue(e.NodeKey);
                        if (node != null) level = node.Level;
                    }
                }

                if (level != 3 && (new string[] { "IdNomen", "StareMobil", "NumeMobil", "OrdineMobil" }).Contains(e.Column.FieldName))
                    e.Editor.Visible = false;                                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_OnItemsRequestedByFilterCondition_SQL(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            //ASPxComboBox comboBox = (ASPxComboBox)source;
            //dsPersons.SelectCommand =
            //       @"SELECT [ID], [Phone], [FirstName], [LastName] FROM (select [ID], [Phone], [FirstName], [LastName], row_number()over(order by t.[LastName]) as [rn] from [Persons] as t where (([FirstName] + ' ' + [LastName] + ' ' + [Phone]) LIKE @filter)) as st where st.[rn] between @startIndex and @endIndex";

            //dsPersons.SelectParameters.Clear();
            //dsPersons.SelectParameters.Add("filter", TypeCode.String, string.Format("%{0}%", e.Filter));
            //dsPersons.SelectParameters.Add("startIndex", TypeCode.Int64, (e.BeginIndex + 1).ToString());
            //dsPersons.SelectParameters.Add("endIndex", TypeCode.Int64, (e.EndIndex + 1).ToString());
            //comboBox.DataSource = dsPersons;
            //comboBox.DataBindItems();
        }

        protected void grDate_OnItemRequestedByValue_SQL(object source, ListEditItemRequestedByValueEventArgs e)
        {
            //if (e.Value == null) return;
            //int value = (int)e.Value;
            //ASPxComboBox comboBox = (ASPxComboBox)source;
            //dsPersons.SelectCommand = @"SELECT ID, LastName, [Phone], FirstName FROM Persons WHERE (ID = @ID) ORDER BY FirstName";

            //dsPersons.SelectParameters.Clear();
            //dsPersons.SelectParameters.Add("ID", TypeCode.Int64, e.Value.ToString());
            //comboBox.DataSource = dsPersons;
            //comboBox.DataBindItems();
        }

        protected void grDate_CommandColumnButtonInitialize(object sender, TreeListCommandColumnButtonEventArgs e)
        {
            try
            {
                if (e.NodeKey != null)
                {
                    ASPxTreeList tl = sender as ASPxTreeList;
                    TreeListNode node = tl.FindNodeByKeyValue(e.NodeKey);
                    if (node.Level == 3 && (e.ButtonType == TreeListCommandColumnButtonType.New))
                    {
                        e.Visible = DefaultBoolean.False;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private DataTable IncarcaIcoane()
        {
            DataTable dt = new DataTable();

            try
            {
                dt.Columns.Add("Denumire");
                dt.Columns.Add("CaleImg");

                string[] arr = Directory.GetFiles(Server.MapPath("~/Fisiere/Imagini/Icoane/"));
                foreach (string l in arr)
                {
                    DataRow dr = dt.NewRow();
                    dr["Denumire"] = Path.GetFileName(l);
                    dr["CaleImg"] = "~/Fisiere/Imagini/Icoane/" + Path.GetFileName(l);
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["Sablon_Tabela"] = "MeniuLista";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}