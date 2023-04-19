#nullable enable
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mochineko.Relent.Extensions.NewtonsoftJson;
using Mochineko.Relent.Result;
using Mochineko.Relent.UncertainResult;

namespace Mochineko.LangChainDockerUnity
{
    public static class LangChainDockerAPI
    {
        private const string BaseURL = "http://127.0.0.1:8000";

        public static async UniTask<IUncertainResult<AgentResponse>>
            ConversationAsync(
                HttpClient httpClient,
                string content,
                CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(content))
            {
                return UncertainResults.FailWithTrace<AgentResponse>(
                    "Failed because content is null or empty.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return UncertainResults.RetryWithTrace<AgentResponse>(
                    "Retryable because cancellation has been already requested.");
            }

            var requestBody = new ConversationAgentRequest(content);

            // Serialize request body
            string requestBodyJson;
            var serializationResult = RelentJsonSerializer.Serialize(requestBody);
            switch (serializationResult)
            {
                case ISuccessResult<string> serializationSuccess:
                    requestBodyJson = serializationSuccess.Result;
                    break;

                case IFailureResult<string> serializationFailure:
                    return UncertainResults.FailWithTrace<AgentResponse>(
                        $"Failed because -> {serializationFailure.Message}.");

                default:
                    throw new ResultPatternMatchException(nameof(serializationResult));
            }

            // Request content
            var requestContent = new StringContent(
                content: requestBodyJson,
                encoding: System.Text.Encoding.UTF8,
                mediaType: "application/json");

            // Build request message
            var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                BaseURL + "/agents/conversation");

            requestMessage.Content = requestContent;

            // Send request
            HttpResponseMessage responseMessage;
            var httpResult = await UncertainTryFactory
                .TryAsync<HttpResponseMessage>(async innerCancellationToken
                    => await httpClient.SendAsync(requestMessage, innerCancellationToken))
                .CatchAsRetryable<HttpResponseMessage, HttpRequestException>(exception
                    => $"Retryable because -> {exception}.")
                .CatchAsRetryable<HttpResponseMessage, HttpRequestException>(exception
                    => $"Retryable because -> {exception}.")
                .CatchAsFailure<HttpResponseMessage, Exception>(exception
                    => $"Failure because -> {exception}.")
                .ExecuteAsync(cancellationToken);
            switch (httpResult)
            {
                case IUncertainSuccessResult<HttpResponseMessage> httpSuccess:
                    responseMessage = httpSuccess.Result;
                    break;

                case IUncertainRetryableResult<HttpResponseMessage> httpRetryable:
                    return UncertainResults.RetryWithTrace<AgentResponse>(
                        $"Retryable because -> {httpRetryable.Message}.");

                case IUncertainFailureResult<HttpResponseMessage> httpFailure:
                    return UncertainResults.FailWithTrace<AgentResponse>(
                        $"Failed because -> {httpFailure.Message}.");

                default:
                    throw new ResultPatternMatchException(nameof(httpResult));
            }

            // Succeeded
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseJson))
                {
                    return UncertainResults.FailWithTrace<AgentResponse>(
                        $"Failed because response string was null.");
                }

                // Deserialize response body
                var deserializationResult = RelentJsonSerializer.Deserialize<AgentResponse>(responseJson);
                switch (deserializationResult)
                {
                    case ISuccessResult<AgentResponse> deserializationSuccess:
                        return UncertainResults.Succeed(deserializationSuccess.Result);

                    case IFailureResult<AgentResponse> deserializationFailure:
                        return UncertainResults.FailWithTrace<AgentResponse>(
                            $"Failed because -> {deserializationFailure.Message}.");

                    default:
                        throw new ResultPatternMatchException(nameof(deserializationResult));
                }
            }
            // Retryable
            else if (responseMessage.StatusCode is HttpStatusCode.TooManyRequests
                     || (int)responseMessage.StatusCode is >= 500 and <= 599)
            {
                return UncertainResults.RetryWithTrace<AgentResponse>(
                    $"Retryable because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode}.");
            }
            // Response error
            else
            {
                return UncertainResults.FailWithTrace<AgentResponse>(
                    $"Failed because the API returned status code:({(int)responseMessage.StatusCode}){responseMessage.StatusCode}."
                );
            }
        }
    }
}