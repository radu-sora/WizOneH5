using System;
using System.Drawing;
using System.Data;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using WizOne.Module;
using DevExpress.XtraPrinting;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace WizOne.Reports
{
    public partial class PontajDinamic : DevExpress.XtraReports.UI.XtraReport
    {
        public PontajDinamic()
        {
            CultureInfo newCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            newCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;

            InitializeComponent();
            IncarcaDate();

            //report.ExportOptions.Xls.TextExportMode = DevExpress.XtraPrinting.TextExportMode.Text;
            this.ExportOptions.Xls.TextExportMode = DevExpress.XtraPrinting.TextExportMode.Value;
            this.ExportOptions.Xlsx.TextExportMode = DevExpress.XtraPrinting.TextExportMode.Value;

        }

        private void IncarcaDate()
        {
            try
            {
                int an = DateTime.Now.Year;
                int luna = DateTime.Now.Month;
                string struc = ";";

                string str = (HttpContext.Current.Session["PrintParametrii"] ?? "").ToString();
                if (str != "")
                {
                    string[] arr = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length > 0 && arr[0] != "") luna = Convert.ToInt32(arr[0]);
                    if (arr.Length > 1 && arr[1] != "") an = Convert.ToInt32(arr[1]);
                    if (arr.Length > 2 && arr[2] != "") struc = arr[2];
                }

                lblPerioada.Text = Dami.NumeLuna(luna) + " " + an.ToString();

                DataTable dt = new DataTable();
                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 22)
                {
                    try
                    {
                        dt = General.IncarcaDT(General.Nz(HttpContext.Current.Session["PrintDnata"],"").ToString(), null);
                    }
                    catch (Exception){}
                }
                else
                    dt = HttpContext.Current.Session["InformatiaCurenta"] as DataTable;

                int margine = 0;
                float pozX = margine;
                int x = 1;
                bool act = false;

                string sql = "SELECT * FROM \"Ptj_tblPrint\" WHERE \"Activ\" = 1 ORDER BY \"Ordine\"";
                DataTable dtPrint = General.IncarcaDT(sql, null);

                string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + an;
                if (Constante.tipBD == 2)
                    strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + an;
                DataTable dtHolidays = General.IncarcaDT(strSql, null);

                if (dtPrint != null)
                {
                    for (int k = 0; k < dtPrint.Rows.Count; k++)
                    {
                        XRLabel lbl = new XRLabel();
                        XRLabel col = new XRLabel();
                        switch (Convert.ToInt32(dtPrint.Rows[k]["Id"].ToString()))
                        {
                            case 1:                          //zilele lunii
                                for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                                {
                                    lbl = CreazaCamp(i.ToString(), pozX, Convert.ToInt32((dtPrint.Rows[k]["Lungime"] ?? 40).ToString()), x, Convert.ToInt32((dtPrint.Rows[k]["Aliniere"] ?? 3).ToString()), Convert.ToInt32((dtPrint.Rows[k]["MarimeText"] ?? 7).ToString()));
                                    TopMargin.Controls.Add(lbl);

                                    col = CreazaCamp("[Ziua" + i + "]", pozX, Convert.ToInt32((dtPrint.Rows[k]["Lungime"] ?? 40).ToString()), x, Convert.ToInt32((dtPrint.Rows[k]["Aliniere"] ?? 3).ToString()), Convert.ToInt32((dtPrint.Rows[k]["MarimeText"] ?? 7).ToString()), 2);
                                    col.NullValueText = "0";
                                    //col.XlsxFormatString = "#,##";
                                    DateTime zi = new DateTime(an, luna, i);
                                    bool ziLibera = false;
                                    for (int z = 0; z < dtHolidays.Rows.Count; z++)
                                        if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
                                        {
                                            ziLibera = true;
                                            break;
                                        }
                                    if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) col.BackColor = Color.FromArgb(217, 243, 253);
                                    Detail.Controls.Add(col);

                                    pozX = pozX + lbl.WidthF;
                                    if (pozX >= this.PageWidth) this.PageWidth = (int)pozX;

                                    x += 1;
                                }
                                break;
                            case 2:                         //data subsol
                                act = true;
                                int parametru = 0;
                                string strZiua = "";
                                try
                                {
                                    parametru = Convert.ToInt32(dtPrint.Rows[k]["Camp"]);
                                }
                                catch { }

                                DateTime dt1 = DateTime.Now;
                                switch (parametru)
                                {
                                    case 0:
                                        //NOP
                                        break;
                                    case 1:
                                        strZiua = dt1.Day.ToString().PadLeft(2, '0') + "." + dt1.Month.ToString().PadLeft(2, '0') + "." + dt1.Year.ToString();
                                        break;
                                    case 2:
                                        dt1.AddDays(1);
                                        strZiua = dt1.Day.ToString().PadLeft(2, '0') + "." + dt1.Month.ToString().PadLeft(2, '0') + "." + dt1.Year.ToString();
                                        break;
                                    case 3:
                                        strZiua = "01." + dt1.Month.ToString().PadLeft(2, '0') + "." + dt1.Year.ToString();
                                        break;
                                    case 4:
                                        strZiua = "01." + Dami.ValoareParam("LunaLucru").PadLeft(2, '0') + "." + Dami.ValoareParam("AnLucru").ToString();
                                        break;
                                }

                                if (lblSemnatura.Text != "") lblSemnatura.Text += "\n\r";
                                lblSemnatura.Text += dtPrint.Rows[k]["TextAfisare"].ToString() + " " + strZiua;
                                lblSemnatura.Font = new Font("Calibri", (float)(Convert.ToInt32((dtPrint.Rows[k]["MarimeText"] ?? 7).ToString())));
                                break;
                            case 3:                     //semnatura
                                act = true;

                                if (lblSemnatura.Text != "") lblSemnatura.Text += "\n\r";
                                lblSemnatura.Text += dtPrint.Rows[k]["Camp"].ToString();
                                lblSemnatura.Font = new Font("Calibri", (float)(Convert.ToInt32((dtPrint.Rows[k]["MarimeText"] ?? 7).ToString())));

                                break;
                            case 4:                     //antet
                                string txt = "";

                                if (struc != "" && struc != ";") txt += struc + "\n\r";
                                string stare = DamiStareFornetii(dt);
                                if (stare != "") txt += "Stare - " + stare + "\n\r";
                                txt += "Nume Manager " + "\n\r";
                                txt += "Nume Operator " + "\n\r";

                                lblAntet.Text = txt;
                                //lblAntet.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                                lblAntet.Font = new Font("Calibri", 10);
                                break;
                            default:                    //restul
                                lbl = CreazaCamp(dtPrint.Rows[k]["TextAfisare"].ToString(), pozX, Convert.ToInt32((dtPrint.Rows[k]["Lungime"] ?? 40).ToString()), x, Convert.ToInt32((dtPrint.Rows[k]["Aliniere"] ?? 3).ToString()), Convert.ToInt32((dtPrint.Rows[k]["MarimeText"] ?? 7).ToString()));
                                TopMargin.Controls.Add(lbl);

                                col = CreazaCamp("[" + dtPrint.Rows[k]["Camp"].ToString() + "]", pozX, Convert.ToInt32((dtPrint.Rows[k]["Lungime"] ?? 40).ToString()), x, Convert.ToInt32((dtPrint.Rows[k]["Aliniere"] ?? 3).ToString()), Convert.ToInt32((dtPrint.Rows[k]["MarimeText"] ?? 7).ToString()), 2);
                                Detail.Controls.Add(col);

                                string cmp = dtPrint.Rows[k]["Camp"].ToString();
                                if ((cmp.Length == 2 || cmp.Length == 3) && cmp.Substring(0,1) == "F" && General.IsNumeric(cmp.Replace("F","")))
                                {
                                    //col.Text = "0";
                                    //col.DataBindings.Add("Text", this.DataSource, cmp);
                                    col.DataBindings.Add("Text", this.DataSource, cmp + "_Tmp");
                                    col.DataBindings["Text"].FormatString = "{0:######}";
                                    col.NullValueText = "0";
                                    col.XlsxFormatString = "#,##0";
                                }

                                col.WordWrap = false;
                                col.TextAlignment = TextAlignment.MiddleLeft;

                                pozX = pozX + lbl.WidthF;
                                if (pozX >= this.PageWidth) this.PageWidth = (int)pozX;

                                x += 1;
                                break;
                        }
                    }
                }

                if (act)
                {
                    lblSemnatura.LocationF = new PointF(0, 100);
                    lblSemnatura.HeightF += 15;                
                }


                this.PageWidth += margine;
                lblPagina.WidthF = this.PageWidth;
                lblSemnatura.WidthF = this.PageWidth; 
                lblTitlu.WidthF = this.PageWidth;
                lblPerioada.WidthF = this.PageWidth;
            
                this.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "PontajDinamic", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private static XRLabel CreazaCamp(string txt, float pozX, int lungime, int x, int aliniere, int fontSize, int tip = 1)
        {
            XRLabel lbl = new XRLabel();

            try
            {
                int pozY = 0;
                if (tip == 1) pozY = 100;

                lbl.Text = txt;
                lbl.Font = new Font("Calibri", (float)fontSize);
                lbl.WidthF = lungime;
                lbl.HeightF = 18;
                lbl.LocationF = new PointF(pozX, pozY);
                if (tip == 1) lbl.BackColor = Color.FromArgb(217, 243, 253);
                switch (aliniere)
                {
                    case 1:
                        lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
                        break;
                    case 2:
                        lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
                        break;
                    case 3:
                        lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                        break;
                    default:
                        lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                        break;
                }

                if (x == 1)
                {
                    if (tip == 1)
                        lbl.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) | DevExpress.XtraPrinting.BorderSide.Right) | DevExpress.XtraPrinting.BorderSide.Bottom)));
                }
                else
                {
                    if (tip == 1)
                        lbl.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right) | DevExpress.XtraPrinting.BorderSide.Bottom)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "PontajDinamic", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lbl;
        }

        private string DamiStareFornetii(DataTable dt)
        {
            string stare = "";

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    int[] listaStari = new int[7];
                    for (int k = 0; k < dt.Rows.Count; k++)
                        listaStari[Convert.ToInt32(dt.Rows[k]["IdStare"].ToString())]++;

                    if (listaStari[1] > 0 && listaStari[2] == 0 && listaStari[3] == 0 && listaStari[4] == 0 && listaStari[5] == 0 && listaStari[6] == 0)
                    {
                        DataTable dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblStariPontaj\" WHERE \"Id\" = 1", null);
                        stare = dtTmp.Rows[0]["Denumire"].ToString();
                    }
                    if ((listaStari[1] > 0 && (listaStari[2] > 0 || listaStari[3] > 0 || listaStari[4] > 0 || listaStari[5] > 0 || listaStari[6] > 0))
                        || ((listaStari[2] > 0 || listaStari[4] > 0)))
                    {
                        DataTable dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblStariPontaj\" WHERE \"Id\" = 2", null);
                        stare = dtTmp.Rows[0]["Denumire"].ToString();
                    }
                    if ((listaStari[1] == 0 && listaStari[2] == 0 && listaStari[4] == 0) && (listaStari[3] > 0 || listaStari[6] > 0))
                    {
                        DataTable dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblStariPontaj\" WHERE \"Id\" = 3", null);
                        stare = dtTmp.Rows[0]["Denumire"].ToString();
                    }
                    if (listaStari[4] > 0 && listaStari[1] == 0 && listaStari[2] == 0 && listaStari[3] == 0 && listaStari[5] == 0 && listaStari[6] == 0)
                    {
                        DataTable dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblStariPontaj\" WHERE \"Id\" = 4", null);
                        stare = dtTmp.Rows[0]["Denumire"].ToString();
                    }
                    if (listaStari[5] > 0 && listaStari[1] == 0 && listaStari[2] == 0 && listaStari[3] == 0 && listaStari[4] == 0 && listaStari[6] == 0)
                    {
                        DataTable dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblStariPontaj\" WHERE \"Id\" = 5", null);
                        stare = dtTmp.Rows[0]["Denumire"].ToString();
                    }
                    if (listaStari[6] > 0 && listaStari[1] == 0 && listaStari[2] == 0 && listaStari[3] == 0 && listaStari[4] == 0 && listaStari[5] == 0)
                    {
                        DataTable dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblStariPontaj\" WHERE \"Id\" = 6", null);
                        stare = dtTmp.Rows[0]["Denumire"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "PontajDinamic", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return stare;
        }


    }
}
