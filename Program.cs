using fun_time.EntityHandlers;
using fun_time.Models;

var bypass = new Bypass("ac_client");
var moduleBase = bypass.GetModuleBase(".exe");

var acEntityManager = new AcEntityManager(moduleBase, bypass);
acEntityManager.ReadLocalPlayer();
acEntityManager.ReadEntities();
await acEntityManager.KeepMeAlive();

var ents = acEntityManager.GetEntities();

if (ents.Count == 0)
{
    Console.WriteLine("No entities found");
    return;
}

// foreach (var entity in ents)
// {
//     Console.WriteLine(entity.Name);
//     Console.WriteLine(entity.Health);
// }