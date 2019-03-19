using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using GraphQl.Extensions.Extensions;
using GraphQL;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using MimeMapping;

namespace GraphQl.Extensions.Formatters
{
    public class GraphQlCsvFormatter : OutputFormatter
    {
        private static readonly string DiagnosticListenerName = typeof(GraphQlCsvFormatter).FullName;

        private static readonly DiagnosticListener DiagnosticListener =
            new DiagnosticListener(DiagnosticListenerName);

        private readonly Encoding _encoding;
        private readonly string _entityType;
        private readonly string _delimiter;

        public GraphQlCsvFormatter(string entityType,
            string delimiter,
            Encoding encoding)
        {
            _delimiter = delimiter;
            _encoding = encoding;
            _entityType = entityType;
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(KnownMimeTypes.Csv));
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var result = context.Object as ExecutionResult;
            if (result == null) throw new FormatException();

            var csvExportActivity = new Activity("CsvGeneration");
            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StartActivity(csvExportActivity, new { context });

            var response = context.HttpContext.Response;

            var writer = new StreamWriter(response.Body, _encoding);

            using (var csv = new CsvWriter(writer, new Configuration
            {
                Delimiter = _delimiter
            }))
            {
                foreach (var record in result.GetRecordsStream(_entityType, true))
                {
                    foreach (var field in record)
                    {
                        csv.WriteField(field);
                    }
                    csv.NextRecord();
                }
            }

            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StopActivity(csvExportActivity, null);

            await writer.FlushAsync();
        }

        protected override bool CanWriteType(Type type) => typeof(ExecutionResult) == type;

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