using DevExpress.Web;
using System;
using System.Web.UI;
using WizOne.Generatoare.Reports.Models;
using WizOne.Module;

namespace WizOne.Generatoare.Reports.Pages
{
    public partial class Reports : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dami.AccesApp();

            #region Traducere
            string ctlPost = Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

            ReportNewButton.Text = Dami.TraduCuvant("ReportNewButton", "Raport nou");
            ReportViewButton.Text = Dami.TraduCuvant("ReportViewButton", "Afisare");
            ReportDesignButton.Text = Dami.TraduCuvant("ReportDesignButton", "Design");
            ExitButton.Text = Dami.TraduCuvant("ExitButton", "Iesire");

            TitleLabel.Text = Dami.TraduCuvant("Modifica sau creaza rapoarte noi");

            foreach (dynamic c in ReportsGridView.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                }
                catch (Exception) { }
            }
            #endregion

            if (!IsPostBack)
                Session.Remove("ReportId");

            if ((Session["EsteAdmin"] ?? "0").ToString() == "0") ReportsGridView.Columns[0].Visible = false;

            ReportsGridView.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaRap", "10"));
        }
        
        protected void ReportsGridView_DataBinding(object sender, EventArgs e)
        {
            (sender as ASPxGridView).ForceDataRowType(typeof(Report));
            ReportsGridView.FocusedRowIndex = -1;
        }

        protected void ReportViewButton_Click(object sender, EventArgs e)
        {
            var selectedValues = ReportsGridView.GetSelectedFieldValues(new string[] { "ReportId" });

            if (selectedValues.Count > 0)
            {
                // Required parameters for ReportView             
                //Session["UserId"] = "1234";                                
                Session["ReportId"] = selectedValues[0];

                Response.Redirect("ReportView");
            }
        }

        protected void ReportDesignButton_Click(object sender, EventArgs e)
        {
            var selectedValues = ReportsGridView.GetSelectedFieldValues(new string[] { "ReportId" });

            if (selectedValues.Count > 0)
            {
                // Required parameters for ReportDesign
                Session["ReportId"] = selectedValues[0];

                Response.Redirect("ReportDesign");
            }
        }     
    }
}