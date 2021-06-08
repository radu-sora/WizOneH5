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
using System.Web.UI.HtmlControls;

namespace WizOne.Pagini
{
    public partial class Captcha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Dami.AccesApp(this.Page);

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes["class"] = "ssSchimba";

            HtmlGenericControl divCap = new HtmlGenericControl("div");
            divCap.Attributes["class"] = "g-recaptcha";
            divCap.Attributes["data-sitekey"] = Dami.ValoareParam("Captcha_Site");

            Button btn = new Button();
            btn.ID = "btnOk";
            btn.Text = "OK";
            btn.TabIndex = 3;
            btn.ValidationGroup = "IntroGrup";
            btn.Click += btnOk_Click;

            div.Controls.Add(divCap);
            divOuter.Controls.Add(div);
            divOuter.Controls.Add(btn);


        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                //verific captcha

                if (IsReCaptchValid())
                {
                    //de facut redirect in continuare
                    if(!string.IsNullOrWhiteSpace(Session["NextPage"].ToString()))
                        Response.Redirect(Session["NextPage"].ToString(), false);
                }
                else
                {
                    MessageBox.Show("Va rugam verificati din nou codul captcha", MessageBox.icoWarning, "Captcha");
                    return;
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
            var secretKey = Dami.ValoareParam("Captcha_Secret");
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