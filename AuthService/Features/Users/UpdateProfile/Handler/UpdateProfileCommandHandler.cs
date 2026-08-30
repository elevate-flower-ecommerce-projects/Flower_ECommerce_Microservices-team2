using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
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
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.DriverProfile)
            .Include(u => u.Documents)
            .FirstOrDefaultAsync(candidate => candidate.Id == userId && !candidate.IsDeleted, cancellationToken);

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

        if (request.PhoneNumber is not null)
        {
            var phone = request.PhoneNumber.Trim();
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

        if (request.PhotoUrl is not null)
        {
            var photoUrl = request.PhotoUrl.Trim();
            var existingPhotoDoc = user.Documents.FirstOrDefault(d => d.DocumentType == "ProfilePhoto");
            if (existingPhotoDoc is not null)
            {
                existingPhotoDoc.DocumentUrl = photoUrl;
                _context.UserDocuments.Update(existingPhotoDoc);
            }
            else
            {
                var newDoc = new UserDocument
                {
                    Id = _snowflake.CreateId(),
                    UserId = userId,
                    DocumentType = "ProfilePhoto",
                    DocumentUrl = photoUrl,
                    DocumentSize = 0,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.UserDocuments.AddAsync(newDoc, cancellationToken);
                user.Documents.Add(newDoc);
            }
        }

        _context.Users.Update(user);

        var primaryRole = user.UserRoles.FirstOrDefault()?.Role?.Name;
        if (string.IsNullOrEmpty(primaryRole))
        {
            primaryRole = user.DriverProfile != null ? "Driver" : "Customer";
        }

        var currentPhotoUrl = user.Documents
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => d.DocumentUrl)
            .FirstOrDefault();

        var profile = new UserProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Gender = user.Gender.ToString(),
            Role = primaryRole,
            PhotoUrl = currentPhotoUrl,
            Status = user.IsBlocked ? "Blocked" : "Active"
        };

        return RequestResult<UserProfileResponse>.Success(profile, "Profile updated successfully.");
    }
}