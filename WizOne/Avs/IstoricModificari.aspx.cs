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

namespace WizOne.Avs
{
    public partial class IstoricModificari : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion


                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                

                if (!IsPostBack)
                {
                    //txtTitlu.Text = (Session["Titlu"] ?? "").ToString();

        
                    DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                    cmbAngFiltru.DataSource = dtAng;
					Session["Modif_Avans_Angajati"] = dtAng;
                    cmbAngFiltru.DataBind();
                    cmbAngFiltru.SelectedIndex = -1;

                    //DataTable dtAtr = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"Avs_tblAtribute\" ORDER BY \"Id\"", null);
                    //cmbAtributeFiltru.DataSource = dtAtr;
                    //cmbAtributeFiltru.DataBind();                   
                 
                }
                else
                {
                    if (IsCallback)
                    {
                        cmbAngFiltru.DataSource = null;
                        cmbAngFiltru.Items.Clear();
                        cmbAngFiltru.DataSource = Session["Modif_Avans_Angajati"];
                        cmbAngFiltru.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }
        

        private string SelectAngajati()
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                        FROM (
                        SELECT A.F10003
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        WHERE B.""IdUser""= {Session["UserId"]}) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }

            return strSql;
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
                    case "6":
                        StergeFiltre();
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

                //if (cmbAngFiltru.SelectedIndex >= 0) filtru += " AND a.F10003 = " + cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString();
                //if (cmbAtributeFiltru.SelectedIndex >= 0) filtru += " AND a.\"IdAtribut\" = " + cmbAtributeFiltru.Items[cmbAtributeFiltru.SelectedIndex].Value.ToString();

                if (cmbAngFiltru.SelectedIndex < 0)
                {
                    ArataMesaj("Nu ati selectat niciun angajat!");
                    return;
                }

                //if (cmbAtributeFiltru.SelectedIndex >= 0)
                //{
                //    lst.Add(Convert.ToInt32(cmbAtributeFiltru.Items[cmbAtributeFiltru.SelectedIndex].Value.ToString()));
                //}
                //else
                //{

                //}

                string sql = "SELECT * FROM F100 WHERE F10003 = " + cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString();
                DataTable dttmp = General.IncarcaDT(sql, null);
                string numeAng = dttmp.Rows[0]["F10008"].ToString() + " " + dttmp.Rows[0]["F10009"].ToString();

                sql = "SELECT * FROM F010";
                dttmp = General.IncarcaDT(sql, null);
                int luna = Convert.ToInt32(dttmp.Rows[0]["F01012"].ToString());
                int an = Convert.ToInt32(dttmp.Rows[0]["F01011"].ToString());

                dt = pag.GetIstoricDateContract(Convert.ToInt32(cmbAngFiltru.Items[cmbAngFiltru.SelectedIndex].Value.ToString()), lst, luna + 1 > 12 ? 1 : luna, luna + 1 > 12 ? an + 1 : an, numeAng, 1);
                grDate.KeyFieldName = "NumeAngajat;DataModif";
                grDate.DataSource = dt;
                grDate.DataBind();
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }



        protected void StergeFiltre()
        {
            cmbAngFiltru.SelectedIndex = -1;
            cmbAngFiltru.Value = null;
            //cmbAtributeFiltru.SelectedIndex = -1;
            //cmbAtributeFiltru.Value = null;          

        }   




        private void ArataMesaj(string mesaj)
        {
            try
            {
                pnlCtl.Controls.Add(new LiteralControl());
                WebControl script = new WebControl(HtmlTextWriterTag.Script);
                pnlCtl.Controls.Add(script);
                script.Attributes["id"] = "dxss_123456";
                script.Attributes["type"] = "text/javascript";
                script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

                    //<td>
                    //    <label id = "lblAtrFiltru" runat="server" style="display:inline-block;">Atribut</label>
                    //    <dx:ASPxComboBox ID = "cmbAtributeFiltru" runat="server" ClientInstanceName="cmbAtributeFiltru" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    //        TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >                           
                    //    </dx:ASPxComboBox>
                    //</td> 


    }
}