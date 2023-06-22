namespace IconCreator.Encoders;

public class IconEncoder : EncoderBase
{
    protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
    {
        var items = new List<int> { 256, 128, 64, 48, 32, 24, 16 };

        using var icon = new BinaryWriter(stream);

        // 保留
        icon.Write((short)0);

        // 类型
        icon.Write((short)1);

        // 个数类型
        icon.Write((short)items.Count);

        // 图标
        var data = new List<byte>();
        var offset = 6 + 16 * items.Count;

        foreach (var item in items)
        {
            // 图标数据
            var imgData = GetData(image, item);

            // 大小
            icon.Write((byte)item);
            icon.Write((byte)item);

            // 颜色数
            icon.Write((byte)0);

            // 保留
            icon.Write((byte)0);

            // 颜色平面
            icon.Write((short)0);

            // 颜色位数
            icon.Write((short)32);

            // 数据大小
            icon.Write(imgData.Length);

            // 偏移
            icon.Write(offset);

            // 缓存
            data.AddRange(imgData);
            offset += imgData.Length;
        }

        // 数据
        icon.Write(data.ToArray());
    }
}
