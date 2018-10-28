using SkiaSharp;

namespace HipsterEngine.Core.Graphics
{
   public class Canvas
    {
        public Surface Surface { get; set; }
        private SKCanvas _canvas { get; set; }
        public Camera Camera { get; set; }

        public Canvas(Surface surface)
        {
            Surface = surface;
            Camera = new Camera(surface);
        }

        public void ClipRect(SKRect rect)
        {
            _canvas.ClipRect(rect);
        }

        public void Save()
        {
            _canvas.Save();
        }

        public void Restore()
        {
            _canvas.Restore();
        }

        public void RotateDegrees(float value)
        {
            _canvas.RotateDegrees(value);
        }
        
        public void RotateRadians(float value)
        {
            _canvas.RotateRadians(value);
        }
        
        public void RotateRadians(float value, float px, float py)
        {
            _canvas.RotateRadians(value, px, py);
        }

        public void DrawPath(SKPath path, SKPaint paint)
        {
            _canvas.DrawPath(path, paint);
        }
        
        public void DrawBitmap(SKBitmap bitmap, float x, float y, SKPaint paint)
        {
            _canvas.DrawBitmap(bitmap, x, y, paint);
        }
        
        public void DrawBitmap(SKBitmap bitmap, SKRect rect, SKPaint paint)
        {
            _canvas.DrawBitmap(bitmap, rect, paint);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, SKPaint paint)
        {
            _canvas.DrawLine(x1, y1, x2, y2, paint);
        }
        
        public void Translate(float x, float y)
        {
            _canvas.Translate(x, y);
        }

        public void DrawCircle(float x, float y, float radius, SKPaint paint)
        {
            _canvas.DrawCircle(x, y, radius, paint);
        }
        
        public void DrawText(string text, float x, float y, SKPaint paint)
        {
            _canvas.DrawText(text, x, y, paint);
        }

        public void DrawRect(float x, float y, float width, float height, SKPaint paint)
        {
            _canvas.DrawRect(x, y, width, height, paint);
        }
        
        public void DrawRoundRect(float x, float y, float width, float height, float rx, float ry, SKPaint paint)
        {
            _canvas.DrawRoundRect(x, y, width, height, rx, ry, paint);
        }
        
        public void SetSkiaCanvas(SKCanvas canvas)
        {
            this._canvas = canvas;
        }

        public SKCanvas GetSkiaCanvas()
        {
            return _canvas;
        }

        public void Scale(float x, float y)
        {
            _canvas.Scale(x + Camera.ScaleX, y + Camera.ScaleY);
            //_canvas.Scale(x + Camera.ScaleX, y + Camera.ScaleY, 0, 0);
        }
    }
}