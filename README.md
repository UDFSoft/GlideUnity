# GlideUnity

**GlideUnity** is a lightweight and convenient wrapper for loading images in Unity, inspired by [Glide for Android](https://github.com/bumptech/glide). It supports loading from the network, file system, and Resources folder, with memory/disk caching, placeholders, and error handling.

## 🚀 Features

- ✅ Load images from:
  - Network (`http/https`)
  - File system (`file://`)
  - Unity `Resources` folder
- ✅ Supports Unity UI components:
  - `RawImage`
  - `Image` (`Sprite`)
- ✅ Caching:
  - In-memory (RAM) with LRU eviction
  - On disk (`Application.persistentDataPath`)
- ✅ Placeholders and error images
- ✅ Custom HTTP headers
- ✅ Safe handling of `null` and empty paths

---

## 🔧 Installation

Copy the following files into your Unity project:

```
Assets/Scripts/GlideUnity/
├── Glide.cs
├── GlideRequestBuilder.cs
├── GlideLoader.cs
├── ImageRequest.cs
├── ImageSourceType.cs
├── ImageCache.cs
```


> 📁 Make sure to include `using UnityEngine.UI` where needed to use Unity UI components.

---

## 🧪 Example Usage

```csharp
Glide.With(this)
     .Load("https://example.com/avatar.png")
     .Placeholder(myPlaceholderTexture)
     .Error(myErrorTexture)
     .Header("Authorization", "Bearer xyz")
     .Into(myRawImage);
```

Or for Image (Sprite):

```csharp
Glide.With(this)
     .Load("https://example.com/icon.png")
     .Placeholder(spriteTexture)
     .Into(myUIImage);
```

## 📦 Caching
### 🧠 In-Memory
LRU cache (default limit: 5 images)

Manually clear:

```csharp
ImageCache.ClearMemory();
```

### 💾 On-Disk
Stored at Application.persistentDataPath/image_cache

Not automatically cleared (by default)

Manually clear:

```csharp
ImageCache.ClearDisk();
```

### 🔐 Safety
Load(null) and Load("") are safely handled

If the path is invalid or loading fails, the placeholder or errorImage is shown (if provided)
