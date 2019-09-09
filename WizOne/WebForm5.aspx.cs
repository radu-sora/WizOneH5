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
            DataTable dt = General.IncarcaDT($@"SELECT 1 AS ID,	'2019-09-09 08:30:00.000' AS StartTime,	'2019-09-09 17:00:00.000' AS EndTime,	'B48CVH' AS Subject,	'REVIZIE 60.000KM/3ANI' AS Description, '1' AS Label
                                                UNION
                                                SELECT 2,	'2019-09-09 08:30:00.000',	'2019-09-09 18:00:00.000',	'IF66SIV',	'REVIZIE INTRETINERE', '2' AS Label
                                                UNION
                                                SELECT 3,	'2019-09-09 08:30:00.000',	'2019-09-09 18:00:00.000',	'B990YRN',	'REVIZIE 40.000KM/2ANI ', '3' AS Label
                                                UNION
                                                SELECT 4,	'2019-09-09 08:30:00.000',	'2019-09-09 18:00:00.000',	'B119EPS',	'REVIZIE', '4' AS Label
                                                UNION
                                                SELECT 5,	'2019-09-09 08:30:00.000',	'2019-09-09 18:00:00.000',	'B51VEK',	'Zgomot la rotirea volanului - comandata (tg)', '5' AS Label
                                                UNION
                                                SELECT 6,	'2019-09-09 09:00:00.000',	'2019-09-09 18:00:00.000',	'B51XPG',	'VERIFICAT LEVIER CV - AGATA', '6' AS Label
                                                UNION
                                                SELECT 7,	'2019-09-09 09:00:00.000',	'2019-09-09 18:00:00.000',	'B76HIO',	'REVIZIE', '7' AS Label
                                                UNION
                                                SELECT 8,	'2019-09-09 09:15:00.000',	'2019-09-09 18:00:00.000',	'B202AVO',	'TROSNESTE PE FATA LA PLECAREA DE PE LOC CU ROTILE VIRATE', '8' AS Label
                                                UNION
                                                SELECT 9,	'2019-09-09 09:30:00.000',	'2019-09-09 18:00:00.000',	'B67NBN',	'VERIFICAT AC - NU FACE RECE CAND STA PE LOC', '9' AS Label
                                                UNION
                                                SELECT 10,	'2019-09-09 09:30:00.000',	'2019-09-09 18:00:00.000',	'B25LRA',	'TRAGE DR LA FRANARE', '10' AS Label
                                                UNION
                                                SELECT 11,	'2019-09-09 10:30:00.000',	'2019-09-09 18:00:00.000',	'AG08VAD',	'REVIZIE INTRETINERE', '11' AS Label
                                                UNION
                                                SELECT 12,	'2019-09-09 11:30:00.000',	'2019-09-09 17:30:00.000',	'B103FSI',	'SCHIMB ULEI+FILTRU CVA', '12' AS Label
                                                UNION
                                                SELECT 13,	'2019-09-09 13:30:00.000',	'2019-09-09 18:00:00.000',	'B118ALR',	'dupa ce ruleaza foarte mult prin oras,automobilul trepideaza  in momentul in care pleaca din loc', '13' AS Label
                                                UNION
                                                SELECT 14,	'2019-09-09 13:30:00.000',	'2019-09-09 13:30:00.000',	'B292YPP',	'verif ans sterg', '14' AS Label
                                                UNION
                                                SELECT 15,	'2019-09-09 14:00:00.000',	'2019-09-09 18:00:00.000',	'B47TNW',	'ZGOMOT LA DENIVELARI - BIELETE REZERVATE', '15' AS Label
                                                UNION
                                                SELECT 16,	'2019-09-09 14:30:00.000',	'2019-09-09 18:00:00.000',	'B76GAZ',	'REVIZIE 40.000KM/2ANI ', '16' AS Label ", null);

            //DataTable dt = General.IncarcaDT($@"SELECT 1 AS ID,	'2019-09-09 08:30:00.000' AS StartTime,	'2019-09-09 17:00:00.000' AS EndTime,	'B48CVH' AS Subject,	'REVIZIE 60.000KM/3ANI' AS Description, '1' AS Label
            //                                    UNION
            //                                    SELECT 2,	'2019-09-09 08:30:00.000',	'2019-09-09 18:00:00.000',	'IF66SIV',	'REVIZIE INTRETINERE', '2' AS Label
            //                                    ", null);


            //DataTable dt = General.IncarcaDT($@"SELECT 1 AS ID,	'2019-09-09 08:30:00.000' AS StartTime,	'2019-09-09 18:00:00.000' AS EndTime,	'B48CVH' AS Subject,	'REVIZIE 60.000KM/3ANI' AS Description, '1' AS Label
            //                                    UNION
            //                                    SELECT 2,	'2019-09-09 08:30:00.000',	'2019-09-09 18:00:00.000',	'IF66SIV',	'REVIZIE INTRETINERE', '2'  ", null);

            //DataTable dt = General.IncarcaDT($@"SELECT 1 AS Id,	'2019-09-09 08:30:00.000' AS StartTime,	'2019-09-09 18:30:00.000' AS EndTime,	'QQQ QQQ' AS Subject,	'111 111' AS Description, '1' AS Label, '1' AS CarId
            //                                    UNION
            //                                    SELECT 2 AS Id,	'2019-09-09 08:30:00.000' AS StartTime,	'2019-09-09 18:00:00.000' AS EndTime,	'AAA AAA' AS Subject,	'222 222' AS Description, '2' AS Label, '2' as CarId", null);

            ASPxScheduler1.AppointmentDataSource = dt;
            ASPxScheduler1.DataBind();
        }
    }
}