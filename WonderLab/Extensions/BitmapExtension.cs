﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using Avalonia.Platform;
using Avalonia.Media.Imaging;
using WonderLab.Views.Controls;

namespace WonderLab.Extensions;
public static class BitmapExtension {
    public static Bitmap ToBitmap(this string uri) {
        var memoryStream = new MemoryStream();
        using var stream = AssetLoader.Open(new Uri(uri));
        stream!.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return new Bitmap(memoryStream);
    }

    public static Bitmap ToBitmap<TPixel>(this Image<TPixel> raw) where TPixel : unmanaged, IPixel<TPixel> {
        using var stream = new MemoryStream();
        raw.Save(stream, new PngEncoder());
        stream.Position = 0;
        return new Bitmap(stream);
    }

    public static IEnumerable<QuantizedColor> GetPaletteFromBitmap(this Image bitmap, int colorCount = 8) {
        Image<Rgba32> image = (Image<Rgba32>)bitmap.Clone(ctx => ctx.Resize(256, 0).Quantize(new OctreeQuantizer(new QuantizerOptions { MaxColors = colorCount + 3 })));
        ConcurrentDictionary<Rgba32, int> dictionary = new();

        Parallel.For(0, image.Width, i => {
            for (int j = 0; j < image.Height; j++) {
                Rgba32 key = image[i, j];
                dictionary.AddOrUpdate(key, 1, (k, v) => v + 1);
            }
        });

        var topColors = dictionary.OrderByDescending(c => c.Value)
            .Where(c => c.Key.A != byte.MaxValue || c.Key.R != byte.MaxValue || c.Key.G != byte.MaxValue || c.Key.B != byte.MaxValue)
            .Take(colorCount)
            .Select(c => new QuantizedColor(new Avalonia.Media.Color(c.Key.A, c.Key.R, c.Key.G, c.Key.B), c.Value))
            .ToList();

        return topColors;
    }
}
