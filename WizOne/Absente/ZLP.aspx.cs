using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Absente
{
    public partial class ZLP : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                GridViewDataComboBoxColumn colAn = (grDate.Columns["An"] as GridViewDataComboBoxColumn);
                colAn.PropertiesComboBox.DataSource = Dami.ListaAni(2000, 2030);
                cmbAn.DataSource = Dami.ListaAni(2000, 2030);
                cmbAn.DataBind();
                

                DataTable dtAng = General.IncarcaDT(Dami.SelectAngajati(), null);
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

                cmbTip.DataSource = Dami.ListaActivi();
                cmbTip.DataBind();
                

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;
                    cmbAn.Value = DateTime.Now.Year;
                    cmbTip.Value = 1;
                }
                else
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }

                foreach (var c in grDate.Columns)
                {
                    try
                    {
                        GridViewDataColumn col = (GridViewDataColumn)c;
                        col.Caption = Dami.TraduCuvant(col.FieldName, col.Caption);
                    }
                    catch (Exception) { }
                }

                grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaZL", "10"));
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

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
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
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.NewRow();

                dr["F10003"] = e.NewValues["F10003"] ?? DBNull.Value;
                dr["An"] = e.NewValues["An"] ?? DBNull.Value;
                dr["Cuvenite"] = (e.NewValues["Cuvenite"] ?? DBNull.Value);
                dr["SoldAnterior"] = (e.NewValues["SoldAnterior"] ?? DBNull.Value);
                dr["Efectuate"] = (e.NewValues["Efectuate"] ?? DBNull.Value);
                dr["CuveniteAn"] = (e.NewValues["CuveniteAn"] ?? DBNull.Value);
                dr["USER_NO"] = Session["UserId"] ?? DBNull.Value;
                dr["TIME"] = DateTime.Now;

                int idAuto = 1;
                if (Constante.tipBD == 1)
                    idAuto = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    idAuto = Dami.NextId("Ptj_tblZLP");

                dr["IdAuto"] = idAuto;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                dr["Cuvenite"] = (e.NewValues["Cuvenite"] ?? DBNull.Value);
                dr["SoldAnterior"] = (e.NewValues["SoldAnterior"] ?? DBNull.Value);
                dr["Efectuate"] = (e.NewValues["Efectuate"] ?? DBNull.Value);
                dr["CuveniteAn"] = (e.NewValues["CuveniteAn"] ?? DBNull.Value);
                dr["USER_NO"] = Session["UserId"] ?? DBNull.Value;
                dr["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
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
                e.NewValues["An"] = cmbAn.Value;
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
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                //if (Constante.tipBD == 1)
                //{
                //    SqlDataAdapter da = new SqlDataAdapter();
                //    da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""Ptj_tblZileCO"" ", null);
                //    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //    da.Update(dt);

                //    da.Dispose();
                //    da = null;
                //}
                //else
                //{
                //    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Ptj_tblZileCO\" WHERE ROWNUM = 0", null);
                //    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                //    oledbAdapter.Update(dt);
                //    oledbAdapter.Dispose();
                //    oledbAdapter = null;
                //}
                General.SalveazaDate(dt, "Ptj_tblZLP");

                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRenunta_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                dt.RejectChanges();

                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //public void CalculZLP(int an = -99, string f10003 = "a.f10003", string filtruIns = "", DateTime? f10022 = null, bool esteNou = false)
        //{
        //    try
        //    {
        //        if (an == -99)
        //            an = Convert.ToInt32(General.Nz(cmbAn.Value, DateTime.Now.Year));
                    
        //        string dtInc = an.ToString() + "-01-01";
        //        string dtSf = an.ToString() + "-12-31";

        //        string strSql = "";
                           

        //        if (cmbAng != null && cmbAng.Value != null)
        //        {
        //            f10003 = cmbAng.Value.ToString();
        //            filtruIns = " AND F10003=" + cmbAng.Value;
        //        }

        //        if (Constante.tipBD == 1)
        //        {

        //            //daca nu exista inseram linie goala si apoi updatam
        //            if (!esteNou)
        //                strSql += "insert into Ptj_tblZLP(F10003, An, USER_NO, TIME) " +
        //                " select F10003, " + an + ", " + Session["UserId"] + ", GetDate() from F100 where F10003 not in (select F10003 from Ptj_tblZLP where An=" + an + ") " +
        //                " and F10022 <= '" + dtSf + "' and '" + dtInc + "' <= F10023" + filtruIns + ";";
        //            //else
        //            //    strSql += "insert into Ptj_tblZLP(F10003, An, USER_NO, TIME) VALUES (" + f10003 + ", " + an + ", " + Session["UserId"] + ", GetDate());";
                    


        //            strSql += "with xx as " +
        //            " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
        //            " (select a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
        //            " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107='2100-01-01' then b.f11106 else b.f11107 end  " +
        //            " and b.f11105 <= case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end) " +
        //            " group by a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end, a.time) t " +
        //            " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
        //            " case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
        //            " union all " +
        //            " select f10003 Marca, datainceput de_la_data, datasfarsit la_data " +
        //            " from ptj_cereri inner join Ptj_tblAbsente on Ptj_Cereri.IdAbsenta = Ptj_tblAbsente.id " +
        //            " where ptj_cereri.IdStare=3 and isnull(Ptj_tblAbsente.AbsenteCFPInCalculCO,0) = 1  ), " +
        //            "  yy as " +
        //            " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
        //            " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //            " group by a.marca, a.de_la_data, a.la_data), " +
        //            " f111_2 as  " +
        //            " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
        //            " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //            " group by a.marca, a.de_la_data, a.la_data) " +
        //            " update x set x.Cuvenite = (select y.ZileCuvenite from " +
        //            " (select a.F10003, " +

        //            " ROUND(( SELECT ISNULL((SELECT CONVERT(int, Valoare) FROM tblParametrii WHERE Nume = 'NumarZileLiberePlatite'), 0))  " +              
        //            " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
        //            " (datediff(day,(CASE WHEN cast(f10022 as date) < '" + dtInc + "' THEN '" + dtInc + "' ELSE cast(f10022 as date) END) " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
        //            " ,(CASE WHEN cast(f10023 as date) < '" + dtSf + "' THEN cast(f10023 as date) ELSE '" + dtSf + "' END))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
        //            " - (SELECT COALESCE(SUM(datediff(d,CASE WHEN F11105 < CONVERT(date,'" + an + "-01-01') THEN CONVERT(date,'" + an + "-01-01') else F11105 END,CASE WHEN F11107 > CONVERT(date,'" + an + "-12-31') THEN CONVERT(date,'" + an + "-12-31') else F11107 END)) + 1,0) from f111_2 Z where f11103=" + f10003 + " and F11105 <= F11107 and F11107 >= CONVERT(date,'" + an + "-01-01') AND F11105 <= CONVERT(date,'" + an + "-12-31'))" +     // AND (year(F11105)=" + an + " or year(F11107)=" + an + ")
        //            " ) " +

        //            " /CONVERT(float,365),0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
        //            " from F100 a " +

        //            " where F10022 <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calculeaza totul pt angajatii activi in anul de referinta
        //            " from Ptj_tblZLP x " +
        //            " where x.An=" + an + filtruIns + ";";


        //            //la fel ca mai sus - fara ponderea cu nr de zile lucrate in an
        //            strSql += "update  x set x.CuveniteAn = (select y.ZileCuvenite from " +
        //            " (select a.F10003, " +

        //            " ( SELECT ISNULL((SELECT CONVERT(int, Valoare) FROM tblParametrii WHERE Nume = 'NumarZileLiberePlatite'), 0) " +              
                    
        //            " ) as ZileCuvenite " +
        //            " from F100 a " +                    
                   
        //            " where F10022 <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calculeaza totul pt angajatii activi in anul de referinta
        //            " from Ptj_tblZLP x " +
        //            " where x.An=" + an + filtruIns + ";";
        //        }
        //        else
        //        {
        //            dtInc = "01-01-" + an.ToString();
        //            dtSf = "31-12-" + an.ToString();

        //            //daca nu exista inseram linie goala si apoi updatam
        //            if (!esteNou)
        //                strSql += "insert into \"Ptj_tblZLP\"(F10003, \"An\", USER_NO, TIME) " +
        //                " select F10003, " + an + ", " + Session["UserId"] + ", SYSDATE from F100 where F10003 not in (select F10003 from \"Ptj_tblZLP\" where \"An\"=" + an + ") " +
        //                " and F10022 <= to_date('" + dtSf + "','DD-MM-YYYY') and to_date('" + dtInc + "','DD-MM-YYYY') <= F10023" + filtruIns + ";";
        //            //else
        //            //    strSql += "insert into \"Ptj_tblZLP\"(F10003, \"An\", USER_NO, TIME) VALUES (" + f10003 + ", " + an + ", " + Session["UserId"] + ", sysdate);";

        //            strSql += "update \"Ptj_tblZLP\" x set x.\"Cuvenite\" = ( " +
        //                    " with xx as " +
        //                    " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107=to_date('01-01-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
        //                    " (select a.f11103, a.f11105, case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
        //                    " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107=to_date('01-01-2100','DD-MM-YYYY') then b.f11106 else b.f11107 end  " +
        //                    " and b.f11105 <= case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end) " +
        //                    " group by a.f11103, a.f11105, case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end, a.time) t " +
        //                    " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
        //                    " case when f111.f11107=to_date('01-01-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
        //                    " union all " +

        //                    " select f10003 Marca, \"DataInceput\" de_la_data, \"DataSfarsit\" la_data " +
        //                    " from \"Ptj_Cereri\" inner join \"Ptj_tblAbsente\" on \"Ptj_Cereri\".\"IdAbsenta\" = \"Ptj_tblAbsente\".\"Id\" " +
        //                    " where \"Ptj_Cereri\".\"IdStare\"=3  ), " +

        //                    "  yy as " +
        //                    " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
        //                    " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //                    " group by a.marca, a.de_la_data, a.la_data), " +
        //                    " f111_2 as  " +
        //                    " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
        //                    " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //                    " group by a.marca, a.de_la_data, a.la_data) " +
        //                    " select y.ZileCuvenite from " +
        //            " (select a.F10003, " +

        //            " ROUND((SELECT nvl((SELECT TO_NUMBER(\"Valoare\") FROM \"tblParametrii\" WHERE \"Nume\" = 'NumarZileLiberePlatite'), 0) from dual) " +               
                    
        //            " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
        //            " (least(trunc(f10023),to_date('" + dtSf + "','DD-MM-YYYY') " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
        //            " ) - greatest(trunc(f10022),to_date('" + dtInc + "','DD-MM-YYYY'))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta                   
        //            " - (select COALESCE(SUM(least(trunc(F11107),to_date('" + dtSf + "','DD-MM-YYYY')) - greatest(trunc(f11105),to_date('" + dtInc + "','DD-MM-YYYY')) + 1),0) from f111_2 Z where f11103=" + f10003 + " and F11105 <= F11107 and F11107 >= to_date('" + dtInc + "','DD-MM-YYYY') AND F11105 <= to_date('" + dtSf + "','DD-MM-YYYY')) " +       //AND (to_Char(F11105,'yyyy')='" + an + "' or to_Char(F11107,'yyyy')='" + an + "')

        //            " ) " +
        //            " /365,0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
        //            " from F100 a " +                    
                  
        //            " where F10022 <= to_date('" + dtSf + "','DD-MM-YYYY') and to_date('" + dtInc + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
        //            " where x.\"An\"=" + an + filtruIns + ";";

        //            strSql += "update \"Ptj_tblZLP\" x set x.\"CuveniteAn\" = (select y.ZileCuvenite from " +
        //            " (select a.F10003, " +

        //            " (SELECT nvl((SELECT TO_NUMBER(\"Valoare\") FROM \"tblParametrii\" WHERE \"Nume\" = 'NumarZileLiberePlatite'), 0) from dual) " +               
                    
        //            "as ZileCuvenite " +
        //            " from F100 a " +
                  
        //            " where F10022 <= to_date('" + dtSf + "','DD-MM-YYYY') and to_date('" + dtInc + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
        //            " where x.\"An\"=" + an + filtruIns + ";";

        //        }

        //        strSql = "BEGIN " + strSql + " END;";

        //        //Radu 21.04.2020
        //        General.ExecutaNonQuery(strSql, null);
        //        //if (Constante.tipBD == 1)
        //        //    General.ExecutaNonQuery("EXEC CalculZLPProc @f10003 = " + f10003 + ", @zi = '" + an + "-12-31', @mod = 1", null);
        //        //else
        //        //    General.ExecutaNonQuery("exec \"CalculZLPProc\" (" + f10003 + ", TO_DATE('31/12/" + an + "', 'dd/mm/yyyy'), 1);", null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        private void CalculSIZLP()
        {
            try
            {
                string ang = "";
                string dt = "GetDate()";
                if (Constante.tipBD == 2) dt = "SYSDATE";
                string strSql = @"INSERT INTO ""Ptj_tblZLP""(F10003, ""An"", USER_NO, TIME)
                            SELECT F10003, {0}, {1}, {2} from F100 
                            WHERE F10003 NOT IN (SELECT F10003 FROM ""Ptj_tblZLP"" WHERE ""An""={0})
                            AND F10022 <= {3} AND {3} <= F10023
                            ORDER BY F10003;";

                if (cmbAng.Value != null) ang = " AND A.F10003=" + cmbAng.Value;
                if (Constante.tipBD == 1)
                    strSql += @"UPDATE A SET A.""SoldAnterior""=B.""Ramase"" FROM ""Ptj_tblZLP"" A INNER JOIN ""SituatieZLP"" B ON  A.F10003=B.F10003 AND B.""An""=({0}-1) AND B.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""DenumireScurta""='ZLP') WHERE A.""An""={0} {4};";
                else
                    strSql += @"UPDATE ""Ptj_tblZLP"" A SET A.""SoldAnterior""=(SELECT B.""Ramase"" FROM ""SituatieZLP"" B where A.F10003=B.F10003 AND B.""An""=({0}-1) AND B.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""DenumireScurta""='ZLP')) WHERE A.""An""={0} {4};";


                strSql = string.Format(strSql, cmbAn.Value, Session["UserId"], dt, General.ToDataUniv(DateTime.Now) , ang);
                strSql = "BEGIN " + strSql + " END;";

                General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                if (e.Parameters != "")
                {
                    if (e.Parameters == "btnZLP")
                    {
                        if (cmbAng.Value != null)
                        {
                            for (int i = 0; i < grDate.VisibleRowCount; i++)
                            {
                                DataRowView obj = grDate.GetRow(i) as DataRowView;
                                string marca = obj["F10003"].ToString();
                                General.CalculCO(Convert.ToInt32(General.Nz(cmbAn.Value, DateTime.Now.Year)), Convert.ToInt32(marca), true, true);
                            }
                        }
                        else
                        {
                            string marci = "";
                            switch (Convert.ToInt32(General.Nz(cmbTip.Value, 1)))
                            {
                                case 1:                         //toti
                                    marci = "P1";
                                    break;
                                case 2:                         //activi
                                    marci = "P2";
                                    break;
                                case 3:                         //plecati
                                    marci = "P3";
                                    break;
                                case 4:                         //in avans
                                    marci = "P4";
                                    break;
                                default:
                                    marci = "P1";
                                    break;
                            }

                            string dtInc = Convert.ToInt32(General.Nz(cmbAn.Value, DateTime.Now.Year)).ToString() + "-01-01";
                            string dtSf = Convert.ToInt32(General.Nz(cmbAn.Value, DateTime.Now.Year)).ToString() + "-12-31";

                            //Florin 2021.04.02 - am inlocuit peste tot unde aparea ultima zi din an cu ziua curenta
                            //Radu 21.04.2021 - data se citeste cf. param.
                            int param = Convert.ToInt32(Dami.ValoareParam("ModCalculZileCOCuveniteDataReferinta", "1"));
                            string dtCalcul = "'" + dtSf + "'";
                            switch (param)
                            {
                                case 1:
                                    dtCalcul = "'" + dtSf + "'";
                                    break;
                                case 2:
                                    dtCalcul = "'" + dtInc + "'";
                                    break;
                                case 3:
                                    dtCalcul = General.CurrentDate();
                                    break;
                            }

                            General.ExecutaNonQuery(" EXEC CalculZLPProc '" + marci + "', " + dtCalcul + ", 1, 0 ", null);
                        }
                    }
                    if (e.Parameters == "btnSI") CalculSIZLP();
                    IncarcaGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string strSql = "";

                strSql = $@"SELECT A.F10003, A.F10003 AS ""Nume"", ""An"", ""Cuvenite"", ""SoldAnterior"", ""Efectuate"", ""CuveniteAn"", A.USER_NO, A.TIME, A.""IdAuto"" FROM ""Ptj_tblZLP"" A
                        INNER JOIN F100 B ON A.F10003=B.F10003
                        WHERE A.""An""={Convert.ToInt32(General.Nz(cmbAn.Value, DateTime.Now.Year))}";

                if (cmbAng.Value != null) strSql += " AND A.F10003=" + cmbAng.Value;


                switch (Convert.ToInt32(General.Nz(cmbTip.Value,1)))
                {
                    case 1:                         //toti
                        //NOP
                        break;
                    case 2:                         //activi
                        if (Constante.tipBD == 1) strSql += " AND CONVERT(date,B.F10022)<=" + General.ToDataUniv(DateTime.Now) + " AND " + General.ToDataUniv(DateTime.Now) + "<=CONVERT(date,B.F10023)";
                        if (Constante.tipBD == 2) strSql += " AND TRUNC(B.F10022)<=" + General.ToDataUniv(DateTime.Now) + " AND " + General.ToDataUniv(DateTime.Now) + "<=TRUNC(B.F10023)";
                        break;
                    case 3:                         //plecati
                        if (Constante.tipBD == 1) strSql += " AND " + General.ToDataUniv(DateTime.Now) + " > CONVERT(date,B.F10023)";
                        if (Constante.tipBD == 2) strSql += " AND " + General.ToDataUniv(DateTime.Now) + " > TRUNC(B.F10023)";
                        break;
                    case 4:                         //in avans
                        if (Constante.tipBD == 1) strSql += " AND CONVERT(date,B.F10022) > " + General.ToDataUniv(DateTime.Now);
                        if (Constante.tipBD == 2) strSql += " AND TRUNC(B.F10022) > " + General.ToDataUniv(DateTime.Now);
                        break;
                }

                DataTable dt = General.IncarcaDT(strSql, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                Session["InformatiaCurenta"] = dt;

                grDate.DataSource = Session["InformatiaCurenta"];
                grDate.KeyFieldName = "IdAuto";
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




    }
}