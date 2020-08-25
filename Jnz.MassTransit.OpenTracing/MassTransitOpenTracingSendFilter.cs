using GreenPipes;
using MassTransit;
using MassTransit.Transports.InMemory;
using MicroserviceBase.Api.OpenTracing;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jnz.MassTransit.OpenTracing
{
    public class MassTransitOpenTracingSendFilter<T> : IFilter<SendContext<T>> where T : class
    {
        private readonly ITracer tracer;

        public MassTransitOpenTracingSendFilter(ITracer tracer)
        {
            this.tracer = tracer;
        }
        public void Probe(ProbeContext context) { }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            var trace = tracer.ActiveSpan.Context.GetBaggageItems().Where(k => k.Key.Equals("Correlation-Token", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (trace.Key == null)
                return next.Send(context);

            if (Guid.TryParse(trace.Value, out var guid))
                context.CorrelationId = guid;

            var operationName = $"Publishing Message: {context.DestinationAddress.GetQueueOrExchangeName()}";

            var spanBuilder = GlobalTracer.Instance.BuildSpan(operationName)
               .AsChildOf(GlobalTracer.Instance.ActiveSpan.Context)
               .WithTag("destination-address", context.DestinationAddress?.ToString())
               .WithTag("source-address", context.SourceAddress?.ToString())
               .WithTag("initiator-id", context.InitiatorId?.ToString())
               .WithTag("message-id", context.MessageId?.ToString());

            using var scope = spanBuilder.StartActive();
            GlobalTracer.Instance.Inject(
               GlobalTracer.Instance.ActiveSpan.Context,
               BuiltinFormats.TextMap,
               new MassTransitTextMapInjectAdapter(context));

            return next.Send(context);
        }
    }
}
