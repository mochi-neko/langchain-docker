using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mochineko.Relent.UncertainResult;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mochineko.LangChainDockerUnity.Tests
{
    [TestFixture]
    public class LangChainDockerAPITest
    {
        [Test]
        [RequiresPlayMode(false)]
        public async Task ConversationAsyncTest()
        {
            var httpClient = new HttpClient();
            var content = "こんにちは";

            var result = await LangChainDockerAPI.ConversationAsync(
                httpClient,
                content,
                CancellationToken.None);

            switch (result)
            {
                case IUncertainSuccessResult<AgentResponse> success:
                    Debug.Log($"{success.Result.Result}");
                    break;
                
                case IUncertainRetryableResult<AgentResponse> retryable:
                    Debug.LogError($"Retryable -> {retryable.Message}");
                    break;
                
                case IUncertainFailureResult<AgentResponse> failure:
                    Debug.LogError($"Failure -> {failure.Message}");
                    break;
            }
        }
    }
}