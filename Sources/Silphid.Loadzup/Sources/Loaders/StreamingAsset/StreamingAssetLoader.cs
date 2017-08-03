﻿using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Silphid.Loadzup.StreamingAsset
{
    public class StreamingAssetLoader : ILoader
    {
        private const string PathSeparator = "/";
        private readonly IRequester _requester;
        private readonly IConverter _converter;

        public StreamingAssetLoader(IRequester requester, IConverter converter)
        {
            _requester = requester;
            _converter = converter;
        }

        public bool Supports<T>(Uri uri) => 
            uri.Scheme == Scheme.StreamingAsset;

        public IObservable<T> Load<T>(Uri uri, Options options = null)
        {
            var contentType = options?.ContentType;
            var path = uri.GetPathAndContentType(ref contentType, PathSeparator, true);

            return LoadFile(uri, options, path, contentType)
                .ContinueWith(x => _converter.Convert<T>(x.Bytes, options?.ContentType ?? x.ContentType ?? contentType, x.Encoding));
        }

        private IObservable<Response> LoadFile(Uri uri, Options options, string path, ContentType contentType)
        {
            var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, path);

            if (filePath.Contains("://"))
            {
                options.ContentType = contentType;
                return _requester.Request(uri, options);
            }

            return Observable.Return(System.IO.File.ReadAllBytes(filePath))
                .Select(x => new Response(x, new Dictionary<string, string>()));
        }
    }
}