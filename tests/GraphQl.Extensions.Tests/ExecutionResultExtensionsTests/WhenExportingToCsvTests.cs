using System.ComponentModel.DataAnnotations;
using AutoFixture.Xunit2;
using GraphQl.Extensions.Extensions;
using GraphQL;
using Xunit;

namespace GraphQl.Extensions.Tests.ExecutionResultExtensionsTests
{
    public class WhenExportingToCsvTests
    {
        [Theory]
        [AutoData]
        public void ThenGeneratesProperNumberOfRows(string entityType,
            [Range(10, 20)] int numberOfRows)
        {
            // Arrange
            var executionResult = new ExecutionResult
            {
                Data = ExecutionResultMockFactory
                    .GetGraphQlResult(entityType, 5,
                        numberOfRows)
            };

            // Act
            executionResult.TryExportToCsvContent(entityType, "|", out var result);

            // Assert
            Assert.Equal(numberOfRows + 1, result.ToString().Split('\n').Length);
        }
    }
}