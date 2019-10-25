using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class ProfileDetaliu : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();
                Dami.AccesAdmin();


                DataTable dtCmb = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""tblGrupUsers"" ", null);
                GridViewDataComboBoxColumn colGr = (grDate.Columns["IdGrup"] as GridViewDataComboBoxColumn);
                colGr.PropertiesComboBox.DataSource = dtCmb;

                if (!IsPostBack)
                {
                    string id = Request["id"].ToString();
                    
                    DataTable dt = new DataTable();
                    if (Request["tip"].ToString() == "Clone")
                        dt = General.IncarcaDT(@"SELECT * FROM ""tblProfileLinii"" WHERE ""Id""=@1", new object[] { -99 });
                    else
                        dt = General.IncarcaDT(@"SELECT * FROM ""tblProfileLinii"" WHERE ""Id""=@1", new object[] { id });


                    //dt = General.IncarcaDT(@"SELECT * FROM ""tblProfileLinii"" WHERE ""Id""=@1", new object[] { id });

                    switch (Request["tip"].ToString())
                    {
                        case "New":
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                //incarcam header-ul
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""tblProfile"" WHERE ""Id""=@1", new object[] { id });
                                if (dtHead.Rows.Count > 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["Id"].ToString();
                                    txtDenumire.Text = (dtHead.Rows[0]["Denumire"] ?? "").ToString();
                                    chkActiv.Checked = Convert.ToBoolean(dtHead.Rows[0]["Activ"] ?? 0);
                                    chkImplicit.Checked = Convert.ToBoolean(dtHead.Rows[0]["Implicit"] ?? 0);
                                }

                                //incarcam liniile
                                if (Request["tip"].ToString() == "Clone")
                                {
                                    DataTable dtOri = General.IncarcaDT(@"SELECT * FROM ""tblProfileLinii"" WHERE Id=@1", new object[] { id.ToString() });

                                    foreach (DataRow dr in dtOri.Rows)
                                    {
                                        DataRow drDes = dt.NewRow();
                                        drDes["Id"] = -99;
                                        drDes["IdGrup"] = dr["IdGrup"];
                                        drDes["IdUser"] = dr["IdUser"];
                                        drDes["IdCub"] = dr["IdCub"];
                                        drDes["USER_NO"] = Session["UserId"];
                                        drDes["TIME"] = DateTime.Now;
                                        dt.Rows.Add(drDes);
                                    }
                                }
                            }
                            break;
                    }

                    //dt.PrimaryKey = new DataColumn[] { dt.Columns["IdMeniu"] };
                    //Session["InformatiaCurenta"] = dt;
                    //grDate.DataSource = dt;
                    //grDate.DataBind();

                    ////dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"], dt.Columns["IdGrup"] , dt.Columns["IdUser"] , dt.Columns["IdCub"] };
                    ////grDate.KeyFieldName = "Id, IdGrup, IdUser, IdCub";
                    grDate.KeyFieldName = "Id; IdGrup; IdUser; IdCub";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"], dt.Columns["IdGrup"], dt.Columns["IdUser"], dt.Columns["IdCub"] };
                    Session["Date_Profile"] = dt;
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
                else
                {
                    grDate.DataSource = Session["Date_Profile"];
                    grDate.DataBind();
                }
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
                if (txtDenumire.Text == "") 
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipseste denumirea"), MessageBox.icoWarning);
                    return;
                }


                int id = -99;
                if (Request["tip"].ToString() == "Edit") id = Convert.ToInt32(Request["id"]);
                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""tblProfile"" WHERE Id=@1 ", new object[] { id.ToString() });
                DataTable dt = Session["Date_Profile"] as DataTable;

                //DataTable dt = Session["InformatiaCurenta"] as DataTable;
                var edc = Session["Profil_DataGrid"];


                DataRow drHead = dtHead.NewRow();

                switch (Request["tip"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) FROM ""tblProfile"" ", null) ?? 0) + 1;
                            drHead["Id"] = id;
                            dtHead.Rows.Add(drHead);
                            drHead["Continut"] = Session["Profil_DataGrid"];       // grDate.SaveClientLayout();

                            foreach (DataRow dr in dt.Rows)
                            {
                                dr["Id"] = id;
                            }
                        }
                        break;
                    case "Edit":
                        {
                            drHead = dtHead.Rows[0];
                        }
                        break;
                }

                //var ert = Session["Profil_DataGrid"];
                //drHead["Pagina"] = Dami.PaginaWeb();
                drHead["Pagina"] = General.Nz(Session["PaginaWeb"], "").ToString().Replace("\\", ".");
                drHead["Denumire"] = txtDenumire.Text;
                drHead["Activ"] = chkActiv.Checked;
                drHead["Implicit"] = chkImplicit.Checked;
                drHead["TIME"] = DateTime.Now;
                drHead["USER_NO"] = Session["UserId"];

                General.SalveazaDate(dt, "tblProfileLinii");
                General.SalveazaDate(dtHead, "tblProfile");

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY2", "window.parent.popGen.Hide();", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Date_Profile"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Date_Profile"] as DataTable;
                object[] row = new object[dt.Columns.Count];
                DataRow dr = dt.NewRow();
                dr["Id"] = Request["id"];
                dr["IdGrup"] = e.NewValues["IdGrup"] ?? -99;
                dr["IdUser"] = -99;
                dr["IdCub"] = -99;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;
                dr["IdAuto"] = 23;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["Date_Profile"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["Id"] = Request["id"];
                e.NewValues["IdUser"] = -99;
                e.NewValues["IdCub"] = -99;
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}