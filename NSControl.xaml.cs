using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PerformanceMonitor
{
	/// <summary>
	/// NSControl.xaml 的交互逻辑
	/// </summary>
	public partial class NSControl : UserControl
	{
		public NSControl()
		{
			InitializeComponent();

			ArrayList adapters = new ArrayList();

			foreach (var name in new PerformanceCounterCategory("Network Interface").GetInstanceNames())
			{
				if (name != "NS TCP Loopback interface")
				{
					adapters.Add(new NetworkAdapter(name));
				}
			}

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1000);
			timer.Tick += (object sender, EventArgs e) =>
			{
				double totalUploadSpeed = 0, totalDownloadSpeed = 0;
				foreach (NetworkAdapter adapter in adapters)
				{
					totalUploadSpeed += adapter.UploadSpeed;
					totalDownloadSpeed += adapter.DownloadSpeed;
				}
				NSUnit uploadUnit = MatchUnit(totalUploadSpeed);
				NSUnit downloadUnit = MatchUnit(totalDownloadSpeed);
				this.uploadSpeed.Text = uploadUnit.Value.ToString("0.#");
				this.uploadUnit.Text = $"{uploadUnit.Name}/s";
				this.downloadSpeed.Text = downloadUnit.Value.ToString("0.#");
				this.downloadUnit.Text = $"{ downloadUnit.Name}/s";
			};
			timer.Start();
		}

		/// <summary>
		/// 单位集合
		/// </summary>
		NSUnit[] units = {
			//new NSUnit("BB", Math.Pow(1024, 9)),
			//new NSUnit("YB", Math.Pow(1024, 8)),
			//new NSUnit("ZB", Math.Pow(1024, 7)),
			//new NSUnit("EB", Math.Pow(1024, 6)),
			//new NSUnit("PB", Math.Pow(1024, 5)),
			new NSUnit("TB", Math.Pow(1024, 4)),
			new NSUnit("GB", Math.Pow(1024, 3)),
			new NSUnit("MB", Math.Pow(1024, 2)),
			new NSUnit("KB", Math.Pow(1024, 1)),
			//new NSUnit("B", Math.Pow(1024, 0))
		};

		/// <summary>
		/// 匹配单位
		/// </summary>
		/// <param name="value">被匹配的值</param>
		/// <returns></returns>
		private NSUnit MatchUnit(double value)
		{
			foreach (var unit in this.units)
			{
				double result = value / unit.Value;
				if (result >= 1)
				{
					return new NSUnit(unit.Name, result);
				}
			}
			return new NSUnit("KB", value / 1024);
		}
	}

	/// <summary>
	/// 网络适配器
	/// </summary>
	public class NetworkAdapter
	{
		public NetworkAdapter(string name)
		{
			this.Name = name;
			this.UploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
			this.DownloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
		}

		public string Name { get; private set; }

		public PerformanceCounter UploadCounter { get; private set; }

		public PerformanceCounter DownloadCounter { get; private set; }

		public double UploadSpeed
		{
			get
			{
				return this.UploadCounter.NextValue();
			}
		}

		public double DownloadSpeed
		{
			get
			{
				return this.DownloadCounter.NextValue();
			}
		}

	}

	public class NSUnit
	{
		public NSUnit(string name, double value)
		{
			this.Name = name;
			this.Value = value;
		}

		public string Name { get; private set; }
		public double Value { get; private set; }
	}
}
