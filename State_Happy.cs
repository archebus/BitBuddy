namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class HappyState : PetState
{
	public override void Update(Pet pet, double elapsedTime)
	{
		// Gradual stat decay
		pet.Hunger = Math.Max(0, pet.Hunger - 1.0 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Energy = Math.Max(0, pet.Energy - 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Happiness = Math.Max(0, pet.Happiness - 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Weight = Math.Max(1, pet.Weight - 0.01 * elapsedTime * Pet.DecayRateMultiplier);

		// State transitions based on stats
		if (IsHungry(pet))
			TransitionTo(pet, new HungryState());
		else if (IsTired(pet))
			TransitionTo(pet, new TiredState());
		else if (IsBored(pet))
			TransitionTo(pet, new BoredState());
	}

	public override void Feed(Pet pet)
	{
		if (IsFull(pet))
		{
			Render.DisplayMessage($"{pet.Name} is already full!");
			return;
		}

		pet.Hunger = Math.Min(100, pet.Hunger + 20);
		pet.Weight = Math.Min(20, pet.Weight + 0.7);
		Render.DisplayMessage($"{pet.Name} eats happily!");

		if (IsFull(pet))
		TransitionTo(pet, new ContentState());
	}

	public override void Play(Pet pet)
	{
		pet.Happiness = Math.Min(100, pet.Happiness + 10);
		pet.Energy = Math.Max(0, pet.Energy - 10);
		pet.Weight = Math.Max(1, pet.Weight - 0.2);

		Render.DisplayMessage($"{pet.Name} plays enthusiastically!");

		if (IsTired(pet))
			TransitionTo(pet, new TiredState());
		else if (IsHappy(pet))
			TransitionTo(pet, new ExcitedState());
	}

	public override void Sleep(Pet pet)
	{
		if (IsEnergetic(pet))
		{
			Render.DisplayMessage($"{pet.Name} isn't tired enough to sleep!");
			return;
		}

		TransitionTo(pet, new SleepingState());
	}

	public override string GetStatus() 	=> "Your pet is happy!";
	public override string[] GetEyes() 	=> new string[] {"•","•"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
        		"     ",
        		"     ",
        		"   ♪ ",
        		" ♫   ",
        		"     ",
        		"     ",
        		"   ♪ ",
        		"    ♫",
        		" ♪   ",
        		"  ♫  ",
    		};
	}
}

