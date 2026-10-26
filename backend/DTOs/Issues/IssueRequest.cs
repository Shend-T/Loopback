using backend.Models;

namespace backend.DTOs.Issues;

public record IssueRequest(
    int OrganizationId,
    string Title,
    string Description,
    IssueStatus Status,
    IssueSeverity Severity,
    int ReportedById
);
