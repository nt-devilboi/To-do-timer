using To_do_timer.Models.Book;

namespace To_do_timer.Tests;

public static class DataMemory
{
    public static Status ExampleStatusCSharp = new Status()
    {
        Id = Guid.NewGuid(), 
        Desc = "ООО велий шарп",
        Name = "изучение asp.net"
    };
    
    public static Status ExampleStatusTypeScript = new Status()
    {
        Id = Guid.NewGuid(), 
        Desc = "ООО велий Typescript",
        Name = "изучение react mobx"
    };
    
    public static Status ExampleStatusChill = new Status()
    {
        Id = Guid.NewGuid(), 
        Desc = "ООО отдых",
        Name = "отдых"
    };
    public static Status Unknown = new Status()
    {
        Id = Guid.NewGuid(), 
        Desc = "дальше неизвество",
        Name = "хз"
    };
}