using System;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace PerformanceMonitor
{
	/// <summary>
	/// MainControl.xaml 的交互逻辑
	/// </summary>
	public partial class MainControl : UserControl
	{
		public MainControl()
		{
			InitializeComponent();
		}

		public float[] GetScale()
		{
			foreach (ManagementObject mo in new ManagementClass(@"\\.\ROOT\cimv2:Win32_DesktopMonitor").GetInstances())
			{
				foreach (var p in mo.Properties)
				{
					return new float[] {
						Convert.ToInt32(mo.GetPropertyValue("PixelsPerXLogicalInch")) / 96,
						Convert.ToInt32(mo.GetPropertyValue("PixelsPerYLogicalInch")) / 96
					};
				}
			}
			return new float[] { 1, 1 };
		}

		public void LoadControl(string name)
		{
			try
			{
				Control control = (Control)Assembly.Load("PerformanceMonitor").CreateInstance($"PerformanceMonitor.{name}Control");
				control.ContextMenu = (ContextMenu)Resources["ContextMenu"];
				this.content.Children.Clear();
				this.content.Children.Add(control);
			}
			catch (Exception e) { }
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.LoadControl("NS");
		}

		private void Item_Click(object sender, RoutedEventArgs e)
		{
			LoadControl(((MenuItem)sender).Name);
		}
	}
}
