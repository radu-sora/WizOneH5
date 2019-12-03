using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;
using System.Text;

namespace WizOne.Personal
{
    public partial class ListaDocumente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                if (!IsPostBack)
                {      
                    string qwe = Convert.ToString(General.Nz(Request["qwe"], "-99"));           
                    string idButon = "0";
                    switch (qwe)
                    {
                        case "btnDocPersonal":
                            idButon = "1";
                            break;
                    }

                    string strSql = "";
                    string filtru = " AND b.\"IdModul\"= " + idButon;
                    string inn = "";
                    if (General.VarSession("EsteAdmin").ToString() == "0")
                    {
                        filtru += " and c.\"IdUser\" = " + Session["UserId"].ToString();
                        inn = @" inner join ""relGrupUser"" c on b.""IdGrup"" = c.""IdGrup"" ";
                    }

                    strSql = @"select a.""DynReportId"" as ""Id"", a.""Name"" as ""Denumire"" 
                          from ""DynReports"" a                          
                          join ""relRapGrupModul"" b on b.""IdRaport"" = a.""DynReportId""
                          {0}                          
                          where 1=1 {1}
                          order by ""Denumire""";

                    strSql = string.Format(strSql, inn, filtru);
                    DataTable dt = General.IncarcaDT(strSql, null);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                    grDate.KeyFieldName = "Id";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
                string url = "Generatoare/Reports/Pages/ReportView.aspx?q=" + General.URLEncode("Angajat=" + Session["Marca"].ToString());
                Session["ListaDoc_URL"] = url;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;           
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    switch (arr[0])
                    {
                        case "btnArata":
                            Session["ReportId"] = Convert.ToInt32(arr[1]);                           
                            break;
                        case "btnPrint":
                            Session["ReportId"] = Convert.ToInt32(arr[1]);
                            Session["PrintareAutomata"] = 1;
                            break;
                    }
                }  
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;         
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}