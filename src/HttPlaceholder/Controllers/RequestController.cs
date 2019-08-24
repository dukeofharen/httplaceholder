using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Requests.Commands.CreateStubForRequest;
using HttPlaceholder.Application.Requests.Commands.DeleteAllRequest;
using HttPlaceholder.Application.Requests.Queries.GetAllRequests;
using HttPlaceholder.Application.Requests.Queries.GetByStubId;
using HttPlaceholder.Dto.Requests;
using HttPlaceholder.Dto.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// Controller for request
    /// </summary>
    [Route("ph-api/requests")]
    public class RequestController : BaseApiController
    {
        /// <summary>
        /// Get all Requests.
        /// </summary>
        /// <returns>All request results</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetAll() =>
            Ok(Mapper.Map<IEnumerable<RequestResultDto>>(await Mediator.Send(new GetAllRequestsQuery())));

        /// <summary>
        /// Get requests for the given stub ID.
        /// </summary>
        /// <returns>request results for the given stubId</returns>
        [HttpGet]
        [Route("{StubId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetByStubId([FromRoute]GetByStubIdQuery query) =>
            Ok(Mapper.Map<IEnumerable<RequestResultDto>>(await Mediator.Send(query)));

        /// <summary>
        /// Delete all requests. This call flushes all the requests.
        /// </summary>
        /// <returns>OK, but no content returned</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAll()
        {
            await Mediator.Send(new DeleteAllRequestsCommand());
            return NoContent();
        }

        /// <summary>
        /// An endpoint which accepts the correlation ID of a request made earlier.
        /// HttPlaceholder will create a stub based on this request for you to tweak lateron.
        /// </summary>
        /// <returns>OK, with the generated stub</returns>
        [HttpPost("{CorrelationId}/stubs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<FullStubDto>> CreateStubForRequest(
            [FromRoute] CreateStubForRequestCommand command) =>
            Mapper.Map<FullStubDto>(await Mediator.Send(command));
    }
}
