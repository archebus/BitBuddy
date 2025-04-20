namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class TiredState : PetState
{
	public override void Update(Pet pet, double elapsedTime)
	{
		pet.Hunger = Math.Max(0, pet.Hunger - 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Energy = Math.Max(0, pet.Energy - 0.2 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Happiness = Math.Max(0, pet.Happiness - 1.0 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Weight = Math.Max(1, pet.Weight - 0.01 * elapsedTime * Pet.DecayRateMultiplier);
		
		if (IsHungry(pet))
			TransitionTo(pet, new HungryState());
		else if (IsBored(pet))
			TransitionTo(pet, new BoredState());
	}

	public override void Feed(Pet pet)
	{
		pet.Hunger = Math.Min(100, pet.Hunger + 10);
		pet.Weight = Math.Min(20, pet.Weight + 0.4);

		Render.DisplayMessage($"{pet.Name} eats slowly, seeming tired.", _messageDelay);

		if (!IsHungry(pet) && !IsTired(pet))
			TransitionTo(pet, new HappyState());
	}

	public override void Play(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} is too tired to play!", _messageDelay);
		pet.Happiness = Math.Max(0, pet.Happiness - 5);
	}

	public override void Sleep(Pet pet)
	{
		TransitionTo(pet, new SleepingState());
	}

	public override string GetStatus()	=> "Your pet seems tired.";
	public override string[] GetEyes() 	=> new string[] {"｡","｡"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
       	 		"     ",
       	 		"    Z",
       	 		"   Zz",
       	 		"  Zz ",
       	 		" Zz  ",
       	 		"     ",
       	 		"     ",
       	 		"     ",
       	 		"     ",
       	 		"     ",
       	 		"     ",
    		};
	}
}
