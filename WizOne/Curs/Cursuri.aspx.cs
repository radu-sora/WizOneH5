using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
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

namespace WizOne.Curs
{
    public partial class Cursuri : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

       

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnDoc.Image.ToolTip = Dami.TraduCuvant("btnDoc", "Documente");
                btnComp.Image.ToolTip = Dami.TraduCuvant("btnComp", "Competente");
                btnTitl.Image.ToolTip = Dami.TraduCuvant("btnTitl", "Titluri");
                btnDept.Image.ToolTip = Dami.TraduCuvant("btnDept", "Departamente");
                btnAng.Image.ToolTip = Dami.TraduCuvant("btnAng", "Angajati");
                btnCursAnt.Image.ToolTip = Dami.TraduCuvant("btnCursAnt", "Cursuri anterioare");
                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn coloana = c as GridViewDataColumn;
                            coloana.Caption = Dami.TraduCuvant(coloana.FieldName ?? coloana.Caption, coloana.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                    Session["Cursuri_Grid"] = null;

                DataTable dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblCateg_Niv1"" ", null);
                GridViewDataComboBoxColumn col = (grDate.Columns["Categ_Niv1Id"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblCateg_Niv2"" ", null);
                col = (grDate.Columns["Categ_Niv2Id"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblCateg_Niv3"" ", null);
                col = (grDate.Columns["Categ_Niv3Id"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblOrganizatori"" ", null);
                col = (grDate.Columns["OrganizatorId"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;
                Session["Curs_Organizatori"] = dt;

                dt = General.IncarcaDT(@"SELECT ""IdCategorie"", ""DenumireCategorie"" FROM ""Curs_tblCategoriiNote"" ", null);
                col = (grDate.Columns["IdCategorieNota"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT ""IdDocument"", ""DescrDocument"" FROM ""Documente"" ", null);
                col = (grDate.Columns["TemplateDiploma"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT ""IdUser"", ""Denumire"" FROM ""Curs_tblTraineri"" ", null);
                col = (grDate.Columns["User_TrainerId"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                if (!IsPostBack)
                {
                    string tipFunctii = Dami.ValoareParam("TipFunctiiCurs", "0");
                    string afisareOrganizatorCursuri = Dami.ValoareParam("afisareOrganizatorCursuri", "0");
                    string curs_CompletareAngajati = Dami.ValoareParam("Curs_CompletareAngajati", "0");
                    string curs_CompletareDepartament = Dami.ValoareParam("Curs_CompletareDepartament", "0");
                    string curs_CompletareCompetente = Dami.ValoareParam("Curs_CompletareCompetente", "0"); 
                    string curs_CompletareFunctii = Dami.ValoareParam("Curs_CompletareFunctii", "0");

                    Session["TipFunctiiCurs"] = tipFunctii;
                    Session["AfisareOrganizatorCursuri"] = afisareOrganizatorCursuri;
                    Session["Curs_CompletareAngajati"] = curs_CompletareAngajati;
                    Session["Curs_CompletareDepartament"] = curs_CompletareDepartament;
                    Session["Curs_CompletareCompetente"] = curs_CompletareCompetente;
                    Session["Curs_CompletareFunctii"] = curs_CompletareFunctii;
                }


                switch (Convert.ToInt32(Session["Curs_CompletareAngajati"].ToString()))
                {
                    case 0:
                        grDate.Columns["colAngajati"].Visible = false;
                        break;
                    case 1:
                    case 2:
                    case 3:
                        grDate.Columns["colAngajati"].Visible = true;
                        break;
                }
                switch (Convert.ToInt32(Session["Curs_CompletareDepartament"].ToString()))
                {
                    case 0:
                        grDate.Columns["colDept"].Visible = false;
                        break;
                    case 1:
                    case 2:
                    case 3:
                        grDate.Columns["colDept"].Visible = true;
                        break;
                }
                switch (Convert.ToInt32(Session["Curs_CompletareCompetente"].ToString()))
                {
                    case 0:
                        grDate.Columns["colCompetente"].Visible = false;
                        break;
                    case 1:
                    case 2:
                    case 3:
                        grDate.Columns["colCompetente"].Visible = true;
                        break;
                }
                switch (Convert.ToInt32(Session["Curs_CompletareFunctii"].ToString()))
                {
                    case 0:
                        grDate.Columns["colTitluri"].Visible = false;
                        break;
                    case 1:
                    case 2:
                    case 3:
                        grDate.Columns["colTitluri"].Visible = true;
                        break;
                }

                grDate.Columns["OrganizatorId"].Visible = (Convert.ToInt32(Session["AfisareOrganizatorCursuri"].ToString()) == 0) ? true : false;

                IncarcaGrid();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //IncarcaGrid();
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
                DataTable dt = Session["Cursuri_Grid"] as DataTable;

                General.SalveazaDate(dt, "Curs_tblCurs");

                MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

     


        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {

                if (Session["Cursuri_Grid"] == null)
                {
                    grDate.KeyFieldName = "IdAuto";

                    dt = General.IncarcaDT("SELECT * FROM \"Curs_tblCurs\"", null);


                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["Cursuri_Grid"] = dt;
                }
                else
                {
                    grDate.KeyFieldName = "IdAuto";

                    dt = Session["Cursuri_Grid"] as DataTable;


                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
     
                
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

      

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Cursuri_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                if (e.NewValues["Denumire"] == null || e.NewValues["Denumire"].ToString().Length <= 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste denumirea!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                if (e.NewValues["OrganizatorId"] != null)
                {
                    DataTable dt1 = Session["Curs_Organizatori"] as DataTable;
                    if (dt1 != null && dt1.Select("Id = " + e.NewValues["OrganizatorId"].ToString()) != null && dt1.Select("Id = " + e.NewValues["OrganizatorId"].ToString()).Length > 0)
                       row["OrganizatorNume"] = dt1.Select("Id = " + e.NewValues["OrganizatorId"].ToString()).FirstOrDefault()["Denumire"].ToString();
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Cursuri_Grid"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Cursuri_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;

                e.NewValues["Id"] = Dami.NextId("Curs_tblCurs");
                if (e.NewValues["Activ"] == null)
                    e.NewValues["Activ"] = 1;
                if (e.NewValues["Importat"] == null)
                    e.NewValues["Importat"] = 0;

                if (e.NewValues["OrganizatorId"] != null)
                {
                    DataTable dt1 = Session["Curs_Organizatori"] as DataTable;
                    if (dt1 != null && dt1.Select("Id = " + e.NewValues["OrganizatorId"].ToString()) != null && dt1.Select("Id = " + e.NewValues["OrganizatorId"].ToString()).Length > 0)
                        e.NewValues["OrganizatorNume"] = dt1.Select("Id = " + e.NewValues["OrganizatorId"].ToString()).FirstOrDefault()["Denumire"].ToString();
                }

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
                    e.NewValues["IdAuto"] = Dami.NextId("Curs_tblCurs");

                if (e.NewValues["Denumire"] == null || e.NewValues["Denumire"].ToString().Length <= 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste denumirea!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
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

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["Cursuri_Grid"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Cursuri_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Cursuri_Grid"] = dt;
                grDate.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //  <dx:GridViewDataTextColumn FieldName="DenumireValoare" Name="DenumireValoare" Caption="Categorie"  Width="75px" Visible="false" />

    }
}