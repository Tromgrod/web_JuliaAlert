using System;
using System.Web.Mvc;
using System.Collections.Generic;
using LIB.Helpers;
using LIB.BusinessObjects.Barcode;
using Weblib.Controllers;
using JuliaAlert.Models.Objects;
using OpenHtmlToPdf;
using UrlHelper = LIB.Tools.Utils.URLHelper;

namespace JuliaAlert.Controllers
{
    public class BarcodeHelperController : BaseController
    {
        public JsonResult Scanning()
        {
            var code = Request.Form["Code"];

            RequestResult requestResult = new RequestResult { Result = RequestResultType.Fail };
            if (Barcode_EAN13.IsBarcode(code))
            {
                requestResult = Scanning_EAN13(code);
            }
            else if (Barcode_CODE128.IsBarcode(code))
            {
                requestResult = Scanning_CODE128(code);
            }

            return this.Json(requestResult);
        }

        private RequestResult Scanning_CODE128(string code)
        {
            var requestResult = new RequestResult();

            try
            {
                var barcode = new Barcode_CODE128(code);

                switch (barcode.BarType)
                {
                    case Barcode_CODE128.Type.Finding:
                        var findingColor = barcode.GetObject<FindingColor>();
                        if (findingColor.Id > 0)
                        {
                            requestResult.Result = RequestResultType.Success;
                            requestResult.RedirectURL = UrlHelper.GetUrl("DocControl/FindingColor/" + findingColor.Id);
                            requestResult.Data = new Dictionary<string, object>
                            {
                                { "BarFun", nameof(findingColor) },
                                { "Object", findingColor }
                            };
                        }
                        else
                        {
                            requestResult.Result = RequestResultType.Fail;
                            requestResult.Message = "Такой фурнитуры не существует!";
                        }
                        break;

                    case Barcode_CODE128.Type.Textile:
                        var textileColor = barcode.GetObject<TextileColor>();
                        if (textileColor.Id > 0)
                        {
                            requestResult.Result = RequestResultType.Success;
                            requestResult.RedirectURL = UrlHelper.GetUrl("DocControl/TextileColor/" + textileColor.Id);
                            requestResult.Data = new Dictionary<string, object>
                            {
                                { "BarFun", nameof(textileColor) },
                                { "Object", textileColor }
                            };
                        }
                        else
                        {
                            requestResult.Result = RequestResultType.Fail;
                            requestResult.Message = "Такой ткани не существует!";
                        }
                        break;

                    default:
                        requestResult.Result = RequestResultType.Fail;
                        requestResult.Message = "Недействительный штирхкод! " + barcode.Code;
                        break;
                }
            }
            catch (Exception ex)
            {
                requestResult.Result = RequestResultType.Fail;
                requestResult.Message = ex.Message;
            }

            return requestResult;
        }

        private RequestResult Scanning_EAN13(string code)
        {
            var requestResult = new RequestResult();

            var barcode = new Barcode_EAN13(code);

            var specificProduct = barcode.GetObject<SpecificProduct>();

            if (specificProduct.Id > 0)
            {
                requestResult.Result = RequestResultType.Success;
                requestResult.RedirectURL = UrlHelper.GetUrl("DocControl/UniqueProduct/" + specificProduct.UniqueProduct.Id);
                requestResult.Data = new Dictionary<string, object>
                {
                    { "BarFun", nameof(specificProduct) },
                    { "Object", specificProduct }
                };
            }
            else
            {
                requestResult.Result = RequestResultType.Fail;
                requestResult.Message = "Такой модели не существует! " + barcode.Code;
            }

            return requestResult;
        }

        [ValidateInput(false)]
        public void BarcodePreviewPdf_CODE128(string barcodeData, int barcodeType = default, int count = 1, double width = 4, double height = 1.5)
        {
            var codes = barcodeData.Split('|');
            var barcodes = new Barcode_CODE128[codes.Length];

            for (var index = 0; index < codes.Length; index++)
                barcodes[index] = new Barcode_CODE128(codes[index], (Barcode_CODE128.Type)barcodeType);

            ViewData["Count"] = count;

            var content = this.RenderRazorViewToString("~/Views/BarcodeHelper/Barcode_CODE128.cshtml", barcodes);

            var paperSize = new PaperSize(Length.Inches(width), Length.Inches(height));

            this.WritePdfToContext(content, paperSize);
        }

        [ValidateInput(false)]
        public void BarcodePreviewPdf_EAN13(string barcodeData, int count = 1, string dateMoving = default)
        {
            var codes = barcodeData.Split('|');
            var barcodes = new Barcode_EAN13[codes.Length];

            for (var index = 0; index < codes.Length; index++)
                barcodes[index] = new Barcode_EAN13(codes[index], 3.2f);

            ViewData["DateMoving"] = dateMoving;
            ViewData["Count"] = count;

            var content = this.RenderRazorViewToString("~/Views/BarcodeHelper/Barcode_EAN13.cshtml", barcodes);

            int A4_Width = 210,
                A4_Height = 297;

            var A4Rotated_x2 = new PaperSize(Length.Millimeters(A4_Height * 2), Length.Millimeters(A4_Width * 2));

            this.WritePdfToContext(content, A4Rotated_x2);
        }

        private void WritePdfToContext(string contentPdf, PaperSize paperSize)
        {
            if (string.IsNullOrWhiteSpace(contentPdf))
                contentPdf = "<h1>No Data</h1>";

            var margins = PaperMargins.All(Length.Millimeters(0));

            var bytesRes = Pdf.From(contentPdf).OfSize(paperSize).WithMargins(margins).Content();

            HttpContext.Response.Clear();
            HttpContext.Response.Charset = "UTF-8";
            HttpContext.Response.ContentType = "application/pdf";
            HttpContext.Response.AddHeader("Content-Length", bytesRes.Length.ToString());
            HttpContext.Response.OutputStream.Write(bytesRes, 0, bytesRes.Length);
        }
    }
}