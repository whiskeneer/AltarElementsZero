static class Program
{
	[STAThread]
	static void Main()
	{
		using (GameClass game = new())
		{
			game.Run();
		}
	}
}