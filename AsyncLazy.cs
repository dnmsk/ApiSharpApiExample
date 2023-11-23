using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ApiSharpApiExample
{

    public class AsyncLazy<T> : LiteLazy<Task<T>> {
        public AsyncLazy(T value) : base(Task.FromResult(value)) { }

        public AsyncLazy(Func<T> valueFactory) : base(() => Task.Run(valueFactory)) { }
        //public AsyncLazy(Task<T> valueFactory) : base(valueFactory) { }

        public AsyncLazy(Func<Task<T>> taskFactory) : base(async () => {
            try {
                return await Task.Factory.StartNew(taskFactory, TaskCreationOptions.PreferFairness).Unwrap().ConfigureAwait(false);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return default;
        }) {}

        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
    }
}
