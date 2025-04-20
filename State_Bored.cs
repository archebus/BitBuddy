namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class BoredState : PetState
{
	public override void Update(Pet pet, double elapsedTime)
	{
		pet.Hunger = Math.Max(0, pet.Hunger - 1.0 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Energy = Math.Max(0, pet.Energy - 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Happiness = Math.Max(0, Math.Min(pet.Happiness, 30));
		pet.Weight = Math.Max(1, pet.Weight - 0.01 * elapsedTime * Pet.DecayRateMultiplier);

		if (IsHungry(pet))
			TransitionTo(pet, new HungryState());
		else if (IsTired(pet))
			TransitionTo(pet, new TiredState());
	}

	public override void Feed(Pet pet)
	{
		if (IsFull(pet))
		{
			Render.DisplayMessage($"{pet.Name} is already full!", _messageDelay);
			return;
		}

		pet.Hunger = Math.Min(100, pet.Hunger + 10);
		Render.DisplayMessage($"{pet.Name} eats, but doesn't seem very interested in food.", _messageDelay);
		pet.Weight = Math.Min(20, pet.Weight + 0.7);

		if (!IsHungry(pet) && !IsBored(pet))
			TransitionTo(pet, new HappyState());
	}

	public override void Play(Pet pet)
	{
		pet.Happiness = Math.Min(100, pet.Happiness + 25);
		pet.Energy = Math.Max(0, pet.Energy - 10);
		pet.Weight = Math.Max(1, pet.Weight - 0.1);
		Render.DisplayMessage($"{pet.Name} plays with a lot of enthusiasm!", _messageDelay);

		if (IsTired(pet))
			TransitionTo(pet, new TiredState());
		else
			TransitionTo(pet, new ExcitedState());
	}

	public override void Sleep(Pet pet)
	{
		if (IsEnergetic(pet))
		{
			Render.DisplayMessage($"{pet.Name} isn't tired, just bored....", _messageDelay);
			return;
		}

		TransitionTo(pet, new SleepingState());
	}

	public override string GetStatus()	=> "He looks bored...";
	public override string[] GetEyes()	=> new string[] {"•́","•̀"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
       	 		"      ",
        		"     .",
        		"    ..",
        		"   ...",
        		"  ... ",
        		" ...  ",
        		" ..   ",
        		" .    ",
        		"      ",
    		};
	}
}
