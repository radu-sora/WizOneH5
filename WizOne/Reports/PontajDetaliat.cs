﻿using System;
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

            string strSql = $@"SELECT A.*, B.F10008 {Dami.Operator()} ' ' {Dami.Operator()} B.F10009 AS ""NumeComplet"" 
                            FROM ""Ptj_Intrari"" A 
                            INNER JOIN F100 B ON A.F10003=B.F10003
                            WHERE 1=1";
            //if (Constante.tipBD == 2) strSql = @"SELECT * FROM ""Ptj_Intrari"" WHERE 1=1 ";

            string param = (HttpContext.Current.Session["PrintParametrii"] ?? "").ToString();
            //HttpContext.Current.Session["PrintParametrii"] = "";
            if (param != "")
            {
                string[] lst = param.Split(new string[] { "#$" }, StringSplitOptions.RemoveEmptyEntries);
                if (lst.Length > 0)
                {
                    int tip = 1;
                    if (lst.Length == 5) tip = Convert.ToInt32(General.Nz(lst[4], 1));

                    //Florin 2019.11.11

                    //if (tip == 1 || tip == 10)
                    //{
                    //    filtru = " AND A.F10003 IN (@1) AND YEAR(A.Ziua)=@2 AND MONTH(A.Ziua)=@3";
                    //    //Florin 2019.11.07
                    //    //if (Constante.tipBD == 2) strSql = @" AND F10003=@1 AND to_char(""Ziua"",'MM/yyyy') = '@2/@3' ";
                    //    if (Constante.tipBD == 2) filtru = @" AND F10003 IN (@1) AND TO_NUMBER(TO_CHAR(""Ziua"",'YYYY')) = @2 AND TO_NUMBER(TO_CHAR(""Ziua"",'MM')) = @3 ";
                    //}
                    //else
                    //{
                    //    filtru = " AND A.F10003 IN (@1) AND YEAR(A.Ziua)=@2 AND MONTH(A.Ziua)=@3 AND DAY(A.Ziua)=@4";
                    //    //Florin 2019.11.07
                    //    //if (Constante.tipBD == 2) strSql = @" AND F10003=@1 AND to_char(""Ziua"",'DD/MM/yyyy') = '@4/@3/@2' ";
                    //    if (Constante.tipBD == 2) filtru = @" AND F10003 IN (@1) AND TO_NUMBER(TO_CHAR(""Ziua"",'YYYY')) = @2 AND TO_NUMBER(TO_CHAR(""Ziua"",'MM')) = @3 AND TO_NUMBER(TO_CHAR(""Ziua"",'DD')) = @4 ";
                    //}


                    filtru = " AND A.F10003 IN (@1) AND YEAR(A.Ziua)=@2 AND MONTH(A.Ziua)=@3";
                    if (Constante.tipBD == 2) filtru = @" AND A.F10003 IN (@1) AND TO_NUMBER(TO_CHAR(A.""Ziua"",'YYYY')) = @2 AND TO_NUMBER(TO_CHAR(A.""Ziua"",'MM')) = @3 ";

                    if (tip == 2 || tip == 20)
                    {
                        if (Constante.tipBD == 1)
                            filtru += " AND DAY(A.Ziua)=@4";
                        else
                            filtru += @" AND TO_NUMBER(TO_CHAR(A.""Ziua"",'DD')) = @4 ";
                    }

                    for (int i = 0; i < lst.Length; i++)
                    {
                        filtru = filtru.Replace("@" + (i + 1).ToString(), lst[i]);
                    }
                }
            }

            this.DataSource = General.IncarcaDT(strSql + filtru + @" ORDER BY B.F10008, B.F10009, A.""Ziua"" ", null);
        }

    }
}
