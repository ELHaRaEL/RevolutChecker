using RevolutChecker;

Manager ManagerService = new();

Task task = Task.Run(async () => 
{
await ManagerService.StartManager();
});




Console.WriteLine("Press enter to close");
Console.ReadKey();