using System.Collections.Generic;

namespace NavigationDrawerPopUpMenu2.Classes
{
    class Prediction
    {
        public static List<string> Models()
        {
            List<string> data = new List<string>
            {
                "bed",
                "chair",
                "sofa",
                "table",
                "vase",
                "mirror",
                "coffee table",
                "sofa",
                "bar rack",
                "coffee chair",
                "kitchen door",
                "cabinet",
                "bench",
                "tree",
                "tv",
                "television",
                "door",
                "kitchen table",
                "kitchen door",
                "pillow",
                "kitchen shelf",
                "Hole",
                "garden",
                "restaurant",
                "Bed room",
            };
            return data;
        }
    }
}
//if (found)
//{
//    var nameObject = MovieDb.Objects.Where(g => g.Name.ToLower() == (query.ToLower())
//                                                || g.Name.StartsWith(query));
//    if (nameObject.Any())
//    {
//        found = true;
//        foreach (var item in nameObject)
//        {
//            addItem(nameObject.ToString());
//            Prediction.Models().Add(item.ToString());
//        }
//    }

//    var nameEnviroment = MovieDb.Environments.Where(g => g.Name.ToLower() == (query.ToLower())
//                                                         || g.Name.StartsWith(query));
//    if (nameEnviroment.Any())
//    {
//        found = true;
//        foreach (var item in nameEnviroment)
//        {
//            addItem(nameEnviroment.ToString());
//            Prediction.Models().Add(item.ToString());
//        }
//    }
//}