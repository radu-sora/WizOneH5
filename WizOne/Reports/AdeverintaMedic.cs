using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WizOne.Module;
using System.Web;
using System.Data;
using System.Globalization;
using System.Web.Hosting;
using System.IO;

namespace WizOne.Reports
{
    public partial class AdeverintaMedic : DevExpress.XtraReports.UI.XtraReport
    {
        public AdeverintaMedic()
        {
            InitializeComponent();            
           
        }

        private void AdeverintaMedic_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //HttpContext.Current.Session["User_Marca"] = "100009";

            if (HttpContext.Current.Session["User_Marca"] == null || Convert.ToInt32(HttpContext.Current.Session["User_Marca"].ToString()) < 0)
                return;

            string sql = "";
            //string op = "";
            if (Constante.tipBD == 1)
                sql = "SELECT F00204, F00207, F00281, F00282, 'Str. ' + F00233 + ', nr. ' + F00234 + ', ' + F00231 AS \"AdresaAngajator\", "
                                    + "F00241 + '/' + CONVERT(VARCHAR, F00242) + '/' + CONVERT(VARCHAR, F00243) AS \"NrOrdine\",  (select f06304 from f063 where f06302=f00284) as Casa FROM F002 WHERE F00202 = (SELECT F10002 FROM F100 WHERE F10003 = {0})";
            else
                sql = "SELECT F00204, F00207, F00281, F00282, 'Str. ' || F00233 || ', nr. ' || F00234 || ', ' || F00231 AS \"AdresaAngajator\", "
                                    + "F00241 || '/' || F00242 || '/' || F00243 AS \"NrOrdine\",  (select f06304 from f063 where f06302=f00284) as \"Casa\" FROM F002 WHERE F00202 = (SELECT F10002 FROM F100 WHERE F10003 = {0})";
            sql = string.Format(sql, HttpContext.Current.Session["User_Marca"].ToString());
            DataTable dt = General.IncarcaDT(sql, null);
                        
            this.DataSource = dt;

            int nrInreg = Convert.ToInt32(HttpContext.Current.Session["NrInregAdev"].ToString());
            xrLabel16.Text = "Nr. înregistrare: " + nrInreg;
            xrLabel12.Text = "Data: " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "." + DateTime.Now.Month.ToString().PadLeft(2, '0') + "." + DateTime.Now.Year;
                    
            if (Constante.tipBD == 2)
            {
                sql = "SELECT F10008 || ' ' || F10009 AS NUME, F10052, F100521, TO_CHAR(F100522, 'dd/mm/yyyy') AS F100522, TO_CHAR(F10022, 'dd/mm/yyyy') AS F10022, F10017, F100891, F10047 AS SEX, F100983, F100984 AS TIPCTR, F1009741 FROM F100 WHERE F10003=" + HttpContext.Current.Session["User_Marca"].ToString();
            }
            else
            {
                sql = "SELECT  F10008 + ' ' + F10009 AS NUME, F10052, F100521, CONVERT(VARCHAR, F100522, 103) AS F100522, CONVERT(VARCHAR, F10022, 103) AS F10022, F10017, F100891, F10047 AS SEX, F100983, F100984 AS TIPCTR, F1009741 FROM F100 WHERE F10003=" + HttpContext.Current.Session["User_Marca"].ToString();
            }
            DataTable dtAng = General.IncarcaDT(sql, null);

            string casa = dt.Rows[0]["F00204"].ToString() + "/" + dt.Rows[0]["Casa"].ToString();
            xrLabel3.Text = casa;
            

            string text = "                    Prin prezenta se certifică faptul că {0} {1}, CNP {2}, act de identitate seria {3} eliberat de {4}, cu domiciliul în județul {5} are calitate de persoană asigurată pentru concedii și indemnizații de asigurări sociale de sănătate în sistemul de asigurări sociale de sănătate, " 
                + " potrivit Ordonanței de urgență a Guvernului nr. 158/2005 privind concediile și indemizațiile de asigurări sociale de sănătate, aprobată cu modificări și completări prin Legea nr. 399/2006, cu modificările și completările ulterioare.";
      
            text = string.Format(text, (dtAng.Rows[0]["SEX"].ToString() == "1" ? "domnul" : "doamna"), dtAng.Rows[0]["NUME"].ToString(), dtAng.Rows[0]["F10017"].ToString(), dtAng.Rows[0]["F10052"].ToString(),
                                    dtAng.Rows[0]["F100521"].ToString(), dtAng.Rows[0]["F100891"].ToString());
            xrLabel14.Text = text;

            sql = "";
            if (Constante.tipBD == 2)
                sql = "SELECT TRIM(F11010)||' '||TRIM(F11005) AS NUME_COASIG, F11012 AS CNP_COASIG FROM F110 WHERE F11017 = 0 AND F11003=" + HttpContext.Current.Session["User_Marca"].ToString();
            else
                sql = "SELECT CAST(LTRIM(RTRIM(F11010)) AS VARCHAR(256)) +CAST(' ' AS VARCHAR(256))+CAST(LTRIM(RTRIM(F11005)) AS VARCHAR(256)) AS NUME_COASIG, F11012 AS CNP_COASIG FROM F110 WHERE F11017 = 0 AND F11003=" + HttpContext.Current.Session["User_Marca"].ToString();

            DataTable dtCoasig = General.IncarcaDT(sql, null);

            xrTable2.Borders = DevExpress.XtraPrinting.BorderSide.All;
            if (dtCoasig != null && dtCoasig.Rows.Count > 0)
                for (int x = 0; x < dtCoasig.Rows.Count; x++)
                {
                    XRTableRow row = new XRTableRow();
                    row.HeightF = 25f;
                    row.CanGrow = true;
                    row.CanShrink = true;
                    for (int j = 0; j <= dtCoasig.Columns.Count - 1; j++)
                    {
                        XRTableCell cell = new XRTableCell();
                        //cell.WidthF = latime[j];
                        if (j == 0)
                            cell.Text =  (x + 1). ToString() + ". Nume, Prenume: " + dtCoasig.Rows[x][j].ToString() ;
                        else
                            cell.Text = "CNP: " + dtCoasig.Rows[x][j].ToString();
                        row.Cells.Add(cell);
                    }
                    xrTable2.Rows.Add(row);
                }
            xrTable2.AdjustSize();

            double interval = 0;
            //if (lista["INT"] == "24")
            //    interval = 23.9;
            //else
                interval = 11.9;

            DataTable dtData = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);
            string luna_istoric_F940 = "01/" + (int.Parse(dtData.Rows[0][0].ToString()) < 10 ? "0" + dtData.Rows[0][0].ToString() : dtData.Rows[0][0].ToString()) + "/" + dtData.Rows[0][1].ToString();

            if (Constante.tipBD == 2)
            {
                luna_istoric_F940 = "TO_DATE('" + luna_istoric_F940 + "', 'DD/MM/YYYY') ";
                luna_istoric_F940 = "TO_DATE('01/'||TO_CHAR(F940.MONTH)||'/'||TO_CHAR(F940.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F940;
            }
            else
            {
                luna_istoric_F940 = "CONVERT(DATETIME, '" + luna_istoric_F940 + "', 103)";
                luna_istoric_F940 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F940.MONTH)+'/'+CONVERT(VARCHAR,F940.YEAR), 103) < " + luna_istoric_F940;
            }

            if (Constante.tipBD == 1)
                sql = sql = "select (ISNULL(a.diff, 0) + ISNULL(b. diff, 0)) as diff, f940607, f300607, b.DATA from (select sum(diff) as diff, f940607, NULL AS DATA from (SELECT ISNULL(DATEDIFF(DAY, Max(f94037), Min(F94038))+1, 0) "
                    + "as diff, F940607 FROM F940, F010 WHERE F94003 = " + HttpContext.Current.Session["User_Marca"].ToString() + " AND F940.F94010>=4401 AND F940.F94010<4449 AND "
                    + "(DATEDIFF(DAY, CONVERT(DATETIME, '01/'+CONVERT(VARCHAR, MONTH)+'/'+CONVERT(VARCHAR, YEAR), 103), "
                    + "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F01012)+'/'+CONVERT(VARCHAR,F01011), 103) )/30.436875E) "
                    + "<" + interval.ToString(new CultureInfo("en-US"))
                    + " AND convert(int, F940607) <> 5 AND convert(int, F940607) <> 6 "
                    + " AND " + luna_istoric_F940
                    + " GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) t1 "
                    + "group by f940607 ) a "
                    + "full outer join	"
                    + "( select sum(diff) as diff, f300607, CONVERT(VARCHAR, MAX(F30038), 103) AS DATA from  (SELECT ISNULL(DATEDIFF(DAY, Max(F30037), Min(F30038))+1, 0) AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + HttpContext.Current.Session["User_Marca"].ToString()
                    + " AND F300.F30010>=4401 "
                    + "AND F300.F30010<4449 AND convert(int, F300607) <> 5 AND convert(int, F300607) <> 6 GROUP BY F300.F300601, F300.F300602, F30036, F30037, F30038, F300607) t2 group by F300607 ) b  on f940607 = f300607 order by f300607";

            else
                sql = "select distinct diff, f940607, f300607, DATA from ("
                        + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                        + "(select sum(diff) as diff, f940607, NULL AS DATA "
                        + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + HttpContext.Current.Session["User_Marca"].ToString() + " AND F940.F94010>=4401 "
                        + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))<" + interval.ToString(new CultureInfo("en-US"))
                        + " AND TO_NUMBER(f940607) <> 5 AND TO_NUMBER(f940607) <> 6 "
                        + " AND " + luna_istoric_F940
                        + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                        + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                        + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + HttpContext.Current.Session["User_Marca"].ToString() + " AND F300.F30010>=4401 AND F300.F30010<4449 AND TO_NUMBER(f300607) <> 5 AND TO_NUMBER(f300607) <> 6 GROUP BY F300.F300601, F300.F300602, "
                        + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b  "
                        + "where f940607 = f300607(+) "
                        + "union all "
                        + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                        + "(select sum(diff) as diff, f940607, NULL AS DATA "
                        + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + HttpContext.Current.Session["User_Marca"].ToString() + " AND F940.F94010>=4401 "
                        + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))< " + interval.ToString(new CultureInfo("en-US"))
                        + " AND TO_NUMBER(f940607) <> 5 AND TO_NUMBER(f940607) <> 6 "
                        + " AND " + luna_istoric_F940
                        + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                        + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                        + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + HttpContext.Current.Session["User_Marca"].ToString() + " AND F300.F30010>=4401 AND F300.F30010<4449 AND TO_NUMBER(f300607) <> 5 AND TO_NUMBER(f300607) <> 6 GROUP BY F300.F300601, F300.F300602, "
                        + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b "
                        + "where f940607(+) = f300607) c";

            DataTable dtCM = General.IncarcaDT(sql, null);
            DataTable dtLC = new DataTable();

            if (Constante.tipBD == 1)
                dtLC = General.IncarcaDT("SELECT '01/'+CONVERT(VARCHAR, RIGHT(REPLICATE('0',2)+CAST(f01012 AS VARCHAR(2)),2))+'/'+CONVERT(VARCHAR, F01011) AS LC from F010", null);
            else
                dtLC = General.IncarcaDT("SELECT '01/'||LPAD(F01012, 2, '0')||'/'||F01011 AS LC from F010", null);

            string text2 = "                    Numărul de zile de concediu medical de care persoana asigurată a beneficiat în ultimele 12 luni este de {0} zile, până la data de {1}, aferente fiecărei afecţiuni în parte, după cum urmează:";

            DateTime dataUCM = new DateTime(1900, 1, 1);
            int total = 0;
            if (dtCM != null && dtCM.Rows.Count > 0 && dtCM.Rows[0][0].ToString() != String.Empty)
            {                
                for (int x = 0; x < dtCM.Rows.Count; x++)
                {
                    total += int.Parse(dtCM.Rows[x]["diff"].ToString());
                    DateTime tmpCM = new DateTime(1900, 1, 1);
                    if (dtCM.Rows[x]["Data"].ToString().Length >= 10)
                        tmpCM = new DateTime(Convert.ToInt32(dtCM.Rows[x]["Data"].ToString().Substring(6, 4)),
                                                Convert.ToInt32(dtCM.Rows[x]["Data"].ToString().Substring(3, 2)),
                                                Convert.ToInt32(dtCM.Rows[x]["Data"].ToString().Substring(0, 2)));
                    if (tmpCM > dataUCM)
                        dataUCM = tmpCM;
                }                
            }

            text2 = string.Format(text2, total, dataUCM > new DateTime(1900, 1, 1) ? dataUCM.Day.ToString().PadLeft(2, '0') + "/" + dataUCM.Month.ToString().PadLeft(2, '0') + "/" + dataUCM.Year.ToString()
                                : dtLC.Rows[0]["LC"].ToString());
            xrLabel19.Text = text2;


            

            xrTable1.Borders = DevExpress.XtraPrinting.BorderSide.All;

            XRTableRow rowH = new XRTableRow();
            rowH.HeightF = 25f;
            rowH.CanGrow = true;
            rowH.CanShrink = true;
            for (int j = 0; j <= 1; j++)
            {
                XRTableCell cell = new XRTableCell();
                if (j == 0)
                    cell.Text = "Cod de indemnizaţie";
                else
                    cell.Text = "Număr zile concediu medical în ultimele 12 luni";
                //cell.WidthF = latime[j];
                cell.BackColor = Color.FromArgb(206, 255, 206);
                rowH.Cells.Add(cell);
            }
            xrTable1.Rows.Add(rowH);

            if (dtCM != null && dtCM.Rows.Count > 0 && dtCM.Rows[0][0].ToString() != String.Empty)
            {
                for (int x = 0; x < dtCM.Rows.Count; x++)
                {
                    XRTableRow row = new XRTableRow();
                    row.HeightF = 25f;
                    row.CanGrow = true;
                    row.CanShrink = true;
                    if (dtCM.Rows[x]["f940607"].ToString() != String.Empty)
                    {
                        XRTableCell cell = new XRTableCell();
                        cell.Text = dtCM.Rows[x]["f940607"].ToString();
                        row.Cells.Add(cell);

                        XRTableCell cell1 = new XRTableCell();
                        cell1.Text = dtCM.Rows[x]["diff"].ToString();
                        row.Cells.Add(cell1);
                    }
                    if (dtCM.Rows[x]["f300607"].ToString() != String.Empty)
                    {
                        XRTableCell cell = new XRTableCell();
                        cell.Text = dtCM.Rows[x]["f300607"].ToString();
                        row.Cells.Add(cell);

                        XRTableCell cell1 = new XRTableCell();
                        cell1.Text = dtCM.Rows[x]["diff"].ToString();
                        row.Cells.Add(cell1);
                    }
                    xrTable1.Rows.Add(row);
                }
            }
            else
            {
                XRTableRow row = new XRTableRow();
                row.HeightF = 25f;
                row.CanGrow = true;
                row.CanShrink = true;

                XRTableCell cell = new XRTableCell();
                cell.Text = "                    ————";
                row.Cells.Add(cell);

                XRTableCell cell1 = new XRTableCell();
                cell1.Text = "                    ————";
                row.Cells.Add(cell1);

                xrTable1.Rows.Add(row);
            }

            if (Directory.Exists(HostingEnvironment.MapPath("~/Fisiere/Imagini/Clienti/" + HttpContext.Current.Session["IdClient"].ToString())))
            {
                DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Fisiere/Imagini/Clienti/" + HttpContext.Current.Session["IdClient"].ToString()));
                FileInfo[] listfiles = root.GetFiles("Logo.*");
                //string logo = "";
                if (listfiles.Length > 0)
                {
                    string path = root.FullName + "/" + listfiles[0].Name;
                    xrPictureBox1.Image = Image.FromFile(path);

                }

                FileInfo[] listfilesSub = root.GetFiles("Subsol.*");
                //logo = "";
                if (listfilesSub.Length > 0)
                {
                    string path = root.FullName + "/" + listfilesSub[0].Name;
                    xrPictureBox2.Image = Image.FromFile(path);

                }

            }


            xrTable1.AdjustSize();



        }
    }
}
