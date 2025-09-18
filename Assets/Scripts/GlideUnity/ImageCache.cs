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
 
using System.Collections.Generic;
using UnityEngine;

public static class ImageCache
{

    private static int _memoryLimit = 5;

    private static readonly Dictionary<string, Texture2D> _memoryCache = new();
    private static readonly LinkedList<string> _lruList = new();

    private static string _DiskCachePath => Application.persistentDataPath + "/image_cache/";

    // static ImageCache()
    // {
    //     if (!System.IO.Directory.Exists(DiskCachePath))
    //         System.IO.Directory.CreateDirectory(DiskCachePath);
    // }

    public static bool TryGet(string key, out Texture2D tex)
    {
        if (_memoryCache.TryGetValue(key, out tex))
        {
            // Поднимаем в начало — последнее использование
            _lruList.Remove(key);
            _lruList.AddFirst(key);
            return true;
        }

        // Попробуем с диска
        string filePath = GetDiskCachePath() + HashKey(key) + ".png";
        if (System.IO.File.Exists(filePath))
        {
            byte[] data = System.IO.File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            if (tex.LoadImage(data))
            {
                Store(key, tex); // Добавим в память
                return true;
            }
        }

        tex = null;
        return false;
    }

    public static void Store(string key, Texture2D tex)
    {
        // Если уже есть — обновляем и поднимаем в начало
        if (_memoryCache.ContainsKey(key))
        {
            _memoryCache[key] = tex;
            _lruList.Remove(key);
            _lruList.AddFirst(key);
        }
        else
        {
            // Добавляем
            _memoryCache[key] = tex;
            _lruList.AddFirst(key);

            // Удаляем самый старый элемент, если превышен лимит
            if (_memoryCache.Count > _memoryLimit)
            {
                string oldestKey = _lruList.Last.Value;
                _lruList.RemoveLast();

                if (_memoryCache.TryGetValue(oldestKey, out Texture2D toRemove))
                {
                    Debug.Log("Remove old texture: " + oldestKey);

                    if (oldestKey.StartsWith("http") || oldestKey.StartsWith("https") ||
                    oldestKey.StartsWith("/") || oldestKey.Contains(":\\"))
                    {
                        // remove only file or web resource
                        Object.Destroy(toRemove);
                    }
                }

                _memoryCache.Remove(oldestKey);
            }
        }

        // Сохраняем на диск
        string filePath = GetDiskCachePath() + HashKey(key) + ".png";

        if (!tex.isReadable)
            tex = MakeReadableCopy(tex);

        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePath, bytes);
    }

    private static Texture2D MakeReadableCopy(Texture2D tex)
    {
        RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(tex, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTex = new(tex.width, tex.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTex;
    }

    private static string HashKey(string input)
    {
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        byte[] hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public static void ClearMemory()
    {
        foreach (var tex in _memoryCache.Values)
            Object.Destroy(tex);
        _memoryCache.Clear();
        _lruList.Clear();
    }

    public static void ClearDisk()
    {
        if (System.IO.Directory.Exists(_DiskCachePath))
            System.IO.Directory.Delete(_DiskCachePath, true);
        System.IO.Directory.CreateDirectory(_DiskCachePath);
    }

    private static string GetDiskCachePath()
    {
        if (!System.IO.Directory.Exists(_DiskCachePath))
            System.IO.Directory.CreateDirectory(_DiskCachePath);
        return _DiskCachePath;
    }
}