using Mapster;
using WatchStore.Core.Models;
using WatchStore.Core.Requests;
using WatchStore.Core.Responses;

namespace WatchStore.Api.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateClientRequest, Client>()
            .Ignore(dest => dest.Id)    
            .Map(dest => dest.Name, src => src.Name.Trim()); 

        config.NewConfig<Client, ClientResponse>();

        config.NewConfig<CreateWatchRequest, Watch>()
            .Ignore(dest => dest.Id);   

        config.NewConfig<Watch, WatchResponse>();
    }
}