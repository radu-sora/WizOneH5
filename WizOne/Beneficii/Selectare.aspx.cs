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
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Beneficii
{
    public partial class Selectare : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        public class metaUploadFile
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                lblBen.InnerText = Dami.TraduCuvant("In acest moment, urmatoarele Beneficii sunt active/aprobate:");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnValidare.Text = Dami.TraduCuvant("btnValidare", "Validare selectie Beneficii");
                pnlBen.HeaderText = Dami.TraduCuvant("Beneficii active");
                pnlSes.HeaderText = Dami.TraduCuvant("Sesiune activa pentru selectarea Beneficiilor");

                foreach (GridViewColumn c in grDateBen.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                foreach (GridViewColumn c in grDateSes.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = Dami.TraduCuvant("Vizualizare si selectare Beneficii", "Vizualizare si selectare Beneficii");


                DataTable dtBen = General.IncarcaDT(SelectBeneficii(), null);
                GridViewDataComboBoxColumn colBen = (grDateBen.Columns["IdBeneficiu"] as GridViewDataComboBoxColumn);
                colBen.PropertiesComboBox.DataSource = dtBen;
                GridViewDataComboBoxColumn colBenSes = (grDateSes.Columns["Id"] as GridViewDataComboBoxColumn);
                colBenSes.PropertiesComboBox.DataSource = dtBen;

                IncarcaGridBen();                

                if (!IsPostBack)
                {
                    IncarcaBenAles();
                    Session["Select_GridBen"] = null;
                    Session["Select_GridSes"] = null;
                    //DataTable dtSes = General.IncarcaDT("SELECT COUNT(*) FROM Ben_Sesiuni WHERE F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99) + "  AND DataInceput <= GETDATE() AND GETDATE() <= DataSfarsit ", null);
                    string strSql = @"Select Ben_tblSesiuni.* from Ben_tblSesiuni join Ben_relSesGrupAng
                                    On Ben_tblSesiuni.Id = Ben_relSesGrupAng.IdSesiune
                                    where cast(getdate() as date) between DataInceput and DataSfarsit
                                    And " + Convert.ToInt32(Session["User_Marca"] ?? -99) + @" in (Select F10003 from Ben_SetAngajatiDetail a inner join Ben_relSesGrupAng b on a.IdGrup = b.IdGrup)";
                    DataTable dtSes = General.IncarcaDT(strSql, null);

                    if (dtSes == null || dtSes.Rows.Count <= 0)
                    {
                        string msg = "In acest moment nu este disponibila o sesiune pentru selectarea Beneficiilor. Vei primi un e-mail de informare atunci cand urmatoarea sesiune va deveni activa.";
                        lblSes.Text = Dami.TraduCuvant(msg);
                        grDateSes.Visible = false;
                        btnValidare.Visible = false;
                        Session["SelectareBeneficii"] = 0;
                        Session["SelectareText"] = msg;
                    }
                    else
                    {
                        IncarcaGridSes();                        
                        DataTable dt = Session["SelectBen_Ses"] as DataTable;
                        string msg = "Sesiunea pentru selectarea Beneficiilor este deschisa pana in data de <b>" + Convert.ToDateTime(dtSes.Rows[0]["DataSfarsit"].ToString()).ToShortDateString() 
                            + "</b>.\n Selectarea Beneficiilor se face pentru perioada specificata in tabelul de mai jos.";
                        Session["Select_Sesiune"] = dtSes;
                        int idStare = 1;
                        if (dt != null && dt.Rows.Count > 0)
                            idStare = Convert.ToInt32(dt.Rows[0]["IdStare"].ToString());
                        switch (idStare)
                        {
                            case 1:
                                msg += "\n<b>Pana in acest moment nu ti-ai exprimat optiunea privind selectarea Beneficiilor. Te rugam sa faci acest lucru pana la expirarea sesiunii</b>.";
                                Session["SelectareBeneficii"] = 1;
                                Session["SelectareText"] = msg;
                                lblSes.Text = Dami.TraduCuvant(msg);
                                break;
                            case 2:
                                msg += "\n<b>Optiunea ta privind selectarea Beneficiilor este in curs de aprobare. Vei primi un e-mail in momentul in care solicitarea ta va fi validata</b>.";
                                Session["SelectareBeneficii"] = 2;
                                Session["SelectareText"] = msg;
                                lblSes.Text = Dami.TraduCuvant(msg);

                                DataTable dtBenAles1 = Session["Select_BenAles"] as DataTable;
                                if (dtBenAles1 != null && dtBenAles1.Rows.Count > 0)
                                    for (int i = 0; i < grDateSes.VisibleRowCount; i++)
                                    {
                                        DataRowView obj = grDateSes.GetRow(i) as DataRowView;
                                        if (Convert.ToInt32(obj["Id"].ToString()) == Convert.ToInt32(dtBenAles1.Rows[0]["IdBeneficiu"].ToString()))
                                        {
                                            grDateSes.Selection.SelectRow(i);
                                            break;
                                        }
                                    }
                                break;
                            case 3:
                                msg += "\n<b>Optiunea ta privind selectarea Beneficiilor a fost aprobata</b>!";
                                Session["SelectareBeneficii"] = 3;
                                Session["SelectareText"] = msg;
                                lblSes.Text = Dami.TraduCuvant(msg);
                                btnValidare.Enabled = false;
                                DataTable dtBenAles = Session["Select_BenAles"] as DataTable;
                                for (int i = 0; i < grDateSes.VisibleRowCount; i++)
                                {
                                    DataRowView obj = grDateSes.GetRow(i) as DataRowView;
                                    if (Convert.ToInt32(obj["Id"].ToString()) == Convert.ToInt32(dtBenAles.Rows[0]["IdBeneficiu"].ToString()))
                                    {
                                        grDateSes.Selection.SelectRow(i);
                                        break;
                                    }
                                }
                                grDateSes.SettingsBehavior.AllowSelectByRowClick = false;
                                grDateSes.Enabled = false;
                                break;
                            case 4:
                                msg += "\n<b>Optiunea ta privind selectarea Beneficiilor a fost respinsa! Motivul este urmatorul:" + dt.Rows[0]["Motiv"].ToString() + ". Te rugam sa efectuezi o noua selectie pana la expirarea sesiunii</b>.";
                                Session["SelectareBeneficii"] = 4;
                                Session["SelectareText"] = msg;
                                lblSes.Text = Dami.TraduCuvant(msg);                               
                                break;

                        }
                    }
                }
                else
                {
                    IncarcaGridSes();
                    string msg = Session["SelectareText"].ToString();
                    lblSes.Text = Dami.TraduCuvant(msg);
                    if (Session["SelectareBeneficii"].ToString() == "0")
                    {
                        grDateSes.Visible = false;
                        btnValidare.Visible = false;
                    }
                        
                    if (Session["SelectareBeneficii"].ToString() == "3")
                    {
                        btnValidare.Enabled = false;
                        DataTable dtBenAles = Session["Select_BenAles"] as DataTable;
                        for (int i = 0; i < grDateSes.VisibleRowCount; i++)
                        {
                            DataRowView obj = grDateSes.GetRow(i) as DataRowView;
                            if (Convert.ToInt32(obj["Id"].ToString()) == Convert.ToInt32(dtBenAles.Rows[0]["IdBeneficiu"].ToString()))
                            {
                                grDateSes.Selection.SelectRow(i);
                                break;
                            }
                        }
                        grDateSes.SettingsBehavior.AllowSelectByRowClick = false;
                        grDateSes.Enabled = false;
                    }
                }               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnValidare_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> lst = grDateSes.GetSelectedFieldValues(new string[] { "Id", "Descriere" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {

                    grDateSes.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }

                object[] arr = lst[0] as object[];

                DataTable dt = Session["Select_Sesiune"] as DataTable;

                General.ExecutaNonQuery("DELETE FROM Ben_Cereri WHERE \"IdSesiune\" = " + dt.Rows[0]["Id"].ToString() + " AND F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99), null);

                General.ExecutaNonQuery("INSERT INTO \"Ben_Cereri\" (IdSesiune, F10003, IdBeneficiu, IdStare, USER_NO, TIME) VALUES (" + dt.Rows[0]["Id"].ToString() + ", " + Convert.ToInt32(Session["User_Marca"] ?? -99) + ", "
                    + arr[0].ToString() + ", 2, " + Session["UserId"] + ", GETDATE())", null);           

                Session["SelectareBeneficii"] = 2;
                string msg = Session["SelectareText"].ToString();
                string[] param = msg.Split('\n');
                lblSes.Text = Dami.TraduCuvant(param[0] + "\n" + param[1] + "\n" + "<b>Optiunea ta privind selectarea Beneficiilor este in curs de aprobare. Vei primi un e-mail in momentul in care solicitarea ta va fi validata</b>.");

                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        private void IncarcaGridBen()
        {
            DataTable dt = new DataTable();

            try
            {


                if (Session["Select_GridBen"] == null)
                {
                    dt = SelectGridBen();
                    grDateBen.DataSource = dt;
                    grDateBen.KeyFieldName = "IdBeneficiu";
                    grDateBen.DataBind();
                    Session["Select_GridBen"] = dt;
                }
                else
                {
                    grDateBen.KeyFieldName = "IdBeneficiu";

                    dt = Session["Select_GridBen"] as DataTable;
                    grDateBen.DataSource = dt;
                    grDateBen.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGridSes()
        {
            DataTable dt = new DataTable();

            try
            {


                if (Session["Select_GridSes"] == null)
                {
                    dt = SelectGridSes();
                    grDateSes.DataSource = dt;
                    grDateSes.KeyFieldName = "Id";
                    grDateSes.DataBind();
                    Session["Select_GridSes"] = dt;
                }
                else
                {
                    grDateSes.KeyFieldName = "Id";

                    dt = Session["Select_GridSes"] as DataTable;
                    grDateSes.DataSource = dt;
                    grDateSes.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private string SelectBeneficii()
        {
            string strSql = "";

            try
            {
                strSql = @"select CAST (a.""Id"" AS INT) as ""Id"", a.""Denumire"", a.""Descriere""
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal') ORDER BY a.""Denumire""";


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

     
        public DataTable SelectGridBen()
        {

            DataTable q = null;

            try
            {
                string strSql = "";

                //strSql = @"select a.""IdCategorie"", a.""Id"", a.""Denumire"", a.""DeLaData"", a.""LaData"", a.""Descriere"", a.USER_NO, a.TIME, a.ValoareEstimata
                //                 from ""Admin_Obiecte"" a
                //                 inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                //                 where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal')
                //                 AND a.""DeLaData"" <= getdate() and getdate() <= a.""LaData""
                //                 ORDER BY a.""Denumire""";
                strSql = @"Select Ben_Cereri.*,  e.DataInceput as DataInceputBen, e.DataSfarsit as DataSfarsitBen, c.Descriere from Ben_Cereri
                    Join Ben_tblSesiuni on Ben_Cereri.IdSesiune = Ben_tblSesiuni.Id
                     LEFT JOIN Admin_Obiecte c ON c.Id = Ben_Cereri.IdBeneficiu 
                     inner join Admin_Categorii d on c.IdCategorie = d.Id 
                     left join Ben_relSesGrupBen e on e.IdSesiune = Ben_Cereri.IdSesiune and e.IdGrup = c.IdGrup   
                     where d.IdArie = (select Valoare from tblParametrii where Nume = 'ArieTabBeneficiiDinPersonal')                   
                    AND Ben_Cereri.idstare = 3 
                    and cast(getdate() as date) <= e.DataSfarsit
                    And F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99);

                //Radu 09.11.2021 - #1047 - sa vada toate beneficiile active/aprobate       and cast(getdate() as date) between e.DataInceput and e.DataSfarsit

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public void IncarcaBenAles()
        {

            DataTable q = null;

            try
            {
                string strSql = "";
       
                strSql = @"Select Ben_Cereri.*, e.DataSfarsit as DataSfarsitBen, c.Descriere from Ben_Cereri
                    Join Ben_tblSesiuni on Ben_Cereri.IdSesiune = Ben_tblSesiuni.Id
                     LEFT JOIN Admin_Obiecte c ON c.Id = Ben_Cereri.IdBeneficiu 
                     inner join Admin_Categorii d on c.IdCategorie = d.Id 
                    left join Ben_relSesGrupBen e on e.IdSesiune = Ben_Cereri.IdSesiune and e.IdGrup = c.IdGrup 
                     where d.IdArie = (select Valoare from tblParametrii where Nume = 'ArieTabBeneficiiDinPersonal')   
                    and cast(getdate() as date) <= e.DataSfarsit
                    And F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99);

                //AND Ben_Cereri.idstare = 3  and cast(getdate() as date) between e.DataInceput and e.DataSfarsit

                q = General.IncarcaDT(strSql, null);
                Session["Select_BenAles"] = q;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }          

        }

        public DataTable SelectGridSes()
        {

            DataTable q = null;

            try
            {
                string strSql = "";

                strSql = "SELECT Ben_relSesGrupBen.IdAuto, F10003, Ben_Cereri.IdSesiune, IdBeneficiu, Ben_tblSesiuni.DataInceput, Ben_tblSesiuni.DataSfarsit, Ben_Cereri.IdStare, Ben_relSesGrupBen.DataInceput as DataInceputBen, Ben_relSesGrupBen.DataSfarsit as DataSfarsitBen, Motiv, Ben_Cereri.USER_NO, Ben_Cereri.TIME FROM Ben_Cereri "
                    + " LEFT JOIN Ben_tblSesiuni ON Ben_Cereri.IdSesiune = Ben_tblSesiuni.Id "
                    + " left join Ben_relSesGrupBen on Ben_tblSesiuni.Id = Ben_relSesGrupBen.IdSesiune "
                    + " WHERE convert(date, Ben_tblSesiuni.DataInceput) <= convert(date, GETDATE()) AND convert(date, GETDATE()) <= convert(date, Ben_tblSesiuni.DataSfarsit) AND F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99);
                DataTable dt = General.IncarcaDT(strSql, null);
                Session["SelectBen_Ses"] = dt;

                //strSql = @"select a.""Id"", a.""Descriere"", a.USER_NO, a.TIME
                //                from ""Admin_Obiecte"" a
                //                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                //                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal')
                //                AND a.""DeLaData"" <=GETDATE() AND GETDATE() <= a.""LaData""
                //                ORDER BY a.""Denumire""";

                strSql = @"select a.""IdCategorie"", a.""Id"", a.""Denumire"", d.DataInceput as DataInceputBen, d.DataSfarsit as DataSfarsitBen, a.""Descriere"", a.USER_NO, a.TIME, a.ValoareEstimata, {0} AS F10003
                                 from ""Admin_Obiecte"" a
                                 inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                 LEFT JOIN Ben_tblSesiuni c on convert(date, c.""DataInceput"") <= Convert(date, getdate()) and Convert(date, getdate()) <= convert(date, c.""DataSfarsit"")
                                 left join Ben_relSesGrupBen d on c.Id = d.IdSesiune and d.IdGrup = a.IdGrup  
                                 where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal') 
                                 and convert(date, a.""DeLaData"") <= convert(date, getdate()) and convert(date, getdate()) <= convert(date, a.""LaData"")  
                                 and   d.DataInceput is not null and d.DataSfarsit is not null
                                 ORDER BY a.""Denumire""";

                strSql = string.Format(strSql, Convert.ToInt32(Session["User_Marca"] ?? -99));

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        protected void btnDocUploadBen_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_SelBen"] = itm;

                //btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void btnDocUploadSes_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_SelSes"] = itm;

                //btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateBen_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Select_GridBen"] as DataTable;

                DataRow dr = dt.Rows.Find(keys);

                metaUploadFile itm = Session["DocUpload_SelBen"] as metaUploadFile;
                if (itm != null)
                    General.LoadFile(itm.UploadedFileName.ToString(), itm.UploadedFile, "Beneficii_Ang", Convert.ToInt32(Session["User_Marca"] ?? -99));

                e.Cancel = true;
                grDateBen.CancelEdit();
                grDateBen.DataSource = dt;
                grDateBen.KeyFieldName = "IdBeneficiu";
                Session["Select_GridBen"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSes_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Select_GridSes"] as DataTable;

                DataRow dr = dt.Rows.Find(keys);

                metaUploadFile itm = Session["DocUpload_SelSes"] as metaUploadFile;
                if (itm != null)
                    General.LoadFile(itm.UploadedFileName.ToString(), itm.UploadedFile, "Beneficii_Ang", Convert.ToInt32(Session["User_Marca"] ?? -99));

                e.Cancel = true;
                grDateSes.CancelEdit();
                grDateSes.DataSource = dt;
                grDateSes.KeyFieldName = "Id";
                Session["Select_GridSes"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}