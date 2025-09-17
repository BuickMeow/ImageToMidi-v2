using Avalonia;
using Avalonia.Controls;
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

    public ZoomableImage()
    {
        InitializeComponent();
        
        // Get reference to the image control
        _imageControl = this.FindControl<Image>("ImageControl");
        
        // Subscribe to property changes
        ImageSourceProperty.Changed.AddClassHandler<ZoomableImage>((sender, e) =>
        {
            if (sender is ZoomableImage control)
            {
                control.OnImageSourceChanged(e.NewValue as string);
            }
        });
    }

    private void OnImageSourceChanged(string? imagePath)
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
            }
        }
        catch (Exception)
        {
            // Handle image loading errors gracefully
            _imageControl.Source = null;
        }
    }
}