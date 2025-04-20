namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class HungryState : PetState
{
	public override void Update(Pet pet, double elapsedTime)
	{
		pet.Hunger = Math.Max(0, pet.Hunger - 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Energy = Math.Max(0, pet.Energy - 1.0 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Happiness = Math.Max(0, pet.Happiness - 1.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Weight = Math.Max(1, pet.Weight - 0.06 * elapsedTime * Pet.DecayRateMultiplier);

		if (IsTired(pet))
			TransitionTo(pet, new TiredState());
		else if (IsBored(pet))
			TransitionTo(pet, new BoredState());
	}

	public override void Feed(Pet pet)
	{
		pet.Hunger = Math.Min(100, pet.Hunger + 20);
		pet.Happiness = Math.Min(100, pet.Happiness + 5);
		pet.Weight = Math.Min(20, pet.Weight + 0.7);

		Render.DisplayMessage($"{pet.Name} devours the food hungrily!", _messageDelay);

		if (IsFull(pet))
			TransitionTo(pet, new ContentState());
		else if (!IsHungry(pet))
			TransitionTo(pet, new HappyState());
	}

	public override void Play(Pet pet)
	{
		pet.Happiness = Math.Min(100, pet.Happiness + 5);
		pet.Energy = Math.Max(0, pet.Energy - 15);
		pet.Hunger = Math.Max(0, pet.Hunger - 5);
		pet.Weight = Math.Max(1, pet.Weight - 0.1);

		Render.DisplayMessage($"{pet.Name} plays, but seems distracted by hunger.", _messageDelay);

		if(IsTired(pet))
			TransitionTo(pet, new TiredState());
	}

	public override void Sleep(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} is too hungry to sleep!", _messageDelay);
	}

	public override string GetStatus() 	=> "Your pet is hungry!";
	public override string[] GetEyes() 	=> new string[] {"•̀","•́"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
        		"     ",
        		"    ?",
        		"   ? ",
        		"  ?  ",
        		" ?   ",
        		"     ",
        		"     ",
        		"     ",
        		"     ",
        		"     ",
    		};
	}
}
