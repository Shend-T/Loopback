using backend.Auth;
using backend.Data;
using backend.DTOs.Issues;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Issues;

public class IssueService : IIssueService
{
    private readonly AppDbContext _db;

    public IssueService(AppDbContext db) => _db = db;

    private IQueryable<Issue> VisibleIssues(CallerContext caller) =>
        _db.Issues.Where(i =>
            i.OrganizationId == caller.OrganizationId
            || _db.Organizations.Any(o => o.Id == caller.OrganizationId && o.IsPlatformOrganization)
        );

    public async Task<IReadOnlyList<IssueResponse>> GetAllAsync(
        CallerContext caller,
        CancellationToken ct
    )
    {
        return await VisibleIssues(caller)
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Select(i => new IssueResponse(
                i.Id,
                i.OrganizationId,
                i.Title,
                i.Description,
                i.Status,
                i.Severity,
                i.ReportedById,
                i.AssignedToId,
                i.CreatedAt,
                i.DueDate
            ))
            .ToListAsync(ct);
    }

    public async Task<IssueResponse> CreateAsync(
        IssueRequest request,
        CallerContext caller,
        CancellationToken ct
    )
    {
        if (request.AssignedToId != null)
        {
            if (caller.Role != UserRole.Manager && caller.Role != UserRole.Admin)
                throw new UnauthorizedException("You are not authorized to assign issues");

            var assigneeInOrg = await _db.Users.AnyAsync(
                u => u.Id == request.AssignedToId && u.OrganizationId == caller.OrganizationId,
                ct
            );
            if (!assigneeInOrg)
                throw new NotFoundException("Assignee not found in your organization");
        }

        var issue = new Issue
        {
            OrganizationId = caller.OrganizationId,
            ReportedById = caller.Id,

            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Severity = request.Severity,
            Status = IssueStatus.New,

            AssignedToId = request.AssignedToId,
            // DueDate = request.DueDate,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _db.Issues.Add(issue);
        await _db.SaveChangesAsync(ct);

        return new IssueResponse(
            issue.Id,
            issue.OrganizationId,
            issue.Title,
            issue.Description,
            issue.Status,
            issue.Severity,
            issue.ReportedById,
            issue.AssignedToId,
            issue.CreatedAt,
            issue.DueDate
        );
    }
}
