using DevExpress.Web;
using DevExpress.Web.Data;
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

namespace WizOne.Eval
{
    public partial class Form1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session["Eval_ActiveTab"] = 1;
                Session["CompletareChestionar_F10003"] = 9;
                Session["CompletareChestionar_IdQuiz"] = 47;

                DataTable tableBBB = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = @1 AND ""F10003"" = @2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                tableBBB.TableName = "Eval_RaspunsLinii";

                //Florin 2020.11.13
                Session["Eval_RaspunsLinii_Tabel"] = tableBBB;

                //if (!IsPostBack)
                    divIntrebari.Controls.Add(CreeazaTabel(1008, "1"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private ASPxGridView CreeazaTabel(int id, string super)
        {
            ASPxGridView gr = new ASPxGridView();
            try
            {
                #region GridProperties
                gr.Width = new Unit(100, UnitType.Percentage);
                gr.ID = "grTabela" + "_WXY_" + id.ToString();
                gr.ClientInstanceName = "grTabela" + "_WXY_" + id.ToString();
                gr.ClientIDMode = ClientIDMode.Static;
                gr.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;
                gr.SettingsText.ConfirmDelete = "";

                gr.SettingsBehavior.AllowFocusedRow = true;
                gr.SettingsBehavior.EnableCustomizationWindow = true;
                gr.SettingsBehavior.AllowSelectByRowClick = true;
                gr.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn;
                gr.Settings.ShowFilterRow = false;
                gr.Settings.ShowGroupPanel = false;
                gr.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                gr.SettingsSearchPanel.Visible = false;
                gr.AutoGenerateColumns = false;
                gr.ClientSideEvents.ContextMenu = "ctx";

                gr.SettingsBehavior.ConfirmDelete = true;
                gr.SettingsText.ConfirmDelete = "Confirmati operatia de stergere?";

                gr.SettingsEditing.Mode = GridViewEditingMode.Batch;
                gr.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                gr.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;
                gr.SettingsEditing.BatchEditSettings.ShowConfirmOnLosingChanges = false;

                gr.BatchUpdate += gr_BatchUpdate;
                //gr.InitNewRow += gr_InitNewRow;
                Session["NumeGriduri"] += ";" + gr.ID;

                #endregion

                #region Grid Command Buttons
                gr.SettingsCommandButton.NewButton.Image.ToolTip = "Rand nou";
                gr.SettingsCommandButton.NewButton.Image.Url = "~/Fisiere/Imagini/Icoane/New.png";

                gr.SettingsCommandButton.DeleteButton.Image.ToolTip = "Sterge";
                gr.SettingsCommandButton.DeleteButton.Image.Url = "~/Fisiere/Imagini/Icoane/sterge.png";
                #endregion

                GridViewCommandColumn colCommand = new GridViewCommandColumn();
                colCommand.Width = 80;
                colCommand.ShowDeleteButton = true;
                colCommand.ShowNewButtonInHeader = true;
                colCommand.VisibleIndex = 0;
                colCommand.Caption = " ";
                colCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                gr.Columns.Add(colCommand);

                //string[] arr = { "IdQuiz", "F10003", "Id", "Linia", "TipData", "Descriere", super, "USER_NO", "TIME" };
                string[] arr = { "IdQuiz", "F10003", "Id", "Linia", "TipData", "USER_NO", "TIME" };

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    GridViewDataTextColumn col = new GridViewDataTextColumn();
                    col.FieldName = arr[i];
                    col.Name = Dami.TraduCuvant(arr[i]);
                    col.Caption = Dami.TraduCuvant(arr[i]);
                    col.Visible = false;
                    col.ShowInCustomizationForm = false;
                    gr.Columns.Add(col);
                }

                DataTable dtConfig = General.IncarcaDT(@"SELECT * FROM ""Eval_ConfigTipTabela"" WHERE ""IdQuiz""=@1 AND ""IdLinie""=@2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), id });
                for (int i = 0; i < dtConfig.Rows.Count; i++)
                {
                    DataRow dr = dtConfig.Rows[i];
                    string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + dr["Coloana"];

                    GridViewDataTextColumn col = new GridViewDataTextColumn();
                    col.FieldName = camp;
                    col.Name = camp;
                    col.Caption = Dami.TraduCuvant(General.Nz(dr["Alias"], "Coloana " + dr["Coloana"]).ToString());
                    col.Width = Convert.ToInt32(General.Nz(dr["Lungime"], 250));
                    col.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
                    gr.Columns.Add(col);
                }

                //gr.DataSource = lstEval_RaspunsLinii.Where(p => p.Id == id);
                DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                DataRow[] arrDr = dtTbl.Select("Id=" + id);
                //dtTbl.PrimaryKey = new DataColumn[] { dtTbl.Columns["IdQuiz"], dtTbl.Columns["F10003"], dtTbl.Columns["Id"], dtTbl.Columns["Linia"] };
                if (arrDr.Count() > 0)
                    gr.DataSource = arrDr.CopyToDataTable();
                else
                    gr.DataSource = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE 1=2");
                //gr.DataSource = dtTbl;
                //gr.KeyFieldName = "IdQuiz; F10003; Id; Linia";
                gr.KeyFieldName = "IdQuiz;F10003;Id;Linia";
                gr.DataBind();
                //tableBBB.Columns["IdQuiz"], tableBBB.Columns["F10003"], tableBBB.Columns["Id"], tableBBB.Columns["Linia"]
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return gr;
        }

        private void gr_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                ASPxGridView grid = sender as ASPxGridView;
                grid.CancelEdit();

                DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;

                for (int x = 0; x < e.InsertValues.Count; x++)
                {
                    ASPxDataInsertValues vals = e.InsertValues[x] as ASPxDataInsertValues;
                    DataRow dr = dtTbl.NewRow();
                    dr["F10003"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                    dr["IdQuiz"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                    dr["Id"] = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
                    dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    dr["TIME"] = DateTime.Now;

                    int max = Convert.ToInt32(dtTbl.Compute("MAX(Linia)", "Id=" + dr["Id"]));
                    dr["Linia"] = max + 1;

                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        dr[camp] = vals.NewValues[camp];
                    }

                    dtTbl.Rows.Add(dr);
                }

                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues vals = e.UpdateValues[x] as ASPxDataUpdateValues;

                    object[] keys = new object[vals.Keys.Count];
                    for (int y = 0; y < vals.Keys.Count; y++)
                    { keys[y] = vals.Keys[y]; }

                    DataRow dr = dtTbl.Rows.Find(keys);

                    dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    dr["TIME"] = DateTime.Now;

                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        dr[camp] = vals.NewValues[camp];
                    }
                }

                for (int x = 0; x < e.DeleteValues.Count; x++)
                {
                    ASPxDataDeleteValues vals = e.DeleteValues[x] as ASPxDataDeleteValues;

                    object[] keys = new object[vals.Keys.Count];
                    for (int y = 0; y < vals.Keys.Count; y++)
                    { keys[y] = vals.Keys[y]; }

                    DataRow dr = dtTbl.Rows.Find(keys);
                    dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    dr["TIME"] = DateTime.Now;

                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        dr[camp] = DBNull.Value;
                    }
                }

                Session["Eval_RaspunsLinii_Tabel"] = dtTbl;
                General.SalveazaDate(dtTbl, "Eval_RaspunsLinii");

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SalveazaGridurile();
            DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
            General.SalveazaDate(dtTbl, "Eval_RaspunsLinii");
        }

        public void SalveazaGridurile()
        {
            try
            {
                if (General.Nz(Session["NumeGriduri"], "").ToString() != "")
                {
                    string[] arr = Session["NumeGriduri"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ASPxGridView grDate = divIntrebari.FindControl(arr[i]) as ASPxGridView;
                        if (grDate != null)
                            grDate.UpdateEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlSectiune_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                SalveazaGridurile();
                DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                General.SalveazaDate(dtTbl, "Eval_RaspunsLinii");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}