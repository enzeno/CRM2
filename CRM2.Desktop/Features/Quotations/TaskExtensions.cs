using System;
using System.Threading.Tasks;

namespace CRM2.Desktop.Features.Quotations;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        if (task.IsCompleted)
        {
            if (task.IsFaulted && task.Exception != null)
            {
                Console.WriteLine($"Unhandled async error: {task.Exception.GetBaseException().Message}");
            }
            return;
        }

        task.ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                Console.WriteLine($"Unhandled async error: {t.Exception.GetBaseException().Message}");
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
} 