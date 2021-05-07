using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Adev
{
    public partial class AdeverintaAngajat : System.Web.UI.Page
    {
        //string fisier = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();



                if (!IsPostBack)
                {

                }
                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                //btnClose.Text = Dami.TraduCuvant("btnClose", "Inchidere luna");
                //btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare luna");

                #endregion


                //cmbAng.SelectedIndex = -1; 

                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));

                table.Rows.Add(0, "Medic");
                table.Rows.Add(1, "Grădiniță");
                table.Rows.Add(2, "Practică");
                table.Rows.Add(3, "Angajat");
                if (Convert.ToInt32(General.Nz(Session["IdClient"],"-99")) == Convert.ToInt32(IdClienti.Clienti.Harting))
                    table.Rows.Add(4, "Fluturaș");
                cmbAdev.DataSource = table;
                cmbAdev.DataBind();

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

               
                cmbAnul.DataSource = General.ListaNumere(2015, DateTime.Now.Year + 5);
                cmbAnul.DataBind();
                cmbLuna.DataSource = General.ListaLuniDesc();
                cmbLuna.DataBind();

            }
            catch (Exception ex)
            {
                ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }





        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                //string[] param = e.Parameter.Split(';');
                if (Convert.ToInt32(cmbAdev.Value ?? -1) == 4)
                {
                    lblAnul.Visible = true;
                    lblLuna.Visible = true;
                    cmbAnul.Visible = true;
                    cmbLuna.Visible = true;
                }
                else
                {
                    lblAnul.Visible = false;
                    lblLuna.Visible = false;
                    cmbAnul.Visible = false;
                    cmbLuna.Visible = false;
                }

            }
            catch (Exception ex)
            {
                ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string SelectAngajati(string filtru = "")
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                        FROM (
                        SELECT A.F10003
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        WHERE B.""IdUser""= {Session["UserId"]}) B                        
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607 {filtru}";

            }
            catch (Exception ex)
            {
                ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void btnGen_Click(object sender, EventArgs e)
        {
            btnGenerare_Click();
        }

        private void btnGenerare_Click()
        {
            try
            {
                if (cmbAng.Value == null)
                {
                    ArataMesaj(Dami.TraduCuvant("Campul angajat nu este completat!"));
                    return;
                }

                if (cmbAdev.Value == null)
                {
                    ArataMesaj(Dami.TraduCuvant("Nu ati selectat tipul de adeverinta!"));
                    return;
                }               


                switch (Convert.ToInt32(cmbAdev.Value))
                {
                    case 0:
                        Session["TactilAdeverinte"] = "Medic";
                        Session["PrintDocument"] = "AdeverintaMedic";
                        break;
                    case 1:
                        Session["TactilAdeverinte"] = "Gradinita";
                        Session["PrintDocument"] = "AdeverintaGenerala";
                        break;
                    case 2:
                        Session["TactilAdeverinte"] = "Practica";
                        Session["PrintDocument"] = "AdeverintaGenerala";
                        break;
                    case 3:
                        Session["TactilAdeverinte"] = "Angajat";
                        Session["PrintDocument"] = "AdeverintaGenerala";
                        break;
                    case 4:
                        Session["Fluturas_An"] = Convert.ToInt32(cmbAnul.Value ?? Dami.ValoareParam("AnLucru"));
                        Session["Fluturas_Luna"] = Convert.ToInt32(cmbLuna.Value ?? Dami.ValoareParam("LunaLucru"));
                        Session["PrintDocument"] = "FluturasHarting";                   
                        break;
                }

                Session["User_Marca_Tmp"] = Session["User_Marca"];
                Session["User_Marca"] = Convert.ToInt32(cmbAng.Value);

                if (Convert.ToInt32(cmbAdev.Value) < 4)
                {     
                    Session["NrInregAdev"] = General.GetnrInreg();
                }

                Session["PaginaWeb"] = "Adev/AdeverintaAngajat";
                Response.Redirect("~/Reports/Imprima", false);
                

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void ArataMesaj(string mesaj)
        {
            pnlCtl.Controls.Add(new LiteralControl());
            WebControl script = new WebControl(HtmlTextWriterTag.Script);
            pnlCtl.Controls.Add(script);
            script.Attributes["id"] = "dxss_123456";
            script.Attributes["type"] = "text/javascript";
            script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        }

    }
}