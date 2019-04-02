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

                DateIdentListView.DataSource = table;
                DateIdentListView.DataBind();


                ASPxTextBox txtV = DateIdentListView.Items[0].FindControl("txtVarsta") as ASPxTextBox;
                ASPxRadioButton chk1 = DateIdentListView.Items[0].FindControl("chkM") as ASPxRadioButton;
                ASPxRadioButton chk2 = DateIdentListView.Items[0].FindControl("chkF") as ASPxRadioButton;

                DateTime dt = DateTime.Now;
                DateTime dtN = Convert.ToDateTime(table.Rows[0]["F10021"].ToString());

                //txtV.Text = (dt.Year - dtN.Year).ToString();
                txtV.Text = Varsta(dtN).ToString();


                if (table.Rows[0]["F10047"].ToString() == "1")
                {
                    chk1.Checked = true;
                    chk2.Checked = false;      
                }
                else
                {
                    chk1.Checked = false;
                    chk2.Checked = true;
                }

                ASPxTextBox txtMarca = DateIdentListView.Items[0].FindControl("txtMarcaDI") as ASPxTextBox;
                txtMarca.ReadOnly = true;
                DataTable dtTemp = General.IncarcaDT("SELECT * FROM \"tblParametrii\" WHERE \"Nume\" = 'MarcaAngajatuluiPoateFiIntrodusa'", null);
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["Valoare"] != null && dtTemp.Rows[0]["Valoare"].ToString() == "1")  
                        txtMarca.ReadOnly = false;

                string[] etichete = new string[12] { "lblMarca", "lblCNP", "lblMarcaUnica", "lblEID", "lblNume", "lblPrenume", "lblNumeAnt", "lblDataModifNume", "lblDataNasterii", "lblSex", "lblStareCivila", "lblVarsta" };
                for (int i = 0; i < etichete.Count(); i++)
                {
                    ASPxLabel lbl = DateIdentListView.Items[0].FindControl(etichete[i]) as ASPxLabel;
                    lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
                }

                string[] butoaneRadio = new string[2] { "chkM", "chkF" };
                for (int i = 0; i < butoaneRadio.Count(); i++)
                {
                    ASPxRadioButton radio = DateIdentListView.Items[0].FindControl(butoaneRadio[i]) as ASPxRadioButton;
                    radio.Text = Dami.TraduCuvant(radio.Text);
                }

                string[] butoane = new string[4] { "btnNume", "btnNumeIst", "btnPrenume", "btnPrenumeIst" };
                for (int i = 0; i < butoane.Count(); i++)
                {
                    ASPxButton btn = DateIdentListView.Items[0].FindControl(butoane[i]) as ASPxButton;
                    btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
                }

                btnDocUpload.BrowseButton.Text = Dami.TraduCuvant(btnDocUpload.BrowseButton.Text);
                btnDocUpload.ToolTip = Dami.TraduCuvant(btnDocUpload.ToolTip);
                btnDoc2.ToolTip = Dami.TraduCuvant(btnDoc2.ToolTip);
                btnDoc2.Text = Dami.TraduCuvant(btnDoc2.Text);
                
                lgFoto.InnerText = Dami.TraduCuvant("Fotografie");
                HtmlGenericControl lgIdent = DateIdentListView.Items[0].FindControl("lgIdent") as HtmlGenericControl;
                lgIdent.InnerText = Dami.TraduCuvant("Date unice de identificare");
                HtmlGenericControl lgSex = DateIdentListView.Items[0].FindControl("lgSex") as HtmlGenericControl;
                lgSex.InnerText = Dami.TraduCuvant("Data nasterii si Sex");
                HtmlGenericControl lgNume = DateIdentListView.Items[0].FindControl("lgNume") as HtmlGenericControl;
                lgNume.InnerText = Dami.TraduCuvant("Nume si prenume");

                if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == 22)
                {//DNATA
                    ASPxTextBox txtCNPDI = DateIdentListView.Items[0].FindControl("txtCNPDI") as ASPxTextBox;
                    ASPxTextBox txtNume = DateIdentListView.Items[0].FindControl("txtNume") as ASPxTextBox;
                    ASPxTextBox txtPrenume = DateIdentListView.Items[0].FindControl("txtPrenume") as ASPxTextBox;
                    txtCNPDI.BackColor = Color.LightGray;
                    txtNume.BackColor = Color.LightGray;
                    txtPrenume.BackColor = Color.LightGray;
                    txtMarca.BackColor = Color.LightGray;
                }

                General.SecuritatePersonal(DateIdentListView, Convert.ToInt32(Session["UserId"].ToString()));
            }
            catch (Exception ex)
            {
                ////MessageBox.Show(this, ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }

           

        }


        protected void pnlCtlDateIdent_Callback(object source, CallbackEventArgsBase e)
        {

            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "btnIncarca":
                    btnIncarca_Click();
                    break;
                case "btnSterge":
                    btnSterge_Click();
                    break;

                case "txtCNPDI":
                    txtCNP_LostFocus();
                    //if (txtCNP_LostFocus())
                    //{
                    ds.Tables[0].Rows[0]["F10017"] = param[1];
                    ds.Tables[1].Rows[0]["F10017"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    //}
                    break;
                case "txtMarcaDI":
                    if (VerificaMarca(param[1]))
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
                        Session["Marca"] = param[1];
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    else
                        pnlCtlDateIdent.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Marca este deja alocata!");
                    break;
                case "txtEIDDI":
                    ds.Tables[0].Rows[0]["F100901"] = param[1];
                    ds.Tables[1].Rows[0]["F100901"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNume":
                    ds.Tables[0].Rows[0]["F10008"] = param[1];
                    ds.Tables[1].Rows[0]["F10008"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtPrenume":
                    ds.Tables[0].Rows[0]["F10009"] = param[1];
                    ds.Tables[1].Rows[0]["F10009"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNumeAnt":
                    ds.Tables[0].Rows[0]["F100905"] = param[1];
                    ds.Tables[1].Rows[0]["F100905"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataNasterii":
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F10021"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F10021"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    ASPxTextBox txtVarsta = DateIdentListView.Items[0].FindControl("txtVarsta") as ASPxTextBox;
                    txtVarsta.Text = Varsta(new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]))).ToString();
                    break;
                case "chkM":
                    ds.Tables[0].Rows[0]["F10047"] = (param[1] == "true" ? 1 : 0);
                    ds.Tables[1].Rows[0]["F10047"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "chkF":
                    ds.Tables[0].Rows[0]["F10047"] = (param[1] == "true" ? 0 : 1);
                    ds.Tables[1].Rows[0]["F10047"] = (param[1] == "true" ? 0 : 1);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataModifNume":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100906"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100906"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbStareCivila":
                    ds.Tables[0].Rows[0]["F10046"] = param[1];
                    ds.Tables[1].Rows[0]["F10046"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "btnNume":
                    ModifAvans((int)Constante.Atribute.NumePrenume);
                    break;
                case "btnPrenume":
                    ModifAvans((int)Constante.Atribute.NumePrenume);
                    break;
                case "PreluareDate":
                    ASPxTextBox txtCNP = DateIdentListView.Items[0].FindControl("txtCNPDI") as ASPxTextBox;
                    PreluareDateContract(txtCNP.Text);
                    break;
            }

        }

        private void btnIncarca_Click()
        {
            try
            {
                ASPxTextBox txtMarca = DateIdentListView.Items[0].FindControl("txtMarca") as ASPxTextBox;
                General.IncarcaFotografie(img, Convert.ToInt32(txtMarca.Text), "F100");
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void btnSterge_Click()
        {
            try
            {
                ASPxTextBox txtMarca = DateIdentListView.Items[0].FindControl("txtMarca") as ASPxTextBox;
                General.StergeFotografie(img, Convert.ToInt32(txtMarca.Text), "F100");
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

  


        private bool txtCNP_LostFocus()
        {
            bool valid = false;
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            try
            {
                ASPxTextBox txtCNP = DateIdentListView.Items[0].FindControl("txtCNPDI") as ASPxTextBox;
                if (txtCNP.Text != null && txtCNP.Text != "")
                {
                    string cnp = txtCNP.Text.ToString();

                    if (General.VerificaCNP(cnp))
                    {
                        DataNasterii(cnp);

                        int varsta = Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                        if (varsta < 16)
                        {
                            pnlCtlDateIdent.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti angaja o persoana cu varsta mai mica de 16 ani!");
                            return false;
                        }

                        valid = true;
                        ASPxRadioButton chk1 = DateIdentListView.Items[0].FindControl("chkM") as ASPxRadioButton;
                        ASPxRadioButton chk2 = DateIdentListView.Items[0].FindControl("chkF") as ASPxRadioButton;

                        int sex = 1;
                        if ((Convert.ToInt32(cnp[0]) % 2) != 0)
                        {
                            sex = 1;
                            chk1.Checked = true;
                            chk2.Checked = false;
                        }
                        else
                        {
                            sex = 2;
                            chk1.Checked = false;
                            chk2.Checked = true;
                        }

                        //Florin 2018.11.22
                        //ds.Tables[0].Rows[0]["F10047"] = cnp[0];
                        //ds.Tables[1].Rows[0]["F10047"] = cnp[0];
                        ds.Tables[0].Rows[0]["F10047"] = sex;
                        ds.Tables[1].Rows[0]["F10047"] = sex;


                        Session["InformatiaCurentaPersonal"] = ds;


                        int marcaUnica = GetMarcaUnica(txtCNP.Text);

                        ASPxTextBox txtMarcaUnica = DateIdentListView.Items[0].FindControl("txtMarcaUnica") as ASPxTextBox;                      

                        txtMarcaUnica.Text = marcaUnica.ToString();
                        txtMarcaUnica.ReadOnly = true;

                        ds.Tables[0].Rows[0]["F1001036"] = marcaUnica;
                        ds.Tables[2].Rows[0]["F1001036"] = marcaUnica;

                        if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                        {
                            DataTable dt100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10017 = '" + cnp + "'", null);

                            if (dt100 != null && dt100.Rows.Count > 0)
                            {
                                pnlCtlDateIdent.JSProperties["cp_InfoMessage"] = Dami.TraduCuvant("Angajatul exista deja in baza de date.\nDoriti preluarea datelor de pe cel mai recent contract?");
                                //Page pagina = HttpContext.Current.Handler as Page;

                                //string mesaj = mesaj.Replace(System.Environment.NewLine, "\\");
                                //string txt = "swal({ " +
                                //            " title: 'Informare', " +
                                //            " text: 'CNP-ul exista deja. Doriti preluarea datelor ultimului contract activ?', " +
                                //            " type: 'info', " +
                                //            " showCancelButton: true, " +
                                //            " confirmButtonColor: '#DD6B55', " +
                                //            " confirmButtonText: 'Da!', " +
                                //            " cancelButtonText: 'Nu!', " +
                                //            " closeOnConfirm: false, " +
                                //            " closeOnCancel: false " +
                                //            " }, " +
                                //            " function(isConfirm) " +
                                //            " { " +
                                //            "     if (isConfirm) " +
                                //            "     { " +
                                //            "         OnConfirm(); " +
                                //            "     }" +
                                //            "     else " +
                                //            "     {" +
                                //            "         " +
                                //            "     }" +
                                //            " });";

                                //pagina.Page.ClientScript.RegisterStartupScript(pagina.GetType(), "MessageBox", txt);


                                //PreluareDateContract(cnp);
                            }

                        }      

                    }
                    else
                    {
                        pnlCtlDateIdent.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("CNP invalid");
                    }
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        private int SexDinCNP(char c)
        {
            // 0 - masculin
            // 1 - feminin

            try
            {
                int gender = -1;

                switch (c)
                {
                    case '1':
                    case '3':
                    case '5':
                        gender = 0;
                        break;

                    case '2':
                    case '4':
                    case '6':
                        gender = 1;
                        break;
                }

                return gender;
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
                return -1;
            }
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
                //img.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray(), 0, ms.ToArray().Length);


           
                //bitmap.Save(ms, ImageFormat.Gif);
                //var base64Data = Convert.ToBase64String(ms.ToArray());
                //img.Src = "data:image/jpeg;base64," + base64Data;


                string base64String = Convert.ToBase64String(btnDocUpload.UploadedFiles[0].FileBytes, 0, btnDocUpload.UploadedFiles[0].FileBytes.Length);
                img.Src = "data:image/jpg;base64," + base64String;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDoc_Click(object sender, EventArgs e)
        {
            try
            {
                //Session["Absente_Cereri_UploadedFile"] = null;
                //Session["Absente_Cereri_UploadedFileName"] = null;
                //Session["Absente_Cereri_UploadedFileExtension"] = null;
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
               // General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void DataNasterii(string cnp)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DateTime dtN = General.getDataNasterii(cnp);
                ASPxDateEdit deDataNasterii = DateIdentListView.Items[0].FindControl("deDataNasterii") as ASPxDateEdit;
                deDataNasterii.Value =dtN;
                //_dataNasterii = deDataNasterii.DateTime;
                ASPxTextBox txtVarsta = DateIdentListView.Items[0].FindControl("txtVarsta") as ASPxTextBox;
                txtVarsta.Text = Varsta(Convert.ToDateTime(deDataNasterii.Value)).ToString();

                ds.Tables[0].Rows[0]["F10021"] = new DateTime(dtN.Year, dtN.Month, dtN.Day);
                ds.Tables[1].Rows[0]["F10021"] = new DateTime(dtN.Year, dtN.Month, dtN.Day);
                Session["InformatiaCurentaPersonal"] = ds;

            }
            catch (Exception ex)
            {
               // Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public int Varsta(DateTime dataNasterii)
        {
            try
            {
                int years = -99;
                int months = -99;
                int days = -99;

                General.DateDiff(DateTime.Now, dataNasterii, out years, out months, out days);

                return years;
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
                return -99;
            }
        }

        private bool VerificaMarca(string marca)
        {
            DataTable dt = General.IncarcaDT("SELECT COUNT(*) FROM F100 WHERE F10003 = " + marca, null);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString() == "1")
                return false;
            else
                return true;
        }


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
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }


        public void PreluareDateContract(string CNP)
        {
            try
            {
                //DataTable dt100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10017 = '" + CNP + "' AND F10025 = 0 ORDER BY F10022 DESC", null);
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
                        ds.Tables[i].Rows[0]["F100903"] = dt100.Rows[0]["F100903"];
                        ds.Tables[i].Rows[0]["F100904"] = dt100.Rows[0]["F100904"];
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
                        }
                    }

                    ASPxTextBox txtNume = DateIdentListView.Items[0].FindControl("txtNume") as ASPxTextBox;
                    ASPxTextBox txtPreume = DateIdentListView.Items[0].FindControl("txtPrenume") as ASPxTextBox;
                    ASPxTextBox txtNumeAnt = DateIdentListView.Items[0].FindControl("txtNumeAnt") as ASPxTextBox;
                    ASPxDateEdit deDataModifNume = DateIdentListView.Items[0].FindControl("deDataModifNume") as ASPxDateEdit;
                    ASPxComboBox cmbStareCivila = DateIdentListView.Items[0].FindControl("cmbStareCivila") as ASPxComboBox;

                    txtNume.Text = dt100.Rows[0]["F10008"].ToString();
                    txtPreume.Text = dt100.Rows[0]["F10009"].ToString();
                    txtNumeAnt.Text = dt100.Rows[0]["F100905"].ToString();
                    if (dt100.Rows[0]["F100906"] != null && dt100.Rows[0]["F100906"].ToString().Length > 0)
                        deDataModifNume.Value = Convert.ToDateTime(dt100.Rows[0]["F100906"].ToString());
                    cmbStareCivila.Value = Convert.ToInt32(dt100.Rows[0]["F10046"].ToString());


                    Session["InformatiaCurentaPersonal"] = ds;
                    Session["PreluareDate"] = 1;
                }

            }
            catch (Exception ex)
            {
                //wiIndicator.DeferedVisibility = false;
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void ModifAvans(int atribut)
        {
            string url = "~/Avs/Cereri.aspx";
            Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Session["MP_Avans"] = "true";
            Session["MP_Avans_Tab"] = "DateIdentificare";
            if (Page.IsCallback)
                ASPxWebControl.RedirectOnCallback(url);
            else
                Response.Redirect(url, false);
        }

        //private void ArataMesaj(string mesaj)
        //{
        //    pnlCtlDateIdent.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtlDateIdent.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        //}







        //        <div >
        //    <dx:ASPxUploadControl ID = "btnDocUpload" runat="server" ClientIDMode="Static" Width="10px"
        //        BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document"
        //        ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
        //        <BrowseButtonStyle CssClass = "upload_btn" ></ BrowseButtonStyle >
        //        < TextBoxStyle CssClass="upload_txt" />
        //    </dx:ASPxUploadControl>
        //</div>

        //<div >
        //    <asp:Button ID = "btnDoc" runat="server" Text="" CssClass="doc_delete" OnClick="btnDoc_Click" ToolTip="sterge document" />
        //</div>



        				    //<dx:ASPxButton ID = "btnIncarca" ClientInstanceName="btnIncarca" ClientIDMode="Static" runat="server" Text="Incarca"  >
                //                <ClientSideEvents Click = "function(s,e){ OnClickDI(s); }" />
                //                < Image Url = "../Fisiere/Imagini/Icoane/incarca.png" />                              
                //            </dx:ASPxButton>
                //            <dx:ASPxButton ID = "btnSterge" ClientInstanceName="btnSterge" ClientIDMode="Static" runat="server" Text="Sterge"  >
                //                <ClientSideEvents Click = "function(s,e){ OnClickDI(s); }" />
                //                < Image Url = "../Fisiere/Imagini/Icoane/sterge.png" />                                
                //            </dx:ASPxButton>

                        //<div >
                        //    <img Height = "180" HorizontalAlignment="Center" ID="img1" runat="server" VerticalAlignment="Center" Width="180" />
                        //</div>


    }
}