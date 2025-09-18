/*
 *    Copyright 2025 UDFOwner
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 *    More details: https://udfsoft.com/
 */
 
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlideRequestBuilder
{
    private readonly MonoBehaviour _context;
    private string _path;
    private ImageSourceType _sourceType = ImageSourceType.Url;
    private Texture2D _placeholder;
    private Texture2D _errorImage;
    private readonly Dictionary<string, string> _headers = new();
    private RawImage _target;

    private Action<Exception> errorCallback;

    private Action successCallback;

    public GlideRequestBuilder(MonoBehaviour context)
    {
        _context = context;
    }

    public GlideRequestBuilder Load(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            _path = null;
            _sourceType = ImageSourceType.None;
            return this;
        }

        _path = path;
        _sourceType = path.StartsWith("http") ? ImageSourceType.Url :
                      path.StartsWith("/") || path.Contains(":\\") ? ImageSourceType.File :
                      ImageSourceType.Resources;
        return this;
    }

    public GlideRequestBuilder Placeholder(Texture2D texture)
    {
        _placeholder = texture;
        return this;
    }

    public GlideRequestBuilder Error(Texture2D texture)
    {
        _errorImage = texture;
        return this;
    }

    public GlideRequestBuilder Header(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    public void Into(RawImage target)
    {
        _target = target;
        var request = BuildRequest();
        GlideLoader.Instance
            .SetErrorCallback(errorCallback)
            .SetSuccessCallback(successCallback)
            .Load(request, _context);
    }

    public void Into(Image target)
    {
        var request = BuildRequest();
        GlideLoader.Instance
            .SetErrorCallback(errorCallback)
            .SetSuccessCallback(successCallback)
            .Load(request, _context, target);
    }

    private ImageRequest BuildRequest()
    {
        var request = new ImageRequest()
            .SetPath(_path)
            .SetSource(_sourceType)
            .SetPlaceholder(_placeholder)
            .SetErrorImage(_errorImage);

        foreach (var kv in _headers)
            request.AddHeader(kv.Key, kv.Value);

        if (_target != null)
            request.SetTarget(_target);

        return request;
    }

    public GlideRequestBuilder SetErrorCallback(Action<Exception> callback)
    {
        errorCallback = callback;

        return this;
    }

    public GlideRequestBuilder SetSuccessCallback(Action callback)
    {
        successCallback = callback;
        return this;
    }

    // public GlideRequestBuilder SetCertificateHandler(CertificateHandler handler)
    // {

    //     return this;
    // }
}
