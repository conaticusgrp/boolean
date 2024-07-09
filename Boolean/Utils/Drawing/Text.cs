using SkiaSharp;

public static class Text
{
    public static void DrawTextBox(SKCanvas canvas, string text, SKRect rect, SKPaint paint)
    {
        var spaceWidth = paint.MeasureText(" ");
        var wordX = rect.Left;
        var wordY = rect.Top + paint.TextSize;

        foreach (var word in text.Split(' '))
        {
            var wordWidth = paint.MeasureText(word);

            if (wordWidth > rect.Right - wordX)
            {
                wordY += paint.FontSpacing;
                wordX = rect.Left;
            }

            canvas.DrawText(word, wordX, wordY, paint);
            wordX += wordWidth + spaceWidth;
        }
    }
}