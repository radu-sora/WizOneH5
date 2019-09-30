using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace WizOne.Module
{
    public class Constante
    {
        //public const string cnWeb = @"User Id=sa;Password=123qwe.;Persist security info=false;Initial catalog=TEST_HGR;Data source=VM-WIZSAL2012\WIZSALARY";
        //"User Id=scott;Password=tiger;Data Source=oracle;"

        //Variabile utilizate pentru criptare
        public const int ENCRYPT = 1;
        public const int DECRYPT = 2;
        public const string cheieCriptare = "WizOne2016";

        public static int tipBD = 1;        //1 = SQL;  2 = Oracle
        public static int idClient = 1;

        public static string cnnWeb = "";
        public static string BD = "";

        public static string DefaultTheme = "SoftOrange";
        public static string CurrentThemeCookieKey = "DXCurrentTheme";

        public enum TipNotificare : int {
            Notificare = 1,
            Validare = 2,
            Remainder = 3
        }

        public enum MesajeValidari : int {
            Avertisment = 1,
            MesajDeEroare = 2
        }

        public enum ArataControl : int
        {
            Ascuns = 1,
            Obligatoriu = 2,
            Optional = 3
        }

        public enum IdCompensareDefault : int
        {
            LaBanca = -13,
            LaPlata = -14,
        }

        public enum TipModificarePontaj : int
        {
            DinCereri = 1,
            DinInitializare = 2,
            DinCalcul = 3,
            ModificatManual = 4
        }


        public const string CuloareDinCereri = "#e6e696";                   //galben pal
        public const string CuloareDinInitializare = "#c8fac8";             //verde pal
        public const string CuloareDinCalcul = "#c8dcfa";                   //albastru pal
        public const string CuloareModificatManual = "#e6c8fa";             //mov pal

        //Florin 2019.09.26
        //am modificat Componente = 27 in Componente = 19 ca in wizsalary

        //Radu
        public enum Atribute : int
        {
            Salariul = 1,
            Functie = 2,
            CodCOR = 3,
            MotivPlecare = 4,
            Organigrama = 5,
            Norma = 6,
            ContrIn = 8,
            ContrITM = 9,
            DataAngajarii = 10,
            Sporuri = 11,
            TitluAcademic = 12,
            MesajPersonal = 13,
            CentrulCost = 14,
            SporTranzactii = 15,
            PunctLucru = 16,
            Meserie = 22,
            PrelungireCIM = 25,
            PrelungireCIM_Vanz = 26,
            Componente = 19,
            Tarife = 28,
            NumePrenume = 101,
            CASS = 102,
            Studii = 103,
            BancaSalariu = 104,
            BancaGarantii = 105,
            LimbiStraine = 106,
            DocId = 107,
            PermisAuto = 108,
            BonusTeamLeader = 109

        }

        public const string lstValuri = "Val0;Val1;Val2;Val3;Val4;Val5;Val6;Val7;Val8;Val9;Val10;Val11;Val12;Val13;Val14;Val15;Val16;Val17;Val18;Val19;Val20;";
        public const string lstFuri = "F1;F2;F3;F4;F5;F6;F7;F8;F9;F10;F11;F12;F13;F14;F15;F16;F17;F18;F19;F20;F21;F22;F23;F24;F25;F26;F27;F28;F29;F30;F31;F32;F33;F34;F35;F36;F37;F38;F39;F40;F41;F42;F43;F44;F45;F46;F47;F48;F49;F50;F51;F52;F53;F54;F55;F56;F57;F58;F59;F60;";
        public static System.Data.DataTable lstSec;

        public static string campuriGDPR = "";
        public static string campuriGDPR_Strip = "";

        public static bool esteTactil = false;

        public static string versiune = "1.1.001";

    }
}