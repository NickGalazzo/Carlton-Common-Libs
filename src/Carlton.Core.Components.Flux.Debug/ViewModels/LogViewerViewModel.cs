﻿using Carlton.Core.Utilities.Logging;

namespace Carlton.Core.Components.Flux.Debug.ViewModels;

public record LogViewerViewModel(IEnumerable<LogMessage> LogMessages);
