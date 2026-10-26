using backend.Models;

namespace backend.DTOs.Issues;

public record IssueResposne(
    int Id,
    int OrganizationId,
    string Title,
    string Description,
    IssueStatus Status,
    IssueSeverity Severity,
    int? ReportedById,
    int? AssignedToId,
    DateTimeOffset CreatedAt,
    DateTimeOffset DueDate
);
