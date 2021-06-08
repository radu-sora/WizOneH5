using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.IO;
using System.Data;
using System.Diagnostics;
using ProceseSec;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace WizOne.Pagini
{
    public partial class SchimbaParolaCaptcha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Dami.AccesApp(this.Page);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                //verific captcha
                if (IsReCaptchValid())
                {

                }
                else
                {
                    MessageBox.Show("Va rugam verificati din nou codul captcha", MessageBox.icoWarning, "Captcha");
                    return;
                }

                if (General.VarSession("UserId").ToString() == "-99" || General.VarSession("User").ToString() == "")
                {
                    MessageBox.Show("Utilizator invalid", MessageBox.icoWarning, "", "Default.aspx");
                }

                DataRow dr = General.IncarcaDR("SELECT F70103 FROM USERS WHERE F70104=@1", new string[] { General.VarSession("User").ToString() });

                if (txtPan1.Text == "" || txtPan2.Text == "" || txtPan3.Text == "")
                {
                    MessageBox.Show("Lipsesc date !", MessageBox.icoWarning, "");
                }
                else
                {
                    CriptDecript prc = new CriptDecript();
                    string parolaDinBaza = prc.EncryptString(Constante.cheieCriptare, dr["F70103"].ToString(), 2);

                    if (dr == null || parolaDinBaza != txtPan1.Text)
                    {
                        MessageBox.Show("Parola actuala eronata", MessageBox.icoWarning, "");
                    }
                    else
                    {
                        if (txtPan2.Text != txtPan3.Text)
                        {
                            MessageBox.Show("Parola nu coincide !", MessageBox.icoWarning, "");
                        }
                        else
                        {
                            if (General.VarSession("ParolaComplexa").ToString() == "1")
                            {
                                var ras = General.VerificaComplexitateParola(txtPan2.Text);
                                if (ras != "")
                                {
                                    MessageBox.Show(ras, MessageBox.icoWarning, "Parola invalida");
                                    return;
                                }
                            }

                            string idUser = General.Nz(HttpContext.Current.Session["UserId"], -99).ToString();
                            General.AddUserIstoric(idUser);

                            string parolaCriptata = prc.EncryptString(Constante.cheieCriptare, txtPan2.Text, 1);

                            string strSql = @"UPDATE USERS SET F70103=@1, F70113=0, F70122=GETDATE() WHERE F70102=@2";
                            if (Constante.tipBD == 2) strSql = @"UPDATE USERS SET F70103=@1, F70113=0, F70122=SYSDATE WHERE F70102=@2";
                            General.ExecutaNonQuery(strSql, new string[] { parolaCriptata, Session["UserId"].ToString() });
                            Session["SecApp"] = "OK";
                            MessageBox.Show("Proces realizat cu succes", MessageBox.icoWarning, "", "MainPage.aspx");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        public bool IsReCaptchValid()
        {
            var result = false;
            var captchaResponse = Request.Form["g-recaptcha-response"];
            var secretKey = "6Lckrq0UAAAAANS6NOM2-zPpYFfrKuWMq0r48keQ";
            var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            var requestUri = string.Format(apiUrl, secretKey, captchaResponse);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    var isSuccess = jResponse.Value<bool>("success");
                    result = (isSuccess) ? true : false;
                }
            }
            return result;
        }

    }
}