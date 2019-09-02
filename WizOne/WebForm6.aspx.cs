using DevExpress.Web;
using DevExpress.Web.Data;
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

namespace WizOne
{
    public partial class WebForm6 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncarcaCC();
        }

        private void IncarcaCC()
        {
            try
            {
                List<object> lst = ValoriChei();

                //lblZiuaCC.Text = "Centrii de cost - Ziua " + Convert.ToDateTime(lst[1]).Day;
                DataTable dt = SursaCC(Convert.ToInt32(lst[0]), General.ToDataUniv(Convert.ToDateTime(lst[1])));
                Session["PtjCC"] = dt;
                grCC.KeyFieldName = "F10003;Ziua;F06204";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };

                grCC.DataSource = dt;
                grCC.DataBind();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private DataTable SursaCC(int f10003, string ziua)
        {
            DataTable dt = new DataTable();

            try
            {
                string strCmp = "";
                for (int i = 1; i <= 10; i++)
                {
                    if (Constante.tipBD == 1)
                        strCmp += $@",CONVERT(datetime,DATEADD(minute, NrOre{i}, '')) AS NrOre{i}_Tmp ";
                    else
                        strCmp += $@",TO_DATE('01-01-1900','DD-MM-YYYY') + NrOre{i}/1440 AS ""NrOre{i}_Tmp"" ";
                }

                //dt = General.IncarcaDT($@"SELECT A.F10003, A.""Ziua"", 
                //        COALESCE(A.F06204,-99) AS F06204, A.""IdProiect"",A.""IdSubProiect"", A.""IdActivitate"", A.""IdDept"", A.""De"", A.""La"", 
                //        CONVERT(nvarchar(10),NrOre1 / 60) + ':' + CONVERT(nvarchar(10),NrOre1 % 60) AS ""NrOre1"",A.""NrOre2"",A.""NrOre3"",A.""NrOre4"",A.""NrOre5"",A.""NrOre6"",A.""NrOre7"",A.""NrOre8"",A.""NrOre9"",A.""NrOre10"",A.""IdStare"", 
                //        A.USER_NO, A.TIME, A.""IdAuto""
                //        FROM ""Ptj_CC"" A 
                //        WHERE A.F10003={f10003} AND A.""Ziua""={ziua}", null);

                dt = General.IncarcaDT($@"SELECT * {strCmp}
                        FROM ""Ptj_CC"" A 
                        WHERE A.F10003={f10003} AND A.""Ziua""={ziua}", null);

                //dt = General.IncarcaDT($@"SELECT B.F10003, B.""Ziua"", B.""Norma"",
                //        COALESCE(A.F06204,-99) AS F06204, A.""IdProiect"",A.""IdSubProiect"", A.""IdActivitate"", A.""IdDept"", A.""De"", A.""La"", 
                //        A.""NrOre1"",A.""NrOre2"",A.""NrOre3"",A.""NrOre4"",A.""NrOre5"",A.""NrOre6"",A.""NrOre7"",A.""NrOre8"",A.""NrOre9"",A.""NrOre10"",A.""IdStare"", 
                //        A.USER_NO, A.TIME, A.""IdAuto"",
                //        CASE WHEN C.""DenumireScurta"" IS NULL THEN 0 ELSE 1 END AS ""EsteAbsenta"" {strCmp} 
                //        FROM ""Ptj_Intrari"" B 
                //        LEFT JOIN ""Ptj_CC"" A ON A.F10003=B.F10003 AND A.""Ziua""=B.""Ziua""
                //        LEFT JOIN ""Ptj_tblAbsente"" C ON B.""ValStr""=C.""DenumireScurta""
                //        WHERE B.F10003={f10003} AND B.""Ziua""={ziua}", null);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        private List<object> ValoriChei()
        {
            List<object> lst = new List<object>();

            lst.Add(1331);
            lst.Add(new DateTime(2019,8,1));

            try
            {
                //if (!ccValori.Contains("cheia")) return lst;


                //int tip = Convert.ToInt32(General.Nz(Request["tip"], 1));
                //if (tip == 1 || tip == 10)
                //{
                //    DateTime dtTmp = Convert.ToDateTime(txtAnLuna.Value);
                //    lst[1] = new DateTime(dtTmp.Year, dtTmp.Month, Convert.ToInt32(ccValori["cheia"]), 0, 0, 0);
                //}
                //else
                //{
                //    lst[0] = Convert.ToInt32(ccValori["cheia"]);
                //}


                //var cmp = grDate.GetSelectedFieldValues(new string[] { "Cheia" });

                //if (cmp != null && cmp.Count > 0)
                //{
                //    var ert = cmp[0];

                //    if (tip == 1 || tip == 10)
                //    {
                //        lst[0] = cmbAng.Value;
                //        lst[1] = new DateTime(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, Convert.ToInt32(cmp[0]));
                //    }
                //    else
                //    {
                //        lst[0] = cmp[0];
                //        lst[1] = txtZiua.Value;
                //    }
                //}
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lst;
        }

        protected void grCC_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {

                //List<object> lst = ValoriChei();

                //string str = e.Parameters;
                //if (str != "")
                //{
                //    string[] arr = e.Parameters.Split(';');
                //    if (arr.Length == 0 || arr[0] == "")
                //    {
                //        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                //        return;
                //    }

                //    switch (arr[0])
                //    {
                //        case "btnCC":
                //            {
                //                lblZiuaCC.Text = "Centrii de cost - Ziua " + Convert.ToDateTime(lst[1]).Day;
                //                DataTable dt = SursaCC(Convert.ToInt32(lst[0]), General.ToDataUniv(Convert.ToDateTime(lst[1])));
                //                Session["PtjCC"] = dt;
                //                grCC.KeyFieldName = "F10003;Ziua;F06204";
                //                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };

                //                grCC.DataSource = dt;
                //                grCC.DataBind();
                //            }
                //            break;
                //        case "btnDeleteCC":
                //            {
                //                DataTable dt = Session["PtjCC"] as DataTable;
                //                object[] arrKey = arr[1].Split('|');
                //                DataRow found = dt.Rows.Find(arrKey);
                //                found.Delete();

                //                btnSaveCC_Click(null, null);

                //                Session["PtjCC"] = dt;
                //                grCC.KeyFieldName = "F10003;Ziua;F06204";
                //                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };
                //                grCC.DataSource = dt;
                //                grCC.DataBind();
                //            }
                //            break;
                //        case "cmbPro":
                //            {
                //                ASPxComboBox cmbSubPro = ((ASPxComboBox)grCC.FindEditRowCellTemplateControl(grCC.Columns["IdSubproiect"] as GridViewDataComboBoxColumn, "cmbSubPro"));
                //                if (cmbSubPro == null) return;
                //                cmbSubPro.SelectedIndex = -1;

                //                if (arr.Count() > 1 && arr[1] != null && arr[1].ToString() != "")
                //                {
                //                    DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" A
                //                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                //                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                //                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                //                                        WHERE A.""IdProiect""={arr[1]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
                //                                        SELECT * FROM ""tblSubProiecte""
                //                                        ELSE
                //                                        SELECT DISTINCT D.* FROM ""Ptj_relAngajatProiect"" A
                //                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                //                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                //                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                //                                        WHERE A.""IdProiect""={arr[1]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

                //                    cmbSubPro.DataSource = dt;
                //                    cmbSubPro.DataBind();
                //                    Session["PtjCC_SubProiecte"] = dt;
                //                }
                //                else
                //                {
                //                    cmbSubPro.DataSource = null;
                //                    cmbSubPro.DataBind();
                //                    Session["PtjCC_SubProiecte"] = null;
                //                }
                //            }
                //            break;
                //        case "cmbSubPro":
                //            {
                //                ASPxComboBox cmbAct = ((ASPxComboBox)grCC.FindEditRowCellTemplateControl(grCC.Columns["IdActivitate"] as GridViewDataComboBoxColumn, "cmbAct"));
                //                if (cmbAct == null) return;
                //                cmbAct.SelectedIndex = -1;

                //                if (arr.Count() > 2 && arr[1] != null && arr[1].ToString() != "" && arr[2] != null && arr[2].ToString() != "")
                //                {
                //                    DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" A
                //                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                //                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                //                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                //                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                //                                        WHERE A.""IdProiect""={arr[1]} AND C.""IdSubproiect""={arr[2]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
                //                                        SELECT * FROM ""tblSubProiecte""
                //                                        ELSE
                //                                        SELECT DISTINCT E.* FROM ""Ptj_relAngajatProiect"" A
                //                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                //                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                //                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                //                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                //                                        WHERE A.""IdProiect""={arr[1]} AND C.""IdSubproiect""={arr[2]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

                //                    cmbAct.DataSource = dt;
                //                    cmbAct.DataBind();
                //                    Session["PtjCC_Activitati"] = dt;
                //                }
                //                else
                //                {
                //                    cmbAct.DataSource = null;
                //                    cmbAct.DataBind();
                //                    Session["PtjCC_Activitati"] = null;
                //                }
                //            }
                //            break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grCC.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = General.Nz(e.GetValue("IdStare"), "").ToString();
                    if (idStare != "")
                    {
                        DataRow[] lst = dt.Select("Id=" + idStare);
                        if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                        {
                            e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
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

        protected void grCC_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                //bool suntModif = false;
                //List<object> lst = ValoriChei();
                //grDate.CancelEdit();
                //DataTable dt = Session["PtjCC"] as DataTable;
                //if (dt == null) return;

                ////daca avem linii noi
                //for (int i = 0; i < e.InsertValues.Count; i++)
                //{
                //    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;
                //    DataRow dr = dt.NewRow();

                //    dr["F10003"] = lst[0];
                //    dr["Ziua"] = Convert.ToDateTime(lst[1]);
                //    dr["F06204"] = upd.NewValues["F06204"] ?? 9999;
                //    dr["IdProiect"] = upd.NewValues["IdProiect"] ?? DBNull.Value;
                //    dr["IdSubproiect"] = upd.NewValues["IdSubproiect"] ?? DBNull.Value;
                //    dr["IdActivitate"] = upd.NewValues["IdActivitate"] ?? DBNull.Value;
                //    dr["IdDept"] = upd.NewValues["IdDept"] ?? DBNull.Value;
                //    dr["De"] = upd.NewValues["De"] ?? DBNull.Value;
                //    dr["La"] = upd.NewValues["La"] ?? DBNull.Value;
                //    dr["NrOre1"] = upd.NewValues["NrOre1"] ?? DBNull.Value;
                //    dr["NrOre2"] = upd.NewValues["NrOre2"] ?? DBNull.Value;
                //    dr["NrOre3"] = upd.NewValues["NrOre3"] ?? DBNull.Value;
                //    dr["NrOre4"] = upd.NewValues["NrOre4"] ?? DBNull.Value;
                //    dr["NrOre5"] = upd.NewValues["NrOre5"] ?? DBNull.Value;
                //    dr["NrOre6"] = upd.NewValues["NrOre6"] ?? DBNull.Value;
                //    dr["NrOre7"] = upd.NewValues["NrOre7"] ?? DBNull.Value;
                //    dr["NrOre8"] = upd.NewValues["NrOre8"] ?? DBNull.Value;
                //    dr["NrOre9"] = upd.NewValues["NrOre9"] ?? DBNull.Value;
                //    dr["NrOre10"] = upd.NewValues["NrOre10"] ?? DBNull.Value;


                //    //for (int x = 1; x <= 10; x++)
                //    //{
                //    //    if (upd.NewValues["NrOre" + i + "_Tmp"] != null)
                //    //        dr["NrOre" + x] = Convert.ToDateTime(upd.NewValues["NrOre" + i + "_Tmp"]).Minute + (Convert.ToDateTime(upd.NewValues["NrOre" + i + "_Tmp"]).Hour * 60);
                //    //    else
                //    //        dr["NrOre" + x] = DBNull.Value;
                //    //}


                //    if (Dami.ValoareParam("PontajCCcuAprobare", "0") == "0")
                //        dr["IdStare"] = 3;
                //    else
                //        dr["IdStare"] = upd.NewValues["IdStare"] ?? DBNull.Value;

                //    dr["USER_NO"] = Session["UserId"];
                //    dr["TIME"] = DateTime.Now;

                //    dt.Rows.Add(dr);

                //    suntModif = true;
                //}


                ////daca avem linii modificate
                //for (int i = 0; i < e.UpdateValues.Count; i++)
                //{
                //    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                //    object[] keys = new object[upd.Keys.Count];
                //    for (int x = 0; x < upd.Keys.Count; x++)
                //    { keys[x] = upd.Keys[x]; }

                //    DataRow dr = dt.Rows.Find(keys);
                //    if (dr == null) continue;

                //    if (upd.NewValues["F06204"] != null) dr["F06204"] = upd.NewValues["F06204"] ?? 9999;
                //    if (upd.NewValues["IdProiect"] != null) dr["IdProiect"] = upd.NewValues["IdProiect"] ?? DBNull.Value;
                //    if (upd.NewValues["IdSubproiect"] != null) dr["IdSubproiect"] = upd.NewValues["IdSubproiect"] ?? DBNull.Value;
                //    if (upd.NewValues["IdActivitate"] != null) dr["IdActivitate"] = upd.NewValues["IdActivitate"] ?? DBNull.Value;
                //    if (upd.NewValues["IdDept"] != null) dr["IdDept"] = upd.NewValues["IdDept"] ?? DBNull.Value;
                //    if (upd.NewValues["De"] != null) dr["De"] = upd.NewValues["De"] ?? DBNull.Value;
                //    if (upd.NewValues["La"] != null) dr["La"] = upd.NewValues["La"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre1"] != null) dr["NrOre1"] = upd.NewValues["NrOre1"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre2"] != null) dr["NrOre2"] = upd.NewValues["NrOre2"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre3"] != null) dr["NrOre3"] = upd.NewValues["NrOre3"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre4"] != null) dr["NrOre4"] = upd.NewValues["NrOre4"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre5"] != null) dr["NrOre5"] = upd.NewValues["NrOre5"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre6"] != null) dr["NrOre6"] = upd.NewValues["NrOre6"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre7"] != null) dr["NrOre7"] = upd.NewValues["NrOre7"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre8"] != null) dr["NrOre8"] = upd.NewValues["NrOre8"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre9"] != null) dr["NrOre9"] = upd.NewValues["NrOre9"] ?? DBNull.Value;
                //    if (upd.NewValues["NrOre10"] != null) dr["NrOre10"] = upd.NewValues["NrOre10"] ?? DBNull.Value;

                //    //for (int x = 1; x <= 10; x++)
                //    //{
                //    //    if (upd.NewValues["NrOre" + i + "_Tmp"] != null)
                //    //    {
                //    //        if (upd.NewValues["NrOre" + i + "_Tmp"] != null)
                //    //            dr["NrOre" + x] = Convert.ToDateTime(upd.NewValues["NrOre" + i + "_Tmp"]).Minute + (Convert.ToDateTime(upd.NewValues["NrOre" + i + "_Tmp"]).Hour * 60);
                //    //        else
                //    //            dr["NrOre" + x] = DBNull.Value;
                //    //    }
                //    //}

                //    if (upd.NewValues["IdStare"] != null) dr["IdStare"] = upd.NewValues["IdStare"] ?? DBNull.Value;
                //    dr["USER_NO"] = Session["UserId"];
                //    dr["TIME"] = DateTime.Now;

                //    suntModif = true;
                //}


                ////daca avem linii modificate
                //for (int i = 0; i < e.DeleteValues.Count; i++)
                //{
                //    ASPxDataDeleteValues upd = e.DeleteValues[i] as ASPxDataDeleteValues;

                //    object[] keys = new object[upd.Keys.Count];
                //    for (int x = 0; x < upd.Keys.Count; x++)
                //    { keys[x] = upd.Keys[x]; }

                //    DataRow dr = dt.Rows.Find(keys);
                //    if (dr == null) continue;

                //    dt.Rows.Remove(dr);

                //    grCC.DataSource = dt;
                //    grCC.DataBind();

                //    suntModif = true;
                //}

                //if (suntModif == true)
                //{
                //    bool faraErori = true;
                //    DataTable dtInt = General.IncarcaDT($@"SELECT COALESCE(B.""Norma"",8) AS ""Norma"", CASE WHEN C.""DenumireScurta"" IS NULL THEN 0 ELSE 1 END AS ""EsteAbsenta"" 
                //        FROM ""Ptj_Intrari"" B
                //        LEFT JOIN ""Ptj_tblAbsente"" C ON B.""ValStr""=C.""DenumireScurta""
                //        WHERE B.F10003={lst[0]} AND B.""Ziua""={General.ToDataUniv(Convert.ToDateTime(lst[1]))}", null);
                //    if (dtInt.Rows.Count > 0)
                //    {
                //        int total = Convert.ToInt32(General.Nz(dt.Compute("Sum(NrOre1)", string.Empty), 0));
                //        if (total > Convert.ToInt32(General.Nz(dtInt.Rows[0]["Norma"], 8)))
                //        {
                //            grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Suma orelor depaseste norma");
                //            faraErori = false;
                //        }

                //        if (Convert.ToInt32(General.Nz(dtInt.Rows[0]["EsteAbsenta"], 0)) == 1 && Dami.ValoareParam("PontajCCCalculTotalPeZi") == "1")
                //        {
                //            grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate salva deoarece exista absenta de tip zi");
                //            faraErori = false;
                //        }
                //    }

                //    //if (dt.Rows.Count > 0)
                //    //{
                //    //    int total = Convert.ToInt32(General.Nz(dt.Compute("Sum(NrOre1)", string.Empty), 0));
                //    //    if (total > Convert.ToInt32(General.Nz(dt.Rows[0]["Norma"],0)))
                //    //    {
                //    //        grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Suma orelor depaseste norma");
                //    //    }
                //    //    else
                //    //        btnSaveCC_Click(null, null);
                //    //}
                //    //else
                //    //    btnSaveCC_Click(null, null);

                //    if (faraErori)
                //        btnSaveCC_Click(null, null);
                //}

                //e.Handled = true;

                //DataTable dtCC = SursaCC(Convert.ToInt32(lst[0]), General.ToDataUniv(Convert.ToDateTime(lst[1])));

                //Session["PtjCC"] = dtCC;
                //grCC.KeyFieldName = "F10003;Ziua;F06204";
                //dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };
                //grCC.DataSource = dtCC;
                //grCC.DataBind();

            }
            catch (Exception ex)
            {
                ////MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ex.Message);
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                //e.Handled = true;


                //if (ex.Message.IndexOf("is constrained to be unique") >= 0)
                //{
                //    e.Handled = true;
                //    grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Adaugati valori duplicate in baza de date");
                //}
            }
        }

        protected void grCC_ParseValue(object sender, ASPxParseValueEventArgs e)
        {
            if (e.FieldName == "NrOre1")
                e.Value = TimeSpanFromString(e.Value);
        }

        protected void grCC_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            if (e.Column.FieldName == "NrOre1")
            {
                e.Editor.Value = StringFromTimeSpan(e.Value);
                DataTable dt = grCC.DataSource as DataTable;

            }
        }

        protected void grCC_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
        {
            //var ert = e.ErrorText;
            //var edc = e.Exception;

        }

        private Int32 TimeSpanFromString(Object value)
        {
            if (value == null || String.IsNullOrEmpty((String)value))
                return 0;

            return int.Parse((String)value);
        }

        private String StringFromTimeSpan(Object value)
        {
            if (value == null)
                return String.Empty;

            int nrOre = (int)value;

            return (nrOre / 60).ToString() + ":" + (nrOre % 60).ToString();

            //String str = nrOre.ToString(@"hh\:mm");

            //if (nrOre < 0)
            //    return String.Format("-0.{0}", str);
            //else
            //    return String.Format("0.{0}", str);


        }

    }
}