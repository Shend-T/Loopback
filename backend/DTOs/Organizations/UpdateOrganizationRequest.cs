using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Organizations;

public record UpdateOrganizationRequest(
    [property: Required] int Id,
    [property: Required] [property: MaxLength(100)] string Name
);
