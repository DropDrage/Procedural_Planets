using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Utils
{
    public class UniTaskUtils
    {
        public async static UniTask RunOnMainThreadFromThreadPool(Action action)
        {
            await UniTask.SwitchToMainThread();
            action();
            await UniTask.SwitchToThreadPool();
        }

        public async static UniTask RunOnMainThreadFromThreadPool<T>(Action<T> action, T input)
        {
            await UniTask.SwitchToMainThread();
            action(input);
            await UniTask.SwitchToThreadPool();
        }

        public static async UniTask<T> RunOnMainThreadFromThreadPool<T>(Func<T> func)
        {
            await UniTask.SwitchToMainThread();
            var result = func();
            await UniTask.SwitchToThreadPool();
            return result;
        }


        public static async UniTask RunOnThreadPool<TInput>(Func<TInput, UniTask> func, TInput input,
            bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    await func(input);
                }
                finally
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            else
            {
                await func(input);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public static async UniTask<T> RunOnThreadPool<T, TInput>(Func<TInput, UniTask<T>> func, TInput input,
            bool configureAwait = true, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            if (configureAwait)
            {
                try
                {
                    return await func(input);
                }
                finally
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.Yield();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            var result = await func(input);
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }
    }
}
