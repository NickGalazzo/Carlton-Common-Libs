﻿using Carlton.Core.Components.Lab.Models.Common;

namespace Carlton.Core.Components.Lab;

public class LabMapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.RequireExplicitMapping = true;
        config.RequireDestinationMemberSource = true;

        config.NewConfig<LabState, LabState>()
            .ConstructUsing(_ => new LabState(_.ComponentStates));

        config.NewConfig<LabState, NavMenuViewModel>()
            .Map(dest => dest.MenuItems, src => src.ComponentStates)
            .Map(dest => dest.SelectedComponentIndex, src => src.SelectedComponentIndex)
            .Map(dest => dest.SelectedStateIndex, src => src.SelectedComponentStateIndex);

        config.NewConfig<LabState, ComponentViewerViewModel>()
            .Map(dest => dest.ComponentType, src => src.SelectedComponentType)
            .Map(dest => dest.ComponentParameters, src => src.SelectedComponentParameters);

        config.NewConfig<LabState, EventConsoleViewModel>()
            .Map(dest => dest.RecordedEvents, src => src.ComponentEvents);

        config.NewConfig<LabState, ParametersViewerViewModel>()
            .Map(dest => dest.ComponentParameters, src => src.SelectedComponentParameters);

        config.NewConfig<LabState, BreadCrumbsViewModel>()
            .Map(dest => dest.SelectedComponent, src => src.SelectedComponentType.GetDisplayName())
            .Map(dest => dest.SelectedComponentState, src => src.SelectedComponentState.DisplayName);

        config.NewConfig<LabState, SourceViewerViewModel>()
            .Map(dest => dest.ComponentSource, src => src.SelectedComponentMarkup);

        config.NewConfig<ComponentRecordedEvent, ComponentRecordedEvent>();

        config.NewConfig<ComponentParameters, ComponentParameters>();

        config.NewConfig<ComponentAvailableStates, ComponentAvailableStates>();

        config.NewConfig<ComponentState, ComponentState>();

        config.NewConfig<TestResult, TestResult>();

        config.Compile();
    }
}

