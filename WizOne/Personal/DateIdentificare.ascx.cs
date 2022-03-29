using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Configuration;
//using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Web.Hosting;

namespace WizOne.Personal
{
    public partial class DateIdentificare : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                
                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                #endregion
                DataTable table = new DataTable();
                
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                table = ds.Tables[0];

                DateIdentificare_DataList.DataSource = table;
                if (!IsPostBack)    //#1032
                    DateIdentificare_DataList.DataBind();

                if (!IsPostBack)
                {
                    DateTime dtN = Convert.ToDateTime(table.Rows[0]["F10021"].ToString());
                    if (dtN.Year != 1900)
                    {
                        ASPxTextBox txtVarsta = DateIdentificare_DataList.Items[0].FindControl("txtVarsta") as ASPxTextBox;
                        txtVarsta.Text = Dami.Varsta(dtN).ToString();  //((DateTime.Now - dtN).TotalDays/365).ToString();
                    }
                }

                string marcaUnica = Dami.ValoareParam("MP_ModificareMarcaUnica", "0");
                ASPxTextBox txtMarcaUnica = DateIdentificare_DataList.Items[0].FindControl("txtMarcaUnica") as ASPxTextBox;
                if (marcaUnica == "1")
                    txtMarcaUnica.ClientEnabled = true;

                btnIncarca_Click();

                ASPxRadioButtonList rbSex = DateIdentificare_DataList.Items[0].FindControl("rbSex") as ASPxRadioButtonList;
                if (!IsPostBack)
                    rbSex.Value = General.Nz(table.Rows[0]["F10047"],1).ToString();
                rbSex.Items[0].Text = Dami.TraduCuvant(rbSex.Items[0].Text);
                rbSex.Items[1].Text = Dami.TraduCuvant(rbSex.Items[1].Text);

                ASPxTextBox txtMarca = DateIdentificare_DataList.Items[0].FindControl("txtMarcaDI") as ASPxTextBox;
                txtMarca.ReadOnly = true;
                DataTable dtTemp = General.IncarcaDT("SELECT * FROM \"tblParametrii\" WHERE \"Nume\" = 'MarcaAngajatuluiPoateFiIntrodusa'", null);
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["Valoare"] != null && dtTemp.Rows[0]["Valoare"].ToString() == "1")  
                        txtMarca.ReadOnly = false;

                string[] etichete = new string[12] { "lblMarca", "lblCNP", "lblMarcaUnica", "lblEIDDI", "lblNume", "lblPrenume", "lblNumeAnt", "lblDataModifNume", "lblDataNasterii", "lblSex", "lblStareCivila", "lblVarsta" };
                for (int i = 0; i < etichete.Count(); i++)
                {
                    ASPxLabel lbl = DateIdentificare_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                    lbl.Text = Dami.TraduCuvant(lbl.Text);
                }

                string[] butoane = new string[4] { "btnNume", "btnNumeIst", "btnPrenume", "btnPrenumeIst" };
                for (int i = 0; i < butoane.Count(); i++)
                {
                    ASPxButton btn = DateIdentificare_DataList.Items[0].FindControl(butoane[i]) as ASPxButton;
                    btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
                }

                btnDocUpload.BrowseButton.Text = Dami.TraduCuvant(btnDocUpload.BrowseButton.Text);
                btnDocUpload.ToolTip = Dami.TraduCuvant(btnDocUpload.ToolTip);
                btnSterge.ToolTip = Dami.TraduCuvant(btnSterge.ToolTip);
                btnSterge.Text = Dami.TraduCuvant(btnSterge.Text);
                
                lgFoto.InnerText = Dami.TraduCuvant("Fotografie");
                HtmlGenericControl lgIdent = DateIdentificare_DataList.Items[0].FindControl("lgIdent") as HtmlGenericControl;
                lgIdent.InnerText = Dami.TraduCuvant("Date unice de identificare");
                HtmlGenericControl lgSex = DateIdentificare_DataList.Items[0].FindControl("lgSex") as HtmlGenericControl;
                lgSex.InnerText = Dami.TraduCuvant("Data nasterii si Sex");
                HtmlGenericControl lgNume = DateIdentificare_DataList.Items[0].FindControl("lgNume") as HtmlGenericControl;
                lgNume.InnerText = Dami.TraduCuvant("Nume si prenume");

                if (!IsPostBack)
                {
                    if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.Trico)
                    {
                        if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                        {
                            ASPxTextBox txtEIDDI = DateIdentificare_DataList.Items[0].FindControl("txtEIDDI") as ASPxTextBox;
                            txtEIDDI.Text = "FM0" + txtMarca.Text;
                        }
                    }
                    if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.ALKA_CO)
                    {
                        if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                        {
                            ASPxTextBox txtEIDDI = DateIdentificare_DataList.Items[0].FindControl("txtEIDDI") as ASPxTextBox;
                            txtEIDDI.Text = "10" + txtMarca.Text.PadLeft(4, '0');
                        }
                    }
                    if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.ALKA_Trading)
                    {
                        if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                        {
                            ASPxTextBox txtEIDDI = DateIdentificare_DataList.Items[0].FindControl("txtEIDDI") as ASPxTextBox;
                            txtEIDDI.Text = "70" + txtMarca.Text.PadLeft(4, '0');
                        }
                    }
                }
                
                if (Dami.ValoareParam("ValidariPersonal") == "1")
                {
                    ASPxTextBox txtCNPDI = DateIdentificare_DataList.Items[0].FindControl("txtCNPDI") as ASPxTextBox;
                    ASPxTextBox txtNume = DateIdentificare_DataList.Items[0].FindControl("txtNume") as ASPxTextBox;
                    ASPxTextBox txtPrenume = DateIdentificare_DataList.Items[0].FindControl("txtPrenume") as ASPxTextBox;
                    List<int> lst = new List<int>();
                    if (Session["MP_CuloareCampOblig"] != null)
                        lst = Session["MP_CuloareCampOblig"] as List<int>;

                    txtCNPDI.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                    txtNume.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                    txtPrenume.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                    txtMarca.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                }          
                General.SecuritatePersonal(DateIdentificare_DataList, Convert.ToInt32(Session["UserId"].ToString()));
                General.SecuritatePersonal(DateIdentificare_pnlCtl, Convert.ToInt32(Session["UserId"].ToString()));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

           

        }


        protected void pnlCtlDateIdent_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string[] param = e.Parameter.Split(';');
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                switch (param[0])
                {  
                    case "txtCNPDI":
                        txtCNP_LostFocus();
                        break;
                    case "txtMarcaDI":
                        int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT COUNT(*) FROM F100 WHERE F10003 =@1", new object[] { param[1] }),0));
                        if (cnt == 0)
                        {
                            ds.Tables[0].Rows[0]["F10003"] = param[1];

                            int x = 0;
                            foreach (DataColumn col in ds.Tables[0].Columns)
                            {
                                switch (col.ColumnName.ToUpper())
                                {
                                    case "F10003":
                                        ds.Tables[0].Rows[0][x + ds.Tables[1].Columns.Count] = param[1];
                                        break;
                                }
                                x++;
                            }

                            ds.Tables[0].Rows[0]["F10003"] = param[1];
                            ds.Tables[1].Rows[0]["F10003"] = param[1];
                            ds.Tables[2].Rows[0]["F10003"] = param[1];
                            ds.Tables[0].Rows[0]["F100985"] = param[1];
                            ds.Tables[1].Rows[0]["F100985"] = param[1];

                            DateAngajat pagDA = new DateAngajat();
                            pagDA.SchimbaMarca(ds, Convert.ToInt32(param[1]), Convert.ToInt32(Session["Marca"].ToString()));

                            Session["Marca"] = param[1];
                            Session["InformatiaCurentaPersonal"] = ds;

                            if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.Trico)
                                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                                {
                                    ASPxTextBox txtEIDDI = DateIdentificare_DataList.Items[0].FindControl("txtEIDDI") as ASPxTextBox;
                                    ASPxTextBox txtMarca = DateIdentificare_DataList.Items[0].FindControl("txtMarcaDI") as ASPxTextBox;
                                    txtEIDDI.Text = "FM0" + txtMarca.Text;
                                }
                        }
                        else
                            DateIdentificare_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Marca este deja alocata!");
                        break;
                    case "btnNume":
                        ModifAvans((int)Constante.Atribute.NumePrenume);
                        break;
                    case "btnPrenume":
                        ModifAvans((int)Constante.Atribute.NumePrenume);
                        break;
                    case "PreluareDate":
                        ASPxTextBox txtCNP = DateIdentificare_DataList.Items[0].FindControl("txtCNPDI") as ASPxTextBox;
                        PreluareDateContract(txtCNP.Text);
                        break;
                    case "img":
                        byte[] fisier = Session["DateIdentificare_Fisier"] as byte[];
                        string base64String = Convert.ToBase64String(fisier, 0, fisier.Length);
                        img.Src = "data:image/jpg;base64," + base64String;
                        break;
                    case "btnSterge":
                        btnSterge_Click();
                        img.Src = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Florin 20.08.2020 - refacuta
        private void btnIncarca_Click()
        {
            try
            {
                if (Session["Marca"] != null)
                {
                    byte[] fisier = null;
                    string numeFisier = "";
                    DataTable dt = new DataTable();
                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                    DataRow dr = null;
                    if (ds.Tables.Contains("tblFisiere"))
                    {
                        dt = ds.Tables["tblFisiere"];
                        DataRow[] arr = dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString());
                        if (arr.Count() > 0) dr = arr[0];
                    }
                    else
                        dr = General.IncarcaDR($@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela"" = 'F100' AND ""Id"" = {General.Nz(Session["Marca"], -99)}", null);
                    

                    if (dr != null)
                    {
                        fisier = (byte[])General.Nz(dr["Fisier"],null);
                        numeFisier = General.Nz(dr["FisierNume"],"").ToString();
                    }

                    if (fisier != null)
                    {
                        string base64String = Convert.ToBase64String(fisier, 0, fisier.Length);
                        img.Src = "data:image/jpg;base64," + base64String;
                    }
                    else
                    {
                        if (numeFisier != "")
                        {
                            string cale = "~/FisiereApp/Fisiere/" + numeFisier;
                            if (File.Exists(HostingEnvironment.MapPath(cale)))
                            {
                                img.Src = cale;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //private void btnIncarca_Click()
        //{
        //    try
        //    {
        //        if (Session["Marca"] != null)
        //        {
        //            byte[] fisier = null;
        //            DataTable dt = new DataTable();
        //            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //            if (ds.Tables.Contains("tblFisiere"))
        //            {
        //                dt = ds.Tables["tblFisiere"];
        //                DataRow dr = null;
        //                if (dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString()).Count() > 0)
        //                {
        //                    dr = dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString()).FirstOrDefault();
        //                    fisier = dr["Fisier"] as byte[];
        //                }
        //                else
        //                {
        //                    fisier = General.IncarcaFotografie(img, Convert.ToInt32(Session["Marca"].ToString()), "F100") as byte[];
        //                }
        //            }
        //            else
        //            {
        //                fisier = General.IncarcaFotografie(img, Convert.ToInt32(Session["Marca"].ToString()), "F100") as byte[];
        //            }
        //            if (fisier != null)
        //            {
        //                string base64String = Convert.ToBase64String(fisier, 0, fisier.Length);
        //                img.Src = "data:image/jpg;base64," + base64String;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        private void btnSterge_Click()
        {
            try
            {
                string sql = "SELECT * FROM \"tblFisiere\"";
                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("tblFisiere"))
                {
                    dt = ds.Tables["tblFisiere"];
                }
                else
                {
                    dt = General.IncarcaDT(sql, null);
                    dt.TableName = "tblFisiere";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }
                DataRow dr = null;
                if (dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString()).Count() > 0)               
                {
                    dr = dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString()).FirstOrDefault();
                    dr.Delete();
                }
                Session["InformatiaCurentaPersonal"] = ds;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private bool txtCNP_LostFocus()
        {
            bool valid = false;
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            try
            {
                ASPxTextBox txtCNP = DateIdentificare_DataList.Items[0].FindControl("txtCNPDI") as ASPxTextBox;
                if (txtCNP.Text != null && txtCNP.Text != "")
                {
                    int marcaUnica = GetMarcaUnica(txtCNP.Text);

                    ASPxTextBox txtMarcaUnica = DateIdentificare_DataList.Items[0].FindControl("txtMarcaUnica") as ASPxTextBox;

                    txtMarcaUnica.Text = marcaUnica.ToString();
                    //txtMarcaUnica.ReadOnly = true;

                    ds.Tables[0].Rows[0]["F1001036"] = marcaUnica;
                    ds.Tables[2].Rows[0]["F1001036"] = marcaUnica;

                    //Florin 2019.12.30
                    //dezactivat de moment

                    //if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    //{
                    //    DataTable dt100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10017 = '" + txtCNP.Text + "'", null);

                    //    if (dt100 != null && dt100.Rows.Count > 0)
                    //        DateIdentificare_pnlCtl.JSProperties["cp_InfoMessage"] = Dami.TraduCuvant("Angajatul exista deja in baza de date.\nDoriti preluarea datelor de pe cel mai recent contract?");
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {

                string sql = "SELECT * FROM \"tblFisiere\"";
                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("tblFisiere"))
                {
                    dt = ds.Tables["tblFisiere"];
                }
                else
                {
                    dt = General.IncarcaDT(sql, null);
                    dt.TableName = "tblFisiere";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }

                DataRow dr = null;
                if (dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString()).Count() == 0)
                {
                    dr = dt.NewRow();
                    dr["Tabela"] = "F100";
                    dr["Id"] = Convert.ToInt32(Session["Marca"].ToString());
                    dr["Fisier"] = btnDocUpload.UploadedFiles[0].FileBytes;
                    dr["FisierNume"] = btnDocUpload.UploadedFiles[0].FileName;
                    dr["FisierExtensie"] = btnDocUpload.UploadedFiles[0].ContentType;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    if (Constante.tipBD == 1)
                        dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                    else
                        dr["IdAuto"] = Dami.NextId("tblFisiere");
                    dr["EsteCerere"] = 0;
                    dt.Rows.Add(dr);
                }
                else
                {
                    dr = dt.Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString()).FirstOrDefault();
                    dr["Fisier"] = btnDocUpload.UploadedFiles[0].FileBytes;
                    dr["FisierNume"] = btnDocUpload.UploadedFiles[0].FileName;
                    dr["FisierExtensie"] = btnDocUpload.UploadedFiles[0].ContentType;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                }

                Session["InformatiaCurentaPersonal"] = ds;
                
                MemoryStream ms = new MemoryStream(btnDocUpload.UploadedFiles[0].FileBytes);

                Session["DateIdentificare_Fisier"] = btnDocUpload.UploadedFiles[0].FileBytes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnDoc_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //Session["Absente_Cereri_UploadedFile"] = null;
        //        //Session["Absente_Cereri_UploadedFileName"] = null;
        //        //Session["Absente_Cereri_UploadedFileExtension"] = null;
        //    }
        //    catch (Exception ex)
        //    {
        //       // MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //       // General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private int SexDinCNP(char c)
        //{
        //    // 0 - masculin
        //    // 1 - feminin

        //    try
        //    {
        //        int gender = -1;

        //        switch (c)
        //        {
        //            case '1':
        //            case '3':
        //            case '5':
        //                gender = 0;
        //                break;

        //            case '2':
        //            case '4':
        //            case '6':
        //                gender = 1;
        //                break;
        //        }

        //        return gender;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //        return -1;
        //    }
        //}


        //private void DataNasterii(string cnp)
        //{
        //    try
        //    {
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

        //        DateTime dtN = General.getDataNasterii(cnp);
        //        ASPxDateEdit deDataNasterii = DateIdentificare_DataList.Items[0].FindControl("deDataNasterii") as ASPxDateEdit;
        //        deDataNasterii.Value =dtN;
        //        //_dataNasterii = deDataNasterii.DateTime;
        //        ASPxTextBox txtVarsta = DateIdentificare_DataList.Items[0].FindControl("txtVarsta") as ASPxTextBox;
        //        txtVarsta.Text = Varsta(Convert.ToDateTime(deDataNasterii.Value)).ToString();

        //        ds.Tables[0].Rows[0]["F10021"] = new DateTime(dtN.Year, dtN.Month, dtN.Day);
        //        ds.Tables[1].Rows[0]["F10021"] = new DateTime(dtN.Year, dtN.Month, dtN.Day);
        //        Session["InformatiaCurentaPersonal"] = ds;

        //    }
        //    catch (Exception ex)
        //    {
        //       // Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //public int Varsta(DateTime dataNasterii)
        //{
        //    try
        //    {
        //        int years = -99;
        //        int months = -99;
        //        int days = -99;

        //        General.DateDiff(DateTime.Now, dataNasterii, out years, out months, out days);

        //        return years;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //        return -99;
        //    }
        //}

        //private bool VerificaMarca(string marca)
        //{
        //    DataTable dt = General.IncarcaDT("SELECT COUNT(*) FROM F100 WHERE F10003 = " + marca, null);
        //    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString() == "1")
        //        return false;
        //    else
        //        return true;
        //}


        public int GetMarcaUnica(string cnp)
        {
            int rez = 0;

            try
            {
                DataTable dtAngajat = General.IncarcaDT("SELECT * FROM F100 WHERE F10017 = '" + cnp + "'", null);

                if (dtAngajat != null && dtAngajat.Rows.Count > 0)
                {
                    DataTable dt1001 = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + dtAngajat.Rows[0]["F10003"].ToString(), null);
                    if (dt1001 != null && dt1001.Rows.Count > 0 && dt1001.Rows[0]["F1001036"] != null && dt1001.Rows[0]["F1001036"].ToString().Length > 0)
                    {                  
                        rez = Convert.ToInt32(dt1001.Rows[0]["F1001036"].ToString());                        
                    }                    
                }
                else
                {
                    DataTable dt1001 = General.IncarcaDT("SELECT MAX(F1001036) FROM F1001", null);
                    int mr = 0;
                    if (dt1001 != null && dt1001.Rows.Count > 0 && dt1001.Rows[0][0] != null && dt1001.Rows[0][0].ToString().Length > 0)
                        mr = Convert.ToInt32(dt1001.Rows[0][0].ToString());                 
                    rez = mr + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }


        public void PreluareDateContract(string CNP)
        {
            try
            {
                DataTable dt100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10017 = '" + CNP + "'  ORDER BY F10022 DESC", null);

                if (dt100 != null && dt100.Rows.Count > 0)
                {
                    int f1003Dup = Convert.ToInt32(dt100.Rows[0]["F10003"].ToString());

                    //incarcam datele pt F100
                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                    for (int i = 0; i <= 1; i++)
                    {
                        ds = Session["InformatiaCurentaPersonal"] as DataSet;
                        ds.Tables[i].Rows[0]["F10008"] = dt100.Rows[0]["F10008"];
                        ds.Tables[i].Rows[0]["F10009"] = dt100.Rows[0]["F10009"];
                        ds.Tables[i].Rows[0]["F10017"] = dt100.Rows[0]["F10017"];
                        ds.Tables[i].Rows[0]["F10012"] = dt100.Rows[0]["F10012"];
                        ds.Tables[i].Rows[0]["F10013"] = dt100.Rows[0]["F10013"];
                        ds.Tables[i].Rows[0]["F10021"] = dt100.Rows[0]["F10021"];
                        ds.Tables[i].Rows[0]["F10047"] = dt100.Rows[0]["F10047"];
                        ds.Tables[i].Rows[0]["F100905"] = dt100.Rows[0]["F100905"];
                        ds.Tables[i].Rows[0]["F100906"] = dt100.Rows[0]["F100906"];
                        ds.Tables[i].Rows[0]["F10046"] = dt100.Rows[0]["F10046"];
                        ds.Tables[i].Rows[0]["F100987"] = dt100.Rows[0]["F100987"];
                        ds.Tables[i].Rows[0]["F100981"] = dt100.Rows[0]["F100981"];
                        ds.Tables[i].Rows[0]["F100911"] = dt100.Rows[0]["F100911"];
                        ds.Tables[i].Rows[0]["F100912"] = dt100.Rows[0]["F100912"];
                        ds.Tables[i].Rows[0]["F100913"] = dt100.Rows[0]["F100913"];
                        ds.Tables[i].Rows[0]["F100982"] = dt100.Rows[0]["F100982"];
                        ds.Tables[i].Rows[0]["F100983"] = dt100.Rows[0]["F100983"];
                        ds.Tables[i].Rows[0]["F10052"] = dt100.Rows[0]["F10052"];
                        ds.Tables[i].Rows[0]["F100521"] = dt100.Rows[0]["F100521"];
                        ds.Tables[i].Rows[0]["F100522"] = dt100.Rows[0]["F100522"];
                        ds.Tables[i].Rows[0]["F100980"] = dt100.Rows[0]["F100980"];
                        ds.Tables[i].Rows[0]["F100988"] = dt100.Rows[0]["F100988"];
                        ds.Tables[i].Rows[0]["F100989"] = dt100.Rows[0]["F100989"];
                        ds.Tables[i].Rows[0]["F100994"] = dt100.Rows[0]["F100994"];
                        ds.Tables[i].Rows[0]["F100940"] = dt100.Rows[0]["F100940"];
                        ds.Tables[i].Rows[0]["F100941"] = dt100.Rows[0]["F100941"];
                        ds.Tables[i].Rows[0]["F100942"] = dt100.Rows[0]["F100942"];
                        ds.Tables[i].Rows[0]["F10028"] = dt100.Rows[0]["F10028"];
                        ds.Tables[i].Rows[0]["F10024"] = dt100.Rows[0]["F10024"];
                        ds.Tables[i].Rows[0]["F10050"] = dt100.Rows[0]["F10050"];           
                        ds.Tables[i].Rows[0]["F10051"] = dt100.Rows[0]["F10051"];
                        ds.Tables[i].Rows[0]["F10073"] = dt100.Rows[0]["F10073"];
                        ds.Tables[i].Rows[0]["F100571"] = dt100.Rows[0]["F100571"];
                        ds.Tables[i].Rows[0]["F100572"] = dt100.Rows[0]["F100572"];
                        ds.Tables[i].Rows[0]["F100573"] = dt100.Rows[0]["F100573"];
                        ds.Tables[i].Rows[0]["F100574"] = dt100.Rows[0]["F100574"];
                        ds.Tables[i].Rows[0]["F100575"] = dt100.Rows[0]["F100575"];
                        ds.Tables[i].Rows[0]["F100576"] = dt100.Rows[0]["F100576"];
                    }
                    //incarcam datele pt F1001
                    DataTable dt1001 = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + f1003Dup, null);
                    if (dt1001 != null && dt1001.Rows.Count > 0)
                    {
                        for (int i = 0; i <= 2; i = i + 2)
                        {
                            ds.Tables[i].Rows[0]["F1001037"] = dt1001.Rows[0]["F1001037"];
                            ds.Tables[i].Rows[0]["F1001000"] = dt1001.Rows[0]["F1001000"];
                            ds.Tables[i].Rows[0]["F1001001"] = dt1001.Rows[0]["F1001001"];
                            ds.Tables[i].Rows[0]["F1001002"] = dt1001.Rows[0]["F1001002"];
                            ds.Tables[i].Rows[0]["F100963"] = dt1001.Rows[0]["F100963"];
                            ds.Tables[i].Rows[0]["F1001132"] = dt1001.Rows[0]["F1001132"];
                            ds.Tables[i].Rows[0]["F1001133"] = dt1001.Rows[0]["F1001133"];
                        }
                    }

                    //ASPxTextBox txtNume = DateIdentificare_DataList.Items[0].FindControl("txtNume") as ASPxTextBox;
                    //ASPxTextBox txtPreume = DateIdentificare_DataList.Items[0].FindControl("txtPrenume") as ASPxTextBox;
                    //ASPxTextBox txtNumeAnt = DateIdentificare_DataList.Items[0].FindControl("txtNumeAnt") as ASPxTextBox;
                    //ASPxDateEdit deDataModifNume = DateIdentificare_DataList.Items[0].FindControl("deDataModifNume") as ASPxDateEdit;
                    //ASPxComboBox cmbStareCivila = DateIdentificare_DataList.Items[0].FindControl("cmbStareCivila") as ASPxComboBox;

                    //txtNume.Text = dt100.Rows[0]["F10008"].ToString();
                    //txtPreume.Text = dt100.Rows[0]["F10009"].ToString();
                    //txtNumeAnt.Text = dt100.Rows[0]["F100905"].ToString();
                    //if (dt100.Rows[0]["F100906"] != null && dt100.Rows[0]["F100906"].ToString().Length > 0)
                    //    deDataModifNume.Value = Convert.ToDateTime(dt100.Rows[0]["F100906"].ToString());
                    //cmbStareCivila.Value = Convert.ToInt32(dt100.Rows[0]["F10046"].ToString());


                    Session["InformatiaCurentaPersonal"] = ds;
                    //Session["PreluareDate"] = 1;

                    ASPxPageControl ctl = this.NamingContainer as ASPxPageControl;
                    foreach (TabPage tab in ctl.TabPages)
                    {
                        for (int j = 0; j < tab.Controls[0].Controls.Count; j++)
                        {
                            if (tab.Controls[0].Controls[j].GetType() == typeof(DevExpress.Web.ASPxCallbackPanel))
                            {
                                ASPxCallbackPanel cb = tab.Controls[0].Controls[j] as ASPxCallbackPanel;
                                for (int k = 0; k < cb.Controls.Count; k++)
                                {
                                    if (cb.Controls[k].GetType() == typeof(DataList))
                                    {
                                        DataList dl = cb.Controls[k] as DataList;
                                        dl.DataSource = null;
                                        dl.DataBind();
                                        DataSet dsNou = Session["InformatiaCurentaPersonal"] as DataSet;
                                        DataTable table = dsNou.Tables[0];
                                        dl.DataSource = table;
                                        dl.DataBind();
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void ModifAvans(int atribut)
        {
            try
            {
                string strRol = Avs.Cereri.DamiRol(Convert.ToInt32(General.Nz(Session["Marca"], -99)), atribut);
                if (strRol == "")
                {
                    if (Page.IsCallback)
                        DateIdentificare_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie");
                    else
                        MessageBox.Show("Nu aveti drepturi pentru aceasta operatie", MessageBox.icoWarning, "Atentie");
                }
                else
                {
                    string url = "~/Avs/Cereri";
                    Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString() + ";" + strRol;
                    Session["MP_Avans"] = "true";
                    Session["MP_Avans_Tab"] = "DateIdentificare";
                    if (Page.IsCallback)
                        ASPxWebControl.RedirectOnCallback(url);
                    else
                        Response.Redirect(url, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}