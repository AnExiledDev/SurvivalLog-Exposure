using System;
using Exposure.Core;

namespace Exposure.Adapters;

/// <summary>Shows a notice as an Exiled's Toolkit toast. The caller also writes it to the log.</summary>
internal static class WarningsToast
{
    /// <param name="key">A notice with the same key replaces the one still on screen.</param>
    public static void Show(string title, string? body, bool isWarning, string key, Action<string> log)
    {
        var request = new ExiledToolkit.ToastRequest(WarningText.Source, title)
        {
            Body = body,
            Kind = isWarning ? ExiledToolkit.ToastKind.Warning : ExiledToolkit.ToastKind.Info,
            Key = key,
        };

        if (ExiledToolkit.Toasts.Show(request).IsNone) log("a toast was not shown: " + title);
    }
}
