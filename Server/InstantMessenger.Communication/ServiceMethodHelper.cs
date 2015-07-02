using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InstantMessenger.Communication
{
    public static class ServiceMethodHelper<TService>
    {
        public static MethodInfo GetMethodInfo(Expression<Func<TService, Func<TransportObject, TransportObject>>> method)
        {
            const string message = @"Chybná metoda v ServiceMethod.GetName";

            var unary = (method.Body as UnaryExpression);
            if (unary == null)
                throw new ApplicationException(message);

            var methodCall = (unary.Operand as MethodCallExpression);
            if (methodCall == null || methodCall.Arguments.Count != 3)
                throw new ApplicationException(message);

            var methodConstant = methodCall.Arguments[2] as ConstantExpression; // volaná metoda bude jako 3. argument
            if (methodConstant == null)
                throw new ApplicationException(message);

            if (methodCall.Arguments[1].Type != typeof(TService)) // kontrola, jestli je metoda na správném interface
                throw new ApplicationException(message);

            var methodInfo = (methodConstant.Value as MethodInfo);
            if (methodInfo == null)
                throw new ApplicationException(message);

            return methodInfo;
        }
    }
}
