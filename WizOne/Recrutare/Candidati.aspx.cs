using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Recrutare
{
    public partial class Candidati : System.Web.UI.Page
    {

 
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                #endregion

                if (!IsPostBack)
                {
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                    string[] tbls = { "Rec_Candidati", "Rec_Experienta"};
                    DataSet ds = new DataSet();
                    for(int i = 0; i < tbls.Length; i++)
                    {
                        DataTable dt = General.IncarcaDT($@"SELECT * FROM ""{tbls[i]}"" WHERE ""Id""=@1", new object[] { Session["Sablon_CheiePrimara"] });
                        dt.TableName = tbls[i];
                        ds.Tables.Add(dt);
                    }
                    
                    Session["InformatiaCurenta"] = ds;
                }

                if (General.Nz(Session["Rec_NumeTab"], "").ToString() == "")
                    Session["Rec_NumeTab"] = "tabGen";

                LoadPanel(Session["Rec_NumeTab"].ToString());
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
                //ne asiguram ca datele de pe forma curenta au fost salvate
                SavePanel();

                if (General.Nz(Session["InformatiaCurenta"], "").ToString() == "")
                {
                    MessageBox.Show("Eroare la salvare", MessageBox.icoError, "");
                    return;
                }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Eroare la salvare", MessageBox.icoError, "");
                    return;
                }

                DataRow dr = dt.Rows[0];

                string strErr = "";

                if (General.Nz(dr["Nume"],"").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("nume");
                if (General.Nz(dr["Prenume"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("prenume");
                if (General.Nz(dr["DataNastere"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("data nastere");
                if (General.Nz(dr["CNP"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("cnp");
                if (General.Nz(dr["Judet"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("judet");
                if (General.Nz(dr["Localitate"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("localitate");
                if (General.Nz(dr["AdresaCompleta"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("adresa");
                if (General.Nz(dr["Mail"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("mail");
                if (General.Nz(dr["Telefon"], "").ToString().Trim() == "") strErr += ", " + Dami.TraduCuvant("telefon");

                if (strErr != "")
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    return;
                }

                //Notif.TrimiteNotificare("tbl.Stiri", (int)Constante.TipNotificare.Notificare, @"SELECT * FROM ""Stiri"" WHERE ""Id""=" + id, "Stiri", id, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                General.SalveazaDate(dt, "Rec_Candidati");
                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void Iesire()
        {
            try
            {
                HttpContext.Current.Session["Sablon_Tabela"] = "Rec_Candidati";
                Response.Redirect("~/Pagini/SablonLista", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCall_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                LoadPanel(e.Parameter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void LoadPanel(string numeTab)
        {
            try
            {
                SavePanel();

                pnlGeneral.Controls.Clear();
                Control ctrl = new Control();

                switch (numeTab)
                {
                    case "tabGen":
                        ctrl = this.LoadControl("DateGenerale.ascx");
                        break;
                    case "tabExp":
                        ctrl = this.LoadControl("Experienta.ascx");
                        break;
                }

                if (ctrl != null)
                    pnlGeneral.Controls.Add(ctrl);

                Session["Rec_NumeTab"] = numeTab;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SavePanel()
        {
            try
            {
                if (pnlGeneral.Controls.Count > 0)
                {
                    dynamic pag = pnlGeneral.Controls[0];
                    if (pag != null)
                        pag.btnUpdate_Click(null, null);
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