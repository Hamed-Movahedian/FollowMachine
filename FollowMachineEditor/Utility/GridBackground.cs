using System;
using UnityEngine;

namespace FMachine.Editor
{
    [Serializable]
    public class GridBackground
    {
        private readonly Color _backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
        private readonly Color _backgroundLineColor01 = new Color(0.14f, 0.14f, 0.14f, 1f);
        private readonly Color _backgroundLineColor02 = new Color(0.10f, 0.10f, 0.10f, 1f);

        private const int Height=100;
        private const int Width=100;

        private Texture2D _texture;

        public void CreateTexture()
        {
            _texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
            for (var x = 0; x < _texture.width; x++)
            {
                for (var y = 0; y < _texture.width; y++)
                {
                    bool isVerticalLine = (x % 11 == 0);
                    bool isHorizontalLine = (y % 11 == 0);
                    if (x == 0 || y == 0) _texture.SetPixel(x, y, _backgroundLineColor02);
                    else if (isVerticalLine || isHorizontalLine) _texture.SetPixel(x, y, _backgroundLineColor01);
                    else _texture.SetPixel(x, y, _backgroundColor);
                }
            }
            _texture.filterMode = FilterMode.Trilinear;
            _texture.wrapMode = TextureWrapMode.Repeat;
            _texture.Apply();
        }

        public void Draw(FMCanvas canvas)
        {
            if (_texture == null)
                CreateTexture();

            GUI.DrawTextureWithTexCoords(
                canvas.WindowRect,
                _texture,
                new Rect(
                    (canvas.Position.x - canvas.CanvasSize) * canvas.Zoom / -Width,
                    (canvas.Position.y - canvas.CanvasSize) * canvas.Zoom / Height,
                    canvas.WindowRect.width / Width,
                    canvas.WindowRect.height / Height));

        }
    }
}