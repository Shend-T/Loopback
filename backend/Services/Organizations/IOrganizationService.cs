using backend.DTOs.Organizations;

namespace backend.Services.Organizations;

public interface IOrganizationService
{
    Task<IReadOnlyList<OrganizationResponse>> GetAllAsync(CancellationToken ct);
    Task<OrganizationResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<OrganizationResponse> CreateAsync(CreateOrganizationRequest request, CancellationToken ct);
    Task<OrganizationResponse> UpdateAsync(
        int id,
        UpdateOrganizationRequest request,
        CancellationToken ct
    );
    Task DeleteAsync(int id, CancellationToken ct);
}
