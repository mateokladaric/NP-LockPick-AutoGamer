using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NOPixelLockPickSolver
{
	internal class Program
	{
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern short GetAsyncKeyState(Keys vKey);

		static void Main(string[] args)
		{
			Task Solver = new Task(() =>
			{
				Point LP = new Point(960, 520);
				Bitmap lpimg = new Bitmap(380, 380);
				List<int> colorCircles = new List<int>();
				List<int> colorMatches = new List<int>();

				while (true)
				{
					if (GetAsyncKeyState(Keys.F1) != 0)
					{
						break;
					}
					if (GetAsyncKeyState(Keys.XButton2) != 0)
					{
						using (Graphics g = Graphics.FromImage(lpimg))
						{
							g.CopyFromScreen(LP.X - 190, LP.Y - 190, 0, 0, lpimg.Size);
						}
						for (int numOfTimes = 0; numOfTimes < 4; numOfTimes++)
						{
							colorCircles.Clear();
							colorMatches.Clear();
							int newOffsets = (numOfTimes * 40);
							for (int i = 0; i < 12; i++)
							{
								double newCos = Math.Cos(i * 30 * Math.PI / 180);
								double newSin = Math.Sin(i * 30 * Math.PI / 180);
								colorCircles.Add(GetColorAt(lpimg, new Point(LP.X + (int)(newOffsets * newCos + 35 * newCos), LP.Y + (int)(newOffsets * newSin + 35 * newSin))));
							}
							for (int i = 0; i < 12; i++)
							{
								double newCos = Math.Cos(i * 30 * Math.PI / 180);
								double newSin = Math.Sin(i * 30 * Math.PI / 180);
								colorMatches.Add(GetColorAt(lpimg, new Point(LP.X + (int)(newOffsets * newCos + 54 * newCos), LP.Y + (int)(newOffsets * newSin + 54 * newSin))));
							}
							List<string> moveSet = SolveMatch(colorCircles, colorMatches);
							foreach (string move in moveSet)
							{
								if (move == "L")
								{
									SendKeys.SendWait("{LEFT}");
								}
								else if (move == "R")
								{
									SendKeys.SendWait("{RIGHT}");
								}
								System.Threading.Thread.Sleep(50);
							}
							SendKeys.SendWait("{SPACE}");
							System.Threading.Thread.Sleep(50);
						}
						System.Threading.Thread.Sleep(300);
					}
				}
			});
			Solver.Start();
			Solver.Wait();
		}

		private static List<string> SolveMatch(List<int> colorCircles, List<int> colorMatches)
		{
			int n = colorCircles.Count;

			if (colorCircles.SequenceEqual(colorMatches))
			{
				return new List<string>();
			}

			int min_shifts = int.MaxValue;
			List<string> min_directions = new List<string>();

			for (int direction = 0; direction < 2; direction++)
			{
				var shiftedCircles = new List<int>(colorCircles);

				for (int i = 0; i < n; i++)
				{
					if (direction == 0)
					{
						shiftedCircles.Add(shiftedCircles[0]);
						shiftedCircles.RemoveAt(0);
					}
					else
					{
						shiftedCircles.Insert(0, shiftedCircles[n - 1]);
						shiftedCircles.RemoveAt(n);
					}

					int num_matches = 0;
					for (int j = 0; j < n; j++)
					{
						if (shiftedCircles[j] == colorMatches[j] || colorMatches[j] == 0)
						{
							num_matches++;
						}
					}

					if (num_matches == n)
					{
						if (i < min_shifts)
						{
							min_shifts = i;
							min_directions = new List<string>();
							for (int k = 0; k < i + 1; k++)
							{
								min_directions.Add(direction == 0 ? "L" : "R");
							}
						}
						break;
					}
				}
			}
			return min_directions;
		}

		private static int GetColorAt(Bitmap clp, Point point)
		{
			Color color = clp.GetPixel(point.X - 770, point.Y - 330);

			if (color.R > 90 && color.G > 120 && color.B < 120)
			{
				return 1; // yellow
			}
			else if (color.R > 100 && color.G < 90 && color.B > 45)
			{
				return 2; // purple
			}
			else if (color.R < 100 && color.G > 120 && color.B > 200)
			{
				return 3; // blue
			}
			else
			{
				clp.SetPixel(point.X - 770, point.Y - 330, Color.White);
				return 0;
			}
		}
	}
}