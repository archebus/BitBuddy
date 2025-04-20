namespace SuperPet.Model.State;

// Abstract base class for all pet states.
// Contains command methods with overrides for current pet state.
public abstract class PetState
{
	public readonly int _messageDelay = 2000;
	
	// Core state behaviors
	public abstract void Update(Pet pet, double elapsedTime);
	public abstract void Feed(Pet pet);
	public abstract void Play(Pet pet);
	public abstract void Sleep(Pet pet);

	// Display related methods
	public abstract string GetStatus();
	public abstract string[] GetEyes();

	// Animation frames
	public abstract string[] GetFrames();

	// Utility method for state transitions
	protected void TransitionTo(Pet pet, PetState newState)
	{
		pet.CurrentState = newState;
	}

	// Clean-up state name for display.
	private string GetStateName(PetState state)
	{
		return state.GetType().Name.Replace("State", "");
	}

	// Utility methods to check pet condition.
	protected bool IsHungry(Pet pet) 	=> pet.Hunger    < 40;
	protected bool IsTired(Pet pet)  	=> pet.Energy    < 20;
	protected bool IsBored(Pet pet)  	=> pet.Happiness < 40;
	protected bool IsFull(Pet pet)   	=> pet.Hunger    > 90;
	protected bool IsEnergetic(Pet pet)	=> pet.Energy    > 80;
	protected bool IsHappy(Pet pet) 	=> pet.Happiness > 70;
}
