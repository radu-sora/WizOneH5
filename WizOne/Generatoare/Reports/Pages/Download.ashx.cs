using System;
using System.IO;
using System.Web;

namespace WizOne.Generatoare.Reports.Pages
{
    /// <summary>
    /// File download http handler
    /// </summary>
    public class Download : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string fileName = string.Empty;

            // Get the file name from the querystring
            if (context.Request.QueryString["FileName"] != null)
                fileName = context.Request.QueryString["FileName"].ToString();

            fileName = context.Server.MapPath("~/Temp/" + fileName);

            FileInfo fileInfo = new FileInfo(fileName);

            try
            {
                if (fileInfo.Exists)
                {
                    context.Response.Clear();
                    context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileInfo.Name + "\"");
                    context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    context.Response.ContentType = "application/octet-stream";
                    context.Response.TransmitFile(fileInfo.FullName);
                    context.Response.Flush();
                }
                else
                    throw new Exception("File not found.");                
            }
            catch (Exception ex)
            {
                // Log error here
                // For now, just display the error message
                context.Response.ContentType = "text/plain";
                context.Response.Write(ex.Message);
            }
            finally
            {
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}