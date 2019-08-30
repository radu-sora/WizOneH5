using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne
{
    public partial class WebForm5 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = General.IncarcaDT($@"SELECT 1 AS Id,	'2019-08-27 08:30:00.000' AS StartTime,	'2019-08-27 17:00:00.000' AS EndTime,	'B48CVH' AS Subject,	'REVIZIE 60.000KM/3ANI' AS Description
                                                UNION
                                                SELECT 2,	'2019-08-27 08:30:00.000',	'2019-08-27 18:00:00.000',	'IF66SIV',	'REVIZIE INTRETINERE'
                                                UNION
                                                SELECT 3,	'2019-08-27 08:30:00.000',	'2019-08-27 18:00:00.000',	'B990YRN',	'REVIZIE 40.000KM/2ANI '
                                                UNION
                                                SELECT 4,	'2019-08-27 08:30:00.000',	'2019-08-27 18:00:00.000',	'B119EPS',	'REVIZIE'
                                                UNION
                                                SELECT 5,	'2019-08-27 08:30:00.000',	'2019-08-27 18:00:00.000',	'B51VEK',	'Zgomot la rotirea volanului - comandata (tg)'
                                                UNION
                                                SELECT 6,	'2019-08-27 09:00:00.000',	'2019-08-27 18:00:00.000',	'B51XPG',	'VERIFICAT LEVIER CV - AGATA'
                                                UNION
                                                SELECT 7,	'2019-08-27 09:00:00.000',	'2019-08-27 18:00:00.000',	'B76HIO',	'REVIZIE'
                                                UNION
                                                SELECT 8,	'2019-08-27 09:15:00.000',	'2019-08-27 18:00:00.000',	'B202AVO',	'TROSNESTE PE FATA LA PLECAREA DE PE LOC CU ROTILE VIRATE'
                                                UNION
                                                SELECT 9,	'2019-08-27 09:30:00.000',	'2019-08-27 18:00:00.000',	'B67NBN',	'VERIFICAT AC - NU FACE RECE CAND STA PE LOC'
                                                UNION
                                                SELECT 10,	'2019-08-27 09:30:00.000',	'2019-08-27 18:00:00.000',	'B25LRA',	'TRAGE DR LA FRANARE'
                                                UNION
                                                SELECT 11,	'2019-08-27 10:30:00.000',	'2019-08-27 18:00:00.000',	'AG08VAD',	'REVIZIE INTRETINERE'
                                                UNION
                                                SELECT 12,	'2019-08-27 11:30:00.000',	'2019-08-27 17:30:00.000',	'B103FSI',	'SCHIMB ULEI+FILTRU CVA'
                                                UNION
                                                SELECT 13,	'2019-08-27 13:30:00.000',	'2019-08-27 18:00:00.000',	'B118ALR',	'dupa ce ruleaza foarte mult prin oras,automobilul trepideaza  in momentul in care pleaca din loc'
                                                UNION
                                                SELECT 14,	'2019-08-27 13:30:00.000',	'2019-08-27 13:30:00.000',	'B292YPP',	'verif ans sterg'
                                                UNION
                                                SELECT 15,	'2019-08-27 14:00:00.000',	'2019-08-27 18:00:00.000',	'B47TNW',	'ZGOMOT LA DENIVELARI - BIELETE REZERVATE'
                                                UNION
                                                SELECT 16,	'2019-08-27 14:30:00.000',	'2019-08-27 18:00:00.000',	'B76GAZ',	'REVIZIE 40.000KM/2ANI '", null);

            ASPxScheduler1.AppointmentDataSource = dt;
            ASPxScheduler1.DataBind();
        }
    }
}