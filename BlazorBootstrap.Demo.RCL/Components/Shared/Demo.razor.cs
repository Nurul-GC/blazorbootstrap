﻿namespace BlazorBootstrap.Demo.RCL;

public partial class Demo : BlazorBootstrapComponentBase
{
    #region Fields and Constants

    private IconColor clipboardTooltipIconColor = IconColor.None;

    private IconName clipboardTooltipIconName = IconName.Clipboard;

    private string? clipboardTooltipTitle = "Copy to clipboard";

    /// <summary>
    /// A reference to this component instance for use in JavaScript calls.
    /// </summary>
    private DotNetObjectReference<Demo> objRef = default!;

    private string? snippet;

    #endregion

    #region Methods

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.InvokeVoidAsync("highlightCode");

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        objRef ??= DotNetObjectReference.Create(this);
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (snippet is null)
        {
            var resourceFullName = Type.FullName + ".razor";

            using (var stream = Type.Assembly.GetManifestResourceStream(resourceFullName)!)
            {
                try
                {
                    if (stream is null)
                        return;

                    using (var reader = new StreamReader(stream))
                    {
                        snippet = await reader.ReadToEndAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Handles a copy error event from JavaScript.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    [JSInvokable]
    public void OnCopyFailJS(string errorMessage)
    {
        // TODO: show message
    }

    /// <summary>
    /// Handles a copy success event from JavaScript.
    /// </summary>
    [JSInvokable]
    public void OnCopySuccessJS()
    {
        clipboardTooltipTitle = "Copied!";
        clipboardTooltipIconName = IconName.Check2;
        clipboardTooltipIconColor = IconColor.Success;

        StateHasChanged();
    }

    /// <summary>
    /// Handles a copy status reset event from JavaScript.
    /// </summary>
    [JSInvokable]
    public void ResetCopyStatusJS()
    {
        clipboardTooltipTitle = "Copy to clipboard";
        clipboardTooltipIconName = IconName.Clipboard;
        clipboardTooltipIconColor = IconColor.None;

        StateHasChanged();
    }

    private async Task CopyToClipboardAsync() => await JS.InvokeVoidAsync("copyToClipboard", snippet, objRef);

    #endregion

    #region Properties, Indexers

    protected override string? ClassNames => BuildClassNames(Class, ("bd-example-snippet bd-code-snippet", true));

    [Inject] protected IJSRuntime JS { get; set; } = default!;

    [Parameter] public LanguageCode LanguageCode { get; set; } = LanguageCode.Razor;

    [Parameter] public bool ShowCodeOnly { get; set; }

    [Parameter] public bool Tabs { get; set; } = false;

    [Parameter] public Type Type { get; set; } = default!;

    #endregion
}
