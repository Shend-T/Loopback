using backend.Auth;
using backend.DTOs.Issues;

namespace backend.Services.Issues;

public interface IIssueService
{
    Task<IReadOnlyList<IssueResponse>> GetAllAsync(CallerContext caller, CancellationToken ct);

    Task<IssueResponse> CreateAsync(
        IssueRequest request,
        CallerContext caller,
        CancellationToken ct
    );
}
