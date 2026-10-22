using System;
using System.IO;
using System.Runtime.CompilerServices;
using Exposure.Core;

namespace Exposure.Adapters;

/// <summary>
/// Shows a notice through Exiled's Toolkit when it is installed. Without it nothing shows, and the caller's log line
/// is the only trace. Only <see cref="ShowWithToolkit"/> names a Toolkit type, so the mod loads without it.
/// </summary>
internal static class WarningsToast
{
    private static bool _hasReportedFailure;

    /// <param name="key">A notice with the same key replaces the one still on screen.</param>
    public static void Show(string title, string? body, bool isWarning, string key, Action<string> log)
    {
        if (!OptionalLive.IsToolkitInstalled) return;

        try
        {
            ShowWithToolkit(title, body, isWarning, key);
        }
        catch (Exception ex) when (ex is TypeLoadException or MissingMethodException or FileNotFoundException or FileLoadException)
        {
            ReportOnce(log, $"Exiled's Toolkit could not show a notice, so warnings and trip summaries are only logged: {ex.Message}");
        }
        catch (Exception ex)
        {
            ReportOnce(log, $"could not show a notice: {ex.Message}");
        }
    }

    // Kept in its own method so the Toolkit's types are only looked up when it is installed.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ShowWithToolkit(string title, string? body, bool isWarning, string key) =>
        ExiledToolkit.Toasts.Show(new ExiledToolkit.ToastRequest(WarningText.Source, title)
        {
            Body = body,
            Kind = isWarning ? ExiledToolkit.ToastKind.Warning : ExiledToolkit.ToastKind.Info,
            Key = key,
        });

    private static void ReportOnce(Action<string> log, string message)
    {
        if (_hasReportedFailure) return;

        _hasReportedFailure = true;
        log(message);
    }
}
