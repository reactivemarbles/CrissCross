// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Imaging;

namespace CrissCross.WPF.UI.Controls
{
    internal static class AnimationCache
    {
        private static readonly Dictionary<CacheKey, AnimationCacheEntry> _animationCache = new Dictionary<CacheKey, AnimationCacheEntry>();
        private static readonly Dictionary<CacheKey, HashSet<Image>> _imageControls = new Dictionary<CacheKey, HashSet<Image>>();

        public static void AddControlForSource(ImageSource source, Image imageControl)
        {
            var cacheKey = new CacheKey(source);
            if (!_imageControls.TryGetValue(cacheKey, out var controls))
            {
                _imageControls[cacheKey] = controls = new HashSet<Image>();
            }

            controls.Add(imageControl);
        }

        public static void RemoveControlForSource(ImageSource source, Image imageControl)
        {
            var cacheKey = new CacheKey(source);
            if (_imageControls.TryGetValue(cacheKey, out var controls) && controls.Remove(imageControl) && controls.Count == 0)
            {
                _animationCache.Remove(cacheKey);
                _imageControls.Remove(cacheKey);
            }
        }

        public static void Add(ImageSource source, AnimationCacheEntry entry)
        {
            var key = new CacheKey(source);
            _animationCache[key] = entry;
        }

        public static void Remove(ImageSource source)
        {
            var key = new CacheKey(source);
            _animationCache.Remove(key);
        }

        public static AnimationCacheEntry? Get(ImageSource source)
        {
            var key = new CacheKey(source);
            _animationCache.TryGetValue(key, out var entry);
            return entry;
        }

        private readonly struct CacheKey(ImageSource source) : IEquatable<CacheKey>
        {
            private readonly ImageSource _source = source;

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (Equals(obj))
                {
                    return true;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((CacheKey)obj);
            }

            public bool Equals(CacheKey other) => ImageEquals(_source, other._source);

            public override int GetHashCode() => ImageGetHashCode(_source);

            private static int ImageGetHashCode(ImageSource image)
            {
                if (image != null)
                {
                    var uri = GetUri(image);
                    if (uri != null)
                    {
                        return uri.GetHashCode();
                    }
                }

                return 0;
            }

            private static bool ImageEquals(ImageSource x, ImageSource y)
            {
                if (Equals(x, y))
                {
                    return true;
                }

                if ((x == null) != (y == null))
                {
                    return false;
                }

                // They can't both be null or Equals would have returned true
                // and if any is null, the previous would have detected it
                // ReSharper disable PossibleNullReferenceException
                if (x?.GetType() != y?.GetType())
                {
                    return false;
                }

                var xUri = GetUri(x);
                var yUri = GetUri(y);
                return xUri != null && xUri == yUri;
            }

            private static Uri? GetUri(ImageSource? image)
            {
                if (image is BitmapImage bmp && bmp.UriSource != null)
                {
                    if (bmp.UriSource.IsAbsoluteUri)
                    {
                        return bmp.UriSource;
                    }

                    if (bmp.BaseUri != null)
                    {
                        return new Uri(bmp.BaseUri, bmp.UriSource);
                    }
                }

                if (image is BitmapFrame frame)
                {
                    var s = frame.ToString();
                    if (s != frame.GetType().FullName && Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var fUri))
                    {
                        if (fUri.IsAbsoluteUri)
                        {
                            return fUri;
                        }

                        if (frame.BaseUri != null)
                        {
                            return new Uri(frame.BaseUri, fUri);
                        }
                    }
                }

                return null;
            }
        }
    }
}
