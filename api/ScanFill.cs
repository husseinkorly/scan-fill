using Azure;
using Azure.Identity;
using Azure.AI.FormRecognizer.DocumentAnalysis;

using System;
using System.Threading.Tasks;

using Aspose.Pdf;
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
            // using managed identity
            string userAssignedClientId = "106104ba-4c3a-407d-a785-d663cea015ac";
            var credential = new ChainedTokenCredential(
                new ManagedIdentityCredential(userAssignedClientId),
                new AzureCliCredential()
            );

            var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
            Document pdfDocument = new Document(files[0].OpenReadStream());
            // creating list of Invoice objects to store the results
            List<Invoice> invoices = new List<Invoice>();

            for (int pageCount = 1; pageCount <= pdfDocument.Pages.Count; pageCount++)
            {
                AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed,
                    "prebuilt-invoice", 
                    files[0].OpenReadStream(),
                    options: new AnalyzeDocumentOptions() { Pages = { $"{pageCount}-{pageCount}" } });
                AnalyzedDocument document = operation.Value.Documents[0];
                // creating invoice payload from the scanned document page
                Invoice invoiceData = CreateInvoicePayload(document);
                invoices.Add(invoiceData);
                // reset the stream position for the next page
                files[0].OpenReadStream().Position = 0;
            }

            var json = JsonConvert.SerializeObject(invoices);
            
            //return json payload with content type
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        private static Invoice CreateInvoicePayload(AnalyzedDocument document)
        {
            Invoice invoice = new()
            {
                Header = new Header(),
                LineItems = new List<LineItem>(),
                Summary = new Summary()
            };
            Address shipFrom = new();
            Address shipTo = new();
           
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
            if (items?.FieldType == DocumentFieldType.List)
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

            return invoice;
        }
    }
}
