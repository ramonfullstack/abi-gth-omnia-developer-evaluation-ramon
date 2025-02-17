using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

/// <summary>
/// Profile for mapping between User entity and CreateUserResponse
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateUser operation
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, SaleEntity>();
        CreateMap<SaleEntity, CreateSaleResult>();
    }
}

