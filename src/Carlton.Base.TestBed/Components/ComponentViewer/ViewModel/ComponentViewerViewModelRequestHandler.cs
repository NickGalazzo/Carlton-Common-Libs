﻿namespace Carlton.Base.TestBed;

public sealed class ComponentViewerViewModelRequestHandler : TestBedRequestHandlerViewModelBase<ComponentViewerViewModelRequest, ComponentViewerViewModel>
{

    public ComponentViewerViewModelRequestHandler(TestBedState state)
        : base(state)
    {
    }

    public override Task<ComponentViewerViewModel> Handle(ComponentViewerViewModelRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ComponentViewerViewModel
        (
            State.SelectedComponentType,
            State.SelectedComponentParameters
        ));
    }
}
