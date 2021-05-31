using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class StiriDetaliu : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                if (!IsPostBack)
                {
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                    cmbLimbi.DataSource = Dami.ListaLimbi();
                    cmbLimbi.DataBind();
                    cmbLimbi.SelectedIndex = 0;

                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            txtDataInc.Date = DateTime.Today;
                            txtDataSf.Date = new DateTime(2100, 1, 1);
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Stiri"" WHERE ""Id""=@1 ", new string[] { Session["Sablon_CheiePrimara"].ToString() });
                                if (dt.Rows.Count > 0)
                                {
                                    if (Session["Sablon_TipActiune"].ToString() == "Edit") txtId.Text = dt.Rows[0]["Id"].ToString();
                                    txtDenumire.Text = dt.Rows[0]["Denumire"].ToString();
                                    if (dt.Rows[0]["DataInceput"].ToString() != "") txtDataInc.Date = Convert.ToDateTime(dt.Rows[0]["DataInceput"]);
                                    if (dt.Rows[0]["DataSfarsit"].ToString() != "") txtDataSf.Date = Convert.ToDateTime(dt.Rows[0]["DataSfarsit"]);
                                    chkActiv.Checked = Convert.ToBoolean(dt.Rows[0]["Activ"] ?? 1);
                                    txtContinut.Html = dt.Rows[0]["Continut"].ToString();
                                    cmbLimbi.Value = dt.Rows[0]["IdLimba"].ToString();
                                }
                            }
                            break;
                    }
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
                string strErr = "";
                if (txtDenumire.Text == "") strErr += ", denumire";
                if (txtDataInc.Text == "") strErr += ", data inceput";
                if (txtDataSf.Text == "") strErr += ", data sfarsit";
                if (txtContinut.Html == "") strErr += ", continut";

                if (strErr != "")
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    return;
                }


                int id = -99;
                if (Session["Sablon_TipActiune"].ToString() == "Edit") id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Stiri"" WHERE ""Id""=@1 ", new string[] { id.ToString() });

                DataRow dr = dt.NewRow();

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) FROM ""Stiri"" ", null) ?? 0) + 1;
                            dr["Id"] = id;
                            dr["TIME"] = DateTime.Now;
                            dr["USER_NO"] = Session["UserId"];
                        }
                        break;
                    case "Edit":
                        {
                            dr = dt.Rows[0];
                        }
                        break;
                }

                dr["Denumire"] = txtDenumire.Text;
                dr["DataInceput"] = txtDataInc.Date;
                dr["DataSfarsit"] = txtDataSf.Date;
                dr["Activ"] = chkActiv.Checked;
                dr["Continut"] = txtContinut.Html;
                dr["IdLimba"] = cmbLimbi.Value;

                if (Session["Sablon_TipActiune"].ToString() == "New" || Session["Sablon_TipActiune"].ToString() == "Clone") dt.Rows.Add(dr);

                string msg = Notif.TrimiteNotificare("tbl.Stiri", (int)Constante.TipNotificare.Validare, General.CreazaSelectFromRow(dr), "", id, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg == Constante.MesajeValidari.MesajDeEroare.ToString())
                {
                    return;
                }
                else
                {
                    General.SalveazaDate(dt, "Stiri");
                    Notif.TrimiteNotificare("tbl.Stiri", (int)Constante.TipNotificare.Notificare, @"SELECT Z.* FROM ""Stiri"" Z WHERE ""Id""=" + id, "Stiri", id, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                    if (msg == "") Iesire();
                }
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
                HttpContext.Current.Session["Sablon_Tabela"] = "Stiri";
                Response.Redirect("~/Pagini/SablonLista", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}