﻿namespace Carlton.Base.TestBed;

public abstract class TestBedRequestHandlerViewModelBase<TRequest, TViewModel> : ComponentViewModelRequestHandlerBase<TRequest, TViewModel, TestBedState>
    where TRequest : IViewModelRequest<TViewModel>
{
    protected TestBedRequestHandlerViewModelBase(TestBedState state) : base(state)
    {
    }
}
