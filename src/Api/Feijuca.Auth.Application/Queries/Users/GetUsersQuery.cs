using Feijuca.Auth.Application.Requests.User;
using Feijuca.Auth.Application.Responses;
using Feijuca.Auth.Http.Responses;
using LiteBus.Queries.Abstractions;
using Mattioli.Configurations.Models;
using MediatR;

namespace Feijuca.Auth.Application.Queries.Users
{
    public record GetUsersQuery(GetUsersRequest GetUsersRequest) : IQuery<Result<PagedResult<UserResponse>>>;
}
