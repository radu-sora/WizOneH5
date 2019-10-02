using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;

namespace WizOne.Pagini
{
    public partial class MeniuSecuritate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();


                txtTitlu.Text = General.VarSession("Titlu").ToString();

                foreach (var c in grDate.Columns)
                {
                    try
                    {
                        TreeListDataColumn col = (TreeListDataColumn)c;
                        col.Caption = Dami.TraduCuvant(col.Caption ?? col.FieldName);
                    }
                    catch (Exception){}
                }


                if (!IsPostBack)
                {
                    DataTable dtCmb = General.IncarcaDT(@"SELECT * FROM ""tblGrupUsers"" ", null);
                    cmbGr.DataSource = dtCmb;
                    cmbGr.DataBind();
                }
                else
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }

                grDate.ExpandAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbGr.Value == null)
                {
                    MessageBox.Show("Nu exista grup de utilizatori selectat", MessageBox.icoInfo, "");
                    return;
                }

                General.ExecutaNonQuery(@"DELETE FROM ""relGrupMeniu2"" WHERE ""IdGrup""=@1", new object[] { cmbGr.Value });

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""relGrupMeniu2"" WHERE ""IdGrup""=@1 ", new object[] { cmbGr.Value });

                foreach(TreeListNode node in grDate.GetAllNodes())
                {
                    if (node.Selected == true)
                    {
                        DataRow dr = dt.NewRow();
                        dr["IdMeniu"] = node.Key;
                        dr["IdGrup"] = cmbGr.Value;
                        dt.Rows.Add(dr);
                    }
                }

                General.SalveazaDate(dt, "relGrupMeniu2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbGr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //string strSql = @"SELECT (B.""Id"" * 100000 ) + B.""IdMeniu"" AS ""Id"", 
                //                CASE WHEN B.""Parinte""=0 THEN B.""Parinte"" ELSE (B.""Id"" * 100000 ) + B.""Parinte"" END AS ""Parinte"", B.""Nume"",
                //                CASE WHEN C.""IdGrup"" IS NULL THEN 0 ELSE 1 END AS ""Bifat""
                //                FROM ""MeniuLista"" A
                //                INNER JOIN ""MeniuLinii"" B ON A.""Id"" = B.""Id""
                //                LEFT JOIN ""relGrupMeniu2"" C ON A.""Id"" = C.""IdMeniu"" AND  C.""IdGrup""=@1
                //                GROUP BY B.""Id"", B.""Parinte"", B.""IdMeniu"", B.""Nume"", B.""Ordine"", C.""IdGrup""
                //                ORDER BY B.""Parinte"", B.""Ordine"" ";


                //Radu 26.06.2018 - am adaugat si tab-urile din Personal
                string strSql = @"SELECT B.""IdMeniu"", B.""Parinte"", B.""Nume"", CASE WHEN C.""IdGrup"" IS NULL THEN 0 ELSE 1 END AS ""Bifat""
                                FROM ""MeniuLinii"" B
                                LEFT JOIN ""relGrupMeniu2"" C ON B.""IdMeniu"" = C.""IdMeniu"" AND  C.""IdGrup""=@1
                                GROUP BY B.""IdMeniu"", B.""Parinte"", B.""Nume"", B.""Ordine"", C.""IdGrup""

                                UNION

                                select (-1) * B.""IdAuto"" as ""IdMeniu"", 
                                (select ""IdMeniu"" from ""MeniuLinii"" where ""IdNomen"" = (select ""Id"" from ""tblMeniuri"" where ""Pagina"" = 'Personal\Lista')) as ""Parinte"", 
                                B.""Denumire"",  CASE WHEN C.""IdGrup"" IS NULL THEN 0 ELSE 1 END AS ""Bifat""
                                FROM ""MP_tblTaburi"" B
                                LEFT JOIN ""relGrupMeniu2"" C ON (-1) * B.""IdAuto"" = C.""IdMeniu"" AND  C.""IdGrup"" = @1
                                ORDER BY ""Parinte"" ";

                DataTable dt = General.IncarcaDT(strSql, new object[] { General.Nz(cmbGr.Value,-99) });

                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();

                //for(int i=0; i < grDate.Nodes.Count; i++)
                foreach (TreeListNode node in grDate.GetAllNodes())
                {
                    //TreeListNode node = grDate.Nodes[i];
                    DataRowView row = node.DataItem as DataRowView;
                    object[] obj = row.Row.ItemArray;
                    if (Convert.ToInt32(obj[3]) == 0)
                        node.Selected = false;
                    else
                        node.Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



    }
}