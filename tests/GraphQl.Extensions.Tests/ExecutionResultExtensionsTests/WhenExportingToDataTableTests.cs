using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using AutoFixture.Xunit2;
using GraphQl.Extensions.Extensions;
using GraphQL;
using Xunit;

namespace GraphQl.Extensions.Tests.ExecutionResultExtensionsTests
{
    public class WhenExportingToDataTableTests
    {
        [Theory]
        [AutoData]
        public void ThenReturnTrue(
            string entityType)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = ExecutionResultMockFactory.GetGraphQlResult(entityType, 5, 5)
            };

            // Act
            var result = executionResult.TryExportToDataTable(entityType, out var table);

            // Assert
            Assert.True(result);
        }

        private void AssertDataTableRows(DataTable dataTable, int numberOfRows, int numberOfColumns)
        {
            for (var rowIndex = 0; rowIndex < numberOfRows; rowIndex++)
            for (var colIndex = 0; colIndex < numberOfColumns; colIndex++)
            {
                var expectedValue = $"Column {colIndex + 1}-Row {rowIndex + 1}";
                var currentValue = dataTable.Rows[rowIndex].ItemArray[colIndex].GetValue();
                Assert.Equal(expectedValue, currentValue);
            }
        }

        [Theory]
        [AutoData]
        public void ThenGeneratesValidRowsValues(
            string entityType,
            [Range(2, 10)] int numberOfRows,
            [Range(10, 20)] int numberOfColumns)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = ExecutionResultMockFactory.GetGraphQlResult(entityType, numberOfColumns, numberOfRows)
            };

            // Act
            executionResult.TryExportToDataTable(entityType, out var dataTable);

            // Assert
            AssertDataTableRows(dataTable, numberOfRows, numberOfColumns);
        }

        [Theory]
        [AutoData]
        public void ThenReturnValidNumberOfRows(
            string entityType,
            [Range(10, 20)] int numberOfRows)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = ExecutionResultMockFactory.GetGraphQlResult(entityType, 5, numberOfRows)
            };

            // Act
            executionResult.TryExportToDataTable(entityType, out var table);

            // Assert
            Assert.Equal(numberOfRows, table.Rows.Count);
        }

        [Theory]
        [AutoData]
        public void ThenReturnValidNumberOfColumns(
            string entityType,
            [Range(10, 20)] int numberOfColumns)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = ExecutionResultMockFactory.GetGraphQlResult(entityType, numberOfColumns, 1)
            };

            // Act
            executionResult.TryExportToDataTable(entityType, out var table);

            // Assert
            Assert.Equal(numberOfColumns, table.Columns.Count);
        }

        [Theory]
        [AutoData]
        public void WithInvalidInputData_ThenReturnFalse(string entityType)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = new Dictionary<string, string>()
            };

            // Act
            var result = executionResult.TryExportToDataTable(entityType, out var table);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [AutoData]
        public void WithNullValues_ThenDoesNotThrows(string entityType)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = ExecutionResultMockFactory.GetGraphQlResult(entityType, 5, 5, true)
            };

            // Act, Assert
            var result = executionResult.TryExportToDataTable(entityType, out var table);

            // Assert
            Assert.True(result);
        }
    }
}