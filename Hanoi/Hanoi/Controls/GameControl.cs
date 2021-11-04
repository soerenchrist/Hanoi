using Hanoi.Logic;
using Hanoi.Models;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Hanoi.Controls
{
    public class GameControl : SKCanvasView
    {

        public static readonly BindableProperty GameLogicProperty =
            BindableProperty.Create(nameof(GameLogic), typeof(GameLogic), typeof(GameControl), null, BindingMode.OneWay, propertyChanged: GameLogicPropertyChanged);

        public GameLogic? GameLogic {
            get => (GameLogic) GetValue(GameLogicProperty);
            set => SetValue(GameLogicProperty, value);
        }

        private readonly SKPaint _stickPaint = new()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.DarkGray
        };
        private readonly SKPaint _discPaint = new()
        {
            Style = SKPaintStyle.Fill
        };

        const int StickWidth = 10;
        const int DiscHeight = 40;
        const int MinDiscWidth = 100;
        const int MarginBetweenDiscs = 50;

        private int _canvasWidth;
        private int _canvasHeight;
        private float _maxDiscSize;

        private Dictionary<int, float> _discSizes = new Dictionary<int, float>();

        public GameControl()
        {
            EnableTouchEvents = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            _canvasWidth = e.Info.Width;
            _canvasHeight = e.Info.Height;

            if (GameLogic == null)
                return;


            DrawSticks(canvas);
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            if (GameLogic == null)
                return;
            if (e.ActionType != SKTouchAction.Pressed)
                return;

            var sectionWidth = _canvasWidth / 3;
            var leftSection = sectionWidth * 1;
            var middleSecion = sectionWidth * 2;

            if (e.Location.X < leftSection)
                GameLogic.SelectStack(StackName.Left);
            else if (e.Location.X < middleSecion)
                GameLogic.SelectStack(StackName.Middle);
            else
                GameLogic.SelectStack(StackName.Right);

            InvalidateSurface();
        }

        private void DrawStack(SKCanvas canvas, IEnumerable<Disc> stack, float offsetX)
        {
            int count = 0;

            // from bottom to top
            foreach (var disc in stack.Reverse())
            {
                var width = _discSizes[disc.Size];

                var startX = offsetX - width / 2;

                var startY = count * DiscHeight;
                startY = _canvasHeight - startY - DiscHeight; 
                _discPaint.Color = disc.Color;
                canvas.DrawRect(startX, startY, width, DiscHeight, _discPaint);

                count++;
            }
        }

        private void DrawSticks(SKCanvas canvas)
        {
            if (GameLogic == null)
                return;

            var stickHeight = _canvasHeight * 0.75f;
            var top = _canvasHeight - stickHeight;

            var currentX = 0.0f;
            for (int i = 1; i < 4; i++)
            {
                currentX += MarginBetweenDiscs;
                currentX += _maxDiscSize / 2;

                var stickX = currentX - (StickWidth / 2);

                canvas.DrawRect(stickX, top, StickWidth, stickHeight, _stickPaint);

                DrawStack(canvas, i switch
                {
                    1 => GameLogic.Left,
                    2 => GameLogic.Middle,
                    _ => GameLogic.Right,
                }, currentX);
                currentX += StickWidth + (_maxDiscSize / 2);
            }
        }

        private static void GameLogicPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GameControl control && newValue != null)
            {
                control.CalculateDiscSizes();
                control.InvalidateSurface();
            }
        }

        private void CalculateDiscSizes()
        {
            if (GameLogic == null)
                return;
            var spaceForDiscs = _canvasWidth - 4 * MarginBetweenDiscs;
            _maxDiscSize = ((float)spaceForDiscs / 3);
            _discSizes = new Dictionary<int, float>();

            var sizeStep = (_maxDiscSize - MinDiscWidth) / (GameLogic.NumberOfDiscs - 1);

            for (int i = 0; i < GameLogic.NumberOfDiscs; i++)
            {
                var step = sizeStep * i;
                _discSizes[GameLogic.NumberOfDiscs - i] = _maxDiscSize - step;
            }
        }
    }
}
