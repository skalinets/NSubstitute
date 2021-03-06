using System;
using System.Collections.Generic;
using NSubstitute.Routing;
using NSubstitute.Routing.Definitions;

namespace NSubstitute.Core
{
    public class CallRouter : ICallRouter
    {
        readonly ISubstitutionContext _context;
        private readonly IReceivedCalls _receivedCalls;
        readonly IResultSetter _resultSetter;
        IRoute _currentRoute;
        IRouteFactory _routeFactory;

        public CallRouter(ISubstitutionContext context, IReceivedCalls receivedCalls, IResultSetter resultSetter, IRouteFactory routeFactory)
        {
            _context = context;
            _receivedCalls = receivedCalls;
            _resultSetter = resultSetter;
            _routeFactory = routeFactory;

            UseDefaultRouteForNextCall();
        }

        public void SetRoute<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition
        {
            _currentRoute = _routeFactory.Create<TRouteDefinition>(routeArguments);
        }

        public void ClearReceivedCalls()
        {
            _receivedCalls.Clear();
        }

        public IEnumerable<ICall> ReceivedCalls()
        {
            return _receivedCalls.AllCalls();
        }

        private void UseDefaultRouteForNextCall()
        {
            SetRoute<RecordReplayRoute>();
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            var routeToUseForThisCall = _currentRoute;
            UseDefaultRouteForNextCall();
            return routeToUseForThisCall.Handle(call);
        }

        public void LastCallShouldReturn(IReturn returnValue, MatchArgs matchArgs)
        {
            _resultSetter.SetResultForLastCall(returnValue, matchArgs);
        }
    }
}