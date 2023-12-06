using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IconCreator.Encoders;

public abstract class EncoderBase : ImageEncoder
{
    protected static byte[] GetData<TPixel>(Image<TPixel> source, int size) where TPixel : unmanaged, IPixel<TPixel>
    {
        using var clone = source.Clone();
        clone.Mutate(x => x.Resize(size, size));

        using var stream = new MemoryStream();
        clone.SaveAsPng(stream);
        return stream.ToArray();
    }
}
