using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicShop.Test.Common
{
    public sealed class FakeTimeProvider : TimeProvider
    {
        private DateTimeOffset utcNow;

        public void SetUtcNow(DateTimeOffset utcNow)
        {
            this.utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => utcNow;

        public override long GetTimestamp() => utcNow.ToUnixTimeMilliseconds();
    }
}