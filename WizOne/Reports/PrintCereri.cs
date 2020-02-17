using System;
using System.Web;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WizOne.Module;
using System.Data;
using System.IO;
using System.Web.Hosting;

namespace WizOne.Reports
{
    public partial class PrintCereri : DevExpress.XtraReports.UI.XtraReport
    {
        public PrintCereri()
        {
            InitializeComponent();


            DataTable dt = General.IncarcaDT($@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""='Ptj_Cereri' AND ""Id""={(HttpContext.Current.Session["IdCerereTactil"] ?? "0")} AND ""EsteCerere""=1", null);
            if (dt.Rows.Count != 0)
            {
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Temp/CereriTactil/"));
                if (!folder.Exists)
                    folder.Create();

                foreach (FileInfo file in folder.GetFiles())
                    file.Delete();


                string fisier = "CereriTactil_" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_"
                                                + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + ".html";
                string FileName = HostingEnvironment.MapPath("~/Temp/CereriTactil/") + fisier;

                BinaryWriter bw;

                try
                {
                    bw = new BinaryWriter(new FileStream(FileName, FileMode.Create));

                    bw.Write((byte[])dt.Rows[0]["Fisier"]);
                }
                catch (IOException)
                {
                    return;
                }
                bw.Close();

                xrRichText1.LoadFile(FileName);

            }
        }

    }
}
