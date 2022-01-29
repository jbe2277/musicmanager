﻿using Windows.Foundation;
using Waf.MusicManager.Applications;

namespace Waf.MusicManager.Presentation;

public static class AsyncOperationExtensions
{
    public static void Wait<T>(this IAsyncOperation<T> asyncOperation) => GetResult(asyncOperation, CancellationToken.None);

    public static TResult GetResult<TResult>(this IAsyncOperation<TResult> asyncOperation) => GetResult(asyncOperation, CancellationToken.None);

    public static TResult GetResult<TResult>(this IAsyncOperation<TResult> asyncOperation, CancellationToken cancellationToken) => TaskUtility.GetResult(asyncOperation.AsTask(cancellationToken));
}
