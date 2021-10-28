using System;
using DevExpress.XtraReports.UI;
using System.Linq;
using System.Data;
using System.Web;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class AvsXDec_OrdinDeplasare_Fata : DevExpress.XtraReports.UI.XtraReport
    {

        public AvsXDec_OrdinDeplasare_Fata()
        {
            InitializeComponent();
        }


        private void RaportForm8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int DocumentId = Convert.ToInt32(HttpContext.Current.Session["AvsXDec_IdDocument"].ToString());
            int F10003 = Convert.ToInt32(HttpContext.Current.Session["AvsXDec_Marca"].ToString());

            #region comentate
            //srvBuiltIn ctxBuiltIn = new srvBuiltIn();
            //var entAvans = (from a in ctxBuiltIn.GetVwAvsXDec_Avans()
            //                where a.DocumentId == DocumentId && a.F10003 == F10003
            //                select new
            //                {
            //                    DocumentId = a.DocumentId
            //                }).ToList();
            //if (entAvans.Count != 0)
            //{
            //    var ent = (from a in ctxBuiltIn.GetVwAvsXDec_Avans()
            //               join b in ctxBuiltIn.GetF001() on 1 equals 1
            //               join f in ctxBuiltIn.GetAvsXDec_DocumentStateHistory() on new { DocumentId = a.DocumentId, Pozitie = 2 } equals new { DocumentId = f.DocumentId, Pozitie = f.Pozitie ?? -99 } into f1
            //               from f2 in f1.DefaultIfEmpty()
            //               join fManager in ctxBuiltIn.GetUSERS() on f2.USER_NO equals fManager.F70102 into fManager1
            //               from fManager2 in fManager1.DefaultIfEmpty()
            //               join numeManager in ctxBuiltIn.GetF100() on fManager2.F10003 equals numeManager.F10003 into numeManager1
            //               from numeManager2 in numeManager1.DefaultIfEmpty()
            //               where a.DocumentId == DocumentId && a.F10003 == F10003
            //               select new
            //               {
            //                   CompanyFreeTxt = b.F00104,
            //                   DocumentId = a.DocumentId,
            //                   DocumentDate = a.DocumentDate,
            //                   NumeComplet = a.NumeComplet,
            //                   Functie = a.Functie,
            //                   ActionReason = a.ActionReason,
            //                   ActionPlace = a.ActionPlace,
            //                   StartDate = a.StartDate,
            //                   StartHour = a.StartHour,
            //                   EndDate = a.EndDate,
            //                   EndHour = a.EndHour,
            //                   NumeManager = numeManager2.F10008,
            //                   PrenumeManager = numeManager2.F10009
            //               }).ToList()
            //              .Select(x => new
            //              {
            //                  CompanyFreeTxt = x.CompanyFreeTxt == null ? "" : x.CompanyFreeTxt,
            //                  OrdinDeplasareFreeTxt = (x.DocumentId == null ? "" : x.DocumentId.ToString()) + " / " + (x.DocumentDate == null ? "" : x.DocumentDate.Value.ToString("dd-MM-yyyy")),
            //                  DocumentNumber = x.DocumentId == null ? "" : x.DocumentId.ToString(),
            //                  NumeComplet = x.NumeComplet == null ? "" : x.NumeComplet,
            //                  Functie = x.Functie == null ? "" : x.Functie,
            //                  ActionReason = x.ActionReason == null ? "" : x.ActionReason,
            //                  ActionPlace = x.ActionPlace == null ? "" : x.ActionPlace,
            //                  StartTime = (x.StartDate == null) ? "" : (x.StartDate.Value.ToString("dd-MM-yyyy") + " " + x.StartHour.Value.ToString("HH:mm")),
            //                  EndTime = (x.EndDate == null) ? "" : (x.EndDate.Value.ToString("dd-MM-yyyy") + " " + x.EndHour.Value.ToString("HH:mm")),
            //                  SefCompartiment = (x.NumeManager == null ? "" : x.NumeManager) + " " + (x.PrenumeManager == null ? "" : x.PrenumeManager)
            //              });

            //    this.DataSource = ent;
            //}
            //else
            //{
            //    var ent = (from a in ctxBuiltIn.GetVwAvsXDec_Decont()
            //               join b in ctxBuiltIn.GetF001() on 1 equals 1
            //               join f in ctxBuiltIn.GetAvsXDec_DocumentStateHistory() on new { DocumentId = a.DocumentId, Pozitie = 2 } equals new { DocumentId = f.DocumentId, Pozitie = (f.Pozitie ?? -99) } into f1
            //               from f2 in f1.DefaultIfEmpty()
            //               join fManager in ctxBuiltIn.GetUSERS() on f2.USER_NO equals fManager.F70102 into fManager1
            //               from fManager2 in fManager1.DefaultIfEmpty()
            //               join numeManager in ctxBuiltIn.GetF100() on fManager2.F10003 equals numeManager.F10003 into numeManager1
            //               from numeManager2 in numeManager1.DefaultIfEmpty()
            //               where a.DocumentId == DocumentId && a.F10003 == F10003
            //               select new
            //               {
            //                   CompanyFreeTxt = b.F00104,
            //                   DocumentId = a.DocumentId,
            //                   DocumentDate = a.DocumentDate,
            //                   NumeComplet = a.NumeComplet,
            //                   Functie = a.Functie,
            //                   ActionReason = a.ActionReason,
            //                   ActionPlace = a.ActionPlace,
            //                   StartDate = a.StartDate,
            //                   StartHour = a.StartHour,
            //                   EndDate = a.EndDate,
            //                   EndHour = a.EndHour,
            //                   NumeManager = numeManager2.F10008,
            //                   PrenumeManager = numeManager2.F10009
            //               }).ToList()
            //              .Select(x => new
            //              {
            //                  CompanyFreeTxt = x.CompanyFreeTxt == null ? "" : x.CompanyFreeTxt,
            //                  OrdinDeplasareFreeTxt = (x.DocumentId == null ? "" : x.DocumentId.ToString()) + " / " + (x.DocumentDate == null ? "" : x.DocumentDate.Value.ToString("dd-MM-yyyy")),
            //                  DocumentNumber = x.DocumentId == null ? "" : x.DocumentId.ToString(),
            //                  NumeComplet = x.NumeComplet == null ? "" : x.NumeComplet,
            //                  Functie = x.Functie == null ? "" : x.Functie,
            //                  ActionReason = x.ActionReason == null ? "" : x.ActionReason,
            //                  ActionPlace = x.ActionPlace == null ? "" : x.ActionPlace,
            //                  StartTime = (x.StartDate == null) ? "" : (x.StartDate.Value.ToString("dd-MM-yyyy") + " " + ((x.StartHour == null) ? "" : x.StartHour.Value.ToString("HH:mm"))),
            //                  EndTime = (x.EndDate == null) ? "" : (x.EndDate.Value.ToString("dd-MM-yyyy") + " " + ((x.EndHour == null) ? "" : x.EndHour.Value.ToString("HH:mm"))),
            //                  SefCompartiment = (x.NumeManager == null ? "" : x.NumeManager) + " " + (x.PrenumeManager == null ? "" : x.PrenumeManager)
            //              });

            //    this.DataSource = ent;
            //}
            #endregion

            DataTable ent = General.rapOrdinDeplasare(F10003, DocumentId);
            this.DataSource = ent;
        }
    }
}
 