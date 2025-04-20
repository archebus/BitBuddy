namespace SuperPet.Model;

using SuperPet.Model;

public class PetComponent
{
	public string[] Art { get; set; }
	public int OffsetX { get; }
	public int OffsetY { get; }

	public PetComponent(string[] art, int offsetX = 0, int offsetY = 0)
	{
		Art = art;
		OffsetX = offsetX;
		OffsetY = offsetY;
	}
}

public class PetAppearance
{
	private readonly Pet _pet;
	private readonly Random _rng;

	// Componenets
	private Dictionary<string, List<PetComponent>> _bodyComponents = new();
	private Dictionary<string, List<PetComponent>> _earComponents = new();
	private Dictionary<string, List<PetComponent>> _legComponents = new();
	private Dictionary<string, List<PetComponent>> _tailComponents = new();

	// Selected Componenets for this pet
	private Dictionary<GrowthStage, PetComponent> _selectedBodies = new();
	private Dictionary<GrowthStage, PetComponent> _selectedEars = new();
	private Dictionary<GrowthStage, PetComponent> _selectedLegs = new();
	private Dictionary<GrowthStage, PetComponent> _selectedTails = new();

	public PetAppearance(Pet pet)
	{
		_pet = pet;
		_rng = new Random(pet.Name.GetHashCode());	// Using pet name as seed.

		InitComponents();
		SelectComponents();
	}

	public int GetStageOffset()
	{
		if(_pet.CurrentStage == GrowthStage.Baby)
			return 2;

		else if(_pet.CurrentStage == GrowthStage.Child)
			return 1;

		else if(_pet.CurrentStage == GrowthStage.Teen)
			return 1;
		
		else if(_pet.CurrentStage == GrowthStage.Adult)
			return 1;

		else if(_pet.CurrentStage == GrowthStage.Elder)
			return 0;
			
		else
			return 0;
	}

	private void InitComponents()
	{
		// Bodies
		_bodyComponents["Baby"] = new List<PetComponent>
		{
			new PetComponent(new[] {
				"╭─────╮",
				"│     │",
				"╰─────╯"
			}),
			new PetComponent(new[] {
				"╭─────╮",
				"│     │",
				"╰──○──╯"
			}),
			new PetComponent(new[] {
				"╭──○──╮",
				"│     │",
				"╰─────╯"
			})
		};

		_bodyComponents["Child"] = new List<PetComponent>
		{
			new PetComponent(new[] {
				"╭───────╮",
				"│       │",
				"╰───────╯"
			}),
			new PetComponent(new[] {
				"╭───────╮",
				"│       │",
				"╰───○───╯"
			}),
			new PetComponent(new[] {
				"╭───○───╮",
				"│       │",
				"╰───────╯"
			})
		};

		_bodyComponents["Teen"] = new List<PetComponent>
		{
			new PetComponent(new[] {
				"╭─────────╮",
				"│         │",
				"╰─────────╯"
			}),
			new PetComponent(new[] {
				"╭─────────╮",
				"│         │",
				"╰────○────╯"
			}),
				new PetComponent(new[] {
				"╭─── ◠ ───╮",
				"│         │",
				"╰─────────╯"
			})
		};

		_bodyComponents["Adult"] = new List<PetComponent>
		{
			new PetComponent(new[] {
				"╭───────────╮",
				"│           │",
				"╰───────────╯"
			}),
			new PetComponent(new[] {
				"╭─── ◠◠◠ ───╮",
				"│           │",
				"╰───────────╯"
			}),
			new PetComponent(new[] {
				"╭───────────╮",
				"│  ◕     ◕  │",
				"╰───────────╯"
			})
		};

		_bodyComponents["Elder"] = new List<PetComponent>
		{
			new PetComponent(new[] {
				"╭───────────╮",
				"│           │",
				"├───────────┤",
				"╰───────────╯"
			}),
			new PetComponent(new[] {
				"╭─────○─────╮",
				"│           │",
				"├───────────┤",
				"╰───────────╯"
			}),
			new PetComponent(new[] {
				"╭───────────╮",
				"│  ~     ~  │",
				"├───────────┤",
				"╰───────────╯"
			})
		};

		// Ears
		_earComponents["Baby"] = new List<PetComponent>(); // No ears for babies.

		_earComponents["Child"] = new List<PetComponent>
		{
			new PetComponent(new[] { "^ ^" }, -1, -1),
            		new PetComponent(new[] { "⌒ ⌒" }, -1, -1),
            		new PetComponent(new[] { "∧ ∧" }, -1, -1)
		};

		_earComponents["Teen"] = new List<PetComponent>
		{
			new PetComponent(new[] { "/\\ /\\" }, -1, -1),
            		new PetComponent(new[] { "⌒⌒ ⌒⌒" }, -1, -1),
            		new PetComponent(new[] { "∩∩ ∩∩" }, -1, -1)
		};

		_earComponents["Adult"] = new List<PetComponent>
		{
			new PetComponent(new[] { "/\\   /\\" }, -1, -1),
            		new PetComponent(new[] { "⌒⌒   ⌒⌒" }, -1, -1),
            		new PetComponent(new[] { "∩∩   ∩∩" }, -1, -1)
		};

		_earComponents["Elder"] = new List<PetComponent>
		{
			new PetComponent(new[] { "/\\     /\\" }, -2, -1),
            		new PetComponent(new[] { "⌒⌒     ⌒⌒" }, -2, -1),
            		new PetComponent(new[] { "∩∩     ∩∩" }, -2, -1)
		};

		// Legs
		_legComponents["Baby"] = new List<PetComponent>(); // Babies don't get no legs.

		_legComponents["Child"] = new List<PetComponent>
		{
			new PetComponent(new[] { "| |" }, 1, 3),
            		new PetComponent(new[] { "╷ ╷" }, 1, 3),
            		new PetComponent(new[] { "o o" }, 1, 3)
		};

		_legComponents["Teen"] = new List<PetComponent>
		{ 
			new PetComponent(new[] { 
				"|   |",
				"╵   ╵"
				}, 1, 3),
			new PetComponent(new[] { 
				"/   \\",
				"╵   ╵"
				}, 1, 3),
			new PetComponent(new[] { 
				"\\   /",
				" | | "
				}, 1, 3)
		};

		_legComponents["Adult"] = new List<PetComponent>
		{
			new PetComponent(new[] { 
				"|     |",
				"╵     ╵",
				"      "
				}, 1, 3),
			new PetComponent(new[] { 
				"/     \\",
				"╵     ╵",
				"╱     ╲"
				}, 1, 3),
			new PetComponent(new[] { 
				"\\     /",
				" \\   / ",
				"  | |  "
				}, 1, 3)
		};

		_legComponents["Elder"] = new List<PetComponent>
		{
			new PetComponent(new[] { 
				"|       |",
				"╵       ╵",
				"\\       /"
				}, 0, 4),
			new PetComponent(new[] { 
				"/       \\",
				"╵       ╵",
				"╱       ╲"
				}, 0, 4),
			new PetComponent(new[] { 
				"\\       /",
				" \\     / ",
				"  \\   /  "
				}, 0, 4)
		};

		// Tails
		_tailComponents["Baby"] = new List<PetComponent>(); // No tails for babies.

		_tailComponents["Child"] = new List<PetComponent>
		{	
			new PetComponent(new[] { "~" }, 9, 2),
            		new PetComponent(new[] { "○" }, 9, 2),
            		new PetComponent(new[] { ">" }, 9, 2)
		};

		_tailComponents["Teen"] = new List<PetComponent>
		{
			new PetComponent(new[] { "~~" }, 11, 2),
            		new PetComponent(new[] { "○○" }, 11, 2),
            		new PetComponent(new[] { ">>" }, 11, 2)
		};

		_tailComponents["Adult"] = new List<PetComponent>
		{
			new PetComponent(new[] { "~~~" }, 13, 2),
            		new PetComponent(new[] { "○○○" }, 13, 2),
            		new PetComponent(new[] { ">>>" }, 13, 2)
		};

		_tailComponents["Elder"] = new List<PetComponent>
		{
			new PetComponent(new[] { "~~~~" }, 13, 3),
            		new PetComponent(new[] { "○○○○" }, 13, 3),
            		new PetComponent(new[] { ">>>>" }, 13, 3)
		};
	}

	private void SelectComponents()
	{
		foreach (GrowthStage stage in Enum.GetValues(typeof(GrowthStage)))
        	{
            		string stageName = stage.ToString();
            
            		// Select body
            		if (_bodyComponents.ContainsKey(stageName) && _bodyComponents[stageName].Count > 0)
                		_selectedBodies[stage] = _bodyComponents[stageName][_rng.Next(_bodyComponents[stageName].Count)];
            
            		// Select ears
            		if (_earComponents.ContainsKey(stageName) && _earComponents[stageName].Count > 0)
                		_selectedEars[stage] = _earComponents[stageName][_rng.Next(_earComponents[stageName].Count)];
            
            		// Select legs
            		if (_legComponents.ContainsKey(stageName) && _legComponents[stageName].Count > 0)
                		_selectedLegs[stage] = _legComponents[stageName][_rng.Next(_legComponents[stageName].Count)];
            
           		 // Select tail
            		if (_tailComponents.ContainsKey(stageName) && _tailComponents[stageName].Count > 0)
                		_selectedTails[stage] = _tailComponents[stageName][_rng.Next(_tailComponents[stageName].Count)];
        	}
	}

	public string[] RenderPet(Pet pet, int blinkFrame = 0)
	{
		GrowthStage stage = pet.CurrentStage;

		PetComponent body = _selectedBodies.ContainsKey(stage) ? _selectedBodies[stage] : new PetComponent(new[] { "○" });

		int width = 20;
		int height = 15;
		string[][] canvas = new string [height][];

		string[] eyes = pet.CurrentState.GetEyes();
		
		// Update eyes to blink on blinkframe.
		if(blinkFrame == 4)
		{
			eyes[0] = "-";
			eyes[1] = "-";
		}
		
		// Init empty canvas.
		for (int i = 0; i < height; i++)
		{
			canvas[i] = new string[width];
			for(int j = 0; j < width; j++)
			{
				canvas[i][j] = " ";
			}
		}

		// Get middle of canvas. 
		int bodyX = width / 2 - body.Art[0].Length / 2;
		int bodyY = height / 2 - body.Art.Length / 2;

		// Draw the body. (draw the rest of the owl..)
		DrawComponent(canvas, body, bodyX, bodyY);

		// Add eyes and nose
		int eyeLineIndex = body.Art.Length > 1 ? 1 : 0;
		int eyeY = bodyY + eyeLineIndex;

		if (eyeY >= 0 && eyeY < height)
		{
			string bodyLine = body.Art[eyeLineIndex];
			int middleX = bodyX + bodyLine.Length / 2;
			
			// Find eye and nose pos.
			int eyeSpacing = 2;
			int leftEyeX = middleX - eyeSpacing;
			int rightEyeX = middleX + eyeSpacing;
			int noseX = middleX;
			
			// Draw eyes if within canvas.
			if(leftEyeX >= 0 && leftEyeX < width)
			{
				canvas[eyeY][leftEyeX] = eyes[0];
			}

			if(rightEyeX >= 0 && rightEyeX < width)
			{
				canvas[eyeY][rightEyeX] = eyes[1];
			}


			// Draw nose.
			if(noseX >= 0 && noseX < width)
			{
				canvas[eyeY][noseX] = pet.Nose;
			}
		}

		// Draw other components if available.
		if(_selectedEars.ContainsKey(stage))
		{
			DrawComponent(canvas, _selectedEars[stage], bodyX+4, bodyY);
		}

		if(_selectedLegs.ContainsKey(stage))
		{
			DrawComponent(canvas, _selectedLegs[stage], bodyX+2, bodyY);
		}

		if(_selectedTails.ContainsKey(stage))
		{
			DrawComponent(canvas, _selectedTails[stage], bodyX, bodyY);
		}

		// Prepare final string for return
		string[] result = new string[height];
		for(int i = 0; i < height; i++)
			result[i] = string.Join("", canvas[i]);

		return TrimEmpty(result);
	}

	private void DrawComponent(string[][] canvas, PetComponent component, int baseX, int baseY)
	{
		int x = baseX + component.OffsetX;
		int y = baseY + component.OffsetY;

		for(int i = 0; i < component.Art.Length; i++)
		{
			if(y + i < 0 || y + i >= canvas.Length)
				continue;

			string line = component.Art[i];

			for(int j = 0; j < line.Length; j++)
			{
				if(x + j < 0 || x + j >= canvas[0].Length)
					continue;

				if(line[j] != ' ') // Avoid overwriting spaces.
					canvas[y+i][x+j] = line[j].ToString();
			}
		}
	}

	private string[] TrimEmpty(string[] lines)
	{
		int startLine = 0;
		int endLine = lines.Length - 1;

		while(startLine <= endLine && lines[startLine].Trim() == "")
			startLine++;

		while(endLine >= startLine && lines[endLine].Trim() == "")
			endLine--;

		int count = endLine - startLine + 1;
		if(count <= 0)
			return new[] { " " };	// Have to return something.
		
		string[] result = new string[count];
		Array.Copy(lines, startLine, result, 0, count);
		return result;
	}


	public string[] GetMultiLineArt(string[] eyes, string nose)
	{
		switch(_pet.CurrentStage)
		{
			case GrowthStage.Baby:
				return GetBabyArt(eyes, nose);
			case GrowthStage.Child:
				return GetChildArt(eyes, nose);
			case GrowthStage.Teen:
				return GetTeenArt(eyes, nose);
			case GrowthStage.Adult:
				return GetAdultArt(eyes, nose);
			case GrowthStage.Elder:
				return GetSeniorArt(eyes, nose);
			default:
				return GetBabyArt(eyes, nose);
		}
	}

	private string[] GetBabyArt(string[] eyes, string nose)
	{
		return new string[]
		{
			$"   ╭─────╮   ",
            		$"   │{eyes[0]} {nose} {eyes[1]}│   ",
            		$"   ╰─────╯   ",
		};
	}

	private string[] GetChildArt(string[] eyes, string nose)
	{
		return new string[]
		{
			$"  ╭───────╮  ",
            		$"  │ {eyes[0]} {nose} {eyes[1]} │  ",
            		$"  ╰─┬───┬─╯  ",
            		$"    │   │    ",
		};
	}

	private string[] GetTeenArt(string[] eyes, string nose)
	{
		return new string[]
		{
			$" ╭─────────╮ ",
            		$" │  {eyes[0]} {nose} {eyes[1]}  │ ",
            		$" ╰─┬─────┬─╯ ",
            		$"   ╵     ╵   ",
            		$"  ╱       ╲  ",
		};
	}

	private string[] GetAdultArt(string[] eyes, string nose)
	{
		return new string[]
		{
            		$"╭───────────╮",
            		$"│   {eyes[0]} {nose} {eyes[1]}   │",
            		$"╰─┬───────┬─╯",
            		$"  │       │  ",
            		$" ╱╵       ╵╲ ",
            		$"╱           ╲",
		};
	}

	private string[] GetSeniorArt(string[] eyes, string nose)
	{
		return new string[]
		{
			$" ╭─────────╮ ",
            		$" │   {eyes[0]} {nose} {eyes[1]}   │ ",
            		$" ├─────────┤ ",
            		$" │         │ ",
            		$" ╰┬───────┬╯ ",
            		$"  ╵       ╵  ",
            		$" ╱         ╲ ",
		};
	}
}
