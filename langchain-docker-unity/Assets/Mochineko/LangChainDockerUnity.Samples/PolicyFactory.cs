#nullable enable
using System;
using Mochineko.Relent.Resilience;
using Mochineko.Relent.Resilience.Bulkhead;
using Mochineko.Relent.Resilience.Retry;
using Mochineko.Relent.Resilience.Timeout;
using Mochineko.Relent.Resilience.Wrap;

namespace Mochineko.LangChainDockerUnity.Samples
{
    internal static class PolicyFactory
    {
        private const float TotalTimeoutSeconds = 60f;
        private const float EachTimeoutSeconds = 30f;
        private const int MaxRetryCount = 5;
        private const float RetryIntervalSeconds = 1f;
        private const int MaxParallelization = 1;

        public static IPolicy<AgentResponse> BuildPolicy()
        {
            var totalTimeoutPolicy = TimeoutFactory.Timeout<AgentResponse>(
                timeout: TimeSpan.FromSeconds(TotalTimeoutSeconds));

            var retryPolicy = RetryFactory.RetryWithInterval<AgentResponse>(
                MaxRetryCount,
                interval: TimeSpan.FromSeconds(RetryIntervalSeconds));

            var eachTimeoutPolicy = TimeoutFactory.Timeout<AgentResponse>(
                timeout: TimeSpan.FromSeconds(EachTimeoutSeconds));

            var bulkheadPolicy = BulkheadFactory.Bulkhead<AgentResponse>(
                MaxParallelization);

            return totalTimeoutPolicy
                .Wrap(retryPolicy)
                .Wrap(eachTimeoutPolicy)
                .Wrap(bulkheadPolicy);
        }
    }
}