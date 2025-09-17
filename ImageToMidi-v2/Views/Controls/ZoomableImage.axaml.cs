using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;

namespace ImageToMidi_v2;

public partial class ZoomableImage : UserControl
{
    public static readonly StyledProperty<string?> ImageSourceProperty =
        AvaloniaProperty.Register<ZoomableImage, string?>(nameof(ImageSource));

    public string? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    private Image? _imageControl;
    private ScaleTransform? _scaleTransform;
    private TranslateTransform? _translateTransform;
    
    // Zoom and pan properties
    private double _zoom = 1.0;
    private double _targetZoom = 1.0;
    private Point _offset = new(0, 0);
    private Size _containerSize = new(1, 1);
    private Size _imageSize = new(1, 1);
    
    // Mouse handling
    private bool _mouseNotMoved = true;
    private bool _mouseIsDown = false;
    private Point _mouseMoveStart;
    private Point _offsetStart;
    
    // Animation
    private System.Threading.Timer? _zoomTimer;
    
    // Constants - �������ŷ���
    private const double ZoomStep = 1; // ��0.2��Ϊ1���������ŷ���
    private const double MinZoom = 0.1;
    private const double MaxZoom = 10.0;
    private const double MaxOffset = 2.0; // ������ק�߽�

    public ZoomableImage()
    {
        InitializeComponent();
        
        ImageSourceProperty.Changed.AddClassHandler<ZoomableImage>((sender, e) =>
        {
            if (sender is ZoomableImage control)
            {
                control.OnImageSourceChanged(e.NewValue as string);
            }
        });

        this.SizeChanged += OnSizeChanged;
    }

    protected override void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        _imageControl = this.FindControl<Image>("ImageControl");

        if (_imageControl?.RenderTransform is TransformGroup transformGroup)
        {
            foreach (var transform in transformGroup.Children)
            {
                if (transform is ScaleTransform scaleTransform && _scaleTransform == null)
                {
                    _scaleTransform = scaleTransform;
                }
                else if (transform is TranslateTransform translateTransform && _translateTransform == null)
                {
                    _translateTransform = translateTransform;
                }
            }
        }

        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        this.PointerWheelChanged += OnPointerWheelChanged;
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    private void OnImageSourceChanged(string? imagePath)
    {
        LoadImage(imagePath);
    }

    private void LoadImage(string? imagePath)
    {
        if (_imageControl == null) return;

        if (string.IsNullOrEmpty(imagePath))
        {
            _imageControl.Source = null;
            return;
        }

        try
        {
            if (File.Exists(imagePath))
            {
                var bitmap = new Bitmap(imagePath);
                _imageControl.Source = bitmap;
                _imageSize = new Size(bitmap.PixelSize.Width, bitmap.PixelSize.Height);
                
                Avalonia.Threading.Dispatcher.UIThread.Post(FitImageToControl);
            }
        }
        catch (Exception)
        {
            _imageControl.Source = null;
        }
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _containerSize = e.NewSize;
        
        if (_imageControl?.Source != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(FitImageToControl);
        }
    }

    #region Zoom and Pan Logic - �޸��������ĵ�����

    // �������λ�õ�����ƫ�Ƽ���
    private void UpdateZoomOffsetAtMousePosition(double oldZoom, double newZoom, Point mousePosition)
    {
        if (_imageSize.Width <= 0 || _imageSize.Height <= 0 || _containerSize.Width <= 0 || _containerSize.Height <= 0) 
            return;

        double scaleMult = newZoom / oldZoom;
        
        // ����ͼƬ��ʾ�ߴ�
        double aspect = _imageSize.Width / _imageSize.Height;
        double containerAspect = _containerSize.Width / _containerSize.Height;
        
        double displayWidth, displayHeight;
        if (aspect > containerAspect)
        {
            displayWidth = _containerSize.Width;
            displayHeight = _containerSize.Width / aspect;
        }
        else
        {
            displayWidth = _containerSize.Height * aspect;
            displayHeight = _containerSize.Height;
        }

        // ����ͼƬ�������е�ƫ�ƣ�������ʾʱ��ƫ�ƣ�
        double imageOffsetX = (_containerSize.Width - displayWidth) / 2;
        double imageOffsetY = (_containerSize.Height - displayHeight) / 2;

        // �����λ��ת��Ϊ�����ͼƬ��λ��
        double relativeMouseX = (mousePosition.X - imageOffsetX) / displayWidth;
        double relativeMouseY = (mousePosition.Y - imageOffsetY) / displayHeight;

        // ���ǵ�ǰ�����ź�ƽ��
        double currentImageX = relativeMouseX / oldZoom - _offset.X / oldZoom;
        double currentImageY = relativeMouseY / oldZoom - _offset.Y / oldZoom;

        // �����µ�ƫ�ƣ�ʹ���λ�ñ�������ͬ��ͼƬλ����
        double newOffsetX = relativeMouseX / newZoom - currentImageX;
        double newOffsetY = relativeMouseY / newZoom - currentImageY;

        _offset = new Point(newOffsetX, newOffsetY);
        ClampOffset();
        UpdateTransforms();
    }

    // �������ŵ�ƫ�Ƽ���
    private void UpdateZoomOffsetAtCenter(double oldZoom, double newZoom)
    {
        if (_imageSize.Width <= 0 || _imageSize.Height <= 0) return;

        // �Կؼ�����Ϊ���ŵ�
        var centerPoint = new Point(_containerSize.Width / 2, _containerSize.Height / 2);
        UpdateZoomOffsetAtMousePosition(oldZoom, newZoom, centerPoint);
    }

    private void ClampOffset()
    {
        // ��̬����߽磬���ڵ�ǰ���ż���
        double maxOffsetX = MaxOffset;
        double maxOffsetY = MaxOffset;

        if (_zoom > 1.0)
        {
            // ����ʱ��������ƫ�Ʒ�Χ
            maxOffsetX *= _zoom;
            maxOffsetY *= _zoom;
        }

        _offset = new Point(
            Math.Max(-maxOffsetX, Math.Min(maxOffsetX, _offset.X)),
            Math.Max(-maxOffsetY, Math.Min(maxOffsetY, _offset.Y))
        );
    }

    private void UpdateTransforms()
    {
        if (_scaleTransform != null)
        {
            _scaleTransform.ScaleX = _zoom;
            _scaleTransform.ScaleY = _zoom;
        }

        if (_translateTransform != null && _containerSize.Width > 0 && _containerSize.Height > 0 && _imageSize.Width > 0 && _imageSize.Height > 0)
        {
            double aspect = _imageSize.Width / _imageSize.Height;
            double containerAspect = _containerSize.Width / _containerSize.Height;
            
            double displayWidth = aspect > containerAspect ? _containerSize.Width : _containerSize.Height * aspect;
            double displayHeight = aspect > containerAspect ? _containerSize.Width / aspect : _containerSize.Height;

            _translateTransform.X = _offset.X * displayWidth;
            _translateTransform.Y = _offset.Y * displayHeight;
        }
    }

    #endregion

    #region Mouse Event Handlers - �޸����λ������

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_imageSize.Width <= 0) return;

        // �������ŷ��ȣ�ʹ�ø������������
        double scaleMult = Math.Pow(1.3, e.Delta.Y / 120.0); // ��1.2��Ϊ1.3
        double oldZoom = _targetZoom;
        _targetZoom *= scaleMult;

        // �������ŷ�Χ
        _targetZoom = Math.Max(MinZoom, Math.Min(MaxZoom, _targetZoom));

        if (_targetZoom < 1)
        {
            _targetZoom = 1;
            if (_zoom <= 1)
            {
                // ����ʱ�����⴦��
                scaleMult = scaleMult * scaleMult * scaleMult;
                _offset = new Point(_offset.X * scaleMult, _offset.Y * scaleMult);
                UpdateTransforms();
            }
        }

        // ��ȡ���λ�ò��Դ�Ϊ��������
        var mousePosition = e.GetPosition(this);
        StartSmoothZoomAtPosition(mousePosition);
        
        ClampOffset();
        e.Handled = true;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _mouseIsDown = true;
        _mouseNotMoved = true;
        _mouseMoveStart = e.GetPosition(this);
        _offsetStart = _offset;
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_imageSize.Width <= 0) return;
        if (!_mouseIsDown) return;

        Point currentMousePos = e.GetPosition(this);
        Vector mouseOffset = currentMousePos - _mouseMoveStart;

        if (mouseOffset.X != 0 || mouseOffset.Y != 0)
        {
            _mouseNotMoved = false;

            // �Ľ���ק���жȼ���
            double zoom = Math.Max(1.0, _zoom);
            double sensitivity = 1.0 / zoom;

            // ������ʾ�ߴ�
            double aspect = _imageSize.Width / _imageSize.Height;
            double containerAspect = _containerSize.Width / _containerSize.Height;
            double displayWidth = aspect > containerAspect ? _containerSize.Width : _containerSize.Height * aspect;
            double displayHeight = aspect > containerAspect ? _containerSize.Width / aspect : _containerSize.Height;

            // ����ȷ����ק����
            _offset = new Point(
                _offset.X + mouseOffset.X * sensitivity / displayWidth,
                _offset.Y + mouseOffset.Y * sensitivity / displayHeight
            );
            ClampOffset();

            _mouseMoveStart = currentMousePos;
            UpdateTransforms();
        }

        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _mouseIsDown = false;
        e.Handled = true;
    }

    #endregion

    #region Animation - ֧�ֻ���λ�õ����Ŷ���

    private void StartSmoothZoom()
    {
        var centerPoint = new Point(_containerSize.Width / 2, _containerSize.Height / 2);
        StartSmoothZoomAtPosition(centerPoint);
    }

    private void StartSmoothZoomAtPosition(Point mousePosition)
    {
        _zoomTimer?.Dispose();

        var duration = TimeSpan.FromMilliseconds(150);
        var easing = new QuadraticEaseOut();
        var startZoom = _zoom;
        var startTime = DateTime.Now;

        _zoomTimer = new System.Threading.Timer(_ =>
        {
            var elapsed = DateTime.Now - startTime;
            var progress = Math.Min(1.0, elapsed.TotalMilliseconds / duration.TotalMilliseconds);
            
            if (progress >= 1.0)
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    var oldZoom = _zoom;
                    _zoom = _targetZoom;
                    UpdateZoomOffsetAtMousePosition(oldZoom, _zoom, mousePosition);
                });
                _zoomTimer?.Dispose();
                _zoomTimer = null;
                return;
            }

            var easedProgress = easing.Ease(progress);
            var interpolatedZoom = startZoom + (_targetZoom - startZoom) * easedProgress;
            
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                var oldZoom = _zoom;
                _zoom = interpolatedZoom;
                UpdateZoomOffsetAtMousePosition(oldZoom, _zoom, mousePosition);
            });
            
        }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
    }

    #endregion

    #region Public Methods

    public void FitImageToControl()
    {
        _targetZoom = 1.0;
        _offset = new Point(0, 0);
        StartSmoothZoom();
    }

    public void ZoomIn()
    {
        _targetZoom *= (1 + ZoomStep);
        _targetZoom = Math.Min(MaxZoom, _targetZoom);
        StartSmoothZoom();
    }

    public void ZoomOut()
    {
        _targetZoom *= (1 - ZoomStep);
        _targetZoom = Math.Max(MinZoom, _targetZoom);
        StartSmoothZoom();
    }

    public void ResetZoom()
    {
        FitImageToControl();
    }

    #endregion

    protected override void OnUnloaded(Avalonia.Interactivity.RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _zoomTimer?.Dispose();
        _zoomTimer = null;
    }
}