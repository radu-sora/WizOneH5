using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Avs
{
    public partial class Detalii : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion

                               

                if (!IsPostBack)
                {
                    int qwe = Convert.ToInt32(General.Nz(Request["qwe"], -99));
                    DataTable dt = General.IncarcaDT("SELECT * FROM \"Avs_Cereri\" WHERE \"Id\" = " + qwe, null);
                    IncarcaGrid(dt);
                }
     
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        private void IncarcaGrid(DataTable dt)
        {
            DataTable dtComp = new DataTable();
            switch (Convert.ToInt32(dt.Rows[0]["IdAtribut"].ToString()))
            {
                case (int)Constante.Atribute.Sporuri:
                    string cmp = "ISNULL";
                    string sql = "";
                    string sir = dt.Rows[0]["Tarife"].ToString();
                    if (Constante.tipBD == 2)
                        cmp = "NVL";

                    for (int i = 0; i <= 19; i++)
                    {
                        string val = "0";
                        DataTable dtTemp = General.IncarcaDT("select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + dt.Rows[0]["Spor" + i].ToString(), null);
                        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                            val = dtTemp.Rows[0][0].ToString();

                        if (Constante.tipBD == 1)
                            sql += "select " + (i + 1).ToString() + " as \"Coloana0\", CASE WHEN \"Spor" + i + "\" = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT TOP 1 F02505 FROM F025 WHERE F02504 = \"Spor" + i + "\") END as \"Coloana1\", "                            
                                + " CASE WHEN(case when \"Spor" + i + "\" = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                                + cmp + "((select top 1 f01107 from f025 "
                                + " left join f021 on f02510 = f02104 "
                                + " left join f011 on f02106 = f01104 "
                                + " where f02504 = \"Spor" + i + "\" and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + "), '---')  END as \"Coloana2\" "
                                + " from \"Avs_Cereri\" where f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString();
                        else
                            sql += "select " + (i + 1).ToString() + " as \"Coloana0\",  CASE WHEN \"Spor" + i + "\" = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT F02505 FROM F025 WHERE F02504 = \"Spor" + i + "\" AND ROWNUM <= 1) END as \"Coloana1\", "                              
                                + " CASE WHEN(case when \"Spor" + i + "\" = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                                + cmp + "((select f01107 from f025 "
                                + " left join f021 on f02510 = f02104 "
                                + " left join f011 on f02106 = f01104 "
                                + " where f02504 = \"Spor" + i + "\" and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " AND ROWNUM <= 1), '---')  END as \"Coloana2\" "
                                + " from \"Avs_Cereri\" where f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString();


                        if (i < 19)
                            sql += " UNION ";
                    }            

                    dt = General.IncarcaDT(sql, null);
                 
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Coloana0"] };                   
            
                    grDate.KeyFieldName = "Coloana0";
                    grDate.DataSource = dt;
                    grDate.SettingsPager.PageSize = 20;
                    grDate.DataBind();
                    foreach (dynamic c in grDate.Columns)
                    {
                        try
                        {
                            if (c.Caption == "Coloana1")
                                c.Caption = Dami.TraduCuvant("Spor");
                            if (c.Caption == "Coloana2")
                                c.Caption = Dami.TraduCuvant("Tarif");
                        }
                        catch (Exception) { }
                    }
                    break;
                case (int)Constante.Atribute.SporTranzactii:                   
                    sql = "";
                    //Florin 2019.10.28 - am comentat partea de Oracle deoarece este acelasi select si pe sql si pe oracle
                    sql = $@"SELECT 0 as F02104, '---' AS F02105 {(Constante.tipBD == 2 ? " FROM DUAL" : "")} UNION SELECT F02104, F02105 FROM F021 WHERE F02162 IS NOT NULL AND F02162 <> 0";
                    string tabela = @"SELECT 0 as F02104, '---' AS F02105 UNION SELECT F02104, F02105 FROM F021 WHERE F02162 IS NOT NULL AND F02162 <> 0";
                    //if (Constante.tipBD == 2)
                    //    tabela = "SELECT 0 as F02104, '---' AS F02105 FROM DUAL UNION " + General.SelectOracle("F021", "F02104") + " WHERE F02162 IS NOT NULL AND F02162 <> 0 ";
                    for (int i = 0; i <= 19; i++)
                    {
                        sql += "select " + i.ToString() + " as \"Coloana0\", \"SporTran" + i + "\" as \"Coloana1\", CASE WHEN \"SporTran" + i + "\" = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT F02105 FROM (" + tabela + ") a WHERE F02104 = \"SporTran" + i + "\") END as \"Coloana2\" from \"Avs_Cereri\" where f10003 = " 
                            + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString();
                        ;
                        if (i < 19)
                            sql += " UNION ";
                    }

                    dt = General.IncarcaDT(sql, null);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Coloana0"] };             
            
                    grDate.KeyFieldName = "Coloana0";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    grDate.SettingsPager.PageSize = 20;
                    foreach (dynamic c in grDate.Columns)
                    {
                        try
                        {
                            if (c.Caption == "Coloana1")
                                c.Caption = Dami.TraduCuvant("Cod");
                            if (c.Caption == "Coloana2")
                                c.Caption = Dami.TraduCuvant("Spor");
                        }
                        catch (Exception) { }
                    }
                    break;
                case (int)Constante.Atribute.Componente:
                    sql = " select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp0\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4001 and \"Comp0\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp1\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4002 and \"Comp1\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp2\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4003 and \"Comp2\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp3\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4004 and \"Comp3\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp4\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4005 and \"Comp4\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp5\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4006 and \"Comp5\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp6\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4007 and \"Comp6\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp7\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4008 and \"Comp7\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp8\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4009 and \"Comp8\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString()
                                 + "union "
                                 + "select F02104 as \"Coloana0\", F02105 as \"Coloana1\", \"Comp9\" as \"Coloana2\" from f021 join \"Avs_Cereri\" on f02104 = 4010 and \"Comp9\" > 0 and f10003 = " + dt.Rows[0]["F10003"].ToString() + " AND \"Id\" = " + dt.Rows[0]["Id"].ToString() + " ORDER BY F02104";
                    dtComp = General.IncarcaDT(sql, null);
                    grDate.KeyFieldName = "Coloana0";                    
                    grDate.DataSource = dtComp;
                    grDate.DataBind();
                    foreach (dynamic c in grDate.Columns)
                    {
                        try
                        {
                            if (c.Caption == "Coloana1")
                                c.Caption = Dami.TraduCuvant("Componenta");
                            if (c.Caption == "Coloana2")
                                c.Caption = Dami.TraduCuvant("Suma");
                        }
                        catch (Exception) { }
                    }
                    break;
                case (int)Constante.Atribute.Tarife:
                    sir = dt.Rows[0]["Tarife"].ToString();
                    string sqlFinal = "", cond = "";
                    sql = "SELECT (SELECT TOP 1 b.F01107 FROM F011 b WHERE b.F01104 = a.F01104) AS \"Coloana1\", "
                            + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"Coloana2\" FROM F011 a ";
                    string col0 = "SELECT CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS \"Coloana0\", a.* FROM (";

                    if (Constante.tipBD == 2)
                    {
                        sql = "SELECT (SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND ROWNUM = 1) AS \"Coloana1\", "
                            + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"Coloana2\" FROM F011 a ";
                        col0 = "SELECT ROWNUM AS \"Coloana0\", a.* FROM (";
                    }
                    for (int i = 0; i < sir.Length; i++)
                        if (sir[i] != '0')
                        {
                            cond = "WHERE (a.F01104 = " + (i + 1).ToString() + " AND a.F01105 = " + sir[i] + ") ";
                            sqlFinal += (sqlFinal.Length <= 0 ? "" : " UNION ") + sql + cond;
                        }

                    if (sqlFinal.Length > 0)
                        dt = General.IncarcaDT(col0 + sqlFinal + ") a ", null);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Coloana0"] };                
                    grDate.KeyFieldName = "Coloana0";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    foreach (dynamic c in grDate.Columns)
                    {
                        try
                        {
                            if (c.Caption == "Coloana1")
                                c.Caption = Dami.TraduCuvant("Categorie");
                            if (c.Caption == "Coloana2")
                                c.Caption = Dami.TraduCuvant("Tarif");
                        }
                        catch (Exception) { }
                    }
                    break;
            }


        }



    }
}