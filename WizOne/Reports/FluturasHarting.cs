using System;
using System.Web;
using WizOne.Module;




namespace WizOne.Reports
{
    public partial class FluturasHarting : DevExpress.XtraReports.UI.XtraReport
    {
        public FluturasHarting()
        {
            InitializeComponent();

            //#1068
            int marca = Convert.ToInt32(HttpContext.Current.Session["User_Marca"].ToString());
            if (300000 <= marca && marca <= 399999)
            {
                xrTableCell3.Text = "pentru informatii si intrebari sunati la d-na Izabela Boldor - Resurse Umane tel 0369/102607; 0369/102803; 0369/102847";
            }

            string strSql = $@"SELECT TOP 1 A.*, 
                CONVERT(int,(SELECT F30013 FROM F300 WHERE F30003={HttpContext.Current.Session["User_Marca"].ToString()} AND F30010=4307 AND YEAR(F30035)={HttpContext.Current.Session["Fluturas_An"].ToString()} AND MONTH(F30035)={HttpContext.Current.Session["Fluturas_Luna"].ToString()})) AS Camp75
                FROM FLUTURAS_ANGAJAT_DOC A
                WHERE ""Camp44"" = {HttpContext.Current.Session["Fluturas_An"].ToString()} AND ""Camp48"" = {HttpContext.Current.Session["Fluturas_Luna"].ToString()} AND ""Camp2"" = {HttpContext.Current.Session["User_Marca"].ToString()} ";
            this.DataSource = General.IncarcaDT(strSql, null); this.DataSource = General.IncarcaDT(strSql, null);
        }

    }
}
