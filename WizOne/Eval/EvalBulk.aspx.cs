using DevExpress.Web;
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
    public partial class EvalBulk : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                txt1.InnerText = Dami.TraduCuvant(txt1.InnerText);
                txt2.InnerText = Dami.TraduCuvant(txt2.InnerText);

                btnTrimite.Text = Dami.TraduCuvant(btnTrimite.Text);

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    ListEditItem lst = new ListEditItem();
                    lst.Text = Dami.TraduCuvant("Locatie");
                    lst.Value = 1;
                    lst.Selected = true;

                    ListEditItem lst2 = new ListEditItem();
                    lst2.Text = Dami.TraduCuvant("Departament");
                    lst2.Value = 2;

                    cmbTip.Items.Add(lst);
                    cmbTip.Items.Add(lst2);

                    DataTable dt = General.IncarcaDT(@"SELECT Id, Denumire FROM ""Eval_Quiz"" WHERE COALESCE(""CategorieQuiz"",0)=1 ", null);
                    cmbQuiz.DataSource = dt;
                    cmbQuiz.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnTrimite_Click(object sender, EventArgs e)
        {
            try
            {
                if (General.Nz(cmbQuiz.Value,"").ToString() == "" || General.Nz(cmbTip.Value,"").ToString() == "")
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoWarning, "Atentie");
                    return;
                }

                DataTable dt = General.IncarcaDT(@"SELECT * FROM Eval_SetAngajatiDetail WHERE IdSetAng = (SELECT IdGrup FROM Eval_relGrupAngajatQuiz WHERE IdQuiz=@1)", new object[] { cmbQuiz.Value });
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    //facem o initializare clasica apoi stergem istoricul si il cream pt 360
                    List<metaQuizAngajat> lst = new List<metaQuizAngajat>();
                    lst.Add(new metaQuizAngajat { IdQuiz = Convert.ToInt32(General.Nz(cmbQuiz.Value,-99)), F10003 = Convert.ToInt32(General.Nz(dt.Rows[i]["Id"], -99)) });
                    //initializam clasic
                    Evaluare.InitializareChestionar(lst, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                    //stergem
                    General.ExecutaNonQuery(@"DELETE FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 ", new object[] { cmbQuiz.Value, dt.Rows[i]["Id"] });
                    //cream istoricul
                    //General.ExecutaScalar(
                    //    $@"INSERT INTO ""Eval_RaspunsIstoric""(""IdQuiz"", F10003, ""IdSuper"", ""IdUser"", ""Pozitie"", ""Culoare"", USER_NO, TIME)
                    //    SELECT @1 AS IdQuiz, @2 AS F10003, 
                    //    -1 * COALESCE((SELECT COALESCE(IdSuper,1) FROM F100Supervizori WHERE F10003=@2 AND IdUser=B.F70102),1) AS IdSuper, 
                    //    B.F70102,  
                    //    CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS Pozitie, 
                    //    '#FFFFFF00' AS Culoare, 1 AS USER_NO, GetDate() AS TIME
                    //    FROM F100 A
                    //    INNER JOIN USERS B ON A.F10003=B.F10003
                    //    WHERE A.F10003 <> @2 AND A.F10007=(SELECT X.F10007 FROM F100 X WHERE X.F10003=@2)", new object[] { cmbQuiz.Value, dt.Rows[i]["Id"] });

                    General.ExecutaScalar(
                        $@"INSERT INTO ""Eval_RaspunsIstoric""(""IdQuiz"", F10003, ""IdSuper"", ""IdUser"", ""Pozitie"", ""Culoare"", USER_NO, TIME)
                        SELECT @1 AS IdQuiz, @2 AS F10003, B.F70102 AS IdSuper, B.F70102,  
                        CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS Pozitie, 
                        '#FFFFFF00' AS Culoare, 1 AS USER_NO, GetDate() AS TIME
                        FROM F100 A
                        INNER JOIN USERS B ON A.F10003=B.F10003
                        WHERE A.F10003 <> @2 AND A.F10007=(SELECT X.F10007 FROM F100 X WHERE X.F10003=@2)", new object[] { cmbQuiz.Value, dt.Rows[i]["Id"] });

                }

                MessageBox.Show(Dami.TraduCuvant("Operatie realizata cu succes"), MessageBox.icoSuccess, Dami.TraduCuvant("Initializare in masa"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}