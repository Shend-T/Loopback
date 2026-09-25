using backend.Models;

namespace backend.DTOs;

public record UpdateOrganizationRequest(int Id, string Name) { }
