using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Wordel.Model;

namespace Wordel.Components;

public partial class AnswerField : UserControl
{
    public static readonly DirectProperty<AnswerField, int> MaxLengthProperty =
        AvaloniaProperty.RegisterDirect<AnswerField, int>(
            nameof(MaxLength),
            o => o.MaxLength,
            (o, v) => o.MaxLength = v);

    private int _maxLength = 5;

    public int MaxLength
    {
        get => _maxLength;
        set => SetAndRaise(MaxLengthProperty, ref _maxLength, value);
    }

    public static readonly StyledProperty<double> CellSpacingProperty =
        AvaloniaProperty.Register<AnswerField, double>(nameof(CellSpacing),8.0);

    public double CellSpacing
    {
        get => GetValue(CellSpacingProperty);
        set => SetValue(CellSpacingProperty, value);
    }

    public double CellWidth
    {
        get => GetValue(WidthProperty) / MaxLength - CellSpacing;
    }

    public double CellHeight
    {
        get => GetValue(HeightProperty) - BorderThickness * 2;
    }

    public static readonly StyledProperty<ISolidColorBrush> BackgroundCorrectProperty = AvaloniaProperty.Register<AnswerField, ISolidColorBrush>(
        "BackgroundCorrect", Brushes.DarkGreen);

    public ISolidColorBrush BackgroundCorrect
    {
        get => GetValue(BackgroundCorrectProperty);
        set => SetValue(BackgroundCorrectProperty, value);
    }

    public static readonly StyledProperty<ISolidColorBrush> BackgroundPossibleProperty = AvaloniaProperty.Register<AnswerField, ISolidColorBrush>(
        "BackgroundPossible", Brushes.DarkBlue);

    public ISolidColorBrush BackgroundPossible
    {
        get => GetValue(BackgroundPossibleProperty);
        set => SetValue(BackgroundPossibleProperty, value);
    }

    public static readonly StyledProperty<ISolidColorBrush> BackgroundWrongProperty = AvaloniaProperty.Register<AnswerField, ISolidColorBrush>(
        "BackgroundWrong", Brushes.DarkRed);

    public ISolidColorBrush BackgroundWrong
    {
        get => GetValue(BackgroundWrongProperty);
        set => SetValue(BackgroundWrongProperty, value);
    }

    public new static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<AnswerField, double>(nameof(BorderThickness), 2.0);

    public new double BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    private static readonly DirectProperty<AnswerField, string> CurrentAnswerProperty =
        AvaloniaProperty.RegisterDirect<AnswerField, string>(
            nameof(CurrentAnswer),
            o => o.CurrentAnswer,
            (o, v) => o.CurrentAnswer = v);

    private string _currentAnswer = "";

    public string CurrentAnswer
    {
        get => _currentAnswer;
        set => SetAndRaise(CurrentAnswerProperty, ref _currentAnswer, value);
    }

    public static readonly DirectProperty<AnswerField, string> CorrectAnswerProperty =
        AvaloniaProperty.RegisterDirect<AnswerField, string>(
            nameof(CorrectAnswer),
            o => o.CorrectAnswer,
            (o, v) => o.CorrectAnswer = v);

    private string _correctAnswer = "";

    public string CorrectAnswer
    {
        get => _correctAnswer;
        set => SetAndRaise(CorrectAnswerProperty, ref _correctAnswer, value);
    }

    public AnswerField()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public ISolidColorBrush[] UseColors(LetterUse[] uses, int length)
    {
        var result = Enumerable.Repeat((ISolidColorBrush) Brushes.Transparent, length).ToArray();
        for (var i = 0; i < uses.Length; i++)
        {
            result[i] = uses[i] switch
            {
                LetterUse.Correct => BackgroundCorrect,
                LetterUse.Possible => BackgroundPossible,
                LetterUse.Wrong => BackgroundWrong,
                _ => Brushes.Transparent
            };
        }
        return result;
    }

    public override void Render(DrawingContext context)
    {
        var fill = UseColors(WordUtil.MatchInput(_correctAnswer, _currentAnswer), MaxLength);
        
        for (var i = 0; i < MaxLength; i++)
        {
            var pos = new Point(BorderThickness + i * (CellWidth + CellSpacing), BorderThickness);

            var pen = new Pen(BorderBrush, BorderThickness);
            context.DrawRectangle(fill[i], pen,
                new Rect(pos, new Size(CellWidth, CellHeight)), 4, 4);
            
            if (i >= _currentAnswer.Length)
            {
                continue;
            }

            var answerLetter = _currentAnswer.ToCharArray().GetValue(i);

            var text = new FormattedText(answerLetter?.ToString() ?? "", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                Typeface.Default, CellHeight / 2.0,
                Brushes.WhiteSmoke);
            var textPos = new Point(pos.X + (CellWidth / 2.0) - (text.Width / 2.0),
                pos.Y + (CellHeight / 2.0) - (text.Height / 2.0));
            context.DrawText(text, textPos);
        }
    }
}