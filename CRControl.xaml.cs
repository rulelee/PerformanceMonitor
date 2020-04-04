using System;
using System.Diagnostics;
using System.Management;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PerformanceMonitor
{
	/// <summary>
	/// CRControl.xaml 的交互逻辑
	/// </summary>
	public partial class CRControl : UserControl
	{
		public CRControl()
		{
			InitializeComponent();

			PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
			PerformanceCounter ram = new PerformanceCounter("Process", "Working Set", "_Total");
			long totalCapacity = 0;
			try
			{
				foreach (ManagementObject mo in new ManagementClass(@"\\.\ROOT\cimv2:Win32_PhysicalMemory").GetInstances())
				{
					totalCapacity += long.Parse(mo.GetPropertyValue("Capacity").ToString());
				}
			}
			catch (Exception e)
			{
				totalCapacity = -1;
			}

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1000);
			timer.Tick += (object sender, EventArgs e) =>
			{
				this.cpu.Text = $"{cpu.NextValue().ToString("0")}%";
				this.ram.Text = $"{(ram.NextValue() / totalCapacity * 100).ToString("0")}%";
			};
			timer.Start();
		}
	}
}
