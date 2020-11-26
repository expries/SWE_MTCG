using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MTCG.Contracts
{
    public class ActionResult<TError>
    {
        private readonly List<TError> _errors;
        public bool Success => !_errors.Any();

        public ActionResult()
        {
            _errors = new List<TError>();
        }

        public ActionResult(TError error) : this()
        {
            _errors.Add(error);
        }

        public bool CaughtError(TError error)
        {
            return _errors.Contains(error);
        }

        public void Clear()
        {
            _errors.Clear();
        }
    }
}