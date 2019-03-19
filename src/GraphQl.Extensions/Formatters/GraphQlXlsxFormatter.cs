using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using GraphQl.Extensions.Extensions;
using GraphQL;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using MimeMapping;

namespace GraphQl.Extensions.Formatters
{
    public class GraphQlXlsxFormatter : OutputFormatter
    {
        private static readonly string DiagnosticListenerName = typeof(GraphQlXlsxFormatter).FullName;

        private static readonly DiagnosticListener DiagnosticListener =
            new DiagnosticListener(DiagnosticListenerName);

        private readonly string _entityType;

        public GraphQlXlsxFormatter(string entityType)
        {
            _entityType = entityType;
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(KnownMimeTypes.Xlsx));
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var result = context.Object as ExecutionResult;
            if (result == null) throw new FormatException();

            if (!result.TryExportToDataTable(_entityType, out var dataTable)) throw new FormatException();


            var csvExportActivity = new Activity("XlsxGeneration");
            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StartActivity(csvExportActivity, new { context });

            var response = context.HttpContext.Response;
            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{_entityType}.xlsx\"");

            using (var memoryStream = new MemoryStream())
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(_entityType);
                    var rowIndex = 1;
                    foreach (var record in result.GetRecordsStream(_entityType, true))
                    {
                        var cellIndex = 1;
                        foreach (var field in record)
                        {
                            worksheet.Cell(rowIndex, cellIndex).Value = field;
                            cellIndex++;
                        }
                        rowIndex++;
                    }

                    workbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(response.Body);
                }
            }

            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StopActivity(csvExportActivity, null);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(ExecutionResult) == type;
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (!base.CanWriteResult(context))
            {
                return false;
            }

            return (context.Object as ExecutionResult)?.ContainsRequiredEntityType(_entityType) ?? false;
        }
    }
}