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
 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Libs.Glide.Scripts
{
    public class ImageUtils
    {

        public static void SetSpriteSafe(Image image, Sprite sprite)
        {
            if (image != null)
                image.sprite = sprite;
        }
    }
}