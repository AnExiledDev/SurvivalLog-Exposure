using System;
using Exposure.Core;
using HarmonyLib;

namespace Exposure.Adapters;

/// <summary>Hooking and failure logging shared by the patch classes. A hook that cannot be placed costs its feature, never the mod.</summary>
internal static class Hooks
{
    public static Action<string> Log { get; set; } = _ => { };

    /// <param name="parameters">The target's parameter types, when the name alone could match an overload.</param>
    public static void Patch(Harmony harmony, Type target, string method, Type patches, string? prefix, string? postfix, string consequence, Type[]? parameters = null, string? finalizer = null)
    {
        try
        {
            var original = AccessTools.DeclaredMethod(target, method, parameters)
                           ?? throw new MissingMethodException(target.Name, method);

            harmony.Patch(
                original,
                prefix: prefix is null ? null : new HarmonyMethod(patches, prefix),
                postfix: postfix is null ? null : new HarmonyMethod(patches, postfix),
                finalizer: finalizer is null ? null : new HarmonyMethod(patches, finalizer));
            Log($"hooked {target.Name}.{method}");
        }
        catch (Exception ex)
        {
            Log($"{HookFailure.Prefix} {target.Name}.{method}, {consequence}: {ex.Message}");
        }
    }
}

/// <summary>Runs from per-frame hooks, so a failure is written once instead of every frame.</summary>
internal sealed class FailureLog
{
    private readonly string _message;
    private bool _hasReported;

    public FailureLog(string message) => _message = message;

    public void Report(Exception ex)
    {
        if (_hasReported) return;

        _hasReported = true;
        Hooks.Log($"{_message}: {ex.Message}");
    }
}
