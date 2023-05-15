﻿namespace Carlton.Base.State;

public class CommandRequest<TStateCommand> : RequestBase, IRequest
    where TStateCommand : ICommand
{
    public TStateCommand Command { get; init; }

    public override string RequestName => $"{typeof(CommandRequest<>).GetDisplayName()}_{nameof(TStateCommand)}";

    public CommandRequest(IDataWrapper sender, TStateCommand command) : base(sender)
        => Command = command;
}




