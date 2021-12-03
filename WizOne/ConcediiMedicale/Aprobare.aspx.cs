using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using ProceseSec;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ConcediiMedicale
{
    public partial class Aprobare : System.Web.UI.Page
    {

        public class metaCereriCM
        {
            public int Id { get; set; }
            public int Initial { get; set; }
            public double BazaCalculCM { get; set; }
            public int F10003 { get; set; }
            public int ZileBazaCalculCM { get; set; }
            public double MedieZileBazaCalcul { get; set; }
            public double MedieZilnicaCM { get; set; }
            public int Optiune { get; set; }
            public int Stagiu { get; set; }
            public DateTime DataInceput { get; set; }
            public string SerieCMInitial { get; set; }
            public string NumarCMInitial { get; set; }

        }     


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                txtTitlu.Text = Dami.TraduCuvant("Lista concedii medicale");

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnTransfera.Text = Dami.TraduCuvant("btnTransfera", "Transfera");
                btnAdauga.Text = Dami.TraduCuvant("btnAdauga", "Adauga CM");
                btnRapCM.Text = Dami.TraduCuvant("btnRapCM", "Rapoarte");
                btnCalcul.Text = Dami.TraduCuvant("btnCalcul", "Calcul medie CM");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aprobare");
                btnAnulare.Text = Dami.TraduCuvant("btnAnulare", "Anulare");

                lblAnLuna.InnerText = Dami.TraduCuvant("Luna/An");
                lblAngFiltru.InnerText = Dami.TraduCuvant("Angajat");

                foreach (ListBoxColumn col in cmbAngFiltru.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                foreach (GridViewColumn c in grDate.Columns)
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

                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                string sql = "SELECT COUNT(*) FROM \"F100Supervizori\" WHERE \"IdUser\" = {0} AND \"IdSuper\" IN ({1})";
                sql = string.Format(sql, Session["UserId"].ToString(), idHR);
                DataTable dtHR = General.IncarcaDT(sql, null);
                int tip = 1;
                if (dtHR != null && dtHR.Rows.Count > 0 && dtHR.Rows[0][0] != null && dtHR.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtHR.Rows[0][0].ToString()) > 0)
                {
                    tip = 2;
                    Session["CM_HR"] = "1";
                }
                else
                    Session["CM_HR"] = "0";

                if (tip == 1)
                {
                    btnAproba.ClientVisible = false;
                    btnTransfera.ClientVisible = false; 
                    btnRapCM.ClientVisible = false;
                    btnCalcul.ClientVisible = false;
                }              

                //txtTitlu.Text = General.VarSession("Titlu").ToString();    

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""CM_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;  
 
                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                if (!IsPostBack)
                {
                    Session["CM_Grid"] = null;
                    Session["CM_Aprobare"] = null;
                    Session["CM_Id"] = null;
                    Session["CM_Stare"] = null;

                    sql = "SELECT F01012, F01011 FROM F010";
                    DataTable dtLC = General.IncarcaDT(sql, null);
                    DateTime dtF010 = new DateTime(Convert.ToInt32(dtLC.Rows[0][1].ToString()), Convert.ToInt32(dtLC.Rows[0][0].ToString()), 1);

                    txtAnLuna.Value = dtF010.Date;
                }

                IncarcaGrid();
                grDate.SettingsPager.PageSize = 25;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CM_Grid"] = null;
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                int nrSel = 0;
                string ids = "" ;
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "BazaCalculCM", "ZileBazaCalculCM", "MedieZileBazaCalcul", "MedieZilnicaCM", "F10003", "IdStare", "Stagiu", "DI" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");         
                    return;
                }

                string sql = "SELECT F01012, F01011 FROM F010";
                DataTable dtLC = General.IncarcaDT(sql, null);
                DateTime dtF010 = new DateTime(Convert.ToInt32(dtLC.Rows[0][1].ToString()), Convert.ToInt32(dtLC.Rows[0][0].ToString()), 1);

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[8] as DateTime?;

                    if (data < dtF010)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("este anterior lunii de calcul") + System.Environment.NewLine;
                        continue;
                    }

                    if ((Convert.ToDouble(General.Nz(arr[1], 0)) == 0 || Convert.ToInt32(General.Nz(arr[2], 0)) == 0 || Convert.ToDouble(General.Nz(arr[3], 0)) == 0 || Convert.ToDouble(General.Nz(arr[4], 0)) == 0) && Convert.ToInt32(General.Nz(arr[7], 0)) == 0)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("date lipsa") + System.Environment.NewLine;
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) > 2)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("deja aprobat") + System.Environment.NewLine;
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) == -1)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("anulat") + System.Environment.NewLine;
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) < 2)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("nu are calculata media") + System.Environment.NewLine;
                        continue;
                    }

                    ids += "," + arr[0];
                    nrSel++;
                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)                    
                        MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");  
                    else
                        MessageBox.Show(msg, MessageBox.icoWarning, "");
                    return;
                }
                string[] lstIds = ids.Substring(1).Split(',');
                for (int i = 0; i < lstIds.Length; i++)
                {
                    sql = "UPDATE CM_CereriIstoric SET IdStare = 3, Aprobat = 1, DataAprobare = GETDATE(), CULOARE = (SELECT Culoare FROM CM_tblStari WHERE Id = 3), IdUser = " + Session["UserId"].ToString() + " WHERE IdCerere = " + lstIds[i] + " AND Pozitie = 2";
                    General.ExecutaNonQuery(sql, null);

                    sql = "UPDATE CM_Cereri SET IdStare = 3 WHERE Id = " + lstIds[i];
                    General.ExecutaNonQuery(sql, null);

                    TrimiteNotificare(Convert.ToInt32(lstIds[i]));
                }

                Session["CM_Grid"] = null;
                IncarcaGrid();

                MessageBox.Show(msg + (msg.Length > 0 ? System.Environment.NewLine : "") + (nrSel == 1 ? Dami.TraduCuvant("S-a aprobat un concediu medical!") : Dami.TraduCuvant("S-au aprobat") + " " + nrSel + " " + Dami.TraduCuvant("concedii medicale") + "!"), MessageBox.icoInfo, "");

             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAdauga_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "~/ConcediiMedicale/Introducere.aspx";
                Session["CM_Id"] = null;
                Session["CM_Id_Nou"] = null;
                Session["CM_NrZile"] = null;
                Session["CM_Grid"] = null;
                Session["CM_Document"] = null;
                Session["MarcaCM"] = null;

                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnCalcul_Click(object sender, EventArgs e)
        {
            try
            {
                int nrSel = 0;
                string msg = "";
                string msgTransf = "", msgAnulat = "", msgModMan = "", msgAv = "";
                List<metaCereriCM> ids = new List<metaCereriCM>();

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "BazaCalculCM", "ZileBazaCalculCM", "MedieZileBazaCalcul", "MedieZilnicaCM", "F10003", "IdStare", "DataInceput", "Initial", "ModifManuala", "Optiune", "Stagiu", "SerieCMInitial", "NumarCMInitial" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");
                    return;
                }

                string sql = "SELECT F01012, F01011 FROM F010";
                DataTable dtLC = General.IncarcaDT(sql, null);

                //for (int i = 0; i < lst.Count(); i++)
                for (int i = lst.Count() - 1; i >= 0; i--)
                {
                    object[] arr = lst[i] as object[];

                    if ((Convert.ToDateTime(arr[7]).Year < Convert.ToInt32(dtLC.Rows[0][1].ToString())) ||
                        (Convert.ToDateTime(arr[7]).Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && Convert.ToDateTime(arr[7]).Month < Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                    {//CM anterior lunii de calcul
                        //msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("este in avans") + System.Environment.NewLine;
                        //msgAv += ", " + arr[5];
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) == 4)
                    {
                        //msg += Dami.TraduCuvant("CM pentru marca") + " "  + arr[5] + " - " + Dami.TraduCuvant("deja transferat") + System.Environment.NewLine;
                        msgTransf += ", " + arr[5];
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) == -1)
                    {
                        //msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("anulat") + System.Environment.NewLine;
                        msgAnulat += ", " + arr[5];
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[9], 0)) == 1)
                    {
                        //msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("modificare manuala") + System.Environment.NewLine;
                        msgModMan += ", " + arr[5];
                        continue;
                    }

                    if ((Convert.ToDateTime(arr[7]).Year > Convert.ToInt32(dtLC.Rows[0][1].ToString())) ||
                        (Convert.ToDateTime(arr[7]).Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && Convert.ToDateTime(arr[7]).Month > Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                    {
                        //msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("este in avans") + System.Environment.NewLine;
                        msgAv += ", " + arr[5];
                        continue;
                    }

                    ids.Add(new metaCereriCM { Id = Convert.ToInt32(General.Nz(arr[0], 0)), F10003 = Convert.ToInt32(General.Nz(arr[5], 0)), Initial = Convert.ToInt32(General.Nz(arr[8], 0)),
                                    BazaCalculCM = Convert.ToDouble(General.Nz(arr[1], 0)), ZileBazaCalculCM = Convert.ToInt32(General.Nz(arr[2], 0)), MedieZileBazaCalcul = Convert.ToDouble(General.Nz(arr[3], 0)),
                                    MedieZilnicaCM = Convert.ToDouble(General.Nz(arr[4], 0)), Optiune = Convert.ToInt32(General.Nz(arr[10], 0)), Stagiu = Convert.ToInt32(General.Nz(arr[11], 0)),
                                    SerieCMInitial = Convert.ToString(General.Nz(arr[12], "")),
                                    NumarCMInitial = Convert.ToString(General.Nz(arr[13], ""))
                    });
                    nrSel++;
                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                        MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");
                    else
                        MessageBox.Show(msg, MessageBox.icoWarning, "");
                    return;
                }

                string msgStagiu = "";

                for (int k = 0; k < ids.Count; k++)
                {
                    double BCCM = 0, Medie6 = 0, MNTZ = 0;
                    bool areStagiu = false;
                    int ZBCCM = 0;
                    string strCNP_marci = "", cmpStagiu = "";

                    if (ids[k].Initial == 1)
                    {
                        DataTable dt = General.IncarcaDT("SELECT F10003 FROM F100 WHERE F10017 = (SELECT F10017 FROM F100 WHERE F10003 =  " + ids[k].F10003 + ")", null);
                        for (int i = 0; i < dt.Rows.Count; i++)
                            strCNP_marci += "," + dt.Rows[i][0].ToString();
                        Module.ConcediiMedicale.CreateDetails(ids[k].F10003, strCNP_marci.Substring(1), ids[k].Optiune, out Medie6, out BCCM, out ZBCCM, out MNTZ, out areStagiu);
                        if (!areStagiu)
                        {
                            if (ids[k].Stagiu == 0)
                                msgStagiu += ", " + ids[k].F10003;
                            string sqltmp = "UPDATE CM_Cereri SET Tarif = 0 WHERE Id = {0}";
                            sqltmp = string.Format(sqltmp, ids[k].Id);
                            General.ExecutaNonQuery(sqltmp, null);
                            string sqlParam = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'BCCM'";
                            DataTable dtParam = General.IncarcaDT(sqlParam, null);                           
                            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                            {
                                int nn = Convert.ToInt32(dtParam.Rows[0][0].ToString()) - 1;
                                sqltmp = "UPDATE F100 SET F10069{0} = 0.0 WHERE F10003 = {1}";
                                sqltmp = string.Format(sqltmp, nn, ids[k].F10003);
                                General.ExecutaNonQuery(sqltmp, null);                                  
                            }
                            BCCM = 0;
                            ZBCCM = 0;
                            MNTZ = 0;
                            Medie6 = 0;
                            if (ids[k].Stagiu == 1)
                            {
                                cmpStagiu = ", CodTransfer5 = 0, NrZileCT5 = 0 ";
                            }
                        }
                    }
                    else
                    {
                        if (ids[k].BazaCalculCM != 0 && ids[k].ZileBazaCalculCM != 0 && ids[k].MedieZileBazaCalcul != 0 && ids[k].MedieZilnicaCM != 0)
                        {
                            BCCM = ids[k].BazaCalculCM;
                            ZBCCM = ids[k].ZileBazaCalculCM;
                            MNTZ = ids[k].MedieZileBazaCalcul;
                            Medie6 = ids[k].MedieZilnicaCM;
                        }
                        else
                        {
                            sql = "select CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as Id, BazaCalculCM, ZileBazaCalculCM, MedieZileBazaCalcul, MedieZilnicaCM from cm_cereri where SerieCM = '" + ids[k].SerieCMInitial + "' and NumarCM = '" + ids[k].NumarCMInitial + "' "
                                    + " union "
                                    + " select 100 + CONVERT(int, ROW_NUMBER() OVER(ORDER BY(SELECT 1))) as Id, f300612 as BazaCalculCM, F300613 as ZileBazaCalculCM, f300614 as MedieZileBazaCalcul, f300620 as MedieZilnicaCM "
                                    + " from f300 where F300601 = '" + ids[k].SerieCMInitial + "' and F300602 = '" + ids[k].NumarCMInitial + "' "
                                    + " union "
                                    + " select 200 + CONVERT(int, ROW_NUMBER() OVER(ORDER BY(SELECT 1))) as Id, f940612 as BazaCalculCM, F940613 as ZileBazaCalculCM, f940614 as MedieZileBazaCalcul, f940620 as MedieZilnicaCM "
                                    + " from f940 where F940601 = '" + ids[k].SerieCMInitial + "' and F940602 = '" + ids[k].NumarCMInitial + "' ORDER BY Id";
                            DataTable dtCalcul = General.IncarcaDT(sql, null);
                            if (dtCalcul != null && dtCalcul.Rows.Count > 0)
                            {
                                BCCM = Convert.ToDouble(dtCalcul.Rows[0]["BazaCalculCM"].ToString());
                                ZBCCM = Convert.ToInt32(dtCalcul.Rows[0]["ZileBazaCalculCM"].ToString());
                                MNTZ = Convert.ToDouble(dtCalcul.Rows[0]["MedieZileBazaCalcul"].ToString());
                                Medie6 = Convert.ToDouble(dtCalcul.Rows[0]["MedieZilnicaCM"].ToString());
                            }
                            else
                            {
                                BCCM = 0;
                                ZBCCM = 0;
                                MNTZ = 0;
                                Medie6 = 0;
                            }
                        }
                    }

                    sql = "UPDATE CM_Cereri SET BazaCalculCM = " + BCCM.ToString().Replace(',', '.') + ", ZileBazaCalculCM = " + ZBCCM.ToString() + ", MedieZileBazaCalcul = " 
                        + MNTZ.ToString("0.##").Replace(',', '.') + ", MedieZilnicaCM = " + Medie6.ToString("0.##").Replace(',', '.') + cmpStagiu + " WHERE Id = " + ids[k].Id;
                    General.ExecutaNonQuery(sql, null);

                    if ((BCCM != 0 && ZBCCM != 0 && MNTZ != 0 && Medie6 != 0) || ids[k].Stagiu == 1)
                    {
                        sql = "UPDATE CM_Cereri SET IdStare = 2, USER_NO = " + Session["UserId"].ToString() + ", TIME = GETDATE() WHERE Id = " + ids[k].Id + " AND IdStare < 2";
                        General.ExecutaNonQuery(sql, null);

                        sql = "UPDATE CM_CereriIstoric SET IdStare = 2, IdUser = " + Session["UserId"].ToString() + ", Culoare = (SELECT Culoare FROM CM_tblStari WHERE Id = 2)  WHERE IdCerere = " + ids[k].Id + " AND Pozitie = 2 AND IdStare < 2";
                        General.ExecutaNonQuery(sql, null);

                        TrimiteNotificare(ids[k].Id);
                    }
                }

                string text = "";
                text = Dami.TraduCuvant("S-a calculat media pentru") + " " + (nrSel == 1 ? Dami.TraduCuvant("un concediu medical") + "!" : nrSel + " " + Dami.TraduCuvant("concedii medicale") + "!");

                //MessageBox.Show(msg + (msg.Length > 0 ? System.Environment.NewLine : "") + Dami.TraduCuvant("S-a calculat media pentru") + " " + (nrSel == 1 ? Dami.TraduCuvant("un concediu medical") + "!" : nrSel + " " + Dami.TraduCuvant("concedii medicale") + "!") +
                MessageBox.Show(text + (msgStagiu.Length > 0 ? System.Environment.NewLine + Dami.TraduCuvant("Urmatorii angajati nu au stagiu") + ":" + msgStagiu.Substring(1) + System.Environment.NewLine + Dami.TraduCuvant("Va rugam verificati si bifati corespunzator concediile medicale respective!") : "") + System.Environment.NewLine + Dami.TraduCuvant("Rezultatele au fost salvate in fisier."));

                //(msgStagiu.Length > 0 ? System.Environment.NewLine + Dami.TraduCuvant("Urmatorii angajati nu au stagiu") + ":" + msgStagiu.Substring(1) : ""), MessageBox.icoInfo, ""); ;

                try
                {
                    string cale = Dami.ValoareParam("CM_CaleFisierRezultat", "");
                    if (cale.Length > 0)
                    {
                        StreamWriter sw = new StreamWriter(cale + "\\Calcul.txt", true);
                        //              
                        text += "\r\n" + "Data:    " + DateTime.Now.ToString() + "\r\n";

                        if (msgTransf.Length > 0)
                            text += "CM deja transferat:" + msgTransf.Substring(1) + "\r\n";

                        if (msgAnulat.Length > 0)
                            text += "CM anulat:" + msgAnulat.Substring(1) + "\r\n";

                        if (msgModMan.Length > 0)
                            text += "CM modificat manual:" + msgModMan.Substring(1) + "\r\n";

                        if (msgAv.Length > 0)
                            text += "CM in avans:" + msgAv.Substring(1) + "\r\n";

                        if (msgStagiu.Length > 0)
                            text += "Urmatorii angajati nu au stagiu:" + msgStagiu.Substring(1) + "\r\n";
                        //
                        sw.Write(text + "-----------------------------------------------------" + "\r\n");
                        //
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                }

                Session["CM_Grid"] = null;
                IncarcaGrid();

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void btnTransfera_Click(object sender, EventArgs e)
        {
            try
            {
                int nrSel = 0;
                string ids = "";
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "BazaCalculCM", "ZileBazaCalculCM", "MedieZileBazaCalcul", "MedieZilnicaCM", "F10003", "IdStare", "DataInceput" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");
                    return;
                }

                string sql = "SELECT F01012, F01011 FROM F010";
                DataTable dtLC = General.IncarcaDT(sql, null);

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) < 3)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("nu este aprobat") + System.Environment.NewLine;
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) == 4)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("deja transferat") + System.Environment.NewLine;
                        continue;
                    }

                    if ((Convert.ToDateTime(arr[7]).Year < Convert.ToInt32(dtLC.Rows[0][1].ToString())) ||
                        (Convert.ToDateTime(arr[7]).Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && Convert.ToDateTime(arr[7]).Month < Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                    {
                        msg += Dami.TraduCuvant("CM pt marca") + " " + arr[5] + " - " + Dami.TraduCuvant("este anterior lunii de calcul") + System.Environment.NewLine;
                        continue;
                    }

                    ids += "," + arr[0];
                    nrSel++;
                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                        MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");
                    else
                        MessageBox.Show(msg, MessageBox.icoWarning, "");
                    return;
                }
                string[] lstIds = ids.Substring(1).Split(',');
                for (int i = 0; i < lstIds.Length; i++)
                {
                    DataTable dt = General.IncarcaDT("SELECT * FROM CM_Cereri WHERE Id = " + lstIds[i], null);
                    Session["CM_Aprobare_Detalii"] = dt;

                    //sql = "INSERT INTO F300 (F30001, F30002, F30003, F30004, F30005, F30006, F30007, F30010, F30011, F30015, F30035, F30036, F30037, F30038, F30050,  USER_NO, TIME,"
                    //         + " F30012, F30014,  F30021, F30022, F30023, F30039, F30040, F30041, F30044, F30045, F30046, F30051, F30053, F300612, F300613, F300614, F30054, F30042, F300620, F300601, F300602, F300607, F300609, F300610, F300619, F300603, F300606, F300608, F300611, F300615, F300616, F300617, F300618, F30013)"
                    //         + " SELECT 300, F10002, F10003, F10004, F10005, F10006, F10007, {0},  1, {1}, {2}, {2}, "
                    //         + " {3}, {4}, CASE WHEN F10053 IS NULL OR F10053=0 THEN F00615 ELSE F10053 END, "
                    //         + " {5}, {6}, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {8}, {9}, {10}, 0, 'Transfer', {11}, '{12}', '{13}', {14}, {15}, {16}, {17}, {2}, '{18}', '{19}', {20}, '{21}', '{22}', '{23}', {24}, {25}"
                    //         + " FROM F100, F006 WHERE F10003 = {7} AND F10007=F00607";
                    //sql = string.Format(sql, dt.Rows[0]["Cod"].ToString(), dt.Rows[0]["Suma"].ToString().Replace(',', '.'),
                    //    "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()).Year + "', 103)",
                    //    "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).Year + "', 103)",
                    //    "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).Year + "', 103)",

                    //    Session["UserId"].ToString(),
                    //     (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), dt.Rows[0]["F10003"].ToString(),
                    //     dt.Rows[0]["BazaCalculCM"].ToString().Replace(',', '.'), dt.Rows[0]["ZileBazaCalculCM"].ToString(), dt.Rows[0]["MedieZilnicaCM"].ToString().Replace(',', '.'), dt.Rows[0]["MedieZileBazaCalcul"].ToString().Replace(',', '.'),
                    //      dt.Rows[0]["SerieCM"].ToString(), dt.Rows[0]["NumarCM"].ToString(), dt.Rows[0]["CodIndemnizatie"].ToString(), dt.Rows[0]["CodUrgenta"].ToString(), dt.Rows[0]["CodInfectoContag"].ToString(), dt.Rows[0]["CodDiagnostic"].ToString(),
                    //      dt.Rows[0]["SerieCMInitial"].ToString(), dt.Rows[0]["NumarCMInitial"].ToString(), dt.Rows[0]["LocPrescriere"].ToString(), dt.Rows[0]["NrAvizMedicExpert"].ToString(), dt.Rows[0]["MedicCurant"].ToString(), dt.Rows[0]["CNPCopil"].ToString(),
                    //      "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString()).Year + "', 103)",
                    //       dt.Rows[0]["NrZile"].ToString());
                    //General.ExecutaNonQuery(sql, null);

                    double procent, suma4449 = 0, suma4450 = 0, vechime;
                    double sumaReducereTimpLucru = 0;
                    double suma_4450_subplafon = 0;
                    int cc = 0; // Convert.ToInt32(txtCC.Text);

                    bool avans = false;
                    DateTime dataInc = Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString());
                    DateTime dataSf = Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString());
                    if (dataInc.Month != Convert.ToInt32(dtLC.Rows[0][0].ToString()))
                    {

                        if (dataInc.Year > Convert.ToInt32(dtLC.Rows[0][1].ToString()) || (dataInc.Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && dataInc.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                            avans = true;                
                    }

                    DataTable dtAng = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + dt.Rows[0]["F10003"].ToString(), null);
                    vechime = ConvertVechime(dtAng.Rows[0]["F100644"].ToString());

                    int no = Convert.ToInt32(dt.Rows[0]["TipConcediu"].ToString());
                    sql = "SELECT * FROM MARDEF WHERE NO = " + no;
                    DataTable dtMARDEF = General.IncarcaDT(sql, null);

                    DateTime odtCarantina = new DateTime(2020, 7, 21, 0, 0, 0);
                    int tab_procent = Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString());
                    if (Convert.ToInt32(dt.Rows[0]["Urgenta"].ToString()) == 1) 
                        tab_procent = 3;
                    if (Convert.ToInt32(dt.Rows[0]["CodIndemnizatie"].ToString()) == 7 && Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()) < odtCarantina)
                        tab_procent = 1;

                    DataTable dt069 = General.IncarcaDT("SELECT * FROM F069 WHERE F06904 = " + dtLC.Rows[0][1].ToString() + " AND F06905 = " + dtLC.Rows[0][0].ToString(), null);


                    if (!avans)
                    {
                        int zile2 = 0;
                        int zile = 0;
                        if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) >= 0 && (Convert.ToInt32(dt.Rows[0]["CodIndemnizatie"].ToString()) != 5 || dt.Rows[0]["CodDiagnostic"].ToString() != "064"))
                        {
                            zile = Convert.ToInt32(dt.Rows[0]["NrZileCT1"].ToString());
                            zile2 = Convert.ToInt32(dt.Rows[0]["NrZileCT2"].ToString());
                            if (zile > 0 || (zile == 0 && zile2 == 0))
                            {
                                procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) / 100);
                                suma4450 += (Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * zile * (procent / 100));
                                suma4450 = rotunjire(2, 1, suma4450);
                                AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()), zile, cc, procent, 0, true, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), false);
                            }
                        }
                        if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) >= 0)
                        {
                            if (Convert.ToInt32(dt.Rows[0]["CodIndemnizatie"].ToString()) == 5 && dt.Rows[0]["CodDiagnostic"].ToString() == "064")
                                zile = Convert.ToInt32(dt.Rows[0]["NrZileCT1"].ToString()) + Convert.ToInt32(dt.Rows[0]["NrZileCT2"].ToString());
                            else
                                zile = Convert.ToInt32(dt.Rows[0]["NrZileCT2"].ToString());
                            if (zile > 0)
                            {
                                procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) / 100);
                                suma4450 += (Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * zile * (procent / 100));
                                suma4450 = rotunjire(2, 1, suma4450);
                                AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()), zile, cc, procent, 0, true, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), false);
                            }
                        }
                        if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) >= 0)
                        {
                            zile = Convert.ToInt32(dt.Rows[0]["NrZileCT3"].ToString());
                            if (zile > 0)
                            {
                                procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) / 100);
                                // mihad 19.03.2018
                                suma4450 += (Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * zile * (procent / 100));
                                suma4450 = rotunjire(2, 1, suma4450);
                                AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()), zile, cc, procent, 0, true, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), false);
                            }
                        }
                        if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) >= 0)
                        {
                            zile = Convert.ToInt32(dt.Rows[0]["NrZile"].ToString());
                            if (zile >= 0)  //Radu 27.05.2015	- am pus >= ca sa poata fi introduse concedii medicale cu Cod 4 de 0 ZL
                            {
                                int cod_ind = Convert.ToInt32(dt.Rows[0]["CodIndemnizatie"].ToString());
                                double mntz = Convert.ToDouble(dt.Rows[0]["MedieZileBazaCalcul"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZileBazaCalcul"].ToString().Trim());
                                if (cod_ind == 10 && mntz > 0)
                                {// reducere timp de lucru
                                    sumaReducereTimpLucru = (Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * mntz);
                                    sumaReducereTimpLucru = sumaReducereTimpLucru - (Convert.ToInt32(dtAng.Rows[0]["F100699"].ToString()) * 0.75);
                                    if (sumaReducereTimpLucru > (Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * mntz * 0.25))
                                        sumaReducereTimpLucru = Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * mntz * 0.25;
                                    sumaReducereTimpLucru = rotunjire(2, 1, (sumaReducereTimpLucru / Convert.ToInt32(dt069.Rows[0]["F06907"].ToString())) * zile);
                                }
                                else
                                    sumaReducereTimpLucru = 0;

                                procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) / 100);
                                // mihad 19.03.2018
                                suma4450 += (Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim()) * zile * (procent / 100));
                                suma4450 = rotunjire(2, 1, suma4450);
                                AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()), zile, cc, procent, 0, true, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), false);
                            }
                        }

                        if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC5"].ToString()) >= 0)
                        {
                            zile = Convert.ToInt32(dt.Rows[0]["NrZileCT5"].ToString());
                            if (zile > 0)
                            {
                                // cod 4449 in loc de 4450 pentru concediile pe perioada 01.01.2018 - 30.06.2018
                                if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()) == 4450)
                                {

                                    // venituri mici ce le iese suma CM mai mica decat plafon
                                    suma4449 = rotunjire(2, 1, (((3131.0 / Convert.ToInt32(dt069.Rows[0]["F06907"].ToString())) * 0.35) * zile * 0.105));
                                    suma4450 = rotunjire(2, 1, suma4450);

                                    if (rotunjire(2, 1, (suma4450 * 0.25)) < suma4449)
                                    {
                                        suma_4450_subplafon = suma4450;
                                    }
                                    //
                                    else
                                    {
                                        suma_4450_subplafon = 0;


                                        if ((dataInc.Year == 2018 && dataInc.Month <= 6) || dataInc.Year < 2018)
                                            dtMARDEF.Rows[0]["CODE5"] = 4449;
                                        else
                                        {
                                            // caut daca nu e cod special		
                                            string szCoduri = "";
                                            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CM_TARIF35'", null);
                                            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                                                szCoduri = dtParam.Rows[0][0].ToString();

                                            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && szCoduri.Contains(dtMARDEF.Rows[0]["CODE4"].ToString()) && ((dataInc.Year == 2018 && dataInc.Month <= 9) || dataInc.Year < 2018))
                                            {
                                                dtMARDEF.Rows[0]["CODE5"] = 4449;
                                            }
                                            else
                                            {
                                                // caut initialul daca nu e in perioada
                                                string serie_i = "", numar_i = "";
                                                serie_i = dt.Rows[0]["SerieCMInitial"].ToString();
                                                numar_i = dt.Rows[0]["NumarCMInitial"].ToString();
                                                if (serie_i.Length > 0 && numar_i.Length > 0)
                                                {
                                                    string szT = "";
                                                    if (Constante.tipBD == 2)
                                                        szT = "SELECT TO_CHAR(F94037,'DD/MM/YYYY') FROM F940 WHERE F940601 = '{0}' AND F940602 = '{1}' AND F94003 = {2} AND F94010 >= 4401 AND F94010 < 4449 ORDER BY F94010";
                                                    else
                                                        szT = "SELECT CONVERT(VARCHAR,F94037,103) FROM F940 WHERE F940601 = '{0}' AND F940602 = '{1}' AND F94003 = {2} AND F94010 >= 4401 AND F94010 < 4449 ORDER BY F94010";

                                                    sql = "";
                                                    sql = string.Format(szT, serie_i, numar_i, dt.Rows[0]["F10003"].ToString());

                                                    DataTable dtIstoric = General.IncarcaDT(sql, null);

                                                    if (dtIstoric != null && dtIstoric.Rows.Count > 0)
                                                    {
                                                        for (int k = 0; k < dtIstoric.Rows.Count; k++)
                                                        {
                                                            DateTime data_i = new DateTime(Convert.ToInt32(dtIstoric.Rows[k][0].ToString().Substring(6, 4)), Convert.ToInt32(dtIstoric.Rows[k][0].ToString().Substring(3, 2)), Convert.ToInt32(dtIstoric.Rows[k][0].ToString().Substring(0, 2)));

                                                            if ((data_i.Year == 2018 && data_i.Month <= 6) || dataInc.Year < 2018)
                                                                dtMARDEF.Rows[0]["CODE5"] = 4449;
                                                            else
                                                            {
                                                                if ((Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0) && szCoduri.Contains(dtMARDEF.Rows[0]["CODE4"].ToString()) && ((dataInc.Year == 2018 && dataInc.Month <= 9) || dataInc.Year < 2018))
                                                                {
                                                                    dtMARDEF.Rows[0]["CODE5"] = 4449;
                                                                }
                                                                else
                                                                    suma_4450_subplafon = suma4450;
                                                            }
                                                        }
                                                    }
                                                    else
                                                        suma_4450_subplafon = suma4450;
                                                }
                                                else
                                                    suma_4450_subplafon = suma4450;
                                            }
                                        }
                                    }
                                }

                                procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC5"].ToString()) / 100);
                                DataTable dtF300 = General.IncarcaDT("SELECT * FROM F300 WHERE F30003 = " + dt.Rows[0]["F10003"].ToString() + " AND F30010 = " + dtMARDEF.Rows[0]["CODE5"].ToString(), null);

                                foreach (DataColumn col in dtF300.Columns)
                                    col.ReadOnly = false;

                                if (dtF300 != null && dtF300.Rows.Count > 0)
                                {
                                    if (!(Convert.ToInt32(dt.Rows[0]["Stagiu"] == DBNull.Value ? "0" : dt.Rows[0]["Stagiu"].ToString()) == 1 && (Convert.ToInt32(dtF300.Rows[0]["F30010"].ToString()) == 4450 || Convert.ToInt32(dtF300.Rows[0]["F30010"].ToString()) == 4449)))
                                    {
                                        string sName = dtMARDEF.Rows[0]["NAME"].ToString();
                                        if (!(Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || sName.Contains("AMBP")))
                                        {
                                            dtF300.Rows[0]["F30013"] = Convert.ToInt32(Convert.ToDouble(dtF300.Rows[0]["F30013"].ToString())) + zile;
                                            dtF300.Rows[0]["F30015"] = Convert.ToInt32(Convert.ToDouble(dtF300.Rows[0]["F30015"].ToString())) + suma_4450_subplafon;
                                            sql = "UPDATE F300 SET F30013 = {0}, F30015 = {1} WHERE F30003 = " + dt.Rows[0]["F10003"].ToString() + " AND F30010 = " + dtMARDEF.Rows[0]["CODE5"].ToString();
                                            sql = string.Format(sql, dtF300.Rows[0]["F30013"].ToString(), dtF300.Rows[0]["F30015"].ToString());
                                            General.ExecutaNonQuery(sql, null);
                                        }
                                    }
                                    if (Convert.ToInt32(Convert.ToDouble(dtF300.Rows[0]["F300620"].ToString())) > 0 && Convert.ToInt32(dtF300.Rows[0]["F300607"].ToString()) == 10 && sumaReducereTimpLucru > 0)
                                        dtF300.Rows[0]["F30015"] = Convert.ToInt32(Convert.ToDouble(dtF300.Rows[0]["F30015"].ToString())) + sumaReducereTimpLucru;
                                }
                                else
                                    AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()), zile, cc, procent, suma4450 /*0*/, true, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), false);

                                //CR Ambasada SUA
                                string data1Amb = "CONVERT(DATETIME,'" + dataInc.Day.ToString().PadLeft(2, '0') + "/" + dataInc.Month.ToString().PadLeft(2, '0') + "/" + dataInc.Year.ToString() + "',103)";
                                string data2Amb = "CONVERT(DATETIME,'" + dataSf.Day.ToString().PadLeft(2, '0') + "/" + dataSf.Month.ToString().PadLeft(2, '0') + "/" + dataSf.Year.ToString() + "',103)";
                                string sqlAmb = "UPDATE F300 SET F30042 = '{0} ' + F30042 WHERE F30003 = {1} AND F30037 = {2} AND F30038 = {3} AND CONVERT(VARCHAR, F30010) IN ('{4}', '{5}', '{6}', '{7}')";
                                sqlAmb = string.Format(sqlAmb, dtMARDEF.Rows[0]["CODE5"].ToString(), dt.Rows[0]["F10003"].ToString(), data1Amb, data2Amb, dtMARDEF.Rows[0]["CODE1"].ToString(), dtMARDEF.Rows[0]["CODE2"].ToString(),
                                    dtMARDEF.Rows[0]["CODE3"].ToString(), dtMARDEF.Rows[0]["CODE4"].ToString());
                                General.ExecutaNonQuery(sqlAmb, null);
                            }
                        }

                        //if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE6"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC6"].ToString()) >= 0)
                        //{
                        //    zile = Convert.ToInt32(txtCT6.Text);
                        //    if (zile > 0)
                        //    {
                        //        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC6"].ToString()) / 100);
                        //        DataTable dtF300 = General.IncarcaDT("SELECT * FROM F300 WHERE F30003 = " + dt.Rows[0]["F10003"].ToString() + " AND F30010 = " + dtMARDEF.Rows[0]["CODE6"].ToString(), null);

                        //        if (dtF300 != null && dtF300.Rows.Count > 0)
                        //        {
                        //            //string sz;
                        //            dtF300.Rows[0]["F300611"] = Convert.ToInt32(cmbLocPresc.Value);
                        //            dtF300.Rows[0]["F300612"] = Convert.ToInt32(txtBCCM.Text);
                        //            dtF300.Rows[0]["F300613"] = Convert.ToInt32(txtZBC.Text);
                        //            dtF300.Rows[0]["F300614"] = Convert.ToInt32(dt.Rows[0]["MedieZilnicaCM"].ToString().Trim().Length <= 0 ? "0" : dt.Rows[0]["MedieZilnicaCM"].ToString().Trim());
                        //            dtF300.Rows[0]["F300615"] = txtNrAviz.Text;
                        //            dtF300.Rows[0]["F300616"] = txtMedic.Text;
                        //            dtF300.Rows[0]["F300617"] = (cmbCNPCopil.Value ?? "").ToString();
                        //            dtF300.Rows[0]["F300620"] = Convert.ToDouble(dt.Rows[0]["MedieZileBazaCalcul"].ToString());


                        //            string sName = dtMARDEF.Rows[0]["NAME"].ToString();
                        //            if (!(Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || sName.Contains("AMBP")))
                        //            {
                        //                dtF300.Rows[0]["F30013"] = Convert.ToInt32(dtF300.Rows[0]["F30013"].ToString()) + zile;
                        //                sql = "UPDATE F300 SET F30013 = {0}, F300611 = {1}, F300612 = {2}, F300613 = {3}, F300614 = {4}, F300615 = {5}, F300616 = {6}, F300617 = {7} WHERE F30003 = " + dt.Rows[0]["F10003"].ToString() + " AND F30010 = " + dtMARDEF.Rows[0]["CODE6"].ToString();
                        //                sql = string.Format(sql, dtF300.Rows[0]["F30013"].ToString(), dtF300.Rows[0]["F300611"].ToString(), dtF300.Rows[0]["F300612"].ToString(), dtF300.Rows[0]["F300613"].ToString(), dtF300.Rows[0]["F300614"].ToString(),
                        //                     dtF300.Rows[0]["F300615"].ToString(), dtF300.Rows[0]["F300616"].ToString(), dtF300.Rows[0]["F300617"].ToString());
                        //                General.ExecutaNonQuery(sql, null);
                        //            }
                        //        }
                        //        else
                        //            AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE6"].ToString()), zile, cc, procent, 0, true, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), false);

                        //    }
                        //}

                    }
                    else
                    {
                        int zile = Convert.ToInt32(dt.Rows[0]["NrZile"].ToString());
                        AddConcediu(0, zile, cc, 0, 0, false, Convert.ToInt32(dt.Rows[0]["F10003"].ToString()), avans);
                    }

                    sql = "UPDATE CM_Cereri SET IdStare = 4 WHERE Id = " + lstIds[i];
                    General.ExecutaNonQuery(sql, null);

                    sql = "UPDATE CM_CereriIstoric SET IdStare = 4, IdUser = " + Session["UserId"].ToString() + ", Culoare = (SELECT Culoare FROM CM_tblStari WHERE Id = 4)  WHERE IdCerere = " + lstIds[i] + " AND Pozitie = 2";
                    General.ExecutaNonQuery(sql, null);

                    //#1062
                    TransferPontaj(dt.Rows[0]["F10003"].ToString(), dataInc, dataSf, "CM");

                    TrimiteNotificare(Convert.ToInt32(lstIds[i]));
                }


                Session["CM_Aprobare_Detalii"] = null;
                Session["CM_Grid"] = null;
                IncarcaGrid();

                MessageBox.Show(msg + (msg.Length > 0 ? System.Environment.NewLine : "") + (nrSel == 1 ? Dami.TraduCuvant("S-a transferat un concediu medical!") : Dami.TraduCuvant("S-au transferat") + " "  + nrSel + " " + Dami.TraduCuvant("concedii medicale") + "!"), MessageBox.icoInfo, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            { 

                grDate.KeyFieldName = "Id";

                if (Session["CM_Grid"] == null)
                    dt = SelectGrid();
                else
                    dt = Session["CM_Grid"] as DataTable;

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["CM_Grid"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Replace(',', ';').Split(';');        

                    switch (arr[0])
                    {      
                        case "btnEdit":
                            Session["CM_Id"] = arr[1];
                            Session["CM_Stare"] = arr[2];
                            Session["CM_DataStart"] = arr[3];
                            CriptDecript prc = new CriptDecript();
                            //string param = General.Nz(Request["tip"], prc.EncryptString(Constante.cheieCriptare, "Introducere", 1)).ToString();
                            //param = prc.EncryptString(Constante.cheieCriptare, param, 2);
                            //int tip = param == "Aprobare" ? 2 : 1;  
                            string url = "~/ConcediiMedicale/Introducere.aspx";
                            if (Page.IsCallback)
                                ASPxWebControl.RedirectOnCallback(url);
                            else
                                Response.Redirect(url, false);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
                    DataRow[] lst = dt.Select("Id=" + idStare);
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
                }
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

                DataTable dt = Session["CM_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

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
                Session["CM_Grid"] = dt;
                grDate.DataSource = dt;
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        {
            try
            {
                if (e.VisibleIndex == -1) return;

                if (e.VisibleIndex >= 0)
                {
                    DataRowView values = grDate.GetRow(e.VisibleIndex) as DataRowView;
                    DateTime dtLucru = General.DamiDataLucru();
                    if (values != null)
                    {
                        DateTime dtInceput = Convert.ToDateTime(values.Row["DataInceput"].ToString());
                        if (e.ButtonID == "btnDelete")
                        {
                            if (dtLucru.Year > dtInceput.Year || (dtLucru.Year == dtInceput.Year && dtLucru.Month > dtInceput.Month)) 
                                e.Visible = DefaultBoolean.False;
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

        protected void grDate_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            try
            {
                DateTime dtLucru = General.DamiDataLucru();

                if (e.VisibleIndex >= 0)
                {
                    DataRowView values = grDate.GetRow(e.VisibleIndex) as DataRowView;

                    if (values != null)
                    {
                        DateTime dtInceput = Convert.ToDateTime(values.Row["DataInceput"].ToString());
                        int idStare = Convert.ToInt32(values.Row["IdStare"].ToString());

                        if ((dtLucru.Year != dtInceput.Year && dtLucru.Month !=  dtInceput.Month) || (idStare == 3 || idStare == 4))
                        {
                            if (e.ButtonType == ColumnCommandButtonType.Edit)

                                e.Visible = false;
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



        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
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

        private string SelectAngajati()
        {
            string strSql = "";

            try
            {


                strSql = $@"select F10003, ""NumeComplet"", ""Functia"", ""Subcompanie"", ""Filiala"", ""Sectie"", ""Departament""
                        from
                        (SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"",
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                        FROM  F100 A
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" FF on A.f10003 = FF.f10003 
                        WHERE  FF.IdUser  = {Session["UserId"]}
                        union
                        SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament""
                        FROM F100 A 
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" GG on A.f10003 = GG.f10003 and CHARINDEX(',' + CONVERT(nvarchar(20),GG.""IdSuper"") + ','  ,  ',' + (SELECT Valoare FROM tblParametrii WHERE Nume='Avans_IDuriRoluriHR') + ',') > 0                     
                        WHERE gg.iduser = {Session["UserId"]} ) T order by T.""NumeComplet"" ";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }



        public DataTable SelectGrid()
        {

            DataTable q = null;

            string sql = "SELECT F01012, F01011 FROM F010";
            DataTable dtLC = General.IncarcaDT(sql, null);

            int an, luna;

            an = Convert.ToInt32(dtLC.Rows[0][0].ToString());
            luna = Convert.ToInt32(dtLC.Rows[0][1].ToString());

            if (txtAnLuna.Value != null)
            {
                an = Convert.ToDateTime(txtAnLuna.Value).Year;
                luna = Convert.ToDateTime(txtAnLuna.Value).Month;
            }

            try
            {
                string strSql = "";
      
                int tip = Convert.ToInt32(General.Nz(Session["CM_HR"], "0").ToString());
                if (tip == 1)
                    strSql = "SELECT F10003, SerieCM, NumarCM, SerieCMInitial, NumarCMInitial, CodDiagnostic, DataInceput, DataSfarsit, BazaCalculCM, ZileBazaCalculCM, MedieZileBazaCalcul, MedieZilnicaCM, "
                        + "ModifManuala, Optiune,  Id, USER_NO, TIME, IdStare, Document, NrZile, Initial, Stagiu, Convert(VARCHAR, DataInceput, 103) as DI FROM CM_Cereri where year(DataInceput) = " + an + " and month(DataInceput) = " + luna + " ORDER BY Id DESC";
                else
                    strSql = "SELECT F10003, SerieCM, NumarCM, SerieCMInitial, NumarCMInitial, CodDiagnostic, DataInceput, DataSfarsit, BazaCalculCM, ZileBazaCalculCM, MedieZileBazaCalcul, MedieZilnicaCM, "
                        + "ModifManuala, Optiune,  Id, USER_NO, TIME, IdStare, Document, NrZile, Initial, Stagiu, Convert(VARCHAR, DataInceput, 103) as DI FROM CM_Cereri WHERE F10003 IN (SELECT F10003 FROM F100Supervizori WHERE IdUser = " + Session["UserId"].ToString() + ") AND year(DataInceput) = " + an + " and month(DataInceput) = " + luna + " ORDER BY Id DESC";


                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        protected void btnAnulare_Click(object sender, EventArgs e)
        {
            try
            {
                int nrSel = 0;
                List<metaCereriCM> ids = new List<metaCereriCM>();
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "BazaCalculCM", "ZileBazaCalculCM", "MedieZileBazaCalcul", "MedieZilnicaCM", "F10003", "IdStare", "DataInceput" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");
                    return;
                }

                string sql = "SELECT F01012, F01011 FROM F010";
                DataTable dtLC = General.IncarcaDT(sql, null);

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[7] as DateTime?;

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) < 0)
                    {
                        msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("deja anulat") + System.Environment.NewLine;
                        continue;
                    }

                    if (Session["CM_HR"] == null || Session["CM_HR"].ToString() != "1")
                    {
                        if (Convert.ToInt32(General.Nz(arr[6], 0)) > 1)
                        {
                            msg += Dami.TraduCuvant("CM pentru marca") + " " + arr[5] + " - " + Dami.TraduCuvant("nu aveti drepturi") + System.Environment.NewLine;
                            continue;
                        }
                    }

                    if ((Convert.ToDateTime(arr[7]).Year < Convert.ToInt32(dtLC.Rows[0][1].ToString())) ||
                        (Convert.ToDateTime(arr[7]).Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && Convert.ToDateTime(arr[7]).Month < Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                    {
                        msg += Dami.TraduCuvant("CM pt marca") + " " + arr[5] + " - " + Dami.TraduCuvant("este anterior lunii de calcul") + System.Environment.NewLine;
                        continue;
                    }

                    ids.Add(new metaCereriCM
                    {
                        Id = Convert.ToInt32(General.Nz(arr[0], 0)),
                        F10003 = Convert.ToInt32(General.Nz(arr[5], 0)),
                        BazaCalculCM = Convert.ToDouble(General.Nz(arr[1], 0)),
                        ZileBazaCalculCM = Convert.ToInt32(General.Nz(arr[2], 0)),
                        MedieZileBazaCalcul = Convert.ToDouble(General.Nz(arr[3], 0)),
                        MedieZilnicaCM = Convert.ToDouble(General.Nz(arr[4], 0)),
                        DataInceput = (data ?? new DateTime(2100, 1, 1))
                    });
                    nrSel++;
                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                        MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate"), MessageBox.icoWarning, "");
                    else
                        MessageBox.Show(msg, MessageBox.icoWarning, "");
                    return;
                }

                for (int i = 0; i < ids.Count; i++)
                {
                    sql = "UPDATE CM_CereriIstoric SET IdStare = -1, Aprobat = 1, DataAprobare = GETDATE(), CULOARE = (SELECT Culoare FROM CM_tblStari WHERE Id = -1), IdUser = " + Session["UserId"].ToString() + " WHERE IdCerere = " + ids[i].Id + " AND Pozitie = 2";
                    General.ExecutaNonQuery(sql, null);

                    sql = "UPDATE CM_Cereri SET IdStare = -1 WHERE Id = " + ids[i].Id;
                    General.ExecutaNonQuery(sql, null);

                    TrimiteNotificare(ids[i].Id);

                    //actualizare F300
                    DataTable dtVerif = General.IncarcaDT("SELECT COUNT(*) FROM F300 WHERE F30003 = " + ids[i].F10003 + " AND F30010 <> 4450 AND F30036 <> CONVERT(DATETIME, '" 
                        + ids[i].DataInceput.Day.ToString().PadLeft(2, '0') + "/" + ids[i].DataInceput.Month.ToString().PadLeft(2, '0') + "/" + ids[i].DataInceput.Year + "', 103) AND F30010 BETWEEN 4400 AND 4499", null);
                    if (dtVerif != null && dtVerif.Rows.Count > 0 && Convert.ToInt32(dtVerif.Rows[0][0].ToString()) > 0)
                    {
                        DataTable dtTemp = General.IncarcaDT("select sum(F30013), round(sum(F30012 * F30013 * F30014/100), 0) FROM F300 WHERE F30003 = " + +ids[i].F10003 + " AND F30010 BETWEEN 4400 AND 4499 AND F30010 <> 4450 AND F30036 = CONVERT(DATETIME, '"
                        + ids[i].DataInceput.Day.ToString().PadLeft(2, '0') + "/" + ids[i].DataInceput.Month.ToString().PadLeft(2, '0') + "/" + ids[i].DataInceput.Year + "', 103)", null);

                        int zile = Convert.ToInt32(Convert.ToDouble(dtTemp.Rows[0][0].ToString()));
                        int suma = Convert.ToInt32(Convert.ToDouble(dtTemp.Rows[0][1].ToString()));

                        sql = "UPDATE F300 SET F30013 = F30013 - " + zile + ", F30015 = F30015 - " + suma.ToString() + " WHERE F30010 = 4450 AND F30003 = " + ids[i].F10003;
                        General.ExecutaNonQuery(sql, null);

                        General.ExecutaNonQuery("DELETE FROM F300 WHERE F30003 = " + +ids[i].F10003 + " AND F30010 BETWEEN 4400 AND 4499 AND F30010 <> 4450 AND F30036 = CONVERT(DATETIME, '"
                        + ids[i].DataInceput.Day.ToString().PadLeft(2, '0') + "/" + ids[i].DataInceput.Month.ToString().PadLeft(2, '0') + "/" + ids[i].DataInceput.Year + "', 103)", null);
                    }
                    else
                    {
                        General.ExecutaNonQuery("DELETE FROM F300 WHERE F30003 = " + +ids[i].F10003 + " AND F30010 BETWEEN 4400 AND 4499", null);
                    }
                }

                Session["CM_Grid"] = null;
                IncarcaGrid();

                MessageBox.Show(msg + (msg.Length > 0 ? System.Environment.NewLine : "") + (nrSel == 1 ? Dami.TraduCuvant("S-a anulat un concediu medical!") : Dami.TraduCuvant("S-au anulat") + " " + nrSel + " " + Dami.TraduCuvant("concedii medicale cu stergere din tranzactiile angajatului") + "!"), MessageBox.icoInfo, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        bool AddConcediu(int cod, int zile, int cc, double proc, double suma_4450_subplafon, bool bFTarif, int marca, bool avans)
        {
            double suma = 0, tarif = 0;

            string sqlAng = "SELECT * FROM F100 WHERE F10003 = " + marca;
            DataTable dtAng = General.IncarcaDT(sqlAng, null);

            //ASPxComboBox cmbLocPresc = DataList1.Items[0].FindControl("cmbLocPresc") as ASPxComboBox;
            //ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            //ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
            //ASPxDateEdit deDataAviz = DataList1.Items[0].FindControl("deDataAviz") as ASPxDateEdit;
            //ASPxDateEdit deData = DataList1.Items[0].FindControl("deData") as ASPxDateEdit;
            //ASPxTextBox txtSerie = DataList1.Items[0].FindControl("txtSerie") as ASPxTextBox;
            //ASPxTextBox txtNr = DataList1.Items[0].FindControl("txtNr") as ASPxTextBox;
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtDetalii = DataList1.Items[0].FindControl("txtDetalii") as ASPxTextBox;
            //ASPxTextBox txtSCMInit = DataList1.Items[0].FindControl("txtSCMInit") as ASPxTextBox;
            //ASPxTextBox txtCodIndemn = DataList1.Items[0].FindControl("txtCodIndemn") as ASPxTextBox;
            //ASPxTextBox txtCodDiag = DataList1.Items[0].FindControl("txtCodDiag") as ASPxTextBox;
            //ASPxTextBox txtNrCMInit = DataList1.Items[0].FindControl("txtNrCMInit") as ASPxTextBox;
            //ASPxTextBox txtCodUrgenta = DataList1.Items[0].FindControl("txtCodUrgenta") as ASPxTextBox;
            //ASPxTextBox txtCodInfCont = DataList1.Items[0].FindControl("txtCodInfCont") as ASPxTextBox;
            //ASPxTextBox txtBCCM = DataList1.Items[0].FindControl("txtBCCM") as ASPxTextBox;
            //ASPxTextBox txtZBC = DataList1.Items[0].FindControl("txtZBC") as ASPxTextBox;
            //ASPxTextBox txtMZ = DataList1.Items[0].FindControl("txtMZ") as ASPxTextBox;
            //ASPxTextBox txtNrAviz = DataList1.Items[0].FindControl("txtNrAviz") as ASPxTextBox;
            //ASPxTextBox txtMedic = DataList1.Items[0].FindControl("txtMedic") as ASPxTextBox;
            //ASPxTextBox txtCNP = DataList1.Items[0].FindControl("txtCNP") as ASPxTextBox;
            //ASPxCheckBox chkCalcul = DataList1.Items[0].FindControl("chkCalcul") as ASPxCheckBox;
            //ASPxCheckBox chkStagiu = DataList1.Items[0].FindControl("chkStagiu") as ASPxCheckBox;

            DataTable dt = Session["CM_Aprobare_Detalii"] as DataTable;

            DateTime dtStart = Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString());
            DateTime dtEnd = Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString());
            DateTime dtData = Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString());
            DateTime dtAviz = Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString());
            DateTime dtDataCMInit = Convert.ToDateTime(dt.Rows[0]["DataCMInitial"].ToString());

            string detalii = "";

            string BCCM = "0", ZileBCCM = "0", MZCM = "0", MNTZ = "0";

            if (!avans || Convert.ToInt32(dt.Rows[0]["Initial"].ToString()) == 0)
            {
                BCCM = dt.Rows[0]["BazaCalculCM"].ToString();
                ZileBCCM = dt.Rows[0]["ZileBazaCalculCM"].ToString();
                MZCM = dt.Rows[0]["MedieZilnicaCM"].ToString();
                MNTZ = dt.Rows[0]["MedieZileBazaCalcul"].ToString();
            }

            if (cod == 4450)
                suma += suma_4450_subplafon;

            if (cc == 0 || cc == 9999)
            {
                if (dtAng.Rows[0]["F10053"] != null && dtAng.Rows[0]["F10053"].ToString().Length > 0 && Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString()) != 0 && Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString()) != 9999)
                    cc = Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString());
                else
                {
                    string sqlDep = "SELECT F00615 FROM F006 WHERE F00607 = " + Convert.ToInt32(dtAng.Rows[0]["F10007"].ToString());
                    DataTable dtDep = General.IncarcaDT(sqlDep, null);
                    if (dtDep != null && dtDep.Rows.Count > 0 && dtDep.Rows[0][0] != null && dtDep.Rows[0][0].ToString().Length > 0)
                        cc = Convert.ToInt32(dtDep.Rows[0][0].ToString());
                }
            }

            if (bFTarif)
                tarif = Convert.ToDouble(dt.Rows[0]["MedieZilnicaCM"].ToString());

            //if (Convert.ToInt32(MNTZ) > 0 && Convert.ToInt32(dt.Rows[0]["CodIndemnizatie"].ToString()) == 10 && sumaReducereTimpLucru > 0)
            //    suma = sumaReducereTimpLucru;

            //if (chkCalcul.Checked)
            //{
            //    detalii = "CALCUL ZILE MANUAL";
            //    if (Convert.ToInt32(dt.Rows[0]["Stagiu"] == DBNull.Value ? "0" : dt.Rows[0]["Stagiu"].ToString()) == 1)
            //    {
            //        detalii += " / NU ARE STAGIU COTIZARE";
            //        //DM.Zile5 = 0;
            //        tarif = 0;
            //        BCCM = "0";
            //        ZileBCCM = "0";
            //        MZCM = "0";
            //    }
            //}
            // else 
            if (Convert.ToInt32(dt.Rows[0]["Stagiu"] == DBNull.Value ? "0" : dt.Rows[0]["Stagiu"].ToString()) == 1)
            {
                detalii = "NU ARE STAGIU COTIZARE";
                //DM.Zile5 = 0;
                tarif = 0;
                BCCM = "0";
                ZileBCCM = "0";
                MZCM = "0";
                if (cod == 4450)
                    return true;
            }

            string cmpAvans = "", valAvans = "";
            if (avans)
            {
                cmpAvans = ", ZILE, ZILE1, ZILE2, ZILE3, ZILE5, ZILE6, TIP, OPTIUNE, F30051, F30052";
                valAvans = ", {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, 0, 0";
                valAvans = string.Format(valAvans, dt.Rows[0]["NrZile"].ToString(),
                    (Session["CM_NrZileCT1"] == null ? "0" : Session["CM_NrZileCT1"].ToString()),
                    (Session["CM_NrZileCT2"] == null ? "0" : Session["CM_NrZileCT2"].ToString()),
                    (Session["CM_NrZileCT3"] == null ? "0" : Session["CM_NrZileCT3"].ToString()),
                    (Session["CM_NrZileCT5"] == null ? "0" : Session["CM_NrZileCT5"].ToString()), /*(txtCT6.Text.Length <= 0 ? "0" : txtCT6.Text)*/ "0", dt.Rows[0]["TipConcediu"].ToString(), dt.Rows[0]["Optiune"].ToString());
            }


            if (dtAviz.Year <= 1900)
                dtAviz = new DateTime(2100, 1, 1);
            //                                                                                                                               0              1       2       3       4                               5       5       6       7
            string sql = "INSERT INTO " + (avans ? "F300_CM_AVANS" : "F300") + " (F30001, F30002, F30003, F30004, F30005, F30006, F30007, F30010, F30011, F30012, F30013, F30014, F30015, F30021, F30022, F30023, F30036, F30037, F30038, F30050, " +
                //    8        9       10       11      12                       13      5        14       15       16      17       18       19        20      21      22         23       24      25        26       31       27      30               29
                " F300601, F300602, F300603,  F30053, F300618, F30039, F30040, F30042, F30035, F300606, F300607, F300619, F300608, F300609, F300610, F300611, F300612, F300613, F300614, F300615, F300616, F300617, F300620, F300621, USER_NO " + (avans ? cmpAvans : "") + ") ";

            //string sql = "INSERT INTO CM_Cereri (Id, F10003, TipProgram, TipConcediu, CodIndemnizatie, SerieCM, NumarCM, DataCM, LocPrescriere, DataInceput, DataSfarsit, NrZile, CodDiagnostic, CodUrgenta, CodInfectoContag, Initial, ZileCMInitial, SerieCMInitial, NumarCMInitial, DataCMInitial, " +
            //         " CodTransfer1, CodTransfer2, CodTransfer3,  CodTransfer4, CodTransfer5, NrZileCT1, NrZileCT2, NrZileCT3, NrZileCT4, NrZileCT5, BazaCalculCM, ZileBazaCalculCM, MedieZileBazaCalcul, MedieZilnicaCM, NrAvizMedicExpert, DataAvizDSP, MedicCurant, CNPCopil, IdStare, Document, Urgenta, Suma, Tarif, Cod, ModifManuala, Optiune, USER_NO, TIME) ";

            sql += " SELECT 300, F10002, F10003, F10004, F10005, F10006, F10007, {0},  1, {1}, {2}, {3}, "
                + " {4}, 0, 0, 0, {5}, {5}, {6}, {7}, '{8}', '{9}', {10}, {11}, {12}, 0, 0, '{13}', {5}, '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', {20}, {21}, {22}, {23}, '{24}', '{25}', '{26}', {31}, {27}, {30}  {29}"

                + " FROM F100 WHERE F10003 = {28}";

            //sql += "VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, '{17}', '{18}', {19}, "
            //    + " {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32},  {33}, '{34}', {35}, '{36}', '{37}', {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45}, {46}, {47} )";

            //sql = string.Format(sql, dtAng.Rows[0]["F10003"].ToString(), dtAng.Rows[0]["F10004"].ToString(), dtAng.Rows[0]["F10005"].ToString(), dtAng.Rows[0]["F10006"].ToString(), dtAng.Rows[0]["F10007"].ToString(), //4
            //    cod, 1, tarif.ToString(new CultureInfo("en-US")), zile, proc.ToString(new CultureInfo("en-US")), suma.ToString(new CultureInfo("en-US")), 0, 0, 0, //13
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') +  "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)" 
            //    : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //14
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //15
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 'dd/mm/yyyy')"), cc, txtSerie.Text, txtNr.Text, //19
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 'dd/mm/yyyy')"), (txtZCMAnt.Text.Length <= 0 ? "0" : txtZCMAnt.Text), //21
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 'dd/mm/yyyy')"), 0, 0, detalii, //25
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"), txtSCMInit.Text, txtCodIndemn.Text, txtCodDiag.Text,  //29
            //    txtNrCMInit.Text, txtCodUrgenta.Text, txtCodInfCont.Text, (cmbLocPresc.SelectedItem == null ? "0" : cmbLocPresc.SelectedItem.Value.ToString()), BCCM, ZileBCCM, MZCM, txtNrAviz.Text, txtMedic.Text, (cmbCNPCopil.Value ?? "").ToString(),
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 'dd/mm/yyyy')"), (avans ? valAvans : "")); //41
            int id = -99;
            //if (Session["CM_Id"] != null)
            //{
            //    General.ExecutaNonQuery("DELETE FROM CM_Cereri WHERE Id = " + Session["CM_Id"].ToString(), null);
            //    id = Convert.ToInt32(Session["CM_Id"].ToString());
            //}
            //else if (Session["CM_Id_Nou"] != null)
            //    id = Convert.ToInt32(Session["CM_Id_Nou"].ToString());
            //else
            //    id = Dami.NextId("CM_Cereri");

            //DataTable dtF = new DataTable();
            //dtF = General.IncarcaDT("SELECT * FROM \"tblFisiere\"", null);
            //if (Session["CM_Id"] != null && dtF.Select("Tabela = 'CM_Cereri' AND Id = " + Session["CM_Id"].ToString()).Count() == 1)
            //    Session["CM_Document"] = 1;

            //sql = string.Format(sql, id, Session["MarcaCM"].ToString(), (rbProgrNorm.Checked ? "1" : "0"), Convert.ToInt32(cmbTipConcediu.Value ?? 0), txtCodIndemn.Text, txtSerie.Text, //5
            //    txtNr.Text, "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)", Convert.ToInt32(cmbLocPresc.Value ?? 0), //8
            //    "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)", "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)", //10
            //    zile, txtCodDiag.Text.Length <= 0 ? "0" : txtCodDiag.Text, txtCodUrgenta.Text.Length <= 0 ? "NULL" : txtCodUrgenta.Text, txtCodInfCont.Text.Length <= 0 ? "NULL" : txtCodInfCont.Text, (rbConcInit.Checked ? "1" : "0"), txtZCMAnt.Text.Length <= 0 ? "0" : txtZCMAnt.Text, //16
            //    txtSCMInit.Text, txtNrCMInit.Text, "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + (dtDataCMInit.Year < 1900 ? 2100 : dtDataCMInit.Year).ToString() + "', 103)", //19
            //    Convert.ToInt32(cmbCT1.Value ?? 0), Convert.ToInt32(cmbCT2.Value ?? 0), Convert.ToInt32(cmbCT3.Value ?? 0), Convert.ToInt32(cmbCT4.Value ?? 0), Convert.ToInt32(cmbCT5.Value ?? 0), //24
            //    (Session["CM_NrZileCT1"] == null ? "0" : Session["CM_NrZileCT1"].ToString()), (Session["CM_NrZileCT2"] == null ? "0" : Session["CM_NrZileCT2"].ToString()), (Session["CM_NrZileCT3"] == null ? "0" : Session["CM_NrZileCT3"].ToString()), (Session["CM_NrZileCT4"] == null ? "0" : Session["CM_NrZileCT4"].ToString()), (Session["CM_NrZileCT5"] == null ? "0" : Session["CM_NrZileCT5"].ToString()),  //29
            //    txtBCCM.Text.Length <= 0 ? "0" : txtBCCM.Text.ToString(new CultureInfo("en-US")), txtZBC.Text.Length <= 0 ? "0" : txtZBC.Text, txtMZBC.Text.Length <= 0 ? "0" : txtMZBC.Text.ToString(new CultureInfo("en-US")), txtMZ.Text.Length <= 0 ? "0" : txtMZ.Text.Replace(',', '.').ToString(new CultureInfo("en-US")), //33
            //    txtNrAviz.Text, "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + (dtAviz.Year < 1900 ? 2100 : dtAviz.Year).ToString() + "', 103)", txtMedic.Text, //36
            //    (cmbCNPCopil.Value ?? ""), (chkModMan.Checked ? 2 : 1), (Session["CM_Document"] == null ? 0 : 1), (chkUrgenta.Checked ? "1" : "0"), suma.ToString().Replace(',', '.'), tarif.ToString().Replace(',', '.'), cod, (chkModMan.Checked ? "1" : "0"), (rbOptiune1.Checked ? "1" : "2"), Convert.ToInt32(Session["UserId"].ToString()), "GETDATE()"); //47

            //                        0                         1                                    2                              3                                  4      
            sql = string.Format(sql, cod, (cod == 4450 ? "0" : tarif.ToString().Replace(',', '.')), zile, (cod == 4450 ? "0" : proc.ToString().Replace(',', '.')), suma.ToString().Replace(',', '.'),
            "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)",  //5
            "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)",  //6
            //7     8                                       9
            cc, dt.Rows[0]["SerieCM"].ToString(), dt.Rows[0]["NumarCM"].ToString(),
            "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)", //10

            dt.Rows[0]["ZileCMInitial"].ToString(), //11
            "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 103)", //12
            // 13           14                                                  15                                      16                                          17                                          18                                                                                               19                                                                                         20
             detalii, dt.Rows[0]["SerieCMInitial"].ToString(), dt.Rows[0]["CodIndemnizatie"].ToString().PadLeft(2, '0'), dt.Rows[0]["CodDiagnostic"].ToString(), dt.Rows[0]["NumarCMInitial"].ToString(), (dt.Rows[0]["CodUrgenta"] == DBNull.Value ? "" : dt.Rows[0]["CodUrgenta"].ToString()), (dt.Rows[0]["CodInfectoContag"] == DBNull.Value ? "" : dt.Rows[0]["CodInfectoContag"].ToString()), dt.Rows[0]["LocPrescriere"].ToString(),
             //21    22         23                                  24                                      25                                                  26                                                          
             BCCM.Replace(',', '.'), ZileBCCM, MZCM.Replace(',', '.'), dt.Rows[0]["NrAvizMedicExpert"].ToString(), dt.Rows[0]["MedicCurant"].ToString(), (dt.Rows[0]["CNPCopil"] == DBNull.Value ? "" : dt.Rows[0]["CNPCopil"].ToString()), 
             "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 103)" //27
            //          28                                29                              30                31
            , dt.Rows[0]["F10003"].ToString(), (avans ? valAvans : ""), Session["UserId"].ToString(), MNTZ.Replace(',', '.'));

            //verificare existenta linie cu 4450 in F300 pe marca actuala; daca exista, se actualizeaza, nu se insereaza alta noua
            if (cod == 4450)
            {
                DataTable dtVerif = General.IncarcaDT("SELECT COUNT(*) FROM F300 WHERE F30010 = 4450 AND F30003 = " + marca, null);
                if (dtVerif != null && dtVerif.Rows.Count > 0 && Convert.ToInt32(dtVerif.Rows[0][0].ToString()) > 0)
                {
                    sql = "UPDATE F300 SET F30013 = F30013 + " + zile + ", F30015 = F30015 + " + suma.ToString().Replace(',', '.') + " WHERE F30010 = 4450 AND F30003 = " + marca;
                }
            }

            if (!((Convert.ToInt32(dt.Rows[0]["Stagiu"] == DBNull.Value ? "0" : dt.Rows[0]["Stagiu"].ToString()) == 1 && (cod == 4450 || cod == 4449))))
                if (General.ExecutaNonQuery(sql, null))
                {
                    if (Convert.ToInt32(dt.Rows[0]["Stagiu"] == DBNull.Value ? "0" : dt.Rows[0]["Stagiu"].ToString()) == 1)
                    {
                        int nn = 0;

                        string sqlParam = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'BCCM'";
                        DataTable dtParam = General.IncarcaDT(sqlParam, null);
                        string sqltmp = "";
                        if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                        {
                            nn = Convert.ToInt32(dtParam.Rows[0][0].ToString()) - 1;
                            sqltmp = "UPDATE F100 SET F10069{0} = 0.0 WHERE F10003 = {1}";
                            sqltmp = string.Format(sqltmp, nn, marca);
                            if (!General.ExecutaNonQuery(sqltmp, null))
                                MessageBox.Show(Dami.TraduCuvant("Nu am putut actualiza Tariful CM pe angajat!"));
                        }
                        sqltmp = "UPDATE CM_Cereri SET  BazaCalculCM = 0, ZileBazaCalculCM = 0, MedieZileBazaCalcul = 0, MedieZilnicaCM = 0, Tarif = 0 WHERE Id = {0}";
                        sqltmp = string.Format(sqltmp, id);
                        General.ExecutaNonQuery(sqltmp, null);
                    }     
                }
                else
                {
                    //MessageBox.Show(Dami.TraduCuvant("Tranzactia nu a fost adaugata!"));
                    return false;
                }
            return true;
        }



        double rotunjire(int tip, int cantit, double nr)
        {
            if (cantit == 0)
                cantit = 1;

            nr = Math.Round(nr, 5);

            switch (tip)
            {
                case 1:
                    return (Math.Floor(Math.Floor(nr - 0.1) / cantit) + 1) * cantit;

                case 2:
                    {
                        double add = (Math.Floor(Math.Floor(nr - 0.1) / cantit) + 1) * cantit;
                        double sub = (Math.Ceiling(Math.Ceiling(nr + 0.1) / cantit) - 1) * cantit;
                        if (Math.Abs(add - nr) <= Math.Abs(sub - nr))
                            return add;
                        else
                            return sub;
                    }

                case 3:
                    return (Math.Ceiling(Math.Ceiling(nr + 0.1) / cantit) - 1) * cantit;

            }
            return cantit;
        }



        double GetMARpercent(int Table_No, double Vechime)
        {
            string sql, sql_tmp;

            sql_tmp = "SELECT  * FROM MARTABLE";
            sql_tmp += " WHERE TABLE_NO={0} AND SEN_INF<={1} AND SEN_SUP>{1} ";
            sql = string.Format(sql_tmp, Table_No, Vechime.ToString(new CultureInfo("en-US")));

            DataTable dt = General.IncarcaDT(sql, null);

            if (dt == null || dt.Rows.Count <= 0 || dt.Rows[0][0] == null)
                return -1;

            return Convert.ToDouble(dt.Rows[0]["PERCENT"].ToString());

        }
        int GetZileMed_cod_ind(int marca, DateTime di_med, string cod_ind, string CNPCopil, out int limitazile)
        {
            string sql, sql_tmp, line = "";
            int days = 0;

            DateTime ddi_med = di_med.AddYears(-1);

            if (Convert.ToInt32(cod_ind) == 1 || Convert.ToInt32(cod_ind) == 5 || Convert.ToInt32(cod_ind) == 6 || Convert.ToInt32(cod_ind) == 10)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F30037,103) AS DI, CONVERT(VARCHAR,F30038,103) AS DSF FROM F300 WHERE (F300607 = '01' OR  ";
                sql_tmp += " F300607 = '05' OR F300607 = '06' OR F300607 = '10') AND F30003 = {0} AND F30010 <> 0 ";
                sql_tmp += " AND (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca);
            }
            else if (Convert.ToInt32(cod_ind) == 12 || Convert.ToInt32(cod_ind) == 13 || Convert.ToInt32(cod_ind) == 14)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F30037,103) AS DI, CONVERT(VARCHAR,F30038,103) AS DSF FROM F300 WHERE (F300607 = '12' OR  ";
                sql_tmp += " F300607 = '13' OR F300607 = '14') AND F30003 = {0} AND F30010 <> 0 ";
                sql_tmp += " AND (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca);
            }
            else
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F30037,103) AS DI, CONVERT(VARCHAR,F30038,103) AS DSF FROM F300 WHERE F300607 = '{0}' ";
                sql_tmp += " AND F30003 = {1} AND F30010 <> 0 ";
                sql_tmp += " AND (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, cod_ind, marca);

                if (Convert.ToInt32(cod_ind) == 9)
                {
                    sql += " AND F300617 = '" + CNPCopil + "'";
                }
            }

            DataTable dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dtStart = new DateTime(Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(0, 2)));
                    DateTime dtSfarsit = new DateTime(Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(0, 2)));

                    days += (dtSfarsit - dtStart).Days + 1;
                }

            sql = " SELECT F01011, F01012 FROM F010";
            DataTable dt010 = General.IncarcaDT(sql, null);

            int an_c, luna_c, an_ant;

            an_c = Convert.ToInt32(dt010.Rows[0][0].ToString());
            luna_c = Convert.ToInt32(dt010.Rows[0][1].ToString());

            an_ant = an_c - 1;
            string data = "CONVERT(DATETIME,'" + di_med.Day.ToString().PadLeft(2, '0') + "/" + di_med.Month.ToString().PadLeft(2, '0') + "/" + di_med.Year.ToString() + "',103)";

            if (Convert.ToInt32(cod_ind) == 1 || Convert.ToInt32(cod_ind) == 5 || Convert.ToInt32(cod_ind) == 6 || Convert.ToInt32(cod_ind) == 10)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F94037,103) AS DI, CONVERT(VARCHAR,F94038,103) AS DSF FROM F940 WHERE (F940607 = '01' OR  ";
                sql_tmp += " F940607 = '05' OR F940607 = '06' OR F940607 = '10') AND F94003 = {0} AND F94010 <> 0 ";
                sql_tmp += " AND ((YEAR ={1} AND MONTH < {2}) OR (YEAR = {3} AND MONTH >= {2})) AND F94038 >= DATEADD(MONTH, -12, {4}) ";
                sql_tmp += " AND (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca, an_c, luna_c, an_ant, data);
            }
            else if (Convert.ToInt32(cod_ind) == 12 || Convert.ToInt32(cod_ind) == 13 || Convert.ToInt32(cod_ind) == 14)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F94037,103) AS DI, CONVERT(VARCHAR,F94038,103) AS DSF FROM F940 WHERE (F940607 = '12' OR  ";
                sql_tmp += " F940607 = '13' OR F940607 = '14') AND F94003 = {0} AND F94010 <> 0 ";
                sql_tmp += " AND ((YEAR ={1} AND MONTH < {2}) OR (YEAR = {3} AND MONTH >= {2})) AND F94038 >= DATEADD(MONTH, -12, {4}) ";
                sql_tmp += " AND (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca, an_c, luna_c, an_ant, data);
            }
            else
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F94037,103) AS DI, CONVERT(VARCHAR,F94038,103) AS DSF FROM F940 WHERE F940607 = '{0}' ";
                sql_tmp += " AND F94003 = {1} AND F94010 <> 0 ";
                sql_tmp += " AND ((YEAR ={2} AND MONTH < {3}) OR (YEAR = {4} AND MONTH >= {3})) AND F94038 >= DATEADD(MONTH, -12, {5}) ";
                sql_tmp += " AND (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, cod_ind, marca, an_c, luna_c, an_ant, data);

                if (Convert.ToInt32(cod_ind) == 9)
                {
                    sql += " AND F940617 = '" + CNPCopil + "'";
                }
            }

            dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dtStart = new DateTime(Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(0, 2)));
                    if (ddi_med > dtStart)
                        dtStart = ddi_med;
                    DateTime dtSfarsit = new DateTime(Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(0, 2)));

                    days += (dtSfarsit - dtStart).Days + 1;
                }

            if (Convert.ToInt32(cod_ind) == 9)
                limitazile = 45;
            else if (Convert.ToInt32(cod_ind) == 8)
                limitazile = 126;
            else if (Convert.ToInt32(cod_ind) == 15)
                limitazile = 120;
            else if (Convert.ToInt32(cod_ind) == 12 || Convert.ToInt32(cod_ind) == 13 || Convert.ToInt32(cod_ind) == 14)
                limitazile = 365;
            else if (Convert.ToInt32(cod_ind) == 1 || Convert.ToInt32(cod_ind) == 5 || Convert.ToInt32(cod_ind) == 6 || Convert.ToInt32(cod_ind) == 10)
                limitazile = 90;
            else
                limitazile = 99999;

            return days;
        }

        double ConvertVechime(string vechime)
        {
            string an, luna;

            if (vechime.Substring(0, 1) == "0")
                an = vechime.Substring(1, 1);
            else
                an = vechime.Substring(0, 2);

            luna = vechime.Substring(2, 2);
            int l = Convert.ToInt32(luna);
            int a = Convert.ToInt32(an);
            return (double)((double)l / 12.0 + (double)a);
        }

        private void TrimiteNotificare(int id)
        {
            #region  Notificare start

            string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
            int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);

            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
            {
                NotifAsync.TrimiteNotificare("ConcediiMedicale.Aprobare", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM CM_Cereri Z WHERE Id=" + id, "CM_Cereri", id, Convert.ToInt32(Session["UserId"].ToString()), marcaUser, arrParam);
            });

            #endregion
        }

        public static void TransferPontaj(string marca, DateTime dataInceput, DateTime dataSfarsit, string denScurta)
        {
            try
            {
                DateTime dtSf = dataSfarsit;

                string strSql = "";
                int idAbs = -99;
                DataTable dtAbsNomen = General.IncarcaDT("SELECT * FROM \"Ptj_tblAbsente\" WHERE \"DenumireScurta\" = '" + denScurta + "'", null);
                if (dtAbsNomen != null && dtAbsNomen.Rows.Count > 0)
                    idAbs = Convert.ToInt32(dtAbsNomen.Rows[0]["Id"].ToString());
                else
                    return;

                string sql = "DELETE FROM  \"Ptj_IstoricVal\" WHERE F10003 = " + marca + " AND  \"Ziua\" BETWEEN " + General.ToDataUniv(dataInceput.Date) + " AND " + General.ToDataUniv(dtSf.Date);
                General.ExecutaNonQuery(sql, null);

              
                string sqlIst = $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", USER_NO, TIME, ""Observatii"") 
                                        SELECT {marca}, ""Zi"", ""DenumireScurta"", (SELECT ""ValStr"" FROM ""Ptj_Intrari"" WHERE F10003 = {marca} AND ""Ziua"" = ""Zi""), 
                                {HttpContext.Current.Session["UserId"].ToString() }, {General.CurrentDate()}, {HttpContext.Current.Session["UserId"].ToString() }, {General.CurrentDate()}, 'Transfer din Aprobare CM'
                                    FROM 
                                    (select case when (SELECT count(*) FROM ""Ptj_Intrari"" WHERE f10003 = {marca} and ""Ziua"" = a.""Zi"") = 0 then 0 else 1 end as prezenta, 
                                    b.* from  ""tblZile"" a
                                    left join 
                                    (SELECT P.""Zi"",
                                    CASE WHEN COALESCE(b.SL,0) <> 0 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) = 1 THEN 1 ELSE
                                    CASE WHEN COALESCE(b.ZL,0) <> 0 AND P.""ZiSapt"" < 6 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) <> 1 THEN 1 ELSE 
                                    CASE WHEN COALESCE(b.S,0) <> 0 AND P.""ZiSapt"" = 6 THEN 1 ELSE 
                                    CASE WHEN COALESCE(b.D,0) <> 0 AND P.""ZiSapt"" = 7 THEN 1 ELSE 0 
                                    END
                                    END
                                    END
                                    END AS ""AreDrepturi"", ""DenumireScurta""
                                    FROM ""tblZile"" P
                                    INNER JOIN ""Ptj_tblAbsente"" A ON 1=1
                                    INNER JOIN ""Ptj_ContracteAbsente"" B ON A.""Id"" = B.""IdAbsenta""
                                    LEFT JOIN HOLIDAYS D on P.""Zi""=D.DAY
                                    WHERE { General.ToDataUniv(dataInceput.Date)} <= CAST(P.""Zi"" AS date) AND CAST(P.""Zi"" AS date) <=  {General.ToDataUniv(dtSf.Date)}
                                    AND A.""Id"" = {idAbs}
                                    AND COALESCE(A.""DenumireScurta"", '~') <> '~'
                                    AND B.""IdContract"" = (SELECT MAX(""IdContract"") FROM ""F100Contracte"" WHERE F10003 = {marca} AND ""DataInceput"" <= { General.ToDataUniv(dataInceput.Date)} AND {General.ToDataUniv(dtSf.Date)} <= ""DataSfarsit"") 
                                    ) b
                                    on a.""Zi"" = b.""Zi""

                                    where a.""Zi"" between  { General.ToDataUniv(dataInceput.Date)} AND    {General.ToDataUniv(dtSf.Date)} and aredrepturi = 1) x   ";

                General.ExecutaNonQuery(sqlIst, null);

                string campuri = "";
                for (int i = 0; i <= 20; i++)
                    campuri += ", \"Val" + i.ToString() + "\" = NULL";
                for (int i = 1; i <= 60; i++)
                    campuri += ", F" + i.ToString() + " = NULL";

                strSql = $@"MERGE INTO ""Ptj_intrari"" USING 
                            (Select  case when ""AreDrepturi"" = 1 then ""DenumireScurta"" else null end  as ""Denumire"", x.* from                                
                            (select {marca} as F10003, case when (SELECT count(*) FROM ""Ptj_Intrari"" WHERE f10003 = {marca} and ""Ziua"" = a.""Zi"") = 0 then 0 else 1 end as prezenta, 
                            b.* from  ""tblZile"" a
                            left join 

                            (SELECT P.""Zi"", P.""ZiSapt"", CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",
                            CASE WHEN P.""ZiSapt""=6 OR P.""ZiSapt""=7 OR D.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLibera"", 
                            CASE WHEN COALESCE(b.SL,0) <> 0 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) = 1 THEN 1 ELSE
                            CASE WHEN COALESCE(b.ZL,0) <> 0 AND P.""ZiSapt"" < 6 AND (CASE WHEN D.DAY IS NOT NULL THEN 1 ELSE 0 END) <> 1 THEN 1 ELSE 
                            CASE WHEN COALESCE(b.S,0) <> 0 AND P.""ZiSapt"" = 6 THEN 1 ELSE 
                            CASE WHEN COALESCE(b.D,0) <> 0 AND P.""ZiSapt"" = 7 THEN 1 ELSE 0 
                            END
                            END
                            END
                            END AS ""AreDrepturi"", ""DenumireScurta""
                            FROM ""tblZile"" P
                            INNER JOIN ""Ptj_tblAbsente"" A ON 1=1
                            INNER JOIN ""Ptj_ContracteAbsente"" B ON A.""Id"" = B.""IdAbsenta""
                            LEFT JOIN HOLIDAYS D on P.""Zi""=D.DAY
                            WHERE {General.ToDataUniv(dataInceput.Date)} <= CAST(P.""Zi"" AS date) AND CAST(P.""Zi"" AS date) <= {General.ToDataUniv(dtSf.Date)}
                            AND A.""Id"" = {idAbs}
                            AND COALESCE(A.""DenumireScurta"", '~') <> '~'
                            AND B.""IdContract"" = (SELECT MAX(""IdContract"") FROM ""F100Contracte"" WHERE F10003 =  {marca} AND ""DataInceput"" <= {General.ToDataUniv(dataInceput.Date)} AND {General.ToDataUniv(dtSf.Date)} <= ""DataSfarsit"") 
                            ) b
                            on a.""Zi"" = b.""Zi""

                            where a.""Zi"" between  {General.ToDataUniv(dataInceput.Date)} AND    {General.ToDataUniv(dtSf.Date)} ) x

                            ) Tmp 
                            ON (""Ptj_Intrari"".""Ziua"" = ""Zi"" AND ""Ptj_Intrari"".F10003 = Tmp.F10003 and prezenta = 1) 
                            WHEN MATCHED THEN UPDATE SET ""ValStr"" = ""Denumire"" {campuri} , USER_NO ={HttpContext.Current.Session["UserId"].ToString()}, TIME = {General.CurrentDate()}
                            WHEN NOT MATCHED THEN INSERT (F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""ZiLiberaLegala"", ""IdContract"", ""Norma"", F10002, F10004, F10005, F10006, F10007, F06204, ""ValStr"", USER_NO, TIME)
                             VALUES ({marca}, ""Zi"", ""ZiSapt"" ,""ZiLibera"" , ""ZiLiberaLegala"", 
                            (SELECT X.""IdContract"" FROM ""F100Contracte"" X WHERE X.F10003 = {marca} AND X.""DataInceput"" <= ""Zi"" AND ""Zi"" <= X.""DataSfarsit""), 
                            (SELECT F10043 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10002 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10004 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10005 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10006 FROM F100 WHERE F10003 = {marca}), 
                             (SELECT F10007 FROM F100 WHERE F10003 = {marca}), 
                             -1,  ""Denumire"", {HttpContext.Current.Session["UserId"].ToString() }, {General.CurrentDate()});";
                General.ExecutaNonQuery(strSql, null);  
               
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Aprobare", "TransferPontaj");
            }
        }
    }
}