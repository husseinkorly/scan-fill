using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using api.DTOs;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ScanFill.Function
{
    public static class ScanFill
    {
        [FunctionName("ScanFill")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/scanFill")] HttpRequest req,
            ILogger log)
        {
            // read the form data
            var files = req.Form.Files;
            if (files.Count == 0)
            {
                return new BadRequestObjectResult("No files received");
            }

            string endpoint = "https://scanfill.cognitiveservices.azure.com/";
            string key = "";
            AzureKeyCredential credential = new AzureKeyCredential(key);

            // analyze the form data
            var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", files[0].OpenReadStream());
            // expecting to send a single invoice document for now
            AnalyzedDocument document = operation.Value.Documents[0];

            // return the results
            Invoice invoice = new Invoice
            {
                Header = new Header(),
                LineItems = new List<LineItem>(),
                Summary = new Summary()
            };
            Address shipFrom = new Address();
            Address shipTo = new Address();
           
            if (document.Fields.TryGetValue("InvoiceId", out DocumentField invoiceId))
            {
                invoice.Header.InvoiceId = invoiceId.Value.AsString();
            }
            if (document.Fields.TryGetValue("InvoiceDate", out DocumentField invoiceDate))
            {
                invoice.Header.InvoiceDate = invoiceDate.Value.AsDate();
            }
            if (document.Fields.TryGetValue("PurchaseOrder", out DocumentField purchaseOrder))
            {
                invoice.Header.PurchaseOrder = purchaseOrder.Value.AsString();
            }
            if (document.Fields.TryGetValue("DueDate", out DocumentField dueDate))
            {
                invoice.Header.DueDate = dueDate.Value.AsDate();
            }
            if (document.Fields.TryGetValue("SubTotal", out DocumentField subTotal))
            {
                invoice.Summary.Subtotal = (decimal)subTotal.Value.AsCurrency().Amount;
            }
            if (document.Fields.TryGetValue("TotalTax", out DocumentField totalTax))
            {
                invoice.Summary.TotalTax = (decimal)totalTax.Value.AsCurrency().Amount;
            }
            if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField invoiceTotal))
            {
                invoice.Summary.InvoiceTotal = (decimal)invoiceTotal.Value.AsCurrency().Amount;
            }
            if (document.Fields.TryGetValue("VendorAddress", out DocumentField docShipFrom))
            {
                shipFrom.StreetAddress = docShipFrom?.Value?.AsAddress().StreetAddress;
                shipFrom.City = docShipFrom?.Value?.AsAddress().City;
                shipFrom.PostalCode = docShipFrom?.Value?.AsAddress().PostalCode;
            }
            if (document.Fields.TryGetValue("BillingAddress", out DocumentField DocShipTo))
            {
                shipTo.StreetAddress = DocShipTo?.Value?.AsAddress().StreetAddress;
                shipTo.City = docShipFrom?.Value?.AsAddress().City;
                shipTo.PostalCode = docShipFrom?.Value?.AsAddress().PostalCode;
            }
            


            document.Fields.TryGetValue("Items", out DocumentField items);
            if (items.FieldType == DocumentFieldType.List)
            {
                foreach (var item in items.Value.AsList())
                {
                    var lineItem = new LineItem();
                    var fields = item.Value.AsDictionary();
                    if (fields.TryGetValue("Description", out DocumentField description))
                    {
                        lineItem.Description = description.Value.AsString();
                    }
                    if (fields.TryGetValue("Quantity", out DocumentField quantity))
                    {
                        lineItem.Quantity = (int)quantity.Value.AsDouble();
                    }
                    if (fields.TryGetValue("UnitPrice", out DocumentField unitPrice))
                    {
                        lineItem.UnitPrice = (decimal)unitPrice.Value.AsCurrency().Amount;
                    }
                    lineItem.ShipFrom = shipFrom;
                    lineItem.ShipTo = shipTo;
                    invoice.LineItems.Add(lineItem);
                }
            }
            var json = JsonConvert.SerializeObject(invoice);
            
            //return json payload with content type
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}
