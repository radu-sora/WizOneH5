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

namespace WizOne.Pagini
{
    public partial class SablonLista : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
              
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnClone.Image.ToolTip = Dami.TraduCuvant("btnClone", "Duplicare");
                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                grDate.DataBind();  //Radu 24.09.2020 - am mutat aici (din Page_Init), deoarece nu se actualizau ToolTip-urile
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "";
                int id = -99;

                switch (Session["Sablon_Tabela"].ToString())
                {
                    case "Stiri":
                        url = "~/Pagini/StiriDetaliu.aspx";
                        break;
                    case "Intro":
                        url = "~/Pagini/IntroDetaliu.aspx";
                        break;
                    case "MeniuLista":
                        url = "~/Pagini/MeniuDetaliu.aspx";
                        break;
                    case "Rec_Candidati":
                        url = "~/Recrutare/Candidati.aspx";
                        id = Dami.NextId("Rec_Candidati");
                        break;
                    /*LeonardM 16.10.2017*/
                    case "Eval_Obiectiv":
                        url = "~/Eval/ActivitatiObiective.aspx";
                        break;
                    case "Eval_CategCompetente":
                        url = "~/Eval/Competente.aspx";
                        break;
                    case "Eval_ListaObiectiv":
                        url = "~/Eval/ListaObiective.aspx";
                        break;
                    case "Eval_SetCalificativ":
                        url = "~/Eval/Calificative.aspx";
                        break;
                    //Radu 24.04.2018
                    case "Eval_tblTipValori":
                        url = "~/Eval/ValoriLinii.aspx";
                        break;
                }

                if (url != "")
                {
                    Session["Sablon_CheiePrimara"] = id;
                    Session["Sablon_TipActiune"] = "New";
                    Response.Redirect(url, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid(bool AdaugaColoanele = false)
        {
            DataTable dt = new DataTable();

            try
            {
                switch (Session["Sablon_Tabela"].ToString())
                {
                    case "Stiri":
                        dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""DataInceput"" AS ""Data Inceput"", ""DataSfarsit"" AS ""Data Sfarsit"", ""Activ"" FROM ""Stiri"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    case "Intro":
                        dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Activ"" FROM ""Intro"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    case "MeniuLista":
                        dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Descriere"" FROM ""MeniuLista"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    case "Rec_Candidati":
                        dt = General.IncarcaDT(@"SELECT ""Id"", ""Nume"", ""Prenume"", ""AdresaCompleta"", ""Judet"", ""Localitate"" FROM ""Rec_Candidati"" ORDER BY TIME DESC ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    /*LeonardM 16.10.2017*/
                    case "Eval_Obiectiv":
                        dt = General.IncarcaDT(@"SELECT ""IdObiectiv"" as ""Id"", ""Obiectiv"" FROM ""Eval_Obiectiv"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    case "Eval_CategCompetente":
                        dt = General.IncarcaDT(
                                @"SELECT A.""IdCategorie"" as ""Id"", A.""CodCategorie"", A.""DenCategorie"", A.""Sectiune"", A.""Subsectiune"", B.""Denumire"" AS ""Calificativ"" 
                                FROM ""Eval_CategCompetente"" A
                                LEFT JOIN ""Eval_tblTipValori"" B ON A.""IdCalificativ""=B.""Id""", null);
                         grDate.KeyFieldName = "Id";
                        break;
                    case "Eval_ListaObiectiv":
                        dt = General.IncarcaDT(@"select ""IdLista"" as ""Id"", ""CodLista"", ""DenLista"" from ""Eval_ListaObiectiv"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    case "Eval_SetCalificativ":
                        dt = General.IncarcaDT(@"select ""IdSet"" as ""Id"", ""CodSet"" as ""Cod"", ""DenSet"" as ""Den"" from ""Eval_SetCalificativ"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                    //Radu 24.04.2018
                    case "Eval_tblTipValori":
                        dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Eval_tblTipValori"" ", null);
                        grDate.KeyFieldName = "Id";
                        break;
                }

                grDate.DataSource = dt;

                //if (AdaugaColoanele)
                if (grDate.Columns.Count <= 1)
                {
                    //adugam coloanele
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName.ToLower() == "activ")
                        {
                            GridViewDataCheckColumn c = new GridViewDataCheckColumn();
                            c.Name = col.ColumnName;
                            c.FieldName = col.ColumnName;
                            c.Caption = Dami.TraduCuvant(col.ColumnName);
                            c.ReadOnly = true;
                            grDate.Columns.Add(c);
                        }
                        else
                        {
                            GridViewDataColumn c = new GridViewDataColumn();
                            c.Name = col.ColumnName;
                            c.FieldName = col.ColumnName;
                            c.Caption = Dami.TraduCuvant(col.ColumnName);
                            c.ReadOnly = true;
                            grDate.Columns.Add(c);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
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
                        MessageBox.Show("Insuficienti parametrii", MessageBox.icoError, "Eroare !");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnEdit":
                        case "btnClone":
                            {
                                string url = "~/Pagini/MainPage.aspx";
                                switch (Session["Sablon_Tabela"].ToString())
                                {
                                    case "Stiri":
                                        url = "~/Pagini/StiriDetaliu.aspx";
                                        break;
                                    case "Intro":
                                        url = "~/Pagini/IntroDetaliu.aspx";
                                        break;
                                    case "MeniuLista":
                                        url = "~/Pagini/MeniuDetaliu.aspx";
                                        break;
                                    case "Rec_Candidati":
                                        url = "~/Recrutare/Candidati.aspx";
                                        break;
                                    /*LeonardM 16.10.2017*/
                                    case "Eval_Obiectiv":
                                        url = "~/Eval/ActivitatiObiective.aspx";
                                        break;
                                    case "Eval_CategCompetente":
                                        url = "~/Eval/Competente.aspx";
                                        break;
                                    case "Eval_ListaObiectiv":
                                        url = "~/Eval/ListaObiective.aspx";
                                        break;
                                    case "Eval_SetCalificativ":
                                        url = "~/Eval/Calificative.aspx";
                                        break;
                                    //Radu 24.04.2018
                                    case "Eval_tblTipValori":
                                        url = "~/Eval/ValoriLinii.aspx";
                                        break;                                        
                                }
                                if (url != "")
                                {
                                    Session["Sablon_CheiePrimara"] = arr[1];
                                    Session["Sablon_TipActiune"] = arr[0].Replace("btn", "");

                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                }
                            }
                            break;
                        case "btnDelete":
                            {
                                switch (Session["Sablon_Tabela"].ToString())
                                {
                                    case "Stiri":
                                        General.ExecutaNonQuery(@"BEGIN
                                                  DELETE FROM ""relGrupStire"" WHERE ""IdStire""=@1;
                                                  DELETE FROM ""Stiri"" WHERE ""Id""=@1;
                                                  END;", new string[] { arr[1] });
                                        break;
                                    case "Intro":
                                        General.ExecutaNonQuery(@"BEGIN
                                                  DELETE FROM ""Intro"" WHERE ""Id""=@1;
                                                  DELETE FROM ""relGrupIntro"" WHERE ""IdIntro""=@1;
                                                  END;", new string[] { arr[1] });
                                        break;
                                    case "MeniuLista":
                                        General.ExecutaNonQuery(@"BEGIN
                                                  DELETE FROM ""relGrupMeniu2"" WHERE IdMeniu=@1;
                                                  DELETE FROM ""MeniuLinii"" WHERE ""Id""=@1;
                                                  DELETE FROM ""MeniuLista"" WHERE ""Id""=@1;
                                                  END;", new string[] { arr[1] });
                                        break;
                                    case "Rec_Candidati":
                                        General.ExecutaNonQuery(@"BEGIN
                                                  DELETE FROM ""Rec_Candidati"" WHERE Id=@1;
                                                  END;", new string[] { arr[1] });
                                        break;
                                    /*LeonardM 16.10.2017*/
                                    case "Eval_Obiectiv":
                                        General.ExecutaNonQuery(@"BEGIN
                                                   DELETE FROM ""Eval_ListaObiectivDet"" where ""IdObiectiv"" = @1;
                                                   DELETE FROM ""Eval_ObiectivXActivitate"" where ""IdObiectiv"" = @1;
                                                   DELETE FROM ""Eval_Obiectiv"" where ""IdObiectiv"" = @1;
                                                   END; ", new string[] { arr[1] });
                                        break;
                                    case "Eval_CategCompetente":
                                        General.ExecutaNonQuery(@"begin
                                                delete from ""Eval_CategCompetenteDet"" where ""IdCategorie"" = @1;
                                                delete from ""Eval_CategCompetente"" where ""IdCategorie"" = @1;
                                                end; ", new string[] { arr[1] });
                                        break;
                                    case "Eval_ListaObiectiv":
                                        General.ExecutaNonQuery(@"begin
                                                delete from ""Eval_ListaObiectivDet"" where ""IdLista"" = @1;
                                                delete from ""Eval_ListaObiectiv"" where ""IdLista"" = @1;
                                                end; ", new string[] { arr[1] });
                                        break;
                                    case "Eval_SetCalificativ":
                                        General.ExecutaNonQuery(@"begin
                                                delete from ""Eval_SetCalificativDet"" where ""IdSet"" = @1;
                                                delete from ""Eval_SetCalificativ"" where ""IdSet"" = @1;
                                                end;", new string[] { arr[1] });
                                        break;
                                    //Radu 24.04.2018
                                    case "Eval_tblTipValori":
                                        General.ExecutaNonQuery(@"BEGIN
                                                   DELETE FROM ""Eval_tblTipValoriLinii"" where ""Id"" = @1;
                                                   DELETE FROM ""Eval_tblTipValori"" where ""Id"" = @1;
                                                   END; ", new string[] { arr[1] });
                                        break;
                                }

                                grDate.DataBind();

                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.Items.Clear();
                        cmb.ValueType = chk.PropertiesCheckEdit.ValueType;
                        cmb.Items.Add(string.Empty, null);
                        cmb.Items.Add(Dami.TraduCuvant("BIfat"), chk.PropertiesCheckEdit.ValueChecked);
                        cmb.Items.Add(Dami.TraduCuvant("Nebifat"), chk.PropertiesCheckEdit.ValueUnchecked);
                    }
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