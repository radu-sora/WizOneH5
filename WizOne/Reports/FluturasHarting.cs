using System.Web;
using WizOne.Module;




namespace WizOne.Reports
{
    public partial class FluturasHarting : DevExpress.XtraReports.UI.XtraReport
    {
        public FluturasHarting()
        {
            InitializeComponent();

            //string strSql = @"SELECT TOP 1 A.* FROM FLUTURAS_ANGAJAT_DOC A
            //            INNER JOIN F010 B ON 1=1
            //            WHERE ""Camp44"" = B.F01011 AND ""Camp48"" = B.F01012 AND (""Camp2"" IN (SELECT MIN(F10003) FROM USERS WHERE F70102=" + HttpContext.Current.Session["UserId"] + "))";            
            string strSql = @"SELECT TOP 1 A.* FROM FLUTURAS_ANGAJAT_DOC A                       
                        WHERE ""Camp44"" = " + HttpContext.Current.Session["Fluturas_An"].ToString() + @" AND ""Camp48"" = " + HttpContext.Current.Session["Fluturas_Luna"].ToString() + @" AND ""Camp2"" =" + HttpContext.Current.Session["User_Marca"].ToString();
            this.DataSource = General.IncarcaDT(strSql, null);
        }

    }
}
