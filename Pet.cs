namespace SuperPet.Model;

using SuperPet.Renderer;
using SuperPet.Model.State;

// Possible growth stages of pet (used by pet appearance class).
public enum GrowthStage
{
	Egg,		// First 2 hours.
	Baby,		// First 2 days.
	Child,		// 3-6 days.
	Teen,		// 7-14 days.
	Adult,		// 15-30 days.
	Elder,		// 31+ days.
}

public class Pet
{
	// Name and Appearance.
	public string Name { get; set; } = default!;
	public ConsoleColor Color { get; set; } = default!;
	public string Nose { get; set; }
	public GrowthStage CurrentStage => GetGrowthStage();
	private PetAppearance _appearance;
	
	// Age calculations.
	public DateTime Birthday { get; set; }
	public int Age => (DateTime.Now - Birthday).Days;
	public GrowthStage Stage { get; set; } = default!;

	// Main stat props.
	public double Hunger { get; set; }
	public double Energy { get; set; }
	public double Happiness { get; set; }
	public double Weight { get; set; }

	// Last save for save system
	public DateTime LastSave { get; set; }
	
	// State reference.
	public PetState CurrentState { get; set; } = default!;

	// Speed of stat decay.
	// This was originally fast for testing, but for actual play 0.05 is a good modifier.
    	public static double DecayRateMultiplier { get; } = 0.05;
	
	// Internal prop for managing stat decay.
	private DateTime _lastUpdate;

	// Rng instance for random elements.
	private static Random rng = new Random();

	public Pet()
	{
		// Build pet with player input.
		Name 		= GetName();
		Nose 		= GetNose();
		Color 		= GetColor();
		_appearance 	= new PetAppearance(this);

		// Default game start stats.
		Birthday = DateTime.Now;
		Hunger = 50;
		Energy = 50;
		Happiness = 50;
		Weight = 1.0;   // kg

		// Pet starts happy.
		// Last update was right now as pet was just created.
		CurrentState = new HappyState();
		_lastUpdate = DateTime.Now;
	}

	public Pet(bool loadSave)
	{
		// All values init to default, load save [should] overwrite them.
		Name = "";
		Nose = "";
		Color = ConsoleColor.White;
		_appearance = default!;
		Hunger = 0;
		Energy = 0;
		Happiness = 0;
		Weight = 0;
		CurrentState = new HappyState();
		_lastUpdate = DateTime.Now;
	}

	private GrowthStage GetGrowthStage()
	{
		if (this.Age < 3)
			return GrowthStage.Baby;
		else if (this.Age < 7)
			return GrowthStage.Child;
		else if (this.Age < 15)
			return GrowthStage.Teen;
		else if (this.Age < 31)
			return GrowthStage.Adult;
		else
			return GrowthStage.Elder;
	}

	public string[] GetArt()
	{
		if(_appearance == null)
			_appearance = new PetAppearance(this);
		
		return _appearance.RenderPet(this);
	}

	// Helper methods to manage player input of pet creation.
	private string GetName()
	{
		string? input = default!;
		
		while(input == null)
		{
			Console.Clear();
			Console.Write("What would you like your pet to be called? ");
			input = Console.ReadLine()!;

			if(input != null)
			{
				return input;
			}
		}

		return input;

	}

	private string GetNose()
	{
		string? input = default!;
		int result;
		string[] noses = new string[]
		{
			"ᴥ",	// 0
			"︿",	// 1
			"ω",	// 2
			"_",	// 3
			"ᴗ",	// 4
			"ɞ",	// 5
			"⌄",	// 6
			".",	// 7
		};
		
		while(true)
		{
			Console.Clear();
			Console.WriteLine("What kind of pet would you like? ");
			Console.WriteLine("1 | Playful");
			Console.WriteLine("2 | Timid");
			Console.WriteLine("3 | Mischievous");
			Console.WriteLine("4 | Stoic");
			Console.WriteLine("5 | Happy");
			Console.WriteLine("6 | Shy");
			Console.WriteLine("7 | Flirty");
			Console.WriteLine("8 | Cold");
			Console.WriteLine();
			Console.WriteLine("9 | Random");
			
			input = Console.ReadLine()!;

			if(int.TryParse(input, out result))
				if(result >= 1 && result <= 9)
					break;

			Console.WriteLine("Please input a valid number");
			Thread.Sleep(1000);
		}

		if(result == 9)
			return noses[rng.Next(0,8)];
		
		return noses[result-1];
	}

	public ConsoleColor GetColor()
	{
		ConsoleColor[] colors = new ConsoleColor[]
		{
			ConsoleColor.White,		// 0
			ConsoleColor.Cyan,		// 1
			ConsoleColor.Yellow,		// 2
			ConsoleColor.Magenta,		// 3
			ConsoleColor.Green,		// 4
			ConsoleColor.Red,		// 5
			ConsoleColor.Blue,		// 6
			ConsoleColor.Gray,		// 7
			ConsoleColor.DarkGreen,		// 8
			ConsoleColor.DarkCyan,		// 9
			ConsoleColor.DarkMagenta,	// 10
		};

		return colors[rng.Next(0,11)];
	}

	// Main pet gameplay methods.
	public void Update()
	{
		DateTime now = DateTime.Now;
		TimeSpan elapsed = now - _lastUpdate;
		double elapsedSeconds = elapsed.TotalSeconds;

		CurrentState.Update(this, elapsedSeconds);

		_lastUpdate = now;
	}

	public void Feed()
	{
		CurrentState.Feed(this);
	} 

	public void Play()
	{
		CurrentState.Play(this);
	}

	public void Sleep() 
	{
		CurrentState.Sleep(this);
	}

	public string GetStatus()
	{
		return CurrentState.GetStatus();
	}

	public string[] GetEyes()
	{
		return CurrentState.GetEyes();
	}

	public void UpdateStateBasedOnStats()
	{
		if(Hunger < 30 && Hunger <= Energy && Hunger <= Happiness)
		{
			CurrentState = new HungryState();
		}
		else if(Energy < 30 && Energy <= Hunger && Energy <= Happiness)
		{
			CurrentState = new TiredState();
		}
		else if(Happiness < 30 && Happiness <= Hunger && Happiness <= Energy)
		{
			CurrentState = new BoredState();
		}
		else if(Hunger >= 90 && Energy >= 50)
		{
			CurrentState = new ContentState();
		}
		else if(Happiness >= 90 && Energy >= 50)
		{
			CurrentState = new ExcitedState();
		}
		else
		{
			CurrentState = new HappyState();
		}
	}
}
