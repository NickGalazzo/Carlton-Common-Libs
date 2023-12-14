﻿using MapsterMapper;
namespace Carlton.Core.Components.Flux.State;

public class FluxState<TState> : IMutableFluxState<TState>
{
    private readonly IList<string> _recordedEventStore = new List<string>();
    private readonly IMutationResolver<TState> _mutationResolver;
    private readonly IMapper _mapper;
    private readonly ILogger<FluxState<TState>> _logger;

    public event Func<string, Task> StateChanged;
    public IReadOnlyList<string> RecordedEventStore { get => _recordedEventStore.AsReadOnly(); }
    public TState State { get; private set; }
    private TState RollbackState { get; set; }

    public FluxState(TState state, IMutationResolver<TState> mutationResolver,
        IMapper mapper, ILogger<FluxState<TState>> logger) =>
            (State, _mutationResolver, _mapper, _logger) = (state, mutationResolver, mapper, logger);

    public async Task MutateState<TInput>(TInput input)
    {
        var displayName = typeof(TInput).GetDisplayName();
        try
        {
            _logger.MutationApplyStarted(displayName);

            //Find the correct mutations and save a rollback state
            var mutation = _mutationResolver.Resolve<TInput>();
            _mapper.Map(State, RollbackState);

            //Run the non-destructive mutation to generate a new state from the old, 
            //and replace the old state with the new
            var mutatedState = mutation.Mutate(State, input);
            _mapper.Map(mutatedState, State);

            //Update EventStore 
            _recordedEventStore.Add(mutation.StateEvent);

            //Raise Event if it was a command mutation
            if(!mutation.IsRefreshMutation)
                await InvokeStateChanged(mutation.StateEvent);

            _logger.MutationApplyCompleted(displayName);
        }
        catch(Exception ex)
        {
            _logger.MutationApplyError(ex, displayName);
            _mapper.Map(RollbackState, State);
            throw;
        }
    }

    private async Task InvokeStateChanged(string evt)
    {
        if(StateChanged != null)
        {
            var tasks = new List<Task>();
            foreach(var handler in StateChanged.GetInvocationList())
            {
                tasks.Add(Task.Run(async () =>
                {  
                    var castedDelegate = (Func<string, Task>)handler;
                    await castedDelegate(evt);
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
