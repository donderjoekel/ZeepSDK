internal class LocalChatCommandHelpPage
{
	private const int MAX_LINES_PER_PAGE = 38;

	private int linesUsed = 0;
	private bool groupedLast = false;
	private string textString = "";

	public LocalChatCommandHelpPage() {}

	public string getText() => textString;

	public bool addSingleLine(string lineToAdd)
	{
		int linesRequired = groupedLast ? 2 : 0;

		if (linesRequired + linesUsed > MAX_LINES_PER_PAGE) return false;

		if (groupedLast)
		{
			addLine("");
			addLine("");
		}
		addLine(lineToAdd);
		groupedLast = false;

		return true;
	}

	public bool addLines(string linesToAdd)
	{
		string[] commandLines = linesToAdd.Split('\n');
		if (commandLines.Length == 1) return addSingleLine(commandLines[0]);

		int linesRequired = groupedLast ? 2 : 0;
		foreach (string commandLine in commandLines)
		{
			++linesRequired;
		}

		if (linesRequired + linesUsed > MAX_LINES_PER_PAGE) return false;

		if (linesUsed > 0)
		{
			addLine("");
			addLine("");
		}
		foreach (string commandLine in commandLines)
		{
			addLine(commandLine);
		}

		groupedLast = true;
		return true;
	}

	private void addLine(string line)
	{
		if (linesUsed > 0) textString += '\n';
		textString += line;
		++linesUsed;
	}
}
