using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetUsersByRole;

public sealed record GetUsersByRoleQuery(string Role, int Page = 1, int PageSize = 20) : IRequest<Result<List<ApplicationUserGuidDto>>>;
