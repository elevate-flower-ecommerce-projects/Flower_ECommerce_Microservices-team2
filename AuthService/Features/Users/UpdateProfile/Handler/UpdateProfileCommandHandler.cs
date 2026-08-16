using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Users.UpdateProfile;

public sealed class UpdateProfileCommandHandler(BaseParameters baseParameters)
    : BaseHandler<UpdateProfileCommand, RequestResult<UserProfileResponse>>(baseParameters)
{
    public override async Task<RequestResult<UserProfileResponse>> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId <= 0)
            return RequestResult<UserProfileResponse>.Failure(
                ErrorCode.Unauthorized,
                "Authenticated user was not found.");

        var user = await _context.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        if (user is null)
            return RequestResult<UserProfileResponse>.Failure(
                ErrorCode.Unauthorized,
                "Authenticated user was not found.");

        if (request.Email is not null)
        {
            var email = request.Email.Trim();
            if (!string.Equals(email, user.Email, StringComparison.OrdinalIgnoreCase) &&
                await _context.Users.AnyAsync(candidate => candidate.Id != userId && candidate.Email == email, cancellationToken))
                return RequestResult<UserProfileResponse>.Failure(
                    ErrorCode.InvalidInput,
                    "email is already used by another account.");

            user.Email = email;
        }

        if (request.Phone is not null)
        {
            var phone = request.Phone.Trim();
            if (!string.Equals(phone, user.PhoneNumber, StringComparison.Ordinal) &&
                await _context.Users.AnyAsync(candidate => candidate.Id != userId && candidate.PhoneNumber == phone, cancellationToken))
                return RequestResult<UserProfileResponse>.Failure(
                    ErrorCode.InvalidInput,
                    "phone is already used by another account.");

            user.PhoneNumber = phone;
        }

        if (request.FullName is not null)
            user.FullName = request.FullName.Trim();
        if (request.Gender is not null)
            user.Gender = request.Gender.Value;

        var existingDocument = await _context.UserDocuments
            .Where(existing => existing.UserId == userId)
            .Select(existing => new UserDocumentResponse(
                existing.Id,
                existing.DocumentUrl,
                existing.DocumentType,
                existing.DocumentSize))
            .FirstOrDefaultAsync(cancellationToken);

        var document = existingDocument;
        if (request.Document is not null)
        {
            var documentResult = await _mediator.Send(
                new UpdateProfileDocumentCommand(request.Document),
                cancellationToken);

            if (!documentResult.IsSuccess)
            {
                return RequestResult<UserProfileResponse>.Failure(
                    documentResult.ErrorCode,
                    documentResult.Message);
            }

            document = documentResult.Data;
        }

        _context.Users.Attach(user);
        _context.Entry(user).State = EntityState.Modified;

        var profile = new UserProfileResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            user.Gender,
            document);

        return RequestResult<UserProfileResponse>.Success(profile);
    }
}