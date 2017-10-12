﻿using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Silphid.Extensions;
using UniRx;
using UnityEngine;

namespace Silphid.Loadzup.Http
{
    public class HttpRequester : IHttpRequester
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HttpRequester));
            
        private static readonly string NewLine = Environment.NewLine;

        private static readonly string[] MeaningfulHeaders =
        {
            KnownHttpHeaders.ContentType,
            KnownHttpHeaders.LastModified,
            KnownHttpHeaders.ETag,
            KnownHttpHeaders.Status
        };

        public IObservable<Response> Request(Uri uri, Options options = null) =>
            ObservableWebRequest
                .Get(uri.AbsoluteUri, options?.Headers)
                .DoOnSubscribe(() => Log.Info($"GET {uri}"))
                .DoOnError(ex => Log.Error($"Failed GET {uri}", ex))
                .Select(www => new Response(www.downloadHandler.data, GetMeaningfulHeaders(www.GetResponseHeaders())));

        public IObservable<Response> Get(Uri uri, Options options = null) =>
            Request(uri, options);

        public IObservable<Response> Post(Uri uri, WWWForm form, Options options = null) =>
            ObservableWebRequest
                .Post(uri.AbsoluteUri, form, options?.Headers)
                .DoOnSubscribe(() =>
                    Log.Info($"POST {uri}{NewLine}Form: {form}{NewLine}Headers: {options?.Headers}"))
                .DoOnError(ex =>
                    Log.Error($"Failed POST {uri}{NewLine}Form: {form}{NewLine}Headers: {options?.Headers}", ex))
                .Select(www => new Response(www.downloadHandler.data, GetMeaningfulHeaders(www.GetResponseHeaders())));

        public IObservable<Response> Put(Uri uri, string body, Options options = null) =>
            ObservableWebRequest
                .Put(uri.AbsoluteUri, body, options?.Headers)
                .DoOnSubscribe(() =>
                    Log.Info($"PUT {uri}{NewLine}Body: {body}{NewLine}Headers: {options?.Headers}"))
                .DoOnError(ex =>
                    Log.Error($"Failed PUT {uri}{NewLine}Body: {body}{NewLine}Headers: {options?.Headers}", ex))
                .Select(www => new Response(www.downloadHandler.data, GetMeaningfulHeaders(www.GetResponseHeaders())));

        private Dictionary<string, string> GetMeaningfulHeaders(IDictionary<string, string> allHeaders) =>
            MeaningfulHeaders
                .Select(x => new KeyValuePair<string, string>(x, allHeaders.GetValueOrDefault(x)))
                .Where(x => x.Value != null)
                .ToDictionary(x => x.Key, x => x.Value);
    }
}