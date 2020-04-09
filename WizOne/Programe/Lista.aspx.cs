using DevExpress.Web;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Programe
{
    public partial class Lista : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                txtTitlu.Text = (Session["Titlu"] ?? "").ToString();

                if (!IsPostBack)
                {
                    IncarcaGrid();
                }
                else
                {
                    if (grDate.IsCallback)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_Programe"" ORDER BY ""Id""", null);

                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["InformatiaCurenta"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    {                        
                        return;
                    }

                    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "Denumire" }) as object[];

                    int idPrg = Convert.ToInt32(arr[1]);

                    switch (arr[0])
                    {
                        case "btnEdit":
                            {
                                string url = "~/Programe/Detalii.aspx";
                                if (url != "")
                                {
                                    Session["InformatiaCurenta"] = null;
                                    Session["IdProgram"] = arr[1];
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                }
                            }
                            break;
                        case "btnDuplica":
                            DuplicarePrg(Convert.ToInt32(arr[1]));
                            grDate.DataBind();
                            break;
                        case "btnSterge":
                            {
                                int are = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(""IdProgram"") FROM ""Ptj_Intrari"" WHERE ""IdProgram""=@1", new object[] { idPrg }));
                                if (are > 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Acest program a fost deja folosit in pontaj." + Environment.NewLine + "Nu se mai poate sterge.");
                                    return;
                                }

                                string strSql = $@"SELECT CONVERT(nvarchar(10),F10003) + ',' FROM ""F100Contracte"" WHERE ""IdContract""=@1 AND CAST(""DataInceput"" AS DATE) <= {General.CurrentDate()} AND {General.CurrentDate()} <= CAST(""DataSfarsit"" AS DATE) FOR XML PATH ('')";
                                if (Constante.tipBD == 2)
                                    strSql = $@"SELECT LISTAGG(F10003, ',') WITHIN GROUP (ORDER BY F10003) AS Marci FROM ""F100Contracte"" WHERE ""IdContract""=@1 AND CAST(""DataInceput"" AS DATE) <= {General.CurrentDate()} AND {General.CurrentDate()} <= CAST(""DataSfarsit"" AS DATE)";

                                string marci = General.Nz(General.ExecutaScalar(strSql, new object[] { idPrg }), "").ToString();
                                if (marci != "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Urmatoarele marci au atribut acest contract:" + Environment.NewLine + marci);
                                    return;
                                }

                                General.ExecutaNonQuery(
                                    $@"BEGIN
                                        DELETE FROM ""F100Contracte2"" WHERE ""IdContract""=@1;
                                        DELETE FROM ""Ptj_ContracteAbsente"" WHERE ""IdContract""=@1;
                                        DELETE FROM ""Ptj_ContracteSchimburi"" WHERE ""IdContract""=@1;
                                        DELETE FROM ""Ptj_Contracte"" WHERE ""Id""=@1;
                                    END;", new object[] { idPrg });
                                IncarcaGrid();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "~/Programe/Detalii.aspx";
                if (url != "")
                {
                    Session["IdProgram"] = -99;
                    Session["InformatiaCurenta"] = null;
                    if (Page.IsCallback)
                        ASPxWebControl.RedirectOnCallback(url);
                    else
                        Response.Redirect(url, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        public void ProgramSterge(int id)
        {
            try
            {
                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgrameAlteOre\" WHERE \"IdProgram\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgramePauza\" WHERE \"IdProgram\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgrameTrepte\" WHERE \"IdProgram\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"Ptj_Programe\" WHERE \"Id\" = " + id, null);

                //Florin 2019.08.21
                General.ExecutaNonQuery("DELETE FROM \"Ptj_ProgrameOreNoapte\" WHERE \"IdProgram\" = " + id, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        public int ProgramUrmatorulId()
        {
            try
            {
                int val = 0;
                string sql = "SELECT MAX(\"Id\") FROM \"Ptj_Programe\"";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows. Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)                
                    val = Convert.ToInt32(dt.Rows[0][0].ToString());                    
                
                return (val + 1);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
                return 1;
            }
        }


        private void DuplicarePrg(int id)
        {

            int idUrm = ProgramUrmatorulId();

            string[] listaTabele = { "Ptj_Programe", "Ptj_ProgrameAlteOre", "Ptj_ProgramePauza", "Ptj_ProgrameTrepte", "Ptj_ProgrameOreNoapte" };

            foreach (string tabela in listaTabele)
            {
                DataTable dtrPrg = General.IncarcaDT("SELECT * FROM \"" + tabela + "\" WHERE " + (tabela == "Ptj_Programe" ? "\"Id\" = " : "\"IdProgram\" = ") + id, null);

                string sqlPrg = "";
                if (dtrPrg != null && dtrPrg.Rows.Count > 0)
                {
                    sqlPrg = "INSERT INTO \"" + tabela + "\" (";
                    for (int i = 0; i < dtrPrg.Columns.Count; i++)
                    {
                        if (dtrPrg.Columns[i].ColumnName != "IdAuto")
                        {
                            sqlPrg += "\"" + dtrPrg.Columns[i].ColumnName + "\"";
                            if (i < dtrPrg.Columns.Count - 1)
                                sqlPrg += ", ";
                        }
                        else
                            if (sqlPrg.Length > 2 && i == dtrPrg.Columns.Count - 1)
                            sqlPrg = sqlPrg.Substring(0, sqlPrg.Length - 2);
                    }
                    sqlPrg += ") VALUES (";
                    for (int i = 0; i < dtrPrg.Columns.Count; i++)
                    {
                        if (dtrPrg.Columns[i].ColumnName != "IdAuto")
                        {
                            switch (dtrPrg.Columns[i].ColumnName)
                            {
                                case "Id":
                                case "IdProgram":
                                    sqlPrg += idUrm;
                                    break;
                                case "Denumire":
                                    sqlPrg += "'" + dtrPrg.Rows[0]["Denumire"].ToString() + " - Copie'";
                                    break;
                                case "USER_NO":
                                    sqlPrg += Convert.ToInt32(Session["UserId"].ToString());
                                    break;
                                case "TIME":
                                    sqlPrg += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                    break;
                                default:
                                    if (dtrPrg.Rows[0][dtrPrg.Columns[i].ColumnName] == null || dtrPrg.Rows[0][dtrPrg.Columns[i].ColumnName].ToString().Length <= 0)
                                        sqlPrg += "NULL";
                                    else
                                    {
                                        switch (dtrPrg.Columns[i].DataType.ToString())
                                        {
                                            case "System.String":
                                                sqlPrg += "'" + dtrPrg.Rows[0][dtrPrg.Columns[i].ColumnName].ToString() + "'";
                                                break;
                                            case "System.DateTime":
                                                DateTime dt = Convert.ToDateTime(General.Nz(dtrPrg.Rows[0][dtrPrg.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                                                sqlPrg += General.ToDataUniv(dt);
                                                break;
                                            default:
                                                sqlPrg += dtrPrg.Rows[0][dtrPrg.Columns[i].ColumnName].ToString();
                                                break;
                                        }
                                    }
                                    break;
                            }
                            if (i < dtrPrg.Columns.Count - 1)
                                sqlPrg += ", ";
                        }
                        else
                        {
                            if (sqlPrg.Length > 2 && i == dtrPrg.Columns.Count - 1)
                                sqlPrg = sqlPrg.Substring(0, sqlPrg.Length - 2);
                        }
                    }

                    sqlPrg += ")";
                }
                if (sqlPrg.Length > 0)
                    General.IncarcaDT(sqlPrg, null);
            }
        }




    }
}