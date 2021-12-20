﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Waf.MusicManager.Presentation.Controls
{
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

        private IReadOnlyList<string> textParts = Array.Empty<string>();

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
            var newTextParts = SplitText();
            if (textParts.SequenceEqual(newTextParts)) return;
            
            var highlightBackground = HighlightBackground;
            Inlines.Clear();
            bool isHighlight = false;
            foreach (var textPart in newTextParts)
            {
                if (!string.IsNullOrEmpty(textPart))
                {
                    if (isHighlight)
                    {
                        Inlines.Add(new Run(textPart) { Background = highlightBackground });
                    }
                    else
                    {
                        Inlines.Add(new Run(textPart));
                    }
                }
                isHighlight = !isHighlight;
            }
            textParts = newTextParts;
        }

        private IReadOnlyList<string> SplitText()
        {
            var text = Text;
            var searchText = SearchText;

            if (string.IsNullOrEmpty(searchText)) return new[] { text };
            
            var parts = new List<string>();
            var index = 0;
            var comparisonType = IsMatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            while (true)
            {
                int position = text.IndexOf(searchText, index, comparisonType);
                if (position < 0) break;
                parts.Add(text[index..position]);
                parts.Add(text.Substring(position, searchText.Length));
                index = position + searchText.Length;
            }
            parts.Add(text[index..]);
            return parts;
        }

        private static void ControlPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SearchableTextBlock)d).UpdateContent();
    } 
}
