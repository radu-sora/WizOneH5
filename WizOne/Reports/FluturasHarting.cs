using System.Web;
using WizOne.Module;




namespace WizOne.Reports
{
    public partial class FluturasHarting : DevExpress.XtraReports.UI.XtraReport
    {
        public FluturasHarting()
        {
            InitializeComponent();

            string strSql = $@"SELECT TOP 1 A.*, 
                CONVERT(int,(SELECT F30013 FROM F300 WHERE F30003={HttpContext.Current.Session["User_Marca"].ToString()} AND F30010=4307 AND YEAR(F30035)={HttpContext.Current.Session["Fluturas_An"].ToString()} AND MONTH(F30035)={HttpContext.Current.Session["Fluturas_Luna"].ToString()})) AS Camp75
                FROM FLUTURAS_ANGAJAT_DOC A
                WHERE ""Camp44"" = {HttpContext.Current.Session["Fluturas_An"].ToString()} AND ""Camp48"" = {HttpContext.Current.Session["Fluturas_Luna"].ToString()} AND ""Camp2"" = {HttpContext.Current.Session["User_Marca"].ToString()} ";
            this.DataSource = General.IncarcaDT(strSql, null); this.DataSource = General.IncarcaDT(strSql, null);
        }

    }
}
