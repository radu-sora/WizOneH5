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
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ConcediiMedicale
{
    public partial class Aprobare : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

       

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnPrint.Text = Dami.TraduCuvant("btnPrint", "Print CM");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnTransfera.Text = Dami.TraduCuvant("btnTransfera", "Transfera");
                btnAdauga.Text = Dami.TraduCuvant("btnAdauga", "Adauga CM");

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");               
                
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

                int tip = Convert.ToInt32(General.Nz(Request["tip"], 1));
                if (tip == 1)
                {
                    btnPrint.ClientVisible = false;
                    btnAproba.ClientVisible = false;
                    btnTransfera.ClientVisible = false;
                }

                txtTitlu.Text = General.VarSession("Titlu").ToString();    

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""CM_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;  
 

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

                if (!IsPostBack)
                    Session["CM_Grid"] = null;

                IncarcaGrid();


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
                //Session["FormDetaliu_IdFormular"] = Convert.ToInt32(cmbForm.Items[cmbForm.SelectedIndex].Value.ToString());
                //Session["FormDetaliu_IdStare"] = 0;
                //Session["FormDetaliu_PoateModifica"] = 1;
                //Session["FormDetaliu_EsteNou"] = 1;
                //Session["FormDetaliu_Pozitie"] = 1;

                //Session["FormDetaliu_NumeFormular"] = cmbForm.Text;
                //Session["FormDetaliu_DataVigoare"] = DateTime.Now;

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

        protected void btnPrint_Click(object sender, EventArgs e)
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

        protected void btnTransfera_Click(object sender, EventArgs e)
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

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            { 

                grDate.KeyFieldName = "IdAuto";

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
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Insuficienti parametrii");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
                                MetodeCereri(3,1);
                                IncarcaGrid();
                            }
                            break;
                        case "btnEdit":
                            Session["CM_Id"] = arr[1];
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

         


        private void MetodeCereri(int tipActiune, int tipMsg = 0)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            //actiune  3  - anulare 

            try
            {
                int nrSel = 0;
                string ids = "", idsAtr = "", lstDataModif = "", lstMarci = "";
                string comentarii = "";
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdAtribut", "NumeAngajat", "DataModif", "F10003", "Revisal", "Actualizat", "PoateModifica", "ValoareNoua", "Semnat" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    if (tipMsg == 0)
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    else
                        grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[4] as DateTime?;
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:                            
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                            continue; 
                        case 4:
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    //Florin 2019.03.26
                    //Nu se poate anula o cerere daca este trimisa in revisal
                    if (tipActiune == 3 && Convert.ToInt32(General.Nz(arr[6],0)) == 1)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Cererea este trimisa in Revisal") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2020.08.04
                    //Nu se poate respinge sau anula o cerere pt program de lucru daca este trimisa in status semnat
                    if ((tipActiune == 2 || tipActiune == 3) && (Convert.ToInt32(General.Nz(arr[2], 0)) == 34 || Convert.ToInt32(General.Nz(arr[2], 0)) == 35) && Convert.ToInt32(General.Nz(arr[10], 0)) == 1)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Cererea este trimisa la semnat") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2019.05.23
                    //Nu se poate respinge sau anula o cerere daca este actualizata in F100
                    if ((tipActiune == 2 || tipActiune == 3) && Convert.ToInt32(General.Nz(arr[7], 0)) == 1)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Datele au fost trimise in personal") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2019.05.28
                    //daca este aprobare sau respingere si nu are dreptul de a modifica, il blocam; aici, implicit, se respecta ordinea
                    //algoritmul de modificare se calculeaza in selectul cu care se populeaza grodul
                    //daca este HR are voie, daca 'Fara Rol' si userul logat este la pozitie care trebuie sa aprobe poate modifica, daca rolul selectat di combo box si userul logat este acelasi cu idSuper si user-ul de la pozitia care trebuie sa aprobe poate modifica
                    if ((tipActiune == 1 || tipActiune == 2) && Convert.ToInt32(General.Nz(arr[8], 0)) == 0)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Nu este randul dvs.") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2020.05.13
                    if (tipActiune == 3 && (Convert.ToInt32(General.Nz(arr[2],0)) == Convert.ToInt32(Constante.Atribute.Suspendare) || Convert.ToInt32(General.Nz(arr[2], 0)) == Convert.ToInt32(Constante.Atribute.RevenireSuspendare)))
                    {
                        int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar(
                            @"SELECT COUNT(*)
                            FROM ""Avs_Cereri"" B
                            INNER JOIN F111 A ON A.F11103 = B.F10003 AND A.F11104 = B.""IdAtribut"" AND CAST(A.F11105 AS DATE) = CAST(B.""DataInceputSusp"" AS DATE)
                            AND CAST(A.F11106 AS DATE) = CAST(B.""DataSfEstimSusp"" AS DATE) AND CAST(A.F11107 AS DATE) = CAST(B.""DataIncetareSusp"" AS DATE)
                            WHERE B.""Id""=706", new object[] { arr[0] }),0));
                        if (cnt == 1)
                        {
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Datele au fost actualizate in personal") + System.Environment.NewLine;
                            continue;
                        }
                    }

           

                    

                    nrSel++;

                }


                //Florin 2019.03.04

                //grDate.JSProperties["cpAlertMessage"] = msg;
                //grDate.DataBind();

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                    {
                        if (tipMsg == 0)
                            MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                        else
                            grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";

                    }
                    else
                    {
                        if (tipMsg == 0)
                            MessageBox.Show(msg, MessageBox.icoWarning, "");
                        else
                            grDate.JSProperties["cpAlertMessage"] = msg;
                    }
                    return;
                }

                msg = msg + AprobaCerere(Convert.ToInt32(Session["UserId"].ToString()), ids, idsAtr, lstDataModif, lstMarci, nrSel, tipActiune, General.ListaCuloareValoare()[5], false, comentarii, Convert.ToInt32(Session["User_Marca"].ToString()));
                //grDate.JSProperties["cpAlertMessage"] = msg;
                //Session["Avs_Grid"] = null;
                if (tipMsg == 0)
                    MessageBox.Show(msg, MessageBox.icoWarning, "");
                else
                    grDate.JSProperties["cpAlertMessage"] = msg;

                IncarcaGrid();

                grDate.Selection.UnselectAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string AprobaCerere(int idUser, string ids, string idsAtr, string lstDataModif, string lstMarci, int total, int actiune, string culoareValoare, bool HR, string comentarii, int f10003)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            //actiune  3  - anulare 


            string msg = "";
            string msgValid = "";

            try
            {
                if (ids == "") return "Nu exista cereri pentru aceasta actiune !";

                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                //if (("," + idHR + ",").IndexOf("," + General.Nz(cmbRol.Value, -99).ToString() + ",") >= 0)
                //    HR = true;

                int nr = 0;
                string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrAtr = idsAtr.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrDataModif = lstDataModif.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrMarci = lstMarci.Split(new string[] { ";" }, StringSplitOptions.None);

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i] != "")
                    {
                        int id = -99;

                        try
                        {
                            id = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (id != -99)
                        {
                            string sql = "SELECT * FROM \"Avs_Cereri\" WHERE \"Id\" = " + id.ToString();
                            DataTable dtCer = General.IncarcaDT(sql, null);
                            sql = "SELECT * FROM \"Avs_CereriIstoric\" WHERE \"Id\" = " + id.ToString() + (!HR ? " AND \"IdUser\" = " + idUser.ToString() : "") 
                                + (actiune == 1 || actiune == 2 ? " AND \"Aprobat\" IS NULL" : " AND (\"Aprobat\" IS NULL  OR (\"Aprobat\" = 1 AND \"Pozitie\" = " + dtCer.Rows[0]["TotalCircuit"].ToString() + "))");
                            DataTable dtCerIst = General.IncarcaDT(sql, null);

                            if (dtCerIst == null || dtCerIst.Rows.Count == 0) continue;


                            sql = "SELECT * FROM \"Avs_Circuit\" WHERE \"IdAtribut\" = " + arrAtr[i];
                            DataTable dtCir = General.IncarcaDT(sql, null);


                            //Florin 2019.03.25
                            //Nu exista campul RespectaOrdinea

                            //if (dtCir != null && dtCir.Rows.Count > 0)
                            //{
                            //    if (!HR && dtCir.Rows[0]["RespectaOrdinea"] != null && Convert.ToInt32(dtCir.Rows[0]["RespectaOrdinea"].ToString()) == 1)
                            //        if (Convert.ToInt32(General.Nz(dtCerIst.Rows[0]["Pozitie"], 0)) != (Convert.ToInt32(General.Nz(dtCer.Rows[0]["Pozitie"], 0)) + 1))
                            //            continue;
                            //}
                            //else
                            //    continue;

                            int idStare = 2;

                            //Florin 2019.05.28
                            //if (HR)
                            //    idStare = 3;
                            //else
                            //    if (idStare == 2 && dtCer.Rows[0]["TotalCircuit"].ToString() == dtCerIst.Rows[0]["Pozitie"].ToString()) idStare = 3;

                            if (idStare == 2 && dtCer.Rows[0]["TotalCircuit"].ToString() == dtCerIst.Rows[0]["Pozitie"].ToString()) idStare = 3;


                            if (actiune == 2)
                                idStare = 0;

                            if (actiune == 3)
                            {
                                idStare = -1;
                                sql = "DELETE FROM F704 WHERE F70403 = " + arrMarci[i] + " AND F70404 = " + arrAtr[i] + " AND F70406 = "
                                          + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + arrDataModif[i] + "', 103) " : "TO_DATE('" + arrDataModif[i] + "', 'dd/mm/yyyy')"); 
                                General.ExecutaNonQuery(sql, null);
                            }

                            string culoare = "";
                            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString();
                            DataTable dtCul = General.IncarcaDT(sql, null);
                            if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != null && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                                culoare = dtCul.Rows[0]["Culoare"].ToString();
                            else
                                culoare = "#FFFFFFFF";

                            sql = "UPDATE \"Avs_Cereri\" SET \"Pozitie\" = " + dtCerIst.Rows[0]["Pozitie"].ToString() + ", \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare 
                                + "' WHERE \"Id\" = " + id.ToString();
                            General.IncarcaDT(sql, null);

                            if (actiune == 1 || actiune == 2)
                            {
                                //Florin 2019.05.28
                                //sql = "UPDATE \"Avs_CereriIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                //    + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                //    + (dtCerIst.Rows[0]["IdUser"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
                                //    + " WHERE \"Id\" = " + id.ToString() + " AND \"IdUSer\"=" + Session["UserId"];
                                sql = "UPDATE \"Avs_CereriIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                + (dtCerIst.Rows[0]["IdUser"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
                                + " WHERE \"Id\" = " + id.ToString() + " AND \"Pozitie\"=" + dtCerIst.Rows[0]["Pozitie"].ToString();
                            }
                            else
                            {
                                //Florin 2019.06.03
                                //daca anuleaza, introducem o linie noua cu anulat
                                //sql = $@"INSERT INTO ""Avs_CereriIstoric"" (""Id"", ""IdCircuit"", ""IdUser"", ""IdStare"", ""Pozitie"", ""Culoare"", ""Aprobat"", ""DataAprobare"", ""Inlocuitor"", ""IdSuper"")
                                //        VALUES ({id}, {dtCerIst.Rows[0]["IdCircuit"]}, {Session["UserId"]}, -1, 22, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1), 1, {General.CurrentDate()}, null, {-1 * Convert.ToInt32(General.Nz(cmbRol.Value,0))})";
                            }
                            General.IncarcaDT(sql, null);


                            #region Validare start

                            //string corpMesaj = "";
                            //bool stop;

                            //srvNotif ctxNtf = new srvNotif();
                            //ctxNtf.ValidareRegula("Avs.Aprobare", "grDate", entCer, idUser, f10003, out corpMesaj, out stop);

                            //if (corpMesaj != "")
                            //{
                            //    msgValid += corpMesaj + "\r\n";
                            //    if (stop)
                            //    {
                            //        continue;
                            //    }
                            //}

                            #endregion

                            //ctx.SaveChanges();



                            if (actiune == 3)
                            {
                                try
                                {
                                    General.ExecutaNonQuery(
                                        $@"BEGIN
                                        DELETE FROM ""Admin_NrActAd"" WHERE ""IdAuto""=(SELECT ""IdActAd"" FROM ""Avs_Cereri"" WHERE ""ID""=@1);
                                        UPDATE ""Avs_Cereri"" SET ""IdActAd"" = null WHERE ""Id""=@1;
                                        END", new object[] { id });


                                    //UPDATE ""Admin_NrActAd"" SET ""DocNr""=null, ""DocData""=null, ""Tiparit""=0, ""Semnat""=0, ""Revisal""=0 WHERE ""IdAuto""=(SELECT ""IdActAd"" FROM ""Avs_Cereri"" WHERE ""ID""=@1);
                                }
                                catch (Exception){}
                            }

                            #region  Notificare start

                            //ctxNtf.TrimiteNotificare("Avs.Aprobare", "grDate", entCer, idUser, f10003);

                            #endregion

                            nr++;
                            
                        }
                    }
                }

                if (nr > 0)
                {
                    if (actiune == 1)
                        msg = "S-au aprobat " + nr.ToString() + " cereri din " + total + " !";
                    else
                        msg = "S-au respins " + nr.ToString() + " cereri din " + total + " !";

                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
                }
                else
                {
                    if (msgValid != "")
                        msg = msgValid;
                    else
                        msg = "Nu exista cereri pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

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
                        FROM ""Avs_CereriIstoric"" B
                        INNER JOIN ""Avs_Cereri"" C ON B.""Id"" = C.""Id""
                        INNER JOIN F100 A ON A.F10003 = C.F10003
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" FF on C.f10003 = FF.f10003 and(-1 * B.""IdSuper"") = FF.IdSuper
                        WHERE case when B.""IdSuper"" >= 0 then  B.""IdUser"" else FF.IdUser end = {Session["UserId"]}
                        union
                        SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament""
                        FROM ""Avs_CereriIstoric"" B
                        INNER JOIN ""Avs_Cereri"" C ON B.""Id"" = C.""Id""
                        INNER JOIN F100 A ON A.F10003 = C.F10003
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" GG on C.f10003 = GG.f10003 and CHARINDEX(',' + CONVERT(nvarchar(20),GG.""IdSuper"") + ','  ,  ',' + (SELECT Valoare FROM tblParametrii WHERE Nume='Avans_IDuriRoluriHR') + ',') > 0                     
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

            try
            {
                string strSql = "";
                string filtru = "";


                if (Convert.ToInt32(cmbAngFiltru.Value ?? -99) != -99) filtru += " AND a.F10003 = " + Convert.ToInt32(cmbAngFiltru.Value ?? -99);

                //daca este rol de hr aratam toate cererile
                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");

                strSql = "SELECT * FROM CM_Cereri";

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }




    }
}