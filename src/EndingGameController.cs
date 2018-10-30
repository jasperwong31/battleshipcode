using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

static class EndingGameController
{

	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public static void DrawEndOfGame()
	{
		UtilityFunctions.DrawField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);
		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

		if (GameController.HumanPlayer.IsDestroyed) {
			SwinGame.DrawTextLines("YOU LOSE!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		} else {
			SwinGame.DrawTextLines("-- WINNER --", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
	}

	public static void DrawEndOfGame2()
	{
		UtilityFunctions.DrawField(GameController.HumanPlayer2.PlayerGrid, GameController.HumanPlayer2, true);
		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

		if (GameController.HumanPlayer.IsDestroyed)
		{
			
			SwinGame.DrawTextLines("PLAYER 2 WIN!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
		else
		{
			SwinGame.DrawTextLines("PLAYER 1 WIN!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}	}

	public static void DrawEndOfGame3()
	{
		UtilityFunctions.DrawField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer, true);
		UtilityFunctions.DrawSmallField(GameController.HumanPlayer2.PlayerGrid, GameController.HumanPlayer2);

		if (GameController.HumanPlayer.IsDestroyed)
		{
			SwinGame.DrawTextLines("PLAYER 2 WIN!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
		else
		{
			SwinGame.DrawTextLines("PLAYER 1 WIN!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}	}

	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highsSwinGame.
	/// </summary>
	public static void HandleEndOfGameInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_RETURN) || SwinGame.KeyTyped(KeyCode.vk_F11) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE)) {
			HighScoreController.ReadHighScore(GameController.HumanPlayer.Score);
			GameController.EndCurrentState();
		}
	}

	public static void HandleEndOfGameInput2()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_RETURN) || SwinGame.KeyTyped(KeyCode.vk_F11) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			GameController.EndCurrentState();
		}	}
}
