﻿namespace Carlton.Base.Components.TestBed;

internal static class PageTitleTestStates
{
    public static Dictionary<string, object> DefaultState()
    {
        return new Dictionary<string, object>
        {
            { "Title", "Test Page"},
            { "BreadCrumbs", "Home > Test" }
        };
    }
}
