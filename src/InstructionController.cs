using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SwinGameSDK;
static class InstructionController
{
	public static void Background()
	{
		UtilityFunctions.DrawBackground();
		SwinGame.DrawBitmap(GameResources.GameImage("Instruction"), 213, 52);
	}
	public static void HandleHighScoreInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE) || SwinGame.KeyTyped(KeyCode.vk_RETURN))
		{
			GameController.EndCurrentState();
		}
	}
}