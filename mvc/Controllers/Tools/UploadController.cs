namespace Galex.Controllers
{
    using LIB.BusinessObjects;
    using LIB.Tools.Security;
    using LIB.Tools.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Weblib.Controllers;
    using LIB.Helpers;
    using System.Web;

    public class UploadController : BaseController
    {
        [HttpPost]
        public ActionResult DoUploadImage(HttpPostedFileBase file)
        {
            if (!Authentication.CheckUser(this.HttpContext))
                return new RedirectResult(Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));

            if (file != null
                && !string.IsNullOrEmpty(Request.QueryString["AdminWidth"])
                && !string.IsNullOrEmpty(Request.QueryString["AdminHeight"])
                && !string.IsNullOrEmpty(Request.QueryString["Width"])
                && !string.IsNullOrEmpty(Request.QueryString["Height"]))
            {
                try
                {
                    var BOName = Request.QueryString["BOName"];
                    var AdminWidth = Request.QueryString["AdminWidth"] != "0" ? Convert.ToInt32(Request.QueryString["AdminWidth"]) : 50;
                    var AdminHeight = Request.QueryString["AdminHeight"] != "0" ? Convert.ToInt32(Request.QueryString["AdminHeight"]) : 50;
                    var Width = Request.QueryString["Width"] != "0" ? Convert.ToInt32(Request.QueryString["Width"]) : 50;
                    var Height = Request.QueryString["Height"] != "0" ? Convert.ToInt32(Request.QueryString["Height"]) : 50;

                    string pic = Path.GetFileNameWithoutExtension(file.FileName);
                    string ext = Path.GetExtension(file.FileName);

                    var UploadPart = Config.GetConfigValue("UploadPart");

                    if (ext.ToLower() != ".png" && ext.ToLower() != ".jpg" && ext.ToLower() != ".jpeg" && ext.ToLower() != ".gif" && ext.ToLower() != ".bmp")
                    {
                        return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "Ошибка формата" });
                    }
                    if (!IsImage(file))
                    {
                        return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "Загруженный фаил не является изображением" });
                    }
                    if (!Directory.Exists(Server.MapPath(UploadPart)))
                    {
                        Directory.CreateDirectory(Server.MapPath(UploadPart));
                    }
                    if (!Directory.Exists(Server.MapPath(UploadPart + BOName)))
                    {
                        Directory.CreateDirectory(Server.MapPath(UploadPart + BOName));
                    }
                    var i = 0;
                    while (System.IO.File.Exists(Path.Combine(Server.MapPath(UploadPart + BOName), pic + "_original" + ext)))
                    {
                        i++;
                        pic = Path.GetFileNameWithoutExtension(file.FileName) + "_" + i.ToString();
                    }
                    string path = Path.Combine(Server.MapPath(UploadPart + BOName), pic + "_original" + ext);
                    // file is uploaded
                    file.SaveAs(path);

                    ImageResizer.ResizeImageAndRatio(path, Path.Combine(Server.MapPath(UploadPart + BOName), pic + "_adminthumb.jpeg"), AdminWidth, AdminHeight);
                    ImageResizer.ResizeImageAndRatio(path, Path.Combine(Server.MapPath(UploadPart + BOName), pic + ".jpeg"), Width, Height);

                    Graphic uploadedImage = new Graphic
                    {
                        BOName = BOName,
                        Ext = ext.Replace(".", ""),
                        Name = pic
                    };

                    uploadedImage.Insert(uploadedImage);
                    var Data = new Dictionary<string, object>
                    {
                        { "Id", uploadedImage.Id },
                        { "thumb", uploadedImage.AdminThumbnail }
                    };

                    return this.Json(Data);
                }
                catch (Exception ex)
                {
                    return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "File Uploading Failed:" + ex.ToString() });
                }
            }
            return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "File Uploading Failed" });
        }

        [HttpPost]
        public ActionResult DoUploadFile(HttpPostedFileBase file)
        {
            if (!Authentication.CheckUser(this.HttpContext)) //TBD
                return new RedirectResult(Config.GetConfigValue("LoginPage") + "?ReturnUrl=" + HttpUtility.UrlEncode(Request.Url.AbsolutePath));

            if (file != null)
            {
                try
                {
                    string name = Path.GetFileNameWithoutExtension(file.FileName);
                    string ext = Path.GetExtension(file.FileName).ToLower();

                    var UploadPart = Config.GetConfigValue("UploadPart");

                    if (ext != ".pdf" && ext != ".doc" && ext != ".docx" && ext != ".xls" && ext != ".xlsx" && ext != ".xlsm" && ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    {
                        return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = $"Ошибка фомата {ext.ToLower()}" });
                    }
                    if (!Directory.Exists(Server.MapPath(UploadPart)))
                    {
                        Directory.CreateDirectory(Server.MapPath(UploadPart));
                    }
                    if (!Directory.Exists(Server.MapPath(UploadPart + "Documents")))
                    {
                        Directory.CreateDirectory(Server.MapPath(UploadPart + "Documents"));
                    }
                    var i = 0;
                    while (System.IO.File.Exists(Path.Combine(Server.MapPath(UploadPart + "Documents"), name + ext)))
                    {
                        i++;
                        name = Path.GetFileNameWithoutExtension(file.FileName) + "_" + i.ToString();
                    }
                    string path = Path.Combine(Server.MapPath(UploadPart + "Documents"), name + ext);
                    // file is uploaded
                    file.SaveAs(path);

                    Document uploadedDoc = new Document
                    {
                        Ext = ext.Replace(".", ""),
                        Name = Path.GetFileNameWithoutExtension(file.FileName),
                        FileName = name
                    };

                    uploadedDoc.Insert(uploadedDoc);
                    var Data = new Dictionary<string, object>
                    {
                        { "Id", uploadedDoc.Id },
                        { "file", uploadedDoc.FileWeb },
                        { "path", path },
                        { "name", $"{uploadedDoc.Name}.{uploadedDoc.Ext}" }
                    };

                    return this.Json(new RequestResult() { Result = RequestResultType.Success, Data = Data });
                }
                catch (Exception ex)
                {
                    return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "File Uploading Failed:" + ex.ToString() });
                }
            }
            return this.Json(new RequestResult() { Result = RequestResultType.Fail, Message = "File Uploading Failed" });
        }

        public const int ImageMinimumBytes = 512;

        private static bool IsImage(HttpPostedFileBase postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }

                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[512];
                postedFile.InputStream.Read(buffer, 0, 512);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}