using System.Collections.Generic;
using TMPro;
using UnityEngine;

internal static class LocalChatCommandF1Help
{
    private static List<LocalChatCommandHelpPage> commandPages = new List<LocalChatCommandHelpPage>();
	private static int currentPage;

    public static void registerBaseGameCommands(OnlineGameplayUI instance)
    {
		LocalChatCommandHelpPage firstPage = new LocalChatCommandHelpPage();

        GameObject commands = instance.tooltips.transform.GetChild(0).gameObject;
        TextMeshProUGUI commandsText = commands.GetComponent<TextMeshProUGUI>();
        firstPage.addLines(commandsText.text);

		commandPages.Insert(0, firstPage);
		currentPage = 0;
        commandsText.text = getPage();
    }

	public static void nextPage(OnlineGameplayUI instance)
	{
		if (instance.showTooltip && instance.OnlineTabLeaderboard.SwitchAction.buttonDown)
		{
			GameObject commands = instance.tooltips.transform.GetChild(0).gameObject;
			TextMeshProUGUI commandsText = commands.GetComponent<TextMeshProUGUI>();
			pageChange();
			commandsText.text = getPage();
		}
	}

	public static int getCurrentPage() { return currentPage; }

	public static int getPageCount() { return commandPages.Count; }

	private static string getPage()
	{
		if (commandPages.Count == 0) return "";
		else
		{
			return commandPages[currentPage].getText();
		}
	}

	private static void pageChange()
	{
		if (commandPages.Count == 0) return;

		currentPage = ((currentPage + 1) % commandPages.Count);
	}

	public static void addCommands(string commands)
	{
		// Try to all lines on an existing page
		foreach (LocalChatCommandHelpPage page in commandPages)
		{
			if (page.addLines(commands)) return;
		}

		addPage(commands);
	}

	private static void addPage(string commands)
	{
		LocalChatCommandHelpPage newPage = new LocalChatCommandHelpPage();

		// Try to add all lines on the same new page
		if (newPage.addLines(commands))
		{
			commandPages.Add(newPage);
			return;
		}

		// Add lines individually since that didn't work
		foreach (string line in commands.Split('\n'))
		{
            if (!newPage.addSingleLine(line))
			{
				commandPages.Add(newPage);
				newPage = new LocalChatCommandHelpPage();
			}
		}

		commandPages.Add(newPage);
	}
}
