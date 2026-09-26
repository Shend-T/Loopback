using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Organizations;

public record CreateOrganizationRequest(
    [property: Required] [property: MaxLength(100)] string Name
);
