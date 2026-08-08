using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.Exceptions;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Users.UpdateProfile;

public sealed class UpdateProfileCommandHandler(BaseParameters baseParameters)
    : BaseHandler<UpdateProfileCommand, UserProfileResponse>(baseParameters)
{
    public override async Task<UserProfileResponse> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId <= 0)
            throw new BusinessException(ErrorCode.Unauthorized, "Authenticated user was not found.");

        var user = await _context.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        if (user is null)
            throw new BusinessException(ErrorCode.Unauthorized, "Authenticated user was not found.");

        if (request.Email is not null)
        {
            var email = request.Email.Trim();
            if (!string.Equals(email, user.Email, StringComparison.OrdinalIgnoreCase) &&
                await _context.Users.AnyAsync(candidate => candidate.Id != userId && candidate.Email == email, cancellationToken))
                throw new BusinessException(ErrorCode.InvalidInput, "email is already used by another account.");

            user.Email = email;
        }

        if (request.Phone is not null)
        {
            var phone = request.Phone.Trim();
            if (!string.Equals(phone, user.PhoneNumber, StringComparison.Ordinal) &&
                await _context.Users.AnyAsync(candidate => candidate.Id != userId && candidate.PhoneNumber == phone, cancellationToken))
                throw new BusinessException(ErrorCode.InvalidInput, "phone is already used by another account.");

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
            var documentType = request.Document.DocumentType.Trim();

            await _context.UserDocuments
                .Where(existing => existing.UserId == userId && existing.DocumentType == documentType)
                .ExecuteDeleteAsync(cancellationToken);

            var replacement = new UserDocument
            {
                Id = _snowflake.CreateId(),
                UserId = userId,
                DocumentUrl = request.Document.DocumentUrl.Trim(),
                DocumentType = documentType,
                DocumentSize = request.Document.DocumentSize
            };

            await _context.UserDocuments.AddAsync(replacement, cancellationToken);
            document = new UserDocumentResponse(
                replacement.Id,
                replacement.DocumentUrl,
                replacement.DocumentType,
                replacement.DocumentSize);
        }

        _context.Users.Attach(user);
        _context.Entry(user).State = EntityState.Modified;

        return new UserProfileResponse(user.Id, user.FullName, user.Email, user.PhoneNumber, user.Gender, document);
    }
}