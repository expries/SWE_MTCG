using System.Collections.Generic;
using System.Linq;

namespace MTCG.Mapper
{
    public abstract class Mapper<TSource, TTarget>
    {
        public abstract TTarget Map(TSource obj);

        public IEnumerable<TTarget> Map(IEnumerable<TSource> objList)
        {
            return objList.Select(Map);
        }
    }
}