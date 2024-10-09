using Bol.Core.Abstractions;
using Bol.Core.Services;
using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class BolServiceFactory : IRecipient<WalletClosedMessage>, IRecipient<WalletCreatedMessage>
{
    private IServiceScope _serviceScope;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BolServiceFactory> _logger;

    public BolServiceFactory(
        IServiceScopeFactory serviceScopeFactory,
        IMessenger messenger,
        ILogger<BolServiceFactory> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        messenger.Register<WalletClosedMessage>(this);
        messenger.Register<WalletCreatedMessage>(this);
    }

    public IBolService Create()
    {
        _serviceScope ??= _serviceScopeFactory.CreateScope();
        return _serviceScope.ServiceProvider.GetRequiredService<BolService>();
    }

    public void Receive(WalletClosedMessage message)
    {
        _logger.LogInformation("Received {Message}, disposing BOL services...", message.GetType().Name);
        _serviceScope?.Dispose();
        _serviceScope = null;
    }
    
    public void Receive(WalletCreatedMessage message)
    {
        _logger.LogInformation("Received {Message}, disposing BOL services so new instances get access to new wallet...", message.GetType().Name);
        _serviceScope?.Dispose();
        _serviceScope = null;
    }
}
