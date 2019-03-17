using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string _separator;

        public GraphQlCsvFormatter(string entityType,
            string separator,
            Encoding encoding)
        {
            _separator = separator;
            _encoding = encoding;
            _entityType = entityType;
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(KnownMimeTypes.Csv));
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var result = context.Object as ExecutionResult;
            if (result == null) throw new FormatException();

            var csvExportActivity = new Activity("CsvGeneration");
            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StartActivity(csvExportActivity, new { context });

            if (!result.TryExportToCsvContent(_entityType, _separator, out var buffer)) throw new FormatException();

            if (DiagnosticListener.IsEnabled(DiagnosticListenerName))
                DiagnosticListener.StopActivity(csvExportActivity, null);

            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, _encoding))
            {
                return writer.WriteAsync(buffer.ToString());
            }
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