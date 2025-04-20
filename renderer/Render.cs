namespace SuperPet.Renderer;

using SuperPet.Model;
using SuperPet.Model.State;
using System.Text;

public static class Render
{ 
	public static ConsoleColor green = ConsoleColor.Green;
	public static ConsoleColor blue = ConsoleColor.Blue;
	public static ConsoleColor red = ConsoleColor.Red;
	public static ConsoleColor yellow = ConsoleColor.Yellow;
	public static ConsoleColor frame = ConsoleColor.Cyan;
	
	// Day night cycle
	private static bool IsNightTime => DateTime.Now.Hour >= 20 || DateTime.Now.Hour < 6;

	// Frame control.
	private static int _indexFrame = 0;
	private static bool _indexUp = true;

	// Dev mode enable.
	private static readonly bool _devMode = false;

	// Box drawing characters
	private const char BOX_TL = '╔';
	private const char BOX_TR = '╗';
	private const char BOX_BL = '╚';
	private const char BOX_BR = '╝';
	private const char BOX_H = '═';
	private const char BOX_V = '║';
	private const char BOX_VR = '╠';
	private const char BOX_VL = '╣';
	private const char BOX_HU = '╩';
	private const char BOX_HD = '╦';
	private const char BOX_C = '╬';

	//Screen statics
	private static int screenWidth;
	private static int screenHeight;
	private static int centerX;
	private static int petBoxWidth = 50;
	private static int petBoxHeight = 15;

	private static void InitConsole()
	{
		try
		{
			Console.WindowWidth = Math.Min(120, Console.LargestWindowWidth);
			Console.WindowHeight = Math.Min(40, Console.LargestWindowHeight);
			Console.CursorVisible = false;
		}
		catch
		{
			// Do nothing if above commands won't run on current OS
		}

		screenWidth = Console.WindowWidth;
		screenHeight = Console.WindowHeight;
		centerX = screenWidth / 2;

		petBoxWidth = Math.Min(60, screenWidth - 4);
		petBoxHeight = Math.Min(20, screenHeight - 10);
	}

	private static void DrawBox(int x, int y, int width, int height, string title = "")
	{
		ConsoleColor start = Console.ForegroundColor;
		Console.ForegroundColor = frame;

		Console.SetCursorPosition(x,y);
		Console.Write(BOX_TL);

		// TOP
		if(!string.IsNullOrEmpty(title))
		{
			int titlePos = (width - title.Length - 2) / 2;
			for (int i = 1; i < titlePos; i++)
				Console.Write(BOX_H);

			Console.Write(' ');
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(title);
			Console.ForegroundColor = frame;
			Console.Write(' ');

			for(int i = titlePos + title.Length + 2; i < width; i++)
				Console.Write(BOX_H);
		}
		else
		{
			for (int i = 1; i < width; i++)
				Console.Write(BOX_H);
		}

		Console.Write(BOX_TR);

		// SIDES
		for (int i = 1; i < height; i++)
		{
			Console.SetCursorPosition(x,y+i);
			Console.Write(BOX_V);
			Console.SetCursorPosition(x + width, y + i);
			Console.Write(BOX_V);
		}

		// BOTTOM
		Console.SetCursorPosition(x,y+height);
		Console.Write(BOX_BL);
		for(int i = 1; i < width; i++)
			Console.Write(BOX_H);
		Console.Write(BOX_BR);

		Console.ForegroundColor = start;

	}

	private static void DrawHorizontalSeparator(int x, int y, int width)
	{
		ConsoleColor start = Console.ForegroundColor;
		Console.ForegroundColor = frame;

		Console.SetCursorPosition(x,y);
		Console.Write(BOX_VR);
		for(int i = 1; i < width - 1; i++)
			Console.Write(BOX_H);
		Console.Write(BOX_VL);

		Console.ForegroundColor = start;
	}

	public static void ShowCommands(int y)
	{
		string commands = "Keys: [F]eed [T]hrow ball [R]est [S]tatus e[X]it";

		Console.SetCursorPosition(centerX - commands.Length / 2, y);
		Console.Write(commands);
	}

	public static void Screen(Pet p, int f)
	{
		InitConsole();
		Console.Clear();

		string timeIcon = IsNightTime ? "🌙" : "☀️";

		int petBoxX = centerX - petBoxWidth / 2;
		int petBoxY = 3;
		int statsBoxWidth = petBoxWidth;
		int statsBoxHeight = 6;
		int statsBoxX = petBoxX;
		int statsBoxY = petBoxY + petBoxHeight + 1;
		
		// Main pet box.
		DrawBox(petBoxX, petBoxY, petBoxWidth, petBoxHeight, $" {p.Name} ");

		// Stats box.
		DrawBox(statsBoxX, statsBoxY, statsBoxWidth, statsBoxHeight, " Stats ");
		
		// Now draw things in each box using SetCursor.
		
		// Get pet appearance.
		var petAppearance = new PetAppearance(p);
		string[] petArt = petAppearance.RenderPet(p, f);
		int stageOffset = petAppearance.GetStageOffset();

		// Find center.
		int petCenterX = petBoxX + petBoxWidth / 2;
		int petCenterY = petBoxY + petBoxHeight  / 2;
		
		// Status animation. 
		StatusAnimation(p, f, petCenterX, petBoxY + petCenterY - petArt.Length - stageOffset);

		// Draw pet art.
		PetAnimation(p, petArt, petCenterX, petCenterY, f);

		// Draw stats in stat box.
		DisplayStats(p, statsBoxX + 7, statsBoxY + 1);
		
		// Draw commands at screen bottom.
		ShowCommands(statsBoxHeight + statsBoxY + 1);

		// Draw time and time icon.
		string timeDisplay = $"{DateTime.Now:HH:mm} {timeIcon}";
		Console.SetCursorPosition(petBoxX + petBoxWidth - timeDisplay.Length - 1, petBoxY + 1);
		Console.Write(timeDisplay);
	
		// Draw age of pet.
		string ageDisplay = $"Age: {p.Age} days [{p.CurrentStage}]";
		Console.SetCursorPosition(petBoxX + 2, petBoxY + 1);
		Console.Write(ageDisplay);

		// Draw weight of pet.
		string weightDisplay = $"Weight: {p.Weight:F1}kg";
		Console.SetCursorPosition(petBoxX + 2, petBoxY + 2);
		Console.Write(weightDisplay);

		// Display dev stats if devmode is enabled.
		if(_devMode)
			DevOutput(p);
	}

	private static void PetAnimation(Pet p, string[] petArt, int centerX, int centerY, int frame)
	{

		// Starting Pos.
		int startX = centerX - (petArt[0].Length / 2);
		int startY = centerY - petArt.Length / 2;
		int offsetX = 0;

		// Pet moves when not sleeping.
		if(p.CurrentState is not SleepingState)
		{
			if(frame % 2 == 0)
			{
				if(_indexUp)
				{
					_indexFrame++;
					if(_indexFrame == 5)
						_indexUp = false;
				}
				else
				{
					_indexFrame--;
					if(_indexFrame == 0)
						_indexUp = true;
				}
			}
		}

		offsetX = _indexFrame;

		// Draw each line of the pet:
		for(int i = 0; i < petArt.Length; i++)
		{
			Console.SetCursorPosition(startX + offsetX, startY + i);
			ColorWrite(petArt[i], p.Color);
		}
	}

	private static void DisplayStats(Pet p, int x, int y)
	{
		Console.SetCursorPosition(x,y+1);
		Console.Write("Hunger:   ");
		ColorWrite(DisplayStat(p.Hunger), green);
		
		Console.SetCursorPosition(x,y+2);
		Console.Write("Energy:   ");
		ColorWrite(DisplayStat(p.Energy), blue);
		
		Console.SetCursorPosition(x,y+3);
		Console.Write("Happiness:");
		ColorWrite(DisplayStat(p.Happiness), red);
	}

	public static void StatusAnimation(Pet p, int frame, int x, int y)
	{
		string[] frames = p.CurrentState.GetFrames();

		int frameIndex = (frame + 1) % frames.Length;

		Console.SetCursorPosition(x,y);
		ColorWrite(frames[frameIndex], ConsoleColor.White);
	}

	public static string DisplayStat(double stat)
	{
		stat = Math.Max(0, Math.Min(100, stat));
		
		int barWidth = 30;
		int filledChars = (int)(stat / 100 * barWidth);

		var sb = new StringBuilder();
		sb.Append(" ");

		for(int i = 0; i < barWidth; i++)
		{
			if (i < filledChars)
				sb.Append("█");
			else
				sb.Append("░");
		}

		sb.Append($" {stat:F0}%");

		return sb.ToString();
	}

	public static void ColorWrite(string s, ConsoleColor c)
	{
		ConsoleColor start = Console.ForegroundColor;
		
		Console.ForegroundColor = c;
		Console.Write(s);
		Console.ForegroundColor = start;
	}

	public static void ShowSaveIndicator()
	{
		int x = centerX - 3;
		int y = 1;

		int left = Console.CursorLeft;
		int top = Console.CursorTop;

		Console.SetCursorPosition(x,y);
		ColorWrite("[SAVED]", ConsoleColor.Cyan);

		var timer = new System.Timers.Timer(1500);
		timer.Elapsed += (s, e) => {
			Console.SetCursorPosition(Console.WindowWidth - 15, 1);
			Console.Write("       ");
			timer.Dispose();
		};
		timer.AutoReset = false;
		timer.Start();

		Console.SetCursorPosition(left, top);
	}

	public static void DisplayMessage(string message, int duration = 1500)
	{
		int messageX = centerX - message.Length / 2;
		int messageY = screenHeight - 5;

		int left = Console.CursorLeft;
		int top = Console.CursorTop;

		Console.SetCursorPosition(messageX, messageY);
		Console.Write(message);
		Thread.Sleep(duration);

		Console.SetCursorPosition(messageX, messageY);
		Console.Write(new string(' ', message.Length));

		Console.SetCursorPosition(left, top);
	}

	public static void DevOutput(Pet p)
	{
		Console.SetCursorPosition(1, screenHeight - 4);
		Console.WriteLine($"DEBUG - H:{p.Hunger:F2} E:{p.Energy:F2} H:{p.Happiness:F2}");
	}
}
