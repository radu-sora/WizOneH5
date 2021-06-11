using DevExpress.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class ListaDocumente : Page
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
                        case "btnRapCM":
                            idButon = "2";
                            break;
                    }

                    if (idButon == "2")
                    {
                        GridViewDataTextColumn colId = (grDate.Columns["Id"] as GridViewDataTextColumn);
                        colId.Visible = false;
                    }

                    string strSql = "";
                    string filtru = " AND b.\"IdModul\"= " + idButon;
                    string inn = "";
                    if (General.VarSession("EsteAdmin").ToString() == "0")
                    {
                        filtru += " and c.\"IdUser\" = " + HttpContext.Current.Session["UserId"].ToString();
                        inn = @" inner join ""relGrupUser"" c on b.""IdGrup"" = c.""IdGrup"" ";
                    }

                    strSql = @"select a.""DynReportId"" as ""Id"", a.""Name"" as ""Denumire"" 
                          from ""DynReports"" a                          
                          join ""relRapGrupModul"" b on b.""IdRaport"" = a.""DynReportId""
                          {0}                          
                          where 1=1 {1}
                          order by ""Denumire""";

                    strSql = string.Format(strSql, inn, filtru);
                    DataTable dt = General.IncarcaDT(strSql, null); // TODO: FIX02 - Use repository pattern e.g. Manage.GetReports() from MainPage.
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                    grDate.KeyFieldName = "Id";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }                
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
                var param = JObject.Parse(e.Parameters) as dynamic;

                // New report access interface
                if (param.command == "btnArata")
                {                    
                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings((int)param.reportId);
                    var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl((int)param.reportId, reportSettings.ToolbarType, reportSettings.ExportOptions, new { Angajat = Session["Marca"].ToString() });

                    grDate.JSProperties["cpReportUrl"] = ResolveClientUrl(reportUrl);
                }
                else if (param.command == "btnPrint")
                    Wizrom.Reports.Code.ReportProxy.Print((int)param.reportId);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}