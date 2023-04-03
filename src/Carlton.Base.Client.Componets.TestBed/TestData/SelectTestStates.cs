﻿namespace Carlton.Base.Components.TestBed;

internal static class SelectTestStates
{
    public static Dictionary<string, object> Default()
    {
        return new Dictionary<string, object>()
          {
              {"Label", "Test" },
              {"Options",  new Dictionary<string, int>
                    {
                      { "Option 1", 1 },
                      { "Option 2", 2},
                      { "Option 3", 3 }
                    }
              }
          };
    }
}