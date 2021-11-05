using Hanoi.Logic;
using Hanoi.Models;
using Hanoi.Util;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Hanoi.Controls
{
    public class GameControl : SKCanvasView
    {

        public static readonly BindableProperty GameLogicProperty =
            BindableProperty.Create(nameof(GameLogic), typeof(GameLogic), typeof(GameControl), null, BindingMode.OneWay, propertyChanged: GameLogicPropertyChanged);

        public static readonly BindableProperty GameSettingsProperty =
            BindableProperty.Create(nameof(GameSettings), typeof(GameSettings), typeof(GameControl), null, BindingMode.OneWay, propertyChanged: GameSettingsPropertyChanged);

        public static readonly BindableProperty GameRunningProperty =
            BindableProperty.Create(nameof(GameRunning), typeof(bool), typeof(GameControl), false, BindingMode.OneWay, propertyChanged: GameSettingsPropertyChanged);

        public GameLogic? GameLogic {
            get => (GameLogic) GetValue(GameLogicProperty);
            set => SetValue(GameLogicProperty, value);
        }

        public bool GameRunning
        {
            get => (bool) GetValue(GameRunningProperty);
            set => SetValue(GameRunningProperty, value);
        }

        public GameSettings? GameSettings
        {
            get => (GameSettings) GetValue(GameSettingsProperty);
            set => SetValue(GameSettingsProperty, value);
        }

        private readonly SKPaint _stickPaint = new()
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.DarkGray
        };
        private readonly SKPaint _textPaint = new()
        {
            Color = SKColors.Black,
            TextSize = 32,
            TextAlign = SKTextAlign.Center,
        };
        private readonly SKPaint _discPaint = new()
        {
            Style = SKPaintStyle.Fill
        };

        private readonly float[] _stickPositions = new float[3];

        const int StickWidth = 10;
        const int DiscHeight = 50;
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

            if (GameLogic == null || !GameRunning)
                return;


            DrawSticks(canvas);
            DrawSelected(canvas);
        }

        private void DrawSelected(SKCanvas canvas)
        {
            if (GameLogic?.SelectedDisc == null
                || GameLogic?.SelectedStack == null)
                return;

            int index = GameLogic.SelectedStack switch
            {
                StackName.Left => 0,
                StackName.Middle => 1,
                _ => 2
            };

            var currentX = _stickPositions[index];
            var y = _canvasHeight * 0.15f;

            var discWidth = _discSizes[GameLogic.SelectedDisc.Size];
            _discPaint.Color = GameLogic.SelectedDisc.Color;

            var x = currentX - discWidth / 2;
            var rect = new SKRect(x, y, x + discWidth, y + DiscHeight);
            var roundRect = new SKRoundRect(rect, 16f);
            canvas.DrawRoundRect(roundRect, _discPaint);

            DrawText(currentX, y, GameLogic.SelectedDisc, canvas);
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
            if (GameLogic == null)
                return;
            int count = 0;

            // from bottom to top
            foreach (var disc in stack.Reverse())
            {
                if (disc == GameLogic.SelectedDisc)
                    continue;
                var width = _discSizes[disc.Size];

                var startX = offsetX - width / 2;

                var startY = count * DiscHeight;
                startY = _canvasHeight - startY - DiscHeight; 
                _discPaint.Color = disc.Color;
                var roundRect = new SKRoundRect(new SKRect(startX, startY, startX + width, startY + DiscHeight), 15f);
                canvas.DrawRoundRect(roundRect, _discPaint);
                DrawText(offsetX, startY, disc, canvas);

                count++;
            }
        }

        private void DrawText(float x, float y, Disc disc, SKCanvas canvas)
        {
            if (GameSettings?.ShowDiscNumbers ?? false)
            {
                string text = disc.Size.ToString();
                SKRect bounds = new SKRect();
                _textPaint.MeasureText(text, ref bounds);

                var startY = y + DiscHeight / 2 + bounds.Height / 2;
                var isDark = Colors.IsDarkColor(disc.Color);
                _textPaint.Color = isDark ? SKColors.White : SKColors.Black;
                canvas.DrawText(text, x, startY, _textPaint);
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

                _stickPositions[i - 1] = currentX;
                DrawStack(canvas, i switch
                {
                    1 => GameLogic.Left,
                    2 => GameLogic.Middle,
                    _ => GameLogic.Right,
                }, currentX);
                currentX += StickWidth + (_maxDiscSize / 2);
            }
        }

        private static void GameSettingsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GameControl control)
                control.InvalidateSurface();
        }

        private static void GameLogicPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GameControl control && newValue != null)
            {
                control.CalculateDiscSizes();
                control.InvalidateSurface();
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            InvalidateSurface();
            CalculateDiscSizes();
            InvalidateSurface();

            return base.OnMeasure(widthConstraint, heightConstraint);
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
