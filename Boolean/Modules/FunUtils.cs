using Boolean.Util;
using Discord;
using Discord.Interactions;
using SkiaSharp;

namespace Boolean;

public class FunUtils : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("execute", "Executes a fiendish individual.")]
    public async Task Execute(IUser user, string? reason = null)
    {
        await RespondAsync(embed: new EmbedBuilder
        {
            Description = $"{user.Mention} has been executed by {Context.User.Mention}"
                          + (reason != null ? $"\nReason: `{reason}`" : ""),
            Color = EmbedColors.Normal,
        }.Build());
    }

    [SlashCommand("conatidrake", "Creates the drake preference meme BUT conaticus")]
    public async Task Conatidrake(string top, string bottom)
    {
        try
        {
            var templateImage = SKImage.FromEncodedData("./images/Conatidrake.jpg");
            using (var surface = SKSurface.Create(templateImage.Info))
            {
                surface.Canvas.DrawImage(templateImage, 0, 0);

                var font = new SKPaint
                {
                    TextSize = 50,
                    IsAntialias = true,
                    Color = new SKColor(0, 0, 0),
                    Style = SKPaintStyle.Fill,
                };

                var width = templateImage.Width;
                var height = templateImage.Height;

                var topRect = new SKRect(width / 2, 0, width, height / 2);
                var bottomRect = new SKRect(width / 2, height / 2, width, height);

                Text.DrawTextBox(surface.Canvas, top, topRect, font);
                Text.DrawTextBox(surface.Canvas, bottom, bottomRect, font);

                var stream = surface.Snapshot().Encode().AsStream();
                await Context.Channel.SendFileAsync(stream, "conatidrake.png");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}