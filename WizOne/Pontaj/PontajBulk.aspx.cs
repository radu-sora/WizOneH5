using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajBulk : System.Web.UI.Page
    {

        string msgError = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                foreach (var col in grCC.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                Session["PaginaWeb"] = "Pontaj.PontajBulk";
                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (Session["User_Marca"] == null || Session["User_Marca"].ToString() == "-99" || Session["User_Marca"].ToString().Trim() == "")
                    grCC.Enabled = false;

                DataTable dtF062 = General.IncarcaDT("SELECT * FROM F062", null);
                GridViewDataComboBoxColumn colAbs = (grCC.Columns["F06204"] as GridViewDataComboBoxColumn);
                colAbs.PropertiesComboBox.DataSource = dtF062;

                DataTable dtAct = General.IncarcaDT(@"SELECT * FROM ""tblActivitati"" ", null);
                GridViewDataComboBoxColumn colAct = (grCC.Columns["IdActivitate"] as GridViewDataComboBoxColumn);
                colAct.PropertiesComboBox.DataSource = dtAct;

                //Radu 26.08.2020
                string sql = @"SELECT F10003, F10008 + ' ' + f10009 as ""Nume"" FROM F100 ";
                if (Constante.tipBD == 2) sql = @"SELECT F10003, F10008 || ' ' || f10009 as ""Nume"" FROM F100 ";
                DataTable dtAng = General.IncarcaDT(sql, null);
                GridViewDataComboBoxColumn colAng = (grCC.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                if (!IsPostBack)
                {
                    txtDataInc.Value = DateTime.Now;
                    txtDataSf.Value = DateTime.Now;

                    IncarcaGrid();
                }
                else
                {
                    grCC.DataSource = Session["InformatiaCurenta"];
                    grCC.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                if (e.NewValues["Ziua"] == null || e.NewValues["F06204"] == null || 
                    e.NewValues["IdActivitate"] == null || e.NewValues["NrOre1"] == null)
                {
                    msgError = "Campurile Zi, Contract/Proiect, Activitate si Nr Ore sunt obligatorii";
                    return;
                }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.NewRow();

                GridViewDataComboBoxColumn colAng = (grCC.Columns["F10003"] as GridViewDataComboBoxColumn);
                if (colAng.Visible == false)
                    dr["F10003"] = Session["User_Marca"];
                else
                    dr["F10003"] = e.NewValues["F10003"] ?? DBNull.Value;
                dr["Ziua"] = e.NewValues["Ziua"] ?? DBNull.Value;
                dr["F06204"] = e.NewValues["F06204"] ?? DBNull.Value;
                dr["IdActivitate"] = e.NewValues["IdActivitate"] ?? DBNull.Value;
                dr["NrOre1"] = e.NewValues["NrOre1"] ?? DBNull.Value;
                dr["NrOre10"] = e.NewValues["NrOre10"] ?? DBNull.Value;
                dr["Observatii"] = e.NewValues["Observatii"] ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                //Florin 2020.05.14 - am adaugat validarile
                string sqlPtj = General.CreazaSelectFromRow(dr);
                string msg = Notif.TrimiteNotificare("Pontaj.PontajBulk", (int)Constante.TipNotificare.Validare, sqlPtj, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    msgError = Dami.TraduCuvant(msg.Substring(2));
                    e.Cancel = false;
                }
                else
                {
                    dt.Rows.Add(dr);
                    //string txt = SalveazaDate(Convert.ToInt32(dr["F10003"] as int? ?? -99), Convert.ToDateTime(dr["Ziua"]));
                    string txt = General.SalveazaDate(dt, "Ptj_CC");
                    if (txt != "")
                    {
                        msgError = txt;
                        e.Cancel = false;
                    }
                    else
                    {
                        if (msg != "")
                            grCC.JSProperties["cpAlertMessage"] = msg;

                        e.Cancel = true;
                        grCC.CancelEdit();
                        //Session["InformatiaCurenta"] = dt;
                        //grCC.DataSource = dt;

                        IncarcaGrid();
                    }  
                }
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);


                if (e.NewValues["Ziua"] == null || e.NewValues["F06204"] == null ||
                    e.NewValues["IdActivitate"] == null || e.NewValues["NrOre1"] == null)
                {
                    msgError = "Campurile Zi, Contract/Proiect, Activitate si Nr Ore sunt obligatorii";
                    return;
                }

                GridViewDataComboBoxColumn colAng = (grCC.Columns["F10003"] as GridViewDataComboBoxColumn);
                if (colAng.Visible == false)
                    dr["F10003"] = Session["User_Marca"];
                else
                    dr["F10003"] = e.NewValues["F10003"] ?? DBNull.Value;
                dr["Ziua"] = e.NewValues["Ziua"] ?? DBNull.Value;
                dr["F06204"] = e.NewValues["F06204"] ?? DBNull.Value;
                dr["IdActivitate"] = e.NewValues["IdActivitate"] ?? DBNull.Value;
                dr["NrOre1"] = e.NewValues["NrOre1"] ?? DBNull.Value;
                dr["NrOre10"] = e.NewValues["NrOre10"] ?? DBNull.Value;
                dr["Observatii"] = e.NewValues["Observatii"] ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                //Florin 2020.05.14 - am adaugat validarile
                string sqlPtj = General.CreazaSelectFromRow(dr);
                string msg = Notif.TrimiteNotificare("Pontaj.PontajBulk", (int)Constante.TipNotificare.Validare, sqlPtj, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    msgError = Dami.TraduCuvant(msg.Substring(2));
                    e.Cancel = false;
                }
                else
                {
                    //string txt = SalveazaDate(Convert.ToInt32(dr["F10003"] as int? ?? -99), Convert.ToDateTime(dr["Ziua"]));
                    string txt = General.SalveazaDate(dt, "Ptj_CC");
                    if (txt != "")
                    {
                        msgError = txt;
                        e.Cancel = false;
                    }
                    else
                    {
                        if (msg != "")
                            grCC.JSProperties["cpAlertMessage"] = msg;

                        e.Cancel = true;
                        grCC.CancelEdit();
                        //Session["InformatiaCurenta"] = dt;
                        //grCC.DataSource = dt;

                        IncarcaGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["F10003"] = Session["User_Marca"];
                e.NewValues["Ziua"] = DateTime.Now;

                if (Dami.ValoareParam("PontajCCcuAprobare", "0") == "0")
                    e.NewValues["IdStare"] = 3;
                else
                    e.NewValues["IdStare"] = 1;

                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);
                int f10003 = Convert.ToInt32(dr["F10003"] as int? ?? -99);
                DateTime ziua = Convert.ToDateTime(dr["Ziua"]);

                //Florin 2020.05.14 - am adaugat validarile
                string sqlPtj = General.CreazaSelectFromRow(dr);
                string msg = Notif.TrimiteNotificare("Pontaj.PontajBulk", (int)Constante.TipNotificare.Validare, sqlPtj, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    msgError = Dami.TraduCuvant(msg.Substring(2));
                    e.Cancel = false;
                }
                else
                {
                    dr.Delete();
                    //SalveazaDate(f10003, ziua);
                    General.SalveazaDate(dt, "Ptj_CC");
                    if (msg != "")
                        grCC.JSProperties["cpAlertMessage"] = msg;

                    e.Cancel = true;
                    grCC.CancelEdit();
                    Session["InformatiaCurenta"] = dt;
                    grCC.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected string SalveazaDate(int f10003, DateTime ziua)
        {
            string txt = "";

            try
            {
                //salvam datele in tabela de centrii de cost
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                txt = General.SalveazaDate(dt, "Ptj_CC");

                if (txt != "")
                {
                    return txt;
                }

                //initializam linii in Pontaj
                bool ras = General.PontajInit(Convert.ToInt32(Session["UserId"]), ziua.Year, ziua.Month, -99, true , false, "", Convert.ToInt32(Session["User_Marca"] ?? -99), -99, -99, -99, "", false, false, false, 0);
                

                if (Dami.ValoareParam("PontajCCCalculTotalPeZi", "0") == "1")
                {
                    string cmp = "";
                    //DataTable dtAdm = Session["Ptj_tblAdminCC"] as DataTable;
                    DataTable dtAdm = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAdminCC"" ", null);

                    for (int i = 0; i < dtAdm.Rows.Count; i++)
                    {
                        DataRow dr = dtAdm.Rows[i];
                        if (General.Nz(dr["Destinatie"], "").ToString() != "" && dr["Camp"].ToString().Substring(0, 5) == "NrOre")
                        {
                            cmp += ", " + dr["Destinatie"] + "=(SELECT SUM(\"" + dr["Camp"] + "\") * 60 FROM \"Ptj_CC\" WHERE F10003=@1 AND \"Ziua\"=@2)";
                        }
                    }

                    if (cmp != "")
                    {
                        //transferam suma minutelor din CC in Ptj_Intrari
                        string sqlVal = $@"UPDATE ""Ptj_Intrari"" SET {cmp.Substring(1)} WHERE F10003=@1 AND ""Ziua""=@2;";
                        sqlVal = sqlVal.Replace("@1", f10003.ToString()).Replace("@2", General.ToDataUniv(ziua));

                        //refacem ValStr
                        string sqlTot = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr"" ={General.CalculValStr(f10003, ziua, "", "", 0)}
                                        WHERE  F10003={f10003.ToString()} AND ""Ziua"" ={General.ToDataUniv(ziua)};";


                        General.ExecutaNonQuery("BEGIN" + "\n\r" +
                                                sqlVal + "\n\r" +
                                                sqlTot + "\n\r" +
                                                "END;"
                                                , null);

                        General.CalculFormuleCumulat($@"ent.F10003={f10003} AND ent.""An""={ziua.Year} AND ent.""Luna""={ziua.Month}");
                    }
                }
            }
            catch (Exception ex)
            {
                txt = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return txt;
        }

        private void IncarcaGrid()
        {
            try
            {
                string filtru = " AND F10003 = " + Session["User_Marca"];

                //Radu 27.07.2020 - se doreste ca utilizatorii cu rol HR sa poata introduce inregistrari pentru toti angajatii
                bool esteHR = false;
                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                string sqlHr = $@"SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                DataTable dtHr = General.IncarcaDT(sqlHr, null);
                if (dtHr != null && dtHr.Rows.Count > 0) esteHR = true;

                if (General.Nz(Session["EsteAdmin"], "").ToString() == "1" || esteHR)
                {
                    filtru = "";
                    //Radu 26.08.2020
                    GridViewDataComboBoxColumn colAng = (grCC.Columns["F10003"] as GridViewDataComboBoxColumn);
                    colAng.Visible = true;
                    colAng.ShowInCustomizationForm = true;
                }
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_CC"" WHERE {General.ToDataUniv(Convert.ToDateTime(txtDataInc.Value))} <= ""Ziua"" AND ""Ziua"" <= {General.ToDataUniv(Convert.ToDateTime(txtDataSf.Value))} {filtru}", null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                Session["InformatiaCurenta"] = dt;

                grCC.DataSource = Session["InformatiaCurenta"];
                grCC.KeyFieldName = "IdAuto";
                grCC.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
        {
            try
            {
                e.ErrorText = msgError;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Ziua" || e.Column.FieldName == "F06204") e.Editor.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}