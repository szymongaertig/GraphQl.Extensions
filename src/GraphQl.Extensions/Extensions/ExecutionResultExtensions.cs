using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using GraphQL;

namespace GraphQl.Extensions.Extensions
{
    public static class ExecutionResultExtensions
    {
        public static bool ContainsRequiredEntityType(this ExecutionResult executionResult,
            string requestedEntityType)
        {
            return executionResult.Data is Dictionary<string, object> entities
                   && entities.ContainsKey(requestedEntityType);
        }

        public static IEnumerable<string[]> GetRecordsStream(this ExecutionResult executionResult,
            string requestedEntityType,
            bool exportHeaders)
        {
            if (!(executionResult.Data is Dictionary<string, object> entities)
                || !entities.ContainsKey(requestedEntityType))
                throw new FormatException();

            var expectedEntity = entities[requestedEntityType];

            if (!(expectedEntity is List<object> rows)) throw new FormatException();

            var columns = new List<string>();
            bool firstRow = true;
            foreach (var rawRow in rows)
            {
                if (!(rawRow is Dictionary<string, object> rowData)) throw new FormatException();

                if (firstRow)
                {
                    InitColumns(rowData, columns);

                    if (exportHeaders)
                    {
                        yield return columns.ToArray();
                    }
                    firstRow = false;
                }

                var rowValue = new string[columns.Count];

                // we expect, that all rows will contain all columns
                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var columnKey = columns[columnIndex];
                    if (!rowData.ContainsKey(columnKey)) throw new FormatException();

                    var value = rowData[columnKey]?.GetValue().ToString();
                    rowValue[columnIndex] = value;
                }

                yield return rowValue;
            }

        }

        public static bool TryExportToDataTable(this ExecutionResult executionResult,
            string requestedEntityType,
            out DataTable result)
        {
            result = new DataTable(requestedEntityType);

            if (!(executionResult.Data is Dictionary<string, object> entities)
                || !entities.ContainsKey(requestedEntityType))
                return false;

            var expectedEntity = entities[requestedEntityType];

            if (!(expectedEntity is List<object> rows)) return false;

            var columns = new List<string>();

            foreach (var rawRow in rows)
            {
                if (!(rawRow is Dictionary<string, object> rowData)) return false;

                InitColumns(rowData, columns, result);

                var rowValue = new object[columns.Count];
                // we expect, that all rows will contain all columns
                for (var columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    var columnKey = columns[columnIndex];
                    if (!rowData.ContainsKey(columnKey)) return false;

                    var value = rowData[columnKey]?.GetValue().ToString();
                    rowValue[columnIndex] = value;
                }

                result.Rows.Add(rowValue);
            }

            return true;
        }

        private static void InitColumns(Dictionary<string, object> rowData,
            List<string> columns)
        {
            if (columns == null) throw new ArgumentNullException(nameof(columns));
            if (columns.Count > 0) return;

            foreach (var row in rowData)
            {
                columns.Add(row.Key);
            }
        }

        private static void InitColumns(Dictionary<string, object> rowData,
            List<string> columns,
            DataTable table)
        {
            if (columns == null) throw new ArgumentNullException(nameof(columns));
            if (columns.Count > 0) return;

            foreach (var row in rowData)
            {
                columns.Add(row.Key);
                table.Columns.Add(row.Key);
            }
        }

        public static bool TryExportToCsvContent(this ExecutionResult executionResult,
            string requestedEntityType,
            string separator,
            out StringBuilder buffer)
        {
            buffer = new StringBuilder();

            if (!TryExportToDataTable(executionResult, requestedEntityType, out var dataTable)) return false;

            GenerateCsvHeader(dataTable, buffer, separator);
            GenerateCsvContent(dataTable, buffer, separator);
            return true;
        }

        private static void GenerateCsvContent(DataTable dataTable,
            StringBuilder sb,
            string separator)
        {
            for (var rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                sb.Append(dataTable.Rows[rowIndex].ItemArray
                    .Aggregate((p, n) => p + separator + n));

                if (rowIndex < dataTable.Rows.Count - 1) sb.Append(Environment.NewLine);
            }
        }

        private static void GenerateCsvHeader(DataTable dataTable,
            StringBuilder sb,
            string separator)
        {
            for (var columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
            {
                sb.Append(dataTable.Columns[columnIndex].GetValue());
                sb.Append(columnIndex < dataTable.Columns.Count - 1 ? separator : Environment.NewLine);
            }
        }
    }
}