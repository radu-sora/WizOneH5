using DevExpress.Web;
using ProceseSec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class VerificareBD : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtRef = new DataTable();
            try
            {
                Dami.AccesApp();


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                //btnClose.Text = Dami.TraduCuvant("btnClose", "Inchidere luna");
                //btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare luna");

                #endregion

                string[] file1 = File.ReadAllLines(HostingEnvironment.MapPath("~/Fisiere/" + "BDReferintaSQL.csv"));

                if (Constante.tipBD == 2)
                    file1 = File.ReadAllLines(HostingEnvironment.MapPath("~/Fisiere/" + "BDReferintaORA.csv"));

                // General.ExecutaNonQuery("DROP TABLE \"StructuraRef\"", null);

                dtRef.Columns.Add("IdAuto", typeof(int));
                dtRef.Columns.Add("TABLE_NAME_REF", typeof(string));
                dtRef.Columns.Add("COLUMN_NAME_REF", typeof(string));
                dtRef.Columns.Add("COLUMN_DEFAULT_REF", typeof(string));
                dtRef.Columns.Add("IS_NULLABLE_REF", typeof(string));
                dtRef.Columns.Add("DATA_TYPE_REF", typeof(string));
                dtRef.Columns.Add("CHARACTER_MAXIMUM_LENGTH_REF", typeof(int));
                dtRef.Columns.Add("NUMERIC_PRECISION_REF", typeof(int));
                dtRef.Columns.Add("NUMERIC_SCALE_REF", typeof(int));
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i].Split(',').Length > 8)
                    {
                        dtRef.Rows.Add(i + 1, file1[i].Split(',')[0], file1[i].Split(',')[1], file1[i].Split(',')[2] + file1[i].Split(',')[3] + file1[i].Split(',')[4],
                        file1[i].Split(',')[5], file1[i].Split(',')[6], file1[i].Split(',')[7] == "NULL" ? null : file1[i].Split(',')[7],
                        file1[i].Split(',')[8] == "NULL" ? null : file1[i].Split(',')[8], file1[i].Split(',')[9] == "NULL" ? null : file1[i].Split(',')[9]);
                    }
                    else
                        dtRef.Rows.Add(i + 1, file1[i].Split(',')[0], file1[i].Split(',')[1], file1[i].Split(',')[2] == "NULL" ? null : file1[i].Split(',')[2],
                            file1[i].Split(',')[3], file1[i].Split(',')[4], file1[i].Split(',')[5] == "NULL" ? null : file1[i].Split(',')[5],
                            file1[i].Split(',')[6] == "NULL" ? null : file1[i].Split(',')[6], file1[i].Split(',')[7] == "NULL" ? null : file1[i].Split(',')[7]);
                }

                //General.ExecutaNonQuery("CREATE TABLE \"StructuraRef\" (TABLE_NAME_REF VARCHAR(128), COLUMN_NAME_REF VARCHAR(128), COLUMN_DEFAULT_REF VARCHAR(256), IS_NULLABLE_REF VARCHAR(3), " 
                //                     + " DATA_TYPE_REF VARCHAR(20), CHARACTER_MAXIMUM_LENGTH_REF INT, NUMERIC_PRECISION_REF INT, NUMERIC_SCALE_REF INT) ", null);

                //for (int i = 0; i < dtRef.Rows.Count; i++)
                //    General.ExecutaNonQuery("INSERT INTO \"StructuraRef\" VALUES ('" + dtRef.Rows[i][0].ToString() + "', '" + dtRef.Rows[i][1].ToString() + "', '" + dtRef.Rows[i][2].ToString() + "', '" 
                //        + dtRef.Rows[i][3].ToString() + "', '" + dtRef.Rows[i][4].ToString() + "', " + dtRef.Rows[i][5].ToString() + ", " + dtRef.Rows[i][6].ToString() + ", " + dtRef.Rows[i][7].ToString() + ")", null);

                string sql = "";
                if (Constante.tipBD == 1)
                    sql = "select  a.* from ("
                    + "select  TABLE_NAME as TABLE_NAME_CLIENT, COLUMN_NAME AS COLUMN_NAME_CLIENT, "
                    + " ISNULL(COLUMN_DEFAULT, '') AS COLUMN_DEFAULT_CLIENT, ISNULL(IS_NULLABLE, '') AS IS_NULLABLE_CLIENT, ISNULL(DATA_TYPE, '') AS DATA_TYPE_CLIENT, ISNULL(CHARACTER_MAXIMUM_LENGTH, '') AS CHARACTER_MAX_LENGTH_CLIENT, " 
                    + "CONVERT(INT, ISNULL(NUMERIC_PRECISION, 0)) AS NUMERIC_PRECISION_CLIENT, CONVERT(INT, ISNULL(NUMERIC_SCALE, 0)) AS NUMERIC_SCALE_CLIENT from INFORMATION_SCHEMA.COLUMNS "
                    + " where TABLE_NAME not in ('ADDRESS', 'BENEFICII', 'CAEN_ITM', 'CCCLIENT', 'CLIENTI', 'CODURIPOSTALE', 'COMPACC', 'CONTRACTE', 'COR_ITM', 'CORECTIE', 'CRTEVAL', "
                                          + "  'CURS_PENSIE_F', 'DAILYCHANGES', 'DEPCLNT', 'ETICHETE', 'F1002', 'HOLIDAYS', 'JUDETE', 'LOCALITATI', 'LOCATIE_MUNCA', 'LOCATII', 'MARDEF', 'MARTABLE', "
                                          + "  'NOTACONT', 'NOTAPLAUTO', 'NROP_SOMAJ', 'ORASE_ITM', 'PENSIE_F', 'PERSCONT', 'PLAN_TABLE', 'PLCLIENT', 'POZE', 'PROCESE', 'PSTCLNT', "
                                          + "  'RAPORT_DATE_ANGAJAT_TBL', 'REGISTRU_VECHIME', 'REPORTS', 'RSCPST', 'SALARIATI', 'SCHCLNT', 'SCRACC', 'SPORCLNT', 'SPORURI', 'SPVCLNT', "
                                          + "   'STATUS', 'TABLEDEN', 'TAXE', 'TAXE_ARHIVA', 'USERHIST', 'ZileHand', 'F100', 'F099', 'F1001', 'F0991') "
                                          + "  and TABLE_NAME not like  'D00%' and TABLE_NAME not like 'D100%' and TABLE_NAME not like 'D110%' and TABLE_NAME not like 'D205%' and TABLE_NAME not like 'F2%' "
                                          + "  and TABLE_NAME not like 'F3%' and TABLE_NAME not like 'F4%' and TABLE_NAME not like 'F5%' and TABLE_NAME not like 'F7%' and TABLE_NAME not like 'F9%' "
                                          + "  and TABLE_NAME not like 'EMPL%' and TABLE_NAME not like 'ANG_%'  and TABLE_NAME not like 'TEST%' "
                                          + "  and TABLE_NAME not like 'H0%' and TABLE_NAME not like 'H1%' and TABLE_NAME not like 'REP_%' and TABLE_NAME not like 'WT_%' and TABLE_NAME not like '%TMP%' and TABLE_NAME not in (select TABLE_NAME from INFORMATION_SCHEMA.VIEWS) "

                                          + " UNION "

                                          + "   select 'VIEW: ' + TABLE_NAME, COLUMN_NAME, '' AS COLUMN_DEFAULT_CLIENT, '' AS IS_NULLABLE,  '' AS DATA_TYPE, " 
                                          + " 0 as CHARACTER_MAXIMUM_LENGTH,  0 as NUMERIC_PRECISION, 0 NUMERIC_SCALE from INFORMATION_SCHEMA.COLUMNS  where TABLE_NAME in (select TABLE_NAME from INFORMATION_SCHEMA.VIEWS)"

                                          + " UNION "

                                          + "select  routine_type as TABLE_NAME, ROUTINE_NAME as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                                          + " '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                                          + " from information_schema.routines  where routine_type = 'FUNCTION' "

                                            + " UNION "

                                          + "select  'SEQUENCE' as TABLE_NAME, SEQUENCE_NAME as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                                          + " '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                                          + " from information_schema.SEQUENCES  "

                                          + " union "

                                          + " select  'tblParametrii' as TABLE_NAME, Nume as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                                          + " '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                                          + " from tblParametrii "

                                          + " UNION "

                                            + " select  'tblMeniuri' as TABLE_NAME, Pagina as COLUMN_NAME, '' as COLUMN_DEFAULT,  '' as IS_NULLABLE, '' as DATA_TYPE, 0 as "
                                           + "    CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE  from tblMeniuri "

                                          + ") a order by TABLE_NAME_CLIENT, COLUMN_NAME_CLIENT";
                else
                {
                    string numeBaza = "";

                    CriptDecript prc = new CriptDecript();
                    Constante.cnnWeb = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);                   
                    string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                    numeBaza = tmp.Split(';')[0];
                  

                    sql = "select a.* from  ( "
                        + "select  TABLE_NAME AS TABLE_NAME_CLIENT, COLUMN_NAME AS COLUMN_NAME_CLIENT, '' AS COLUMN_DEFAULT_CLIENT, NVL(NULLABLE, '') AS IS_NULLABLE_CLIENT, NVL(DATA_TYPE, '') AS DATA_TYPE_CLIENT, " 
                        + " NVL(DATA_LENGTH, 0) AS CHARACTER_MAX_LENGTH_CLIENT, nvl(DATA_PRECISION, 0) AS NUMERIC_PRECISION_CLIENT, NVL(DATA_SCALE, 0) AS NUMERIC_SCALE_CLIENT from dba_tab_columns  where TABLE_NAME not in "
                        + "('ADDRESS', 'BENEFICII', 'CAEN_ITM', 'CCCLIENT', 'CLIENTI', 'CODURIPOSTALE', 'COMPACC', 'CONTRACTE', 'COR_ITM', 'CORECTIE', 'CRTEVAL', 'CURS_PENSIE_F', "
                        + " 'DAILYCHANGES', 'DEPCLNT', 'ETICHETE', 'F1002', 'JUDETE', 'LOCALITATI', 'LOCATIE_MUNCA', 'LOCATII', 'MARDEF', 'MARTABLE', 'NOTACONT', 'NOTAPLAUTO', "
                        + " 'NROP_SOMAJ', 'ORASE_ITM', 'PENSIE_F', 'PERSCONT', 'PLAN_TABLE', 'PLCLIENT', 'POZE', 'PROCESE', 'PSTCLNT', 'RAPORT_DATE_ANGAJAT_TBL', 'REGISTRU_VECHIME', "
                        + " 'REPORTS', 'RSCPST', 'SALARIATI', 'SCHCLNT', 'SCRACC', 'SPORCLNT', 'SPORURI', 'SPVCLNT', 'STATUS', 'TABLEDEN', 'TAXE', 'TAXE_ARHIVA', 'USERHIST', 'ZileHand', "
                        + "  'F100', 'F099', 'F1001', 'F0991')   and TABLE_NAME not like  'D00%' and TABLE_NAME not like 'D100%' and TABLE_NAME not like 'D110%' "
                        + "  and TABLE_NAME not like 'D205%' and TABLE_NAME not like 'F2%'   and TABLE_NAME not like 'F3%' and TABLE_NAME not like 'F4%' "
                        + "  and TABLE_NAME not like 'F5%' and TABLE_NAME not like 'F7%' and TABLE_NAME not like 'F9%'   and TABLE_NAME not like 'H0%' "
                        + "  and TABLE_NAME not like 'H1%' and TABLE_NAME not like 'REP_%' and TABLE_NAME not like 'WT_%'  and TABLE_NAME not like '%TMP%' "
                        + "  and TABLE_NAME not like 'EMPL%' and TABLE_NAME not like 'ANG_%'  and TABLE_NAME not like 'TEST%' "
                        + "  and TABLE_NAME not in (select view_NAME from dba_views where owner = '{0}') and owner = '{0}' "

                        + "  UNION "

                       + "   select 'VIEW: ' || TABLE_NAME, COLUMN_NAME,   ''  AS COLUMN_DEFAULT, '' AS IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, "
                       + " 0 AS NUMERIC_PRECISION, 0 AS NUMERIC_SCALE from dba_tab_columns where TABLE_NAME in (select view_NAME from  dba_views where owner = '{0}') and owner = '{0}' "

                       + " union "

                       + "   select  object_type as TABLE_NAME, OBJECT_NAME as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                        + "  '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                        + "    from dba_objects  where owner = '{0}' and object_type = 'PROCEDURE' "


                        + "    UNION "
                       + "       select  object_type as TABLE_NAME, OBJECT_NAME as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                        + "  '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                        + "    from dba_objects  where owner = '{0}' and object_type = 'FUNCTION' "


                        + "        UNION "
                        + "      select  object_type as TABLE_NAME, OBJECT_NAME as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                        + "  '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                        + "    from dba_objects  where owner = '{0}' and object_type = 'SEQUENCE' "


                         + "   union "

                        + "          select  'tblParametrii' as TABLE_NAME, \"Nume\" as COLUMN_NAME, '' as COLUMN_DEFAULT, "
                        + "  '' as IS_NULLABLE, '' as DATA_TYPE, 0 as CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE "
                        + "    from \"tblParametrii\" "

                        + " UNION "

                        + " select  'tblMeniuri' as TABLE_NAME, \"Pagina\" as COLUMN_NAME, '' as COLUMN_DEFAULT,  '' as IS_NULLABLE, '' as DATA_TYPE, 0 as "
                       + "    CHARACTER_MAXIMUM_LENGTH, 0 as NUMERIC_PRECISION, 0 as NUMERIC_SCALE  from \"tblMeniuri\" "

                         + "   ) a  order by TABLE_NAME_CLIENT, COLUMN_NAME_CLIENT ";
                    sql = string.Format(sql, numeBaza);
                }

                DataTable dtClient = General.IncarcaDT(sql, null);


                DataColumn[] dc1 = new DataColumn[2], dc2 = new DataColumn[2];
                for (int i = 1; i < 3; i++)
                    dc1[i - 1] = dtRef.Columns[i];
                for (int i = 1; i < 3; i++)
                    dc2[i - 1] = dtClient.Columns[i - 1];


                //var res = from bdRef in dtRef.AsEnumerable()
                //          join bdClient in dtClient.AsEnumerable()
                //            on bdRef.Field<string>("TABLE_NAME_REF") equals bdClient.Field<string>("TABLE_NAME_CLIENT")
                //          select bdRef;

                ////myRow.Field<int>("RowNo")


                //var leftOuterJoin =
                //    from bdRef in dtRef.AsEnumerable()
                //    join bdClient in dtClient.AsEnumerable() on new { TableName = bdRef.Field<string>("TABLE_NAME_REF"), ColumnName =  bdRef.Field<string>("COLUMN_NAME_REF") } 
                //        equals new { TableName = bdClient.Field<string>("TABLE_NAME_CLIENT"), ColumnName = bdClient.Field<string>("COLUMN_NAME_CLIENT") } into temp
                //    from bdClient in temp.DefaultIfEmpty()
                //    select new
                //    {
                //        TableNameRef = bdRef.Field<string>("TABLE_NAME_REF"),
                //        ColumnNameRef = bdRef.Field<string>("COLUMN_NAME_REF"),
                //        ColumnDefaultRef = bdRef.Field<string>("COLUMN_DEFAULT_REF"),
                //        IsNullableRef = bdRef.Field<string>("IS_NULLABLE_REF"),
                //        DataTypeRef = bdRef.Field<string>("DATA_TYPE_REF"),
                //        CharMaxLengthRef = bdRef.Field<int?>("CHARACTER_MAXIMUM_LENGTH_REF"),
                //        NumPrecRef = bdRef.Field<int?>("NUMERIC_PRECISION_REF"),
                //        NumScaleRef = bdRef.Field<int?>("NUMERIC_SCALE_REF"),
                //        TableNameClient = bdClient.Field<string>("TABLE_NAME_CLIENT"),
                //        ColumnNameClient = bdClient.Field<string>("COLUMN_NAME_CLIENT"),
                //        ColumnDefaultClient = bdClient.Field<string>("COLUMN_DEFAULT_CLIENT"),
                //        IsNullableClient = bdClient.Field<string>("IS_NULLABLE_CLIENT"),
                //        DataTypeClient = bdClient.Field<string>("DATA_TYPE_CLIENT"),
                //        CharMaxLengthClient = bdClient.Field<int?>("CHARACTER_MAXIMUM_LENGTH_CLIENT"),
                //        NumPrecClient = bdClient.Field<int?>("NUMERIC_PRECISION_CLIENT"),
                //        NumScaleClient = bdClient.Field<int?>("NUMERIC_SCALE_CLIENT")
                //    };
                //var rightOuterJoin =
                //    from bdClient in dtClient.AsEnumerable()
                //    join bdRef in dtRef.AsEnumerable() on new { TableName = bdClient.Field<string>("TABLE_NAME_CLIENT"), ColumnName = bdClient.Field<string>("TABLE_NAME_CLIENT") }
                //        equals new { TableName = bdRef.Field<string>("TABLE_NAME_REF"), ColumnName = bdRef.Field<string>("TABLE_NAME_REF") } into temp
                //    from bdRef in temp.DefaultIfEmpty()
                //    select new
                //    {
                //        TableNameRef = bdRef.Field<string>("TABLE_NAME_REF"),
                //        ColumnNameRef = bdRef.Field<string>("COLUMN_NAME_REF"),
                //        ColumnDefaultRef = bdRef.Field<string>("COLUMN_DEFAULT_REF"),
                //        IsNullableRef = bdRef.Field<string>("IS_NULLABLE_REF"),
                //        DataTypeRef = bdRef.Field<string>("DATA_TYPE_REF"),
                //        CharMaxLengthRef = bdRef.Field<int?>("CHARACTER_MAXIMUM_LENGTH_REF"),
                //        NumPrecRef = bdRef.Field<int?>("NUMERIC_PRECISION_REF"),
                //        NumScaleRef = bdRef.Field<int?>("NUMERIC_SCALE_REF"),
                //        TableNameClient = bdClient.Field<string>("TABLE_NAME_CLIENT"),
                //        ColumnNameClient = bdClient.Field<string>("COLUMN_NAME_CLIENT"),
                //        ColumnDefaultClient = bdClient.Field<string>("COLUMN_DEFAULT_CLIENT"),
                //        IsNullableClient = bdClient.Field<string>("IS_NULLABLE_CLIENT"),
                //        DataTypeClient = bdClient.Field<string>("DATA_TYPE_CLIENT"),
                //        CharMaxLengthClient = bdClient.Field<int?>("CHARACTER_MAXIMUM_LENGTH_CLIENT"),
                //        NumPrecClient = bdClient.Field<int?>("NUMERIC_PRECISION_CLIENT"),
                //        NumScaleClient = bdClient.Field<int?>("NUMERIC_SCALE_CLIENT")
                //    };
                //var fullOuterJoin = leftOuterJoin.Union(rightOuterJoin);



                DataTable dtJoin = Join(dtRef, dtClient, dc1, dc2);
                //DataTable dtJoin = Join(dtRef, dtClient, null, null);


                DataTable dtRezultat = new DataTable();
                foreach (DataColumn col in dtJoin.Columns)
                {
                    dtRezultat.Columns.Add(col.ColumnName, col.DataType);
                }

                for (int i = 0; i < dtJoin.Rows.Count; i++)
                    if (dtJoin.Rows[i]["TABLE_NAME_REF"].ToString().ToUpper() != dtJoin.Rows[i]["TABLE_NAME_CLIENT"].ToString().ToUpper()
                        || dtJoin.Rows[i]["COLUMN_NAME_REF"].ToString().ToUpper() != dtJoin.Rows[i]["COLUMN_NAME_CLIENT"].ToString().ToUpper()
                        || dtJoin.Rows[i]["COLUMN_DEFAULT_REF"].ToString().ToUpper() != dtJoin.Rows[i]["COLUMN_DEFAULT_CLIENT"].ToString().ToUpper()
                        || dtJoin.Rows[i]["IS_NULLABLE_REF"].ToString() != dtJoin.Rows[i]["IS_NULLABLE_CLIENT"].ToString()
                        || dtJoin.Rows[i]["DATA_TYPE_REF"].ToString() != dtJoin.Rows[i]["DATA_TYPE_CLIENT"].ToString()
                        || dtJoin.Rows[i]["CHARACTER_MAXIMUM_LENGTH_REF"].ToString() != dtJoin.Rows[i]["CHARACTER_MAX_LENGTH_CLIENT"].ToString()
                        || dtJoin.Rows[i]["NUMERIC_PRECISION_REF"].ToString() != dtJoin.Rows[i]["NUMERIC_PRECISION_CLIENT"].ToString()
                        || dtJoin.Rows[i]["NUMERIC_SCALE_REF"].ToString() != dtJoin.Rows[i]["NUMERIC_SCALE_CLIENT"].ToString())
                        dtRezultat.ImportRow(dtJoin.Rows[i]);

                //grDate.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords;

                for (int i = 0; i < grDate.Columns.Count; i++)
                    if (grDate.Columns[i].Name.Contains("REF"))
                        grDate.Columns[i].HeaderStyle.BackColor = Color.FromArgb(255, 255, 179, 128);

                grDate.SettingsPager.PageSize = 25;
                grDate.DataSource = dtRezultat;
                grDate.DataBind();
           



            }
            catch (Exception ex)
            {
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        public static DataTable Join(DataTable First, DataTable Second, DataColumn[] FJC, DataColumn[] SJC)
        {
            //Create Empty Table
            DataTable table = new DataTable("Join");

            // Use a DataSet to leverage DataRelation
            using (DataSet ds = new DataSet())
            {
                //Add Copy of Tables
                ds.Tables.AddRange(new DataTable[] { First.Copy(), Second.Copy() });

                //Identify Joining Columns from First                               
                DataColumn[] parentcolumns = new DataColumn[FJC != null ? FJC.Length : 0];
                for (int i = 0; i < parentcolumns.Length; i++)
                {
                    parentcolumns[i] = ds.Tables[0].Columns[FJC[i].ColumnName];
                }
                //Identify Joining Columns from Second
                DataColumn[] childcolumns = new DataColumn[SJC != null ? SJC.Length : 0];
                for (int i = 0; i < childcolumns.Length; i++)
                {
                    childcolumns[i] = ds.Tables[1].Columns[SJC[i].ColumnName];
                }

                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, parentcolumns, childcolumns, false);
                ds.Relations.Add(r);

                //Create Columns for JOIN table
                for (int i = 0; i < First.Columns.Count; i++)
                {
                    table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
                }
                for (int i = 0; i < Second.Columns.Count; i++)
                {
                    //Beware Duplicates
                    if (!table.Columns.Contains(Second.Columns[i].ColumnName))
                        table.Columns.Add(Second.Columns[i].ColumnName, Second.Columns[i].DataType);
                    else
                        table.Columns.Add(Second.Columns[i].ColumnName + "_Second", Second.Columns[i].DataType);
                }

                //Loop through First table
                table.BeginLoadData();               
                foreach (DataRow firstrow in ds.Tables[0].Rows)
                {                  
                    //Get "joined" rows                  
                    DataRow[] childrows = firstrow.GetChildRows(r);
                    if (childrows != null && childrows.Length > 0)
                    {
                        object[] parentarray = firstrow.ItemArray;
                        foreach (DataRow secondrow in childrows)
                        {                    
                            object[] secondarray = secondrow.ItemArray;
                            object[] joinarray = new object[parentarray.Length + secondarray.Length];
                            Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                            Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);
                            table.LoadDataRow(joinarray, true);                     
                        }
                    }
                    else
                    {
                        object[] parentarray = firstrow.ItemArray;  
                        object[] joinarray = new object[parentarray.Length + parentarray.Length - 1];
                        Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                        //Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);
                        table.LoadDataRow(joinarray, true);
                        
                    }
                 
                }
                table.EndLoadData();
            }

            return table;
        }



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                //txtFisier1.Text = "";
                //txtFisier2.Text = "";

                //string[] tabeleWizSalary = { "ADDRESS", "BENEFICII", "CAEN_ITM", "CCCLIENT", "CLIENTI", "CODURIPOSTALE", "COMPACC", "CONTRACTE", "COR_ITM", "CORECTIE", "CRTEVAL",
                //                            "CURS_PENSIE_F", "DAILYCHANGES", "DEPCLNT", "ETICHETE", "F1002", "JUDETE", "LOCALITATI", "LOCATIE_MUNCA", "LOCATII", "MARDEF", "MARTABLE",
                //                            "NOTACONT", "NOTAPLAUTO", "NROP_SOMAJ", "ORASE_ITM", "PENSIE_F", "PERSCONT", "PLAN_TABLE", "PLCLIENT", "POZE", "PROCESE", "PSTCLNT",
                //                            "RAPORT_DATE_ANGAJAT_TBL", "REGISTRU_VECHIME", "REPORTS", "RSCPST", "SALARIATI", "SCHCLNT", "SCRACC", "SPORCLNT", "SPORURI", "SPVCLNT",
                //                             "STATUS", "TABLEDEN", "TAXE", "TAXE_ARHIVA", "USERHIST", "ZileHand", "F100", "F099"};
                //string[] tabeleGenWizSalary = { "D00", "D100", "D110", "D205", "F2", "F3", "F4", "F5", "F7", "F9", "H0", "H1", "REP_", "WT_" };

                //string path = "";
                //if (Session["CaleFisierBDClient"] != null)
                //    path = Session["CaleFisierBDClient"].ToString();
                //else
                //    return;

                //string[] file2 = File.ReadAllLines(path);


                //bool startComp = false, diff = false;
                //string[] temp = new string[1000];
                //int index = -1, nrLinii = 0;
                //foreach (var line in File.ReadLines(HostingEnvironment.MapPath("~/Fisiere/" + "BDReferinta.sql"))) 
                //{
                //    if (line.Contains("GO"))
                //    {
                //        startComp = false;
                //        if (diff)
                //            for (int i = 0; i < tabeleWizSalary.Length; i++)
                //                if (temp[0].Contains("[" + tabeleWizSalary[i] + "]"))
                //                {
                //                    diff = false;
                //                    break;
                //                }
                //        if (diff)
                //            for (int i = 0; i < tabeleGenWizSalary.Length; i++)
                //                if (temp[0].Contains(tabeleGenWizSalary[i]))
                //                {
                //                    diff = false;
                //                    break;
                //                }

                //        if (diff)
                //        {
                //            bool spatii = false;
                //            if (index > 0)
                //            {
                //                Dictionary<string, string> listaRef = new Dictionary<string, string>();
                //                Dictionary<string, string> listaClient = new Dictionary<string, string>();

                //                for (int j = 0; j < nrLinii; j++)
                //                {
                //                    if (temp[j].Contains("PRIMARY"))
                //                        break;
                //                    else
                //                        if (!temp[j].Contains("CREATE TABLE"))
                //                        listaRef.Add(temp[j], temp[j]);
                //                }
                //                for (int j = index; j < index + nrLinii; j++)
                //                {
                //                    if (file2.ElementAt(j).Contains("PRIMARY"))
                //                        break;
                //                    else
                //                        if (!file2.ElementAt(j).Contains("CREATE TABLE"))
                //                        listaClient.Add(file2.ElementAt(j), file2.ElementAt(j));
                //                }
                //                var keysRef = listaRef.Keys.ToList();
                //                foreach (var key in keysRef)
                //                {
                //                    if (listaClient.ContainsKey(key))
                //                    {
                //                        listaRef.Remove(key);
                //                        listaClient.Remove(key);
                //                    }
                //                }
                //            }



                //            for (int i = 0; i < nrLinii; i++)
                //                txtFisier1.Text += temp[i] + Environment.NewLine;
                //            if (index > 0)
                //            {       
                //                for (int i = index; i < index + nrLinii; i++)
                //                {
                //                    if (file2.ElementAt(i).Contains("GO"))
                //                    {
                //                        spatii = true;
                //                        for (int j = i; j < index + nrLinii; j++)
                //                            file2[j] = "";
                //                    }
                //                    txtFisier2.Text += (spatii ? "" : file2.ElementAt(i)) + Environment.NewLine;
                //                }
                //                if (file2.ElementAt(index + nrLinii - 3).Length <= 0 && file2.ElementAt(index + nrLinii - 2).Length <= 0 && file2.ElementAt(index + nrLinii - 1).Length <= 0)
                //                    txtFisier2.Text += "" + Environment.NewLine;

                //                //if (file2.ElementAt(index + nrLinii - 1).Contains("PRIMARY") && temp[nrLinii - 2].Contains("PRIMARY"))
                //                //    txtFisier2.Text += "" + Environment.NewLine;
                //            }
                //            else
                //                for (int i = 0; i < nrLinii; i++)
                //                    txtFisier2.Text += "" + Environment.NewLine;
                //            txtFisier1.Text += "--------------------------------------------" + Environment.NewLine;
                //            txtFisier2.Text += "--------------------------------------------" + Environment.NewLine;
                //        }

                //        diff = false;
                //        nrLinii = 0;
                //        index = -1;
                //    }
                //    if (line.Contains("CREATE TABLE"))
                //    {
                //        if (file2.Contains(line))
                //        {
                //            temp = new string[1000];
                //            startComp = true;
                //            index = Array.IndexOf(file2, line);
                //        }
                //        else
                //        {
                //            diff = true;
                //            temp[nrLinii++] = line;
                //        }
                //    }
                //    if (startComp)
                //    {
                //        temp[nrLinii] = line;
                //        if (index > 0 && line.ToUpper() != file2.ElementAt(index + nrLinii).ToUpper())
                //        {
                //            diff = true;
                //        }
                //        nrLinii++;
                //    }
                //}
            }
            catch (Exception ex)
            {
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        //{
        //    try
        //    {       
        //        var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Temp/Temp"));
        //        if (!folder.Exists)
        //            folder.Create();

        //        foreach (FileInfo file in folder.GetFiles())
        //        {
        //            file.Delete();
        //        }

        //        string path = folder.FullName + "\\BDClient.txt";
        //        File.WriteAllBytes(path, btnDocUpload.UploadedFiles[0].FileBytes);
        //        Session["CaleFisierBDClient"] = path;


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}





    }


}