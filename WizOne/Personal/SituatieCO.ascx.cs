using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.IO;
using System.Diagnostics;

namespace WizOne.Personal
{
    public partial class SituatieCO : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            btnCalc.Text = Dami.TraduCuvant("btnCalc", "Calcul CO");
            btnCalcSI.Text = Dami.TraduCuvant("btnCalcSI", "Calcul SI");

            grDateSituatieCO.DataBind();
            foreach (dynamic c in grDateSituatieCO.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateSituatieCO.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateSituatieCO.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateSituatieCO.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateSituatieCO.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateSituatieCO.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
            btnCalc.Text = Dami.TraduCuvant(btnCalc.Text);
            btnCalcSI.Text = Dami.TraduCuvant(btnCalcSI.Text);

            
        }

        protected void Page_Load(object sender, EventArgs e)
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

        //protected void grDateSituatieCO_DataBinding(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!IsPostBack)
        //            IncarcaGrid();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private void IncarcaGrid()
        {
            grDateSituatieCO.DataSource = null;

            string sqlFinal = "SELECT * FROM \"SituatieZileAbsente\" WHERE F10003 = " + Session["Marca"].ToString() + " ORDER BY \"An\" ";
            DataTable dt = new DataTable();
            dt = General.IncarcaDT(sqlFinal, null);
                       
            grDateSituatieCO.KeyFieldName = "F10003;An";
            grDateSituatieCO.DataSource = dt;
            grDateSituatieCO.DataBind();

            grDateSituatieCO.Columns.Clear();

            foreach (DataColumn col in dt.Columns)
            {  
                GridViewDataColumn c = new GridViewDataColumn();
                c.Name = col.ColumnName;
                c.FieldName = col.ColumnName;
                c.Caption = Dami.TraduCuvant(col.ColumnName);
                c.ReadOnly = true;
                c.Width = 75;
                if (col.ColumnName.Length >= 12)
                    c.Width = 120;
                c.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
                if (col.ColumnName == "IdAbsenta" || col.ColumnName == "COPlanificat" || col.ColumnName == "F10003")
                    c.Visible = false;  
                grDateSituatieCO.Columns.Add(c);                
            }

        }

        protected void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                //Florin 2019.07.01
                //s-a inlocuit functia comentata cu cea din General
                General.CalculCO(DateTime.Now.Year, Convert.ToInt32(General.Nz(Session["Marca"],-98)), true);
                IncarcaGrid();

                //int an = DateTime.Now.Year;

                //string dtInc = an.ToString() + "-01-01";
                //string dtSf = an.ToString() + "-12-31";

                //string strSql = "";
                //string filtruIns = "";
                //string f10003 = "-99";


                //f10003 = Session["Marca"].ToString();
                //filtruIns = " AND F10003=" + f10003;


                //if (Constante.tipBD == 1)
                //{

                //    //daca nu exista inseram linie goala si apoi updatam
                //    strSql += "insert into Ptj_tblZileCO(F10003, An, USER_NO, TIME) " +
                //    " select F10003, " + an + ", " + Session["UserId"] + ", GetDate() from F100 where F10003 not in (select F10003 from Ptj_tblZileCO where An=" + an + ") " +
                //    " and F10022 <= '" + dtSf + "' and '" + dtInc + "' <= F10023" + filtruIns + ";";


                //    strSql += "with xx as " +
                //    " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
                //    " (select a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
                //    " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107='2100-01-01' then b.f11106 else b.f11107 end  " +
                //    " and b.f11105 <= case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end) " +
                //    " group by a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end, a.time) t " +
                //    " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
                //    " case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
                //    " union all " +
                //    " select f10003 Marca, datainceput de_la_data, datasfarsit la_data " +
                //    " from ptj_cereri inner join Ptj_tblAbsente on Ptj_Cereri.IdAbsenta = Ptj_tblAbsente.id " +
                //    " where ptj_cereri.IdStare=3 and isnull(Ptj_tblAbsente.AbsenteCFPInCalculCO,0) = 1  ), " +
                //    "  yy as " +
                //    " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
                //    " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                //    " group by a.marca, a.de_la_data, a.la_data), " +
                //    " f111_2 as  " +
                //    " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
                //    " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                //    " group by a.marca, a.de_la_data, a.la_data) " +
                //    " update x set x.Cuvenite = (select y.ZileCuvenite from " +
                //    " (select a.F10003, " +
                //    " ROUND((case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end " +                                                   //nr zile cuvenite conform grilei
                //    " + (CASE WHEN ISNULL(a.F10027,0)>=2 THEN Convert(int,isnull((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END)) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
                //    " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
                //    " (datediff(day,(CASE WHEN cast(f10022 as date) < '" + dtInc + "' THEN '" + dtInc + "' ELSE cast(f10022 as date) END) " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
                //    " ,(CASE WHEN cast(f10023 as date) < '" + dtSf + "' THEN cast(f10023 as date) ELSE '" + dtSf + "' END))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
                //    " - (SELECT COALESCE(SUM(datediff(d,CASE WHEN F11105 < CONVERT(date,'" + an + "-01-01') THEN CONVERT(date,'" + an + "-01-01') else F11105 END,CASE WHEN F11107 > CONVERT(date,'" + an + "-12-31') THEN CONVERT(date,'" + an + "-12-31') else F11107 END)) + 1,0) from f111_2 A where f11103=" + f10003 + " and F11105 <= F11107 AND (year(F11105)=" + an + " or year(F11107)=" + an + "))" +
                //    " ) " +
                //    " /CONVERT(float,365),0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
                //    " from F100 a " +
                //    " left join (select ISNULL(convert(int,substring(F100644,1,2)),0) * 12 + ISNULL(convert(int,substring(F100644,3,2)),0) + DATEDIFF (MONTH,  " +
                //    " (select convert(nvarchar(4),F01011) + '-' + convert(nvarchar(4),F01012) + '-01' from F010),'" + dtSf + "' " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                //    " ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma
                //    " left join F026 c on convert(int,a.F10072) = c.F02604 and (convert(int,c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int,c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                //    " where F10022 <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
                //    " from Ptj_tblZileCO x " +
                //    " where x.An=" + an + filtruIns + ";";


                //    //la fel ca mai sus - fara ponderea cu nr de zile lucrate in an
                //    strSql += "update x set x.CuveniteAn = (select y.ZileCuvenite from " +
                //    " (select a.F10003, " +
                //    " (case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end " +                                                   //nr zile cuvenite conform grilei
                //    " + (CASE WHEN ISNULL(a.F10027,0)>=2 THEN Convert(int,isnull((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
                //    " ) as ZileCuvenite " +
                //    " from F100 a " +
                //    " left join (select ISNULL(convert(int,substring(F100644,1,2)),0) * 12 + ISNULL(convert(int,substring(F100644,3,2)),0) + DATEDIFF (MONTH,  " +
                //    " (select convert(nvarchar(4),F01011) + '-' + convert(nvarchar(4),F01012) + '-01' from F010),'" + dtSf + "' " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                //    " ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma
                //    " left join F026 c on convert(int,a.F10072) = c.F02604 and (convert(int,c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int,c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                //    " where F10022 <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
                //    " from Ptj_tblZileCO x " +
                //    " where x.An=" + an + filtruIns + ";";
                //}
                //else
                //{
                //    dtInc = "01-JAN-" + an.ToString();
                //    dtSf = "31-DEC-" + an.ToString();

                //    //daca nu exista inseram linie goala si apoi updatam
                //    strSql += "insert into \"Ptj_tblZileCO\"(F10003, \"An\", USER_NO, TIME) " +
                //    " select F10003, " + an + ", " + Session["UserId"] + ", SYSDATE from F100 where F10003 not in (select F10003 from \"Ptj_tblZileCO\" where \"An\"=" + an + ") " +
                //    " and F10022 <= to_date('" + dtSf + "','DD-MM-YYYY') and to_date('" + dtInc + "','DD-MM-YYYY') <= F10023" + filtruIns + ";";

                //    strSql += "update \"Ptj_tblZileCO\" x set x.\"Cuvenite\" = ( " +
                //            " with xx as " +
                //            " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107=to_date('01-JAN-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
                //            " (select a.f11103, a.f11105, case when a.f11107=to_date('01-JAN-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
                //            " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107=to_date('01-JAN-2100','DD-MM-YYYY') then b.f11106 else b.f11107 end  " +
                //            " and b.f11105 <= case when a.f11107=to_date('01-JAN-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end) " +
                //            " group by a.f11103, a.f11105, case when a.f11107=to_date('01-JAN-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end, a.time) t " +
                //            " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
                //            " case when f111.f11107=to_date('01-JAN-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
                //            " union all " +
                //            " select f10003 Marca, \"DataInceput\" de_la_data, \"DataSfarsit\" la_data " +
                //            " from \"Ptj_Cereri\" inner join \"Ptj_tblAbsente\" on \"Ptj_Cereri\".\"IdAbsenta\" = \"Ptj_tblAbsente\".\"Id\" " +
                //            " where \"Ptj_Cereri\".\"IdStare\"=3 and COALESCE(\"Ptj_tblAbsente\".\"AbsenteCFPInCalculCO\",0) = 1  ), " +
                //            "  yy as " +
                //            " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
                //            " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                //            " group by a.marca, a.de_la_data, a.la_data), " +
                //            " f111_2 as  " +
                //            " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
                //            " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
                //            " group by a.marca, a.de_la_data, a.la_data) " +
                //            " select y.ZileCuvenite from " +
                //    " (select a.F10003, " +
                //    " ROUND((case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end " +                                                   //nr zile cuvenite conform grilei
                //    " + (CASE WHEN NVL(a.F10027,0)>=2 THEN to_number(nvl((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END)) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
                //    " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
                //    " (least(trunc(f10023),to_date('31-DEC-" + an + "','DD-MM-YYYY') " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
                //    " ) - greatest(trunc(f10022),to_date('01-JAN-" + an + "','DD-MM-YYYY'))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
                //    " - nvl(b.cfp,0) " +                                                   //scadem zilele de concediu fara plata luate in anul de referinta
                //    " - (select COALESCE(SUM(least(trunc(F11107),to_date('31-DEC-" + an + "','DD-MM-YYYY')) - greatest(trunc(f11105),to_date('01-JAN-" + an + "','DD-MM-YYYY')) + 1),0) from f111 A where f11103=" + f10003 + " and F11105 <= F11107 AND (to_Char(F11105,'yyyy')='" + an + "' or to_Char(F11107,'yyyy')='" + an + "')) " +
                //    " ) " +
                //    " /365,0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
                //    " from F100 a " +
                //    " left join (select nvl(to_number(substr(F100644,1,2)),0) * 12 + nvl(to_number(substr(F100644,3,2)),0) + trunc(MONTHS_BETWEEN (to_date('31-DEC-" + an + "','DD-MM-YYYY'), " +
                //    " (select to_date('01-' || case F01012 when 1 then 'JAN' when 2 then 'FEB' when 3 then 'MAR' when 4 then 'APR' when 5 then 'MAY' when 6 then 'JUN' when 7 then 'JUL' when 8 then 'AUG' when 9 then 'SEP' when 10 then 'OCT' when 11 then 'NOV' when 12 then 'DEC' end || '-' ||  F01011,'DD-MM-YYYY') from F010) " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                //    " ) + 1 ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma + luna de lucru deschisa pt ca functia MONTHS_BETWEEN nu tine cont de ea
                //    " left join F026 c on a.F10072 = c.F02604 and (to_number(c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                //    " left join ((select F10003, nvl(sum(least(trunc(\"DataSfarsit\"),to_date('31-DEC-" + an + "','DD-MM-YYYY')-1) - greatest(trunc(\"DataInceput\"),to_date('01-JAN-" + an + "','DD-MM-YYYY'))+1),0) as cfp from \"Ptj_Cereri\" where \"IdAbsenta\" in (SELECT \"Id\" from \"Ptj_tblAbsente\" where \"AbsenteCFPInCalculCO\"=1) and \"IdStare\"=3 AND (to_Char(\"DataInceput\",'YYYY') ='" + an + "' OR to_Char(\"DataSfarsit\",'YYYY') ='" + an + "') group by f10003)) b on a.F10003 = b.F10003 " +  //se calcuelaza nr de cfp avute in anul de referinta
                //    " where F10022 <= to_date('31-DEC-" + an + "','DD-MM-YYYY') and to_date('01-JAN-" + an + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
                //    " where x.\"An\"=" + an + filtruIns + ";";

                //    strSql += "update \"Ptj_tblZileCO\" x set x.\"CuveniteAn\" = (select y.ZileCuvenite from " +
                //    " (select a.F10003, " +
                //    " (case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end " +                                                   //nr zile cuvenite conform grilei
                //    " + (CASE WHEN NVL(a.F10027,0)>=2 THEN to_number(nvl((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
                //    " ) as ZileCuvenite " +
                //    " from F100 a " +
                //    " left join (select nvl(to_number(substr(F100644,1,2)),0) * 12 + nvl(to_number(substr(F100644,3,2)),0) + trunc(MONTHS_BETWEEN (to_date('31-DEC-" + an + "','DD-MM-YYYY'), " +
                //    " (select to_date('01-' || case F01012 when 1 then 'JAN' when 2 then 'FEB' when 3 then 'MAR' when 4 then 'APR' when 5 then 'MAY' when 6 then 'JUN' when 7 then 'JUL' when 8 then 'AUG' when 9 then 'SEP' when 10 then 'OCT' when 11 then 'NOV' when 12 then 'DEC' end || '-' ||  F01011,'DD-MM-YYYY') from F010) " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
                //    " ) + 1 ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma + luna de lucru deschisa pt ca functia MONTHS_BETWEEN nu tine cont de ea
                //    " left join F026 c on a.F10072 = c.F02604 and (to_number(c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
                //    " where F10022 <= to_date('31-DEC-" + an + "','DD-MM-YYYY') and to_date('01-JAN-" + an + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
                //    " where x.\"An\"=" + an + filtruIns + ";";

                //}

                //strSql = "BEGIN " + strSql + " END;";

                //General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnCalcSI_Click(object sender, EventArgs e)
        {
            try
            {
                string ang = "";
                string dt = "GetDate()";
                if (Constante.tipBD == 2) dt = "SYSDATE";
                string strSql = @"INSERT INTO ""Ptj_tblZileCO""(F10003, ""An"", USER_NO, TIME)
                            SELECT F10003, {0}, {1}, {2} from F100 
                            WHERE F10003 NOT IN (SELECT F10003 FROM ""Ptj_tblZileCO"" WHERE ""An""={0})
                            AND F10022 <= {3} AND {3} <= F10023
                            ORDER BY F10003;";

                ang = " AND A.F10003=" + Session["Marca"].ToString();
                if (Constante.tipBD == 1)
                    strSql += @"UPDATE A SET A.""SoldAnterior""=B.""Ramase"" FROM ""Ptj_tblZileCO"" A INNER JOIN ""SituatieZileAbsente"" B ON  A.F10003=B.F10003 AND B.""An""=({0}-1) AND B.""IdAbsenta""=1 WHERE A.""An""={0} {4};";
                else
                    strSql += @"UPDATE ""Ptj_tblZileCO"" A SET A.""SoldAnterior""=(SELECT B.""Ramase"" FROM ""SituatieZileAbsente"" B where A.F10003=B.F10003 AND B.""An""=({0}-1) AND B.""IdAbsenta""=1) WHERE A.""An""={0} {4};";


                strSql = string.Format(strSql, DateTime.Now.Year, Session["UserId"], dt, General.ToDataUniv(DateTime.Now), ang);
                strSql = "BEGIN " + strSql + " END;";

                General.ExecutaNonQuery(strSql, null);

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }

            //<Columns>            
            //    <dx:GridViewDataTextColumn FieldName = "F10003" Name="F10003" Caption="Marca"  Width="75px" Visible="false"/>
            //    <dx:GridViewDataTextColumn FieldName = "An" Name="An" Caption="Anul"  Width="75px" />
            //    <dx:GridViewDataTextColumn FieldName = "RamaseAnterior" Name="RamaseAnterior" Caption="Ramase anterior"  Width="75px"  HeaderStyle-Wrap="True" />
            //    <dx:GridViewDataTextColumn FieldName = "Cuvenite" Name="Cuvenite" Caption="Cuvenite"  Width="75px"  HeaderStyle-Wrap="True"/>
            //    <dx:GridViewDataTextColumn FieldName = "Total" Name="Total" Caption="Total cuvenite"  Width="75px"  HeaderStyle-Wrap="True"/>
            //    <dx:GridViewDataTextColumn FieldName = "Aprobate" Name="Aprobate" Caption="Aprobate"  Width="75px"  HeaderStyle-Wrap="True"/>
            //    <dx:GridViewDataTextColumn FieldName = "Ramase" Name="Ramase" Caption="Ramase curent"  Width="75px"  HeaderStyle-Wrap="True"/>
            //    <dx:GridViewDataTextColumn FieldName = "Solicitate" Name="Solicitate" Caption="Solicitate"  Width="75px"  HeaderStyle-Wrap="True"/>
            //    <dx:GridViewDataTextColumn FieldName = "Planificate" Name="Planificate" Caption="Planificate"  Width="75px"  HeaderStyle-Wrap="True"/>
            //    <dx:GridViewDataTextColumn FieldName = "RamaseDePlanificat" Name="RamaseDePlanificat" Caption="Ramase de planificat"  Width="75px"  HeaderStyle-Wrap="True"/>
            //</Columns>


}