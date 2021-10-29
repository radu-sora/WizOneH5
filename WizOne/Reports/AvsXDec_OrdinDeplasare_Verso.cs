using System;
using DevExpress.XtraReports.UI;
using System.Linq;
using System.Web;
using System.Data;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class AvsXDec_OrdinDeplasare_Verso : DevExpress.XtraReports.UI.XtraReport
    {

        public AvsXDec_OrdinDeplasare_Verso()
        {
            InitializeComponent();
        }


        private void RaportForm8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            GroupHeader1.GroupFields.Add(new GroupField("DocumentDecontNumber"));
            int DocumentId = Convert.ToInt32(HttpContext.Current.Session["AvsXDec_IdDocument"].ToString());
            int F10003 = Convert.ToInt32(HttpContext.Current.Session["AvsXDec_Marca"].ToString());

            for (int i = 0; i < this.Parameters.Count; i++)
                this.Parameters[i].Visible = false;

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
            //    var ent = (from a in ctxBuiltIn.GetVwAvsXDec_Decont()
            //               join b in ctxBuiltIn.GetAvsXDec_BusinessTransaction() on a.DocumentId equals b.DestDocId
            //               join c in ctxBuiltIn.GetVwAvsXDec_Avans() on b.SrcDocId equals c.DocumentId
            //               join d in ctxBuiltIn.GetVwAvsXDec_DecDet_DocDecontare() on a.DocumentId equals d.DocumentId
            //               join f in ctxBuiltIn.GetAvsXDec_DocumentStateHistory() on new { DocumentId = a.DocumentId, Pozitie = 2 } equals new { DocumentId = f.DocumentId, Pozitie = f.Pozitie ?? -99 } into f1
            //               from f2 in f1.DefaultIfEmpty()
            //               join fManager in ctxBuiltIn.GetUSERS() on f2.USER_NO equals fManager.F70102 into fManager1
            //               from fManager2 in fManager1.DefaultIfEmpty()
            //               join numeManager in ctxBuiltIn.GetF100() on fManager2.F10003 equals numeManager.F10003 into numeManager1
            //               from numeManager2 in numeManager1.DefaultIfEmpty()
            //               where b.SrcDocId == DocumentId && a.F10003 == F10003 && a.DocumentStateId >= 3
            //               select new
            //               {
            //                   StartDate = a.StartDate,
            //                   StartHour = a.StartHour,
            //                   EndDate = a.EndDate,
            //                   EndHour = a.EndHour,
            //                   DocumentDate = a.DocumentDate,
            //                   AvansDocNumber = c.DocumentId,
            //                   AvansAmount = c.TotalAmount,
            //                   TipDocument = d.TipDocument,
            //                   Furnizor = d.Furnizor,
            //                   DocNumberDecont = d.DocNumberDecont,
            //                   DocDateDecont = d.DocDateDecont,
            //                   TotalPayment = d.TotalPayment,
            //                   CurrencyName = d.CurrencyName,
            //                   UnconfRestAmount = a.UnconfRestAmount,
            //                   TotalDecontValue = a.TotalAmount,
            //                   TitularAvans = c.NumeComplet,
            //                   NumeManager = numeManager2.F10008,
            //                   PrenumeManager = numeManager2.F10009
            //               }).ToList()
            //              .Select(x => new
            //              {                 //Radu 02.09.2016
            //                  StartTime = (x.StartDate != null && x.StartHour != null ? x.StartDate.Value.ToString("dd/MM/yyyy") + " " + x.StartHour.Value.ToString("HH:mm") : null),
            //                  EndTime = (x.EndDate != null && x.EndHour != null ? x.EndDate.Value.ToString("dd/MM/yyyy") + " " + x.EndHour.Value.ToString("HH:mm") : null),
            //                  DecontDate = x.DocumentDate == null ? "" : x.DocumentDate.Value.ToString("dd/MM/yyyy"),
            //                  AvansDocNumber = x.AvansDocNumber == null ? "" : x.AvansDocNumber.ToString(),
            //                  AvansAmount = x.AvansAmount == null ? 0 : x.AvansAmount,
            //                  DocumentDecont = (x.TipDocument == null ? "" : x.TipDocument) + ", " + (x.Furnizor == null ? "" : x.Furnizor),
            //                  DocumentDecontNumber = (x.DocNumberDecont == null ? "" : (x.DocNumberDecont + "/")) + (x.DocDateDecont == null ? "" : x.DocDateDecont.Value.ToString("dd/MM/yyyy")),
            //                  DocumentDecontValue = (x.TotalPayment == null ? "" : x.TotalPayment.ToString()) + " " + (x.CurrencyName == null ? "" : x.CurrencyName),
            //                  UnconfRestAmount = (x.AvansAmount == null ? 0 : x.AvansAmount) - (x.TotalDecontValue == null ? 0 : x.TotalDecontValue),
            //                  TotalDecontValue = x.TotalDecontValue == null ? 0 : x.TotalDecontValue,
            //                  TitularAvans = x.TitularAvans == null ? "" : x.TitularAvans,
            //                  SefCompartiment = (x.NumeManager == null ? "" : x.NumeManager) + " " + (x.PrenumeManager == null ? "" : x.PrenumeManager)
            //              });

            //    this.DataSource = ent;
            //    bool dePrimit = false;
            //    if (ent != null && ent.Count()!=0)
            //    {
            //        decimal AvansAmount = ent.FirstOrDefault().AvansAmount ?? 0;
            //        if (AvansAmount == 0)
            //            dePrimit = true;
            //        else
            //            dePrimit = false;
            //    }

            //    if (dePrimit)
            //    {
            //        lblPrimit.Visible = true;
            //        lblRestituit.Visible = false;
            //    }
            //    else
            //    {
            //        lblPrimit.Visible = false;
            //        lblRestituit.Visible = true;
            //    }
            //}
            //else
            //{
            //    var ent = (from a in ctxBuiltIn.GetVwAvsXDec_Decont()
            //               join d in ctxBuiltIn.GetVwAvsXDec_DecDet_DocDecontare() on a.DocumentId equals d.DocumentId
            //               join f in ctxBuiltIn.GetAvsXDec_DocumentStateHistory() on new { DocumentId = a.DocumentId, Pozitie = 2 } equals new { DocumentId = f.DocumentId, Pozitie = f.Pozitie ?? -99 } into f1
            //               from f2 in f1.DefaultIfEmpty()
            //               join fManager in ctxBuiltIn.GetUSERS() on f2.USER_NO equals fManager.F70102 into fManager1
            //               from fManager2 in fManager1.DefaultIfEmpty()
            //               join numeManager in ctxBuiltIn.GetF100() on fManager2.F10003 equals numeManager.F10003 into numeManager1
            //               from numeManager2 in numeManager1.DefaultIfEmpty()
            //               where a.DocumentId == DocumentId && a.F10003 == F10003 && a.DocumentStateId >= 3
            //               select new
            //               {
            //                   StartDate = a.StartDate,
            //                   StartHour = a.StartHour,
            //                   EndDate = a.EndDate,
            //                   EndHour = a.EndHour,
            //                   DocumentDate = a.DocumentDate,
            //                   AvansDocNumber = a.DocumentId,
            //                   AvansAmount = 0,
            //                   TipDocument = d.TipDocument,
            //                   Furnizor = d.Furnizor,
            //                   DocNumberDecont = d.DocNumberDecont,
            //                   DocDateDecont = d.DocDateDecont,
            //                   TotalPayment = d.TotalPayment,
            //                   CurrencyName = d.CurrencyName,
            //                   UnconfRestAmount = a.UnconfRestAmount,
            //                   TotalDecontValue = a.TotalAmount,
            //                   TitularAvans = a.NumeComplet,
            //                   NumeManager = numeManager2.F10008,
            //                   PrenumeManager = numeManager2.F10009
            //               }).ToList()
            //              .Select(x => new
            //              {                 //Radu 02.09.2016
            //                  StartTime = (x.StartDate != null && x.StartHour != null ? x.StartDate.Value.ToString("dd/MM/yyyy") + " " + ((x.StartHour == null) ? "" : x.StartHour.Value.ToString("HH:mm")) : null),
            //                  EndTime = (x.EndDate != null && x.EndHour != null ? x.EndDate.Value.ToString("dd/MM/yyyy") + " " + ((x.EndHour == null) ? "" : x.EndHour.Value.ToString("HH:mm")) : null),
            //                  DecontDate = x.DocumentDate == null ? "" : x.DocumentDate.Value.ToString("dd/MM/yyyy"),
            //                  AvansDocNumber = x.AvansDocNumber == null ? "" : x.AvansDocNumber.ToString(),
            //                  AvansAmount = x.AvansAmount == null ? 0 : x.AvansAmount,
            //                  DocumentDecont = (x.TipDocument == null ? "" : x.TipDocument) + ", " + (x.Furnizor == null ? "" : x.Furnizor),
            //                  DocumentDecontNumber = (x.DocNumberDecont == null ? "" : (x.DocNumberDecont + "/")) + (x.DocDateDecont == null ? "" : x.DocDateDecont.Value.ToString("dd/MM/yyyy")),
            //                  DocumentDecontValue = (x.TotalPayment == null ? "" : x.TotalPayment.ToString()) + " " + (x.CurrencyName == null ? "" : x.CurrencyName),
            //                  UnconfRestAmount = (x.AvansAmount == null ? 0 : x.AvansAmount) - (x.TotalDecontValue == null ? 0 : x.TotalDecontValue),
            //                  TotalDecontValue = x.TotalDecontValue == null ? 0 : x.TotalDecontValue,
            //                  TitularAvans = x.TitularAvans == null ? "" : x.TitularAvans,
            //                  SefCompartiment = (x.NumeManager == null ? "" : x.NumeManager) + " " + (x.PrenumeManager == null ? "" : x.PrenumeManager)
            //              });

            //    this.DataSource = ent;
            //    bool dePrimit = false;
            //    if (ent != null && ent.Count() != 0)
            //    {
            //        int AvansAmount = ent.FirstOrDefault().AvansAmount;
            //        if (AvansAmount == 0)
            //            dePrimit = true;
            //        else
            //            dePrimit = false;
            //    }

            //    if (dePrimit)
            //    {
            //        lblPrimit.Visible = true;
            //        lblRestituit.Visible = false;
            //    }
            //    else
            //    {
            //        lblPrimit.Visible = false;
            //        lblRestituit.Visible = true;
            //    }
            //}
            #endregion

            string sql = "SELECT a.DocumentId FROM vwAvsXDec_Avans a WHERE a.DocumentId = " + DocumentId + " AND a.F10003 = " + F10003;
            DataTable entAvans = General.IncarcaDT(sql, null);

            string strSQL = string.Empty;
            if (entAvans != null && entAvans.Rows.Count != 0)
            {
                sql = "SELECT a.DestDocId FROM AvsXDec_BusinessTransaction a WHERE a.SrcDocId = " + DocumentId;         
                DataTable entDecont = General.IncarcaDT(sql, null);
                if (entDecont != null && entDecont.Rows.Count != 0)
                    DocumentId = Convert.ToInt32(General.Nz(entDecont.Rows[0]["DestDocId"], -99));
            }

            DataTable ent = General.rapOrdinDeplasareDetail(F10003, DocumentId);
            this.DataSource = ent;
            bool dePrimit = false;
            if (ent != null && ent.Rows.Count != 0)
            {
                decimal UnconfRestAmount = Convert.ToInt32(General.Nz(ent.Rows[0]["UnconfRestAmount"], 0));
                decimal AvansAmount = Convert.ToInt32(General.Nz(ent.Rows[0]["AvansAmount"], 0));
                if (UnconfRestAmount <= 0)
                    dePrimit = true;
                else
                {
                    if (AvansAmount == 0)
                        dePrimit = true;
                    else
                        dePrimit = false;
                }
            }
            if (dePrimit)
            {
                lblPrimit.Visible = true;
                lblRestituit.Visible = false;
            }
            else
            {
                lblPrimit.Visible = false;
                lblRestituit.Visible = true;
            }
        }
    }
}
 