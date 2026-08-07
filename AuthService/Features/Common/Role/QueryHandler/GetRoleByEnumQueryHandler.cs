using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Data;
using AuthService.Features.Common.Role.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Common.Role.QueryHandler
{
    public class GetRoleByEnumQueryHandler : BaseHandler<GetRoleByEnumQuery, RequestResult<long>>
    {
        public GetRoleByEnumQueryHandler(BaseParameters baseParameters) : base(baseParameters)
        {
        }

        public override async Task<RequestResult<long>> Handle(GetRoleByEnumQuery request, CancellationToken cancellationToken)
        {
            var roleId = await _context.Roles.AsNoTracking()
                .Where(r => r.PersonType == request.PersonType)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (roleId == 0)
                return RequestResult<long>.Failure(ErrorCode.BadRequest, $"Role '{request.PersonType}' not found in the system.");

            return RequestResult<long>.Success(roleId, "Role retrieved successfully.");
        }
    }
}
