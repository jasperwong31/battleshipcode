using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

/// <summary>
/// The GameController is responsible for controlling the game,
/// managing user input, and displaying the current state of the
/// game.
/// </summary>
public static class GameController
{

	private static BattleShipsGame _theGame;
	private static BattleShipsGame _theGame2;
	private static Player _human1;
	private static Player _human2;

	private static AIPlayer _ai;

	private static Stack<GameState> _state = new Stack<GameState>();

	private static AIOption _aiSetting;

	public static uint time;

	/// <summary>
	/// Returns the current state of the game, indicating which screen is
	/// currently being used
	/// </summary>
	/// <value>The current state</value>
	/// <returns>The current state</returns>
	public static GameState CurrentState {
		get { return _state.Peek(); }
	}



	/// <summary>
	/// Returns the human player.
	/// </summary>
	/// <value>the human player</value>
	/// <returns>the human player</returns>
	public static Player HumanPlayer {
		get { return _human1; }
	}

	public static Player HumanPlayer2 {
		get { return _human2; }	}

	/// <summary>
	/// Returns the computer player.
	/// </summary>
	/// <value>the computer player</value>
	/// <returns>the conputer player</returns>
	public static Player ComputerPlayer {
		get { return _ai; }
	}

	static GameController()
	{
		//bottom state will be quitting. If player exits main menu then the game is over
		_state.Push(GameState.Quitting);

		//at the start the player is viewing the main menu
		_state.Push(GameState.ViewingMainMenu);
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
	/// <remarks>
	/// Creates an AI player based upon the _aiSetting.
	/// </remarks>
	public static void StartGame()
	{
		if (_theGame != null)
			EndGame();

		//Create the game
		_theGame = new BattleShipsGame();

		//create the players
		// FIXME NO easy setting? FIXED
		switch (_aiSetting) {
			case AIOption.Medium:
				_ai = new AIMediumPlayer(_theGame);
				break;
			case AIOption.Hard:
				_ai = new AIHardPlayer(_theGame);
				break;
            case AIOption.Easy:
                _ai = new AIEasyPlayer(_theGame);
                break;
            default:
				_ai = new AIHardPlayer(_theGame);
				break;
		}

		_human1 = new Player(_theGame);

		//AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed += GridChanged;
		_theGame.AttackCompleted += AttackCompleted;

		AddNewState(GameState.Deploying);
	}

	public static void StartGame2()
	{
		if (_theGame2 != null)
			EndGame2();

		//Create the game
		_theGame2 = new BattleShipsGame();

		_human1 = new Player(_theGame2);
		_human2 = new Player(_theGame2);

		_theGame2.AttackCompleted += AttackCompleted2;

		AddNewState(GameState.Deploying2);	}

	/// <summary>
	/// Stops listening to the old game once a new game is started
	/// </summary>

	private static void EndGame()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed -= GridChanged;
		_theGame.AttackCompleted -= AttackCompleted;
	}

	private static void EndGame2()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_human2.PlayerGrid.Changed -= GridChanged;
		_theGame2.AttackCompleted -= AttackCompleted2;	}

	/// <summary>
	/// Listens to the game grids for any changes and redraws the screen
	/// when the grids change
	/// </summary>
	/// <param name="sender">the grid that changed</param>
	/// <param name="args">not used</param>
	private static void GridChanged(object sender, EventArgs args)
	{
		DrawScreen();
		SwinGame.RefreshScreen();
	}

	private static void PlayHitSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation) {
			UtilityFunctions.AddExplosion(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Hit"));

		UtilityFunctions.DrawAnimationSequence();
	}

	private static void PlayMissSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation) {
			UtilityFunctions.AddSplash(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Miss"));

		UtilityFunctions.DrawAnimationSequence();
	}

	/// <summary>
	/// Listens for attacks to be completed.
	/// </summary>
	/// <param name="sender">the game</param>
	/// <param name="result">the result of the attack</param>
	/// <remarks>
	/// Displays a message, plays sound and redraws the screen
	/// </remarks>
	private static void AttackCompleted(object sender, AttackResult result)
	{
		bool isHuman = false;
		isHuman = object.ReferenceEquals(_theGame.Player, HumanPlayer);

		if (isHuman) {
			UtilityFunctions.Message = "You " + result.ToString();
		} else {
			UtilityFunctions.Message = "The AI " + result.ToString();
		}

		switch (result.Value) {
			case ResultOfAttack.Destroyed:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				break;
			case ResultOfAttack.GameOver:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink"))) {
					SwinGame.Delay(10);
					SwinGame.RefreshScreen();
				}

				if (HumanPlayer.IsDestroyed) {
					Audio.PlaySoundEffect(GameResources.GameSound("Lose"));
				} else {
					Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
				}

				break;
			case ResultOfAttack.Hit:
				PlayHitSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.Miss:
				PlayMissSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.ShotAlready:
				Audio.PlaySoundEffect(GameResources.GameSound("Error"));
				break;
		}
	}

	private static void AttackCompleted2(object sender, AttackResult result)
	{
		bool isHuman = false;
		bool isHuman2 = false;
		isHuman = object.ReferenceEquals(_theGame2.Player, HumanPlayer);
		isHuman2 = object.ReferenceEquals(_theGame2.Player, HumanPlayer2);

		if (isHuman)
		{
			UtilityFunctions.Message = "Player 1 " + result.ToString();
		}
		else
		{
			UtilityFunctions.Message = "Player 2 " + result.ToString();
		}

		if (GameController.CurrentState == GameState.Discovering2)
		{
			switch (result.Value)
			{
				case ResultOfAttack.Destroyed:
					PlayHitSequence(result.Row, result.Column, isHuman);
					Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

					break;
				case ResultOfAttack.GameOver:
					PlayHitSequence(result.Row, result.Column, isHuman);
					Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

					while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink")))
					{
						SwinGame.Delay(10);
						SwinGame.RefreshScreen();
					}

					if (HumanPlayer.IsDestroyed)
					{
						Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
						EndCurrentState();

					}
					else
					{
						Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
						EndCurrentState();
					}

					break;
				case ResultOfAttack.Hit:
					PlayHitSequence(result.Row, result.Column, isHuman);
					break;
				case ResultOfAttack.Miss:
					PlayMissSequence(result.Row, result.Column, isHuman);
					break;
				case ResultOfAttack.ShotAlready:
					Audio.PlaySoundEffect(GameResources.GameSound("Error"));
					break;
			}
		}
		else
		{
			switch (result.Value)
			{
				case ResultOfAttack.Destroyed:
                    PlayHitSequence(result.Row, result.Column, isHuman2);
					Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

					break;
				case ResultOfAttack.GameOver:
					PlayHitSequence(result.Row, result.Column, isHuman2);
					Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

					while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink")))
					{
						SwinGame.Delay(10);
						SwinGame.RefreshScreen();
					}

					if (HumanPlayer.IsDestroyed)
					{
						Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
						EndCurrentState();

					}
					else
					{
						Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
						EndCurrentState();
					}

					break;
				case ResultOfAttack.Hit:
					PlayHitSequence(result.Row, result.Column, isHuman2);
					break;
				case ResultOfAttack.Miss:
					PlayMissSequence(result.Row, result.Column, isHuman2);
					break;
				case ResultOfAttack.ShotAlready:
					Audio.PlaySoundEffect(GameResources.GameSound("Error"));
					break;
			}
		}
	}

	/// <summary>
	/// Completes the deployment phase of the game and
	/// switches to the battle mode (Discovering state)
	/// </summary>
	/// <remarks>
	/// This adds the players to the game before switching
	/// state.
	/// </remarks>
	public static void EndDeployment()
	{
		//deploy the players
		_theGame.AddDeployedPlayer(_human1);
		_theGame.AddDeployedPlayer(_ai);

		SwitchState(GameState.Discovering);
	}

	public static void EndDeployment2()
	{
		//deploy the players
		_theGame2.AddDeployedPlayer(_human1);

		SwitchState(GameState.Deploying3);	}

	public static void EndDeployment3()
	{
		//deploy the players
		_theGame2.AddDeployedPlayer(_human2);

		SwitchState(GameState.Discovering2);	}

	/// <summary>
	/// Gets the player to attack the indicated row and column.
	/// </summary>
	/// <param name="row">the row to attack</param>
	/// <param name="col">the column to attack</param>
	/// <remarks>
	/// Checks the attack result once the attack is complete
	/// </remarks>
	public static void Attack(int row, int col)
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Shoot(row, col);
		CheckAttackResult(result);
	}

	public static void Attack2(int row, int col)
	{
		AttackResult result = default(AttackResult);
		result = _theGame2.Shoot(row, col);
		CheckAttackResult2(result);	}

	public static void Attack3(int row, int col)
	{
		AttackResult result = default(AttackResult);
		result = _theGame2.Shoot(row, col);
		CheckAttackResult3(result);	}

	/// <summary>
	/// Gets the AI to attack.
	/// </summary>
	/// <remarks>
	/// Checks the attack result once the attack is complete.
	/// </remarks>
	private static void AIAttack()
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Player.Attack();
		CheckAttackResult(result);
	}

	/// <summary>
	/// Checks the results of the attack and switches to
	/// Ending the Game if the result was game over.
	/// </summary>
	/// <param name="result">the result of the last
	/// attack</param>
	/// <remarks>Gets the AI to attack if the result switched
	/// to the AI player.</remarks>
	private static void CheckAttackResult(AttackResult result)
	{
		switch (result.Value) {
			case ResultOfAttack.Miss:
				if (object.ReferenceEquals(_theGame.Player, ComputerPlayer))
					AIAttack();
				break;
			case ResultOfAttack.GameOver:
				SwitchState(GameState.EndingGame);
				break;
		}
	}

	private static void CheckAttackResult2(AttackResult result)
	{
		switch (result.Value)
		{
			case ResultOfAttack.Miss:
				if (object.ReferenceEquals(_theGame2.Player, HumanPlayer2))
					_state.Push(GameState.Discovering3);
				break;
			case ResultOfAttack.GameOver:
				SwitchState(GameState.EndingGame2);
				break;
		}	}

	private static void CheckAttackResult3(AttackResult result)
	{
		switch (result.Value)
		{
			case ResultOfAttack.Miss:
				if (object.ReferenceEquals(_theGame2.Player, HumanPlayer))
					_state.Push(GameState.Discovering2);
				break;
			case ResultOfAttack.GameOver:
				SwitchState(GameState.EndingGame3);
				break;
		}	}

	/// <summary>
	/// Handles the user SwinGame.
	/// </summary>
	/// <remarks>
	/// Reads key and mouse input and converts these into
	/// actions for the game to perform. The actions
	/// performed depend upon the state of the game.
	/// </remarks>
	public static void HandleUserInput()
	{
		//Read incoming input events
		SwinGame.ProcessEvents();

		switch (CurrentState) {
			case GameState.ViewingMainMenu:
				MenuController.HandleMainMenuInput();
				break;
			case GameState.ViewingGameMenu:
				MenuController.HandleGameMenuInput();
				break;
			case GameState.AlteringSettings:
				MenuController.HandleSetupMenuInput();
				break;
			case GameState.Deploying:
				DeploymentController.HandleDeploymentInput();
				break;
			case GameState.Deploying2:
				DeploymentController2.HandleDeploymentInput();
				break;
			case GameState.Deploying3:
				DeploymentController3.HandleDeploymentInput();
				break;
			case GameState.Discovering:
				DiscoveryController.HandleDiscoveryInput();
				break;
			case GameState.Discovering2:
				DiscoveryController2.HandleDiscoveryInput();
				break;
			case GameState.Discovering3:
				DiscoveryController3.HandleDiscoveryInput();
				break;
			case GameState.EndingGame:
				EndingGameController.HandleEndOfGameInput(time);
				break;
			case GameState.EndingGame2:
			case GameState.EndingGame3:
				EndingGameController.HandleEndOfGameInput2(time);
				break;
			case GameState.ViewingHighScores:
				HighScoreController.HandleHighScoreInput();
				break;
			case GameState.ChooseMode:
				MenuController.HandleModeMenuInput();
				break;
			case GameState.Instruction:
				InstructionController.HandleHighScoreInput();
				break;
		}

		UtilityFunctions.UpdateAnimations();
	}

	/// <summary>
	/// Draws the current state of the game to the screen.
	/// </summary>
	/// <remarks>
	/// What is drawn depends upon the state of the game.
	/// </remarks>
	public static void DrawScreen()
	{
		UtilityFunctions.DrawBackground();

		switch (CurrentState) {
			case GameState.ViewingMainMenu:
				MenuController.DrawMainMenu();
				break;
			case GameState.ViewingGameMenu:
				MenuController.DrawGameMenu();
				break;
			case GameState.AlteringSettings:
				MenuController.DrawSettings();
				break;
			case GameState.Deploying:
				DeploymentController.DrawDeployment();
				break;
			case GameState.Deploying2:
				DeploymentController2.DrawDeployment();
				break;
			case GameState.Deploying3:
				DeploymentController3.DrawDeployment();
				break;
			case GameState.Discovering:
				DiscoveryController.DrawDiscovery(time);
				break;
			case GameState.Discovering2:
				DiscoveryController2.DrawDiscovery();
				break;
			case GameState.Discovering3:
				DiscoveryController3.DrawDiscovery();
				break;
			case GameState.EndingGame:
				EndingGameController.DrawEndOfGame();
				break;
			case GameState.EndingGame2:
				EndingGameController.DrawEndOfGame2();
				break;
			case GameState.EndingGame3:
				EndingGameController.DrawEndOfGame3();
				break;
			case GameState.ViewingHighScores:
				HighScoreController.DrawHighScores();
				break;
			case GameState.ChooseMode:
				MenuController.DrawChoose();
				break;
			case GameState.Instruction:
				InstructionController.Background();
				break;
		}

		UtilityFunctions.DrawAnimations();
		
		//Lowered to 30 frames per second to see the splash and explosion animations

		SwinGame.RefreshScreen(60);
	}

	/// <summary>
	/// Move the game to a new state. The current state is maintained
	/// so that it can be returned to.
	/// </summary>
	/// <param name="state">the new game state</param>
	public static void AddNewState(GameState state)
	{
		_state.Push(state);
		UtilityFunctions.Message = "";
	}

	/// <summary>
	/// End the current state and add in the new state.
	/// </summary>
	/// <param name="newState">the new state of the game</param>
	public static void SwitchState(GameState newState)
	{
		EndCurrentState();
		AddNewState(newState);
	}

	/// <summary>
	/// Ends the current state, returning to the prior state
	/// </summary>
	public static void EndCurrentState()
	{
		_state.Pop();
	}

	public static void FullScreen()
	{
		SwinGame.ToggleFullScreen();	}


	/// <summary>
	/// Sets the difficulty for the next level of the game.
	/// </summary>
	/// <param name="setting">the new difficulty level</param>
	public static void SetDifficulty(AIOption setting)
	{
		_aiSetting = setting;
	}

}
