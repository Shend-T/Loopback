using System.Linq.Expressions;
using backend.Auth;
using backend.Data;
using backend.DTOs.Issues;
using backend.DTOs.Users;
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

    private static TimeSpan DueWindow(IssueSeverity severity) =>
        severity switch
        {
            IssueSeverity.Critical => TimeSpan.FromHours(24),
            IssueSeverity.High => TimeSpan.FromDays(3),
            IssueSeverity.Normal => TimeSpan.FromDays(7),
            IssueSeverity.Low => TimeSpan.FromDays(14),
            _ => throw new ArgumentOutOfRangeException(nameof(severity)),
        };

    private static readonly Expression<Func<Issue, IssueResponse>> ToResponse =
        i => new IssueResponse(
            i.Id,
            i.OrganizationId,
            i.Title,
            i.Description,
            i.Status,
            i.Severity,
            i.ReportedById,
            i.ReportedBy == null
                ? null
                : new UserResponse(
                    i.ReportedBy.Id,
                    i.ReportedBy.OrganizationId,
                    i.ReportedBy.Email,
                    i.ReportedBy.Role
                ),
            i.AssignedToId,
            i.AssignedTo == null
                ? null
                : new UserResponse(
                    i.AssignedTo.Id,
                    i.AssignedTo.OrganizationId,
                    i.AssignedTo.Email,
                    i.AssignedTo.Role
                ),
            i.CreatedAt,
            i.DueDate
        );

    public async Task<IReadOnlyList<IssueResponse>> GetAllAsync(
        CallerContext caller,
        CancellationToken ct
    )
    {
        return await VisibleIssues(caller)
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Select(ToResponse)
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
            DueDate = DateTimeOffset.UtcNow + DueWindow(request.Severity),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _db.Issues.Add(issue);
        await _db.SaveChangesAsync(ct);

        return await VisibleIssues(caller)
            .AsNoTracking()
            .Where(i => i.Id == issue.Id)
            .Select(ToResponse)
            .SingleAsync(ct);
    }
}
