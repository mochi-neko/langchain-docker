#nullable enable
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mochineko.Relent.Resilience;
using Mochineko.Relent.UncertainResult;
using UnityEngine;

namespace Mochineko.LangChainDockerUnity.Samples
{
    public class LangChainDockerAPIDemo : MonoBehaviour
    {
        [SerializeField, TextArea] private string message = string.Empty;

        private static readonly HttpClient HttpClient = new HttpClient();

        private IPolicy<AgentResponse>? policy;

        private void Start()
        {
            policy = PolicyFactory.BuildPolicy();
        }

        [ContextMenu(nameof(Conversation))]
        public void Conversation()
        {
            ConversationAsync(this.GetCancellationTokenOnDestroy())
                .Forget();
        }

        private async UniTask ConversationAsync(CancellationToken cancellationToken)
        {
            if (policy == null)
            {
                Debug.LogError("Policy is null.");
                return;
            }

            await UniTask.SwitchToThreadPool();

            var result = await policy.ExecuteAsync(async innerCancellationToken =>
                    await LangChainDockerAPI.ConversationAsync(
                        HttpClient,
                        message,
                        innerCancellationToken),
                cancellationToken);

            await UniTask.SwitchToMainThread(cancellationToken);
            
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