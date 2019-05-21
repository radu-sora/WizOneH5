using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataTable dt1 = General.IncarcaDT("Select * from tblGrupUsers", null);
                    grid1.KeyFieldName = "Id";
                    grid1.DataSource = dt1;
                    grid1.DataBind();
                    Session["tbl1"] = dt1;

                    //DataTable dt2 = General.IncarcaDT("Select * from tblGrupAngajati", null);
                    //grid2.KeyFieldName = "Id";
                    //grid2.DataSource = dt2;
                    //grid2.DataBind();
                    //Session["tbl2"] = dt2;

                    //DataTable dt3 = General.IncarcaDT("Select * from tblGrupAngajati", null);
                    //grCC.KeyFieldName = "Id";
                    //grCC.DataSource = dt3;
                    //grCC.DataBind();
                   
                }
                else
                {
                    if (General.Nz(Session["tbl1"],"").ToString() != "")
                    {
                        grid1.DataSource = Session["tbl1"];
                        grid1.DataBind();
                    }
                    //if (General.Nz(Session["tbl2"], "").ToString() != "")
                    //{
                    //    grid2.DataSource = Session["tbl2"];
                    //    grid2.DataBind();
                    //}
                }
                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }

        }

        protected void grid1_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            grid1.CancelEdit();

            DataTable dt = Session["tbl1"] as DataTable;

            for (int x = 0; x < e.UpdateValues.Count; x++)
            {
                ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) continue;

                row["Id"] = upd.NewValues["Id"];
                row["Denumire"] = Session["Denumire"];
            }

            General.SalveazaDate(dt, "tblGrupUsers");
        }

        protected void grid2_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            grid2.CancelEdit();

            DataTable dt = Session["tbl2"] as DataTable;

            for (int x = 0; x < e.UpdateValues.Count; x++)
            {
                ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) continue;

                row["Id"] = upd.NewValues["Id"];
                row["Denumire"] = Session["Denumire"];
            }

            General.SalveazaDate(dt, "tblGrupAngajati");
        }
    }
}