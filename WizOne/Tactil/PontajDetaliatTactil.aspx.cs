using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Tactil
{
    public partial class PontajDetaliatTactil : System.Web.UI.Page
    {

        //tip = 1       Pontaj pe Angajat
        //tip = 2       Pontaj pe Zi
        public int tip = 1;
        string cmp = "USER_NO,TIME,IDAUTO,";
        int luna = -99;
        int an = -99;

        public class metaAbsTipZi
        {
            public int F10003 { get; set; }
            public DateTime Ziua { get; set; }
        }


        public class metaIds
        {
            public Nullable<int> F10003 { get; set; }
            public Nullable<int> Zi { get; set; }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));

                if (!IsPostBack)
                {
                    grDate.Attributes.Add("onkeypress", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));

                    if (tip == 1 || tip == 10)
                    {

                        grDate.Columns["NumeComplet"].Visible = false;

                        grDate.Columns["Cheia"].Caption = "Ziua";
                    }
                    else
                    {
                        grDate.Columns["NumeComplet"].Visible = true;

                        grDate.Columns["Cheia"].Caption = "Marca";
                    }

                    CreazaGrid();

                    DataTable dtVal = General.IncarcaDT(Constante.tipBD == 1 ? @"SELECT TOP 0 * FROM ""Ptj_IstoricVal"" " : @"SELECT * FROM ""Ptj_IstoricVal"" WHERE ROWNUM = 0 ", null);
                    Session["Ptj_IstoricVal"] = dtVal;


                    //DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);
                    //luna = Convert.ToInt32(dt010.Rows[0][1].ToString());
                    //an = Convert.ToInt32(dt010.Rows[0][0].ToString());
                    luna = DateTime.Now.Month;
                    an = DateTime.Now.Year;
                }
                else
                {
                    luna = Convert.ToInt32(cmbLuna.Value);
                    an = Convert.ToInt32(cmbAn.Value);
                }

                if (Dami.ValoareParam("PontajulAreCC") == "1" && (tip == 1 || tip == 10))
                {
                    grDate.Columns[0].Visible = true;

                    if (!IsPostBack)
                    {

                    }
                }

                if (tip == 2 || tip == 20)
                {
                    grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ", "10"));
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesTactil();

                cmbAn.DataSource = General.ListaNumere(2015, 2022);
                cmbAn.DataBind();
                cmbLuna.DataSource = General.ListaLuniDesc();
                cmbLuna.DataBind();
                //DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

                if (!IsPostBack)
                {
                    try
                    {
                        //cmbLuna.Value = Convert.ToInt32(dt010.Rows[0][1].ToString());
                        //cmbAn.Value = Convert.ToInt32(dt010.Rows[0][0].ToString());
                        cmbLuna.Value = DateTime.Now.Month;
                        cmbAn.Value = DateTime.Now.Year;
                    }
                    catch (Exception) { }

                    an = Convert.ToInt32(cmbAn.Value ?? DateTime.Now.Year);
                    luna = Convert.ToInt32(cmbLuna.Value ?? DateTime.Now.Month);
                }
                else
                {
                    luna = Convert.ToInt32(cmbLuna.Value);
                    an = Convert.ToInt32(cmbAn.Value);
                }

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");


                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                #endregion

                //tip = Convert.ToInt32(General.Nz(Request["tip"], 1));

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;

                    if (tip == 10)
                    {
                        NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_PontajulEchipei"] ?? "").ToString());
                        if (lst.Count > 0)
                        {
                            if (lst["An"] != "" && lst["Luna"] != "" && lst["Ziua"] != "")
                            {
                                try
                                {
                                    DateTime dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(lst["Ziua"]));
                                    if (General.Nz(Request.QueryString["Ziua"], "").ToString() != "") dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(General.Nz(Request.QueryString["Ziua"], "").ToString().Replace("Ziua", "")));
                                    luna = dt.Month;
                                    an = dt.Year;
                                }
                                catch (Exception)
                                {
                                }
                            }


                            if (General.Nz(Request.QueryString["idxPag"], "").ToString() != "" && General.Nz(Request.QueryString["idxRow"], "").ToString() != "")
                            {
                                Session["Filtru_PontajulEchipei"] += "&IndexPag=" + General.Nz(Request.QueryString["idxPag"], "").ToString();
                                Session["Filtru_PontajulEchipei"] += "&IndexRow=" + General.Nz(Request.QueryString["idxRow"], "").ToString();
                            }
                        }


                    }

                    if (tip == 20)
                    {
                        NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_PontajulEchipei"] ?? "").ToString());
                        if (lst.Count > 0)
                        {
                            if (lst["An"] != "" && lst["Luna"] != "" && lst["Ziua"] != "")
                            {
                                try
                                {
                                    DateTime dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(lst["Ziua"]));
                                    //txtZiua.Value = dt;
                                }
                                catch (Exception)
                                {
                                }
                            }

                        }
                    }

                    IncarcaDate();
                }
                else
                {
                    if (General.Nz(Session["SurseCombo"], "").ToString() != "")
                    {
                        DataSet ds = Session["SurseCombo"] as DataSet;
                        if (ds.Tables.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables.Count; i++)
                            {
                                GridViewDataComboBoxColumn colAbs = (grDate.Columns[ds.Tables[i].TableName] as GridViewDataComboBoxColumn);
                                if (colAbs != null) colAbs.PropertiesComboBox.DataSource = ds.Tables[i];

                            }
                        }
                    }

                    if (Session["InformatiaCurenta"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }

                    if (Session["InformatiaCurenta_Totaluri"] != null)
                    {
                        grDateTotaluri.DataSource = Session["InformatiaCurenta_Totaluri"];
                        grDateTotaluri.DataBind();
                    }

                }


                //if (tip == 1 || tip == 10)
                //{
                grDate.SettingsPager.PageSize = 31;
                //}
                //else
                //{

                //}



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
                //DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);
                DateTime dtData = new DateTime(an, luna, 1);
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));
                //int idRol = Convert.ToInt32(cmbRolAng.Value);
                //if (tip == 2)
                //{
                //    dtData = Convert.ToDateTime(txtZiua.Value);
                //    idRol = Convert.ToInt32(cmbRolZi.Value);
                //}

                General.PontajInitGeneral(Convert.ToInt32(Session["UserId"]), dtData.Year, dtData.Month);

                grDate.KeyFieldName = "Cheia";
                //DataTable dt = PontajCuInOut(Convert.ToInt32(cmbAng.Value ?? -99), dtData, Convert.ToInt32(Session["UserId"] ?? -99), idRol, Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbSubDept.Value ?? -99), Convert.ToInt32(cmbBirou.Value ?? -99), tip);
                DataTable dt = PontajCuInOut();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Cheia"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();

                if (dt.Rows.Count > 0)
                {
                    string cul = General.Nz(dt.Rows[0]["CuloareStare"], "#FFFFFF").ToString();
                    if (cul.Length == 9) cul = "#" + cul.Substring(3);
      
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        
        public DataTable PontajCuInOut()
        {
            //tip  =  1    pontaj pe angajat
            //tip  =  2    pontaj pe zi

            DataTable dt = new DataTable();

            try
            {
                string filtru = "";
                string cheia = "";
                int tipInreg = 1;

                //DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                DateTime ziua = new DateTime(an, luna, 1);
                int idRol = 0;

                //if (General.Nz(Request.QueryString["Tip"], "").ToString() == "1" || General.Nz(Request.QueryString["Tip"], "").ToString() == "10")
                //{
                    filtru = $@" AND {General.ToDataUniv(ziua.Year, ziua.Month)} <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)} AND A.F10003=" + Convert.ToInt32(Session["User_Marca"].ToString());
                    cheia = General.FunctiiData("P.\"Ziua\"", "Z");

                    //2018.02.09 Imbunatatire
                    if (General.Nz(Request.QueryString["Ziua"], "").ToString() != "")
                        filtru += @" AND P.""Ziua""=" + General.ToDataUniv(ziua.Year, ziua.Month, Convert.ToInt32(Request.QueryString["Ziua"].Replace("Ziua", "")));

                    tipInreg = 1;
                //}
                //else
                //{
                //    ziua = Convert.ToDateTime(txtZiua.Value);
                //    idRol = Convert.ToInt32(cmbRolZi.Value);

                //    cheia = "P.F10003";
                //    filtru = $@" AND {General.TruncateDate("P.\"Ziua\"")} = {General.ToDataUniv(ziua.Year, ziua.Month, ziua.Day)}";

                //}


                //Florin 2018.08.27
                if (tipInreg != 1)
                {
                    string conditie = " P.\"ValStr\" not like '%[0-9]%' AND P.\"ValStr\" != '' ";
                    if (Constante.tipBD == 2) conditie = " NOT REGEXP_LIKE(P.\"ValStr\", '[[:digit:]]') ";

                    string erori = " AND (((P.\"In1\" IS NULL AND P.\"Out1\" IS NOT NULL) OR (P.\"In1\" IS NOT NULL AND P.\"Out1\" IS NULL)) "
                                    + "OR ((P.\"In2\" IS NULL AND P.\"Out2\" IS NOT NULL) OR (P.\"In2\" IS NOT NULL AND P.\"Out2\" IS NULL)) "
                                    + "OR ((P.\"In3\" IS NULL AND P.\"Out3\" IS NOT NULL) OR (P.\"In3\" IS NOT NULL AND P.\"Out3\" IS NULL)) "
                                    + "OR ((P.\"In4\" IS NULL AND P.\"Out4\" IS NOT NULL) OR (P.\"In4\" IS NOT NULL AND P.\"Out4\" IS NULL)) "
                                    + "OR ((P.\"In5\" IS NULL AND P.\"Out5\" IS NOT NULL) OR (P.\"In5\" IS NOT NULL AND P.\"Out5\" IS NULL)) "
                                    + "OR ((P.\"In6\" IS NULL AND P.\"Out6\" IS NOT NULL) OR (P.\"In6\" IS NOT NULL AND P.\"Out6\" IS NULL)) "
                                    + "OR ((P.\"In7\" IS NULL AND P.\"Out7\" IS NOT NULL) OR (P.\"In7\" IS NOT NULL AND P.\"Out7\" IS NULL)) "
                                    + "OR ((P.\"In8\" IS NULL AND P.\"Out8\" IS NOT NULL) OR (P.\"In8\" IS NOT NULL AND P.\"Out8\" IS NULL)) "
                                    + "OR ((P.\"In9\" IS NULL AND P.\"Out9\" IS NOT NULL) OR (P.\"In9\" IS NOT NULL AND P.\"Out9\" IS NULL)) "
                                    + "OR ((P.\"In10\" IS NULL AND P.\"Out10\" IS NOT NULL) OR (P.\"In10\" IS NOT NULL AND P.\"Out10\" IS NULL)) "
                                    + "OR ((P.\"In11\" IS NULL AND P.\"Out11\" IS NOT NULL) OR (P.\"In11\" IS NOT NULL AND P.\"Out11\" IS NULL)) "
                                    + "OR ((P.\"In12\" IS NULL AND P.\"Out12\" IS NOT NULL) OR (P.\"In12\" IS NOT NULL AND P.\"Out12\" IS NULL)) "
                                    + "OR ((P.\"In13\" IS NULL AND P.\"Out13\" IS NOT NULL) OR (P.\"In13\" IS NOT NULL AND P.\"Out13\" IS NULL)) "
                                    + "OR ((P.\"In14\" IS NULL AND P.\"Out14\" IS NOT NULL) OR (P.\"In14\" IS NOT NULL AND P.\"Out14\" IS NULL)) "
                                    + "OR ((P.\"In15\" IS NULL AND P.\"Out15\" IS NOT NULL) OR (P.\"In15\" IS NOT NULL AND P.\"Out15\" IS NULL)) "
                                    + "OR ((P.\"In16\" IS NULL AND P.\"Out16\" IS NOT NULL) OR (P.\"In16\" IS NOT NULL AND P.\"Out16\" IS NULL)) "
                                    + "OR ((P.\"In17\" IS NULL AND P.\"Out17\" IS NOT NULL) OR (P.\"In17\" IS NOT NULL AND P.\"Out17\" IS NULL)) "
                                    + "OR ((P.\"In18\" IS NULL AND P.\"Out18\" IS NOT NULL) OR (P.\"In18\" IS NOT NULL AND P.\"Out18\" IS NULL)) "
                                    + "OR ((P.\"In19\" IS NULL AND P.\"Out19\" IS NOT NULL) OR (P.\"In19\" IS NOT NULL AND P.\"Out19\" IS NULL)) "
                                    + "OR ((P.\"In20\" IS NULL AND P.\"Out20\" IS NOT NULL) OR (P.\"In20\" IS NOT NULL AND P.\"Out20\" IS NULL)) "

                                    + "OR ((P.\"In1\" is not null OR P.\"In2\" is not null OR P.\"In3\" is not null OR P.\"In4\" is not null OR P.\"In5\" is not null OR "
                                    + "P.\"In6\" is not null OR P.\"In7\" is not null OR P.\"In8\" is not null OR P.\"In9\" is not null OR P.\"In10\" is not null OR "
                                    + "P.\"In11\" is not null OR P.\"In12\" is not null OR P.\"In13\" is not null OR P.\"In14\" is not null OR P.\"In15\" is not null OR "
                                    + "P.\"In16\" is not null OR P.\"In17\" is not null OR P.\"In18\" is not null OR P.\"In19\" is not null OR P.\"In20\" is not null) "
                                    + "AND " + conditie + " AND P.\"ValStr\" IS NOT NULL) ) ";

                    string lipsaPontaj = " AND (P.\"In1\" IS NULL AND P.\"Out1\" IS NULL AND P.\"In2\" IS NULL AND P.\"Out2\" IS NULL AND " +
                                        " P.\"In3\" IS NULL AND P.\"Out3\" IS NULL AND P.\"In4\" IS NULL AND P.\"Out4\" IS NULL AND " +
                                        " P.\"In5\" IS NULL AND P.\"Out5\" IS NULL AND P.\"In6\" IS NULL AND P.\"Out6\" IS NULL AND " +
                                        " P.\"In7\" IS NULL AND P.\"Out7\" IS NULL AND P.\"In8\" IS NULL AND P.\"Out8\" IS NULL AND " +
                                        " P.\"In9\" IS NULL AND P.\"Out9\" IS NULL AND P.\"In10\" IS NULL AND P.\"Out10\" IS NULL AND " +
                                        " P.\"In11\" IS NULL AND P.\"Out11\" IS NULL AND P.\"In12\" IS NULL AND P.\"Out12\" IS NULL AND " +
                                        " P.\"In13\" IS NULL AND P.\"Out13\" IS NULL AND P.\"In14\" IS NULL AND P.\"Out14\" IS NULL AND " +
                                        " P.\"In15\" IS NULL AND P.\"Out15\" IS NULL AND P.\"In16\" IS NULL AND P.\"Out16\" IS NULL AND " +
                                        " P.\"In17\" IS NULL AND P.\"Out17\" IS NULL AND P.\"In18\" IS NULL AND P.\"Out18\" IS NULL AND " +
                                        " P.\"In19\" IS NULL AND P.\"Out19\" IS NULL AND P.\"In20\" IS NULL AND P.\"Out20\" IS NULL AND (\"ValStr\" IS NULL OR \"ValStr\" = '') ) ";

                    if (tipInreg == 2) filtru += erori;
                    if (tipInreg == 3) filtru += lipsaPontaj;
                    if (tipInreg == 4) filtru += " AND (" + erori.Substring(4) + " OR " + lipsaPontaj.Substring(4) + ")";
                }




                string valTmp = "";
                string[] arrVal = Constante.lstValuri.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < arrVal.Length - 1; i++)
                {
                    valTmp += $@",CONVERT(datetime,DATEADD(minute, P.""{arrVal[i]}"", '')) AS ""ValTmp{arrVal[i].Replace("Val","")}"" ";
                }

                //Schimbam IdAuto in Cheia si modificam valoarea cu ziua cand este pontaj pe zi si cu Marca cand este pontaj pe angajat pentru a putea dezactiva valurile pe care nu avem voie sa pontam

                string op = "+";
                if (Constante.tipBD == 2) op = "||";


                //Florin 2018-07-25 am adugat filtrul CONVERT(date,P.""Ziua"") <= A.F10023

                string strSql = $@"SELECT P.*, {General.FunctiiData("P.\"Ziua\"", "Z")} AS ""Zi"", P.""Ziua"", A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"" {valTmp} , 
                            {cheia} AS ""Cheia"",                           
                            E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Dept"",
                            L.""Denumire"" AS ""DescContract"", M.""Denumire"" AS DescProgram, COALESCE(L.""OreSup"",1) AS ""OreSup"", COALESCE(L.""Afisare"",1) AS ""Afisare"",
                            CASE WHEN A.F10022 <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <= A.F10023 THEN 1 ELSE 0 END AS ""Activ"",  
                            COALESCE(J.""IdStare"",1) AS ""IdStare"", K.""Culoare"" AS ""CuloareStare"", K.""Denumire"" AS ""NumeStare"", 
                            CASE WHEN (SELECT COUNT(*) FROM ""Ptj_Cereri"" Z 
                            INNER JOIN ""Ptj_tblAbsente"" Y ON Z.""IdAbsenta"" = Y.""Id""
                            WHERE Z.F10003 = P.F10003 AND Z.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= Z.""DataSfarsit"" AND Z.""IdStare"" = 3
                            AND Z.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""IdTipOre"" = 1) AND COALESCE(Y.""NuTrimiteInPontaj"", 0) = 0) = 0 THEN 0 ELSE 1 END AS ""VineDinCereri"", 
                            A.F10022, A.F10023,
                            (SELECT COALESCE(A.""OreInVal"",'') + ';'
                            FROM ""Ptj_tblAbsente"" a
                            INNER JOIN ""Ptj_ContracteAbsente"" b ON a.""Id"" = b.""IdAbsenta""
                            INNER JOIN ""Ptj_relRolAbsenta"" c ON a.""Id"" = c.""IdAbsenta""
                            WHERE A.""OreInVal"" IS NOT NULL AND RTRIM(LTRIM(A.""OreInVal"")) <> '' AND B.""IdContract""=P.""IdContract"" AND C.""IdRol""={idRol} AND 
                            (((CASE WHEN(P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0) THEN 1 ELSE 0 END) = COALESCE(B.ZL,0)) OR
                            ((CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) = COALESCE(B.S,0)) OR
                            ((CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) = COALESCE(B.D,0)) OR
                            COALESCE(P.""ZiLiberaLegala"",0) = COALESCE(B.SL,0)) 
                            GROUP BY A.""OreInVal""
                            ORDER BY A.""OreInVal""
                            FOR XML PATH ('')) AS ""ValActive"",


							(SELECT COALESCE(A.""Coloana"",'') + ';'
                            FROM(
                            SELECT ""Coloana"" FROM ""Ptj_tblAdmin"" WHERE SUBSTRING(""Coloana"", 1, 3) = 'Val' AND ""Coloana"" NOT IN('ValAbs', 'ValStr') AND COALESCE(""Blocat"", 0) = 1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                            WHERE B.""IdUser"" = {Session["UserId"]} AND A.""IdForm"" = 'pontaj.pontajone' AND SUBSTRING(A.""IdColoana"", 1, 6) = 'ValTmp' AND COALESCE(A.""Blocat"",0)=1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'pontaj.pontajone' AND SUBSTRING(A.""IdColoana"", 1, 6) = 'ValTmp' AND COALESCE(A.""Blocat"",0)=1
                            ) A
                            GROUP BY A.""Coloana""
                            ORDER BY A.""Coloana""
                            FOR XML PATH('')) AS ""ValSecuritate"",


	                        (select ',' + X.""DenumireScurta"" + '=' + X.""Denumire"" from ( 
                            select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 0 AS ""Tip"" 
                            from ""Ptj_tblAbsente"" a
                            inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                            inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                            WHERE A.""IdTipOre"" = 1
                            group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal""
                            UNION
                            select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 1 AS ""Tip""
                            from ""Ptj_tblAbsente"" a
                            inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                            inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                            WHERE A.""OreInVal"" IS NOT NULL
                            group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"") x
                            WHERE COALESCE(X.DenumireScurta,'') <> '' AND X.""IdContract"" = P.""IdContract"" and X.""IdRol"" = {idRol} AND
                            (COALESCE(X.""ZileSapt"",0)=(CASE WHEN P.""ZiSapt""<6 AND P.""ZiLibera""=0 THEN 1 ELSE 0 END) 
                            AND COALESCE(X.S,0) = (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) 
                            AND COALESCE(X.D,0) = (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END)
							AND COALESCE(X.SL,0) = (CASE WHEN P.""ZiSapt"" < 6 AND COALESCE(P.""ZiLiberaLegala"",0) = 1 THEN 1 ELSE 0 END))
                            GROUP BY X.""Id"", X.""DenumireScurta"", X.""Denumire""
                            ORDER BY X.""Id"", X.""DenumireScurta"", X.""Denumire""
                            FOR XML PATH ('')) AS ""ValAbsente"",


                            CASE WHEN {idRol} = 3 THEN 1 ELSE 
                            CASE WHEN ({idRol} = 2 AND ((COALESCE(J.""IdStare"",1)=1 OR COALESCE(J.""IdStare"",1) = 2 OR COALESCE(J.""IdStare"",1) = 4 OR COALESCE(J.""IdStare"",1) = 6))) THEN 1 ELSE 
                            CASE WHEN ({idRol} = 1 AND(COALESCE(J.""IdStare"", 1) = 1 OR COALESCE(J.""IdStare"", 1) = 4)) THEN 1 ELSE 0
                            END END END AS ""DrepturiModif""
                            FROM ""Ptj_Intrari"" P
                            LEFT JOIN F100 A ON A.F10003 = P.F10003
                            LEFT JOIN F1001 C ON A.F10003=C.F10003
                            LEFT JOIN F002 E ON P.F10002 = E.F00202
                            LEFT JOIN F003 F ON P.F10004 = F.F00304
                            LEFT JOIN F004 G ON P.F10005 = G.F00405
                            LEFT JOIN F005 H ON P.F10006 = H.F00506
                            LEFT JOIN F006 I ON P.F10007 = I.F00607
                            LEFT JOIN ""Ptj_Cumulat"" J ON J.F10003=A.F10003 AND J.""An""={General.FunctiiData("P.\"Ziua\"", "A")} AND J.""Luna""={General.FunctiiData("P.\"Ziua\"", "L")}
                            LEFT JOIN ""Ptj_tblStariPontaj"" K ON COALESCE(J.""IdStare"",1) = K.""Id""
                            LEFT JOIN ""Ptj_Contracte"" L ON P.""IdContract""=L.""Id""
                            LEFT JOIN ""Ptj_Programe"" M ON P.""IdProgram""=M.""Id""
                            WHERE CONVERT(date,P.""Ziua"") <= A.F10023
                            {filtru}
                            ORDER BY A.F10003, {General.TruncateDate("P.Ziua")}";


                //LEFT JOIN ""Ptj_Cereri"" M ON A.F10003=M.F10003 AND M.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= M.""DataSfarsit"" AND M.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""IdTipOre""=1)

                //(select ',' + '""' + CONVERT(nvarchar(10),COALESCE(X.Id,'')) + ' = ' + X.DenumireScurta + '""' from ( 

                dt = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }




  




        private string SelectComun()
        {
            string strSql = "";
            try
            {
                string semn = "+";
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2)
                {
                    semn = "||";
                    cmp = "ROWNUM";
                }
                
                //DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);
                DateTime dtData = new DateTime(an, luna, 1);
                //if (tip == 2) dtData = Convert.ToDateTime(txtZiua.Value);


                strSql = @"SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) AS int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Alias"", S.""Denumire"") AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN ""tblSupervizori"" S ON C.""IdRol""=S.""Id""
                                WHERE C.""IdSuper"" = {0}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) as int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Alias"", S.""Denumire"") AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN ""tblSupervizori"" S ON C.""IdRol""=S.""Id""
                                WHERE J.""IdUser"" = {0}";

                strSql = string.Format(strSql, Session["UserId"], semn, cmp);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

#region grDate


        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName.Length >= 3)
                {
                    switch (e.DataColumn.FieldName.ToLower().Substring(0, 3))
                    {
                        case "in1":
                        case "in2":
                        case "in3":
                        case "in4":
                        case "in5":
                        case "in6":
                        case "in7":
                        case "in8":
                        case "in9":
                        case "out":
                            {
                                if (e.CellValue != null && e.CellValue != DBNull.Value)
                                {
                                    e.Cell.ToolTip = e.GetValue(e.DataColumn.FieldName).ToString();
                                    
                                    object obj = grDate.GetRowValues(e.VisibleIndex, "Ziua");
                                    DateTime inOut = Convert.ToDateTime(e.CellValue);
                                    DateTime zi = Convert.ToDateTime(obj);
                                    if (inOut.Date != zi.Date) e.Cell.BackColor = System.Drawing.Color.LightGray;
                                }
                            }
                            break;
                        case "val":
                            {
                                string col = e.DataColumn.FieldName;
                                if (Constante.lstValuri.IndexOf(col.Replace("Tmp", "") + ";") >= 0)
                                {
                                    //setam culoarea pentru weekend si sarbatori legale


                                    //setam culoarea de fundal daca a fost modificata
                                    object obj = grDate.GetRowValues(e.VisibleIndex, "ValModif" + col.Replace("ValTmp", ""));

                                    if (General.Nz(obj, "").ToString() != "")
                                    {
                                        if (General.CuloarePontaj((int)obj) != null) e.Cell.BackColor = (System.Drawing.Color)General.CuloarePontaj((int)obj);
                                    }

                                }
                            }
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

        protected void grDate_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {
            try
            {
                int ziSapt = Convert.ToInt32(e.GetValue("ZiSapt"));
                int ziLib = Convert.ToInt32(e.GetValue("ZiLibera"));
                if (ziSapt == 6 || ziSapt == 7 || ziLib == 1)
                {
                    e.Row.BackColor = System.Drawing.Color.Aquamarine;
                }
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
                if (e.Column.FieldName.Length >= 6 && e.Column.FieldName.Substring(0, 6) == "ValTmp")
                {
                    e.Editor.ReadOnly = false;
                }
   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_DataBound(object sender, EventArgs e)
        {
            try
            {
                var clientData = new Dictionary<int, object>();
                var lstDrepturi = new Dictionary<int, int>();
                var lstValAbsente = new Dictionary<int, object>();
                var lstValSec = new Dictionary<int, object>();
                var lstAfisare = new Dictionary<int, object>();

                var grid = sender as ASPxGridView;
                for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
                {
                    var rowValues = grid.GetRowValues(i, new string[] { "Cheia", "ValActive", "DrepturiModif", "ValAbsente", "ValSecuritate", "Afisare" }) as object[];
                    clientData.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[1] ?? "");
                    lstDrepturi.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), Convert.ToInt32(rowValues[2] ?? 0));
                    lstValAbsente.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[3] ?? "");
                    lstValSec.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[4] ?? "");
                    lstAfisare.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[5] ?? "");
                }

                grid.JSProperties["cp_cellsToDisable"] = clientData;
                grid.JSProperties["cp_cellsDrepturi"] = lstDrepturi;
                grid.JSProperties["cp_ValAbsente"] = lstValAbsente;
                grid.JSProperties["cp_ValSec"] = lstValSec;
                grid.JSProperties["cp_Afisare"] = lstAfisare;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


#endregion 



   

   

        private void CreazaGrid()
        {
            try
            {
                string cmp = "SUBSTRING";
                if (Constante.tipBD == 2)
                    cmp = "SUBSTR";
                DataTable dtCol = General.IncarcaDT($@"SELECT A.*, 
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' AND COALESCE(B.""DenumireScurta"",'') <> '' THEN REPLACE(B.""DenumireScurta"",' ','') ELSE A.""Coloana"" END AS ""ColDen"",
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' AND COALESCE(B.""DenumireScurta"",'') <> '' THEN B.""DenumireScurta"" ELSE A.""Alias"" END AS ""ColAlias"",
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' AND COALESCE(B.""Denumire"",'') <> '' THEN B.""Denumire"" ELSE (CASE WHEN COALESCE(A.""AliasToolTip"",'') <> '' THEN A.""AliasToolTip"" ELSE A.""Coloana"" END) END AS ""ColTT"",
                                COALESCE(B.""DenumireScurta"",'') AS ""ColScurta""
                                FROM ""Ptj_tblAdmin"" A
                                LEFT JOIN ""Ptj_tblAbsente"" B ON A.""Coloana""=B.""OreInVal""
                                ORDER BY A.""Ordine"" ", null);

                if (dtCol != null)
                {
                    DataSet ds = new DataSet();

                    for (int i = 0; i < dtCol.Rows.Count; i++)
                    {
                        DataRow dr = dtCol.Rows[i];
                        string colField = General.Nz(dr["Coloana"], "col" + i).ToString();
                        string colName = General.Nz(dr["ColDen"], "col" + i).ToString() + "_" + General.Nz(dr["ColScurta"], "").ToString();
                        //string colName = General.Nz(dr["ColDen"], "col" + i).ToString();
                        string alias = General.Nz(dr["ColAlias"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
                        bool vizibil = Convert.ToBoolean(General.Nz(dr["Vizibil"], false));
                        bool blocat = Convert.ToBoolean(General.Nz(dr["Blocat"], false));
                        int latime = Convert.ToInt32(General.Nz(dr["Latime"], 80));
                        int tipCol = Convert.ToInt32(General.Nz(dr["TipColoana"],1));
                        string tt = General.Nz(dr["ColTT"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
                        bool unb = false;

                        if (colField != "ValStr" && colField != "Dept")
                            continue;
                        if (colField == "Dept")
                            latime = 420;                
                        if (colField == "ValStr")
                            latime = 300;


                        if (Constante.lstValuri.IndexOf(colField + ";") >= 0)
                        {
                            unb = true;
                            colField = "ValTmp" + colField.Replace("Val","");
                        }

                        switch (tipCol)
                        {
                            case 0:                             //General
                                {
                                    GridViewDataColumn c = new GridViewDataColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    //Settings-AutoFilterCondition="Contains"
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 1:                             //CheckBox
                                {
                                    GridViewDataCheckColumn c = new GridViewDataCheckColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 2:                             //ComboBox
                                {
                                    
                                    GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (colName.Length >= 6 && colName.Substring(0, 6) == "ValAbs")
                                    {
                                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                                    }

                                    c.PropertiesComboBox.AllowNull = true;

                                    if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
                                    {
                                        string sursa = (dr["SursaCombo"] as string ?? "").ToString().Trim();
                                        DataTable dtCmb = General.IncarcaDT(sursa, null);
                                        dtCmb.TableName = colField;
                                        ds.Tables.Add(dtCmb);

                                        Session["SurseCombo"] = ds;
                                        
                                        c.PropertiesComboBox.DropDownWidth = 350;
                                        c.PropertiesComboBox.DataSource = dtCmb;
                                        c.PropertiesComboBox.ValueField = dtCmb.Columns[0].ColumnName;
                                        c.PropertiesComboBox.ValueType = dtCmb.Columns[0].DataType;
                                        c.PropertiesComboBox.TextFormatString = "{0}";
                                        switch (dtCmb.Columns.Count)
                                        {
                                            case 1:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[0].ColumnName;
                                                break;
                                            case 2:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[1].ColumnName;
                                                break;
                                        }

                                        for (int f = 0; f < dtCmb.Columns.Count; f++)
                                        {
                                            ListBoxColumn lstCol = new ListBoxColumn();
                                            lstCol.FieldName = dtCmb.Columns[f].ColumnName;
                                            lstCol.Caption = dtCmb.Columns[f].ColumnName;
                                            if (f == 0)
                                                lstCol.Width = 60;
                                            else
                                                lstCol.Width = 250;
                                            c.PropertiesComboBox.Columns.Add(lstCol);
                                        }

                                    }

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 3:                             //Date
                                {
                                    GridViewDataDateColumn c = new GridViewDataDateColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 4:                             //Memo
                                {
                                    GridViewDataMemoColumn c = new GridViewDataMemoColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 5:                             //Color
                                {
                                    GridViewDataColorEditColumn c = new GridViewDataColorEditColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 6:                             //Text
                                {
                                    GridViewDataTextColumn c = new GridViewDataTextColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    //if (col.MaxLength != -1) c.PropertiesTextEdit.MaxLength = col.MaxLength;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 7:                             //Numeric
                                {
                                    GridViewDataSpinEditColumn c = new GridViewDataSpinEditColumn();
                                    //c.Name = colName;
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 8:                             //Time
                                {
                                    GridViewDataTimeEditColumn c = new GridViewDataTimeEditColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    //c.PropertiesTimeEdit.ClientSideEvents.KeyDown = "function(s, e) { TestGigi(s,e) }";

                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;

                                    c.PropertiesTimeEdit.DisplayFormatString = "HH:mm";
                                    c.PropertiesTimeEdit.DisplayFormatInEditMode = true;
                                    c.PropertiesTimeEdit.EditFormatString = "HH:mm";
                                    c.PropertiesTimeEdit.EditFormat = EditFormat.DateTime;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
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

        #region Pontaj Init

   

        #endregion

        protected void grDate_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e)
        {
            e.Properties["cpIsEditing"] = grDate.IsEditing;
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Tactil/Main.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../DefaultTactil.aspx", false);
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
                IncarcaDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void IncarcaDate()
        {
            IncarcaGrid();

            //DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

            if (!IsPostBack)
            {
                //luna = Convert.ToInt32(dt010.Rows[0][1].ToString());
                //an = Convert.ToInt32(dt010.Rows[0][0].ToString());
                luna = DateTime.Now.Month;
                an = DateTime.Now.Year;
            }

            string func = "LEN";
            if (Constante.tipBD == 2)
                func = "LENGTH";

            DataTable dtDen = General.IncarcaDT("SELECT \"Denumire\" FROM \"Ptj_tblPrint\" where \"Activ\" = 1 and \"Denumire\" like 'F%' and " + func + "(\"Denumire\") <= 3", null);
            if (dtDen != null && dtDen.Rows.Count > 0)
            {
                string listaDen = "";
                for (int i = 0; i < dtDen.Rows.Count; i++)
                {
                    listaDen += dtDen.Rows[i][0].ToString();
                    if (i < dtDen.Rows.Count - 1)
                        listaDen += ", ";
                }

                string camp = "CONVERT(INT, Valoare)";
                if (Constante.tipBD == 2)
                    camp = "ROUND(\"Valoare\")";

                string sql = "SELECT distinct p.\"Id\", \"An\", \"Luna\", F10003, " + camp + " AS \"Valoare\", COALESCE(r.\"Explicatii\", p.\"TextAfisare\") as \"TextAfisare\" FROM  (SELECT \"An\", \"Luna\", F10003, " + listaDen + " FROM \"Ptj_Cumulat\" "
                    + " where \"An\" = " + an + " and \"Luna\" = " + luna + ") p UNPIVOT  (\"Valoare\" FOR \"Denumire\" IN  (" + listaDen + ") )AS unpvt "
                    + " left join \"Ptj_tblPrint\" p on p.\"Denumire\" = unpvt.\"Denumire\" left join \"Ptj_tblFormuleCumulat\" r on r.\"Coloana\" = unpvt.\"Denumire\"   where \"Activ\" = 1 and p.\"Denumire\" like 'F%' and " + func + "(p.\"Denumire\") <= 3 and f10003 = "
                    + Session["User_Marca"].ToString() + " AND \"An\" = " + an + " AND \"Luna\" = " + luna;

                grDateTotaluri.SettingsPager.PageSize = 40;
                DataTable dtTot = General.IncarcaDT(sql, null);
                grDateTotaluri.DataSource = dtTot;
                grDateTotaluri.KeyFieldName = "TextAfisare";
                grDateTotaluri.DataBind();
                Session["InformatiaCurenta_Totaluri"] = dtTot;
            }
        }

    }
}