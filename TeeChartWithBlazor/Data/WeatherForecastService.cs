using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Steema.TeeChart;

namespace TeeChartWithBlazor.Data
{
	public class WeatherForecastService
	{
		private static readonly string[] Summaries = new[]
		{
						"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private static readonly int[] temps = new[]
		{
				54,28,54,15,23,74	 //using fixed, common temp source
		};

		public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
		{
			var rng = new Random();
			return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = startDate.AddDays(index),
				TemperatureC = temps[index],
				Summary = Summaries[rng.Next(Summaries.Length)]
			}).ToArray());
		}

		public Task<String> GetChart(DateTime startDate, int width, int height)
		{
			Steema.TeeChart.Chart mChart = new Chart();
			Steema.TeeChart.Styles.Bar mBar = new Steema.TeeChart.Styles.Bar();

			mChart.Header.Text = "TeeChart in Blazor example";

			mChart.Series.Add(mBar);
			double Date = startDate.Date.AddDays(1).ToOADate();

			//mBar.FillSampleValues();
			for (int i = 1; i < 6; i++)
			{
				mBar.Add(Date, temps[i]);
				Date = startDate.Date.AddDays(i + 1).ToOADate();
			}

			mBar.XValues.DateTime = true;
			//mChart.Axes.Bottom.Labels.Angle = 90;
			mChart.Axes.Bottom.Increment = Steema.TeeChart.Utils.GetDateTimeStep(DateTimeSteps.OneDay);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			mChart.Export.Image.JPEG.Width = width;
			mChart.Export.Image.JPEG.Height = height;
			mChart.Export.Image.JPEG.Save(ms);
			System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

			String str = Convert.ToBase64String(ms.ToArray());

			return Task.FromResult("data:image/bmp;base64," + str);

		}

		public static byte[] ReadFully(MemoryStream input)
		{
			var buffer = new byte[16 * 1024];
			var ms = new MemoryStream();
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				ms.Write(buffer, 0, read);
			}
			return ms.ToArray();
		}

		public Task<string> GetJSChart(DateTime startDate, int width, int height)
		{
			Steema.TeeChart.Chart mChart = new Chart();
			Steema.TeeChart.Styles.Bar mBar = new Steema.TeeChart.Styles.Bar();

			mChart.Header.Text = "TeeChart in Blazor example";

			mChart.Series.Add(mBar);
			double Date = startDate.Date.AddDays(1).ToOADate();

			//mBar.FillSampleValues();
			for (int i = 1; i < 6; i++)
			{
				mBar.Add(Date, temps[i]);
				Date = startDate.Date.AddDays(i + 1).ToOADate();
			}

			mBar.XValues.DateTime = true;
			//mChart.Axes.Bottom.Labels.Angle = 90;
			mChart.Axes.Bottom.Increment = Steema.TeeChart.Utils.GetDateTimeStep(DateTimeSteps.OneDay);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			mChart.Export.Image.JScript.Width = width;
			mChart.Export.Image.JScript.Height = height;
			mChart.Export.Image.JScript.DoFullPage = false; //inline, no page <html> header tags
			mChart.Export.Image.JScript.CustomCode = new string[] { "resize(chart1);" };
			mChart.Export.Image.JScript.Save(ms);
			

			ms.Position = 0;

			StreamReader reader = new StreamReader(ms);
			string result = "<script> var chart1; " + reader.ReadToEnd() + "</script>";
			//string result = reader.ReadToEnd();

			//Response.ContentType = "text/html";
			//Response.Body.Write(output, 0, output.Length);

			return Task.FromResult(result); // ("data:image/bmp;base64," + str);
		}
	
	}

}
