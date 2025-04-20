namespace SuperPet;

using SuperPet.Model;
using SuperPet.Renderer;
using SuperPet.Persistance;
using System.Timers;

public class Game
{
	// Static instance for Save System
	public static Game Instance { get; private set; } = default!;
	public SaveSystem SaveManager { get; private set; } = new SaveSystem();

	public Pet PlayerPet { get; set; } = default!;

	public Game() 
	{ 
		Instance = this;

		SaveManager.SaveCompleted += (s, e) => {
			Render.ShowSaveIndicator();
		};
	}

	public void Run()
	{
		
		Console.Title = "SuperPet";

		if(SaveManager.HasSaveFile())
		{
			PlayerPet = SaveManager.LoadPet();
		}
		else
		{
			PlayerPet = new Pet();
		}

		while(true)
		{
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			bool keyPress = false;
			int frameIndex = 0;

			while (!keyPress)
			{
				keyPress = Console.KeyAvailable;
				
				// Render every 500ms
				if(stopwatch.ElapsedMilliseconds % 500 < 50)
				{
					Render.Screen(PlayerPet, frameIndex);

					var stateBefore = PlayerPet.CurrentState.GetType();

					PlayerPet.Update();

					bool stateChanged = PlayerPet.CurrentState.GetType() != stateBefore;

					SaveManager.CheckAutoSave(PlayerPet, stateChanged);

					frameIndex++;
				}

				// Prevent CPU hogging.
				Thread.Sleep(50);
			}

			if (keyPress)
			{
				var key = Console.ReadKey(true).Key;
				ParseCommand(key, PlayerPet);
			}
		}
	}

	private void ParseCommand(ConsoleKey key, Pet pet)
	{

		switch(key)
		{
			case ConsoleKey.F:
				pet.Feed();
				break;
			case ConsoleKey.T:
				pet.Play();
				break;
			case ConsoleKey.R:
				pet.Sleep();
				break;
			case ConsoleKey.X:
				SaveManager.SaveSync(pet);
				Console.SetCursorPosition(0,0);
				Console.Clear();
				Environment.Exit(0);
				break;
			case ConsoleKey.S:
				Render.DisplayMessage(pet.GetStatus(), 2500);
				break;
			default:
				Render.DisplayMessage("Unknown key", 2500);
				break;
		}
	}
}
