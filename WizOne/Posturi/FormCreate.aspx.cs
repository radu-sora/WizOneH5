using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Posturi
{
    public partial class FormCreate : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];

                #endregion


                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                

                if (!IsPostBack)
                {

        
     //               DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
     //               cmbAngFiltru.DataSource = dtAng;
					//Session["Modif_Avans_Angajati"] = dtAng;
     //               cmbAngFiltru.DataBind();
     //               cmbAngFiltru.SelectedIndex = -1;

                }
                else
                {
                    if (IsCallback)
                    {
                        //cmbAngFiltru.DataSource = null;
                        //cmbAngFiltru.Items.Clear();
                        //cmbAngFiltru.DataSource = Session["Modif_Avans_Angajati"];
                        //cmbAngFiltru.DataBind();
                    }

                    DataTable dt = Session["Istoric_Grid"] as DataTable;
                    grDate.KeyFieldName = "NumeAngajat;DataModif";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }
        




        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter.Split(';')[0];
                switch(tip)
                {
                    case "5":
                        IncarcaGrid();
                        break;    
                }


 
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }


        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                Avs.Istoric pag = new Avs.Istoric();
                List<int> lst = new List<int>(5);
                lst.Add((int)Constante.Atribute.Salariul);
                lst.Add((int)Constante.Atribute.Functie);
                lst.Add((int)Constante.Atribute.CodCOR);
                lst.Add((int)Constante.Atribute.Organigrama);
                lst.Add((int)Constante.Atribute.NumePrenume);

                //if (cmbAngFiltru.SelectedIndex < 0)
                //{
                //    ArataMesaj("Nu ati selectat niciun angajat!");
                //    return;
                //}

        

                //string sql = "SELECT * FROM F100 WHERE F10003 = " + cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString();
                //DataTable dttmp = General.IncarcaDT(sql, null);
                //string numeAng = dttmp.Rows[0]["F10008"].ToString() + " " + dttmp.Rows[0]["F10009"].ToString();

                //sql = "SELECT * FROM F010";
                //dttmp = General.IncarcaDT(sql, null);
                //int luna = Convert.ToInt32(dttmp.Rows[0]["F01012"].ToString());
                //int an = Convert.ToInt32(dttmp.Rows[0]["F01011"].ToString());

                //dt = pag.GetIstoricDateContract(Convert.ToInt32(cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString()), lst, luna + 1 > 12 ? 1 : luna, luna + 1 > 12 ? an + 1 : an, numeAng, 1);
                //grDate.KeyFieldName = "NumeAngajat;DataModif";
                //grDate.DataSource = dt;
                //grDate.DataBind();
                //Session["Istoric_Grid"] = dt;
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }







 

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

        }

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {

        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {

        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {

        }




    }
}