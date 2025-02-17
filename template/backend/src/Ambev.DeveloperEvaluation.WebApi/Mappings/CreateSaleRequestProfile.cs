using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings
{
    public class CreateSaleRequestProfile : Profile
    {
        public CreateSaleRequestProfile()
        {
            CreateMap<CreateSaleRequest, CreateSaleCommand>();
        }
    }
}
