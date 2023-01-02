using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;

namespace ClaraMundi
{
    public class RequestResponse : NetworkBehaviour
    {
        public readonly Dictionary<string, Func<object, object>> Responders = new();
        public readonly Dictionary<string, Func<object, Task<object>>> AsyncResponders = new();
        private event Action<string, object> Responses;

        private void ResolvePromise<T>(TaskCompletionSource<T> promise, object response) =>
            promise.SetResult(response is T r ? r : default);

        public async Task<R> Request<T, R>(string requestName, T request)
        {
            var currentRequestId = StringUtils.UniqueId();
            var promise = new TaskCompletionSource<R>();

            void Cb(string requestId, object response)
            {
                if (currentRequestId != requestId) return;
                ResolvePromise(promise, response);
                Responses -= Cb;
            }

            Responses += Cb;
            OnRequest(requestName, currentRequestId, request);

            return await promise.Task;
        }

        private async void HandleRequest(string responder, string requestId, object request)
        {
            void Res(object result) => Respond(Owner, requestId, result);
            if (AsyncResponders.ContainsKey(responder))
                Res(await AsyncResponders[responder].Invoke(request));
            else if (Responders.ContainsKey(responder))
                Res(Responders[responder].Invoke(request));
        }

        [ServerRpc]
        private void OnRequest(string responder, string requestId, object request) =>
            HandleRequest(responder, requestId, request);

        [TargetRpc]
        private void Respond(NetworkConnection conn, string requestId, object response) =>
            Responses?.Invoke(requestId, response);
    }
}