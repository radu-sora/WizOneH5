using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;
using System.Text;

namespace WizOne.WebService
{
    public partial class Detalii : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                SodexoOnline.resultContactList listaContacte = new SodexoOnline.resultContactList();
                if (Session["Sodexo_ListaContacte"] != null)
                {
                    listaContacte = Session["Sodexo_ListaContacte"] as SodexoOnline.resultContactList;
                    DataTable dtCont = new DataTable();
                    dtCont.Columns.Add("Id", typeof(int));
                    dtCont.Columns.Add("Denumire", typeof(string));
                    for (int z = 0; z < listaContacte.contacts.Count(); z++)
                    {
                        dtCont.Rows.Add(listaContacte.contacts.ElementAt(z).contactId, listaContacte.contacts.ElementAt(z).firstName.ToUpper().Trim() + " " + listaContacte.contacts.ElementAt(z).lastName.ToUpper().Trim());
                    }
                    GridViewDataComboBoxColumn colPersContact = (grDate.Columns["NumePersContact"] as GridViewDataComboBoxColumn);
                    colPersContact.PropertiesComboBox.DataSource = dtCont;
                }

                if (!IsPostBack)
                {
                    string comp = Convert.ToString(General.Nz(Request["comp"], -99));
                    int c1 = Convert.ToInt32(Convert.ToString(General.Nz(Request["c1"], -99)));
                    int c2 = Convert.ToInt32(Convert.ToString(General.Nz(Request["c2"], -99)));
                    int c3 = Convert.ToInt32(Convert.ToString(General.Nz(Request["c3"], -99)));
                    IncarcaGrid(comp, c1, c2, c3);
                }
                else
                {
                    DataTable dt = Session["Sodexo_Detalii"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

     

        private void IncarcaGrid(string comp, int c1, int c2, int c3)
        {
            try
            {
                if ((c1 < 0 && (c2 > 0 || c3 > 0)) || (c2 < 0 && c3 > 0))
                {
                    MessageBox.Show("Criteriile de sortare sunt invalide!", MessageBox.icoError);
                    return;
                }

                string ordonare = "";
                string temp = " ORDER BY ";
                if (c1 > 0)
                    ordonare += DaMiValoare(Convert.ToInt32(c1));
                if (c2 > 0)
                    ordonare += DaMiValoare(Convert.ToInt32(c2));
                if (c3 > 0)
                    ordonare += DaMiValoare(Convert.ToInt32(c3));
                if (ordonare.Length > 0)
                    ordonare = temp + ordonare.Substring(0, ordonare.Length - 1);
                else
                    ordonare = " order by a.\"NumeAng\", a.\"PrenumeAng\" ";


                if (comp.Length <= 0)
                    return;
                string sql = "";
                if (Constante.tipBD == 1)
                    sql = "select  Row_Number() Over ( " + ordonare + " ) as IdAuto, a.* from viewSodexoRequest a WHERE \"NumeCompanie\" = '" + comp + "'";
                else
                    sql = "select  rownum as \"IdAuto\" a.* from viewSodexoRequest a " + ordonare + " WHERE \"NumeCompanie\" = '" + comp + "'";

                DataTable dt = General.IncarcaDT(sql, null);



                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;
                grDate.DataBind();
                grDate.SettingsPager.PageSize = 15;

                Session["Sodexo_Detalii"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }

        }

        private string DaMiValoare(int id)
        {
            switch (id)
            {
                case 1:
                    return "Departament,";
                case 2:
                    return "PunctLucru,";
                case 3:
                    return "Locatie,";
                case 4:
                    return "NumeAng, PrenumeAng,";
                default:
                    return "";
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["Sodexo_Detalii"] as DataTable;
                DevExpress.Spreadsheet.Workbook book = new DevExpress.Spreadsheet.Workbook();
                DevExpress.Spreadsheet.Worksheet ws2 = book.Worksheets["Sheet1"];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws2.Cells[1, i].Value = dt.Columns[i].ColumnName;
                }

                int row = 3;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string val = General.Nz(dt.Rows[i][j], "").ToString();
                        if (val != "" && val.Substring(0, 1) == "#")
                            ws2.Cells[row, j].FillColor = General.Culoare(dt.Rows[i][j].ToString());
                        else
                            ws2.Cells[row, j].Value = dt.Rows[i][j].ToString();
                    }
                    row++;
                }


                //row = 3;
                //int col = dt.Columns.Count + 2;
                //ws2.Cells[1, col].Value = "Legenda";



                //for (int i = 0; i < grLeg.VisibleRowCount; i++)
                //{
                //    DataRowView obj = grLeg.GetRow(i) as DataRowView;
                //    ws2.Cells[row, col].Value = obj["Denumire"].ToString();
                //    ws2.Cells[row, col + 1].FillColor = General.Culoare(obj["Culoare"].ToString());
                //    row++;
                //}


                ws2.Columns.AutoFit(1, 100);

                byte[] byteArray = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xls);

                string data = DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString()
                            + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');

                string numeFis = "ComandaTichete_" + dt.Rows[0]["NumeCompanie"].ToString().Replace(" ", "_") + "_" + data;

                MemoryStream stream = new MemoryStream(byteArray);
                Response.Clear();
                MemoryStream ms = stream;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", "attachment;filename=" + numeFis +".xls");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



    }
}