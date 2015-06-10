using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace timer
{
	class Program
	{
		private enum Number
		{
			Decimal, Roman
		}

		private enum Display
		{
			Seconds, Minutes, Binary, Full
		}

		private static Timer timer;
		private static readonly int size = Console.WindowHeight * Console.WindowWidth - 1;
		private static int lastSeconds = -1;
		private static Random r = new Random(DateTime.Now.Day);
		private static Number format;
		private static DateTime target;
		private static Display display;

		static void Main(string[] args)
		{
			try
			{
				if (args.Length > 0 && args[0] == "?")
				{
					Console.WriteLine("timer <target>");
					Console.WriteLine();
					//Console.WriteLine("WS - Waterfall - Static");
					//Console.WriteLine("WC - Waterfall - Collapsing");
					//Console.WriteLine("WL - Waterfall - Ladder");
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
				target = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
				if (now > target)
					target = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0);
				string c = "SD";
				if (args.Length >= 1) target = DateTime.Parse(args[0]);
				if (args.Length >= 2) c = args[1];
				Console.Clear();
				Console.CursorVisible = false;
				Console.Title = string.Format("{0}: {1}", c, target);
				format = Number.Decimal;
				if (c.Contains("R")) format = Number.Roman;
				display = Display.Full;
				if (c.Contains("S")) display = Display.Seconds;
				if (c.Contains("M")) display = Display.Minutes;
				if (c.Contains("B")) display = Display.Binary;
				using (timer = new Timer(100))
				{
					timer.Elapsed += new ElapsedEventHandler(OnTimer);
					timer.AutoReset = false;
					timer.Enabled = true;
					Console.ReadLine();
					timer.Enabled = false;
					timer.Close();
				}
			}
			catch
			{
			}
			finally
			{
				Console.CursorVisible = true;
			}
		}

		static void OnTimer(object sender, ElapsedEventArgs args)
		{
			timer.Enabled = false;
			DateTime now = DateTime.Now;
			int d, h, m, s;
			string caption = "";
			int timeLeft = (int)Math.Ceiling(target.Subtract(DateTime.Now).TotalSeconds);
			if (timeLeft >= 0)
			{
				switch (display)
				{
					case Display.Seconds:
						s = (int)Math.Ceiling(target.Subtract(now).TotalSeconds);
						caption = (format == Number.Roman) ? string.Format("{0}", ToRoman(s)) : string.Format("{0:#,##0}", s);
						if (caption != Console.Title) Console.Title = caption;
						break;
					case Display.Minutes:
						m = (int)Math.Ceiling(target.Subtract(DateTime.Now).TotalMinutes);
						caption = (format == Number.Roman) ? string.Format("{0}", ToRoman(m)) : string.Format("{0:#,##0}", m);
						if (caption != Console.Title) Console.Title = caption;
						break;
					case Display.Binary:
						s = (int)Math.Ceiling(target.Subtract(now).TotalSeconds);
						caption = ToBinary(s);
						if (caption != Console.Title) { Console.Title = caption; Console.WriteLine(caption); }
						break;
					case Display.Full:
						s = (int)Math.Ceiling(target.Subtract(now).TotalSeconds);
						d = s / 86400;
						s -= (d * 86400);
						h = s / 3600;
						s -= (h * 3600);
						m = s / 60;
						s -= (m * 60);
						caption = 
							(d > 0) ? string.Format("{0,1:#,###} Days {1}:{2,2:00}:{3,2:00}", d, h, m, s) : 
							(h > 0) ? string.Format("{0}:{1,2:00}:{2,2:00}", h, m, s) : 
							(m > 0) ? string.Format("{0}:{1,2:00}", m, s) : 
							string.Format("{0}", s);
						if (caption != Console.Title) Console.Title = caption; 
						Console.WriteLine(string.Format("   Days: {0:#,##0.0000}", target.Subtract(DateTime.Now).TotalDays));
						Console.WriteLine(string.Format("  Hours: {0:#,##0.000}", target.Subtract(DateTime.Now).TotalHours));
						Console.WriteLine(string.Format("Minutes: {0:#,##0.00}", target.Subtract(DateTime.Now).TotalMinutes));
						Console.WriteLine(string.Format("Seconds: {0:#,##0.0}", target.Subtract(DateTime.Now).TotalSeconds));
						break;
				}
				timer.Enabled = true;
			}
		}

		//static void WaterfallStatic()
		//{
		//	Console.ReadLine();
		//	int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
		//	Console.Clear();
		//	List<Item> items = null;
		//	int sum = 0;
		//	int interval = 0;
		//	BuildItems(ref items, ref sum);
		//	while (timeLeft >= 0)
		//	{
		//		interval = 100;
		//		Caption(timeLeft, "sec");
		//		if (sum > timeLeft)
		//		{
		//			int i = r.Next(items.Count);
		//			items[i].counter--;
		//			items[i].Write();
		//			if (items[i].counter == 0)
		//				items.RemoveAt(i);
		//			sum--;
		//			interval = 5;
		//		}
		//		Wait(interval);
		//		timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
		//	}
		//}

		//static void MinutesCollapse()
		//{
		//	Console.ReadLine();
		//	int Width = Console.WindowWidth;
		//	int Height = Console.WindowHeight;
		//	string chars = " 123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		//	int timeLeft = (int)target.Subtract(DateTime.Now).TotalMinutes;
		//	int zero = -1;
		//	Console.Clear();
		//	List<int> items = new List<int>();
		//	int sum = 0;
		//	int interval = 0;
		//	for (int i = 0; i < size; i++)
		//	{
		//		items.Add(35);
		//		Console.SetCursorPosition(i % Width, i / Width);
		//		Console.Write(chars[items[i]]);
		//		sum += items[i];
		//	}
		//	while (timeLeft >= 0)
		//	{
		//		interval = 100;
		//		Caption(timeLeft, "min");
		//		if (sum > timeLeft)
		//		{
		//			if (zero >= 0)
		//			{
		//				Console.SetCursorPosition(zero % Width, zero / Width);
		//				for (int j = zero; j < items.Count; j++)
		//					Console.Write(chars[items[j]]);
		//				Console.Write(' ');
		//				zero = -1;
		//			}
		//			int i = r.Next(items.Count);
		//			items[i]--;
		//			Console.SetCursorPosition(i % Width, i / Width);
		//			Console.Write(chars[items[i]]);
		//			if (items[i] == 0)
		//			{
		//				zero = i;
		//				items.RemoveAt(i);
		//			}
		//			sum--;
		//			interval = 5;
		//		}
		//		Wait(interval);
		//		if (Console.KeyAvailable)
		//		{
		//			ConsoleKeyInfo key = Console.ReadKey(true);
		//			if (key.Key == ConsoleKey.Q)
		//				break;
		//		}
		//		timeLeft = (int)target.Subtract(DateTime.Now).TotalMinutes;
		//	}
		//}

		//static void WaterfallCollapse()
		//{
		//	Console.ReadLine();
		//	int Width = Console.WindowWidth;
		//	int Height = Console.WindowHeight;
		//	string chars = " 123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		//	int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
		//	int zero = -1;
		//	Console.Clear();
		//	List<int> items = new List<int>();
		//	int sum = 0;
		//	int interval = 0;
		//	for (int i = 0; i < size; i++)
		//	{
		//		items.Add(35);
		//		Console.SetCursorPosition(i % Width, i / Width);
		//		Console.Write(chars[items[i]]);
		//		sum += items[i];
		//	}
		//	while (timeLeft >= 0)
		//	{
		//		interval = 100;
		//		Caption(timeLeft, "sec");
		//		if (sum > timeLeft)
		//		{
		//			if (zero >= 0)
		//			{
		//				Console.SetCursorPosition(zero % Width, zero / Width);
		//				for (int j = zero; j < items.Count; j++)
		//					Console.Write(chars[items[j]]);
		//				Console.Write(' ');
		//				zero = -1;
		//			}
		//			int i = r.Next(items.Count);
		//			items[i]--;
		//			Console.SetCursorPosition(i % Width, i / Width);
		//			Console.Write(chars[items[i]]);
		//			if (items[i] == 0)
		//			{
		//				zero = i;
		//				items.RemoveAt(i);
		//			}
		//			sum--;
		//			interval = 5;
		//		}
		//		Wait(interval);
		//		if (Console.KeyAvailable)
		//		{
		//			ConsoleKeyInfo key = Console.ReadKey(true);
		//			if (key.Key == ConsoleKey.Q)
		//				break;
		//		}
		//		timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
		//	}
		//}

		//static void WaterfallLadder()
		//{
		//	Console.ReadLine();
		//	int timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
		//	int interval = 25;
		//	Console.Clear();
		//	List<Item> items = null;
		//	int sum = 0;
		//	int wait = 0;
		//	BuildItems(ref items, ref sum);
		//	int j = items.Count - 1;
		//	int k = 0;
		//	while (timeLeft >= 0)
		//	{
		//		wait = 100;
		//		Caption(timeLeft, "sec");
		//		if (sum > timeLeft)
		//		{
		//			items[j + k].counter--;
		//			items[j + k].Write();
		//			if (items[j + k].counter == 0)
		//				items.RemoveAt(j + k);
		//			sum--;
		//			wait = 5;
		//			k += interval;
		//			if ((j + k) > (items.Count - 1))
		//			{
		//				k = 0;
		//				j--;
		//				if (j < 0) j = interval - 1;
		//			}
		//		}
		//		Wait(wait);
		//		if (Console.KeyAvailable)
		//		{
		//			ConsoleKeyInfo key = Console.ReadKey(true);
		//			if (key.Key == ConsoleKey.Q)
		//				break;
		//		}
		//		timeLeft = (int)target.Subtract(DateTime.Now).TotalSeconds;
		//	}
		//}

		//static void Wait(int interval)
		//{
		//	Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
		//	Thread.Sleep(interval);
		//}

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

		static void Caption(int timeLeft, string suffix)
		{
			string caption;
			caption = string.Format("{0:#,##0} {1}", timeLeft, suffix);
			if (caption != Console.Title)
				Console.Title = caption;
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
			if (seconds == 0)
				return "0";
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

