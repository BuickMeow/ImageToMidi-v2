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
/// ������ͼƬ�ؼ����ṩͼƬ��ʾ�����š�ƽ�ƺͶ�������
/// ֧�����������š���קƽ���Լ�ƽ������Ч��
/// </summary>
public partial class ZoomableImage : UserControl
{
    /// <summary>
    /// ͼƬԴ·������������
    /// </summary>
    public static readonly StyledProperty<string?> ImageSourceProperty =
        AvaloniaProperty.Register<ZoomableImage, string?>(nameof(ImageSource));

    /// <summary>
    /// ��ȡ������ͼƬԴ·��
    /// </summary>
    /// <value>ͼƬ�ļ��ı���·��</value>
    public string? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    // ˽���ֶ�
    private Image? _imageControl;
    private ScaleTransform? _scaleTransform;
    private TranslateTransform? _translateTransform;
    
    private readonly ZoomPanState _state = new();
    private readonly IZoomPanCalculator _zoomPanCalculator;
    private readonly IImageAnimationService _animationService;

    /// <summary>
    /// ��ʼ�� ZoomableImage �ؼ�����ʵ����ʹ��Ĭ�Ϸ���
    /// </summary>
    public ZoomableImage() : this(new ZoomPanCalculator(), null)
    {
    }

    /// <summary>
    /// ��ʼ�� ZoomableImage �ؼ�����ʵ����ʹ��ָ������
    /// </summary>
    /// <param name="zoomPanCalculator">����ƽ�Ƽ������</param>
    /// <param name="animationService">�����������Ϊ null ��ʹ��Ĭ��ʵ��</param>
    public ZoomableImage(IZoomPanCalculator zoomPanCalculator, IImageAnimationService? animationService)
    {
        _zoomPanCalculator = zoomPanCalculator ?? throw new ArgumentNullException(nameof(zoomPanCalculator));
        _animationService = animationService ?? new ImageAnimationService(zoomPanCalculator);
        
        InitializeComponent();
        
        // ����ͼƬԴ���Ա仯
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
    /// �ؼ��������ʱ�Ĵ���
    /// </summary>
    /// <param name="e">·���¼�����</param>
    protected override void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        // ��ȡͼƬ�ؼ�����
        _imageControl = this.FindControl<Image>("ImageControl");

        // ��ȡ�任��������
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
    /// ��������¼�������
    /// </summary>
    private void SetupEventHandlers()
    {
        this.PointerWheelChanged += OnPointerWheelChanged;
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    /// <summary>
    /// ����ͼƬԴ·���仯
    /// </summary>
    /// <param name="imagePath">�µ�ͼƬ·��</param>
    private void OnImageSourceChanged(string? imagePath)
    {
        LoadImage(imagePath);
    }

    /// <summary>
    /// ����ָ��·����ͼƬ
    /// </summary>
    /// <param name="imagePath">ͼƬ�ļ�·��</param>
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
                
                // �첽ִ����Ӧ�ؼ���С�Ĳ���
                Avalonia.Threading.Dispatcher.UIThread.Post(FitImageToControl);
            }
        }
        catch (Exception)
        {
            // ����ʧ��ʱ���ͼƬ
            _imageControl.Source = null;
        }
    }

    /// <summary>
    /// ����ؼ���С�仯
    /// </summary>
    /// <param name="sender">�¼�������</param>
    /// <param name="e">�ߴ�仯�¼�����</param>
    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _state.ContainerSize = e.NewSize;
        
        if (_imageControl?.Source != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(FitImageToControl);
        }
    }

    /// <summary>
    /// ���±任����Ӧ�õ�ǰ�����ź�ƽ��
    /// </summary>
    private void UpdateTransforms()
    {
        // Ӧ�����ű任
        if (_scaleTransform != null)
        {
            _scaleTransform.ScaleX = _state.Zoom;
            _scaleTransform.ScaleY = _state.Zoom;
        }

        // Ӧ��ƽ�Ʊ任
        if (_translateTransform != null && _state.ContainerSize.Width > 0 && _state.ContainerSize.Height > 0 && _state.ImageSize.Width > 0 && _state.ImageSize.Height > 0)
        {
            var displaySize = _zoomPanCalculator.GetDisplaySize(_state);
            _translateTransform.X = _state.Offset.X * displaySize.Width;
            _translateTransform.Y = _state.Offset.Y * displaySize.Height;
        }
    }

    #region ����¼�����

    /// <summary>
    /// �����������¼���ʵ�����Ź���
    /// </summary>
    /// <param name="sender">�¼�������</param>
    /// <param name="e">�������¼�����</param>
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_state.ImageSize.Width <= 0) return;

        // �������ű���
        double scaleMult = Math.Pow(ZoomPanConstants.ZoomScaleMultiplier, e.Delta.Y / ZoomPanConstants.ZoomSensitivityFactor);
        _state.TargetZoom *= scaleMult;

        // �������ŷ�Χ
        _state.TargetZoom = Math.Max(ZoomPanConstants.MinZoom, Math.Min(ZoomPanConstants.MaxZoom, _state.TargetZoom));

        // ������С��ԭʼ��С���µ��������
        if (_state.TargetZoom < 1)
        {
            _state.TargetZoom = 1;
            if (_state.Zoom <= 1)
            {
                // ����ʱ�����⴦��
                var resetMultiplier = Math.Pow(scaleMult, ZoomPanConstants.ResetZoomMultiplier);
                _state.Offset = new Point(_state.Offset.X * resetMultiplier, _state.Offset.Y * resetMultiplier);
                UpdateTransforms();
            }
        }

        // ��ȡ���λ�ò��Դ�Ϊ��������
        var mousePosition = e.GetPosition(this);
        StartSmoothZoomAtPosition(mousePosition);
        
        _zoomPanCalculator.ClampOffset(_state);
        e.Handled = true;
    }

    /// <summary>
    /// ������갴���¼�����ʼ��ק����
    /// </summary>
    /// <param name="sender">�¼�������</param>
    /// <param name="e">��갴���¼�����</param>
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _state.MouseIsDown = true;
        _state.MouseNotMoved = true;
        _state.MouseMoveStart = e.GetPosition(this);
        _state.OffsetStart = _state.Offset;
        e.Handled = true;
    }

    /// <summary>
    /// ��������ƶ��¼���ʵ����קƽ�ƹ���
    /// </summary>
    /// <param name="sender">�¼�������</param>
    /// <param name="e">����ƶ��¼�����</param>
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_state.ImageSize.Width <= 0) return;
        if (!_state.MouseIsDown) return;

        Point currentMousePos = e.GetPosition(this);
        Vector mouseOffset = currentMousePos - _state.MouseMoveStart;

        if (mouseOffset.X != 0 || mouseOffset.Y != 0)
        {
            _state.MouseNotMoved = false;

            // ������ק���жȣ����ż���Խ�����ж�Խ��
            double zoom = Math.Max(1.0, _state.Zoom);
            double sensitivity = 1.0 / zoom;

            // ������ʾ�ߴ�
            var displaySize = _zoomPanCalculator.GetDisplaySize(_state);

            // ����ƫ����
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
    /// ��������ͷ��¼���������ק����
    /// </summary>
    /// <param name="sender">�¼�������</param>
    /// <param name="e">����ͷ��¼�����</param>
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _state.MouseIsDown = false;
        e.Handled = true;
    }

    #endregion

    #region ��������

    /// <summary>
    /// �����Կؼ�����Ϊ��׼��ƽ�����Ŷ���
    /// </summary>
    private void StartSmoothZoom()
    {
        var centerPoint = new Point(_state.ContainerSize.Width / 2, _state.ContainerSize.Height / 2);
        StartSmoothZoomAtPosition(centerPoint);
    }

    /// <summary>
    /// ������ָ��λ��Ϊ���ĵ�ƽ�����Ŷ���
    /// </summary>
    /// <param name="mousePosition">��������λ��</param>
    private void StartSmoothZoomAtPosition(Point mousePosition)
    {
        _animationService.AnimateZoomToPositionAsync(_state, mousePosition, UpdateTransforms, () => { });
    }

    #endregion

    #region ��������

    /// <summary>
    /// ʹͼƬ��Ӧ�ؼ���С���������ź�ƫ��
    /// </summary>
    public void FitImageToControl()
    {
        _state.TargetZoom = 1.0;
        _state.Offset = new Point(0, 0);
        StartSmoothZoom();
    }

    /// <summary>
    /// �Ŵ�ͼƬ
    /// </summary>
    public void ZoomIn()
    {
        _state.TargetZoom *= (1 + ZoomPanConstants.ZoomStep);
        _state.TargetZoom = Math.Min(ZoomPanConstants.MaxZoom, _state.TargetZoom);
        StartSmoothZoom();
    }

    /// <summary>
    /// ��СͼƬ
    /// </summary>
    public void ZoomOut()
    {
        _state.TargetZoom *= (1 - ZoomPanConstants.ZoomStep);
        _state.TargetZoom = Math.Max(ZoomPanConstants.MinZoom, _state.TargetZoom);
        StartSmoothZoom();
    }

    /// <summary>
    /// �������ţ�ʹͼƬ��Ӧ�ؼ���С
    /// </summary>
    public void ResetZoom()
    {
        FitImageToControl();
    }

    #endregion

    /// <summary>
    /// �ؼ�ж��ʱ��������
    /// </summary>
    /// <param name="e">·���¼�����</param>
    protected override void OnUnloaded(Avalonia.Interactivity.RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        // ֹͣ�������ͷ���Դ
        _animationService.StopAnimation();
    }
}