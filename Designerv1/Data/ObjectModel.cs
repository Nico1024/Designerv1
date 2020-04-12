using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Dynamic;

namespace Designerv1.Data
{
    public class ObjectModel
    {
        public string Title;
        public List<app_object> app_Objects { get; set; }

        public ObjectModel()
        {
            app_Objects = new List<app_object>();
          
        }

    }

    public class Instance
    {
        public Guid     instance_id { get; set; }
        public Guid     parent_id { get; set; }
        public string   instance_tostring { get; set; }
        public Instance()
        {
            instance_id = new Guid();
            parent_id = new Guid();
            instance_tostring = "";
            
        }
    }

    public class app_object 
    {
        public int      object_id { get; set;        }
        public string   Name { get; set; }
        public string   Namespace { get; set; }
        public List<Instance> instances { get; set; }
        public List<object_attribute> Attributes { get; set; }
        public app_object()
        {
            instances = new List<Instance>();
        }
    }

    public class object_attribute
    {
        public string instance_tostring { get; set; }
    }

    public class Js_tree_element
    {
        public string id { get; set; }
        public string text { get; set; }
        public List<Js_tree_element> children { get; set; }

        public Js_tree_element(app_object _obj)
        {
            id = _obj.object_id.ToString();
            text = _obj.Name;
            children = new List<Js_tree_element>();
        }

        public Js_tree_element(Instance _ins)
        {
            id = _ins.instance_id.ToString();
            text = _ins.instance_tostring;
            children = new List<Js_tree_element>();
        }



        public Js_tree_element(ExpandoObject expo)
        {
            var expandoDict = expo as IDictionary<string, object>;
            try
            {
                id = expandoDict["id"].ToString();
                text = expandoDict["text"].ToString();
                children = new List<Js_tree_element>();
            }
            catch(Exception e)
            {
                id = expandoDict["instance_id"].ToString();
                text = expandoDict["instance_tostring"].ToString();
                children = new List<Js_tree_element>();
            }
            
        }
    }

    
}
