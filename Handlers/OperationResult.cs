using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    public class OperationResult
    {
        public Exception? Error { get; private set; }
        public bool IsSuccess => Error == null;

        public OperationResult(Exception? error)
        {
            Error = error;
        }

        public static OperationResult Success()
        {
            return new OperationResult(null);
        }

        public static OperationResult Failure(Exception error)
        {
            return new OperationResult(error);
        }
    }
    public class OperationResult<TResult> : OperationResult
    {
        public TResult? Result { get; private set; }

        private OperationResult(TResult? result, Exception? error)
            : base(error)
        {
            Result = result;
        }

        public new static OperationResult<TResult> Success(TResult result)
        {
            return new OperationResult<TResult>(result, null);
        }

        public new static OperationResult<TResult> Failure(Exception error)
        {
            return new OperationResult<TResult>(default, error);
        }
    }
}
