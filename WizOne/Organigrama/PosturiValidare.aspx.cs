using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Organigrama
{
    public partial class PosturiValidare : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnValidare.Text = Dami.TraduCuvant("btnValidare", "Validare");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();



                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;

                    string sqlAng = 
                    $@"SELECT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", A.F10017 AS CNP, A.F10022 AS ""DataAngajarii"", A.F10011 AS ""NrContract""
                    FROM F100 A
                    INNER JOIN ""F100Supervizori"" B ON A.F10003 = B.F10003 
                    WHERE A.F10003={Session["User_Marca"]} OR B.""IdUser""={Session["UserId"]}";

                    DataTable dtAng = General.IncarcaDT(sqlAng, null);
                    cmbAng.DataSource = dtAng;
                    cmbAng.DataBind();

                    string sqlPost = $@"SELECT COALESCE(A.""Id"",1) AS ""Id"", A.""Denumire"", A.""NivelIerarhic"", B.F00204 AS ""Companie"", C.F00305 AS ""Subcompanie"", D.F00406 as ""Filiala"", E.F00507 AS ""Sectie"", F.F00608 AS ""Departament""
                        FROM ""Org_Posturi"" A
                        LEFT JOIN F002 B ON A.F10002 = B.F00202
                        LEFT JOIN F003 C ON A.F10004 = C.F00304
                        LEFT JOIN F004 D ON A.F10005 = D.F00405
                        LEFT JOIN F005 E ON A.F10006 = E.F00506
                        LEFT JOIN F006 F ON A.F10007 = F.F00607
                        WHERE {General.TruncateDate("A.DataInceput")} <= {General.CurrentDate()} AND {General.CurrentDate()} <= {General.TruncateDate("A.DataSfarsit")}  ";

                    DataTable dtPost = General.IncarcaDT(sqlPost, null);
                    cmbSup.DataSource = dtPost;
                    cmbSup.DataBind();
                }
                else
                {
                    foreach (var c in grDate.Columns)
                    {
                        try
                        {
                            GridViewDataColumn col = (GridViewDataColumn)c;
                            col.Caption = Dami.TraduCuvant(col.FieldName);
                        }
                        catch (Exception) { }
                    }

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnValidare_Click(object sender, EventArgs e)
        {
            try
            {
                string ids = ",1,2";
                int nr = 0;

                //string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (ids == "") return;

                DateTime dtLuc = DateTime.Now;
                DataRow drLuc = General.IncarcaDR("SELECT * FROM F010", null);
                if (drLuc.ToString() != "" && drLuc != null && drLuc["F01011"] != null && drLuc["F01012"] != null) dtLuc = new DateTime(Convert.ToInt32(drLuc["F01011"]), Convert.ToInt32(drLuc["F01012"]), 1);

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Org_relPostAngajat"" WHERE ""IdAuto"" IN ({ids.Substring(1)})", null);
                for(int i =0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        DataRow dr = dt.Rows[i];

                        DateTime dtRef = Convert.ToDateTime(dr["DataReferinta"]).Date;
                        if (dtRef >= dtLuc || (dtRef.Year == dtLuc.Year && dtRef.Month == dtLuc.Month))
                        {
                            if ((dtRef.Year == dtLuc.Year && dtRef.Month == dtLuc.Month))
                                TrimiteInF704(Convert.ToInt32(General.Nz(dr["IdAuto"], -99)), dtRef, true);
                            else
                                TrimiteInF704(Convert.ToInt32(General.Nz(dr["IdAuto"], -99)), dtRef, false);
                        }

                        dr["Stare"] = 1;
                        dr["DataReferinta"] = null;
                        dr["modifCOR"] = 0;
                        dr["modifFunctie"] = 0;
                        dr["modifSalariu"] = 0;
                        dr["modifStructura"] = 0;
                        dr["IdPostVechi"] = null;
                        dr["SalariulAvizat"] = null;

                        nr++;
                    }
                    catch (Exception){}
                }


                if (nr != 0) General.SalveazaDate(dt, "Org_relPostAngajat");

                MessageBox.Show("S-au validat " + nr.ToString() + " inregistrari", MessageBox.icoInfo, "Validare Posturi");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string strJoin = "";
                string strFiltru = "";

                if (General.Nz(Session["EsteAdmin"],0).ToString() != "1")
                {
                    strJoin = "inner join \"F100Supervizori\" d on a.F10003 = d.F10003 ";
                    strFiltru = "and d.\"IdUser\" = " + Session["UserId"] + " and (d.\"IdSuper\" = 3 or d.\"IdSuper\" = 12) ";
                }

                string strSql = $@"select a.""IdAuto"", a.F10003, b.F10008 {Dami.Operator()} ' ' {Dami.Operator()} b.F10009 as ""NumeComplet"", c.""Id"" as ""IdPost"", c.""Denumire"" as ""NumePost"", a.""DataReferinta"", k.""Denumire"" as ""Locatie"",  
                    a.""modifCOR"", a.""modifSalariu"", a.""modifFunctie"", a.""modifStructura"" 
                    from ""Org_relPostAngajat"" a 
                    inner join F100 b on a.F10003 = b.F10003 
                    inner join ""Org_Posturi"" c on a.""IdPost""=c.""Id"" and c.""DataInceput""  <= a.""DataReferinta"" and a.""DataReferinta"" <= c.""DataSfarsit"" 
                    left join ""Org_tblLocatii"" k on c.""IdLocatie"" = k.""Id"" 
                    {strJoin}
                    where a.""Stare"" = 2 and a.""DataReferinta"" is not null 
                    {strFiltru}
                    group by a.""IdAuto"", a.F10003, b.F10008, b.F10009, c.""Id"", c.""Denumire"", a.""DataReferinta"", k.""Denumire"", a.""modifCOR"", a.""modifSalariu"", a.""modifFunctie"", a.""modifStructura"" ";

                DataTable dt = General.IncarcaDT(strSql, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                Session["InformatiaCurenta"] = dt;

                grDate.DataSource = Session["InformatiaCurenta"];
                grDate.KeyFieldName = "IdAuto";
                grDate.DataBind();
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
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            try
            {
                switch(e.ButtonID)
                {
                    case "btnPrint":
                        {
                            //General.Imprima("PostModificat", "idAuto=" + ent.Id);
                        }
                        break;
                    case "btnDelete":
                        {
                            object[] arr = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAuto" }) as object[];
                            if (arr != null && arr.Count() > 0)
                            {
                                General.ExecutaNonQuery(
                                    @"UPDATE ""Org_relPostAngajat"" 
                                    SET ""Stare""=1, ""DataReferinta""=null, 
                                    ""modifCOR""=null, ""modifSalariu""=null, ""modifFunctie""=null, ""modifStructura""=null 
                                    WHERE ""IdAuto""={arr[0]}", null);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void TrimiteInF704(int idAuto, DateTime dtRef, bool addF100)
        {
            try
            {
                DataTable dtRel = General.IncarcaDT(@"SELECT * FROM ""Org_relPostAngajat"" WHERE ""IdAuto""=@1", new object[] { idAuto });
                if (dtRel == null || dtRel.Rows.Count == 0) return;

                DataTable dtPost = General.IncarcaDT($@"SELECT * FROM ""Org_Posturi"" WHERE ""IdPost""={dtRel.Rows[0]["IdPost"]} AND ""DataInceput"" <= {General.ToDataUniv(dtRef)} AND {General.ToDataUniv(dtRef)} <= ""DataSfarsit"" ", null);
                if (dtPost == null || dtPost.Rows.Count == 0) return;

                int idComp = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT MIN(F00202) FROM F002", null), 1));

                DataTable dtF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003=@1", new object[] { dtRel.Rows[0]["F10003"] });
                DataTable dtDen = General.IncarcaDT("SELECT * FROM F718 WHERE F71804=@1 ", new object[] { dtPost.Rows[0]["Denumire"] });

                DataTable dt = General.IncarcaDT(@"SELECT TOP 0 * FROM ""Org_relPostAngajat"" ", null);

                if (General.Nz(dtRel.Rows[0]["modifCOR"],0).ToString() == "1")
                {
                    DataRow dr = dt.NewRow();
                    dr["F70401"] = 704;
                    dr["F70402"] = idComp;
                    dr["F70403"] = dtRel.Rows[0]["F10003"];
                    dr["F70404"] = 3;
                    dr["F70405"] = "Cod COR";
                    dr["F70406"] = dtRef;
                    dr["F70407"] = dtPost.Rows[0]["CodCOR"];
                    dr["F70409"] = "Referat";
                    dr["F70410"] = "ModifPost";
                    if (addF100)
                        dr["F70420"] = 1;
                    else
                        dr["F70420"] = 0;
                    dr["USER_NO"] = -9;
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }

                if (General.Nz(dtRel.Rows[0]["modifSalariu"], 0).ToString() == "1")
                {
                    DataRow dr = dt.NewRow();
                    dr["F70401"] = 704;
                    dr["F70402"] = idComp;
                    dr["F70403"] = dtRel.Rows[0]["F10003"];
                    dr["F70404"] = 1;
                    dr["F70405"] = "Salariul Tarifar";
                    dr["F70406"] = dtRef;
                    dr["F70407"] = dtRel.Rows[0]["SalariulAvizat"];
                    dr["F70409"] = "Referat";
                    dr["F70410"] = "ModifPost";
                    if (addF100)
                        dr["F70420"] = 1;
                    else
                        dr["F70420"] = 0;
                    dr["USER_NO"] = -9;
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }

                //cautam id-ul din nomenclatorul de functii din WizSalary
                if (General.Nz(dtRel.Rows[0]["modifFunctie"], 0).ToString() == "1" && dtDen != null && dtDen.Rows.Count > 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["F70401"] = 704;
                    dr["F70402"] = idComp;
                    dr["F70403"] = dtRel.Rows[0]["F10003"];
                    dr["F70404"] = 2;
                    dr["F70405"] = "Functie";
                    dr["F70406"] = dtRef;
                    dr["F70407"] = dtDen.Rows[0]["F71802"];
                    dr["F70409"] = "Referat";
                    dr["F70410"] = "ModifPost";
                    if (addF100)
                        dr["F70420"] = 1;
                    else
                        dr["F70420"] = 0;
                    dr["USER_NO"] = -9;
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }


                if (General.Nz(dtRel.Rows[0]["modifStructura"], 0).ToString() == "1")
                {
                    DataRow dr = dt.NewRow();
                    dr["F70401"] = 704;
                    dr["F70402"] = idComp;
                    dr["F70403"] = dtRel.Rows[0]["F10003"];
                    dr["F70404"] = 5;
                    dr["F70405"] = "Organigrama";
                    dr["F70406"] = dtRef;
                    dr["F70409"] = "Referat";
                    dr["F70410"] = "ModifPost";
                    dr["F70414"] = dtPost.Rows[0]["F10004"];
                    dr["F70415"] = dtPost.Rows[0]["F10005"];
                    dr["F70416"] = dtPost.Rows[0]["F10006"];
                    dr["F70417"] = dtPost.Rows[0]["F10007"];
                    if (addF100)
                        dr["F70420"] = 1;
                    else
                        dr["F70420"] = 0;
                    dr["USER_NO"] = -9;
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }

                if (addF100)
                {
                    if (dtF100 != null && dtF100.Rows.Count > 0)
                    {
                        if (General.Nz(dtRel.Rows[0]["modifSalariu"], 0).ToString() == "1")
                        {
                            //Radu 05.12.2019
                            string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                            dtF100.Rows[0][salariu] = dtRel.Rows[0]["SalariulAvizat"];
                            dtF100.Rows[0]["F100991"] = dtRef;
                        }

                        if (General.Nz(dtRel.Rows[0]["modifCOR"], 0).ToString() == "1")
                        {
                            dtF100.Rows[0]["F10098"] = dtPost.Rows[0]["CodCOR"];
                            General.ExecutaNonQuery($@"UPDATE F1001 SET F100956={General.ToDataUniv(dtRef)} WHERE F10003=" + dtF100.Rows[0]["F10003"], null);
                        }

                        if (General.Nz(dtRel.Rows[0]["modifFunctie"], 0).ToString() == "1" && dtDen != null && dtDen.Rows.Count > 0)
                        {
                            dtF100.Rows[0]["F10071"] = dtDen.Rows[0]["F71802"];
                            dtF100.Rows[0]["F100992"] = dtRef;
                        }

                        if (General.Nz(dtRel.Rows[0]["modifStructura"], 0).ToString() == "1")
                        {
                            dtF100.Rows[0]["F10002"] = (short)idComp;
                            dtF100.Rows[0]["F10004"] = dtPost.Rows[0]["F10004"];
                            dtF100.Rows[0]["F10005"] = dtPost.Rows[0]["F10005"];
                            dtF100.Rows[0]["F10006"] = dtPost.Rows[0]["F10006"];
                            dtF100.Rows[0]["F10007"] = dtPost.Rows[0]["F10007"];
                            dtF100.Rows[0]["F100910"] = dtRef;
                        }
                    }
                }

                General.SalveazaDate(dt, "F704");
                General.SalveazaDate(dtF100, "F100");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}