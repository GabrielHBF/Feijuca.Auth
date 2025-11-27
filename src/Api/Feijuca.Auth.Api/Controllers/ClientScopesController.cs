using Feijuca.Auth.Application.Commands.ClientScopes;
using Feijuca.Auth.Application.Queries.ClientScopes;
using Feijuca.Auth.Application.Requests.Client;
using Feijuca.Auth.Application.Requests.ClientScopes;
using Feijuca.Auth.Attributes;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feijuca.Auth.Api.Controllers
{
    [Route("api/v1/clients-scopes")]
    [ApiController]
    [Authorize]
    public class ClientScopesController(ICommandMediator commandMediator, IQueryMediator queryMediator) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiReader")]
        public async Task<IActionResult> GetClientScopes(CancellationToken cancellationToken)
        {
            var result = await queryMediator.QueryAsync(new GetClientScopesQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiWriter")]
        public async Task<IActionResult> AddClientScope([FromBody] AddClientScopesRequest addClientScopesRequest, CancellationToken cancellationToken)
        {
            var result = await commandMediator.SendAsync(new AddClientScopesCommand([addClientScopesRequest]), cancellationToken);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Add a client scope to the client.
        /// </summary>
        /// <returns>
        /// A 200 OK status code along with the list of clients if the operation is successful;
        /// otherwise, a 400 Bad Request status code with an error message, or a 500 Internal Server Error status code if something goes wrong.
        /// </returns>
        /// <param name="addClientScopesRequest">The body containing client and scopes informations. </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> used to observe cancellation requests for the operation.</param>
        [HttpPost("assign-to-client")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiWriter")]
        public async Task<IActionResult> AddClientScopeToClient([FromBody] AddClientScopeToClientRequest addClientScopesRequest, CancellationToken cancellationToken)
        {
            var result = await commandMediator.SendAsync(new AddClientScopeToClientCommand(addClientScopesRequest), cancellationToken);

            if (result.IsSuccess)
            {
                return Created("/", true);
            }

            return BadRequest("Error when tried add client scope to the client. ");
        }
    }
}
