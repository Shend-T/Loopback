using backend.DTOs.Users;
using backend.Models;

namespace backend.DTOs.Issues;

public record IssueResponse(
    int Id,
    int OrganizationId,
    string Title,
    string Description,
    IssueStatus Status,
    IssueSeverity Severity,
    int? ReportedById,
    UserResponse? ReportedBy,
    int? AssignedToId,
    UserResponse? AssignedTo,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DueDate
);
