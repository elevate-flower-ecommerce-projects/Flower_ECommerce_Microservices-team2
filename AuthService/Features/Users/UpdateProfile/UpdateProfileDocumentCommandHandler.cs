using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Features.Users.UpdateProfile;

public sealed class UpdateProfileDocumentCommandHandler(BaseParameters baseParameters)
    : BaseHandler<UpdateProfileDocumentCommand, RequestResult<UserDocumentResponse>>(baseParameters)
{
    public override async Task<RequestResult<UserDocumentResponse>> Handle(
        UpdateProfileDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId <= 0)
        {
            return RequestResult<UserDocumentResponse>.Failure(
                ErrorCode.Unauthorized,
                "Authenticated user was not found.");
        }

        var documentType = request.Document.DocumentType.Trim();
        var existingDocument = await _context.UserDocuments
            .FirstOrDefaultAsync(
                existing => existing.UserId == userId && existing.DocumentType == documentType,
                cancellationToken);

        var document = existingDocument ?? new UserDocument
        {
            Id = _snowflake.CreateId(),
            UserId = userId,
        };

        document.DocumentUrl = request.Document.DocumentUrl.Trim();
        document.DocumentType = documentType;
        document.DocumentSize = request.Document.DocumentSize;

        if (existingDocument is null)
        {
            await _context.UserDocuments.AddAsync(document, cancellationToken);
        }
        else
        {
            _context.UserDocuments.Attach(document);
            _context.Entry(document).State = EntityState.Modified;
        }

        var response = new UserDocumentResponse(
            document.Id,
            document.DocumentUrl,
            document.DocumentType,
            document.DocumentSize);

        return RequestResult<UserDocumentResponse>.Success(response);
    }
}