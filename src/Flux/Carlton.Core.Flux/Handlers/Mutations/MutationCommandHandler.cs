﻿using Carlton.Core.Flux.Exceptions;

namespace Carlton.Core.Flux.Handlers.Mutations;

public class MutationCommandHandler<TState>(
    IFluxStateObservable<TState> observable,
    IMutationResolver<TState> resolver,
    TState state) 
    : IMutationCommandHandler<TState>
{
    private readonly IFluxStateObservable<TState> _observable = observable;
    private readonly IMutationResolver<TState> _resolver = resolver;
    private readonly TState _state = state;

    public async Task Handle<TCommand>(MutationCommandContext<TCommand> context, CancellationToken cancellationToken)
    {
        try
        {
            //Find the appropriate mutation
            var mutation = _resolver.Resolve(context.MutationCommand.GetType());
            var stateEvent = mutation.StateEvent;

            //Create the new state using the mutation
            var newState = mutation.Mutate(_state, context.MutationCommand);

            //Update the state
            newState.Adapt(_state);
            context.MarkAsStateMutationApplied(stateEvent);

            //Notify of state changed
            var args = new FluxStateChangedEventArgs(stateEvent, context);
            await _observable.OnStateChanged(args);
            context.MarkAsSucceeded();
        }
        catch(CompileException ex)
        {
            context.MarkAsErrored(ex);
            throw MutationCommandFluxException<TState, TCommand>.MutationApplyError(context, ex);
        }
    }
}

