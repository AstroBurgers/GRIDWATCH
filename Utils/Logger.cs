using System;
using System.IO;
using System.Runtime.CompilerServices;
using Rage;

namespace GRIDWATCH.Utils;

internal static class Logger
{
    // Thanks Khori
    internal static void Error(Exception ex, [CallerFilePath] string p = "", [CallerMemberName] string m = "",
        [CallerLineNumber] int l = 0)
    {
        Game.LogTrivial($"[ERROR] GRIDWATCH: Exception at '{Path.GetFileName(p)}' -> {m} (line {l})");
        Game.LogTrivial($"[ERROR] Message: {ex.Message}");
        Game.LogTrivial($"[ERROR] Type: {ex.GetType().FullName}");
        Game.LogTrivial($"[ERROR] Stack Trace: {ex.StackTrace}");

        var inner = ex.InnerException;
        var depth = 1;
        while (inner != null)
        {
            Game.LogTrivial($"[ERROR] Inner Exception (Depth {depth}): {inner.GetType().FullName} - {inner.Message}");
            Game.LogTrivial($"[ERROR] Inner Stack Trace: {inner.StackTrace}");
            inner = inner.InnerException;
            depth++;
        }
    }

    internal static void Warn(string msg)
    {
        Game.LogTrivial($"[WARN] GRIDWATCH: {msg}");
    }

    internal static void Info(string msg)
    {
        Game.LogTrivial($"[INFO] GRIDWATCH: {msg}");
    }

    internal static void Debug(string msg)
    {
        Game.LogTrivial($"[DEBUG] GRIDWATCH: {msg}");
    }

    internal static void Normal(string msg) => Game.LogTrivial($"[NORMAL] GRIDWATCH: {msg}");

}