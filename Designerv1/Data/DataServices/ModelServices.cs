using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;


namespace Designerv1.Data.DataServices
{
   public interface IModelServices
    {
        IMetaModelService metaModelService { get; set; }
        ObjectModel objectModel { get; set; }
        List<Instance> GetInstances(app_object obj);
    }

    public class ModelServices : IModelServices
    {
        public IMetaModelService metaModelService { get; set; }
        public ObjectModel objectModel { get; set; }

        public ModelServices(IMetaModelService _metaModelService)
        {

            System.Diagnostics.Debug.Print("--------Inicio de carga---------------///////");
            try
            {
                metaModelService = _metaModelService;
                objectModel = new ObjectModel();
                objectModel.app_Objects.AddRange(metaModelService.Objects.Cast<dynamic>().Select(x => new app_object { Name = x.Title, object_id = x.ObjectId }));


                //objectModel.app_Objects.Where(x => x.object_id == 2022040).FirstOrDefault().instances.AddRange(metaModelService.ReadInstancesWithObjectIdForDesigner(2022040).Cast<dynamic>().Select(x => new Instance { instance_id = x.instance_id }));



                System.Diagnostics.Debug.Print("--------Objectos Cargados---------------///////");
            }catch(Exception e)
            {
                System.Diagnostics.Debug.Print("--------ERROR---------------:"+e.Message);
            }
        }

        public List<Instance> GetInstances(app_object obj)
        {
            return metaModelService.ReadInstancesWithObjectIdForDesigner(obj.object_id)
                .Cast<dynamic>()
                .Select(x => new Instance { instance_id = (Guid)x.instance_id, instance_tostring = x.instance_tostring }).ToList();
        }
    }


}
