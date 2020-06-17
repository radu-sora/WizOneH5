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
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;

namespace WizOne.Absente
{
    public partial class AnulareCO : System.Web.UI.Page
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
                btnAnulare.Text = Dami.TraduCuvant("btnAnulare", "Anulare");

                if (!IsPostBack)
                    txtAnLuna.Value = DateTime.Now;

                DataTable dtPer = new DataTable();
                dtPer.Columns.Add("Id", typeof(int));
                dtPer.Columns.Add("Denumire", typeof(string));

                dtPer.Rows.Add(1, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 1).ToString() + " inclusiv");
                dtPer.Rows.Add(2, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 2).ToString() + " inclusiv");
                dtPer.Rows.Add(3, "Mai vechi");

                cmbPerioada.DataSource = dtPer;
                cmbPerioada.DataBind();

                DataTable dtAng = General.IncarcaDT(Dami.SelectAngajati(), null);
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                if (IsPostBack)
                {
                    DataTable dt = Session["AnulareCO_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                    int an = Convert.ToDateTime(txtAnLuna.Value).Year;

                    GridViewDataTextColumn colAnCurent = (grDate.Columns["ZileCOAnC"] as GridViewDataTextColumn);
                    GridViewDataTextColumn colAncurentAnt = (grDate.Columns["ZileCOAnAnt"] as GridViewDataTextColumn);
                    GridViewDataTextColumn colAnCurentAnt2 = (grDate.Columns["ZileCOAnAnt2"] as GridViewDataTextColumn);
                    colAnCurent.Caption = "Zile CO (" + an + ")";
                    colAncurentAnt.Caption = "Zile CO (" + (an - 1).ToString() + ")";
                    colAnCurentAnt2.Caption = "Zile CO (" + (an - 2).ToString() + ")";
                }
                else
                {
                    IncarcaGrid();
                }

                grDate.SettingsPager.PageSize = 20;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        internal static string SelectAngajati()
        {
            string strSql = "";

            try
            {
                strSql = $@"SELECT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS NumeComplet, G.F00406 AS Filiala, H.F00507 AS Sectie, I.F00608 AS Departament
                        FROM F100 A
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string ctrl = e.Parameter.Split(';')[0];
                switch (ctrl)
                {
                    case "txtAnLuna":                    
                        IncarcaGrid();
                        break;
                    case "cmbStare":
                        string filtru = "";
                        if (checkComboBoxStare.Value != null) filtru += FiltruTipStari(checkComboBoxStare.Value.ToString()).Substring(0, FiltruTipStari(checkComboBoxStare.Value.ToString()).Length - 1);

                        grDate.FilterExpression = filtru ;
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

                if (txtAnLuna.Value == null)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat luna si anul!");
                    return;
                }

                if (IsPostBack)
                {
                    DataTable dtPer = new DataTable();
                    dtPer.Columns.Add("Id", typeof(int));
                    dtPer.Columns.Add("Denumire", typeof(string));

                    dtPer.Rows.Add(1, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 1).ToString() + " inclusiv");
                    dtPer.Rows.Add(2, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 2).ToString() + " inclusiv");
                    dtPer.Rows.Add(3, "Mai vechi");
                }

                int an = Convert.ToDateTime(txtAnLuna.Value).Year;

                string data = General.ToDataUniv(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, 99);
                string idAuto = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) ";     
                if (Constante.tipBD == 2)                
                    idAuto = "ROWNUM";


                //string sql = "select " + idAuto + " as \"IdAuto\", a.F10003, CASE WHEN d.F10025 = 900 THEN 'Candidat' ELSE CASE WHEN d.F10025 = 999 THEN 'Angajat in avans' ELSE (CASE WHEN d.F10025 = 0 THEN "
                //                + "(CASE WHEN(d.F100925 <> 0 AND F100922 IS NOT NULL AND F100923 IS NOT NULL AND F100923 IS NOT NULL AND F100922 <= {0} AND {0} <= F100923 AND {0} <= F100924) "
                //                + "THEN 'Activ suspendat' ELSE CASE WHEN(d.F100915 <= {0} AND {0} <= d.F100916) THEN 'Activ detasat' ELSE 'Activ' END END) ELSE 'Inactiv' END) END END AS \"Stare\", "
                //                + " case when a.\"An\" is null then 0 else " + func + "(a.\"Cuvenite\", 0) + " + func + "(a.\"SoldAnterior\", 0) - " + func + "(a.\"Efectuate\", 0) - " + func + "(a.\"Anulate\", 0) end as \"ZileCO\", "
                //                + " case when a.\"An\" is null then 0 else case when " + func + "(a.\"SoldAnterior\", 0) - " + func + "(a.\"Anulate\", 0) < " + func + "(a.\"Efectuate\", 0) then " + func + "(a.\"Cuvenite\", 0) + " + func + "(a.\"SoldAnterior\", 0) - " + func + "(a.\"Efectuate\", 0) - " + func + "(a.\"Anulate\", 0) else " + func + "(a.\"Cuvenite\", 0) end end as \"ZileCOAnC\", "
                //                + " case when b.\"An\" is null then 0 else case when " + func + "(b.\"SoldAnterior\", 0) - " + func + "(b.\"Anulate\", 0) < " + func + "(b.\"Efectuate\", 0) then " + func + "(b.\"Cuvenite\", 0) + " + func + "(b.\"SoldAnterior\", 0) - " + func + "(b.\"Efectuate\", 0) - " + func + "(b.\"Anulate\", 0) else " + func + "(b.\"Cuvenite\", 0) end end as \"ZileCOAnAnt\","
                //                + " case when c.\"An\" is null then 0 else case when " + func + "(c.\"SoldAnterior\", 0) - " + func + "(c.\"Anulate\", 0) < " + func + "(c.\"Efectuate\", 0) + " + func + "(b.\"Efectuate\", 0) then " + func + "(c.\"Cuvenite\", 0) + " + func + "(c.\"SoldAnterior\", 0) - " + func + "(c.\"Efectuate\", 0) - " + func + "(b.\"Efectuate\", 0) - " + func + "(c.\"Anulate\", 0) else " + func + "(c.\"Cuvenite\", 0) end end as \"ZileCOAnAnt2\","
                //                + " case when c.\"An\" is null then 0 else case when " + func + "(c.\"SoldAnterior\", 0) - " + func + "(c.\"Anulate\", 0) > " + func + "(c.\"Efectuate\", 0) + " + func + "(b.\"Efectuate\", 0) + " + func + "(a.\"Efectuate\", 0)  then " + func + "(c.\"SoldAnterior\", 0) - " + func + "(c.\"Anulate\", 0) - " + func + "(c.\"Efectuate\", 0) - " + func + "(b.\"Efectuate\", 0) - " + func + "(a.\"Efectuate\", 0) else 0 end end as \"ZileCOMaiVechi\""
                //                + " from \"Ptj_tblZileCO\" a "
                //                + "left join \"Ptj_tblZileCO\" b on a.f10003 = b.f10003 and b.\"An\" = a.\"An\" - 1 "
                //                + "left join \"Ptj_tblZileCO\" c on a.f10003 = c.f10003  and c.\"An\" = a.\"An\" - 2 "
                //                + " LEFT JOIN F100 D on a.F10003 = d.F10003 "
                //                + "where a.\"An\" = " + an;

                string sql = " with \"Ptj_Detaliat\" as "
                             + "(select ptj.f10003, ptj.\"An\", coalesce(ptj_an.\"Ramase\", 0)  limita, "
                             + " coalesce(ptj.\"Cuvenite\", 0) as \"Cuvenite\", sum(coalesce(ptj.\"Cuvenite\", 0)) over(partition by ptj.f10003 order by ptj.\"An\" desc) suma "
                            + " from (select * from \"Ptj_tblZileCO\" where \"An\" <= " + an + ") ptj "
                            + " left join (select * from \"SituatieZileAbsente\" where \"An\" = " + an + ") Ptj_An on ptj.f10003 = ptj_An.f10003), "
                            + " Ptj_Raport as (select f10003, limita, \"An\", case when suma <= limita then \"Cuvenite\" when suma-\"Cuvenite\" >= limita then 0 else limita - suma + \"Cuvenite\"   end as Sold_An from \"Ptj_Detaliat\") "

                            + "select " + idAuto + " as \"IdAuto\", F100.F10003, CASE WHEN F100.F10025 = 900 THEN 'Candidat' ELSE CASE WHEN F100.F10025 = 999 THEN 'Angajat in avans' ELSE (CASE WHEN F100.F10025 = 0 THEN "
                                + "(CASE WHEN(F100.F100925 <> 0 AND F100922 IS NOT NULL AND F100923 IS NOT NULL AND F100923 IS NOT NULL AND F100922 <= {0} AND {0} <= F100923 AND {0} <= F100924) "
                                + "THEN 'Activ suspendat' ELSE CASE WHEN(F100.F100915 <= {0} AND {0} <= F100.F100916) THEN 'Activ detasat' ELSE 'Activ' END END) ELSE 'Inactiv' END) END END AS \"Stare\", "

                                + " coalesce(rap0.limita, 0) as \"ZileCO\", "
                               + "  coalesce(rap0.Sold_An, 0) \"ZileCOAnC\", coalesce(rap1.Sold_An, 0) \"ZileCOAnAnt\", coalesce(rap2.Sold_An, 0) \"ZileCOAnAnt2\", coalesce(rap3.Sold_An, 0) \"ZileCOMaiVechi\" "
                               + "      from f100  left join(select * from ptj_raport where \"An\" = " + an + ") rap0 on f100.f10003 = rap0.f10003 "
                               + " left join (select * from ptj_raport where \"An\"= " + an + " - 1) rap1 on f100.f10003 = rap1.f10003 "
                               + " left join (select * from ptj_raport where \"An\"= " + an + " - 2) rap2 on f100.f10003 = rap2.f10003 "
                               + " left join (select f10003, sum(coalesce(sold_an,0)) sold_an from ptj_raport where \"An\" <= " + an + " - 3  group by f10003) rap3 on f100.f10003 = rap3.f10003 "
                               + " WHERE coalesce(rap0.limita, 0) > 0";

                sql = string.Format(sql, data);
                DataTable dt = General.IncarcaDT(sql, null);
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;              
                grDate.DataBind();

                GridViewDataTextColumn colAnCurent = (grDate.Columns["ZileCOAnC"] as GridViewDataTextColumn);
                GridViewDataTextColumn colAncurentAnt = (grDate.Columns["ZileCOAnAnt"] as GridViewDataTextColumn);
                GridViewDataTextColumn colAnCurentAnt2 = (grDate.Columns["ZileCOAnAnt2"] as GridViewDataTextColumn);
                colAnCurent.Caption = "Zile CO (" + an + ")";
                colAncurentAnt.Caption = "Zile CO (" + (an - 1).ToString() + ")";
                colAnCurentAnt2.Caption = "Zile CO (" + (an - 2).ToString() + ")";

                Session["AnulareCO_Grid"] = dt;   

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

 

        public static string FiltruTipStari(string stari)
        {
            string val = "";

            try
            {
                string[] param = stari.Split(',');
                foreach (string elem in param)
                {
                    switch (elem.ToLower())
                    {
                        case "activ":
                            val += "OR [Stare] = 'Activ' ";
                            break;
                        case "activ detasat":
                            val += "OR [Stare] = 'Activ detasat' ";
                            break;
                        case "activ suspendat":
                            val += "OR [Stare] = 'Activ suspendat' ";
                            break;
                        case "inactiv":
                            val += "OR [Stare] = 'Inactiv' ";
                            break;
                        case "angajat in avans":
                            val += "OR [Stare] = 'Angajat in avans' ";
                            break;
                        case "candidat":
                            val += "OR [Stare] = 'Candidat' ";
                            break;      
                    }
                }
            }
            catch (Exception)
            {
            }

            return val.Substring(2);
        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            IncarcaGrid();
        }

        protected void btnAnulare_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "ZileCOAnAnt", "ZileCOAnAnt2", "ZileCOMaiVechi" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"), MessageBox.icoError);
                    return;
                }

                if (cmbPerioada.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat perioada!"), MessageBox.icoError);
                    return;
                }

                if (txtAnLuna.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat luna si anul!"), MessageBox.icoError);
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    int nrZile = 0;

                    switch(Convert.ToInt32(cmbPerioada.Value))
                    {
                        case 1:
                            nrZile = Convert.ToInt32(General.Nz(arr[1], -99)) + Convert.ToInt32(General.Nz(arr[2], -99)) + Convert.ToInt32(General.Nz(arr[3], -99));
                            break;
                        case 2:
                            nrZile = Convert.ToInt32(General.Nz(arr[2], -99)) + Convert.ToInt32(General.Nz(arr[3], -99));
                            break;
                        case 3:
                            nrZile = Convert.ToInt32(General.Nz(arr[3], -99));
                            break;

                    }

                    string sql = "UPDATE \"Ptj_tblZileCO\" SET \"Anulate\" = {0} WHERE F10003 = {1} AND \"An\" = {2}";
                    sql = string.Format(sql, nrZile, General.Nz(arr[0], -99), Convert.ToDateTime(txtAnLuna.Value).Year);
                    General.ExecutaNonQuery(sql, null);
                }

                grDate.Selection.UnselectAll();

                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }


}