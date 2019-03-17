using System.Collections.Generic;

namespace GraphQl.Extensions.Tests.ExecutionResultExtensionsTests
{
    public static class ExecutionResultMockFactory
    {
        public static Dictionary<string, object> GetGraphQlResult(
                   string entityType,
                   int numberOfColumns, int numberOfRecords,
                   bool createNullColumns = false)
        {
            var result = new Dictionary<string, object>();
            var entityResult = new List<object>();

            result.Add(entityType, entityResult);
            var columns = new List<string>();
            for (var i = 1; i <= numberOfColumns; i++)
            {
                columns.Add($"Column {i}");
            }

            for (var rowIndex = 1; rowIndex <= numberOfRecords; rowIndex++)
            {
                var row = new Dictionary<string, object>();
                foreach (var column in columns)
                {
                    var columnValue = createNullColumns
                        ? null
                        : $"{column}-Row {rowIndex}";

                    row.Add(column, columnValue);
                }

                entityResult.Add(row);
            }

            return result;
        }
    }
}