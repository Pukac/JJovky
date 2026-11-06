namespace JJPoems.Models;

public static class ColorBlendHelper
{
	public static string Blend(string fromHex, string toHex, double amount)
	{
		var from = Parse(fromHex);
		var to = Parse(toHex);
		var clamped = Math.Clamp(amount, 0d, 1d);

		var red = (byte)Math.Round(from.Red + ((to.Red - from.Red) * clamped));
		var green = (byte)Math.Round(from.Green + ((to.Green - from.Green) * clamped));
		var blue = (byte)Math.Round(from.Blue + ((to.Blue - from.Blue) * clamped));

		return $"#{red:X2}{green:X2}{blue:X2}";
	}

	private static (byte Red, byte Green, byte Blue) Parse(string hex)
	{
		var normalized = hex.TrimStart('#');
		return
		(
			Convert.ToByte(normalized.Substring(0, 2), 16),
			Convert.ToByte(normalized.Substring(2, 2), 16),
			Convert.ToByte(normalized.Substring(4, 2), 16)
		);
	}
}
