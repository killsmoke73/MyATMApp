using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.UI
{
	public static class Utility
	{
		private static long tranId;
		private static CultureInfo culture = new CultureInfo("IG-NG");

		public static long GetTransactionId()
		{
			return ++tranId;
		}
		public static string GetSecretInput(string prompt)
		{
			bool isprompt = true;
			string asterics = "";

			StringBuilder input = new StringBuilder();

			while (true)
			{
				if (isprompt)
					Console.WriteLine(prompt);
				isprompt = false;

				ConsoleKeyInfo InputKey = Console.ReadKey(true);

				if (InputKey.Key == ConsoleKey.Enter)
				{
					if (input.Length == 6)
					{
						break;
					}
					else
					{
						printMessage("\nPlease enter only 6 digits.", false);
						isprompt = true;
						input.Clear();
						continue;
					}
				}
				if (InputKey.Key == ConsoleKey.Backspace && input.Length > 0)
				{
					input.Remove(input.Length - 1, 1);
				}
				else if (InputKey.Key != ConsoleKey.Backspace)
				{
					input.Append(InputKey.KeyChar);
					Console.Write(asterics + "*");
				}
			}
			return input.ToString();
		}
		public static void printMessage(string msg, bool success)
		{
			if (success)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
			}
			Console.WriteLine(msg);
			Console.ForegroundColor = ConsoleColor.Yellow;
			PressEnterToContinue();

		}
		public static string GetUserInput(string prompt)
		{
			Console.WriteLine($"Enter {prompt}");
			return Console.ReadLine();
		}

		public static void PrintDotAnimaton(int timer = 10)
		{
			for (int i = 0; i < timer; i++)
			{
				Console.Write(".");
				Thread.Sleep(200);
			}
			Console.Clear();
		}
		public static void PressEnterToContinue()
		{
			Console.WriteLine("\n\nPress Enter to continue....\n\n");
			Console.ReadLine();
		}

		public static string FormatAmount(double amt)
		{
			return amt.ToString("C3");
		}

		
		
		
	}
}
