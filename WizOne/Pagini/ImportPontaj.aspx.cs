using DevExpress.Web;
using ProceseSec;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Net.Http;
using Newtonsoft.Json;

namespace WizOne.Pagini
{
    public partial class ImportPontaj : System.Web.UI.Page
    {

        public class Pontare
        {
            public DateTime timeStamp { get; set; }
            public string deviceId { get; set; }
            public string deviceName { get; set; }
            public string cardId { get; set; }
            public string cardName { get; set; }
            public string cardTag { get; set; }
            public int eventCode { get; set; }
            public string @event { get; set; }
            public int? actionCode { get; set; }
            public string action { get; set; }
            public string areaId { get; set; }
            public string areaName { get; set; }
        }

        public class ListaPontari
        {
            public int totalCount { get; set; }
            public List<Pontare> items { get; set; }
            public string message { get; set; }
        }

        public class Login
        {
            public string token { get; set; }
            public string message { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                //btnClose.Text = Dami.TraduCuvant("btnClose", "Inchidere luna");
                //btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare luna");

                #endregion


                if (!IsPostBack)
                {
                    deDataStart.Value = DateTime.Now.AddDays(-1);
                    deDataSfarsit.Value = DateTime.Now;
                }


            }
            catch (Exception ex)
            {
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                switch (e.Parameter)
                {
                    case "btnSave":
                        Salvare();
                        break;
                }
            }
            catch (Exception ex)
            {
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private async void Salvare()
        {       
            var baseAddress = new Uri("https://wifi.creasoft.ro/Api/Account");
            string utilizator = "", parola = "";
            Login logare = new Login();

            if (deDataStart.Value == null || deDataSfarsit.Value == null)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat intervalul de timp!");
                return;
            }

            if (Convert.ToDateTime(deDataSfarsit.Value) < Convert.ToDateTime(deDataStart.Value))
            {
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data de sfarsit este anterioara celei de start!");
                return;
            }

            //if ((Convert.ToDateTime(deDataSfarsit.Value) - Convert.ToDateTime(deDataStart.Value)).Days > 1)
            //{
            //    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Raportul nu poate fi cerut pe o perioada mai mare de o zi!");
            //    return;
            //}

            DataTable dtLogare = General.IncarcaDT("SELECT \"Nume\", \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" IN ('ImportPontaj_Utilizator', 'ImportPontaj_Parola')", null);
            if (dtLogare != null && dtLogare.Rows.Count == 2)
            {
                CriptDecript prc = new CriptDecript();
                utilizator = prc.EncryptString(Constante.cheieCriptare, (dtLogare.Rows[0]["Nume"].ToString() == "ImportPontaj_Utilizator" ? dtLogare.Rows[0]["Valoare"].ToString() : dtLogare.Rows[1]["Valoare"].ToString()), 2); 
                parola = prc.EncryptString(Constante.cheieCriptare, (dtLogare.Rows[0]["Nume"].ToString() == "ImportPontaj_Parola" ? dtLogare.Rows[0]["Valoare"].ToString() : dtLogare.Rows[1]["Valoare"].ToString()), 2); ;
            }
            else
            {
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu s-au gasit datele de conectare!");
                return;
            }

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                using (var content = new StringContent("{  \"username\": \"" + utilizator + "\",  \"password\": \"" + parola + "\", \"tenancyName\" : \"WIZROM\"}", System.Text.Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync("account", content))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        logare = JsonConvert.DeserializeObject<Login>(responseData);

                        if (logare.message != null && logare.message.Length > 0)
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(logare.message);
                            return;
                        }
                    }
                }
            }
            
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", "Bearer " + logare.token);
                string dtStart = Convert.ToDateTime(deDataStart.Value).Year.ToString() + "-" + Convert.ToDateTime(deDataStart.Value).Month.ToString().PadLeft(2, '0') + "-"
                    + Convert.ToDateTime(deDataStart.Value).Day.ToString().PadLeft(2, '0');
                string dtSfarsit = Convert.ToDateTime(deDataSfarsit.Value).Year.ToString() + "-" + Convert.ToDateTime(deDataSfarsit.Value).Month.ToString().PadLeft(2, '0') + "-"
                    + Convert.ToDateTime(deDataSfarsit.Value).Day.ToString().PadLeft(2, '0');

                using (var content = new StringContent("{  \"start\": \"" + dtStart + "\",  \"end\": \"" + dtSfarsit + "\"}", System.Text.Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync("reports/events", content))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        ListaPontari lista = JsonConvert.DeserializeObject<ListaPontari>(responseData);

                        if (lista.message != null && lista.message.Length > 0)
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(lista.message);
                            return;
                        }

                        for (int i = 0; i < lista.totalCount; i++)
                        {
                            if (lista.items[i].actionCode == null || lista.items[i].actionCode == 2)
                                continue;

                            if (lista.items[i].eventCode != 0 && lista.items[i].eventCode != 3 && lista.items[i].eventCode != 11)
                                continue;                            
                                                   
                            string sql = "SELECT COUNT(*) FROM \"Ptj_tmpCeasuri\" WHERE \"Cartela\" = '{0}' AND \"DataPontare\" = {1} AND \"InOut\" = {2}";
                            sql = string.Format(sql, lista.items[i].cardTag, 
                                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + lista.items[i].timeStamp.Year.ToString()  
                                + "-" + lista.items[i].timeStamp.Month.ToString().PadLeft(2, '0') + "-" + lista.items[i].timeStamp.Day.ToString().PadLeft(2, '0') 
                                + " " + lista.items[i].timeStamp.Hour.ToString().PadLeft(2, '0') + ":" + lista.items[i].timeStamp.Minute.ToString().PadLeft(2, '0') 
                                + ":" + lista.items[i].timeStamp.Second.ToString().PadLeft(2, '0') + "', 120)" : 
                                "TO_DATE('" + lista.items[i].timeStamp.Day.ToString().PadLeft(2, '0')
                                + "/" + lista.items[i].timeStamp.Month.ToString().PadLeft(2, '0') + "/" + lista.items[i].timeStamp.Year.ToString() + " " 
                                + lista.items[i].timeStamp.Hour.ToString().PadLeft(2, '0') + ":" + lista.items[i].timeStamp.Minute.ToString().PadLeft(2, '0')
                                + ":" + lista.items[i].timeStamp.Second.ToString().PadLeft(2, '0') + "', 'dd/mm/yyyy HH24:mi:ss')"), 
                                lista.items[i].actionCode);
                            DataTable dt = General.IncarcaDT(sql, null);

                            if (dt == null || dt.Rows.Count == 0 || dt.Rows[0][0] == null || dt.Rows[0][0].ToString().Length <= 0 || Convert.ToInt32(dt.Rows[0][0].ToString()) == 0)
                            {                                
                                sql = "INSERT INTO \"Ptj_tmpCeasuri\" (\"Cartela\", \"DataPontare\", \"InOut\", \"DataImportare\", \"Importat\") VALUES ("
                                    + "'" + lista.items[i].cardTag + "', " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + lista.items[i].timeStamp.Year.ToString()
                                + "-" + lista.items[i].timeStamp.Month.ToString().PadLeft(2, '0') + "-" + lista.items[i].timeStamp.Day.ToString().PadLeft(2, '0')
                                + " " + lista.items[i].timeStamp.Hour.ToString().PadLeft(2, '0') + ":" + lista.items[i].timeStamp.Minute.ToString().PadLeft(2, '0')
                                + ":" + lista.items[i].timeStamp.Second.ToString().PadLeft(2, '0') + "', 120)" :
                                "TO_DATE('" + lista.items[i].timeStamp.Day.ToString().PadLeft(2, '0')
                                + "/" + lista.items[i].timeStamp.Month.ToString().PadLeft(2, '0') + "/" + lista.items[i].timeStamp.Year.ToString() + " "
                                + lista.items[i].timeStamp.Hour.ToString().PadLeft(2, '0') + ":" + lista.items[i].timeStamp.Minute.ToString().PadLeft(2, '0')
                                + ":" + lista.items[i].timeStamp.Second.ToString().PadLeft(2, '0') + "', 'dd/mm/yyyy HH24:mi:ss')") + ", "
                                + lista.items[i].actionCode + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ", 0)";
                                General.ExecutaNonQuery(sql, null);
                            }
                        }
                    }
                }
            }

            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Import terminat cu succes!");
        }
     }        
}