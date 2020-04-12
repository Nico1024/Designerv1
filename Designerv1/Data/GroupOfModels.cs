using System;
using System.Collections.Generic;

namespace Designerv1.Data
{
    public static class GroupOfModels
    {
        public static List<MetaModel> Models;
        
        static GroupOfModels()
        {
            Models = new List<MetaModel>();
        }

        public static void AddNewModel(string model_name)
        {
            MetaModel metaModel = new MetaModel(model_name);
            Models.Add(metaModel);
        }
    }
}
