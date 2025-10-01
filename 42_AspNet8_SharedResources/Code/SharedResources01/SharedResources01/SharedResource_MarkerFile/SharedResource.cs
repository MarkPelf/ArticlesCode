
//SharedResource.cs===================================================
namespace SharedResources01
{
    /*
    * This is just a dummy marker class to group shared resources
    * We need it for its name and type
    * 
    * It seems the namespace needs to be the same as app root namespace
    * which needs to be the same as the assembly name.
    * I had some problems when changing the namespace, it would not work.
    * If it doesn't work for you, you can try to use full class name
    * in your DI instruction, like this one:
    * IStringLocalizer<SharedResources01.SharedResource> StringLocalizer
    * 
    * There is no magic in the name "SharedResource", you can
    * name it "MyResources" and change all references in the code
    * to "MyResources" and all will still work
    * 
    * Location seems can be any folder, although some
    * articles claim it needs to be the root project folder
    * I do not see such problems in this example. 
    * To me looks it can be any folder, just keep your
    * namespace tidy. 
    */

    public class SharedResource
    {
    }
}
