namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class SleepingState : PetState
{
	private int _sleepCycles = 0;
	private readonly int _maxSleep = 200;

	public override void Update(Pet pet, double elapsedTime) 
	{
		pet.Energy = Math.Min(100, pet.Energy + (5 * elapsedTime * (Pet.DecayRateMultiplier * 4)));
		pet.Hunger = Math.Max(0, pet.Hunger - (0.2 * elapsedTime * Pet.DecayRateMultiplier));
		pet.Happiness = Math.Max(0, pet.Happiness - (0.2 * elapsedTime * Pet.DecayRateMultiplier));
		pet.Weight = Math.Max(1, pet.Weight - 0.01 * elapsedTime * Pet.DecayRateMultiplier);

		_sleepCycles++;
		
		if(_sleepCycles >= _maxSleep || pet.Energy >= 100)
		{
			Render.DisplayMessage($"{pet.Name} wakes up refreshed!", _messageDelay);

			if (IsHungry(pet))
				TransitionTo(pet, new HungryState());
			else if (IsBored(pet))
				TransitionTo(pet, new BoredState());
			else
				TransitionTo(pet, new HappyState());
		}
	}

	public override void Feed(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} is sleeping and can't eat now..", _messageDelay);
	}
	
	public override void Play(Pet pet)
	{
		Render.DisplayMessage($"You poke {pet.Name} ...", _messageDelay);

		if(_sleepCycles < 50)
		{
			Render.DisplayMessage($"{pet.Name} doesn't wake ...", _messageDelay);
			pet.Happiness = Math.Max(0, pet.Happiness - 5);
			return;
		}

		Render.DisplayMessage($"{pet.Name} wakes up to play.", _messageDelay);

		pet.Happiness = Math.Max(0, pet.Happiness - 10);

		if (IsHungry(pet))
			TransitionTo(pet, new HungryState());
		else if (IsBored(pet))
			TransitionTo(pet, new BoredState());
		else
			TransitionTo(pet, new HappyState());
	}

	public override void Sleep(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} is already fast asleep...", _messageDelay);
	}

	public override string GetStatus()	=> "Your pet is sleeping ...";
	public override string[] GetEyes()	=> new string[] {"-","-"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
       	 		"   z  ",
       	 		"    Z ",
       	 		"     z",
       	 		" z    ",
       	 		"  Z   ",
       	 		"   z  ",
       	 		"    Z ",
       	 		"     z",
       	 		" Z    ",
       	 		"  z   ",
       	 		"   Z  ",
    		};
	}
}
