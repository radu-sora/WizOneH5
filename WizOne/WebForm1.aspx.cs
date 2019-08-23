using DevExpress.Web;
using DevExpress.Web.Data;
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
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = General.IncarcaDT($@"SELECT A.F10003, A.Ziua, A.F06204, A.IdProiect, A.IdSubproiect, A.IdActivitate, A.IdDept, A.De, A.La, A.NrOre1, A.NrOre2, A.NrOre3, A.NrOre4, A.NrOre5, A.NrOre6, A.NrOre7, A.NrOre8, A.NrOre9, A.NrOre10, A.IdStare, A.USER_NO, A.TIME, A.IdAuto, 
B.F10023 ,CONVERT(datetime,DATEADD(minute, NrOre1, '2019-01-01')) AS NrOre1_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre2, '2019-01-01')) AS NrOre2_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre3, '2019-01-01')) AS NrOre3_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre4, '')) AS NrOre4_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre5, '')) AS NrOre5_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre6, '')) AS NrOre6_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre7, '')) AS NrOre7_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre8, '')) AS NrOre8_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre9, '')) AS NrOre9_Tmp ,CONVERT(datetime,DATEADD(minute, NrOre10, '')) AS NrOre10_Tmp 
FROM Ptj_CC A 
INNER JOIN F100 B ON A.F10003 = B.F10003
WHERE A.F10003 = 2403 AND A.Ziua = CONVERT(date, '2019-08-06')", null);

            grCC.KeyFieldName = "F10003;Ziua;F06204";
            dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };

            grCC.DataSource = dt;
            grCC.DataBind();

        }

    }
}