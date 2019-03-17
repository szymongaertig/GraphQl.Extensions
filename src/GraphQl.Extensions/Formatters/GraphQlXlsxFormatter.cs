using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GraphQl.Extensions.Extensions;
using GraphQL;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using MimeMapping;
using OfficeOpenXml;

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

            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(KnownMimeTypes.Xls));
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var result = context.Object as ExecutionResult;
            if (result == null) throw new FormatException();

            if (!result.TryExportToDataTable(_entityType, out var dataTable)) throw new FormatException();

            var csvExportActivity = new Activity("XlsxGeneration");
            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StartActivity(csvExportActivity, new { context });

            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(_entityType);
            worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

            var bytes = package.GetAsByteArray();

            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StopActivity(csvExportActivity, null);

            await context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            await package.Stream.FlushAsync();
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