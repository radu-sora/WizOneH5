using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WizOne.Module;
using System.Web;

namespace WizOne.Reports
{
    public partial class PontajDetaliat : DevExpress.XtraReports.UI.XtraReport
    {
        public PontajDetaliat()
        {
            InitializeComponent();

            string filtru = " AND 1=2";
            //string strSql = @"SELECT A.*, B.F10008 + ' ' + B.F10009 AS NumeComplet 
            //                FROM Ptj_Intrari A 
            //                INNER JOIN F100 B ON A.F10003=B.F10003
            //                WHERE A.F10003=@1 AND YEAR(A.Ziua)=@2 AND MONTH(A.Ziua)=@3";
            //if (Constante.tipBD == 2) strSql = @"SELECT * FROM ""Ptj_Intrari"" WHERE F10003=@1 AND to_char(""Ziua"",'MM/yyyy') = '@3/@2' ";

            string strSql = @"SELECT A.*, B.F10008 + ' ' + B.F10009 AS NumeComplet 
                            FROM Ptj_Intrari A 
                            INNER JOIN F100 B ON A.F10003=B.F10003
                            WHERE 1=1";
            if (Constante.tipBD == 2) strSql = @"SELECT * FROM ""Ptj_Intrari"" WHERE 1=1 ";

            string param = (HttpContext.Current.Session["PrintParametrii"] ?? "").ToString();
            if (param != "")
            {
                string[] lst = param.Split(';');
                if (lst.Length > 0)
                {
                    int tip = 1;
                    if (lst.Length == 5) tip = Convert.ToInt32(General.Nz(lst[4],1));
                    if (tip == 1 || tip == 10)
                    {
                        filtru = " AND A.F10003 IN (@1) AND YEAR(A.Ziua)=@2 AND MONTH(A.Ziua)=@3";
                        if (Constante.tipBD == 2) strSql = @" AND F10003=@1 AND to_char(""Ziua"",'MM/yyyy') = '@2/@3' ";
                    }
                    else
                    {
                        filtru = " AND A.F10003 IN (@1) AND YEAR(A.Ziua)=@2 AND MONTH(A.Ziua)=@3 AND DAY(A.Ziua)=@4";
                        if (Constante.tipBD == 2) strSql = @" AND F10003=@1 AND to_char(""Ziua"",'DD/MM/yyyy') = '@4/@3/@2' ";
                    }
                    for (int i=0;i<lst.Length;i++)
                    {
                        filtru = filtru.Replace("@" + (i+1).ToString(), lst[i]);
                    }
                }
            }

            this.DataSource = General.IncarcaDT(strSql + filtru, null);
        }

    }
}
