using DevExpress.XtraPrinting;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Hosting;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class AdeverintaGenerala : DevExpress.XtraReports.UI.XtraReport
    {
        public AdeverintaGenerala()
        {
            InitializeComponent();

            this.PrintingSystem.SetCommandVisibility(PrintingSystemCommand.DocumentMap, CommandVisibility.None);
            this.PrintingSystem.SetCommandVisibility(new PrintingSystemCommand[] {
        PrintingSystemCommand.ExportCsv, PrintingSystemCommand.ExportTxt, PrintingSystemCommand.ExportDocx,
        PrintingSystemCommand.ExportHtm, PrintingSystemCommand.ExportMht, PrintingSystemCommand.ExportPdf,
        PrintingSystemCommand.ExportRtf, PrintingSystemCommand.ExportXls, PrintingSystemCommand.ExportXlsx,
        PrintingSystemCommand.ExportGraphic },
                CommandVisibility.None);
            this.PrintingSystem.SetCommandVisibility(new PrintingSystemCommand[] {
        PrintingSystemCommand.ExportCsv, PrintingSystemCommand.ExportTxt },
                CommandVisibility.All);

        }

        private void AdeverintaGenerala_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            //HttpContext.Current.Session["User_Marca"] = "100009";

            if (HttpContext.Current.Session["User_Marca"] == null || Convert.ToInt32(HttpContext.Current.Session["User_Marca"].ToString()) < 0)
                return;

            string sql = "";
            string op = "";
            if (Constante.tipBD == 1)
                sql = "SELECT F00204, F00207, F00281, F00282, 'Str. ' + F00233 + ', nr. ' + F00234 + ', ' + F00231 AS \"AdresaAngajator\", "
                                    + "F00241 + '/' + CONVERT(VARCHAR, F00242) + '/' + CONVERT(VARCHAR, F00243) AS \"NrOrdine\" FROM F002 WHERE F00202 = (SELECT F10002 FROM F100 WHERE F10003 = {0})";
            else
                sql = "SELECT F00204, F00207, F00281, F00282, 'Str. ' || F00233 || ', nr. ' || F00234 || ', ' || F00231 AS \"AdresaAngajator\", "
                                    + "F00241 || '/' || F00242 || '/' || F00243 AS \"NrOrdine\" FROM F002 WHERE F00202 = (SELECT F10002 FROM F100 WHERE F10003 = {0})";
            sql = string.Format(sql, HttpContext.Current.Session["User_Marca"].ToString());
            DataTable dt = General.IncarcaDT(sql, null);

            int nrInreg = Convert.ToInt32(HttpContext.Current.Session["NrInregAdev"].ToString());
            xrLabel16.Text = "Nr. înregistrare: " + nrInreg;
            xrLabel12.Text = "Data: " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "." + DateTime.Now.Month.ToString().PadLeft(2, '0') + "." + DateTime.Now.Year;
            
            this.DataSource = dt;

           if (Constante.tipBD == 2)
            {
                sql = "SELECT F10008 || ' ' || F10009 AS NUME, F10052, F100521, TO_CHAR(F100522, 'dd/mm/yyyy') AS F100522, TO_CHAR(F10022, 'dd/mm/yyyy') AS F10022, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, F10047 AS SEX, F100983, F100984 AS TIPCTR, F1009741 FROM F100 WHERE F10003=" + HttpContext.Current.Session["User_Marca"].ToString();
            }
            else
            {
                sql = "SELECT  F10008 + ' ' + F10009 AS NUME, F10052, F100521, CONVERT(VARCHAR, F100522, 103) AS F100522, CONVERT(VARCHAR, F10022, 103) AS F10022, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, F10047 AS SEX, F100983, F100984 AS TIPCTR, F1009741 FROM F100 WHERE F10003=" + HttpContext.Current.Session["User_Marca"].ToString();
            }
            DataTable dtAng = General.IncarcaDT(sql, null);

            string text = "                    Prin prezenta {0}, cu sediul în {1}, CUI R{2}, Nr. de ordine în registrul comerţului {3} adevereşte că {4} {5}, {6} în localitatea {18}, {7}, județ/sector {19}, {8} {9} seria {10} {11} de {12} în data de {13} CNP {14} este {15} societăţii noastre în calitate de salariat cu contract individual de muncă pe perioadă {16}, din data de {17}";
            string text2 = "";

            text2 = ((dtAng.Rows[0]["F10083"].ToString().Length == 0 || dtAng.Rows[0]["F10083"].ToString() == "0") ? "" : " str. " + dtAng.Rows[0]["F10083"].ToString());
            text2 += ((dtAng.Rows[0]["F10084"].ToString().Length == 0 || dtAng.Rows[0]["F10084"].ToString() == "0") ? "" : ", nr. " + dtAng.Rows[0]["F10084"].ToString());
            text2 += ((dtAng.Rows[0]["F10085"].ToString().Length == 0 || dtAng.Rows[0]["F10085"].ToString() == "0") ? "" : ", bl. " + dtAng.Rows[0]["F10085"].ToString());
            text2 += ((dtAng.Rows[0]["F100892"].ToString().Length == 0 || dtAng.Rows[0]["F100892"].ToString() == "0") ? "" : ", sc. " + dtAng.Rows[0]["F100892"].ToString());
            text2 += ((dtAng.Rows[0]["F100893"].ToString().Length == 0 || dtAng.Rows[0]["F100893"].ToString() == "0") ? "" : ", et. " + dtAng.Rows[0]["F100893"].ToString());
            text2 += ((dtAng.Rows[0]["F10086"].ToString().Length == 0 || dtAng.Rows[0]["F10086"].ToString() == "0") ? "" : ", ap. " + dtAng.Rows[0]["F10086"].ToString());

            text = string.Format(text, dt.Rows[0]["F00204"].ToString(), dt.Rows[0]["AdresaAngajator"].ToString(), dt.Rows[0]["F00207"].ToString(), dt.Rows[0]["NrOrdine"].ToString(), 
                                (dtAng.Rows[0]["SEX"].ToString() == "1" ? "domnul" : "doamna"), dtAng.Rows[0]["NUME"].ToString(), (dtAng.Rows[0]["SEX"].ToString() == "1" ? "domiciliat" : "domiciliată"), 
                                text2, (dtAng.Rows[0]["SEX"].ToString() == "1" ? "posesor al" : "posesoare a"), (dtAng.Rows[0]["F100983"].ToString() == "1" ? "cărţii de identitate" : "buletinului de identitate"),
                                dtAng.Rows[0]["F10052"].ToString(), (dtAng.Rows[0]["F100983"].ToString() == "1" ? "eliberată de" : "eliberat de"), dtAng.Rows[0]["F100521"].ToString(), dtAng.Rows[0]["F100522"].ToString(), dtAng.Rows[0]["F10017"].ToString(), (dtAng.Rows[0]["SEX"].ToString() == "1" ? "angajatul" : "angajata"),
                                 (dtAng.Rows[0]["F1009741"].ToString() == "1" ? "nedeterminată" : "determinată"), dtAng.Rows[0]["F10022"].ToString(), dtAng.Rows[0]["F10081"].ToString(), (dtAng.Rows[0]["F100891"] == null || dtAng.Rows[0]["F100891"].ToString().Length < 0 ? dtAng.Rows[0]["F10082"].ToString() : dtAng.Rows[0]["F100891"].ToString()));
            
           
            string text3 = "";
            switch (HttpContext.Current.Session["TactilAdeverinte"].ToString())
            {
                case "Angajat":
                    text3 = "S-a eliberat prezenta la cerere";
                    break;
                case "Practica":
                    text3 = "S-a eliberat prezenta pentru a-i servi la facultate/școală.";
                    break;
                case "Gradinita":
                    text3 = "S-a eliberat prezenta pentru a-i servi la creșă/grădiniță.";
                    break;
            }

            xrLabel14.Text = text;
            xrLabel15.Text = text3;

            

            if (Directory.Exists(HostingEnvironment.MapPath("~/Fisiere/Imagini/Clienti/" + HttpContext.Current.Session["IdClient"].ToString())))
            {
                DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Fisiere/Imagini/Clienti/" + HttpContext.Current.Session["IdClient"].ToString()));
                FileInfo[] listfiles = root.GetFiles("Logo.*");
                string logo = "";
                if (listfiles.Length > 0)
                {
                    string path = root.FullName + "/" + listfiles[0].Name;
                    xrPictureBox1.Image = Image.FromFile(path);

                }

                FileInfo[] listfilesSub = root.GetFiles("Subsol.*");
                logo = "";
                if (listfilesSub.Length > 0)
                {
                    string path = root.FullName + "/" + listfilesSub[0].Name;
                    xrPictureBox2.Image = Image.FromFile(path);

                }

            }


        }
    }
}
