using backend.Models;

namespace backend.DTOs.Issues;

public record IssueRequest(
    string Title,
    string Description,
    IssueSeverity Severity,
    int? AssignedToId
);
