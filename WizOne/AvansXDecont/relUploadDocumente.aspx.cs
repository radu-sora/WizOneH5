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

namespace WizOne.AvansXDecont
{
    public partial class relUploadDocumente : System.Web.UI.Page
    {
        public class metaUploadFile
        {
            public byte[] UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);
       
                //btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnIncarca.ToolTip = Dami.TraduCuvant("btnIncarca", "Incarca document");

                string qwe = General.Nz(Request["qwe"], "-99").ToString();
                int tip = Convert.ToInt32(General.Nz(Request["tip"], -99));

                if (tip == 0)
                    btnIncarca.Visible = false;

                if (!IsPostBack)
                    Session["relUploadDocumente_Grid"] = null;

                IncarcaGrid(qwe);

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

      

        private void IncarcaGrid(string id)
        {
            try
            {
                DataTable dt = new DataTable();

                string strSql = @"SELECT * FROM ""tblFisiere"" WHERE ""Id"" IN (SELECT ""IdDocument"" FROM ""AvsXDec_relUploadDocumente"" WHERE ""DocumentId"" = {0} and ""DocumentDetailId"" = {1} ) AND ""Tabela"" = 'AvsXDec_relUploadDocumente'";
                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, id.Split(',')[0], id.Split(',')[1]);
                else
                    strSql = string.Format(strSql, id.Split(',')[0], id.Split(',')[1]);

                if (Session["relUploadDocumente_Grid"] == null)
                {
                    dt = General.IncarcaDT(strSql, null);
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["relUploadDocumente_Grid"] = dt;
                }
                else
                {
                    dt = Session["relUploadDocumente_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void btnIncarca_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_relUploadDocumente"] = itm;

                btnIncarca.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;

                int id = Dami.NextId("AvsXDec_relUploadDocumente", 1);
                string qwe = General.Nz(Request["qwe"], "-99").ToString();


                bool esteNou = false;
                string strSql = "SELECT * FROM \"tblFisiere\" WHERE \"Tabela\" = 'AvsXDec_relUploadDocumente' AND \"Id\" = " + id;
                DataTable dt = General.IncarcaDT(strSql, null);
                if (dt == null || dt.Rows.Count <= 0)
                    esteNou = true;

                string sql = "";

                if (esteNou)
                {
                    sql = "INSERT INTO \"AvsXDec_relUploadDocumente\" (\"DocumentId\", \"DocumentDetailId\", \"IdDocument\", USER_NO, TIME) "
                            + " VALUES ({0}, {1}, {2}, {3}, {4})";
                    sql = string.Format(sql, qwe.Split(',')[0], qwe.Split(',')[1], id, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));

                    General.ExecutaNonQuery(sql, null);


                    sql = "INSERT INTO \"tblFisiere\" (\"Tabela\", \"Id\", \"Fisier\", \"FisierNume\", \"FisierExtensie\", USER_NO, TIME) "
                        + " VALUES ('AvsXDec_relUploadDocumente', {0}, @1, '{1}', '{2}', {3}, {4})";
                }
                else
                {

                    sql = "UPDATE \"tblFisiere\" SET \"Fisier\" = @1, \"FisierNume\" = '{1}', \"FisierExtensie\" = '{2}', USER_NO = {3}, TIME = {4} WHERE \"Tabela\" = 'AvsXDec_relUploadDocumente' AND \"Id\" = {0} ";

                }
                sql = string.Format(sql, id, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));
                General.ExecutaNonQuery(sql, new object[] { itm.UploadedFile });
                Session["relUploadDocumente_Grid"] = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        } 



        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["relUploadDocumente_Grid"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                string strSql = @"BEGIN ";
                strSql += @"DELETE FROM ""tblFisiere"" WHERE ""Tabela"" = 'AvsXDec_relUploadDocumente' AND ""Id""={2};";
                strSql += @"DELETE FROM ""AvsXDec_relUploadDocumente"" WHERE ""DocumentId"" = {0} AND {3} ""DocumentDetailId"", -99) = {1} AND ""IdDocument""={2};";
                strSql += @"END; ";

                string qwe = General.Nz(Request["qwe"], "-99").ToString();

                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, qwe.Split(',')[0], qwe.Split(',')[1], row["Id"].ToString(), "isnull( ");
                else
                    strSql = string.Format(strSql, qwe.Split(',')[0], qwe.Split(',')[1], row["Id"].ToString(), "nvl( ");

                General.ExecutaNonQuery(strSql, null);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["relUploadDocumente_Grid"] = dt;
                grDate.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string qwe = General.Nz(Request["qwe"], "-99").ToString();
            IncarcaGrid(qwe);
        }
    }
}