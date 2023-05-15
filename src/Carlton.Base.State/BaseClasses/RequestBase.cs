﻿namespace Carlton.Base.State;

public abstract class RequestBase
{
    public Guid RequestID { get; init; }

    public abstract string RequestName { get; }

    public IDataWrapper Sender { get; init; }

    public Type SenderWrappedType { get => Sender.WrappedComponentType; }

    public object State { get => Sender.State; }

    public bool IsCompleted { get; private set; }

    public bool RequestErrored { get; private set; }

    public DateTime CreatedDateTime { get; }

    public DateTime CompletedDateTime { get; private set; }
    public DateTime ErroredDateTime { get; private set; }
    public bool ServerCalled { get; private set; }

    protected RequestBase(IDataWrapper sender)
    {
        Sender = sender;
        CreatedDateTime = DateTime.UtcNow;
        RequestID = Guid.NewGuid();
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        CompletedDateTime = DateTime.Now;
    }

    public void MarkErrored()
    {
        RequestErrored = true;
        ErroredDateTime = DateTime.Now;
    }

    public void MarkAsServerCalled()
    {
        ServerCalled = true;
    }
}
