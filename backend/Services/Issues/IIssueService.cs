using backend.Auth;
using backend.DTOs.Issues;

namespace backend.Services.Issues;

public interface IIssueService
{
    Task<IReadOnlyList<IssueResposne>> GetAllAsync(CallerContext caller, CancellationToken ct);
    Task<IReadOnlyList<IssueResposne>> GetAllForOrgAsync(
        CallerContext caller,
        CancellationToken ct
    );

    Task<IssueResposne> CreateAsync(
        IssueRequest request,
        CallerContext caller,
        CancellationToken ct
    );
}
