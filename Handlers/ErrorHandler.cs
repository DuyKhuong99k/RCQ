using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    public static class ErrorHandler
    {
        public static OperationResult Handle(Action action)
        {
            try
            {
                action();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(ex);
            }
        }
        public static OperationResult<TResult> Handle<TResult>(Func<TResult> func)
        {
            try
            {
                var result = func();
                return OperationResult<TResult>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<TResult>.Failure(ex);
            }
        }

        public static async Task<OperationResult> HandleAsync(Func<Task> func)
        {
            try
            {
                await func();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(ex);
            }
        }
        public static async Task<OperationResult<TResult>> HandleAsync<TResult>(Func<Task<TResult>> func)
        {
            try
            {
                var result = await func();
                return OperationResult<TResult>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<TResult>.Failure(ex);
            }
        }

    }
}
