using System;
using Xunit;

namespace RiderExceptionDebuggingBug
{
    using System.Threading;
    using System.Threading.Tasks;

    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            
            var anyTask = DoSomethingAsync().CancelWithCancellationToken(cancellationToken);
            

            await Task.Delay(1000);

            cancellationTokenSource.Cancel();

            var result = await anyTask;

            async Task<string> DoSomethingAsync()
            {
                await Task.Delay(3000);
                return "done!";
            }
        }
    }
    
    public static class TaskExtensionMethods
    {
        public static async Task<TResult> CancelWithCancellationToken<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());
            var anyTask = await Task.WhenAny(task, taskCompletionSource.Task);
            return await anyTask;
        }
    }
}
