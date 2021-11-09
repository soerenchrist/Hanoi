using Hanoi.Themes;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hanoi.Controls
{
    public class ThemeDisplay : SKCanvasView
    {
        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(GameTheme), typeof(ThemeDisplay), GameTheme.Blue, BindingMode.OneWay, propertyChanged: ThemePropertyChanged);

        public static readonly BindableProperty NumberOfDiscsProperty =
            BindableProperty.Create(nameof(NumberOfDiscs), typeof(int), typeof(ThemeDisplay), 3, BindingMode.OneWay, propertyChanged: NumberOfDiscsChanged);

        public static readonly BindableProperty SelectedThemeProperty =
            BindableProperty.Create(nameof(SelectedTheme), typeof(GameTheme), typeof(ThemeDisplay), GameTheme.Blue, BindingMode.OneWay, propertyChanged: SelectedPropertyChanged);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ThemeDisplay), null, BindingMode.OneWay);

        public ICommand? Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public GameTheme SelectedTheme
        {
            get => (GameTheme)GetValue(SelectedThemeProperty);
            set => SetValue(SelectedThemeProperty, value);
        }

        public GameTheme Theme
        {
            get => (GameTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public int NumberOfDiscs
        {
            get => (int) GetValue(NumberOfDiscsProperty);
            set => SetValue(NumberOfDiscsProperty, value);
        }

        private long _tapStartedTimeStamp;

        private ResourceDictionary? _currentDict;
        private int _canvasWidth;
        private int _canvasHeight;
        private float _maxDiscWidth;
        private float _maxDiscHeight;
        const int DefaultDiscHeight = 30;
        private const float MinDiscWidth = 80;
        private float[]? _discSizes;
        private float DiscHeight => Math.Min(_maxDiscHeight, DefaultDiscHeight);

        private readonly SKPaint _paint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        private readonly SKPaint _backgroundPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        private readonly SKPaint _selectedPaint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 12,
            IsAntialias = true
        };

        public ThemeDisplay()
        {
            _currentDict = ThemeHelper.GetDictionaryForTheme(Theme);
            EnableTouchEvents = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            (_canvasWidth, _canvasHeight) = (e.Info.Width, e.Info.Height);

            if (_currentDict == null)
                return;

            CalculateDiscSizes();
            CalculateMaxDiscHeight();

            canvas.Clear();

            var rect = new SKRect(0, 0, _canvasWidth, _canvasHeight);
            _backgroundPaint.Color = GetColorFromDictionary("BackgroundColor");
            canvas.DrawRoundRect(rect, 20f, 20f, _backgroundPaint);

            if (SelectedTheme == Theme)
            {
                _selectedPaint.Color = GetColorFromDictionary("AccentColor");
                canvas.DrawRoundRect(rect, 20f, 20f, _selectedPaint);
            }

            DrawStack(canvas);
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            const int thresh = 500;
            if (e.ActionType == SKTouchAction.Pressed)
            {
                _tapStartedTimeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
            else if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Exited)
            {
                var currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (currentTime - _tapStartedTimeStamp < thresh)
                    Command?.Execute(Theme);
            }
            e.Handled = true;
        }

        private void DrawStack(SKCanvas canvas)
        {
            if (_discSizes == null)
                return;
            for (int i = 0; i < NumberOfDiscs; i++)
            {
                var discSize = _discSizes[i];
                var left = _canvasWidth / 2 - discSize / 2;
                var bottom = _canvasHeight - DiscHeight * i - 10;
                var top = bottom - DiscHeight;
                var right = _canvasWidth / 2 + discSize / 2;

                var rect = new SKRect(left, top, right, bottom);
                _paint.Color = GetColorFromDictionary("Disc" + i);
                canvas.DrawRoundRect(rect, 15f, 15f, _paint);
            }
        }

        private SKColor GetColorFromDictionary(string key)
            => ((Color) _currentDict![key]).ToSKColor();

        private static void ThemePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ThemeDisplay themeDisplay && newValue is GameTheme theme)
            {
                themeDisplay._currentDict = ThemeHelper.GetDictionaryForTheme(theme);
                themeDisplay.InvalidateSurface();
            }
        }

        private void CalculateDiscSizes()
        {
            const int offset = 20;
            _maxDiscWidth = _canvasWidth - offset * 2;

            var sizes = new float[NumberOfDiscs];

            var sizeStep = (_maxDiscWidth - MinDiscWidth) / (NumberOfDiscs - 1);

            for (int i = 0; i < NumberOfDiscs; i++)
            {
                var step = sizeStep * i;
                sizes[i] = _maxDiscWidth - step;
            }

            _discSizes = sizes;
        }

        private void CalculateMaxDiscHeight()
        {
            var heightWithOffset = _canvasHeight - 30;
            _maxDiscHeight = heightWithOffset / NumberOfDiscs;
        }

        private static void NumberOfDiscsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ThemeDisplay themeDisplay)
            {
                themeDisplay.InvalidateSurface();
            }
        }

        private static void SelectedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ThemeDisplay themeDisplay)
            {
                themeDisplay.InvalidateSurface();
            }
        }

    }
}
