using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.IO;
using ImageToMidi_v2.Configuration;
using ImageToMidi_v2.Models;
using ImageToMidi_v2.Services.Interfaces;
using ImageToMidi_v2.Services.Implementation;

namespace ImageToMidi_v2;

/// <summary>
/// 可缩放图片控件，提供图片显示、缩放、平移和动画功能
/// 支持鼠标滚轮缩放、拖拽平移以及平滑动画效果
/// </summary>
public partial class ZoomableImage : UserControl
{
    /// <summary>
    /// 图片源路径的依赖属性
    /// </summary>
    public static readonly StyledProperty<string?> ImageSourceProperty =
        AvaloniaProperty.Register<ZoomableImage, string?>(nameof(ImageSource));

    /// <summary>
    /// 获取或设置图片源路径
    /// </summary>
    /// <value>图片文件的本地路径</value>
    public string? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    // 私有字段
    private Image? _imageControl;
    private ScaleTransform? _scaleTransform;
    private TranslateTransform? _translateTransform;
    
    private readonly ZoomPanState _state = new();
    private readonly IZoomPanCalculator _zoomPanCalculator;
    private readonly IImageAnimationService _animationService;

    /// <summary>
    /// 初始化 ZoomableImage 控件的新实例（使用默认服务）
    /// </summary>
    public ZoomableImage() : this(new ZoomPanCalculator(), null)
    {
    }

    /// <summary>
    /// 初始化 ZoomableImage 控件的新实例（使用指定服务）
    /// </summary>
    /// <param name="zoomPanCalculator">缩放平移计算服务</param>
    /// <param name="animationService">动画服务，如果为 null 则使用默认实现</param>
    public ZoomableImage(IZoomPanCalculator zoomPanCalculator, IImageAnimationService? animationService)
    {
        _zoomPanCalculator = zoomPanCalculator ?? throw new ArgumentNullException(nameof(zoomPanCalculator));
        _animationService = animationService ?? new ImageAnimationService(zoomPanCalculator);
        
        InitializeComponent();
        
        // 监听图片源属性变化
        ImageSourceProperty.Changed.AddClassHandler<ZoomableImage>((sender, e) =>
        {
            if (sender is ZoomableImage control)
            {
                control.OnImageSourceChanged(e.NewValue as string);
            }
        });

        this.SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// 控件加载完成时的处理
    /// </summary>
    /// <param name="e">路由事件参数</param>
    protected override void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        // 获取图片控件引用
        _imageControl = this.FindControl<Image>("ImageControl");

        // 获取变换对象引用
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

    /// <summary>
    /// 设置鼠标事件处理器
    /// </summary>
    private void SetupEventHandlers()
    {
        this.PointerWheelChanged += OnPointerWheelChanged;
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    /// <summary>
    /// 处理图片源路径变化
    /// </summary>
    /// <param name="imagePath">新的图片路径</param>
    private void OnImageSourceChanged(string? imagePath)
    {
        LoadImage(imagePath);
    }

    /// <summary>
    /// 加载指定路径的图片
    /// </summary>
    /// <param name="imagePath">图片文件路径</param>
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
                _state.ImageSize = new Size(bitmap.PixelSize.Width, bitmap.PixelSize.Height);
                
                // 异步执行适应控件大小的操作
                Avalonia.Threading.Dispatcher.UIThread.Post(FitImageToControl);
            }
        }
        catch (Exception)
        {
            // 加载失败时清空图片
            _imageControl.Source = null;
        }
    }

    /// <summary>
    /// 处理控件大小变化
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">尺寸变化事件参数</param>
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _state.ContainerSize = e.NewSize;
        
        if (_imageControl?.Source != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(FitImageToControl);
        }
    }

    /// <summary>
    /// 更新变换矩阵，应用当前的缩放和平移
    /// </summary>
    private void UpdateTransforms()
    {
        // 应用缩放变换
        if (_scaleTransform != null)
        {
            _scaleTransform.ScaleX = _state.Zoom;
            _scaleTransform.ScaleY = _state.Zoom;
        }

        // 应用平移变换
        if (_translateTransform != null && _state.ContainerSize.Width > 0 && _state.ContainerSize.Height > 0 && _state.ImageSize.Width > 0 && _state.ImageSize.Height > 0)
        {
            var displaySize = _zoomPanCalculator.GetDisplaySize(_state);
            _translateTransform.X = _state.Offset.X * displaySize.Width;
            _translateTransform.Y = _state.Offset.Y * displaySize.Height;
        }
    }

    #region 鼠标事件处理

    /// <summary>
    /// 处理鼠标滚轮事件，实现缩放功能
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">鼠标滚轮事件参数</param>
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_state.ImageSize.Width <= 0) return;

        // 计算缩放倍数
        double scaleMult = Math.Pow(ZoomPanConstants.ZoomScaleMultiplier, e.Delta.Y / ZoomPanConstants.ZoomSensitivityFactor);
        _state.TargetZoom *= scaleMult;

        // 限制缩放范围
        _state.TargetZoom = Math.Max(ZoomPanConstants.MinZoom, Math.Min(ZoomPanConstants.MaxZoom, _state.TargetZoom));

        // 处理缩小到原始大小以下的特殊情况
        if (_state.TargetZoom < 1)
        {
            _state.TargetZoom = 1;
            if (_state.Zoom <= 1)
            {
                // 重置时的特殊处理
                var resetMultiplier = Math.Pow(scaleMult, ZoomPanConstants.ResetZoomMultiplier);
                _state.Offset = new Point(_state.Offset.X * resetMultiplier, _state.Offset.Y * resetMultiplier);
                UpdateTransforms();
            }
        }

        // 获取鼠标位置并以此为中心缩放
        var mousePosition = e.GetPosition(this);
        StartSmoothZoomAtPosition(mousePosition);
        
        _zoomPanCalculator.ClampOffset(_state);
        e.Handled = true;
    }

    /// <summary>
    /// 处理鼠标按下事件，开始拖拽操作
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">鼠标按下事件参数</param>
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _state.MouseIsDown = true;
        _state.MouseNotMoved = true;
        _state.MouseMoveStart = e.GetPosition(this);
        _state.OffsetStart = _state.Offset;
        e.Handled = true;
    }

    /// <summary>
    /// 处理鼠标移动事件，实现拖拽平移功能
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">鼠标移动事件参数</param>
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_state.ImageSize.Width <= 0) return;
        if (!_state.MouseIsDown) return;

        Point currentMousePos = e.GetPosition(this);
        Vector mouseOffset = currentMousePos - _state.MouseMoveStart;

        if (mouseOffset.X != 0 || mouseOffset.Y != 0)
        {
            _state.MouseNotMoved = false;

            // 计算拖拽敏感度，缩放级别越高敏感度越低
            double zoom = Math.Max(1.0, _state.Zoom);
            double sensitivity = 1.0 / zoom;

            // 计算显示尺寸
            var displaySize = _zoomPanCalculator.GetDisplaySize(_state);

            // 更新偏移量
            _state.Offset = new Point(
                _state.Offset.X + mouseOffset.X * sensitivity / displaySize.Width,
                _state.Offset.Y + mouseOffset.Y * sensitivity / displaySize.Height
            );
            _zoomPanCalculator.ClampOffset(_state);

            _state.MouseMoveStart = currentMousePos;
            UpdateTransforms();
        }

        e.Handled = true;
    }

    /// <summary>
    /// 处理鼠标释放事件，结束拖拽操作
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="e">鼠标释放事件参数</param>
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _state.MouseIsDown = false;
        e.Handled = true;
    }

    #endregion

    #region 动画功能

    /// <summary>
    /// 启动以控件中心为基准的平滑缩放动画
    /// </summary>
    private void StartSmoothZoom()
    {
        var centerPoint = new Point(_state.ContainerSize.Width / 2, _state.ContainerSize.Height / 2);
        StartSmoothZoomAtPosition(centerPoint);
    }

    /// <summary>
    /// 启动以指定位置为中心的平滑缩放动画
    /// </summary>
    /// <param name="mousePosition">缩放中心位置</param>
    private void StartSmoothZoomAtPosition(Point mousePosition)
    {
        _animationService.AnimateZoomToPositionAsync(_state, mousePosition, UpdateTransforms, () => { });
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 使图片适应控件大小，重置缩放和偏移
    /// </summary>
    public void FitImageToControl()
    {
        _state.TargetZoom = 1.0;
        _state.Offset = new Point(0, 0);
        StartSmoothZoom();
    }

    /// <summary>
    /// 放大图片
    /// </summary>
    public void ZoomIn()
    {
        _state.TargetZoom *= (1 + ZoomPanConstants.ZoomStep);
        _state.TargetZoom = Math.Min(ZoomPanConstants.MaxZoom, _state.TargetZoom);
        StartSmoothZoom();
    }

    /// <summary>
    /// 缩小图片
    /// </summary>
    public void ZoomOut()
    {
        _state.TargetZoom *= (1 - ZoomPanConstants.ZoomStep);
        _state.TargetZoom = Math.Max(ZoomPanConstants.MinZoom, _state.TargetZoom);
        StartSmoothZoom();
    }

    /// <summary>
    /// 重置缩放，使图片适应控件大小
    /// </summary>
    public void ResetZoom()
    {
        FitImageToControl();
    }

    #endregion

    /// <summary>
    /// 控件卸载时的清理工作
    /// </summary>
    /// <param name="e">路由事件参数</param>
    protected override void OnUnloaded(Avalonia.Interactivity.RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        // 停止动画以释放资源
        _animationService.StopAnimation();
    }
}