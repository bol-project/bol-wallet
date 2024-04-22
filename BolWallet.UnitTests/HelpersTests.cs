using BolWallet.Helpers;
using FluentAssertions;

namespace BolWallet.UnitTests;

public class HelpersTests
{
    private readonly ViewModelToViewBinder _sut = new();

    [Fact]
    public void Binder_Should_Correctly_Bind_View_to_ViewModel()
    {
        var viewType = typeof(TestView);
        var viewModelType = typeof(TestViewModel);
        
        _sut.Bind<TestViewModel, TestView>();
        
        _sut.GetBoundViewType(viewModelType).Should().Be(viewType);
    }
}

internal class TestView
{
}

internal class TestViewModel
{
}
