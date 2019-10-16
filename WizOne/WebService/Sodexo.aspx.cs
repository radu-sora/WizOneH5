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

namespace WizOne.WebService
{
    public partial class Sodexo : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                
                
               // grDateAdrese.DataBind();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                //btnClose.Text = Dami.TraduCuvant("btnClose", "Inchidere luna");
                //btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare luna");

                #endregion

                if (IsPostBack)
                {
                    if (Session["Sodexo_TicheteGrid"] != null)
                    {
                        DataTable table = Session["Sodexo_TicheteGrid"] as DataTable;
                        GridViewDataComboBoxColumn colTichete = (grDate.Columns["Tichete"] as GridViewDataComboBoxColumn);
                        colTichete.PropertiesComboBox.DataSource = table;

                    }

                    if (Session["Sodexo_Conectare"] != null)
                    {
                        SodexoOnline.company[] listaCompanii = Session["Sodexo_ListaCompanii"] as SodexoOnline.company[];
                        DataTable tableC = new DataTable();
                        tableC.Columns.Add("Id", typeof(int));
                        tableC.Columns.Add("Denumire", typeof(string));
                        for (int i = 0; i < listaCompanii.Count(); i++)
                            tableC.Rows.Add(listaCompanii.ElementAt(i).companyId, listaCompanii.ElementAt(i).companyName);
                        cmbCompanie.DataSource = tableC;
                        cmbCompanie.DataBind();

                        if (cmbCompanie.SelectedIndex >= 0)
                            ComboTipAdresa();
                    }

                    if (Session["Sodexo_Adrese"] != null)
                    {
                        DataTable dtAdr = Session["Sodexo_Adrese"] as DataTable;
                        grDateAdrese.DataSource = dtAdr;
                        grDateAdrese.DataBind();
                    }

                    DataTable dt = new DataTable();
                    if (cmbCompanie.Value != null)
                    {
                        if (Session["Sodexo_Grid"] == null)
                        {
                            dt = General.IncarcaDT("SELECT * FROM \"tblTichete_WebService\" WHERE \"Companie\" = '" + cmbCompanie.Text + "'", null);
                        }
                        else
                        {
                            dt = Session["Sodexo_Grid"] as DataTable;
                        }
                        grDate.DataSource = dt;
                        grDate.DataBind();
                        grDate.KeyFieldName = "IdAuto";
                        Session["Sodexo_Grid"] = dt;
                    }
                    else
                    {
                        dt = null;                      
                        grDate.DataSource = dt;
                        grDate.DataBind();
                        grDate.KeyFieldName = "IdAuto";
                        Session["Sodexo_Grid"] = dt;
                    }

                }
                else
                {

                    Session["Sodexo_Grid"] = null;
                    Session["Sodexo_Adrese"] = null;

                    string sql = "SELECT \"Nume\", \"Valoare\", \"Criptat\"  FROM \"tblParametrii\" WHERE \"Nume\" IN ('WebService_Utilizator', 'WebService_Parola')";
                    DataTable dtParam = General.IncarcaDT(sql, null);
                    string user = "", pwd = "";
                    if (dtParam != null && dtParam.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtParam.Rows.Count; i++)
                        {
                            if (dtParam.Rows[i]["Criptat"] == null || dtParam.Rows[i]["Criptat"].ToString().Length <= 0 || dtParam.Rows[i]["Criptat"].ToString() == "0")
                            {
                                if (dtParam.Rows[i]["Nume"].ToString() == "WebService_Utilizator")
                                    user = dtParam.Rows[i]["Valoare"].ToString();
                                else
                                    pwd = dtParam.Rows[i]["Valoare"].ToString();
                            }
                            else
                            {
                                CriptDecript prc = new CriptDecript();
                                if (dtParam.Rows[i]["Nume"].ToString() == "WebService_Utilizator")
                                    user = prc.EncryptString("WizOne2016", (dtParam.Rows[i]["Valoare"] as string ?? "").ToString(), 2);
                                else
                                    pwd = prc.EncryptString("WizOne2016", (dtParam.Rows[i]["Valoare"] as string ?? "").ToString(), 2);
                            }
                        }
                    }

                    if (user.Length > 0 && pwd.Length > 0)
                        Conectare(user, pwd, 1);
                }

                DataTable dtCrit = new DataTable();
                dtCrit.Columns.Add("Id", typeof(int));
                dtCrit.Columns.Add("Denumire", typeof(string));
                dtCrit.Rows.Add(1, "Departament");
                dtCrit.Rows.Add(2, "Punct de lucru");
                dtCrit.Rows.Add(3, "Locatie");
                dtCrit.Rows.Add(4, "Nume");
                cmbCriteriu1.DataSource = dtCrit;
                cmbCriteriu1.DataBind();
                cmbCriteriu2.DataSource = dtCrit;
                cmbCriteriu2.DataBind();
                cmbCriteriu3.DataSource = dtCrit;
                cmbCriteriu3.DataBind();

                //grDate.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string[] param = e.Parameter.Split(';');
                switch (param[0])
                {
                    case "btnConectare":
                        Conectare(txtUtilizator.Text, txtParola.Text, 2);
                        break;
                    case "cmbCompanie":
                        grDate.DataSource = null;
                        grDate.DataBind();
                                           

                        ComboTipAdresa();

                        SodexoOnline.company[] listaCompanii = Session["Sodexo_ListaCompanii"] as SodexoOnline.company[];
                        SodexoOnline.productFacialValues[] listaTichete = Session["Sodexo_ListaTichete"] as SodexoOnline.productFacialValues[];
                        DataTable table = new DataTable();
                        table.Columns.Add("Id", typeof(string));
                        table.Columns.Add("Denumire", typeof(string));
                        for (int k = 0; k < listaCompanii.Count(); k++)
                            if (listaCompanii.ElementAt(k).companyId == Convert.ToInt32(param[1]))
                            {
                                for (int l = 0; l < listaCompanii.ElementAt(k).products.Count(); l++)
                                    for (int i = 0; i < listaTichete.Count(); i++)
                                        for (int j = 0; j < listaTichete.ElementAt(i).values.Count(); j++)
                                            if (listaCompanii.ElementAt(k).products[l].productId == listaTichete.ElementAt(i).productId)
                                                table.Rows.Add(listaTichete.ElementAt(i).productId.ToString() + "_" + listaTichete.ElementAt(i).values[j].ToString(),
                                                    listaTichete.ElementAt(i).productName + " - " + listaTichete.ElementAt(i).values[j].ToString());
                                break;
                            }    

                        GridViewDataComboBoxColumn colTichete = (grDate.Columns["Tichete"] as GridViewDataComboBoxColumn);
                        colTichete.PropertiesComboBox.DataSource = table;
                        Session["Sodexo_TicheteGrid"] = table;

                        Session["Sodexo_ListaAdrese"] = null;
                        Session["Sodexo_ListaContacte"] = null;
                        SodexoOnline.v10paper infoSodexo = new SodexoOnline.v10paper();
                        SodexoOnline.resultCompanyAddressList listaAdrese = infoSodexo.companyAddressList(Convert.ToInt32(cmbCompanie.Value), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
                        Session["Sodexo_ListaAdrese"] = listaAdrese;
                        SodexoOnline.resultContactList listaContacte = infoSodexo.companyContactList(Convert.ToInt32(cmbCompanie.Value), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
                        Session["Sodexo_ListaContacte"] = listaContacte;

                        grDateAdrese.DataSource = null;
                        grDateAdrese.DataBind();

                        break;
                    case "cmbTipAdresa":
                        Session["Sodexo_Adrese"] = null;
                        grDateAdrese.DataSource = null;
                        grDateAdrese.DataBind();
                        break;
                    case "cmbCriteriu1":
                        if ((cmbCriteriu2.Value != null && Convert.ToInt32(cmbCriteriu2.Value) == Convert.ToInt32(param[1])) 
                            || (cmbCriteriu3.Value != null && Convert.ToInt32(cmbCriteriu3.Value) == Convert.ToInt32(param[1])))
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Valoarea a fost deja selectata!");
                            cmbCriteriu1.Value = null;
                        }
                        break;
                    case "cmbCriteriu2":
                        if ((cmbCriteriu1.Value != null && Convert.ToInt32(cmbCriteriu1.Value) == Convert.ToInt32(param[1]))
                            || (cmbCriteriu3.Value != null && Convert.ToInt32(cmbCriteriu3.Value) == Convert.ToInt32(param[1])))
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Valoarea a fost deja selectata!");
                            cmbCriteriu2.Value = null;
                        }
                        break;
                    case "cmbCriteriu3":
                        if ((cmbCriteriu2.Value != null && Convert.ToInt32(cmbCriteriu2.Value) == Convert.ToInt32(param[1]))
                             || (cmbCriteriu1.Value != null && Convert.ToInt32(cmbCriteriu1.Value) == Convert.ToInt32(param[1])))
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Valoarea a fost deja selectata!");
                            cmbCriteriu3.Value = null;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ex.ToString());
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Salvare();
            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ex.ToString());
                //ArataMesaj("");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void Salvare()
        {
            if (grDate.VisibleRowCount <= 0)
            {
                MessageBox.Show(Dami.TraduCuvant("Nu ati introdus tichetele!"));
                //ArataMesaj(Dami.TraduCuvant("Nu ati introdus tichetele!"));
                return;
            }

            btnSave.Enabled = false;
            string mesaj = TrimiteOrdin();
            
            btnSave.Enabled = true;
            MessageBox.Show(Dami.TraduCuvant(mesaj));
            //ArataMesaj(Dami.TraduCuvant(mesaj));

        }


        private void Conectare(string utilizator, string parola, int tip)
        {
            SodexoOnline.v10paper infoSodexo = new SodexoOnline.v10paper();
            Session["Sodexo_Utilizator"] = utilizator;
            Session["Sodexo_Parola"] = parola;
            SodexoOnline.resultProductList tichete = new SodexoOnline.resultProductList();
            try
            {
                tichete = infoSodexo.productList(utilizator, parola);
            }
            catch(Exception ex)
            {
                MessageBox.Show(Dami.TraduCuvant("Conectare esuata!"));
            }
            if (tichete.requestResponse.responseCode == "00")
            {
                var listaTichete = tichete.products;

                var companii = infoSodexo.companyList(utilizator, parola);
                if (companii.requestResponse.responseCode == "00")
                {
                    var listaCompanii = companii.companies;

                    DataTable table = new DataTable();

                    table.Columns.Add("Id", typeof(int));
                    table.Columns.Add("Denumire", typeof(string));

                    for (int i = 0; i < listaCompanii.Count(); i++)
                        table.Rows.Add(listaCompanii.ElementAt(i).companyId, listaCompanii.ElementAt(i).companyName);
                    cmbCompanie.DataSource = table;
                    cmbCompanie.DataBind();

                    Session["Sodexo_ListaTichete"] = listaTichete;
                    Session["Sodexo_ListaCompanii"] = listaCompanii;
                    Session["Sodexo_Conectare"] = "1";

                    //ArataMesaj("Conectare reusita!");
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Conectare reusita!"));
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Conectare reusita!");

                    string sql = "DELETE FROM \"tblParametrii\" WHERE \"Nume\" IN ('WebService_Utilizator', 'WebService_Parola')";
                    General.ExecutaNonQuery(sql, null);

                    CriptDecript prc = new CriptDecript();
                    string user = prc.EncryptString("WizOne2016", utilizator, 1);
                    string pwd = prc.EncryptString("WizOne2016", parola, 1);

                    sql = "INSERT INTO \"tblParametrii\" (\"Nume\", \"Valoare\", \"Explicatie\", \"Criptat\") VALUES('{0}', '{1}', '{2}', 1) ";
                    string strSql = string.Format(sql, "WebService_Utilizator", user, "Utilizator pentru pagina WebService - tichete de masa");
                    General.ExecutaNonQuery(strSql, null);
                    strSql = string.Format(sql, "WebService_Parola", pwd, "Parola pentru pagina WebService - tichete de masa");
                    General.ExecutaNonQuery(strSql, null);
                }
                else
                {
                    //ArataMesaj("Conectare esuata! (" + companii.requestResponse.responseText + ")");
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Conectare esuata! (" + companii.requestResponse.responseText + ")"));
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Conectare esuata! (" + companii.requestResponse.responseText + ")");
                }
            }
            else
            {
                if (tip == 1)
                    MessageBox.Show(Dami.TraduCuvant("Conectare esuata! (" + tichete.requestResponse.responseText + ")"));
                else
                    //ArataMesaj("Conectare esuata! (" + tichete.requestResponse.responseText + ")");
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Conectare esuata! (" + tichete.requestResponse.responseText + ")");
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataTable dt = General.IncarcaDT("SELECT * FROM \"tblTichete_WebService\"", null); 
                if (dt.Columns["IdAuto"] != null)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                        e.NewValues["IdAuto"] = max;
                    }
                    else
                        e.NewValues["IdAuto"] = 1;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string TrimiteOrdin()
        {
            string mesaj = "";

            try
            {
                //DataTable dtGrid = Session["Sodexo_Grid"] as DataTable;
                //SqlDataAdapter da = new SqlDataAdapter();
                //SqlCommandBuilder cb = new SqlCommandBuilder();

                //da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"tblTichete_WebService\"", null);
                //cb = new SqlCommandBuilder(da);
                //da.Update(dtGrid);
                //da.Dispose();
                //da = null;
                //General.SalveazaDate(dtGrid, "tblTichete_WebService");

                if ((cmbCriteriu1.Value == null && (cmbCriteriu2.Value != null || cmbCriteriu3.Value != null)) 
                    || (cmbCriteriu2.Value == null && cmbCriteriu3.Value != null))
                    return "Criteriile de sortare sunt invalide!";

                SodexoOnline.v10paper infoSodexo = new SodexoOnline.v10paper();
                SodexoOnline.resultCompanyAddressList listaAdrese = new SodexoOnline.resultCompanyAddressList();
                SodexoOnline.company[] listaCompanii = Session["Sodexo_ListaCompanii"] as SodexoOnline.company[];
                SodexoOnline.resultContactList listaContacte = new SodexoOnline.resultContactList();
                string numeCompanie = "";

                for (int i = 0; i < listaCompanii.Count(); i++)
                    if (listaCompanii.ElementAt(i).companyId == Convert.ToInt32(cmbCompanie.Value.ToString()))
                    {
                        numeCompanie = listaCompanii.ElementAt(i).companyName;
                        if (Session["Sodexo_ListaAdrese"] != null)
                            listaAdrese = Session["Sodexo_ListaAdrese"] as SodexoOnline.resultCompanyAddressList;
                        else
                            listaAdrese = infoSodexo.companyAddressList(listaCompanii.ElementAt(i).companyId, Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
                        if (Session["Sodexo_ListaContacte"] != null)
                            listaContacte = Session["Sodexo_ListaContacte"] as SodexoOnline.resultContactList;
                        else
                            listaContacte = infoSodexo.companyContactList(listaCompanii.ElementAt(i).companyId, Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());                        
                    }

                for (int k = 0; k < grDate.VisibleRowCount; k++)
                {
                    object obj = grDate.GetRowValues(k, "Tichete");
                    string[] param = obj.ToString().Split('_');

                    SodexoOnline.requestPaperOrder request = new SodexoOnline.requestPaperOrder();
                    List<SodexoOnline.paperOrderAddress> orderDetails = new List<SodexoOnline.paperOrderAddress>();

                    request.companyId = Convert.ToInt32(cmbCompanie.Value.ToString());
                    request.email = txtEMail.Text;
                    request.productId = Convert.ToInt32(param[0]);
           
                    int addressId = 0;                        
                    for (int l = 0; l < listaAdrese.addresses.Count(); l++)
                    {
                        for (int m = 0; m < listaAdrese.addresses[l].products.Count(); m++)
                            if (listaAdrese.addresses[l].products.ElementAt(m).productId == request.productId)
                            {
                                addressId = listaAdrese.addresses[l].addressId;
                                break;
                            }
                        if (addressId > 0)
                            break;
                    }

                    if (addressId > 0)
                    {
                        SodexoOnline.paperOrderAddress orderDetail = new SodexoOnline.paperOrderAddress();

                        //orderDetail.addressId = addressId;

                        orderDetail.productValue = (float)Convert.ToDouble(param[1]);                            
                            
                        List<SodexoOnline.paperOrderPerson> vouchers = new List<SodexoOnline.paperOrderPerson>();                          
                        DataTable dt = new DataTable();
                        
                        SodexoOnline.productFacialValues[] listaTichete = Session["Sodexo_ListaTichete"] as SodexoOnline.productFacialValues[];
                        string numeTichet = "";
                        for (int i = 0; i < listaTichete.Count(); i++)
                            if (listaTichete.ElementAt(i).productId == Convert.ToInt32(param[0]))
                            {
                                numeTichet = listaTichete.ElementAt(i).productName;
                                break;
                            }
                        string ordonare = "";
                        string temp = " ORDER BY ";
                        if (cmbCriteriu1.Value != null)
                            ordonare += DaMiValoare(Convert.ToInt32(cmbCriteriu1.Value));
                        if (cmbCriteriu2.Value != null)
                            ordonare += DaMiValoare(Convert.ToInt32(cmbCriteriu2.Value));
                        if (cmbCriteriu3.Value != null)
                            ordonare += DaMiValoare(Convert.ToInt32(cmbCriteriu3.Value));
                        if (ordonare.Length > 0)
                            ordonare = temp + ordonare.Substring(0, ordonare.Length - 1);

                        dt = General.IncarcaDT("SELECT * FROM \"viewSodexoRequest\" WHERE \"NumeCompanie\" = '" + numeCompanie + "' AND \"NumeTichet\" = '" + numeTichet + "' " + ordonare, null);
                        for (int y = 0; y < dt.Rows.Count; y++)
                        {
                            SodexoOnline.paperOrderPerson voucher = new SodexoOnline.paperOrderPerson();
                            voucher.firstName = dt.Rows[y]["PrenumeAng"].ToString();
                            voucher.lastName = dt.Rows[y]["NumeAng"].ToString();
                            voucher.SSN = dt.Rows[y]["CNP"].ToString();
                            voucher.department = dt.Rows[y]["Departament"].ToString();
                            voucher.responsiblePerson = dt.Rows[y]["Responsabil"].ToString();
                            voucher.voucherNo = Convert.ToInt32(dt.Rows[y]["NrTichete"].ToString());
                            vouchers.Add(voucher);
                        }
                        orderDetail.vouchers = vouchers.ToArray();
                        orderDetail.addressId = (dt.Rows[0]["CodAdresaWiz"] != null && dt.Rows[0]["CodAdresaWiz"].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0]["CodAdresaWiz"].ToString()) > 0
                            ? Convert.ToInt32(dt.Rows[0]["CodAdresaWiz"].ToString()) : addressId);
                        for (int z = 0; z < listaContacte.contacts.Count(); z++)
                            if (listaContacte.contacts.ElementAt(z).firstName.ToUpper().Trim() == dt.Rows[0]["PrenumePersContact"].ToString().ToUpper().Trim()
                                && listaContacte.contacts.ElementAt(z).lastName.ToUpper().Trim() == dt.Rows[0]["NumePersContact"].ToString().ToUpper().Trim())
                                orderDetail.contactId = listaContacte.contacts.ElementAt(z).contactId;

                        var setAddress = infoSodexo.addressIdentifierSet(addressId, dt.Rows[0]["CodAdresaWiz"].ToString(), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
                        orderDetail.clientAddressId = "";
                        orderDetails.Add(orderDetail);                        
                    }

                    request.orderDetails = orderDetails.ToArray();

                    var ordin = infoSodexo.paperOrder(request, Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());

                    if (ordin.requestResponse.responseCode != "00")
                    {
                        return "Eroare la trimiterea cererii! (" + ordin.requestResponse.responseText + ")";
                    }

                }
                return "Ordin trimis cu succes!";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        private string DaMiValoare(int id)
        {
            switch(id)
            {
                case 1:
                    return "Departament,";                    
                case 2:
                    return "PunctLucru,";
                case 3:
                    return "Locatie,";
                case 4:
                    return "NumeAng, PrenumeAng,";
                default:
                    return "";
            }            
        }


        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //DataTable dt = new DataTable();

                ////dt.Columns.Add("IdAuto", typeof(int));
                ////dt.Columns.Add("Tichete", typeof(string));
                ////dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ////if (Session["Sodexo_Grid"] != null)
                ////    dt = Session["Sodexo_Grid"] as DataTable;  

                //if (Session["Sodexo_Grid"] == null)
                //{
                //    if (cmbCompanie.Value != null)
                //        dt = General.IncarcaDT("SELECT * FROM \"tblTichete_WebService\" WHERE \"Companie\" = '" + cmbCompanie.Text + "'", null);
                //}
                //else
                //    dt = Session["Sodexo_Grid"] as DataTable;

                //grDate.DataSource = dt;
                //grDate.KeyFieldName = "IdAuto";
                //Session["Sodexo_Grid"] = dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void grDateAdrese_DataBinding(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();              
                if (Session["Sodexo_Adrese"] != null)
                    dt = Session["Sodexo_Adrese"] as DataTable;
                else
                    dt = General.IncarcaDT("SELECT * FROM \"tblTipAdresa_WebService\" WHERE \"TipAdresa\" = " + Convert.ToInt32(cmbTipAdresa.Value), null);
                grDateAdrese.DataSource = dt;
                grDateAdrese.KeyFieldName = "TipAdresa; IdAdresaWiz";

                string sql = "";
                switch (Convert.ToInt32(cmbTipAdresa.Value))
                {
                    case 1:
                        sql = "SELECT F08002 AS \"Id\", F08003 AS \"Denumire\" FROM F080";
                        break;
                    case 2:
                        sql = "SELECT ID_LOCATIE AS \"Id\", LOCATIE_MUNCA AS \"Denumire\" FROM LOCATIE_MUNCA";
                        break;
                    case 3:
                        sql = "SELECT F00607 AS \"Id\", F00608 AS \"Denumire\" FROM F006";
                        break;
                }
                GridViewDataComboBoxColumn colIdAdresaWiz = (grDateAdrese.Columns["IdAdresaWiz"] as GridViewDataComboBoxColumn);
                colIdAdresaWiz.PropertiesComboBox.DataSource = General.IncarcaDT(sql, null);

                DataTable dtAdr = new DataTable();
                dtAdr.Columns.Add("Id", typeof(int));
                dtAdr.Columns.Add("Denumire", typeof(string));
                SodexoOnline.v10paper infoSodexo = new SodexoOnline.v10paper();
                SodexoOnline.resultCompanyAddressList listaAdrese = new SodexoOnline.resultCompanyAddressList();
                SodexoOnline.resultContactList listaContacte = new SodexoOnline.resultContactList();
                if (Session["Sodexo_ListaAdrese"] != null)
                    listaAdrese = Session["Sodexo_ListaAdrese"] as SodexoOnline.resultCompanyAddressList;
                else
                    listaAdrese = infoSodexo.companyAddressList(Convert.ToInt32(cmbCompanie.Value), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
                if (Session["Sodexo_ListaContacte"] != null)
                    listaContacte = Session["Sodexo_ListaContacte"] as SodexoOnline.resultContactList;
                else
                    listaContacte = infoSodexo.companyContactList(Convert.ToInt32(cmbCompanie.Value), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());

                for (int l = 0; l < listaAdrese.addresses.Count(); l++)
                {           
                    dtAdr.Rows.Add(listaAdrese.addresses[l].addressId, listaAdrese.addresses[l].fullAddress);
                }

                DataTable dtCont = new DataTable();
                dtCont.Columns.Add("Id", typeof(int));
                dtCont.Columns.Add("Denumire", typeof(string));
                for (int z = 0; z < listaContacte.contacts.Count(); z++)
                {
                    dtCont.Rows.Add(listaContacte.contacts.ElementAt(z).contactId, listaContacte.contacts.ElementAt(z).firstName.ToUpper().Trim() + " " + listaContacte.contacts.ElementAt(z).lastName.ToUpper().Trim());
                }         

                GridViewDataComboBoxColumn colIdAdresaSodexo = (grDateAdrese.Columns["IdAdresaSodexo"] as GridViewDataComboBoxColumn);
                colIdAdresaSodexo.PropertiesComboBox.DataSource = dtAdr;

                GridViewDataComboBoxColumn colPersContact = (grDateAdrese.Columns["PersContact"] as GridViewDataComboBoxColumn);
                colPersContact.PropertiesComboBox.DataSource = dtCont;

                Session["Sodexo_Adrese"] = dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Sodexo_Grid"] as DataTable;
                DataTable dtTichete = General.IncarcaDT("SELECT * FROM \"tblTichete_WebService\"", null);

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName)
                        {
                            case "IdAuto":
                                row[x] = Convert.ToInt32(General.Nz(dtTichete.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "Companie":
                                row[x] = cmbCompanie.Text;
                                break;
                            case "Adresa":
                                string id = e.NewValues["Tichete"].ToString().Split('_')[0];
                                SodexoOnline.productFacialValues[] listaTichete = Session["Sodexo_ListaTichete"] as SodexoOnline.productFacialValues[];
                                for (int i = 0; i < listaTichete.Count(); i++)
                                    if (listaTichete.ElementAt(i).productId == Convert.ToInt32(id))
                                    {
                                        DataTable dtAdr = Session["Sodexo_Adrese"] as DataTable;
                                        for (int j = 0; j < dtAdr.Rows.Count; j++)
                                            if (dtAdr.Rows[j]["Tichete"].ToString().Contains(listaTichete.ElementAt(i).productName))
                                            {
                                                row[x] = dtAdr.Rows[j]["Id"].ToString();
                                                break; 
                                            }
                                        break;
                                    }  
                                break;
                            case "NumeTichet":
                                id = e.NewValues["Tichete"].ToString().Split('_')[0];
                                listaTichete = Session["Sodexo_ListaTichete"] as SodexoOnline.productFacialValues[];
                                for (int i = 0; i < listaTichete.Count(); i++)
                                    if (listaTichete.ElementAt(i).productId == Convert.ToInt32(id))
                                    {
                                        row[x] = listaTichete.ElementAt(i).productName;
                                        break;
                                    }
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }
                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                //grDate.DataBind();
                grDate.KeyFieldName = "IdAuto";
                Session["Sodexo_Grid"] = dt;

                General.SalveazaDate(dt, "tblTichete_WebService");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Sodexo_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName].Visible)
                    {   
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;

                        if (col.ColumnName == "Adresa")
                        {
                            string id = e.NewValues[col.ColumnName].ToString().Split('_')[0];
                            SodexoOnline.productFacialValues[] listaTichete = Session["Sodexo_ListaTichete"] as SodexoOnline.productFacialValues[];
                            for (int i = 0; i < listaTichete.Count(); i++)
                                if (listaTichete.ElementAt(i).productId == Convert.ToInt32(id))
                                {
                                    DataTable dtAdr = Session["Sodexo_Adrese"] as DataTable;
                                    for (int j = 0; j < dtAdr.Rows.Count; j++)
                                        if (dtAdr.Rows[j]["Tichete"].ToString().Contains(listaTichete.ElementAt(i).productName))
                                        {
                                            row[col.ColumnName] = dtAdr.Rows[j]["Id"].ToString();
                                            break;
                                        }
                                    break;
                                }
                        }

                        if (col.ColumnName == "NumeTichet")
                        {
                            string id = e.NewValues[col.ColumnName].ToString().Split('_')[0];
                            SodexoOnline.productFacialValues[] listaTichete = Session["Sodexo_ListaTichete"] as SodexoOnline.productFacialValues[];
                            for (int i = 0; i < listaTichete.Count(); i++)
                                if (listaTichete.ElementAt(i).productId == Convert.ToInt32(id))
                                {
                                    row[col.ColumnName] = listaTichete.ElementAt(i).productName;
                                    break;
                                }
                        }

                    }                 

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Sodexo_Grid"] = dt;
                grDate.DataSource = dt;
                //grDate.DataBind();

                General.SalveazaDate(dt, "tblTichete_WebService");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Sodexo_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Sodexo_Grid"] = dt;
                grDate.DataSource = dt;

                General.SalveazaDate(dt, "tblTichete_WebService");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = General.IncarcaDT("SELECT * FROM \"tblTichete_WebService\" WHERE \"Companie\" = '" + cmbCompanie.Text + "'", null);
                grDate.DataSource = dt;
                grDate.DataBind();
                grDate.KeyFieldName = "IdAuto";
                Session["Sodexo_Grid"] = dt;
            }
            catch (Exception ex)
            {

            }
        }

        protected void grDateAdrese_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

            //string comp = e.Parameters;
            //SodexoOnline.v10paper infoSodexo = new SodexoOnline.v10paper();
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Id", typeof(int));
            //dt.Columns.Add("Adresa", typeof(string));
            //dt.Columns.Add("Tichete", typeof(string));
            //var listaAdrese = infoSodexo.companyAddressList(Convert.ToInt32(comp), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
            //for (int l = 0; l < listaAdrese.addresses.Count(); l++)
            //{
            //    string tichete = "";
            //    for (int m = 0; m < listaAdrese.addresses[l].products.Count(); m++)
            //    {
            //        tichete += listaAdrese.addresses[l].products[m].productName;
            //        if (m < listaAdrese.addresses[l].products.Count() - 1)
            //            tichete += "; ";
            //    }
            //    dt.Rows.Add(listaAdrese.addresses[l].addressId, listaAdrese.addresses[l].fullAddress, tichete);

            //}
            //Session["Sodexo_Adrese"] = dt;
            //grDateAdrese.DataSource = dt;
            //grDateAdrese.DataBind();

        }

        private void ComboTipAdresa()
        {
            DataTable dtTipAdresa = new DataTable();
            dtTipAdresa.Columns.Add("Id", typeof(int));
            dtTipAdresa.Columns.Add("Denumire", typeof(string));
            dtTipAdresa.Rows.Add(1, "Punct de lucru");
            dtTipAdresa.Rows.Add(2, "Locatie");
            dtTipAdresa.Rows.Add(3, "Departament");
            cmbTipAdresa.DataSource = dtTipAdresa;
            cmbTipAdresa.DataBind();
        }




        protected void grDateAdrese_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Sodexo_Adrese"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName)
                        {
                            case "TipAdresa":
                                row[x] = Convert.ToInt32(cmbTipAdresa.Value);
                                break;
                            case "Tichete":
                                SodexoOnline.v10paper infoSodexo = new SodexoOnline.v10paper();
                                SodexoOnline.resultCompanyAddressList listaAdrese = new SodexoOnline.resultCompanyAddressList();
                                if (Session["Sodexo_ListaAdrese"] != null)
                                    listaAdrese = Session["Sodexo_ListaAdrese"] as SodexoOnline.resultCompanyAddressList;
                                else
                                    listaAdrese = infoSodexo.companyAddressList(Convert.ToInt32(cmbCompanie.Value), Session["Sodexo_Utilizator"].ToString(), Session["Sodexo_Parola"].ToString());
                                string tichete = "";
                                for (int l = 0; l < listaAdrese.addresses.Count(); l++)
                                {                                   
                                    if (Convert.ToInt32(e.NewValues["IdAdresaSodexo"].ToString()) == listaAdrese.addresses[l].addressId)
                                        for (int m = 0; m < listaAdrese.addresses[l].products.Count(); m++)
                                        {
                                            tichete += listaAdrese.addresses[l].products[m].productName;
                                            if (m < listaAdrese.addresses[l].products.Count() - 1)
                                                tichete += "; ";
                                        } 
                                }
                                row[x] = tichete;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }
                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDateAdrese.CancelEdit();
                grDateAdrese.DataSource = dt;
                //grDate.DataBind();
                grDateAdrese.KeyFieldName = "TipAdresa; IdAdresaWiz";
                Session["Sodexo_Adrese"] = dt;

                General.SalveazaDate(dt, "tblTipAdresa_WebService");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAdrese_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Sodexo_Adrese"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDateAdrese.Columns[col.ColumnName] != null && grDateAdrese.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;        

                    }

                }

                e.Cancel = true;
                grDateAdrese.CancelEdit();
                Session["Sodexo_Adrese"] = dt;
                grDateAdrese.DataSource = dt;
                //grDate.DataBind();

                General.SalveazaDate(dt, "tblTipAdresa_WebService");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAdrese_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Sodexo_Adrese"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateAdrese.CancelEdit();
                Session["Sodexo_Adrese"] = dt;
                grDateAdrese.DataSource = dt;

                General.SalveazaDate(dt, "tblTipAdresa_WebService");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAdrese_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                //DataTable dt = Session["Sodexo_Adrese"] as DataTable;
                //if (dt.Columns["IdAuto"] != null)
                //{
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                //        e.NewValues["IdAuto"] = max;
                //    }
                //    else
                //        e.NewValues["IdAuto"] = 1;
                //}
            }
            catch (Exception ex)
            {

            }
        }

        //private void ArataMesaj(string mesaj)
        //{
        //    pnlCtl.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtl.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        //}



    }
}