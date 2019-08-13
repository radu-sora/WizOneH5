using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using WizOne.Module;

namespace WizOne
{
    public partial class WebForm4 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //            DataTable dt = General.IncarcaDT(
            //                $@"SELECT 1 AS ID, 'Gigi' AS Descriprion, '2019-07-22 10:00:00.000' AS StartTime, '2019-07-22 11:00:00.000' AS EndTime, 'acasa' AS Label, 'Programare I' AS Subject
            //UNION
            //SELECT 2 AS ID, 'Fifi' AS Descriprion, '2019-07-22 13:00:00.000' AS StartTime, '2019-07-22 14:00:00.000' AS EndTime, 'acasa' AS Label, 'Programare II' AS Subject
            //UNION
            //SELECT 3 AS ID, 'Mimi' AS Descriprion, '2019-07-22 16:00:00.000' AS StartTime, '2019-07-22 17:30:00.000' AS EndTime, 'acasa' AS Label, 'Programare III' AS Subject
            //", null);


            DataTable dt = General.IncarcaDT(
             $@"SELECT 1 AS ID, '2019-07-22 10:00:00.000' AS StartTime, '2019-07-22 11:00:00.000' AS EndTime, 'Popescu Mihai' AS Subject, 'Audi - schimb ulei' AS Description
                UNION
                SELECT 2 AS ID, '2019-07-22 13:00:00.000' AS StartTime, '2019-07-22 14:00:00.000' AS EndTime, 'Ionescu Marcel' AS Subject, 'Seat - constatare: bataie la motor' AS Description
                UNION
                SELECT 3 AS ID, '2019-07-22 16:00:00.000' AS StartTime, '2019-07-22 17:30:00.000' AS EndTime, 'S.C. Fabrica De Softuri S.R.L.' AS Subject, 'Mercedes - schimb bascule si bieleta' AS Description
                ", null);


            ASPxScheduler1.AppointmentDataSource = dt;
            ASPxScheduler1.DataBind();
        }


    }
}