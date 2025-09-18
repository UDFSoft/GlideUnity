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
using System.Collections;
using Assets.Libs.Glide.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GlideLoader : MonoBehaviour
{
    private static GlideLoader _instance;
    public static GlideLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new("GlideLoader");
                _instance = obj.AddComponent<GlideLoader>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // private readonly Lazy<CertificateHandler> certificateHandler = new(() => NetworkFactory.GetCertificateHandler());

    private Action<Exception> errorCallback;

    private Action successCallback;

    public GlideLoader SetErrorCallback(Action<Exception> callback)
    {
        errorCallback = callback;

        return this;
    }

    public GlideLoader SetSuccessCallback(Action callback)
    {
        successCallback = callback;

        return this;
    }

    public void Load(ImageRequest request, MonoBehaviour context)
    {
        if (request == null || request.Target == null)
            return;

        if (string.IsNullOrEmpty(request.Path) || request.SourceType == ImageSourceType.None)
        {
            if (request.Placeholder != null)
                request.Target.texture = request.Placeholder;
            else if (request.ErrorImage != null)
                request.Target.texture = request.ErrorImage;
            return;
        }

        if (request.Placeholder != null)
            request.Target.texture = request.Placeholder;

        if (ImageCache.TryGet(request.Path, out var cached))
        {
            successCallback?.Invoke();
            request.Target.texture = cached;
            return;
        }

        // context.StartCoroutine(LoadRoutine(request));
        StartCoroutineSafe(context, LoadRoutine(request));
    }

    private IEnumerator LoadRoutine(ImageRequest request)
    {
        Texture2D result = null;

        switch (request.SourceType)
        {
            case ImageSourceType.Url:
                UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(request.Path);
                uwr.certificateHandler = NetworkFactory.GetCertificateHandler();
                foreach (var h in request.Headers)
                    uwr.SetRequestHeader(h.Key, h.Value);
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    successCallback?.Invoke();
                    result = DownloadHandlerTexture.GetContent(uwr);
                }
                else
                {
                    errorCallback?.Invoke(new Exception(uwr.error));
                }
                break;

            case ImageSourceType.File:
                UnityWebRequest fwr = UnityWebRequestTexture.GetTexture("file://" + request.Path);
                yield return fwr.SendWebRequest();
                if (fwr.result == UnityWebRequest.Result.Success)
                    result = DownloadHandlerTexture.GetContent(fwr);
                break;

            case ImageSourceType.Resources:
                result = Resources.Load<Texture2D>(request.Path);
                break;
        }

        if (result != null)
        {
            request.Target.texture = result;
            ImageCache.Store(request.Path, result);
        }
        else if (request.ErrorImage != null)
        {
            request.Target.texture = request.ErrorImage;
        }
    }

    public void Load(ImageRequest request, MonoBehaviour context, Image uiImage)
    {
        if (request == null || uiImage == null)
            return;

        if (string.IsNullOrEmpty(request.Path) || request.SourceType == ImageSourceType.None)
        {
            if (request.Placeholder != null)
                uiImage.sprite = SpriteFromTexture(request.Placeholder);
            else if (request.ErrorImage != null)
                uiImage.sprite = SpriteFromTexture(request.ErrorImage);
            return;
        }

        if (request.Placeholder != null)
            uiImage.sprite = SpriteFromTexture(request.Placeholder);

        if (ImageCache.TryGet(request.Path, out var cached))
        {
            successCallback?.Invoke();
            uiImage.sprite = SpriteFromTexture(cached);
            return;
        }

        StartCoroutineSafe(context, LoadRoutine(request, uiImage));
    }

    private IEnumerator LoadRoutine(ImageRequest request, Image uiImage)
    {
        Texture2D result = null;

        switch (request.SourceType)
        {
            case ImageSourceType.Url:
                UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(request.Path);
                uwr.certificateHandler = NetworkFactory.GetCertificateHandler();
                foreach (var h in request.Headers)
                    uwr.SetRequestHeader(h.Key, h.Value);
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    successCallback?.Invoke();
                    result = DownloadHandlerTexture.GetContent(uwr);
                }
                else
                {
                    errorCallback?.Invoke(new Exception(uwr.error));
                }
                break;

            case ImageSourceType.File:
                UnityWebRequest fwr = UnityWebRequestTexture.GetTexture("file://" + request.Path);
                yield return fwr.SendWebRequest();
                if (fwr.result == UnityWebRequest.Result.Success)
                    result = DownloadHandlerTexture.GetContent(fwr);
                break;

            case ImageSourceType.Resources:
                result = Resources.Load<Texture2D>(request.Path);
                break;
        }

        if (result != null)
        {
            ImageCache.Store(request.Path, result);
            //     uiImage.sprite = SpriteFromTexture(result);
            ImageUtils.SetSpriteSafe(uiImage, SpriteFromTexture(result));
        }
        else if (request.ErrorImage != null)
        {
            //     uiImage.sprite = SpriteFromTexture(request.ErrorImage);
            ImageUtils.SetSpriteSafe(uiImage, SpriteFromTexture(request.ErrorImage));
        }
    }

    private Sprite SpriteFromTexture(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private Coroutine StartCoroutineSafe(MonoBehaviour context, IEnumerator routine)
    {

        if (context != null && context.gameObject.activeInHierarchy)
            return context.StartCoroutine(routine);
        else
            return Instance.StartCoroutine(routine);
    }
}