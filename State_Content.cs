namespace SuperPet.Model.State;

using SuperPet.Renderer;

public class ContentState : PetState
{
	private int _contentmentCycles = 0;
	private readonly int _maxCycles = 100;

	public override void Update(Pet pet, double elapsedTime)
	{
		pet.Happiness = Math.Min(100, pet.Happiness + 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Hunger = Math.Max(0, pet.Hunger - 0.2 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Energy = Math.Max(0, pet.Energy - 0.5 * elapsedTime * Pet.DecayRateMultiplier);
		pet.Weight = Math.Max(1, pet.Weight - 0.01 * elapsedTime * Pet.DecayRateMultiplier);

		_contentmentCycles++;

		if (_contentmentCycles >= _maxCycles)
		{
			if (IsTired(pet))
				TransitionTo(pet, new TiredState());
			else if (IsBored(pet))
				TransitionTo(pet, new BoredState());
			else
				TransitionTo(pet, new HappyState());
		}
	}

	public override void Feed(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} is too full to eat more!", _messageDelay); 
	}

	public override void Play(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} looks lazily at your attempt to play.", _messageDelay);
		pet.Happiness = Math.Min(100, pet.Happiness + 5);

		if (IsHappy(pet))
			TransitionTo(pet, new HappyState());
	}

	public override void Sleep(Pet pet)
	{
		Render.DisplayMessage($"{pet.Name} passes out and begins to snore loudly...", _messageDelay);
		TransitionTo(pet, new SleepingState());
	}

	public override string GetStatus()	=> "Your pet has a fully belly!";
	public override string[] GetEyes()	=> new string[] {"◕","◕"};
	public override string[] GetFrames()
	{
		return new string[]
    		{
        		"     ",
        		"    ♥",
        		"   ♥ ",
        		"  ♥  ",
        		" ♥   ",
        		"     ",
        		"     ",
        		"     ",
        		"    ♥",
        		"   ♥ ",
        		"  ♥  ",
        		" ♥   ",
        		"     ",
    		};
	}
}
