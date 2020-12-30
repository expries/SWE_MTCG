using System;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace MTCG.Results
{
    public class Result
    {
        public Error Error { get; protected init; }
        
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
            return new Result();
        }
        
        public bool HasError<TError>()
        {
            return typeof(TError) == Error.GetType();
        }

        public bool HasError<T1, T2>()
        {
            return HasError<T1>() || HasError<T2>();
        }
        
        public bool HasError<T1, T2, T3>()
        {
            return HasError<T1, T2>() || HasError<T3>();
        }
        
        public bool HasError<T1, T2, T3, T4>()
        {
            return HasError<T1, T2, T3>() || HasError<T4>();
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