using Feijuca.Auth.Application.Commands.UserAttributes;
using Feijuca.Auth.Application.Queries.UserAttributes;
using Feijuca.Auth.Application.Requests.UsersAttributes;
using Feijuca.Auth.Attributes;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Feijuca.Auth.Api.Controllers
{
    [Route("api/v1/users-attributes")]
    [ApiController]
    public class UsersAttributesController(ICommandMediator commandMediator, IQueryMediator queryMediator) : ControllerBase
    {
        /// <summary>
        /// Adds a new attribute to a user on the specified Keycloak realm.
        /// </summary>
        /// <param name="username">The username necessary to get attributes from an user.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> that can be used to signal cancellation for the operation.</param>
        /// <returns>
        /// A 200 Created status code if the user is successfully created;
        /// otherwise, a 400 Bad Request status code with an error message.
        /// </returns>
        [HttpGet]
        [EndpointDescription("This endpoint returns the attributes related to the an user.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiReader")]
        public async Task<IActionResult> GetUserAttributes([FromQuery] string username, CancellationToken cancellationToken)
        {
            var result = await queryMediator.QueryAsync(new GetUserAttributeQuery(username), cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Adds a new attribute to a user on the specified Keycloak realm.
        /// </summary>
        /// <param name="username">The username necessary to get an user.</param>
        /// <param name="addUserAttributeRequest">The request object containing the necessary details to add attribute to the user.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> that can be used to signal cancellation for the operation.</param>
        /// <returns>
        /// A 201 Created status code if the user is successfully created;
        /// otherwise, a 400 Bad Request status code with an error message.
        /// </returns>
        [HttpPost]
        [EndpointDescription("This endpoint add new attributes related to the an existing user.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiWriter")]
        public async Task<IActionResult> AddAtribute([FromQuery] string username, AddUserAttributesRequest addUserAttributeRequest,
            CancellationToken cancellationToken)
        {
            var result = await commandMediator.SendAsync(new AddUserAttributeCommand(username, addUserAttributeRequest), cancellationToken);

            if (result.IsSuccess)
            {
                return Created("/", true);
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Adds a new attribute to a user on the specified Keycloak realm.
        /// </summary>
        /// <param name="username">The username necessary to get attributes from an user.</param>
        /// <param name="updateUserAttributeRequest">The request object containing the necessary details to add attribute to the user.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> that can be used to signal cancellation for the operation.</param>
        /// <returns>
        /// A 201 Created status code if the user is successfully created;
        /// otherwise, a 400 Bad Request status code with an error message.
        /// </returns>
        [HttpPatch]
        [EndpointDescription("This endpoint update existing attributes related to the an existing user.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiWriter")]
        public async Task<IActionResult> UpdateUserAttributes([FromQuery] string username, UserAttributeRequest updateUserAttributeRequest, CancellationToken cancellationToken)
        {
            var result = await commandMediator.SendAsync(new UpdateUserAttributesCommand(username, updateUserAttributeRequest), cancellationToken);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.Error);
        }

        /// <summary>
        /// Adds a new attribute to a user on the specified Keycloak realm.
        /// </summary>
        /// <param name="username">The username necessary to get attributes from an user.</param>
        /// <param name="attributeKeys">The request object containing the necessary details to add attribute to the user.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> that can be used to signal cancellation for the operation.</param>
        /// <returns>
        /// A 201 Created status code if the user is successfully created;
        /// otherwise, a 400 Bad Request status code with an error message.
        /// </returns>
        [HttpDelete]
        [EndpointDescription("This endpoint delete existing attributes related to the an existing user.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequiredRole("Feijuca.ApiWriter")]
        public async Task<IActionResult> DeleteUserAttributes([FromQuery] string username, IEnumerable<string> attributeKeys, CancellationToken cancellationToken)
        {
            var result = await commandMediator.SendAsync(new DeleteUserAttributesCommand(username, attributeKeys), cancellationToken);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(result.Error);
        }
    }
}
