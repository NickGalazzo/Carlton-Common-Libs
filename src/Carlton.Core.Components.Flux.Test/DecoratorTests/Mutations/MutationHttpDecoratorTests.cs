﻿using AutoFixture.AutoMoq;
using Carlton.Core.Components.Flux.Attributes;
using Carlton.Core.Components.Flux.Contracts;
using Carlton.Core.Components.Flux.Decorators.Mutations;
using Carlton.Core.Components.Flux.Models;
using Carlton.Core.Components.Flux.Test.Common;
using Carlton.Core.Components.Flux.Test.Common.Extensions;
using Microsoft.Extensions.Logging;
using MockHttp;

namespace Carlton.Core.Components.Flux.Test.DecoratorTests.Mutations;

public class MutationHttpDecoratorTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMutationCommandDispatcher<TestState>> _decorated;
    private readonly Mock<IMutableFluxState<TestState>> _state;
    private readonly Mock<ILogger<MutationHttpDecorator<TestState>>> _logger;
    private readonly MockHttpHandler mockHttp = new();

    public MutationHttpDecoratorTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _decorated = _fixture.Freeze<Mock<IMutationCommandDispatcher<TestState>>>();
        _state = _fixture.Freeze<Mock<IMutableFluxState<TestState>>>();
        _logger = _fixture.Freeze<Mock<ILogger<MutationHttpDecorator<TestState>>>>();
        _fixture.Register(() => new HttpClient(mockHttp));
        _state.Setup(_ => _.State).Returns(new TestState());
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetCommandData), MemberType = typeof(TestDataGenerator))]
    public async Task Dispatch_DispatchAndHttpRefreshCalled<TCommand>(TCommand command)
        where TCommand : MutationCommand
    {
        //Arrange
        var sender = new HttpRefreshCaller();
        var sut =  _fixture.Create<MutationHttpDecorator<TestState>>();

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, command);

        //Act 
        await sut.Dispatch(sender, command, CancellationToken.None);

        //Assert
         mockHttp.VerifyMockHttpHandler("POST", "http://test.carlton.com/");
        _decorated.VerifyDispatchCalled(command);
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetCommandData), MemberType = typeof(TestDataGenerator))]
    public async Task Dispatch_DispatchAndHttpRefreshWithComponentParametersCalled<TCommand>(TCommand command)
      where TCommand : MutationCommand
    {
        //Arrange
        var sender = new HttpRefreshWithComponentParametersCaller();
        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/clients/5/users/10", 200, command, command);
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        //Act 
        await sut.Dispatch(sender, command, CancellationToken.None);

        //Assert
        mockHttp.VerifyMockHttpHandler("POST", "http://test.carlton.com/clients/5/users/10");
        _decorated.VerifyDispatchCalled(command);
    }

    [Theory]
    [MemberData(nameof(TestDataGenerator.GetCommandData), MemberType = typeof(TestDataGenerator))]
    public async Task Dispatch_DispatchAndHttpRefreshWithStateParametersCalled<TCommand>(TCommand command)
      where TCommand : MutationCommand
    {
        //Arrange
        var sender = new HttpRefreshWithStateParametersCaller();
        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/clients/5/users/10", 200, command, command);
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        //Act 
        await sut.Dispatch(sender, command, CancellationToken.None);

        //Assert
        mockHttp.VerifyMockHttpHandler("POST", "http://test.carlton.com/clients/5/users/10");
        _decorated.VerifyDispatchCalled(command);
    }

    [Fact]
    public async Task Dispatch_UpdateCommandWithExternalResponse_ProcessSkipped()
    {
        //Arrange
        var caller = new HttpRefreshCaller();
        var command = _fixture.Create<TestCommand2>();
        var initialCommand = command with { };
        var serverResponse = _fixture.Create<MockServerResponse>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, serverResponse);

        //Act 
        await sut.Dispatch(caller, command, CancellationToken.None);

        //Assert
        Assert.Equal(initialCommand, command);
    }

    [Fact]
    public async Task Dispatch_UpdateCommandWithExternalResponse_ProcessCompleted()
    {
        //Arrange
        var caller = new HttpRefreshCaller();
        var command = _fixture.Create<TestCommand1>();
        var initialCommand = command with { };
        var serverResponse = _fixture.Create<MockServerResponse>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, serverResponse);

        //Act 
        await sut.Dispatch(caller, command, CancellationToken.None);

        //Assert
        Assert.NotEqual(initialCommand, command);
        Assert.Equal(serverResponse.ServerName, command.Name);
        Assert.Equal(serverResponse.ServerDescription, command.Description);
    }

    [Fact]
    public async Task Dispatch_UpdateCommandWithInvalidJsonParse_ShouldThrowArgumentException()
    {
        //Arrange
        var caller = new HttpRefreshCaller();
        var command = _fixture.Create<TestCommand3>();
        var initialCommand = command with { };
        var serverResponse = _fixture.Create<MockServerResponse>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();
        var expected = "HttpResponseTypeAttribute";

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, serverResponse);

        //Act 
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await sut.Dispatch(caller, command, CancellationToken.None));

        //Assert
        Assert.Equal(expected, ex.ParamName);
    }

    [Fact]
    public async Task Dispatch_UpdateCommandWithInvalidHttpUrl_ShouldThrowArgumentException()
    {
        //Arrange
        var expectedMessage = "The HTTP refresh endpoint is invalid (Parameter 'HttpRefreshAttribute')";
        var caller = new HttpRefreshWithInvalidHttpUrlCaller();
        var command = _fixture.Create<TestCommand3>();
        var initialCommand = command with { };
        var serverResponse = _fixture.Create<MockServerResponse>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, serverResponse);

        //Act 
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await sut.Dispatch(caller, command, CancellationToken.None));

        //Assert
        Assert.Equal(nameof(HttpRefreshAttribute), ex.ParamName);
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task Dispatch_UpdateCommandWithInvalidHttpParameter_ShouldThrowArgumentException()
    {
        //Arrange
        var expectedMessage = "The HTTP refresh endpoint is invalid, following URL parameters were not replaced: {ClientID}, {UserID} (Parameter 'HttpRefreshParameterAttribute')";
        var caller = new HttpRefreshWithInvalidParametersCaller();
        var command = _fixture.Create<TestCommand1>();
        var initialCommand = command with { };
        var serverResponse = _fixture.Create<MockServerResponse>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, serverResponse);

        //Act 
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await sut.Dispatch(caller, command, CancellationToken.None));

        //Assert
        Assert.Equal(nameof(HttpRefreshParameterAttribute), ex.ParamName);
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task Dispatch_UpdateCommandWithInvalidHttpResponseProperty_ShouldThrowArgumentException()
    {
        //Arrange
        var expectedMessage = "An error occured updating the command with the server response (Parameter 'HttpResponsePropertyAttribute')";
        var caller = new HttpRefreshCaller();
        var command = _fixture.Create<TestCommand4>();
        var initialCommand = command with { };
        var serverResponse = _fixture.Create<MockServerResponse>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        mockHttp.SetupMockHttpHandler("POST", "http://test.carlton.com/", 200, command, serverResponse);

        //Act 
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await sut.Dispatch(caller, command, CancellationToken.None));

        //Assert
        Assert.Equal(nameof(HttpResponsePropertyAttribute), ex.ParamName);
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task Dispatch_NoAttribute_HttpRefreshNotCalled()
    {
        //Arrange
        var caller = new NoRefreshCaller();
        var command = _fixture.Create<TestCommand1>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        //Act 
        await sut.Dispatch(caller, command, CancellationToken.None);

        //Assert
        Assert.False(mockHttp.InvokedRequests.Any());
        _decorated.VerifyDispatchCalled(command);
    }

    [Fact]
    public async Task Dispatch_WithNeverAttribute_HttpRefreshNotCalled()
    {
        //Arrange
        var caller = new HttpNeverRefreshCaller();
        var command = _fixture.Create<TestCommand1>();
        var sut = _fixture.Create<MutationHttpDecorator<TestState>>();

        //Act 
        await sut.Dispatch(caller, command, CancellationToken.None);

        //Assert
        Assert.False(mockHttp.InvokedRequests.Any());
        _decorated.VerifyDispatchCalled(command);
    }
}

