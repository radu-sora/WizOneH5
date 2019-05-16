using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data;
using DevExpress.Web.Data;
using System.ComponentModel;
using DevExpress.Web;

namespace WizOne
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    DataTable dt1 = General.IncarcaDT("Select * from tblGrupUsers", null);
                    gridA.KeyFieldName = "Id";
                    gridA.DataSource = dt1;
                    gridA.DataBind();
                    Session["tbl1"] = dt1;

                    DataTable dt2 = General.IncarcaDT("Select * from tblGrupAngajati", null);
                    gridB.KeyFieldName = "IdAuto";
                    gridB.DataSource = dt2;
                    gridB.DataBind();
                    Session["tbl2"] = dt2;

                    //DataTable dt3 = General.IncarcaDT("Select * from tblGrupAngajati", null);
                    //grCC.KeyFieldName = "Id";
                    //grCC.DataSource = dt3;
                    //grCC.DataBind();

                }
                else
                {
                    if (General.Nz(Session["tbl1"], "").ToString() != "")
                    {
                        gridA.DataSource = Session["tbl1"];
                        gridA.DataBind();
                    }
                    if (General.Nz(Session["tbl2"], "").ToString() != "")
                    {
                        gridB.DataSource = Session["tbl2"];
                        gridB.DataBind();
                    }
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void grid1_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            gridA.CancelEdit();

            DataTable dt = Session["tbl1"] as DataTable;

            for (int x = 0; x < e.UpdateValues.Count; x++)
            {
                ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) continue;

                if (upd.NewValues["Id"] != null) row["Id"] = upd.NewValues["Id"];
                if (upd.NewValues["Denumire"] != null) row["Denumire"] = upd.NewValues["Denumire"];
            }

            General.SalveazaDate(dt, "tblGrupUsers");
        }

        protected void grid2_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            gridB.CancelEdit();

            DataTable dt = Session["tbl2"] as DataTable;

            for (int x = 0; x < e.UpdateValues.Count; x++)
            {
                ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) continue;

                if (upd.NewValues["Id"] != null) row["Id"] = upd.NewValues["Id"];
                if (upd.NewValues["Denumire"] != null) row["Denumire"] = upd.NewValues["Denumire"];
            }

            General.SalveazaDate(dt, "tblGrupAngajati");
        }

        protected void gridA_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            CancelEditing(sender, e);
        }

        protected void gridA_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            CancelEditing(sender, e);
        }

        protected void gridA_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            CancelEditing(sender, e);
        }

        protected void gridB_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            CancelEditing(sender, e);
        }

        protected void gridB_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            CancelEditing(sender, e);
        }

        protected void gridB_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {

        }

        protected void CancelEditing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ASPxGridView grid = sender as ASPxGridView;
            grid.CancelEdit();
        }



    }
}