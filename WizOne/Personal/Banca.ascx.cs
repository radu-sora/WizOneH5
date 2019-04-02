using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class Banca : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            DataList1.DataSource = table;               
            DataList1.DataBind();

            ASPxComboBox cmbBancaSal = DataList1.Items[0].FindControl("cmbBancaSal") as ASPxComboBox;
            ASPxComboBox cmbSucSal = DataList1.Items[0].FindControl("cmbSucSal") as ASPxComboBox;
            ASPxComboBox cmbBancaGar = DataList1.Items[0].FindControl("cmbBancaGar") as ASPxComboBox;
            ASPxComboBox cmbSucGar = DataList1.Items[0].FindControl("cmbSucGar") as ASPxComboBox;

            int sal = 0, gar = 0;
            if (ds.Tables[0].Rows[0]["F10018"] != DBNull.Value)
                sal = Convert.ToInt32(ds.Tables[0].Rows[0]["F10018"].ToString());
            if (ds.Tables[0].Rows[0]["F1001026"] != DBNull.Value)
                gar = Convert.ToInt32(ds.Tables[0].Rows[0]["F1001026"].ToString());

            cmbSucSal.DataSource = General.GetSucursale(Convert.ToInt32(cmbBancaSal.SelectedItem == null ? sal : cmbBancaSal.SelectedItem.Value));
            cmbSucSal.DataBindItems();

            cmbSucGar.DataSource = General.GetSucursale(Convert.ToInt32(cmbBancaGar.SelectedItem == null ? gar : cmbBancaGar.SelectedItem.Value));
            cmbSucGar.DataBindItems();

            string[] etichete = new string[7] { "lblIBANSal", "lblNrCard", "lblBancaSal", "lblSucursalaSal", "lblIBANGar", "lblBancaGar", "lblSucursalaGar" };
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = DataList1.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            string[] butoane = new string[4] { "btnContSal", "btnContSalIst", "btnContGar", "btnContGarIst"};
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = DataList1.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }
            General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));

        }

        protected void pnlCtlBanca_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "txtIBANSal":
                    if (param[1] != null && param[1].ToString().Length > 0)
                    {
                        if (param[1].Length != 24)
                        {
                            pnlCtlBanca.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lungime cont IBAN invalida");
                        }
                        else
                        {
                            VerificareIBAN(param[1]);
                            ds.Tables[0].Rows[0]["F10020"] = param[1];
                            ds.Tables[1].Rows[0]["F10020"] = param[1];
                            Session["InformatiaCurentaPersonal"] = ds;
                        }
                    }
                    break;
                case "txtNrCard":
                    ds.Tables[0].Rows[0]["F10055"] = param[1];
                    ds.Tables[1].Rows[0]["F10055"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbBancaSal":
                    cmbBancaSal_SelectedIndexChanged(param[1]);
                    ds.Tables[0].Rows[0]["F10018"] = param[1];
                    ds.Tables[1].Rows[0]["F10018"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbSucSal":
                    ds.Tables[0].Rows[0]["F10019"] = param[1];
                    ds.Tables[1].Rows[0]["F10019"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtIBANGar":
                    if (param[1] != null && param[1].ToString().Length > 0)
                    {
                        if (param[1].Length != 24)
                        {
                            pnlCtlBanca.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lungime cont IBAN invalida");
                        }
                        else
                        {
                            VerificareIBAN(param[1]);
                            ds.Tables[0].Rows[0]["F1001028"] = param[1];
                            ds.Tables[2].Rows[0]["F1001028"] = param[1];
                            Session["InformatiaCurentaPersonal"] = ds;
                        }
                    }
                    break;
                case "cmbBancaGar":
                    cmbBancaGar_SelectedIndexChanged(param[1]);
                    ds.Tables[0].Rows[0]["F1001026"] = param[1];
                    ds.Tables[2].Rows[0]["F1001026"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbSucGar":
                    ds.Tables[0].Rows[0]["F1001027"] = param[1];
                    ds.Tables[2].Rows[0]["F1001027"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "btnContSal":
                    ModifAvans((int)Constante.Atribute.BancaSalariu);
                    break;
                case "btnContGar":
                    ModifAvans((int)Constante.Atribute.BancaGarantii);
                    break;
            }
        }

        private void cmbBancaSal_SelectedIndexChanged(string val)
        {
            ASPxComboBox cmbSucSal = DataList1.Items[0].FindControl("cmbSucSal") as ASPxComboBox;
            cmbSucSal.DataSource = General.GetSucursale(Convert.ToInt32(val));
            cmbSucSal.DataBindItems();
        }

        private void cmbBancaGar_SelectedIndexChanged(string val)
        {
            ASPxComboBox cmbSucGar = DataList1.Items[0].FindControl("cmbSucGar") as ASPxComboBox;
            cmbSucGar.DataSource = General.GetSucursale(Convert.ToInt32(val));
            cmbSucGar.DataBindItems();
        }

        private void ModifAvans(int atribut)
        {
            string url = "~/Avs/Cereri.aspx";
            Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Session["MP_Avans"] = "true";
            Session["MP_Avans_Tab"] = "Banca";
            if (Page.IsCallback)
                ASPxWebControl.RedirectOnCallback(url);
            else
                Response.Redirect(url, false);
        }


        private void VerificareIBAN(string IBAN)
        {

            try
            {
                bool valideazaIban = IbanValid(IBAN);

                if (!valideazaIban)
                {
                    pnlCtlBanca.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Cont IBAN invalid");
                }
                else
                {
                    string bnc = IBAN.Substring(4, 4);
                    //var ent = ddsBanca.DataView.Cast<metaF075Banca>().Where(p => p.F07511 == bnc).FirstOrDefault();

                    //if (ent != null) cmbBancaSal.EditValue = ent.F07503;
                }



            }
            catch (Exception ex)
            {
                //wiIndicator.DeferedVisibility = false;
            }
        }

        public bool IbanValid(string source)
        {
            bool result = false;

            source = AnulareCaractere(source);
            if (source.Length != 24)
            {
                return false;
            }

            source = Reordonare(source);
            if (source.Length != 24)
            {
                return false;
            }
            source = Conversie(source);
            result = Validare(source);

            return result;

        }

        private static string Reordonare(string source)
        {
            return source.Substring(4) + source.Substring(0, 4);
        }

        private static string Conversie(string source)
        {
            string result = "";
            for (int i = 0; i < source.Length; i++)
            {
                result += Substitutie(source[i]);
            }
            return result;
        }

        private static bool Validare(string source)
        {
            BigInteger bigint = new BigInteger();
            BigInteger.TryParse(source, out bigint);
            BigInteger rest = BigInteger.Remainder(bigint, new BigInteger(97.0));

            return rest == 1;

        }

        private static string Substitutie(char SourceC)
        {
            string ResStr = "";
            switch (SourceC)
            {
                case 'A': ResStr = "10"; break;
                case 'B': ResStr = "11"; break;
                case 'C': ResStr = "12"; break;
                case 'D': ResStr = "13"; break;
                case 'E': ResStr = "14"; break;
                case 'F': ResStr = "15"; break;
                case 'G': ResStr = "16"; break;
                case 'H': ResStr = "17"; break;
                case 'I': ResStr = "18"; break;
                case 'J': ResStr = "19"; break;
                case 'K': ResStr = "20"; break;
                case 'L': ResStr = "21"; break;
                case 'M': ResStr = "22"; break;
                case 'N': ResStr = "23"; break;
                case 'O': ResStr = "24"; break;
                case 'P': ResStr = "25"; break;
                case 'Q': ResStr = "26"; break;
                case 'R': ResStr = "27"; break;
                case 'S': ResStr = "28"; break;
                case 'T': ResStr = "29"; break;
                case 'U': ResStr = "30"; break;
                case 'V': ResStr = "31"; break;
                case 'W': ResStr = "32"; break;
                case 'X': ResStr = "33"; break;
                case 'Y': ResStr = "34"; break;
                case 'Z': ResStr = "35"; break;

                case '0': ResStr = "0"; break;
                case '1': ResStr = "1"; break;
                case '2': ResStr = "2"; break;
                case '3': ResStr = "3"; break;
                case '4': ResStr = "4"; break;
                case '5': ResStr = "5"; break;
                case '6': ResStr = "6"; break;
                case '7': ResStr = "7"; break;
                case '8': ResStr = "8"; break;
                case '9': ResStr = "9"; break;
            };// 
            return ResStr;
        }
        private static string AnulareCaractere(string source)
        {
            //_isValid = false;
            source = source.Replace(" ", "");
            if (string.IsNullOrEmpty(source)) return source;

            if (source.Length != 24)
            {
                return "";
            }

            int j = 0;
            int i = 0;

            string test = "";
            while (i < source.Length)
            {
                switch (source[i])
                {
                    case 'A':
                    case 'a':
                    case 'B':
                    case 'b':
                    case 'C':
                    case 'c':
                    case 'D':
                    case 'd':
                    case 'E':
                    case 'e':
                    case 'F':
                    case 'f':
                    case 'G':
                    case 'g':
                    case 'H':
                    case 'h':
                    case 'I':
                    case 'i':
                    case 'J':
                    case 'j':
                    case 'K':
                    case 'k':
                    case 'L':
                    case 'l':
                    case 'M':
                    case 'm':
                    case 'N':
                    case 'n':
                    case 'O':
                    case 'o':
                    case 'P':
                    case 'p':
                    case 'Q':
                    case 'q':
                    case 'R':
                    case 'r':
                    case 'S':
                    case 's':
                    case 'T':
                    case 't':
                    case 'U':
                    case 'u':
                    case 'V':
                    case 'v':
                    case 'W':
                    case 'w':
                    case 'X':
                    case 'x':
                    case 'Y':
                    case 'y':
                    case 'Z':
                    case 'z':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        test += source[i];
                        i++; break;

                    default:
                        i++; break;
                };//	
            }
            j = source.Length - test.Length;// nr caractere non alfanumerice

            if (j != 0) //nr caractere non alfa numerice;
            {
                return "";
            }

            source = source.ToUpper();
            return source;

        }

    }
}