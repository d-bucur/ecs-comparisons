internal class Program {
	private static void Main(string[] args) {
		Console.WriteLine($"--- FLECS ---");
		FlecsExample.FlecsExample.Run();
		Console.WriteLine($"--- FRIFLO ---");
		FrifloExample.FrifloExample.Run();
	}
}