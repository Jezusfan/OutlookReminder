using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OutlookReminder
{
    static class EventProxy
    {

        //void delegates with no parameters
        static public Delegate Create(EventInfo evt, Action d)
        {
            var handlerType = evt.EventHandlerType;
            var eventParams = handlerType.GetMethod("Invoke").GetParameters();

            //lambda: (object x0, EventArgs x1) => d()
            var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "x"));
            var body = Expression.Call(Expression.Constant(d), d.GetType().GetMethod("Invoke"));
            var lambda = Expression.Lambda(body, parameters.ToArray());
            return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
        }

        //void delegate with one parameter
        static public Delegate Create<T>(EventInfo evt, Action<T> handlerAction)
        {
            var handlerType = evt.EventHandlerType;
            var eventParams = handlerType.GetMethod("Invoke").GetParameters();

            //lambda: (object x0, ExampleEventArgs x1) => d(x1.IntArg)
            var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "x")).ToArray();

            var arg = getArgExpression(parameters[0], eventParams[0].Name);
            var body = Expression.Call(Expression.Constant(handlerAction), handlerAction.GetType().GetMethod("Invoke"), arg);
            var lambda = Expression.Lambda(body, parameters);
            return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
        }

        //returns an expression that represents an argument to be passed to the delegate
        static Expression getArgExpression(ParameterExpression eventArgs, string parameterName)
        {
            var memberInfo = eventArgs.Type.GetMember(parameterName)[0];
            return Expression.MakeMemberAccess(eventArgs, memberInfo);
        }
    }
}
