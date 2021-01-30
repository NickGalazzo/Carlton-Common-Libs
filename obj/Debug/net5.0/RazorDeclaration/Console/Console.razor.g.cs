// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace Carlton.Base.Client.Components
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\nicho\Documents\code\Project Carlton\Carlton.Base.Client.Componnents\_imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
    public partial class Console : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 11 "C:\Users\nicho\Documents\code\Project Carlton\Carlton.Base.Client.Componnents\Console\Console.razor"
      
    [Parameter]
    public string Text { get; set; }
    [Parameter]
    public bool IsReadOnly { get; set; }
    [Parameter]
    public Func<string, Task<bool>> ValidateFunc { get; set; } = (_) => Task.FromResult(true);
    [Parameter]
    public EventCallback<ChangeEventArgs> OnChangeCallback { get; set; }

    private bool IsValid { get; set; }

    protected async override Task OnInitializedAsync()
    {
        IsValid = await ValidateFunc(Text);
        base.OnInitialized();
    }

    private async Task OnViewModelChanged(ChangeEventArgs args)
    {
        IsValid = await ValidateFunc(args.Value.ToString());

        if(IsValid)
            await OnChangeCallback.InvokeAsync(args);
    }

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
