using System.Collections.Generic;

namespace NavigationDrawerPopUpMenu2.Classes
{
    class ListOfEnvironmentsAnditsObjects
    {
        public static List<ThingsInEnvironments> EnvAndObjects()
        {
            List<ThingsInEnvironments> Environments = new List<ThingsInEnvironments>()
            {
                new ThingsInEnvironments()
                {
                    NameofEnvironment = "BedRoom",
                    Objects = new List<string>()
                    {
                        "bed"
                    }
                }, new ThingsInEnvironments()
                {
                    NameofEnvironment = "Restaurant",
                    Objects = new List<string>()
                    {
                        "mirror",
                        "coffee table",
                        "bar rack",
                        "coffee chair",
                        "kitchen door",
                        "cabinet"
                    }
                },new ThingsInEnvironments()
                {
                    NameofEnvironment = "garden",
                    Objects = new List<string>()
                    {
                        "bench",
                        "tree"

                    }
                },
                new ThingsInEnvironments()
                {
                    NameofEnvironment = "Hole",
                    Objects = new List<string>()
                    {
                        "tv",
                        "television",
                        "door",
                        "kitchen table",
                        "kitchen door",
                        "pillow",
                        "kitchen shelf",
                    }
                },
                new ThingsInEnvironments()
                {
                    NameofEnvironment = "classRoom",
                    Objects = new List<string>()
                    {
                        "room",
                    }
                }

            };
            return Environments;
        }
    }
}
