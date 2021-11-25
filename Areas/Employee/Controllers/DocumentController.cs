using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using protean.Models;
using protean.Areas.Employee.Models;
using System.Threading.Tasks;

namespace protean.Areas.Employee.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Employee/Document
        public ActionResult Document()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Document(Document obj)
        {
            try
            {
                string strDateTime = System.DateTime.Now.ToString("ddMMyyyy");
                string finalPath = "\\UploadedFile\\" + strDateTime + obj.UploadFile.FileName;

                ///obj.UploadFile.SaveAs(Server.MapPath("~") + finalPath);
                obj.FileImg = finalPath;

                string path = Path.Combine(Server.MapPath("~/UploadedFiles"), Path.GetFileName(obj.UploadFile.FileName));
                obj.UploadFile.SaveAs(path);
                ViewBag.Message = "Saved Successfully";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return View();
            }
        }



    }
}