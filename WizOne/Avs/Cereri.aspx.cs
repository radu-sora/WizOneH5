using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;
using System.Web;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using System.Globalization;
using DevExpress.Utils;
using System.Numerics;
//using Resources;

namespace WizOne.Avs
{
    public partial class Cereri : System.Web.UI.Page
    {
        int F10003 = -99;

        public class metaCereriDate
        {   
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;
         
                if (!IsPostBack)
                {
                    Session["AvsCereri"] = null;
                    Session["AvsCereriCalcul"] = null;
                    Session["Avs_Cereri_Date"] = null;
                    Session["Avs_ChkEnabled"] = null;
                    Session["Avs_ChkGen"] = null;

                    Session["Avs_NrLuni"] = "";
                    Session["Avs_NrZile"] = "";

                    if (Session["MP_Avans"] == null)
                    {
                        btnBack.Visible = false;
                        Session["Marca_atribut"] = null;
                    }
                    else
                        Session["MP_Avans"] = null;

                    txtDataMod.Date = DateTime.Now;
                    lblDataRevisal.Visible = false;
                    deDataRevisal.Visible = false;

                    DataTable dtAng = General.IncarcaDT(SelectAngajati(-44), null, "F10003;Rol");

                    DataView view = new DataView(dtAng);
                    DataTable dtRol = view.ToTable(true, "Rol", "RolDenumire");

                    switch (dtRol.Rows.Count)
                    {
                        case 0:
                            divRol.Visible = false;
                            cmbRol.Value = 0;
                            break;
                        case 1:
                            divRol.Visible = false;
                            cmbRol.DataSource = dtRol;
                            cmbRol.DataBind();
                            cmbRol.SelectedIndex = 0;

                            //if (Convert.ToInt32(General.Nz(cmbRol.Value, 0)) != 0)
                            //{
                            //    cmbAng.Buttons.Add(new EditButton { Text = "Activi" });
                            //    cmbAng.Buttons.Add(new EditButton { Text = "Toti" });
                            //}
                            break;
                        default:
                            divRol.Visible = true;
                            cmbRol.DataSource = dtRol;
                            cmbRol.DataBind();
                            cmbRol.SelectedIndex = 0;
                            cmbAng.SelectedIndex = 0;

                            //cmbAng.Buttons.Add(new EditButton { Text = "Activi" });
                            //cmbAng.Buttons.Add(new EditButton { Text = "Toti" });

                            break;
                    }

                    DataTable dtAngFiltrati = dtAng;
                    if (cmbRol.Value != null && Convert.ToInt32(cmbRol.Value) != -44 && dtAng != null && dtAng.Rows.Count > 0) dtAngFiltrati = dtAng.Select("Rol=" + cmbRol.Value).CopyToDataTable();

                    cmbAng.DataSource = dtAngFiltrati;
                    cmbAngFiltru.DataSource = dtAngFiltrati;
                    Session["Modif_Avans_Angajati"] = dtAngFiltrati;
                    cmbAng.DataBind();
                    cmbAngFiltru.DataBind();

                    //cmbAng.DataSource = dtAng;
                    //cmbAngFiltru.DataSource = dtAng;
                    //Session["Modif_Avans_Angajati"] = dtAng;
                    //cmbAng.DataBind();
                    //cmbAngFiltru.DataBind();
                    cmbAng.SelectedIndex = -1;
                    cmbAngFiltru.SelectedIndex = -1;

                    //DataTable dtAtr = General.IncarcaDT(
                    //    $@"SELECT A.Id, A.Denumire 
                    //    FROM Avs_tblAtribute A
                    //    INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
                    //    INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper OR (-1 * B.Super2) = C.IdSuper OR (-1 * B.Super3) = C.IdSuper OR (-1 * B.Super4) = C.IdSuper OR (-1 * B.Super5) = C.IdSuper OR (-1 * B.Super6) = C.IdSuper OR (-1 * B.Super7) = C.IdSuper OR (-1 * B.Super8) = C.IdSuper OR (-1 * B.Super9) = C.IdSuper OR (-1 * B.Super10) = C.IdSuper
                    //    WHERE C.IdUser=@1 AND C.IdSuper=@2
                    //    GROUP BY A.Id, A.Denumire
                    //    ORDER BY A.Denumire", new object[] { Session["UserId"], General.Nz(cmbRol.Value,-99) });



                    //Florin 2019.08.12

                    //if (Session["Marca_atribut"] != null)
                    //{
                    //    string[] param = Session["Marca_atribut"].ToString().Split(';');
                    //    F10003 = Convert.ToInt32(param[0]);
                    //    for (int i = 0; i < cmbAng.Items.Count; i++)
                    //        if (cmbAng.Items[i].Value.ToString() == param[0])
                    //        {
                    //            cmbAng.SelectedIndex = i;
                    //            cmbAngFiltru.SelectedIndex = i;
                    //            break;
                    //        }
                    //    cmbAng.Enabled = false;
                    //    cmbAngFiltru.Enabled = false;
                    //    for (int i = 0; i < cmbAtribute.Items.Count; i++)
                    //        if (Convert.ToInt32(cmbAtribute.Items[i].Value) == Convert.ToInt32(param[1]))
                    //        {
                    //            cmbAtribute.SelectedIndex = i;
                    //            break;
                    //        }
                    //    cmbAtribute.Enabled = false;
                    //}

                    if (General.Nz(Session["Marca_atribut"],"").ToString() != "")
                    {
                        string[] arr = Session["Marca_atribut"].ToString().Split(';');
                        if (arr.Length == 3 && arr[0].ToString() != "" && arr[1].ToString() != "" && arr[2].ToString() != "" && General.IsNumeric(arr[0]) && General.IsNumeric(arr[1]))
                        {
                            cmbAng.Value = Convert.ToInt32(arr[0]);
                            cmbAngFiltru.Value = Convert.ToInt32(arr[0]);
                            cmbAtribute.Value = Convert.ToInt32(arr[1]);

                            cmbAng.Enabled = false;
                            cmbAngFiltru.Enabled = false;
                            cmbAtribute.Enabled = false;

                            string[] arrRol = arr[2].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrRol.Length > 0)
                                cmbRol.Items.Clear();

                            for(int i = 0; i < arrRol.Length; i++)
                            {
                                string[] lst = arrRol[i].Split('=');
                                if (lst.Length == 2)
                                    cmbRol.Items.Add(new ListEditItem() { Text = General.Nz(lst[1], "").ToString(), Value = -1 * Convert.ToInt32(General.Nz(lst[0], -99)) });  
                            }
                            cmbRol.SelectedIndex = 0;
                            if (cmbRol.Items.Count == 1)
                                cmbRol.Enabled = false;

                            if (Convert.ToInt32(arr[1]) == (int)Constante.Atribute.Functie || Convert.ToInt32(arr[1]) == (int)Constante.Atribute.CodCOR)
                            {
                                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                                Session["Avs_DurCtr"] = ds.Tables[0].Rows[0]["F1009741"].ToString();
                                Session["Avs_NrLuni"] = ds.Tables[0].Rows[0]["F100935"].ToString();
                                Session["Avs_NrZile"] = ds.Tables[0].Rows[0]["F100936"].ToString();
                                Session["Avs_GrdInv"] = ds.Tables[0].Rows[0]["F10027"].ToString();
                            }
                        }
                    }


                    DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAng.Value, -99) });
                    cmbAtribute.DataSource = dtAtr;
                    cmbAtribute.DataBind();
                    //cmbAtributeFiltru.DataSource = dtAtr;
                    //cmbAtributeFiltru.DataBind();


                    AscundeCtl();
                    IncarcaDate();

                    DataTable dtNvlFunc = General.IncarcaDT("SELECT \"Id\", \"NrZileLucrProba\", \"NrZileCalProba\", \"NrZileDemisie\", \"NrZileConcediere\", \"Conducere\" FROM \"tblNivelFunctie\"", null);
                    string nvlFunc = "";
                    for (int i = 0; i < dtNvlFunc.Rows.Count; i++)
                    {
                        nvlFunc += dtNvlFunc.Rows[i]["Id"].ToString() + "," + (dtNvlFunc.Rows[i]["NrZileLucrProba"] as int? ?? 0).ToString() + "," + (dtNvlFunc.Rows[i]["NrZileCalProba"] as int? ?? 0).ToString() + ","
                            + (dtNvlFunc.Rows[i]["NrZileDemisie"] as int? ?? 0).ToString() + "," + (dtNvlFunc.Rows[i]["NrZileConcediere"] as int? ?? 0).ToString() + "," + (dtNvlFunc.Rows[i]["Conducere"] as int? ?? 0).ToString();
                        if (i < dtNvlFunc.Rows.Count - 1)
                            nvlFunc += ";";
                    }
                    Session["Avs_NvlFunc"] = nvlFunc;


                }
                else
                {
                    if (Session["Avs_MarcaFiltru1"] != null)
                    {
                        DataTable dtAtrF = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), Convert.ToInt32(Session["Avs_MarcaFiltru1"].ToString()) });
                        cmbAtributeFiltru.DataSource = dtAtrF;
                        cmbAtributeFiltru.DataBind();
                    } 

                    DataTable dt = Session["Avs_Grid"] as DataTable;
                    grDate.KeyFieldName = "Id";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                    if (IsCallback)
                    {
                        cmbAng.DataSource = null;
                        cmbAng.Items.Clear();
                        cmbAng.DataSource = Session["Modif_Avans_Angajati"];
                        cmbAng.DataBind();

                        cmbAngFiltru.DataSource = null;
                        cmbAngFiltru.Items.Clear();
                        cmbAngFiltru.DataSource = Session["Modif_Avans_Angajati"];
                        cmbAngFiltru.DataBind();

                        //DataTable dtAtr = General.IncarcaDT($@"SELECT A.Id, A.Denumire 
                        //FROM Avs_tblAtribute A
                        //INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
                        //INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper OR (-1 * B.Super2) = C.IdSuper OR (-1 * B.Super3) = C.IdSuper OR (-1 * B.Super4) = C.IdSuper OR (-1 * B.Super5) = C.IdSuper OR (-1 * B.Super6) = C.IdSuper OR (-1 * B.Super7) = C.IdSuper OR (-1 * B.Super8) = C.IdSuper OR (-1 * B.Super9) = C.IdSuper OR (-1 * B.Super10) = C.IdSuper
                        //WHERE C.IdUser=@1 AND C.IdSuper=@2
                        //GROUP BY A.Id, A.Denumire
                        //ORDER BY A.Denumire", new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99) });

                        DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAng.Value, -99) });
                        cmbAtribute.DataSource = dtAtr;
                        cmbAtribute.DataBind();
                        //cmbAtributeFiltru.DataSource = dtAtr;
                        //cmbAtributeFiltru.DataBind();

                        if (Session["Marca_atribut"] != null)
                        {
                            string[] param = Session["Marca_atribut"].ToString().Split(';');
                            F10003 = Convert.ToInt32(param[0]);
                        }

                        if (cmbAng.Value != null && cmbAtribute.Value != null)
                        {
                            F10003 = Convert.ToInt32(cmbAng.Value.ToString());
                            IncarcaDate();
                        }

                        if (Session["Avs_MarcaFiltru"] != null)
                        {
                            if (cmbAtributeFiltru.SelectedIndex < 0 && Convert.ToInt32(Session["Avs_AtributFiltru"].ToString()) != -1)
                                cmbAngFiltru.SelectedIndex = Convert.ToInt32(Session["Avs_MarcaFiltru"].ToString());
                            if (Session["Avs_AtributFiltru"] != null && cmbAtributeFiltru.SelectedIndex < 0 && Convert.ToInt32(Session["Avs_AtributFiltru"].ToString()) != -1)
                                cmbAtributeFiltru.SelectedIndex = Convert.ToInt32(Session["Avs_AtributFiltru"].ToString());
                            IncarcaGrid();
                        }

                        metaCereriDate itm = new metaCereriDate();
                        if (Session["Avs_Cereri_Date"] != null)
                        {
                            itm = Session["Avs_Cereri_Date"] as metaCereriDate;
                            lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();
                        }
                    }
                    if (Session["Avs_ChkGen"] != null )
                    {
                        if (Session["Avs_ChkGen"].ToString() == "false")
                        {
                            chkGen.ClientVisible = false;
                        }
                        else
                        {
                            chkGen.ClientVisible = false;
                        }
                    }

                    if (Session["Avs_ChkEnabled"] != null)
                    {
                        if (Session["Avs_ChkEnabled"].ToString() == "false")
                        {                     
                            chk2.ClientEnabled = false;
                            chk3.ClientEnabled = false;
                            chk4.ClientEnabled = false;
                            chk5.ClientEnabled = false;
                        }
                        else
                        {
                            chk2.ClientEnabled = true;
                            chk3.ClientEnabled = true;
                            chk4.ClientEnabled = true;
                            chk5.ClientEnabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Dami.AccesApp();

        //        #region Traducere
        //        string ctlPost = Request.Params["__EVENTTARGET"];
        //        //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

        //        #endregion

        //        btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
        //        btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
        //        btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

        //        DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
        //        GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
        //        colStari.PropertiesComboBox.DataSource = dtStari;

        //        var ert = cmb1Nou.Value;

        //        if (!IsPostBack)
        //        {
        //            if (Session["MP_Avans"] == null)
        //            {
        //                btnBack.Visible = false;
        //                Session["Marca_atribut"] = null;
        //            }
        //            else
        //                Session["MP_Avans"] = null;

        //            txtDataMod.Date = DateTime.Now;
        //            lblDataRevisal.Visible = false;
        //            deDataRevisal.Visible = false;

        //            DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
        //            cmbAng.DataSource = dtAng;
        //            cmbAngFiltru.DataSource = dtAng;
        //            Session["Modif_Avans_Angajati"] = dtAng;
        //            cmbAng.DataBind();
        //            cmbAng.SelectedIndex = -1;
        //            cmbAngFiltru.DataBind();
        //            cmbAngFiltru.SelectedIndex = -1;

        //            //Florin 2019.05.20
        //            //DataTable dtAtr = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"Avs_tblAtribute\" ORDER BY \"Id\"", null);
        //            DataTable dtAtr = General.IncarcaDT(
        //                $@"SELECT A.Id, A.Denumire 
        //                FROM Avs_tblAtribute A
        //                INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
        //                INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper OR (-1 * B.Super2) = C.IdSuper OR (-1 * B.Super3) = C.IdSuper OR (-1 * B.Super4) = C.IdSuper OR (-1 * B.Super5) = C.IdSuper OR (-1 * B.Super6) = C.IdSuper OR (-1 * B.Super7) = C.IdSuper OR (-1 * B.Super8) = C.IdSuper OR (-1 * B.Super9) = C.IdSuper OR (-1 * B.Super10) = C.IdSuper
        //                WHERE C.IdUser=@1
        //                GROUP BY A.Id, A.Denumire
        //                ORDER BY A.Denumire", new object[] { Session["UserId"] });
        //            cmbAtribute.DataSource = dtAtr;
        //            cmbAtribute.DataBind();
        //            cmbAtributeFiltru.DataSource = dtAtr;
        //            cmbAtributeFiltru.DataBind();

        //            if (Session["Marca_atribut"] != null)
        //            {
        //                string[] param = Session["Marca_atribut"].ToString().Split(';');
        //                F10003 = Convert.ToInt32(param[0]);
        //                for (int i = 0; i < cmbAng.Items.Count; i++)
        //                    if (cmbAng.Items[i].Value.ToString() == param[0])
        //                    {
        //                        cmbAng.SelectedIndex = i;
        //                        cmbAngFiltru.SelectedIndex = i;
        //                        break;
        //                    }
        //                cmbAng.Enabled = false;
        //                cmbAngFiltru.Enabled = false;
        //                for (int i = 0; i < cmbAtribute.Items.Count; i++)
        //                    if (Convert.ToInt32(cmbAtribute.Items[i].Value) == Convert.ToInt32(param[1]))
        //                    {
        //                        cmbAtribute.SelectedIndex = i;
        //                        break;
        //                    }
        //                cmbAtribute.Enabled = false;
        //            }
        //            AscundeCtl();
        //            IncarcaDate();


        //        }
        //        else
        //        {
        //            if (IsCallback)
        //            {
        //                cmbAng.DataSource = null;
        //                cmbAng.Items.Clear();
        //                cmbAng.DataSource = Session["Modif_Avans_Angajati"];
        //                cmbAng.DataBind();

        //                cmbAngFiltru.DataSource = null;
        //                cmbAngFiltru.Items.Clear();
        //                cmbAngFiltru.DataSource = Session["Modif_Avans_Angajati"];
        //                cmbAngFiltru.DataBind();

        //                //Florin 2019.05.20
        //                //DataTable dtAtr = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"Avs_tblAtribute\" ORDER BY \"Id\"", null);
        //                DataTable dtAtr = General.IncarcaDT($@"SELECT A.Id, A.Denumire 
        //                FROM Avs_tblAtribute A
        //                INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
        //                INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper OR (-1 * B.Super2) = C.IdSuper OR (-1 * B.Super3) = C.IdSuper OR (-1 * B.Super4) = C.IdSuper OR (-1 * B.Super5) = C.IdSuper OR (-1 * B.Super6) = C.IdSuper OR (-1 * B.Super7) = C.IdSuper OR (-1 * B.Super8) = C.IdSuper OR (-1 * B.Super9) = C.IdSuper OR (-1 * B.Super10) = C.IdSuper
        //                WHERE C.IdUser=@1
        //                GROUP BY A.Id, A.Denumire
        //                ORDER BY A.Denumire", new object[] { Session["UserId"] });
        //                cmbAtribute.DataSource = dtAtr;
        //                cmbAtribute.DataBind();
        //                cmbAtributeFiltru.DataSource = dtAtr;
        //                cmbAtributeFiltru.DataBind();

        //                if (Session["Marca_atribut"] != null)
        //                {
        //                    string[] param = Session["Marca_atribut"].ToString().Split(';');
        //                    F10003 = Convert.ToInt32(param[0]);
        //                }

        //                if (cmbAng.Value != null && cmbAtribute.Value != null)
        //                {
        //                    F10003 = Convert.ToInt32(cmbAng.Value.ToString());
        //                    IncarcaDate();
        //                }

        //                //if (cmbAngFiltru.Value != null)
        //                if (Session["Avs_MarcaFiltru"] != null)
        //                {
        //                    //F10003 = Convert.ToInt32(cmbAngFiltru.Value.ToString());
        //                    if (cmbAtributeFiltru.SelectedIndex < 0 && Convert.ToInt32(Session["Avs_AtributFiltru"].ToString()) != -1)
        //                        cmbAngFiltru.SelectedIndex = Convert.ToInt32(Session["Avs_MarcaFiltru"].ToString());
        //                    if (Session["Avs_AtributFiltru"] != null && cmbAtributeFiltru.SelectedIndex < 0 && Convert.ToInt32(Session["Avs_AtributFiltru"].ToString()) != -1)
        //                        cmbAtributeFiltru.SelectedIndex = Convert.ToInt32(Session["Avs_AtributFiltru"].ToString());
        //                    IncarcaGrid();
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ArataMesaj("");
        //        //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Session["Avs_MarcaFiltru"] = null;
                Session["Avs_AtributFiltru"] = null;
                //Iesire();
            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void Iesire()
        {
            try
            {
                Response.Redirect("~/Avs/Lista.aspx", false);
            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //  private string SelectAngajati()
        //  {
        //      string strSql = "";

        //      try
        //      {
        //          string op = "+";
        //          if (Constante.tipBD == 2) op = "||";

        //          strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
        //                  X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
        //                  FROM (
        //                  SELECT A.F10003
        //                  FROM F100 A
        //                  WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
        //                  UNION
        //                  SELECT A.F10003
        //                  FROM F100 A
        //                  INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
        //                  WHERE B.""IdUser""= {Session["UserId"]}) B
        //                  INNER JOIN F100 A ON A.F10003=B.F10003
        //                  LEFT JOIN F718 X ON A.F10071=X.F71802
        //                  LEFT JOIN F003 F ON A.F10004 = F.F00304
        //                  LEFT JOIN F004 G ON A.F10005 = G.F00405
        //                  LEFT JOIN F005 H ON A.F10006 = H.F00506
        //                  LEFT JOIN F006 I ON A.F10007 = I.F00607
        //where A.f10025 IN (0, 999) ";

        //      }
        //      catch (Exception ex)
        //      {
        //          //ArataMesaj("");
        //          //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //          General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //      }

        //      return strSql;
        //  }


        private string SelectAngajati(int idRol = -44)
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT  B.""Rol"", B.""RolDenumire"", A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", C.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                        FROM (
                        SELECT A.F10003, 0 AS ""Rol"", COALESCE((SELECT COALESCE(""Alias"", ""Denumire"") FROM ""tblSupervizori"" WHERE ""Id""=0),'Angajat') AS ""RolDenumire""
                        FROM F100 A
                        WHERE A.F10003 = {Session["UserId"]} AND (SELECT COUNT(*) FROM ""Avs_Circuit"" WHERE ""Super1""=0) > 0
                        UNION
                        SELECT A.F10003, B.""IdSuper"" AS ""Rol"", CASE WHEN D.""Alias"" IS NOT NULL AND D.""Alias"" <> '' THEN D.""Alias"" ELSE D.""Denumire"" END AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        INNER JOIN ""Avs_Circuit"" C ON B.""IdSuper"" = -1 * c.""Super1""
                        LEFT JOIN ""tblSupervizori"" D ON D.""Id"" = B.""IdSuper""
                        WHERE B.""IdUser""={Session["UserId"]}
                        UNION
                        SELECT A.F10003, 76 AS ""Rol"", '{Dami.TraduCuvant("Fara rol")}' AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""Avs_Circuit"" C ON C.""Super1""={Session["UserId"]}
                        ) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F002 C ON A.F10002 = C.F00202
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
						WHERE A.F10025 IN (0, 999) ";

                if (idRol != -44) strSql += @" AND ""Rol""=" + idRol;

            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


        private void AscundeCtl()
        {
            lbl1Act.Visible = false;
            lbl2Act.Visible = false;
            lbl3Act.Visible = false;
            lbl4Act.Visible = false;
            lbl5Act.Visible = false;
            lbl6Act.Visible = false;
            lbl7Act.Visible = false;
            lbl8Act.Visible = false;
            lbl9Act.Visible = false;
            lbl10Act.Visible = false;
            lbl11Act.Visible = false;
            lbl12Act.Visible = false;
            lbl13Act.Visible = false;
            lbl14Act.Visible = false;
            lbl15Act.Visible = false;

            lbl1Nou.Visible = false;
            lbl2Nou.Visible = false;
            lbl3Nou.Visible = false;
            lbl4Nou.Visible = false;
            lbl5Nou.Visible = false;
            lbl6Nou.Visible = false;
            lbl7Nou.Visible = false;
            lbl8Nou.Visible = false;
            lbl9Nou.Visible = false;
            lbl10Nou.Visible = false;
            lbl11Nou.Visible = false;
            lbl12Nou.Visible = false;
            lbl14Nou.Visible = false;
            lbl13Nou.Visible = false;
            lbl15Nou.Visible = false;
            lbl16Nou.Visible = false;

            lblTxt1Act.Visible = false;
            lblTxt1Act.Text = "";
            lblTxt2Act.Visible = false;
            lblTxt2Act.Text = "";
            lblTxt14Act.Visible = false;
            lblTxt14Act.Text = "";
            lblTxt15Act.Visible = false;
            lblTxt15Act.Text = "";
            txt1Nou.Text = "";
            txt2Nou.Text = "";
            txt3Nou.Text = "";
            txt4Nou.Text = "";
            txt1Act.Visible = false;
            txt1Act.Enabled = false;
            txt2Act.Visible = false;
            txt2Act.Enabled = false;
            txt3Act.Visible = false;
            txt3Act.Enabled = false;
            txt4Act.Visible = false;
            txt4Act.Enabled = false;


            lblTxt1Nou.Visible = false;
            lblTxt1Nou.Text = "";
            lblTxt2Nou.Visible = false;
            lblTxt2Nou.Text = "";
            lblTxt15Nou.Visible = false;
            lblTxt15Nou.Text = "";
            lblTxt16Nou.Visible = false;
            lblTxt16Nou.Text = "";
            txt1Nou.Visible = false;
            txt2Nou.Visible = false;
            txt3Nou.Visible = false;
            txt4Nou.Visible = false;

            lblTxt3Act.Visible = false;
            lblTxt3Act.Text = "";
            cmb1Act.Enabled = false;
            cmb1Act.Visible = false;
            cmb1Act.DataSource = null;
            cmb1Act.Items.Clear();
            lblTxt3Nou.Visible = false;
            lblTxt3Nou.Text = "";
            cmb1Nou.Enabled = false;
            cmb1Nou.Visible = false;
            cmb1Nou.DataSource = null;
            cmb1Nou.Items.Clear();

            lblTxt4Act.Visible = false;
            lblTxt4Act.Text = "";
            lblTxt5Act.Visible = false;
            lblTxt5Act.Text = "";
            lblTxt6Act.Visible = false;
            lblTxt6Act.Text = "";
            lblTxt4Nou.Visible = false;
            lblTxt4Nou.Text = "";
            lblTxt5Nou.Visible = false;
            lblTxt5Nou.Text = "";
            lblTxt6Nou.Visible = false;
            lblTxt6Nou.Text = "";

            lblTxt7Act.Visible = false;
            lblTxt7Act.Text = "";
            lblTxt8Act.Visible = false;
            lblTxt8Act.Text = "";
            lblTxt9Act.Visible = false;
            lblTxt9Act.Text = "";
            lblTxt7Nou.Visible = false;
            lblTxt7Nou.Text = "";
            lblTxt8Nou.Visible = false;
            lblTxt8Nou.Text = "";
            lblTxt9Nou.Visible = false;
            lblTxt9Nou.Text = "";
            lblTxt10Act.Visible = false;
            lblTxt10Act.Text = "";
            lblTxt11Act.Visible = false;
            lblTxt11Act.Text = "";
            lblTxt13Act.Visible = false;
            lblTxt13Act.Text = "";
            lblTxt10Nou.Visible = false;
            lblTxt10Nou.Text = "";
            lblTxt11Nou.Visible = false;
            lblTxt11Nou.Text = "";
            lblTxt13Nou.Visible = false;
            lblTxt13Nou.Text = "";
            lblTxt14Nou.Visible = false;
            lblTxt14Nou.Text = "";

            cmb2Act.Enabled = false;
            cmb2Act.Visible = false;
            cmb2Act.DataSource = null;
            cmb2Act.Items.Clear();
            cmb2Nou.Enabled = false;
            cmb2Nou.Visible = false;
            cmb2Nou.DataSource = null;
            cmb2Nou.Items.Clear();

            de1Act.Visible = false;
            de1Act.Value = null;
            de2Act.Visible = false;
            de2Act.Value = null;
            de1Nou.Visible = false;
            de1Nou.Value = null;
            de2Nou.Visible = false;
            de2Nou.Value = null;
            de3Nou.Visible = false;
            de3Nou.Value = null;

            cmb3Act.Enabled = false;
            cmb3Act.Visible = false;
            cmb3Act.DataSource = null;
            cmb3Act.Items.Clear();
            cmb3Nou.Enabled = false;
            cmb3Nou.Visible = false;
            cmb3Nou.DataSource = null;
            cmb3Nou.Items.Clear();

            cmb4Act.Enabled = false;
            cmb4Act.Visible = false;
            cmb4Act.DataSource = null;
            cmb4Act.Items.Clear();
            cmb4Nou.Enabled = false;
            cmb4Nou.Visible = false;
            cmb4Nou.DataSource = null;
            cmb4Nou.Items.Clear();

            cmb5Act.Enabled = false;
            cmb5Act.Visible = false;
            cmb5Act.DataSource = null;
            cmb5Act.Items.Clear();
            cmb5Nou.Enabled = false;
            cmb5Nou.Visible = false;
            cmb5Nou.DataSource = null;
            cmb5Nou.Items.Clear();

            cmb6Act.Enabled = false;
            cmb6Act.Visible = false;
            cmb6Act.DataSource = null;
            cmb6Act.Items.Clear();
            cmb6Nou.Enabled = false;
            cmb6Nou.Visible = false;
            cmb6Nou.DataSource = null;
            cmb6Nou.Items.Clear();

            cmb7Act.Enabled = false;
            cmb7Act.Visible = false;
            cmb7Act.DataSource = null;
            cmb7Act.Items.Clear();
            cmb7Nou.Enabled = false;
            cmb7Nou.Visible = false;
            cmb7Nou.DataSource = null;
            cmb7Nou.Items.Clear();

            cmb8Act.Enabled = false;
            cmb8Act.Visible = false;
            cmb8Act.DataSource = null;
            cmb8Act.Items.Clear();
            cmb8Nou.Enabled = false;
            cmb8Nou.Visible = false;
            cmb8Nou.DataSource = null;
            cmb8Nou.Items.Clear();

            cmbStructOrgAct.Enabled = false;
            cmbStructOrgAct.Visible = false;
            cmbStructOrgAct.DataSource = null;
            cmbStructOrgAct.Items.Clear();
            cmbStructOrgNou.Enabled = false;
            cmbStructOrgNou.Visible = false;
            cmbStructOrgNou.DataSource = null;
            cmbStructOrgNou.Items.Clear();

            lblTxt12Act.Visible = false;
            lblTxt12Act.Text = "";
            lblTxt12Nou.Visible = false;
            lblTxt12Nou.Text = "";

            Session["Valoare1Noua"] = null;
            Session["Valoare2Noua"] = null;
            Session["Valoare3Noua"] = null;
            Session["Valoare4Noua"] = null;
            Session["Valoare5Noua"] = null;
            Session["Valoare6Noua"] = null;
            Session["Valoare7Noua"] = null;
            Session["Valoare8Noua"] = null;
            Session["Valoare9Noua"] = null;

            grDateComponente.Visible = false;
            grDateTarife.Visible = false;
            grDateSporuri1.Visible = false;
            grDateSporuri2.Visible = false;
            grDateSporTran.Visible = false;

            lblDoc.Visible = false;
            btnDocUpload.Visible = false;
            btnDocSterge.Visible = false;

            chk1.Visible = false;
            chk2.Visible = false;
            chk3.Visible = false;
            chk4.Visible = false;
            chk5.Visible = false;

            cmb1Nou.ClientEnabled = true;
            txt1Nou.ClientEnabled = true;
            txt2Nou.ClientEnabled = true;
            de1Nou.ClientEnabled = true;
        }

        private void ArataCtl(int nr, string text1, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9, string text10, string text11 = "")
        {
            if (nr == 1)
            {// 1 x TB
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text1;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text2;
                txt1Nou.Visible = true;
            }
            if (nr == 2)
            {// 2 X TB
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lbl9Act.Visible = true;
                lbl9Nou.Visible = true;
                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text1;
                lblTxt2Act.Visible = true;
                lblTxt2Act.Text = text3;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                txt2Act.Visible = true;
                txt2Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text2;
                lblTxt2Nou.Visible = true;
                lblTxt2Nou.Text = text4;
                txt1Nou.Visible = true;
                txt2Nou.Visible = true;
            }
            if (nr == 3)
            {// 1 x CB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;
            }
            if (nr == 4)
            {// 0.5 x CB
                lbl1Nou.Visible = true;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text1;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;
            }
            if (nr == 5)
            {// 1 x TB + 1 x DE
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text1;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text2;
                txt1Nou.Visible = true;

                lblTxt5Act.Visible = true;
                lblTxt5Act.Text = text3;
                de1Act.Visible = true;
                de1Act.Enabled = false;
                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text4;
                de1Nou.Visible = true;
            }
            if (nr == 6)
            {// 1 x DE
                lbl10Act.Visible = true;
                lbl10Nou.Visible = true;
                lblTxt5Act.Visible = true;
                lblTxt5Act.Text = text1;
                de1Act.Visible = true;
                de1Act.Enabled = false;
                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text2;
                de1Nou.Visible = true;
            }
            if (nr == 7)
            {// 2 x DE
                lbl10Act.Visible = true;
                lbl10Nou.Visible = true;
                lbl11Act.Visible = true;
                lbl11Nou.Visible = true;
                lblTxt5Act.Visible = true;
                lblTxt5Act.Text = text1;
                de1Act.Visible = true;
                de1Act.Enabled = false;
                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text2;
                de1Nou.Visible = true;

                lblTxt6Act.Visible = true;
                lblTxt6Act.Text = text3;
                de2Act.Visible = true;
                de2Act.Enabled = false;
                lblTxt6Nou.Visible = true;
                lblTxt6Nou.Text = text4;
                de2Nou.Visible = true;
            }
            if (nr == 8)
            {// 2 x CB + 2 x TB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lbl2Act.Visible = true;
                lbl2Nou.Visible = true;
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lbl9Act.Visible = true;
                lbl9Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                lblTxt4Act.Visible = true;
                lblTxt4Act.Text = text3;
                cmb2Act.Visible = true;
                cmb2Act.Enabled = false;
                lblTxt4Nou.Visible = true;
                lblTxt4Nou.Text = text4;
                cmb2Nou.Visible = true;
                cmb2Nou.Enabled = true;

                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text5;
                lblTxt2Act.Visible = true;
                lblTxt2Act.Text = text6;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                txt2Act.Visible = true;
                txt2Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text7;
                lblTxt2Nou.Visible = true;
                lblTxt2Nou.Text = text8;
                txt1Nou.Visible = true;
                txt2Nou.Visible = true;

            }
            if (nr == 9)
            {// 2 x CB + 1 x TB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lbl2Act.Visible = true;
                lbl2Nou.Visible = true;
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                lblTxt4Act.Visible = true;
                lblTxt4Act.Text = text3;
                cmb2Act.Visible = true;
                cmb2Act.Enabled = false;
                lblTxt4Nou.Visible = true;
                lblTxt4Nou.Text = text4;
                cmb2Nou.Visible = true;
                cmb2Nou.Enabled = true;

                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text5;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text6;
                txt1Nou.Visible = true;
            }
            if (nr == 10)
            {// 1 x CB + 2 x TB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lbl9Act.Visible = true;
                lbl9Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text3;
                lblTxt2Act.Visible = true;
                lblTxt2Act.Text = text4;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                txt2Act.Visible = true;
                txt2Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text5;
                lblTxt2Nou.Visible = true;
                lblTxt2Nou.Text = text6;
                txt1Nou.Visible = true;
                txt2Nou.Visible = true;
            }
            if (nr == 11)
            {// 1 x CB + 2 x TB + 2 x DE
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lbl9Act.Visible = true;
                lbl9Nou.Visible = true;
                lbl10Act.Visible = true;
                lbl10Nou.Visible = true;
                lbl11Act.Visible = true;
                lbl11Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text3;
                lblTxt2Act.Visible = true;
                lblTxt2Act.Text = text5;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                txt2Act.Visible = true;
                txt2Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text4;
                lblTxt2Nou.Visible = true;
                lblTxt2Nou.Text = text6;
                txt1Nou.Visible = true;
                txt2Nou.Visible = true;

                lblTxt5Act.Visible = true;
                lblTxt5Act.Text = text7;
                de1Act.Visible = true;
                de1Act.Enabled = false;
                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text8;
                de1Nou.Visible = true;

                lblTxt6Act.Visible = true;
                lblTxt6Act.Text = text9;
                de2Act.Visible = true;
                de2Act.Enabled = false;
                lblTxt6Nou.Visible = true;
                lblTxt6Nou.Text = text10;
                de2Nou.Visible = true;
            }
            if (nr == 12)
            {// 8 x CB + 1 x TB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lbl2Act.Visible = true;
                lbl2Nou.Visible = true;
                lbl3Act.Visible = true;
                lbl3Nou.Visible = true;
                lbl4Act.Visible = true;
                lbl4Nou.Visible = true;
                lbl5Act.Visible = true;
                lbl5Nou.Visible = true;
                lbl6Act.Visible = true;
                lbl6Nou.Visible = true;
                lbl7Act.Visible = true;
                lbl7Nou.Visible = true;
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lbl13Act.Visible = true;
                lbl14Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                lblTxt4Act.Visible = true;
                lblTxt4Act.Text = text3;
                cmb2Act.Visible = true;
                cmb2Act.Enabled = false;
                lblTxt4Nou.Visible = true;
                lblTxt4Nou.Text = text3;
                cmb2Nou.Visible = true;
                cmb2Nou.Enabled = true;

                lblTxt7Act.Visible = true;
                lblTxt7Act.Text = text4;
                cmb3Act.Visible = true;
                cmb3Act.Enabled = false;
                lblTxt7Nou.Visible = true;
                lblTxt7Nou.Text = text4;
                cmb3Nou.Visible = true;
                cmb3Nou.Enabled = true;

                lblTxt8Act.Visible = true;
                lblTxt8Act.Text = text5;
                cmb4Act.Visible = true;
                cmb4Act.Enabled = false;
                lblTxt8Nou.Visible = true;
                lblTxt8Nou.Text = text5;
                cmb4Nou.Visible = true;
                cmb4Nou.Enabled = true;

                lblTxt9Act.Visible = true;
                lblTxt9Act.Text = text6;
                cmb5Act.Visible = true;
                cmb5Act.Enabled = false;
                lblTxt9Nou.Visible = true;
                lblTxt9Nou.Text = text6;
                cmb5Nou.Visible = true;
                cmb5Nou.Enabled = true;

                lblTxt10Act.Visible = true;
                lblTxt10Act.Text = text7;
                cmb6Act.Visible = true;
                cmb6Act.Enabled = false;
                lblTxt10Nou.Visible = true;
                lblTxt10Nou.Text = text7;
                cmb6Nou.Visible = true;
                cmb6Nou.Enabled = true;

                lblTxt11Act.Visible = true;
                lblTxt11Act.Text = text8;
                cmb7Act.Visible = true;
                cmb7Act.Enabled = false;
                lblTxt11Nou.Visible = true;
                lblTxt11Nou.Text = text8;
                cmb7Nou.Visible = true;
                cmb7Nou.Enabled = true;

                lblTxt13Act.Visible = true;
                lblTxt13Act.Text = text10;
                cmb8Act.Visible = true;
                cmb8Act.Enabled = false;
                lblTxt14Nou.Visible = true;
                lblTxt14Nou.Text = text10;
                cmb8Nou.Visible = true;
                cmb8Nou.Enabled = true;

                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text9;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text9;
                txt1Nou.Visible = true;
            }
            if (nr == 13)
            {// struct org
                lbl12Act.Visible = true;
                lbl12Nou.Visible = true;
                lblTxt12Act.Visible = true;
                lblTxt12Act.Text = text1;
                cmbStructOrgAct.Visible = true;
                cmbStructOrgAct.Enabled = false;
                lblTxt12Nou.Visible = true;
                lblTxt12Nou.Text = text2;
                cmbStructOrgNou.Visible = true;
                cmbStructOrgNou.Enabled = true;
            }
            if (nr == 14)
            {// 0.5 x CB + 3 x 0.5 x DE
                lbl1Nou.Visible = true;
                lbl10Nou.Visible = true;
                lbl11Nou.Visible = true;
                lbl13Nou.Visible = true;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text1;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;
           
                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text8;
                de1Nou.Visible = true;
        
                lblTxt6Nou.Visible = true;
                lblTxt6Nou.Text = text10;
                de2Nou.Visible = true;

                lblTxt13Nou.Visible = true;
                lblTxt13Nou.Text = text11;
                de3Nou.Visible = true;
            }
            if (nr == 15)
            {// 0.5 x CB + 2 x 0.5 x TB + 3 x 0.5 x DE +  5 x bifa
            
                lbl1Nou.Visible = true;        
                lbl8Nou.Visible = true;        
                lbl9Nou.Visible = true;      
                lbl10Nou.Visible = true;     
                lbl11Nou.Visible = true;
                lbl13Nou.Visible = true;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text4;
                lblTxt2Nou.Visible = true;
                lblTxt2Nou.Text = text6;
                txt1Nou.Visible = true;
                txt2Nou.Visible = true;

                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text8;
                de1Nou.Visible = true;

                lblTxt6Nou.Visible = true;
                lblTxt6Nou.Text = text10;
                de2Nou.Visible = true;

                lblTxt13Nou.Visible = true;
                lblTxt13Nou.Text = text11;
                de3Nou.Visible = true;

                chk1.Visible = true;
                chk2.Visible = true;
                chk3.Visible = true;
                chk4.Visible = true;
                chk5.Visible = true;
            }
            if (nr == 16)
            {// 2 x CB + 4 x TB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Functie)
                {
                    lbl2Act.Visible = true;
                    lbl2Nou.Visible = true;
                }
                lbl8Act.Visible = true;
                lbl8Nou.Visible = true;
                lbl9Act.Visible = true;
                lbl9Nou.Visible = true;
                lbl14Act.Visible = true;
                lbl15Nou.Visible = true;
                lbl15Act.Visible = true;
                lbl16Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;

                if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Functie)
                {
                    lblTxt4Act.Visible = true;
                    lblTxt4Act.Text = text3;
                    cmb2Act.Visible = true;
                    cmb2Act.Enabled = false;
                    lblTxt4Nou.Visible = true;
                    lblTxt4Nou.Text = text4;
                    cmb2Nou.Visible = true;
                    cmb2Nou.Enabled = true;
                }

                lblTxt1Act.Visible = true;
                lblTxt1Act.Text = text5;
                lblTxt2Act.Visible = true;
                lblTxt2Act.Text = text6;
                lblTxt14Act.Visible = true;
                lblTxt14Act.Text = text7;
                lblTxt15Act.Visible = true;
                lblTxt15Act.Text = text8;
                txt1Act.Visible = true;
                txt1Act.Enabled = false;
                txt2Act.Visible = true;
                txt2Act.Enabled = false;
                txt3Act.Visible = true;
                txt3Act.Enabled = false;
                txt4Act.Visible = true;
                txt4Act.Enabled = false;

                lblTxt1Nou.Visible = true;
                lblTxt1Nou.Text = text5;
                lblTxt2Nou.Visible = true;
                lblTxt2Nou.Text = text6;
                lblTxt15Nou.Visible = true;
                lblTxt15Nou.Text = text7;
                lblTxt16Nou.Visible = true;
                lblTxt16Nou.Text = text8;
                txt1Nou.Visible = true;
                txt2Nou.Visible = true;
                txt3Nou.Visible = true;
                txt4Nou.Visible = true;

            }
            if (nr == 17)
            {// 2 x CB + 2 x DE + 2 x TB
                lbl1Act.Visible = true;
                lbl1Nou.Visible = true;
                lbl2Act.Visible = true;
                lbl2Nou.Visible = true;
                lbl14Act.Visible = true;
                lbl15Nou.Visible = true;
                lbl14Act.Visible = true;
                lbl15Act.Visible = true;
                lbl16Nou.Visible = true;
                lbl10Act.Visible = true;
                lbl10Nou.Visible = true;
                lbl11Act.Visible = true;
                lbl11Nou.Visible = true;
                lblTxt3Act.Visible = true;
                lblTxt3Act.Text = text1;
                cmb1Act.Visible = true;
                cmb1Act.Enabled = false;
                cmb2Act.Visible = true;
                cmb2Act.Enabled = false;
                lblTxt3Nou.Visible = true;
                lblTxt3Nou.Text = text2;
                cmb1Nou.Visible = true;
                cmb1Nou.Enabled = true;
                cmb2Nou.Visible = true;
                cmb2Nou.Enabled = true;
                lblTxt4Act.Visible = true;
                lblTxt4Act.Text = text3;
                lblTxt4Nou.Visible = true;
                lblTxt4Nou.Text = text4;

                lblTxt14Act.Visible = true;
                lblTxt14Act.Text = text5;
                lblTxt15Act.Visible = true;
                lblTxt15Act.Text = text6;
                txt3Act.Visible = true;
                txt3Act.Enabled = false;
                txt4Act.Visible = true;
                txt4Act.Enabled = false;

                lblTxt15Nou.Visible = true;
                lblTxt15Nou.Text = text5;
                lblTxt16Nou.Visible = true;
                lblTxt16Nou.Text = text6;
                txt3Nou.Visible = true;
                txt4Nou.Visible = true;

                lblTxt5Act.Visible = true;
                lblTxt5Act.Text = text7;
                de1Act.Visible = true;
                de1Act.Enabled = false;
                lblTxt5Nou.Visible = true;
                lblTxt5Nou.Text = text7;
                de1Nou.Visible = true;

                lblTxt6Act.Visible = true;
                lblTxt6Act.Text = text8;
                de2Act.Visible = true;
                de2Act.Enabled = false;
                lblTxt6Nou.Visible = true;
                lblTxt6Nou.Text = text8;
                de2Nou.Visible = true;
            }

        }


        private void IncarcaDate()
        {
            string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Salariul)
            {
                ArataCtl(2, "Salariu brut actual", "Salariu brut nou", "Salariu net actual", "Salariu net nou", "", "", "", "", "", "");
                DataTable dtTemp = General.IncarcaDT("SELECT " + salariu + " FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();

                //Florin 2019.12.19
                //CalcSalariu(1, txt1Act, txt2Act);
                decimal venitCalculat = 0m;
                string text = "";
                General.CalcSalariu(1, txt1Act.Value, Convert.ToInt32(General.Nz(cmbAng.Value, -99)), out venitCalculat, out text);
                txt2Act.Value = venitCalculat;
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Functie)
            {
                //ArataCtl(3, "Functia actuala", "Functia noua", "", "", "", "", "", "", "", "");
                ArataCtl(16, "Functia actuala", "Functia noua", "Nivel actual", "Nivel nou", "Per. proba zile lucratoare", "Per. proba zile calendaristice", "Nr. zile preaviz demisie", "Nr. zile preaviz concediere", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F71802 AS \"Id\", F71804 AS \"Denumire\" from F100, f718 WHERE F10071 = F71802 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71802 AS \"Id\", F71804 AS \"Denumire\" from f718", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"tblNivelFunctie\" left join F718 on F71813 = \"Id\" LEFT JOIN F100 on F10071=F71802 WHERE F10003 = "+ cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " ORDER BY \"Denumire\"", null);
                dtTemp2 = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"tblNivelFunctie\" ORDER BY \"Denumire\"", null);
                IncarcaComboBox(cmb2Act, cmb2Nou, dtTemp1, dtTemp2);

                DataTable dtTemp = General.IncarcaDT("SELECT F100975 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();

                dtTemp = General.IncarcaDT("SELECT F1001063 FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt2Act.Text = dtTemp.Rows[0][0].ToString();

                dtTemp = General.IncarcaDT("SELECT F1009742 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt3Act.Text = dtTemp.Rows[0][0].ToString();

                dtTemp = General.IncarcaDT("SELECT F100931 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt4Act.Text = dtTemp.Rows[0][0].ToString();
                
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.CodCOR)
            {
                int afisare = Convert.ToInt32(Dami.ValoareParam("Avs_CampuriZileProbaPreavizCOR", "0"));
                if (afisare == 0)
                    ArataCtl(3, "Cod COR actual", "Cod COR nou", "", "", "", "", "", "", "", "");
                else
                    ArataCtl(16, "Cod COR actual", "Cod COR nou", "", "", "Per. proba zile lucratoare", "Per. proba zile calendaristice", "Nr. zile preaviz demisie", "Nr. zile preaviz concediere", "", "");
                string sql = "";
                if (Constante.tipBD == 1)
                    sql = "select F72202 AS \"Id\", CONVERT(VARCHAR, F72202) + ' - '  +  F72204 AS \"Denumire\" from F100 LEFT JOIN F1001 ON F100.F10003 = F1001.F10003 LEFT JOIN f722 ON F10098 = F72202 WHERE F1001082=F72206 AND F100.F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                else
                    sql = "select F72202 AS \"Id\", F72202 || ' - '  ||  F72204 AS \"Denumire\" from LEFT JOIN F1001 ON F100.F10003 = F1001.F10003 LEFT JOIN f722 ON F10098 = F72202 WHERE F1001082=F72206 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                DataTable dtTemp1 = General.IncarcaDT(sql, null);
                if (Constante.tipBD == 1)
                    sql = "select F72202 AS \"Id\", CONVERT(VARCHAR, F72202) + ' - '  +  F72204 AS \"Denumire\" from f722 WHERE f72206 = (select max(f72206) from f722) ";
                else
                    sql = "select F72202 AS \"Id\", F72202 || ' - '  ||  F72204 AS \"Denumire\" from f722 WHERE f72206 = (select max(f72206) from f722) "; ;
                DataTable dtTemp2 = General.IncarcaDT(sql, null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                if (afisare == 1)
                {
                    DataTable dtTemp = General.IncarcaDT("SELECT F100975 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                    txt1Act.Text = dtTemp.Rows[0][0].ToString();

                    dtTemp = General.IncarcaDT("SELECT F1001063 FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                    txt2Act.Text = dtTemp.Rows[0][0].ToString();

                    dtTemp = General.IncarcaDT("SELECT F1009742 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                    txt3Act.Text = dtTemp.Rows[0][0].ToString();

                    dtTemp = General.IncarcaDT("SELECT F100931 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                    txt4Act.Text = dtTemp.Rows[0][0].ToString();
                }
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.MotivPlecare)
            {
                ArataCtl(4, "Motiv plecare", "", "", "", "", "", "", "", "", "");
                DataTable dtTemp = General.IncarcaDT("select F09802 AS \"Id\", F09804 AS \"Denumire\" from f098 ", null);
                //cmb1Nou.DataSource = dtTemp;
                //cmb1Nou.DataBind();
                //cmb1Nou.SelectedIndex = 0;
                IncarcaComboBox(cmb1Act, cmb1Nou, null, dtTemp);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Organigrama)
            {
                ArataCtl(13, "Structura actuala", "Structura noua", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.GetStructOrgAng(Convert.ToInt32(cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()));
                DataTable dtTemp2 = General.GetStructOrgModif(Convert.ToDateTime(txtDataMod.Value));
                IncarcaComboBox(cmbStructOrgAct, cmbStructOrgNou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Norma)
            {
                ArataCtl(12, "Tip angajat actual", "Tip angajat nou", "Timp partial", "Norma", "Tip norma", "Durata", "Repartizare", "Interval", "Nr. de ore luna/sapt", "Program de lucru");
                DataTable dtTemp1 = General.IncarcaDT("select F71602 AS \"Id\", F71604 AS \"Denumire\" from F100, F716 WHERE F10010 = F71602 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71602 AS \"Id\", F71604 AS \"Denumire\" from F716", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                int valMin = 1, valMax = 8;
                if (cmb1Nou.Value != null)
                {
                    if (Convert.ToInt32(cmb1Nou.Value) == 0)
                    {
                        valMin = 6;
                        valMax = 8;
                    }
                    if (Convert.ToInt32(cmb1Nou.Value) == 2)
                    {
                        valMin = 1;
                        valMax = 7;
                    }
                }
                dtTemp1 = General.IncarcaDT("select F10043 AS \"Id\", F10043 AS \"Denumire\" from F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.ListaNumere(valMin, valMax);
                IncarcaComboBox(cmb2Act, cmb2Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select F100973 AS \"Id\", F100973 AS \"Denumire\" from F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.ListaNumere(6, 8);
                IncarcaComboBox(cmb3Act, cmb3Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select F09202 AS \"Id\", F09203 AS \"Denumire\" from F100, F092 WHERE F100926 = F09202 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.IncarcaDT("select F09202 AS \"Id\", F09203 AS \"Denumire\" from F092", null);
                IncarcaComboBox(cmb4Act, cmb4Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select F09102 AS \"Id\", F09103 AS \"Denumire\" from F100, F091 WHERE F100927 = F09102 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.IncarcaDT("select F09102 AS \"Id\", F09103 AS \"Denumire\" from F091", null);
                IncarcaComboBox(cmb5Act, cmb5Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select F09302 AS \"Id\", F09303 AS \"Denumire\" from F100, F093 WHERE F100928 = F09302 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.IncarcaDT("select F09302 AS \"Id\", F09303 AS \"Denumire\" from F093", null);
                IncarcaComboBox(cmb6Act, cmb6Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select F09602 AS \"Id\", F09603 AS \"Denumire\" from F100, F096 WHERE F100939 = F09602 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.IncarcaDT("select F09602 AS \"Id\", F09603 AS \"Denumire\" from F096", null);
                dtTemp2.TableName = "F096";
                IncarcaComboBox(cmb7Act, cmb7Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select\"Id\", \"Denumire\", \"DenumireInAdmin\" from \"Ptj_Contracte\", \"F100Contracte\" WHERE \"IdContract\" = \"Id\" AND \"DataSfarsit\" = " + General.ToDataUniv(new DateTime(2100, 1, 1)) + " AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.IncarcaDT("select \"Id\", \"Denumire\", \"DenumireInAdmin\" from \"Ptj_Contracte\"", null);
                IncarcaComboBox(cmb8Act, cmb8Nou, dtTemp1, dtTemp2);
                if (Session["Valoare9Noua"] == null)
                    cmb8Nou.Value = cmb8Act.Value;

                DataTable dtTemp = General.IncarcaDT("SELECT F100964 FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.ContrIn)
            {
                ArataCtl(5, "Nr. actual", "Nr. nou", "Data actuala", "Data noua", "", "", "", "", "", "");
                string data = "";
                if (Constante.tipBD == 1)
                    data = " CONVERT(VARCHAR, F100986, 103) ";
                else
                    data = " TO_CHAR(F100986, 'dd/mm/yyyy') ";
                DataTable dtTemp = General.IncarcaDT("SELECT F100985, " + data + " AS F100986 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][1].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][1].ToString());
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.ContrITM)
            {
                ArataCtl(5, "Nr. actual", "Nr. nou", "Data actuala", "Data noua", "", "", "", "", "", "");
                string data = "";
                if (Constante.tipBD == 1)
                    data = " CONVERT(VARCHAR, FX1, 103) ";
                else
                    data = " TO_CHAR(FX1, 'dd/mm/yyyy') ";
                DataTable dtTemp = General.IncarcaDT("SELECT F10011, " + data + " AS FX1 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][1].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][1].ToString());
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.DataAngajarii)
            {
                ArataCtl(6, "Data actuala", "Data noua", "", "", "", "", "", "", "", "");
                string data = "";
                if (Constante.tipBD == 1)
                    data = " CONVERT(VARCHAR, F10022, 103) ";
                else
                    data = " TO_CHAR(F10022, 'dd/mm/yyyy') ";
                DataTable dtTemp = General.IncarcaDT("SELECT " + data + " AS F10022 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][0].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][0].ToString());
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Sporuri)
            {
                //Session["AvsCereri"] = null;
                //Session["AvsCereriCalcul"] = null;
                grDateSporuri1.Visible = true;
                grDateSporuri1.DataBind();
                grDateSporuri2.Visible = true;
                grDateSporuri2.DataBind();
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.TitluAcademic)
            {
                ArataCtl(3, "Titlu actual", "Titlu nou", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F71302 AS \"Id\", F71304 AS \"Denumire\" from F100, f713 WHERE F10051 = F71302 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71302 AS \"Id\", F71304 AS \"Denumire\" from f713", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.MesajPersonal)
            {
                ArataCtl(3, "Mesaj actual", "Mesaj nou", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F72402 AS \"Id\", F72404 AS \"Denumire\" from F100, f724 WHERE F10061 = F72402 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F72402 AS \"Id\", F72404 AS \"Denumire\" from f724", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.CentrulCost)
            {
                ArataCtl(3, "Centru cost actual", "Centru cost nou", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F06204 AS \"Id\", F06205 AS \"Denumire\" from F100, F062 WHERE F10053 = F06204 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F06204 AS \"Id\", F06205 AS \"Denumire\" from F062", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.SporTranzactii)
            {
                //Session["AvsCereri"] = null;
                //Session["AvsCereriCalcul"] = null;
                grDateSporTran.Visible = true;
                grDateSporTran.DataBind();
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PunctLucru)
            {
                ArataCtl(3, "Punct lucru actual", "Punct lucru nou", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F08002 AS \"Id\", F08003 AS \"Denumire\" from F100, F080 WHERE F10079 = F08002 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F08002 AS \"Id\", F08003 AS \"Denumire\" from F080", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Meserie)
            {
                ArataCtl(3, "Meserie actuala", "Meserie noua", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F71702 AS \"Id\", F71704 AS \"Denumire\" from F100, F717 WHERE F10029 = F71702 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71702 AS \"Id\", F71704 AS \"Denumire\" from F717", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM || Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM_Vanz)
            {
                ArataCtl(11, "Tip contract actual", "Tip contract nou", "Nr. luni actuale", "Nr. luni noi", "Nr. zile actuale", "Nr. zile noi", "Data Inc. CIM actuala", "Data Inc. CIM noua", "Data Sf. CIM actuala", "Data Sf. CIM noua");
                //ArataCtl(7, "Data Inc. CIM actuala", "Data Inc. CIM noua", "Data Sf. CIM actuala", "Data Sf. CIM noua", "", "", "", "", "", "");

                DataTable dtTemp1 = General.IncarcaDT("select F08902 AS \"Id\", F08903 AS \"Denumire\" from F100, F089 WHERE F08902 = F1009741 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F08902 AS \"Id\", F08903 AS \"Denumire\" from F089", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                string data1 = "", data2 = "";
                if (Constante.tipBD == 1)
                {
                    data1 = " CONVERT(VARCHAR, F100933, 103) ";
                    data2 = " CONVERT(VARCHAR, F100934, 103) ";
                }
                else
                {
                    data1 = " TO_CHAR(F100933, 'dd/mm/yyyy') ";
                    data2 = " CONVERT(VARCHAR, F100934, 103) ";
                }

                DataTable dtTemp = General.IncarcaDT("SELECT " + data1 + " AS F100933, " + data2 + " AS F100934 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][0].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][0].ToString());
                de2Act.Date = Convert.ToDateTime(dtTemp.Rows[0][1].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][1].ToString());

                int nrLuni = 0, nrZile = 0;
                Personal.Contract ctr = new Personal.Contract();
                ctr.CalculLuniSiZile(Convert.ToDateTime(de1Act.Date), Convert.ToDateTime(de2Act.Date), out nrLuni, out nrZile);
                txt1Act.Value = nrLuni;
                txt2Act.Value = nrZile;

                if (cmb1Act != null && cmb1Act.Value != null && Convert.ToInt32(cmb1Act.Value) == 1)
                {
                    lblTxt1Act.Visible = false;
                    lblTxt2Act.Visible = false;
                    txt1Act.Visible = false;
                    txt2Act.Visible = false;
                    lblTxt5Act.Visible = false;
                    lblTxt6Act.Visible = false;
                    de1Act.Visible = false;
                    de2Act.Visible = false;
                }
                else
                {
                    lblTxt1Act.Visible = true;
                    lblTxt2Act.Visible = true;
                    txt1Act.Visible = true;
                    txt2Act.Visible = true;
                    lblTxt5Act.Visible = true;
                    lblTxt6Act.Visible = true;
                    de1Act.Visible = true;
                    de2Act.Visible = true;
                }

                if (cmb1Nou != null && cmb1Nou.Value != null && Convert.ToInt32(cmb1Nou.Value) == 1)
                {
                    lblTxt1Nou.Visible = false;
                    lblTxt2Nou.Visible = false;
                    txt1Nou.Visible = false;
                    txt2Nou.Visible = false;
                    lblTxt5Nou.Visible = false;
                    lblTxt6Nou.Visible = false;
                    de1Nou.Visible = false;
                    de2Nou.Visible = false;
                    de1Nou.Value = new DateTime(2100, 1, 1);
                    de2Nou.Value = new DateTime(2100, 1, 1);
                }
                else
                {
                    lblTxt1Nou.Visible = true;
                    lblTxt2Nou.Visible = true;
                    txt1Nou.Visible = true;
                    txt2Nou.Visible = true;
                    lblTxt5Nou.Visible = true;
                    lblTxt6Nou.Visible = true;
                    de1Nou.Visible = true;
                    de2Nou.Visible = true;
                }

            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Componente)
            {
                //Session["AvsCereri"] = null;
                //Session["AvsCereriCalcul"] = null;
                grDateComponente.Visible = true;
                grDateComponente.DataBind();
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Tarife)
            {
                //Session["AvsCereri"] = null;
                //Session["AvsCereriCalcul"] = null;
                grDateTarife.Visible = true;
                grDateTarife.DataBind();
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.NumePrenume)
            {
                ArataCtl(2, "Nume actual", "Nume nou", "Prenume actual", "Prenume nou", "", "", "", "", "", "");
                DataTable dtTemp = General.IncarcaDT("SELECT F10008, F10009 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                txt2Act.Text = dtTemp.Rows[0][1].ToString();
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.CASS)
            {
                ArataCtl(3, "CASS actuala", "CASS noua", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F06302 AS \"Id\", F06303 AS \"Denumire\" from F100, F063 WHERE F1003900 = F06302 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F06302 AS \"Id\", F06303 AS \"Denumire\" from F063", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Studii)
            {
                ArataCtl(3, "Studii actuale", "Studii noi", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F71202 AS \"Id\", F71204 AS \"Denumire\" from F100, F712 WHERE F10050 = F71202 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71202 AS \"Id\", F71204 AS \"Denumire\" from F712", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.BancaSalariu)
            {
                ArataCtl(8, "Banca actuala", "Banca noua", "Sucursala actuala", "Sucursala noua", "IBAN actual", "Nr. card actual", "IBAN nou", "Nr. card nou", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F100, F075 WHERE F10018 = F07503 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " group by F07503 , F07509 ", null);
                DataTable dtTemp2 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F075 group by F07503 , F07509", null);
                   
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
                DataTable dtTemp = General.IncarcaDT("SELECT F10020, F10055 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                txt2Act.Text = dtTemp.Rows[0][1].ToString();

                dtTemp1 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F100, F075 WHERE F10019 = F07504 and f10018=f07503 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = null;
                if (Session["Valoare1Noua"] != null)
                    dtTemp2 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F075 WHERE F07503 = " + Session["Valoare1Noua"].ToString().Split(';')[1], null);
                else
                    if (cmb1Nou.Value != null && cmb1Nou.SelectedIndex >= 0)
                        dtTemp2 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F075 WHERE F07503 = " + cmb1Nou.Items[cmb1Nou.SelectedIndex].Value.ToString(), null);

                IncarcaComboBox(cmb2Act, cmb2Nou, dtTemp1, dtTemp2);
                lblDoc.Visible = true;
                btnDocUpload.Visible = true;
                btnDocSterge.Visible = true;

            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.BancaGarantii)
            {
                ArataCtl(9, "Banca actuala", "Banca noua", "Sucursala actuala", "Sucursala noua", "IBAN actual", "IBAN nou", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F1001, F075 WHERE F1001026 = F07503 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + "  group by F07503 , F07509", null);
                DataTable dtTemp2 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F075  group by F07503 , F07509", null);


                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
                DataTable dtTemp = General.IncarcaDT("SELECT F1001028 FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();

                dtTemp1 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F1001, F075 WHERE F1001027 = F07504 AND F1001026 = F07503 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = null;
                if (Session["Valoare1Noua"] != null)
                    dtTemp2 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F075 WHERE F07503 = " + Session["Valoare1Noua"].ToString().Split(';')[1], null);
                else
                    if (cmb1Nou.Value != null && cmb1Nou.SelectedIndex >= 0)
                        dtTemp2 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F075 WHERE F07503 = " + cmb1Nou.Items[cmb1Nou.SelectedIndex].Value.ToString(), null);
          
                IncarcaComboBox(cmb2Act, cmb2Nou, dtTemp1, dtTemp2);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.LimbiStraine)
            {
                ArataCtl(10, "Limba", "Limba noua", "Nivel actual", "Nr. ani vorbit actual", "Nivel nou", "Nr. ani vorbit nou", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select \"IdAuto\" AS \"Id\", \"Denumire\" from \"tblLimbi\" ", null);
                DataTable dtTemp2 = General.IncarcaDT("select \"IdAuto\" AS \"Id\", \"Denumire\" from \"tblLimbi\" ", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
                //cmb1Act.SelectedIndex = cmb1Nou.SelectedIndex;

                DataTable dtTemp = General.IncarcaDT("SELECT \"Nivel\", \"NrAniVorbit\" FROM \"Admin_Limbi\" WHERE \"Marca\" = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND \"IdLimba\" = " + cmb1Act.Items[cmb1Act.SelectedIndex].Value.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    txt1Act.Text = dtTemp.Rows[0][0].ToString();
                    txt2Act.Text = dtTemp.Rows[0][1].ToString();
                }
                lblDoc.Visible = true;
                btnDocUpload.Visible = true;
                btnDocSterge.Visible = true;
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.DocId)
            {
                ArataCtl(11, "Tip act ident. actual", "Tip act ident. nou", "Serie si nr. actuale", "Serie si nr. noi", "Emis de (actual)", "Emis de (nou)", "Data elib. actuala", "Data elib. noua", "Data exp. actuala", "Data exp. noua");
                DataTable dtTemp1 = General.IncarcaDT("select F08502 AS \"Id\", F08503 AS \"Denumire\" from F100, F085 WHERE F08502 = F100983 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F08502 AS \"Id\", F08503 AS \"Denumire\" from F085", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                string data1 = "", data2 = "";
                if (Constante.tipBD == 1)
                {
                    data1 = " CONVERT(VARCHAR, F100522, 103) ";
                    data2 = " CONVERT(VARCHAR, F100963, 103) ";
                }
                else
                {
                    data1 = " TO_CHAR(F100522, 'dd/mm/yyyy') ";
                    data2 = " CONVERT(VARCHAR, F100963, 103) ";
                }
                DataTable dtTemp = General.IncarcaDT("SELECT F10052, F100521, " + data1 + " AS F100522, " + data2 + " AS F100963 FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                txt2Act.Text = dtTemp.Rows[0][1].ToString();
                de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][2].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][2].ToString());
                de2Act.Date = Convert.ToDateTime(dtTemp.Rows[0][3].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][3].ToString());
                lblDoc.Visible = true;
                btnDocUpload.Visible = true;
                btnDocSterge.Visible = true;
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PermisAuto)
            {
                ArataCtl(11, "Categoria actuala", "Categoria noua", "Nr. actual", "Nr. nou", "Emis de (actual)", "Emis de (nou)", "Data emit. actuala", "Data emit. noua", "Data exp. actuala", "Data exp. noua");
                DataTable dtTemp1 = General.IncarcaDT("select F71402 AS \"Id\", F71404 AS \"Denumire\" from F100, F714 WHERE F71402 = F10028 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71402 AS \"Id\", F71404 AS \"Denumire\" from F714", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                string data1 = "", data2 = "";
                if (Constante.tipBD == 1)
                {
                    data1 = " CONVERT(VARCHAR, F10024, 103) ";
                    data2 = " CONVERT(VARCHAR, F1001000, 103) ";
                }
                else
                {
                    data1 = " TO_CHAR(F10024, 'dd/mm/yyyy') ";
                    data2 = " CONVERT(VARCHAR, F1001000, 103) ";
                }
                DataTable dtTemp = General.IncarcaDT("SELECT F1001001, F1001002, " + data1 + " AS F10024, " + data2 + " AS F1001000 FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                txt2Act.Text = dtTemp.Rows[0][1].ToString();
                de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][2].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][2].ToString());
                de2Act.Date = Convert.ToDateTime(dtTemp.Rows[0][3].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][3].ToString());
                lblDoc.Visible = true;
                btnDocUpload.Visible = true;
                btnDocSterge.Visible = true;
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.BonusTeamLeader)
            {
                ArataCtl(5, "Valoare actuala", "Valoare noua", "Data actuala", "Data noua", "", "", "", "", "", "");
                string data = "";
                if (Constante.tipBD == 1)
                    data = " CONVERT(VARCHAR, F70406, 103) ";
                else
                    data = " TO_CHAR(F70406, 'dd/mm/yyyy') ";
                DataTable dtTemp = General.IncarcaDT("SELECT F70407, " + data + " AS F70406 FROM F704 WHERE F70404 = 109 AND F70403 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    txt1Act.Text = dtTemp.Rows[0][0].ToString();
                    de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][1].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][1].ToString());
                    de1Nou.Date = DateTime.Now;
                    de1Nou.Enabled = false;
                }
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Suspendare)
            {
                ArataCtl(14, "Motiv suspendare", "", "", "", "", "", "", "Data inceput", "", "Data sfarsit estimata", "Data incetare");
                string op = "+";
                if (Constante.tipBD == 2)
                    op = "||";
                DataTable dtTemp = General.IncarcaDT("select F09002 AS \"Id\", F09004 " + op + " ' - ' " + op + " F09003 AS \"Denumire\" from f090 ", null);        
                IncarcaComboBox(cmb1Act, cmb1Nou, null, dtTemp);
                string circuit = Dami.ValoareParam("FinalizareCuActeAditionale", "0");
                if (circuit == "1")
                    de3Nou.ClientEnabled = false;
            }
            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.RevenireSuspendare)
            {
                ArataCtl(14, "Motiv suspendare", "", "", "", "", "", "", "Data inceput", "", "Data sfarsit estimata", "Data incetare");
                string op = "+";
                if (Constante.tipBD == 2)
                    op = "||";
                DataTable dtTemp = General.IncarcaDT("select F09002 AS \"Id\", F09004 " + op + " ' - ' " + op + " F09003 AS \"Denumire\" from f090 join f111 on f11104 = f09002 and f11103 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() 
                    + " WHERE F11107 IS NULL OR F11107 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") , null);
                IncarcaComboBox(cmb1Act, cmb1Nou, null, dtTemp);
                de1Nou.ClientEnabled = false;
                de2Nou.ClientEnabled = false;
                if (cmb1Nou.SelectedIndex >= 0)
                {
                    DataTable dtTempRev = General.IncarcaDT("select * from f111 Where F11103 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND (F11107 IS NULL OR F11107 = "
                                + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ") AND F11104 = " + cmb1Nou.Items[cmb1Nou.SelectedIndex].Value.ToString() + " ORDER BY F11105", null);
                    if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11105"] != null && dtTempRev.Rows[0]["F11105"].ToString().Length > 0)
                        de1Nou.Value = Convert.ToDateTime(dtTempRev.Rows[0]["F11105"].ToString());
                    else
                        de1Nou.Value = new DateTime(2100, 1, 1);

                    if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11106"] != null && dtTempRev.Rows[0]["F11106"].ToString().Length > 0)
                        de2Nou.Value = Convert.ToDateTime(dtTempRev.Rows[0]["F11106"].ToString());
                    else
                        de2Nou.Value = new DateTime(2100, 1, 1);
                }
            }
            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Detasare)
            {
                ArataCtl(15, "", "Tara detasare", "", "Nume angajator", "", "CUI", "", "Data inceput", "", "Data sfarsit estimata", "Data incetare");
                DataTable dtTemp = General.IncarcaDT("select F73302 AS \"Id\", F73304 AS \"Denumire\" from F733 ", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, null, dtTemp);
                de3Nou.ClientEnabled = false;
            }
            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.RevenireDetasare)
            {
                ArataCtl(15, "", "Tara detasare", "", "Nume angajator", "", "CUI", "", "Data inceput", "", "Data sfarsit estimata", "Data incetare");
                DataTable dtTemp = General.IncarcaDT("select F73302 AS \"Id\", F73304 AS \"Denumire\" from F733 ", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, null, dtTemp);
                cmb1Nou.ClientEnabled = false;
                txt1Nou.ClientEnabled = false;
                txt2Nou.ClientEnabled = false;
                de1Nou.ClientEnabled = false;
                chk1.ClientEnabled = false;
                chk2.ClientEnabled = false;
                chk3.ClientEnabled = false;
                chk4.ClientEnabled = false;
                chk5.ClientEnabled = false;

                DataTable dtTempRev = General.IncarcaDT("select * from f112 Where F11203 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND (F11209 IS NULL OR F11209 = "
                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ")  ORDER BY F11207", null);
                if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11206"] != null && dtTempRev.Rows[0]["F11206"].ToString().Length > 0)
                    cmb1Nou.Value = Convert.ToInt32(dtTempRev.Rows[0]["F11206"].ToString());
                else
                    cmb1Nou.Value = 0;

                if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11204"] != DBNull.Value && dtTempRev.Rows[0]["F11204"].ToString().Length > 0)
                    txt1Nou.Text = dtTempRev.Rows[0]["F11204"].ToString();
                else
                    txt1Nou.Text = "";

                if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11205"] != DBNull.Value && dtTempRev.Rows[0]["F11205"].ToString().Length > 0)
                    txt2Nou.Text = dtTempRev.Rows[0]["F11205"].ToString();
                else
                    txt2Nou.Text = "";

                if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11207"] != DBNull.Value && dtTempRev.Rows[0]["F11207"].ToString().Length > 0)
                    de1Nou.Value = Convert.ToDateTime(dtTempRev.Rows[0]["F11207"].ToString());
                else
                    de1Nou.Value = new DateTime(2100, 1, 1);

                if (dtTempRev != null && dtTempRev.Rows.Count > 0 && dtTempRev.Rows[0]["F11208"] != DBNull.Value && dtTempRev.Rows[0]["F11208"].ToString().Length > 0)
                    de2Nou.Value = Convert.ToDateTime(dtTempRev.Rows[0]["F11208"].ToString());
                else
                    de2Nou.Value = new DateTime(2100, 1, 1);

                if (dtTempRev != null && dtTempRev.Rows.Count > 0)
                {
                    chk1.Checked = dtTempRev.Rows[0]["F11210"] == DBNull.Value ? false : (Convert.ToInt32(dtTempRev.Rows[0]["F11210"].ToString()) == 1 ? true : false);
                    chk2.Checked = dtTempRev.Rows[0]["F11211"] == DBNull.Value ? false : (Convert.ToInt32(dtTempRev.Rows[0]["F11211"].ToString()) == 1 ? true : false);
                    chk3.Checked = dtTempRev.Rows[0]["F11212"] == DBNull.Value ? false : (Convert.ToInt32(dtTempRev.Rows[0]["F11212"].ToString()) == 1 ? true : false);
                    chk4.Checked = dtTempRev.Rows[0]["F11213"] == DBNull.Value ? false : (Convert.ToInt32(dtTempRev.Rows[0]["F11213"].ToString()) == 1 ? true : false);
                    chk5.Checked = dtTempRev.Rows[0]["F11214"] == DBNull.Value ? false : (Convert.ToInt32(dtTempRev.Rows[0]["F11214"].ToString()) == 1 ? true : false);
                }
            }

            //Radu 14.07.2020
            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.ProgramLucru)
            {
                ArataCtl(3, "Program actual", "Program nou", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select\"Id\", \"Denumire\", \"DenumireInAdmin\" from \"Ptj_Contracte\", \"F100Contracte\" WHERE \"IdContract\" = \"Id\" AND \"DataSfarsit\" = " + General.ToDataUniv(new DateTime(2100, 1, 1)) + " AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select \"Id\", \"Denumire\", \"DenumireInAdmin\" from \"Ptj_Contracte\"", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

            }

            //Radu 10.09.2020
            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.TipContract || Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.DurataContract)
            {
                //ArataCtl(17, "Tip contract actual", "Tip contract nou", "Duarata contract actual", "Durata contract nou", "Nr. luni", "Nr. zile", "Data Inceput", "Data Sfarsit", "", "");
                ArataCtl(3, "Tip contract actual", "Tip contract nou", "", "", "", "", "", "", "", "");

                DataTable dtTempCtr = General.IncarcaDT("select * from F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp1 = General.GetTipContract().Select("Id = " + dtTempCtr.Rows[0]["F100984"].ToString()).CopyToDataTable();
                DataTable dtTemp2 = General.GetTipContract();
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                //dtTemp1 = General.IncarcaDT("select F08902 AS \"Id\", F08903 AS \"Denumire\" from F100, F089 WHERE F08902 = F1009741 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                //dtTemp2 = General.IncarcaDT("select F08902 AS \"Id\", F08903 AS \"Denumire\" from F089", null);
                //IncarcaComboBox(cmb2Act, cmb2Nou, dtTemp1, dtTemp2);

                //string data1 = "", data2 = "";
                //if (Constante.tipBD == 1)
                //{
                //    data1 = " CONVERT(VARCHAR, F100933, 103) ";
                //    data2 = " CONVERT(VARCHAR, F100934, 103) ";
                //}
                //else
                //{
                //    data1 = " TO_CHAR(F100933, 'dd/mm/yyyy') ";
                //    data2 = " TO_CHAR(F100934, 'dd/mm/yyyy') ";
                //}

                //DataTable dtTemp = General.IncarcaDT("SELECT " + data1 + " AS F100933, " + data2 + " AS F100934 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                //de1Act.Date = Convert.ToDateTime(dtTemp.Rows[0][0].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][0].ToString());
                //de2Act.Date = Convert.ToDateTime(dtTemp.Rows[0][1].ToString().Length <= 0 ? "01/01/2100" : dtTemp.Rows[0][1].ToString());

                //int nrLuni = 0, nrZile = 0;
                //Personal.Contract ctr = new Personal.Contract();
                //ctr.CalculLuniSiZile(Convert.ToDateTime(de1Act.Date), Convert.ToDateTime(de2Act.Date), out nrLuni, out nrZile);
                //txt3Act.Value = nrLuni;
                //txt4Act.Value = nrZile;

                //if (cmb2Act != null && cmb2Act.Value != null && Convert.ToInt32(cmb2Act.Value) == 1)
                //{
                //    lblTxt14Act.Visible = false;
                //    lblTxt15Act.Visible = false;
                //    txt3Act.Visible = false;
                //    txt4Act.Visible = false;
                //    lblTxt5Act.Visible = false;
                //    lblTxt6Act.Visible = false;
                //    de1Act.Visible = false;
                //    de2Act.Visible = false;
                //}
                //else
                //{
                //    lblTxt14Act.Visible = true;
                //    lblTxt15Act.Visible = true;
                //    txt3Act.Visible = true;
                //    txt4Act.Visible = true;
                //    lblTxt5Act.Visible = true;
                //    lblTxt6Act.Visible = true;
                //    de1Act.Visible = true;
                //    de2Act.Visible = true;
                //}

                //if (cmb2Nou != null && cmb2Nou.Value != null && Convert.ToInt32(cmb2Nou.Value) == 1)
                //{
                //    lblTxt15Nou.Visible = false;
                //    lblTxt16Nou.Visible = false;
                //    txt3Nou.Visible = false;
                //    txt4Nou.Visible = false;
                //    lblTxt5Nou.Visible = false;
                //    lblTxt6Nou.Visible = false;
                //    de1Nou.Visible = false;
                //    de2Nou.Visible = false;
                //    de1Nou.Value = new DateTime(2100, 1, 1);
                //    de2Nou.Value = new DateTime(2100, 1, 1);
                //}
                //else
                //{
                //    lblTxt15Nou.Visible = true;
                //    lblTxt16Nou.Visible = true;
                //    txt3Nou.Visible = true;
                //    txt4Nou.Visible = true;
                //    lblTxt5Nou.Visible = true;
                //    lblTxt6Nou.Visible = true;
                //    de1Nou.Visible = true;
                //    de2Nou.Visible = true;
                //}

            }
        }



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {

                string tip = e.Parameter.Split(';')[0];
                switch (tip)
                {
                    case "1":
                        Session["Avs_ChkGen"] = null;
                        txtDataMod.Date = DateTime.Now;
                        AscundeCtl();
                        IncarcaDate();
                        string data = "";
                        int idAtr = Convert.ToInt32(cmbAtribute.Value);
                        SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out data);
                        if (idAtr == (int)Constante.Atribute.Functie || idAtr == (int)Constante.Atribute.CodCOR || idAtr == (int)Constante.Atribute.Norma || idAtr == (int)Constante.Atribute.ProgramLucru || idAtr == (int)Constante.Atribute.PrelungireCIM
                                || idAtr == (int)Constante.Atribute.PrelungireCIM_Vanz || idAtr == (int)Constante.Atribute.ContrITM || idAtr == (int)Constante.Atribute.ContrIn ||
                                idAtr == (int)Constante.Atribute.Salariul || idAtr == (int)Constante.Atribute.Sporuri || idAtr == (int)Constante.Atribute.MotivPlecare
                                || idAtr == (int)Constante.Atribute.Suspendare || idAtr == (int)Constante.Atribute.Detasare || idAtr == (int)Constante.Atribute.RevenireSuspendare || idAtr == (int)Constante.Atribute.RevenireDetasare
                                || idAtr == (int)Constante.Atribute.TipContract || idAtr == (int)Constante.Atribute.DurataContract)
                            if (Convert.ToDateTime(deDataRevisal.Value).Date < DateTime.Now.Date)
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");                               
                            }


                        string idExcluse = "," + Dami.ValoareParam("IdExcluseCircuitDoc") + ",";
                        if (Dami.ValoareParam("FinalizareCuActeAditionale") == "0" || (Dami.ValoareParam("FinalizareCuActeAditionale") == "1" && idExcluse.IndexOf("," + idAtr + ",") >= 0))
                        {
                            chkGen.ClientVisible = false;
                            chkGen.Checked = false;
                            Session["Avs_ChkGen"] = "false";
                        }
                        else
                        {
                            chkGen.ClientVisible = true;
                            chkGen.Checked = true;
                            Session["Avs_ChkGen"] = "true";
                        }
                        if (idAtr == (int)Constante.Atribute.Functie || idAtr == (int)Constante.Atribute.CodCOR)
                        {
                            DataTable dtCtr = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                            Session["Avs_DurCtr"] = dtCtr.Rows[0]["F1009741"].ToString();
                            Session["Avs_NrLuni"] = dtCtr.Rows[0]["F100935"].ToString();
                            Session["Avs_NrZile"] = dtCtr.Rows[0]["F100936"].ToString();
                            Session["Avs_GrdInv"] = dtCtr.Rows[0]["F10027"].ToString();                            
                        }
                        break;                        
                    case "2":
                        if (e.Parameter.Split(';')[1] == "cmb1Nou")
                            Session["Valoare1Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb2Nou")
                            Session["Valoare2Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb3Nou")
                            Session["Valoare3Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb4Nou")
                            Session["Valoare4Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb5Nou")
                            Session["Valoare5Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb6Nou")
                            Session["Valoare6Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb7Nou")
                            Session["Valoare7Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmb8Nou")
                            Session["Valoare9Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "cmbStructOrgNou")
                            Session["Valoare8Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];


                        //Florin 2019.12.19
                        if (e.Parameter.Split(';')[1] == "txt1Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Salariul)
                        {
                            //CalcSalariu(1, txt1Nou, txt2Nou);
                            decimal venitCalculat = 0m;
                            string text = "";
                            General.CalcSalariu(1, txt1Nou.Value, Convert.ToInt32(General.Nz(cmbAng.Value, -99)), out venitCalculat, out text);
                            txt2Nou.Value = venitCalculat;
                        }
                        if (e.Parameter.Split(';')[1] == "txt2Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Salariul)
                        {
                            //CalcSalariu(2, txt1Nou, txt2Nou);
                            decimal venitCalculat = 0m;
                            string text = "";
                            General.CalcSalariu(2, txt2Nou.Value, Convert.ToInt32(General.Nz(cmbAng.Value, -99)), out venitCalculat, out text);
                            txt1Nou.Value = venitCalculat;
                        }


                        if ((e.Parameter.Split(';')[1] == "de1Nou" || e.Parameter.Split(';')[1] == "de2Nou") && (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM ||
                            Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM_Vanz))
                        {
                            if (de1Nou.Value != null && de2Nou.Value != null)
                            {
                                int nrLuni = 0, nrZile = 0;
                                Personal.Contract ctr = new Personal.Contract();
                                ctr.CalculLuniSiZile(Convert.ToDateTime(de1Nou.Date), Convert.ToDateTime(de2Nou.Date), out nrLuni, out nrZile);
                                txt1Nou.Value = nrLuni;
                                txt2Nou.Value = nrZile;
                            }
                        }
                        if ((e.Parameter.Split(';')[1] == "de1Nou" || e.Parameter.Split(';')[1] == "de2Nou") && (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.TipContract ||
                            Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.DurataContract))
                        {
                            if (de1Nou.Value != null && de2Nou.Value != null)
                            {
                                int nrLuni = 0, nrZile = 0;
                                Personal.Contract ctr = new Personal.Contract();
                                ctr.CalculLuniSiZile(Convert.ToDateTime(de1Nou.Date), Convert.ToDateTime(de2Nou.Date), out nrLuni, out nrZile);
                                txt3Nou.Value = nrLuni;
                                txt4Nou.Value = nrZile;
                            }
                        }

                        if ((e.Parameter.Split(';')[1] == "cmb1Nou") && (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM ||
                            Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM_Vanz))
                        {
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 0)
                            {
                                de1Nou.Value = null;
                                de2Nou.Value = null;
                                txt1Nou.Value = "";
                                txt2Nou.Value = "";
                            }
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 1)
                            {
                                de1Nou.Value = new DateTime(2100, 1, 1);
                                de2Nou.Value = new DateTime(2100, 1, 1);
                                txt1Nou.Value = "";
                                txt2Nou.Value = "";
                            }
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 2)
                            {
                                if (de2Act.Value != null && Convert.ToDateTime(de2Act.Value) != new DateTime(2100, 1, 1))
                                    de1Nou.Value = Convert.ToDateTime(de2Act.Value).AddDays(1);
                                else
                                    de1Nou.Value = null;
                                de2Nou.Value = null;                   
                            }
                        }
                        if ((e.Parameter.Split(';')[1] == "cmb2Nou") && (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.TipContract ||
                            Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.DurataContract))
                        {
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 0)
                            {
                                de1Nou.Value = null;
                                de2Nou.Value = null;
                                txt3Nou.Value = "";
                                txt4Nou.Value = "";
                            }
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 1)
                            {
                                de1Nou.Value = new DateTime(2100, 1, 1);
                                de2Nou.Value = new DateTime(2100, 1, 1);
                                txt3Nou.Value = "";
                                txt4Nou.Value = "";
                            }
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 2)
                            {
                                if (de2Act.Value != null && Convert.ToDateTime(de2Act.Value) != new DateTime(2100, 1, 1))
                                    de1Nou.Value = Convert.ToDateTime(de2Act.Value).AddDays(1);
                                else
                                    de1Nou.Value = null;
                                de2Nou.Value = null;
                            }
                        }

                        if ((e.Parameter.Split(';')[1] == "cmb1Nou") && (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Detasare ||
                                Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.RevenireDetasare))
                        {
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 1)
                            {
                                chk2.Checked = false;
                                chk3.Checked = false;
                                chk4.Checked = false;
                                chk5.Checked = false;
                                chk2.ClientEnabled = false;
                                chk3.ClientEnabled = false;
                                chk4.ClientEnabled = false;
                                chk5.ClientEnabled = false;
                                Session["Avs_ChkEnabled"] = "false";
                            }
                            else
                            {
                                chk2.ClientEnabled = true;
                                chk3.ClientEnabled = true;
                                chk4.ClientEnabled = true;
                                chk5.ClientEnabled = true;
                                Session["Avs_ChkEnabled"] = "true"; 
                            }
                        }    

                        IncarcaDate();
                        SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out data);
                        if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Norma)
                            SetNorma(e.Parameter.Split(';')[1]);

                        if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.BancaSalariu || Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.BancaGarantii)
                        {
                            if (e.Parameter.Split(';')[1] == "txt1Nou" && !IbanValid(txt1Nou.Text))
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Cont IBAN invalid!");
                                txt1Nou.Value = "";
                            }
                        }

                        if (e.Parameter.Split(';')[1] == "de2Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Suspendare)
                        {
                            string sqlVerif = "SELECT COUNT(*) FROM F111 WHERE F11103 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND (F11107 IS NULL OR F11107 = "
                                            + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ") ";
                            DataTable dtVerif = General.IncarcaDT(sqlVerif, null);
                            if (dtVerif != null && dtVerif.Rows.Count > 0 && dtVerif.Rows[0][0] != null && Convert.ToInt32(dtVerif.Rows[0][0].ToString()) > 0)
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Atentie!\nAngajatul are cel putin o suspendare activa!");
                            }
                        }

                        if (e.Parameter.Split(';')[1] == "cmb2Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Norma)
                        {
                            SetTarif(e.Parameter.Split(';')[2]);
                        }

                        if (e.Parameter.Split(';')[1] == "cmb1Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Norma && e.Parameter.Split(';')[2] == "2")
                        {
                            SetTarif(e.Parameter.Split(';')[2]);
                        }

                        if (e.Parameter.Split(';')[1] == "cmb2Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Functie)
                        {
                            CompletareZile();
                            ValidareZile(1);
                        }

                        if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Functie || Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.CodCOR)
                        {
                            if (e.Parameter.Split(';')[1] == "txt1Nou" || e.Parameter.Split(';')[1] == "txt2Nou" || e.Parameter.Split(';')[1] == "txt3Nou" || e.Parameter.Split(';')[1] == "txt4Nou")
                                ValidareZile(0);
                        }
                        break;
                    case "3":
                        {

                        }
                        break;
                    case "4":
                        if (SalveazaDate())
                        {
                            Session["Avs_MarcaFiltru"] = cmbAngFiltru.SelectedIndex;
                            Session["Avs_AtributFiltru"] = cmbAtributeFiltru.SelectedIndex;
                            //Florin 2019.05.23
                            //IncarcaGrid(Convert.ToInt32(General.Nz(cmbAng.Value, -99)));
                            cmbAngFiltru.Value = cmbAng.Value;
                            grDate.DataSource = null;

                            Session["Avs_MarcaFiltru"] = cmbAngFiltru.SelectedIndex;
                            Session["Avs_AtributFiltru"] = cmbAtributeFiltru.SelectedIndex;
                            IncarcaGrid();
                            txtDataMod.Date = DateTime.Now;
                        }
                        break;
                    case "5":
                        Session["Avs_MarcaFiltru"] = cmbAngFiltru.SelectedIndex;
                        Session["Avs_AtributFiltru"] = cmbAtributeFiltru.SelectedIndex;
                        IncarcaGrid();
                        break;
                    case "6":
                        StergeFiltre();
                        break;
                    case "7":
                        if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Organigrama)
                        {
                            DataTable dtTemp1 = General.GetStructOrgAng(Convert.ToInt32(cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()));
                            DataTable dtTemp2 = General.GetStructOrgModif(Convert.ToDateTime(txtDataMod.Value));
                            IncarcaComboBox(cmbStructOrgAct, cmbStructOrgNou, dtTemp1, dtTemp2);
                        }
                        data = "";
                        SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out data);
                        string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + Convert.ToDateTime(txtDataMod.Value).Year;
                        if (Constante.tipBD == 2)
                            strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + Convert.ToDateTime(txtDataMod.Value).Year;
                        DataTable dtHolidays = General.IncarcaDT(strSql, null);
                        bool ziLibera = EsteZiLibera(Convert.ToDateTime(txtDataMod.Value), dtHolidays);
                        if (Convert.ToDateTime(txtDataMod.Value).DayOfWeek.ToString().ToLower() == "saturday" || Convert.ToDateTime(txtDataMod.Value).DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data modificarii este intr-o zi nelucratoare!");
                        }
                        idAtr = Convert.ToInt32(cmbAtribute.Value);
                        if (idAtr == (int)Constante.Atribute.Functie || idAtr == (int)Constante.Atribute.CodCOR || idAtr == (int)Constante.Atribute.Norma || idAtr == (int)Constante.Atribute.ProgramLucru || idAtr == (int)Constante.Atribute.PrelungireCIM
                          || idAtr == (int)Constante.Atribute.PrelungireCIM_Vanz || idAtr == (int)Constante.Atribute.ContrITM || idAtr == (int)Constante.Atribute.ContrIn ||
                          idAtr == (int)Constante.Atribute.Salariul || idAtr == (int)Constante.Atribute.Sporuri || idAtr == (int)Constante.Atribute.MotivPlecare
                          || idAtr == (int)Constante.Atribute.Suspendare || idAtr == (int)Constante.Atribute.Detasare || idAtr == (int)Constante.Atribute.RevenireSuspendare || idAtr == (int)Constante.Atribute.RevenireDetasare
                           || idAtr == (int)Constante.Atribute.TipContract || idAtr == (int)Constante.Atribute.DurataContract)
                            if (Convert.ToDateTime(deDataRevisal.Value).Date < DateTime.Now.Date)
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");
                            }             
                        break;
                    case "8":               //cmbRol
                        {
                            DataTable dtAng = General.IncarcaDT(SelectAngajati(Convert.ToInt32(cmbRol.Value ?? -99)), null);
                            cmbAng.DataSource = dtAng;
                            Session["Modif_Avans_Angajati"] = dtAng;
                            cmbAng.DataBind();
                            //cmbAng.SelectedIndex = 0;
                            cmbAng.SelectedIndex = -1;
                            //cmbAtribute.SelectedIndex = -1;
                            //cmbAtribute.DataSource = null;
                            //cmbAtribute.DataBind();

                            ////acelasi cod ca la case "9"
                            DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAng.Value, -99) });
                            cmbAtribute.DataSource = dtAtr;
                            cmbAtribute.DataBind();
                            //cmbAtributeFiltru.DataSource = dtAtr;
                            //cmbAtributeFiltru.DataBind();

                            AscundeCtl();
                            txtExpl.Text = "";
                            cmbAtribute.Value = null;
                        }
                        break;
                    case "9":               //cmbAng
                        {
                            txtDataMod.Date = DateTime.Now;
                            DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAng.Value, -99) });
                            cmbAtribute.DataSource = dtAtr;
                            cmbAtribute.DataBind();
                            //cmbAtributeFiltru.DataSource = dtAtr;
                            //cmbAtributeFiltru.DataBind();
                            Session["AvsCereri"] = null;
                            Session["AvsCereriCalcul"] = null;
                            Session["Avs_ChkEnabled"] = null;
                            AscundeCtl();
                            cmbAtribute.Value = null;
                        }
                        break;
                    case "10":                //Document
                        break;
                    case "11":               //cmbAngFiltru
                        {
                            DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAngFiltru.Value, -99) });                 
                            cmbAtributeFiltru.DataSource = dtAtr;
                            cmbAtributeFiltru.DataBind();
                            Session["AvsCereri"] = null;
                            Session["AvsCereriCalcul"] = null;
                            //AscundeCtl();
                            cmbAtributeFiltru.Value = null;
                            Session["Avs_MarcaFiltru1"] = Convert.ToInt32(cmbAngFiltru.Value);
                        }
                        break;

                }



            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SetTarif(string val)
        {
            DataTable dt = new DataTable();
            DataSet ds = Session["AvsCereri"] as DataSet;
            if (ds == null)
            {
                ds = new DataSet();

                dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);
            }

            DataTable dtZL = General.IncarcaDT("SELECT * FROM F069 WHERE F06904 = (SELECT F01011 FROM F010) AND F06905 = (SELECT F01012 FROM F010)", null);
            DataTable dtTarife = General.IncarcaDT("SELECT * FROM F011", null);
            int poz = 0, valoare = 0;
            int zile_lucratoare_luna = Convert.ToInt32(dtZL.Rows[0]["F06907"].ToString());
            int ore_lucratoare_luna = zile_lucratoare_luna * Convert.ToInt32(val);
            bool gasit = false;
            for (int i = 0; i < dtTarife.Rows.Count; i++)
            {
                if (Convert.ToInt32(General.Nz(dtTarife.Rows[i]["F01114"], "0").ToString()) == 1 && Convert.ToDecimal(General.Nz(dtTarife.Rows[i]["F01108"], "0").ToString()) == ore_lucratoare_luna)
                {
                    gasit = true;
                    poz = Convert.ToInt32(dtTarife.Rows[i]["F01104"].ToString());
                    valoare = Convert.ToInt32(dtTarife.Rows[i]["F01105"].ToString());
                    break;
                }
            }
            if (gasit)
            {
                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                string sirNou = "";
                for (int i = 0; i < sir.Length; i++)
                    if (i == poz - 1)
                        sirNou += valoare.ToString();
                    else
                        sirNou += sir[i];

                ds.Tables[0].Rows[0]["F10067"] = sirNou;
            }
            else
                if (Convert.ToDateTime(txtDataMod.Value).Month == General.DamiDataLucru().Month && Convert.ToDateTime(txtDataMod.Value).Year == General.DamiDataLucru().Year)
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu a fost gasit tarif corespunzator normei selectate! Setati tariful manual!");
            Session["AvsCereri"] = ds;
        }

        private void SetNorma(string cmb)
        {
            switch (cmb)
            {
                case "cmb1Nou":
                    if (Convert.ToInt32(cmb1Nou.Value) == 0)
                    {                    
                        cmb2Nou.Value = 8;
                        cmb3Nou.Value = 8;
                        cmb4Nou.Value = 1;
                        cmb5Nou.Value = 1;
                        cmb6Nou.Value = 1;
                        Session["Valoare2Noua"] = "cmb2Nou;8";
                        Session["Valoare3Noua"] = "cmb3Nou;8";
                        Session["Valoare4Noua"] = "cmb4Nou;1";
                        Session["Valoare5Noua"] = "cmb5Nou;1";
                        Session["Valoare6Noua"] = "cmb6Nou;1";
                        IncarcaDate();
                    }
                    if (Convert.ToInt32(cmb1Nou.Value) == 2)
                    {                   
                        cmb2Nou.Value = 1;
                        cmb3Nou.Value = 8;
                        cmb4Nou.Value = 2;
                        cmb5Nou.Value = 5;
                        cmb6Nou.Value = 1;
                        Session["Valoare2Noua"] = "cmb2Nou;1";
                        Session["Valoare3Noua"] = "cmb3Nou;8";
                        Session["Valoare4Noua"] = "cmb4Nou;2";
                        Session["Valoare5Noua"] = "cmb5Nou;5";
                        Session["Valoare6Noua"] = "cmb6Nou;1";
                        IncarcaDate();
                    }    
                    break;
                case "cmb2Nou":
                    if (Convert.ToInt32(cmb2Nou.Value ?? 1) > Convert.ToInt32(cmb3Nou.Value ?? 8))
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timpul partial este mai mare decat norma!");
                        cmb2Nou.Value = 1;
                        Session["Valoare2Noua"] = "cmb2Nou;1";
                        IncarcaDate();
                    }
                    if (Convert.ToInt32(cmb1Nou.Value) == 2 && Convert.ToInt32(cmb2Nou.Value ?? 1) == 8)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timpul partial nu poate fi de 8 ore pentru angajat de tip partial!");
                        cmb2Nou.Value = 1;
                        Session["Valoare2Noua"] = "cmb2Nou;1";
                        IncarcaDate();
                    }
                    break; 
                case "cmb3Nou":
                    {
                        cmb2Nou.Value = Convert.ToInt32(cmb3Nou.Value ?? 8);
                        Session["Valoare2Noua"] = "cmb2Nou;" + Convert.ToInt32(cmb3Nou.Value ?? 8);
                    }
                    break;
                case "cmb4Nou":
                    if (Convert.ToInt32(cmb1Nou.Value) == 2 && Convert.ToInt32(cmb4Nou.Value) != 2)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti schimba tip norma pentru tip angajat partial!");
                        cmb4Nou.Value = 2;
                        Session["Valoare4Noua"] = "cmb4Nou;2";
                        IncarcaDate();
                    }
                    break;
                case "cmb5Nou":
                    if (Convert.ToInt32(cmb5Nou.Value ?? -1) == 2 && txt1Nou.Text.Length > 0 && Convert.ToInt32(txt1Nou.Text) > 30)
                        txt1Nou.Text = "30";                   
                    if (Convert.ToInt32(cmb1Nou.Value) == 2 && Convert.ToInt32(cmb5Nou.Value) != 5)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti schimba durata pentru tip angajat partial!");
                        cmb5Nou.Value = 5;
                        Session["Valoare5Noua"] = "cmb5Nou;5";
                        IncarcaDate();
                    }
                    break;
                case "cmb7Nou":
                    if (Convert.ToInt32(cmb7Nou.Value) == 2 || Convert.ToInt32(cmb7Nou.Value) == 3)
                        txt1Nou.Enabled = true;
                    else
                    {
                        txt1Nou.Text = "";
                        txt1Nou.Enabled = false;
                    }
                    break;
                case "txt1Nou":
                    if (Convert.ToInt32(cmb7Nou.Value ?? -1) == 2 && Convert.ToInt32(cmb5Nou.Value ?? -1) > 0)
                    {
                        if (Convert.ToInt32(cmb5Nou.Value ?? -1) == 1 && Convert.ToInt32(txt1Nou.Text) > 40)
                        {// 8/40
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti introduce mai mult de 40 ore/saptamana!");
                            txt1Nou.Text = "40";
                        }
                        if (Convert.ToInt32(cmb5Nou.Value ?? -1) == 2 && Convert.ToInt32(txt1Nou.Text) > 30)
                        {// 6/30
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti introduce mai mult de 30 ore/saptamana!");
                            txt1Nou.Text = "30";
                        }
                    }
                    else
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati introdus intervalul corect!");
                        txt1Nou.Text = "";
                    }
                    break;
            }
        }

        private void SetDataRevisal(int param, DateTime dataMod, int atribut, out string data)
        {
            data = "";
            lblDataRevisal.Visible = false;
            deDataRevisal.Visible = false;
            if (atribut == (int)Constante.Atribute.Functie || atribut == (int)Constante.Atribute.CodCOR || atribut == (int)Constante.Atribute.Norma || atribut == (int)Constante.Atribute.ProgramLucru || atribut == (int)Constante.Atribute.PrelungireCIM
                || atribut == (int)Constante.Atribute.PrelungireCIM_Vanz || atribut == (int)Constante.Atribute.ContrITM || atribut == (int)Constante.Atribute.ContrIn ||
                 atribut == (int)Constante.Atribute.Salariul || atribut == (int)Constante.Atribute.Sporuri || atribut == (int)Constante.Atribute.MotivPlecare
                || atribut == (int)Constante.Atribute.Suspendare || atribut == (int)Constante.Atribute.Detasare || atribut == (int)Constante.Atribute.RevenireSuspendare || atribut == (int)Constante.Atribute.RevenireDetasare 
                || atribut == (int)Constante.Atribute.TipContract || atribut == (int)Constante.Atribute.DurataContract)
            {
                string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataMod.Year + " UNION SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + (dataMod.Year - 1).ToString();
                if (Constante.tipBD == 2)
                    strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataMod.Year + " UNION SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY)  = " + (dataMod.Year - 1).ToString();
                DataTable dtHolidays = General.IncarcaDT(strSql, null);
                DateTime dataRevisal = dataMod.AddDays(-1);
                bool ziLibera = EsteZiLibera(dataRevisal, dtHolidays);
                while (dataRevisal.DayOfWeek.ToString().ToLower() == "saturday" || dataRevisal.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                {
                    dataRevisal = dataRevisal.AddDays(-1);
                    ziLibera = EsteZiLibera(dataRevisal, dtHolidays);
                }
                deDataRevisal.Value = dataRevisal;
                if (param == 1)
                {
                    lblDataRevisal.Visible = true;
                    deDataRevisal.Visible = true;
                }
                else
                    data = dataRevisal.Day.ToString().PadLeft(2, '0') + "/" + dataRevisal.Month.ToString().PadLeft(2, '0') + "/" + dataRevisal.Year.ToString();
            }

            //Radu 06.01.2020
            if (atribut == (int)Constante.Atribute.MotivPlecare)
            {
                string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataMod.Year + " UNION SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + (dataMod.Year - 1).ToString();
                if (Constante.tipBD == 2)
                    strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataMod.Year + " UNION SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY)  = " + (dataMod.Year - 1).ToString();
                DataTable dtHolidays = General.IncarcaDT(strSql, null);
                DateTime dataRevisal = dataMod;
                bool ziLibera = EsteZiLibera(dataRevisal, dtHolidays);
                if (dataRevisal.DayOfWeek.ToString().ToLower() == "saturday" || dataRevisal.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                {
                    if (param == 1)
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Atentie: data incetarii este zi nelucratoare!");
                }
                deDataRevisal.Value = dataRevisal;
                if (param == 1)
                {
                    lblDataRevisal.Visible = true;
                    deDataRevisal.Visible = true;
                }
                else
                    data = dataRevisal.Day.ToString().PadLeft(2, '0') + "/" + dataRevisal.Month.ToString().PadLeft(2, '0') + "/" + dataRevisal.Year.ToString();
            }

            if (atribut == (int)Constante.Atribute.Salariul || atribut == (int)Constante.Atribute.Sporuri)
            {
                string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataMod.Year + " UNION SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + (dataMod.Year - 1).ToString();
                if (Constante.tipBD == 2)
                    strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataMod.Year + " UNION SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY)  = " + (dataMod.Year - 1).ToString();
                DataTable dtHolidays = General.IncarcaDT(strSql, null);
                DateTime dataRevisal = dataMod;
                int i = 0;
                while (i < 19)
                {
                    dataRevisal = dataRevisal.AddDays(1);
                    bool ziLibera = EsteZiLibera(dataRevisal, dtHolidays);
                    if (dataRevisal.DayOfWeek.ToString().ToLower() != "saturday" && dataRevisal.DayOfWeek.ToString().ToLower() != "sunday" && !ziLibera)
                        i++;
                }
                deDataRevisal.Value = dataRevisal;
                if (param == 1)
                {
                    lblDataRevisal.Visible = true;
                    deDataRevisal.Visible = true;
                }
                else
                    data = dataRevisal.Day.ToString().PadLeft(2, '0') + "/" + dataRevisal.Month.ToString().PadLeft(2, '0') + "/" + dataRevisal.Year.ToString();
            }


        }

        private bool EsteZiLibera(DateTime data, DataTable dtHolidays)
        {
            bool ziLibera = false;
            for (int z = 0; z < dtHolidays.Rows.Count; z++)
                if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == data.Date)
                {
                    ziLibera = true;
                    break;
                }
            return ziLibera;
        }

        private bool SalveazaDate()
        {
            try
            {
                string strErr = "";
                string dataRev = "";
                int idAtr = -99;

                int val = 1;
                string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TermenDepasireRevisal'";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                    val = Convert.ToInt32(dt.Rows[0][0].ToString());

                idAtr = Convert.ToInt32((cmbAtribute.Value ?? -99));
                SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out dataRev);
                if (idAtr == (int)Constante.Atribute.Functie || idAtr == (int)Constante.Atribute.CodCOR || idAtr == (int)Constante.Atribute.Norma || idAtr == (int)Constante.Atribute.ProgramLucru || idAtr == (int)Constante.Atribute.PrelungireCIM
                        || idAtr == (int)Constante.Atribute.PrelungireCIM_Vanz || idAtr == (int)Constante.Atribute.ContrITM || idAtr == (int)Constante.Atribute.ContrIn ||
                        idAtr == (int)Constante.Atribute.Salariul || idAtr == (int)Constante.Atribute.Sporuri || idAtr == (int)Constante.Atribute.MotivPlecare
                         || idAtr == (int)Constante.Atribute.TipContract || idAtr == (int)Constante.Atribute.DurataContract)
                    if (Convert.ToDateTime(deDataRevisal.Value).Date < DateTime.Now.Date && val == 1)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");
                        return false;
                    }

                int afisare = Convert.ToInt32(Dami.ValoareParam("Avs_CampuriZileProbaPreavizCOR", "0"));
                if (idAtr == (int)Constante.Atribute.Functie || (afisare == 1 && idAtr == (int)Constante.Atribute.CodCOR))
                {
                    if (Dami.ValoareParam("ValidariPersonal") == "1")
                    {
                        string mesajF = "";
                        if ((txt1Nou.Text.Length <= 0 || txt1Nou.Text == "0") && (txt2Nou.Text.Length <= 0 || txt2Nou.Text == "0"))
                            mesajF += " - perioada de proba (zile lucratoare sau zile calendaristice)" + Environment.NewLine;
                        if (txt3Nou.Text.Length <= 0 || txt3Nou.Text == "0")
                            mesajF += " - nr zile preaviz demisie" + Environment.NewLine;
                        if (txt4Nou.Text.Length <= 0 || txt4Nou.Text == "0")
                            mesajF += " - nr zile preaviz concediere" + Environment.NewLine;

                        if (mesajF.Length > 0)
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = "Nu ati completat: " + Environment.NewLine + mesajF;
                            return false;
                        }
                    }
                }

                if (idAtr == (int)Constante.Atribute.Suspendare || idAtr == (int)Constante.Atribute.Detasare)
                {
                    if (Convert.ToDateTime(deDataRevisal.Value).Date >= Convert.ToDateTime(de1Nou.Value).Date && val == 1)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");
                        SetDataRevisal(1, Convert.ToDateTime(de1Nou.Value).AddDays(-1), Convert.ToInt32(cmbAtribute.Value), out dataRev);
                        return false;
                    }

                    string data1 = Convert.ToDateTime(de1Nou.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de1Nou.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de1Nou.Value).Year.ToString();
                    string data2 = Convert.ToDateTime(de2Nou.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de2Nou.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de2Nou.Value).Year.ToString();                
                    string sqlVerif = "SELECT COUNT(*) FROM F112 WHERE F11203 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND (F11209 IS NULL OR F11209 = "
                                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ") AND ((F11207 <= " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data1 + "', 103)" : "TO_DATE('" + data1 + "', 'dd/mm/yyyy')")
                                     + " AND " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data1 + "', 103)" : "TO_DATE('" + data1 + "', 'dd/mm/yyyy')") + " <= F11208) "
                                     + " OR (F11207 <= " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data2 + "', 103)" : "TO_DATE('" + data2 + "', 'dd/mm/yyyy')") + " AND " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data2 + "', 103)" : "TO_DATE('" + data2 + "', 'dd/mm/yyyy')") + " <= F11208))";
                    DataTable dtVerif = General.IncarcaDT(sqlVerif, null);
                    if (dtVerif != null && dtVerif.Rows.Count > 0 && dtVerif.Rows[0][0] != null && Convert.ToInt32(dtVerif.Rows[0][0].ToString()) > 0)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul are o detasare activa!");
                        return false;
                    }

                    string sqlDet = "SELECT * FROM F112 WHERE F11203 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " ORDER BY F11207 DESC";
                    DataTable dtDet = General.IncarcaDT(sqlDet, null);
                    if (dtDet != null && dtDet.Rows.Count > 0)
                    {
                        if (Convert.ToDateTime(dtDet.Rows[0]["F11209"] == DBNull. Value ? "01/01/2100" : dtDet.Rows[0]["F11209"].ToString()) > Convert.ToDateTime(de1Nou.Value))
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Detasarea se suprapune cu cea precedenta!");
                            return false;
                        }
                    }

                }

                if (idAtr == (int)Constante.Atribute.Salariul)
                {
                    string data1 = Convert.ToDateTime(txtDataMod.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(txtDataMod.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(txtDataMod.Value).Year.ToString();
                    string sqlVerif = "SELECT COUNT(*) FROM F111 WHERE F11103 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND (F11107 IS NULL OR F11107 = "
                                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ") AND F11105 <= " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data1 + "', 103)" : "TO_DATE('" + data1 + "', 'dd/mm/yyyy')")
                                     + " AND " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data1 + "', 103)" : "TO_DATE('" + data1 + "', 'dd/mm/yyyy')") + " <= F11106";
                    DataTable dtVerif = General.IncarcaDT(sqlVerif, null);
                    if (dtVerif != null && dtVerif.Rows.Count > 0 && dtVerif.Rows[0][0] != null && Convert.ToInt32(dtVerif.Rows[0][0].ToString()) > 0)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data modificarii este intr-o perioada in care angajatul este suspendat!");
                        return false;
                    }
                }

                if (idAtr == (int)Constante.Atribute.Suspendare)
                {
                    if (Convert.ToDateTime(de1Nou.Value).Date != Convert.ToDateTime(txtDataMod.Value).Date)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data modificarii trebuie sa fie egala cu Data inceput suspendare!");
                        return false;
                    }

                    DataTable dtAng = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString());
                    if (General.Nz(dtAng.Rows[0]["F1009741"], "").ToString() == "2")
                    {
                        if (Convert.ToDateTime(dtAng.Rows[0]["F10023"].ToString()).Date < Convert.ToDateTime(de2Nou.Value).Date)
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data sfarsit estimata este ulterioara datei ultimei zile de plata!");
                            return false;
                        }
                    }

                    string data1 = Convert.ToDateTime(de1Nou.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de1Nou.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de1Nou.Value).Year.ToString();
                    string data2 = Convert.ToDateTime(de2Nou.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de2Nou.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(de2Nou.Value).Year.ToString();
                    string sqlVerif = "SELECT COUNT(*) FROM F111 WHERE F11103 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " AND (F11107 IS NULL OR F11107 = "
                                    + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ") AND ((F11105 <= " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data1 + "', 103)" : "TO_DATE('" + data1 + "', 'dd/mm/yyyy')")
                                     + " AND " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data1 + "', 103)" : "TO_DATE('" + data1 + "', 'dd/mm/yyyy')") + " <= F11106) "
                                     + " OR (F11105 <= " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data2 + "', 103)" : "TO_DATE('" + data2 + "', 'dd/mm/yyyy')") + " AND " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + data2 + "', 103)" : "TO_DATE('" + data2 + "', 'dd/mm/yyyy')") + " <= F11106))";
                    DataTable dtVerif = General.IncarcaDT(sqlVerif, null);
                    if (dtVerif != null && dtVerif.Rows.Count > 0 && dtVerif.Rows[0][0] != null && Convert.ToInt32(dtVerif.Rows[0][0].ToString()) > 0)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Intervalul introdus se suprapune cu cel al unei suspendari active!");
                        return false;
                    }
                }

                if (idAtr == (int)Constante.Atribute.RevenireSuspendare || idAtr == (int)Constante.Atribute.RevenireDetasare)
                {
                    if (Convert.ToDateTime(deDataRevisal.Value).Date >= Convert.ToDateTime(de3Nou.Value).Date && val == 1)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");
                        SetDataRevisal(1, Convert.ToDateTime(de3Nou.Value).AddDays(-1), Convert.ToInt32(cmbAtribute.Value), out dataRev);
                        return false;
                    }
                }

                if (idAtr == (int)Constante.Atribute.RevenireSuspendare)
                {
                    if (Convert.ToDateTime(de3Nou.Value).Date != Convert.ToDateTime(txtDataMod.Value).Date)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data modificarii trebuie sa fie egala cu Data incetare suspendare!");
                        return false;
                    }
                }

                if (idAtr == (int)Constante.Atribute.Norma && Convert.ToInt32(cmb6Nou.Value) == 3 && Convert.ToInt32(cmb7Nou.Value ?? -1) >= 2 && txt1Nou.Text.Length <= 0)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pentru repartizare inegala trebuie sa completati intervalul si numarul de ore!");
                    return false;
                }

                //verificare ca salariul sa nu fie mai mic decat cel minim (in functie si de timp partial) la schimbare norma sau salariu
                if (idAtr == (int)Constante.Atribute.Norma)
                {
                    string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                    //DataTable dtSal = General.IncarcaDT("SELECT COALESCE(F100699, 0) FROM F100 WHERE F10003 = " + F10003, null);
                    sql = "select f100.f10003 as Marca, case when f100991 is null or f100991 = convert(datetime, '01/01/2100', 103) then  "
                                            + "  convert(datetime, '01/' + convert(varchar, f01012) + '/' + convert(varchar, f01011), 103) "
                                            + " else f100991 end as Data, COALESCE(" + salariu + ", 0) as Valoare from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                                            + " union "
                                            + " select f70403 as Marca, f70406 as Data, COALESCE(F70407, 0) as Valoare from f704 where f70404 = 1 and f70420 = 0 "
                                            + " union "
                                            + " select f10003 as Marca, datamodif as Data, COALESCE(SalariulBrut, 0) as Valoare from Avs_Cereri where IdAtribut = 1 and IdStare in (1, 2, 3)";
                    if (Constante.tipBD == 2)
                        sql = "select f100.f10003 as \"Marca\", case when f100991 is null or f100991 = TO_DATE('01/01/2100', 'dd/mm/yyyy') then  "
                            + "  TO_DATE('01/' ||  f01012 || '/' || F01011, 'dd/mm/yyyy') "
                            + " else f100991 end as \"Data\", COALESCE(" + salariu + ", 0) as \"Valoare\" from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                            + " union "
                            + " select f70403 as \"Marca\", f70406 as \"Data\", COALESCE(F70407, 0) as \"Valoare\" from f704 where f70404 = 1 and f70420 = 0 "
                            + " union "
                            + " select f10003 as \"Marca\", \"DataModif\" as \"Data\", COALESCE(\"SalariulBrut\", 0) as \"Valoare\" from \"Avs_Cereri\" where \"IdAtribut\" = 1 and \"IdStare\" in (1, 2, 3)";
                    DataTable dtSal = General.IncarcaDT(sql, null);
                    string dtModif = General.ToDataUniv(Convert.ToDateTime(txtDataMod.Value).Year, Convert.ToDateTime(txtDataMod.Value).Month, Convert.ToDateTime(txtDataMod.Value).Day);
                    DataRow[] drSal = dtSal.Select("Marca = " + F10003 + " AND Data <= " + dtModif, "Data DESC");

                    if (!VerificareSalariu(Convert.ToInt32(Convert.ToDouble(drSal[0]["Valoare"].ToString())), Convert.ToInt32(cmb2Nou.Value)))
                    {                       
                        Session["Avs_MesajNorma"] = Dami.TraduCuvant("Salariul angajatului este mai mic decat cel minim (raportat la timp partial)!\nVa rugam sa efectuati o cerere de modificare salariu in concordanta cu noul Timp partial!\n\n");
                        //pnlCtl.JSProperties["cpAlertMessage"]
                        //return false;
                    }
                }
                if (idAtr == (int)Constante.Atribute.Salariul)
                {
                    //DataTable dtNorma = General.IncarcaDT("SELECT COALESCE(F10043, 0) FROM F100 WHERE F10003 = " + F10003, null);
                    sql = "select f100.f10003 as Marca, case when f100955 is null or f100955 = convert(datetime, '01/01/2100', 103) then  "
                    + "  convert(datetime, '01/' + convert(varchar, f01012) + '/' + convert(varchar, f01011), 103) "
                    + " else f100955 end as Data, COALESCE(f10043, 0) as Valoare from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                    + " union "
                    + " select f70403 as Marca, f70406 as Data, COALESCE(f70422, 0) as Valoare from f704 where f70404 = 6 and f70420 = 0 "
                    + " union "
                    + " select f10003 as Marca, datamodif as Data, COALESCE(TimpPartial, 0) as Valoare from Avs_Cereri where IdAtribut = 6 and IdStare in (1, 2, 3)";
                    if (Constante.tipBD == 2)
                        sql = "select f100.f10003 as \"Marca\", case when f100955 is null or f100955 = TO_DATE('01/01/2100', 'dd/mm/yyyy') then  "
                            + "  TO_DATE('01/' ||  f01012 || '/' || F01011, 'dd/mm/yyyy') "
                            + " else f100955 end as \"Data\", COALESCE(f10043, 0) as \"Valoare\" from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                            + " union "
                            + " select f70403 as \"Marca\", f70406 as \"Data\", COALESCE(f70422, 0) as \"Valoare\" from f704 where f70404 = 6 and f70420 = 0 "
                            + " union "
                            + " select f10003 as \"Marca\", \"DataModif\" as \"Data\", COALESCE(\"TimpPartial\", 0) as \"Valoare\" from \"Avs_Cereri\" where \"IdAtribut\" = 6 and \"IdStare\" in (1, 2, 3)";
                    DataTable dtNorma = General.IncarcaDT(sql, null);
                    string dtModif = General.ToDataUniv(Convert.ToDateTime(txtDataMod.Value).Year, Convert.ToDateTime(txtDataMod.Value).Month, Convert.ToDateTime(txtDataMod.Value).Day);
                    DataRow[] drNorma = dtNorma.Select("Marca = " + F10003 + " AND Data <= " + dtModif, "Data DESC");
                    if (!VerificareSalariu(Convert.ToInt32(txt1Nou.Text.Length > 0 ? txt1Nou.Text : "0"), Convert.ToInt32(drNorma[0]["Valoare"].ToString())))
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Salariul introdus este mai mic decat cel minim (raportat la timp partial)!");
                        return false;
                    }
                }

                //Radu 05.05.2020
                int valSusp = Convert.ToInt32(Dami.ValoareParam("BlocareIncetariSuspendariDeschise", "0"));
                if (idAtr == (int)Constante.Atribute.MotivPlecare && valSusp == 1)
                {
                    DataTable dtSusp = General.IncarcaDT("SELECT COUNT(*) FROM F111 WHERE F11103 = " + F10003 + " AND (F11107 IS NULL OR F11107 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103))" : "TO_DATE('01/01/2100', 'dd/mm/yyyy'))"), null);
                    if (dtSusp != null && dtSusp.Rows.Count > 0 && Convert.ToInt32(dtSusp.Rows[0][0].ToString()) > 0)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti inceta acest contract deoarece angajatul are cel putin o suspendare activa!");
                        return false;
                    }
                }

                if (cmbAng.Value == null) strErr += ", angajat";
                if (idAtr == -99) strErr += ", atribut";
                if (txtDataMod.Value == null) strErr += ", data modificare";

                DateTime dtLucru = General.DamiDataLucru();
                if (txtDataMod.Value != null && Convert.ToDateTime(txtDataMod.Value) < dtLucru)
                    strErr += ", data modificarii este anterioara lunii de lucru";                               


                switch (idAtr)
                {
                    case (int)Constante.Atribute.Salariul:
                        if (txt1Nou.Value == null || txt2Nou.Value == null) strErr += ", salariul";
                        break;
                    case (int)Constante.Atribute.Functie:
                        if (cmb1Nou.Value == null) strErr += ", functia";
                        break;
                    case (int)Constante.Atribute.CodCOR:
                        if (cmb1Nou.Value == null) strErr += ", cod COR";
                        break;
                    case (int)Constante.Atribute.MotivPlecare:
                        if (cmb1Nou.Value == null) strErr += ", motiv plecare";
                        break;
                    case (int)Constante.Atribute.Organigrama:
                        if (cmbStructOrgNou.Value == null) strErr += ", organigrama";
                        break;
                    case (int)Constante.Atribute.Norma:
                        if (cmb2Nou.Value == null) strErr += ", timp partial";
                        break;
                    case (int)Constante.Atribute.ContrIn:
                        if (txt1Nou.Value == null || de1Nou.Value == null) strErr += ", contract intern";
                        break;
                    case (int)Constante.Atribute.ContrITM:
                        if (txt1Nou.Value == null || de1Nou.Value == null) strErr += ", contract ITM";
                        break;
                    case (int)Constante.Atribute.DataAngajarii:
                        if (de1Nou.Value == null) strErr += ", data angajarii";
                        break;
                    case (int)Constante.Atribute.Sporuri:
                        break;
                    case (int)Constante.Atribute.TitluAcademic:
                        if (cmb1Nou.Value == null) strErr += ", titlul academic";
                        break;
                    case (int)Constante.Atribute.MesajPersonal:
                        if (cmb1Nou.Value == null) strErr += ", mesajul personal";
                        break;
                    case (int)Constante.Atribute.CentrulCost:
                        if (cmb1Nou.Value == null) strErr += ", centrul de cost";
                        break;
                    case (int)Constante.Atribute.SporTranzactii:
                        break;
                    case (int)Constante.Atribute.PunctLucru:
                        if (cmb1Nou.Value == null) strErr += ", punct de lucru";
                        break;
                    case (int)Constante.Atribute.Meserie:
                        if (cmb1Nou.Value == null) strErr += ", meserie";
                        break;
                    case (int)Constante.Atribute.PrelungireCIM:
                    case (int)Constante.Atribute.PrelungireCIM_Vanz:
                        if ((cmb1Nou.Value == null || Convert.ToInt32(cmb1Nou.Value) != 1) && (de1Nou.Value == null || de2Nou.Value == null)) strErr += ", date prelungire CIM";
                        DataTable dtTemp = General.IncarcaDT("SELECT F1009741 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString() == "1")
                            strErr += ", prelungirea se face doar pt. contractele pe perioada determinata";
                        break;
                    case (int)Constante.Atribute.NumePrenume:
                        if (txt1Nou.Value == null || txt2Nou.Value == null) strErr += ", nume si prenume";
                        break;
                    case (int)Constante.Atribute.CASS:
                        if (cmb1Nou.Value == null) strErr += ", CASS";
                        break;
                    case (int)Constante.Atribute.Studii:
                        if (cmb1Nou.Value == null) strErr += ", studii";
                        break;
                    case (int)Constante.Atribute.BancaSalariu:
                        if (cmb1Nou.Value == null || cmb2Nou.Value == null || (txt1Nou.Value == null && txt2Nou.Value == null)) strErr += ", banca - cont salariu";
                        break;
                    case (int)Constante.Atribute.BancaGarantii:
                        if (cmb1Nou.Value == null || cmb2Nou.Value == null || txt1Nou.Value == null) strErr += ", banca - cont garantii";
                        break;
                    case (int)Constante.Atribute.LimbiStraine:
                        if (cmb1Nou.Value == null || txt1Nou.Value == null || txt2Nou.Value == null) strErr += ", limbi straine";
                        break;
                    case (int)Constante.Atribute.DocId:
                        if (cmb1Nou.Value == null || txt1Nou.Value == null || txt2Nou.Value == null || de1Nou.Value == null || de2Nou.Value == null) strErr += ", document identitate";
                        break;
                    case (int)Constante.Atribute.PermisAuto:
                        if (cmb1Nou.Value == null || txt1Nou.Value == null || txt2Nou.Value == null || de1Nou.Value == null || de2Nou.Value == null) strErr += ", permis auto";
                        break;
                    case (int)Constante.Atribute.BonusTeamLeader:
                        if (txt1Nou.Value == null) strErr += ", bonus Team Leader";
                        break;
                    case (int)Constante.Atribute.Suspendare:
                        if (de1Nou.Value == null) strErr += ", suspendare";
                        break;
                    case (int)Constante.Atribute.RevenireSuspendare:
                        if (de3Nou.Value == null) strErr += ", revenire suspendare";
                        break;
                    case (int)Constante.Atribute.Detasare:
                        if (cmb1Nou.Value == null || txt1Nou.Value == null || txt2Nou.Value == null || de1Nou.Value == null) strErr += ", detasare";
                        break;
                    case (int)Constante.Atribute.RevenireDetasare:
                        if (de3Nou.Value == null) strErr += ", revenire detasare";
                        break;
                    case (int)Constante.Atribute.ProgramLucru:
                        if (cmb1Nou.Value == null) strErr += ", program de lucru";
                        break;
                    case (int)Constante.Atribute.TipContract:
                    case (int)Constante.Atribute.DurataContract:
                        if (cmb1Nou.Value == null) strErr += ", tip contract";
                        //if ((cmb2Nou.Value == null || Convert.ToInt32(cmb2Nou.Value) != 1) && (de1Nou.Value == null || de2Nou.Value == null)) strErr += ", date durata contract";
                        break;
                }

                if (strErr != "")
                {
                    //ArataMesaj("Lipsesc date: " + strErr.Substring(1));
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date: ") + strErr.Substring(1);
                    SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out dataRev);
                    //MessageBox.Show(Dami.TraduCuvant("Lipsesc date: " + strErr.Substring(1)), MessageBox.icoError);
                    return false;
                }


                return AdaugaCerere();
                

            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return false;
            }
        }


        #region OLD

        //private void CalcSalariu(int tipVenit, ASPxTextBox txt1, ASPxTextBox txt2)
        //{
        //    try
        //    {
        //        //tipVenit = 1     VB -> SN
        //        //tipVenit = 2     SN -> VB

        //        if (tipVenit == 1 && !General.IsNumeric(txt1.Value))
        //        {
        //            //ArataMesaj("Lipseste venitul brut !");
        //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipseste venitul brut !");
        //            //MessageBox.Show(Dami.TraduCuvant("Lipseste venitul brut !"), MessageBox.icoError);
        //            return;
        //        }

        //        if (tipVenit == 2 && !General.IsNumeric(txt2.Value))
        //        {
        //            //ArataMesaj("Lipseste salariul net !");
        //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipseste salariul net !");
        //            //MessageBox.Show(Dami.TraduCuvant("Lipseste salariul net !"), MessageBox.icoError);                  
        //            return;
        //        }

        //        int i = 0;              //daca ajunge la 100 oprim iteratia ca sa nu devina bucla infinita

        //        decimal varCas = 10.5m;
        //        decimal varCass = 5.5m;
        //        decimal varSom = 0.5m;
        //        decimal varNr = 0;
        //        decimal scutit = 0;
        //        decimal tipAng = 1;
        //        decimal varDed = 250;
        //        decimal varImp = 16;


        //        List<decimal> lst = GetVariabileVB();
        //        varCass = lst[0];
        //        varSom = lst[1];
        //        varCas = lst[2];
        //        varNr = lst[3];
        //        scutit = lst[4];
        //        varImp = lst[7];

        //        //DataTable dtTemp = General.IncarcaDT("SELECT F10026 FROM F100 WHERE F10003 = " + F10003.ToString(), null);
        //        //scutit = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());

        //        //scutit = Convert.ToDecimal(txtScutitAct.EditValue ?? 0);
        //        tipAng = lst[5];
        //        decimal salMediu = lst[6];

        //        if (scutit == 1) varImp = 0;
        //        if (tipAng == 2) varSom = 0;         //daca este pensionar nu plateste somaj


        //        if (tipVenit == 1)           //VB -> SN
        //        {
        //            varDed = DamiValDeducere(varNr, Convert.ToDecimal(txt1.Value ?? 0));
        //            txt2.Text = CalcSN(Convert.ToDecimal(txt1.Value ?? 0), varCas, varCass, varSom, varImp, varDed, salMediu).ToString();
        //        }
        //        else                    //SN -> VB
        //        {
        //            decimal SN = Convert.ToDecimal(txt2.Value ?? 1);
        //            varDed = DamiValDeducere(varNr, SN);
        //            decimal tmpVB = Math.Round((SN - (1.5m * varImp / 100 * varDed)) / (1 - varImp / 100 - (varImp / 100 * varDed / 2000) - ((1 - varImp / 100) * (varCas + varCass + varSom) / 100)));
        //            decimal tmpSN = 0;

        //            while (tmpSN != SN)
        //            {
        //                if (i > 100)
        //                {
        //                    return;
        //                }
        //                else
        //                {
        //                    i += 1;

        //                    varDed = DamiValDeducere(varNr, tmpVB);
        //                    tmpSN = CalcSN(tmpVB, varCas, varCass, varSom, varImp, varDed, salMediu);
        //                    if (tmpSN != SN)
        //                    {
        //                        if (tmpSN > SN)
        //                            tmpVB -= 1;
        //                        else
        //                            tmpVB += 1;
        //                    }
        //                }
        //            }

        //            txt1.Text = tmpVB.ToString();
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        //public List<decimal> GetVariabileVB()
        //{
        //    List<decimal> lst = new List<decimal>();

        //    try
        //    {
        //        int idCass = 0, idSomaj = 0, idCas = 0;
        //        decimal cass = 0, som = 0, cas = 0, nrDed = 0, scutit = 0, tipAng = 0, salMediu = 0, procImp = 0;
        //        DataTable dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CASS'", null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            idCass = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'SOMAJ'", null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            idSomaj = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CAS'", null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            idCas = Convert.ToInt32(dtTemp.Rows[0][0].ToString());

        //        dtTemp = General.IncarcaDT("SELECT F01324 FROM F013 WHERE F01304 = " + idCass.ToString(), null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            cass = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT F01324 FROM F013 WHERE F01304 = " + idSomaj.ToString(), null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            som = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT F01324 FROM F013 WHERE F01304 = " + idCas.ToString(), null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            cas = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT COUNT(*) FROM F110 WHERE F11003 = " + F10003.ToString() + " AND F11016 = 1 ", null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            nrDed = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT F10026 FROM F100 WHERE F10003 = " + F10003.ToString(), null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            scutit = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
        //        dtTemp = General.IncarcaDT("SELECT F10010 FROM F100 WHERE F10003 = " + F10003.ToString(), null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            tipAng = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());

        //        dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE UPPER(\"Nume\") = 'SALARIULMEDIU'", null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            salMediu = Convert.ToInt32(dtTemp.Rows[0][0].ToString());

        //        dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE UPPER(\"Nume\") = 'PROCENT_IMPOZIT'", null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
        //            procImp = Convert.ToInt32(dtTemp.Rows[0][0].ToString());

        //        lst.Add(cass);
        //        lst.Add(som);
        //        lst.Add(cas);
        //        lst.Add(nrDed);
        //        lst.Add(scutit);
        //        lst.Add(tipAng);
        //        lst.Add(salMediu);
        //        lst.Add(procImp);
        //    }
        //    catch (Exception ex)
        //    {
        //        //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return lst;
        //}


        //private decimal DamiValDeducere(decimal nrPersIntretinere, decimal VB)
        //{
        //    decimal? varDed = 0;

        //    try
        //    {
        //        DataTable dtTemp = General.IncarcaDT("SELECT * FROM F730 WHERE F73004 <= " + Convert.ToInt32(VB).ToString() + " AND  F73006 >= " + Convert.ToInt32(VB).ToString(), null);
        //        if (dtTemp != null && dtTemp.Rows.Count > 0)
        //        {
        //            switch (Convert.ToInt32(nrPersIntretinere))
        //            {
        //                case 0:
        //                    varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73008"].ToString());
        //                    break;
        //                case 1:
        //                    varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73009"].ToString());
        //                    break;
        //                case 2:
        //                    varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73010"].ToString());
        //                    break;
        //                case 3:
        //                    varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73011"].ToString());
        //                    break;
        //                default:
        //                    varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73012"].ToString());
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return Convert.ToDecimal(varDed ?? 250);
        //}


        //private decimal CalcSN(decimal VB, decimal varCas, decimal varCass, decimal varSom, decimal varImp, decimal varDed, decimal salMediu)
        //{
        //    try
        //    {
        //        //teorie:
        //        //SN = VB - IMP - TAXE
        //        //TAXE = CAS + CASS + SOM
        //        //DED = sumafixa * (1-(VB-1000)/2000)
        //        //IMP = varImp/100 * (VB - TAXE - DED)

        //        //unde:
        //        //CAS = round(VB * 10,5/100)
        //        //CASS = round(VB * 5,5/100)
        //        //SOM = round(VB * 0,5/100)

        //        //sumafixa: (este tabel)
        //        //250 fara pers. in intretinere
        //        //350 1 pers
        //        //450 2 pers

        //        decimal SN = 0;
        //        decimal cas = 0;
        //        decimal cass = 0;
        //        decimal som = 0;

        //        //in calculul CAS-ului, daca VB este mai mare decat salariul mediu * 5 ori atunci se plafoneaza la salariul mediu * 5 ori
        //        //if (VB > (salMediu * 5))
        //        //    cas = MathExt.Round((salMediu * 5) * varCas / 100, WizOne.Module.MidpointRounding.AwayFromZero);
        //        //else
        //        cas = MathExt.Round(VB * varCas / 100, MidpointRounding.AwayFromZero);

        //        if (0 < cas && cas <= 1) cas = 1;   //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0

        //        cass = VB * varCass / 100;
        //        som = VB * varSom / 100;

        //        if (0 < cass && cass <= 1)          //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0
        //            cass = 1;
        //        else
        //            cass = MathExt.Round(VB * varCass / 100, MidpointRounding.AwayFromZero);

        //        if (0 < som && som <= 1)            //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0
        //            som = 1;
        //        else
        //            som = MathExt.Round(VB * varSom / 100, MidpointRounding.AwayFromZero);

        //        decimal taxe = cas + cass + som;

        //        decimal ded = varDed;

        //        if (1001 <= VB && VB <= 3000)
        //        {
        //            //ded = Math.Round((varDed * (1 - (VB - 1000) / 2000)), 0);
        //            ded = (varDed * (1 - (VB - 1000) / 2000));
        //            ded = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(ded / 10)) * 10);
        //        }

        //        decimal imp = MathExt.Round(varImp / 100 * (VB - taxe - ded), MidpointRounding.AwayFromZero);
        //        if (imp < 0) imp = 0;

        //        SN = MathExt.Round((VB - imp - taxe), MidpointRounding.AwayFromZero);

        //        return SN;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //        return 0;
        //    }
        //}

        #endregion


        private bool AdaugaCerere()
        {

            int idUrm = -99;
            idUrm = Convert.ToInt32(Dami.NextId("Avs_Cereri"));

            DataSet ds = Session["AvsCereri"] as DataSet;
            DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

            if (ds == null)
            {
                ds = new DataSet();

                DataTable dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);
            }

            string sql = "SELECT COUNT(*) FROM \"Avs_Circuit\" WHERE \"IdAtribut\" = " + cmbAtribute.Value.ToString();
            string strSql = "";
            DataTable dtTemp = General.IncarcaDT(sql, null);
            int nr = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
            if (nr == 0)
            {
                //ArataMesaj("Atributul nu are circuit alocat!");
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Atributul nu are circuit alocat!");
                //MessageBox.Show(Dami.TraduCuvant("Atributul nu are circuit alocat!"), MessageBox.icoError);
                return false;
            }

            //prima data determinam circuitul in functie de userul logat;
            //in cazul in care nu exista, luam primul circuit pe care il gasim
            int idCircuit = -99;

            string cond1 = "", cond2 = "";
            if (Constante.tipBD == 1)
            {
                cond1 = " TOP 1 ";
                cond2 = "";
            }
            else
            {
                cond1 = "";
                cond2 = " AND ROWNUM = 1 ";
            }

            sql = "SELECT " + cond1 + " c.*, b.\"IdUser\" FROM \"F100Supervizori\" b, \"Avs_Circuit\" c WHERE b.F10003 = " + F10003.ToString() + " AND c.\"IdAtribut\" = " + cmbAtribute.Value.ToString()
                + " AND (c.\"Super1\" = 0 OR c.\"Super1\" = -1 * b.\"IdSuper\" OR c.\"Super2\" = 0 OR c.\"Super2\" = -1 * b.\"IdSuper\" OR c.\"Super3\" = 0 OR c.\"Super3\" = -1 * b.\"IdSuper\" OR c.\"Super4\" = 0 OR c.\"Super4\" = -1 * b.\"IdSuper\" OR c.\"Super5\" = 0 OR c.\"Super5\" = -1 * b.\"IdSuper\" OR c.\"Super6\" = 0 OR c.\"Super6\" = -1 * b.\"IdSuper\" OR c.\"Super7\" = 0 OR c.\"Super7\" = -1 * b.\"IdSuper\" OR c.\"Super8\" = 0 OR c.\"Super8\" = -1 * b.\"IdSuper\" OR c.\"Super9\" = 0 OR c.\"Super9\" = -1 * b.\"IdSuper\" OR c.\"Super10\" = 0 OR c.\"Super10\" = -1 * b.\"IdSuper\" OR c.\"Super11\" = 0 OR c.\"Super11\" = -1 * b.\"IdSuper\" OR c.\"Super12\" = 0 OR c.\"Super12\" = -1 * b.\"IdSuper\" OR c.\"Super13\" = 0 OR c.\"Super13\" = -1 * b.\"IdSuper\" OR c.\"Super14\" = 0 OR c.\"Super14\" = -1 * b.\"IdSuper\" OR c.\"Super15\" = 0 OR c.\"Super15\" = -1 * b.\"IdSuper\" OR c.\"Super16\" = 0 OR c.\"Super16\" = -1 * b.\"IdSuper\" OR c.\"Super17\" = 0 OR c.\"Super17\" = -1 * b.\"IdSuper\" OR c.\"Super18\" = 0 OR c.\"Super18\" = -1 * b.\"IdSuper\" OR c.\"Super19\" = 0 OR c.\"Super19\" = -1 * b.\"IdSuper\" OR c.\"Super20\" = 0 OR c.\"Super20\" = -1 * b.\"IdSuper\") "
                + " AND b.\"IdUser\" = " + Session["UserId"] + cond2;
            DataTable dtCir = General.IncarcaDT(sql, null);


            if (dtCir != null && dtCir.Rows.Count > 0)
                idCircuit = Convert.ToInt32(dtCir.Rows[0]["Id"].ToString());
            else
            {
                sql = "SELECT " + cond1 + " c.*, b.\"IdUser\" FROM \"F100Supervizori\" b, \"Avs_Circuit\" c WHERE b.F10003 = " + F10003.ToString() + " AND c.\"IdAtribut\" = " + cmbAtribute.Value.ToString()
                    + " AND (c.\"Super1\" = 0 OR c.\"Super1\" = -1 * b.\"IdSuper\" OR c.\"Super2\" = 0 OR c.\"Super2\" = -1 * b.\"IdSuper\" OR c.\"Super3\" = 0 OR c.\"Super3\" = -1 * b.\"IdSuper\" OR c.\"Super4\" = 0 OR c.\"Super4\" = -1 * b.\"IdSuper\" OR c.\"Super5\" = 0 OR c.\"Super5\" = -1 * b.\"IdSuper\" OR c.\"Super6\" = 0 OR c.\"Super6\" = -1 * b.\"IdSuper\" OR c.\"Super7\" = 0 OR c.\"Super7\" = -1 * b.\"IdSuper\" OR c.\"Super8\" = 0 OR c.\"Super8\" = -1 * b.\"IdSuper\" OR c.\"Super9\" = 0 OR c.\"Super9\" = -1 * b.\"IdSuper\" OR c.\"Super10\" = 0 OR c.\"Super10\" = -1 * b.\"IdSuper\" OR c.\"Super11\" = 0 OR c.\"Super11\" = -1 * b.\"IdSuper\" OR c.\"Super12\" = 0 OR c.\"Super12\" = -1 * b.\"IdSuper\" OR c.\"Super13\" = 0 OR c.\"Super13\" = -1 * b.\"IdSuper\" OR c.\"Super14\" = 0 OR c.\"Super14\" = -1 * b.\"IdSuper\" OR c.\"Super15\" = 0 OR c.\"Super15\" = -1 * b.\"IdSuper\" OR c.\"Super16\" = 0 OR c.\"Super16\" = -1 * b.\"IdSuper\" OR c.\"Super17\" = 0 OR c.\"Super17\" = -1 * b.\"IdSuper\" OR c.\"Super18\" = 0 OR c.\"Super18\" = -1 * b.\"IdSuper\" OR c.\"Super19\" = 0 OR c.\"Super19\" = -1 * b.\"IdSuper\" OR c.\"Super20\" = 0 OR c.\"Super20\" = -1 * b.\"IdSuper\") "
                    + cond2;
                dtCir = General.IncarcaDT(sql, null);

                if (dtCir != null && dtCir.Rows.Count > 0)
                    idCircuit = Convert.ToInt32(dtCir.Rows[0]["Id"].ToString());
            }

            if (idCircuit == -99 || idCircuit == 0)
            {
                //ArataMesaj("Angajatul nu are supervizor pe circuit!");
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul nu are supervizor pe circuit!");
                //MessageBox.Show(Dami.TraduCuvant("Angajatul nu are supervizor pe circuit!"), MessageBox.icoError);
                return false;
            }

            DateTime dtModif = Convert.ToDateTime(txtDataMod.Value);
            string data = "";
            if (Constante.tipBD == 1)
                data = " CONVERT(DATETIME, '" + dtModif.Day.ToString().PadLeft(2, '0') + "/" + dtModif.Month.ToString().PadLeft(2, '0') + "/" + dtModif.Year.ToString() + "', 103) ";
            else
                data = " TO_DATE('" + dtModif.Day.ToString().PadLeft(2, '0') + "/" + dtModif.Month.ToString().PadLeft(2, '0') + "/" + dtModif.Year.ToString() + "', 'dd/mm/yyyy') ";

            sql = "SELECT * FROM \"Avs_Cereri\" WHERE F10003 = " + F10003 + " AND \"IdAtribut\" = " + cmbAtribute.Value.ToString() + " AND \"DataModif\" = " + data + " AND \"IdStare\" > 0";
            dtTemp = General.IncarcaDT(sql, null);
            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                //ArataMesaj("Exista cerere pt acest angajat, atribut si aceasta data!");
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Exista cerere pt acest angajat, atribut si aceasta data!");
                //MessageBox.Show(Dami.TraduCuvant("Exista cerere pt acest angajat, atribut si aceasta data!"), MessageBox.icoError);
                return false;
            }

            sql = "SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = 1";
            dtTemp = General.IncarcaDT(sql, null);
            string culoare = (dtTemp.Rows[0][0] as string ?? "#FFFFFFFF").ToString();


            //Adaugare in Avs_Cereri
            int idAtr = -99;
            string dataModif = "";
            DateTime dm = Convert.ToDateTime(txtDataMod.Value);
            if (Constante.tipBD == 1)
                dataModif = " CONVERT(DATETIME, '" + dm.Day.ToString().PadLeft(2, '0') + "/" + dm.Month.ToString().PadLeft(2, '0') + "/" + dm.Year.ToString() + "', 103) ";
            else
                dataModif = " TO_DATE('" + dm.Day.ToString().PadLeft(2, '0') + "/" + dm.Month.ToString().PadLeft(2, '0') + "/" + dm.Year.ToString() + "', 'dd/mm/yyyy') ";

            idAtr = Convert.ToInt32((cmbAtribute.Value ?? -99));
            string camp1 = "", camp2 = "", sqlFunc = "";
            switch (idAtr)
            {
                case (int)Constante.Atribute.Salariul:
                    camp1 = "\"SalariulBrut\", \"SalariulNet\"";
                    camp2 = txt1Nou.Text + ", " + txt2Nou.Text;
                    break;
                case (int)Constante.Atribute.Functie:
                    camp1 = "\"FunctieId\", \"FunctieNume\", \"PerProbaZL\", \"PerProbaZC\", \"PreavizDemisie\", \"PreavizConcediere\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "', " + (txt1Nou.Text.Length <= 0 ? "NULL" : txt1Nou.Text) + ", " + (txt2Nou.Text.Length <= 0 ? "NULL" : txt2Nou.Text) + ", " + (txt3Nou.Text.Length <= 0 ? "NULL" : txt3Nou.Text) + ", " + (txt4Nou.Text.Length <= 0 ? "NULL" : txt4Nou.Text);
                    if (cmb2Nou.Value != null)
                        sqlFunc = "UPDATE F718 SET F71813 = " + cmb2Nou.Value.ToString() + " WHERE F71802 = " + cmb1Nou.Value.ToString();
                    break;
                case (int)Constante.Atribute.CodCOR:
                    camp1 = "\"CORCod\", \"CORNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    int afisare = Convert.ToInt32(Dami.ValoareParam("Avs_CampuriZileProbaPreavizCOR", "0"));
                    if (afisare == 1)
                    {
                        camp1 = "\"CORCod\", \"CORNume\", \"PerProbaZL\", \"PerProbaZC\", \"PreavizDemisie\", \"PreavizConcediere\"";
                        camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "', " + (txt1Nou.Text.Length <= 0 ? "NULL" : txt1Nou.Text) + ", " + (txt2Nou.Text.Length <= 0 ? "NULL" : txt2Nou.Text) + ", " + (txt3Nou.Text.Length <= 0 ? "NULL" : txt3Nou.Text) + ", " + (txt4Nou.Text.Length <= 0 ? "NULL" : txt4Nou.Text);
                    }
                    break;
                case (int)Constante.Atribute.MotivPlecare:
                    camp1 = "\"MotivId\", \"MotivNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.Organigrama:
                    //Florin 2018.11.23
                    camp1 = "\"SubcompanieId\", \"SubcompanieNume\", \"FilialaId\", \"FilialaNume\", \"SectieId\", \"SectieNume\", \"DeptId\", \"DeptNume\", \"SubdeptId\", \"SubdeptNume\", \"BirouId\", \"BirouNume\"";
                    ListEditItem itm = cmbStructOrgNou.SelectedItem;
                    camp2 = itm.GetFieldValue("F00304") + ",'" + itm.GetFieldValue("F00305") + "'," +
                            itm.GetFieldValue("F00405") + ",'" + itm.GetFieldValue("F00406") + "'," +
                            itm.GetFieldValue("F00506") + ",'" + itm.GetFieldValue("F00507") + "'," +
                            itm.GetFieldValue("F00607") + ",'" + itm.GetFieldValue("F00608") + "'," +
                            (itm.GetFieldValue("F00708") == null || itm.GetFieldValue("F00708").ToString().Length <= 0 ? "NULL" : itm.GetFieldValue("F00708")) + ",'" + itm.GetFieldValue("F00709") + "'," +
                            (itm.GetFieldValue("F00809") == null || itm.GetFieldValue("F00809").ToString().Length <= 0 ? "NULL" : itm.GetFieldValue("F00809")) + ",'" + itm.GetFieldValue("F00810") + "'"; 
                    //ListEditItem linie = cmbStructOrgNou.SelectedItem;
                    //int subc = linie.GetValue("F00304").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00304").ToString()) : -1;
                    //int fil = linie.GetValue("F00405").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00405").ToString()) : -1;
                    //int sec = linie.GetValue("F00506").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00506").ToString()) : -1;
                    //int dept = linie.GetValue("F00607").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00607").ToString()) : -1;

                    //string subcNume = linie.GetValue("F00305").ToString().Length > 0 ? linie.GetValue("F00305").ToString() : "";
                    //string filNume = linie.GetValue("F00406").ToString().Length > 0 ? linie.GetValue("F00406").ToString() : "";
                    //string secNume = linie.GetValue("F00507").ToString().Length > 0 ? linie.GetValue("F00507").ToString() : "";
                    //string deptNume = linie.GetValue("F00608").ToString().Length > 0 ? linie.GetValue("F00608").ToString() : "";
                    //camp2 = subc.ToString() + ", '" + subcNume + "'" + fil.ToString() + ", '" + filNume + "'" + sec.ToString() + ", '" + secNume + "'" + dept.ToString() + ", '" + deptNume + "'";
                    break;
                case (int)Constante.Atribute.Norma:
                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    camp1 = "\"TipAngajat\", \"TimpPartial\", \"Norma\", \"TipNorma\", \"DurataTimpMunca\", \"RepartizareTimpMunca\", \"IntervalRepartizare\", \"NrOreLuna\", \"Tarife\", \"ProgramLucru\"";
                    camp2 = cmb1Nou.Value.ToString() + "," + cmb2Nou.Value.ToString() + "," + cmb3Nou.Value.ToString() + "," + cmb4Nou.Value.ToString() + "," + cmb5Nou.Value.ToString() + "," + cmb6Nou.Value.ToString() + "," + (cmb7Nou.Value == null ? "NULL" : cmb7Nou.Value.ToString()) + "," + (txt1Nou.Text.Length <= 0 ? "NULL" : txt1Nou.Text) + "," + "'" + sir + "', " + (cmb8Nou.Value == null ? "NULL" : cmb8Nou.Value.ToString());
                    break;
                case (int)Constante.Atribute.ContrIn:
                    camp1 = "\"NrIntern\", \"DataIntern\"";
                    camp2 = txt1Nou.Text + ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy')");
                    break;
                case (int)Constante.Atribute.ContrITM:
                    camp1 = "\"NrITM\", \"DataITM\"";
                    camp2 = txt1Nou.Text + ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy')");
                    break;
                case (int)Constante.Atribute.DataAngajarii:
                    camp1 = "\"DataAngajarii\"";
                    camp2 = (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy') ");
                    break;
                case (int)Constante.Atribute.Sporuri:
                    for (int i = 0; i < dsCalcul.Tables["Sporuri1"].Rows.Count; i++)
                    {
                        camp1 += "\"Spor" + i.ToString() + "\"" + ", ";
                        camp2 += dsCalcul.Tables["Sporuri1"].Rows[i]["F02504"].ToString() + ", ";                 
                    }
                    for (int i = 0; i < dsCalcul.Tables["Sporuri2"].Rows.Count; i++)
                    {
                        camp1 += "\"Spor" + (i + 10).ToString() + "\"" + ", ";
                        camp2 += dsCalcul.Tables["Sporuri2"].Rows[i]["F02504"].ToString() + ", ";              
                    }
                    sir = ds.Tables[0].Rows[0]["F10067"].ToString();          
                    camp1 += "\"Tarife\"";
                    camp2 += "'" + sir + "'";
                    break;
                case (int)Constante.Atribute.TitluAcademic:
                    camp1 = "\"TitlulAcademicId\", \"TitlulAcademicNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.MesajPersonal:
                    camp1 = "\"MesajPersonalId\", \"MesajPersonalNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.CentrulCost:
                    camp1 = "\"CentruCostId\", \"CentruCostNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.SporTranzactii:
                    for (int i = 0; i < dsCalcul.Tables["SporTran"].Rows.Count; i++)
                    {
                        camp1 += "\"SporTran" + i.ToString() + "\"";
                        camp2 += dsCalcul.Tables["SporTran"].Rows[i]["Spor"].ToString();
                        if (i < dsCalcul.Tables["SporTran"].Rows.Count - 1)
                        {
                            camp1 += ", ";
                            camp2 += ", ";
                        }
                    }
                    break;
                case (int)Constante.Atribute.PunctLucru:
                    camp1 = "\"PunctLucruId\", \"PunctLucruNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.Meserie:
                    camp1 = "\"MeserieId\", \"MeserieNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.PrelungireCIM:
                case (int)Constante.Atribute.PrelungireCIM_Vanz:
                    camp1 = "\"DataInceputCIM\", \"DataSfarsitCIM\", \"DurataContract\"";
                    camp2 = (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy') ") + ", "
                        + (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de2Nou.Text + "', 103)" : "TO_DATE('" + de2Nou.Text + "', 'dd/mm/yyyy') ") + ", " + cmb1Nou.Value.ToString();
                    break;
                case (int)Constante.Atribute.Componente:
                    for (int i = 0; i < dsCalcul.Tables["Componente"].Rows.Count; i++)
                    {
                        camp1 += "\"Comp" + (Convert.ToInt32(dsCalcul.Tables["Componente"].Rows[i]["F02104"].ToString().Substring(2)) - 1).ToString() + "\"";
                        camp2 += dsCalcul.Tables["Componente"].Rows[i]["Suma"].ToString().Replace(',', '.');
                        if (i < dsCalcul.Tables["Componente"].Rows.Count - 1)
                        {
                            camp1 += ", ";
                            camp2 += ", ";
                        }
                    }     
                    break;
                case (int)Constante.Atribute.Tarife:
                    sir = ds.Tables[0].Rows[0]["F10067"].ToString();                  
        
                    camp1 = "\"Tarife\"";
                    camp2 = "'" + sir + "'";
                    break;
                case (int)Constante.Atribute.NumePrenume:
                    camp1 = "\"Nume\", \"Prenume\"";
                    camp2 = "'" + txt1Nou.Text + "', '" + txt2Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.CASS:
                    camp1 = "\"CASS\"";
                    camp2 = cmb1Nou.Value.ToString();
                    break;
                case (int)Constante.Atribute.Studii:
                    camp1 = "\"Studii\"";
                    camp2 = cmb1Nou.Value.ToString();
                    break;
                case (int)Constante.Atribute.BancaSalariu:
                    camp1 = "\"BancaSal\", \"SucursalaSal\", \"IBANSal\", \"NrCard\"";
                    camp2 = cmb1Nou.Value.ToString() + ", " + cmb2Nou.Value.ToString() + ", '" + txt1Nou.Text + "', '" + txt2Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.BancaGarantii:
                    camp1 = "\"BancaGar\", \"SucursalaGar\", \"IBANGar\"";
                    camp2 = cmb1Nou.Value.ToString() + ", " + cmb2Nou.Value.ToString() + ", '" + txt1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.LimbiStraine:
                    camp1 = "\"IdLimba\", \"NivelVorbit\", \"NrAniVorbit\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + txt1Nou.Value.ToString() + "', " + txt2Nou.Text;
                    break;
                case (int)Constante.Atribute.DocId:
                    camp1 = "\"TipID\", \"SerieNrID\", \"IDEmisDe\", \"DataElibID\", \"DataExpID\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + txt1Nou.Text + "', '" + txt2Nou.Text + "', "
                        + (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy') ") + ", "
                        + (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de2Nou.Text + "', 103)" : "TO_DATE('" + de2Nou.Text + "', 'dd/mm/yyyy') ");
                    break;
                case (int)Constante.Atribute.PermisAuto:
                    camp1 = "\"IdCateg\", \"NrPermis\", \"PermisEmisDe\", \"DataEmiterePermis\", \"DataExpirarePermis\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + txt1Nou.Text + "', '" + txt2Nou.Text + "', "
                        + (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy') ") + ", "
                        + (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de2Nou.Text + "', 103)" : "TO_DATE('" + de2Nou.Text + "', 'dd/mm/yyyy') ");
                    break;
                case (int)Constante.Atribute.BonusTeamLeader:
                    camp1 = "\"BonusTeamLeader\"";
                    camp2 = txt1Nou.Text;
                    break;
                case (int)Constante.Atribute.Suspendare:    
                    camp1 = "\"MotivSuspId\", \"MotivSuspNume\", \"DataInceputSusp\", \"DataSfEstimSusp\", \"DataIncetareSusp\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "', " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy')") + 
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 103)" : "TO_DATE('" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 'dd/mm/yyyy')") + 
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + (de3Nou.Text.Length <= 0 ? "01/01/2100" : de3Nou.Text) + "', 103)" : "TO_DATE('" + (de3Nou.Text.Length <= 0 ? "01/01/2100" : de3Nou.Text) + "', 'dd/mm/yyyy')");
                    break;
                case (int)Constante.Atribute.RevenireSuspendare:
                    camp1 = "\"MotivSuspId\", \"MotivSuspNume\", \"DataInceputSusp\", \"DataSfEstimSusp\", \"DataIncetareSusp\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "', " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy')") +
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 103)" : "TO_DATE('" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 'dd/mm/yyyy')") +
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de3Nou.Text + "', 103)" : "TO_DATE('" + de3Nou.Text + "', 'dd/mm/yyyy')");
                    break;
                case (int)Constante.Atribute.Detasare:
                    camp1 = "\"IdNationalitAng\", \"NumeAngajator\", \"CUIAngajator\", \"DataInceputDet\", \"DataSfEstimDet\", \"DataIncetareDet\", \"DetBifa1\", \"DetBifa2\", \"DetBifa3\", \"DetBifa4\", \"DetBifa5\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + txt1Nou.Text + "', '" + txt2Nou.Text + "', " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy')") +
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 103)" : "TO_DATE('" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 'dd/mm/yyyy')") +
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + (de3Nou.Text.Length <= 0 ? "01/01/2100" : de3Nou.Text) + "', 103)" : "TO_DATE('" + (de3Nou.Text.Length <= 0 ? "01/01/2100" : de3Nou.Text) + "', 'dd/mm/yyyy')") +
                        ", " + (chk1.Checked ? "1" : "0") + ", " + (chk2.Checked ? "1" : "0") + ", " + (chk4.Checked ? "1" : "0") + ", " + (chk3.Checked ? "1" : "0") + ", " + (chk5.Checked ? "1" : "0");
                    break;
                case (int)Constante.Atribute.RevenireDetasare:
                    camp1 = "\"IdNationalitAng\", \"NumeAngajator\", \"CUIAngajator\", \"DataInceputDet\", \"DataSfEstimDet\", \"DataIncetareDet\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + txt1Nou.Text + "', '" + txt2Nou.Text + "', " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy')") +
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 103)" : "TO_DATE('" + (de2Nou.Text.Length <= 0 ? "01/01/2100" : de2Nou.Text) + "', 'dd/mm/yyyy')") +
                        ", " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + de3Nou.Text + "', 103)" : "TO_DATE('" + de3Nou.Text + "', 'dd/mm/yyyy')");
                    break;
                case (int)Constante.Atribute.ProgramLucru:
                    camp1 = "\"ProgramLucru\"";
                    camp2 = cmb1Nou.Value.ToString();
                    break;
                case (int)Constante.Atribute.TipContract:
                case (int)Constante.Atribute.DurataContract:
                    //camp1 = "\"DataInceputCIM\", \"DataSfarsitCIM\", \"DurataContract\", \"TipContract\", \"TipContractNume\", \"DurataContractNume\"";
                    //camp2 = (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de1Nou.Text + "', 103)" : "TO_DATE('" + de1Nou.Text + "', 'dd/mm/yyyy') ") + ", "
                    //    + (Constante.tipBD == 1 ? " CONVERT(DATETIME, '" + de2Nou.Text + "', 103)" : "TO_DATE('" + de2Nou.Text + "', 'dd/mm/yyyy') ") + ", " + cmb2Nou.Value.ToString() + ", " + cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "', '" + cmb2Nou.Text + "'";
                    camp1 = "\"TipContract\", \"TipContractNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";

                    break;
            }


            int total = 0;
            int idStare = 2;
            int pozUser = 1;

            //aflam totalul de utilizatori din circuit
            for (int i = 1; i <= 20; i++)
            {
                if (dtCir != null && dtCir.Rows.Count > 0 && dtCir.Rows[0]["Super" + i] != null && dtCir.Rows[0]["Super" + i].ToString().Length > 0)
                {
                    string idSuper = dtCir.Rows[0]["Super" + i].ToString();
                    if (idSuper != null && Convert.ToInt32(idSuper) != -99)
                    {
                        //ne asiguram ca exista user pentru supervizorul din circuit
                        if (Convert.ToInt32(idSuper) < 0)
                        {
                            int idSpr = Convert.ToInt32(idSuper);
                            strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + F10003.ToString() + " AND \"IdSuper\" = " + (-1 * idSpr).ToString();
                            dtTemp = General.IncarcaDT(strSql, null);
                            if (dtTemp == null || dtTemp.Rows.Count == 0 || dtTemp.Rows[0]["IdUser"] == null || dtTemp.Rows[0]["IdUser"].ToString().Length <= 0)
                            {
                                continue;
                            }
                        }

                        total++;
                    }
                }
            }

            //validari
            string sqlCer = @"SELECT " +
                                idUrm.ToString() + " AS \"Id\", " +
                                F10003.ToString() + " AS \"F10003\", " +
                                idAtr.ToString() + " AS \"IdAtribut\", " +
                                idCircuit.ToString() + " AS \"IdCircuit\", " +
                                dataModif + " AS \"DataModif\", " +
                                idStare.ToString() + " AS \"IdStare\", " +
                                Session["UserId"] + " AS \"UserIntrod\", " +
                                "(SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString() + ") AS \"Culoare\", " +
                                total.ToString() + " AS \"TotalCircuit\", " +
                                pozUser.ToString() + " AS \"Pozitie\", " +
                                (chkGen.Checked ? "1" : "0") + " AS \"GenerareDoc\", " +
                                Session["UserId"] + " AS USER_NO, " + General.CurrentDate() + " AS TIME ";
            string msg = Notif.TrimiteNotificare("Avs.Cereri", (int)Constante.TipNotificare.Validare, sqlCer + ", 1 AS \"Actiune\", 1 AS \"IdStareViitoare\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
            if (msg != "" && msg.Substring(0, 1) == "2")
            {
                pnlCtl.JSProperties["cpAlertMessage"] = msg.Substring(2);
                return false;
            }


            //adaugam istoricul
            int poz = 0;
            int idUserCalc = -99;

            for (int i = 1; i <= 20; i++)
            {
                if (dtCir != null && dtCir.Rows.Count > 0 && dtCir.Rows[0]["Super" + i] != null && dtCir.Rows[0]["Super" + i].ToString().Length > 0)
                {
                    string valId = dtCir.Rows[0]["Super" + i].ToString();
                    if (valId != null && Convert.ToInt32(valId) != -99)
                    {
                        //poz++;
                        int usr = Convert.ToInt32(valId);

                        //IdUser
                        if (Convert.ToInt32(valId) == 0)
                        {
                            //idUserCalc = idUser;
                            strSql = "SELECT * FROM USERS WHERE F10003 = " + F10003.ToString();
                            dtTemp = General.IncarcaDT(strSql, null);
                            if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["F70102"] != null)
                            {
                                idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["F70102"].ToString());
                            }
                        }
                        if (Convert.ToInt32(valId) > 0) idUserCalc = Convert.ToInt32(valId);
                        if (Convert.ToInt32(valId) < 0)
                        {
                            int idSpr = Convert.ToInt32(valId);
                            //verif. daca nu cumva user-ul logat este deja un superviozr pt acest angajat;
                            //astfel se rezolva problema cand, de exemplu, un angajat are mai multi AdminRu
                            strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + F10003.ToString() + " AND \"IdSuper\" = " + (-1 * idSpr).ToString() + " AND \"IdUser\" = " + Session["UserId"].ToString();
                            dtTemp = General.IncarcaDT(strSql, null);
                            if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["IdUser"] != null && dtTemp.Rows[0]["IdUser"].ToString().Length > 0)
                            {
                                idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["IdUser"].ToString());
                            }
                            else
                            {
                                //ne asiguram ca exista user pentru supervizorul din circuit
                                strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + F10003.ToString() + " AND \"IdSuper\" = " + (-1 * idSpr).ToString();
                                dtTemp = General.IncarcaDT(strSql, null);
                                if (dtTemp == null || dtTemp.Rows.Count == 0 || dtTemp.Rows[0]["IdUser"] == null || dtTemp.Rows[0]["IdUser"].ToString().Length <= 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["IdUser"].ToString());
                                }
                            }
                        }

                        poz += 1;


                        //starea
                        string camp3 = ", \"IdStare\", \"Culoare\"";
                        //string camp4 = ", " + idStare + ", (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString() + ")";
                        string camp4 = ",null, null";
                        if (idUserCalc == Convert.ToInt32(Session["UserId"].ToString()))
                        {
                            pozUser = poz;
                            if (poz == 1) idStare = 1;
                            if (poz == total) idStare = 3;
                            camp3 = ", \"Aprobat\", \"DataAprobare\", \"IdStare\", \"Culoare\"";
                            camp4 = ", 1, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", " + idStare + ", (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString() + ")";
                        }

                        strSql = "INSERT INTO \"Avs_CereriIstoric\" (\"Id\", \"IdCircuit\", \"IdSuper\", \"Pozitie\", USER_NO, TIME, \"Inlocuitor\", \"IdUser\" {0}) "
                            + "VALUES (" + idUrm + ", " + idCircuit + ", " + Convert.ToInt32(valId) + ", " + poz + ", " + Session["UserId"].ToString() + ", "
                            + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", 0, " + idUserCalc + " {1})";
                        strSql = string.Format(strSql, camp3, camp4);
                        General.ExecutaNonQuery(strSql, null);
                    }
                }
            }

            sql = "INSERT INTO \"Avs_Cereri\" (\"Id\", F10003, \"IdAtribut\", \"IdCircuit\", \"Explicatii\", \"Document\", \"Motiv\", \"DataModif\", \"DataConsemnare\", \"Corectie\", \"Actualizat\", \"UserIntrod\", USER_NO, TIME, \"IdStare\", \"Culoare\", \"TotalCircuit\", \"Pozitie\", \"GenerareDoc\", {0}) "
                + "VALUES (" + idUrm.ToString() + ", " + F10003.ToString() + ", " + idAtr.ToString() + ", " + idCircuit.ToString() + ", '" + txtExpl.Text + "', '" + txtDocument.Text + "', '', " + dataModif + ", null, 0, 0, " + Session["UserId"].ToString() + ", "
                + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", " + idStare.ToString() + ", (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString() + "), " + total.ToString() + ", " + pozUser.ToString() + ", " + (chkGen.Checked ? "1" : "0") + ",  {1})";


            sql = string.Format(sql, camp1, camp2);
            General.ExecutaNonQuery(sql, null);

            if (sqlFunc.Length > 0)
                General.ExecutaNonQuery(sqlFunc, null);

            if (Session["Avs_Cereri_Date"] != null)
            {
                metaCereriDate itm = Session["Avs_Cereri_Date"] as metaCereriDate;
                if (itm.UploadedFile != null)
                {
                    string sqlFis = $@"INSERT INTO ""tblFisiere""(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                    General.ExecutaNonQuery(sqlFis, new object[] { "Avs_Cereri", idUrm, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
                }
            }

            #region OLD
            //dtTemp = General.IncarcaDT(sql, null);
            //if (Constante.tipBD == 1)
            //{
            //    SqlDataAdapter da = new SqlDataAdapter();
            //    SqlCommandBuilder cb = new SqlCommandBuilder();
            //    da = new SqlDataAdapter();
            //    da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"Avs_Cereri\"", null);
            //    cb = new SqlCommandBuilder(da);
            //    da.Update(dtTemp);
            //    da.Dispose();
            //    da = null;

            //    dtTemp = General.IncarcaDT(strSql, null);
            //    da = new SqlDataAdapter();
            //    da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"Avs_CereriIstoric\"", null);
            //    cb = new SqlCommandBuilder(da);
            //    da.Update(dtTemp);
            //    da.Dispose();
            //    da = null;
            //}
            //else
            //{
            //    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
            //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Avs_Cereri\" WHERE ROWNUM = 0", null);
            //    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
            //    oledbAdapter.Update(dtTemp);
            //    oledbAdapter.Dispose();
            //    oledbAdapter = null;

            //    dtTemp = General.IncarcaDT(strSql, null);
            //    oledbAdapter = new OracleDataAdapter();
            //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Avs_CereriIstoric\" WHERE ROWNUM = 0", null);
            //    cb = new OracleCommandBuilder(oledbAdapter);
            //    oledbAdapter.Update(dtTemp);
            //    oledbAdapter.Dispose();
            //    oledbAdapter = null;

            //}
            #endregion


            //Florin 2018.11.16
            //daca s-a modificat functia modificam si in posturi 
            //Florin 2019.03.01
            //s-a adaugat conditia cu parametrul
            //Florin 2019.07.29
            //s-a adaugat si parametrul cu id-uri excluse
            string idExcluse = "," + Dami.ValoareParam("IdExcluseCircuitDoc") + ",";
            if (idStare == 3 && (Dami.ValoareParam("FinalizareCuActeAditionale") == "0" || (Dami.ValoareParam("FinalizareCuActeAditionale") == "1" && idExcluse.IndexOf("," + idAtr + ",") >=0) || !chkGen.Checked))
            {
                TrimiteInF704(idUrm);
                if (idAtr == 2)
                    General.ModificaFunctieAngajat(F10003, Convert.ToInt32(General.Nz(cmb1Nou.Value,-99)), Convert.ToDateTime(txtDataMod.Value), new DateTime(2100,1,1));
            }


            string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };

            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
            {
                NotifAsync.TrimiteNotificare("Avs.Cereri", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Avs_Cereri"" Z WHERE ""Id""=" + idUrm, "Avs_Cereri", idUrm, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), arrParam);
            });

            Session["AvsCereri"] = null;
            Session["AvsCereriCalcul"] = null;
            Session["Avs_Cereri_Date"] = null;

            //ArataMesaj("Proces finalizat cu succes!");
            pnlCtl.JSProperties["cpAlertMessage"] = (Session["Avs_MesajNorma"] != null ? Session["Avs_MesajNorma"].ToString() : "") + Dami.TraduCuvant("Proces finalizat cu succes!");
            Session["Avs_MesajNorma"] = null;
            AscundeCtl();
            txtExpl.Text = "";
            cmbAtribute.Value = null;
            return true;

        }

        private void IncarcaGrid(int f10003 = -99)
        {
            DataTable dt = null;

            try
            {
                string strSql = "";
                string filtru = "";

                if (cmbAngFiltru.SelectedIndex >= 0) filtru += " AND a.F10003 = " + cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString();
                if (cmbAtributeFiltru.SelectedIndex >= 0) filtru += " AND a.\"IdAtribut\" = " + cmbAtributeFiltru.Items[cmbAtributeFiltru.SelectedIndex].Value.ToString();
                if (checkComboBoxStare.Value != null) filtru += " AND a.\"IdStare\" IN (" + FiltruTipStari(checkComboBoxStare.Value.ToString()).Replace(";", ",").Substring(0, FiltruTipStari(checkComboBoxStare.Value.ToString()).Length - 1) + ")";
                if (General.Nz(cmbAngFiltru.Value, "").ToString() == "" && f10003 != -99) filtru += " AND a.F10003=" + f10003;

                if (Constante.tipBD == 1)
                {
                    strSql = "select a.\"Id\", a.F10003, b.F10008 + ' ' + b.F10009 AS \"NumeAngajat\", a.\"IdAtribut\", c.\"Denumire\" AS \"NumeAtribut\" , a.\"DataModif\", a.\"Explicatii\", a.\"Motiv\", a.\"IdStare\", a.\"Culoare\",  " +
                            " COALESCE((SELECT COALESCE(F70420,0) FROM F704 WHERE F70403 = A.\"F10003\" AND F70404 = A.\"IdAtribut\" AND F70406 = A.\"DataModif\"),0) AS \"Actualizat\", " +
                            " case a.\"IdAtribut\"  " +
                            " when 1 then convert(nvarchar(20),a.\"SalariulBrut\")  " +
                            " when 2 then a.\"FunctieNume\"  " +
                            " when 3 then a.\"CORNume\"  " +
                            " when 4 then a.\"MotivNume\"  " +
                            " when 5 then a.\"DeptNume\"  " +
                            " when 6 then convert(nvarchar(20),a.\"TimpPartial\")  " +
                            " when 8 then convert(nvarchar(20),a.\"NrIntern\") + ' / ' + convert(nvarchar(20),a.\"DataIntern\",103)  " +
                            " when 9 then convert(nvarchar(20),a.\"NrITM\") + ' / ' + convert(nvarchar(20),a.\"DataITM\",103)  " +
                            " when 10 then convert(nvarchar(20),a.\"DataAngajarii\",103)  " +
                            " when 11 then ''  " +
                            " when 12 then a.\"TitlulAcademicNume\"  " +
                            " when 13 then a.\"MesajPersonalNume\"  " +
                            " when 14 then a.\"CentruCostNume\"  " +
                            " when 15 then ''  " +
                            " when 16 then a.\"PunctLucruNume\"  " +
                            " when 22 then a.\"MeserieNume\"  " +
                            " when 23 then ''  " +
                            " when 24 then ''  " +
                            " when 25 then convert(nvarchar(20),a.\"DataInceputCIM\",103) + ' - ' + convert(nvarchar(20),a.\"DataSfarsitCIM\",103)  " +
                            " when 26 then convert(nvarchar(20),a.\"DataInceputCIM\",103) + ' - ' + convert(nvarchar(20),a.\"DataSfarsitCIM\",103)  " +
                            " when 30 then a.\"MotivSuspNume\" + ' ' + convert(nvarchar(20),a.\"DataInceputSusp\",103) + ' - ' + convert(nvarchar(20),a.\"DataSfEstimSusp\",103) " +
                            " when 31 then a.\"MotivSuspNume\" + ' ' + convert(nvarchar(20),a.\"DataInceputSusp\",103) + ' - ' + convert(nvarchar(20),a.\"DataIncetareSusp\",103) " +
                            " when 32 then convert(nvarchar(20),a.\"DataInceputDet\",103) + ' - ' + convert(nvarchar(20),a.\"DataSfEstimDet\",103) " +
                            " when 33 then convert(nvarchar(20),a.\"DataInceputDet\",103) + ' - ' + convert(nvarchar(20),a.\"DataIncetareDet\",103) " +
                            " when 34 then (select \"Ptj_Contracte\".\"Denumire\" from \"Ptj_Contracte\" where \"Ptj_Contracte\".\"Id\" = a.\"ProgramLucru\") " +
                            " when 35 then a.\"TipContractNume\"" +
                            " when 36 then a.\"DurataContractNume\"" +
                            " when 101 then a.Nume + ' ' + a.Prenume " +
                            " when 102 then (select F06303 from F063 where F06302 = a.CASS) " +
                            " when 103 then (select F71204 from F712 where F71202 = a.\"Studii\") " +
                            " when 104 then CASE WHEN a.\"IBANSal\" IS NULL OR a.\"IBANSal\" = '' THEN a.\"NrCard\" ELSE a.\"IBANSal\" END " +
                            " when 105 then a.\"IBANGar\" " +
                            " when 106 then (select \"Denumire\" from \"tblLimbi\" where \"tblLimbi\".\"IdAuto\" = a.\"IdLimba\") " +
                            " when 107 then a.\"SerieNrID\" " +
                            " when 108 then (select F71404 from F714 where F71402 = a.\"IdCateg\") " +
                            " when 109 then convert(varchar, a.\"BonusTeamLeader\") " +
                            " end AS \"ValoareNoua\",  " +
                            " a.\"SalariulNet\", a.\"ScutitImpozit\"  " +
                            " from \"Avs_Cereri\" a  " +
                            " inner join F100 b on a.F10003=b.F10003  " +
                            " inner join \"Avs_tblAtribute\" c on a.\"IdAtribut\"=c.\"Id\"  " +
                            " inner join (SELECT \"Id\" FROM \"Avs_CereriIstoric\" WHERE \"IdUser\"=" + Session["UserId"].ToString() + " GROUP BY \"Id\") f on a.\"Id\" = f.\"Id\"  " +
                            " where 1=1 " + filtru +
                            " order by a.\"DataModif\"";
                }
                else
                {
                    strSql = "select a.\"Id\", a.F10003, b.F10008 || ' ' || b.F10009 AS \"NumeAngajat\", a.\"IdAtribut\", c.\"Denumire\" AS \"NumeAtribut\" , a.\"DataModif\", a.\"Explicatii\", a.\"Motiv\", a.\"IdStare\", a.\"Culoare\", " +
                            " COALESCE((SELECT COALESCE(F70420,0) FROM F704 WHERE F70403 = A.\"F10003\" AND F70404 = A.\"IdAtribut\" AND F70406 = A.\"DataModif\"),0) AS \"Actualizat\", " +
                            " case a.\"IdAtribut\" " +
                            " when 1 then to_char(a.\"SalariulBrut\") " +
                            " when 2 then a.\"FunctieNume\" " +
                            " when 3 then a.\"CORNume\" " +
                            " when 4 then a.\"MotivNume\" " +
                            " when 5 then a.\"DeptNume\"  " +
                            " when 6 then to_char(a.\"TimpPartial\") " +
                            " when 8 then to_char(a.\"NrIntern\") || ' / ' || to_char(a.\"DataIntern\",'DD/MM/YYYY') " +
                            " when 9 then to_char(a.\"NrITM\") || ' / ' || to_char(a.\"DataITM\",'DD/MM/YYYY') " +
                            " when 10 then to_char(a.\"DataAngajarii\",'DD/MM/YYYY') " +
                            " when 11 then '' " +
                            " when 12 then a.\"TitlulAcademicNume\" " +
                            " when 13 then a.\"MesajPersonalNume\" " +
                            " when 14 then a.\"CentruCostNume\" " +
                            " when 15 then '' " +
                            " when 16 then a.\"PunctLucruNume\" " +
                            " when 22 then a.\"MeserieNume\" " +
                            " when 23 then '' " +
                            " when 24 then '' " +
                            " when 25 then to_char(a.\"DataInceputCIM\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfarsitCIM\",'DD/MM/YYYY') " +
                            " when 26 then to_char(a.\"DataInceputCIM\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfarsitCIM\",'DD/MM/YYYY') " +
                            " when 30 then a.\"MotivSuspNume\" || ' ' || to_char(a.\"DataInceputSusp\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfEstimSusp\",'DD/MM/YYYY') " +
                            " when 31 then a.\"MotivSuspNume\" || ' ' || to_char(a.\"DataInceputSusp\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataIncetareSusp\",'DD/MM/YYYY') " +
                            " when 32 then to_char(a.\"DataInceputDet\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfEstimDet\",'DD/MM/YYYY') " +
                            " when 33 then to_char(a.\"DataInceputDet\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataIncetareDet\",'DD/MM/YYYY') " +
                            " when 34 then (select \"Ptj_Contracte\".\"Denumire\" from \"Ptj_Contracte\" where \"Ptj_Contracte\".\"Id\" = a.\"ProgramLucru\") " +
                            " when 35 then a.\"TipContractNume\"" +
                            " when 36 then a.\"DurataContractNume\"" +
                            " when 101 then a.\"Nume\" || ' ' || a.\"Prenume\" " +
                            " when 102 then (select F06303 from F063 where F06302 = a.CASS) " +
                            " when 103 then (select F71204 from F712 where F71202 = a.\"Studii\") " +
                            " when 104 then CASE WHEN a.\"IBANSal\" IS NULL THEN a.\"NrCard\" ELSE a.\"IBANSal\" END  " +
                            " when 105 then a.\"IBANGar\" " +
                            " when 106 then (select \"Denumire\" from \"tblLimbi\" where \"tblLimbi\".\"IdAuto\" = a.\"IdLimba\") " +
                            " when 107 then a.\"SerieNrID\" " +
                            " when 108 then (select F71404 from F714 where F71402 = a.\"IdCateg\") " +
                            " when 109 then TO_CHAR(a.\"BonusTeamLeader\") " +
                            " end AS \"ValoareNoua\", " +
                            " a.\"SalariulNet\", a.\"ScutitImpozit\" " +
                            " from \"Avs_Cereri\" a " +
                            " inner join F100 b on a.F10003=b.F10003 " +
                            " inner join \"Avs_tblAtribute\" c on a.\"IdAtribut\"=c.\"Id\" " +
                            " inner join (SELECT \"Id\" FROM \"Avs_CereriIstoric\" WHERE \"IdUser\"=" + Session["UserId"].ToString() + " GROUP BY \"Id\") f on a.\"Id\" = f.\"Id\" " +
                            " where 1=1 " + filtru +
                            " order by a.\"DataModif\"";
                }

                dt = General.IncarcaDT(strSql, null);
                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["Avs_Grid"] = dt;
            }
            catch (Exception)
            {
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string FiltruTipStari(string stari)
        {
            string val = "";

            try
            {
                string[] param = stari.Split(';');
                foreach (string elem in param)
                {
                    switch (elem.ToLower())
                    {
                        case "solicitat":
                            val += "1;";
                            break;
                        case "in curs":
                            val += "2;";
                            break;
                        case "aprobat":
                            val += "3;";
                            break;
                        case "respins":
                            val += "0;";
                            break;
                        case "anulat":
                            val += "-1;";
                            break;
                        case "planificat":
                            val += "4;";
                            break;
                        case "prezent":
                            val += "5;";
                            break;
                        case "absent":
                            val += "6;";
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }

            return val;
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
                //if (e.DataColumn.Name == "butoaneGrid")
                //{
                //    int idAtr = Convert.ToInt32(e.GetValue("IdAtribut").ToString());
                //    if (idAtr != 11 && idAtr != 15 && idAtr != 27 && idAtr != 28)
                //        btnDetalii.Visibility = GridViewCustomButtonVisibility.Invisible;
                //}
                
            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDate.GetRow(e.VisibleIndex) as DataRowView;
                if (values != null)
                {
                    int idAtr = Convert.ToInt32(values.Row["IdAtribut"].ToString());
                    if (e.ButtonID == "btnDetalii")
                    {
                        if (idAtr != (int)Constante.Atribute.Sporuri && idAtr != (int)Constante.Atribute.SporTranzactii
                            && idAtr != (int)Constante.Atribute.Componente && idAtr != (int)Constante.Atribute.Tarife)
                        {
                            e.Visible = DefaultBoolean.False;
                            
                        }
                    }

                    if (e.ButtonID == "btnArata")
                    {     
                        if (idAtr == (int)Constante.Atribute.BancaSalariu || idAtr == (int)Constante.Atribute.BancaGarantii
                            || idAtr == (int)Constante.Atribute.DocId || idAtr == (int)Constante.Atribute.PermisAuto)
                        {
                            e.Visible = DefaultBoolean.True;

                        }
                        else
                            e.Visible = DefaultBoolean.False;
                    }

                }
            }
        }

        protected void StergeFiltre()
        {
            cmbAngFiltru.SelectedIndex = -1;
            cmbAngFiltru.Value = null;
            cmbAtributeFiltru.SelectedIndex = -1;
            cmbAtributeFiltru.Value = null;
            checkComboBoxStare.Value = null;
            Session["Avs_MarcaFiltru"] = null;
            Session["Avs_AtributFiltru"] = null;
            Session["Avs_MarcaFiltru1"] = null;

            grDate.KeyFieldName = "Id";
            grDate.DataSource = null;
            grDate.DataBind();
            Session["Avs_Grid"] = null;

        }



        public void TrimiteInF704(int id)
        {
            try
            {
                string data = "", data2 = "", data3 = "", data4 = "", data5 = "", data6 = "", data7 = "", data8 = "", data9 = "", data10 = "", 
                    data11 = "", data12 = "", data13 = "", data14 = "", data15 = "", data16 = "";
                if (Constante.tipBD == 1)
                {
                    data = "CASE WHEN a.DataModif IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataModif, 103) END";
                    data2 = "CASE WHEN a.DataIntern IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataIntern, 103) END";
                    data3 = "CASE WHEN a.DataITM IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataITM, 103) END";
                    data4 = "CASE WHEN a.DataAngajarii IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataAngajarii, 103) END";
                    data5 = "CASE WHEN a.DataElibID IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataElibID, 103) END";
                    data6 = "CASE WHEN a.DataExpID IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataExpID, 103) END";
                    data7 = "CASE WHEN a.DataEmiterePermis IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataEmiterePermis, 103) END";
                    data8 = "CASE WHEN a.DataExpirarePermis IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataExpirarePermis, 103) END";
                    data9 = "CASE WHEN a.DataInceputCIM IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataInceputCIM, 103) END";
                    data10 = "CASE WHEN a.DataSfarsitCIM IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataSfarsitCIM, 103) END";
                    data11 = "CASE WHEN a.DataInceputSusp IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataInceputSusp, 103) END";
                    data12 = "CASE WHEN a.DataSfEstimSusp IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataSfEstimSusp, 103) END";
                    data13 = "CASE WHEN a.DataIncetareSusp IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataIncetareSusp, 103) END";
                    data14 = "CASE WHEN a.DataInceputDet IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataInceputDet, 103) END";
                    data15 = "CASE WHEN a.DataSfEstimDet IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataSfEstimDet, 103) END";
                    data16 = "CASE WHEN a.DataIncetareDet IS NULL THEN '01/01/2100' ELSE CONVERT(VARCHAR, a.DataIncetareDet, 103) END";
                }
                else
                {
                    data = "CASE WHEN a.\"DataModif\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataModif\", 'dd/mm/yyyy') END";
                    data2 = "CASE WHEN a.\"DataIntern\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataIntern\", 'dd/mm/yyyy') END";
                    data3 = "CASE WHEN a.\"DataITM\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataITM\", 'dd/mm/yyyy') END";
                    data4 = "CASE WHEN a.\"DataAngajarii\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataAngajarii\", 'dd/mm/yyyy') END";
                    data5 = "CASE WHEN a.\"DataElibID\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataElibID\", 'dd/mm/yyyy') END";
                    data6 = "CASE WHEN a.\"DataExpID\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataExpID\", 'dd/mm/yyyy') END";
                    data7 = "CASE WHEN a.\"DataEmiterePermis\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataEmiterePermis\", 'dd/mm/yyyy') END";
                    data8 = "CASE WHEN a.\"DataExpirarePermis\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataExpirarePermis\", 'dd/mm/yyyy') END";
                    data9 = "CASE WHEN a.\"DataInceputCIM\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataInceputCIM\", 'dd/mm/yyyy') END";
                    data10 = "CASE WHEN a.\"DataSfarsitCIM\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataSfarsitCIM\", 'dd/mm/yyyy') END";
                    data11 = "CASE WHEN a.\"DataInceputSusp\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataInceputSusp\", 'dd/mm/yyyy') END";
                    data12 = "CASE WHEN a.\"DataSfEstimSusp\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataSfEstimSusp\", 'dd/mm/yyyy') END";
                    data13 = "CASE WHEN a.\"DataIncetareSusp\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataIncetareSusp\", 'dd/mm/yyyy') END";
                    data14 = "CASE WHEN a.\"DataInceputDet\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataInceputDet\", 'dd/mm/yyyy') END";
                    data15 = "CASE WHEN a.\"DataSfEstimDet\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataSfEstimDet\", 'dd/mm/yyyy') END";
                    data16 = "CASE WHEN a.\"DataIncetareDet\" IS NULL THEN '01/01/2100' ELSE TO_CHAR(a.\"DataIncetareDet\", 'dd/mm/yyyy') END";
                }


                //Florin 2020.06.11 Begin
                //1 - am adaugat campul DocumentCircuit
                //2 - s-a inlocuit peste tot dtCer.Rows[0]["Document"].ToString() cu dateDoc 

                string sqlDoc = @"SELECT CONVERT(nvarchar(20),X.DocNr) + '/' + CONVERT(nvarchar(10),X.DocData,103) FROM Admin_NrActAd X WHERE X.IdAuto=A.IdActAd";
                if (Constante.tipBD == 2)
                    sqlDoc = @"SELECT X.""DocNr"" || '/' || TO_CHAR('DD/MM/YYYY',X.""DocData"") FROM ""Admin_NrActAd"" X WHERE X.""IdAuto"" = A.""IdActAd""";

                DataTable dtCer = General.IncarcaDT("SELECT " + data + " AS DM, " + data2 + " AS DI, " + data3 + " AS DITM, " + data4 + " AS DA, " + data5 + " AS DELIB, "
                    + data6 + " AS DEXP, " + data7 + " AS DEMIT, " + data8 + " AS DEXPP, " + data9 + " AS DINC, " + data10 + " AS DSF, "
                    + data11 + " AS DIS, " + data12 + " AS DSES, " + data13 + " AS DSFS, " + data14 + " AS DID, " + data15 + " AS DSED, " + data16 + " AS DSFD, " +
                    " (" + sqlDoc + ") AS \"DocumentCircuit\", " +
                    "A.* FROM \"Avs_Cereri\" a WHERE \"Id\" = " + id.ToString(), null);
                if (dtCer == null || dtCer.Rows.Count == 0) return;
                
                string dateDoc = General.Nz(dtCer.Rows[0]["Document"], "").ToString();
                if (General.Nz(dtCer.Rows[0]["DocumentCircuit"], "").ToString() != "")
                    dateDoc = General.Nz(dtCer.Rows[0]["DocumentCircuit"], "").ToString();

                //Florin 2020.06.11 End


                int? idComp = 1;
                DataTable dtComp = General.IncarcaDT("SELECT F10002 FROM F100 WHERE F10003 = " + dtCer.Rows[0]["F10003"].ToString(), null);
                if (dtComp != null && dtComp.Rows.Count > 0 && dtComp.Rows[0]["F10002"] != null && dtComp.Rows[0]["F10002"].ToString().Length > 0)
                    idComp = Convert.ToInt32(dtComp.Rows[0]["F10002"].ToString());

                DateTime dtLucru = General.DamiDataLucru();
                int f10003 = Convert.ToInt32(dtCer.Rows[0]["F10003"].ToString());
                DateTime dtModif = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DM"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DM"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DM"].ToString().Substring(0, 2)));
                DateTime dtIntern = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DI"].ToString().Substring(0, 2)));
                DateTime dtITM = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DITM"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DITM"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DITM"].ToString().Substring(0, 2)));
                DateTime dtAng = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DA"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DA"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DA"].ToString().Substring(0, 2)));
                DateTime dtElib = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DELIB"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DELIB"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DELIB"].ToString().Substring(0, 2)));
                DateTime dtExp = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DEXP"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DEXP"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DEXP"].ToString().Substring(0, 2)));
                DateTime dtEmit = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DEMIT"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DEMIT"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DEMIT"].ToString().Substring(0, 2)));
                DateTime dtExpP = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DEXPP"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DEXPP"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DEXPP"].ToString().Substring(0, 2)));
                DateTime dtInc = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DINC"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DINC"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DINC"].ToString().Substring(0, 2)));
                DateTime dtSf = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DSF"].ToString().Substring(0, 2)));
                DateTime dtIncSusp = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DIS"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DIS"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DIS"].ToString().Substring(0, 2)));
                DateTime dtSfEstSusp = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DSES"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DSES"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DSES"].ToString().Substring(0, 2)));
                DateTime dtSfSusp = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DSFS"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DSFS"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DSFS"].ToString().Substring(0, 2)));
                DateTime dtIncDet = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DID"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DID"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DID"].ToString().Substring(0, 2)));
                DateTime dtSfEstDet = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DSED"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DSED"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DSED"].ToString().Substring(0, 2)));
                DateTime dtSfDet = new DateTime(Convert.ToInt32(dtCer.Rows[0]["DSFD"].ToString().Substring(6, 4)), Convert.ToInt32(dtCer.Rows[0]["DSFD"].ToString().Substring(3, 2)), Convert.ToInt32(dtCer.Rows[0]["DSFD"].ToString().Substring(0, 2)));


                DataTable dtF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + f10003.ToString(), null);
                DataTable dtF1001 = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + f10003.ToString(), null);

                string data1 = "";
                //string dataInceputSusp = "", dataInceputDet = "";
                if (Constante.tipBD == 1)
                {
                    data = "CONVERT(DATETIME, '" + dtModif.Day.ToString().PadLeft(2, '0') + "/" + dtModif.Month.ToString().PadLeft(2, '0') + "/" + dtModif.Year.ToString() + "', 103)";
                    data1 = "CONVERT(DATETIME, '" + dtModif.AddDays(-1).Day.ToString().PadLeft(2, '0') + "/" + dtModif.AddDays(-1).Month.ToString().PadLeft(2, '0') + "/" + dtModif.AddDays(-1).Year.ToString() + "', 103)";
                    data2 = "CONVERT(DATETIME, '" + dtIntern.Day.ToString().PadLeft(2, '0') + "/" + dtIntern.Month.ToString().PadLeft(2, '0') + "/" + dtIntern.Year.ToString() + "', 103)";
                    data3 = "CONVERT(DATETIME, '" + dtITM.Day.ToString().PadLeft(2, '0') + "/" + dtITM.Month.ToString().PadLeft(2, '0') + "/" + dtITM.Year.ToString() + "', 103)";
                    data4 = "CONVERT(DATETIME, '" + dtAng.Day.ToString().PadLeft(2, '0') + "/" + dtAng.Month.ToString().PadLeft(2, '0') + "/" + dtAng.Year.ToString() + "', 103)";
                    data5 = "CONVERT(DATETIME, '" + dtElib.Day.ToString().PadLeft(2, '0') + "/" + dtElib.Month.ToString().PadLeft(2, '0') + "/" + dtElib.Year.ToString() + "', 103)";
                    data6 = "CONVERT(DATETIME, '" + dtExp.Day.ToString().PadLeft(2, '0') + "/" + dtExp.Month.ToString().PadLeft(2, '0') + "/" + dtExp.Year.ToString() + "', 103)";
                    data7 = "CONVERT(DATETIME, '" + dtEmit.Day.ToString().PadLeft(2, '0') + "/" + dtEmit.Month.ToString().PadLeft(2, '0') + "/" + dtEmit.Year.ToString() + "', 103)";
                    data8 = "CONVERT(DATETIME, '" + dtExpP.Day.ToString().PadLeft(2, '0') + "/" + dtExpP.Month.ToString().PadLeft(2, '0') + "/" + dtExpP.Year.ToString() + "', 103)";
                    data9 = "CONVERT(DATETIME, '" + dtInc.Day.ToString().PadLeft(2, '0') + "/" + dtInc.Month.ToString().PadLeft(2, '0') + "/" + dtInc.Year.ToString() + "', 103)";
                    data10 = "CONVERT(DATETIME, '" + dtSf.Day.ToString().PadLeft(2, '0') + "/" + dtSf.Month.ToString().PadLeft(2, '0') + "/" + dtSf.Year.ToString() + "', 103)";
                    data11 = "CONVERT(DATETIME, '" + dtIncSusp.Day.ToString().PadLeft(2, '0') + "/" + dtIncSusp.Month.ToString().PadLeft(2, '0') + "/" + dtIncSusp.Year.ToString() + "', 103)";
                    data12 = "CONVERT(DATETIME, '" + dtSfEstSusp.Day.ToString().PadLeft(2, '0') + "/" + dtSfEstSusp.Month.ToString().PadLeft(2, '0') + "/" + dtSfEstSusp.Year.ToString() + "', 103)";
                    data13 = "CONVERT(DATETIME, '" + dtSfSusp.Day.ToString().PadLeft(2, '0') + "/" + dtSfSusp.Month.ToString().PadLeft(2, '0') + "/" + dtSfSusp.Year.ToString() + "', 103)";
                    data14 = "CONVERT(DATETIME, '" + dtIncDet.Day.ToString().PadLeft(2, '0') + "/" + dtIncDet.Month.ToString().PadLeft(2, '0') + "/" + dtIncDet.Year.ToString() + "', 103)";
                    data15 = "CONVERT(DATETIME, '" + dtSfEstDet.Day.ToString().PadLeft(2, '0') + "/" + dtSfEstDet.Month.ToString().PadLeft(2, '0') + "/" + dtSfEstDet.Year.ToString() + "', 103)";
                    data16 = "CONVERT(DATETIME, '" + dtSfDet.Day.ToString().PadLeft(2, '0') + "/" + dtSfDet.Month.ToString().PadLeft(2, '0') + "/" + dtSfDet.Year.ToString() + "', 103)";
                }
                else
                {
                    data = "TO_DATE('" + dtModif.Day.ToString().PadLeft(2, '0') + "/" + dtModif.Month.ToString().PadLeft(2, '0') + "/" + dtModif.Year.ToString() + "', 'dd/mm/yyyy')";
                    data1 = "TO_DATE('" + dtModif.AddDays(-1).Day.ToString().PadLeft(2, '0') + "/" + dtModif.AddDays(-1).Month.ToString().PadLeft(2, '0') + "/" + dtModif.AddDays(-1).Year.ToString() + "', 'dd/mm/yyyy')";
                    data2 = "TO_DATE('" + dtIntern.Day.ToString().PadLeft(2, '0') + "/" + dtIntern.Month.ToString().PadLeft(2, '0') + "/" + dtIntern.Year.ToString() + "', 'dd/mm/yyyy')";
                    data3 = "TO_DATE('" + dtITM.Day.ToString().PadLeft(2, '0') + "/" + dtITM.Month.ToString().PadLeft(2, '0') + "/" + dtITM.Year.ToString() + "', 'dd/mm/yyyy')";
                    data4 = "TO_DATE('" + dtAng.Day.ToString().PadLeft(2, '0') + "/" + dtAng.Month.ToString().PadLeft(2, '0') + "/" + dtAng.Year.ToString() + "', 'dd/mm/yyyy')";
                    data5 = "TO_DATE('" + dtElib.Day.ToString().PadLeft(2, '0') + "/" + dtElib.Month.ToString().PadLeft(2, '0') + "/" + dtElib.Year.ToString() + "', 'dd/mm/yyyy')";
                    data6 = "TO_DATE('" + dtExp.Day.ToString().PadLeft(2, '0') + "/" + dtExp.Month.ToString().PadLeft(2, '0') + "/" + dtExp.Year.ToString() + "', 'dd/mm/yyyy')";
                    data7 = "TO_DATE('" + dtEmit.Day.ToString().PadLeft(2, '0') + "/" + dtEmit.Month.ToString().PadLeft(2, '0') + "/" + dtEmit.Year.ToString() + "', 'dd/mm/yyyy')";
                    data8 = "TO_DATE('" + dtExpP.Day.ToString().PadLeft(2, '0') + "/" + dtExpP.Month.ToString().PadLeft(2, '0') + "/" + dtExpP.Year.ToString() + "', 'dd/mm/yyyy')";
                    data9 = "TO_DATE('" + dtInc.Day.ToString().PadLeft(2, '0') + "/" + dtInc.Month.ToString().PadLeft(2, '0') + "/" + dtInc.Year.ToString() + "', 'dd/mm/yyyy')";
                    data10 = "TO_DATE('" + dtSf.Day.ToString().PadLeft(2, '0') + "/" + dtSf.Month.ToString().PadLeft(2, '0') + "/" + dtSf.Year.ToString() + "', 'dd/mm/yyyy')";
                    data11 = "TO_DATE('" + dtIncSusp.Day.ToString().PadLeft(2, '0') + "/" + dtIncSusp.Month.ToString().PadLeft(2, '0') + "/" + dtIncSusp.Year.ToString() + "', 'dd/mm/yyyy')";
                    data12 = "TO_DATE('" + dtSfEstSusp.Day.ToString().PadLeft(2, '0') + "/" + dtSfEstSusp.Month.ToString().PadLeft(2, '0') + "/" + dtSfEstSusp.Year.ToString() + "', 'dd/mm/yyyy')";
                    data13 = "TO_DATE('" + dtSfSusp.Day.ToString().PadLeft(2, '0') + "/" + dtSfSusp.Month.ToString().PadLeft(2, '0') + "/" + dtSfSusp.Year.ToString() + "', 'dd/mm/yyyy')";
                    data14 = "TO_DATE('" + dtIncDet.Day.ToString().PadLeft(2, '0') + "/" + dtIncDet.Month.ToString().PadLeft(2, '0') + "/" + dtIncDet.Year.ToString() + "', 'dd/mm/yyyy')";
                    data15 = "TO_DATE('" + dtSfEstDet.Day.ToString().PadLeft(2, '0') + "/" + dtSfEstDet.Month.ToString().PadLeft(2, '0') + "/" + dtSfEstDet.Year.ToString() + "', 'dd/mm/yyyy')";
                    data16 = "TO_DATE('" + dtSfDet.Day.ToString().PadLeft(2, '0') + "/" + dtSfDet.Month.ToString().PadLeft(2, '0') + "/" + dtSfDet.Year.ToString() + "', 'dd/mm/yyyy')";
                }
                int act = 0;
                string sql = "", sql1 = "", sql100 = "", sql1001 = "";

                switch (Convert.ToInt32(dtCer.Rows[0]["IdAtribut"].ToString()))
                {
                    case (int)Constante.Atribute.Salariul:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                                act = 1;
                                sql100 = "UPDATE F100 SET F10026 = " + dtCer.Rows[0]["ScutitImpozit"].ToString() + ", " + salariu + " = " + dtCer.Rows[0]["SalariulBrut"].ToString() + ", F100991 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70452, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 1, 'Salariu Tarifar', " + data + ", " + dtCer.Rows[0]["SalariulBrut"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", " + dtCer.Rows[0]["ScutitImpozit"].ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Functie:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10071 = " + dtCer.Rows[0]["FunctieId"].ToString() + ", F100992 = " + data + ", F100975 = " + dtCer.Rows[0]["PerProbaZL"].ToString()
                                    + ", F1009742 = " + dtCer.Rows[0]["PreavizDemisie"].ToString() + ", F100931 = " + dtCer.Rows[0]["PreavizConcediere"].ToString() + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F1001063 = " + dtCer.Rows[0]["PerProbaZC"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 2, 'Functie', " + data + ", " + dtCer.Rows[0]["FunctieId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.CodCOR:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10098 = " + dtCer.Rows[0]["CORCod"].ToString() + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F100956 = " + data + " WHERE F10003 = " + f10003.ToString();

                                int afisare = Convert.ToInt32(Dami.ValoareParam("Avs_CampuriZileProbaPreavizCOR", "0"));
                                if (afisare == 1)
                                {
                                    sql100 = "UPDATE F100 SET F10098 = " + dtCer.Rows[0]["CORCod"].ToString() + ", F100975 = " + dtCer.Rows[0]["PerProbaZL"].ToString()
                                            + ", F1009742 = " + dtCer.Rows[0]["PreavizDemisie"].ToString() + ", F100931 = " + dtCer.Rows[0]["PreavizConcediere"].ToString() + " WHERE F10003 = " + f10003.ToString();
                                    if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                        sql1001 = "UPDATE F1001 SET F100956 = " + data + ", F1001063 = " + dtCer.Rows[0]["PerProbaZC"].ToString() + " WHERE F10003 = " + f10003.ToString();
                                }

                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 3, 'Cod COR', " + data + ", " + dtCer.Rows[0]["CORCod"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.MotivPlecare:
                        {
                            //Florin 2019.09.30
                            //if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            //{
                            act = 1;
                            string sql100Temp = "UPDATE F100 SET F10023 = " + data1 + ", F100993 = " + data + " WHERE F10003 = " + f10003.ToString();
                            General.IncarcaDT(sql100Temp, null);
                            //}


                            //sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            //+ " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 4, 'Motiv plecare', " + data + ", " + dtCer.Rows[0]["MotivId"].ToString() + ", 'Modificari in avans', '"
                            //+ dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";


                            //Florin 2019.09.30
                            //s-a adaugat top 1/rownum in subselect

                            //sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            //    + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 4, 'Motiv plecare', " + data + ", (SELECT F72102 FROM F721 WHERE F72106 = " + dtCer.Rows[0]["MotivId"].ToString() + "), 'Modificari in avans', '"
                            //    + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";

                            string subSel = $@"(SELECT TOP 1 F72102 FROM F721 WHERE F72106 = " + dtCer.Rows[0]["MotivId"].ToString() + ")";
                            if (Constante.tipBD == 2)
                                subSel = $@"(SELECT F72102 FROM F721 WHERE ROWNUM <= 1 AND F72106 = " + dtCer.Rows[0]["MotivId"].ToString() + ")";

                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                                + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 4, 'Motiv plecare', " + data + ", " + subSel + ", 'Modificari in avans', '"
                                + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";

                            //Radu 01.11.2019 - se modifica data plecarii, deci trebuie refacut CalculCO      
                            if (dtModif.Year > DateTime.Now.Year)
                            {
                                General.CalculCO(dtModif.Year, f10003);
                                General.CalculCO(DateTime.Now.Year, f10003);
                            }
                            else
                                General.CalculCO(dtModif.Year, f10003);

                            AdaugaNotaLichidare(f10003);
                        }
                        break;
                    case (int)Constante.Atribute.Norma:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10010 = " + dtCer.Rows[0]["TipAngajat"].ToString() + ", F10043 = " + dtCer.Rows[0]["TimpPartial"].ToString() + ", F100973 = " + dtCer.Rows[0]["Norma"].ToString()
                                    + ", F100926 = " + dtCer.Rows[0]["TipNorma"].ToString() + ", F100927 = " + dtCer.Rows[0]["DurataTimpMunca"].ToString() + ", F100928 = " + dtCer.Rows[0]["RepartizareTimpMunca"].ToString()
                                    + ", F100939 = " + dtCer.Rows[0]["IntervalRepartizare"].ToString() + ", F10067 = '" + dtCer.Rows[0]["Tarife"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F100955 = " + data + ", F100964 = " + dtCer.Rows[0]["NrOreLuna"].ToString() + "  WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70421, F70422, F70423, F70424, F70425, F70426, F70427, F70428, F70467, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 6, 'Norma Contract', " + data + ", " + dtCer.Rows[0]["TimpPartial"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", " + dtCer.Rows[0]["TipAngajat"].ToString() + ", " + dtCer.Rows[0]["TimpPartial"].ToString() + ", "
                            + dtCer.Rows[0]["Norma"].ToString() + ", " + dtCer.Rows[0]["TipNorma"].ToString() + ", " + dtCer.Rows[0]["DurataTimpMunca"].ToString() + ", " + dtCer.Rows[0]["RepartizareTimpMunca"].ToString()
                            + ", " + dtCer.Rows[0]["IntervalRepartizare"].ToString() + ", " + General.Nz(dtCer.Rows[0]["NrOreLuna"], "0").ToString() + ", '" + dtCer.Rows[0]["Tarife"].ToString() + "', -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";

                            //Radu 14.07.2020
                            string sqlNorma = "SELECT \"IdContract\" FROM \"F100Contracte\" WHERE F10003 = " + f10003.ToString() + " AND \"DataSfarsit\" = " + General.ToDataUniv(new DateTime(2100, 1, 1));
                            DataTable dtNorma = General.IncarcaDT(sqlNorma, null);
                            if ((dtNorma != null && dtNorma.Rows.Count > 0 && Convert.ToInt32(dtNorma.Rows[0][0].ToString()) != Convert.ToInt32(dtCer.Rows[0]["ProgramLucru"].ToString())) || dtNorma == null || dtNorma.Rows.Count <= 0)
                            {
                                DateTime tmpDtModif = Convert.ToDateTime(dtCer.Rows[0]["DataModif"]);
                                sqlNorma = "UPDATE \"F100Contracte\" SET \"DataSfarsit\" = " + General.ToDataUniv(tmpDtModif.AddDays(-1)) + " WHERE F10003 = " + f10003.ToString() + " AND \"DataSfarsit\" = " + General.ToDataUniv(new DateTime(2100, 1, 1));
                                General.ExecutaNonQuery(sqlNorma);

                                sqlNorma = "INSERT INTO \"F100Contracte\"(F10003, \"IdContract\", \"DataInceput\", \"DataSfarsit\", USER_NO, TIME) VALUES (" + f10003.ToString() + ", " + dtCer.Rows[0]["ProgramLucru"].ToString()
                                    + ", " + data + ", " + General.ToDataUniv(new DateTime(2100, 1, 1)) + ", " + Session["UserId"].ToString() + ", " + General.CurrentDate() + ")";
                                General.ExecutaNonQuery(sqlNorma);
                            }
                        }
                        break;
                    case (int)Constante.Atribute.ContrIn:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F100985 = " + dtCer.Rows[0]["NrIntern"].ToString() + ", F100986 = " + data2 + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70411, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 8, 'Nr si Data Contr.In', " + data + ", " + dtCer.Rows[0]["NrIntern"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + data2 + ", " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.ContrITM:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10011 = " + dtCer.Rows[0]["NrITM"].ToString() + ", FX1 = " + data3 + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 9, 'Nr si Data Contr.ITM', " + data + ", " + dtCer.Rows[0]["NrITM"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.DataAngajarii:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10022 = " + data4 + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70409, F70410, F70411, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 10, 'Data Angajarii', " + data + ", 'Modificari in avans', '"
                            + dateDoc + "', " + data4 + ", " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.TitluAcademic:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10051 = " + dtCer.Rows[0]["TitlulAcademicId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 12, 'Titlul academic', " + data + ", " + dtCer.Rows[0]["TitlulAcademicId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.MesajPersonal:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10061 = " + dtCer.Rows[0]["MesajPersonalId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 13, 'Mesaj personal', " + data + ", " + dtCer.Rows[0]["MesajPersonalId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.CentrulCost:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10053 = " + dtCer.Rows[0]["CentruCostId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 14, 'Centru cost', " + data + ", " + dtCer.Rows[0]["CentruCostId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.PunctLucru:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10079 = " + dtCer.Rows[0]["PunctLucruId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 16, 'Punct lucru', " + data + ", " + dtCer.Rows[0]["PunctLucruId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Meserie:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10029 = " + dtCer.Rows[0]["MeserieId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 22, 'Meserie', " + data + ", " + dtCer.Rows[0]["MeserieId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.PrelungireCIM:
                    case (int)Constante.Atribute.PrelungireCIM_Vanz:
                        {
                            //Florin 2019.04.09
                            //se doreste acelasi proces ca si la restul atributelor cu diferenta ca totul se face in WizOne fara a folosi tabela F704
                            //ne folosim de procesul de Inchidere luna din WizOne


                            //Florin 2019.09.10
                            //se doreste ca aceste modificari sa se duca in F100 indiferent daca sunt sau nu in luna de lucru
                            //if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            //{
                            //    act = 1;
                            //Radu - se salveaza mai intai in F100 si apoi in F095, deoarece contractul vechi exista deja in F095
                            //                  si apare eroare de violare cheie primara
                            int nrLuni = 0, nrZile = 0;
                            string sql100Tmp = "";
                            DateTime dtTmp = new DateTime();
                            DateTime dtf = new DateTime(2100, 1, 1, 0, 0, 0);
                            if (dtSf != dtf)
                                dtTmp = dtSf.AddDays(1);
                            else
                                dtTmp = dtSf;

                            Personal.Contract ctr = new Personal.Contract();
                            ctr.CalculLuniSiZile(Convert.ToDateTime(dtInc.Date), Convert.ToDateTime(dtSf.Date), out nrLuni, out nrZile);

                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100Tmp = "UPDATE F100 SET F100933 = " + data9 + ", F100934 = " + data10 + ", F100936 = " + nrZile.ToString() + ", F100935 = "
                                + nrLuni.ToString() + ", F100938 = 1, F10023 = " + data10
                                + ", F100993 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 103)"
                                : "TO_DATE('" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 'dd/mm/yyyy')") + ", F1009741 = " + dtCer.Rows[0]["DurataContract"].ToString() + "  WHERE F10003 = " + f10003.ToString();
                                General.IncarcaDT(sql100Tmp, null);

                                sql1001 = "UPDATE F1001 SET F1001138 = " + data + " WHERE F10003 = " + f10003.ToString();

                                string sql095 = "INSERT INTO F095 (F09501, F09502, F09503, F09504, F09505, F09506, F09507, F09508, F09509, F09510, F09511, USER_NO, TIME) "
                                    + " VALUES (95, '" + dtF100.Rows[0]["F10017"].ToString() + "', " + dtF100.Rows[0]["F10003"].ToString() + ", '" + dtF100.Rows[0]["F10011"].ToString() + "', " + data9 + ", " + data10
                                    + ", " + nrLuni.ToString() + ", " + nrZile.ToString() + ", " + dtF100.Rows[0]["F100929"].ToString() + ", 1, " + (Convert.ToInt32(dtCer.Rows[0]["DurataContract"].ToString()) == 1 ? "'Nedeterminat'" : "'Determinat'") + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                                General.IncarcaDT(sql095, null);

                                //Radu 01.11.2019 - se modifica data plecarii, deci trebuie refacut CalculCO   
                                if (dtSf.Year > DateTime.Now.Year)
                                {
                                    General.CalculCO(dtSf.Year, f10003);
                                    General.CalculCO(DateTime.Now.Year, f10003);
                                }
                                else
                                    General.CalculCO(dtSf.Year, f10003);
                            }
                            else
                            {
                                sql100 = "UPDATE F100 SET  F10023 = " + data10
                                + ", F100993 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 103)"
                                : "TO_DATE('" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 'dd/mm/yyyy')") + "  WHERE F10003 = " + f10003.ToString();

                                sql1001 = "UPDATE F1001 SET F1001138 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }    
                            //}   

                            //Radu 26.10.2020
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                                + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", " + dtCer.Rows[0]["IdAtribut"].ToString() + ", 'Prelungire CIM', " + data + ", " + dtCer.Rows[0]["DurataContract"].ToString() + ", 'Modificari in avans', '"
                                + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";


                        }
                        break;
                    case (int)Constante.Atribute.Organigrama:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10004 = " + dtCer.Rows[0]["SubcompanieId"].ToString() + ", F10005 = " + dtCer.Rows[0]["FilialaId"].ToString() + ", F10006 = "
                                    + dtCer.Rows[0]["SectieId"].ToString() + ", F10007 = " + dtCer.Rows[0]["DeptId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F100958 = " + dtCer.Rows[0]["SubdeptId"].ToString() + ", F100959 = " + dtCer.Rows[0]["BirouId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70414, F70415, F70416, F70417, F70418, F704180, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 5, 'Organigrama', " + data + ", " + dtCer.Rows[0]["DeptId"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + dtCer.Rows[0]["SubcompanieId"].ToString() + ", " + dtCer.Rows[0]["FilialaId"].ToString() + ", " + dtCer.Rows[0]["SectieId"].ToString()
                            + ", " + dtCer.Rows[0]["DeptId"].ToString() + ", " + dtCer.Rows[0]["SubdeptId"].ToString() + ", " + dtCer.Rows[0]["BirouId"].ToString() + ",  " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Componente:
                        if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                        {
                            act = 1;
                            sql100 = "UPDATE F100 SET ";
                            for (int i = 0; i <= 9; i++)
                            {
                                sql100 += " F10069" + i + " = " + dtCer.Rows[0]["Comp" + i].ToString().Replace(',', '.');
                                if (i < 9)
                                    sql100 += ", ";
                            }
                            sql100 += " WHERE F10003 = " + f10003.ToString();
                        }
                        string camp1 = "", camp2 = "";
                        for (int i = 0; i <= 9; i++)
                        {
                            camp1 += " F704" + (41 + i).ToString() + ", ";
                            camp2 += dtCer.Rows[0]["Comp" + i].ToString().Replace(',', '.') + ", ";
                        }
                        sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, " + camp1 + " F70420, USER_NO, TIME) "
                        + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 19, 'Componente', " + data + ", 0, 'Modificari in avans', '"
                        + dateDoc + "', " + camp2 + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        break;
                    case (int)Constante.Atribute.Tarife:
                        if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                        {
                            act = 1;
                            sql100 = "UPDATE F100 SET F10067 = '" + dtCer.Rows[0]["Tarife"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                        }
                        sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70467, F70420, USER_NO, TIME) "
                        + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 28, 'Tarife', " + data + ", 0, 'Modificari in avans', '"
                        + dateDoc + "','" + dtCer.Rows[0]["Tarife"].ToString() + "' ," + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        break;
                    case (int)Constante.Atribute.Sporuri:
                        if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                        {
                            act = 1;
                            sql100 = "UPDATE F100 SET ";
                            for (int i = 50; i <= 69; i++)
                            {
                                sql100 += " F1006" + i + " = " + dtCer.Rows[0]["Spor" + (i - 50).ToString()].ToString();
                                if (i < 69)
                                    sql100 += ", ";
                            }

                            sql100 += ", F10067 = '" + dtCer.Rows[0]["Tarife"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                        }
                        camp1 = ""; camp2 = "";
                        for (int i = 50; i <= 69; i++)
                        {
                            camp1 += " F7046" + i + ", ";
                            camp2 += dtCer.Rows[0]["Spor" + (i - 50).ToString()].ToString() + ", ";
                        }
                        sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, " + camp1 + " F70467, F70420, USER_NO, TIME) "
                        + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 11, 'Sporuri', " + data + ", 0, 'Modificari in avans', '"
                        + dateDoc + "', " + camp2 + "'" + dtCer.Rows[0]["Tarife"].ToString() + "'" + ", " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        break;
                    case (int)Constante.Atribute.SporTranzactii:
                        if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                        {
                            act = 1;
                            sql1001 = "UPDATE F1001 SET ";
                            for (int i = 80; i <= 99; i++)
                            {
                                sql1001 += " F10095" + i + " = " + dtCer.Rows[0]["SporTran" + (i - 80).ToString()].ToString();
                                if (i < 99)
                                    sql1001 += ", ";
                            }
                            sql1001 += " WHERE F10003 = " + f10003.ToString();
                        }
                        camp1 = ""; camp2 = "";
                        for (int i = 80; i <= 99; i++)
                        {
                            camp1 += " F70495" + i + ", ";
                            camp2 += dtCer.Rows[0]["SporTran" + (i - 80).ToString()].ToString() + ", ";
                        }
                        sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, " + camp1 + " F70420, USER_NO, TIME) "
                        + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 15, 'Spor tranzactii', " + data + ", 0, 'Modificari in avans', '"
                        + dateDoc + "', " + camp2 + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        break;
                    case (int)Constante.Atribute.NumePrenume:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10008 = '" + dtCer.Rows[0]["Nume"].ToString() + "', F10009 = '" + dtCer.Rows[0]["Prenume"].ToString()
                                    + "', F100905 = '" + dtCer.Rows[0]["Nume"].ToString() + " " + dtCer.Rows[0]["Prenume"].ToString() + "'  WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, \"Nume\", \"Prenume\", F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 101, 'Nume si prenume', " + data + ", -99, 'Modificari in avans', '"
                            + dateDoc + "', '" + dtCer.Rows[0]["Nume"].ToString() + "', '" + dtCer.Rows[0]["Prenume"].ToString() + "', "
                            + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.CASS:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F1003900 = " + dtCer.Rows[0]["CASS"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 102, 'CASS angajat', " + data + ", " + dtCer.Rows[0]["CASS"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Studii:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10050 = " + dtCer.Rows[0]["Studii"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 103, 'Studii', " + data + ", " + dtCer.Rows[0]["Studii"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.BancaSalariu:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10018 = " + dtCer.Rows[0]["BancaSal"].ToString() + ", F10019 = " + dtCer.Rows[0]["SucursalaSal"].ToString() + ", F10020 = '"
                                    + dtCer.Rows[0]["IBANSal"].ToString() + "', F10055 = '" + dtCer.Rows[0]["NrCard"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F1001040 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70431, F70432, F70433, F70434, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 104, 'Banca - cont salariu', " + data + ", " + dtCer.Rows[0]["BancaSal"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", '" + dtCer.Rows[0]["IBANSal"].ToString() + "', " + dtCer.Rows[0]["BancaSal"].ToString() + ", "
                            + dtCer.Rows[0]["SucursalaSal"].ToString() + ", '" + dtCer.Rows[0]["NrCard"].ToString() + "', -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.BancaGarantii:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF1001 != null && dtF1001.Rows.Count > 0)
                            {
                                act = 1;
                                sql1001 = "UPDATE F1001 SET F1001026 = " + dtCer.Rows[0]["BancaGar"].ToString() + ", F1001027 = " + dtCer.Rows[0]["SucursalaGar"].ToString() + ", F1001028 = '"
                                    + dtCer.Rows[0]["IBANGar"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F1001041 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70435, F70436, F70437, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 105, 'Banca - cont gar.', " + data + ", " + dtCer.Rows[0]["BancaGar"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", '" + dtCer.Rows[0]["IBANGar"].ToString() + "', " + dtCer.Rows[0]["BancaGar"].ToString() + ", "
                            + dtCer.Rows[0]["SucursalaGar"].ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.LimbiStraine:
                        {
                            DataTable dtLimbi = General.IncarcaDT("SELECT * FROM \"Admin_Limbi\" WHERE \"Marca\" = " + f10003.ToString() + " AND \"IdLimba\" = " + dtCer.Rows[0]["IdLimba"].ToString(), null);
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month)
                            {
                                string sqlLimbi = "";
                                if (dtLimbi != null && dtLimbi.Rows.Count > 0)
                                    sqlLimbi = "UPDATE \"Admin_Limbi\" SET \"Nivel\" = '" + dtCer.Rows[0]["NivelVorbit"].ToString() + "', \"NrAniVorbit\" = " + dtCer.Rows[0]["NrAniVorbit"].ToString()
                                        + " WHERE \"Marca\" = " + f10003.ToString() + " AND \"IdLimba\" = " + dtCer.Rows[0]["IdLimba"].ToString();
                                else
                                    sqlLimbi = "INSERT INTO \"Admin_Limbi\" (\"Marca\", \"IdLimba\", \"Nivel\", \"NrAniVorbit\", USER_NO, TIME) "
                                        + "VALUES (" + f10003.ToString() + ", " + dtCer.Rows[0]["IdLimba"].ToString() + ", '" + dtCer.Rows[0]["NivelVorbit"].ToString() + "', "
                                        + dtCer.Rows[0]["NrAniVorbit"].ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";

                                General.IncarcaDT(sqlLimbi, null);
                                act = 1;
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, \"NivelVorbit\", \"NrAniVorbit\" USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 106, 'Limbi straine', " + data + ", " + dtCer.Rows[0]["IdLimba"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", '" + dtCer.Rows[0]["NivelVorbit"].ToString() + "', " + dtCer.Rows[0]["NrAniVorbit"].ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.DocId:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F100983 = " + dtCer.Rows[0]["TipID"].ToString() + ", F10052 = '" + dtCer.Rows[0]["SerieNrID"].ToString() + "', F100521 = '" + dtCer.Rows[0]["IDEmisDe"].ToString()
                                    + "', F100522 = " + data5 + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F100963 = " + data6 + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, \"SerieNrID\", \"IDEmisDe\", \"DataElibID\", \"DataExpID\", USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 107, 'Document identitate', " + data + ", " + dtCer.Rows[0]["TipID"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", '" + dtCer.Rows[0]["SerieNrID"].ToString() + "', '" + dtCer.Rows[0]["IDEmisDe"].ToString() + "', " + data5
                            + ", " + data6 + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.PermisAuto:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10028 = " + dtCer.Rows[0]["IdCateg"].ToString() + ", F10024 = " + data7 + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F1001001 = '" + dtCer.Rows[0]["NrPermis"].ToString() + "', F1001002 = '" + dtCer.Rows[0]["PermisEmisDe"].ToString() + "', F1001000 = " + data8 + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, \"NrPermis\", \"PermisEmisDe\", \"DataEmiterePermis\", \"DataExpirarePermis\", USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 108, 'Permis auto', " + data + ", " + dtCer.Rows[0]["IdCateg"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", '" + dtCer.Rows[0]["NrPermis"].ToString() + "', '" + dtCer.Rows[0]["PermisEmisDe"].ToString() + "', " + data7
                            + ", " + data8 + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.BonusTeamLeader:
                        {
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 109, 'Bonus Team Leader', " + data + ", " + dtCer.Rows[0]["BonusTeamLeader"].ToString() + ", 'Modificari in avans', '"
                            + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Suspendare:
                        DateTime dtLuc = General.DamiDataLucru();
                        DataTable dtSuspNomen = General.IncarcaDT("SELECT * FROM F090 WHERE F09002 = " + dtCer.Rows[0]["MotivSuspId"].ToString(), null);
                        sql100 = "UPDATE F100 SET F100925 = " + dtCer.Rows[0]["MotivSuspId"].ToString() + ", F100922 = " + data11 + ", F100923 = " + data12 + ", F100924 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") +
                             (Convert.ToInt32(dtCer.Rows[0]["MotivSuspId"].ToString()) == 11 ? ", F10076 = " + data11 + ", F10077 = " + data12 + " - 1" : "") + " WHERE F10003 = " + f10003.ToString();
                        if (dtSuspNomen.Rows[0]["F09004"].ToString() != "Art52Alin1LiteraC" && dtSuspNomen.Rows[0]["F09004"].ToString() != "Art52Alin3")
                        {
                            DateTime dtIntrare, dtIesire;
                            General.CalculDateCategorieAsigurat(f10003, Convert.ToDateTime(dtCer.Rows[0]["DataInceputSusp"].ToString()), Convert.ToDateTime(dtCer.Rows[0]["DataSfEstimSusp"].ToString()), Convert.ToDateTime(dtCer.Rows[0]["DataIncetareSusp"].ToString()), out dtIntrare, out dtIesire);
                            string dataIn = (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtIntrare.Day.ToString().PadLeft(2, '0') + "/" + dtIntrare.Month.ToString().PadLeft(2, '0') + "/" + dtIntrare.Year.ToString() + "', 103)" : "TO_DATE('" + dtIntrare.Day.ToString().PadLeft(2, '0') + "/" + dtIntrare.Month.ToString().PadLeft(2, '0') + "/" + dtIntrare.Year.ToString() + "', 'dd/mm/yyyy')");
                            string dataOut = (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtIesire.Day.ToString().PadLeft(2, '0') + "/" + dtIesire.Month.ToString().PadLeft(2, '0') + "/" + dtIesire.Year.ToString() + "', 103)" : "TO_DATE('" + dtIesire.Day.ToString().PadLeft(2, '0') + "/" + dtIesire.Month.ToString().PadLeft(2, '0') + "/" + dtIesire.Year.ToString() + "', 'dd/mm/yyyy')");
                            sql1001 = "UPDATE F1001 SET F1001101 = " + dataIn + ", F1001102 = " + dataOut + " WHERE F10003 = " + f10003.ToString();
                        }
                        string sql111 = $@"INSERT INTO F111 (F11101, F11102, F11103, F11104, F11105, F11106, F11107, YEAR, MONTH, USER_NO, TIME)
                               VALUES (111, '{General.Nz(dtF100.Rows[0]["F10017"], "")}', {f10003}, {dtCer.Rows[0]["MotivSuspId"].ToString()},{data11}, {data12}, {data13},
                               {dtLuc.Year}, {dtLuc.Month}, {Session["UserId"]}, {General.CurrentDate()})";
                        General.IncarcaDT(sql111, null);
                        ActualizareSusp(f10003, ref sql100, ref sql1001);
                        break;
                    case (int)Constante.Atribute.RevenireSuspendare:
                        dtSuspNomen = General.IncarcaDT("SELECT * FROM F090 WHERE F09002 = " + dtCer.Rows[0]["MotivSuspId"].ToString(), null);
                        sql100 = "UPDATE F100 SET F100922 = " + data11 + ", F100923 = " + data12 + ", F100924 = " + data13 + (Convert.ToInt32(dtCer.Rows[0]["MotivSuspId"].ToString()) == 11 ? ",F10076 = " + data11 + ", F10077 = " + data13 + " - 1" : "") + ", F100925 = " + dtCer.Rows[0]["MotivSuspId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                        if (dtSuspNomen.Rows[0]["F09004"].ToString() != "Art52Alin1LiteraC" && dtSuspNomen.Rows[0]["F09004"].ToString() != "Art52Alin3")
                            sql1001 = "UPDATE F1001 SET F1001101 = " + data13 + ", F1001102 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + " WHERE F10003 = " + f10003.ToString();
                        sql111 = "UPDATE F111 SET F11107 = " + data13 + " WHERE F11103 = " + f10003 + " AND F11104 = " + dtCer.Rows[0]["MotivSuspId"].ToString() + " AND F11105 = " + data11;
                        General.IncarcaDT(sql111, null);
                        ActualizareSusp(f10003, ref sql100, ref sql1001, Convert.ToInt32(dtCer.Rows[0]["MotivSuspId"].ToString()), Convert.ToDateTime(dtCer.Rows[0]["DataInceputSusp"].ToString()), Convert.ToDateTime(dtCer.Rows[0]["DataSfEstimSusp"].ToString()), Convert.ToDateTime(dtCer.Rows[0]["DataIncetareSusp"].ToString()), true);
                        break;
                    case (int)Constante.Atribute.Detasare:
                        dtLuc = General.DamiDataLucru();
                        if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                        {
                            sql100 = "UPDATE F100 SET F100915 = " + data14 + ", F100916 = " + data15 + ", F100917 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") 
                                + ", F100920 = " + dtCer.Rows[0]["IdNationalitAng"].ToString() + ", F100918 = '" + dtCer.Rows[0]["NumeAngajator"].ToString() + "', " 
                                + " F100919 = '" + dtCer.Rows[0]["CUIAngajator"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                            if (chk1 != null)
                            {
                                sql1001 = "UPDATE F1001 SET F1001125 = " + (chk1.Checked ? "1" : "0") + ", F1001126 = " + (chk2.Checked ? "1" : "0") + ", F1001127 = " + (chk4.Checked ? "1" : "0")
                                    + ", F1001128 = " + (chk3.Checked ? "1" : "0") + ", F1001129 = " + (chk5.Checked ? "1" : "0") + " WHERE F10003 = " + f10003.ToString();
                            }
                        }
                        string sql112 = $@"INSERT INTO F112 (F11201, F11202, F11203, F11204, F11205, F11206, F11207, F11208, F11209, F11210, F11211, F11212, F11213, F11214, YEAR, MONTH, USER_NO, TIME)
                               VALUES (112, '{General.Nz(dtF100.Rows[0]["F10017"], "")}', {f10003}, '{dtCer.Rows[0]["NumeAngajator"].ToString()}','{dtCer.Rows[0]["CUIAngajator"].ToString()}',{dtCer.Rows[0]["IdNationalitAng"].ToString()},
                                {data14}, {data15}, {data16}, {(dtCer.Rows[0]["DetBifa1"] == DBNull.Value? "0" : dtCer.Rows[0]["DetBifa1"].ToString())}, {(dtCer.Rows[0]["DetBifa2"] == DBNull.Value ? "0" : dtCer.Rows[0]["DetBifa2"].ToString())}, 
                                {(dtCer.Rows[0]["DetBifa4"] == DBNull.Value ? "0" : dtCer.Rows[0]["DetBifa4"].ToString())}, {(dtCer.Rows[0]["DetBifa3"] == DBNull.Value ? "0" : dtCer.Rows[0]["DetBifa3"].ToString())},
                                {(dtCer.Rows[0]["DetBifa5"] == DBNull.Value ? "0" : dtCer.Rows[0]["DetBifa5"].ToString())}, {dtLuc.Year}, {dtLuc.Month}, {Session["UserId"]}, {General.CurrentDate()})";
                        General.IncarcaDT(sql112, null);

                        ActualizareDet(f10003, ref sql100, ref sql1001);
                        break;
                    case (int)Constante.Atribute.RevenireDetasare:
                        if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                        {
                            sql100 = "UPDATE F100 SET F100915 = " + data14 + ", F100916 = " + data15 + ", F100917 = " + data16 + " WHERE F10003 = " + f10003.ToString();
                            sql1001 = "UPDATE F1001 SET F1001125 = 0, F1001126 = 0, F1001127 = 0, F1001128 = 0, F1001129 = 0 WHERE F10003 = " + f10003.ToString();
                        }
                        sql112 = "UPDATE F112 SET F11209 = " + data16 + " WHERE F11203 = " + f10003 + " AND F11207 = " + data14;
                        General.IncarcaDT(sql112, null);
                        ActualizareDet(f10003, ref sql100, ref sql1001);
                        break;
                    case (int)Constante.Atribute.ProgramLucru:
                        DateTime tmpDtModif2 = Convert.ToDateTime(dtCer.Rows[0]["DataModif"]);
                        string sqlCtr = "UPDATE \"F100Contracte\" SET \"DataSfarsit\" = " + General.ToDataUniv(tmpDtModif2.AddDays(-1)) + " WHERE F10003 = " + f10003.ToString() + " AND \"DataSfarsit\" = " + General.ToDataUniv(new DateTime(2100, 1, 1));
                        General.ExecutaNonQuery(sqlCtr);

                        sqlCtr = "INSERT INTO \"F100Contracte\"(F10003, \"IdContract\", \"DataInceput\", \"DataSfarsit\", USER_NO, TIME) VALUES (" + f10003.ToString() + ", " + dtCer.Rows[0]["ProgramLucru"].ToString()
                            + ", " + data + ", " + General.ToDataUniv(new DateTime(2100, 1, 1)) + ", " + Session["UserId"].ToString() + ", " + General.CurrentDate() + ")";
                        General.ExecutaNonQuery(sqlCtr);
                        break;
                    case (int)Constante.Atribute.TipContract:
                    case (int)Constante.Atribute.DurataContract:
                        {
                            int nrLuni = 0, nrZile = 0;
                            string sql100Tmp = "";
                            DateTime dtTmp = new DateTime();
                            DateTime dtf = new DateTime(2100, 1, 1, 0, 0, 0);
                            if (dtSf != dtf)
                                dtTmp = dtSf.AddDays(1);
                            else
                                dtTmp = dtSf;

                            bool modifTip = false;
                            bool modifDur = false;

                            Personal.Contract ctr = new Personal.Contract();
                            ctr.CalculLuniSiZile(Convert.ToDateTime(dtInc.Date), Convert.ToDateTime(dtSf.Date), out nrLuni, out nrZile);

                            if (dtF100.Rows[0]["F100984"].ToString() != dtCer.Rows[0]["TipContract"].ToString())
                                modifTip = true;

                            if (dtF100.Rows[0]["F1009741"].ToString() != dtCer.Rows[0]["DurataContract"].ToString() || dtF100.Rows[0]["F100933"].ToString() != dtCer.Rows[0]["DataInceputCIM"].ToString()
                                || dtF100.Rows[0]["F100934"].ToString() != dtCer.Rows[0]["DataSfarsitCIM"].ToString())
                                modifDur = true;

                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                sql100Tmp = "UPDATE F100 SET " + (modifTip ? "F100984 = " + dtCer.Rows[0]["TipContract"].ToString() : "") + (modifDur ? (modifTip ? "," : "") +
                                    (dtCer.Rows[0]["DurataContract"].ToString() == "1" ? " F10023 = " + General.ToDataUniv(new DateTime(2100, 1, 1))
                                    + ", F100993 = " + General.ToDataUniv(new DateTime(2100, 1, 1)) + ", F1009741 = 1, F100935 = 0, F100936 = 0 "
                                    : "F100933 = " + data9 + ", F100934 = " + data10 + ", F100936 = " + nrZile.ToString() + ", F100935 = " + nrLuni.ToString() + ", F100938 = 1, F10023 = " + data10
                                    + ", F100993 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 103)"
                                    : "TO_DATE('" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 'dd/mm/yyyy')") + ", F1009741 = " + dtCer.Rows[0]["DurataContract"].ToString() )
                                    : "") +  " WHERE F10003 = " + f10003.ToString();
                                sql1001 = "UPDATE F1001 SET " + (modifTip ? "F1001137 = " + data : "") + (modifDur ? (modifTip ? "," : "") + "F1001138 = " + data : "") + " WHERE F10003 = " + f10003.ToString();


                                General.IncarcaDT(sql100Tmp, null); //pentru ca se modifica F10023, trebuie rulat CalculCO, de aceea se executa aici si nu la sfarsit
                            }

                            if (modifTip)
                                sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                                    + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 35, 'Tip contract', " + data + ", " + dtCer.Rows[0]["TipContract"].ToString() + ", 'Modificari in avans', '"
                                    + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";

                            if (modifDur)
                                sql1 = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70468, F70469, F70409, F70410, F70420, USER_NO, TIME) "
                                    + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 36, 'Durata contract', " + data + ", " + dtCer.Rows[0]["DurataContract"].ToString() + "," +
                                    (dtCer.Rows[0]["DurataContract"].ToString() == "1" ? General.ToDataUniv(new DateTime(2100, 1, 1)) + ", " + General.ToDataUniv(new DateTime(2100, 1, 1)) + ", " : data9 + ", " + data10 + ", ") 
                                    + " 'Modificari in avans', '" + dateDoc + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";

                            //Radu 01.11.2019 - se modifica data plecarii, deci trebuie refacut CalculCO   
                            if (dtSf.Year > DateTime.Now.Year)
                            {
                                General.CalculCO(dtSf.Year, f10003);
                                General.CalculCO(DateTime.Now.Year, f10003);
                            }
                            else
                                General.CalculCO(dtSf.Year, f10003);
                        }
                        break;
                    default:
                        return;
                }

                if (sql.Length > 0) General.IncarcaDT(sql, null);
                if (sql1.Length > 0) General.IncarcaDT(sql1, null);
                if (sql100.Length > 0) General.IncarcaDT(sql100, null);
                if (sql1001.Length > 0) General.IncarcaDT(sql1001, null);


                //Radu 09.09.2020 - actualizare data consemnare
                //if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month)
                //{
                    DateTime dtConsemn = General.FindDataConsemnare(f10003);
                    General.ExecutaNonQuery("UPDATE F1001 SET F1001109 = " + General.ToDataUniv(dtConsemn) + " WHERE F10003 = " + f10003, null);
                //}


                //Florin 2019-04-10
                //procesul acesta s-a mutat din ActeAditionale aici
                //marcam campul Actualizat din Avs_Cereri cand se duce in F100
                if (act == 1)
                General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""Actualizat""=1 WHERE ""Id""=@1", new object[] { id });

            }
            catch (Exception)
            {
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Radu 30.01.2020
        private void ActualizareSusp(int f10003, ref string sql100, ref string sql1001, int idMotiv = -1, DateTime? dataStart = null, DateTime? dataSfarsit = null, DateTime? dataIncetare = null, bool rev = false)
        {
            DataTable dtSuspAng = General.IncarcaDT("select * from f111 Where F11103 = " + f10003.ToString() + " AND (F11107 IS NULL OR F11107 = "
                + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ")  ORDER BY F11105", null);

            if (idMotiv > 0)
                dtSuspAng = General.IncarcaDT("select * from f111 Where F11103 = " + f10003.ToString() + " AND F11104 = " + idMotiv + " AND  F11105 = "
                + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dataStart.Value.Day.ToString().PadLeft(2, '0') + "/" + dataStart.Value.Month.ToString().PadLeft(2, '0') + "/" + dataStart.Value.Year.ToString() + "', 103)" 
                : "TO_DATE('" + dataStart.Value.Day.ToString().PadLeft(2, '0') + "/" + dataStart.Value.Month.ToString().PadLeft(2, '0') + "/" + dataStart.Value.Year.ToString() + "', 'dd/mm/yyyy')") + " AND " 
                + " F11106 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dataSfarsit.Value.Day.ToString().PadLeft(2, '0') + "/" + dataSfarsit.Value.Month.ToString().PadLeft(2, '0') + "/" + dataSfarsit.Value.Year.ToString() + "', 103)"
                : "TO_DATE('" + dataSfarsit.Value.Day.ToString().PadLeft(2, '0') + "/" + dataSfarsit.Value.Month.ToString().PadLeft(2, '0') + "/" + dataSfarsit.Value.Year.ToString() + "', 'dd/mm/yyyy')") + " AND "
                + " F11107 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dataIncetare.Value.Day.ToString().PadLeft(2, '0') + "/" + dataIncetare.Value.Month.ToString().PadLeft(2, '0') + "/" + dataIncetare.Value.Year.ToString() + "', 103)"
                : "TO_DATE('" + dataIncetare.Value.Day.ToString().PadLeft(2, '0') + "/" + dataIncetare.Value.Month.ToString().PadLeft(2, '0') + "/" + dataIncetare.Value.Year.ToString() + "', 'dd/mm/yyyy')") + "  ORDER BY F11105", null);

            if (dtSuspAng != null && dtSuspAng.Rows.Count > 0)
            {
                string data1 = "", data2 = "";
                if (Constante.tipBD == 1)
                {
                    data1 = "CONVERT(DATETIME, '" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Year.ToString() + "', 103)";
                    data2 = "CONVERT(DATETIME, '" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()).Year.ToString() + "', 103)";
                }
                else
                {
                    data1 = "TO_DATE('" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()).Year.ToString() + "', 'dd/mm/yyyy')";
                    data2 = "TO_DATE('" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()).Year.ToString() + "', 'dd/mm/yyyy')";
               }
                DataTable dtSuspNomen = General.IncarcaDT("SELECT * FROM F090 WHERE F09002 = " + dtSuspAng.Rows[0]["F11104"].ToString(), null);

                if (!rev)
                    sql100 = "UPDATE F100 SET F100925 = " + dtSuspAng.Rows[0]["F11104"].ToString() + ", F100922 = " + data1 + ", F100923 = " + data2 + ", F100924 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") +
                         (Convert.ToInt32(dtSuspAng.Rows[0]["F11104"].ToString()) == 11 ? ", F10076 = " + data1 + ", F10077 = " + data2 + " - 1" : "") + " WHERE F10003 = " + f10003.ToString();
                if (dtSuspNomen.Rows[0]["F09004"].ToString() != "Art52Alin1LiteraC" && dtSuspNomen.Rows[0]["F09004"].ToString() != "Art52Alin3")
                {
                    DateTime dtIntrare, dtIesire;
                    General.CalculDateCategorieAsigurat(f10003, Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()), Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()), Convert.ToDateTime(dtSuspAng.Rows[0]["F11107"].ToString()), out dtIntrare, out dtIesire);
                    string dataIn = (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtIntrare.Day.ToString().PadLeft(2, '0') + "/" + dtIntrare.Month.ToString().PadLeft(2, '0') + "/" + dtIntrare.Year.ToString() + "', 103)" : "TO_DATE('" + dtIntrare.Day.ToString().PadLeft(2, '0') + "/" + dtIntrare.Month.ToString().PadLeft(2, '0') + "/" + dtIntrare.Year.ToString() + "', 'dd/mm/yyyy')");
                    string dataOut = (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtIesire.Day.ToString().PadLeft(2, '0') + "/" + dtIesire.Month.ToString().PadLeft(2, '0') + "/" + dtIesire.Year.ToString() + "', 103)" : "TO_DATE('" + dtIesire.Day.ToString().PadLeft(2, '0') + "/" + dtIesire.Month.ToString().PadLeft(2, '0') + "/" + dtIesire.Year.ToString() + "', 'dd/mm/yyyy')");
                    sql1001 = "UPDATE F1001 SET F1001101 = " + dataIn + ", F1001102 = " + dataOut + " WHERE F10003 = " + f10003.ToString();
                }
          
                //transfer          
                if (dtSuspNomen != null && dtSuspNomen.Rows.Count > 0)
                {
                    int cod = (dtSuspNomen.Rows[0]["CodTranzactie"] as int?) ?? 0;
                    if (dtSuspNomen.Rows[0]["TransferTranzactii"] != null && dtSuspNomen.Rows[0]["TransferTranzactii"].ToString().Length > 0 && Convert.ToInt32(dtSuspNomen.Rows[0]["TransferTranzactii"].ToString()) == 1)
                        General.TransferTranzactii(f10003.ToString(), cod.ToString(),
                            Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()), Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()), Convert.ToDateTime(dtSuspAng.Rows[0]["F11107"].ToString()));
                    if (dtSuspNomen.Rows[0]["TransferPontaj"] != null && dtSuspNomen.Rows[0]["TransferPontaj"].ToString().Length > 0 && Convert.ToInt32(dtSuspNomen.Rows[0]["TransferPontaj"].ToString()) == 1)
                        General.TransferPontaj(f10003.ToString(), Convert.ToDateTime(dtSuspAng.Rows[0]["F11105"].ToString()),
                             Convert.ToDateTime(dtSuspAng.Rows[0]["F11106"].ToString()), Convert.ToDateTime(dtSuspAng.Rows[0]["F11107"].ToString()),
                            (dtSuspNomen.Rows[0]["DenumireScurta"] as string), new DateTime(2100, 1, 1));                    
                }
            }
        }
        private void ActualizareDet(int f10003, ref string sql100, ref string sql1001)
        {
            DataTable dtSuspAng = General.IncarcaDT("select * from f112 Where F11203 = " + f10003.ToString() + " AND (F11209 IS NULL OR F11209 = "
                + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + ")  ORDER BY F11207", null);
            if (dtSuspAng != null && dtSuspAng.Rows.Count > 0)
            {
                string data1 = "", data2 = "";
                if (Constante.tipBD == 1)
                {
                    data1 = "CONVERT(DATETIME, '" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11207"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11207"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11207"].ToString()).Year.ToString() + "', 103)";
                    data2 = "CONVERT(DATETIME, '" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11208"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11208"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11208"].ToString()).Year.ToString() + "', 103)";
                }
                else
                {
                    data1 = "TO_DATE('" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11207"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11207"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11207"].ToString()).Year.ToString() + "', 'dd/mm/yyyy')";
                    data2 = "TO_DATE('" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11208"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11208"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dtSuspAng.Rows[0]["F11208"].ToString()).Year.ToString() + "', 'dd/mm/yyyy')";
                }
                sql100 = "UPDATE F100 SET F100915 = " + data1 + ", F100916 = " + data2 + ", F100917 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103)" : "TO_DATE('01/01/2100', 'dd/mm/yyyy')") + " WHERE F10003 = " + f10003.ToString();
                //sql1001 = "UPDATE F1001 SET F1001125 = " + (chk1.Checked ? "1" : "0") + ", F1001126 = " + (chk2.Checked ? "1" : "0") + ", F1001127 = " + (chk3.Checked ? "1" : "0")
                //    + ", F1001128 = " + (chk4.Checked ? "1" : "0") + ", F1001129 = " + (chk5.Checked ? "1" : "0") + " WHERE F10003 = " + f10003.ToString();
            }
        }
        private void IncarcaComboBox(ASPxComboBox cmbAct, ASPxComboBox cmbNou, DataTable dt1, DataTable dt2)
        {
            cmbAct.DataSource = dt1;
            cmbAct.DataBind();            
            cmbNou.DataSource = dt2;
            cmbNou.DataBind();
            cmbAct.SelectedIndex = 0;

            if (Session["Valoare1Noua"] != null)
            {
                string[] param = Session["Valoare1Noua"].ToString().Split(';');
                cmb1Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare2Noua"] != null)
            {
                string[] param = Session["Valoare2Noua"].ToString().Split(';');
                cmb2Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare3Noua"] != null)
            {
                string[] param = Session["Valoare3Noua"].ToString().Split(';');
                cmb3Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare4Noua"] != null)
            {
                string[] param = Session["Valoare4Noua"].ToString().Split(';');
                cmb4Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare5Noua"] != null)
            {
                string[] param = Session["Valoare5Noua"].ToString().Split(';');
                cmb5Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare6Noua"] != null)
            {
                string[] param = Session["Valoare6Noua"].ToString().Split(';');
                cmb6Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare7Noua"] != null)
            {
                string[] param = Session["Valoare7Noua"].ToString().Split(';');
                cmb7Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }
            else
            {
                if (dt2.TableName == "F096")
                    cmb7Nou.SelectedIndex = 0;
            }

            if (Session["Valoare8Noua"] != null)
            {
                string[] param = Session["Valoare8Noua"].ToString().Split(';');
                cmbStructOrgNou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (Session["Valoare9Noua"] != null)
            {
                string[] param = Session["Valoare9Noua"].ToString().Split(';');
                cmb8Nou.Value = Convert.ToInt32(param[1]);
                //return;
            }

            if (dt1 != null && dt1.Columns.Count > 2)
            {
                cmbAct.Columns.Clear();
                for (int i = 0; i < dt2.Columns.Count; i++)
                {
                    ListBoxColumn col = new ListBoxColumn();
                    col.FieldName = dt1.Columns[i].ColumnName;
                    cmbAct.Columns.Add(col);
                }
            }

            if (dt2 != null && dt2.Columns.Count > 2)
            {
                cmbNou.Columns.Clear();
                for (int i = 0; i < dt2.Columns.Count; i++)
                {
                    ListBoxColumn col = new ListBoxColumn();
                    col.FieldName = dt2.Columns[i].ColumnName;
                    cmbNou.Columns.Add(col);
                }
            }

            //for (int i = 0; i < cmbNou.Items.Count; i++)
            //    if (cmbNou.Items[i].Text.ToUpper().Equals(cmbAct.Text.ToUpper()))
            //    {
            //        cmbNou.SelectedIndex = i;
            //        break;
            //    }

        }

        protected void grDate_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "DataRevisal")
            {
                string data = "";
                if (e.GetListSourceFieldValue("DataModif") != null && e.GetListSourceFieldValue("DataModif").ToString().Length > 0)
                {

                    DateTime dt = Convert.ToDateTime(e.GetListSourceFieldValue("DataModif").ToString());
                    DateTime dataMod = new DateTime(dt.Year, dt.Month, dt.Day);

                    SetDataRevisal(2, dataMod, Convert.ToInt32(e.GetListSourceFieldValue("IdAtribut").ToString()), out data);

                    e.Value = data;
                }
            }
        }

        //private void OnClose()
        //{
        //    Session["Marca_atribut"] = null;
        //}

        //protected void btnExit_Click(object sender, EventArgs e)
        //{
        //    Session["Marca_atribut"] = null;
        //}

        //private void ArataMesaj(string mesaj)
        //{
        //    pnlCtl.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtl.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));
        //}

        private string SelectAtribute()
        {
            string strSql = "";

            try
            {
                //strSql = $@"SELECT A.Id, A.Denumire 
                //        FROM Avs_tblAtribute A
                //        INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
                //        INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper OR (-1 * B.Super2) = C.IdSuper OR (-1 * B.Super3) = C.IdSuper OR (-1 * B.Super4) = C.IdSuper OR (-1 * B.Super5) = C.IdSuper OR (-1 * B.Super6) = C.IdSuper OR (-1 * B.Super7) = C.IdSuper OR (-1 * B.Super8) = C.IdSuper OR (-1 * B.Super9) = C.IdSuper OR (-1 * B.Super10) = C.IdSuper
                //        WHERE C.IdUser=@1 AND C.IdSuper=@2
                //        GROUP BY A.Id, A.Denumire
                //        ORDER BY A.Denumire";

                strSql = $@"SELECT A.""Id"", A.""Denumire"" FROM ""Avs_tblAtribute"" A
                        INNER JOIN ""Avs_Circuit"" B ON A.""Id""=B.""IdAtribut"" AND B.""Super1""=0
                        WHERE {Session["User_Marca"]} = @3 AND {cmbRol.Value} = 0
                        UNION
                        SELECT A.""Id"", A.""Denumire"" 
                        FROM ""Avs_tblAtribute"" A
                        INNER JOIN ""Avs_Circuit"" B ON A.""Id""=B.""IdAtribut""
                        INNER JOIN ""F100Supervizori"" C ON (-1 * B.""Super1"") = C.""IdSuper""
                        WHERE C.""IdUser""=@1 AND C.""IdSuper""=@2 AND C.F10003=@3
                        GROUP BY A.""Id"", A.""Denumire""
                        UNION
                        SELECT A.""Id"", A.""Denumire"" 
                        FROM ""Avs_tblAtribute"" A
                        INNER JOIN ""Avs_Circuit"" B ON A.""Id""=B.""IdAtribut"" AND B.""Super1""=@1
                        ORDER BY ""Denumire"" ";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;

        }

        //public static int DamiRol(int f10003, int atribut)
        //{
        //    int idRol = -99;

        //    try
        //    {
        //        string strSql = $@"SELECT B.Super1 AS Id, COALESCE(S.Alias,S.Denumire) AS Denumire 
        //            FROM Avs_tblAtribute A
        //            INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut AND B.Super1=0
        //            LEFT JOIN tblSupervizori S ON S.Id = (-1 * B.Super1)
        //            WHERE @1 = @2 AND A.Id=@3
        //            UNION
        //            SELECT B.Super1 AS Id, COALESCE(S.Alias,S.Denumire) AS Denumire
        //            FROM Avs_tblAtribute A
        //            INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
        //            INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper
        //            LEFT JOIN tblSupervizori S ON S.Id = (-1 * B.Super1)
        //            WHERE C.IdUser=@1 AND A.Id=@3 AND C.F10003=@2
        //            GROUP BY B.Super1, S.Alias, S.Denumire
        //            UNION
        //            SELECT B.Super1 AS Id, COALESCE(S.Alias,S.Denumire) AS Denumire
        //            FROM Avs_tblAtribute A
        //            INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut AND B.Super1=@1
        //            LEFT JOIN tblSupervizori S ON S.Id = (-1 * B.Super1)";

        //        DataTable dt = General.IncarcaDT(strSql, new object[] { HttpContext.Current.Session["User_Marca"], f10003, atribut });
        //        if (dt.Rows.Count > 0)
        //            idRol = Convert.ToInt32(General.Nz(dt.Rows[0]["Id"],-99));
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Avs.Cereri", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return idRol;
        //}


        public static string DamiRol(int f10003, int atribut)
        {
            string str = "";

            try
            {
                string strSql = $@"SELECT B.Super1 AS Id, COALESCE(S.Alias,S.Denumire) AS Denumire 
                    FROM Avs_tblAtribute A
                    INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut AND B.Super1=0
                    LEFT JOIN tblSupervizori S ON S.Id = (-1 * B.Super1)
                    WHERE @4 = @2 AND A.Id=@3
                    UNION
                    SELECT B.Super1 AS Id, COALESCE(S.Alias,S.Denumire) AS Denumire
                    FROM Avs_tblAtribute A
                    INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut
                    INNER JOIN F100Supervizori C ON (-1 * B.Super1) = C.IdSuper
                    LEFT JOIN tblSupervizori S ON S.Id = (-1 * B.Super1)
                    WHERE C.IdUser=@1 AND A.Id=@3 AND C.F10003=@2
                    GROUP BY B.Super1, S.Alias, S.Denumire
                    UNION
                    SELECT B.Super1 AS Id, COALESCE(S.Alias,S.Denumire) AS Denumire
                    FROM Avs_tblAtribute A
                    INNER JOIN Avs_Circuit B ON A.Id=B.IdAtribut AND B.Super1=@1
                    LEFT JOIN tblSupervizori S ON S.Id = (-1 * B.Super1)";

                DataTable dt = General.IncarcaDT(strSql, new object[] { HttpContext.Current.Session["UserId"], f10003, atribut, HttpContext.Current.Session["User_Marca"] });

                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    str += dt.Rows[i]["Id"] + "=" + dt.Rows[i]["Denumire"] + "|";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Avs.Cereri", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }



        //////////////////////////////////////////////////////////// Componente

        protected void grDateComponente_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGridComp();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGridComp()
        {
            //Florin 2019.09.26
            //se afiseaza toate componentele, deoarece componenta F70450 trebuie afisata oricum

            DataSet ds = Session["AvsCereri"] as DataSet;
            DataTable dt = new DataTable();
            if (ds == null)
            {
                ds = new DataSet();

                dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);
            }

            //Florin 2019.09.26
            //string sql = " select F02104, f100690 as \"Suma\" from f021 join f100 on f02104 = 4001 and f100690 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100691 as \"Suma\" from f021 join f100 on f02104 = 4002 and f100691 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100692 as \"Suma\" from f021 join f100 on f02104 = 4003 and f100692 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100693 as \"Suma\" from f021 join f100 on f02104 = 4004 and f100693 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100694 as \"Suma\" from f021 join f100 on f02104 = 4005 and f100694 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100695 as \"Suma\" from f021 join f100 on f02104 = 4006 and f100695 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100696 as \"Suma\" from f021 join f100 on f02104 = 4007 and f100696 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100697 as \"Suma\" from f021 join f100 on f02104 = 4008 and f100697 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100698 as \"Suma\" from f021 join f100 on f02104 = 4009 and f100698 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString()
            //            + "union "
            //            + "select F02104, f100699 as \"Suma\" from f021 join f100 on f02104 = 4010 and f100699 > 0 and f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " ORDER BY F02104";

            string sql = $@"
                SELECT X.F02104, Y.F02105 AS ""Denumire"", X.""Suma"" FROM (
                SELECT 4001 AS F02104, F100690 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4002 AS F02104, F100691 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4003 AS F02104, F100692 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4004 AS F02104, F100693 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4005 AS F02104, F100694 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4006 AS F02104, F100695 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4007 AS F02104, F100696 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4008 AS F02104, F100697 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4009 AS F02104, F100698 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}
                UNION
                SELECT 4010 AS F02104, F100699 AS ""Suma"" FROM F100 WHERE F10003 = {cmbAng.Value}) X
                LEFT JOIN F021 Y ON X.F02104 = Y.F02104";

            DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Componente"))
            {
                dt = dsCalcul.Tables["Componente"];
            }
            else
            {
                dt = General.IncarcaDT(sql, null);
                dt.TableName = "Componente";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F02104"] };

                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateComponente.KeyFieldName = "F02104";
            grDateComponente.DataSource = dt;

            //sql = @"SELECT F02104 AS Id, CONVERT(VARCHAR, F02104) + ' - ' + F02105 as Denumire FROM F021 WHERE F02104 BETWEEN 4001 AND 4010";
            //if (Constante.tipBD == 2)
            //    sql = @"SELECT F02104 AS ""Id"", F02104 || ' - ' || F02105 as ""Denumire"" FROM F021 WHERE F02104 BETWEEN 4001 AND 4010";
            //DataTable dtGrup = General.IncarcaDT(sql, null);
            //GridViewDataComboBoxColumn colComp = (grDateComponente.Columns["F02104"] as GridViewDataComboBoxColumn);
            //colComp.PropertiesComboBox.DataSource = dtGrup;

            Session["AvsCereri"] = ds;
            Session["AvsCereriCalcul"] = dsCalcul;

        }

        //Florin 2019.09.26
        //protected void grDateComponente_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        //{
        //    try
        //    {

        //        DataSet ds = Session["AvsCereri"] as DataSet;   
        //        DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

        //        object[] rowComp = new object[dsCalcul.Tables["Componente"].Columns.Count];
        //        int x = 0;

        //        bool dublura = false;
        //        for (int i = 0; i < dsCalcul.Tables["Componente"].Rows.Count; i++)
        //        {
        //            if (dsCalcul.Tables["Componente"].Rows[i]["F02104"].ToString() == e.NewValues["F02104"].ToString())
        //            {
        //                dublura = true;
        //                break;
        //            }
        //        }

        //        if (dublura)
        //        {
        //            grDateComponente.JSProperties["cpAlertMessage"] = "Codul a mai fost deja atribuit acestui angajat!";
        //        }
        //        else
        //        {
        //            foreach (DataColumn col in dsCalcul.Tables["Componente"].Columns)
        //            {
        //                switch (col.ColumnName.ToUpper())
        //                {
        //                    case "SUMA":
        //                        rowComp[x] = e.NewValues[col.ColumnName];
        //                        ds.Tables[0].Rows[0]["F10069" + (Convert.ToInt32(e.NewValues["F02104"].ToString().Substring(2)) - 1).ToString()] = e.NewValues[col.ColumnName];
        //                        break;
        //                    default:
        //                        rowComp[x] = e.NewValues[col.ColumnName];
        //                        break;
        //                }
        //                x++;
        //            }
        //            dsCalcul.Tables["Componente"].Rows.Add(rowComp);
        //        }

        //        e.Cancel = true;
        //        grDateComponente.CancelEdit();
        //        grDateComponente.DataSource = dsCalcul.Tables["Componente"];
        //        grDateComponente.KeyFieldName = "F02104";

        //        Session["AvsCereri"] = ds;
        //        Session["AvsCereriCalcul"] = dsCalcul;
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
        //    }
        //}

        //protected void grDateComponente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        //{
        //    try
        //    {
        //        object[] keys = new object[e.Keys.Count];
        //        for (int i = 0; i < e.Keys.Count; i++)
        //        { keys[i] = e.Keys[i]; }

        //        bool dublura = false;

        //        DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

        //        DataSet ds = Session["AvsCereri"] as DataSet;  
        //        DataRow rowComp = dsCalcul.Tables["Componente"].Rows.Find(keys);

        //        for (int i = 0; i < dsCalcul.Tables["Componente"].Rows.Count; i++)
        //        {
        //            if (grDateComponente.EditingRowVisibleIndex != i && dsCalcul.Tables["Componente"].Rows[i]["F02104"].ToString() == General.Nz(e.NewValues["F02104"], "").ToString())
        //            {
        //                dublura = true;
        //                break;
        //            }
        //        }


        //        if (dublura)
        //        {
        //            grDateComponente.JSProperties["cpAlertMessage"] = "Codul a mai fost deja atribuit acestui angajat!";
        //        }
        //        else
        //        {
        //            foreach (DataColumn col in dsCalcul.Tables["Componente"].Columns)
        //            {
        //                if (col.ColumnName.ToUpper() == "SUMA")
        //                {
        //                    col.ReadOnly = false;
        //                    var edc = e.NewValues[col.ColumnName];
        //                    rowComp[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
        //                    ds.Tables[0].Rows[0]["F10069" + (Convert.ToInt32(e.NewValues["F02104"].ToString().Substring(2)) - 1).ToString()] = e.NewValues[col.ColumnName];
        //                }
        //            }
        //        }

        //        e.Cancel = true;
        //        grDateComponente.CancelEdit();
        //        Session["AvsCereri"] = ds;
        //        Session["AvsCereriCalcul"] = dsCalcul;
        //        grDateComponente.DataSource = dsCalcul.Tables["Componente"];
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
        //    }
        //}

        protected void grDateComponente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["AvsCereri"] as DataSet;
                DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
                
                DataRow rowComp = dsCalcul.Tables["Componente"].Rows.Find(keys);

                DataColumn col = dsCalcul.Tables["Componente"].Columns["Suma"];
                int nr = Convert.ToInt32(General.Nz(e.Keys[0],"1").ToString().Replace("40",""));
                col.ReadOnly = false;
                rowComp["Suma"] = e.NewValues["Suma"];
                ds.Tables[0].Rows[0]["F10069" + (nr-1)] = e.NewValues["Suma"];

                e.Cancel = true;
                grDateComponente.CancelEdit();
                Session["AvsCereri"] = ds;
                Session["AvsCereriCalcul"] = dsCalcul;
                grDateComponente.DataSource = dsCalcul.Tables["Componente"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void grDateComponente_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            var grid = sender as ASPxGridView;
            e.Editor.ReadOnly = false;   
            if (e.Column.FieldName == "Suma")
            {
                var tb = e.Editor as ASPxTextBox;
                tb.ClientSideEvents.TextChanged = "OnTextChangedComp";
            }
        }


        //////////////////////////////////////////////////////////// Tarife

        protected void grDateTarife_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGridTarife();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGridTarife()
        {

            DataTable dt = new DataTable();
            DataSet ds = Session["AvsCereri"] as DataSet;
            if (ds == null)
            {
                ds = new DataSet();

                dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };              
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };            
                ds.Tables.Add(dt);
            }
            DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Tarife"))
            {
                dt = dsCalcul.Tables["Tarife"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("DenCateg", typeof(string));
                dt.Columns.Add("DenTarif", typeof(string));
                dt.Columns.Add("F01104", typeof(int));
                dt.Columns.Add("F01105", typeof(int));



                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                string sqlFinal = "";
                string sql = "SELECT F01104, F01105, (SELECT TOP 1 b.F01107 FROM F011 b WHERE b.F01104 = a.F01104) AS \"DenCateg\", "
                        + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"DenTarif\" FROM F011 a ", cond = "";

                if (Constante.tipBD == 2)
                    sql = "SELECT F01104, F01105, (SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND ROWNUM = 1) AS \"DenCateg\", "
                        + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"DenTarif\" FROM F011 a ";

                for (int i = 0; i < sir.Length; i++)
                    if (sir[i] != '0')
                    {
                        cond = "WHERE (a.F01104 = " + (i + 1).ToString() + " AND a.F01105 = " + sir[i] + ") ";
                        sqlFinal += (sqlFinal.Length <= 0 ? "" : " UNION ") + sql + cond;
                    }

                if (sqlFinal.Length > 0)
                    dt = General.IncarcaDT(sqlFinal, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F01104"] };
                dt.TableName = "Tarife";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateTarife.KeyFieldName = "F01104";
            grDateTarife.DataSource = dt;

            Session["AvsCereri"] = ds;
            Session["AvsCereriCalcul"] = dsCalcul;

        }


        protected void grDateTarife_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["DenCateg"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster");
                e.NewValues["F01104"] = cb1.Value;
                e.NewValues["DenCateg"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["DenTarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["DenTarif"] = cb2.Text;

                if (e.NewValues["DenCateg"] == null || e.NewValues["DenCateg"].ToString().Length < 0)
                    return;

                if (e.NewValues["F01104"].ToString() == "0")
                {
                    e.NewValues["F01105"] = 0;
                    e.NewValues["DenTarif"] = "---";
                }

                DataSet ds = Session["AvsCereri"] as DataSet;
                DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

                object[] row = new object[dsCalcul.Tables["Tarife"].Columns.Count];
                int x = 0, poz = 0, val = 0;

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Tarife"].Rows.Count; i++)
                {
                    if (dsCalcul.Tables["Tarife"].Rows[i]["F01104"].ToString() == e.NewValues["F01104"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateTarife.JSProperties["cpAlertMessage"] = "Aceasta categorie a mai fost deja atribuita acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Tarife"].Columns)
                    {
                        row[x] = e.NewValues[col.ColumnName];
                        if (col.ColumnName == "F01104")
                            poz = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        if (col.ColumnName == "F01105")
                            val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        x++;
                    }
                    dsCalcul.Tables["Tarife"].Rows.Add(row);
                }

                e.Cancel = true;
                grDateTarife.CancelEdit();
                grDateTarife.DataSource = dsCalcul.Tables["Tarife"];
                grDateTarife.KeyFieldName = "F01104";

                if (!dublura)
                {
                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    string sirNou = "";
                    for (int i = 0; i < sir.Length; i++)
                        if (i == poz - 1)
                            sirNou += val.ToString();
                        else
                            sirNou += sir[i];

                    ds.Tables[0].Rows[0]["F10067"] = sirNou;                  
                }
                Session["AvsCereri"] = ds;
                Session["AvsCereriCalcul"] = dsCalcul;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateTarife_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["DenCateg"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster");
                e.NewValues["F01104"] = cb1.Value;
                e.NewValues["DenCateg"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["DenTarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["DenTarif"] = cb2.Text;

                if (e.NewValues["DenCateg"] == null || e.NewValues["DenCateg"].ToString().Length < 0)
                    return;

                if (e.NewValues["F01104"].ToString() == "0")
                {
                    e.NewValues["F01105"] = 0;
                    e.NewValues["DenTarif"] = "---";
                }

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["AvsCereri"] as DataSet;
                DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["Tarife"].Rows.Find(keys);
                int poz = 0, val = 0;

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Tarife"].Rows.Count; i++)
                {
                    if (grDateTarife.EditingRowVisibleIndex != i && dsCalcul.Tables["Tarife"].Rows[i]["F01104"].ToString() == e.NewValues["F01104"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateTarife.JSProperties["cpAlertMessage"] = "Aceasta categorie a mai fost deja atribuita acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Tarife"].Columns)
                    {
                        col.ReadOnly = false;
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                        if (col.ColumnName == "F01104")
                            poz = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        if (col.ColumnName == "F01105")
                            val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                    }
                }

                e.Cancel = true;
                grDateTarife.CancelEdit();

                if (!dublura)
                {
                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    string sirNou = "";
                    for (int i = 0; i < sir.Length; i++)
                        if (i == poz - 1)
                            sirNou += val.ToString();
                        else
                            sirNou += sir[i];

                    ds.Tables[0].Rows[0]["F10067"] = sirNou;
                }

                Session["AvsCereri"] = ds;
                Session["AvsCereriCalcul"] = dsCalcul;
                grDateTarife.DataSource = dsCalcul.Tables["Tarife"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        } 

       

        protected void cmbMaster_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            string[] param = templateContainer.ClientID.Split('_');
            if (param[1] != "editnew")
            {
                cmbParent.Value = Convert.ToInt32(templateContainer.KeyValue);
                Session["Tarife_cmbMaster"] = templateContainer.KeyValue;
            }

            ObjectDataSource cmbParentDataSource = cmbParent.NamingContainer.FindControl("adsMaster") as ObjectDataSource;
            cmbParentDataSource.SelectParameters.Clear();
            cmbParentDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbParent.DataBindItems();

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged(s, e, {0}); }}", templateContainer.VisibleIndex);
        }

        protected void cmbChild_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            string[] param = templateContainer.ClientID.Split('_');

            cmbChild.ClientInstanceName = String.Format("cmbChild_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild_new", templateContainer.VisibleIndex);
            else
            {
                ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

                cmbChildDataSource.SelectParameters.Clear();
                cmbChildDataSource.SelectParameters.Add("categ", Session["Tarife_cmbMaster"].ToString());
                cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
                cmbChild.DataBindItems();
                //cmbChild.Value = Convert.ToInt32(param[2]);
            }


            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild_Callback);

        }

        protected void cmbChild_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

            cmbChildDataSource.SelectParameters.Clear();
            cmbChildDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbChild.DataBindItems();
        }



        protected void grDateSporuri1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid1();
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void grDateSporuri2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid2();
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void IncarcaGrid1()
        {

            DataTable dt = new DataTable();
            DataSet ds = Session["AvsCereri"] as DataSet;
            if (ds == null)
            {
                ds = new DataSet();

                dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);
            }
            DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Sporuri1"))
            {
                dt = dsCalcul.Tables["Sporuri1"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Spor", typeof(string));
                dt.Columns.Add("Tarif", typeof(string));
                dt.Columns.Add("F02504", typeof(int));
                dt.Columns.Add("F01105", typeof(int));
                dt.Columns.Add("Id", typeof(int));

                string sql = "";
                string cmp = "ISNULL";
                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                if (Constante.tipBD == 2)
                    cmp = "NVL";

                for (int i = 0; i <= 9; i++)
                {
                    string val = "0";
                    DataTable dtTemp = General.IncarcaDT("select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + ds.Tables[0].Rows[0]["F10065" + i].ToString(), null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                        val = dtTemp.Rows[0][0].ToString();


                    //Florin 2019.06.20
                    //am inlocuit TOP 1 cu ROWNUM
                    if (Constante.tipBD == 1)
                        sql += "select " + (i + 1).ToString() + " as \"Id\", f10065" + i + " as F02504, CASE WHEN f10065" + i + " = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT TOP 1 F02505 FROM F025 WHERE F02504 = F10065" + i + ") END as \"Spor\", "
                            + " case when f10065" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when f10065" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select top 1 f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = f10065" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + "), '---')  END as \"Tarif\" "
                            + " from f100 where f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                    else
                        sql += "select " + (i + 1).ToString() + " as \"Id\", f10065" + i + " as F02504, CASE WHEN f10065" + i + " = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT F02505 FROM F025 WHERE F02504 = F10065" + i + " AND ROWNUM <= 1) END as \"Spor\", "
                            + " case when f10065" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when f10065" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = f10065" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " AND ROWNUM <= 1), '---')  END as \"Tarif\" "
                            + " from f100 where f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();


                    if (i < 9)
                        sql += " UNION ";
                }

                dt = General.IncarcaDT(sql, null);
                dt.TableName = "Sporuri1";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                dsCalcul.Tables.Add(dt);
            }
            grDateSporuri1.KeyFieldName = "Id";
            grDateSporuri1.DataSource = dt;

            Session["AvsCereri"] = ds;
            Session["AvsCereriCalcul"] = dsCalcul;

        }

        private void IncarcaGrid2()
        {
            DataTable dt = new DataTable();
            DataSet ds = Session["AvsCereri"] as DataSet;
            if (ds == null)
            {
                ds = new DataSet();

                dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);
            }
            DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Sporuri2"))
            {
                dt = dsCalcul.Tables["Sporuri2"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Spor", typeof(string));
                dt.Columns.Add("Tarif", typeof(string));
                dt.Columns.Add("F02504", typeof(int));
                dt.Columns.Add("F01105", typeof(int));
                dt.Columns.Add("Id", typeof(int));

                string sql = "";
                string cmp = "ISNULL";
                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                if (Constante.tipBD == 2)
                    cmp = "NVL";
                for (int i = 0; i <= 9; i++)
                {
                    string val = "0";
                    DataTable dtTemp = General.IncarcaDT("select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + ds.Tables[0].Rows[0]["F10066" + i].ToString(), null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                        val = dtTemp.Rows[0][0].ToString();


                    //Florin 2019.06.20
                    //am inlocuit TOP 1 cu ROWNUM
                    if (Constante.tipBD == 1)
                        sql += "select " + (i + 11).ToString() + " as \"Id\", f10066" + i + " as F02504, CASE WHEN f10066" + i + " = 0 THEN 'Spor " + (i + 11).ToString() + "' ELSE (SELECT TOP 1 F02505 FROM F025 WHERE F02504 = F10066" + i + ") END as \"Spor\", "
                            + " case when f10066" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when f10066" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select top 1 f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = f10066" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + "), '---')  END as \"Tarif\" "
                            + " from f100 where f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                    else
                        sql += "select " + (i + 11).ToString() + " as \"Id\", f10066" + i + " as F02504, CASE WHEN f10066" + i + " = 0 THEN 'Spor " + (i + 11).ToString() + "' ELSE (SELECT F02505 FROM F025 WHERE F02504 = F10066" + i + " AND ROWNUM <= 1) END as \"Spor\", "
                            + " case when f10066" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when f10066" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = f10066" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " AND ROWNUM <= 1), '---')  END as \"Tarif\" "
                            + " from f100 where f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();


                    if (i < 9)
                        sql += " UNION ";
                }

                dt = General.IncarcaDT(sql, null);
                dt.TableName = "Sporuri2";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                dsCalcul.Tables.Add(dt);
            }
            grDateSporuri2.KeyFieldName = "Id";
            grDateSporuri2.DataSource = dt;

            Session["AvsCereri"] = ds;
            Session["AvsCereriCalcul"] = dsCalcul;
        }

        protected void grDateSporuri1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["Spor"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster1");
                e.NewValues["F02504"] = cb1.Value;
                e.NewValues["Spor"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["Tarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild1");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["Tarif"] = cb2.Text;

                if (e.NewValues["F02504"].ToString() == "0")
                {
                    e.NewValues["Spor"] = "Spor " + (grDateSporuri1.EditingRowVisibleIndex + 1).ToString();
                    e.NewValues["Tarif"] = "---";
                }

                if (e.NewValues["Spor"] == null || e.NewValues["Spor"].ToString().Length < 0)
                    return;

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["AvsCereri"] as DataSet;
                DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["Sporuri1"].Rows.Find(keys);
                int poz = 0, val = 0;

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Sporuri1"].Rows.Count; i++)
                {
                    if (grDateSporuri1.EditingRowVisibleIndex != i && e.NewValues["F02504"].ToString() != "0" && dsCalcul.Tables["Sporuri1"].Rows[i]["F02504"].ToString() == e.NewValues["F02504"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateSporuri1.JSProperties["cpAlertMessage"] = "Acest spor a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Sporuri1"].Columns)
                    {
                        if (col.ColumnName != "Id")
                        {
                            col.ReadOnly = false;
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                            if (col.ColumnName == "F02504")
                            {
                                DataTable dt = General.IncarcaDT("  select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + e.NewValues[col.ColumnName], null);
                                poz = Convert.ToInt32(dt == null || dt.Rows.Count <= 0 || dt.Rows[0][0] == DBNull.Value ? "0" : dt.Rows[0][0].ToString());
                            }
                            if (col.ColumnName == "F01105")
                                val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        }
                    }
                }

                e.Cancel = true;
                grDateSporuri1.CancelEdit();

                if (!dublura)
                {
                    ds.Tables[0].Rows[0]["F10065" + (Convert.ToInt32(keys[0]) - 1).ToString()] = e.NewValues["F02504"];


                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    string sirNou = "";
                    for (int i = 0; i < sir.Length; i++)
                        if (i == poz - 1)
                            sirNou += val.ToString();
                        else
                            sirNou += sir[i];

                    ds.Tables[0].Rows[0]["F10067"] = sirNou;
                }

                Session["AvsCereri"] = ds;
                Session["AvsCereriCalcul"] = dsCalcul;
                grDateSporuri1.DataSource = dsCalcul.Tables["Sporuri1"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateSporuri2_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {


                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["Spor"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster2");
                e.NewValues["F02504"] = cb1.Value;
                e.NewValues["Spor"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["Tarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild2");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["Tarif"] = cb2.Text;

                if (e.NewValues["F02504"].ToString() == "0")
                {
                    e.NewValues["Spor"] = "Spor " + (grDateSporuri2.EditingRowVisibleIndex + 11).ToString();
                    e.NewValues["Tarif"] = "---";
                }

                if (e.NewValues["Spor"] == null || e.NewValues["Spor"].ToString().Length < 0)
                    return;

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["AvsCereri"] as DataSet;
                DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
                int poz = 0, val = 0;
                DataRow row = dsCalcul.Tables["Sporuri2"].Rows.Find(keys);

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Sporuri2"].Rows.Count; i++)
                {
                    if (grDateSporuri2.EditingRowVisibleIndex != i && e.NewValues["F02504"].ToString() != "0" && dsCalcul.Tables["Sporuri2"].Rows[i]["F02504"].ToString() == e.NewValues["F02504"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateSporuri2.JSProperties["cpAlertMessage"] = "Acest spor a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Sporuri2"].Columns)
                    {
                        if (col.ColumnName != "Id")
                        {
                            col.ReadOnly = false;
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                            if (col.ColumnName == "F02504")
                            {
                                DataTable dt = General.IncarcaDT("  select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + e.NewValues[col.ColumnName], null);
                                poz = Convert.ToInt32(dt == null || dt.Rows.Count <= 0 || dt.Rows[0][0] == DBNull.Value ? "0" : dt.Rows[0][0].ToString());
                            }
                            if (col.ColumnName == "F01105")
                                val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        }
                    }
                }
                e.Cancel = true;
                grDateSporuri2.CancelEdit();

                if (!dublura)
                {
                    ds.Tables[0].Rows[0]["F10066" + (Convert.ToInt32(keys[0]) - 11).ToString()] = e.NewValues["F02504"];

                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    string sirNou = "";
                    for (int i = 0; i < sir.Length; i++)
                        if (i == poz - 1)
                            sirNou += val.ToString();
                        else
                            sirNou += sir[i];

                    ds.Tables[0].Rows[0]["F10067"] = sirNou;
                }

                Session["AvsCereri"] = ds;
                Session["AvsCereriCalcul"] = dsCalcul;
                grDateSporuri2.DataSource = dsCalcul.Tables["Sporuri2"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void cmbMaster1_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged1(s, e, {0}); }}", templateContainer.VisibleIndex);

            ObjectDataSource cmbMasterDataSource = cmbParent.NamingContainer.FindControl("adsMaster1") as ObjectDataSource;

            cmbMasterDataSource.SelectParameters.Clear();
            cmbMasterDataSource.SelectParameters.Add("param", "1");
            cmbMasterDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbParent.DataBindItems();

            string[] param = templateContainer.ClientID.Split('_');
            if (param[1] != "editnew")
            {
                object[] obj = grDateSporuri1.GetRowValues(grDateSporuri1.FocusedRowIndex, new string[] { "Spor", "Tarif", "F02504" }) as object[];
                cmbParent.Value = Convert.ToInt32(obj[2].ToString());
                Session["Sporuri_cmbMaster1"] = Convert.ToInt32(obj[2].ToString());
            }


        }

        protected void cmbChild1_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild1_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild1_new", templateContainer.VisibleIndex);
            else
            {
                ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild1") as ObjectDataSource;

                cmbChildDataSource.SelectParameters.Clear();
                cmbChildDataSource.SelectParameters.Add("categ", Session["Sporuri_cmbMaster1"].ToString());
                cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
                cmbChild.DataBindItems();
                //cmbChild.Value = Convert.ToInt32(param[2]);
            }

            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild1_Callback);

        }

        protected void cmbChild1_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild1") as ObjectDataSource;

            cmbChildDataSource.SelectParameters.Clear();
            cmbChildDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbChild.DataBindItems();
        }



        protected void cmbMaster2_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged2(s, e, {0}); }}", templateContainer.VisibleIndex);

            ObjectDataSource cmbMasterDataSource = cmbParent.NamingContainer.FindControl("adsMaster2") as ObjectDataSource;

            cmbMasterDataSource.SelectParameters.Clear();
            cmbMasterDataSource.SelectParameters.Add("param", "0");
            cmbMasterDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbParent.DataBindItems();

            string[] param = templateContainer.ClientID.Split('_');
            if (param[1] != "editnew")
            {
                object[] obj = grDateSporuri2.GetRowValues(grDateSporuri2.FocusedRowIndex, new string[] { "Spor", "Tarif", "F02504" }) as object[];
                cmbParent.Value = Convert.ToInt32(obj[2].ToString());
                Session["Sporuri_cmbMaster2"] = Convert.ToInt32(obj[2].ToString());
            }
        }

        protected void cmbChild2_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild2_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild2_new", templateContainer.VisibleIndex);
            else
            {
                ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild2") as ObjectDataSource;

                cmbChildDataSource.SelectParameters.Clear();
                cmbChildDataSource.SelectParameters.Add("categ", Session["Sporuri_cmbMaster2"].ToString());
                cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
                cmbChild.DataBindItems();
                //cmbChild.Value = Convert.ToInt32(param[2]);
            }

            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild2_Callback);

        }

        protected void cmbChild2_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild2") as ObjectDataSource;

            cmbChildDataSource.SelectParameters.Clear();
            cmbChildDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbChild.DataBindItems();
        }


        protected void grDateSporTran_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGridSpTr();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGridSpTr()
        {
            DataTable dt = new DataTable();
            DataSet ds = Session["AvsCereri"] as DataSet;
            if (ds == null)
            {
                ds = new DataSet();

                dt = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F100";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);

                dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dt.TableName = "F1001";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                ds.Tables.Add(dt);
            }
            DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;
            string sql = "";
            if (dsCalcul != null && dsCalcul.Tables.Contains("SporTran"))
            {
                dt = dsCalcul.Tables["SporTran"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Spor", typeof(int));
                dt.Columns.Add("Cod", typeof(string));



                string cmp = "CONVERT(VARCHAR, ";
                if (Constante.tipBD == 2)
                    cmp = "TO_CHAR(";
                for (int i = 80; i <= 99; i++)
                {
                    sql += "select " + (i - 79).ToString() + " as \"Id\", f10095" + i + " as \"Spor\", CASE WHEN f10095" + i + " = 0 THEN 'Spor " + (i - 79).ToString() + "' ELSE " + cmp + " f10095" + i + ") END as \"Cod\" from f1001 where f10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                    if (i < 99)
                        sql += " UNION ";
                }

                dt = General.IncarcaDT(sql, null);
                dt.Columns["Spor"].ReadOnly = false;
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                dt.TableName = "SporTran";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateSporTran.KeyFieldName = "Id";
            grDateSporTran.DataSource = dt;
            grDateSporTran.SettingsPager.PageSize = 20;


            //Florin 2019.10.28 - am comentat partea de Oracle deoarece este acelasi select si pe sql si pe oracle
            sql = $@"SELECT 0 as F02104, '---' AS F02105 {(Constante.tipBD == 2 ? " FROM DUAL" : "")} UNION SELECT F02104, F02105 FROM F021 WHERE F02162 IS NOT NULL AND F02162 <> 0";
            //if (Constante.tipBD == 2)
            //    sql = "SELECT 0 as F02104, '---' AS F02105 FROM DUAL UNION " + General.SelectOracle("F021", "F02104") + " WHERE F02162 IS NOT NULL AND F02162 <> 0 ";
            DataTable dtSpor = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colSpor = (grDateSporTran.Columns["Spor"] as GridViewDataComboBoxColumn);
            colSpor.PropertiesComboBox.DataSource = dtSpor;

            Session["AvsCereri"] = ds;
            Session["AvsCereriCalcul"] = dsCalcul;

        }



        protected void grDateSporTran_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool dublura = false;
                DataSet ds = Session["AvsCereri"] as DataSet;
                DataSet dsCalcul = Session["AvsCereriCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["SporTran"].Rows.Find(keys);
                for (int i = 0; i < dsCalcul.Tables["SporTran"].Rows.Count; i++)
                {
                    if (grDateSporTran.EditingRowVisibleIndex != i && e.NewValues["Spor"].ToString() != "0" && dsCalcul.Tables["SporTran"].Rows[i]["Spor"].ToString() == e.NewValues["Spor"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateSporTran.JSProperties["cpAlertMessage"] = "Acest spor a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["SporTran"].Columns)
                    {
                        col.ReadOnly = false;
                        if (col.ColumnName != "Id")
                        {
                            //row[col.ColumnName] = e.NewValues[col.ColumnName];
                            if (col.ColumnName == "Cod" && e.NewValues["Spor"].ToString() == "0")
                                row[col.ColumnName] = "Spor " + keys[0];
                            else
                                row[col.ColumnName] = e.NewValues["Spor"];
                        }
                    }
                }
                e.Cancel = true;
                grDateSporTran.CancelEdit();

                if (!dublura)
                {
                    ds.Tables[1].Rows[0]["F10095" + (79 + Convert.ToInt32(keys[0])).ToString()] = e.NewValues["Spor"];
                }

                Session["AvsCereri"] = ds;
                Session["AvsCereriCalcul"] = dsCalcul;

                grDateSporTran.DataSource = dsCalcul.Tables["SporTran"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }


        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                metaCereriDate itm = new metaCereriDate();
                if (Session["Avs_Cereri_Date"] != null) itm = Session["Avs_Cereri_Date"] as metaCereriDate;

                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["Avs_Cereri_Date"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AdaugaNotaLichidare(int f10003)
        {
            try
            {        
                string sql = "INSERT INTO \"MP_NotaLichidare\"(F10003, \"NrDoc\", \"DataDoc\", \"IdStare\", USER_NO, TIME) OUTPUT Inserted.IdAuto "
                    + " VALUES (" + f10003 + ", (CASE WHEN (SELECT COUNT(*) FROM \"MP_NotaLichidare\") = 0 THEN 1 ELSE  (SELECT max(\"NrDoc\")+1 FROM \"MP_NotaLichidare\") END), "
                    + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") +", 1, " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ") ";

                if (Constante.tipBD == 2)
                    sql = "INSERT INTO \"MP_NotaLichidare\"(F10003, \"NrDoc\", \"DataDoc\", \"IdStare\", USER_NO, TIME) "
                    + " VALUES (" + f10003 + ", (CASE WHEN (SELECT COUNT(*) FROM \"MP_NotaLichidare\") = 0 THEN 1 ELSE  (SELECT max(\"NrDoc\")+1 FROM \"MP_NotaLichidare\") END), "
                    + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ", 1, " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ") RETURNING \"IdAuto\" INTO @out_1";

                DataTable dtCer = new DataTable();
                int idAuto = -99;
                if (Constante.tipBD == 1)
                {
                    dtCer = General.IncarcaDT(sql, null);
                    if (dtCer.Rows.Count > 0)                    
                        idAuto = Convert.ToInt32(dtCer.Rows[0]["IdAuto"]);                    
                }
                else
                {
                    List<string> lstOut = General.DamiOracleScalar(sql, new object[] { "int" });
                    if (lstOut.Count == 1)                    
                        idAuto = Convert.ToInt32(lstOut[0]);                    
                }

                sql = "SELECT * FROM \"MP_NotaLichidare_Circuit\" WHERE \"GrupAngajati\" IN (SELECT \"IdGrup\" FROM \"relGrupAngajat\" WHERE F10003 = " + f10003 + ") ORDER BY \"GrupAngajati\" DESC";
                DataTable dt = General.IncarcaDT(sql, null);            
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    int grup = Convert.ToInt32(dt.Rows[0]["GrupAngajati"].ToString());
                    int i = 0;
                    while (grup == Convert.ToInt32(dt.Rows[i]["GrupAngajati"].ToString()))
                    {
                        string valoare = "NULL";
                        if (dt.Rows[i]["Valoare"] != null && dt.Rows[i]["Valoare"].ToString().Length > 0)
                        {
                            DataTable dtVal = General.IncarcaDT(dt.Rows[i]["Valoare"].ToString().Replace("ent.F10003", f10003.ToString()), null);
                            if (dtVal != null && dtVal.Rows.Count > 0 && dtVal.Rows[0][0] != null)
                                valoare = "'" + dtVal.Rows[0][0].ToString() + "'";
                        }

                        sql = "INSERT INTO \"MP_NotaLichidare_Detalii\"(F10003, \"IdStare\", \"IdNotaLichidare\", \"Rol\", USER_NO, TIME, \"Valoare\", \"Supervizor\") "
                            + " VALUES (" + f10003 + ", 1, " + idAuto + ", '" + dt.Rows[i]["Rol"].ToString() + "', " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ", " + valoare + ", '" + dt.Rows[i]["Supervizor"].ToString() + "')";
                        General.ExecutaNonQuery(sql, null);

                        i++;
                        if (i == dt.Rows.Count)
                            break;
                        grup = Convert.ToInt32(dt.Rows[i]["GrupAngajati"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private  void CompletareZile()
        {
            if (cmbAtribute.Value != null && Convert.ToInt32(cmbAtribute.Value) == 2 && cmb2Nou.Value != null)
            { 
                string nvlFunc = Session["Avs_NvlFunc"].ToString();
                string[] resNF = nvlFunc.Split(';');
                for (int i = 0; i < resNF.Length; i++)
                {
                    string[] linieNF = resNF[i].Split(',');
                    if (Convert.ToInt32(linieNF[0]) == Convert.ToInt32(cmb2Nou.Value))
                    {
                        if (linieNF[1].Length > 0)
                            txt1Nou.Text = linieNF[1];
                        else
                            txt1Nou.Text = "0";
                        if (linieNF[2].Length > 0)
                            txt2Nou.Text = linieNF[2];
                        else
                            txt2Nou.Text = "0";
                        if (linieNF[3].Length > 0)
                            txt3Nou.Text = linieNF[3];
                        else
                            txt3Nou.Text = "0";
                        if (linieNF[4].Length > 0)
                            txt4Nou.Text = linieNF[4];
                        else
                            txt4Nou.Text = "0";
                    }
                }
            }
        }
        private void ValidareZile(int tip)
        {
            if (cmbAtribute.Value != null && Convert.ToInt32(cmbAtribute.Value) == 2)
            {
                string mesaj = "";
                bool conducere = false;
                string nvlFunc = Session["Avs_NvlFunc"].ToString();
                string durCtr = Session["Avs_DurCtr"].ToString();
                string nrLuni = Session["Avs_NrLuni"].ToString();
                string nrZile = Session["Avs_NrZile"].ToString();
                string grdInv = Session["Avs_GrdInv"].ToString();
                string[] resNF = nvlFunc.Split(';');
                if (cmb2Nou.Value != null)
                    for (int i = 0; i < resNF.Length; i++)
                    {
                        string[] linieNF = resNF[i].Split(',');
                        if (Convert.ToInt32(linieNF[0]) == Convert.ToInt32(cmb2Nou.Value))
                        {
                            if (Convert.ToInt32(linieNF[5]) == 1)
                                conducere = true;
                        }
                    }

                if (Convert.ToInt32(durCtr) == 2 && txt1Nou.Text != "" && nrLuni != "" && nrZile != "")
                {
                    if (Convert.ToInt32(nrLuni) != 0 || Convert.ToInt32(nrZile) != 0)
                    {
                        if (Convert.ToInt32(nrLuni) < 3 && Convert.ToInt32(txt1Nou.Text) > 5)
                        {
                            mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 5 zile!\n";
                            if (tip == 1)
                                txt1Nou.Text = "5";
                        }
                        if (Convert.ToInt32(nrLuni) >= 3 && Convert.ToInt32(nrLuni) < 6 && Convert.ToInt32(txt1Nou.Text) > 15)
                        {
                            mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 15 zile!\n";
                            if (tip == 1)
                                txt1Nou.Text = "15";
                        }
                        if (Convert.ToInt32(nrLuni) >= 6 && !conducere && Convert.ToInt32(txt1Nou.Text) > 30)
                        {
                            mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 30 zile!\n";
                            if (tip == 1)
                                txt1Nou.Text = "30";
                        }
                        if (Convert.ToInt32(nrLuni) >= 6 && conducere && Convert.ToInt32(txt1Nou.Text) > 45)
                        {
                            mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 45 zile!\n";
                            if (tip == 1)
                                txt1Nou.Text = "45";
                        }
                        //txtPerProbaZC.SetValue("0");
                    }
                }

                if (Convert.ToInt32(durCtr) == 1 && txt2Nou.Text != "")
                {
                    if (!conducere && Convert.ToInt32(txt2Nou.Text) > 90)
                    {
                        mesaj += "Perioada de proba (zile calendaristice): - valoarea maxima cf. legii este de 90 zile!\n";
                        if (tip == 1)
                            txt2Nou.Text = "90";
                    }
                    if (conducere && Convert.ToInt32(txt2Nou.Text) > 120)
                    {
                        mesaj += "Perioada de proba (zile calendaristice): - valoarea maxima cf. legii este de 120 zile!\n";
                        if (tip == 1)
                            txt2Nou.Text = "120";
                    }
                    if ((Convert.ToInt32(grdInv) == 2 || Convert.ToInt32(grdInv) == 3) && Convert.ToInt32(txt2Nou.Text) > 30)
                    {
                        mesaj += "Perioada de proba (zile calendaristice): - valoarea maxima cf. legii este de 30 zile!\n";
                        if (tip == 1)
                            txt2Nou.Text = "30";
                    }
                    //txtPerProbaZL.SetValue("0");
                }

                if (txt3Nou.Text != "")
                {
                    if (!conducere && Convert.ToInt32(txt3Nou.Text) > 20)
                    {
                        mesaj += "Numar zile preaviz demisie: - valoarea maxima cf. legii este de 20 zile!\n";
                        if (tip == 1)
                            txt3Nou.Text = "20";
                    }
                    if (conducere && Convert.ToInt32(txt3Nou.Text) > 45)
                    {
                        mesaj += "Numar zile preaviz demisie: - valoarea maxima cf. legii este de 45 zile!\n";
                        if (tip == 1)
                            txt3Nou.Text = "45";
                    }
                }

                if (txt4Nou.Text.Length <= 0 || Convert.ToInt32(txt4Nou.Text) < 20)
                {
                    mesaj += "Numar zile preaviz concediere: - valoarea minima cf. legii este de 20 zile!\n";
                    if (tip == 1)
                        txt4Nou.Text = "20";
                }

                if (mesaj.Length > 0)
                    pnlCtl.JSProperties["cpAlertMessage"] = mesaj;
            }
        }



        //Validare IBAN
        private static string AnulareCaractere(string source)
        {
            //_isValid = false;
            source = source.Replace(" ", "");
            if (string.IsNullOrEmpty(source)) return source;

            if (source.Length != 24)
            {
                return "";
            }

            int j = 0;
            int i = 0;

            string test = "";
            while (i < source.Length)
            {
                switch (source[i])
                {
                    case 'A':
                    case 'a':
                    case 'B':
                    case 'b':
                    case 'C':
                    case 'c':
                    case 'D':
                    case 'd':
                    case 'E':
                    case 'e':
                    case 'F':
                    case 'f':
                    case 'G':
                    case 'g':
                    case 'H':
                    case 'h':
                    case 'I':
                    case 'i':
                    case 'J':
                    case 'j':
                    case 'K':
                    case 'k':
                    case 'L':
                    case 'l':
                    case 'M':
                    case 'm':
                    case 'N':
                    case 'n':
                    case 'O':
                    case 'o':
                    case 'P':
                    case 'p':
                    case 'Q':
                    case 'q':
                    case 'R':
                    case 'r':
                    case 'S':
                    case 's':
                    case 'T':
                    case 't':
                    case 'U':
                    case 'u':
                    case 'V':
                    case 'v':
                    case 'W':
                    case 'w':
                    case 'X':
                    case 'x':
                    case 'Y':
                    case 'y':
                    case 'Z':
                    case 'z':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        test += source[i];
                        i++; break;

                    default:
                        i++; break;
                };//	
            }
            j = source.Length - test.Length;// nr caractere non alfanumerice

            if (j != 0) //nr caractere non alfa numerice;
            {
                return "";
            }

            source = source.ToUpper();
            return source;

        }

        private static string Reordonare(string source)
        {
            return source.Substring(4) + source.Substring(0, 4);
        }

        private static string Conversie(string source)
        {
            string result = "";
            for (int i = 0; i < source.Length; i++)
            {
                result += Substitutie(source[i]);
            }
            return result;
        }

        private static string Substitutie(char SourceC)
        {
            string ResStr = "";
            switch (SourceC)
            {
                case 'A': ResStr = "10"; break;
                case 'B': ResStr = "11"; break;
                case 'C': ResStr = "12"; break;
                case 'D': ResStr = "13"; break;
                case 'E': ResStr = "14"; break;
                case 'F': ResStr = "15"; break;
                case 'G': ResStr = "16"; break;
                case 'H': ResStr = "17"; break;
                case 'I': ResStr = "18"; break;
                case 'J': ResStr = "19"; break;
                case 'K': ResStr = "20"; break;
                case 'L': ResStr = "21"; break;
                case 'M': ResStr = "22"; break;
                case 'N': ResStr = "23"; break;
                case 'O': ResStr = "24"; break;
                case 'P': ResStr = "25"; break;
                case 'Q': ResStr = "26"; break;
                case 'R': ResStr = "27"; break;
                case 'S': ResStr = "28"; break;
                case 'T': ResStr = "29"; break;
                case 'U': ResStr = "30"; break;
                case 'V': ResStr = "31"; break;
                case 'W': ResStr = "32"; break;
                case 'X': ResStr = "33"; break;
                case 'Y': ResStr = "34"; break;
                case 'Z': ResStr = "35"; break;

                case '0': ResStr = "0"; break;
                case '1': ResStr = "1"; break;
                case '2': ResStr = "2"; break;
                case '3': ResStr = "3"; break;
                case '4': ResStr = "4"; break;
                case '5': ResStr = "5"; break;
                case '6': ResStr = "6"; break;
                case '7': ResStr = "7"; break;
                case '8': ResStr = "8"; break;
                case '9': ResStr = "9"; break;
            };// 
            return ResStr;
        }
      
        public bool IbanValid(string source)
        {
            bool result = false;

            source = AnulareCaractere(source);
            if (source.Length != 24)
            {
                return false;
            }

            source = Reordonare(source);
            if (source.Length != 24)
            {
                return false;
            }
            source = Conversie(source);
            result = Validare(source);

            return result;

        }

        private static bool Validare(string source)
        {
            BigInteger bigint = new BigInteger();
            BigInteger.TryParse(source, out bigint);
            BigInteger rest = BigInteger.Remainder(bigint, new BigInteger(97.0));

            return rest == 1;

        }

        private bool VerificareSalariu(int salariu, int timpPartial)
        {
            int salMin = Convert.ToInt32(Dami.ValoareParam("SAL_MIN", "0"));
            if (salMin * timpPartial / 8 > salariu)
                return false;
            else
                return true;
        }

    }


}