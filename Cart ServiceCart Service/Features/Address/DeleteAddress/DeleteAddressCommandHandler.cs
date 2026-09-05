using Cart_ServiceCart_Service.Common.BaseHandler;
using Cart_ServiceCart_Service.Common.Enums;
using Cart_ServiceCart_Service.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Cart_ServiceCart_Service.Features.Address.DeleteAddress;

/// <summary>
/// Handles <see cref="DeleteAddressCommand"/> by soft-deleting the address,
/// automatically designating a new default address if the deleted address was default,
/// and preserving historical order references via soft-deletion.
/// </summary>
public sealed class DeleteAddressCommandHandler(BaseParameters baseParameters)
    : BaseHandler<DeleteAddressCommand, RequestResult<bool>>(baseParameters)
{
    public override async Task<RequestResult<bool>> Handle(
        DeleteAddressCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            return RequestResult<bool>.Failure(ErrorCode.Unauthorized, "User must be authenticated.");
        }

        var address = await _context.Addresses
            .SingleOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken);

        if (address is null)
        {
            return RequestResult<bool>.Failure(ErrorCode.NotFound, "Address not found.");
        }

        // 1. Soft-delete the address to preserve historical order references
        address.IsDeleted = true;
        address.UpdatedAt = DateTime.UtcNow;
        address.UpdatedBy = request.UserId;

        // 2. If deleting the current default address, designate the next most-recent active address as default
        if (address.IsDefault)
        {
            address.IsDefault = false;

            var nextDefault = await _context.Addresses
                .Where(a => a.UserId == request.UserId && a.Id != address.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
                nextDefault.UpdatedAt = DateTime.UtcNow;
                nextDefault.UpdatedBy = request.UserId;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true, "Address deleted successfully.");
    }
}
