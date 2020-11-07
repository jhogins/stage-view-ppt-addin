using System;
using System.Drawing;
using System.Linq;
using WindowsDisplayAPI;

namespace StageViewPpt
{
  public static class DisplayExtensions
  {
    public static Rectangle Bounds(this Display display) => new Rectangle(display.CurrentSetting.Position, display.CurrentSetting.Resolution);

    public static string SmartDeviceName(this Display display)
    {
      string name = display.DeviceName;
      return Display.GetDisplays().Count(d => d.DeviceName == name) > 1 ? display.DisplayName : name;
    }
  }
}
