using backend.DTOs;

namespace backend.Services;

public interface IOrganizationService
{
    Task<IReadOnlyList<OrganizationResponse>> GetAllAsync(CancellationToken ct);
    Task<OrganizationResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<OrganizationResponse> CreateAsync(CreateOrganizationRequest request, CancellationToken ct);
}
