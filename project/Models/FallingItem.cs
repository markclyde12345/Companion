namespace MindBloom.Models;

public enum FallingItemType
{
    Star,   // positive word → catch it
    Cloud   // negative word → avoid it
}

public class FallingItem
{
    public string Word       { get; set; } = string.Empty;
    public FallingItemType Type { get; set; }
    public double X          { get; set; }  // 0.0 – 1.0 (relative to screen width)
    public double Y          { get; set; }  // pixels from top
    public double Speed      { get; set; }  // pixels per tick
    public bool   IsActive   { get; set; } = true;

    // Visual helpers
    public string Emoji => Type == FallingItemType.Star ? "⭐" : "☁️";
    public Color  BgColor => Type == FallingItemType.Star
        ? Color.FromArgb("#3D9E5C")
        : Color.FromArgb("#C0392B");
    public Color  TextColor => Colors.White;
}
