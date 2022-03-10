using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Wizrom.Reports.Code;
using Wizrom.Reports.Models;

namespace WizOne.Module
{

    public class TrimitereWhatsApp
    {
        private class ReportExportException : Exception
        {
            public ReportExportException(string message) : base(message)
            { }
        }

        internal static bool UploadFile(string name, Stream content)
        {
            // Get SFTP address, user & password from config and upload the file in blocking mode (same thread)
            // ...
            var result = true;

            try
            {
                //Renci.SshNet.SftpClient client = new Renci.SshNet.SftpClient(...);
                //client.UploadFile(...)
            }
            catch (Exception ex)
            {
                result = false;
                General.MemoreazaEroarea(ex, "TrimitereWhatsApp", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return result;
        }

        internal static void SendNotification(string message, string fileName)
        {
            // Get message template and base download file url from config
            // Call Twilio API and send the message with a download file url in blocking mode (same thread)
            try
            {
                if (fileName != null)
                {
                    // OK message with download file url
                }
                else
                {
                    // End user error message with no download file url
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "TrimitereWhatsApp", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        internal static (string, Stream, string) GenerareDocument(string telefon, int parametru)
        {
            var fileName = null as string;
            var fileContent = null as Stream;
            var errorMessage = null as string;           

            try
            {
                string msg = "";
                DataTable dtAng = General.IncarcaDT("SELECT * FROM F100 WHERE F10088 = '" + telefon + "'", null);

                if (dtAng != null && dtAng.Rows.Count > 0)
                {
                    if (dtAng.Rows[0]["F10016"] == DBNull.Value || dtAng.Rows[0]["F10016"].ToString().Length <= 0)
                    {
                        msg = "Angajatul cu marca " + dtAng.Rows[0]["F10003"].ToString() + " nu are completata parola pentru PDF!";

                        throw new ReportExportException(msg);
                    }

                    string sirParam = Dami.ValoareParam("ListaIDuriRapoarteWA", "");
                    if (sirParam.Length <= 0)
                    {
                        msg = "Nu este definita lista cu ID-urile rapoartelor";
                        
                        throw new ReportExportException(msg);
                    }

                    string[] lstRap = sirParam.Split(';');
                    if (lstRap.Length < parametru)
                    {
                        msg = "Nu este specificat ID-ul pentru parametrul " + parametru;
                        
                        throw new ReportExportException(msg);
                    }

                    int luna = DateTime.Now.Month;
                    int an = DateTime.Now.Year;

                    if (lstRap[parametru - 1].StartsWith("G"))
                    {                        
                        using (var xtraReport = new XtraReport())
                        {
                            var report = null as Report;
                            var reportId = Convert.ToInt32(lstRap[parametru - 1].Substring(1));

                            using (var entities = new ReportsEntities())
                                report = entities.Reports.Find(reportId);

                            if (report == null)
                                throw new Exception($"Nici un raport gasit cu ID-ul {reportId}");

                            if (report.LayoutData == null)
                                throw new Exception($"Nu exista un layout definit pentru raportul cu ID-ul {reportId}");

                            using (var memStream = new MemoryStream(report.LayoutData))
                                xtraReport.LoadLayoutFromXml(memStream);

                            var values = new
                            {
                                Implicit = new { UserId = HttpContext.Current.Session?["UserId"] },
                                Explicit = new { Angajat = dtAng.Rows[0]["F10003"].ToString(), Luna = luna, An = an }
                            };
                            var implicitValues = values.Implicit.GetType().GetProperties() as PropertyInfo[];
                            var explicitValues = values.Explicit?.GetType().GetProperties() as PropertyInfo[];
                            var parameters = xtraReport.ObjectStorage.OfType<DevExpress.DataAccess.Sql.SqlDataSource>().
                                SelectMany(ds => ds.Queries).SelectMany(q => q.Parameters).
                                Where(p => p.Type != typeof(Expression)).
                                Union(xtraReport.ComponentStorage.OfType<DevExpress.DataAccess.Sql.SqlDataSource>().
                                SelectMany(ds => ds.Queries).SelectMany(q => q.Parameters).
                                Where(p => p.Type != typeof(Expression)));

                            foreach (var param in parameters)
                            {
                                var name = param.Name.TrimStart('@');
                                var value = explicitValues?.SingleOrDefault(p => p.Name == name)?.GetValue(values.Explicit) ??
                                    implicitValues.SingleOrDefault(p => p.Name == name)?.GetValue(values.Implicit);

                                if (value != null)
                                    param.Value = Convert.ChangeType(value, param.Type);
                            }

                            xtraReport.PrintingSystem.AddService(typeof(IConnectionProviderService), new ReportConnectionProviderService());
                            xtraReport.ExportOptions.Pdf.PasswordSecurityOptions.OpenPassword = dtAng.Rows[0]["F10016"].ToString();

                            fileContent = new MemoryStream();
                            xtraReport.ExportToPdf(fileContent, xtraReport.ExportOptions.Pdf);
                            fileContent.Position = 0;                            
                        }    
                        
                        string numeFis = "Fluturaș_" + dtAng.Rows[0]["F10003"].ToString() + ".pdf";
                        if (Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.Elanor)
                        {
                            string dataInc = an.ToString() + luna.ToString().PadLeft(2, '0') + "01";
                            string dataSf = an.ToString() + luna.ToString().PadLeft(2, '0') + DateTime.DaysInMonth(an, luna).ToString();

                            numeFis = "P_SLP_02344_" + dataInc + "_" + dataSf + "_00_V2_0000_00000_FILE_" + dtAng.Rows[0]["F10003"].ToString() + "_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + ".pdf";
                        }

                        fileName = numeFis;
                    }
                    else //#1014 - Adeverinta
                    {
                        List<int> lst = new List<int>();
                        lst.Add(Convert.ToInt32(dtAng.Rows[0]["F10003"].ToString()));
                        string numeFisier = "";
                        Pagini.TrimitereFluturasi pag = new Pagini.TrimitereFluturasi();
                        byte[] fisier = pag.GenerareAdeverinta(lst, Convert.ToInt32(lstRap[parametru - 1].Substring(1)), DateTime.Now.Year, dtAng.Rows[0]["F10016"].ToString(), out numeFisier);

                        fileContent = new MemoryStream(fisier);                        
                        fileName = numeFisier;
                    }
                }
                else
                {
                    msg = "Numarul " + telefon  + " nu a fost gasit in baza de date!";

                    throw new ReportExportException(msg);
                }     
            }
            catch (Exception ex)
            {
                fileContent?.Dispose();

                if (ex.GetType() == typeof(ReportExportException))
                    errorMessage = ex.Message;
                else                    
                    throw;
            }

            return (fileName, fileContent, errorMessage);
        }

        public static void SendAsync(string phone, int serial)
        {
            HostingEnvironment.QueueBackgroundWorkItem(_ => 
            {
                try
                {
                    (string fileName, Stream fileContent, string errorMessage) = GenerareDocument(phone, serial);

                    if (errorMessage == null)
                    {
                        if (UploadFile(fileName, fileContent))
                            SendNotification("[OK message]", fileName);
                        /*else
                            SendNotification("[File not available message]", null);*/

                        fileContent?.Dispose();
                    }
                    else
                        SendNotification(errorMessage, null);                    
                }
                catch (Exception ex)
                {
                    General.MemoreazaEroarea(ex, "TrimitereWhatsApp", new StackTrace().GetFrame(0).GetMethod().Name);
                }
            });
        }
    }
}