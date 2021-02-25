using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Organigrama
{
    public partial class Lista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable dtMtv = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Org_MotiveModif"" ", null);
                cmbMotiv.DataSource = dtMtv;
                cmbMotiv.DataBind();

                DataTable dtPar = General.IncarcaDT(@"SELECT ""Camp"", ""Denumire"" FROM ""Org_tblParinte"" ", null);
                cmbParinte.DataSource = dtPar;
                cmbParinte.DataBind();
                
                if (!IsPostBack)
                {
                    int idPost = 0;
                    Session["InformatiaCurenta"] = null;

                    if (Session["Filtru_Posturi"] == null)
                    {
                        txtDtVig.Value = DateTime.Now;
                        if (dtPar.Rows.Count > 0) cmbParinte.SelectedIndex = 0;
                    }
                    else
                    {
                        Dictionary<string, object> dic = Session["Filtru_Posturi"] as Dictionary<string, object>;
                        if (dic != null)
                        {
                            txtDtVig.Value = (DateTime?)dic["Ziua"];
                            chkActiv.Value = (bool?)dic["Activ"];
                            cmbParinte.Value = dic["Parinte"];
                            cmbAng.Value = dic["StareAng"];
                            idPost = Convert.ToInt32(dic["IdPost"] ?? 1);
                        }
                        Session["Filtru_Posturi"] = null;
                    }

                    IncarcaGrid();
                       
                    if (grDate.FindNodesByFieldValue("Id", idPost).Count>0)
                    {
                        grDate.ExpandAll();
                        grDate.FindNodesByFieldValue("Id", idPost)[0].Focus();
                    }

                    ////#805  Florin
                    //NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_CereriAbs"] ?? "").ToString());
                    //if (lst.Count > 0)
                    //{
                    //    if (General.Nz(lst["DataSelectie"], "").ToString() != "") txtDtVig.Date = Convert.ToDateTime(lst["DataSelectie"]);
                    //    if (General.Nz(lst["PostActiv"], "").ToString() != "") chkActiv.Value = Convert.ToBoolean(lst["PostActiv"]);
                    //    if (General.Nz(lst["Angajati"], "").ToString() != "") cmbAng.SelectedIndex = Convert.ToInt32(lst["Angajati"]);
                    //    if (General.Nz(lst["Superior"], "").ToString() != "") cmbParinte.SelectedIndex = Convert.ToInt32(lst["Superior"]);

                    //    Session["Filtru_Org"] = "";
                    //}
                }
                else
                {
                    if (Session["InformatiaCurenta"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            //DateTime dt, int tip, int f10003, int posturiActive

            try
            {
                string strSql = "";
                string str = "";
                string strFiltru = "";

                DateTime dt = Convert.ToDateTime(txtDtVig.Value);
                int f10003 = -99;

                switch (General.Nz(cmbAng.Value,"1").ToString())
                {
                    case "1":                         //Activi
                        str = " or x.\"StareAngajat\"=1 ";
                        break;
                    case "2":                         //Suspendati
                        str = " or x.\"StareAngajat\"=2 ";
                        break;
                    case "3":                         //Inactivi
                        str = " or x.\"StareAngajat\"=3 ";
                        break;
                    case "4":                         //Toti
                        str = " or x.\"StareAngajat\" IN (1, 2, 3) ";
                        break;
                }

                if (Convert.ToInt32(General.Nz(chkActiv.Value,0)) == 1) strFiltru = " COALESCE(x.\"Activ\",0) = " + Convert.ToInt32(General.Nz(chkActiv.Value, 0)) + " AND ";

                //Radu 13.12.2016 - am inlocuit  case when a.F10025 = 0 then 1 else 0 end as "Activ" cu case when r."Stare" is null then 0 else r."Stare" end as "Activ" in toate select-urile de mai jos, deoarece trebuie luata starea legaturii dintre angajat si post
                if (Constante.tipBD == 1)
                {
                    if (f10003 == -99)
                    {
                        strSql = @"select * from ( 
                            select a.""IdAuto"", a.""Id"", COALESCE(a.""IdSuperior"",0) AS ""IdSuperior"", COALESCE(a.""IdSuperiorFunctional"",0) AS ""IdSuperiorFunctional"", a.""Denumire"", '' as ""Nume"", '' as ""Prenume"", b.F00204 as ""Companie"", c.F00305 as ""Subcompanie"", d.F00406 as ""Filiala"", e.F00507 as ""Sectie"", f.F00608 as ""Dept"", 
                             '#FFFFFFFF' as ""Culoare"", 1 as ""EstePost"", case when a.""Stare"" is null then 0 else a.""Stare"" end as ""Activ"", null as F10003, 0 as ""StareAngajat"",
                            W.Pozitii AS PozitiiPlanificate,W.PozitiiAprobate,W.Activi AS AngajatiActivi,W.Suspendati AS AngajatiSuspendati,W.Candidati, H.Nr AS AngPost
                             from ""Org_Posturi"" a  
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607
                            OUTER APPLY dbo.DamiPozitii(A.Id, {0}) W
                            LEFT JOIN 
                            (SELECT X.IdPost, COUNT(*) AS Nr
                            FROM Org_relPostAngajat X 
                            INNER JOIN F100 Y ON X.F10003=Y.F10003
                            OUTER APPLY DamiDataPlecare(X.F10003, {0}) Z
                            WHERE X.DataInceput <= {0} AND {0} <= X.DataSfarsit
                            AND (Y.F10022 <= {0} AND {0} <= Z.DataPlecare) AND 
                            NOT (Y.F100922 <= {0} AND {0} <= (CASE WHEN COALESCE(Y.F100924, '2100-01-01') = '2100-01-01' THEN Y.F100923 ELSE Y.F100924 END)) 
                            GROUP BY X.IdPost) H ON A.Id = H.IdPost
                             where CONVERT(date,a.""DataInceput"") <= {0} and {0} <= CONVERT(date,a.""DataSfarsit"") 
                             union 
                             select -1 * r.""IdAuto"" as ""IdAuto"", -1 * a.F10003 as ""Id"", r.""IdPost"" as ""IdSuperior"", r.""IdPost"" as ""IdSuperiorFunctional"", ' ' + a.F10008 + ' ' + a.F10009 as ""Denumire"",  
                             a.F10008 as ""Nume"", a.F10009 as ""Prenume"", b.F00204 as ""Companie"", c.F00305 as ""Subcompanie"", d.F00406 as ""Filiala"", e.F00507 as ""Sectie"", f.F00608 as ""Dept"",  
                             case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then '#ffc8ffc8' else '#ffffffc8' end else '#ffffc8c8' end as ""Culoare"", 
                             0 as ""EstePost"",  case when r.""Stare"" is null then 0 else r.""Stare"" end as ""Activ"", a.F10003 as F10003,  
                            CASE WHEN 
			                (A.F100922 <= {0} AND {0} <= (CASE WHEN COALESCE(A.F100924, '2100-01-01') = '2100-01-01' THEN A.F100923 ELSE A.F100924 END)) 
			                AND 
			                (A.F10022 <= {0} AND {0} <= Z.DataPlecare) 
		                    THEN 2 ELSE
                            CASE WHEN A.F10022 <= {0} AND {0} <= Z.DataPlecare THEN 1 ELSE 3 END END AS ""StareAngajat"", 0, 0, 0, 0, 0, 0  
                             from ""Org_relPostAngajat"" r 
                             inner join F100 a on r.F10003 = a.F10003 
                             inner join ""Org_Posturi"" x on r.""IdPost"" = x.""Id"" 
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607  
                            OUTER APPLY DamiDataPlecare(a.F10003, {0}) Z
                             where CONVERT(date,r.""DataInceput"") <= {0} and {0} <= CONVERT(date,r.""DataSfarsit"") 
                             and CONVERT(date,x.""DataInceput"") <= {0} and {0} <= CONVERT(date,x.""DataSfarsit"") ) x 
                             where {1} (x.""StareAngajat""=0 {2}) 
                             order by x.""IdSuperior"", x.""EstePost"", x.""Denumire""";

                        strSql = string.Format(strSql, General.ToDataUniv(dt), strFiltru, str);

                    }
                    else
                    {
                        strSql = @"WITH Posturi (Id)
                                    AS
                                    (
                                        SELECT Id
                                        FROM Org_Posturi
                                        WHERE Id =  (select TOP 1 IdPost from Org_relPostAngajat where F10003={3} and CONVERT(date,DataInceput) <= {0} and {0} <= CONVERT(date,DataSfarsit)) 
   
                                        UNION ALL
   
                                        SELECT B.Id
                                        FROM Org_Posturi AS B 
                                        JOIN Posturi ON Posturi.Id = B.IdSuperior 
                                        WHERE CONVERT(date,B.DataInceput) <= {0} and {0} <= CONVERT(date,B.DataSfarsit) 
                                    )
                                    select * from (
                                    SELECT a.IdAuto, a.Id, COALESCE(a.IdSuperior,0) AS IdSuperior, COALESCE(a.""IdSuperiorFunctional"",0) AS ""IdSuperiorFunctional"", a.Denumire, '' as Nume, '' as Prenume, b.F00204 as Companie, c.F00305 as Subcompanie, d.F00406 as Filiala, e.F00507 as Sectie, f.F00608 as Dept,  
                                    '#FFFFFFFF' as Culoare, 1 as EstePost, case when a.Stare is null then 0 else a.Stare end as Activ, null as F10003, 0 as StareAngajat,
                                    W.Pozitii AS PosturiPlanificate,W.PozitiiAprobate,W.Activi AS AngajatiActivi,W.Suspendati AS AngajatiSuspendati,W.Candidati
                                    FROM Posturi
                                    INNER JOIN Org_Posturi A ON Posturi.Id = A.Id
                                    LEFT JOIN F002 b on a.F10002=b.F00202  
                                    LEFT JOIN F003 c on a.F10004=c.F00304  
                                    LEFT JOIN F004 d on a.F10005=d.F00405  
                                    LEFT JOIN F005 e on a.F10006=e.F00506  
                                    LEFT JOIN F006 f on a.F10007=f.F00607  
                                    OUTER APPLY dbo.DamiPozitii(A.Id, {0}) W
                                    WHERE CONVERT(date,A.DataInceput) <= {0} and {0} <= CONVERT(date,A.DataSfarsit)   
                         
                                    UNION

                                    SELECT -1 * r.IdAuto as IdAuto, -1 * a.F10003 as Id, r.IdPost as IdSuperior, r.""IdPost"" as ""IdSuperiorFunctional"", ' ' + a.F10008 + ' ' + a.F10009 as Denumire,  
                                    a.F10008 as Nume, a.F10009 as Prenume, b.F00204 as Companie, c.F00305 as Subcompanie, d.F00406 as Filiala, e.F00507 as Sectie, f.F00608 as Dept,  
                                    case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then '#ffc8ffc8' else '#ffffffc8' end else '#ffffc8c8' end as Culoare, 
                                    0 as EstePost,  case when r.Stare is null then 0 else r.Stare end as Activ, a.F10003 as F10003,  
                                    CASE WHEN 
			                        (A.F100922 <= {0} AND {0} <= (CASE WHEN COALESCE(A.F100924, '2100-01-01') = '2100-01-01' THEN A.F100923 ELSE A.F100924 END)) 
			                        AND 
			                        (A.F10022 <= {0} AND {0} <= Z.DataPlecare) 
		                            THEN 2 ELSE
                                    CASE WHEN A.F10022 <= {0} AND {0} <= Z.DataPlecare THEN 1 ELSE 3 END END AS ""StareAngajat"", 0, 0, 0, 0, 0  
                                    FROM Posturi
                                    INNER JOIN Org_relPostAngajat R ON R.IdPost=Posturi.Id
                                    inner join F100 a on R.F10003 = a.F10003 
                                    LEFT JOIN F002 b on a.F10002=b.F00202  
                                    LEFT JOIN F003 c on a.F10004=c.F00304  
                                    LEFT JOIN F004 d on a.F10005=d.F00405  
                                    LEFT JOIN F005 e on a.F10006=e.F00506  
                                    LEFT JOIN F006 f on a.F10007=f.F00607  
                                    OUTER APPLY DamiDataPlecare(a.F10003, {0}) Z
                                    WHERE CONVERT(date,R.DataInceput) <= {0} and {0} <= CONVERT(date,R.DataSfarsit) 
                                    ) x 
                                    WHERE {1} (x.StareAngajat=0 {2}) 
                                    ORDER BY x.IdSuperior, x.EstePost, x.Denumire";


                        strSql = string.Format(strSql, General.ToDataUniv(dt), strFiltru, str, f10003);
                    }
                }
                else
                {
                    if (f10003 == -99)
                    {
                        strSql = @"select * from ( 
                            select a.""IdAuto"", a.""Id"", COALESCE(a.""IdSuperior"",0) AS ""IdSuperior"", COALESCE(a.""IdSuperiorFunctional"",0) AS ""IdSuperiorFunctional"", a.""Denumire"", '' as ""Nume"", '' as ""Prenume"", b.F00204 as ""Companie"", c.F00305 as ""Subcompanie"", d.F00406 as ""Filiala"", e.F00507 as ""Sectie"", f.F00608 as ""Dept"", 
                             '#FFFFFFFF' as ""Culoare"", 1 as ""EstePost"", case when a.""Stare"" is null then 0 else a.""Stare"" end as ""Activ"", null as F10003, 0 as ""StareAngajat"" 
                             from ""Org_Posturi"" a  
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607  
                             where trunc(a.""DataInceput"") <= {0} and {0} <= trunc(a.""DataSfarsit"") 
                             union 
                             select -1 * r.""IdAuto"" as ""IdAuto"", -1 * a.F10003 as ""Id"", r.""IdPost"" as ""IdSuperior"", r.""IdPost"" as ""IdSuperiorFunctional"", ' ' || a.F10008 || ' ' || a.F10009 as ""Denumire"",  
                             a.F10008 as ""Nume"", a.F10009 as ""Prenume"", b.F00204 as ""Companie"", c.F00305 as ""Subcompanie"", d.F00406 as ""Filiala"", e.F00507 as ""Sectie"", f.F00608 as ""Dept"",  
                             case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then '#ffc8ffc8' else '#ffffffc8' end else '#ffffc8c8' end as ""Culoare"", 
                             0 as ""EstePost"",  case when r.""Stare"" is null then 0 else r.""Stare"" end as ""Activ"", a.F10003 as F10003,  
                             case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then 1 else 2 end else 3 end as ""StareAngajat"" 
                             from ""Org_relPostAngajat"" r 
                             inner join F100 a on r.F10003 = a.F10003 
                             inner join ""Org_Posturi"" x on r.""IdPost"" = x.""Id"" 
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607  
                             where trunc(r.""DataInceput"") <= {0} and {0} <= trunc(r.""DataSfarsit"") 
                             and trunc(x.""DataInceput"") <= {0} and {0} <= trunc(x.""DataSfarsit"") ) x 
                             where {1} (x.""StareAngajat""=0 {2}) 
                             order by x.""IdSuperior"", x.""EstePost"", x.""Denumire""";

                        strSql = string.Format(strSql, General.ToDataUniv(dt), strFiltru, str);

                    }
                    else
                    {
                        strSql = @"select * from ( 
                            select a.""IdAuto"", a.""Id"", COALESCE(a.""IdSuperior"",0) AS ""IdSuperior"", COALESCE(a.""IdSuperiorFunctional"",0) AS ""IdSuperiorFunctional"", a.""Denumire"", '' as ""Nume"", '' as ""Prenume"", b.F00204 as ""Companie"", c.F00305 as ""Subcompanie"", d.F00406 as ""Filiala"", e.F00507 as ""Sectie"", f.F00608 as ""Dept"",  
                             '#FFFFFFFF' as ""Culoare"", 1 as ""EstePost"", case when a.""Stare"" is null then 0 else a.""Stare"" end as ""Activ"", null as F10003, 0 as ""StareAngajat"" 
                             from ""Org_Posturi"" a  
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607  
                             LEFT JOIN ""Org_Posturi"" t2 ON t2.""Id"" = a.""IdSuperior""  
                             where trunc(a.""DataInceput"") <= {0} and {0} <= trunc(a.""DataSfarsit"") 
                             START WITH a.""Id"" = (select ""IdPost"" from ""Org_relPostAngajat"" where F10003={3} and Rownum=1 and trunc(""DataInceput"") <= {0} and {0} <= trunc(""DataSfarsit"")) 
                             CONNECT BY PRIOR a.""Id"" = a.""IdSuperior"" 
                             union 
                             select -1 * r.""IdAuto"" as ""IdAuto"", -1 * a.F10003 as ""Id"", r.""IdPost"" as ""IdSuperior"", r.""IdPost"" as ""IdSuperiorFunctional"", ' ' || a.F10008 || ' ' || a.F10009 as ""Denumire"",  
                             a.F10008 as ""Nume"", a.F10009 as ""Prenume"", b.F00204 as ""Companie"", c.F00305 as ""Subcompanie"", d.F00406 as ""Filiala"", e.F00507 as ""Sectie"", f.F00608 as ""Dept"",  
                             case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then '#ffc8ffc8' else '#ffffffc8' end else '#ffffc8c8' end as ""Culoare"", 
                             0 as ""EstePost"",  case when r.""Stare"" is null then 0 else r.""Stare"" end as ""Activ"", a.F10003 as F10003,  
                             case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then 1 else 2 end else 3 end as ""StareAngajat"" 
                             from ""Org_relPostAngajat"" r 
                             inner join F100 a on r.F10003 = a.F10003 
                             inner join ""Org_Posturi"" x on r.""IdPost"" = x.""Id"" 
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607  
                             LEFT JOIN ""Org_Posturi"" t2 ON t2.""Id"" = x.""IdSuperior""  
                             where trunc(r.""DataInceput"") <= {0} and {0} <= trunc(r.""DataSfarsit"") 
                             and trunc(x.""DataInceput"") <= {0} and {0} <= trunc(x.""DataSfarsit"") 
                             START WITH x.""Id"" = (select ""IdPost"" from ""Org_relPostAngajat"" where F10003={3} and Rownum=1 and trunc(""DataInceput"") <= {0} and {0} <= trunc(""DataSfarsit""))  
                             CONNECT BY PRIOR x.""Id"" = x.""IdSuperior"") x 
                             where {1} (x.""StareAngajat""=0 {2}) 
                             order by x.""IdSuperior"", x.""EstePost"", x.""Denumire"" ";

                        strSql = string.Format(strSql, General.ToDataUniv(dt), strFiltru, str, f10003);
                    }
                }

                DataTable dtPst = General.IncarcaDT(strSql, null);
                Session["InformatiaCurenta"] = dtPst;
                grDate.DataSource = dtPst;
                grDate.ParentFieldName = General.Nz(cmbParinte.Value, "IdSuperior").ToString();
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

        protected void btnNou_Click(object sender, EventArgs e)
        {
            try
            {
                RetineFiltru();
                Session["DataVigoare"] = txtDtVig.Value;
                Session["IdAuto"] = -97;
                Response.Redirect("~/Organigrama/Posturi", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnModif_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(General.Nz(grDate.FocusedNode.Key, 0)) > 0)
                {
                    RetineFiltru();
                    Session["DataVigoare"] = txtDtVig.Value;
                    Session["IdAuto"] = grDate.FocusedNode.GetValue("IdAuto");
                    Response.Redirect("~/Organigrama/Posturi", false);
                }
                else
                    MessageBox.Show("Nu s-a selectat nici un post", MessageBox.icoInfo, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDuplicare_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(General.Nz(grDate.FocusedNode.Key, 0)) > 0)
                {
                    RetineFiltru();
                    Session["DataVigoare"] = txtDtVig.Value;
                    Session["IdAuto"] = grDate.FocusedNode.GetValue("IdAuto");
                    Session["Org_Duplicare"] = "1"; 
                    Response.Redirect("~/Organigrama/Posturi", false);
                }
                else
                    MessageBox.Show("Nu s-a selectat nici un post", MessageBox.icoInfo, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void ModifStruc()
        {
            try
            {
                if (!hf.Contains("Nod") || !hf.Contains("Target") || cmbMotiv.SelectedIndex == -1) return;

                int target_idAuto = Convert.ToInt32(General.Nz(hf["Target"], -99));
                int nod_idAuto = Convert.ToInt32(General.Nz(hf["Nod"], -99));

                if (!chkStruc.Checked)
                {
                    //modificare de superior
                    General.ExecutaNonQuery($@"
                    IF ({ General.ToDataUniv(txtDtVig.Date) } = (SELECT CONVERT(date,DataInceput) FROM Org_Posturi WHERE IdAuto={ nod_idAuto }))
	                    BEGIN
		                    UPDATE Org_Posturi SET IdSuperior=(SELECT Id FROM Org_Posturi WHERE IdAuto={ target_idAuto }) WHERE IdAuto={ nod_idAuto }
	                    END
                    ELSE
	                    BEGIN
		                    INSERT INTO Org_Posturi(Id, Denumire, DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN, DataInceput, DataSfarsit, Stare, F10002, F10004, F10005, F10006, F10007, IdSuperior, NivelIerarhic, PlanHC, IdResponsabilitate, IdTipPost, CodBuget, IdLocatie, IdMotivModif, IdFamFR, CodCOR, FunctieCOR, IdSchema, IdFamGAM, Atribute, Criterii, ObservatiiModif, NivelHay, SalariuMin,  SalariuMed,  SalariuMax,  IdSuperiorFunctional, TIME, USER_NO) 
		                    SELECT Id, Denumire, DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN, CONVERT(date,{ General.ToDataUniv(Convert.ToDateTime(txtDtVig.Value)) }) AS DataInceput, DataSfarsit, 1 AS Stare, F10002, F10004, F10005, F10006, F10007, (SELECT Id FROM Org_Posturi WHERE IdAuto={ target_idAuto }) AS IdSuperior, NivelIerarhic, PlanHC, IdResponsabilitate, IdTipPost, CodBuget, IdLocatie, IdMotivModif, IdFamFR, CodCOR, FunctieCOR, IdSchema, IdFamGAM, Atribute, Criterii, ObservatiiModif, NivelHay, SalariuMin,  SalariuMed,  SalariuMax,  IdSuperiorFunctional, GETDATE(), { Session["UserId"] } AS USER_NO 
		                    FROM Org_Posturi WHERE IdAuto={ nod_idAuto };

		                    UPDATE Org_Posturi SET DataSfarsit= DATEADD(d, -1, CONVERT(date,{ General.ToDataUniv(Convert.ToDateTime(txtDtVig.Value)) })) WHERE IdAuto={ nod_idAuto };
	                    END", null);
                }
                else
                {
                    //daca se doreste si modificare de structura
                    int nod = -99;
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Org_Posturi"" WHERE ""IdAuto""=@1", new object[] { nod_idAuto });
                    if (dt.Rows.Count == 0) return;
                    nod = Convert.ToInt32(General.Nz(dt.Rows[0]["Id"],-99));
                    if (nod == -99) return;

                    //string strSql = "";
                    string arrId = "," + nod;
                    string arrIdAuto = "," + nod_idAuto;

                    //pentru nodul parinte
                    ExecutaModif(target_idAuto, nod_idAuto, nod, Convert.ToDateTime(dt.Rows[0]["DataInceput"]), true);

                    //cautam nodurile copii afectate de modificare
                    if (arrId != "")
                    {
                        do
                        {
                            var po = General.IncarcaDT($@"SELECT ""Id"", ""IdAuto"", ""DataInceput"" FROM ""Org_Posturi"" WHERE ""DataInceput"" <= { General.ToDataUniv(txtDtVig.Date) } AND { General.ToDataUniv(txtDtVig.Date) } <= ""DataSfarsit"" AND ""IdSuperior"" IN ({ arrId.Substring(1) })", null);
                            arrId = "";
                            for (int i = 0; i < po.Rows.Count; i++)
                            {
                                DataRow dr = po.Rows[i];
                                ExecutaModif(target_idAuto, Convert.ToInt32(dr["IdAuto"]), Convert.ToInt32(dr["Id"]), Convert.ToDateTime(dr["DataInceput"]));
                                arrId += "," + po.Rows[i]["Id"];
                            }
                        } while (arrId != "");
                    }
                }

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void ExecutaModif(int target_idAuto, int idAuto, int id, DateTime dtInc, bool adaugaSuperior = false)
        {
            try
            {
                string strSql = "";
                string sqlSup = $@"(SELECT ""Id"" FROM ""Org_Posturi"" WHERE ""IdAuto""={ target_idAuto })";
                if (!adaugaSuperior) sqlSup = "";
                if (Convert.ToDateTime(dtInc).Date == Convert.ToDateTime(txtDtVig.Date).Date)
                {
                    strSql = $@"UPDATE ""Org_Posturi"" SET 
                                    ""IdMotivModif""={cmbMotiv.Value},
                                    {(adaugaSuperior ? @" ""IdSuperior""=" + sqlSup + "," : "")}
                                    F10002 = (SELECT F10002 FROM ""Org_Posturi"" WHERE ""IdAuto""={target_idAuto}), 
                                    F10004 = (SELECT F10004 FROM ""Org_Posturi"" WHERE ""IdAuto""={target_idAuto}),
                                    F10005 = (SELECT F10005 FROM ""Org_Posturi"" WHERE ""IdAuto""={target_idAuto}),
                                    F10006 = (SELECT F10006 FROM ""Org_Posturi"" WHERE ""IdAuto""={target_idAuto}),
                                    F10007 = (SELECT F10007 FROM ""Org_Posturi"" WHERE ""IdAuto""={target_idAuto}) 
                                    WHERE ""IdAuto""={idAuto}";
                }
                else
                {
                    strSql = $@"BEGIN
                                INSERT INTO ""Org_Posturi""(
                                ""Id"", ""Denumire"", DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN, ""DataInceput"", ""DataSfarsit"", F10002, F10004, F10005, F10006, F10007, 
                                ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
                                ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                                ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", ""IdMotivModif"", USER_NO, TIME)
                                SELECT ""Id"", ""Denumire"", DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN,{General.ToDataUniv(txtDtVig.Date)}, ""DataSfarsit"", 
                                (SELECT F10002 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10002, 
                                (SELECT F10004 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10004,
                                (SELECT F10005 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10005,
                                (SELECT F10006 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10006,
                                (SELECT F10007 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10007,
                                ""Stare"", {(adaugaSuperior ? sqlSup : @" ""IdSuperior"" ")}, ""IdSuperiorFunctional"", 
                                ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                                ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", {cmbMotiv.Value}, {Session["UserId"]}, GetDate()
                                FROM ""Org_Posturi"" WHERE ""IdAuto""={idAuto};
                                UPDATE ""Org_Posturi"" SET ""DataSfarsit""={General.ToDataUniv(txtDtVig.Date.AddDays(-1))} WHERE ""IdAuto""={idAuto};
                            END;";
                }

                General.ExecutaNonQuery(strSql, null);

                General.SchimbaInPlanificat(Convert.ToDateTime(txtDtVig.Date).Date, id, 1, 0, 0, 0);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlRowPrepared(object sender, DevExpress.Web.ASPxTreeList.TreeListHtmlRowEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(General.Nz(e.GetValue("Id"), 1)) < 0)
                {
                    switch (Convert.ToInt32(General.Nz(e.GetValue("StareAngajat"), 1)))
                    {
                        case 1:
                            e.Row.BackColor = System.Drawing.Color.FromArgb(200, 255, 200);
                            break;
                        case 2:
                            e.Row.BackColor = System.Drawing.Color.FromArgb(255, 255, 200);
                            break;
                        case 3:
                            e.Row.BackColor = System.Drawing.Color.FromArgb(255, 200, 200);
                            break;
                        default:
                            e.Row.BackColor = System.Drawing.Color.FromArgb(200, 255, 200);
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

        protected void btnOkModif_Click(object sender, EventArgs e)
        {
            try
            {
                ModifStruc();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, DevExpress.Web.ASPxTreeList.TreeListCustomCallbackEventArgs e)
        {
            try
            {
                RetineFiltru();
                grDate.JSProperties["cpReportUrl"] = ResolveClientUrl("~/Organigrama/Diagrama");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void RetineFiltru()
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                dic.Add("Ziua", Convert.ToDateTime(txtDtVig.Value));
                dic.Add("Activ", chkActiv.Value);
                dic.Add("Parinte", cmbParinte.Value);
                dic.Add("StareAng", cmbAng.Value);

                if (Convert.ToInt32(General.Nz(grDate.FocusedNode.Key, 0)) > 0)
                    dic.Add("IdPost", grDate.FocusedNode.GetValue("Id"));
                else
                    dic.Add("IdPost", 1);

                Session["Filtru_Posturi"] = dic;

                //string req = "";
                //if (txtDtVig.Date != null) req += "&DataSelectie=" + txtDtVig.Date;
                //if (chkActiv.Value != null) req += "&PostActiv=" + chkActiv.Value;
                //req += "&Angajati=" + cmbAng.SelectedIndex;
                //req += "&Superior=" + cmbParinte.Value;

                //Session["Filtru_Org"] = req;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}