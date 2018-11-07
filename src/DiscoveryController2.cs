using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

/// <summary>
/// The battle phase is handled by the DiscoveryController.
/// </summary>
static class DiscoveryController2
{

	/// <summary>
	/// Handles input during the discovery phase of the game.
	/// </summary>
	/// <remarks>
	/// Escape opens the game menu. Clicking the mouse will
	/// attack a location.
	/// </remarks>
	public static void HandleDiscoveryInput()
	{
		if (SwinGame.KeyTyped(KeyCode.vk_F11)) 
		{
			GameController.FullScreen();
		}

		if (SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			GameController.AddNewState(GameState.ViewingGameMenu);
		}

		if (SwinGame.MouseClicked(MouseButton.LeftButton))
		{
			if (UtilityFunctions.IsMouseInRectangle(650, 80, 115, 25))
			{
				Random rnd = new Random();
				int row = rnd.Next(10);
				int column = rnd.Next(10);
				GameController.RandomAttack2(row, column);
			}
			else
			{
				DoAttack();
			}
		}
	}

	/// <summary>
	/// Attack the location that the mouse if over.
	/// </summary>
	private static void DoAttack()
	{
		Point2D mouse = default(Point2D);

		mouse = SwinGame.MousePosition();

		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
		col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

		if (row >= 0 & row < GameController.HumanPlayer.EnemyGrid.Height)
		{
			if (col >= 0 & col < GameController.HumanPlayer.EnemyGrid.Width)
			{
				GameController.Attack2(row, col);
			}
		}
	}

	/// <summary>
	/// Draws the game during the attack phase.
	/// </summary>s
	public static void DrawDiscovery()
	{
		const int SCORES_LEFT = 172;
		const int SHOTS_TOP = 157;
		const int HITS_TOP = 206;
		const int SPLASH_TOP = 256;

		if ((SwinGame.KeyDown(KeyCode.vk_LSHIFT) | SwinGame.KeyDown(KeyCode.vk_RSHIFT)) & SwinGame.KeyDown(KeyCode.vk_c))
		{
			UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.HumanPlayer2, true);
		}
		else
		{
			UtilityFunctions.DrawField(GameController.HumanPlayer.EnemyGrid, GameController.HumanPlayer2, false);
		}

		UtilityFunctions.DrawMessage();
		SwinGame.DrawText(GameController.HumanPlayer.Shots.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
		SwinGame.DrawText(GameController.HumanPlayer.Hits.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, HITS_TOP);
		SwinGame.DrawText(GameController.HumanPlayer.Missed.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
		SwinGame.DrawText("Player 1's Phase", Color.White, GameResources.GameFont("Menu"), 100, 300);
		SwinGame.DrawBitmap(GameResources.GameImage("RandomHit"), 650, 80);
		//SwinGame.DrawText(GameController.setting.ToString(), Color.White, GameResources.GameFont("Menu"), SCORES_LEFT, SPLASH_BOTTOM);
	}

}
