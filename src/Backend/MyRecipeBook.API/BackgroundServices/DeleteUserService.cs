using Azure.Messaging.ServiceBus;
using MyRecipeBook.Application.UseCases.User.Delete.Delete;
using MyRecipeBook.Infraestructure.Services.ServiceBus;

namespace MyRecipeBook.APIBackgroundServices;

public class DeleteUserService : BackgroundService
{
    private readonly IServiceProvider _service;
    private readonly ServiceBusProcessor _processor;

    public DeleteUserService(IServiceProvider service, DeleteUserProcessor processor)
    {
        _service = service;
        _processor = processor.GetBusProcessor();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ExceptionReceivedHandler;

       await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs eventArgs)
    {
        var message = eventArgs.Message.Body.ToString();

        var userIdentifier = Guid.Parse(message);

        var scope = _service.CreateScope();

        var deleteUserUseCase = scope.ServiceProvider.GetRequiredService<IDeleteUserAccountUseCase>();

        await deleteUserUseCase.Execute(userIdentifier);
    }

    private async Task ExceptionReceivedHandler(ProcessErrorEventArgs _)
    {
        await Task.CompletedTask;
    }

    ~DeleteUserService() => Dispose();

    public override void Dispose()
    {
        base.Dispose();

        GC.SuppressFinalize(this);
    }
}