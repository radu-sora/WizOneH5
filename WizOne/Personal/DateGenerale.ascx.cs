using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Configuration;

namespace WizOne.Personal
{
    public partial class DateGenerale : System.Web.UI.UserControl
    {

        protected void Page_Init(object sender, EventArgs e)
        {

            try
            {

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");


                

                #endregion

                DataTable table = new DataTable();

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                table = ds.Tables[0];

                //bindingSource1.DataSource = table;
                DateGenListView.DataSource = table;
                DateGenListView.DataBind();

                ASPxRadioButton chk1 = DateGenListView.Items[0].FindControl("chkM") as ASPxRadioButton;
                ASPxRadioButton chk2 = DateGenListView.Items[0].FindControl("chkF") as ASPxRadioButton;

                if (table.Rows[0]["F10047"].ToString() == "1")
                {
                    chk1.Checked = true;
                    chk2.Checked = false;
                }
                else
                {
                    chk1.Checked = false;
                    chk2.Checked = true;
                }

                ASPxTextBox txtMarca = DateGenListView.Items[0].FindControl("txtMarca") as ASPxTextBox;
                //txtMarca.ReadOnly = false;
                txtMarca.Text = Session["Marca"].ToString();
                txtMarca.DataBind();
                //txtMarca.ReadOnly = true;

                string[] etichete = new string[25] { "lblMarca", "lblCNP", "lblCNPVechi", "lblEID", "lblNrCtr", "lblNume", "lblPrenume", "lblNumeAnt", "lblPanaLa", "lblDataNasterii", "lblSex", "lblStructOrg", "lblCompanie",
                                                     "lblSubcompanie", "lblFiliala", "lblSectie", "lblDepartament", "lblSubdept", "lblBirouEchipa", "lblDataAng", "lblUltimaZiLucr", "lblMotivPlecare", "lblTimpPartial", "lblNorma", "lblCategAng"};
                for (int i = 0; i < etichete.Count(); i++)
                {
                    ASPxLabel lbl = DateGenListView.Items[0].FindControl(etichete[i]) as ASPxLabel;
                    lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
                }

                string[] butoaneRadio = new string[2] { "chkM", "chkF" };
                for (int i = 0; i < butoaneRadio.Count(); i++)
                {
                    ASPxRadioButton radio = DateGenListView.Items[0].FindControl(butoaneRadio[i]) as ASPxRadioButton;
                    radio.Text = Dami.TraduCuvant(radio.Text);
                }

                HtmlGenericControl lgAng = DateGenListView.Items[0].FindControl("lgAng") as HtmlGenericControl;
                lgAng.InnerText = Dami.TraduCuvant("Date despre angajare");
                HtmlGenericControl lgIdent = DateGenListView.Items[0].FindControl("lgIdent") as HtmlGenericControl;
                lgIdent.InnerText = Dami.TraduCuvant("Date unice de identificare");
                HtmlGenericControl lgSex = DateGenListView.Items[0].FindControl("lgSex") as HtmlGenericControl;
                lgSex.InnerText = Dami.TraduCuvant("Data nasterii si Sex");
                HtmlGenericControl lgNume = DateGenListView.Items[0].FindControl("lgNume") as HtmlGenericControl;
                lgNume.InnerText = Dami.TraduCuvant("Nume si prenume");

                General.SecuritatePersonal(DateGenListView, Convert.ToInt32(Session["UserId"].ToString()));

            }
            catch (Exception ex)
            {
                ////MessageBox.Show(this, ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }

        }


        protected void pnlCtlDateGen_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "txtCNP":
                    txtCNP_LostFocus();
                    //if (txtCNP_LostFocus())
                    //{
                    ds.Tables[0].Rows[0]["F10017"] = param[1];
                    ds.Tables[1].Rows[0]["F10017"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    //}
                    break;
                case "txtCNPVechi":
                    if (txtCNP_LostFocus())
                    {
                        ds.Tables[0].Rows[0]["F100171"] = param[1];
                        ds.Tables[1].Rows[0]["F100171"] = param[1];
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "txtEID":
                    ds.Tables[0].Rows[0]["F100901"] = param[1];
                    ds.Tables[1].Rows[0]["F100901"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNrCtr":
                    ds.Tables[0].Rows[0]["F100985"] = param[1];
                    ds.Tables[1].Rows[0]["F100985"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNumeDG":
                    ds.Tables[0].Rows[0]["F10008"] = param[1];
                    ds.Tables[1].Rows[0]["F10008"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtPrenumeDG":
                    ds.Tables[0].Rows[0]["F10009"] = param[1];
                    ds.Tables[1].Rows[0]["F10009"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNumeAntDG":
                    ds.Tables[0].Rows[0]["F100905"] = param[1];
                    ds.Tables[1].Rows[0]["F100905"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "dePanaLa":
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100906"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100906"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataNasteriiDG":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F10021"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F10021"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbStructOrg":
                    cmbStructOrg_SelectedIndexChanged();
                    break;
                case "deDataAngDG":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F10022"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F10022"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deUltimaZiLucrDG":
                    data = param[1].Split('.');
                    DateTime ultimaZiLucr = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[0].Rows[0]["F10023"] = ultimaZiLucr;
                    ds.Tables[1].Rows[0]["F10023"] = ultimaZiLucr;
                    DateTime dataPlec = Convert.ToDateTime(ds.Tables[1].Rows[0]["F100993"].ToString());
                    dataPlec = ultimaZiLucr.AddDays(1);
                    ds.Tables[0].Rows[0]["F100993"] = dataPlec;
                    ds.Tables[1].Rows[0]["F100993"] = dataPlec;
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbMotivPlecareDG":
                    ds.Tables[0].Rows[0]["F10025"] = param[1];
                    ds.Tables[1].Rows[0]["F10025"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTimpPartialDG":
                    if (cmbTimpPartial_SelectedIndexChanged())
                    {
                        ds.Tables[0].Rows[0]["F10043"] = param[1];
                        ds.Tables[1].Rows[0]["F10043"] = param[1];
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "cmbNormaDG":
                    if (cmbNorma_LostFocus())
                    {
                        ds.Tables[0].Rows[0]["F100973"] = param[1];
                        ds.Tables[1].Rows[0]["F100973"] = param[1];
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "cmbCategAng":
                    ds.Tables[0].Rows[0]["F10061"] = param[1];
                    ds.Tables[1].Rows[0]["F10061"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
            }
        }

        protected void cmbStructOrg_SelectedIndexChanged()
        {
            ASPxComboBox cmbStructOrg = DateGenListView.Items[0].FindControl("cmbStructOrg") as ASPxComboBox;
            //ASPxComboBox cmbCompanie = DataList1.Items[0].FindControl("cmbCompanie") as ASPxComboBox;
            ASPxComboBox cmbSubcompanie = DateGenListView.Items[0].FindControl("cmbSubcompanie") as ASPxComboBox;
            ASPxComboBox cmbFiliala = DateGenListView.Items[0].FindControl("cmbFiliala") as ASPxComboBox;
            ASPxComboBox cmbSectie = DateGenListView.Items[0].FindControl("cmbSectie") as ASPxComboBox;
            ASPxComboBox cmbDepartament = DateGenListView.Items[0].FindControl("cmbDepartament") as ASPxComboBox;
            ASPxComboBox cmbSubdept = DateGenListView.Items[0].FindControl("cmbSubdept") as ASPxComboBox;
            ASPxComboBox cmbBirouEchipa = DateGenListView.Items[0].FindControl("cmbBirouEchipa") as ASPxComboBox;

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

            ListEditItem linie = cmbStructOrg.SelectedItem;
            if (linie != null)
            {
                int subc = linie.GetValue("F00304").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00304").ToString()) : -1;
                int fil = linie.GetValue("F00405").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00405").ToString()) : -1;
                int sec = linie.GetValue("F00506").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00506").ToString()) : -1;
                int dept = linie.GetValue("F00607").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00607").ToString()) : -1;
                int subdept = linie.GetValue("F00708").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00708").ToString()) : -1;
                int birou = linie.GetValue("F00809").ToString().Length > 0 ? Convert.ToInt32(linie.GetValue("F00809").ToString()) : -1;

                if (subc >= 0)
                {
                    for (int i = 0; i < cmbSubcompanie.Items.Count; i++)
                        if (Convert.ToInt32(cmbSubcompanie.Items[i].Value) == subc)
                        {
                            cmbSubcompanie.SelectedIndex = i;
                            break;
                        }
                }
                else
                {
                    cmbSubcompanie.SelectedIndex = -1;
                    subc = 0;
                }

                if (fil >= 0)
                {
                    for (int i = 0; i < cmbFiliala.Items.Count; i++)
                        if (Convert.ToInt32(cmbFiliala.Items[i].Value) == fil)
                        {
                            cmbFiliala.SelectedIndex = i;
                            break;
                        }
                }
                else
                {
                    cmbFiliala.SelectedIndex = -1;
                    fil = 0;
                }

                if (sec >= 0)
                {
                    for (int i = 0; i < cmbSectie.Items.Count; i++)
                        if (Convert.ToInt32(cmbSectie.Items[i].Value) == sec)
                        {
                            cmbSectie.SelectedIndex = i;
                            break;
                        }
                }
                else
                {
                    cmbSectie.SelectedIndex = -1;
                    sec = 0;
                }

                if (dept >= 0)
                {
                    for (int i = 0; i < cmbDepartament.Items.Count; i++)
                        if (Convert.ToInt32(cmbDepartament.Items[i].Value) == dept)
                        {
                            cmbDepartament.SelectedIndex = i;
                            break;
                        }
                }
                else
                {
                    cmbDepartament.SelectedIndex = -1;
                    dept = 0;
                }

                if (subdept >= 0)
                {
                    for (int i = 0; i < cmbSubdept.Items.Count; i++)
                        if (Convert.ToInt32(cmbSubdept.Items[i].Value) == subdept)
                        {
                            cmbSubdept.SelectedIndex = i;
                            break;
                        }
                }
                else
                {
                    cmbSubdept.SelectedIndex = -1;
                    subdept = 0;
                }

                if (birou >= 0)
                {
                    for (int i = 0; i < cmbBirouEchipa.Items.Count; i++)
                        if (Convert.ToInt32(cmbBirouEchipa.Items[i].Value) == birou)
                        {
                            cmbBirouEchipa.SelectedIndex = i;
                            break;
                        }
                }
                else
                {
                    cmbBirouEchipa.SelectedIndex = -1;
                    birou = 0;
                }
                ds.Tables[0].Rows[0]["F10004"] = subc;
                ds.Tables[0].Rows[0]["F10005"] = fil;
                ds.Tables[0].Rows[0]["F10006"] = sec;
                ds.Tables[0].Rows[0]["F10007"] = dept;
                ds.Tables[0].Rows[0]["F100958"] = subdept;
                ds.Tables[0].Rows[0]["F100959"] = birou;

                ds.Tables[1].Rows[0]["F10004"] = subc;
                ds.Tables[1].Rows[0]["F10005"] = fil;
                ds.Tables[1].Rows[0]["F10006"] = sec;
                ds.Tables[1].Rows[0]["F10007"] = dept;
                ds.Tables[2].Rows[0]["F100958"] = subdept;
                ds.Tables[2].Rows[0]["F100959"] = birou;

                Session["InformatiaCurentaPersonal"] = ds;
            }
        }



        private bool txtCNP_LostFocus()
        {
            bool valid = false;
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            try
            {
                ASPxTextBox txtCNP = DateGenListView.Items[0].FindControl("txtCNP") as ASPxTextBox;
                if (txtCNP.Text != null && txtCNP.Text != "")
                {
                    string cnp = txtCNP.Text.ToString();

                    if (General.VerificaCNP(cnp))
                    {
                        valid = true;
                        DataNasterii(cnp);
                        ASPxRadioButton chk1 = DateGenListView.Items[0].FindControl("chkM") as ASPxRadioButton;
                        ASPxRadioButton chk2 = DateGenListView.Items[0].FindControl("chkF") as ASPxRadioButton;

                        int sex = 1;
                        if ((Convert.ToInt32(cnp[0]) % 2) != 0)
                        {
                            sex = 1;
                            chk1.Checked = true;
                            chk2.Checked = false;
                        }
                        else
                        {
                            sex = 2;
                            chk1.Checked = false;
                            chk2.Checked = true;
                        }

                        //Florin 2018.11.22
                        //ds.Tables[0].Rows[0]["F10047"] = cnp[0];
                        //ds.Tables[1].Rows[0]["F10047"] = cnp[0];
                        ds.Tables[0].Rows[0]["F10047"] = sex;
                        ds.Tables[1].Rows[0]["F10047"] = sex;


                        Session["InformatiaCurentaPersonal"] = ds;

                        //operation = ctxOri.GetMarcaUnica(txtCNP.Text);
                        //operation.Completed += (s, args) =>
                        //{
                        //    if (!operation.HasError)
                        //    {
                        //        txtMarcaUnica.Text = operation.Value.ToString();
                        //        pagAD.txtMarca.Text = operation.Value.ToString();
                        //        txtMarcaUnica.IsEnabled = false;

                        //        if (esteNou)
                        //        {
                        //            srvBuiltIn ctxBuiltIn = new srvBuiltIn();
                        //            LoadOperation<F100> lo = ctxBuiltIn.Load(ctxBuiltIn.GetF100Query().Where(p => p.F10017 == cnp), LoadBehavior.RefreshCurrent, op =>
                        //            {
                        //                if (op.Entities.Count() > 0)
                        //                {
                        //                    CustomMessage msg = new CustomMessage(Dami.TraduCuvant("CNP-ul exista deja. Doriti preluarea datelor ultimului contract activ? "), CustomMessage.MessageType.Confirm);
                        //                    msg.OKButton.Click += (send, arg) =>
                        //                    {
                        //                        pagAD.PreluareDateContract(Convert.ToInt32(F10003), cnp);
                        //                    };
                        //                    msg.Show();
                        //                }
                        //            }, null);
                        //        }
                        //    }
                        //};
                        //return;

                    }
                    else
                    {
                        //WizOne.Module.MessageBox.Show("CNP invalid", WizOne.Module.MessageBox.icoError);                        
                        //MessageBox.Show("CNP invalid", MessageBox.icoSuccess);
                        pnlCtlDateGen.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("CNP invalid");




                        //deDataNasterii.DateTime = DateTime.Now;
                        //operation = ctxOri.GetMarcaUnica(txtCNP.Text);
                        //operation.Completed += (s, args) =>
                        //{
                        //    if (!operation.HasError)
                        //    {
                        //        txtMarcaUnica.Text = operation.Value.ToString();
                        //        txtMarcaUnica.IsEnabled = false;
                        //    }
                        //};
                        //_dataNasterii = deDataNasterii.DateTime;
                        //if (pagAD != null)
                        //{
                        //    pagAD.txtCNP.Text = "";
                        //    pagAD.txtCNPText.Visibility = Visibility.Collapsed;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        //private void ArataMesaj(string mesaj)
        //{
        //    pnlCtlDateGen.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtlDateGen.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));
        //}

        private bool cmbTimpPartial_SelectedIndexChanged()
        {
            bool valid = true;
            try
            {
                ASPxComboBox cmbNorma = DateGenListView.Items[0].FindControl("cmbNorma") as ASPxComboBox;
                ASPxComboBox cmbTimpPartial = DateGenListView.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
                if (cmbTimpPartial.Value != null && cmbNorma.Value != null && Convert.ToInt32(cmbNorma.Value.ToString()) < Convert.ToInt32(cmbTimpPartial.Value.ToString()))
                {
                    //MessageBox.Show("Timpul partial este mai mic decat norma!", MessageBox.icoError);
                    pnlCtlDateGen.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timpul partial este mai mic decat norma!");
                    cmbTimpPartial.SelectedItem.Value = 1;
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        private bool cmbNorma_LostFocus()
        {
            bool valid = true;
            try
            {
                ASPxComboBox cmbNorma = DateGenListView.Items[0].FindControl("cmbNorma") as ASPxComboBox;
                ASPxComboBox cmbTimpPartial = DateGenListView.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
                if (cmbNorma.Value == null || (cmbNorma.Value.ToString().Trim().Length <= 0))
                {
                    //MessageBox.Show("Nu ati completat norma!", MessageBox.icoError);
                    pnlCtlDateGen.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat norma!");
                    cmbNorma.SelectedIndex = cmbNorma.Items.Count - 1;
                    cmbNorma.Focus();
                    valid = false;
                }
                else
                {
                    cmbTimpPartial.SelectedItem.Value = Convert.ToInt32(cmbNorma.Value.ToString());
                }


                if (cmbNorma.Value != null)
                    cmbTimpPartial.SelectedItem.Value = Convert.ToInt32(cmbNorma.Value.ToString());
                else
                    cmbTimpPartial.SelectedItem.Value = 1;
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        private void DataNasterii(string cnp)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DateTime dtN = General.getDataNasterii(cnp);
                ASPxDateEdit deDataNasterii = DateGenListView.Items[0].FindControl("deDataNasteriiDG") as ASPxDateEdit;
                deDataNasterii.Value = dtN;
                //_dataNasterii = deDataNasterii.DateTime;

                ds.Tables[0].Rows[0]["F10021"] = new DateTime(dtN.Year, dtN.Month, dtN.Day);
                ds.Tables[1].Rows[0]["F10021"] = new DateTime(dtN.Year, dtN.Month, dtN.Day);
                Session["InformatiaCurentaPersonal"] = ds;

            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }



    							//<dx:ASPxComboBox DataSourceID = "dsN"  Value='<%#Eval("F100973") %>' ID="cmbNormaDG" Width="100" runat="server"   TextField="Denumire" ValueField="Id"  AutoPostBack="false" ValueType="System.Int32" >
           //                     <ClientSideEvents SelectedIndexChanged = "function(s,e){ OnValueChangedHandlerCtr(s); }" />

           //                 </ dx:ASPxComboBox>



    							//<dx:ASPxComboBox DataSourceID = "dsCategAng"  Value='<%#Eval("F10061") %>' ID="cmbCategAng"   runat="server" DropDownStyle="DropDown"  TextField="F72404" ValueField="F72402" ValueType="System.Int32">
           //                     <ClientSideEvents SelectedIndexChanged = "function(s,e){ OnValueChangedHandlerDG(s); }" />

           //                 </ dx:ASPxComboBox >


}