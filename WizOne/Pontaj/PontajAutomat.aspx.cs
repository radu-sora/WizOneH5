using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajAutomat : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnAct.Text = Dami.TraduCuvant("btnAct", "Actualizeaza absenta");
                btnSte.Text = Dami.TraduCuvant("btnSte", "Sterge absenta");

                //Radu 09.12.2019
                lblCtrInc.InnerText = Dami.TraduCuvant("Contract Inceput");
                lblCtrSf.InnerText = Dami.TraduCuvant("Contract Sfarsit");
                lblZiua.InnerText = Dami.TraduCuvant("Ziua");
                lblAbs.InnerText = Dami.TraduCuvant("Tip Cerere");
                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                DataTable dtAbs = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ", null);
                cmbAbs.DataSource = dtAbs;
                cmbAbs.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAct_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtCtrInc.Value == null || txtCtrSf.Value == null || txtZiua.Value == null || cmbAbs.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoError, "");
                    return;
                }

                if (Convert.ToInt32(txtCtrInc.Value) > Convert.ToInt32(txtCtrSf.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Contract inceput este mai mare decat contract sfarsit !"), MessageBox.icoError, "");
                    return;
                }

                DoAction(1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSte_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtCtrInc.Value == null || txtCtrSf.Value == null || txtZiua.Value == null || cmbAbs.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoError, "");
                    return;
                }

                if (Convert.ToInt32(txtCtrInc.Value) > Convert.ToInt32(txtCtrSf.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Contract inceput este mai mare decat contract sfarsit !"), MessageBox.icoError, "");
                    return;
                }

                DoAction(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void DoAction(int tip)
        {
            try
            {
                //1  -  actualizeaza cu tipul de absenta
                //2  -  sterge absenta

                if (PonteazaAbsenta(Convert.ToInt32(Session["UserId"]), tip, Convert.ToInt32(txtCtrInc.Value), Convert.ToInt32(txtCtrSf.Value), Convert.ToDateTime(txtZiua.Value), Convert.ToInt32(cmbAbs.Value), "#FFFFFF"))
                    MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes !"), MessageBox.icoSuccess, "");
                else
                    MessageBox.Show(Dami.TraduCuvant("Eroare in proces!"), MessageBox.icoError, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public bool PonteazaAbsenta(int idUser, int tip, int ctrIn, int ctrSf, DateTime ziua, int idAbs, string culoareValoare)
        {
            bool ras = false;

            try
            {
                string strSql = "";

                if (Constante.tipBD == 1)
                {
                    string strZi = General.ToDataUniv(ziua);

                    if (tip == 1)                        //Actualizare
                    {
                        //inseram datele care nu exista in tabela
                        strSql = "insert into Ptj_Intrari(F10003, Ziua, ZiSapt, ZiLibera, Parinte, Linia, Norma, F10002, F10004, F10005, F10006, F10007, IdContract, F06204) " +     //Radu 25.05.2016 - aduagare F06204
                                        " select a.F10003, " + strZi + ",  " +
                                        " case when datepart(dw,convert(datetime," + strZi + ")) - 1 =0 then 7 else datepart(dw,convert(datetime," + strZi + ")) - 1 end as ZiSapt, " +
                                        " CASE WHEN datepart(dw," + strZi + ")=1 OR datepart(dw," + strZi + ")=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = " + strZi + ")<>0 THEN 1 ELSE 0 END AS ZiLibera,  " +
                                        " 0, 0, CAST(a.F10043 as int) as F10043, a.F10002, a.F10004, a.F10005, a.F10006, a.F10007, b.IdContract, -1 " +         //Radu 25.05.2016 - aduagare F06204 implicit -1
                                        " from F100 a  " +
                                        " inner join F100Contracte b on a.F10003 = b.F10003 and " + ctrIn + " <= b.IdContract and b.IdContract <= " + ctrSf + " and b.DataInceput <= " + strZi + " and " + strZi + " <= b.DataSfarsit " +
                                        " where a.F10003 not in (select F10003 from Ptj_Intrari where year(Ziua)=" + ziua.Year + " and month(Ziua)=" + ziua.Month + " and day(Ziua)=" + ziua.Day + " and COALESCE(Linia,0)=0);";

                        //actualizam toate inregistrarile pt linia principala (suma centrelor de cost)
                        strSql += "update Ptj_Intrari set Valstr=(select TOP 1 DenumireScurta from Ptj_tblAbsente where Id=" + idAbs + "), Val0=null, Val1=null, Val2=null, Val3=null, Val4=null, Val5=null, Val6=null, Val7=null, Val8=null, Val9=null, Val10=null, Val11=null, Val12=null, Val13=null, Val14=null, Val15=null, Val16=null, Val17=null, Val18=null, Val19=null, Val20=null " +
                                  " where year(Ziua)=" + ziua.Year + " and month(Ziua)=" + ziua.Month + " and day(Ziua)=" + ziua.Day + " and COALESCE(Linia,0)=0 and " + ctrIn + " <= IdContract and IdContract <= " + ctrSf + ";";

                        //actualizam toate inregistrarile pt liniile secundare (centrii de cost)
                        strSql += "update Ptj_Intrari set Valstr=null, Val0=null, Val1=null, Val2=null, Val3=null, Val4=null, Val5=null, Val6=null, Val7=null, Val8=null, Val9=null, Val10=null, Val11=null, Val12=null, Val13=null, Val14=null, Val15=null, Val16=null, Val17=null, Val18=null, Val19=null, Val20=null " +
                                  " where year(Ziua)=" + ziua.Year + " and month(Ziua)=" + ziua.Month + " and day(Ziua)=" + ziua.Day + " and COALESCE(Linia,0) <> 0 and " + ctrIn + " <= IdContract and IdContract <= " + ctrSf + ";";
                    }
                    else
                    {
                        //stergem 
                        strSql = "update Ptj_Intrari set ValStr=null " +
                                 " where year(Ziua)=" + ziua.Year + " and month(Ziua)=" + ziua.Month + " and day(Ziua)=" + ziua.Day + " and Valstr=(select TOP 1 DenumireScurta from Ptj_tblAbsente where Id=" + idAbs + ") and " + ctrIn + " <= IdContract and IdContract <= " + ctrSf;
                    }
                }
                else
                {
                    string strZi = General.ToDataUniv(ziua);

                    if (tip == 1)                        //Actualizare
                    {                                                                                                                                                                                        //Radu 25.05.2016 - aduagare F06204 implicit -1
                        strSql = @"insert into ""Ptj_Intrari""(F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""Parinte"", ""Linia"", ""Norma"", F10002, F10004, F10005, F10006, F10007, ""IdContract"", F06204)    
                                select a.F10003, {0},  
                                1 + TRUNC({0}) - TRUNC({0}, 'IW') AS ""ZiSapt"",
                                CASE WHEN (1 + TRUNC ({0}) - TRUNC ({0}, 'IW'))=6 OR (1 + TRUNC ({0}) - TRUNC ({0}, 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = {0})<>0 THEN 1 ELSE 0 END AS ""ZiLibera"", 
                                0, 0, CAST(a.F10043 as int) as F10043, a.F10002, a.F10004, a.F10005, a.F10006, a.F10007, b.""IdContract"", -1 
                                from F100 a  
                                inner join ""F100Contracte"" b on a.F10003 = b.F10003 and {1} <= b.""IdContract"" and b.""IdContract"" <= {2} and b.""DataInceput"" <= {0} and {0} <= b.""DataSfarsit""
                                where a.F10003 not in (select F10003 from ""Ptj_Intrari"" where TRUNC(""Ziua"")={0} and COALESCE(""Linia"",0)=0);";

                        //actualizam toate inregistrarile pt linia principala (suma centrelor de cost)
                        strSql += @"update ""Ptj_Intrari"" set ""ValStr""=(select ""DenumireScurta"" from ""Ptj_tblAbsente"" where ""Id""={3} AND ROWNUM<=1), ""Val0""=null, ""Val1""=null, ""Val2""=null, ""Val3""=null, ""Val4""=null, ""Val5""=null, ""Val6""=null, ""Val7""=null, ""Val8""=null, ""Val9""=null, ""Val10""=null, ""Val11""=null, ""Val12""=null, ""Val13""=null, ""Val14""=null, ""Val15""=null, ""Val16""=null, ""Val17""=null, ""Val18""=null, ""Val19""=null, ""Val20""=null
                                    where TRUNC(""Ziua"")={0} and COALESCE(""Linia"",0)=0 and {1} <= ""IdContract"" and ""IdContract"" <= {2};";

                        //actualizam toate inregistrarile pt liniile secundare (centrii de cost)
                        strSql += @"update ""Ptj_Intrari"" set ""ValStr""=null, ""Val0""=null, ""Val1""=null, ""Val2""=null, ""Val3""=null, ""Val4""=null, ""Val5""=null, ""Val6""=null, ""Val7""=null, ""Val8""=null, ""Val9""=null, ""Val10""=null, ""Val11""=null, ""Val12""=null, ""Val13""=null, ""Val14""=null, ""Val15""=null, ""Val16""=null, ""Val17""=null, ""Val18""=null, ""Val19""=null, ""Val20""=null
                                    where TRUNC(""Ziua"")={0} and COALESCE(""Linia"",0)<>0 and {1} <= ""IdContract"" and ""IdContract"" <= {2};";

                        strSql = string.Format(strSql, strZi, ctrIn, ctrSf, idAbs);
                        strSql = "BEGIN " + strSql + " END;";
                    }
                    else
                    {
                        //stergem 
                        strSql = @"update ""Ptj_Intrari"" set ""ValStr""=null
                                        where TRUNC(""Ziua"")={0} and ""ValStr""=(select ""DenumireScurta"" from ""Ptj_tblAbsente"" where ""Id""={3}) and {1} <= ""IdContract"" and ""IdContract"" <= {2}";
                        strSql = string.Format(strSql, strZi, ctrIn, ctrSf, idAbs);
                    }
                }

                ras = General.ExecutaNonQuery(strSql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }



    }
}