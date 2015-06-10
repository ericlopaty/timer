using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace timer
{
	class Program
	{
		private enum Number
		{
			Decimal, Roman
		}

		static readonly int size = Console.WindowHeight * Console.WindowWidth - 1;
		static int lastSeconds = -1;
		static Random r = new Random(DateTime.Now.Day);

		static void Main(string[] args)
		{
			try
			{
				if (args.Length > 0 && args[0] == "?")
				{
					Console.WriteLine("timer <target> WS|WC|WL|SD|SR|BI|MD|MR|DHMS");
					Console.WriteLine();
					Console.WriteLine("WS - Waterfall - Static");
					Console.WriteLine("WC - Waterfall - Collapsing");
					Console.WriteLine("WL - Waterfall - Ladder");
					Console.WriteLine("SD - Seconds, decimal");
					Console.WriteLine("SR - Seconds, roman");
					Console.WriteLine("BI - Binary");
					Console.WriteLine("MD - Minutes, decimal");
					Console.WriteLine("MR - Minutes, roman");
					Console.WriteLine("MC - Minutes, collapse");
					Console.WriteLine("LB - Label");
					return;
				}
				DateTime now = DateTime.Now;
				DateTime target = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
				if (now > target)
					target = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
				string c = "SD";
				if (args.Length >= 1) target = DateTime.Parse(args[0]);
				if (args.Length >= 2) c = args[1];
				Console.Clear();
				Console.CursorVisible = false;
				Console.Title = string.Format("{0}: {1}", c, target);
				if (string.Compare("WS", c, true) == 0) WaterfallStatic(target);
				if (string.Compare("WC", c, true) == 0) WaterfallCollapse(target);
				if (string.Compare("WL", c, true) == 0) WaterfallLadder(target);
				if (string.Compare("SD", c, true) == 0) Seconds(target, Number.Decimal);
				if (string.Compare("SR", c, true) == 0) Seconds(target, Number.Roman);
				if (string.Compare("BI", c, true) == 0) Binary(target);
				if (string.Compare("MD", c, true) == 0) Minutes(target, Number.Decimal);
				if (string.Compare("MR", c, true) == 0) Minutes(target, Number.Roman);
				if (string.Compare("MC", c, true) == 0) MinutesCollapse(target);
				if (string.Compare("DHMS", c, true) == 0) DaysHoursMinutesSeconds(target);
				Console.ReadLine();
			}
			catch
			{
			}
			finally
			{
				Console.CursorVisible = true;
			}
		}

		static void BuildItems(ref List<Item> items, ref int sum)
		{
			items = new List<Item>();
			sum = 0;
			for (int i = 0; i < size; i++)
			{
				items.Add(new Item(i, 35));
				items[i].Write();
				sum += items[i].counter;
			}
		}

		static void Caption(int timeLeft, string suffix, int sum)
		{
			string caption;
			caption = string.Format("{0:#,##0} {1}", timeLeft, suffix);
			//if (sum > timeLeft)
			//    caption = string.Format("{0:#,##0} {1} ({2})", timeLeft, suffix, sum);
			//else if (timeLeft > sum)
			//    caption = string.Format("-{0:#,##0} {1}", timeLeft - sum, suffix);
			//else
			//    caption = string.Format("{0:#,##0} {1}", timeLeft, suffix);
			if (caption != Console.Title)
				Console.Title = caption;
		}

		static void DaysHoursMinutesSeconds(DateTime target)
		{
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			Console.Clear();
			while (timeLeft >= 0)
			{
				if (timeLeft != lastSeconds)
				{
					int days = timeLeft / 86400;
					timeLeft -= (days * 86400);
					int hours = timeLeft / 3600;
					timeLeft -= (hours * 3600);
					int minutes = timeLeft / 60;
					timeLeft -= (minutes * 60);
					int seconds = timeLeft;
					Console.SetCursorPosition(0, 0);
					if (days > 0)
						Console.Title = string.Format("{0,1:#,###} Days {1}:{2,2:00}:{3,2:00}", days, hours, minutes, seconds);
					else if (hours > 0)
						Console.Title = string.Format("{0}:{1,2:00}:{2,2:00}", hours, minutes, seconds);
					else if (minutes > 0)
						Console.Title = string.Format("{0}:{1,2:00}", minutes, seconds);
					else if (seconds > 0)
						Console.Title = string.Format("{0}S", seconds);
					else
						Console.Title = "";
					double ds = target.Subtract(DateTime.Now).TotalSeconds;
					double dm = ds / 60;
					double dh = ds / 3600;
					double dd = ds / 86400;
					Console.WriteLine(string.Format("   Days: {0:#,##0.0000}", dd).PadRight(40));
					Console.WriteLine(string.Format("  Hours: {0:#,##0.000}", dh).PadRight(40));
					Console.WriteLine(string.Format("Minutes: {0:#,##0.00}", dm).PadRight(40));
					Console.WriteLine(string.Format("Seconds: {0:#,##0.0}", ds).PadRight(40));
					lastSeconds = seconds;
				}
				Wait(100);
				timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			}
		}

		static void WaterfallStatic(DateTime target)
		{
			Console.ReadLine();
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			Console.Clear();
			List<Item> items = null;
			int sum = 0;
			int interval = 0;
			BuildItems(ref items, ref sum);
			while (timeLeft >= 0)
			{
				interval = 100;
				Caption(timeLeft, "sec", sum);
				if (sum > timeLeft)
				{
					int i = r.Next(items.Count);
					items[i].counter--;
					items[i].Write();
					if (items[i].counter == 0)
						items.RemoveAt(i);
					sum--;
					interval = 5;
				}
				Wait(interval);
				timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			}
		}

		static void MinutesCollapse(DateTime target)
		{
			Console.ReadLine();
			int Width = Console.WindowWidth;
			int Height = Console.WindowHeight;
			string chars = " 123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalMinutes;
			int zero = -1;
			Console.Clear();
			List<int> items = new List<int>();
			int sum = 0;
			int interval = 0;
			for (int i = 0; i < size; i++)
			{
				items.Add(35);
				Console.SetCursorPosition(i % Width, i / Width);
				Console.Write(chars[items[i]]);
				sum += items[i];
			}
			while (timeLeft >= 0)
			{
				interval = 100;
				Caption(timeLeft, "min", sum);
				if (sum > timeLeft)
				{
					if (zero >= 0)
					{
						Console.SetCursorPosition(zero % Width, zero / Width);
						for (int j = zero; j < items.Count; j++)
							Console.Write(chars[items[j]]);
						Console.Write(' ');
						zero = -1;
					}
					int i = r.Next(items.Count);
					items[i]--;
					Console.SetCursorPosition(i % Width, i / Width);
					Console.Write(chars[items[i]]);
					if (items[i] == 0)
					{
						zero = i;
						items.RemoveAt(i);
					}
					sum--;
					interval = 5;
				}
				Wait(interval);
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Q)
						break;
				}
				timeLeft = (int)target.Subtract(DateTime.Now).TotalMinutes;
			}
		}

		static void WaterfallCollapse(DateTime target)
		{
			Console.ReadLine();
			int Width = Console.WindowWidth;
			int Height = Console.WindowHeight;
			string chars = " 123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			int zero = -1;
			Console.Clear();
			List<int> items = new List<int>();
			int sum = 0;
			int interval = 0;
			for (int i = 0; i < size; i++)
			{
				items.Add(35);
				Console.SetCursorPosition(i % Width, i / Width);
				Console.Write(chars[items[i]]);
				sum += items[i];
			}
			while (timeLeft >= 0)
			{
				interval = 100;
				Caption(timeLeft, "sec", sum);
				if (sum > timeLeft)
				{
					if (zero >= 0)
					{
						Console.SetCursorPosition(zero % Width, zero / Width);
						for (int j = zero; j < items.Count; j++)
							Console.Write(chars[items[j]]);
						Console.Write(' ');
						zero = -1;
					}
					int i = r.Next(items.Count);
					items[i]--;
					Console.SetCursorPosition(i % Width, i / Width);
					Console.Write(chars[items[i]]);
					if (items[i] == 0)
					{
						zero = i;
						items.RemoveAt(i);
					}
					sum--;
					interval = 5;
				}
				Wait(interval);
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Q)
						break;
				}
				timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			}
		}

		static void WaterfallLadder(DateTime target)
		{
			Console.ReadLine();
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			int interval = 25;
			Console.Clear();
			List<Item> items = null;
			int sum = 0;
			int wait = 0;
			BuildItems(ref items, ref sum);
			int j = items.Count - 1;
			int k = 0;
			while (timeLeft >= 0)
			{
				wait = 100;
				Caption(timeLeft, "sec", sum);
				if (sum > timeLeft)
				{
					items[j + k].counter--;
					items[j + k].Write();
					if (items[j + k].counter == 0)
						items.RemoveAt(j + k);
					sum--;
					wait = 5;
					k += interval;
					if ((j + k) > (items.Count - 1))
					{
						k = 0;
						j--;
						if (j < 0) j = interval - 1;
					}
				}
				Wait(wait);
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Q)
						break;
				}
				timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			}
		}

		static void Wait(int interval)
		{
			Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
			Thread.Sleep(interval);
		}

		static void Seconds(DateTime target, Number format)
		{
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			while (timeLeft >= 0)
			{
				string caption =
					(format == Number.Roman) ?
					string.Format("{0}", ToRoman(timeLeft)) :
					string.Format("{0:#,##0}", timeLeft);
				if (caption != Console.Title)
					Console.Title = caption;
				Thread.Sleep(100);
				timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			}
			Console.Title = "";
		}

		static void Binary(DateTime target)
		{
			int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			while (timeLeft >= 0)
			{
				string caption = ToBinary(timeLeft);
				if (caption != Console.Title)
				{
					Console.Title = caption;
					Console.WriteLine(caption);
				}
				Thread.Sleep(100);
				timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
			}
		}

		static void Minutes(DateTime target, Number format)
		{
			string caption, display;
			double timeLeft = target.Subtract(DateTime.Now).TotalMinutes;
			string lastDisplay = "";
			while (timeLeft >= 0)
			{
				if (format == Number.Roman)
					caption = string.Format("{0}", ToRoman((int)Math.Ceiling(timeLeft)));
				else
					caption = string.Format("{0:#,##0.0}", timeLeft);
				if (caption != Console.Title)
					Console.Title = caption;
				display = "".PadRight((int)Math.Ceiling(timeLeft), 'X');
				if (display != lastDisplay)
				{
					Console.Clear();
					Console.Write(display);
					lastDisplay = display;
				}
				Thread.Sleep(100);
				if (Console.KeyAvailable)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Q)
						break;
				}
				timeLeft = target.Subtract(DateTime.Now).TotalMinutes;
			}
			Console.Clear();
			Console.Title = "";
		}

		public static string ToRoman(int i)
		{
			object[,] groups = new object[,] {
                {1000,"M"},{900,"CM"},{500,"D"},{400,"CD"},{100,"C"},{90, "XC"},{50, "L"},
                {40, "XL"},{10, "X"},{9, "IX"},{5, "V"},{4, "IV"},{1, "I"}};
			StringBuilder roman = new StringBuilder();
			while (i > 0)
			{
				for (int j = 0; j < groups.Length; j++)
				{
					if ((int)groups[j, 0] <= i)
					{
						roman.Append((string)groups[j, 1]);
						i -= (int)groups[j, 0];
						break;
					}
				}
			}
			return roman.ToString();
		}

		static string ToBinary(int seconds)
		{
			StringBuilder s = new StringBuilder();
			while (seconds > 0)
			{
				s.Insert(0, string.Format("{0}", seconds % 2));
				seconds /= 2;
			}
			return s.ToString();
		}
	}

	class Item
	{
		public int counter, left, top;
		private const string chars = " 123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public Item(int i, int counter)
		{
			this.left = i % Console.WindowWidth;
			this.top = i / Console.WindowWidth;
			this.counter = counter;
		}
		public void Write()
		{
			Console.SetCursorPosition(left, top);
			Console.Write(chars.Substring(counter, 1));
		}
	}
}

