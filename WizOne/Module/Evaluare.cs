using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WizOne.Module
{
    #region Classes
    public class Eval_RaspunsLinii
    {
        public int IdQuiz { get; set; }
        public int F10003 { get; set; }
        public int Id { get; set; }
        public int Linia { get; set; }
        public string Super1 { get; set; }
        public string Super2 { get; set; }
        public string Super3 { get; set; }
        public string Super4 { get; set; }
        public string Super5 { get; set; }
        public string Super6 { get; set; }
        public string Super7 { get; set; }
        public string Super8 { get; set; }
        public string Super9 { get; set; }
        public string Super10 { get; set; }
        public string Super11 { get; set; }
        public string Super12 { get; set; }
        public string Super13 { get; set; }
        public string Super14 { get; set; }
        public string Super15 { get; set; }
        public string Super16 { get; set; }
        public string Super17 { get; set; }
        public string Super18 { get; set; }
        public string Super19 { get; set; }
        public string Super20 { get; set; }
        public int USER_NO { get; set; }
        public DateTime TIME { get; set; }
        public string Descriere { get; set; }
        public int TipData { get; set; }
        public int TipValoare { get; set; }
        public int Sublinia { get; set; }
        public string Tinta { get; set; }

        public string Super1_1 { get; set; }
        public string Super1_2 { get; set; }
        public string Super1_3 { get; set; }
        public string Super2_1 { get; set; }
        public string Super2_2 { get; set; }
        public string Super2_3 { get; set; }
        public string Super3_1 { get; set; }
        public string Super3_2 { get; set; }
        public string Super3_3 { get; set; }
        public string Super4_1 { get; set; }
        public string Super4_2 { get; set; }
        public string Super4_3 { get; set; }
        public string Super5_1 { get; set; }
        public string Super5_2 { get; set; }
        public string Super5_3 { get; set; }
        public string Super6_1 { get; set; }
        public string Super6_2 { get; set; }
        public string Super6_3 { get; set; }
        public string Super7_1 { get; set; }
        public string Super7_2 { get; set; }
        public string Super7_3 { get; set; }
        public string Super8_1 { get; set; }
        public string Super8_2 { get; set; }
        public string Super8_3 { get; set; }
        public string Super9_1 { get; set; }
        public string Super9_2 { get; set; }
        public string Super9_3 { get; set; }
        public string Super10_1 { get; set; }
        public string Super10_2 { get; set; }
        public string Super10_3 { get; set; }
        public int IdGrup { get; set; }
        public int PondereRatingGlobal { get; set; }
        public string NumeGrup { get; set; }
        public string Super11_1 { get; set; }
        public string Super11_2 { get; set; }
        public string Super11_3 { get; set; }
        public string Super12_1 { get; set; }
        public string Super12_2 { get; set; }
        public string Super12_3 { get; set; }
        public string Super13_1 { get; set; }
        public string Super13_2 { get; set; }
        public string Super13_3 { get; set; }
        public string Super14_1 { get; set; }
        public string Super14_2 { get; set; }
        public string Super14_3 { get; set; }
        public string Super15_1 { get; set; }
        public string Super15_2 { get; set; }
        public string Super15_3 { get; set; }
        public string Super16_1 { get; set; }
        public string Super16_2 { get; set; }
        public string Super16_3 { get; set; }
        public string Super17_1 { get; set; }
        public string Super17_2 { get; set; }
        public string Super17_3 { get; set; }
        public string Super18_1 { get; set; }
        public string Super18_2 { get; set; }
        public string Super18_3 { get; set; }
        public string Super19_1 { get; set; }
        public string Super19_2 { get; set; }
        public string Super19_3 { get; set; }
        public string Super20_1 { get; set; }
        public string Super20_2 { get; set; }
        public string Super20_3 { get; set; }
        public string Super1_4 { get; set; }
        public string Super1_5 { get; set; }
        public string Super1_6 { get; set; }
        public string Super2_4 { get; set; }
        public string Super2_5 { get; set; }
        public string Super2_6 { get; set; }
        public string Super3_4 { get; set; }
        public string Super3_5 { get; set; }
        public string Super3_6 { get; set; }
        public string Super4_4 { get; set; }
        public string Super4_5 { get; set; }
        public string Super4_6 { get; set; }
        public string Super5_4 { get; set; }
        public string Super5_5 { get; set; }
        public string Super5_6 { get; set; }
        public string Super6_4 { get; set; }
        public string Super6_5 { get; set; }
        public string Super6_6 { get; set; }
        public string Super7_4 { get; set; }
        public string Super7_5 { get; set; }
        public string Super7_6 { get; set; }
        public string Super8_4 { get; set; }
        public string Super8_5 { get; set; }
        public string Super8_6 { get; set; }
        public string Super9_4 { get; set; }
        public string Super9_5 { get; set; }
        public string Super9_6 { get; set; }
        public string Super10_4 { get; set; }
        public string Super10_5 { get; set; }
        public string Super10_6 { get; set; }
        public string Super11_4 { get; set; }
        public string Super11_5 { get; set; }
        public string Super11_6 { get; set; }
        public string Super12_4 { get; set; }
        public string Super12_5 { get; set; }
        public string Super12_6 { get; set; }
        public string Super13_4 { get; set; }
        public string Super13_5 { get; set; }
        public string Super13_6 { get; set; }
        public string Super14_4 { get; set; }
        public string Super14_5 { get; set; }
        public string Super14_6 { get; set; }
        public string Super15_4 { get; set; }
        public string Super15_5 { get; set; }
        public string Super15_6 { get; set; }
        public string Super16_4 { get; set; }
        public string Super16_5 { get; set; }
        public string Super16_6 { get; set; }
        public string Super17_4 { get; set; }
        public string Super17_5 { get; set; }
        public string Super17_6 { get; set; }
        public string Super18_4 { get; set; }
        public string Super18_5 { get; set; }
        public string Super18_6 { get; set; }
        public string Super19_4 { get; set; }
        public string Super19_5 { get; set; }
        public string Super19_6 { get; set; }
        public string Super20_4 { get; set; }
        public string Super20_5 { get; set; }
        public string Super20_6 { get; set; }
        public string DescriereInRatingGlobal { get; set; }

        public Eval_RaspunsLinii() { }

        public Eval_RaspunsLinii(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString() == string.Empty ? "-99" : dr["IdQuiz"].ToString()) : -99;
            F10003 = columns.Contains("F10003") == true ? Convert.ToInt32(dr["F10003"].ToString() == string.Empty ? "-99" : dr["F10003"].ToString()) : -99;
            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            Linia = columns.Contains("Linia") == true ? Convert.ToInt32(dr["Linia"].ToString() == string.Empty ? "-99" : dr["Linia"].ToString()) : -99;
            Super1 = columns.Contains("Super1") == true ? dr["Super1"].ToString() : "";
            Super2 = columns.Contains("Super2") == true ? dr["Super2"].ToString() : "";
            Super3 = columns.Contains("Super3") == true ? dr["Super3"].ToString() : "";
            Super4 = columns.Contains("Super4") == true ? dr["Super4"].ToString() : "";
            Super5 = columns.Contains("Super5") == true ? dr["Super5"].ToString() : "";
            Super6 = columns.Contains("Super6") == true ? dr["Super6"].ToString() : "";
            Super7 = columns.Contains("Super7") == true ? dr["Super7"].ToString() : "";
            Super8 = columns.Contains("Super8") == true ? dr["Super8"].ToString() : "";
            Super9 = columns.Contains("Super9") == true ? dr["Super9"].ToString() : "";
            Super10 = columns.Contains("Super10") == true ? dr["Super10"].ToString() : "";
            Super11 = columns.Contains("Super11") == true ? dr["Super11"].ToString() : "";
            Super12 = columns.Contains("Super12") == true ? dr["Super12"].ToString() : "";
            Super13 = columns.Contains("Super13") == true ? dr["Super13"].ToString() : "";
            Super14 = columns.Contains("Super14") == true ? dr["Super14"].ToString() : "";
            Super15 = columns.Contains("Super15") == true ? dr["Super15"].ToString() : "";
            Super16 = columns.Contains("Super16") == true ? dr["Super16"].ToString() : "";
            Super17 = columns.Contains("Super17") == true ? dr["Super17"].ToString() : "";
            Super18 = columns.Contains("Super18") == true ? dr["Super18"].ToString() : "";
            Super19 = columns.Contains("Super19") == true ? dr["Super19"].ToString() : "";
            Super20 = columns.Contains("Super20") == true ? dr["Super20"].ToString() : "";
            //USER_NO = columns.Contains("Id") == true ? dr["Id"].ToString() : "";
            //TIME = columns.Contains("Id") == true ? dr["Id"].ToString()) : -99;
            Descriere = columns.Contains("Descriere") == true ? dr["Descriere"].ToString() : "";
            TipData = columns.Contains("TipData") == true ? Convert.ToInt32(dr["TipData"].ToString() == string.Empty ? "-99" : dr["TipData"].ToString()) : -99;
            TipValoare = columns.Contains("TipValoare") == true ? Convert.ToInt32(dr["TipValoare"].ToString() == string.Empty ? "-99" : dr["TipValoare"].ToString()) : -99;
            Sublinia = columns.Contains("Sublinia") == true ? Convert.ToInt32(dr["Sublinia"].ToString() == string.Empty ? "-99" : dr["Sublinia"].ToString()) : -99;
            Tinta = columns.Contains("Tinta") == true ? dr["Tinta"].ToString() : "";
            Super1_1 = columns.Contains("Super1_1") == true ? dr["Super1_1"].ToString() : "";
            Super1_2 = columns.Contains("Super1_2") == true ? dr["Super1_2"].ToString() : "";
            Super1_3 = columns.Contains("Super1_3") == true ? dr["Super1_3"].ToString() : "";
            Super2_1 = columns.Contains("Super2_1") == true ? dr["Super2_1"].ToString() : "";
            Super2_2 = columns.Contains("Super2_2") == true ? dr["Super2_2"].ToString() : "";
            Super2_3 = columns.Contains("Super2_3") == true ? dr["Super2_3"].ToString() : "";
            Super3_1 = columns.Contains("Super3_1") == true ? dr["Super3_1"].ToString() : "";
            Super3_2 = columns.Contains("Super3_2") == true ? dr["Super3_2"].ToString() : "";
            Super3_3 = columns.Contains("Super3_3") == true ? dr["Super3_3"].ToString() : "";
            Super4_1 = columns.Contains("Super4_1") == true ? dr["Super4_1"].ToString() : "";
            Super4_2 = columns.Contains("Super4_2") == true ? dr["Super4_2"].ToString() : "";
            Super4_3 = columns.Contains("Super4_3") == true ? dr["Super4_3"].ToString() : "";
            Super5_1 = columns.Contains("Super5_1") == true ? dr["Super5_1"].ToString() : "";
            Super5_2 = columns.Contains("Super5_2") == true ? dr["Super5_2"].ToString() : "";
            Super5_3 = columns.Contains("Super5_3") == true ? dr["Super5_3"].ToString() : "";
            Super6_1 = columns.Contains("Super6_1") == true ? dr["Super6_1"].ToString() : "";
            Super6_2 = columns.Contains("Super6_2") == true ? dr["Super6_2"].ToString() : "";
            Super6_3 = columns.Contains("Super6_3") == true ? dr["Super6_3"].ToString() : "";
            Super7_1 = columns.Contains("Super7_1") == true ? dr["Super7_1"].ToString() : "";
            Super7_2 = columns.Contains("Super7_2") == true ? dr["Super7_2"].ToString() : "";
            Super7_3 = columns.Contains("Super7_3") == true ? dr["Super7_3"].ToString() : "";
            Super8_1 = columns.Contains("Super8_1") == true ? dr["Super8_1"].ToString() : "";
            Super8_2 = columns.Contains("Super8_2") == true ? dr["Super8_2"].ToString() : "";
            Super8_3 = columns.Contains("Super8_3") == true ? dr["Super8_3"].ToString() : "";
            Super9_1 = columns.Contains("Super9_1") == true ? dr["Super9_1"].ToString() : "";
            Super9_2 = columns.Contains("Super9_2") == true ? dr["Super9_2"].ToString() : "";
            Super9_3 = columns.Contains("Super9_3") == true ? dr["Super9_3"].ToString() : "";
            Super10_1 = columns.Contains("Super10_1") == true ? dr["Super10_1"].ToString() : "";
            Super10_2 = columns.Contains("Super10_2") == true ? dr["Super10_2"].ToString() : "";
            Super10_3 = columns.Contains("Super10_3") == true ? dr["Super10_3"].ToString() : "";
            IdGrup = columns.Contains("IdGrup") == true ? Convert.ToInt32(dr["IdGrup"].ToString() == string.Empty ? "-99" : dr["IdGrup"].ToString()) : -99;
            PondereRatingGlobal = columns.Contains("PondereRatingGlobal") == true ? Convert.ToInt32(dr["PondereRatingGlobal"].ToString() == string.Empty ? "-99" : dr["PondereRatingGlobal"].ToString()) : -99;
            NumeGrup = columns.Contains("NumeGrup") == true ? dr["NumeGrup"].ToString() : "";
            Super11_1 = columns.Contains("Super11_1") == true ? dr["Super11_1"].ToString() : "";
            Super11_2 = columns.Contains("Super11_2") == true ? dr["Super11_2"].ToString() : "";
            Super11_3 = columns.Contains("Super11_3") == true ? dr["Super11_3"].ToString() : "";
            Super12_1 = columns.Contains("Super12_1") == true ? dr["Super12_1"].ToString() : "";
            Super12_2 = columns.Contains("Super12_2") == true ? dr["Super12_2"].ToString() : "";
            Super12_3 = columns.Contains("Super12_3") == true ? dr["Super12_3"].ToString() : "";
            Super13_1 = columns.Contains("Super13_1") == true ? dr["Super13_1"].ToString() : "";
            Super13_2 = columns.Contains("Super13_2") == true ? dr["Super13_2"].ToString() : "";
            Super13_3 = columns.Contains("Super13_3") == true ? dr["Super13_3"].ToString() : "";
            Super14_1 = columns.Contains("Super14_1") == true ? dr["Super14_1"].ToString() : "";
            Super14_2 = columns.Contains("Super14_2") == true ? dr["Super14_2"].ToString() : "";
            Super14_3 = columns.Contains("Super14_3") == true ? dr["Super14_3"].ToString() : "";
            Super15_1 = columns.Contains("Super15_1") == true ? dr["Super15_1"].ToString() : "";
            Super15_2 = columns.Contains("Super15_2") == true ? dr["Super15_2"].ToString() : "";
            Super15_3 = columns.Contains("Super15_3") == true ? dr["Super15_3"].ToString() : "";
            Super16_1 = columns.Contains("Super16_1") == true ? dr["Super16_1"].ToString() : "";
            Super16_2 = columns.Contains("Super16_2") == true ? dr["Super16_2"].ToString() : "";
            Super16_3 = columns.Contains("Super16_3") == true ? dr["Super16_3"].ToString() : "";
            Super17_1 = columns.Contains("Super17_1") == true ? dr["Super17_1"].ToString() : "";
            Super17_2 = columns.Contains("Super17_2") == true ? dr["Super17_2"].ToString() : "";
            Super17_3 = columns.Contains("Super17_3") == true ? dr["Super17_3"].ToString() : "";
            Super18_1 = columns.Contains("Super18_1") == true ? dr["Super18_1"].ToString() : "";
            Super18_2 = columns.Contains("Super18_2") == true ? dr["Super18_2"].ToString() : "";
            Super18_3 = columns.Contains("Super18_3") == true ? dr["Super18_3"].ToString() : "";
            Super19_1 = columns.Contains("Super19_1") == true ? dr["Super19_1"].ToString() : "";
            Super19_2 = columns.Contains("Super19_2") == true ? dr["Super19_2"].ToString() : "";
            Super19_3 = columns.Contains("Super19_3") == true ? dr["Super19_3"].ToString() : "";
            Super20_1 = columns.Contains("Super20_1") == true ? dr["Super20_1"].ToString() : "";
            Super20_2 = columns.Contains("Super20_2") == true ? dr["Super20_2"].ToString() : "";
            Super20_3 = columns.Contains("Super20_3") == true ? dr["Super20_3"].ToString() : "";
            Super1_4 = columns.Contains("Super1_4") == true ? dr["Super1_4"].ToString() : "";
            Super1_5 = columns.Contains("Super1_5") == true ? dr["Super1_5"].ToString() : "";
            Super1_6 = columns.Contains("Super1_6") == true ? dr["Super1_6"].ToString() : "";
            Super2_4 = columns.Contains("Super2_4") == true ? dr["Super2_4"].ToString() : "";
            Super2_5 = columns.Contains("Super2_5") == true ? dr["Super2_5"].ToString() : "";
            Super2_6 = columns.Contains("Super2_6") == true ? dr["Super2_6"].ToString() : "";
            Super3_4 = columns.Contains("Super3_4") == true ? dr["Super3_4"].ToString() : "";
            Super3_5 = columns.Contains("Super3_5") == true ? dr["Super3_5"].ToString() : "";
            Super3_6 = columns.Contains("Super3_6") == true ? dr["Super3_6"].ToString() : "";
            Super4_4 = columns.Contains("Super4_4") == true ? dr["Super4_4"].ToString() : "";
            Super4_5 = columns.Contains("Super4_5") == true ? dr["Super4_5"].ToString() : "";
            Super4_6 = columns.Contains("Super4_6") == true ? dr["Super4_6"].ToString() : "";
            Super5_4 = columns.Contains("Super5_4") == true ? dr["Super5_4"].ToString() : "";
            Super5_5 = columns.Contains("Super5_5") == true ? dr["Super5_5"].ToString() : "";
            Super5_6 = columns.Contains("Super5_6") == true ? dr["Super5_6"].ToString() : "";
            Super6_4 = columns.Contains("Super6_4") == true ? dr["Super6_4"].ToString() : "";
            Super6_5 = columns.Contains("Super6_5") == true ? dr["Super6_5"].ToString() : "";
            Super6_6 = columns.Contains("Super6_6") == true ? dr["Super6_6"].ToString() : "";
            Super7_4 = columns.Contains("Super7_4") == true ? dr["Super7_4"].ToString() : "";
            Super7_5 = columns.Contains("Super7_5") == true ? dr["Super7_5"].ToString() : "";
            Super7_6 = columns.Contains("Super7_6") == true ? dr["Super7_6"].ToString() : "";
            Super8_4 = columns.Contains("Super8_4") == true ? dr["Super8_4"].ToString() : "";
            Super8_5 = columns.Contains("Super8_5") == true ? dr["Super8_5"].ToString() : "";
            Super8_6 = columns.Contains("Super8_6") == true ? dr["Super8_6"].ToString() : "";
            Super9_4 = columns.Contains("Super9_4") == true ? dr["Super9_4"].ToString() : "";
            Super9_5 = columns.Contains("Super9_5") == true ? dr["Super9_5"].ToString() : "";
            Super9_6 = columns.Contains("Super9_6") == true ? dr["Super9_6"].ToString() : "";
            Super10_4 = columns.Contains("Super10_4") == true ? dr["Super10_4"].ToString() : "";
            Super10_5 = columns.Contains("Super10_5") == true ? dr["Super10_5"].ToString() : "";
            Super10_6 = columns.Contains("Super10_6") == true ? dr["Super10_6"].ToString() : "";
            Super11_4 = columns.Contains("Super11_4") == true ? dr["Super11_4"].ToString() : "";
            Super11_5 = columns.Contains("Super11_5") == true ? dr["Super11_5"].ToString() : "";
            Super11_6 = columns.Contains("Super11_6") == true ? dr["Super11_6"].ToString() : "";
            Super12_4 = columns.Contains("Super12_4") == true ? dr["Super12_4"].ToString() : "";
            Super12_5 = columns.Contains("Super12_5") == true ? dr["Super12_5"].ToString() : "";
            Super12_6 = columns.Contains("Super12_6") == true ? dr["Super12_6"].ToString() : "";
            Super13_4 = columns.Contains("Super13_4") == true ? dr["Super13_4"].ToString() : "";
            Super13_5 = columns.Contains("Super13_5") == true ? dr["Super13_5"].ToString() : "";
            Super13_6 = columns.Contains("Super13_6") == true ? dr["Super13_6"].ToString() : "";
            Super14_4 = columns.Contains("Super14_4") == true ? dr["Super14_4"].ToString() : "";
            Super14_5 = columns.Contains("Super14_5") == true ? dr["Super14_5"].ToString() : "";
            Super14_6 = columns.Contains("Super14_6") == true ? dr["Super14_6"].ToString() : "";
            Super15_4 = columns.Contains("Super15_4") == true ? dr["Super15_4"].ToString() : "";
            Super15_5 = columns.Contains("Super15_5") == true ? dr["Super15_5"].ToString() : "";
            Super15_6 = columns.Contains("Super15_6") == true ? dr["Super15_6"].ToString() : "";
            Super16_4 = columns.Contains("Super16_4") == true ? dr["Super16_4"].ToString() : "";
            Super16_5 = columns.Contains("Super16_5") == true ? dr["Super16_5"].ToString() : "";
            Super16_6 = columns.Contains("Super16_6") == true ? dr["Super16_6"].ToString() : "";
            Super17_4 = columns.Contains("Super17_4") == true ? dr["Super17_4"].ToString() : "";
            Super17_5 = columns.Contains("Super17_5") == true ? dr["Super17_5"].ToString() : "";
            Super17_6 = columns.Contains("Super17_6") == true ? dr["Super17_6"].ToString() : "";
            Super18_4 = columns.Contains("Super18_4") == true ? dr["Super18_4"].ToString() : "";
            Super18_5 = columns.Contains("Super18_5") == true ? dr["Super18_5"].ToString() : "";
            Super18_6 = columns.Contains("Super18_6") == true ? dr["Super18_6"].ToString() : "";
            Super19_4 = columns.Contains("Super19_4") == true ? dr["Super19_4"].ToString() : "";
            Super19_5 = columns.Contains("Super19_5") == true ? dr["Super19_5"].ToString() : "";
            Super19_6 = columns.Contains("Super19_6") == true ? dr["Super19_6"].ToString() : "";
            Super20_4 = columns.Contains("Super20_4") == true ? dr["Super20_4"].ToString() : "";
            Super20_5 = columns.Contains("Super20_5") == true ? dr["Super20_5"].ToString() : "";
            Super20_6 = columns.Contains("Super20_6") == true ? dr["Super20_6"].ToString() : "";
            DescriereInRatingGlobal = columns.Contains("DescriereInRatingGlobal") == true ? dr["DescriereInRatingGlobal"].ToString() : "";
        }
    }
    public class Eval_QuizIntrebari
    {
        public int Id { get; set; }
        public string Descriere { get; set; }
        public int TipValoare { get; set; }
        public string Ordine { get; set; }
        public int IdIntrebare { get; set; }
        public int TipData { get; set; }
        public int IdQuiz { get; set; }
        public int Orientare { get; set; }
        public int Obligatoriu { get; set; }
        public int Parinte { get; set; }
        public int EsteSectiune { get; set; }
        public string DescriereInRatingGlobal { get; set; }
        public DateTime TIME { get; set; }
        public int USER_NO { get; set; }
        public int OrdineInt { get; set; }
        public int TemplateIdObiectiv { get; set; }
        public int TemplateIdCompetenta { get; set; }

        public Eval_QuizIntrebari() { }

        public Eval_QuizIntrebari(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            Descriere = columns.Contains("Descriere") == true ? dr["Descriere"].ToString() : "";
            TipValoare = columns.Contains("TipValoare") == true ? Convert.ToInt32(dr["TipValoare"].ToString() == string.Empty ? "-99" : dr["TipValoare"].ToString()) : -99;
            Ordine = columns.Contains("Ordine") == true ? dr["Ordine"].ToString() : "";
            IdIntrebare = columns.Contains("IdIntrebare") == true ? Convert.ToInt32(dr["IdIntrebare"].ToString() == string.Empty ? "-99" : dr["IdIntrebare"].ToString()) : -99;
            TipData = columns.Contains("TipData") == true ? Convert.ToInt32(dr["TipData"].ToString() == string.Empty ? "-99" : dr["TipData"].ToString()) : -99;
            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString() == string.Empty ? "-99" : dr["IdQuiz"].ToString()) : -99;
            Orientare = columns.Contains("Orientare") == true ? Convert.ToInt32(dr["Orientare"].ToString() == string.Empty ? "-99" : dr["Orientare"].ToString()) : -99;
            Obligatoriu = columns.Contains("Obligatoriu") == true ? Convert.ToInt32(dr["Obligatoriu"].ToString() == string.Empty ? "-99" : dr["Obligatoriu"].ToString()) : -99;
            Parinte = columns.Contains("Parinte") == true ? Convert.ToInt32(dr["Parinte"].ToString() == string.Empty ? "-99" : dr["Parinte"].ToString()) : -99;
            EsteSectiune = columns.Contains("EsteSectiune") == true ? Convert.ToInt32(dr["EsteSectiune"].ToString() == string.Empty ? "-99" : dr["EsteSectiune"].ToString()) : -99;
            DescriereInRatingGlobal = columns.Contains("DescriereInRatingGlobal") == true ? dr["DescriereInRatingGlobal"].ToString() : "";
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
            OrdineInt = columns.Contains("OrdineInt") == true ? Convert.ToInt32(dr["OrdineInt"].ToString() == string.Empty ? "-99" : dr["OrdineInt"].ToString()) : -99;
            TemplateIdObiectiv = columns.Contains("TemplateIdObiectiv") == true ? Convert.ToInt32(dr["TemplateIdObiectiv"].ToString() == string.Empty ? "-99" : dr["TemplateIdObiectiv"].ToString()) : -99;
            TemplateIdCompetenta = columns.Contains("TemplateIdCompetenta") == true ? Convert.ToInt32(dr["TemplateIdCompetenta"].ToString() == string.Empty ? "-99" : dr["TemplateIdCompetenta"].ToString()) : -99;
        }
    }

    public class Eval_Raspuns
    {
        public int IdQuiz { get; set; }
        public int F10003 { get; set; }
        public int Pozitie { get; set; }
        public int TotalCircuit { get; set; }
        public string Culoare { get; set; }
        public string Observatii { get; set; }
        public int USER_NO { get; set; }
        public DateTime TIME { get; set; }
        public int Inlocuitor { get; set; }
        public int Finalizat { get; set; }
        public int LuatLaCunostinta { get; set; }
        public int IdAuto { get; set; }
        public int LuatUser { get; set; }
        public int LuatData { get; set; }
        public int LuatAutomat { get; set; }

        public Eval_Raspuns() { }

        public Eval_Raspuns(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString()) : -99;
            F10003 = columns.Contains("F10003") == true ? Convert.ToInt32(dr["F10003"].ToString()) : -99;
            Pozitie = columns.Contains("Pozitie") == true ? Convert.ToInt32(dr["Pozitie"].ToString()) : -99;
            TotalCircuit = columns.Contains("TotalCircuit") == true ? Convert.ToInt32(dr["TotalCircuit"].ToString()) : -99;
            Culoare = columns.Contains("Culoare") == true ? dr["Culoare"].ToString() : "";
            Observatii = columns.Contains("Observatii") == true ? dr["Observatii"].ToString() : "";
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString()) : -99;
            //TIME = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString()) : -99;
            Inlocuitor = columns.Contains("Inlocuitor") == true ? Convert.ToInt32(dr["Inlocuitor"].ToString() == string.Empty ? "-99" : dr["Inlocuitor"].ToString()) : -99;
            Finalizat = columns.Contains("Finalizat") == true ? Convert.ToInt32(dr["Finalizat"].ToString() == string.Empty ? "-99" : dr["Finalizat"].ToString()) : -99;
            LuatLaCunostinta = columns.Contains("LuatLaCunostinta") == true ? Convert.ToInt32(dr["LuatLaCunostinta"].ToString() == string.Empty ? "-99" : dr["LuatLaCunostinta"].ToString()) : -99;
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
            LuatUser = columns.Contains("LuatUser") == true ? Convert.ToInt32(dr["LuatUser"].ToString() == string.Empty ? "-99" : dr["LuatUser"].ToString()) : -99;
            LuatData = columns.Contains("LuatData") == true ? Convert.ToInt32(dr["LuatData"].ToString() == string.Empty ? "-99" : dr["LuatData"].ToString()) : -99;
            LuatAutomat = columns.Contains("LuatAutomat") == true ? Convert.ToInt32(dr["LuatAutomat"].ToString() == string.Empty ? "-99" : dr["LuatAutomat"].ToString()) : -99;
        }

    }

    public class Eval_Circuit
    {
        public int IdQuiz { get; set; }
        public int Super1 { get; set; }
        public int Super2 { get; set; }
        public int Super3 { get; set; }
        public int Super4 { get; set; }
        public int Super5 { get; set; }
        public int Super6 { get; set; }
        public int Super7 { get; set; }
        public int Super8 { get; set; }
        public int Super9 { get; set; }
        public int Super10 { get; set; }
        public int Super11 { get; set; }
        public int Super12 { get; set; }
        public int Super13 { get; set; }
        public int Super14 { get; set; }
        public int Super15 { get; set; }
        public int Super16 { get; set; }
        public int Super17 { get; set; }
        public int Super18 { get; set; }
        public int Super19 { get; set; }
        public int Super20 { get; set; }
        public int USER_NO { get; set; }
        public DateTime TIME { get; set; }
        public int IdAuto { get; set; }
        public int RespectaOrdinea { get; set; }

        public Eval_Circuit() { }

        public Eval_Circuit(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString()) : -99;
            Super1 = columns.Contains("Super1") == true ? Convert.ToInt32(dr["Super1"].ToString() == string.Empty ? "-99" : dr["Super1"].ToString()) : -99;
            Super2 = columns.Contains("Super2") == true ? Convert.ToInt32(dr["Super2"].ToString() == string.Empty ? "-99" : dr["Super2"].ToString()) : -99;
            Super3 = columns.Contains("Super3") == true ? Convert.ToInt32(dr["Super3"].ToString() == string.Empty ? "-99" : dr["Super3"].ToString()) : -99;
            Super4 = columns.Contains("Super4") == true ? Convert.ToInt32(dr["Super4"].ToString() == string.Empty ? "-99" : dr["Super4"].ToString()) : -99;
            Super5 = columns.Contains("Super5") == true ? Convert.ToInt32(dr["Super5"].ToString() == string.Empty ? "-99" : dr["Super5"].ToString()) : -99;
            Super6 = columns.Contains("Super6") == true ? Convert.ToInt32(dr["Super6"].ToString() == string.Empty ? "-99" : dr["Super6"].ToString()) : -99;
            Super7 = columns.Contains("Super7") == true ? Convert.ToInt32(dr["Super7"].ToString() == string.Empty ? "-99" : dr["Super7"].ToString()) : -99;
            Super8 = columns.Contains("Super8") == true ? Convert.ToInt32(dr["Super8"].ToString() == string.Empty ? "-99" : dr["Super8"].ToString()) : -99;
            Super9 = columns.Contains("Super9") == true ? Convert.ToInt32(dr["Super9"].ToString() == string.Empty ? "-99" : dr["Super9"].ToString()) : -99;
            Super10 = columns.Contains("Super10") == true ? Convert.ToInt32(dr["Super10"].ToString() == string.Empty ? "-99" : dr["Super10"].ToString()) : -99;
            Super11 = columns.Contains("Super11") == true ? Convert.ToInt32(dr["Super11"].ToString() == string.Empty ? "-99" : dr["Super11"].ToString()) : -99;
            Super12 = columns.Contains("Super12") == true ? Convert.ToInt32(dr["Super12"].ToString() == string.Empty ? "-99" : dr["Super12"].ToString()) : -99;
            Super13 = columns.Contains("Super13") == true ? Convert.ToInt32(dr["Super13"].ToString() == string.Empty ? "-99" : dr["Super13"].ToString()) : -99;
            Super14 = columns.Contains("Super14") == true ? Convert.ToInt32(dr["Super14"].ToString() == string.Empty ? "-99" : dr["Super14"].ToString()) : -99;
            Super15 = columns.Contains("Super15") == true ? Convert.ToInt32(dr["Super15"].ToString() == string.Empty ? "-99" : dr["Super15"].ToString()) : -99;
            Super16 = columns.Contains("Super16") == true ? Convert.ToInt32(dr["Super16"].ToString() == string.Empty ? "-99" : dr["Super16"].ToString()) : -99;
            Super17 = columns.Contains("Super17") == true ? Convert.ToInt32(dr["Super17"].ToString() == string.Empty ? "-99" : dr["Super17"].ToString()) : -99;
            Super18 = columns.Contains("Super18") == true ? Convert.ToInt32(dr["Super18"].ToString() == string.Empty ? "-99" : dr["Super18"].ToString()) : -99;
            Super19 = columns.Contains("Super19") == true ? Convert.ToInt32(dr["Super19"].ToString() == string.Empty ? "-99" : dr["Super19"].ToString()) : -99;
            Super20 = columns.Contains("Super20") == true ? Convert.ToInt32(dr["Super20"].ToString() == string.Empty ? "-99" : dr["Super20"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
            //TIME = columns.Contains("TotalCircuit") == true ? Convert.ToInt32(dr["TotalCircuit"].ToString()) : -99;
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
            RespectaOrdinea = columns.Contains("RespectaOrdinea") == true ? Convert.ToInt32(dr["RespectaOrdinea"].ToString() == string.Empty ? "-99" : dr["RespectaOrdinea"].ToString()) : -99;
        }
    }

    public class tblSupervizori
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public string Alias { get; set; }
        public int IdUser { get; set; }
        public int ModululCereriAbsente { get; set; }
        public int USER_NO { get; set; }
        public DateTime TIME { get; set; }

        public tblSupervizori() { }

        public tblSupervizori(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString()) : -99;
            Denumire = columns.Contains("Denumire") == true ? dr["Denumire"].ToString() : "";
            Alias = columns.Contains("Alias") == true ? dr["Alias"].ToString() : "";
            IdUser = columns.Contains("IdUser") == true ? Convert.ToInt32(dr["IdUser"].ToString() == string.Empty ? "-99" : dr["IdUser"].ToString()) : -99;
            ModululCereriAbsente = columns.Contains("ModululCereriAbsente") == true ? Convert.ToInt32(dr["ModululCereriAbsente"].ToString() == string.Empty ? "-99" :
                dr["ModululCereriAbsente"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
        }
    }

    public class USERS
    {
        public int F70101 { get; set; }
        public int F70102 { get; set; }
        public string F70103 { get; set; }
        public string F70104 { get; set; }
        public string F70105 { get; set; }
        public int F70111 { get; set; }
        public int F70112 { get; set; }
        public int F70113 { get; set; }
        public int F70121 { get; set; }
        public DateTime F70122 { get; set; }
        public int USER_NO { get; set; }
        public DateTime TIME { get; set; }
        public int F70114 { get; set; }
        public string F70123 { get; set; }
        public string IdLimba { get; set; }
        public int F10003 { get; set; }
        public string Mail { get; set; }
        public string NumeComplet { get; set; }
        public int SchimbaParola { get; set; }
        public string Parola { get; set; }
        public string PINInfoChiosc { get; set; }

        public USERS() { }

        public USERS(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            F70101 = columns.Contains("F70101") == true ? Convert.ToInt32(dr["F70101"].ToString() == string.Empty ? "-99" : dr["F70101"].ToString()) : -99;
            F70102 = columns.Contains("F70102") == true ? Convert.ToInt32(dr["F70102"].ToString() == string.Empty ? "-99" : dr["F70102"].ToString()) : -99;
            F70103 = columns.Contains("F70103") == true ? dr["F70103"].ToString() : "";
            F70104 = columns.Contains("F70104") == true ? dr["F70104"].ToString() : "";
            F70105 = columns.Contains("F70105") == true ? dr["F70105"].ToString() : "";
            F70111 = columns.Contains("F70111") == true ? Convert.ToInt32(dr["F70111"].ToString() == string.Empty ? "-99" : dr["F70111"].ToString()) : -99;
            F70112 = columns.Contains("F70112") == true ? Convert.ToInt32(dr["F70112"].ToString() == string.Empty ? "-99" : dr["F70112"].ToString()) : -99;
            F70113 = columns.Contains("F70113") == true ? Convert.ToInt32(dr["F70113"].ToString() == string.Empty ? "-99" : dr["F70113"].ToString()) : -99;
            F70121 = columns.Contains("F70121") == true ? Convert.ToInt32(dr["F70121"].ToString() == string.Empty ? "-99" : dr["F70121"].ToString()) : -99;
            //F70122 = columns.Contains("F70122") == true ? Convert.ToInt32(dr["F70122"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
            F70114 = columns.Contains("F70114") == true ? Convert.ToInt32(dr["F70114"].ToString() == string.Empty ? "-99" : dr["F70114"].ToString()) : -99;
            F70123 = columns.Contains("F70123") == true ? dr["F70123"].ToString() : "";
            IdLimba = columns.Contains("IdLimba") == true ? dr["IdLimba"].ToString() : "";
            F10003 = columns.Contains("F10003") == true ? Convert.ToInt32(dr["F10003"].ToString() == string.Empty ? "-99" : dr["F10003"].ToString()) : -99;
            Mail = columns.Contains("Mail") == true ? dr["Mail"].ToString() : "";
            NumeComplet = columns.Contains("NumeComplet") == true ? dr["NumeComplet"].ToString() : "";
            SchimbaParola = columns.Contains("SchimbaParola") == true ? Convert.ToInt32(dr["SchimbaParola"].ToString() == string.Empty ? "-99" : dr["SchimbaParola"].ToString()) : -99;
            PINInfoChiosc = columns.Contains("PINInfoChiosc") == true ? dr["PINInfoChiosc"].ToString() : "";
        }
    }

    public class Eval_Drepturi
    {
        public int IdQuiz { get; set; }
        public int Pozitie { get; set; }
        public int PozitieVizibila { get; set; }
        public int USER_NO { get; set; }
        public DateTime TIME { get; set; }
        public int IdAuto { get; set; }

        public Eval_Drepturi() { }

        public Eval_Drepturi(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString() == string.Empty ? "-99" : dr["IdQuiz"].ToString()) : -99;
            Pozitie = columns.Contains("Pozitie") == true ? Convert.ToInt32(dr["Pozitie"].ToString() == string.Empty ? "-99" : dr["Pozitie"].ToString()) : -99;
            PozitieVizibila = columns.Contains("PozitieVizibila") == true ? Convert.ToInt32(dr["PozitieVizibila"].ToString() == string.Empty ? "-99" : dr["PozitieVizibila"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
        }
    }

    public class Eval_tblTipValoriLinii
    {
        public int Id { get; set; }
        public string Valoare { get; set; }
        public string Nota { get; set; }
        public int IdAuto { get; set; }
        public DateTime TIME { get; set; }
        public int USER_NO { get; set; }

        public Eval_tblTipValoriLinii() { }

        public Eval_tblTipValoriLinii(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            Valoare = columns.Contains("Valoare") == true ? dr["Valoare"].ToString() : "";
            Nota = columns.Contains("Nota") == true ? dr["Nota"].ToString() : "";
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
        }
    }

    public class Eval_SetAngajati
    {
        public int IdSetAng { get; set; }
        public string CodSet { get; set; }
        public string DenSet { get; set; }
        public string SelectQuery { get; set; }
        public DateTime TIME { get; set; }
        public int USER_NO { get; set; }

        public Eval_SetAngajati() { }

        public Eval_SetAngajati(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdSetAng = columns.Contains("IdSetAng") == true ? Convert.ToInt32(dr["IdSetAng"].ToString() == string.Empty ? "-99" : dr["IdSetAng"].ToString()) : -99;
            CodSet = columns.Contains("CodSet") == true ? dr["CodSet"].ToString() : "";
            DenSet = columns.Contains("DenSet") == true ? dr["DenSet"].ToString() : "";
            SelectQuery = columns.Contains("SelectQuery") == true ? dr["SelectQuery"].ToString() : "";
            USER_NO = columns.Contains("USER_NO") == true ? Convert.ToInt32(dr["USER_NO"].ToString() == string.Empty ? "-99" : dr["USER_NO"].ToString()) : -99;
        }
    }

    public class vwEval_ConfigObiectivCol
    {
        public string ColumnName { get; set; }
        public int IdTipValoare { get; set; }
        public string TipValoare { get; set; }
        public int IdNomenclator { get; set; }
        public string CodNomenclator { get; set; }
        public string DenNomenclator { get; set; }

        public vwEval_ConfigObiectivCol() { }

        public vwEval_ConfigObiectivCol(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            ColumnName = columns.Contains("ColumnName") == true ? dr["ColumnName"].ToString() : "";
            IdTipValoare = columns.Contains("IdTipValoare") == true ? Convert.ToInt32(dr["IdTipValoare"].ToString() == string.Empty ? "-99" : dr["IdTipValoare"].ToString()) : -99;
            TipValoare = columns.Contains("TipValoare") == true ? dr["TipValoare"].ToString() : "";
            IdNomenclator = columns.Contains("IdNomenclator") == true ? Convert.ToInt32(dr["IdNomenclator"].ToString() == string.Empty ? "-99" : dr["IdNomenclator"].ToString()) : -99;
            CodNomenclator = columns.Contains("CodNomenclator") == true ? dr["CodNomenclator"].ToString() : "";
            DenNomenclator = columns.Contains("DenNomenclator") == true ? dr["DenNomenclator"].ToString() : "";
        }
    }

    public class Eval_DictionaryItem
    {
        public int DictionaryItemId { get; set; }
        public int DictionaryId { get; set; }
        public string DictionaryItemCode { get; set; }
        public string DictionaryItemName { get; set; }

        public Eval_DictionaryItem() { }

        public Eval_DictionaryItem(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            DictionaryItemId = columns.Contains("DictionaryItemId") == true ? Convert.ToInt32(dr["DictionaryItemId"].ToString() == string.Empty ? "-99" : dr["DictionaryItemId"].ToString()) : -99;
            DictionaryId = columns.Contains("DictionaryId") == true ? Convert.ToInt32(dr["DictionaryId"].ToString() == string.Empty ? "-99" : dr["DictionaryId"].ToString()) : -99;
            DictionaryItemCode = columns.Contains("DictionaryItemCode") == true ? dr["DictionaryItemCode"].ToString() : "";
            DictionaryItemName = columns.Contains("DictionaryItemName") == true ? dr["DictionaryItemName"].ToString() : "";
        }
    }

    public class Eval_ConfigObTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int NrMinObiective { get; set; }
        public int NrMaxObiective { get; set; }
        public int PoateAdauga { get; set; }
        public int PoateSterge { get; set; }
        public int PoateModifica { get; set; }

        public Eval_ConfigObTemplate() { }

        public Eval_ConfigObTemplate(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            TemplateId = columns.Contains("TemplateId") == true ? Convert.ToInt32(dr["TemplateId"].ToString() == string.Empty ? "-99" : dr["TemplateId"].ToString()) : -99;
            TemplateName = columns.Contains("TemplateName") == true ? dr["TemplateName"].ToString() : "";
            NrMinObiective = columns.Contains("NrMinObiective") == true ? Convert.ToInt32(dr["NrMinObiective"].ToString() == string.Empty ? "-99" : dr["NrMinObiective"].ToString()) : -99;
            NrMaxObiective = columns.Contains("NrMaxObiective") == true ? Convert.ToInt32(dr["NrMaxObiective"].ToString() == string.Empty ? "-99" : dr["NrMaxObiective"].ToString()) : -99;
            PoateAdauga = columns.Contains("PoateAdauga") == true ? Convert.ToInt32(dr["PoateAdauga"].ToString() == string.Empty ? "-99" : dr["PoateAdauga"].ToString()) : -99;
            PoateSterge = columns.Contains("PoateSterge") == true ? Convert.ToInt32(dr["PoateSterge"].ToString() == string.Empty ? "-99" : dr["PoateSterge"].ToString()) : -99;
            PoateModifica = columns.Contains("PoateModifica") == true ? Convert.ToInt32(dr["PoateModifica"].ToString() == string.Empty ? "-99" : dr["PoateModifica"].ToString()) : -99;
        }
    }

    public class Eval_ConfigObTemplateDetail
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string ColumnName { get; set; }
        public int Width { get; set; }
        public bool Obligatoriu { get; set; }
        public bool Citire { get; set; }
        public bool Editare { get; set; }
        public bool Vizibil { get; set; }
        public int TipValoare { get; set; }
        public int IdNomenclator { get; set; }

        public Eval_ConfigObTemplateDetail() { }

        public Eval_ConfigObTemplateDetail(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            TemplateId = columns.Contains("TemplateId") == true ? Convert.ToInt32(dr["TemplateId"].ToString() == string.Empty ? "-99" : dr["TemplateId"].ToString()) : -99;
            ColumnName = columns.Contains("ColumnName") == true ? dr["ColumnName"].ToString() : "";
            Width = columns.Contains("Width") == true ? Convert.ToInt32(dr["Width"].ToString() == string.Empty ? "0" : dr["Width"].ToString()) : 0;
            Obligatoriu = columns.Contains("Obligatoriu") == true ? Convert.ToBoolean(dr["Obligatoriu"].ToString() == string.Empty ? "False" : (dr["Obligatoriu"].ToString() == "0" ? "False" : "True")) : false;
            //Citire = columns.Contains("Citire") == true ? Convert.ToInt32(dr["Citire"].ToString() == string.Empty ? "0" : dr["Citire"].ToString()) : 0;
            Citire = columns.Contains("Citire") == true ? Convert.ToBoolean(dr["Citire"].ToString() == string.Empty ? "False" : (dr["Citire"].ToString() == "0" ? "False" : "True")) : false;
            Editare = columns.Contains("Editare") == true ? Convert.ToBoolean(dr["Editare"].ToString() == string.Empty ? "False" : (dr["Editare"].ToString() == "0" ? "False" : "True")) : false;
            Vizibil = columns.Contains("Vizibil") == true ? Convert.ToBoolean(dr["Vizibil"].ToString() == string.Empty ? "False" : (dr["Vizibil"].ToString() == "0" ? "False" : "True")) : false;
            //Editare = columns.Contains("Editare") == true ? Convert.ToInt32(dr["Editare"].ToString() == string.Empty ? "0" : dr["Editare"].ToString()) : 0;
            //Vizibil = columns.Contains("Vizibil") == true ? Convert.ToInt32(dr["Vizibil"].ToString() == string.Empty ? "0" : dr["Vizibil"].ToString()) : 0;
            TipValoare = columns.Contains("TipValoare") == true ? Convert.ToInt32(dr["TipValoare"].ToString() == string.Empty ? "-99" : dr["TipValoare"].ToString()) : -99;
            IdNomenclator = columns.Contains("IdNomenclator") == true ? Convert.ToInt32(dr["IdNomenclator"].ToString() == string.Empty ? "-99" : dr["IdNomenclator"].ToString()) : -99;

        }
    }

    public class Eval_ConfigObiective
    {
        [Key]
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int Ordine { get; set; }
        public int Vizibil { get; set; }

        public Eval_ConfigObiective() { }
        public Eval_ConfigObiective(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            ColumnName = columns.Contains("ColumnName") == true ? dr["ColumnName"].ToString() : "";
            ColumnType = columns.Contains("ColumnType") == true ? dr["ColumnType"].ToString() : "";
            Ordine = columns.Contains("Ordine") == true ? Convert.ToInt32(dr["Ordine"].ToString() == string.Empty ? "-99" : dr["Ordine"].ToString()) : -99;
            Vizibil = columns.Contains("Vizibil") == true ? Convert.ToInt32(dr["Vizibil"].ToString() == string.Empty ? "-99" : dr["Vizibil"].ToString()) : -99;
        }
    }


    public class Eval_ObiIndividuale
    {
        public int IdPeriod { get; set; }
        public int F10003 { get; set; }
        public int IdObiectiv { get; set; }
        public string Obiectiv { get; set; }
        public int IdActivitate { get; set; }
        public string Activitate { get; set; }
        public decimal Pondere { get; set; }
        public string Descriere { get; set; }
        public decimal Target { get; set; }
        public DateTime Termen { get; set; }
        public int Realizat { get; set; }
        public int IdCalificativ { get; set; }
        public string Calificativ { get; set; }
        public string ExplicatiiCalificativ { get; set; }
        public int IdAuto { get; set; }
        public string ColoanaSuplimentara1 { get; set; }
        public string ColoanaSuplimentara2 { get; set; }

        public Eval_ObiIndividuale() { }

        public Eval_ObiIndividuale(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdPeriod = columns.Contains("IdPeriod ") == true ? Convert.ToInt32(dr["IdPeriod "].ToString() == string.Empty ? "-99" : dr["IdPeriod "].ToString()) : -99;
            F10003 = columns.Contains("F10003 ") == true ? Convert.ToInt32(dr["F10003 "].ToString() == string.Empty ? "-99" : dr["F10003 "].ToString()) : -99;
            IdObiectiv = columns.Contains("IdObiectiv ") == true ? Convert.ToInt32(dr["IdObiectiv "].ToString() == string.Empty ? "-99" : dr["IdObiectiv "].ToString()) : -99;
            Obiectiv = columns.Contains("Obiectiv") == true ? dr["Obiectiv"].ToString() : "";
            IdActivitate = columns.Contains("IdActivitate") == true ? Convert.ToInt32(dr["IdActivitate"].ToString() == string.Empty ? "-99" : dr["IdActivitate"].ToString()) : -99;
            Activitate = columns.Contains("Activitate") == true ? dr["Activitate"].ToString() : "";
            Pondere = columns.Contains("Pondere") == true ? Convert.ToDecimal(dr["Pondere"].ToString() == string.Empty ? "-99" : dr["Pondere"].ToString()) : -99;
            Descriere = columns.Contains("Descriere") == true ? dr["Descriere"].ToString() : "";
            Target = columns.Contains("Target") == true ? Convert.ToDecimal(dr["Target"].ToString() == string.Empty ? "-99" : dr["Target"].ToString()) : -99;
            Termen = columns.Contains("Termen") == true ? Convert.ToDateTime(dr["Termen"].ToString() == string.Empty ? DateTime.Now.ToString("yyyyMMdd HH:mm") : dr["Termen"].ToString()) : DateTime.Now;
            Realizat = columns.Contains("Realizat") == true ? Convert.ToInt32(dr["Realizat"].ToString() == string.Empty ? "-99" : dr["Realizat"].ToString()) : -99;
            IdCalificativ = columns.Contains("IdCalificativ") == true ? Convert.ToInt32(dr["IdCalificativ"].ToString() == string.Empty ? "-99" : dr["IdCalificativ"].ToString()) : -99;
            Calificativ = columns.Contains("Calificativ") == true ? dr["Calificativ"].ToString() : "";
            ExplicatiiCalificativ = columns.Contains("ExplicatiiCalificativ") == true ? dr["ExplicatiiCalificativ"].ToString() : "";
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
            ColoanaSuplimentara1 = columns.Contains("ColoanaSuplimentara1") == true ? dr["ColoanaSuplimentara1"].ToString() : "";
            ColoanaSuplimentara2 = columns.Contains("ColoanaSuplimentara2") == true ? dr["ColoanaSuplimentara2"].ToString() : "";
        }
    }

    public class Eval_ObiIndividualeTemp
    {
        public decimal? Pondere { get; set; }
        public decimal? Target { get; set; }
        public int? Realizat { get; set; }
        public int? IdCalificativ { get; set; }



        public int IdObiectiv { get; set; }
        public string Obiectiv { get; set; }
        public int IdActivitate { get; set; }
        public string Activitate { get; set; }
        public string Descriere { get; set; }
        public DateTime Termen { get; set; }
        public string Calificativ { get; set; }
        public string ExplicatiiCalificativ { get; set; }
        public int IdQuiz { get; set; }
        public int F10003 { get; set; }
        public int Pozitie { get; set; }
        public int Id { get; set; }
        public int IdAuto { get; set; }
        public int IdLinieQuiz { get; set; }
        public string ColoanaSuplimentara1 { get; set; }
        public string ColoanaSuplimentara2 { get; set; }

        public int IdUnic { get; set; }
        public int? USER_NO { get; set; }
        public DateTime? TIME { get; set; }

        public Eval_ObiIndividualeTemp() { }

        public Eval_ObiIndividualeTemp(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdObiectiv = columns.Contains("IdObiectiv") == true ? Convert.ToInt32(dr["IdObiectiv"].ToString() == string.Empty ? "-99" : dr["IdObiectiv"].ToString()) : -99;
            Obiectiv = columns.Contains("Obiectiv") == true ? dr["Obiectiv"].ToString() : "";
            IdActivitate = columns.Contains("IdActivitate") == true ? Convert.ToInt32(dr["IdActivitate"].ToString() == string.Empty ? "-99" : dr["IdActivitate"].ToString()) : -99;
            Activitate = columns.Contains("Activitate") == true ? dr["Activitate"].ToString() : "";
            //Pondere = columns.Contains("Pondere") == true ? Convert.ToDecimal(dr["Pondere"].ToString() == string.Empty ? "-99" : dr["Pondere"].ToString()) : -99;
            Descriere = columns.Contains("Descriere") == true ? dr["Descriere"].ToString() : "";
            //Target = columns.Contains("Target") == true ? Convert.ToDecimal(dr["Target"].ToString() == string.Empty ? "-99" : dr["Target"].ToString()) : -99;
            //Termen = columns.Contains("Termen") == true ? Convert.ToDateTime(dr["Termen"].ToString() == string.Empty ? DateTime.Now.ToString("yyyyMMdd HH:mm") : dr["Termen"].ToString()) : DateTime.Now;
            //Realizat = columns.Contains("Realizat") == true ? Convert.ToInt32(dr["Realizat"].ToString() == string.Empty ? "-99" : dr["Realizat"].ToString()) : -99;
            //IdCalificativ = columns.Contains("IdCalificativ") == true ? Convert.ToInt32(dr["IdCalificativ"].ToString() == string.Empty ? "-99" : dr["IdCalificativ"].ToString()) : -99;
            Calificativ = columns.Contains("Calificativ") == true ? dr["Calificativ"].ToString() : "";
            ExplicatiiCalificativ = columns.Contains("ExplicatiiCalificativ") == true ? dr["ExplicatiiCalificativ"].ToString() : "";
            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString() == string.Empty ? "-99" : dr["IdQuiz"].ToString()) : -99;
            F10003 = columns.Contains("F10003") == true ? Convert.ToInt32(dr["F10003"].ToString() == string.Empty ? "-99" : dr["F10003"].ToString()) : -99;
            Pozitie = columns.Contains("Pozitie") == true ? Convert.ToInt32(dr["Pozitie"].ToString() == string.Empty ? "-99" : dr["Pozitie"].ToString()) : -99;
            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
            IdLinieQuiz = columns.Contains("IdLinieQuiz") == true ? Convert.ToInt32(dr["IdLinieQuiz"].ToString() == string.Empty ? "-99" : dr["IdLinieQuiz"].ToString()) : -99;


            Pondere = columns.Contains("Pondere") == true ? (General.IsNumeric(dr["Pondere"]) ? (decimal?)dr["Pondere"] : null) : null;
            Target = columns.Contains("Target") == true ? (General.IsNumeric(dr["Target"]) ? (decimal?)dr["Target"] : null) : null;
            Realizat = columns.Contains("Realizat") == true ? (General.IsNumeric(dr["Realizat"]) ? (int?)dr["Realizat"] : null) : null;
            IdCalificativ = columns.Contains("IdCalificativ") == true ? (General.IsNumeric(dr["IdCalificativ"]) ? (int?)dr["IdCalificativ"] : null) : null;

            ColoanaSuplimentara1 = columns.Contains("ColoanaSuplimentara1") == true ? dr["ColoanaSuplimentara1"].ToString() : "";
            ColoanaSuplimentara2 = columns.Contains("ColoanaSuplimentara2") == true ? dr["ColoanaSuplimentara2"].ToString() : "";

            IdUnic = columns.Contains("IdUnic") == true ? Convert.ToInt32(dr["IdUnic"].ToString() == string.Empty ? "-99" : dr["IdUnic"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? (General.IsNumeric(dr["USER_NO"]) ? (int?)dr["USER_NO"] : null) : null;
            TIME = columns.Contains("TIME") == true ? (dr["TIME"] != DBNull.Value ? (DateTime?)dr["TIME"] : null) : null;
        }
    }

    public class Eval_SetCalificativDet
    {
        public int IdSet { get; set; }
        public int IdCalificativ { get; set; }
        public string Denumire { get; set; }
        public int Nota { get; set; }
        public int RatingMin { get; set; }
        public int RatingMax { get; set; }
        public int Ordine { get; set; }
        public string Explicatii { get; set; }

        public Eval_SetCalificativDet() { }

        public Eval_SetCalificativDet(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdSet = columns.Contains("IdSet ") == true ? Convert.ToInt32(dr["IdSet "].ToString() == string.Empty ? "-99" : dr["IdSet "].ToString()) : -99;
            IdCalificativ = columns.Contains("IdCalificativ") == true ? Convert.ToInt32(dr["IdCalificativ"].ToString() == string.Empty ? "-99" : dr["IdCalificativ"].ToString()) : -99;
            Denumire = columns.Contains("Denumire") == true ? dr["Denumire"].ToString() : "";
            Nota = columns.Contains("Nota") == true ? Convert.ToInt32(dr["Nota"].ToString() == string.Empty ? "-99" : dr["Nota"].ToString()) : -99;
            RatingMin = columns.Contains("RatingMin") == true ? Convert.ToInt32(dr["RatingMin"].ToString() == string.Empty ? "-99" : dr["RatingMin"].ToString()) : -99;
            RatingMax = columns.Contains("RatingMax") == true ? Convert.ToInt32(dr["RatingMax"].ToString() == string.Empty ? "-99" : dr["RatingMax"].ToString()) : -99;
            Ordine = columns.Contains("Ordine") == true ? Convert.ToInt32(dr["Ordine"].ToString() == string.Empty ? "-99" : dr["Ordine"].ToString()) : -99;
            Explicatii = columns.Contains("Explicatii") == true ? dr["Explicatii"].ToString() : "";
        }

    }

    #region Competente
    public class Eval_ConfigCompetente
    {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public int Ordine { get; set; }
        public int Vizibil { get; set; }

        public Eval_ConfigCompetente() { }

        public Eval_ConfigCompetente(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            ColumnName = columns.Contains("ColumnName") == true ? dr["ColumnName"].ToString() : "";
            ColumnType = columns.Contains("ColumnType") == true ? dr["ColumnType"].ToString() : "";
            Ordine = columns.Contains("Ordine") == true ? Convert.ToInt32(dr["Ordine"].ToString() == string.Empty ? "-99" : dr["Ordine"].ToString()) : -99;
            Vizibil = columns.Contains("Vizibil") == true ? Convert.ToInt32(dr["Vizibil"].ToString() == string.Empty ? "-99" : dr["Vizibil"].ToString()) : -99;

        }
    }

    public class Eval_ConfigCompTemplate
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }

        public Eval_ConfigCompTemplate() { }

        public Eval_ConfigCompTemplate(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            TemplateId = columns.Contains("TemplateId") == true ? Convert.ToInt32(dr["TemplateId"].ToString() == string.Empty ? "-99" : dr["TemplateId"].ToString()) : -99;
            TemplateName = columns.Contains("TemplateName") == true ? dr["TemplateName"].ToString() : "";

        }
    }

    public class Eval_ConfigCompTemplateDetail
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string ColumnName { get; set; }
        public int Width { get; set; }
        public bool Obligatoriu { get; set; }
        public bool Citire { get; set; }
        public bool Editare { get; set; }
        public bool Vizibil { get; set; }
        public int IdNomenclator { get; set; }

        public Eval_ConfigCompTemplateDetail() { }
        public Eval_ConfigCompTemplateDetail(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            TemplateId = columns.Contains("TemplateId") == true ? Convert.ToInt32(dr["TemplateId"].ToString() == string.Empty ? "-99" : dr["TemplateId"].ToString()) : -99;
            ColumnName = columns.Contains("ColumnName") == true ? dr["ColumnName"].ToString() : "";
            Width = columns.Contains("Width") == true ? Convert.ToInt32(dr["Width"].ToString() == string.Empty ? "-99" : dr["Width"].ToString()) : -99;
            Obligatoriu = columns.Contains("Obligatoriu") == true ? Convert.ToBoolean(dr["Obligatoriu"].ToString() == string.Empty ? "False" : (dr["Obligatoriu"].ToString() == "0" ? "False" : "True")) : false;
            Citire = columns.Contains("Citire") == true ? Convert.ToBoolean(dr["Citire"].ToString() == string.Empty ? "False" : (dr["Citire"].ToString() == "0" ? "False" : "True")) : false;
            Editare = columns.Contains("Editare") == true ? Convert.ToBoolean(dr["Editare"].ToString() == string.Empty ? "False" : (dr["Editare"].ToString() == "0" ? "False" : "True")) : false;
            Vizibil = columns.Contains("Vizibil") == true ? Convert.ToBoolean(dr["Vizibil"].ToString() == string.Empty ? "False" : (dr["Vizibil"].ToString() == "0" ? "False" : "True")) : false;
            IdNomenclator = columns.Contains("IdNomenclator") == true ? Convert.ToInt32(dr["IdNomenclator"].ToString() == string.Empty ? "-99" : dr["IdNomenclator"].ToString()) : -99;
        }
    }

    public class Eval_CompetenteAngajat
    {
        public int IdPeriod { get; set; }
        public int F10003 { get; set; }
        public int IdCategCompetenta { get; set; }
        public string CategCompetenta { get; set; }
        public int IdCompetenta { get; set; }
        public string Competenta { get; set; }
        public decimal Pondere { get; set; }
        public int IdCalificativ { get; set; }
        public string Calificativ { get; set; }
        public string ExplicatiiCalificativ { get; set; }
        public string Explicatii { get; set; }
        public int IdAuto { get; set; }

        public Eval_CompetenteAngajat() { }
        public Eval_CompetenteAngajat(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdPeriod = columns.Contains("IdPeriod") == true ? Convert.ToInt32(dr["IdPeriod"].ToString() == string.Empty ? "-99" : dr["IdPeriod"].ToString()) : -99;
            F10003 = columns.Contains("F10003") == true ? Convert.ToInt32(dr["F10003"].ToString() == string.Empty ? "-99" : dr["F10003"].ToString()) : -99;
            IdCategCompetenta = columns.Contains("IdCategCompetenta") == true ? Convert.ToInt32(dr["IdCategCompetenta"].ToString() == string.Empty ? "-99" : dr["IdCategCompetenta"].ToString()) : -99;
            CategCompetenta = columns.Contains("CategCompetenta") == true ? dr["CategCompetenta"].ToString() : "";
            IdCompetenta = columns.Contains("IdCompetenta") == true ? Convert.ToInt32(dr["IdCompetenta"].ToString() == string.Empty ? "-99" : dr["IdCompetenta"].ToString()) : -99;
            Competenta = columns.Contains("Competenta") == true ? dr["Competenta"].ToString() : "";
            Pondere = columns.Contains("Pondere") == true ? Convert.ToDecimal(dr["Pondere"].ToString() == string.Empty ? "-99" : dr["Pondere"].ToString()) : -99;
            IdCalificativ = columns.Contains("IdCalificativ") == true ? Convert.ToInt32(dr["IdCalificativ"].ToString() == string.Empty ? "-99" : dr["IdCalificativ"].ToString()) : -99;
            Calificativ = columns.Contains("Calificativ") == true ? dr["Calificativ"].ToString() : "";
            ExplicatiiCalificativ = columns.Contains("ExplicatiiCalificativ") == true ? dr["ExplicatiiCalificativ"].ToString() : "";
            Explicatii = columns.Contains("Explicatii") == true ? dr["Explicatii"].ToString() : "";
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
        }
    }

    public class Eval_CompetenteAngajatTemp
    {
        public int IdCategCompetenta { get; set; }
        public string CategCompetenta { get; set; }
        public int IdCompetenta { get; set; }
        public string Competenta { get; set; }
        public decimal Pondere { get; set; }
        public int IdCalificativ { get; set; }
        public string Calificativ { get; set; }
        public string ExplicatiiCalificativ { get; set; }
        public string Explicatii { get; set; }
        public int IdQuiz { get; set; }
        public int F10003 { get; set; }
        public int Pozitie { get; set; }
        public int Id { get; set; }
        public int IdAuto { get; set; }
        public int IdLinieQuiz { get; set; }

        public int IdUnic { get; set; }
        public int? USER_NO { get; set; }
        public DateTime? TIME { get; set; }


        public Eval_CompetenteAngajatTemp() { }
        public Eval_CompetenteAngajatTemp(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            IdCategCompetenta = columns.Contains("IdCategCompetenta") == true ? Convert.ToInt32(dr["IdCategCompetenta"].ToString() == string.Empty ? "-99" : dr["IdCategCompetenta"].ToString()) : -99;
            CategCompetenta = columns.Contains("CategCompetenta") == true ? dr["CategCompetenta"].ToString() : "";
            IdCompetenta = columns.Contains("IdCompetenta") == true ? Convert.ToInt32(dr["IdCompetenta"].ToString() == string.Empty ? "-99" : dr["IdCompetenta"].ToString()) : -99;
            Competenta = columns.Contains("Competenta") == true ? dr["Competenta"].ToString() : "";
            Pondere = columns.Contains("Pondere") == true ? Convert.ToDecimal(dr["Pondere"].ToString() == string.Empty ? "-99" : dr["Pondere"].ToString()) : -99;
            IdCalificativ = columns.Contains("IdCalificativ") == true ? Convert.ToInt32(dr["IdCalificativ"].ToString() == string.Empty ? "-99" : dr["IdCalificativ"].ToString()) : -99;
            Calificativ = columns.Contains("Calificativ") == true ? dr["Calificativ"].ToString() : "";
            ExplicatiiCalificativ = columns.Contains("ExplicatiiCalificativ") == true ? dr["ExplicatiiCalificativ"].ToString() : "";
            Explicatii = columns.Contains("Explicatii") == true ? dr["Explicatii"].ToString() : "";
            IdQuiz = columns.Contains("IdQuiz") == true ? Convert.ToInt32(dr["IdQuiz"].ToString() == string.Empty ? "-99" : dr["IdQuiz"].ToString()) : -99;
            F10003 = columns.Contains("F10003") == true ? Convert.ToInt32(dr["F10003"].ToString() == string.Empty ? "-99" : dr["F10003"].ToString()) : -99;
            Pozitie = columns.Contains("Pozitie") == true ? Convert.ToInt32(dr["Pozitie"].ToString() == string.Empty ? "-99" : dr["Pozitie"].ToString()) : -99;
            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
            IdAuto = columns.Contains("IdAuto") == true ? Convert.ToInt32(dr["IdAuto"].ToString() == string.Empty ? "-99" : dr["IdAuto"].ToString()) : -99;
            IdLinieQuiz = columns.Contains("IdLinieQuiz") == true ? Convert.ToInt32(dr["IdLinieQuiz"].ToString() == string.Empty ? "-99" : dr["IdLinieQuiz"].ToString()) : -99;

            IdUnic = columns.Contains("IdUnic") == true ? Convert.ToInt32(dr["IdUnic"].ToString() == string.Empty ? "-99" : dr["IdUnic"].ToString()) : -99;
            USER_NO = columns.Contains("USER_NO") == true ? (General.IsNumeric(dr["USER_NO"]) ? (int?)dr["USER_NO"] : null) : null;
            TIME = columns.Contains("TIME") == true ? (dr["TIME"] != DBNull.Value ? (DateTime?)dr["TIME"] : null) : null;

        }
    }

    public class vwEval_ConfigCompetenteCol
    {
        public string ColumnName { get; set; }
        public int IdNomenclator { get; set; }
        public string CodNomenclator { get; set; }
        public string DenNomenclator { get; set; }

        public vwEval_ConfigCompetenteCol() { }
        public vwEval_ConfigCompetenteCol(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            ColumnName = columns.Contains("ColumnName") == true ? dr["ColumnName"].ToString() : "";
            IdNomenclator = columns.Contains("IdNomenclator") == true ? Convert.ToInt32(dr["IdNomenclator"].ToString() == string.Empty ? "-99" : dr["IdNomenclator"].ToString()) : -99;
            CodNomenclator = columns.Contains("CodNomenclator") == true ? dr["CodNomenclator"].ToString() : "";
            DenNomenclator = columns.Contains("DenNomenclator") == true ? dr["DenNomenclator"].ToString() : "";

        }
    }
    #endregion

    #endregion


    public class metaQuizAngajat
    {
        public int IdQuiz { get; set; }
        public int F10003 { get; set; }
    }

    public class metaEvalDenumireSuper
    {
        public int Pozitie { get; set; }
        public string Denumire { get; set; }
        public int FaraDrepturi { get; set; }
    }
    public static class Evaluare
    {

        public static DataTable GetEval_Quiz(int? IdPerioada, DateTime? dtInceput, DateTime? dtSfarsit)
        {
            DataTable q = null;
            try
            {
                string strSql = @"select eval.""Id"", eval.""Denumire"", eval.""Titlu"", eval.""DataInceput""
                                         , eval.""DataSfarsit"", per.""DenPerioada"" ""Perioada""
                                    from ""Eval_Quiz"" eval
                                    left join ""Eval_Perioada"" per on eval.""Anul"" = per.""IdPerioada""
                                where eval.""Anul"" = {2}
                                and {0}
                                and {1}";
                if (Constante.tipBD == 1) //SQL
                {
                    strSql = string.Format(strSql,
                                           (dtInceput == new DateTime(1900, 1, 1) ? "1=1" : @"datepart(yyyy, eval.""DataInceput"") * 10000 + datepart(MM, eval.""DataInceput"") * 100 
                                            + datepart(dd, eval.""DataInceput"") >= 
                                           " + (dtInceput.Value.Year * 10000 + dtInceput.Value.Month * 100 + dtInceput.Value.Day).ToString()),
                                           (dtSfarsit == new DateTime(1900, 1, 1) ? "1=1" : @"datepart(yyyy, eval.""DataSfarsit"") * 10000 + datepart(MM, eval.""DataSfarsit"") * 100 
                                            + datepart(dd, eval.""DataSfarsit"") <= 
                                            " + (dtSfarsit.Value.Year * 10000 + dtSfarsit.Value.Month * 100 + dtSfarsit.Value.Day).ToString()),
                                           (IdPerioada == -99 ? @"eval.""Anul""" : IdPerioada.ToString())
                                           );

                }
                else //ORCL
                {
                    strSql = string.Format(strSql,
                                           (dtInceput == new DateTime(1900, 1, 1) ? "1=1" : @"to_number(to_char(eval.""DataInceput"", 'yyyyMMdd')) >= " +
                                           (dtInceput.Value.Year * 10000 + dtInceput.Value.Month * 100 + dtInceput.Value.Day).ToString()),
                                           (dtSfarsit == new DateTime(1900, 1, 1) ? "1=1" : @"to_number(to_char(eval.""DataSfarsit"", 'yyyyMMdd'))  <= "
                                            + (dtSfarsit.Value.Year * 10000 + dtSfarsit.Value.Month * 100 + dtSfarsit.Value.Day).ToString()),
                                           (IdPerioada == -99 ? @"eval.""Anul""" : IdPerioada.ToString())
                                           );
                }
                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return q;
        }

        //public static DataTable GetEval_Quiz(int? IdPerioada, DateTime? dtInceput, DateTime? dtSfarsit)
        //{
        //    DataTable q = null;
        //    try
        //    {
        //        string strSql = @"select eval.""Id"", eval.""Denumire"", eval.""Titlu"", eval.""DataInceput""
        //                                 , eval.""DataSfarsit"", per.""DenPerioada"" ""Perioada""
        //                            from ""Eval_Quiz"" eval
        //                            left join ""Eval_Perioada"" per on eval.""Anul"" = per.""IdPerioada""
        //                        where eval.""Anul"" = {0}
        //                        and {1}
        //                        and {2}";
        //        if(Constante.tipBD == 1) //SQL
        //        {
        //            strSql = string.Format(strSql,
        //                                   (IdPerioada == -99 ? @"eval.""Anul""" : IdPerioada.ToString()),
        //                                   (dtInceput == new DateTime(1900, 1, 1) ? "1=1" : @"datepart(yyyy, eval.""DataInceput"") * 10000 + datepart(MM, eval.""DataInceput"") * 100 
        //                                    + datepart(dd, eval.""DataInceput"") >= 
        //                                   " + (dtInceput.Value.Year * 10000 + dtInceput.Value.Month * 100 + dtInceput.Value.Day).ToString()),
        //                                   (dtSfarsit == new DateTime(1900, 1, 1) ? "1=1" : @"datepart(yyyy, eval.""DataSfarsit"") * 10000 + datepart(MM, eval.""DataSfarsit"") * 100 
        //                                    + datepart(dd, eval.""DataSfarsit"") <= 
        //                                    " + (dtSfarsit.Value.Year * 10000 + dtSfarsit.Value.Month * 100 + dtSfarsit.Value.Day).ToString())
        //                                   );

        //        }
        //        else //ORCL
        //        {
        //            strSql = string.Format(strSql,
        //                                   (IdPerioada == -99 ? @"eval.""Anul""" : IdPerioada.ToString()),
        //                                   (dtInceput == new DateTime(1900, 1, 1) ? "1=1" : @"to_number(to_char(eval.""DataInceput"", 'yyyyMMdd')) >= " + 
        //                                   (dtInceput.Value.Year * 10000 + dtInceput.Value.Month * 100 + dtInceput.Value.Day).ToString()),
        //                                   (dtSfarsit == new DateTime(1900, 1, 1) ? "1=1" : @"to_number(to_char(eval.""DataSfarsit"", 'yyyyMMdd'))  <= " 
        //                                    + (dtSfarsit.Value.Year * 10000 + dtSfarsit.Value.Month * 100 + dtSfarsit.Value.Day).ToString())
        //                                   );
        //        }
        //        q = General.IncarcaDT(strSql, null);
        //    }
        //    catch(Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //    return q;
        //}

        public static DataTable GetEval_Perioada()
        {
            DataTable q = null;
            try
            {
                string strSQL = "select * from \"Eval_Perioada\" ";
                q = General.IncarcaDT(strSQL, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return q;
        }

        public static DataTable GetEval_tblCategorie()
        {
            DataTable dt = null;

            try
            {
                dt = General.IncarcaDT(@"SELECT * FROM ""Eval_tblCategorie"" ", null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return dt;
        }


        //Florin 2019.10.04
        //am scos conditia AND rasp.F10003 = {10} din when {0}(istPoz.""IdSuper"", -99) = 0 AND rasp.F10003 = {10} then '@1' de la coloana Stare

        //Florin 2018.12.17
        //functia returneaza string in loc de DataTable
        //Florin 2019.02.01
        //s-a adaugat un filtru in plus IdAuto
        public static string GetEvalLista(int? idUser, int? idQuiz, int? F10003, DateTime? dtInc, DateTime? dtSf, int? tip, int? rol, int ordonat = 1, int idAuto = -99)
        {
            //DataTable dtReturnEvalLista = null;
            string strSQL = string.Empty;
            try
            {
                //Radu 19.02.2019 - am inlocuit ist cu istPoz la Stare (pt Evaluare angajat si Evaluare supervizor) si am adaugat AND rasp.F10003 = {10} la Evaluare angajat
                //Radu 20.02.2019 - am inlocuit Finalizat si PoateModifica
                //Radu 07.05.2019 - am eliminat conditia de CategorieQuiz pentru Culoare       (COALESCE(chest.""CategorieQuiz"",0)=1 OR COALESCE(chest.""CategorieQuiz"",0)=2) AND 
                strSQL = @"
                select distinct rasp.""IdAuto"", rasp.""IdQuiz"", rasp.""F10003"", chest.""Denumire"", ctg.""Denumire"" AS ""DenumireCategorie"", chest.""CategorieQuiz"",
	                chest.""DataInceput"", chest.""DataSfarsit"", {0}(fnume.""F10009"", '') {1} ' ' {1} {0}(fnume.""F10008"", '') as ""Utilizator"",
	                CASE WHEN COALESCE((SELECT COALESCE(""Aprobat"",0) FROM ""Eval_RaspunsIstoric"" WHERE F10003=rasp.F10003 and ""IdQuiz"" = rasp.""IdQuiz"" AND ""IdUser"" = {11}),0)=1 THEN '#FFE18030' ELSE '#FFFFFF00' END AS ""Culoare"",
	                case
                        when rasp.""LuatLaCunostinta"" = 2 then 'Contestat'
		                when rasp.""LuatLaCunostinta"" = 1 then 'Luat la cunostinta'
		                else 
						    CASE 
						    WHEN COALESCE(chest.""CategorieQuiz"",0)=1 THEN (CASE WHEN COALESCE((SELECT COALESCE(""Aprobat"",0) FROM ""Eval_RaspunsIstoric"" WHERE F10003=rasp.F10003 and ""IdQuiz""=rasp.""IdQuiz"" AND ""IdUser""={11}),0) = 1 THEN 'Finalizat' ELSE 'Evaluare 360' END)
						    WHEN COALESCE(chest.""CategorieQuiz"",0)=2 THEN (CASE WHEN COALESCE((SELECT COALESCE(""Aprobat"",0) FROM ""Eval_RaspunsIstoric"" WHERE F10003=rasp.F10003 and ""IdQuiz""=rasp.""IdQuiz"" AND ""IdUser""={11}),0) = 1 THEN 'Finalizat' ELSE 'Evaluare pe proiect' END)
						    ELSE
                                case
			                         when rasp.""Finalizat"" = 1 then 'Evaluare finalizata'
				                     else case 
						                      when {2} {3}) not between {2} chest.""DataInceput"") and {2} chest.""DataSfarsit"") then 'Evaluare expirata'
						                      else CASE WHEN COALESCE(chest.""CategorieQuiz"",0)=1 THEN 'Evaluare 360' ELSE
                                                   case
									                    when {0}(istPoz.""IdSuper"", -99) = 0 then '@1'
									                    else case
											                    when {0}(istPoz.""IdSuper"", -99) > 0 then 'Evaluare ' {1} usSuper.""F70104""
											                    else super.""Alias""
										                     end
							                       end
                                                END
					                      end
			                      end
                        END
	                end as ""Stare"",
	                case 
		                when dr.""PozitieVizibila"" = 0 then 1
		                else dr.""Pozitie""
	                end as ""Pozitie"",
                    rasp.""Pozitie"" as ""PozitiePeCircuit"",
	                /*ist.""IdSuper"",*/
                    CASE   WHEN COALESCE(chest.""CategorieQuiz"",0)=0 THEN rasp.""Finalizat"" ELSE (SELECT COALESCE(""Aprobat"",0) FROM ""Eval_RaspunsIstoric"" WHERE F10003=rasp.F10003 and ""IdQuiz""=rasp.""IdQuiz"" AND ""IdUser""={11}) END AS ""Finalizat"",                     
                    /*rasp.""Finalizat"",*/
	                case
		                when {2} {3}) between {2} chest.""DataInceput"") and {2} chest.""DataSfarsit"") then 1
		                else 0
	                end as ""Expirat"",
	                {0}(ist.""Aprobat"", 0) as ""Aprobat"",
	                case 
		                when {0}({13}rasLinii.""Super5""), -99) <> -99 and {0}({13}ist5.""Aprobat""), -99) = 1 then {13}rasLinii.""Super5"")
		                else case
				                when {0}({13}rasLinii.""Super4""), -99) <> -99 and {0}({13}ist4.""Aprobat""), -99) = 1 then {13}rasLinii.""Super4"")
				                else case
						                when {0}({13}rasLinii.""Super3""), -99) <> -99 and {0}({13}ist3.""Aprobat""), -99) = 1 then {13}rasLinii.""Super3"")
						                else case
								                when {0}({13}rasLinii.""Super2""), -99) <> -99 and {0}({13}ist2.""Aprobat""), -99) = 1 then {13}rasLinii.""Super2"")
								                else case
										                when {0}({13}rasLinii.""Super1""), -99) <> -99 and {0}({13}ist1.""Aprobat""), -99) = 1 then {13}rasLinii.""Super1"")
										                else ''
									                end
							                end
					                  end
			                  end
	                end as ""Rating"",
	                case
		                when {13}ist.""Pozitie"") = 1 then {13}rasLinii.""Super1_2"")
		                else case
				                when {13}ist.""Pozitie"") = 2 then {13}rasLinii.""Super2_2"")
				                else case
						                when {13}ist.""Pozitie"") = 3 then {13}rasLinii.""Super3_2"")
						                else case 
								                when {13}ist.""Pozitie"") = 4 then {13}rasLinii.""Super4_2"")
								                else case
										                when {13}ist.""Pozitie"") = 5 then {13}rasLinii.""Super5_2"")
										                else case 
												                when {13}ist.""Pozitie"") = 6 then {13}rasLinii.""Super6_2"")
												                else case
														                when {13}ist.""Pozitie"") = 7 then {13}rasLinii.""Super7_2"")
														                else case
																                when {13}ist.""Pozitie"") = 8 then {13}rasLinii.""Super8_2"")
																                else
																	                case 
																		                when {13}ist.""Pozitie"") = 9 then {13}rasLinii.""Super9_2"")
																		                else {13}rasLinii.""Super10_2"")
																	                end
															                end
													                 end
											                 end
									                end
							                end
					                end
			                end
	                end as ""Observatii"",
	                case 
		                when rasp.""Pozitie"" = ist.""Pozitie"" then 1
                        when  ctg.""Id"" != 0   then 1
		                else 0
	                end as ""PoateModifica"",
	                case 
		                when {0}(chest.""LuatLaCunostinta"", -99) = -99 then 0
		                else chest.""LuatLaCunostinta""
	                end as ""TrebuieSaIaLaCunostinta"",
	                case
		                when {0}(rasp.""LuatLaCunostinta"", -99) = -99 then 0
		                else rasp.""LuatLaCunostinta""
	                end as ""ALuatLaCunostinta"",
	                /*ist.""IdSuper"" as ""RolulMeu"",*/
	                case
		                when {0}(dr.""PozitieVizibila"", -99) = 0  then 1
		                else 0
	                end as ""FaraDrepturi"", per.""DenPerioada"",
                CASE WHEN (SELECT COUNT(*) FROM ""Eval_RaspunsIstoric"" Z WHERE Z.""IdQuiz"" = rasp.""IdQuiz"" AND ""IdUser"" = {11}) <> 0 THEN 1 ELSE 0 END AS ""Quiz360Completat""
                from ""Eval_Raspuns"" rasp
                join ""Eval_Quiz"" chest on rasp.""IdQuiz"" = chest.""Id""
                join ""F100"" fnume on rasp.""F10003"" = fnume.""F10003""
                {12} join ""Eval_RaspunsIstoric"" ist on rasp.""IdQuiz"" = ist.""IdQuiz""
							                and rasp.""F10003"" = ist.""F10003"" AND ist.""IdUser"" = {4}
                left join ""Eval_RaspunsIstoric"" istPoz on rasp.""IdQuiz"" = istPoz.""IdQuiz""
									                and rasp.""F10003"" = istPoz.""F10003""
									                and rasp.""Pozitie"" = istPoz.""Pozitie""
                left join ""tblSupervizori"" super on (-1 * istPoz.""IdSuper"") = super.""Id""
                left join USERS usSuper on istPoz.""IdUser"" = usSuper.""F70102""
                left join ""Eval_RaspunsLinii"" rasLinii on rasp.""IdQuiz"" = rasLinii.""IdQuiz""
									                and rasp.""F10003"" = rasLinii.""F10003""
									                and rasLinii.""TipData"" = 16
                left join ""Eval_Drepturi"" dr on ist.""IdQuiz"" = dr.""IdQuiz""
						                and ist.""Pozitie"" = dr.""Pozitie""
						                and 0 = dr.""PozitieVizibila""
                left join ""Eval_RaspunsIstoric"" ist1 on rasp.""IdQuiz"" = ist1.""IdQuiz""
								                and rasp.""F10003"" = ist1.""F10003""
								                and 1 = ist1.""Pozitie""
                left join ""Eval_RaspunsIstoric"" ist2 on rasp.""IdQuiz"" = ist2.""IdQuiz""
								                and rasp.""F10003"" = ist2.""F10003""
								                and 2 = ist2.""Pozitie""
                left join ""Eval_RaspunsIstoric"" ist3 on rasp.""IdQuiz"" = ist3.""IdQuiz""
								                and rasp.""F10003"" = ist3.""F10003""
								                and 3 = ist3.""Pozitie""
                left join ""Eval_RaspunsIstoric"" ist4 on rasp.""IdQuiz"" = ist4.""IdQuiz""
								                and rasp.""F10003"" = ist4.""F10003""
								                and 4 = ist4.""Pozitie""
                left join ""Eval_RaspunsIstoric"" ist5 on rasp.""IdQuiz"" = ist5.""IdQuiz""
								                and rasp.""F10003"" = ist5.""F10003""
								                and 5 = ist5.""Pozitie""
                LEFT JOIN ""Eval_tblCategorie"" ctg ON chest.""CategorieQuiz""=ctg.""Id""
                LEFT JOIN ""Eval_Perioada"" per ON chest.""Anul"" = per.""IdPerioada""
                where
                chest.""Activ"" = 1
                and rasp.""IdQuiz"" = {5}
                and rasp.""F10003"" = {6}
                and {2} chest.""DataInceput"") <= {2} {3})
                and {2} {3}) <= {2} chest.""DataSfarsit"")
                and {0}(case 
		                    when dr.""PozitieVizibila"" = 0 then 1
		                    else dr.""Pozitie""
	                    end, -99) {7}
                and {0}(ist.""IdSuper"", -99)  = {8}
                and fnume.F10025 in (0, 999)

                and (ctg.""Id"" = 0 OR (ctg.""Id"" != 0  and rasp.F10003 != {10})) ";


                //Florin 2019.02.01
                if (idAuto != -99)
                    strSQL += @" AND rasp.""IdAuto""=" + idAuto;


                if (ordonat == 1)
                    strSQL += @" order by chest.""Denumire"",  {0}(fnume.""F10009"", '') {1} ' ' {1} {0}(fnume.""F10008"", '')";
                string idUserFiltru = string.Empty;
                if (idUser != -99)
                    idUserFiltru = idUser.ToString();
                else
                    idUserFiltru = @"ist.""IdUser"" ";

                //Radu 09.07.2018
                string sqlCoordonator = string.Empty;
                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 20)
                {
                    sqlCoordonator = @" 	or  ( (ist.""Pozitie"" = 2 and {0} in (select sup.""IdUser"" from ""F100Supervizori"" sup where sup.F10003 in (select users.F10003 FROM users where f70102=ist.""IdUser"") and sup.""IdSuper""  in (1, 4)) )	) ";
                    sqlCoordonator = string.Format(sqlCoordonator, idUserFiltru);
                }

                string idQuizFiltru = string.Empty;
                if (idQuiz != -99)
                    idQuizFiltru = idQuiz.ToString();
                else
                    idQuizFiltru = @"rasp.""IdQuiz"" ";

                string F10003Filtru = string.Empty;
                if (F10003 != -99)
                    F10003Filtru = F10003.ToString();
                else
                    F10003Filtru = @"rasp.""F10003"" ";

                string tipFiltru = string.Empty;
                if (tip != -99)
                {
                    if (tip == 1)
                        tipFiltru = "= 1";
                    else
                        tipFiltru = "<> 1";
                }
                else
                {
                    tipFiltru = @" = {0}(case 
		                                    when dr.""PozitieVizibila"" = 0 then 1
		                                    else dr.""Pozitie""
	                                    end, -99)";
                    if (Constante.tipBD == 1)
                        tipFiltru = string.Format(tipFiltru, "isnull");
                    else
                        tipFiltru = string.Format(tipFiltru, "nvl");
                }

                string rolFiltru = string.Empty;
                if (rol != -99)
                    rolFiltru = "-1 * " + rol.ToString();
                else
                {//Radu 20.02.2019
                    rolFiltru = @"{0}(ist.""IdSuper"", -99) AND (ctg.""Id"" = 0 OR (ctg.""Id"" != 0  and ist.""IdUser"" =  {1}))";
                    rolFiltru = Constante.tipBD == 1 ? string.Format(rolFiltru, "isnull", HttpContext.Current.Session["UserId"].ToString()) : string.Format(rolFiltru, "nvl", HttpContext.Current.Session["UserId"].ToString());
                }

                string conversie = "";
                if (Constante.tipBD == 1)
                    conversie = "convert(varchar, ";
                else
                    conversie = "to_char(";

                //Florin 2019.01.04
                //daca este HR vede toate chestionarele
                string filtruHR = " LEFT ";
                string idHR = Dami.ValoareParam("Eval_IDuriRoluriHR", "-99");
                string sqlHr = $@"SELECT COUNT(""IdUser"") FROM ""F100Supervizori"" WHERE ""IdUser""={HttpContext.Current.Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                //if (Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlHr, null), 0)) == 0) filtruHR = " AND ist.\"IdUser\" = " + idUserFiltru;
                if (Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlHr, null), 0)) == 0) filtruHR = " INNER ";

                if (Constante.tipBD == 1) //SQL
                    strSQL = string.Format(strSQL, "isnull", "+", "convert(date,", "getdate()", idUserFiltru, idQuizFiltru, F10003Filtru, tipFiltru, rolFiltru, sqlCoordonator, HttpContext.Current.Session["User_Marca"].ToString(), HttpContext.Current.Session["UserId"].ToString(), filtruHR, conversie);
                else                      //ORACLE
                    strSQL = string.Format(strSQL, "nvl", "||", "trunc(", "sysdate", idUserFiltru, idQuizFiltru, F10003Filtru, tipFiltru, rolFiltru, sqlCoordonator, HttpContext.Current.Session["User_Marca"].ToString(), HttpContext.Current.Session["UserId"].ToString(), filtruHR, conversie);

                //Florin  2018.07.05
                strSQL = strSQL.Replace("@1", Dami.TraduCuvant("Evaluare angajat"));

                //dtReturnEvalLista = General.IncarcaDT(strSQL, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
                //dtReturnEvalLista = null;
            }
            return strSQL;
        }

        //    public static DataTable GetEvalLista(int? idUser, int? idQuiz, int? F10003, DateTime? dtInc, DateTime? dtSf, int? tip, int? rol)
        //    {
        //        DataTable dtReturnEvalLista = null;
        //        try
        //        {
        //            string strSQL = string.Empty;
        //            strSQL = @"
        //            select distinct rasp.""IdAuto"", rasp.""IdQuiz"", rasp.""F10003"", chest.""Denumire"", ctg.""Denumire"" AS DenumireCategorie,
        //             chest.""DataInceput"", chest.""DataSfarsit"", {0}(fnume.""F10009"", '') {1} ' ' {1} {0}(fnume.""F10008"", '') as ""Utilizator"",
        //             rasp.""Culoare"",
        //             case
        //                    when rasp.""LuatLaCunostinta"" = 2 then 'Contestat'
        //              when rasp.""LuatLaCunostinta"" = 1 then 'Luat la cunostinta'
        //              else 
        //		    CASE 
        //		    WHEN COALESCE(chest.CategorieQuiz,0)=1 THEN 'Evaluare 360'
        //		    WHEN COALESCE(chest.CategorieQuiz,0)=2 THEN 'Evaluare pe proiect'
        //		    ELSE
        //                            case
        //                        when rasp.""Finalizat"" = 1 then 'Evaluare finalizata'
        //                     else case 
        //		                      when {2} {3}) not between {2} chest.""DataInceput"") and {2} chest.""DataSfarsit"") then 'Evaluare expirata'
        //		                      else CASE WHEN COALESCE(chest.CategorieQuiz,0)=1 THEN 'Evaluare 360' ELSE
        //                                               case
        //					                    when {0}(ist.""IdSuper"", -99) = 0 then '@1'
        //					                    else case
        //							                    when {0}(ist.""IdSuper"", -99) > 0 then 'Evaluare ' {1} usSuper.""F70104""
        //							                    else super.""Alias""
        //						                     end
        //			                       end
        //                                            END
        //	                      end
        //                     end
        //                    END
        //             end as ""Stare"",
        //             case 
        //              when dr.""PozitieVizibila"" = 0 then 1
        //              else dr.""Pozitie""
        //             end as ""Pozitie"",
        //                rasp.""Pozitie"" as ""PozitiePeCircuit"",
        //             /*ist.""IdSuper"",*/
        //                rasp.""Finalizat"",
        //             case
        //              when {2} {3}) between {2} chest.""DataInceput"") and {2} chest.""DataSfarsit"") then 1
        //              else 0
        //             end as ""Expirat"",
        //             {0}(ist.""Aprobat"", 0) as ""Aprobat"",
        //             case 
        //              when {0}(rasLinii.""Super5"", -99) <> -99 and {0}(ist5.""Aprobat"", -99) = 1 then rasLinii.""Super5""
        //              else case
        //                when {0}(rasLinii.""Super4"", -99) <> -99 and {0}(ist4.""Aprobat"", -99) = 1 then rasLinii.""Super4""
        //                else case
        //		                when {0}(rasLinii.""Super3"", -99) <> -99 and {0}(ist3.""Aprobat"", -99) = 1 then rasLinii.""Super3""
        //		                else case
        //				                when {0}(rasLinii.""Super2"", -99) <> -99 and {0}(ist2.""Aprobat"", -99) = 1 then rasLinii.""Super2""
        //				                else case
        //						                when {0}(rasLinii.""Super1"", -99) <> -99 and {0}(ist1.""Aprobat"", -99) = 1 then rasLinii.""Super1""
        //						                else ''
        //					                end
        //			                end
        //	                  end
        //                 end
        //             end as ""Rating"",
        //             case
        //              when ist.""Pozitie"" = 1 then rasLinii.""Super1_2""
        //              else case
        //                when ist.""Pozitie"" = 2 then rasLinii.""Super2_2""
        //                else case
        //		                when ist.""Pozitie"" = 3 then rasLinii.""Super3_2""
        //		                else case 
        //				                when ist.""Pozitie"" = 4 then rasLinii.""Super4_2""
        //				                else case
        //						                when ist.""Pozitie"" = 5 then rasLinii.""Super5_2""
        //						                else case 
        //								                when ist.""Pozitie"" = 6 then rasLinii.""Super6_2""
        //								                else case
        //										                when ist.""Pozitie"" = 7 then rasLinii.""Super7_2""
        //										                else case
        //												                when ist.""Pozitie"" = 8 then rasLinii.""Super8_2""
        //												                else
        //													                case 
        //														                when ist.""Pozitie"" = 9 then rasLinii.""Super9_2""
        //														                else rasLinii.""Super10_2""
        //													                end
        //											                end
        //									                 end
        //							                 end
        //					                end
        //			                end
        //	                end
        //               end
        //             end as ""Observatii"",
        //             case 
        //              when rasp.""Pozitie"" = ist.""Pozitie"" then 1
        //              else 0
        //             end as ""PoateModifica"",
        //             case 
        //              when {0}(chest.""LuatLaCunostinta"", -99) = -99 then 0
        //              else chest.""LuatLaCunostinta""
        //             end as ""TrebuieSaIaLaCunostinta"",
        //             case
        //              when {0}(rasp.""LuatLaCunostinta"", -99) = -99 then 0
        //              else rasp.""LuatLaCunostinta""
        //             end as ""ALuatLaCunostinta"",
        //             /*ist.""IdSuper"" as ""RolulMeu"",*/
        //             case
        //              when {0}(dr.""PozitieVizibila"", -99) = 0  then 1
        //              else 0
        //             end as ""FaraDrepturi""
        //            from ""Eval_Raspuns"" rasp
        //            join ""Eval_Quiz"" chest on rasp.""IdQuiz"" = chest.""Id""
        //            join ""F100"" fnume on rasp.""F10003"" = fnume.""F10003""
        //            join ""Eval_RaspunsIstoric"" ist on rasp.""IdQuiz"" = ist.""IdQuiz""
        //			                and rasp.""F10003"" = ist.""F10003""
        //            left join ""Eval_RaspunsIstoric"" istPoz on rasp.""IdQuiz"" = istPoz.""IdQuiz""
        //					                and rasp.""F10003"" = istPoz.""F10003""
        //					                and rasp.""Pozitie"" = istPoz.""Pozitie""
        //            left join ""tblSupervizori"" super on (-1 * istPoz.""IdSuper"") = super.""Id""
        //            left join USERS usSuper on istPoz.""IdUser"" = usSuper.""F70102""
        //            left join ""Eval_RaspunsLinii"" rasLinii on rasp.""IdQuiz"" = rasLinii.""IdQuiz""
        //					                and rasp.""F10003"" = rasLinii.""F10003""
        //					                and rasLinii.""TipData"" = 16
        //            left join ""Eval_Drepturi"" dr on ist.""IdQuiz"" = dr.""IdQuiz""
        //		                and ist.""Pozitie"" = dr.""Pozitie""
        //		                and 0 = dr.""PozitieVizibila""
        //            left join ""Eval_RaspunsIstoric"" ist1 on rasp.""IdQuiz"" = ist1.""IdQuiz""
        //				                and rasp.""F10003"" = ist1.""F10003""
        //				                and 1 = ist1.""Pozitie""
        //            left join ""Eval_RaspunsIstoric"" ist2 on rasp.""IdQuiz"" = ist2.""IdQuiz""
        //				                and rasp.""F10003"" = ist2.""F10003""
        //				                and 2 = ist2.""Pozitie""
        //            left join ""Eval_RaspunsIstoric"" ist3 on rasp.""IdQuiz"" = ist3.""IdQuiz""
        //				                and rasp.""F10003"" = ist3.""F10003""
        //				                and 3 = ist3.""Pozitie""
        //            left join ""Eval_RaspunsIstoric"" ist4 on rasp.""IdQuiz"" = ist4.""IdQuiz""
        //				                and rasp.""F10003"" = ist4.""F10003""
        //				                and 4 = ist4.""Pozitie""
        //            left join ""Eval_RaspunsIstoric"" ist5 on rasp.""IdQuiz"" = ist5.""IdQuiz""
        //				                and rasp.""F10003"" = ist5.""F10003""
        //				                and 5 = ist5.""Pozitie""
        //            LEFT JOIN ""Eval_tblCategorie"" ctg ON chest.""CategorieQuiz""=ctg.""Id""
        //            where 


        //            ((ist.""IdSuper"" >= 0 and ist.""IdUser"" = {4}) or (ist.""IdSuper"" < 0 and
        //{4} in (select sup.""IdUser"" from ""F100Supervizori"" sup where sup.F10003 = ist.F10003 and sup.""IdSuper"" = (-1) * ist.""IdSuper"") ) 
        //            {9} )

        //            and chest.""Activ"" = 1
        //            and rasp.""IdQuiz"" = {5}
        //            and rasp.""F10003"" = {6}
        //            and {2} chest.""DataInceput"") <= {2} {3})
        //            and {2} {3}) <= {2} chest.""DataSfarsit"")
        //            and {0}(case 
        //                  when dr.""PozitieVizibila"" = 0 then 1
        //                  else dr.""Pozitie""
        //                 end, -99) {7}
        //            and {0}(ist.""IdSuper"", -99)  = {8}
        //            and fnume.F10025 in (0, 999)

        //            and (ctg.""Id"" = 0 OR (ctg.""Id"" != 0  and rasp.F10003 != {10}))

        //            order by chest.""Denumire"",  {0}(fnume.""F10009"", '') {1} ' ' {1} {0}(fnume.""F10008"", '')
        //                    ";

        //            string idUserFiltru = string.Empty;
        //            if (idUser != -99)
        //                idUserFiltru = idUser.ToString();
        //            else
        //                idUserFiltru = @"ist.""IdUser"" ";

        //            //Radu 09.07.2018
        //            string sqlCoordonator = string.Empty;
        //            if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 20)
        //            {
        //                sqlCoordonator = @" 	or  ( (ist.""Pozitie"" = 2 and {0} in (select sup.""IdUser"" from ""F100Supervizori"" sup where sup.F10003 in (select users.F10003 FROM users where f70102=ist.""IdUser"") and sup.""IdSuper""  in (1, 4)) )	) ";
        //                sqlCoordonator = string.Format(sqlCoordonator, idUserFiltru);
        //            }

        //            string idQuizFiltru = string.Empty;
        //            if (idQuiz != -99)
        //                idQuizFiltru = idQuiz.ToString();
        //            else
        //                idQuizFiltru = @"rasp.""IdQuiz"" ";

        //            string F10003Filtru = string.Empty;
        //            if (F10003 != -99)
        //                F10003Filtru = F10003.ToString();
        //            else
        //                F10003Filtru = @"rasp.""F10003"" ";

        //            string tipFiltru = string.Empty;
        //            if (tip != -99)
        //            {
        //                if (tip == 1)
        //                    tipFiltru = "= 1";
        //                else
        //                    tipFiltru = "<> 1";
        //            }
        //            else
        //            {
        //                tipFiltru = @" = {0}(case 
        //                                  when dr.""PozitieVizibila"" = 0 then 1
        //                                  else dr.""Pozitie""
        //                                 end, -99)";
        //                if (Constante.tipBD == 1)
        //                    tipFiltru = string.Format(tipFiltru, "isnull");
        //                else
        //                    tipFiltru = string.Format(tipFiltru, "nvl");
        //            }

        //            string rolFiltru = string.Empty;
        //            if (rol != -99)
        //                rolFiltru = "-1 * " + rol.ToString();
        //            else
        //            {
        //                rolFiltru = @"{0}(ist.""IdSuper"", -99)";
        //                rolFiltru = Constante.tipBD == 1 ? string.Format(rolFiltru, "isnull") : string.Format(rolFiltru, "nvl");
        //            }
        //            if (Constante.tipBD == 1) //SQL
        //                strSQL = string.Format(strSQL, "isnull", "+", "convert(date,", "getdate()", idUserFiltru, idQuizFiltru, F10003Filtru, tipFiltru, rolFiltru, sqlCoordonator, HttpContext.Current.Session["User_Marca"].ToString());
        //            else                      //ORACLE
        //                strSQL = string.Format(strSQL, "nvl", "||", "trunc(", "sysdate", idUserFiltru, idQuizFiltru, F10003Filtru, tipFiltru, rolFiltru, sqlCoordonator, HttpContext.Current.Session["User_Marca"].ToString());

        //            //Florin  2018.07.05
        //            strSQL = strSQL.Replace("@1", Dami.TraduCuvant("Evaluare angajat"));

        //            dtReturnEvalLista = General.IncarcaDT(strSQL, null);
        //        }
        //        catch (Exception ex)
        //        {
        //            General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
        //            dtReturnEvalLista = null;
        //        }
        //        return dtReturnEvalLista;
        //    }

        public static string InitializareChestionar(List<metaQuizAngajat> arr, int idUser, int userMarca)
        {
            string log = string.Empty;
            try
            {
                if (arr.Count == 0) return "Eroare";

                bool HR = false;
                //int idRolEcran = 1;
                //string ids = "";

                //for (int i = 0; i < arr.Count; i++)
                //{
                //    ids += "," + arr[i].Id;
                //}

                string strSelectIdQuiz = "";
                string strSelectF10003 = "";
                string strSQL = string.Empty;
                string strSelectIdQuizXF10003 = string.Empty;

                for (int i = 0; i < arr.Count; i++)
                {
                    strSelectIdQuiz += ", " + arr[i].IdQuiz.ToString();
                    strSelectF10003 += ", " + arr[i].F10003.ToString();
                    //strSQL = "delete from \"Eval_Raspuns\" where \"IdQuiz\" = {0} and \"F10003\" = {1}";
                    //strSQL = string.Format(strSQL, arr[i].IdQuiz, arr[i].F10003);
                    General.ExecutaNonQuery(@"delete from ""Eval_Raspuns"" where ""IdQuiz"" = @1 and ""F10003"" = @2", new object[] { arr[i].IdQuiz, arr[i].F10003 });

                    #region Eval_Raspuns

                    //Florin 2019.10.03

                    //strSQL = @"with cte as (select circ.""Pozitie"", max(circ.""Culoare"") as ""Culoare""
                    //           from ""Eval_CircuitCulori"" circ
                    //           where circ.""Pozitie"" = 1
                    //           group by circ.""Pozitie""),
                    //        cteSelect as (select 1 as ""Id"" {0})
                    //        insert into ""Eval_Raspuns""(""IdQuiz"", ""F10003"", ""Pozitie"", ""Finalizat"", 
                    //              ""Culoare"", ""TIME"", ""USER_NO"", ""TotalCircuit"")
                    //        select {1} as ""IdQuiz"", {2} as ""F10003"", 1 as ""Pozitie"", 0 as ""Finalizat"", 
                    //         {3}(cul.""Culoare"", '#FFFFFFFF') as ""Culoare"", {4} as TIME, {5} as USER_NO, 0 as ""TotalCircuit""
                    //        from cteSelect sel
                    //        left join cte cul on 1 = 1";

                    //switch (Constante.tipBD)
                    //{
                    //    case 1: //SQL
                    //        strSQL = string.Format(strSQL, "", arr[i].IdQuiz, arr[i].F10003, "isnull", "getdate()", idUser);
                    //        break;
                    //    case 2: //ORCL
                    //        strSQL = string.Format(strSQL, "from dual", arr[i].IdQuiz, arr[i].F10003, "nvl", "sysdate", idUser);
                    //        break;
                    //}


                    string dual = "";
                    if (Constante.tipBD == 2)
                        dual = " FROM DUAL";

                    strSQL = $@"INSERT INTO ""Eval_Raspuns""(""IdQuiz"", ""F10003"", ""Pozitie"", ""Finalizat"", ""Culoare"", ""TIME"", ""USER_NO"", ""TotalCircuit"")
                            SELECT {arr[i].IdQuiz} as ""IdQuiz"", {arr[i].F10003} as ""F10003"", 1 as ""Pozitie"", 0 as ""Finalizat"", COALESCE(cul.""Culoare"", '#FFFFFFFF') as ""Culoare"", {General.CurrentDate()} as TIME, {idUser} as USER_NO, 0 as ""TotalCircuit""
                            FROM (SELECT 1 AS ""Id"" {dual}) sel
                            LEFT JOIN (SELECT circ.""Pozitie"", MAX(circ.""Culoare"") as ""Culoare""
			                            FROM ""Eval_CircuitCulori"" circ
			                            WHERE circ.""Pozitie"" = 1
			                            GROUP BY circ.""Pozitie"") cul ON 1 = 1";

                    General.ExecutaNonQuery(strSQL, null);
                    #endregion

                    #region Eval_RaspunsIstoric

                    //Florin 2019.10.03


                    //          strSQL = "delete from \"Eval_RaspunsIstoric\" where \"IdQuiz\" = @1 and F10003 = @2";
                    //          //strSQL = string.Format(strSQL, arr[i].IdQuiz, arr[i].F10003);
                    //          General.ExecutaNonQuery(strSQL, new object[] { arr[i].IdQuiz, arr[i].F10003 });

                    //          string strConstructSuperSQL = string.Empty;
                    //          for (int j = 1; j <= 20; j++)
                    //          {
                    //              string strSuper = @" select Top 1 Super{0} as ""IdSuper"", {0} as Pozitie
                    //                                  from ""Eval_Circuit"" circ
                    //                                  where circ.""IdQuiz"" = {1}
                    //                                  and ""Super{0}"" is not null";
                    //              strSuper = string.Format(strSuper, j, arr[i].IdQuiz);
                    //              strConstructSuperSQL += (string.IsNullOrEmpty(strConstructSuperSQL) ? "" : " union all ") + strSuper + Environment.NewLine;
                    //          }
                    //          string selectSupervizori = string.Empty;
                    //          selectSupervizori = @"insert into ""Eval_RaspunsIstoric""(""IdQuiz"", ""F10003"", ""IdSuper"", ""IdUser"", ""Pozitie"", ""Culoare"",
                    //                                                                  USER_NO, TIME) 
                    //                              select {0} as ""IdQuiz"", {1} as F10003, cte.""IdSuper"", max(case when cte.""IdSuper"" = 0 then (case when users.F70102 is null then 0 else users.F70102 end)  when cte.""IdSuper"" > 0 then cte.""IdSuper""  else super.""IdUser"" end) as ""UserId"", 
                    //                                 cte.""Pozitie"",  {2}(cul.""Culoare"", '#FFFFFFFF') as ""Culoare"", {3} as USER_NO, {4} as TIME
                    //                              from cte cte
                    //                              left join ""F100Supervizori"" super on {1} = super.F10003
                    //                                                                  and (-1) * cte.""IdSuper"" = super.""IdSuper""
                    //left join users on users.F10003 = {1}											
                    //                              left join ""Eval_CircuitCulori"" cul on cte.""Pozitie"" = cul.""Pozitie""
                    //                              group by cte.""IdSuper"", cte.""Pozitie"", {2}(cul.""Culoare"", '#FFFFFFFF') ";
                    //          switch (Constante.tipBD)
                    //          {
                    //              case 1:
                    //                  selectSupervizori = string.Format(selectSupervizori, arr[i].IdQuiz, arr[i].F10003, "isnull", idUser, "getdate()");
                    //                  break;
                    //              case 2:
                    //                  selectSupervizori = string.Format(selectSupervizori, arr[i].IdQuiz, arr[i].F10003, "nvl", idUser, "sysdate");
                    //                  break;
                    //          }
                    //          string strRaspIstoric = @"with cte as ({0})
                    //                                  {1}";
                    //          strRaspIstoric = string.Format(strRaspIstoric, strConstructSuperSQL, selectSupervizori);


                    //          General.ExecutaNonQuery(strRaspIstoric, null);



                    string sqlIst_Del = $@"DELETE FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {arr[i].IdQuiz} AND F10003 = {arr[i].F10003};";

                    string sqlJoin = "";
                    for (int j = 1; j <= 20; j++)
                    {
                        if (Constante.tipBD == 1)
                            sqlJoin += $@" UNION ALL SELECT TOP 1 ""Super{j}"" AS ""IdSuper"", {j} AS ""Pozitie"" FROM ""Eval_Circuit"" circ WHERE circ.""IdQuiz"" = {arr[i].IdQuiz} AND ""Super{j}"" IS NOT NULL" + Environment.NewLine;
                        else
                            sqlJoin += $@" UNION ALL SELECT ""Super{j}"" AS ""IdSuper"", {j} AS ""Pozitie"" FROM ""Eval_Circuit"" circ WHERE circ.""IdQuiz"" = {arr[i].IdQuiz} AND ""Super{j}"" IS NOT NULL AND ROWNUM <= 1" + Environment.NewLine;
                    }

                    string sqlIst =
                        $@"INSERT INTO ""Eval_RaspunsIstoric""(""IdQuiz"", ""F10003"", ""IdSuper"", ""IdUser"", ""Pozitie"", ""Culoare"", USER_NO, TIME) 
                            SELECT {arr[i].IdQuiz} AS ""IdQuiz"", {arr[i].F10003} AS F10003, cte.""IdSuper"", 
                            MAX(CASE WHEN cte.""IdSuper"" = 0 THEN (CASE WHEN users.F70102 IS NULL THEN 0 ELSE users.F70102 END) WHEN cte.""IdSuper"" > 0 THEN cte.""IdSuper""  ELSE super.""IdUser"" END) AS ""UserId"", 
                            cte.""Pozitie"",  COALESCE(cul.""Culoare"", '#FFFFFFFF') AS ""Culoare"", {idUser} as USER_NO, {General.CurrentDate()} as TIME
                            FROM ({sqlJoin.Substring(10)}) cte
                            LEFT JOIN ""F100Supervizori"" super ON super.F10003 = {arr[i].F10003} AND (-1) * cte.""IdSuper"" = super.""IdSuper""
							LEFT JOIN users ON users.F10003 = {arr[i].F10003}											
                            LEFT JOIN ""Eval_CircuitCulori"" cul ON cte.""Pozitie"" = cul.""Pozitie""
                            GROUP BY cte.""IdSuper"", cte.""Pozitie"", COALESCE(cul.""Culoare"", '#FFFFFFFF'); ";

                    General.ExecutaNonQuery(
                        "BEGIN " + Environment.NewLine +
                        sqlIst_Del + Environment.NewLine +
                        sqlIst + Environment.NewLine +
                        " END;", null);

                    #endregion

                    #region prepare Eval_RaspunsLinii
                    strSQL = "delete from \"Eval_RaspunsLinii\" where \"IdQuiz\" = {0} and \"F10003\" = {1}";
                    strSQL = string.Format(strSQL, arr[i].IdQuiz, arr[i].F10003);
                    General.ExecutaNonQuery(strSQL, null);

                    if (!string.IsNullOrEmpty(strSelectIdQuizXF10003))
                        strSelectIdQuizXF10003 += "or" + Environment.NewLine;

                    string selectIdQuizXF10003 = string.Empty;
                    selectIdQuizXF10003 = "( rasp.F10003 = {0} and rasp.\"IdQuiz\" = {1})";
                    selectIdQuizXF10003 = string.Format(selectIdQuizXF10003, arr[i].F10003, arr[i].IdQuiz);
                    strSelectIdQuizXF10003 += selectIdQuizXF10003;
                    #endregion

                    //Radu 28.03.2018
                    General.ExecutaNonQuery("DELETE FROM \"Eval_CompetenteAngajatTemp\" WHERE F10003 = " + arr[i].F10003.ToString() + " AND \"IdQuiz\" = " + arr[i].IdQuiz.ToString(), null);
                }

                #region insert Eval_RaspunsLinii
                string cteManageri = @"with cteManageri as (select fnume.F10003, max(man.F10003) as ""F10003Manager"",
                                                                    case when fnume.""IdSuper"" = 1 then 1 else 2 end as ""Pozitie""
                                                            from ""F100Supervizori"" fnume
                                                            join ""Eval_Raspuns"" rasp on fnume.F10003 = rasp.F10003
                                                                                    and ( {0} )
                                                            join USERS us on fnume.""IdUser"" = us.F70102
                                                            join F100 man on us.F10003 = man.F10003
                                                            where fnume.""IdSuper"" in (1, 5)
                                                            group by fnume.F10003, case when fnume.""IdSuper"" = 1 then 1 else 2 end) ";
                cteManageri = string.Format(cteManageri, strSelectIdQuizXF10003);

                string cteF100 = @", cteF100 as (select fnume.F10003, fnume.F10008 {0} ' ' {0} fnume.F10009 as ""NumeComplet"",
                                                        {1}(c.F00305, '') {0} '/' {0} {1}(d.F00406, '') {0} '/' {0} {1}(e.F00507, '') {0} '/' {0} {1}(f.F00608, '') as ""Structura"",
                                                        post.""Denumire"" as ""Post"",
                                                        {1}(fnumeMan1.F10008, '') {0} ' ' {0} {1}(fnumeMan1.F10009, '') as ""NumeManager"",
                                                        postMan1.""Denumire"" as ""PostManager"", 
                                                        {1}(fnumeMan2.F10008, '') {0} ' ' {0} {1}(fnumeMan2.F10009, '') as ""NumeManager2"",
                                                        postMan2.""Denumire"" as ""PostManager2"",
                                                        rasp.""IdQuiz"" as ""IdQuiz""
                                                from F100 fnume
                                                join ""Eval_Raspuns"" rasp on fnume.F10003 = rasp.F10003
                                                                        and ( {2} )
                                                left join F002 b on fnume.F10002 = b.F00202
                                                left join F003 c on fnume.F10004 = c.F00304
                                                left join F004 d on fnume.F10005 = d.F00405
                                                left join F005 e on fnume.F10006 = e.F00506
                                                left join F006 f on fnume.F10007 = f.F00607
                                                left join ""Org_relPostAngajat"" relPost on fnume.F10003 = relPost.F10003
                                                                                and {3} between relPost.""DataInceput"" and relPost.""DataSfarsit""
                                                left join ""Org_Posturi"" post on relPost.""IdPost"" = post.""Id""
                                                left join cteManageri man1 on fnume.F10003 = man1.F10003
                                                                        and 1 = man1.""Pozitie""
                                                left join F100 fnumeMan1 on man1.""F10003Manager"" = fnumeMan1.F10003
                                                left join ""Org_relPostAngajat"" relPostMan1 on fnumeMan1.F10003 = relPostMan1.F10003
                                                                                        and {3} between relPostMan1.""DataInceput"" and relPostMan1.""DataSfarsit""
                                                left join ""Org_Posturi"" postMan1 on relPostMan1.""IdPost"" = postMan1.""Id""
                                                left join cteManageri man2 on fnume.F10003 = man2.F10003
                                                                        and 2 = man2.""Pozitie""
                                                left join F100 fnumeMan2 on man2.""F10003Manager"" = fnumeMan2.F10003
                                                left join ""Org_relPostAngajat"" relPostMan2 on fnumeMan2.F10003 = relPostMan2.F10003
                                                                                        and {3} between relPostMan2.""DataInceput"" and relPostMan2.""DataSfarsit""
                                                left join ""Org_Posturi"" postMan2 on relPostMan2.""IdPost"" = postMan2.""Id""
                                                )";
                switch (Constante.tipBD)
                {
                    case 1: //SQL
                        cteF100 = string.Format(cteF100, "+", "isnull", strSelectIdQuizXF10003, "getdate()");
                        break;
                    case 2: //ORACLE
                        cteF100 = string.Format(cteF100, "||", "nvl", strSelectIdQuizXF10003, "sysdate");
                        break;
                }

                string sqlSuper = string.Empty;
                string sqlSuperData = string.Empty;
                for (int i = 1; i <= 20; i++)
                {
                    string super = @"""Super{0}""";
                    string superData = @"case intre.""TipData""  
                                            when 13 then fnume.""NumeComplet""
                                            when 14 then fnume.""Structura""
                                            when 15 then fnume.""Post""
                                            when 18 then fnume.""NumeManager""
                                            when 19 then fnume.""PostManager""
                                            when 20 then fnume.""NumeManager2""
                                            when 21 then fnume.""PostManager2""
                                            else null
                                        end as Super{0}";
                    super = string.Format(super, i);
                    superData = string.Format(superData, i);
                    sqlSuper += super;
                    sqlSuperData += superData;
                    if (i == 20)
                        break;
                    sqlSuper += ", ";
                    sqlSuperData += ", ";
                }

                string sqlInsert = @"insert into ""Eval_RaspunsLinii""(""IdQuiz"", F10003, ""Id"", ""Linia"", ""Descriere"",
                                                                    ""DescriereInRatingGlobal"",
                                                                    ""TipData"", ""TipValoare"",
                                                                    USER_NO, TIME, {0})";
                sqlInsert = string.Format(sqlInsert, sqlSuper);

                string sqlSelectInsert = @"select fnume.""IdQuiz"", fnume.F10003, intre.""Id"", 1 as ""Linia"", intre.""Descriere"",
                                                   intre.""DescriereInRatingGlobal"",
                                                    intre.""TipData"", intre.""TipValoare"",
                                                    {0} as USER_NO, {1} as TIME,
                                                    {2}
                                             from cteF100 fnume
                                             join ""Eval_QuizIntrebari"" intre on fnume.""IdQuiz"" = intre.""IdQuiz""";
                switch (Constante.tipBD)
                {
                    case 1: //SQL
                        sqlSelectInsert = string.Format(sqlSelectInsert, idUser, "getdate()", sqlSuperData);
                        break;
                    case 2: //ORACLE
                        sqlSelectInsert = string.Format(sqlSelectInsert, idUser, "sysdate", sqlSuperData);
                        break;
                }

                string sqlInsertEval_RaspunsLinii = cteManageri + Environment.NewLine + cteF100 + Environment.NewLine + sqlInsert + Environment.NewLine + sqlSelectInsert;
                if (Constante.tipBD == 2)
                    sqlInsertEval_RaspunsLinii = sqlInsert + Environment.NewLine + cteManageri + Environment.NewLine + cteF100 + Environment.NewLine + sqlSelectInsert;
                General.ExecutaNonQuery(sqlInsertEval_RaspunsLinii, null);


                //Radu 22.02.2018 - Obiectivele
                string sqlObiective =
                    $@"SELECT A.*, B.""CategorieQuiz"" FROM ""Eval_QuizIntrebari"" A
                    INNER JOIN ""Eval_Quiz"" B ON A.""IdQuiz""=B.""Id""
                    WHERE A.""IdQuiz"" IN ({strSelectIdQuiz.Substring(1)}) AND A.""TipData"" = 23 ";
                DataTable dtObiective = General.IncarcaDT(sqlObiective, null);
                if (dtObiective != null && dtObiective.Rows.Count > 0)
                {
                    for (int i = 0; i < dtObiective.Rows.Count; i++)
                    {
                        if (dtObiective.Rows[i]["PreluareObiective"] != null && dtObiective.Rows[i]["PreluareObiective"].ToString().Length > 0)
                        {
                            for (int j = 0; j < arr.Count; j++)
                            {
                                if (dtObiective.Rows[i]["PreluareObiective"].ToString() == "0")
                                {//din nomenclator

                                    string nextId = "NEXT VALUE FOR ObiIndividuale_SEQ";
                                    if (Constante.tipBD == 2)
                                        nextId = General.Nz(General.ExecutaScalar(@"SELECT ""ObiIndividuale_SEQ"".NEXTVAL FROM DUAL", null), 1).ToString();

                                    string sqlTemp =
                                        $@"INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                        SELECT ob.""IdObiectiv"",TO_CHAR(ob.""Obiectiv""), act.""IdActivitate"", TO_CHAR(act.""Activitate""), {dtObiective.Rows[i]["IdQuiz"].ToString()}, {arr[j].F10003.ToString()}, 1, {dtObiective.Rows[i]["Id"].ToString()}, {nextId}, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                        FROM ""Eval_ListaObiectiv"" lista
                                        JOIN ""Eval_ListaObiectivDet"" listaOb on listaOb.""IdLista"" = lista.""IdLista""
                                        JOIN ""Eval_Obiectiv"" ob ON listaOb.""IdObiectiv"" = ob.""IdObiectiv""
                                        JOIN ""Eval_ObiectivXActivitate"" act on listaOb.""IdObiectiv"" = act.""IdObiectiv"" and listaob.""IdActivitate"" = act.""IdActivitate""
                                        JOIN ""Eval_SetAngajatiDetail"" setAng ON setAng.""IdSetAng"" = listaOb.""IdSetAngajat""
                                        JOIN ""Eval_ConfigObTemplateDetail"" tmpl ON  1=1
                                        WHERE setAng.""Id"" = @1 AND lista.""IdLista"" = tmpl.""IdNomenclator"" and tmpl.""TemplateId"" = @2
                                        AND tmpl.""ColumnName"" = 'Obiectiv'
                                        group by ob.""IdObiectiv"", TO_CHAR(ob.""Obiectiv""), act.""IdActivitate"", TO_CHAR(act.""Activitate"") " + Environment.NewLine;

                                    //inseram pt pozitia 1 si pentru id linie tip camp
                                    General.ExecutaNonQuery(@"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE F10003 = @1 AND ""IdQuiz"" = @2 AND ""IdLinieQuiz"" = @3", new object[] { arr[j].F10003.ToString(), dtObiective.Rows[i]["IdQuiz"].ToString(), dtObiective.Rows[i]["Id"].ToString() });
                                    General.ExecutaNonQuery(sqlTemp, new object[] { arr[j].F10003.ToString(), dtObiective.Rows[i]["TemplateIdObiectiv"].ToString() });


                                    //copiem informatia de la pozitia 1 si la celelalte pozitii din istoric
                                    if (Dami.ValoareParam("PreluareDateAutomat", "0") == "1" && Convert.ToInt32(General.Nz(dtObiective.Rows[i]["CategorieQuiz"], 0)) == 0)
                                    {
                                        string sqlInsertObi = "";
                                        string sqlSablon =
                                            @"INSERT INTO ""Eval_ObiIndividualeTemp"" 
                                                  (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                            SELECT ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, @4,          ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""=1 AND ""IdLinieQuiz""=@3;";
                                        DataTable dtIst = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""<>1 ORDER BY ""Pozitie""", new object[] { arr[j].IdQuiz, arr[j].F10003 });
                                        for (int x = 0; x < dtIst.Rows.Count; x++)
                                        {
                                            if (General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString() != "")
                                                sqlInsertObi += sqlSablon.Replace("@4", General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString()) + Environment.NewLine;
                                        }

                                        if (sqlInsertObi != "")
                                        {
                                            General.ExecutaNonQuery("BEGIN " + Environment.NewLine + sqlInsertObi + Environment.NewLine + " END;", new object[] { arr[j].IdQuiz, arr[j].F10003, dtObiective.Rows[i]["Id"].ToString() });
                                        }
                                    }
                                }
                                else
                                {
                                    if (dtObiective.Rows[i]["IdPeriod"] != null && dtObiective.Rows[i]["IdPeriod"].ToString().Length > 0)
                                    {//din obiectivele angajatului

                                        string nextId = "NEXT VALUE FOR ObiIndividuale_SEQ";
                                        if (Constante.tipBD == 2)
                                            nextId = General.Nz(General.ExecutaScalar(@"SELECT ""ObiIndividuale_SEQ"".NEXTVAL FROM DUAL", null), 1).ToString();

                                        string sqlTemp =
                                            $@"INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                            SELECT ob.""IdObiectiv"", TO_CHAR(ob.""Obiectiv""), ob.""IdActivitate"", TO_CHAR(ob.""Activitate""), {dtObiective.Rows[i]["IdQuiz"].ToString()}, {arr[j].F10003.ToString()}, 1, {dtObiective.Rows[i]["Id"].ToString()}, {nextId}, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                            FROM ""Eval_ObiIndividuale"" ob
                                            WHERE ob.""IdPeriod"" = @1 AND ob.F10003 = @2
                                            group by ob.""IdObiectiv"", TO_CHAR(ob.""Obiectiv""), ob.""IdActivitate"", TO_CHAR(ob.""Activitate"") ";

                                        //inseram pt pozitia 1 si pentru id linie tip camp
                                        General.ExecutaNonQuery(@"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE F10003 = @1 AND ""IdQuiz"" = @2 AND ""IdLinieQuiz"" = @3", new object[] { arr[j].F10003.ToString(), dtObiective.Rows[i]["IdQuiz"].ToString(), dtObiective.Rows[i]["Id"].ToString() });
                                        General.ExecutaNonQuery(sqlTemp, new object[] { dtObiective.Rows[i]["IdPeriod"], arr[j].F10003.ToString() });

                                        if (Dami.ValoareParam("PreluareDateAutomat", "0") == "1" && Convert.ToInt32(General.Nz(dtObiective.Rows[i]["CategorieQuiz"], 0)) == 0)
                                        {
                                            string sqlInsertObi = "";
                                            string sqlSablon =
                                                @"INSERT INTO ""Eval_ObiIndividualeTemp"" 
                                                      (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                                SELECT ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, @4,          ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""=1 AND ""IdLinieQuiz""=@3;";
                                            DataTable dtIst = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""<>1 ORDER BY ""Pozitie""", new object[] { arr[j].IdQuiz, arr[j].F10003 });
                                            for (int x = 0; x < dtIst.Rows.Count; x++)
                                            {
                                                if (General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString() != "")
                                                    sqlInsertObi += sqlSablon.Replace("@4", General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString());
                                            }

                                            if (sqlInsertObi != "")
                                            {
                                                General.ExecutaNonQuery("BEGIN " + Environment.NewLine + sqlInsertObi + Environment.NewLine + " END;", new object[] { arr[j].IdQuiz, arr[j].F10003, dtObiective.Rows[i]["Id"].ToString() });
                                            }
                                        }
                                    }
                                }

                                //if (sqlInsertObi != "")
                                //{
                                //    General.ExecutaNonQuery(@"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE F10003 = @1 AND ""IdQuiz"" = @2 AND ""IdLinieQuiz"" = @3", new object[] { arr[j].F10003.ToString(), dtObiective.Rows[i]["IdQuiz"].ToString(), dtObiective.Rows[i]["Id"].ToString() });
                                //    General.ExecutaNonQuery("BEGIN " + Environment.NewLine + sqlInsertObi + Environment.NewLine + " END;", new object[] { arr[j].F10003.ToString(), dtObiective.Rows[i]["TemplateIdObiectiv"].ToString() });
                                //}

                            }
                        }
                    }
                }

                //Radu 06.07.2018 - Competentele
                //string sqlCompetente = "SELECT * FROM \"Eval_QuizIntrebari\" WHERE \"IdQuiz\" in (" + strSelectIdQuiz.Substring(1) + ") AND \"TipData\" = 5 ";
                string sqlCompetente =
                    $@"SELECT A.*, B.""CategorieQuiz"" FROM ""Eval_QuizIntrebari"" A
                    INNER JOIN ""Eval_Quiz"" B ON A.""IdQuiz""=B.""Id""
                    WHERE A.""IdQuiz"" IN ({strSelectIdQuiz.Substring(1)}) AND A.""TipData"" = 5 ";
                DataTable dtCompetente = General.IncarcaDT(sqlCompetente, null);
                if (dtCompetente != null && dtCompetente.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCompetente.Rows.Count; i++)
                    {
                        if (dtCompetente.Rows[i]["PreluareCompetente"] != null && dtCompetente.Rows[i]["PreluareCompetente"].ToString().Length > 0)
                        {
                            for (int j = 0; j < arr.Count; j++)
                            {
                                if (dtCompetente.Rows[i]["PreluareCompetente"].ToString() == "0")
                                {//din nomenclator

                                    string nextId = "NEXT VALUE FOR CompetenteAng_SEQ";
                                    if (Constante.tipBD == 2)
                                        nextId = General.Nz(General.ExecutaScalar(@"SELECT ""CompetenteAng_SEQ"".NEXTVAL FROM DUAL", null), 1).ToString();

                                    string sqlTemp =
                                        $@"INSERT INTO ""Eval_CompetenteAngajatTemp"" (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                        SELECT comp.""IdCategorie"", comp.""DenCategorie"", compDet.""IdCompetenta"", compDet.""DenCompetenta"", {dtCompetente.Rows[i]["IdQuiz"].ToString()}, {arr[j].F10003.ToString()}, 1, {dtCompetente.Rows[i]["Id"].ToString()}, {nextId}, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                        FROM ""Eval_CategCompetente"" comp
                                        JOIN ""Eval_CategCompetenteDet"" compDet on compDet.""IdCategorie"" = comp.""IdCategorie""
                                        JOIN ""Eval_SetAngajati"" setAng ON setAng.""CodSet"" = comp.""CodCategorie""
                                        JOIN ""Eval_SetAngajatiDetail"" setAngDet ON   setAng.""IdSetAng"" = setAngDet.""IdSetAng""
                                        JOIN ""Eval_ConfigCompTemplateDetail"" tmpl ON  1=1
                                        WHERE setAngDet.""Id"" = @1 AND comp.""IdCategorie"" = tmpl.""IdNomenclator"" and tmpl.""TemplateId"" = @2
                                        AND tmpl.""ColumnName"" = 'Competenta'
                                        group by comp.""IdCategorie"", comp.""DenCategorie"", compDet.""IdCompetenta"", compDet.""DenCompetenta"" ";

                                    //inseram pt pozitia 1 si pentru id linie tip camp
                                    General.ExecutaNonQuery(@"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE F10003 = @1 AND ""IdQuiz"" = @2 AND ""IdLinieQuiz"" = @3", new object[] { arr[j].F10003.ToString(), dtCompetente.Rows[i]["IdQuiz"].ToString(), dtCompetente.Rows[i]["Id"].ToString() });
                                    General.ExecutaNonQuery(sqlTemp, new object[] { arr[j].F10003.ToString(), General.Nz(dtCompetente.Rows[i]["TemplateIdCompetenta"].ToString(), "-99") });

                                    //copiem informatia de la pozitia 1 si la celelalte pozitii din istoric
                                    if (Dami.ValoareParam("PreluareDateAutomat", "0") == "1" && Convert.ToInt32(General.Nz(dtCompetente.Rows[i]["CategorieQuiz"], 0)) == 0)
                                    {
                                        string sqlInsertObi = "";
                                        string sqlSablon =
                                            @"INSERT INTO ""Eval_CompetenteAngajatTemp"" 
                                                  (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                            SELECT ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, @4         , ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""=1 AND ""IdLinieQuiz""=@3;";
                                        DataTable dtIst = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""<>1 ORDER BY ""Pozitie""", new object[] { arr[j].IdQuiz, arr[j].F10003 });
                                        for (int x = 0; x < dtIst.Rows.Count; x++)
                                        {
                                            if (General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString() != "")
                                                sqlInsertObi += sqlSablon.Replace("@4", General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString()) + Environment.NewLine;
                                        }

                                        if (sqlInsertObi != "")
                                        {
                                            General.ExecutaNonQuery("BEGIN " + Environment.NewLine + sqlInsertObi + Environment.NewLine + " END;", new object[] { arr[j].IdQuiz, arr[j].F10003, dtCompetente.Rows[i]["Id"].ToString() });
                                        }
                                    }

                                }
                                else
                                {
                                    if (dtCompetente.Rows[i]["IdPeriodComp"] != null && dtCompetente.Rows[i]["IdPeriodComp"].ToString().Length > 0)
                                    {//din competentele angajatului

                                        string nextId = "NEXT VALUE FOR CompetenteAng_SEQ";
                                        if (Constante.tipBD == 2)
                                            nextId = General.Nz(General.ExecutaScalar(@"SELECT ""CompetenteAng_SEQ"".NEXTVAL FROM DUAL", null), 1).ToString();

                                        string sqlTemp =
                                            $@"INSERT INTO ""Eval_CompetenteAngajatTemp"" (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                            SELECT comp.""IdCategCompetenta"", comp.""CategCompetenta"", comp.""IdCompetenta"", comp.""Competenta"", {dtCompetente.Rows[i]["IdQuiz"].ToString()}, {arr[j].F10003.ToString()}, 1, { dtCompetente.Rows[i]["Id"].ToString()}, {nextId}, {HttpContext.Current.Session["UserId"]}, {General.CurrentDate()}
                                            FROM ""Eval_CompetenteAngajat"" comp
                                            WHERE comp.""IdPeriod"" = @1 AND comp.F10003 = @2
                                            GROUP BY comp.""IdCategCompetenta"", comp.""CategCompetenta"", comp.""IdCompetenta"", comp.""Competenta"" ";


                                        //inseram pt pozitia 1 si pentru id linie tip camp
                                        General.ExecutaNonQuery(@"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE F10003 = @1 AND ""IdQuiz"" = @2 AND ""IdLinieQuiz"" = @3", new object[] { arr[j].F10003.ToString(), dtCompetente.Rows[i]["IdQuiz"].ToString(), dtCompetente.Rows[i]["Id"].ToString() });
                                        General.ExecutaNonQuery(sqlTemp, new object[] { dtCompetente.Rows[i]["IdPeriodComp"], arr[j].F10003.ToString() });

                                        if (Dami.ValoareParam("PreluareDateAutomat", "0") == "1" && Convert.ToInt32(General.Nz(dtCompetente.Rows[i]["CategorieQuiz"], 0)) == 0)
                                        {
                                            string sqlInsertObi = "";
                                            string sqlSablon =
                                                @"INSERT INTO ""Eval_CompetenteAngajatTemp"" 
                                                  (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                            SELECT ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, @4         , ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""=1 AND ""IdLinieQuiz""=@3;";
                                            DataTable dtIst = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""<>1 ORDER BY ""Pozitie""", new object[] { arr[j].IdQuiz, arr[j].F10003 });
                                            for (int x = 0; x < dtIst.Rows.Count; x++)
                                            {
                                                if (General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString() != "")
                                                    sqlInsertObi += sqlSablon.Replace("@4", General.Nz(dtIst.Rows[x]["Pozitie"], "").ToString()) + Environment.NewLine;
                                            }

                                            if (sqlInsertObi != "")
                                            {
                                                General.ExecutaNonQuery("BEGIN " + Environment.NewLine + sqlInsertObi + Environment.NewLine + " END;", new object[] { arr[j].IdQuiz, arr[j].F10003, dtCompetente.Rows[i]["Id"].ToString() });
                                            }
                                        }
                                    }
                                }

                                //if (sqlInsertComp != "")
                                //{
                                //    General.ExecutaNonQuery("DELETE FROM \"Eval_CompetenteAngajatTemp\" WHERE F10003 = " + arr[j].F10003.ToString() + " AND \"IdQuiz\" = " + dtCompetente.Rows[i]["IdQuiz"].ToString() + " AND \"IdLinieQuiz\" = " + dtCompetente.Rows[i]["Id"].ToString(), null);
                                //    General.ExecutaNonQuery(sqlInsertComp, null);
                                //}

                            }
                        }
                    }
                }



                #endregion

                #region update TotalCircuit
                string sqlUpdateTotalCircuit = @"select rasp.""IdAuto"", rasp.""IdQuiz"", rasp.F10003, count(*) as ""TotalCircuit""
                                                 from ""Eval_Raspuns"" rasp
                                                 join ""Eval_RaspunsIstoric"" ist on rasp.""IdQuiz"" = ist.""IdQuiz""
                                                                                and rasp.F10003 = ist.F10003
                                                 where {0}
                                                 group by rasp.""IdAuto"", rasp.""IdQuiz"", rasp.F10003";
                sqlUpdateTotalCircuit = string.Format(sqlUpdateTotalCircuit, strSelectIdQuizXF10003);
                DataTable dtUpdateTotalCircuit = General.IncarcaDT(sqlUpdateTotalCircuit, null);
                string sqlUpdate = string.Empty;
                if (dtUpdateTotalCircuit != null && dtUpdateTotalCircuit.Rows.Count != 0)
                {
                    foreach (DataRow rwUpdate in dtUpdateTotalCircuit.Rows)
                    {
                        sqlUpdate = "update \"Eval_Raspuns\" set \"TotalCircuit\" = {0} where \"IdQuiz\" = {1} and \"F10003\" = {2} and \"IdAuto\" = {3} ";
                        sqlUpdate =
                            string.Format(sqlUpdate, rwUpdate["TotalCircuit"].ToString(), rwUpdate["IdQuiz"].ToString(), rwUpdate["F10003"].ToString(), rwUpdate["IdAuto"].ToString());

                        General.ExecutaNonQuery(sqlUpdate, null);
                    }
                }
                #endregion
                log = "Chestionarele au fost initializate!";

                for (int i = 0; i < arr.Count; i++)
                {
                    string msg = Notif.TrimiteNotificare("Eval.EvalLista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"" FROM ""Eval_Raspuns"" Z WHERE ""IdQuiz""=" + arr[i].IdQuiz.ToString() + @"AND F10003 = " + arr[i].F10003.ToString(), "", -99, idUser, userMarca);
                    if (msg.Length > 0)
                        General.CreazaLog(msg);
                }
            }
            catch (Exception ex)
            {
                log = ex.Message;
                General.MemoreazaEroarea(ex, "Evaluare", "InitializareChestionar");
            }

            return log;
        }

        public static DataTable GetSupervizoriEval(int idUser)
        {
            DataTable q = new DataTable();
            try
            {
                string strSQL = @"select super.""IdSuper"" as ""Id"", tblSuper.""Denumire""
                                from ""F100Supervizori"" super
                                join ""tblSupervizori"" tblSuper on super.""IdSuper"" = tblSuper.""Id""
                                where super.""IdUser"" = @1
                                union all
                                select tblSuper.""Id"", tblSuper.""Denumire""
                                from ""tblSupervizori"" tblSuper
                                where tblSuper.""Id"" = 0 ";
                //strSQL = string.Format(strSQL, idUser);
                q = General.IncarcaDT(strSQL, new object[] { idUser });
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return q;
        }

        public static List<metaEvalDenumireSuper> EvalDenumireSuper(int idQuiz, int pozitie, int consultantRU = 0)
        {
            List<metaEvalDenumireSuper> lstEvalDenumireSuper = new List<metaEvalDenumireSuper>();
            try
            {
                List<Eval_Drepturi> lstDr = new List<Eval_Drepturi>();
                string sqlQuery = string.Empty;

                //Florin 2018.12.20
                //sqlQuery = @"select * from ""Eval_Drepturi"" where ""IdQuiz""={0} and ""Pozitie"" <= {1}";
                sqlQuery = @"select * from ""Eval_Drepturi"" where ""IdQuiz""=@1 and ""Pozitie"" = @2";

                //sqlQuery = string.Format(sqlQuery, idQuiz, pozitie);
                DataTable dtDr = General.IncarcaDT(sqlQuery, new object[] { idQuiz, pozitie });

                foreach (DataRow dr in dtDr.Rows)
                {
                    Eval_Drepturi clsDrept = new Eval_Drepturi(dr);
                    lstDr.Add(clsDrept);
                }

                List<Eval_Circuit> lstCir = new List<Eval_Circuit>();
                Eval_Circuit entCir = new Eval_Circuit();
                sqlQuery = @"select * from ""Eval_Circuit"" where ""IdQuiz"" = @1";
                //sqlQuery = string.Format(sqlQuery, idQuiz);
                DataTable dtCir = General.IncarcaDT(sqlQuery, new object[] { idQuiz });
                foreach (DataRow dr in dtCir.Rows)
                {
                    Eval_Circuit clsCircuit = new Eval_Circuit(dr);
                    lstCir.Add(clsCircuit);
                }

                if (lstCir.Count != 0)
                    entCir = lstCir.FirstOrDefault();
                else
                    entCir = null;

                if (entCir != null)
                {
                    for (int i = 1; i <= 20; i++)
                    {
                        PropertyInfo piCir = entCir.GetType().GetProperty("Super" + i);
                        if (piCir != null)
                        {
                            //Florin 2018.12.21

                            //bool ok = false;
                            //if (i == pozitie)
                            //{
                            //    ok = true;
                            //    //cazul cand nu are drepturi
                            //    if (lstDr.Where(p => p.Pozitie == pozitie && p.PozitieVizibila == 0).Count() > 0) ok = false;
                            //}
                            //else
                            //{
                            //    if (lstDr.Where(p => p.PozitieVizibila == i).Count() > 0)
                            //    {
                            //        ok = true;
                            //    }
                            //}

                            bool ok = false;
                            if (lstDr.Where(p => p.PozitieVizibila == i).Count() > 0)
                                ok = true;


                            //consultantul RU vede toate taburile de pe circuit
                            if (consultantRU == 1) ok = true;

                            if (ok)
                            {
                                int id = Convert.ToInt32(piCir.GetValue(entCir, null) ?? -99);
                                if (id == 0)
                                {
                                    //Florin 2018.04.10
                                    //DataTable dtAls = General.IncarcaDT($@"SELECT * FROM ""tblSupervizori"" WHERE ""Id""= -1 * {id}", null);
                                    string idCateg = General.Nz(General.ExecutaScalar(@"SELECT ""CategorieQuiz"" FROM ""Eval_Quiz"" WHERE ""Id""=@1", new object[] { idQuiz }), "0").ToString();
                                    string alias = Dami.TraduCuvant("Angajat", "Evaluat");
                                    if (idCateg == "1" || idCateg == "2") alias = Dami.TraduCuvant("Evaluator ", "Evaluator");
                                    //if (dtAls != null && dtAls.Rows.Count > 0) alias = General.Nz(dtAls.Rows[0]["Alias"], dtAls.Rows[0]["Denumire"]).ToString();
                                    lstEvalDenumireSuper.Add(new metaEvalDenumireSuper() { Pozitie = i, Denumire = alias });
                                }
                                else
                                {
                                    if (id < 0)
                                    {
                                        List<tblSupervizori> entSup = new List<tblSupervizori>();
                                        string strSuper = string.Empty;
                                        strSuper = @"select * from ""tblSupervizori"" where ""Id"" = -1 * @1";
                                        //strSuper = string.Format(strSuper, id);
                                        DataTable dtSuper = General.IncarcaDT(strSuper, new object[] { id });
                                        foreach (DataRow drSuper in dtSuper.Rows)
                                        {
                                            tblSupervizori clsSuper = new tblSupervizori(drSuper);
                                            entSup.Add(clsSuper);
                                        }
                                        if (entSup != null && entSup.Count() != 0)
                                        {
                                            lstEvalDenumireSuper.Add(new metaEvalDenumireSuper() { Pozitie = i, Denumire = entSup.FirstOrDefault().Alias ?? entSup.FirstOrDefault().Denumire });
                                        }
                                    }
                                    else
                                    {
                                        string strUsers = string.Empty;
                                        strUsers = @"select * from USERS where F70102 ={0}";
                                        strUsers = string.Format(strUsers, id);
                                        DataTable dtUsers = General.IncarcaDT(strUsers, null);
                                        List<USERS> entUsr = new List<USERS>();
                                        foreach (DataRow drUser in dtUsers.Rows)
                                        {
                                            USERS clsUser = new USERS(drUser);
                                            entUsr.Add(clsUser);
                                        }

                                        if (entUsr != null && entUsr.Count != 0)
                                            lstEvalDenumireSuper.Add(new metaEvalDenumireSuper() { Pozitie = i, Denumire = entUsr.FirstOrDefault().F70104 ?? "" });

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return lstEvalDenumireSuper;
        }

        public static Color CuloareBrush(string strCuloare)
        {
            try
            {
                string s = "#FFFFFFFF";
                if (strCuloare != null)
                    s = strCuloare.ToString();

                byte a = System.Convert.ToByte(s.Substring(1, 2), 16);
                byte r = System.Convert.ToByte(s.Substring(3, 2), 16);
                byte g = System.Convert.ToByte(s.Substring(5, 2), 16);
                byte b = System.Convert.ToByte(s.Substring(7, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Evaluare", new StackTrace().GetFrame(0).GetMethod().Name);
                return Color.FromArgb(255, 255, 255, 255);
            }
        }

        public static DataTable ToDataTable<T>(this List<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

    }
}