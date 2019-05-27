using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.Drawing;

namespace WizOne.Personal
{
    public partial class Calcul : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            TabPage tabPage = new TabPage();
            tabPage.Name = "Componente";
            tabPage.Text = "Componente";
            Control ctrl = this.LoadControl("Componente.ascx");
            tabPage.Controls.Add(ctrl);
            tabPage.TabStyle.BackColor = Color.FromArgb(255, 194, 194, 214);
            this.ASPxPageControlCalcul.TabPages.Add(tabPage);

            tabPage = new TabPage();
            tabPage.Name = "Tarife";
            tabPage.Text = "Tarife";
            ctrl = this.LoadControl("Tarife.ascx");
            tabPage.Controls.Add(ctrl);
            tabPage.TabStyle.BackColor = Color.FromArgb(255, 194, 194, 214);
            this.ASPxPageControlCalcul.TabPages.Add(tabPage);

            tabPage = new TabPage();
            tabPage.Name = "Sporuri";
            tabPage.Text = "Sporuri";
            tabPage.TabStyle.BackColor = Color.FromArgb(255, 194, 194, 214);
            this.ASPxPageControlCalcul.TabPages.Add(tabPage);

            tabPage = new TabPage();
            tabPage.Name = "Sporuri si tranzactii";
            tabPage.Text = "Sporuri si tranzactii";
            tabPage.TabStyle.BackColor = Color.FromArgb(255, 194, 194, 214);
            this.ASPxPageControlCalcul.TabPages.Add(tabPage);

        }

 

      


    }
}