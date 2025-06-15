using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Waf.MusicManager.Presentation.Controls;

public class SearchableTextBlock : TextBlock
{
    public new static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchableTextBlock), new FrameworkPropertyMetadata(string.Empty, ControlPropertyChangedCallback));

    public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(SearchableTextBlock), new FrameworkPropertyMetadata(string.Empty, ControlPropertyChangedCallback));

    public static readonly DependencyProperty HighlightBackgroundProperty =
        DependencyProperty.Register(nameof(HighlightBackground), typeof(Brush), typeof(SearchableTextBlock), new FrameworkPropertyMetadata(Brushes.Orange, ControlPropertyChangedCallback));

    public static readonly DependencyProperty IsMatchCaseProperty =
        DependencyProperty.Register(nameof(IsMatchCase), typeof(bool), typeof(SearchableTextBlock), new FrameworkPropertyMetadata(false, ControlPropertyChangedCallback));

    private IReadOnlyList<string> textParts = [];

    public new string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public Brush HighlightBackground
    {
        get => (Brush)GetValue(HighlightBackgroundProperty);
        set => SetValue(HighlightBackgroundProperty, value);
    }

    public bool IsMatchCase
    {
        get => (bool)GetValue(IsMatchCaseProperty);
        set => SetValue(IsMatchCaseProperty, value);
    }

    private void UpdateContent()
    {
        var newTextParts = SplitText(Text, SearchText, IsMatchCase);
        if (textParts.SequenceEqual(newTextParts)) return;
            
        var highlightBackground = HighlightBackground;
        Inlines.Clear();
        bool isHighlight = false;
        foreach (var textPart in newTextParts)
        {
            if (!string.IsNullOrEmpty(textPart))
            {
                if (isHighlight) Inlines.Add(new Run(textPart) { Background = highlightBackground });
                else Inlines.Add(new Run(textPart));
            }
            isHighlight = !isHighlight;
        }
        textParts = newTextParts;
    }

    internal static IReadOnlyList<string> SplitText(string text, string searchText, bool isMatchCase)
    {
        if (string.IsNullOrEmpty(searchText)) return [text];
            
        var parts = new List<string>();
        var comparisonType = isMatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        var textSpan = text.AsSpan();
        while (true)
        {            
            int position = textSpan.IndexOf(searchText, comparisonType);
            if (position < 0) break;
            parts.Add(new string(textSpan[..position]));
            parts.Add(new string(textSpan[position..(position + searchText.Length)]));
            textSpan = textSpan[(position + searchText.Length)..];
        }
        parts.Add(new string(textSpan));
        return parts;
    }

    private static void ControlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SearchableTextBlock)d).UpdateContent();
} 
