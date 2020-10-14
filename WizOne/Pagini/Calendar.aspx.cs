using DevExpress.Web.ASPxScheduler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class Calendar : Page
    {
        public class ScheduleItemViewModel
        {
            public string Id { get; set; }            
            public string Name { get; set; }
            public string Remarks { get; set; }
            public int LabelId { get; set; }
            public int StatusId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }            
        }

        [Serializable]
        public class ScheduleItemColor
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
        }

        public List<ScheduleItemViewModel> GetScheduleItems()
        {
            var reports = General.RunSqlQuery<ScheduleItemViewModel>(
                "SELECT * " +
                "FROM [Schedule] s "/* +
                "WHERE [EmployeeId] = @1", HttpContext.Current.Session["UserId"]*/);

            return reports;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)            
            {
                ViewState["Labels"] = General.RunSqlQuery<ScheduleItemColor>("SELECT [Id], [Denumire] AS [Name], [Culoare] AS [Color] FROM [Ptj_tblAbsente] UNION SELECT 0 AS [Id], 'N/A' AS [Name], '#FF9F9F9F' AS [Color]");
                ViewState["Statuses"] = General.RunSqlQuery<ScheduleItemColor>("SELECT [Id], [Denumire] AS [Name], [Culoare] AS [Color] FROM [Ptj_tblStari]");
            }

            if (ViewState["Labels"] != null)
            {
                Scheduler.Storage.Appointments.Labels.Clear();
                Scheduler.Storage.Appointments.Labels.AddRange((ViewState["Labels"] as List<ScheduleItemColor>).
                    Select(s => Scheduler.Storage.Appointments.Labels.CreateNewLabel(s.Id, s.Name, s.Name, ColorTranslator.FromHtml(s.Color))).ToList());
            }

            if (ViewState["Statuses"] != null)
            {
                Scheduler.Storage.Appointments.Statuses.Clear();
                Scheduler.Storage.Appointments.Statuses.AddRange((ViewState["Statuses"] as List<ScheduleItemColor>).
                    Select(s => Scheduler.Storage.Appointments.Statuses.CreateNewStatus(s.Id, s.Name, s.Name, ColorTranslator.FromHtml(s.Color))).ToList());
            }
        }
    }
}