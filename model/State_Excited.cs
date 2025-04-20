namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class ExcitedState : PetState
{
	private int _excitementCycles = 0;
	private readonly int _maxCycles = 100;

	public override void Update(Pet pet, double elapsedTime)
	{
		pet.Hunger = Math.Max(0, pet.Hunger - 1.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Energy = Math.Max(0, pet.Energy - 1.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Weight = Math.Max(1, pet.Weight - 0.01 * elapsedTime * Pet.DecayRateMultiplier);

		_excitementCycles++;

		if(_excitementCycles >= _maxCycles)
		{
			if (IsHungry(pet))
				TransitionTo(pet, new HungryState());
			else if (IsTired(pet))
				TransitionTo(pet, new TiredState());
			else
				TransitionTo(pet, new HappyState());
		}
	}

	public override void Feed(Pet pet)
	{
		if (IsFull(pet))
		{
			Render.DisplayMessage($"{pet.Name} is already full!", _messageDelay);
			return;
		}

		pet.Hunger = Math.Min(100, pet.Hunger + 10);
		pet.Weight = Math.Min(20, pet.Weight + 0.7);
		Render.DisplayMessage($"{pet.Name} quickly gobbles the food down and makes a mess...", _messageDelay);
	}

	public override void Play(Pet pet)
	{
		pet.Happiness = 100;
		pet.Energy = Math.Max(0, pet.Energy - 15);
		pet.Weight = Math.Max(1, pet.Weight - 0.2);
		Render.DisplayMessage($"{pet.Name} plays with great excitement!", _messageDelay);

		_excitementCycles = 0;

		if (IsTired(pet))
			TransitionTo(pet, new TiredState());
	}

	public override void Sleep(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} is too excited to go to sleep!", _messageDelay);
	}

	public override string GetStatus()	=> "Your pet is super excited!";
	public override string[] GetEyes()	=> new string[] {"◉","◉"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
       	 		"     ",
       	 		" !   ",
       	 		" !!  ",
       	 		"  !! ",
       	 		"   !!",
       	 		"    !",
       	 		"     ",
       	 		"     ",
       	 		"     ",
       	 		"     ",
    		};
	}
}
