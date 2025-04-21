namespace SuperPet.Persistance;

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SuperPet.Model;
using SuperPet.Model.State;

public class SaveSystem
{
	private readonly string _saveFilePath 		= default!;
	private DateTime _lastAutoSave 			= DateTime.Now;
	private readonly TimeSpan _autoSaveInterval 	= TimeSpan.FromMinutes(2);
	private bool _saveInProgress 			= false;

	public event EventHandler? SaveCompleted;

	public string[] paths = new string[]
	{
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"SuperPet",
		"pet.json",
	};

	public SaveSystem()
	{
		_saveFilePath = Path.Combine(paths);
		
		// Ensure dir exists.
		Directory.CreateDirectory(Path.GetDirectoryName(_saveFilePath)!);

		AppDomain.CurrentDomain.ProcessExit += (s, e) => SaveSync(Game.Instance.PlayerPet);
		Console.CancelKeyPress += (s, e) => SaveSync(Game.Instance.PlayerPet);
	}

	public void CheckAutoSave(Pet pet, bool stateChanged = false)
	{
		if(_saveInProgress)
			return;

		bool shouldSave = false;

		if(stateChanged)
			shouldSave = true;

		if((DateTime.Now - _lastAutoSave) > _autoSaveInterval)
			shouldSave = true;

		if(shouldSave)
			SaveAsync(pet);
	}

	public async void SaveAsync(Pet pet)
	{
		if(_saveInProgress)
			return;

		_saveInProgress = true;

		try
		{
			await Task.Run(() => SaveSync(pet));
			_lastAutoSave = DateTime.Now;
			SaveCompleted?.Invoke(this, EventArgs.Empty);
		}
		finally
		{
			_saveInProgress = false;
		}
	}

	public void SaveSync(Pet pet)
	{
		if (pet == null)
			return;

		var saveData = new Dictionary<string, object>
		{
			{ "Name", pet.Name },
			{ "Nose", pet.Nose },
			{ "Color", pet.Color.ToString() },
			{ "Birthday", pet.Birthday },
			{ "Hunger", pet.Hunger },
			{ "Energy", pet.Energy },
			{ "Happiness", pet.Happiness },
			{ "Weight", pet.Weight },
			{ "State", pet.CurrentState.GetType().Name },
			{ "LastSave", DateTime.Now },
		};

		var options = new JsonSerializerOptions { WriteIndented = true };
		var json = JsonSerializer.Serialize(saveData, options);

		string tempFilePath = _saveFilePath + ".temp";

		File.WriteAllText(tempFilePath, json);

		if 	(File.Exists(_saveFilePath))
			 File.Delete(_saveFilePath);

		File.Move(tempFilePath, _saveFilePath);
	}

	public Pet LoadPet()
	{
		if (!File.Exists(_saveFilePath))
			return new Pet();

		try
		{
			string json 	= File.ReadAllText(_saveFilePath);
			var saveData 	= JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

			var pet 	= new Pet(true) ?? default!;

			pet.Name 	= saveData["Name"].GetString() ?? "";
			pet.Nose	= saveData["Nose"].GetString();
			pet.Birthday	= saveData["Birthday"].GetDateTime();
			pet.Hunger 	= saveData["Hunger"].GetDouble();
			pet.Energy	= saveData["Energy"].GetDouble();
			pet.Happiness	= saveData["Happiness"].GetDouble();
			pet.Weight	= saveData["Weight"].GetDouble();

			if (saveData.ContainsKey("Color"))
			{
				string colorName = saveData["Color"].GetString() ?? "White";
				if(Enum.TryParse<ConsoleColor>(colorName, out var parsedColor))
				{
					pet.Color = parsedColor;
				}
			}

			if(saveData.ContainsKey("LastSave"))
			{
				DateTime lastSave = saveData["LastSave"].GetDateTime();
				ApplyOfflineDecay(pet, lastSave);
			}
			
			string stateName = saveData["State"].GetString();
			pet.CurrentState = stateName switch
			{
				"HappyState"	=> new HappyState(),
				"ContentState"	=> new ContentState(),
				"ExcitedState"	=> new ExcitedState(),
				"HungryState"	=> new HungryState(),
				"TiredState"	=> new TiredState(),
				"BoredState"	=> new BoredState(),
				"SleepingState"	=> new SleepingState(),
				_ => new HappyState(),
			};

			pet.UpdateStateBasedOnStats();

			return pet;
		}
		catch(Exception ex)
		{
			Console.WriteLine($"Error loading save: {ex.Message}");
			return new Pet();
		}
	}

	private void ApplyOfflineDecay(Pet pet, DateTime lastSave)
	{
		TimeSpan elapsed 	= DateTime.Now - lastSave;
		double elapsedHours 	= elapsed.TotalHours;

		if (elapsedHours <= 0)
			return;

		Console.WriteLine($"Your pet has been alone for {elapsedHours:F1} hours!");

		double elapsedSeconds 	= elapsed.TotalSeconds;
		double offlineDecayRate = 0.01;

		pet.Hunger		= Math.Max(10, pet.Hunger - (1.5 * elapsedSeconds * offlineDecayRate));
		pet.Energy		= Math.Max(10, pet.Energy - (1.0 * elapsedSeconds * offlineDecayRate));
		pet.Happiness		= Math.Max(10, pet.Happiness - (2.0 * elapsedSeconds * offlineDecayRate));

		if (elapsedHours > 48)
		{
			pet.Hunger	= Math.Max(pet.Hunger, 10);
			pet.Energy	= Math.Max(pet.Energy, 10);
			pet.Happiness	= Math.Max(pet.Happiness, 10);
			pet.Weight	= Math.Max(pet.Weight, 1);

			Console.WriteLine($"{pet.Name} looks neglected.");
		}

		Thread.Sleep(3000);
	}

	public bool HasSaveFile()
	{
		return File.Exists(_saveFilePath);
	}

	public TimeSpan TimeSinceLastSave()
	{
		return DateTime.Now - _lastAutoSave;
	}

	public string GetSaveFilePath()
	{
		return _saveFilePath;
	}
}
