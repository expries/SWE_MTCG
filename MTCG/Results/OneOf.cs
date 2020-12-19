using System;
using System.Linq.Expressions;

namespace MTCG.ActionResult
{
    public class OneOf<T1, T2>
    {
        protected T1 Value1 { get; set; }
        
        protected T2 Value2 { get; set; }
        
        protected OneOf()
        {
        }

        private OneOf(T1 value)
        {
            Value1 = value;
        }

        private OneOf(T2 value)
        {
            Value2 = value;
        }

        public T1 GetT1()
        {
            return Value1;
        }
        
        public T2 GetT2()
        {
            return Value2;
        }

        public bool IsT1()
        {
            return Value1 != null;
        }

        public bool IsT2()
        {
            return Value2 != null;
        }
        
        public static implicit operator OneOf<T1, T2>(T1 other)
        {
            return new OneOf<T1, T2>(other);
        }

        public static implicit operator OneOf<T1, T2>(T2 other)
        {
            return new OneOf<T1, T2>(other);
        }

        public TResult Match<TResult>(Func<T1, TResult> handler1, Func<T2, TResult> handler2)
        {
            if (Value1 != null)
            {
                return handler1.Invoke(Value1);
            }
            
            if (Value2 != null)
            {
                return handler2.Invoke(Value2);
            }

            return default;
        }
    }

    public class OneOf<T1, T2, T3> : OneOf<T1, T2>
    {
        protected T3 Value3 { get; set; }
        
        protected OneOf()
        {
        }
        
        private OneOf(T1 value)
        {
            Value1 = value;
        }
        
        private OneOf(T2 value)
        {
            Value2 = value;
        }
        
        private OneOf(T3 value)
        {
            Value3 = value;
        }
        
        public T3 GetT3()
        {
            return Value3;
        }
        
        public bool IsT3()
        {
            return Value3 != null;
        }

        public static implicit operator OneOf<T1, T2, T3>(T1 other)
        {
            return new OneOf<T1, T2, T3>(other);
        }

        public static implicit operator OneOf<T1, T2, T3>(T2 other)
        {
            return new OneOf<T1, T2, T3>(other);
        }
        
        public static implicit operator OneOf<T1, T2, T3>(T3 other)
        {
            return new OneOf<T1, T2, T3>(other);
        }
        
        public TResult Match<TResult>(
            Func<T1, TResult> handler1, 
            Func<T2, TResult> handler2, 
            Func<T3, TResult> handler3)
        {
            var result = Match(handler1, handler2);
            return result != null ? result : handler3.Invoke(Value3);
        }
    }
    
    public class OneOf<T1, T2, T3, T4> : OneOf<T1, T2, T3>
    {
        private T4 Value4 { get; }
        
        protected OneOf()
        {
        }
        
        public OneOf(T1 value)
        {
            Value1 = value;
        }
        
        public OneOf(T2 value)
        {
            Value2 = value;
        }
        
        public OneOf(T3 value)
        {
            Value3 = value;
        }
        
        public OneOf(T4 value)
        {
            Value4 = value;
        }
        
        public T4 GetT4()
        {
            return Value4;
        }
        
        public bool IsT4()
        {
            return Value4 != null;
        }
        
        public static implicit operator OneOf<T1, T2, T3, T4>(T1 other)
        {
            return new OneOf<T1, T2, T3, T4>(other);
        }
        
        public static implicit operator OneOf<T1, T2, T3, T4>(T2 other)
        {
            return new OneOf<T1, T2, T3, T4>(other);
        }
        
        public static implicit operator OneOf<T1, T2, T3, T4>(T3 other)
        {
            return new OneOf<T1, T2, T3, T4>(other);
        }
        
        public static implicit operator OneOf<T1, T2, T3, T4>(T4 other)
        {
            return new OneOf<T1, T2, T3, T4>(other);
        }
        
        public TResult Match<TResult>(
            Func<T1, TResult> handler1, 
            Func<T2, TResult> handler2, 
            Func<T3, TResult> handler3, 
            Func<T4, TResult> handler4)
        {
            var result = Match(handler1, handler2, handler3);
            return result != null ? result : handler4.Invoke(Value4);
        }
    }
}