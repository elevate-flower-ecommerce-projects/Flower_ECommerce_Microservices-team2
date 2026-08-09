using AuthService.Common.BaseHandler;
using AuthService.Common.Enums;
using AuthService.Common.ResultPattern;
using AuthService.Entities;
using AuthService.Features.UserManagement.LoginFeature.Queries;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Cryptography;

namespace AuthService.Features.UserManagement.LoginFeature.Command.Handler
{
    public class GenerateRefreshTokenHandler : BaseHandler<GenerateRefreshTokenCommand, RequestResult<string>>
    {
        private readonly ILogger<GenerateRefreshTokenHandler> _logger;

        public GenerateRefreshTokenHandler(BaseParameters baseParameters, ILogger<GenerateRefreshTokenHandler> logger)
            : base(baseParameters)
        {
            _logger = logger;
        }

        public override async Task<RequestResult<string>> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _mediator.Send(new GetActiveRefreshTokenQuery(request.User.Id), cancellationToken);

            if (existingToken != null)
            {
                var refreshToken  = new RefreshToken
                {
                    Id=  existingToken.Id,
                };
               EntityEntry<RefreshToken> entityEntry =  _context.RefreshTokens.Attach(refreshToken);

                existingToken.IsDeleted = true;
                entityEntry.Property(rt => rt.IsDeleted).IsModified = true;     
            }

            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string refreshTokenHash = Convert.ToBase64String(randomNumber);

            var newRefreshToken = new RefreshToken
            {
                Id = _snowflake.CreateId(),
                Token = refreshTokenHash,
                ExpireDate = DateTime.UtcNow.AddDays(7),
                UserId = request.User.Id
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return RequestResult<string>.Success(refreshTokenHash);


        }
    }
}
