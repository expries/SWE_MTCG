using System;
using System.Linq;

namespace MTCG.Results
{
    public class Result
    {
        public Error Error { get; protected set; }
        
        public bool Success => Error is null;
        
        public Result()
        {
            Error = null;
        }

        public Result(Error error)
        {
            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            
            Error = error;
        }

        public static Result Ok()
        {
            return new ();
        }
        
        public bool HasError<TError>()
        {
            if (Error is null)
            {
                return false;
            }

            bool isError = typeof(TError) == Error.GetType();
            bool isChildOfError =  Error.GetType().IsSubclassOf(typeof(TError));
            return isError || isChildOfError;
        }

        public static implicit operator Result(Error error)
        {
            return new(error);
        }
    }
    
    public class Result<TValue> : Result
    {
        public TValue Value { get; }

        public Result(TValue value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            Value = value;
            Error = null;
        }

        public Result(Error error) : base(error)
        {
            Value = default;
        }

        public static implicit operator Result<TValue>(TValue value)
        {
            return new(value);
        }

        public static implicit operator Result<TValue>(Error error)
        {
            return new(error);
        }
    }
}