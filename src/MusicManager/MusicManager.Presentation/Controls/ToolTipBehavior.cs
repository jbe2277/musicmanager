﻿using System.Windows;
using System.Windows.Controls;

namespace Waf.MusicManager.Presentation.Controls;

public static class ToolTipBehavior
{
    public static readonly DependencyProperty AutoToolTipProperty =
        DependencyProperty.RegisterAttached("AutoToolTip", typeof(bool), typeof(ToolTipBehavior), new FrameworkPropertyMetadata(false, AutoToolTipPropertyChanged));

    [AttachedPropertyBrowsableForType(typeof(TextBlock))]
    public static bool GetAutoToolTip(DependencyObject element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return (bool)element.GetValue(AutoToolTipProperty);
    }

    public static void SetAutoToolTip(DependencyObject element, bool value)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        element.SetValue(AutoToolTipProperty, value);
    }

    private static void AutoToolTipPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        if (element is not TextBlock textBlock) throw new ArgumentException("The attached property AutoToolTip can only be used with a TextBlock.", nameof(element));
        if (textBlock.TextTrimming == TextTrimming.None) throw new InvalidOperationException("The attached property AutoToolTip can only be used with a TextBlock that uses one of the TextTrimming options.");
        var textDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        if (e.NewValue.Equals(true))
        {
            ComputeAutoToolTip(textBlock);
            textDescriptor.AddValueChanged(textBlock, TextBlockTextChanged);
            textBlock.SizeChanged += TextBlockSizeChanged;
        }
        else
        {
            textDescriptor.RemoveValueChanged(textBlock, TextBlockTextChanged);
            textBlock.SizeChanged -= TextBlockSizeChanged;
        }
    }

    private static void TextBlockTextChanged(object? sender, EventArgs e) => ComputeAutoToolTip((TextBlock)sender!);

    private static void TextBlockSizeChanged(object? sender, SizeChangedEventArgs e) => ComputeAutoToolTip((TextBlock)sender!);

    private static void ComputeAutoToolTip(TextBlock textBlock)
    {
        // It is necessary to call Measure so that the DesiredSize gets updated.
        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var desiredWidth = textBlock.DesiredSize.Width;
        ToolTipService.SetToolTip(textBlock, textBlock.ActualWidth < desiredWidth ? textBlock.Text : null);
    }
}
