using System.Drawing;

namespace HomeLightControl.CustomClasses;

public class LampData
{
    public bool IsOn { get; set; }
    public int Brightness { get; set; } = -1;
    
    public Color Color { get; set; } = Color.Blue;
}