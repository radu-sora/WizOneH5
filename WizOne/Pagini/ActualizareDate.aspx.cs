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
using static WizOne.Module.Dami;

namespace WizOne.Pagini
{
    public partial class ActualizareDate : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnAct.Text = Dami.TraduCuvant("btnAct", "Actualizeaza");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {        
                    txtDataInc.Value = DateTime.Now;               
                }

                cmbUserVechi.DataSource = General.IncarcaDT("SELECT F70102, F70104 FROM USERS ORDER BY F70104", null);
                cmbUserVechi.DataBind();

                cmbUserNou.DataSource = General.IncarcaDT("SELECT F70102, F70104 FROM USERS ORDER BY F70104", null);
                cmbUserNou.DataBind();

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
                if (txtDataInc.Value == null  || checkComboBoxModul.Value == null || cmbUserVechi.Value == null || cmbUserNou.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoError, "");
                    return;
                }  
         

                string[] ids = FiltruModul(checkComboBoxModul.Value.ToString()).Split(';');
                for (int i = 0; i < ids.Length; i++)
                    if (ids[i].Length > 0)
                        ActualizeazaDate(Convert.ToInt32(Session["UserId"]), Convert.ToInt32(ids[i]));

                //if ()

                MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes !"), MessageBox.icoSuccess, "");
                
                //else
                //    MessageBox.Show(Dami.TraduCuvant("Eroare in proces!"), MessageBox.icoError, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void ActualizeazaDate(int idUser, int idModul)
        {
            bool ras = false;

            try
            {
                string strSql = "";
                string dataInc = "CONVERT(DATETIME, '" + Convert.ToDateTime(txtDataInc.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(txtDataInc.Value).Month.ToString().PadLeft(2, '0') + Convert.ToDateTime(txtDataInc.Value).Year.ToString() + "', 103)";
                if (Constante.tipBD == 2)
                    dataInc = "TO_DATE('" + Convert.ToDateTime(txtDataInc.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(txtDataInc.Value).Month.ToString().PadLeft(2, '0') + Convert.ToDateTime(txtDataInc.Value).Year.ToString() + "', 'dd/mm/yyyy')";
                string data = "GETDATE()";
                if (Constante.tipBD == 2)
                    data = "SYSDATE";
                string dataSf = "CONVERT(DATETIME, '01/01/2100', 103)";
                if (Constante.tipBD == 2)
                    data = "TO_DATE('01/01/2100', 'dd/mm/yyyy')";
          
                switch (idModul)
                {
                    case 1:
                        strSql = "UPDATE \"F100Supervizori2\" SET \"IdUser\" = " + cmbUserNou.Value.ToString() + ", \"DataInceput\" = " + dataInc + ", \"DataSfarsit\" = " + dataSf + ", USER_NO =  "
                            + idUser + ", TIME = " + data + " WHERE \"IdUser\" = " + cmbUserVechi.Value.ToString();
                        break;
                    case 2:
                        strSql = "BEGIN" +
                            "  UPDATE F003 SET F00309 = (SELECT COALESCE(F10003, -99) FROM USERS WHERE F70102 = " + cmbUserNou.Value.ToString() + "), USER_NO = " + idUser + ", TIME = " + data + "  WHERE F00309 = " + cmbUserVechi.Value.ToString() + ";" +
                            "  UPDATE F004 SET F00410 = (SELECT COALESCE(F10003, -99) FROM USERS WHERE F70102 = " + cmbUserNou.Value.ToString() + "), USER_NO = " + idUser + ", TIME = " + data + "  WHERE F00410 = " + cmbUserVechi.Value.ToString() + ";" +
                            "  UPDATE F005 SET F00511 = (SELECT COALESCE(F10003, -99) FROM USERS WHERE F70102 = " + cmbUserNou.Value.ToString() + "), USER_NO = " + idUser + ", TIME = " + data + "  WHERE F00511 = " + cmbUserVechi.Value.ToString() + ";" +
                            "  UPDATE F006 SET F00620 = (SELECT COALESCE(F10003, -99) FROM USERS WHERE F70102 = " + cmbUserNou.Value.ToString() + "), USER_NO = " + idUser + ", TIME = " + data + "  WHERE F00620 = " + cmbUserVechi.Value.ToString() + ";" +
                            "  UPDATE F007 SET F00713 = (SELECT COALESCE(F10003, -99) FROM USERS WHERE F70102 = " + cmbUserNou.Value.ToString() + "), USER_NO = " + idUser + ", TIME = " + data + "  WHERE F00713 = " + cmbUserVechi.Value.ToString() + ";" +
                            "  UPDATE F008 SET F00813 = (SELECT COALESCE(F10003, -99) FROM USERS WHERE F70102 = " + cmbUserNou.Value.ToString() + "), USER_NO = " + idUser + ", TIME = " + data + "  WHERE F00813 = " + cmbUserVechi.Value.ToString() + ";" +
                            "END;";
                        break;
                    case 3:
                        strSql = "BEGIN " +
                            "UPDATE \"Ptj_CereriIstoric\" SET \"IdUser\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdUser\" = " + cmbUserVechi.Value.ToString() + ";" +
                            "UPDATE \"Ptj_CereriIstoric\" SET \"IdSuper\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdSuper\" = " + cmbUserVechi.Value.ToString() + ";" +
                            "END;";
                        break;
                    case 4:
                        strSql = "BEGIN " +
                            "UPDATE \"Avs_CereriIstoric\" SET \"IdUser\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdUser\" = " + cmbUserVechi.Value.ToString() + ";" +
                            "UPDATE \"Avs_CereriIstoric\" SET \"IdSuper\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdSuper\" = " + cmbUserVechi.Value.ToString() + ";" +
                            "END;"; 
                        break;
                    case 5:
                        strSql = "BEGIN " +
                                "UPDATE \"BP_Istoric\" SET \"IdUser\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdUser\" = " + cmbUserVechi.Value.ToString() + ";" +
                                "UPDATE \"BP_Istoric\" SET \"IdSuper\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdSuper\" = " + cmbUserVechi.Value.ToString() + ";" +
                                "END;";
                        break;
                    case 6:
                        strSql = "BEGIN " +
                                "UPDATE \"MP_CereriIstoric\" SET \"IdUser\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdUser\" = " + cmbUserVechi.Value.ToString() + ";" +
                                "UPDATE \"MP_CereriIstoric\" SET \"IdSuper\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE \"IdStare\" IN (1, 2, 4) AND \"IdSuper\" = " + cmbUserVechi.Value.ToString() + ";" +
                                "END;";
                        break;
                    case 7:
                        if (Constante.tipBD == 1)
                            strSql = "BEGIN " +
                                "UPDATE Eval_RaspunsIstoric SET IdUser = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + "  FROM Eval_RaspunsIstoric a INNER JOIN Eval_Raspuns b on b.F10003 = a.F10003 and b.IdQuiz = a.IdQuiz WHERE Finalizat is null or Finalizat = 0 AND IdUser = " + cmbUserVechi.Value.ToString() + "; " +
                                "UPDATE Eval_RaspunsIstoric SET IdSuper = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + "  FROM Eval_RaspunsIstoric a INNER JOIN Eval_Raspuns b on b.F10003 = a.F10003 and b.IdQuiz = a.IdQuiz WHERE Finalizat is null or Finalizat = 0 AND IdSuper = " + cmbUserVechi.Value.ToString() + "; " 
                                + "END;";
                        else
                            strSql = "BEGIN " +
                                "UPDATE \"Eval_RaspunsIstoric\" SET \"IdUser\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE EXISTS (SELECT 1 FROM \"Eval_Raspuns\" b where (\"Finalizat\" is null or \"Finalizat\" = 0) and b.F10003 = F10003 and b.\"IdQuiz\" = \"IdQuiz\" ) AND \"IdUser\" = " + cmbUserVechi.Value.ToString() + ";" +
                                "UPDATE \"Eval_RaspunsIstoric\" SET \"IdSuper\" = " + cmbUserNou.Value.ToString() + ", USER_NO = " + idUser + ", TIME = " + data + " WHERE EXISTS (SELECT 1 FROM \"Eval_Raspuns\" b where (\"Finalizat\" is null or \"Finalizat\" = 0) and b.F10003 = F10003 and b.\"IdQuiz\" = \"IdQuiz\" ) AND \"IdSuper\" = " + cmbUserVechi.Value.ToString() + ";" +
                                "END;";
                        break;
                }   

                if (strSql.Length > 0)
                    ras = General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            //return ras;

        }

        public static string FiltruModul(string modul)
        {
            string val = "";

            try
            {
                string[] param = modul.Split(';');
                foreach (string elem in param)
                {
                    switch (elem.ToLower())
                    {
                        case "supervizori":
                            val += "1;";
                            break;
                        case "organigrama":
                            val += "2;";
                            break;
                        case "cereri":
                            val += "3;";
                            break;
                        case "avans":
                            val += "4;";
                            break;
                        case "prime":
                            val += "5;";
                            break;
                        case "cereri diverse":
                            val += "6;";
                            break;
                        case "evaluare":
                            val += "7;";
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }

            return val;
        }



    }
}