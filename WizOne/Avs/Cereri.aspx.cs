using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Avs
{
    public partial class Cereri : System.Web.UI.Page
    {
        int F10003 = -99;

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

                    DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAng.Value, -99) });
                    cmbAtribute.DataSource = dtAtr;
                    cmbAtribute.DataBind();
                    cmbAtributeFiltru.DataSource = dtAtr;
                    cmbAtributeFiltru.DataBind();

                    if (Session["Marca_atribut"] != null)
                    {
                        string[] param = Session["Marca_atribut"].ToString().Split(';');
                        F10003 = Convert.ToInt32(param[0]);
                        for (int i = 0; i < cmbAng.Items.Count; i++)
                            if (cmbAng.Items[i].Value.ToString() == param[0])
                            {
                                cmbAng.SelectedIndex = i;
                                cmbAngFiltru.SelectedIndex = i;
                                break;
                            }
                        cmbAng.Enabled = false;
                        cmbAngFiltru.Enabled = false;
                        for (int i = 0; i < cmbAtribute.Items.Count; i++)
                            if (Convert.ToInt32(cmbAtribute.Items[i].Value) == Convert.ToInt32(param[1]))
                            {
                                cmbAtribute.SelectedIndex = i;
                                break;
                            }
                        cmbAtribute.Enabled = false;
                    }
                    AscundeCtl();
                    IncarcaDate();


                }
                else
                {
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
                        cmbAtributeFiltru.DataSource = dtAtr;
                        cmbAtributeFiltru.DataBind();

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
        //        //ArataMesaj("Atentie !");
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
                //ArataMesaj("Atentie !");
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
                //ArataMesaj("Atentie !");
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
        //          //ArataMesaj("Atentie !");
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
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
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
                //ArataMesaj("Atentie !");
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

            lblTxt1Act.Visible = false;
            lblTxt1Act.Text = "";
            lblTxt2Act.Visible = false;
            lblTxt2Act.Text = "";
            txt1Nou.Text = "";
            txt2Nou.Text = "";
            txt1Act.Visible = false;
            txt1Act.Enabled = false;
            txt2Act.Visible = false;
            txt2Act.Enabled = false;

            lblTxt1Nou.Visible = false;
            lblTxt1Nou.Text = "";
            lblTxt2Nou.Visible = false;
            lblTxt2Nou.Text = "";
            txt1Nou.Visible = false;
            txt2Nou.Visible = false;

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
            lblTxt10Nou.Visible = false;
            lblTxt10Nou.Text = "";
            lblTxt11Nou.Visible = false;
            lblTxt11Nou.Text = "";

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
        }

        private void ArataCtl(int nr, string text1, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9, string text10)
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
            {// 7 x CB + 1 x TB
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

        }


        private void IncarcaDate()
        {

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Salariul)
            {
                ArataCtl(2, "Salariu brut actual", "Salariu brut nou", "Salariu net actual", "Salariu net nou", "", "", "", "", "", "");
                DataTable dtTemp = General.IncarcaDT("SELECT F100699 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                CalcSalariu(1, txt1Act, txt2Act);
            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Functie)
            {
                ArataCtl(3, "Functia actuala", "Functia noua", "", "", "", "", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F71802 AS \"Id\", F71804 AS \"Denumire\" from F100, f718 WHERE F10071 = F71802 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71802 AS \"Id\", F71804 AS \"Denumire\" from f718", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.CodCOR)
            {
                ArataCtl(3, "Cod COR actual", "Cod COR nou", "", "", "", "", "", "", "", "");
                string sql = "";
                if (Constante.tipBD == 1)
                    sql = "select F72202 AS \"Id\", CONVERT(VARCHAR, F72202) + ' - '  +  F72204 AS \"Denumire\" from F100, f722 WHERE F10098 = F72202 AND f72206 = (select max(f72206) from f722) AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                else
                    sql = "select F72202 AS \"Id\", F72202 || ' - '  ||  F72204 AS \"Denumire\" from F100, f722 WHERE F10098 = F72202 AND f72206 = (select max(f72206) from f722) AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString();
                DataTable dtTemp1 = General.IncarcaDT(sql, null);
                if (Constante.tipBD == 1)
                    sql = "select F72202 AS \"Id\", CONVERT(VARCHAR, F72202) + ' - '  +  F72204 AS \"Denumire\" from f722";
                else
                    sql = "select F72202 AS \"Id\", F72202 || ' - '  ||  F72204 AS \"Denumire\" from f722"; ;
                DataTable dtTemp2 = General.IncarcaDT(sql, null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
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
                ArataCtl(12, "Tip angajat actual", "Tip angajat nou", "Timp partial", "Norma", "Tip norma", "Durata", "Repartizare", "Interval", "Nr. de ore luna/sapt", "");
                DataTable dtTemp1 = General.IncarcaDT("select F71602 AS \"Id\", F71604 AS \"Denumire\" from F100, F716 WHERE F10010 = F71602 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                DataTable dtTemp2 = General.IncarcaDT("select F71602 AS \"Id\", F71604 AS \"Denumire\" from F716", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);

                dtTemp1 = General.IncarcaDT("select F10043 AS \"Id\", F10043 AS \"Denumire\" from F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = General.ListaNumere(1, 8);
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
                IncarcaComboBox(cmb7Act, cmb7Nou, dtTemp1, dtTemp2);

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
                //nu mai este folosit
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
                //nu mai este folosit
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
                ArataCtl(8, "Banca actuala", "Banca noua", "Sucursala actuala", "Sucursala noua", "IBAN actual", "IBAN nou", "Nr. card actual", "Nr. card nou", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F100, F075 WHERE F10018 = F07503 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + " group by F07503 , F07509 ", null);
                DataTable dtTemp2 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F075 group by F07503 , F07509", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
                DataTable dtTemp = General.IncarcaDT("SELECT F10020, F10055 FROM F100 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();
                txt2Act.Text = dtTemp.Rows[0][1].ToString();

                dtTemp1 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F100, F075 WHERE F10019 = F07504 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                dtTemp2 = null;
                if (Session["Valoare1Noua"] != null)
                    dtTemp2 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F075 WHERE F07503 = " + Session["Valoare1Noua"].ToString().Split(';')[1], null);
                else
                    if (cmb1Nou.Value != null && cmb1Nou.SelectedIndex >= 0)
                    dtTemp2 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F075 WHERE F07503 = " + cmb1Nou.Items[cmb1Nou.SelectedIndex].Value.ToString(), null);

                IncarcaComboBox(cmb2Act, cmb2Nou, dtTemp1, dtTemp2);

            }

            if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.BancaGarantii)
            {
                ArataCtl(9, "Banca actuala", "Banca noua", "Sucursala actuala", "Sucursala noua", "IBAN actual", "IBAN nou", "", "", "", "");
                DataTable dtTemp1 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F1001, F075 WHERE F1001026 = F07503 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString() + "  group by F07503 , F07509", null);
                DataTable dtTemp2 = General.IncarcaDT("select F07503 AS \"Id\", F07509 AS \"Denumire\" from F075  group by F07503 , F07509", null);
                IncarcaComboBox(cmb1Act, cmb1Nou, dtTemp1, dtTemp2);
                DataTable dtTemp = General.IncarcaDT("SELECT F1001028 FROM F1001 WHERE F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
                txt1Act.Text = dtTemp.Rows[0][0].ToString();

                dtTemp1 = General.IncarcaDT("select F07504 AS \"Id\", F07505 AS \"Denumire\" from F1001, F075 WHERE F1001027 = F07504 AND F10003 = " + cmbAng.Items[cmbAng.SelectedIndex].Value.ToString(), null);
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
        }



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter.Split(';')[0];
                switch (tip)
                {
                    case "1":
                        AscundeCtl();
                        IncarcaDate();
                        string data = "";
                        SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out data);
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
                        if (e.Parameter.Split(';')[1] == "cmbStructOrgNou")
                            Session["Valoare8Noua"] = e.Parameter.Split(';')[1] + ";" + e.Parameter.Split(';')[2];
                        if (e.Parameter.Split(';')[1] == "txt1Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Salariul)
                            CalcSalariu(1, txt1Nou, txt2Nou);
                        if (e.Parameter.Split(';')[1] == "txt2Nou" && Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Salariul)
                            CalcSalariu(2, txt1Nou, txt2Nou);
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
                        if ((e.Parameter.Split(';')[1] == "cmb1Nou") && (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM ||
                            Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.PrelungireCIM_Vanz))
                        {
                            if (Convert.ToInt32(e.Parameter.Split(';')[2]) == 1)
                            {
                                de1Nou.Value = new DateTime(2100, 1, 1);
                                de2Nou.Value = new DateTime(2100, 1, 1);
                                txt1Nou.Value = "";
                                txt2Nou.Value = "";
                            }
                        }
                        IncarcaDate();
                        SetDataRevisal(1, Convert.ToDateTime(txtDataMod.Value), Convert.ToInt32(cmbAtribute.Value), out data);
                        if (Convert.ToInt32(cmbAtribute.Value) == (int)Constante.Atribute.Norma)
                            SetNorma(e.Parameter.Split(';')[1]);
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
                            cmbAtributeFiltru.DataSource = dtAtr;
                            cmbAtributeFiltru.DataBind();

                            AscundeCtl();
                            txtExpl.Text = "";
                            cmbAtribute.Value = null;
                        }
                        break;
                    case "9":               //cmbAng
                        {
                            DataTable dtAtr = General.IncarcaDT(SelectAtribute(), new object[] { Session["UserId"], General.Nz(cmbRol.Value, -99), General.Nz(cmbAng.Value, -99) });
                            cmbAtribute.DataSource = dtAtr;
                            cmbAtribute.DataBind();
                            cmbAtributeFiltru.DataSource = dtAtr;
                            cmbAtributeFiltru.DataBind();
                        }
                        break;

                }



            }
            catch (Exception ex)
            {
                //ArataMesaj("Atentie !");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
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
            if (atribut == (int)Constante.Atribute.Functie || atribut == (int)Constante.Atribute.CodCOR || atribut == (int)Constante.Atribute.Norma || atribut == (int)Constante.Atribute.PrelungireCIM
                || atribut == (int)Constante.Atribute.PrelungireCIM_Vanz || atribut == (int)Constante.Atribute.ContrITM || atribut == (int)Constante.Atribute.ContrIn)
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
            if (atribut == (int)Constante.Atribute.Salariul || atribut == (int)Constante.Atribute.SporTranzactii || atribut == (int)Constante.Atribute.Sporuri)
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
            if (atribut == (int)Constante.Atribute.MotivPlecare)
            {
                deDataRevisal.Value = dataMod;
                if (param == 1)
                {
                    lblDataRevisal.Visible = true;
                    deDataRevisal.Visible = true;
                }
                else
                    data = dataMod.Day.ToString().PadLeft(2, '0') + "/" + dataMod.Month.ToString().PadLeft(2, '0') + "/" + dataMod.Year.ToString();
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
                if (idAtr == (int)Constante.Atribute.Functie || idAtr == (int)Constante.Atribute.CodCOR || idAtr == (int)Constante.Atribute.Norma || idAtr == (int)Constante.Atribute.PrelungireCIM
                        || idAtr == (int)Constante.Atribute.PrelungireCIM_Vanz || idAtr == (int)Constante.Atribute.ContrITM || idAtr == (int)Constante.Atribute.ContrIn ||
                        idAtr == (int)Constante.Atribute.Salariul || idAtr == (int)Constante.Atribute.SporTranzactii || idAtr == (int)Constante.Atribute.Sporuri || idAtr == (int)Constante.Atribute.MotivPlecare)
                    if (Convert.ToDateTime(deDataRevisal.Value).Date < DateTime.Now.Date && val == 1)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");             
                        return false;
                    }   
                
                if (idAtr == (int)Constante.Atribute.Norma && Convert.ToInt32(cmb6Nou.Value) == 3 && (Convert.ToInt32(cmb7Nou.Value ?? - 1) <= 0 || txt1Nou.Text.Length <= 0))
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pentru repartizare inegala trebuie sa completati intervalul si numarul de ore!");
                    return false;
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
                        if (de1Nou.Value == null || de2Nou.Value == null) strErr += ", date prelungire CIM";
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
                //ArataMesaj("Atentie !");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return false;
            }
        }



        private void CalcSalariu(int tipVenit, ASPxTextBox txt1, ASPxTextBox txt2)
        {
            try
            {
                //tipVenit = 1     VB -> SN
                //tipVenit = 2     SN -> VB

                if (tipVenit == 1 && !General.IsNumeric(txt1.Value))
                {
                    //ArataMesaj("Lipseste venitul brut !");
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipseste venitul brut !");
                    //MessageBox.Show(Dami.TraduCuvant("Lipseste venitul brut !"), MessageBox.icoError);
                    return;
                }

                if (tipVenit == 2 && !General.IsNumeric(txt2.Value))
                {
                    //ArataMesaj("Lipseste salariul net !");
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipseste salariul net !");
                    //MessageBox.Show(Dami.TraduCuvant("Lipseste salariul net !"), MessageBox.icoError);                  
                    return;
                }

                int i = 0;              //daca ajunge la 100 oprim iteratia ca sa nu devina bucla infinita

                decimal varCas = 10.5m;
                decimal varCass = 5.5m;
                decimal varSom = 0.5m;
                decimal varNr = 0;
                decimal scutit = 0;
                decimal tipAng = 1;
                decimal varDed = 250;
                decimal varImp = 16;


                List<decimal> lst = GetVariabileVB();
                varCass = lst[0];
                varSom = lst[1];
                varCas = lst[2];
                varNr = lst[3];
                scutit = lst[4];
                varImp = lst[7];

                //DataTable dtTemp = General.IncarcaDT("SELECT F10026 FROM F100 WHERE F10003 = " + F10003.ToString(), null);
                //scutit = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());

                //scutit = Convert.ToDecimal(txtScutitAct.EditValue ?? 0);
                tipAng = lst[5];
                decimal salMediu = lst[6];

                if (scutit == 1) varImp = 0;
                if (tipAng == 2) varSom = 0;         //daca este pensionar nu plateste somaj


                if (tipVenit == 1)           //VB -> SN
                {
                    varDed = DamiValDeducere(varNr, Convert.ToDecimal(txt1.Value ?? 0));
                    txt2.Text = CalcSN(Convert.ToDecimal(txt1.Value ?? 0), varCas, varCass, varSom, varImp, varDed, salMediu).ToString();
                }
                else                    //SN -> VB
                {
                    decimal SN = Convert.ToDecimal(txt2.Value ?? 1);
                    varDed = DamiValDeducere(varNr, SN);
                    decimal tmpVB = Math.Round((SN - (1.5m * varImp / 100 * varDed)) / (1 - varImp / 100 - (varImp / 100 * varDed / 2000) - ((1 - varImp / 100) * (varCas + varCass + varSom) / 100)));
                    decimal tmpSN = 0;

                    while (tmpSN != SN)
                    {
                        if (i > 100)
                        {
                            return;
                        }
                        else
                        {
                            i += 1;

                            varDed = DamiValDeducere(varNr, tmpVB);
                            tmpSN = CalcSN(tmpVB, varCas, varCass, varSom, varImp, varDed, salMediu);
                            if (tmpSN != SN)
                            {
                                if (tmpSN > SN)
                                    tmpVB -= 1;
                                else
                                    tmpVB += 1;
                            }
                        }
                    }

                    txt1.Text = tmpVB.ToString();
                }



            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public List<decimal> GetVariabileVB()
        {
            List<decimal> lst = new List<decimal>();

            try
            {
                int idCass = 0, idSomaj = 0, idCas = 0;
                decimal cass = 0, som = 0, cas = 0, nrDed = 0, scutit = 0, tipAng = 0, salMediu = 0, procImp = 0;
                DataTable dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CASS'", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    idCass = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'SOMAJ'", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    idSomaj = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CAS'", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    idCas = Convert.ToInt32(dtTemp.Rows[0][0].ToString());

                dtTemp = General.IncarcaDT("SELECT F01324 FROM F013 WHERE F01304 = " + idCass.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    cass = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT F01324 FROM F013 WHERE F01304 = " + idSomaj.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    som = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT F01324 FROM F013 WHERE F01304 = " + idCas.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    cas = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT COUNT(*) FROM F110 WHERE F11003 = " + F10003.ToString() + " AND F11016 = 1 ", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    nrDed = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT F10026 FROM F100 WHERE F10003 = " + F10003.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    scutit = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());
                dtTemp = General.IncarcaDT("SELECT F10010 FROM F100 WHERE F10003 = " + F10003.ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    tipAng = Convert.ToDecimal(dtTemp.Rows[0][0].ToString());

                dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE UPPER(\"Nume\") = 'SALARIULMEDIU'", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    salMediu = Convert.ToInt32(dtTemp.Rows[0][0].ToString());

                dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE UPPER(\"Nume\") = 'PROCENT_IMPOZIT'", null);
                if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0].ToString().Length > 0)
                    procImp = Convert.ToInt32(dtTemp.Rows[0][0].ToString());

                lst.Add(cass);
                lst.Add(som);
                lst.Add(cas);
                lst.Add(nrDed);
                lst.Add(scutit);
                lst.Add(tipAng);
                lst.Add(salMediu);
                lst.Add(procImp);
            }
            catch (Exception ex)
            {
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lst;
        }


        private decimal DamiValDeducere(decimal nrPersIntretinere, decimal VB)
        {
            decimal? varDed = 0;

            try
            {
                DataTable dtTemp = General.IncarcaDT("SELECT * FROM F730 WHERE F73004 <= " + Convert.ToInt32(VB).ToString() + " AND  F73006 >= " + Convert.ToInt32(VB).ToString(), null);
                if (dtTemp != null && dtTemp.Rows.Count > 0)
                {
                    switch (Convert.ToInt32(nrPersIntretinere))
                    {
                        case 0:
                            varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73008"].ToString());
                            break;
                        case 1:
                            varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73009"].ToString());
                            break;
                        case 2:
                            varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73010"].ToString());
                            break;
                        case 3:
                            varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73011"].ToString());
                            break;
                        default:
                            varDed = Convert.ToDecimal(dtTemp.Rows[0]["F73012"].ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return Convert.ToDecimal(varDed ?? 250);
        }


        private decimal CalcSN(decimal VB, decimal varCas, decimal varCass, decimal varSom, decimal varImp, decimal varDed, decimal salMediu)
        {
            try
            {
                //teorie:
                //SN = VB - IMP - TAXE
                //TAXE = CAS + CASS + SOM
                //DED = sumafixa * (1-(VB-1000)/2000)
                //IMP = varImp/100 * (VB - TAXE - DED)

                //unde:
                //CAS = round(VB * 10,5/100)
                //CASS = round(VB * 5,5/100)
                //SOM = round(VB * 0,5/100)

                //sumafixa: (este tabel)
                //250 fara pers. in intretinere
                //350 1 pers
                //450 2 pers

                decimal SN = 0;
                decimal cas = 0;
                decimal cass = 0;
                decimal som = 0;

                //in calculul CAS-ului, daca VB este mai mare decat salariul mediu * 5 ori atunci se plafoneaza la salariul mediu * 5 ori
                //if (VB > (salMediu * 5))
                //    cas = MathExt.Round((salMediu * 5) * varCas / 100, WizOne.Module.MidpointRounding.AwayFromZero);
                //else
                cas = MathExt.Round(VB * varCas / 100, MidpointRounding.AwayFromZero);

                if (0 < cas && cas <= 1) cas = 1;   //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0

                cass = VB * varCass / 100;
                som = VB * varSom / 100;

                if (0 < cass && cass <= 1)          //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0
                    cass = 1;
                else
                    cass = MathExt.Round(VB * varCass / 100, MidpointRounding.AwayFromZero);

                if (0 < som && som <= 1)            //Radu 04.04.2016 - am pus conditia sa fie strict mai mare ca 0
                    som = 1;
                else
                    som = MathExt.Round(VB * varSom / 100, MidpointRounding.AwayFromZero);

                decimal taxe = cas + cass + som;

                decimal ded = varDed;

                if (1001 <= VB && VB <= 3000)
                {
                    //ded = Math.Round((varDed * (1 - (VB - 1000) / 2000)), 0);
                    ded = (varDed * (1 - (VB - 1000) / 2000));
                    ded = Convert.ToDecimal(Math.Ceiling(Convert.ToDouble(ded / 10)) * 10);
                }

                decimal imp = MathExt.Round(varImp / 100 * (VB - taxe - ded), MidpointRounding.AwayFromZero);
                if (imp < 0) imp = 0;

                SN = MathExt.Round((VB - imp - taxe), MidpointRounding.AwayFromZero);

                return SN;
            }
            catch (Exception ex)
            {
                // Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
                return 0;
            }
        }


        private bool AdaugaCerere()
        {

            int idUrm = -99;
            idUrm = Convert.ToInt32(Dami.NextId("Avs_Cereri"));

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
            string culoare = (dtTemp.Rows[0][0] ?? "#FFFFFFFF").ToString();


            //Adaugare in Avs_Cereri
            int idAtr = -99;
            string dataModif = "";
            DateTime dm = Convert.ToDateTime(txtDataMod.Value);
            if (Constante.tipBD == 1)
                dataModif = " CONVERT(DATETIME, '" + dm.Day.ToString().PadLeft(2, '0') + "/" + dm.Month.ToString().PadLeft(2, '0') + "/" + dm.Year.ToString() + "', 103) ";
            else
                dataModif = " TO_DATE('" + dm.Day.ToString().PadLeft(2, '0') + "/" + dm.Month.ToString().PadLeft(2, '0') + "/" + dm.Year.ToString() + "', 'dd/mm/yyyy') ";

            idAtr = Convert.ToInt32((cmbAtribute.Value ?? -99));
            string camp1 = "", camp2 = "";
            switch (idAtr)
            {
                case (int)Constante.Atribute.Salariul:
                    camp1 = "\"SalariulBrut\", \"SalariulNet\"";
                    camp2 = txt1Nou.Text + ", " + txt2Nou.Text;
                    break;
                case (int)Constante.Atribute.Functie:
                    camp1 = "\"FunctieId\", \"FunctieNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.CodCOR:
                    camp1 = "\"CORCod\", \"CORNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.MotivPlecare:
                    camp1 = "\"MotivId\", \"MotivNume\"";
                    camp2 = cmb1Nou.Value.ToString() + ", '" + cmb1Nou.Text + "'";
                    break;
                case (int)Constante.Atribute.Organigrama:
                    //Florin 2018.11.23
                    camp1 = "\"SubcompanieId\", \"SubcompanieNume\", \"FilialaId\", \"FilialaNume\", \"SectieId\", \"SectieNume\", \"DeptId\", \"DeptNume\"";
                    ListEditItem itm = cmbStructOrgNou.SelectedItem;
                    camp2 = itm.GetFieldValue("F00304") + ",'" + itm.GetFieldValue("F00305") + "'," +
                            itm.GetFieldValue("F00405") + ",'" + itm.GetFieldValue("F00406") + "'," +
                            itm.GetFieldValue("F00506") + ",'" + itm.GetFieldValue("F00506") + "'," +
                            itm.GetFieldValue("F00607") + ",'" + itm.GetFieldValue("F00608") + "'"; 
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
                    camp1 = "\"TipAngajat\", \"TimpPartial\", \"Norma\", \"TipNorma\", \"DurataTimpMunca\", \"RepartizareTimpMunca\", \"IntervalRepartizare\", \"NrOreLuna\"";
                    camp2 = cmb1Nou.Value.ToString() + "," + cmb2Nou.Value.ToString() + "," + cmb3Nou.Value.ToString() + "," + cmb4Nou.Value.ToString() + "," + cmb5Nou.Value.ToString() + "," + cmb6Nou.Value.ToString() + "," + cmb7Nou.Value.ToString() + "," + (txt1Nou.Text.Length <= 0 ? "NULL" : txt1Nou.Text);
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

            sql = "INSERT INTO \"Avs_Cereri\" (\"Id\", F10003, \"IdAtribut\", \"IdCircuit\", \"Explicatii\", \"Motiv\", \"DataModif\", \"DataConsemnare\", \"Corectie\", \"Actualizat\", \"UserIntrod\", USER_NO, TIME, \"IdStare\", \"Culoare\", \"TotalCircuit\", \"Pozitie\", {0}) "
                + "VALUES (" + idUrm.ToString() + ", " + F10003.ToString() + ", " + idAtr.ToString() + ", " + idCircuit.ToString() + ", '" + txtExpl.Text + "', '', " + dataModif + ", null, 0, 0, " + Session["UserId"].ToString() + ", "
                + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", " + idStare.ToString() + ", (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString() + "), " + total.ToString() + ", " + pozUser.ToString() + ",  {1})";


            sql = string.Format(sql, camp1, camp2);
            General.ExecutaNonQuery(sql, null);

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
            if (idStare == 3 && Dami.ValoareParam("FinalizareCuActeAditionale") == "0")
            {
                TrimiteInF704(idUrm);
                if (idAtr == 2)
                    General.ModificaFunctieAngajat(F10003, Convert.ToInt32(General.Nz(cmb1Nou.Value,-99)), Convert.ToDateTime(txtDataMod.Value), new DateTime(2100,1,1));
            }

            //ArataMesaj("Proces finalizat cu succes!");
            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces finalizat cu succes!");
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                //ArataMesaj("Atentie !");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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

        }



        public void TrimiteInF704(int id)
        {
            try
            {
                string data = "", data2 = "", data3 = "", data4 = "", data5 = "", data6 = "", data7 = "", data8 = "", data9 = "", data10 = "";
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
                }
                DataTable dtCer = General.IncarcaDT("SELECT " + data + " AS DM, " + data2 + " AS DI, " + data3 + " AS DITM, " + data4 + " AS DA, " + data5 + " AS DELIB, "
                    + data6 + " AS DEXP, " + data7 + " AS DEMIT, " + data8 + " AS DEXPP, " + data9 + " AS DINC, " + data10 + " AS DSF, a.* FROM \"Avs_Cereri\" a WHERE \"Id\" = " + id.ToString(), null);
                if (dtCer == null || dtCer.Rows.Count == 0) return;

                int? idComp = 1;
                DataTable dtComp = General.IncarcaDT("SELECT * FROM F002", null);
                if (dtComp != null && dtComp.Rows.Count > 0 && dtComp.Rows[0]["F00202"] != null && dtComp.Rows[0]["F00202"].ToString().Length > 0)
                    idComp = Convert.ToInt32(dtComp.Rows[0]["F00202"].ToString());

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

                DataTable dtF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + f10003.ToString(), null);
                DataTable dtF1001 = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = " + f10003.ToString(), null);

                string data1 = "";
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
                }
                int act = 0;
                string sql = "", sql100 = "", sql1001 = "";

                switch (Convert.ToInt32(dtCer.Rows[0]["IdAtribut"].ToString()))
                {
                    case (int)Constante.Atribute.Salariul:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10026 = " + dtCer.Rows[0]["ScutitImpozit"].ToString() + ", F100699 = " + dtCer.Rows[0]["SalariulBrut"].ToString() + ", F100991 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70452, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 1, 'Salariu Tarifar', " + data + ", " + dtCer.Rows[0]["SalariulBrut"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", " + dtCer.Rows[0]["ScutitImpozit"].ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Functie:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10071 = " + dtCer.Rows[0]["FunctieId"].ToString() + ", F100992 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 2, 'Functie', " + data + ", " + dtCer.Rows[0]["FunctieId"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 3, 'Cod COR', " + data + ", " + dtCer.Rows[0]["CORCod"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.MotivPlecare:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10023 = " + data1 + ", F100993 = " + data + " WHERE F10003 = " + f10003.ToString();
                            }
                            //sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            //+ " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 4, 'Motiv plecare', " + data + ", " + dtCer.Rows[0]["MotivId"].ToString() + ", 'Modificari in avans', '"
                            //+ dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                                + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 4, 'Motiv plecare', " + data + ", (SELECT F72102 FROM F721 WHERE F72106 = " + dtCer.Rows[0]["MotivId"].ToString() + "), 'Modificari in avans', '"
                                + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.Norma:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10010 = " + dtCer.Rows[0]["TipAngajat"].ToString() + ", F10043 = " + dtCer.Rows[0]["TimpPartial"].ToString() + ", F100973 = " + dtCer.Rows[0]["Norma"].ToString()
                                    + ", F100926 = " + dtCer.Rows[0]["TipNorma"].ToString() + ", F100927 = " + dtCer.Rows[0]["DurataTimpMunca"].ToString() + ", F100928 = " + dtCer.Rows[0]["RepartizareTimpMunca"].ToString()
                                    + ", F100939 = " + dtCer.Rows[0]["IntervalRepartizare"].ToString() + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F100955 = " + data + ", F100964 = " + dtCer.Rows[0]["NrOreLuna"].ToString() + "  WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70421, F70422, F70423, F70424, F70425, F70426, F70427, F70428, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 6, 'Norma Contract', " + data + ", " + dtCer.Rows[0]["Norma"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", " + dtCer.Rows[0]["TipAngajat"].ToString() + ", " + dtCer.Rows[0]["TimpPartial"].ToString() + ", "
                            + dtCer.Rows[0]["Norma"].ToString() + ", " + dtCer.Rows[0]["TipNorma"].ToString() + ", " + dtCer.Rows[0]["DurataTimpMunca"].ToString() + ", " + dtCer.Rows[0]["RepartizareTimpMunca"].ToString()
                            + ", " + dtCer.Rows[0]["IntervalRepartizare"].ToString() + ", " + General.Nz(dtCer.Rows[0]["NrOreLuna"],"0").ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + data2 + ", " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + data4 + ", " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.PrelungireCIM:
                    case (int)Constante.Atribute.PrelungireCIM_Vanz:
                        {
                            //Florin 2019.04.09
                            //se doreste acelasi proces ca si la restul atributelor cu diferenta ca totul se face in WizOne fara a folosi tabela F704
                            //ne folosim de procesul de Inchidere luna din WizOne

                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                //    act = 1;
                                //Radu - se salveaza mai intai in F100 si apoi in F095, deoarece contractul vechi exista deja in F095
                                //                  si apare eroare de violare cheie primara
                                int nrLuni = 0, nrZile = 0;
                                string sql100Tmp = "";
                                DateTime dtTmp = new DateTime();
                                DateTime dtf = new DateTime(2100, 1, 1, 0, 0, 0);
                                if (dtSf != dtf)
                                    dtTmp = dtSf.AddDays(-1);
                                else
                                    dtTmp = dtSf;

                                Personal.Contract ctr = new Personal.Contract();
                                ctr.CalculLuniSiZile(Convert.ToDateTime(dtInc.Date), Convert.ToDateTime(dtSf.Date), out nrLuni, out nrZile);

                                sql100Tmp = "UPDATE F100 SET F100933 = " + data9 + ", F100934 = " + data10 + ", F100936 = " + nrZile.ToString() + ", F100935 = "
                                    + nrLuni.ToString() + ", F100938 = 1, F100993 = " + data10
                                    + ", F10023 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 103)"
                                    : "TO_DATE('" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 'dd/mm/yyyy')") + ", F1009741 = " + dtCer.Rows[0]["DurataContract"].ToString() + "  WHERE F10003 = " + f10003.ToString();
                                General.IncarcaDT(sql100Tmp, null);

                                string sql095 = "INSERT INTO F095 (F09501, F09502, F09503, F09504, F09505, F09506, F09507, F09508, F09509, F09510, F09511, USER_NO, TIME) "
                                    + " VALUES (95, '" + dtF100.Rows[0]["F10017"].ToString() + "', " + dtF100.Rows[0]["F10003"].ToString() + ", '" + dtF100.Rows[0]["F10011"].ToString() + "', " + data9 + ", " + data10
                                    + ", " + nrLuni.ToString() + ", " + nrZile.ToString() + ", " + dtF100.Rows[0]["F100929"].ToString() + ", 1, " + (Convert.ToInt32(dtCer.Rows[0]["DurataContract"].ToString()) == 1 ? "'Nedeterminat'" : "'Determinat'") + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                                General.IncarcaDT(sql095, null);
                            }

                            string sqlTmp = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                                + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", " + Convert.ToInt32(dtCer.Rows[0]["IdAtribut"].ToString()) + ", 'Prelungire CIM', " + data + ", " + dtCer.Rows[0]["MeserieId"].ToString() + ", 'Modificari in avans', '"
                                + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                            General.IncarcaDT(sqlTmp, null);

                        }
                        break;
                    case (int)Constante.Atribute.Organigrama:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10004 = " + dtCer.Rows[0]["SubcompanieId"].ToString() + ", F10005 = " + dtCer.Rows[0]["FilialaId"].ToString() + ", F10006 = "
                                    + dtCer.Rows[0]["SectieId"].ToString() + ", F10007 = " + dtCer.Rows[0]["DeptId"].ToString() + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70414, F70415, F70416, F70417, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 5, 'Organigrama', " + data + ", " + dtCer.Rows[0]["DeptId"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + dtCer.Rows[0]["SubcompanieId"].ToString() + ", " + dtCer.Rows[0]["FilialaId"].ToString() + ", " + dtCer.Rows[0]["SectieId"].ToString()
                            + ", " + dtCer.Rows[0]["DeptId"].ToString() + ", " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', '" + dtCer.Rows[0]["Nume"].ToString() + "', '" + dtCer.Rows[0]["Prenume"].ToString() + "', "
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.BancaSalariu:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F10018 = " + dtCer.Rows[0]["BancaSal"].ToString() + ", F10019 = " + dtCer.Rows[0]["SucursalaSal"].ToString() + ", F10020 = '"
                                    + dtCer.Rows[0]["IBANSal"].ToString() + "', F10055 = '" + dtCer.Rows[0]["NrCard"].ToString() + "' WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70431, F70432, F70433, F70434, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 104, 'Banca - cont salariu', " + data + ", " + dtCer.Rows[0]["BancaSal"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", '" + dtCer.Rows[0]["IBANSal"].ToString() + "', " + dtCer.Rows[0]["BancaSal"].ToString() + ", "
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
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, F70435, F70436, F70437, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 105, 'Banca - cont gar.', " + data + ", " + dtCer.Rows[0]["BancaGar"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", '" + dtCer.Rows[0]["IBANGar"].ToString() + "', " + dtCer.Rows[0]["BancaGar"].ToString() + ", "
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", '" + dtCer.Rows[0]["NivelVorbit"].ToString() + "', " + dtCer.Rows[0]["NrAniVorbit"].ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.DocId:
                        {
                            if (dtModif.Year == dtLucru.Year && dtModif.Month == dtLucru.Month && dtF100 != null && dtF100.Rows.Count > 0)
                            {
                                act = 1;
                                sql100 = "UPDATE F100 SET F100983 = " + dtCer.Rows[0]["TipID"].ToString() + ", F10052 = '" + dtCer.Rows[0]["SerieNrID"].ToString() + "', F100521 = '" + dtCer.Rows[0]["F100521"].ToString()
                                    + "', F100522 = " + data5 + " WHERE F10003 = " + f10003.ToString();
                                if (dtF1001 != null && dtF1001.Rows.Count > 0)
                                    sql1001 = "UPDATE F1001 SET F100963 = " + data6 + " WHERE F10003 = " + f10003.ToString();
                            }
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, \"SerieNrID\", \"IDEmisDe\", \"DataElibID\", \"DataExpID\", USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 107, 'Document identitate', " + data + ", " + dtCer.Rows[0]["TipID"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", '" + dtCer.Rows[0]["SerieNrID"].ToString() + "', '" + dtCer.Rows[0]["IDEmisDe"].ToString() + "', " + data5
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
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", '" + dtCer.Rows[0]["NrPermis"].ToString() + "', '" + dtCer.Rows[0]["PermisEmisDe"].ToString() + "', " + data7
                            + ", " + data8 + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    case (int)Constante.Atribute.BonusTeamLeader:
                        {
                            sql = "INSERT INTO F704 (F70401, F70402, F70403, F70404, F70405, F70406, F70407, F70409, F70410, F70420, USER_NO, TIME) "
                            + " VALUES (704, " + idComp.ToString() + ", " + f10003.ToString() + ", 109, 'Bonus Team Leader', " + data + ", " + dtCer.Rows[0]["BonusTeamLeader"].ToString() + ", 'Modificari in avans', '"
                            + dtCer.Rows[0]["Explicatii"].ToString() + "', " + act.ToString() + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ")";
                        }
                        break;
                    default:
                        return;
                }

                if (sql.Length > 0) General.IncarcaDT(sql, null);
                if (sql100.Length > 0) General.IncarcaDT(sql100, null);
                if (sql1001.Length > 0) General.IncarcaDT(sql1001, null);



                //Florin 2019-04-10
                //procesul acesta s-a mutat din ActeAditionale aici
                //marcam campul Actualizat din Avs_Cereri cand se duce in F100
                if (act == 1)
                    General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""Actualizat""=1 WHERE ""Id""=@1", new object[] { "Id" });

            }
            catch (Exception ex)
            {
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
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

            if (Session["Valoare8Noua"] != null)
            {
                string[] param = Session["Valoare8Noua"].ToString().Split(';');
                cmbStructOrgNou.Value = Convert.ToInt32(param[1]);
                //return;
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
                string dt = e.GetListSourceFieldValue("DataModif").ToString();
                DateTime dataMod = new DateTime(Convert.ToInt32(dt.Substring(6, 4)), Convert.ToInt32(dt.Substring(3, 2)), Convert.ToInt32(dt.Substring(0, 2)));

                SetDataRevisal(2, dataMod, Convert.ToInt32(e.GetListSourceFieldValue("IdAtribut").ToString()), out data);

                e.Value = data;
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
                        ORDER BY A.""Denumire"" ";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;

        }

    }


}