/*
 * KJV_Bible_Revelation_Tachistoscope.cs
 * in project KJV_Bible_Revelation_Tachistoscope
 * This file has the whole brain work to do the job.
 * It was added to a Visual Studio 2019, new project, Windows Form Application.
 */
 
/*
 * Usage:
 * Execute KJV_Bible_Revelation_Tachistoscope.exe on a Windows computer.
 * Text of the KJV Book of Revelation should appear just above the dock.
 * From time to time, it will jump to a new random location in the text.
 * To exit app, click visible text, or press any key when app has focus.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

/* this is practically a one-file program,
 * just need to add this into the WinForm:
 * 			MainFormFinishConstructor();
 */

namespace KJV_Bible_Revelation_Tachistoscope
{
    public partial class Form1 : Form
	{
		static float fontHeight = 14.0f;
		static TextBox tb = new TextBox();
		static Timer repaintTimer = new Timer();
		static Random rand = new Random();
		static Regex vcolonv = new Regex(@"^\d+:\d+ ");
		static Regex atEos = new Regex(@"([,:;.?!)]\)?)"); // allow any ) after other puncts, or alone

		void MainFormFinishConstructor()
		{
			this.TopMost = true;
			this.ControlBox = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Font = new Font(FontFamily.GenericSerif, fontHeight);
			this.Height = (int)this.Font.GetHeight(this.CreateGraphics()) + 1;
			this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
			this.Left = 0;
			this.Width = Screen.PrimaryScreen.WorkingArea.Width;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = FormStartPosition.Manual;
			this.TransparencyKey = Color.Lime;
			this.BackColor = Color.Lime;

			this.ShowInTaskbar = false;
			
			this.Load += new EventHandler(Form1_Load);
		}
		
		private void Form1_Load(object sender, EventArgs e)
		{
			try
			{
				int RandomFactors = Environment.TickCount;
				RandomFactors <<= 3;
				RandomFactors ^= System.DateTime.Now.DayOfYear;
				RandomFactors <<= 3;
				RandomFactors ^= System.DateTime.Now.Hour;
				RandomFactors <<= 3;
				RandomFactors ^= System.DateTime.Now.Minute;
				RandomFactors <<= 3;
				RandomFactors ^= System.DateTime.Now.Millisecond;
				rand = new Random(RandomFactors);
				
				this.WindowState = FormWindowState.Normal;
				this.Controls.Add(tb);
				tb.BorderStyle = BorderStyle.None;
				tb.Top = 0;
				tb.Left = 0;
				tb.Height = ClientRectangle.Height;
				tb.Width = ClientRectangle.Width;
				tb.Multiline = true;
				tb.AcceptsReturn = false;
				tb.TextAlign = HorizontalAlignment.Center;
				tb.BackColor = Color.Lime;
				tb.ForeColor = Color.Black;
				tb.Font = this.Font;
				tb.Text = "KJV_Bible_Revelation_Tachistoscope";
				tb.Select(0, 0);
				tb.Click += new EventHandler(tb_Click);
				tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);

				repaintTimer.Tick += new EventHandler(repaintTimer_Tick);
				repaintTimer.Interval = 1000;
				repaintTimer.Start();
				
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Form1_Load exception", MessageBoxButtons.OK);
				Application.Exit();
			}
			
		}

		void tb_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		void tb_KeyPress(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		static int nowShowing = -1;
		static string justShown = "";

		void repaintTimer_Tick(object sender, EventArgs e)
		{
			repaintTimer.Stop();
			int msToShow = 1000; // default
			
			try
			{
				// Random starting location
				if(nowShowing == -1)
					nowShowing = rand.Next(KjvArray.bitesize.Length);
				if(++nowShowing >= KjvArray.bitesize.Length)
					nowShowing = 0;
				// Occasionally, at end of sentences, change location
				if(rand.Next(1000) < 10
				   && justShown.EndsWith(".")
				   && ! KjvArray.bitesize[nowShowing].StartsWith(" And", StringComparison.OrdinalIgnoreCase)
				  )
				{
					nowShowing = rand.Next(KjvArray.bitesize.Length);
					justShown = " ... ";
					tb.Text = " " + justShown; // one space away from cursor
					msToShow = 800;
				}
				else
				{
					justShown = KjvArray.bitesize[nowShowing];
					tb.Text = " " + justShown; // one space away from cursor
					msToShow = 150 + justShown.Length * 40;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButtons.OK);
			}
			
			repaintTimer.Interval = msToShow;
			repaintTimer.Start();
		}


		//static void ComputePhrasing()
		//{
		//	List<string>Phrases = new List<string>();
		//	foreach(string line in KjvArray.KjvLines)
		//	{
		//		if(line.StartsWith("=="))
		//			continue;
		//		string s = vcolonv.Replace(line, "");
		//		string [] sa = atEos.Split(s);
		//		string held = "";
		//		foreach(string p in sa)
		//		{
		//			if(p.Length <= 2 && atEos.IsMatch(p))
		//			{
		//				held += p;
		//			}
		//			else
		//			{
		//				if(held != "")
		//				{
		//					// Longest phrase is about 120.
		//					// Split anything over 80 in 2.
		//					if(held.Length > 80)
		//					{
		//						for(int i = held.Length/2; i< held.Length; i++)
		//						{
		//							if(held[i] == ' ')
		//							{
		//								Phrases.Add(held.Substring(0, i));
		//								held = held.Substring(i); // keep the initial space
		//							}
		//						}
		//					}
		//					Phrases.Add(held);
		//					held = "";
		//				}
		//				held = p;
		//			}
		//		}
		//		if(held != "")
		//		{
		//			// Longest phrase is about 120.
		//			// Split anything over 80 in 2.
		//			if(held.Length > 80)
		//			{
		//				for(int i = held.Length/2; i< held.Length; i++)
		//				{
		//					if(held[i] == ' ')
		//					{
		//						Phrases.Add(held.Substring(0, i));
		//						held = held.Substring(i); // keep the initial space
		//					}
		//				}
		//			}
		//			Phrases.Add(held);
		//			held = "";
		//		}
		//	}
		//	bitesize = Phrases.ToArray();
		//	File.WriteAllLines(@"C:\a\ai.txt", bitesize);
		//}

		
	}
}
