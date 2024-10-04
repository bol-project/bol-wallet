using Bol.Core.Abstractions;
using Bol.Core.Services;
using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class BolServiceFactory : IRecipient<WalletClosedMessage>
{
    private IServiceScope _serviceScope;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BolServiceFactory> _logger;

    public BolServiceFactory(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BolServiceFactory> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        WeakReferenceMessenger.Default.Register(this);
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
}
