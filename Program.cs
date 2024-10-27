using fun_time.EntityHandlers;
using fun_time.Models;

var bypass = new Bypass("ac_client");
var moduleBase = bypass.GetModuleBase(".exe");

var acEntityManager = new AcEntityManager(moduleBase, bypass);
acEntityManager.UpdateLocalPlayer();
acEntityManager.UpdateEntities();

foreach (var entity in acEntityManager.GetEntities())
{
    Console.WriteLine(entity.Name);
}