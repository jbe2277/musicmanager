namespace Waf.MusicManager.Applications;

public static class TaskUtility
{
    public static TResult GetResult<TResult>(this Task<TResult> task)
    {
        // This ensures the same exception behavior as it would be with 'await'. No AggregateException - just the first one that occurred.
        return task.GetAwaiter().GetResult();
    }

    // Similar as Task.WhenAll but the task completes after the first one throws an exception (does not wait for all other tasks to complete).
    public static Task WhenAllFast(ReadOnlySpan<Task> tasks)
    {
        if (tasks.IsEmpty) return Task.CompletedTask;

        var taskCompletionSource = new TaskCompletionSource();
        int count = tasks.Length;

        foreach (var task in tasks)
        {
            task.ContinueWith(t =>
            {
                if (taskCompletionSource.Task.IsCompleted)
                {
                    ObserveException(t.Exception);
                    return;
                }

                if (t.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else if (t.IsFaulted)
                {
                    taskCompletionSource.TrySetException(t.Exception!.Flatten().InnerExceptions);
                }
                else
                {
                    // Decrement the count and continue if this was the last task.
                    if (Interlocked.Decrement(ref count) == 0) taskCompletionSource.SetResult();
                }

            }, TaskContinuationOptions.ExecuteSynchronously);
        }
        return taskCompletionSource.Task;

        static void ObserveException(Exception? ex) { /* Nothing to do. */ }
    }
}
