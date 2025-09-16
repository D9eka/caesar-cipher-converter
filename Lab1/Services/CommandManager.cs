using System;

namespace Lab1.Services;

public static class CommandManager
{
    public static event EventHandler RequerySuggested;
    public static void InvalidateRequerySuggested()
    {
        RequerySuggested?.Invoke(null, EventArgs.Empty);
    }
}