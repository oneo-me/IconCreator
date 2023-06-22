using System.Text;

namespace IconCreator.Encoders;

public class IcnsEncoder : EncoderBase
{
    static readonly List<(string osType, int size, string format)> _supportedIconTypes =
    [
        (osType: "is32", size: 16, format: "RGB"),
        (osType: "il32", size: 32, format: "RGB"),
        (osType: "ih32", size: 48, format: "RGB"),
        (osType: "it32", size: 128, format: "RGB"),
        (osType: "s8mk", size: 16, format: "MASK"),
        (osType: "l8mk", size: 32, format: "MASK"),
        (osType: "h8mk", size: 48, format: "MASK"),
        (osType: "t8mk", size: 128, format: "MASK"),
        (osType: "ic04", size: 16, format: "ARGB"),
        (osType: "ic05", size: 32, format: "ARGB"),
        (osType: "icp4", size: 16, format: "PNG"),
        (osType: "icp5", size: 32, format: "PNG"),
        (osType: "icp6", size: 64, format: "PNG"),
        (osType: "ic07", size: 128, format: "PNG"),
        (osType: "ic08", size: 256, format: "PNG"),
        (osType: "ic09", size: 512, format: "PNG"),
        (osType: "ic10", size: 1024, format: "PNG"),
        (osType: "ic11", size: 32, format: "PNG"),
        (osType: "ic12", size: 64, format: "PNG"),
        (osType: "ic13", size: 256, format: "PNG"),
        (osType: "ic14", size: 512, format: "PNG")
    ];

    protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
    {
        using var icon = new BinaryWriter(stream);

        var items = new List<(string osType, byte[]data)>();
        foreach (var (osType, size, format) in _supportedIconTypes)
        {
            if (format != "PNG")
                continue;
            items.Add((osType, GetData(image, size)));
        }

        // 文件头
        icon.Write("icns"u8.ToArray());

        // 文件长度
        icon.Write(BitConverter.GetBytes(4 + 4 + items.Sum(x => 4 + 4 + x.data.Length)).Reverse().ToArray());

        // 图标数据
        foreach (var (osType, data) in items)
        {
            icon.Write(Encoding.ASCII.GetBytes(osType));
            icon.Write(BitConverter.GetBytes(data.Length + 4 + 4).Reverse().ToArray());
            icon.Write(data);
        }
    }
}
