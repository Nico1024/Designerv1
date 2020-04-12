using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Designerv1.Data;
using Designerv1.Data.DataServices;
using System.Dynamic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace Designerv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjecTreeController : ControllerBase
    {
        public IServiceProvider Provider { get; set; }
        public IMetaModelService InjectedMeta_Model { get; set; }
        public IModelServices modelServices { get; set; }
        public ObjecTreeController(IModelServices ms, IServiceProvider provider, IMetaModelService injectedService)
        {
            Provider = provider;
            InjectedMeta_Model = Provider.GetService<IMetaModelService>();
            modelServices = Provider.GetService<IModelServices>();
        }

        [HttpGet]
        public string Get() //14100
        {
            List<Js_tree_element> parent_objects = new List<Js_tree_element>();
            foreach (var item in modelServices.objectModel.app_Objects)
            {
                Js_tree_element js_Tree_Element = new Js_tree_element(item);
                foreach(var expo in InjectedMeta_Model.ReadInstancesWithObjectIdForJsTree(Convert.ToInt32(js_Tree_Element.id)))
                {
                    js_Tree_Element.children.Add(new Js_tree_element(expo));
                }
                
                parent_objects.Add(js_Tree_Element);
            }
            return JsonConvert.SerializeObject(parent_objects);        
        }

        [HttpGet("{id}", Name ="Get")]
        public string Get(string id) //14100
        {
            if (id == "#") return Get();
            else
            {
                int a = Convert.ToInt32(id);
                return JsonConvert.SerializeObject(InjectedMeta_Model.ReadInstancesWithObjectIdForJsTree(a));
            }
        }
    }
}
