using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;

namespace WizOne.Curs
{
    public partial class relCurs : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();
       
                //btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");


                string qwe = General.Nz(Request["qwe"], "-99").ToString();
                int tip = Convert.ToInt32(General.Nz(Request["tip"], -99));

                if (!IsPostBack)
                    Session["relCurs_Grid"] = null;


                switch (tip)
                {
                    case 1:
                        lblTitlu.Text = Dami.TraduCuvant("Competente");
                        dynamic c = new GridViewDataColumn();
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "Id_Competenta";
                        c.FieldName = "Id_Competenta";
                        c.Caption = Dami.TraduCuvant("Competenta");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "Id";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dtComp = GetTblCompetenteAcadOrdonat();
                        GridViewDataComboBoxColumn colComp = (grDate.Columns["Id_Competenta"] as GridViewDataComboBoxColumn);
                        colComp.PropertiesComboBox.DataSource = dtComp;
                        break;
                    case 2:
                        lblTitlu.Text = Dami.TraduCuvant("Competente");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "Id_Competenta";
                        c.FieldName = "Id_Competenta";
                        c.Caption = Dami.TraduCuvant("Competenta");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "Id";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        dtComp = GetTblCompetenteOrdonat();
                        colComp = (grDate.Columns["Id_Competenta"] as GridViewDataComboBoxColumn);
                        colComp.PropertiesComboBox.DataSource = dtComp;
                        break;
                    case 3:
                        lblTitlu.Text = Dami.TraduCuvant("Angajati");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "Id_Angajat";
                        c.FieldName = "Id_Angajat";
                        c.Caption = Dami.TraduCuvant("Angajat");
                        c.PropertiesComboBox.TextField = "Descriere";
                        c.PropertiesComboBox.ValueField = "F10003";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dtAng = GetF100();
                        GridViewDataComboBoxColumn colAng = (grDate.Columns["Id_Angajat"] as GridViewDataComboBoxColumn);
                        colAng.PropertiesComboBox.DataSource = dtAng;
                        break;
                    case 4:
                        lblTitlu.Text = Dami.TraduCuvant("Functii");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "Id_FunctieAcad";
                        c.FieldName = "Id_FunctieAcad";
                        c.Caption = Dami.TraduCuvant("Titlu academic");
                        c.PropertiesComboBox.TextField = "F71304";
                        c.PropertiesComboBox.ValueField = "F71302";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dtFunc = General.IncarcaDT("SELECT * FROM F713");
                        GridViewDataComboBoxColumn colFunc = (grDate.Columns["Id_FunctieAcad"] as GridViewDataComboBoxColumn);
                        colFunc.PropertiesComboBox.DataSource = dtFunc;
                        break;
                    case 5:
                    case 6:
                        lblTitlu.Text = Dami.TraduCuvant("Functii");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "Id_Functie";
                        c.FieldName = "Id_Functie";
                        c.Caption = Dami.TraduCuvant("Post");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "Id";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        dtFunc = General.IncarcaDT("SELECT  F71802 as \"Id\",  F71804 as \"Denumire\" FROM F718");
                        colFunc = (grDate.Columns["Id_Functie"] as GridViewDataComboBoxColumn);
                        colFunc.PropertiesComboBox.DataSource = dtFunc;
                        break;
                    case 7:
                        lblTitlu.Text = Dami.TraduCuvant("Departamente");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "Id_Depart";
                        c.FieldName = "Id_Depart";
                        c.Caption = Dami.TraduCuvant("Departament");
                        c.PropertiesComboBox.TextField = "Dept";
                        c.PropertiesComboBox.ValueField = "F00607";

                        ListBoxColumn column = new ListBoxColumn();
                        column.FieldName = "Subcompanie";
                        column.Caption = Dami.TraduCuvant("Subcompanie");
                        c.PropertiesComboBox.Columns.Add(column);
                        column = new ListBoxColumn();
                        column.FieldName = "Filiala";
                        column.Caption = Dami.TraduCuvant("Filiala");
                        c.PropertiesComboBox.Columns.Add(column);
                        column = new ListBoxColumn();
                        column.FieldName = "Sectie";
                        column.Caption = Dami.TraduCuvant("Sectie");
                        c.PropertiesComboBox.Columns.Add(column);
                        column = new ListBoxColumn();
                        column.FieldName = "Dept";
                        column.Caption = Dami.TraduCuvant("Departament");
                        c.PropertiesComboBox.Columns.Add(column);

                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dtDept = GetDepartamente();
                        GridViewDataComboBoxColumn colDept = (grDate.Columns["Id_Depart"] as GridViewDataComboBoxColumn);
                        colDept.PropertiesComboBox.DataSource = dtDept;
                        break;
                    case 8:
                        lblTitlu.Text = Dami.TraduCuvant("Cursuri anterioare");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "IdCursAnterior";
                        c.FieldName = "IdCursAnterior";
                        c.Caption = Dami.TraduCuvant("Curs anterior");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "Id";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        c = new GridViewDataTextColumn(); 
                        c.Name = "IdCurs";
                        c.FieldName = "IdCurs";
                        c.Caption = Dami.TraduCuvant("Curs");
                        c.Visible = false;
                        c.ShowInCustomizationForm = false;
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dtCurs = General.IncarcaDT("SELECT  * FROM \"Curs_tblCurs\" WHERE \"Id\" <> " + qwe);
                        GridViewDataComboBoxColumn colCurs = (grDate.Columns["IdCursAnterior"] as GridViewDataComboBoxColumn);
                        colCurs.PropertiesComboBox.DataSource = dtCurs;

                        //dtCurs = General.IncarcaDT("SELECT  * FROM \"Curs_tblCurs\" ");
                        //colCurs = (grDate.Columns["IdCurs"] as GridViewDataComboBoxColumn);
                        //colCurs.PropertiesComboBox.DataSource = dtCurs;

                        GridViewDataTextColumn colCursV = (grDate.Columns["Id_Curs"] as GridViewDataTextColumn);
                        if (!IsPostBack)
                            grDate.Columns.Remove(colCursV);
                        break;
                    case 9:
                        lblTitlu.Text = Dami.TraduCuvant("Traineri");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "IdTrainer";
                        c.FieldName = "IdTrainer";
                        c.Caption = Dami.TraduCuvant("Trainer");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "IdUser";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        c = new GridViewDataTextColumn();
                        c.Name = "IdCurs";
                        c.FieldName = "IdCurs";
                        c.Caption = Dami.TraduCuvant("Curs");
                        c.Visible = false;
                        c.ShowInCustomizationForm = false;
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        c = new GridViewDataTextColumn();
                        c.Name = "IdSesiune";
                        c.FieldName = "IdSesiune";
                        c.Caption = Dami.TraduCuvant("Sesiune");
                        c.Visible = false;
                        c.ShowInCustomizationForm = false;
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dtTrainer = General.IncarcaDT("SELECT  * FROM \"Curs_tblTraineri\" ");
                        GridViewDataComboBoxColumn colTrainer = (grDate.Columns["IdTrainer"] as GridViewDataComboBoxColumn);
                        colTrainer.PropertiesComboBox.DataSource = dtTrainer;

                        colCursV = (grDate.Columns["Id_Curs"] as GridViewDataTextColumn);
                        if (!IsPostBack)
                            grDate.Columns.Remove(colCursV);
                        break;
                }
                
                Session["relCurs_Tip"] = tip;
                IncarcaGrid(tip, qwe);

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetTblCompetenteAcadOrdonat()
        {
            try
            {
                string sql = "SELECT b.\"Activ\", b.\"Denumire\", a.\"Id_Competenta\" AS \"Id\", b.\"IdGrup\", a.USER_NO, b.TIME FROM "
                    + " \"Curs_relCompetenteFunctiiAcad\" a "
                    + " JOIN \"tblCompetente\" b on a.\"Id_Competenta\" = b.\"Id\" ORDER BY b.\"Denumire\", b.\"Id\" ";      

                return General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetTblCompetenteOrdonat()
        {
            try
            {
                string sql = "";

                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 34)   //daca este GAM
                {
                    sql = "SELECT b.\"Activ\", b.\"Denumire\", b.\"Id\", b.\"IdGrup\", b.USER_NO, b.TIME FROM \"tblCompetente\" b ORDER BY b.\"Denumire\", b.\"Id\" ";
                }
                else
                {
                    sql = "SELECT b.\"Activ\", b.\"Denumire\", a.\"Id_Competenta\" AS \"Id\", b.\"IdGrup\", a.USER_NO, b.TIME FROM "
                    + " \"Curs_relCompetenteFunctii\" a "
                    + " JOIN \"tblCompetente\" b on a.\"Id_Competenta\" = b.\"Id\" ORDER BY b.\"Denumire\", b.\"Id\" ";
                }
                return General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetF100()
        {
            DataTable q = null;
            try
            {
                string strSql = @"SELECT F10003, F10008 {0} ' ' {0} F10009 AS ""NumeComplet"",  " +
                                @" F10008 {0} ' ' {0} F10009 {0} ' | ' {0} {2} F10022 {3} as ""Descriere"" " +
                                @" FROM F100 WHERE F10022 <= {1} AND {1} <= F10023 ORDER BY F10008, F10009";
                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, "+", "GetDate()", "convert(varchar(10), ", ",3)");
                else
                    strSql = string.Format(strSql, "||", "sysdate", "to_char(", ", 'dd/mm/yyyy')");

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable GetDepartamente()
        {
            DataTable q = null;

            try
            {
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";

                if (Constante.tipBD == 2) cmp = "ROWNUM";

                string strSql = @"SELECT {0} as ""IdAuto"", a.F00204 AS ""Companie"",b.F00305 AS ""Subcompanie"",c.F00406 AS ""Filiala"",d.F00507 AS ""Sectie"",e.F00608 AS ""Dept"", F.F00709 AS ""Subdept"", G.F00810 AS ""Birou"",
                                a.F00202 AS ""IdCompanie"",b.F00304 AS ""IdSubcompanie"",c.F00405 AS ""IdFiliala"",d.F00506 AS ""IdSectie"",e.F00607 AS ""IdDept"", F.F00708 AS ""IdSubdept"", G.F00809 AS ""IdBirou"", e.F00607 
                                FROM F002 A
                                LEFT JOIN F003 B ON A.F00202 = B.F00303
                                LEFT JOIN F004 C ON B.F00304 = C.F00404
                                LEFT JOIN F005 D ON C.F00405 = D.F00505 
                                LEFT JOIN F006 E ON D.F00506 = E.F00606
                                LEFT JOIN F007 F ON E.F00607 = F.F00707
                                LEFT JOIN F008 G ON F.F00708 = G.F00808
                                ORDER BY E.F00607";

                strSql = string.Format(strSql, cmp);

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        private void IncarcaGrid(int tip, string id)
        {
            try
            {
                DataTable dt = new DataTable();

                string strSql = "";

                switch (tip)
                {
                    case 1:
                        strSql = " SELECT * FROM \"Curs_relCursCompetenteAcad\" WHERE \"Id_Curs\" = " + id;
                        break;
                    case 2:
                        strSql = " SELECT * FROM \"Curs_relCursCompetente\" WHERE \"Id_Curs\" = " + id;
                        break;
                    case 3:
                        strSql = " SELECT * FROM \"Curs_relCursAngajati\" WHERE \"Id_Curs\" = " + id;
                        break;
                    case 4:
                        strSql = " SELECT * FROM \"Curs_relCursFunctiiAcad\" WHERE \"Id_Curs\" = " + id;
                        break;
                    case 5:
                    case 6:
                        strSql = " SELECT * FROM \"Curs_relCursFunctii\" WHERE \"Id_Curs\" = " + id;
                        break;
                    case 7:
                        strSql = " SELECT * FROM \"Curs_relCursDepart\" WHERE \"Id_Curs\" = " + id;
                        break;
                    case 8:
                        strSql = " SELECT * FROM \"Curs_relCursAnterior\" WHERE \"IdCurs\" = " + id;
                        break;
                    case 9:
                        strSql = " SELECT * FROM \"Curs_relSesiuneTrainer\" WHERE \"IdCurs\" = " + id.Split(',')[0] + " AND \"IdSesiune\" = " + id.Split(',')[1];
                        break;
                }

                if (Session["relCurs_Grid"] == null)
                {
                    dt = General.IncarcaDT(strSql, null);
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["relCurs_Grid"] = dt;
                }
                else
                {
                    dt = Session["relCurs_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }

                
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

                DataTable dt = Session["relCurs_Grid"] as DataTable;

                switch (Convert.ToInt32(Session["relCurs_Tip"].ToString()))
                {
                    case 1:
                        General.SalveazaDate(dt, "Curs_relCursCompetenteAcad");
                        break;
                    case 2:
                        General.SalveazaDate(dt, "Curs_relCursCompetente");
                        break;
                    case 3:
                        General.SalveazaDate(dt, "Curs_relCursAngajati");
                        break;
                    case 4:
                        General.SalveazaDate(dt, "Curs_relCursFunctiiAcad");
                        break;
                    case 5:
                    case 6:
                        General.SalveazaDate(dt, "Curs_relCursFunctii");
                        break;
                    case 7:
                        General.SalveazaDate(dt, "Curs_relCursDepart");
                        break;
                    case 8:
                        General.SalveazaDate(dt, "Curs_relCursAnterior");
                        break;
                    case 9:
                        General.SalveazaDate(dt, "Curs_relSesiuneTrainer");
                        break;
                }               

                MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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

                DataTable dt = Session["relCurs_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                switch (Convert.ToInt32(Session["relCurs_Tip"].ToString()))
                {
                    case 1:
                    case 2:
                        if (e.NewValues["Id_Competenta"] == null)
                        { 
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste competenta!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 3:
                        if (e.NewValues["Id_Angajat"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste angajatul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 4:
                        if (e.NewValues["Id_FunctieAcad"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste functia academinca!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 5:
                    case 6:
                        if (e.NewValues["Id_Functie"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste functia!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 7:
                        if (e.NewValues["Id_Depart"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste departamentul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 8:
                        if (e.NewValues["IdCursAnterior"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste cursul anterior!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 9:
                        if (e.NewValues["IdTrainer"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste trainer-ul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
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
                Session["relCurs_Grid"] = dt;
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
                DataTable dt = Session["relCurs_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;

                if (Convert.ToInt32(Session["relCurs_Tip"].ToString()) == 9)
                {
                    string iduri = General.Nz(Request["qwe"], -99).ToString();
                    e.NewValues["IdCurs"] = Convert.ToInt32(iduri.Split(',')[0]);
                    e.NewValues["IdSesiune"] = Convert.ToInt32(iduri.Split(',')[1]);
                }
                else if (Convert.ToInt32(Session["relCurs_Tip"].ToString()) == 8)
                    e.NewValues["IdCurs"] = Convert.ToInt32(General.Nz(Request["qwe"], -99));
                else
                    e.NewValues["Id_Curs"] = Convert.ToInt32(General.Nz(Request["qwe"], -99));

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
                {
                    switch (Convert.ToInt32(Session["relCurs_Tip"].ToString()))
                    {
                        case 1:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursCompetenteAcad");
                            break;
                        case 2:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursCompetente");
                            break;
                        case 3:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursAngajati");
                            break;
                        case 4:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursFunctiiAcad");
                            break;
                        case 5:
                        case 6:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursFunctii");
                            break;
                        case 7:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursDepart");
                            break;
                        case 8:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relCursAnterior");
                            break;
                        case 9:
                            e.NewValues["IdAuto"] = Dami.NextId("Curs_relSesiuneTrainer");
                            break;
                    }
                }


                switch (Convert.ToInt32(Session["relCurs_Tip"].ToString()))
                {
                    case 1:
                    case 2:
                        if (e.NewValues["Id_Competenta"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste competenta!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 3:
                        if (e.NewValues["Id_Angajat"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste angajatul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 4:
                        if (e.NewValues["Id_FunctieAcad"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste functia academinca!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 5:
                    case 6:
                        if (e.NewValues["Id_Functie"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste functia!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 7:
                        if (e.NewValues["Id_Depart"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste departamentul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 8:
                        if (e.NewValues["IdCursAnterior"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste cursul anterior!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 9:
                        if (e.NewValues["IdTrainer"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste trainer-ul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
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
                Session["relCurs_Grid"] = dt;
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

                DataTable dt = Session["relCurs_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["relCurs_Grid"] = dt;
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


                    //<dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" RenderMode="Button" AutoPostBack="false"  oncontextMenu="ctx(this,event)" >
                    //    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                    //    <ClientSideEvents Click="function(s, e) {
                    //        setTimeout (window.close, 500);
                    //    }" />
                    //</dx:ASPxButton>
        

    }
}