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
using UnityEngine.UI;

public class ImageRequest
{
    public string Path;
    public RawImage Target;
    public ImageSourceType SourceType = ImageSourceType.Url;
    public Texture2D Placeholder;
    public Texture2D ErrorImage;
    public Dictionary<string, string> Headers = new();

    public ImageRequest SetPath(string path) { Path = path; return this; }
    public ImageRequest SetTarget(RawImage target) { Target = target; return this; }
    public ImageRequest SetSource(ImageSourceType type) { SourceType = type; return this; }
    public ImageRequest SetPlaceholder(Texture2D texture) { Placeholder = texture; return this; }
    public ImageRequest SetErrorImage(Texture2D texture) { ErrorImage = texture; return this; }
    public ImageRequest AddHeader(string key, string value) { Headers[key] = value; return this; }
}
