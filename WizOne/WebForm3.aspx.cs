using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WizOne
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        class GridDataItem
        {
            public Int32 ID { get; set; }
            public TimeSpan Time { get; set; }
        }

        /* Runtime Data-binding */

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Grid.DataSource = TempData;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Grid.DataBind();
        }

        /* Custom Updating action */

        protected void Grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            var item = TempData
                .Where(i => i.ID == Convert.ToInt32(e.Keys["ID"]))
                .First();

            item.Time = (TimeSpan)e.NewValues["Time"];

            e.Cancel = true;
            Grid.CancelEdit();
        }

        /* Custom Inserting action */

        protected void Grid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            var item = TempData
              .OrderBy(i => i.ID)
              .Last();

            GridDataItem newItem = new GridDataItem
            {
                ID = item.ID + 1,
                Time = (TimeSpan)e.NewValues["Time"]
            };

            TempData.Add(newItem);

            e.Cancel = true;
            Grid.CancelEdit();
        }

        #region #ParseValue
        /* Parse String object of the "Time" text box */

        protected void Grid_ParseValue(object sender, DevExpress.Web.Data.ASPxParseValueEventArgs e)
        {
            if (!Grid.IsNewRowEditing && e.FieldName == "Time")
                e.Value = TimeSpanFromString(e.Value);
        }

        /* Convert from TimeSpan to a proper Mask format */

        protected void Grid_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        {
            if (e.Column.FieldName == "Time")
            {
                e.Editor.Value = StringFromTimeSpan(e.Value);
            }
        }

        /* Helper methods */

        private TimeSpan TimeSpanFromString(Object value)
        {
            if (value == null || String.IsNullOrEmpty((String)value))
                return TimeSpan.Zero;

            return TimeSpan.Parse((String)value);
        }

        private String StringFromTimeSpan(Object value)
        {
            if (value == null)
                return String.Empty;

            TimeSpan time = (TimeSpan)value;

            if (time.Days != 0)
                return time.ToString("c");

            String str = time.ToString(@"hh\:mm\:ss");

            if (time < TimeSpan.Zero)
                return String.Format("-0.{0}", str);
            else
                return String.Format("0.{0}", str);
        }
        #endregion #ParseValue

        /* Fake data source */

        List<GridDataItem> TempData
        {
            get
            {
                const String key = "(some guid)";
                if (Session[key] == null)
                {

                    List<GridDataItem> lst = new List<GridDataItem>();

                    /* Initialization with some values */

                    for (Int32 i = -5; i < 6; i++)
                        lst.Add(new GridDataItem { ID = i, Time = new TimeSpan(i, i, i, i) });

                    Session[key] = lst;
                }
                return (List<GridDataItem>)Session[key];
            }
        }

    }
}