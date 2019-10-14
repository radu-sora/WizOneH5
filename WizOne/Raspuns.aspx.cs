using ProceseSec;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne
{
    public partial class Raspuns : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Session.Clear();
                    General.InitSessionVariables();

                    if (string.IsNullOrEmpty(Request["arg"]))
                        Response.Write(Dami.TraduCuvant("Date insuficiente"));
                    else
                    {
                        string txt = General.Decrypt_QueryString(Request["arg"]);

                        if (txt == "")
                            Response.Write(Dami.TraduCuvant("Date insuficiente"));
                        else
                        {
                            string[] arr = txt.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                            string cheie1 = arr[1];
                            string cheie2 = arr[5];
                            string tipActiune = arr[4];
                            string mail = arr[2];
                            string idCerere = arr[7];
                            string numePagina = arr[9];

                            if (cheie1.ToUpper() != "WIZ" || cheie2.ToUpper() != "ONE" || mail == "" || tipActiune == "" || idCerere == "")
                                Response.Write(Dami.TraduCuvant("Date insuficiente"));
                            else
                            {
                                string sqlUsr = $@"SELECT F70102 FROM USERS WHERE ""Mail""=@1
                                                UNION
                                                SELECT F70102 FROM USERS WHERE F10003 IN (SELECT F10003 FROM F100 WHERE F100894=@1)";       //Radu 10.10.2019 - am pus "F10003 IN "  in loc de "F10003 = "  deoarece pot fi mai multe marci cu acelasi e-mail in F100

                                int idUsr = Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlUsr, new object[] { mail }), -99));

                                switch (numePagina)
                                {
                                    case "Absente.Lista":
                                        {
                                            string sqlAdr = $@"SELECT X.""IdUser"", Y.F10003, X.""IdSuper"" AS ""Rol"" FROM (
                                                SELECT ""Pozitie"" AS ""Ordine"", ""IdUser"", (-1 * ""IdSuper"") AS ""IdSuper"", COALESCE(""Aprobat"",0) AS ""Aprobat"" FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=@2 AND ""IdUser"" = @1
                                                UNION
                                                SELECT ""Pozitie"" AS ""Ordine"", @1, 78 AS ""IdSuper"", COALESCE(""Aprobat"",0) AS ""Aprobat"" FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=@2 AND ""IdUser"" IN (SELECT Y.F70102 FROM ""Ptj_Cereri"" X
                                                                                                        INNER JOIN USERS Y ON X.F10003 = Y.F10003
                                                                                                        WHERE X.""Inlocuitor"" = (SELECT G.F10003 FROM USERS G WHERE G.F70102 =@1) AND X.""DataInceput"" <= GetDate() AND GetDate() <= X.""DataSfarsit""
                                                                                                        UNION
                                                                                                        SELECT ""IdUser"" FROM ""tblDelegari"" WHERE COALESCE(""IdModul"", -99) = 1 AND ""IdDelegat"" =@1 AND ""DataInceput"" <= GetDate() AND GetDate() <= ""DataSfarsit"")
                                                UNION
                                                SELECT 99, @1, 0, 1) X 
                                                LEFT JOIN USERS Y ON X.""IdUser""=Y.F70102
                                                ORDER BY X.""Aprobat"", X.""Ordine"" ";

                                            DataRow drAdr = General.IncarcaDR(sqlAdr, new object[] { idUsr, idCerere });
                                            if (drAdr == null)
                                            {
                                                Response.Write(Dami.TraduCuvant("Date insuficiente"));
                                            }
                                            else
                                            {
                                                int idUser = Convert.ToInt32(General.Nz(drAdr["IdUser"], -99));
                                                int marcaUser = Convert.ToInt32(General.Nz(drAdr["IdUser"], -99));
                                                int rol = Convert.ToInt32(General.Nz(drAdr["Rol"], -99));
                                                DataRow dr = General.IncarcaDR(@"SELECT * FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=@1 AND ""IdUser""=@2 AND (""Aprobat""=0 OR ""Aprobat"" IS NULL) AND ""Pozitie""=COALESCE((SELECT COALESCE(""Pozitie"",0) FROM ""Ptj_Cereri"" WHERE ""Id""=@1),0) + 1", new object[] { idCerere, idUser });
                                                if (dr != null && dr["DataAprobare"] != DBNull.Value)
                                                {
                                                    DateTime dtApr = Convert.ToDateTime(dr["DataAprobare"]);
                                                    Response.Write("Cererea a fost deja aprobata in data de " + dtApr.Day.ToString().PadLeft(2, '0') + "-" + dtApr.Month.ToString().PadLeft(2, '0') + "-" + dtApr.Year);
                                                }
                                                else
                                                {
                                                    List<General.metaCereriRol> lst = new List<General.metaCereriRol>();
                                                    lst.Add(new General.metaCereriRol { Id = Convert.ToInt32(idCerere), Rol = rol });
                                                    General.MemoreazaEroarea("Vine din Raspuns");
                                                    string rez = General.MetodeCereri(Convert.ToInt32(tipActiune), lst, idUser, marcaUser, "", rol);
                                                    Response.Write(rez);
                                                }
                                            }
                                        }
                                        break;
                                    case "Pontaj.PontajEchipa":
                                        {
                                            DataRow drCum = General.IncarcaDR(@"SELECT * FROM ""Ptj_Cumulat"" WHERE ""IdAuto""=@1", new object[] { idCerere });
                                            if (drCum == null)
                                            {
                                                Response.Write(Dami.TraduCuvant("Date insuficiente"));
                                                return;
                                            }

                                            int f10003 = Convert.ToInt32(General.Nz(drCum["F10003"], -99));
                                            int an = Convert.ToInt32(General.Nz(drCum["An"], -99));
                                            int luna = Convert.ToInt32(General.Nz(drCum["Luna"], -99));
                                            int idStare = Convert.ToInt32(General.Nz(drCum["IdStare"], -99));

                                            string sqlAdr =
                                                @"SELECT D.F70102 AS ""IdUser"", MAX(COALESCE(C.""IdRol"",1)) AS ""IdRol"", D.F10003 AS ""MarcaUser""
                                                FROM ""relGrupAngajat"" B
                                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                                INNER JOIN USERS D ON C.""IdSuper"" = D.F70102
                                                WHERE D.""Mail"" = @1 AND B.F10003 = @2
                                                GROUP BY D.F70102, D.F10003
                                                UNION
                                                SELECT D.F70102 AS ""IdUser"", MAX(COALESCE(C.""IdRol"", 1)) AS ""IdRol"", D.F10003 AS ""MarcaUser""
                                                FROM ""relGrupAngajat"" B
                                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")
                                                INNER JOIN USERS D ON J.""IdUser"" = D.F70102
                                                WHERE D.""Mail"" = @1 AND B.F10003 = @2
                                                GROUP BY D.F70102, D.F10003";

                                            DataRow drAdr = General.IncarcaDR(sqlAdr, new object[] { mail, f10003 });
                                            if (drAdr == null)
                                            {
                                                Response.Write(Dami.TraduCuvant("Date insuficiente"));
                                                return;
                                            }

                                            int idRol = Convert.ToInt32(General.Nz(drAdr["IdRol"], -99));
                                            int idUser = Convert.ToInt32(General.Nz(drAdr["IdUser"], -99));
                                            int marcaUser = Convert.ToInt32(General.Nz(drAdr["MarcaUser"], -99));

                                            DataRow dr = General.IncarcaDR(@"SELECT * FROM ""Ptj_CumulatIstoric"" WHERE ""F10003""=@1 AND ""An""=@2 AND ""Luna""=@3 AND ""IdUser""=@4 AND ""DataAprobare"" IS NOT NULL", new object[] { f10003, an, luna, idUser });
                                            if (dr != null)
                                            {
                                                DateTime dtApr = Convert.ToDateTime(dr["DataAprobare"]);
                                                Response.Write("Cererea a fost deja aprobata in data de " + dtApr.Day.ToString().PadLeft(2, '0') + "-" + dtApr.Month.ToString().PadLeft(2, '0') + "-" + dtApr.Year);
                                                return;
                                            }

                                            //ca sa fie functional si pt modulul de cereri si pt pontaj
                                            //in cereri respingerea este 2 iar in pontaj este 0
                                            if (tipActiune == "2") tipActiune = "0";
                                            string rez = General.ActiuniExec(Convert.ToInt32(tipActiune), f10003, idRol, idStare, an, luna, "Pontaj.Aprobare", idUser, marcaUser) + System.Environment.NewLine;
                                            Response.Write(rez);



                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}