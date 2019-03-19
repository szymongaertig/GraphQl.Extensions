using System.Threading.Tasks;
using GraphQl.Extensions.Samples.AspNetCore.Model;
using GraphQl.Extensions.Tests.ExecutionResultExtensionsTests;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace GraphQl.Extensions.Samples.AspNetCore.Controllers
{
    public class GraphQlController : Controller
    {
        private readonly ISchema _schema;
        private readonly IDocumentExecuter _documentExecuter;

        public GraphQlController(
            ISchema schema,
            IDocumentExecuter documentExecuter)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
        }

        /// <summary>
        /// Returns mock of executionResult data
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        
        [HttpGet("api/graph-ql/mock/{numberOfRecords}")]
        public async Task<IActionResult> GetMockedData(int numberOfRecords)
        {
            var data = ExecutionResultMockFactory.GetGraphQlResult("products", 20, numberOfRecords);

            return Ok(new ExecutionResult
            {
                Data = data
            });
        }

        [HttpPost("api/graph-ql")]
        public async Task<IActionResult> Post([FromBody] GraphQlQuery query)
        {
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}