using System;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public static class AsyncUtils
    {
        public static Task RunAsyncWithScheduler(Action action, TaskScheduler scheduler) =>
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler);

        public static Task<T> RunAsyncWithScheduler<T>(Func<T> func, TaskScheduler scheduler) =>
            Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, scheduler);
    }
}
