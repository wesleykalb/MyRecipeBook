using Azure.Messaging.ServiceBus;

namespace MyRecipeBook.Infraestructure.Services.ServiceBus;

public class DeleteUserProcessor
{
    private readonly ServiceBusProcessor _serviceBusProcessor;

    public DeleteUserProcessor(ServiceBusProcessor serviceBusProcessor)
    {
        _serviceBusProcessor = serviceBusProcessor;
    }

    public ServiceBusProcessor GetBusProcessor() => _serviceBusProcessor;
}