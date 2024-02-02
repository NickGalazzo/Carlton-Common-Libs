﻿namespace Carlton.Core.Flux.Debug.State.Mutations;

internal class EventLogFiltersMutation : FluxStateMutationBase<FluxDebugState, ChangeEventLogLevelFiltersCommand>
{
    public override string StateEvent => FluxDebugStateEvents.EventLogFilterTextChanged.ToString();

    public override FluxDebugState Mutate(FluxDebugState state, ChangeEventLogLevelFiltersCommand command)
    {
        var currentlyIncluded = state.EventLogViewerFilterState.IncludedLogLevels.Contains(command.LogLevel);
        var shouldBeIncluded = command.IsIncluded;
        var filterList = state.EventLogViewerFilterState.IncludedLogLevels.ToList();

        if (shouldBeIncluded && !currentlyIncluded)
            filterList.Add(command.LogLevel);
        else if (currentlyIncluded && !shouldBeIncluded)
            filterList.Remove(command.LogLevel);

        var updatedFilterState = state.EventLogViewerFilterState with { IncludedLogLevels = filterList };

        return state with { EventLogViewerFilterState = updatedFilterState };
    }
}
