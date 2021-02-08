using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class Fisiere : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //tip - 0 este atasament
                //tip - 1 este cerere

                if (!IsPostBack)
                {
                    string tip = General.Nz(Request["tip"],"0").ToString();
                    string tipTbl = Request["tbl"];
                    string id = Request["id"];

                    if (tipTbl != "" && id != "")
                    {
                        string tbl = "";

                        switch (tipTbl)
                        {
                            case "1":
                                tbl = "Ptj_Cereri";
                                break;
                            case "2":
                                tbl = "MP_Cereri";
                                break;
                            case "3":
                                //Florin 2020.12.11
                                {
                                    //metaCereriDate itm = new metaCereriDate();

                                    //itm.UploadedFile = dtDoc.Rows[0]["Fisier"];
                                    //itm.UploadedFileName = dtDoc.Rows[0]["FisierNume"];
                                    //itm.UploadedFileExtension = dtDoc.Rows[0]["FisierExtensie"];
                                    tbl = "";
                                    Organigrama.Posturi.metaCereriDate itm = Session["Posturi_Upload"] as Organigrama.Posturi.metaCereriDate;
                                    if (itm != null && itm.UploadedFile != null)
                                        scrieDoc(General.Nz(itm.UploadedFileExtension, ".txt").ToString(), (byte[])itm.UploadedFile, General.Nz(itm.UploadedFileName, "Fisier").ToString());
                                    else
                                        Response.Write("Nu exista date de afisat !");
                                }
                                //tbl = "Org_Posturi";
                                break;
                            case "4":
                                {
                                    //Florin 2020.08.20
                                    Dictionary<int, Personal.Atasamente.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Atasamente"] as Dictionary<int, Personal.Atasamente.metaUploadFile>;
                                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(id)) && lstFiles[Convert.ToInt32(id)] != null)
                                    {
                                        scrieDoc(lstFiles[Convert.ToInt32(id)].UploadedFileExtension.ToString(), (byte[])lstFiles[Convert.ToInt32(id)].UploadedFile, lstFiles[Convert.ToInt32(id)].UploadedFileName.ToString());
                                    }
                                    else
                                    {
                                        //DataTable dtAt = General.IncarcaDT("SELECT * FROM \"Atasamente\"", null);
                                        //DataRow drAt = dtAt.Select("IdAuto = " + id).FirstOrDefault();
                                        DataRow drAt = General.IncarcaDR(@"SELECT * FROM ""Atasamente"" WHERE ""IdAuto""=@1", new object[] { id });
                                        if (drAt != null)
                                        {
                                            string numeFiser = General.Nz(drAt["FisierNume"], "").ToString();
                                            object fisier = General.Nz(drAt["Attach"], null);

                                            string cale = HostingEnvironment.MapPath("~/FisiereApp/Atasamente/") + numeFiser;
                                            if (fisier == null && File.Exists(cale))
                                                fisier = File.ReadAllBytes(cale);

                                            if (fisier != null)
                                                scrieDoc(General.Nz(drAt["FisierExtensie"], ".txt").ToString(), (byte[])fisier, numeFiser);
                                            else
                                                Response.Write("Nu exista date de afisat !");
                                        }
                                        else
                                            Response.Write("Nu exista date de afisat !");
                                    }
                                    tbl = "";
                                }
                                break;
                            case "5":
                                {
                                    tbl = "Admin_Medicina";
                                    Dictionary<int, Personal.Medicina.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, Personal.Medicina.metaUploadFile>;
                                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(id)) && lstFiles[Convert.ToInt32(id)] != null)
                                    {
                                        scrieDoc(lstFiles[Convert.ToInt32(id)].UploadedFileExtension.ToString(), (byte[])lstFiles[Convert.ToInt32(id)].UploadedFile, lstFiles[Convert.ToInt32(id)].UploadedFileName.ToString());
                                        tbl = "";
                                    }
                                }
                                break;
                            case "6":
                                {
                                    tbl = "Admin_Sanctiuni";
                                    Dictionary<int, Personal.Sanctiuni.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, Personal.Sanctiuni.metaUploadFile>;
                                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(id)) && lstFiles[Convert.ToInt32(id)] != null)
                                    {
                                        scrieDoc(lstFiles[Convert.ToInt32(id)].UploadedFileExtension.ToString(), (byte[])lstFiles[Convert.ToInt32(id)].UploadedFile, lstFiles[Convert.ToInt32(id)].UploadedFileName.ToString());
                                        tbl = "";
                                    }
                                }
                                break;
                            case "8":
                                tbl = "Admin_NrActAd";
                                break;
                            case "9":
                                tbl = "Avs_Cereri";
                                break;
                            case "10":
                                {
                                    tbl = "Admin_Beneficii";
                                    Dictionary<int, Personal.Beneficii.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, Personal.Beneficii.metaUploadFile>;
                                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(id)) && lstFiles[Convert.ToInt32(id)] != null)
                                    {
                                        scrieDoc(lstFiles[Convert.ToInt32(id)].UploadedFileExtension.ToString(), (byte[])lstFiles[Convert.ToInt32(id)].UploadedFile, lstFiles[Convert.ToInt32(id)].UploadedFileName.ToString());
                                        tbl = "";
                                    }
                                }
                                break;
                            case "11":
                                {
                                    tbl = "F100Studii";
                                    Dictionary<int, Personal.StudiiNou.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Studii"] as Dictionary<int, Personal.StudiiNou.metaUploadFile>;
                                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(id)) && lstFiles[Convert.ToInt32(id)] != null)
                                    {
                                        scrieDoc(lstFiles[Convert.ToInt32(id)].UploadedFileExtension.ToString(), (byte[])lstFiles[Convert.ToInt32(id)].UploadedFile, lstFiles[Convert.ToInt32(id)].UploadedFileName.ToString());
                                        tbl = "";
                                    }
                                }
                                break;
                            case "12":
                                {
                                    tbl = "Admin_Cursuri";
                                    Dictionary<int, Personal.Cursuri.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Cursuri"] as Dictionary<int, Personal.Cursuri.metaUploadFile>;
                                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(id)) && lstFiles[Convert.ToInt32(id)] != null)
                                    {
                                        scrieDoc(lstFiles[Convert.ToInt32(id)].UploadedFileExtension.ToString(), (byte[])lstFiles[Convert.ToInt32(id)].UploadedFile, lstFiles[Convert.ToInt32(id)].UploadedFileName.ToString());
                                        tbl = "";
                                    }
                                }
                                break;
                            case "13":
                                tbl = "Curs_Inregistrare";
                                break;
                            case "14":
                                tbl = "Curs_Anterior";
                                break;
                            case "15":
                                tbl = "Curs_Inregistrare";
                                break;
                            case "16":
                                {
                                    tbl = "Curs_Inregistrare";
                                    Curs.CursuriInregistrare.metaUploadFile fisier = Session["DocUpload_CursInreg"] as Curs.CursuriInregistrare.metaUploadFile;
                                    if (fisier != null)
                                    {
                                        scrieDoc(fisier.UploadedFileExtension.ToString(), (byte[])fisier.UploadedFile, fisier.UploadedFileName.ToString());
                                        tbl = "";
                                    }
                                }
                                break;
                            case "17":
                                tbl = "Curs_relSesiuneDocumente";
                                id = "(SELECT \"IdDocument\" FROM \"Curs_relSesiuneDocumente\" WHERE \"IdCurs\" = " + id + " AND \"IdSesiune\" = -99)";
                                break;
                        }

                        if (tbl.Length > 0)
                        {
                            DataTable dt = General.IncarcaDT($@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""='{tbl}' AND ""Id""={id} AND ""EsteCerere""={tip}", null);

                            if (dt.Rows.Count != 0)
                            {
                                //Florin 2020.08.18
                                byte[] fis = (byte[])General.Nz(dt.Rows[0]["Fisier"], null);
                                string director = tbl.Replace("Admin_", "").Replace("F100", "");
                                string cale = HostingEnvironment.MapPath("~/FisiereApp/" + director + "/") + General.Nz(dt.Rows[0]["FisierNume"], "").ToString();
                                if (fis == null && File.Exists(cale))
                                        fis = File.ReadAllBytes(cale);

                                if (fis != null)
                                    scrieDoc(General.Nz(dt.Rows[0]["FisierExtensie"], "").ToString(), fis, General.Nz(dt.Rows[0]["FisierNume"], "").ToString());
                                else
                                    Response.Write("Nu exista date de afisat !");
                            }
                            else
                            {
                                Response.Write("Nu exista date de afisat !");
                            }
                        }
                    }
                    else
                    {
                        Response.Write("Nu exista date de afisat !");
                    }
                }
            }
            catch (Exception ex)
            {
                string ert = ex.ToString();
            }
        }

        protected void scrieDoc(string extensie, byte[] fisier, string numeFisier)
        {
            Response.ClearContent();

            if (extensie.EndsWith(".pdf"))
                Response.ContentType = "application/pdf";
            else if (extensie.EndsWith(".xlsx"))
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            else if (extensie.EndsWith(".xls"))
                Response.ContentType = "application/vnd.ms-excel";
            else if (extensie.EndsWith(".docx"))
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            else if (extensie.EndsWith(".doc"))
                Response.ContentType = "application/msword";
            else if (extensie.EndsWith(".zip"))
                Response.ContentType = "application/zip";
            else if (extensie.EndsWith(".gif"))
                Response.ContentType = "image/gif";
            else if (extensie.EndsWith(".tiff"))
                Response.ContentType = "image/tiff";
            else if (extensie.EndsWith(".bmp"))
                Response.ContentType = "image/bmp";
            else if (extensie.EndsWith(".png"))
                Response.ContentType = "image/png";
            else if (extensie.EndsWith(".htm"))
                Response.ContentType = "text/html";
            else if (extensie.EndsWith(".html"))
                Response.ContentType = "text/html";
            else if (extensie.EndsWith(".txt"))
                Response.ContentType = "text/plain";
            else if (extensie.EndsWith(".xml"))
                Response.ContentType = "text/xml";
            else if (extensie.EndsWith(".msg"))
            {
                Response.AddHeader("content-disposition", "attachment; filename=test.msg");
            }
            else
                Response.ContentType = "image/jpeg";

            Response.BinaryWrite(fisier);
            Response.ContentEncoding = System.Text.Encoding.ASCII;

            //if (extensie.EndsWith(".doc") || extensie.EndsWith(".docx") || extensie.EndsWith(".xls"))
            //    Response.End();

            Response.AppendHeader("Content-Disposition", "attachment; filename=" + numeFisier);

            HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
            HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            HttpContext.Current.ApplicationInstance.CompleteRequest();


            //Response.End();
            //Response.Flush();
            //System.Threading.Thread.Sleep(1000);
        }


    }
}