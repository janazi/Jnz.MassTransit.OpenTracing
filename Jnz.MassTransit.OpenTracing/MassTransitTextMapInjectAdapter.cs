using MassTransit;
using OpenTracing;
using OpenTracing.Propagation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jnz.MassTransit.OpenTracing
{
    public class MassTransitTextMapInjectAdapter : ITextMap
    {
        private readonly SendContext _context;
        public MassTransitTextMapInjectAdapter(SendContext context)
        {
            _context = context;
        }
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotSupportedException(
                $"{nameof(TextMapInjectAdapter)} should only be used with {nameof(ITracer)}.{nameof(ITracer.Inject)}");
        }

        public void Set(string key, string value)
        {
            _context.Headers.Set(key, value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
