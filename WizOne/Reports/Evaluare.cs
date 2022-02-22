using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Hosting;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class Evaluare : DevExpress.XtraReports.UI.XtraReport
    {
        int spatiuVert = 10;
        int pozY = 0;
        int user_id = 1;
        int idClient = 1;
        public Evaluare()
        {
            InitializeComponent();
        }


        private void Evaluare_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                int idQuiz = -99, f10003 = -99, finalizat = 0;
                string super = "";
                //int idQuiz = Convert.ToInt32(this.Parameters["idQuiz"].Value);
                //int f10003 = Convert.ToInt32(this.Parameters["f10003"].Value);
                //user_id = Convert.ToInt32(this.Parameters["idUser"].Value);
                //string super = this.Parameters["super"].Value.ToString();
                //int finalizat = Convert.ToInt32(this.Parameters["finalizat"].Value);

                //DefaultPrinterSettingsUsing.UseLandscape = false;
                //this.Landscape = true;

                if (Directory.Exists(HostingEnvironment.MapPath("~/Fisiere/Imagini/Clienti/" + HttpContext.Current.Session["IdClient"].ToString())))
                {
                    DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Fisiere/Imagini/Clienti/" + HttpContext.Current.Session["IdClient"].ToString()));
                    FileInfo[] listfiles = root.GetFiles("Logo.*");
                    if (listfiles.Length > 0)
                    {
                        string path = root.FullName + "/" + listfiles[0].Name;
                        xrPictureBox1.Image = Image.FromFile(path);
                    }
                }
                string str = (HttpContext.Current.Session["PrintParametrii"] ?? "").ToString();
                if (str != "")
                {
                    string[] arr = str.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length > 0 && arr[0] != "") idQuiz = Convert.ToInt32(arr[0]);
                    if (arr.Length > 1 && arr[1] != "") f10003 = Convert.ToInt32(arr[1]);
                    if (arr.Length > 2 && arr[2] != "") user_id = Convert.ToInt32(arr[2]);
                    if (arr.Length > 3 && arr[3] != "") super = arr[3];
                    if (arr.Length > 4 && arr[4] != "") finalizat = Convert.ToInt32(arr[4]);
                }

                string sql = "SELECT MAX(\"Pozitie\") FROM \"Eval_RaspunsIstoric\" WHERE \"IdQuiz\" = " + idQuiz + " AND F10003 = " + f10003;
                DataTable dtSuper = General.IncarcaDT(sql, null);
                if (finalizat == 1)
                {
                    super = "Super1";
                    if (dtSuper != null && dtSuper.Rows.Count > 0 && dtSuper.Rows[0][0].ToString().Length > 0)
                        super = "Super" + dtSuper.Rows[0][0].ToString();
                }

                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'IdClient'";
                DataTable dtClient = General.IncarcaDT(sql, null);
                if (dtClient != null && dtClient.Rows.Count > 0 && dtClient.Rows[0][0] != null && dtClient.Rows[0][0].ToString().Length > 0)
                    idClient = Convert.ToInt32(dtClient.Rows[0][0].ToString());

                //titlul raportului
                string titlu = "";
                sql = "SELECT * FROM \"Eval_Quiz\" WHERE \"Id\" = " + idQuiz;
                DataTable dtTit = General.IncarcaDT(sql, null);
                if (dtTit != null && dtTit.Rows.Count > 0 && dtTit.Rows[0]["Titlu"] != null && dtTit.Rows[0]["Titlu"].ToString().Length > 0) titlu = dtTit.Rows[0]["Titlu"].ToString();
                lblTitlu.Text = titlu;
                string culoareTitlu = Dami.ValoareParam("CuloareRaportTitlu");
                if (culoareTitlu != "")
                    lblTitlu.BackColor = General.Culoare(culoareTitlu);

                sql = "SELECT * FROM \"Eval_QuizIntrebari\" WHERE \"IdQuiz\" = " + idQuiz + " ORDER BY \"OrdineAfisare\" ";
                DataTable dtIntr = General.IncarcaDT(sql, null);
                sql = "SELECT * FROM \"Eval_RaspunsLinii\" WHERE \"IdQuiz\" = " + idQuiz + " AND F10003 = " + f10003;
                DataTable dtRasp = General.IncarcaDT(sql, null);


                string sqlEval_ObiIndividuale = @"select * from ""Eval_ObiIndividualeTemp"" where ""IdQuiz"" = " + idQuiz;
                DataTable dtObiIndividuale = General.IncarcaDT(sqlEval_ObiIndividuale, null);


                string sqlEval_CompetenteAngajat = @"select * from ""Eval_CompetenteAngajatTemp"" where ""IdQuiz"" = " + idQuiz;
                DataTable dtCompetenteAngajat = General.IncarcaDT(sqlEval_CompetenteAngajat, null);

                sql = "select Alias, \"ColumnName\", \"Width\", \"Eval_QuizIntrebari\".\"Id\" AS \"Id\", \"IdNomenclator\" from \"Eval_ConfigObTemplateDetail\" left join \"Eval_QuizIntrebari\" on \"TemplateId\" = \"TemplateIdObiectiv\"  where \"IdQuiz\" = " + idQuiz + " and \"Vizibil\" = 1 ORDER BY Eval_ConfigObTemplateDetail.Ordine";
                DataTable dtTemplateOb = General.IncarcaDT(sql, null);

                sql = "select Alias, \"ColumnName\", \"Width\", \"Eval_QuizIntrebari\".\"Id\" AS \"Id\", \"IdNomenclator\" from \"Eval_ConfigCompTemplateDetail\" left join \"Eval_QuizIntrebari\" on \"TemplateId\" = \"TemplateIdCompetenta\"  where \"IdQuiz\" = " + idQuiz + " and \"Vizibil\" = 1 ORDER BY Eval_ConfigCompTemplateDetail.Ordine";
                DataTable dtTemplateComp = General.IncarcaDT(sql, null);

                int idRoot = -99;

                DataRow[] drIntr = dtIntr.Select("Parinte = 0 AND Descriere = 'Root'");
                if (drIntr != null && drIntr[0]["Id"] != null && drIntr[0]["Id"].ToString().Length > 0)
                    idRoot = Convert.ToInt32(drIntr[0]["Id"].ToString());

                DataRow[] drSec = dtIntr.Select("Parinte = " + idRoot + " AND EsteSectiune = 1", "OrdineAfisare");
                if (drSec != null && drSec.Length > 0)
                    for (int i = 0; i < drSec.Length; i++)
                    {
                        //titlul sectiunii
                        XRLabel lblSec = CreeazaTitlu((drSec[i]["Descriere"] != DBNull.Value ? drSec[i]["Descriere"].ToString() : ""));
                        lblSec.SizeF = new System.Drawing.SizeF(720F, 20F);
                        lblSec.LocationF = new PointF(0, pozY);
                        Detail.Controls.Add(lblSec);
                        pozY += (int)lblSec.HeightF + spatiuVert;

                        string idParinte = "-" + (drSec[i]["Id"] != DBNull.Value ? drSec[i]["Id"].ToString() : "") + "-";
                        DataRow[] arrIntre = dtIntr.Select("Ordine IS NOT NULL AND Id <> " + (drSec[i]["Id"] != DBNull.Value ? drSec[i]["Id"].ToString() : "") + " AND Ordine LIKE '%" + idParinte + "%'", "OrdineAfisare");
                        if (arrIntre != null && arrIntre.Length > 0)
                            for (int j = 0; j < arrIntre.Length; j++)
                            {
                                XRControl ctl = null, ctl2 = null;

                                switch (Convert.ToInt32((arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0")))
                                {
                                    case 1:                     //TextSimplu
                                    case 2:                     //Lista derulanta
                                    case 3:                     //Butoane radio
                                    case 4:                     //CheckBox
                                    case 8:                     //Text memo
                                    case 10:                     //Data
                                    case 11:                     //Numeric
                                    case 13:                     //Nume Complet
                                    case 14:                     //Structura organizatorica
                                    case 15:                     //Post
                                    case 18:                     //Nume Manager
                                    case 19:                     //Post Manager
                                    case 20:                     //Nume Manager N+2
                                    case 21:                     //Post Manager N+2
                                    case 31:                     //Ziua curenta
                                    case 33:                     //Functia
                                    case 34:                     //Directia
                                    case 35:                     //Dept din F1001
                                    case 36:                     //Data finalizare
                                    case 37:                     //Perioada
                                    case 38:                     //Categ OMN
                                    case 39:                     //Departament
                                    case 47:                     //Nume Complet (prenume nume)
                                    case 48:                     //Manager Nume (prenume nume)
                                    case 52:                     //Text memo readOnly
                                        {
                                            int tipData = Convert.ToInt32((arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0"));
                                            ctl = CreeazaEticheta(DaMiRaspuns(dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"))[0], super), (tipData == 8 ? 0 : 1), (tipData == 8 ? true : false));
                                        }
                                        break;
                                    case 5:                     //Competente
                                        {
                                            DataRow[] dtColComp = dtTemplateComp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColComp != null && dtColComp.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColComp.Length; k++)
                                                {
                                                    cols.Add(dtColComp[k]["Alias"] == DBNull.Value ? dtColComp[k]["ColumnName"].ToString() : dtColComp[k]["Alias"].ToString());
                                                    if (dtColComp[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColComp[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColComp[k]["IdNomenclator"].ToString()));
                                                }
                                                ctl = CreeazaTabel(5, dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());

                                                //Radu 15.02.2022
                                                int TemplateIdCompetenta = Convert.ToInt32(arrIntre[j]["TemplateIdCompetenta"] != DBNull.Value ? arrIntre[j]["TemplateIdCompetenta"].ToString() : "0");
                                                sql = "SELECT * FROM Eval_ConfigCompTemplateDetail WHERE TemplateId = " + TemplateIdCompetenta + " AND TotalColoana IS NOT NULL";
                                                DataTable dtTemplComp = General.IncarcaDT(sql, null);
                                                if (dtTemplComp != null && dtTemplComp.Rows.Count > 0 && dtTemplComp.Rows[0]["TotalColoana"] != DBNull.Value && dtTemplComp.Rows[0]["TotalColoana"].ToString().Length > 0 &&
                                                    dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)) != null &&
                                                    dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Rows.Count > 0)
                                                {
                                                    string eticheta = "";
                                                    switch (Convert.ToInt32(dtTemplComp.Rows[0]["TotalColoana"].ToString()))
                                                    {
                                                        case 1:
                                                            int suma1 = Convert.ToInt32(dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Compute("SUM(Total1)", "[Total1] IS NOT NULL"));
                                                            eticheta = "Suma {0:N0}";
                                                            eticheta = string.Format(eticheta, suma1);
                                                            break;
                                                        case 2:
                                                            decimal suma2 = Convert.ToDecimal(dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Compute("SUM(Total1)", "[Total1] IS NOT NULL"));
                                                            eticheta = "Suma {0:N2}";
                                                            eticheta = string.Format(eticheta, suma2);
                                                            break;
                                                        case 3:
                                                            int media1 = Convert.ToInt32(dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Compute("AVG(Total1)", string.Empty));
                                                            eticheta = "Media {0:N0}";
                                                            eticheta = string.Format(eticheta, media1);
                                                            break;
                                                        case 4:
                                                            decimal media2 = Convert.ToDecimal(dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Compute("AVG(Total1)", string.Empty));
                                                            eticheta = "Media {0:N2}";
                                                            eticheta = string.Format(eticheta, media2);
                                                            break;
                                                        case 5:
                                                            decimal valMin = Convert.ToDecimal(dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Compute("MIN(Total1)", "[Total1] IS NOT NULL"));
                                                            eticheta = "Val. min. {0}";
                                                            eticheta = string.Format(eticheta, valMin);                                                            
                                                            break;
                                                        case 6:
                                                            decimal valMax = Convert.ToDecimal(dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)).CopyToDataTable().Compute("MIN(Total1)", "[Total1] IS NOT NULL"));
                                                            eticheta = "Val. max. {0}";
                                                            eticheta = string.Format(eticheta, valMax);
                                                            break;
                                                    }

                                                    ctl2 = CreeazaEticheta(eticheta);
                                                }
                                            }
                                        }
                                        break;
                                    case 6:                     //Obiective anterioare
                                        {
                                            DataRow[] dtColOb = dtTemplateOb.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColOb != null && dtColOb.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColOb.Length; k++)
                                                {
                                                    cols.Add(dtColOb[k]["Alias"] == DBNull.Value ? dtColOb[k]["ColumnName"].ToString() : dtColOb[k]["Alias"].ToString());
                                                    if (dtColOb[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColOb[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColOb[k]["IdNomenclator"].ToString()));
                                                }

                                                ctl = CreeazaTabel(6, dtObiIndividuale.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());
                                            }
                                        }
                                        break;
                                    case 12:                     //Obiective viitoare
                                        {
                                            DataRow[] dtColOb = dtTemplateOb.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColOb != null && dtColOb.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColOb.Length; k++)
                                                {
                                                    cols.Add(dtColOb[k]["Alias"] == DBNull.Value ? dtColOb[k]["ColumnName"].ToString() : dtColOb[k]["Alias"].ToString());
                                                    if (dtColOb[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColOb[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColOb[k]["IdNomenclator"].ToString()));
                                                }

                                                ctl = CreeazaTabel(12, dtObiIndividuale.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());
                                            }
                                        }
                                        break;
                                    case 7:                     //Obiective firma
                                        break;
                                    case 9:                     //Eticheta
                                        ctl = CreeazaEticheta((arrIntre[j]["Descriere"] != DBNull.Value ? arrIntre[j]["Descriere"].ToString() : ""));
                                        break;
                                    case 16:                     //Rating
                                        break;
                                    case 17:                     //Tabel
                                        break;
                                    case 22:                     //Plan de dezvoltare
                                        {
                                            string[] cols = { "Obiective", "Modalitati/Actiuni", "Rezultate asteptate" };
                                            string[] arr = { "Descriere", super, super + "_1" };
                                            int[] rat = { 0, 0, 0 };
                                            int[] latime = { 150, 150, 300 };

                                            ctl = CreeazaTabel(22, dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0")), cols, arr, rat, latime);
                                        }
                                        break;
                                    case 23:                     //Obiective angajat
                                        {
                                            DataRow[] dtColOb = dtTemplateOb.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColOb != null && dtColOb.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColOb.Length; k++)
                                                {
                                                    cols.Add(dtColOb[k]["Alias"] == DBNull.Value ? dtColOb[k]["ColumnName"].ToString() : dtColOb[k]["Alias"].ToString());
                                                    if (idClient != 25 && dtColOb[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColOb[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColOb[k]["IdNomenclator"].ToString()));
                                                }

                                                ctl = CreeazaTabel(23, dtObiIndividuale.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());
                                            }
                                        }
                                        break;
                                    case 27:                     //Obiective anterioare fara rating
                                        {
                                            DataRow[] dtColOb = dtTemplateOb.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColOb != null && dtColOb.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColOb.Length; k++)
                                                {
                                                    cols.Add(dtColOb[k]["Alias"] == DBNull.Value ? dtColOb[k]["ColumnName"].ToString() : dtColOb[k]["Alias"].ToString());
                                                    if (dtColOb[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColOb[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColOb[k]["IdNomenclator"].ToString()));
                                                }

                                                ctl = CreeazaTabel(6, dtObiIndividuale.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());
                                            }
                                        }
                                        break;
                                    case 28:                     //Competente eval. intermediara
                                        {
                                            DataRow[] dtColComp = dtTemplateComp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColComp != null && dtColComp.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColComp.Length; k++)
                                                {
                                                    cols.Add(dtColComp[k]["Alias"] == DBNull.Value ? dtColComp[k]["ColumnName"].ToString() : dtColComp[k]["Alias"].ToString());
                                                    if (dtColComp[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColComp[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColComp[k]["IdNomenclator"].ToString()));
                                                }
                                                ctl = CreeazaTabel(5, dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());

                                            }
                                        }
                                        break;
                                    case 29:                     //Obiective anterioare fara rating + Status
                                        {
                                            DataRow[] dtColOb = dtTemplateOb.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColOb != null && dtColOb.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColOb.Length; k++)
                                                {
                                                    cols.Add(dtColOb[k]["Alias"] == DBNull.Value ? dtColOb[k]["ColumnName"].ToString() : dtColOb[k]["Alias"].ToString());
                                                    if (dtColOb[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColOb[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColOb[k]["IdNomenclator"].ToString()));
                                                }

                                                ctl = CreeazaTabel(6, dtObiIndividuale.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1)), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());
                                            }
                                        }
                                        break;
                                    case 30:                     //Competente unificate
                                        {
                                            DataRow[] dtColComp = dtTemplateComp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"));
                                            if (dtColComp != null && dtColComp.Length > 0)
                                            {
                                                List<string> cols = new List<string>();
                                                List<int> rat = new List<int>();
                                                List<int> latime = new List<int>();
                                                List<int> note = new List<int>();

                                                for (int k = 0; k < dtColComp.Length; k++)
                                                {
                                                    cols.Add(dtColComp[k]["Alias"] == DBNull.Value ? dtColComp[k]["ColumnName"].ToString() : dtColComp[k]["Alias"].ToString());
                                                    if (dtColComp[k]["ColumnName"].ToString() == "Calificativ")
                                                        rat.Add(1);
                                                    else
                                                        rat.Add(0);
                                                    latime.Add(Convert.ToInt32(dtColComp[k]["Width"].ToString()));
                                                    note.Add(Convert.ToInt32(dtColComp[k]["IdNomenclator"].ToString()));
                                                }

                                                DataTable dtRaspTemp = GroupBy("IdCategCompetenta", "CategCompetenta", dtCompetenteAngajat);
                                                DataRow[] drRasp = dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1));

                                                if (drRasp != null && drRasp.Length > 0)
                                                    for (int k = 0; k < drRasp.Length; k++)
                                                    {
                                                        ctl = CreeazaTabel(30, dtCompetenteAngajat.Select("F10003 = " + f10003 + " AND IdLinieQuiz = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0") + " AND Pozitie = " + super.Substring(5, 1) + " AND IdCategCompetenta = " + (arrIntre[j]["IdGrup"] != DBNull.Value ? arrIntre[j]["IdGrup"].ToString() : "0")), cols.ToArray(), cols.ToArray(), rat.ToArray(), latime.ToArray(), note.ToArray());
                                                        if (ctl != null) AdaugaControl(ctl, Convert.ToInt32((arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0")), (arrIntre[j]["NumeGrup"] != DBNull.Value ? arrIntre[j]["NumeGrup"].ToString() : ""), 2);
                                                    }

                                            }
                                            ctl = null;
                                        }
                                        break;
                                    case 69:                     //Nota finala
                                        {
                                            decimal nota = Convert.ToDecimal(DaMiRaspuns(dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"))[0], super));
                                            string desc = "";
                                            if (Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.Alka
                                                || Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.ALKA_CO
                                                | Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.ALKA_Trading)
                                            {
                                                if (1 <= nota && nota <= 1.99m) desc = "Necesita imbunatatire rapida";
                                                if (2 <= nota && nota <= 2.99m) desc = "Nesatisfacator/Sub asteptari";
                                                if (3 <= nota && nota <= 3.99m) desc = "Bun/Conform asteptarilor";
                                                if (4 <= nota && nota <= 4.50m) desc = "Foarte bine/ Peste asteptari";
                                                if (4.51m <= nota && nota <= 5) desc = "Excelent";
                                            }

                                            int tipData = Convert.ToInt32((arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0"));
                                            ctl = CreeazaEticheta(nota + " " + desc, (tipData == 8 ? 0 : 1), (tipData == 8 ? true : false));
                                        }
                                        break;
                                }

                                if (ctl != null) AdaugaControl(ctl, Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0"), (arrIntre[j]["Descriere"] != DBNull.Value ? arrIntre[j]["Descriere"].ToString() : ""), Convert.ToInt32(arrIntre[j]["Orientare"] != DBNull.Value ? arrIntre[j]["Orientare"].ToString() : "0"));

                                if (ctl2 != null) AdaugaControl(ctl2, 999, "", 0); //Radu 15.02.2022


                                if (Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0") == 16)
                                {
                                    //rating global propus
                                    XRControl ctlProp = CreeazaEticheta(DaMiRaspuns(dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"))[0], super, 1));
                                    AdaugaControl(ctlProp, Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0"), "Rating global propus: ", 1);
                                    //rating global calculat
                                    XRControl ctlCal = CreeazaEticheta(DaMiRaspuns(dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"))[0], super + "_1", 1));
                                    AdaugaControl(ctlCal, Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0"), "Rating global calculat: ", 1);
                                    //observatii
                                    XRControl ctlObs = CreeazaEticheta(DaMiRaspuns(dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0"))[0], super + "_2"));
                                    AdaugaControl(ctlObs, Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0"), "Observatii: ", 2);
                                }

                                if (Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0") == 5 || Convert.ToInt32(arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0") == 6)
                                {
                                    if (Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.Alka
                                        || Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.ALKA_CO
                                        | Convert.ToInt32(General.Nz(HttpContext.Current.Session["IdClient"], -99)) == (int)IdClienti.Clienti.ALKA_Trading)
                                    {
                                        //adaugam total rating la fiecare grup
                                        int tipCalcul = 1;
                                        if (Convert.ToInt32((arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0")) == 5) tipCalcul = 2;
                                        int valRat = CalculRating(dtRasp.Select("Id = " + (arrIntre[j]["Id"] != DBNull.Value ? arrIntre[j]["Id"].ToString() : "0")), -99, super, tipCalcul);
                                        string descRat = DaMiRating(valRat.ToString());
                                        XRControl ctlRat = CreeazaEticheta(descRat);

                                        if (ctlRat != null) AdaugaControl(ctlRat, Convert.ToInt32((arrIntre[j]["TipData"] != DBNull.Value ? arrIntre[j]["TipData"].ToString() : "0")), "Rating total: ", 1);
                                    }
                                }
                            }
                    }

                DateTime dt = DateTime.Now.Date;
                //la sfarsit de tot punem semnaturile
                string semn = "";
                string op = "+";
                string data = "GETDATE()";
                if (Constante.tipBD == 2)
                {
                    op = "||";
                    data = "SYSDATE";
                }

                sql = " select   a.\"DataAprobare\", case when  b.F10003 is null then b.F70104 else c.F10008 " + op + " ' ' " + op + " c.F10009 end as \"Nume\", a.\"Pozitie\", f.\"Denumire\" AS \"Post\" "
                        + " from \"Eval_RaspunsIstoric\" a "
                        + " join USERS b on a.\"IdUser\" = b.F70102 "
                        + " left join F100 c on b.F10003 = c.F10003 "
                        + " left join \"Org_relPostAngajat\" d on c.F10003 = d.F10003 and d.\"DataInceput\" <= " + data + " AND " + data + " <= d.\"DataSfarsit\" "
                        + " left join \"Org_Posturi\" f on d.\"IdPost\" = f.\"Id\" and f.\"DataInceput\" <= " + data + " and " + data + " <= f.\"DataSfarsit\" "
                        + " Where a.\"IdQuiz\" = " + idQuiz + " AND a.F10003 =  " + f10003 + " ORDER BY a.\"Pozitie\"";

                DataTable dtSem = General.IncarcaDT(sql, null);
                if (dtSem != null && dtSem.Rows.Count > 0)
                    for (int i = 0; i < dtSem.Rows.Count; i++)
                    {
                        semn += (dtSem.Rows[i]["Nume"] != DBNull.Value ? dtSem.Rows[i]["Nume"].ToString() : "") + "   " + (dtSem.Rows[i]["Post"] != DBNull.Value ? dtSem.Rows[i]["Post"].ToString() : "") + "   " + (dtSem.Rows[i]["DataAprobare"] != DBNull.Value ? dtSem.Rows[i]["DataAprobare"].ToString() : "") + "\r\n";
                    }

                //Florin 2020.05.05
                if (dtTit != null && dtTit.Rows.Count > 0 && General.Nz(dtTit.Rows[0]["LuatLaCunostinta"], "0").ToString() == "1")
                {
                    //data luarii la cunostinta
                    string luat = "Angajatul nu a luat la cunostinta evaluarea.";
                    DataTable dtLuat = General.IncarcaDT($@"SELECT * FROM ""Eval_Raspuns"" WHERE ""IdQuiz""={idQuiz} AND F10003={f10003}", null);
                    if (dtLuat != null && dtLuat.Rows.Count > 0)
                    {
                        if (General.Nz(dtLuat.Rows[0]["LuatLaCunostinta"], "0").ToString() == "1")
                            luat = "Angajatul a luat la cunostinta evaluarea in data de " + General.Nz(dtLuat.Rows[0]["LuatData"], "");
                        else if (General.Nz(dtLuat.Rows[0]["LuatLaCunostinta"], "0").ToString() == "2")
                            luat = "Angajatul a contestat evaluarea in data de " + General.Nz(dtLuat.Rows[0]["LuatData"], "");
                        else if (General.Nz(dtLuat.Rows[0]["LuatAutomat"], "0").ToString() == "1") luat = "Proces automat de luare la cunostinta.";
                    }

                    semn += luat;
                }

                XRLabel lblSemn = CreeazaEticheta(semn, 0, true);
                if (lblSemn != null) AdaugaControl(lblSemn, 9, "Semnaturi: ", 1);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private XRLabel CreeazaTitlu(string desc)
        {
            XRLabel lbl = new XRLabel();

            try
            {
                lbl.Text = desc;

                lbl.Font = new Font("Times New Roman", 12F, FontStyle.Underline);
                lbl.WordWrap = true;
                lbl.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 20, 20, 100F);
                lbl.CanGrow = true;
                lbl.CanShrink = true;
                lbl.Text = desc;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lbl;
        }

        private XRLabel CreeazaEticheta(string desc, int esteEticheta = 0, bool multiLine = false)
        {
            XRLabel lbl = new XRLabel();

            try
            {
                lbl.Text = desc;

                if (esteEticheta == 1)
                    lbl.Font = new Font("Times New Roman", 10F, FontStyle.Italic);
                else
                    lbl.Font = new Font("Times New Roman", 10F);
                lbl.WordWrap = true;
                lbl.CanGrow = true;

                lbl.Multiline = multiLine;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lbl;
        }

        private XRTable CreeazaTabel(int tipData, DataRow[] lst, string[] cols, string[] arr, int[] rat, int[] latime, int[] note = null)
        {
            XRTable tbl = new XRTable();

            try
            {
                float rowHeight = 25f;

                tbl.Borders = DevExpress.XtraPrinting.BorderSide.All;

                //adauga capul de tabel
                XRTableRow rowH = new XRTableRow();
                rowH.HeightF = rowHeight;
                rowH.CanGrow = true;
                rowH.CanShrink = true;
                for (int j = 0; j <= cols.Length - 1; j++)
                {
                    XRTableCell cell = new XRTableCell();
                    cell.Text = cols[j];
                    cell.WidthF = latime[j];
                    cell.BackColor = Color.FromArgb(206, 255, 206);
                    string culoareTabel = Dami.ValoareParam("CuloareRaportHeaderTabel");
                    if (culoareTabel != "")
                        cell.BackColor = General.Culoare(culoareTabel);
                    rowH.Cells.Add(cell);
                }
                tbl.Rows.Add(rowH);

                //adauga corpul tabelului
                if (lst != null && lst.Length > 0)
                    for (int i = 0; i < lst.Length; i++)
                    {
                        XRTableRow row = new XRTableRow();
                        row.HeightF = rowHeight;
                        row.CanGrow = true;
                        row.CanShrink = true;
                        for (int j = 0; j <= cols.Length - 1; j++)
                        {
                            XRTableCell cell = new XRTableCell();
                            cell.WidthF = latime[j];
                            cell.Text = DaMiRaspuns(lst[i], arr[j], rat[j], (note != null ? note[j] : -99));
                            row.Cells.Add(cell);
                        }
                        tbl.Rows.Add(row);
                    }

                tbl.AdjustSize();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return tbl;
        }

        private string DaMiRating(string Nota, int nomen = -99, string idCalificativ = "-99")
        {
            string ras = "";

            try
            {
                //string sql = "SELECT * FROM \"Eval_tblRating\" WHERE \"Nota\" = " + Nota;
                //string sql = "SELECT * FROM \"Eval_SetCalificativDet\" WHERE \"IdSet\" = " + nomen + " AND \"Nota\" = " + Nota;
                string sql = "SELECT * FROM \"Eval_SetCalificativDet\" WHERE \"IdSet\" = " + nomen + " AND \"IdCalificativ\" = " + idCalificativ;
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ras = (dt.Rows[0]["Denumire"] != DBNull.Value ? dt.Rows[0]["Denumire"].ToString() : "").ToString();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        private void AdaugaControl(XRControl ctl, int tipData, string descriereEticheta, int orientare)
        {
            try
            {
                if (tipData == 9 || tipData == 999)
                {
                    if (tipData == 9)
                    {
                        ctl.SizeF = new System.Drawing.SizeF(720F, 20F);
                        ctl.LocationF = new PointF(10, pozY);
                        Detail.Controls.Add(ctl);
                        pozY += (int)ctl.HeightF + spatiuVert;
                    }
                    else
                    {//Radu 15.02.2022
                        ctl.SizeF = new System.Drawing.SizeF(720F, 20F);
                        ctl.LocationF = new PointF(660, pozY);
                        Detail.Controls.Add(ctl);
                        pozY += (int)ctl.HeightF + spatiuVert;
                    }
                }
                else
                {
                    if (orientare == 1)             //orientare orizontala
                    {
                        float lt = 150F;
                        float rt = 570f;

                        if (ctl.GetType() == typeof(XRLabel))
                        {
                            XRLabel l = ctl as XRLabel;
                            if (l.Text.Length < 5)
                            {
                                lt = 300f;
                                rt = 420f;
                            }
                        }

                        int iL = 0;
                        if (tipData != 9)           //este eticheta sau rating global
                        {
                            //eticheta
                            if (descriereEticheta.Trim() != "")
                            {
                                XRLabel lbl = CreeazaEticheta(descriereEticheta, 1);
                                lbl.SizeF = new System.Drawing.SizeF(lt, 20F);
                                lbl.LocationF = new PointF(10, pozY);
                                Detail.Controls.Add(lbl);
                                iL = (int)lbl.HeightF;
                            }
                        }

                        ctl.SizeF = new System.Drawing.SizeF(rt, 20F);
                        ctl.LocationF = new PointF(lt + 30, pozY);
                        Detail.Controls.Add(ctl);

                        int iT = (int)ctl.HeightF;
                        if (iL > iT)
                            pozY += iL;
                        else
                            pozY += iT;

                        pozY += spatiuVert;
                    }
                    else                                   //orientare verticala
                    {
                        if (tipData != 9)           //este eticheta sau rating global
                        {
                            //eticheta
                            if (descriereEticheta.Trim() != "")
                            {
                                XRLabel lbl = CreeazaEticheta(descriereEticheta, 1);
                                lbl.SizeF = new System.Drawing.SizeF(720F, 20F);
                                lbl.LocationF = new PointF(10, pozY);
                                Detail.Controls.Add(lbl);
                                pozY += (int)lbl.HeightF + spatiuVert;
                            }
                        }

                        ctl.SizeF = new System.Drawing.SizeF(720F, 20F);
                        ctl.LocationF = new PointF(10, pozY);
                        Detail.Controls.Add(ctl);
                        pozY += (int)ctl.HeightF + spatiuVert;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private int CalculRating(DataRow[] lst, int idGrup, string super, int tipCalcul)
        {
            //1 - se aplica pt obiective si foloseste ponderea pe care utilizatorul o introduce
            //2 - se aplica pt competente si se face media aritmetica

            decimal calc = 0;
            int rasp = 1;
            int nr = 0;

            try
            {
                if (lst != null && lst.Length > 0)
                    for (int i = 0; i < lst.Length; i++)
                    {
                        decimal nota = 0;
                        nota = Convert.ToDecimal((lst[i][super] != DBNull.Value ? lst[i][super].ToString() : "0"));

                        decimal pondere = 0;
                        if (lst[i]["Tinta"] != null && lst[i]["Tinta"].ToString().Length > 0) pondere = Convert.ToDecimal(lst[i]["Tinta"].ToString());

                        if (tipCalcul == 1)
                        {
                            calc += (nota * pondere / 100);
                        }
                        else
                        {
                            nr += 1;
                            calc += nota;
                        }
                    }

                if (tipCalcul == 1)
                    rasp = Convert.ToInt32(Math.Round(calc, MidpointRounding.AwayFromZero));
                else
                    rasp = Convert.ToInt32(Math.Round(calc / nr, MidpointRounding.AwayFromZero));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rasp;
        }

        private string DaMiRaspuns(DataRow entRas, string super, int rat = 0, int nomen = -99)
        {
            string ras = "";

            try
            {
                string descText = "";

                if (entRas != null)
                {
                    descText = (entRas[super] != DBNull.Value ? entRas[super].ToString() : "");
                }

                if (descText.ToLower() == "true") descText = "Da";
                if (descText.ToLower() == "false") descText = "Nu";
                if (rat == 1) descText = DaMiRating(descText, nomen, entRas["IdCalificativ"].ToString());

                if (entRas.Table.Columns.Contains("TipData") && Convert.ToInt32(entRas["TipData"].ToString()) == 3)   //butoane radio
                {//Radu 07.05.2019
                    string sql = "SELECT * FROM \"Eval_tblTipValoriLinii\" WHERE \"Id\" = " + entRas["TipValoare"].ToString() + " AND \"Nota\" = " + descText;
                    DataTable dt = General.IncarcaDT(sql, null);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        descText = (dt.Rows[0]["Valoare"] != DBNull.Value ? dt.Rows[0]["Valoare"].ToString() : descText).ToString();
                    }
                }

                ras = descText;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;
        }

        public DataTable GroupBy(string column1, string column2, DataTable dt)
        {

            DataView dv = new DataView(dt);

            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, new string[] { column1 });

            //adding column for the row count
            dtGroup.Columns.Add("Count", typeof(int));

            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["Count"] = dt.Compute("Count(" + column2 + ")", column1 + " = '" + dr[column1] + "'");
            }

            //returning grouped/counted result
            return dtGroup;
        }



    }
}
