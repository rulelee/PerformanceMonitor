using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;

using CSDeskBand;

namespace PerformanceMonitor
{
	[ComVisible(true)]
	[Guid("AA01ACB3-6CCC-497C-9CE6-9211F2EDFC10")]
	[CSDeskBandRegistration(Name = "性能监视器")]
	public class DeskBand : CSDeskBandWpf
	{
		public DeskBand()
		{
			int width = 75, height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - System.Windows.Forms.SystemInformation.WorkingArea.Height;
			foreach (ManagementObject mo in new ManagementClass(@"\\.\ROOT\cimv2:Win32_DesktopMonitor").GetInstances())
			{
				Options.MinHorizontalSize = new Size(
					width * Convert.ToInt32(mo.GetPropertyValue("PixelsPerXLogicalInch")) / 96,
					height * Convert.ToInt32(mo.GetPropertyValue("PixelsPerYLogicalInch")) / 96
				);
				return;
			}
			Options.MinHorizontalSize = new Size(width, height);
		}

		protected override UIElement UIElement => new MainControl();

	}
}
