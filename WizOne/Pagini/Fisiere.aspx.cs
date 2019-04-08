using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                        string tbl = "xxx";

                        switch (tipTbl)
                        {
                            case "1":
                                tbl = "Ptj_Cereri";
                                break;
                            case "2":   //Radu 13.08.2018
                                tbl = "MP_Cereri";
                                break;
                            case "3":   //Florin 2018.09.28
                                tbl = "Org_Posturi";
                                break;
                            case "4":   //Florin 2019.12.04 (se citeste din tabela Atasamente)
                                {
                                    DataTable dtAta = General.IncarcaDT($@"SELECT * FROM ""Atasamente"" WHERE ""IdAuto""={id}", null);
                                    if (dtAta.Rows.Count > 0)
                                    {
                                        string numeFis = (dtAta.Rows[0]["DescrAttach"] ?? "").ToString();
                                        string ext = "txt";
                                        if (numeFis.LastIndexOf(".") >= 0)
                                            ext = numeFis.Substring(numeFis.LastIndexOf(".") + 1);

                                        scrieDoc(ext, (byte[])dtAta.Rows[0]["Attach"], numeFis);
                                    }
                                    else
                                        Response.Write("Nu exista date de afisat !");
                                    
                                    return;
                                }
                            case "5":
                                tbl = "Admin_Medicina"; //Radu 22.02.2019
                                break;
                            case "6":
                                tbl = "Admin_Sanctiuni"; //Radu 22.02.2019
                                break;
                            case "7":
                                tbl = "Atasamente"; //Radu 22.02.2019
                                break;
                            case "8":
                                tbl = "Admin_NrActAd";
                                break;                                //case "5":   //Florin 2019.12.04 (se citeste din tabela Admin_Medicina)
                                //    {
                                //        DataTable dtAta = General.IncarcaDT($@"SELECT * FROM ""Admin_Medicina"" WHERE ""IdAuto""={id}", null);
                                //        if (dtAta.Rows.Count > 0)
                                //            scrieDoc((dtAta.Rows[0]["FisierExtensie"] ?? "").ToString(), (byte[])dtAta.Rows[0]["Fisier"], (dtAta.Rows[0]["FisierNume"] ?? "").ToString());
                                //        else
                                //            Response.Write("Nu exista date de afisat !");

                                //        return;
                                //    }
                                //case "6":   //Florin 2019.12.07 (se citeste din tabela Admin_Sanctiuni)
                                //    {
                                //        DataTable dtAta = General.IncarcaDT($@"SELECT * FROM ""Admin_Sanctiuni"" WHERE ""IdAuto""={id}", null);
                                //        if (dtAta.Rows.Count > 0)
                                //            scrieDoc((dtAta.Rows[0]["FisierExtensie"] ?? "").ToString(), (byte[])dtAta.Rows[0]["Fisier"], (dtAta.Rows[0]["FisierNume"] ?? "").ToString());
                                //        else
                                //            Response.Write("Nu exista date de afisat !");

                                //        return;
                                //    }
                        }

                        DataTable dt = General.IncarcaDT($@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""='{tbl}' AND ""Id""={id} AND ""EsteCerere""={tip}", null);
                        if (dt.Rows.Count != 0)
                        {
                            scrieDoc((dt.Rows[0]["FisierExtensie"] ?? "").ToString(), (byte[])dt.Rows[0]["Fisier"], (dt.Rows[0]["FisierNume"] ?? "").ToString());
                        }
                        else
                        {
                            Response.Write("Nu exista date de afisat !");
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
            Response.End();
            Response.Flush();
            System.Threading.Thread.Sleep(1000);
        }


    }
}