using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data;
using System.IO;
using System.Diagnostics;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Internal;
using DevExpress.Spreadsheet;
using System.Web.Hosting;

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
                    if (dtPar.Rows.Count > 0) cmbParinte.SelectedIndex = 0;

                    txtDtVig.Value = DateTime.Now;
                    Session["InformatiaCurenta"] = null;
                    IncarcaGrid();
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
                             '#FFFFFFFF' as ""Culoare"", 1 as ""EstePost"", case when a.""Stare"" is null then 0 else a.""Stare"" end as ""Activ"", null as F10003, 0 as ""StareAngajat"" 
                             from ""Org_Posturi"" a  
                             LEFT JOIN F002 b on a.""F10002""=b.F00202  
                             LEFT JOIN F003 c on a.""F10004""=c.F00304  
                             LEFT JOIN F004 d on a.""F10005""=d.F00405  
                             LEFT JOIN F005 e on a.""F10006""=e.F00506  
                             LEFT JOIN F006 f on a.""F10007""=f.F00607  
                             where CONVERT(date,a.""DataInceput"") <= {0} and {0} <= CONVERT(date,a.""DataSfarsit"") 
                             union 
                             select -1 * r.""IdAuto"" as ""IdAuto"", -1 * a.F10003 as ""Id"", r.""IdPost"" as ""IdSuperior"", r.""IdPost"" as ""IdSuperiorFunctional"", ' ' + a.F10008 + ' ' + a.F10009 as ""Denumire"",  
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
                                    '#FFFFFFFF' as Culoare, 1 as EstePost, case when a.Stare is null then 0 else a.Stare end as Activ, null as F10003, 0 as StareAngajat 
                                    FROM Posturi
                                    INNER JOIN Org_Posturi A ON Posturi.Id = A.Id
                                    LEFT JOIN F002 b on a.F10002=b.F00202  
                                    LEFT JOIN F003 c on a.F10004=c.F00304  
                                    LEFT JOIN F004 d on a.F10005=d.F00405  
                                    LEFT JOIN F005 e on a.F10006=e.F00506  
                                    LEFT JOIN F006 f on a.F10007=f.F00607  
                                    WHERE CONVERT(date,A.DataInceput) <= {0} and {0} <= CONVERT(date,A.DataSfarsit)   
                         
                                    UNION

                                    SELECT -1 * r.IdAuto as IdAuto, -1 * a.F10003 as Id, r.IdPost as IdSuperior, r.""IdPost"" as ""IdSuperiorFunctional"", ' ' + a.F10008 + ' ' + a.F10009 as Denumire,  
                                    a.F10008 as Nume, a.F10009 as Prenume, b.F00204 as Companie, c.F00305 as Subcompanie, d.F00406 as Filiala, e.F00507 as Sectie, f.F00608 as Dept,  
                                    case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then '#ffc8ffc8' else '#ffffffc8' end else '#ffffc8c8' end as Culoare, 
                                    0 as EstePost,  case when r.Stare is null then 0 else r.Stare end as Activ, a.F10003 as F10003,  
                                    case when (a.F10025 = 0 or a.F10025 = 999) then case when a.F100925 = 0 then 1 else 2 end else 3 end as StareAngajat 
                                    FROM Posturi
                                    INNER JOIN Org_relPostAngajat R ON R.IdPost=Posturi.Id
                                    inner join F100 a on R.F10003 = a.F10003 
                                    LEFT JOIN F002 b on a.F10002=b.F00202  
                                    LEFT JOIN F003 c on a.F10004=c.F00304  
                                    LEFT JOIN F004 d on a.F10005=d.F00405  
                                    LEFT JOIN F005 e on a.F10006=e.F00506  
                                    LEFT JOIN F006 f on a.F10007=f.F00607  
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                CreazaExcel(txtDtVig.Date, Convert.ToInt32(General.Nz(txtNivel.Value,1)), Convert.ToInt32(grDate.FocusedNode.GetValue("Id")), 1);
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

        //protected void btnModif_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!hf.Contains("Nod") || !hf.Contains("Target") || cmbMotiv.SelectedIndex == -1) return;

        //        int target_idAuto = Convert.ToInt32(General.Nz(hf.Contains("Target"),-99));
        //        int nod_idAuto = Convert.ToInt32(General.Nz(hf.Contains("Nod"),-99));

        //        //modificare de superior
        //        General.ExecutaNonQuery($@"
        //            IF ({ General.ToDataUniv(txtDtVig.Date) } = (SELECT CONVERT(date,DataInceput) FROM Org_Posturi WHERE IdAuto={ nod_idAuto }))
        //             BEGIN
        //              UPDATE Org_Posturi SET IdSuperior=(SELECT Id FROM Org_Posturi WHERE Id={ target_idAuto }) WHERE IdAuto={ nod_idAuto }
        //             END
        //            ELSE
        //             BEGIN
        //              INSERT INTO Org_Posturi(Id, Denumire, DataInceput, DataSfarsit, Stare, F10002, F10004, F10005, F10006, F10007, IdSuperior, NivelIerarhic, PlanHC, IdResponsabilitate, IdTipPost, CodBuget, IdLocatie, IdMotivModif, IdFamFR, CodCOR, FunctieCOR, IdSchema, IdFamGAM, Atribute, Criterii, ObservatiiModif, NivelHay, SalariuMin,  SalariuMed,  SalariuMax,  IdSuperiorFunctional, IdBeneficiu1, IdBeneficiu2, IdBeneficiu3, IdBeneficiu4, IdBeneficiu5, IdBeneficiu6, IdBeneficiu7, IdBeneficiu8, IdBeneficiu9, IdBeneficiu10, TIME, USER_NO) 
        //              SELECT Id, Denumire, CONVERT(date,{ General.ToDataUniv(Convert.ToDateTime(txtDtVig.Value)) }) AS DataInceput, DataSfarsit, 1 AS Stare, F10002, F10004, F10005, F10006, F10007, (SELECT Id FROM Org_Posturi WHERE Id={ target_idAuto }) AS IdSuperior, NivelIerarhic, PlanHC, IdResponsabilitate, IdTipPost, CodBuget, IdLocatie, IdMotivModif, IdFamFR, CodCOR, FunctieCOR, IdSchema, IdFamGAM, Atribute, Criterii, ObservatiiModif, NivelHay, SalariuMin,  SalariuMed,  SalariuMax,  IdSuperiorFunctional, IdBeneficiu1, IdBeneficiu2, IdBeneficiu3, IdBeneficiu4, IdBeneficiu5, IdBeneficiu6, IdBeneficiu7, IdBeneficiu8, IdBeneficiu9, IdBeneficiu10, GETDATE(), { Session["UserId"] } AS USER_NO 
        //              FROM Org_Posturi WHERE IdAuto={ nod_idAuto }

        //              UPDATE Org_Posturi SET DataSfarsit= DATEADD(d, -1, CONVERT(date,{ General.ToDataUniv(Convert.ToDateTime(txtDtVig.Value)) })) WHERE IdAuto={ nod_idAuto }
        //             END", null);

        //        if (chkStruc.Checked)
        //        {
        //            //daca se doreste si modificare de structura
        //            int nod = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT Id FROM Org_Posturi WHERE IdAuto=@1", new object[] { nod_idAuto }),-99));
        //            if (nod == -99) return;

        //            string arrId = "," + nod;
        //            string arrIdAuto = "," + nod_idAuto;

        //            //cautam nodurile afectate de modificare
        //            if (arrId != "")
        //            {
        //                do
        //                {
        //                    arrId = "";
        //                    //var po = this.ObjectContext.Org_Posturi.Where(p => EntityFunctions.TruncateTime(p.DataInceput) <= dtRef.Date && dtRef.Date <= EntityFunctions.TruncateTime(p.DataSfarsit) && lstTmp.Contains((int)p.IdSuperior));
        //                    var po = General.IncarcaDT($@"SELECT ""Id"", ""IdAuto"" FROM ""Org_Posturi"" WHERE ""DataInceput"" <= { General.ToDataUniv(txtDtVig.Date) } AND { General.ToDataUniv(txtDtVig.Date) } <= ""DataSfarsit"" AND ""IdSuperior"" IN ({ arrId.Substring(1) })", null);
        //                    for(int i = 0; i < po.Rows.Count; i++)
        //                    {
        //                        arrIdAuto += "," + po.Rows[i]["IdAuto"];
        //                        arrId += "," + po.Rows[i]["Id"];
        //                    }
        //                } while (arrId != "");
        //            }


        //            string strSql = "";
        //            //pt postul selctionat si pt toti copii lui schimbam structura
        //            DataRow drDes = General.IncarcaDR("SELECT * FROM Org_Posturi WHERE IdAuto=@1", new object[] { target_idAuto });
        //            DataTable dtChi = General.IncarcaDT("SELECT * FROM Org_Posturi WHERE IdAuto IN (" + arrIdAuto.Substring(1) + ")", null);
        //            for (int i = 0; i < dtChi.Rows.Count; i++)
        //            {
        //                DataRow drOri = dtChi.Rows[i];
        //                i += 1;

        //                int modifStruc = 0;
        //                if (drOri["F10002"] != drDes["F10002"] || drOri["F10004"] != drDes["F10004"] || drOri["F10005"] != drDes["F10005"] || drOri["F10006"] != drDes["F10006"] || drOri["F10007"] != drDes["F10007"]) modifStruc = 1;

        //                if (Convert.ToDateTime(drDes["DataInceput"]).Date == txtDtVig.Date)
        //                {
        //                    strSql += $@"UPDATE Org_Posturi SET F10002={drDes["F10002"]}, F10004={drDes["F10004"]}, F10005={drDes["F10005"]}, F10006={drDes["F10006"]}, F10007={drDes["F10007"]}, IdMOtivModif={cmbMotiv.Value} WHERE IdAuto={drOri["IdAuto"]}";
        //                }
        //                else
        //                {
        //                    strSql += $@"INSERT INTO ""Org_Posturi""(
        //                    ""Id"", ""Denumire"",""DataInceput"", ""DataSfarsit"", F10002, F10004, F10005, F10006, F10007, ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
        //                    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
        //                    ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", ""IdMotivModif"",
        //                    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", USER_NO, TIME)
        //                    SELECT ""Id"", ""Denumire"",{General.ToDataUniv(txtDtVig.Date)}, ""DataSfarsit"", 
        //                    (SELECT F10002 FROM Org_Posturi WHERE IdAuto={drDes["IdAuto"]}) AS F10002, 
        //                    (SELECT F10004 FROM Org_Posturi WHERE IdAuto={drDes["IdAuto"]}) AS F10004,
        //                    (SELECT F10005 FROM Org_Posturi WHERE IdAuto={drDes["IdAuto"]}) AS F10005,
        //                    (SELECT F10006 FROM Org_Posturi WHERE IdAuto={drDes["IdAuto"]}) AS F10006,
        //                    (SELECT F10007 FROM Org_Posturi WHERE IdAuto={drDes["IdAuto"]}) AS F10007,
        //                    ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
        //                    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
        //                    ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", {cmbMotiv.Value},
        //                    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", {Session["UserId"]}, GetDate()
        //                    FROM ""Org_Posturi"" WHERE ""IdAuto""={drOri["IdAuto"]}";

        //                    strSql += $@"UPDATE Org_Posturi SET DataSfarsit={txtDtVig.Date.AddDays(-1)} WHERE IdAuto={drOri["IdAuto"]}";
        //                }

        //                //SchimbaInPlanificat(Convert.ToInt32(entOri.Id), dtRef, idUser, modifStruc, 0, 0, 0, f10003);
        //            }
        //        }


        //        IncarcaGrid();


        //        //if (Convert.ToInt32(General.Nz(grDate.FocusedNode.Key, 0)) > 0)
        //        //{
        //        //    Session["DataVigoare"] = txtDtVig.Value;
        //        //    Session["IdAuto"] = grDate.FocusedNode.Key;
        //        //    Response.Redirect("~/Organigrama/Posturi", false);
        //        //}
        //        //else
        //        //    MessageBox.Show("Nu s-a selectat nici un post", MessageBox.icoInfo, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        protected void btnModif_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(General.Nz(grDate.FocusedNode.Key, 0)) > 0)
                {
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
		                    INSERT INTO Org_Posturi(Id, Denumire, DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN, DataInceput, DataSfarsit, Stare, F10002, F10004, F10005, F10006, F10007, IdSuperior, NivelIerarhic, PlanHC, IdResponsabilitate, IdTipPost, CodBuget, IdLocatie, IdMotivModif, IdFamFR, CodCOR, FunctieCOR, IdSchema, IdFamGAM, Atribute, Criterii, ObservatiiModif, NivelHay, SalariuMin,  SalariuMed,  SalariuMax,  IdSuperiorFunctional, IdBeneficiu1, IdBeneficiu2, IdBeneficiu3, IdBeneficiu4, IdBeneficiu5, IdBeneficiu6, IdBeneficiu7, IdBeneficiu8, IdBeneficiu9, IdBeneficiu10, TIME, USER_NO) 
		                    SELECT Id, Denumire, DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN, CONVERT(date,{ General.ToDataUniv(Convert.ToDateTime(txtDtVig.Value)) }) AS DataInceput, DataSfarsit, 1 AS Stare, F10002, F10004, F10005, F10006, F10007, (SELECT Id FROM Org_Posturi WHERE IdAuto={ target_idAuto }) AS IdSuperior, NivelIerarhic, PlanHC, IdResponsabilitate, IdTipPost, CodBuget, IdLocatie, IdMotivModif, IdFamFR, CodCOR, FunctieCOR, IdSchema, IdFamGAM, Atribute, Criterii, ObservatiiModif, NivelHay, SalariuMin,  SalariuMed,  SalariuMax,  IdSuperiorFunctional, IdBeneficiu1, IdBeneficiu2, IdBeneficiu3, IdBeneficiu4, IdBeneficiu5, IdBeneficiu6, IdBeneficiu7, IdBeneficiu8, IdBeneficiu9, IdBeneficiu10, GETDATE(), { Session["UserId"] } AS USER_NO 
		                    FROM Org_Posturi WHERE IdAuto={ nod_idAuto }

		                    UPDATE Org_Posturi SET DataSfarsit= DATEADD(d, -1, CONVERT(date,{ General.ToDataUniv(Convert.ToDateTime(txtDtVig.Value)) })) WHERE IdAuto={ nod_idAuto }
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

                    string strSql = "";
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
                                int modifStruc = 0;
                                //if (drOri["F10002"] != drDes["F10002"] || drOri["F10004"] != drDes["F10004"] || drOri["F10005"] != drDes["F10005"] || drOri["F10006"] != drDes["F10006"] || drOri["F10007"] != drDes["F10007"]) modifStruc = 1;

                                ExecutaModif(target_idAuto, Convert.ToInt32(dr["IdAuto"]), Convert.ToInt32(dr["Id"]), Convert.ToDateTime(dr["DataInceput"]));

                                //if (Convert.ToDateTime(dr["DataInceput"]).Date == txtDtVig.Date)
                                //{
                                //    strSql += $@"UPDATE Org_Posturi SET 
                                //    IdMotivModif={cmbMotiv.Value},
                                //    (SELECT F10002 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10002, 
                                //    (SELECT F10004 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10004,
                                //    (SELECT F10005 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10005,
                                //    (SELECT F10006 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10006,
                                //    (SELECT F10007 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10007 
                                //    WHERE IdAuto={dr["IdAuto"]}";
                                //}
                                //else
                                //{
                                //    strSql += $@"INSERT INTO ""Org_Posturi""(
                                //    ""Id"", ""Denumire"",""DataInceput"", ""DataSfarsit"", F10002, F10004, F10005, F10006, F10007, ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
                                //    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                                //    ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", ""IdMotivModif"",
                                //    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", USER_NO, TIME)
                                //    SELECT ""Id"", ""Denumire"",{General.ToDataUniv(txtDtVig.Date)}, ""DataSfarsit"", 
                                //    (SELECT F10002 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10002, 
                                //    (SELECT F10004 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10004,
                                //    (SELECT F10005 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10005,
                                //    (SELECT F10006 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10006,
                                //    (SELECT F10007 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10007,
                                //    ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
                                //    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                                //    ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", {cmbMotiv.Value},
                                //    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", {Session["UserId"]}, GetDate()
                                //    FROM ""Org_Posturi"" WHERE ""IdAuto""={dr["IdAuto"]}";

                                //    strSql += $@"UPDATE Org_Posturi SET DataSfarsit={txtDtVig.Date.AddDays(-1)} WHERE IdAuto={dr["IdAuto"]}";
                                //}

                                ////SchimbaInPlanificat(Convert.ToInt32(entOri.Id), dtRef, idUser, modifStruc, 0, 0, 0, f10003);


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
                    strSql += $@"UPDATE ""Org_Posturi"" SET 
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
                    strSql += $@"INSERT INTO ""Org_Posturi""(
                                    ""Id"", ""Denumire"", DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN, ""DataInceput"", ""DataSfarsit"", F10002, F10004, F10005, F10006, F10007, 
                                    ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
                                    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                                    ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", ""IdMotivModif"",
                                    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", USER_NO, TIME)
                                    SELECT ""Id"", ""Denumire"", DenumireRO, DenumireEN, NumeGrupRO, NumeGrupEN,{General.ToDataUniv(txtDtVig.Date)}, ""DataSfarsit"", 
                                    (SELECT F10002 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10002, 
                                    (SELECT F10004 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10004,
                                    (SELECT F10005 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10005,
                                    (SELECT F10006 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10006,
                                    (SELECT F10007 FROM Org_Posturi WHERE IdAuto={target_idAuto}) AS F10007,
                                    ""Stare"", {(adaugaSuperior ? sqlSup : @" ""IdSuperior"" ")}, ""IdSuperiorFunctional"", 
                                    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                                    ""CodBuget"", ""CodCOR"", ""Atribute"", ""Criterii"", ""ObservatiiModif"", {cmbMotiv.Value},
                                    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", {Session["UserId"]}, GetDate()
                                    FROM ""Org_Posturi"" WHERE ""IdAuto""={idAuto}";

                    strSql += $@"UPDATE ""Org_Posturi"" SET"" DataSfarsit""={General.ToDataUniv(txtDtVig.Date.AddDays(-1))} WHERE ""IdAuto""={idAuto}";
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

        protected void btnExpand_Click(object sender, EventArgs e)
        {
            try
            {
                //if (grDate.Nodes[0].Expanded)
                //    grDate.CollapseAll();
                //else
                    grDate.ExpandAll();
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


        private string CreazaExcel(DateTime dtRef, int nivel, int idPost, int activ)
        {
            string numeFis = "";

            try
            {
                string clipa = DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                numeFis = $@"Diagrama{clipa}.xlsm";
                //int idUrm = Convert.ToInt32(Dami.NextId("Organigrama"));
                Workbook book = new Workbook();

                string rutaOri = HostingEnvironment.MapPath("~/Organigrama/Diagrama.xlsm");
                string rutaDes = HostingEnvironment.MapPath("~/Organigrama/Temp/" + numeFis);


                File.Copy(rutaOri, rutaDes, true);

                book.LoadDocument(rutaDes);

                book.Worksheets.Remove(book.Worksheets["Sheet1"]);
                book.Worksheets.Insert(0, "Sheet1");
                book.Worksheets.Remove(book.Worksheets["Sheet2"]);
                book.Worksheets.Insert(1, "Sheet2");

                Worksheet ws2 = book.Worksheets["Sheet2"];

                string strRef = dtRef.Day.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + General.NumeLuna(dtRef.Month, 1, "EN") + "-" + dtRef.Year.ToString();

                string strSql = "select \"Id\", \"Denumire\",\"IdSuperior\",\"NivelIerarhic\", Level, NVL(NR_ANG.NR, 0) as \"NrAngajati\"  from \"Org_Posturi\" " +

                                //Radu 07.06.2017 - pentru a aduce nr. de angajati pe post
                                " LEFT JOIN (select \"IdPost\", count(*)as NR from \"Org_relPostAngajat\" group by \"IdPost\") NR_ANG on NR_ANG.\"IdPost\"  = \"Org_Posturi\".\"Id\" " +

                                " where level<=" + nivel +
                                " and TRUNC(\"DataInceput\")<=to_date('" + strRef + "','DD-MM-YYYY') AND to_date('" + strRef + "','DD-MM-YYYY') <=TRUNC(\"DataSfarsit\") " +
                                " start with \"Id\"=" + idPost + " connect by nocycle prior \"Id\" = \"IdSuperior\" " +      //Radu 26.05.2016 - am adaugat nocycle pentru a opri bucla infinita
                               " and TRUNC(\"DataInceput\")<=to_date('" + strRef + "','DD-MM-YYYY') AND to_date('" + strRef + "','DD-MM-YYYY') <=TRUNC(\"DataSfarsit\") " + //Radu 09.01.2017 - conditie pentru a nu returna decat un post activ								
                               " and \"Stare\" = " + activ;     //Radu 07.02.2017

                //Radu 26.05.2016
                if (Constante.tipBD == 1)
                {
                    //strRef = dtRef.Day.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + dtRef.Month.ToString().PadLeft(2, Convert.ToChar("0")) + "/" + dtRef.Year.ToString();

                    //strSql = "WITH tree AS  ( "
                    //      + "SELECT Id, COALESCE(DenumireRO, Denumire) AS DenumireRO, COALESCE(DenumireEN,Denumire) AS DenumireEN, "
                    //      + "COALESCE(NumeGrupRO,Denumire) AS NumeGrupRO, COALESCE(NumeGrupEN,Denumire) AS NumeGrupEN, "
                    //      + "PlanHC, HCAprobat, IdSuperior, NivelIerarhic, 1 as Level "
                    //      + "FROM Org_Posturi as parent "
                    //      + " WHERE Id = " + idPost + " and CONVERT(DATE, DataInceput, 103)<=CONVERT(DATE, '" + strRef + "',103) AND CONVERT(DATE, '" + strRef + "',103) <= CONVERT(DATE, DataSfarsit, 103) AND Stare = " + activ
                    //      + " UNION ALL "                                                                                                                                    // ^Radu 07.02.2017
                    //      + "SELECT child.Id, COALESCE(child.DenumireRO, child.Denumire) AS DenumireRO, COALESCE(child.DenumireEN,child.Denumire) AS DenumireEN, "
                    //      + "COALESCE(child.NumeGrupRO,child.Denumire) AS NumeGrupRO, COALESCE(child.NumeGrupEN,child.Denumire) AS NumeGrupEN, "
                    //      + "child.PlanHC, child.HCAprobat, child.IdSuperior, child.NivelIerarhic, parent.Level + 1 "
                    //      + "FROM Org_Posturi as child "
                    //      + "JOIN tree parent on parent.Id = child.IdSuperior "
                    //      + " WHERE CONVERT(DATE, child.DataInceput, 103)<=CONVERT(DATE, '" + strRef + "',103) AND CONVERT(DATE, '" + strRef + "',103) <= CONVERT(DATE, child.DataSfarsit, 103) AND child.Stare = " + activ //Radu 10.05.2017
                    //      + ") "
                    //      + "SELECT Id, "
                    //      + "CASE WHEN (Level = " + nivel + " AND(SELECT COUNT(*) FROM Org_Posturi X WHERE X." + cmbParinte.Value + " = tree.Id) <> 0) THEN " + (General.Nz(cmbAfisare.Value, 1).ToString() == "1" ? "DenumireRO" : "NumeGrupRO") + " ELSE DenumireRO END AS DenumireRO, "
                    //      + "CASE WHEN (Level = " + nivel + " AND(SELECT COUNT(*) FROM Org_Posturi X WHERE X." + cmbParinte.Value + " = tree.Id) <> 0) THEN " + (General.Nz(cmbAfisare.Value, 1).ToString() == "1" ? "DenumireEN" : "NumeGrupEN") + " ELSE DenumireEN END AS DenumireEN, "
                    //      + "PlanHC, HCAprobat, IdSuperior, NivelIerarhic, Level, isnull(NR_ANG.NR, 0) as NrAngajati  FROM tree "

                    //      //Radu 07.06.2017 - pentru a aduce nr. de angajati pe post
                    //      + "left join (select IdPost, count(*) as NR from Org_relPostAngajat WHERE CONVERT(DATE, DataInceput, 103)<=CONVERT(DATE, '" + strRef + "',103) AND CONVERT(DATE, '" + strRef + "',103) <= CONVERT(DATE, DataSfarsit, 103) group by IdPost) NR_ANG on NR_ANG.IdPost  = tree.Id "

                    //      + " where Level <= " + nivel;



                    strSql = $@"WITH tree AS  
                            (
                            SELECT Id, IdSuperior, NivelIerarhic, 1 as Level,
                            COALESCE(DenumireRO, Denumire) AS DenumireRO, COALESCE(DenumireEN,Denumire) AS DenumireEN, 
                            COALESCE(NumeGrupRO, Denumire) AS NumeGrupRO, COALESCE(NumeGrupEN,Denumire) AS NumeGrupEN
                            FROM Org_Posturi as parent 
                            WHERE Id = {idPost} AND CONVERT(DATE, DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, DataSfarsit, 103) AND Stare = {activ}
                            UNION ALL
                            SELECT child.Id, child.IdSuperior, child.NivelIerarhic, parent.Level + 1,
                            COALESCE(child.DenumireRO, child.Denumire) AS DenumireRO, COALESCE(child.DenumireEN,child.Denumire) AS DenumireEN,
                            COALESCE(child.NumeGrupRO, child.Denumire) AS NumeGrupRO, COALESCE(child.NumeGrupEN,child.Denumire) AS NumeGrupEN
                            FROM Org_Posturi as child
                            JOIN tree parent on parent.Id = child.IdSuperior
                            WHERE CONVERT(DATE, child.DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, child.DataSfarsit, 103) AND child.Stare = {activ}
                            )
                            SELECT Id, IdSuperior, NivelIerarhic, Level,
                            CASE WHEN (Level = {nivel} AND (SELECT COUNT(*) FROM Org_Posturi X WHERE X.{cmbParinte.Value} = tree.Id) <> 0) THEN {(General.Nz(cmbAfisare.Value, 1).ToString() == "1" ? "DenumireRO" : "NumeGrupRO")} ELSE DenumireRO END AS DenumireRO,
                            CASE WHEN (Level = {nivel} AND (SELECT COUNT(*) FROM Org_Posturi X WHERE X.{cmbParinte.Value} = tree.Id) <> 0) THEN {(General.Nz(cmbAfisare.Value, 1).ToString() == "1" ? "DenumireEN" : "NumeGrupEN")} ELSE DenumireEN END AS DenumireEN,
                            dbo.[DamiHC](1, Id, {General.ToDataUniv(dtRef.Date)}) AS PlanHC, 
                            dbo.[DamiHC](2, Id, {General.ToDataUniv(dtRef.Date)}) AS HCAprobat, 
                            dbo.[DamiHC](3, Id, {General.ToDataUniv(dtRef.Date)}) AS HCEfectiv
                            FROM tree
                            where Level <= {nivel}";
                }

                DataTable dt = General.IncarcaDT(strSql, null);
                int x = 0;

                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    x = i + 1;
                    DataRow dr = dt.Rows[i];
                    ws2.Cells["A" + x].Value = General.Nz(dr["Id"],"").ToString();
                    if (General.Nz(dr["Id"], "").ToString() == idPost.ToString())
                        ws2.Cells["B" + x].Value = 0;
                    else
                        ws2.Cells["B" + x].Value = General.Nz(dr["IdSuperior"], "").ToString();

                    string den = General.Nz(dr["DenumireRO"], "").ToString();
                    //string den = General.Nz(dr["Denumire"], "").ToString();
                    //if (rbRO.Checked) den = General.Nz(dr["DenumireRO"], "").ToString();
                    if (General.Nz(cmbLimbi.Value,"RO").ToString() == "EN") den = General.Nz(dr["DenumireEN"], "").ToString();

                    string hc = "";
                    if (chkPlan.Checked) hc += ",Plan: " + General.Nz(dr["PlanHC"],0);
                    if (chkAprobat.Checked) hc += ",Aprobat: " + General.Nz(dr["HCAprobat"], 0);
                    if (chkEfectiv.Checked) hc += ",Efectiv: " + General.Nz(dr["HCEfectiv"], 0);
                    if (hc != "") hc = " (" + hc.Substring(1) + ")";

                    ws2.Cells["C" + x].Value = den + hc;
                }

                book.Worksheets.ActiveWorksheet = book.Worksheets["Sheet1"];
                book.Worksheets["Sheet2"].Visible = false;
                book.SaveDocument(rutaDes);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + numeFis);
                Response.TransmitFile(rutaDes);
                Response.End();

                //var fileInfo = new System.IO.FileInfo(rutaDes);
                //Response.ContentType = "application/octet-stream";
                //Response.AddHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\"", fis.Replace("#", idUrm.ToString())));
                //Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                //Response.WriteFile(fis.Replace("#", idUrm.ToString()));
                //Response.End();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return numeFis;
        }

        protected void btnOkLevel_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport_Click(null, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
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

        //public void SchimbaInPlanificat(int id, int modifStruc, int modifFunctie, int modifCOR, int modifSalariu)
        //{
        //    try
        //    {
        //        DateTime dtRef = Convert.ToDateTime(txtDtVig.Date).Date;

        //        DateTime dtLuc = DateTime.Now;
        //        DataRow drLuc = General.IncarcaDR("SELECT * FROM F010", null);
        //        if (drLuc.ToString() != "" && drLuc != null && drLuc["F01011"] != null && drLuc["F01012"] != null) dtLuc = new DateTime(Convert.ToInt32(drLuc["F01011"]), Convert.ToInt32(drLuc["F01012"]), 1);

        //        if (dtRef >= dtLuc || (dtRef.Year == dtLuc.Year && dtRef.Month == dtLuc.Month))
        //        {
        //            string strSql = $@"SELECT A.* 
        //                FROM ""Org_relPostAngajat"" A
        //                INNER JOIN F100 B ON A.F10003=B.F10003
        //                WHERE A.""IdPost""={id} AND B.F100925=0 AND (B.F10025=0 OR B.F10025 = 999)
        //                AND CONVERT(date,A.""DataInceput"")<={General.ToDataUniv(dtRef)} AND {General.ToDataUniv(dtRef)} <= CONVERT(date,A.""DataSfarsit"")";

        //            DataTable dt = General.IncarcaDT(strSql, null);
        //            for(int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                DataRow dr = dt.Rows[i];

        //                if (modifStruc == 1 || modifFunctie == 1 || modifCOR == 1 || modifSalariu == 1)
        //                {
        //                    if (dr["DataReferinta"] != null && Convert.ToDateTime(dr["DataReferinta"]).Date == Convert.ToDateTime(dtRef.Date).Date)
        //                    {
        //                        //pastrez informatia deja introdusa si fac update doar pe ce s-a modificat
        //                        dr["Stare"] = 2;              //planificat
        //                        if (modifStruc == 1) dr["modifStructura"] = modifStruc;
        //                        if (modifFunctie == 1) dr["modifFunctie"] = modifFunctie;
        //                        if (modifCOR == 1) dr["modifCOR"] = modifCOR;
        //                        if (modifSalariu == 1) dr["modifSalariu"] = modifSalariu;
        //                    }
        //                    else
        //                    {
        //                        //introduc noua informatie
        //                        dr["Stare"] = 2;              //planificat
        //                        dr["DataReferinta"] = dtRef;
        //                        dr["modifStructura"] = modifStruc;
        //                        dr["modifFunctie"] = modifFunctie;
        //                        dr["modifCOR"] = modifCOR;
        //                        dr["modifSalariu"] = modifSalariu;
        //                    }
        //                }
        //                else
        //                {
        //                    dr["Stare"] = 1;
        //                    dr["DataReferinta"] = null;
        //                    dr["modifStructura"] = 0;
        //                    dr["modifFunctie"] = 0;
        //                    dr["modifCOR"] = 0;
        //                    dr["modifSalariu"] = 0;
        //                }
        //            }

        //            General.SalveazaDate(dt, "Org_relPostAngajat");

        //            Notif.TrimiteNotificare("PosturiNomen.PosturiLista",(int)Constante.TipNotificare.Notificare, "SELECT TOP 1 * FROM Org_Posturi WHERE Id=" + id, "Org_Posturi", -99, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["User_Marca"]));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


    }
}