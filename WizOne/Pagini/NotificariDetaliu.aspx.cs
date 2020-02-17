using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class NotificariDetaliu : System.Web.UI.Page
    {

        //int tip = 1;                //1-new; 2-edit; 3-clone
        //int id = -99;
        //int idCl = -99;
        //string cmp = "USER_NO,TIME,IDAUTO,";

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                Dami.AccesApp();
                Dami.AccesAdmin();



                #region Traducere

                #endregion

                switch (Session["TipNotificare"].ToString())
                {
                    case "1":
                        pnlMail.Visible = true;
                        pnlMesaj.Visible = false;
                        pnlAtasament.Visible = true;
                        pnlXLS.Visible = true;
                        break;
                    case "2":
                        pnlMail.Visible = false;
                        pnlMesaj.Visible = true;
                        pnlAtasament.Visible = false;
                        pnlXLS.Visible = false;
                        break;
                    case "3":
                        pnlMail.Visible = true;
                        pnlMesaj.Visible = false;
                        pnlAtasament.Visible = false;
                        pnlXLS.Visible = true;
                        break;
                }

                if (!IsPostBack)
                {
                    string tbl = "";
                    if (Constante.tipBD == 2) tbl = " FROM DUAL";
                    //DataTable dtCmp = General.IncarcaDT($@"SELECT * FROM (
                    //                    SELECT 0 AS ""Ordine"", ""Alias"" AS ""Denumire"", ""IdAuto"" as ""Id"", ""TipData"" FROM ""Ntf_tblCampuri"" WHERE ""Pagina"" = @1
                    //                    UNION
                    //                    SELECT -2, 'Link Aproba',-78, 'Link' {tbl}
                    //                    UNION
                    //                    SELECT -1, 'Link Respinge', -77, 'Link' {tbl} ) X ORDER BY ""Ordine"", ""Denumire"" ", new string[] { Dami.PaginaWeb() });

                    DataTable dtCmp = General.IncarcaDT($@"SELECT * FROM (
                                        SELECT 0 AS ""Ordine"", ""Alias"" AS ""Denumire"", ""IdAuto"" as ""Id"", ""TipData"" FROM ""Ntf_tblCampuri"" WHERE ""Pagina"" = @1
                                        UNION
                                        SELECT -2, 'Link1 Aproba',-78, 'Link' {tbl}
                                        UNION
                                        SELECT -1, 'Link2 Respinge', -77, 'Link' {tbl} ) X ORDER BY ""Ordine"", ""Denumire"" ", new string[] { General.Nz(Session["PaginaWeb"], "").ToString().Replace("\\", ".") });

                    cmbAddCmp.DataSource = dtCmp;
                    cmbAddCmp.DataBindItems();

                    //cmbCol.DataSource = dtCmp;
                    //cmbCol.DataBindItems();

                    //cmbVal.DataSource = dtCmp;
                    //cmbVal.DataBindItems();

                    //cmbVal2.DataSource = dtCmp;
                    //cmbVal2.DataBindItems();

                    int id = Convert.ToInt32(Request["id"] ?? "-99");

                    DataTable dtMail = new DataTable();
                    DataTable dtCond = new DataTable();
                    if (Request["tip"].ToString() == "Clone")
                    {
                        if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3") dtMail = General.IncarcaDT(@"SELECT * FROM ""Ntf_Mailuri"" WHERE ""Id""=@1 ", new string[] { "-99" });
                        dtCond = General.IncarcaDT(@"SELECT * FROM ""Ntf_Conditii"" WHERE ""Id""=@1 ", new string[] { "-99" });
                    }
                    else
                    {
                        if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3") dtMail = General.IncarcaDT(@"SELECT * FROM ""Ntf_Mailuri"" WHERE ""Id""=@1 ", new string[] { id.ToString() });
                        dtCond = General.IncarcaDT(@"SELECT * FROM ""Ntf_Conditii"" WHERE ""Id""=@1 ", new string[] { id.ToString() });
                    }


                    switch (Request["tip"].ToString())
                    {
                        case "New":
                            //NOP
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                //incarcam header-ul
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Ntf_Setari"" WHERE ""Id""=@1 ", new string[] { id.ToString() });
                                if (dtHead.Rows.Count > 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["Id"].ToString();
                                    txtDenumire.Text = (dtHead.Rows[0]["Denumire"]).ToString();
                                    chkActiv.Checked = Convert.ToBoolean(General.Nz(dtHead.Rows[0]["Activ"], 0));
                                    txtSubiect.Text = (dtHead.Rows[0]["Subiect"] ?? "").ToString();
                                    txtContinut.Html = (dtHead.Rows[0]["ContinutMail"]  ?? "").ToString();
                                    cmbValid.SelectedIndex = Convert.ToInt32(General.Nz(dtHead.Rows[0]["ValidTip"], 0));
                                    //cmbValidValoare.SelectedItem.Value = Convert.ToInt32(General.Nz(dtHead.Rows[0]["ValidVal"], -99));
                                    cmbValid_SelectedIndexChanged(sender, e);
                                    if (Convert.ToInt32(General.Nz(dtHead.Rows[0]["ValidVal"], -99)) == -99)
                                        cmbValidValoare.SelectedIndex = -1;
                                    else
                                        cmbValidValoare.Value = Convert.ToInt32(General.Nz(dtHead.Rows[0]["ValidVal"], -99));
                                    txtNume.Text = (dtHead.Rows[0]["NumeAtasament"] ?? "").ToString();   //.Replace("&lt;","<").Replace("&gt;", ">");
                                    chkDisc.Checked = Convert.ToBoolean(General.Nz(dtHead.Rows[0]["SalveazaInDisc"], 0));
                                    chkBaza.Checked = Convert.ToBoolean(General.Nz(dtHead.Rows[0]["SalveazaInBaza"], 0));
                                    chkTrimite.Checked = Convert.ToBoolean(General.Nz(dtHead.Rows[0]["TrimitePeMail"], 0));
                                    txtAtt.Html = (dtHead.Rows[0]["ContinutAtasament"] ?? "").ToString();
                                    cmbMesaj.Value = (dtHead.Rows[0]["Mesaj"] ?? "").ToString();
                                    chkExcel.Checked = Convert.ToBoolean(General.Nz(dtHead.Rows[0]["TrimiteXLS"], 0));
                                    txtExcel.Value = (dtHead.Rows[0]["SelectXLS"] ?? "").ToString();
                                    txtNumeExcel.Text = (dtHead.Rows[0]["NumeExcel"] ?? "").ToString();
                                }


                                if (Request["tip"].ToString() == "Clone")
                                {
                                    if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3")
                                    {
                                        //incarcam mailurile
                                        DataTable dtOri = General.IncarcaDT(@"SELECT * FROM ""Ntf_Mailuri"" WHERE ""Id""=@1", new string[] { id.ToString() });
                                        foreach (DataRow dr in dtOri.Rows)
                                        {
                                            DataRow drDes = dtMail.NewRow();
                                            drDes["Id"] = -99;
                                            drDes["MailTip"] = dr["MailTip"];
                                            drDes["MailAdresaId"] = dr["MailAdresaId"];
                                            drDes["MailAdresaText"] = dr["MailAdresaText"];
                                            drDes["MailDestinatie"] = dr["MailDestinatie"];
                                            drDes["IncludeLinkAprobare"] = dr["IncludeLinkAprobare"];
                                            drDes["USER_NO"] = Session["UserId"];
                                            drDes["TIME"] = DateTime.Now;
                                            dtMail.Rows.Add(drDes);
                                        }
                                    }

                                    //incarcam conditiile
                                    DataTable dtOriCond = General.IncarcaDT(@"SELECT * FROM ""Ntf_Conditii"" WHERE ""Id""=@1", new string[] { id.ToString() });
                                    foreach (DataRow dr in dtOriCond.Rows)
                                    {
                                        DataRow drDes = dtCond.NewRow();
                                        drDes["Id"] = -99;
                                        drDes["Coloana"] = dr["Coloana"];
                                        drDes["Operator"] = dr["Operator"];
                                        drDes["Valoare1"] = dr["Valoare1"];
                                        drDes["Valoare2"] = dr["Valoare2"];
                                        drDes["NrZile1"] = dr["NrZile1"];
                                        drDes["NrZile2"] = dr["NrZile2"];
                                        drDes["IdAuto"] = Convert.ToInt32(General.Nz(dtCond.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                        drDes["USER_NO"] = Session["UserId"];
                                        drDes["TIME"] = DateTime.Now;
                                        dtCond.Rows.Add(drDes);
                                    }
                                }


                            }
                            break;
                    }

                    if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3")
                    {
                        //incarcam mailurile
                        dtMail.PrimaryKey = new DataColumn[] { dtMail.Columns["Id"], dtMail.Columns["MailTip"], dtMail.Columns["MailAdresaId"] };
                        Session["Ntf_Mailuri"] = dtMail;
                        grDateMail.KeyFieldName = "Id;MailTip;MailAdresaId";
                        grDateMail.DataSource = dtMail;
                        grDateMail.DataBind();
                    }

                    //incarcam conditiile
                    dtCond.PrimaryKey = new DataColumn[] { dtCond.Columns["IdAuto"] };
                    Session["Ntf_Conditii"] = dtCond;
                    grDateCond.KeyFieldName = "IdAuto";
                    grDateCond.DataSource = dtCond;
                    grDateCond.DataBind();

                }
                else
                {
                    if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3")
                    {
                        grDateMail.DataSource = Session["Ntf_Mailuri"];
                        grDateMail.DataBind();
                    }
                    grDateCond.DataSource = Session["Ntf_Conditii"];
                    grDateCond.DataBind();
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

                int id = Convert.ToInt32(Request["id"]);

                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Ntf_Setari"" WHERE ""Id""=@1 ", new string[] { id.ToString() });
                DataTable dtMail = Session["Ntf_Mailuri"] as DataTable;
                DataTable dtCond = Session["Ntf_Conditii"] as DataTable;

                switch (Request["tip"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) FROM ""Ntf_Setari"" ", null) ?? 0) + 1;
                            DataRow drHead = dtHead.NewRow();
                            drHead["Id"] = id;
                            drHead["Denumire"] = txtDenumire.Text;

                            string pag = Session["PaginaWeb"].ToString().Replace("\\", ".");
                            if (Session["PaginaWeb"].ToString().ToLower().IndexOf("sablon") >= 0) pag = "tbl." + Session["Sablon_Tabela"].ToString();
                            drHead["Pagina"] = pag;

                            drHead["TipNotificare"] = Convert.ToInt32(Session["TipNotificare"] ?? 1);
                            drHead["Activ"] = chkActiv.Checked;
                            drHead["Subiect"] = txtSubiect.Text;

                            //Florin 2019.10.17 - daca este validare sau alerta ne trebuie plain text, pt notificare ne trebuie Html
                            if (General.Nz(drHead["TipNotificare"],1).ToString() == "1")
                                drHead["ContinutMail"] = txtContinut.Html;
                            else
                            {
                                using (MemoryStream mStream = new MemoryStream())
                                {
                                    txtContinut.Export(HtmlEditorExportFormat.Txt, mStream);
                                    string plainText = System.Text.Encoding.UTF8.GetString(mStream.ToArray());
                                    drHead["ContinutMail"] = plainText;
                                }
                            }

                            if (cmbMesaj.SelectedItem == null)
                                drHead["Mesaj"] = DBNull.Value;
                            else
                                drHead["Mesaj"] = cmbMesaj.SelectedItem.Value;
                            drHead["ValidTip"] = cmbValid.SelectedIndex;
                            if (cmbValidValoare.SelectedItem == null)
                                drHead["ValidVal"] = DBNull.Value;
                            else
                                drHead["ValidVal"] = cmbValidValoare.SelectedItem.Value;
                            drHead["NumeAtasament"] = txtNume.Text;    //.Replace("<", "&lt;").Replace(">", "&gt;");
                            drHead["SalveazaInDisc"] = chkDisc.Checked;
                            drHead["SalveazaInBaza"] = chkBaza.Checked;
                            drHead["TrimitePeMail"] = chkTrimite.Checked;
                            drHead["ContinutAtasament"] = txtAtt.Html;
                            drHead["TrimiteXLS"] = chkExcel.Checked;
                            drHead["SelectXLS"] = txtExcel.Text;
                            drHead["NumeExcel"] = txtNumeExcel.Text;
                            drHead["USER_NO"] = Session["UserId"];
                            drHead["TIME"] = DateTime.Now;
                            dtHead.Rows.Add(drHead);

                            if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3")
                            {
                                foreach (DataRow dr in dtMail.Rows)
                                {
                                    dr["Id"] = id;
                                }
                            }

                            foreach (DataRow dr in dtCond.Rows)
                            {
                                dr["Id"] = id;
                            }
                        }
                        break;
                    case "Edit":
                        {
                            dtHead.Rows[0]["Denumire"] = txtDenumire.Text;
                            dtHead.Rows[0]["Activ"] = chkActiv.Checked;
                            dtHead.Rows[0]["Subiect"] = txtSubiect.Text;

                            //Florin 2019.10.17 - daca este validare sau alerta ne trebuie plain text, pt notificare ne trebuie Html
                            if (General.Nz(dtHead.Rows[0]["TipNotificare"], 1).ToString() == "1")
                                dtHead.Rows[0]["ContinutMail"] = txtContinut.Html;
                            else
                            {
                                using (MemoryStream mStream = new MemoryStream())
                                {
                                    txtContinut.Export(HtmlEditorExportFormat.Txt, mStream);
                                    string plainText = System.Text.Encoding.UTF8.GetString(mStream.ToArray());
                                    dtHead.Rows[0]["ContinutMail"] = plainText;
                                }
                            }

                            if (cmbMesaj.SelectedItem == null)
                                dtHead.Rows[0]["Mesaj"] = DBNull.Value;
                            else
                                dtHead.Rows[0]["Mesaj"] = cmbMesaj.SelectedItem.Value;
                            dtHead.Rows[0]["ValidTip"] = cmbValid.SelectedIndex;
                            if (cmbValidValoare.SelectedItem == null)
                                dtHead.Rows[0]["ValidVal"] = DBNull.Value;
                            else
                                dtHead.Rows[0]["ValidVal"] = cmbValidValoare.SelectedItem.Value;
                            //dtHead.Rows[0]["ValidVal"] = cmbValidValoare.SelectedItem;
                            dtHead.Rows[0]["NumeAtasament"] = txtNume.Text;    //.Replace("<","&lt;").Replace(">","&gt;");
                            dtHead.Rows[0]["SalveazaInDisc"] = chkDisc.Checked;
                            dtHead.Rows[0]["SalveazaInBaza"] = chkBaza.Checked;
                            dtHead.Rows[0]["TrimitePeMail"] = chkTrimite.Checked;
                            dtHead.Rows[0]["ContinutAtasament"] = txtAtt.Html;
                            dtHead.Rows[0]["TrimiteXLS"] = chkExcel.Checked;
                            dtHead.Rows[0]["SelectXLS"] = txtExcel.Text;
                            dtHead.Rows[0]["NumeExcel"] = txtNumeExcel.Text;
                            dtHead.Rows[0]["USER_NO"] = Session["UserId"];
                            dtHead.Rows[0]["TIME"] = DateTime.Now;
                        }
                        break;
                }


                //SqlDataAdapter daHead = new SqlDataAdapter();
                //daHead.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""Ntf_Setari"" ", null);
                //SqlCommandBuilder cbHead = new SqlCommandBuilder(daHead);
                //daHead.Update(dtHead);

                //daHead.Dispose();
                //daHead = null;
                General.SalveazaDate(dtHead, "Ntf_Setari");


                if (Session["TipNotificare"].ToString() == "1" || Session["TipNotificare"].ToString() == "3")
                {
                    //SqlDataAdapter da = new SqlDataAdapter();
                    //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""Ntf_Mailuri"" ", null);
                    //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    //da.Update(dtMail);

                    //da.Dispose();
                    //da = null;
                    General.SalveazaDate(dtMail, "Ntf_Mailuri");
                }



                //SqlDataAdapter daCond = new SqlDataAdapter();
                //daCond.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""Ntf_Conditii"" ", null);
                //SqlCommandBuilder cbCond = new SqlCommandBuilder(daCond);
                //daCond.Update(dtCond);

                //daCond.Dispose();
                //daCond = null;

                General.SalveazaDate(dtCond, "Ntf_Conditii");


                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY2", "window.parent.popGen.Hide();", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY2", "window.parent.popGen.SetContentUrl('../Pagini/Notificari.aspx?tip=" + Session["TipNotificare"].ToString() + "'); popGen.SetHeaderText('Notificari');", true);
                //Response.Redirect("~/Pagini/MeniuLista.aspx", false);
                //MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnSaveMail_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var boolLipsa = false;

        //        if (cmbTip.SelectedIndex == -1 || cmbDes.SelectedIndex == -1) boolLipsa = true;
        //        if (cmbTip.SelectedIndex != -1)
        //        {
        //            if (cmbTip.SelectedIndex == 0 && txtAdr.Text == "") boolLipsa = true;
        //            if (cmbTip.SelectedIndex != 0 && cmbAdr.SelectedIndex == -1) boolLipsa = true;
        //        }

        //        if (boolLipsa)
        //        {
        //            MessageBox.Show(Dami.TraduCuvant("Lipsesc date"), MessageBox.icoWarning);
        //            return;
        //        }

        //        if (txtAdr.Text != "" && !General.IsValidEmail(txtAdr.Text))
        //        {
        //            MessageBox.Show(Dami.TraduCuvant("Adresa de mail nu este valida"), MessageBox.icoWarning);
        //            return;
        //        }

        //        DataTable dt = Session["Ntf_Mailuri"] as DataTable;
        //        DataRow dr = dt.NewRow();
        //        dr["Id"] = Request["id"] ?? "-99";
        //        dr["MailTip"] = cmbTip.SelectedItem.ToString();
        //        if (cmbTip.SelectedIndex == 0)
        //        {
        //            dr["MailAdresaId"] = txtAdr.Text;
        //            dr["MailAdresaText"] = txtAdr.Text;
        //        }
        //        else
        //        {
        //            dr["MailAdresaId"] = cmbAdr.SelectedItem.Value;
        //            dr["MailAdresaText"] = cmbAdr.SelectedItem.Text;
        //        }
        //        dr["MailDestinatie"] = cmbDes.SelectedItem.ToString();
        //        dr["IncludeLinkAprobare"] = chkLink.Checked;
        //        dr["USER_NO"] = Session["UserId"];
        //        dr["TIME"] = DateTime.Now;
        //        dt.Rows.Add(dr);

        //        grDateMail.DataSource = Session["Ntf_Mailuri"];
        //        grDateMail.DataBind();

        //        txtAdr.Text = "";
        //        cmbAdr.SelectedIndex = -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void btnSaveCond_Click(object sender, EventArgs e)
        {
            try
            {
                //var strMsg = "";

                //if (cmbCol.Text == "") strMsg += ", camp";
                //if (cmbCond.Text == "") strMsg += ", operator";
                //if (txtVal.Text == "") strMsg += ", valoare";

                //if (strMsg != "")
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strMsg.Substring(1), MessageBox.icoWarning);
                //    return;
                //}

                //DataTable dt = Session["Ntf_Conditii"] as DataTable;
                //DataRow dr = dt.NewRow();
                //dr["Id"] = Request["id"] ?? "-99";
                //dr["Coloana"] = cmbCol.Text;
                //dr["Operator"] = cmbCond.Text;
                //dr["Valoare1"] = txtVal.Text;
                //if (txtVal2.Text != "") dr["Valoare2"] = txtVal2.Text;
                //if (txtZile.Text != "") dr["NrZile1"] = txtZile.Text;
                //if (txtZile2.Text != "") dr["NrZile2"] = txtZile2.Text;
                //dr["USER_NO"] = Session["UserId"];
                //dr["TIME"] = DateTime.Now;
                ////var ert = dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto"));
                //dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                //dt.Rows.Add(dr);

                //grDateCond.DataSource = Session["Ntf_Conditii"];
                //grDateCond.DataBind();

                //cmbCol.SelectedIndex = -1;
                //cmbCond.SelectedIndex = -1;
                //txtVal.Text = "";
                //cmbVal.SelectedIndex = -1;
                //cmbValData.SelectedIndex = -1;
                //txtVal2.Text = "";
                //cmbVal2.SelectedIndex = -1;
                //cmbValData2.SelectedIndex = -1;
                //txtZile.Text = "";
                //txtZile2.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbTip_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ASPxComboBox cmbAdr = grDateMail.FindEditFormTemplateControl("cmbAdr") as ASPxComboBox;
                ASPxComboBox cmbTip = grDateMail.FindEditFormTemplateControl("cmbTip") as ASPxComboBox;
                ASPxTextBox txtAdr = grDateMail.FindEditFormTemplateControl("txtAdr") as ASPxTextBox;

                if (cmbAdr == null || cmbTip == null || txtAdr == null) return;

                txtAdr.Text = "";
                cmbAdr.Items.Clear();
                cmbAdr.DataSource = null;
                cmbAdr.SelectedIndex = -1;

                if (cmbTip.SelectedIndex == 0)
                {
                    txtAdr.Visible = true;
                    cmbAdr.Visible = false;
                }
                else
                {
                    txtAdr.Visible = false;
                    cmbAdr.Visible = true;

                    DataTable dt = new DataTable();
                    string strSql = "";

                    switch (cmbTip.SelectedIndex)
                    {
                        case 1:
                            strSql = @"SELECT F70102 AS ""Id"", COALESCE(""NumeComplet"", F70104) AS ""Denumire"" FROM USERS";
                            break;
                        case 2:
                            strSql = @"SELECT F10003 AS ""Id"", F10008 + ' ' + F10009 AS ""Denumire"" FROM F100";
                            if (Constante.tipBD == 2) strSql = @"SELECT F10003 AS ""Id"", F10008 || ' ' || F10009 AS ""Denumire"" FROM F100";
                            break;
                        case 3:
                            strSql = @"SELECT ""Id"", ""Denumire"" FROM ""tblGrupUsers""";
                            break;
                        case 4:
                            strSql = @"SELECT ""Id"", ""Denumire"" FROM ""tblGrupAngajati""";
                            break;
                        case 5:
                            {
                                string pag = Session["PaginaWeb"].ToString().Replace("\\", ".");
                                if (Session["PaginaWeb"].ToString().ToLower().IndexOf("sablon") >= 0) pag = "tbl." + Session["Sablon_Tabela"].ToString();

                                //string idFrm = Request["IdForm"] ?? "-";
                                //idFrm = idFrm.Replace(".aspx", "");
                                //if (idFrm.ToLower().IndexOf("sablon") >= 0)
                                //    idFrm = "tbl." + Session["NomenTableName"];

                                strSql = @"SELECT ""IdAuto"" AS ""Id"", ""Alias"" AS ""Denumire"" FROM ""Ntf_tblCampuri"" WHERE ""EsteMail""=1 AND ""Pagina""=@1";
                                dt = General.IncarcaDT(strSql, new string[] { pag });
                                cmbAdr.DataSource = dt;
                                cmbAdr.DataBindItems();
                            }
                            return;
                    }

                    if (strSql != "")
                        dt = General.IncarcaDT(strSql, null);

                    cmbAdr.DataSource = dt;
                    cmbAdr.DataBindItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMail_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Ntf_Mailuri"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDateMail.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbValid_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbValidValoare.SelectedIndex = -1;

                switch (cmbValid.SelectedIndex)
                {
                    case 0:
                        cmbValidValoare.Enabled = false;
                        cmbValidValoare.DataSource = null;
                        break;
                    case 1:
                        {
                            cmbValidValoare.Enabled = true;
                            DataTable dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""tblGrupUsers"" ", null);
                            cmbValidValoare.DataSource = dt;
                            cmbValidValoare.DataBindItems();
                        }
                        break;
                    case 2:
                        {
                            cmbValidValoare.Enabled = true;
                            DataTable dt = General.IncarcaDT(@"SELECT F70102 AS ""Id"", F70104 AS ""Denumire"" FROM ""USERS"" ", null);
                            cmbValidValoare.DataSource = dt;
                            cmbValidValoare.DataBindItems();
                        }
                        break;
                    default:
                        cmbValidValoare.Enabled = false;
                        cmbValidValoare.DataSource = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void cmbCol_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

        protected void grDateCond_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Ntf_Conditii"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDateCond.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void cmbAddCmp_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        txtContinut.Html += "<<" + cmbAddCmp.Text + ">>";
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        #region Cannot unregister UpdatePanel with ID 'up2' since it was not registered with the ScriptManager. This might occur if the UpdatePanel was removed from the control tree and later added again, which is not supported.

        protected void up1_Unload(object sender, EventArgs e)
        {
            try
            {
                RegisterUpdatePanel((UpdatePanel)sender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void up4_Unload(object sender, EventArgs e)
        {
            try
            {
                RegisterUpdatePanel((UpdatePanel)sender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void up2_Unload(object sender, EventArgs e)
        {
            try
            {
                RegisterUpdatePanel((UpdatePanel)sender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void RegisterUpdatePanel(UpdatePanel panel)
        {
            try
            {
                var sType = typeof(ScriptManager);
                var mInfo = sType.GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", BindingFlags.NonPublic | BindingFlags.Instance);
                if (mInfo != null)
                    mInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        #endregion

        protected void grDateMail_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Ntf_Mailuri"] as DataTable;
                DataRow row = dt.Rows.Find(keys);


                ASPxComboBox cmbTip = grDateMail.FindEditFormTemplateControl("cmbTip") as ASPxComboBox;
                if (cmbTip != null)
                {
                    if (cmbTip.SelectedIndex != -1 && cmbTip.Value != null)
                        row["MailTip"] = cmbTip.Value;

                    if (General.Nz(cmbTip.Value, "").ToString() == "mail")
                    {
                        ASPxTextBox txtAdr = grDateMail.FindEditFormTemplateControl("txtAdr") as ASPxTextBox;
                        if (txtAdr != null && txtAdr.Value != null)
                        {
                            row["MailAdresaId"] = txtAdr.Text;
                            row["MailAdresaText"] = txtAdr.Text;
                        }
                    }
                    else
                    {
                        ASPxComboBox cmbAdr = grDateMail.FindEditFormTemplateControl("cmbAdr") as ASPxComboBox;
                        if (cmbAdr != null && cmbAdr.Value != null)
                        {
                            row["MailAdresaId"] = cmbAdr.Value;
                            row["MailAdresaText"] = cmbAdr.Text;
                        }
                    }
                }

                ASPxComboBox cmbDes = grDateMail.FindEditFormTemplateControl("cmbDes") as ASPxComboBox;
                if (cmbDes != null && cmbDes.SelectedIndex != -1 && cmbDes.Value != null)
                    row["MailDestinatie"] = cmbDes.Value;

                ASPxCheckBox chkLink = grDateMail.FindEditFormTemplateControl("chkLink") as ASPxCheckBox;
                if (chkLink != null)
                    row["IncludeLinkAprobare"] = chkLink.Checked;

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDateMail.CancelEdit();
                Session["Ntf_Mailuri"] = dt;
                grDateMail.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCond_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Ntf_Conditii"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                //Florin 2018.09.24
                //am scos selectedIndex != -1


                //ASPxComboBox cmbCol = grDateCond.FindEditFormTemplateControl("cmbCol") as ASPxComboBox;
                //if (cmbCol != null && cmbCol.SelectedIndex != -1 && cmbCol.Value != null)
                //    dr["Coloana"] = cmbCol.Text;

                //ASPxComboBox cmbCond = grDateCond.FindEditFormTemplateControl("cmbCond") as ASPxComboBox;
                //if (cmbCond != null && cmbCond.SelectedIndex != -1 && cmbCond.Value != null)
                //    dr["Operator"] = cmbCond.Value;

                ASPxComboBox cmbCol = grDateCond.FindEditFormTemplateControl("cmbCol") as ASPxComboBox;
                if (cmbCol != null && cmbCol.Value != null)
                    dr["Coloana"] = cmbCol.Text;

                ASPxComboBox cmbCond = grDateCond.FindEditFormTemplateControl("cmbCond") as ASPxComboBox;
                if (cmbCond != null && cmbCond.Value != null)
                    dr["Operator"] = cmbCond.Value;



                ASPxTextBox txtVal = grDateCond.FindEditFormTemplateControl("txtVal") as ASPxTextBox;
                if (txtVal != null && txtVal.Text != "")
                    dr["Valoare1"] = txtVal.Text;

                ASPxSpinEdit txtZile = grDateCond.FindEditFormTemplateControl("txtZile") as ASPxSpinEdit;
                if (txtZile != null && txtZile.Value != null)
                    dr["NrZile1"] = txtZile.Value;

                ASPxTextBox txtVal2 = grDateCond.FindEditFormTemplateControl("txtVal2") as ASPxTextBox;
                if (txtVal2 != null && txtVal2.Text != "")
                    dr["Valoare2"] = txtVal2.Text;

                ASPxSpinEdit txtZile2 = grDateCond.FindEditFormTemplateControl("txtZile2") as ASPxSpinEdit;
                if (txtZile2 != null && txtZile2.Value != null)
                    dr["NrZile2"] = txtZile2.Value;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDateCond.CancelEdit();
                Session["Ntf_Conditii"] = dt;
                grDateCond.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMail_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;

                switch (str)
                {
                    case "cmbTip":
                        cmbTip_SelectedIndexChanged(null, null);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMail_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["Id"] = General.Nz(txtId.ID,-99);
                e.NewValues["MailTip"] = "mail";
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];

                grDateMail.FocusedRowIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMail_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Ntf_Mailuri"] as DataTable;
                object[] row = new object[dt.Columns.Count];
                DataRow dr = dt.NewRow();
                dr["Id"] = General.Nz(txtId.Text, -99);

                ASPxComboBox cmbTip = grDateMail.FindEditFormTemplateControl("cmbTip") as ASPxComboBox;
                if (cmbTip != null)
                {
                    if (cmbTip.SelectedIndex != -1 && cmbTip.Value != null)
                        dr["MailTip"] = cmbTip.Value;

                    if (General.Nz(cmbTip.Value, "").ToString() == "mail")
                    {
                        ASPxTextBox txtAdr = grDateMail.FindEditFormTemplateControl("txtAdr") as ASPxTextBox;
                        if (txtAdr != null && txtAdr.Value != null)
                        {
                            dr["MailAdresaId"] = txtAdr.Text;
                            dr["MailAdresaText"] = txtAdr.Text;
                        }
                    }
                    else
                    {
                        ASPxComboBox cmbAdr = grDateMail.FindEditFormTemplateControl("cmbAdr") as ASPxComboBox;
                        if (cmbAdr != null && cmbAdr.Value != null)
                        {
                            dr["MailAdresaId"] = cmbAdr.Value;
                            dr["MailAdresaText"] = cmbAdr.Text;
                        }
                    }
                }

                ASPxComboBox cmbDes = grDateMail.FindEditFormTemplateControl("cmbDes") as ASPxComboBox;
                if (cmbDes != null && cmbDes.SelectedIndex != -1 && cmbDes.Value != null)
                    dr["MailDestinatie"] = cmbDes.Value;

                ASPxCheckBox chkLink = grDateMail.FindEditFormTemplateControl("chkLink") as ASPxCheckBox;
                if (chkLink != null)
                    dr["IncludeLinkAprobare"] = chkLink.Checked;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDateMail.CancelEdit();
                Session["Ntf_Mailuri"] = dt;
                grDateMail.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCond_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                //string str = e.Parameters;

                //switch (str)
                //{
                //    case "cmbTip":
                //        cmbTip_SelectedIndexChanged(null, null);
                //        break;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCond_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["Id"] = General.Nz(txtId.ID, -99);
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];

                grDateCond.FocusedRowIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCond_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Ntf_Conditii"] as DataTable;
                object[] row = new object[dt.Columns.Count];
                DataRow dr = dt.NewRow();
                dr["Id"] = General.Nz(txtId.Text, -99);

                //Florin 2018.09.24
                //am scos selectedIndex != -1

                //ASPxComboBox cmbCol = grDateCond.FindEditFormTemplateControl("cmbCol") as ASPxComboBox;
                //if (cmbCol != null && cmbCol.SelectedIndex != -1 && cmbCol.Value != null)
                //    dr["Coloana"] = cmbCol.Text;

                //ASPxComboBox cmbCond = grDateCond.FindEditFormTemplateControl("cmbCond") as ASPxComboBox;
                //if (cmbCond != null && cmbCond.SelectedIndex != -1 && cmbCond.Value != null)
                //    dr["Operator"] = cmbCond.Value;

                ASPxComboBox cmbCol = grDateCond.FindEditFormTemplateControl("cmbCol") as ASPxComboBox;
                if (cmbCol != null && cmbCol.Value != null)
                    dr["Coloana"] = cmbCol.Text;

                ASPxComboBox cmbCond = grDateCond.FindEditFormTemplateControl("cmbCond") as ASPxComboBox;
                if (cmbCond != null && cmbCond.Value != null)
                    dr["Operator"] = cmbCond.Value;

                ASPxTextBox txtVal = grDateCond.FindEditFormTemplateControl("txtVal") as ASPxTextBox;
                if (txtVal != null && txtVal.Text != "")
                    dr["Valoare1"] = txtVal.Text;

                ASPxSpinEdit txtZile = grDateCond.FindEditFormTemplateControl("txtZile") as ASPxSpinEdit;
                if (txtZile != null && txtZile.Value != null)
                    dr["NrZile1"] = txtZile.Value;

                ASPxTextBox txtVal2 = grDateCond.FindEditFormTemplateControl("txtVal2") as ASPxTextBox;
                if (txtVal2 != null && txtVal2.Text != "")
                    dr["Valoare2"] = txtVal2.Text;

                ASPxSpinEdit txtZile2 = grDateCond.FindEditFormTemplateControl("txtZile2") as ASPxSpinEdit;
                if (txtZile2 != null && txtZile2.Value != null)
                    dr["NrZile2"] = txtZile2.Value;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.Compute("MAX(IdAuto)", "[IdAuto] IS NOT NULL"), 0)) + 1;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDateCond.CancelEdit();
                Session["Ntf_Conditii"] = dt;
                grDateCond.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMail_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                object[] obj = grDateMail.GetRowValues(grDateMail.FocusedRowIndex, new string[] { "Id", "MailTip", "MailAdresaId", "MailAdresaText", "MailDestinatie", "IncludeLinkAprobare" }) as object[];

                ASPxComboBox cmbTip = grDateMail.FindEditFormTemplateControl("cmbTip") as ASPxComboBox;
                if (cmbTip != null)
                {
                    cmbTip.Value = obj[1];
                    cmbTip_SelectedIndexChanged(null, null);
                }

                ASPxComboBox cmbAdr = grDateMail.FindEditFormTemplateControl("cmbAdr") as ASPxComboBox;
                if (cmbAdr != null)
                {
                    cmbAdr.Value = obj[2];
                    cmbAdr.Text = General.Nz(obj[3], "").ToString();
                }

                ASPxTextBox txtAdr = grDateMail.FindEditFormTemplateControl("txtAdr") as ASPxTextBox;
                if (txtAdr != null)
                    txtAdr.Value = obj[3];

                ASPxComboBox cmbDes = grDateMail.FindEditFormTemplateControl("cmbDes") as ASPxComboBox;
                if (cmbDes != null)
                    cmbDes.Value = obj[4];

                ASPxCheckBox chkLink = grDateMail.FindEditFormTemplateControl("chkLink") as ASPxCheckBox;
                if (chkLink != null)
                    chkLink.Value = obj[5];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCond_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            try
            {
                string tbl = "";
                if (Constante.tipBD == 2) tbl = " FROM DUAL";

                DataTable dtCmp = General.IncarcaDT($@"SELECT * FROM (
                                        SELECT 0 AS ""Ordine"", ""Alias"" AS ""Denumire"", ""IdAuto"" as ""Id"", ""TipData"" FROM ""Ntf_tblCampuri"" WHERE ""Pagina"" = @1
                                        UNION
                                        SELECT -2, 'Link1 Aproba',-78, 'Link' {tbl}
                                        UNION
                                        SELECT -1, 'Link2 Respinge', -77, 'Link' {tbl} ) X ORDER BY ""Ordine"", ""Denumire"" ", new string[] { General.Nz(Session["PaginaWeb"], "").ToString().Replace("\\", ".") });

                object[] obj = grDateCond.GetRowValues(grDateCond.FocusedRowIndex, new string[] { "Id", "Coloana", "Operator", "Valoare1", "Valoare2", "NrZile1", "NrZile2" }) as object[];

                ASPxComboBox cmbCol = grDateCond.FindEditFormTemplateControl("cmbCol") as ASPxComboBox;
                if (cmbCol != null)
                {
                    cmbCol.DataSource = dtCmp;
                    cmbCol.DataBindItems();
                    cmbCol.Text = General.Nz(obj[1], "").ToString();

                    DataRow[] arr = dtCmp.Select("Denumire='" + cmbCol.Text + "'");
                    if (arr.Count() > 0)
                        cmbCol.Value = Convert.ToInt32(General.Nz(arr[0]["Id"], -1));

                    //this.ClientScript.RegisterStartupScript(this.GetType(), "cmbCol_SelChg", "cmbCol_SelectedIndexChanged_Client();");
                }

                ASPxComboBox cmbVal = grDateCond.FindEditFormTemplateControl("cmbVal") as ASPxComboBox;
                if (cmbVal != null)
                {
                    cmbVal.DataSource = dtCmp;
                    cmbVal.DataBind();
                    cmbVal.Text = General.Nz(obj[3], "").ToString();

                    DataRow[] arr = dtCmp.Select("Denumire='" + cmbVal.Text + "'");
                    if (arr.Count() > 0)
                        cmbVal.Value = Convert.ToInt32(General.Nz(arr[0]["Id"], -1));
                }

                ASPxComboBox cmbVal2 = grDateCond.FindEditFormTemplateControl("cmbVal2") as ASPxComboBox;
                if (cmbVal2 != null)
                {
                    cmbVal2.DataSource = dtCmp;
                    cmbVal2.DataBind();
                    cmbVal2.Text = General.Nz(obj[4], "").ToString();

                    DataRow[] arr = dtCmp.Select("Denumire='" + cmbVal2.Text + "'");
                    if (arr.Count() > 0)
                        cmbVal2.Value = Convert.ToInt32(General.Nz(arr[0]["Id"], -1));
                }

                ASPxComboBox cmbCond = grDateCond.FindEditFormTemplateControl("cmbCond") as ASPxComboBox;
                if (cmbCond != null)
                    cmbCond.Value = obj[2];

                ASPxTextBox txtVal = grDateCond.FindEditFormTemplateControl("txtVal") as ASPxTextBox;
                if (txtVal != null)
                    txtVal.Value = obj[3];

                ASPxTextBox txtVal2 = grDateCond.FindEditFormTemplateControl("txtVal2") as ASPxTextBox;
                if (txtVal2 != null)
                    txtVal2.Value = obj[4];

                ASPxSpinEdit txtZile = grDateCond.FindEditFormTemplateControl("txtZile") as ASPxSpinEdit;
                if (txtZile != null)
                    txtZile.Value = obj[5];

                ASPxSpinEdit txtZile2 = grDateCond.FindEditFormTemplateControl("txtZile2") as ASPxSpinEdit;
                if (txtZile2 != null)
                    txtZile2.Value = obj[6];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



    }
}