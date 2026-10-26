using backend.Auth;
using backend.Data;
using backend.DTOs.Issues;

namespace backend.Services.Issues;

public class IssueService : IIssueService
{
    private readonly AppDbContext _db;

    public IssueService(AppDbContext db) => _db = db;

    public async Task<IssueResposne> CreateAsync(
        IssueRequest request,
        CallerContext caller,
        CancellationToken ct
    )
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<IssueResposne>> GetAllAsync(
        CallerContext caller,
        CancellationToken ct
    )
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<IssueResposne>> GetAllForOrgAsync(
        CallerContext caller,
        CancellationToken ct
    )
    {
        throw new NotImplementedException();
    }
}
