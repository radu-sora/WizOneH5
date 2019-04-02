using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ProceseSec;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DevExpress.Web;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace WizOne.Module
{
 //     public enum MidpointRounding
	//{
	//    ToEven,
	//    AwayFromZero
	//}


    public class MathExt
    {

        public static decimal Round(decimal d, MidpointRounding mode)
	    {
	        return MathExt.Round(d, 0, mode);
	    }

        public static decimal Round(decimal d, int decimals, MidpointRounding mode)
        {
            decimal dec = 0;

            try
            {
                if (mode == MidpointRounding.ToEven)
                {
                    dec = decimal.Round(d, decimals);
                }
                else
                {
                    decimal factor = Convert.ToDecimal(Math.Pow(10, decimals));
                    int sign = Math.Sign(d);
                    dec = Decimal.Truncate(d * factor + 0.5m * sign) / factor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dec;
        }



    }
}