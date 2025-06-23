/*
 *    Copyright 2025 UDF Owner
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

    private static string DiskCachePath => Application.persistentDataPath + "/image_cache/";

    static ImageCache()
    {
        if (!System.IO.Directory.Exists(DiskCachePath))
            System.IO.Directory.CreateDirectory(DiskCachePath);
    }

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
        string filePath = DiskCachePath + HashKey(key) + ".png";
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
                    Object.Destroy(toRemove); // Важно!

                _memoryCache.Remove(oldestKey);
            }
        }

        // Сохраняем на диск
        string filePath = DiskCachePath + HashKey(key) + ".png";
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePath, bytes);
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
        if (System.IO.Directory.Exists(DiskCachePath))
            System.IO.Directory.Delete(DiskCachePath, true);
        System.IO.Directory.CreateDirectory(DiskCachePath);
    }
}
