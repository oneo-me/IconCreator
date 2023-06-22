using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using OpenView.Controls;
using SkiaSharp;
using Point = Avalonia.Point;

namespace IconCreator.Views;

public partial class Main : View
{
    public Main()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.Custom(new CustomDrawOp((float)Bounds.Width, (float)Bounds.Height));
    }

    class CustomDrawOp(float width, float height) : ICustomDrawOperation
    {
        const int Size = 14;
        static readonly SKShader _shader = SKShader.CreateColor(new(0, 0, 0, (byte)(2.55 * 6)));

        public void Dispose()
        {
        }

        public bool HitTest(Point p)
        {
            return true;
        }

        public Rect Bounds { get; } = new(0, 0, width, height);

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null) return;

            using var skia = leaseFeature.Lease();

            using var skPath = new SKPath();

            skPath.MoveTo(0, 0);
            skPath.LineTo(Size, 0);
            skPath.LineTo(Size, Size);
            skPath.LineTo(0, Size);
            skPath.Close();

            skPath.MoveTo(Size, Size);
            skPath.LineTo(Size * 2, Size);
            skPath.LineTo(Size * 2, Size * 2);
            skPath.LineTo(Size, Size * 2);
            skPath.Close();

            var skMatrix = SKMatrix.CreateScale(Size * 2, Size * 2);

            using var paint = new SKPaint();
            paint.Shader = _shader;
            paint.PathEffect = SKPathEffect.Create2DPath(skMatrix, skPath);

            skia.SkCanvas.DrawRect(0 - Size * 2, 0 - Size * 2, width + Size * 2, height + Size * 2, paint);
        }
    }
}
