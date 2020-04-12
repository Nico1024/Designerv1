using System;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Designerv1.Data.DataServices
{
    public interface IMetaModelService
    {
        string Name { get; set; }
        string Database { get; set; }
        List<ExpandoObject> Objects { get; set; }
        string connectionString { get; set; }

        public ExpandoObject GetInstance(Guid instance_id, ExpandoObject expando);
        public ExpandoObject ReadObject(int object_id);

        public List<ExpandoObject> ReadInstancesWithObjectId(int object_id);
        public List<ExpandoObject> ReadInstancesWithObjectIdForDesigner(int object_id);
        public List<ExpandoObject> ReadInstancesWithObjectIdForJsTree(int object_id);
        public List<ExpandoObject> ReadInstancesFrom(ExpandoObject expando);
        Task<List<ExpandoObject>>  ReadAll(ExpandoObject expando);
        
    }


    public class MetaModelServices : IMetaModelService
    {
        public static Guid guid { get; set; }
        public string Name { get; set; }
        public string Database { get; set; }
        public List<ExpandoObject> Objects { get; set; }
        public string connectionString { get; set; }

        public MetaModelServices()
        {
            Objects = new List<ExpandoObject>();
            Name = "OrganisatieModel";
            Database = "DELTA_OrganisatieModel";
            connectionString =
                "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;"
                + "User Id=sa; Password=Gibson1!";
            guid = Guid.NewGuid();
            LoadObjects();
            LoadProperties();
            SetInstance_tostring();
        }

        public ExpandoObject ReadObject(int object_id)
        {
            if (object_id != 0)
            {
                return Objects.Cast<dynamic>().Where(x => x.ObjectId == object_id).FirstOrDefault();
            }
            else return null;
        }
        public async Task<List<ExpandoObject>> ReadAll(ExpandoObject expando)
        {
            return await ReadAllInstancesAsJson(expando);
        }
        public async Task<List<ExpandoObject>> ReadAllInstancesAsJson(ExpandoObject expando)
        {
            /*expando is a meta object definition*/
            var expandoDict = expando as IDictionary<string, object>;
            string table_name = expandoDict["Table"].ToString();
            object value = null;
            expandoDict.TryGetValue("ListOfProperties", out value);

            // Provide the query string with a parameter placeholder.
            string queryString = @"SELECT instance_id as instance_id";

            foreach (var item in value as List<ExpandoObject>)
            {
                if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                    queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + " as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value;
                else
                {//i need the id in order to retrieve the whole object
                    if (!(bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                        queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id";
                }

            }

            queryString += @" FROM [DELTA_OrganisatieModel].[dbo].[" + table_name + "]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                ExpandoObject instance = new ExpandoObject();
                List<ExpandoObject> instances = new List<ExpandoObject>();
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        instance = new ExpandoObject();
                        AddProperty(instance, "instance_id", reader[0]);
                        foreach (var item in value as List<ExpandoObject>)
                        {

                            if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                            {
                                //Console.WriteLine(reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
                                AddProperty(
                                    instance,
                                    item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
                                    reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
                                instances.Add(instance);

                            }
                            else
                            {
                                var a = reader[0];
                                //if its a collection                                
                                if ((bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                                {

                                }
                                else //if its just an object
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id")))
                                    {
                                        var new_obj = GetInstance(
                                                (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"],
                                                Objects.Cast<dynamic>().Where(
                                                    x => x.ObjectId == (int)item.Where(
                                                        x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault()
                                                );
                                        AddProperty(
                                            instance,
                                            item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
                                            new_obj
                                        );

                                        instances.Add(instance);
                                    }
                                }
                            }
                        }

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                }
                return instances;
            }
        }
        public List<ExpandoObject> ReadInstancesFrom(ExpandoObject expando)
        {
            /*expando is a meta object definition*/
            var expandoDict = expando as IDictionary<string, object>;
            string table_name = expandoDict["Table"].ToString();
            object value = null;
            expandoDict.TryGetValue("ListOfProperties", out value);

            // Provide the query string with a parameter placeholder.
            string queryString = @"SELECT instance_id as instance_id";

            foreach (var item in value as List<ExpandoObject>)
            {
                if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                    queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + " as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value;
                else
                {//i need the id in order to retrieve the whole object
                    if (!(bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                        queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id";
                }

            }

            queryString += @" FROM [DELTA_OrganisatieModel].[dbo].[" + table_name + "]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                ExpandoObject instance = new ExpandoObject();
                List<ExpandoObject> instances = new List<ExpandoObject>();
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        instance = new ExpandoObject();
                        AddProperty(instance, "instance_id", reader[0]);
                        foreach (var item in value as List<ExpandoObject>)
                        {

                            if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                            {
                                //Console.WriteLine(reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
                                AddProperty(
                                    instance,
                                    item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
                                    reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
                                instances.Add(instance);

                            }
                            else
                            {
                                var a = reader[0];
                                //if its a collection                                
                                if ((bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                                {

                                }
                                else //if its just an object
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id")))
                                    {
                                        var new_obj = GetInstance(
                                                (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"],
                                                Objects.Cast<dynamic>().Where(
                                                    x => x.ObjectId == (int)item.Where(
                                                        x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault()
                                                );
                                        AddProperty(
                                            instance,
                                            item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
                                            new_obj
                                        );

                                        instances.Add(instance);
                                    }
                                }
                            }
                        }

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    ExpandoObject err_obj = new ExpandoObject();
                    AddProperty(err_obj, "Error", ex.Message);
                    instances.Add(err_obj);                    
                    //Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                }
                return instances;
            }
        }
        public List<ExpandoObject> ReadInstancesWithObjectId(int object_id)
        {
            /*expando is a meta object definition*/
            
            ExpandoObject expando = new ExpandoObject();
            List<ExpandoObject> instances = new List<ExpandoObject>();
            //System.Diagnostics.Debug.Print("inicio con object_id:" + object_id + " - - " + instances.Count());
            expando = Objects.Cast<dynamic>().Where(x => x.ObjectId == object_id).FirstOrDefault(); 
            var expandoDict = expando as IDictionary<string, object>;
            string table_name = expandoDict["Table"].ToString();
            object ListOfProperties = null;
            expandoDict.TryGetValue("ListOfProperties", out ListOfProperties);

            if (tableisvalid(table_name))
            {
                string queryString = getQuery(table_name, ListOfProperties, is_set_instance_tostring(expando));
                string connectionString = "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;User Id=sa; Password=Gibson1!";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    ExpandoObject instance = new ExpandoObject();
                    
                    SqlCommand command = new SqlCommand(queryString, connection);
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        int i = 0;
                        while (reader.Read())
                        {
                            //System.Diagnostics.Debug.Print("i:"+i+"|ob_id:"+object_id); i++;
                            instances.Add(createNewInstance(reader, ListOfProperties, is_set_instance_tostring(expando)));
                            /*
                            //instance = new ExpandoObject();
                            //AddProperty(instance, "instance_id", reader[0]);
                            //foreach (var item in ListOfProperties as List<ExpandoObject>)
                            //{
                            //    if (item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() != "sql_statement")
                            //    if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                            //    {
                            //        int idx = reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString());
                            //        //System.Diagnostics.Debug.Print("Not Object- Value:"+reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()].ToString()+"---column:"+ item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()+"--idx:"+idx);
                            //        if (idx > 0)
                            //        {
                            //            AddProperty(
                            //              instance,
                            //              item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
                            //              reader[idx]);
                            //        }
                            //        instances.Add(instance);
                            //        //System.Diagnostics.Debug.Print("------- not object - x" + object_id + " - - " + instances.Count());

                            //    }
                            //    else
                            //    {
                            //        var a = reader[0];
                            //        //if its a collection                                
                            //        if ((bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                            //        {

                            //        }
                            //        else //if its just an object
                            //        {
                            //            if (!reader.IsDBNull(reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id")))
                            //            {
                            //                //System.Diagnostics.Debug.Print("-------Object Inicio---------" + reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"]+"="+ item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id");
                            //                //var new_obj = GetInstance(
                            //                //        (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"],
                            //                //        Objects.Cast<dynamic>().Where(
                            //                //            x => x.ObjectId == (int)item.Where(
                            //                //                x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault()
                            //                //        );
                            //                ExpandoObject asd = Objects.Cast<dynamic>().Where(
                            //                            x => x.ObjectId == (int)item.Where(
                            //                                x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault();
                            //                Guid guid = (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"];
                            //                //System.Diagnostics.Debug.Print("-------Object Fin---------" + reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"]);

                            //                var r = GetInstance(guid, asd);
                            //                    AddProperty(
                            //                        instance,
                            //                        asd.ToString(),
                            //                        guid
                            //                    );

                            //                    object value3;
                            //                    var expandoDict3 = r as IDictionary<string, object>;
                            //                    expandoDict3.TryGetValue("Title", out value3);

                            //                    if (value3 == null) //in case the Title is not set
                            //                    {

                            //                        expandoDict3.TryGetValue("instance_id", out value3);
                            //                        //System.Diagnostics.Debug.Print("Adding Title to Instance" + value2);
                            //                        AddProperty(r, "Title", value3.ToString());
                            //                    }

                            //                    instances.Add(r);
                            //                    System.Diagnostics.Debug.Print("-------just object - " + object_id + " - - " + instances.Count());
                            //            }
                            //        }
                            //    }
                            //}
                            
                            //object value2;
                            //var expandoDict2 = instance as IDictionary<string, object>;
                            //expandoDict2.TryGetValue("Title", out value2);
                            
                            //if (value2 == null) //in case the Title is not set
                            //{
                                
                            //    expandoDict2.TryGetValue("instance_id", out value2);
                            //    //System.Diagnostics.Debug.Print("Adding Title to Instance" + value2);
                            //    AddProperty(instance, "Title", value2.ToString());
                            //}
                            //System.Diagnostics.Debug.Print("Title added:"+value2+" "+JsonConvert.SerializeObject(instance));
                            */
                            //instances.Add(instance);
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        ExpandoObject err_obj = new ExpandoObject();
                        AddProperty(err_obj, "instance_id", Guid.Empty);
                        AddProperty(err_obj, "Title", "Error reading this instance");
                        AddProperty(err_obj, "Error", ex.Message);
                        instances.Add(err_obj);
                        //System.Diagnostics.Debug.Print("-------e"+object_id + " - - " + instances.Count());
                        //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(instance));
                        return instances;
                    }
                    //System.Diagnostics.Debug.Print("-------ne" + object_id+" - - "+instances.Count());
                    //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(instances));
                    return instances;
                }
            }

            //System.Diagnostics.Debug.Print("------->" + object_id + " - - " + instances.Count());
            return instances;
        }
        public List<ExpandoObject> ReadInstancesWithObjectIdForDesigner(int object_id)
        {
            /*expando is a meta object definition*/

            ExpandoObject expando = new ExpandoObject();
            ExpandoObject instance_tostring_prop = new ExpandoObject();

            List<ExpandoObject> instances = new List<ExpandoObject>();
            object ListOfProperties = null;            
            expando = Objects.Cast<dynamic>().Where(x => x.ObjectId == object_id).FirstOrDefault();
            var expandoDict = expando as IDictionary<string, object>;
            string table_name = expandoDict["Table"].ToString();
            

            if (tableisvalid(table_name))
            {
                expandoDict.TryGetValue("ListOfProperties", out ListOfProperties);
                //AddProperty(instance_tostring_prop, "instance_tostring", "");
                string queryString = getQuery(table_name, null, is_set_instance_tostring(expando));
                string connectionString = "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;User Id=sa; Password=Gibson1!";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    ExpandoObject instance = new ExpandoObject();

                    SqlCommand command = new SqlCommand(queryString, connection);
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        int i = 0;
                        while (reader.Read())
                        {
                            //System.Diagnostics.Debug.Print("i:" + i + "|ob_id:" + object_id); i++;
                            instances.Add(createNewInstance(reader, null, is_set_instance_tostring(expando)));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        ExpandoObject err_obj = new ExpandoObject();
                        AddProperty(err_obj, "instance_id", Guid.Empty);
                        AddProperty(err_obj, "instance_tostring", ex.Message);
                        AddProperty(err_obj, "Error", ex.Message);
                        instances.Add(err_obj);
                        //System.Diagnostics.Debug.Print("-------e"+object_id + " - - " + instances.Count());
                        //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(instance));
                        return instances;
                    }
                    //System.Diagnostics.Debug.Print("-------ne" + object_id+" - - "+instances.Count());
                    //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(instances));
                    return instances;
                }
            }
            else
            {
                ExpandoObject err_obj = new ExpandoObject();
                AddProperty(err_obj, "instance_id", Guid.Empty);
                AddProperty(err_obj, "instance_tostring", "Error reading this instance");
                AddProperty(err_obj, "Error", "Table not valid");
                instances.Add(err_obj);
                //System.Diagnostics.Debug.Print("------->" + object_id + " - - " + instances.Count());
                return instances;
            }
        }
        public List<ExpandoObject> ReadInstancesWithObjectIdForJsTree(int object_id)
        {
            /*expando is a meta object definition*/

            ExpandoObject expando = new ExpandoObject();
            ExpandoObject instance_tostring_prop = new ExpandoObject();

            List<ExpandoObject> instances = new List<ExpandoObject>();
            object ListOfProperties = null;
            expando = Objects.Cast<dynamic>().Where(x => x.ObjectId == object_id).FirstOrDefault();
            var expandoDict = expando as IDictionary<string, object>;
            string table_name = expandoDict["Table"].ToString();


            if (tableisvalid(table_name))
            {
                expandoDict.TryGetValue("ListOfProperties", out ListOfProperties);
                //AddProperty(instance_tostring_prop, "instance_tostring", "");
                string queryString = getQuery(table_name, null, is_set_instance_tostring(expando));
                string connectionString = "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;User Id=sa; Password=Gibson1!";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    ExpandoObject instance;

                    SqlCommand command = new SqlCommand(queryString, connection);
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        int i = 0;
                        while (reader.Read())
                        {
                            //System.Diagnostics.Debug.Print("i:" + i + "|ob_id:" + object_id); i++;
                            if (is_set_instance_tostring(expando))
                            {
                                instance = new ExpandoObject();
                                AddProperty(instance, "id", reader[0].ToString());
                                AddProperty(instance, "text", reader["instance_tostring"].ToString());
                                instances.Add(instance);
                            }
                            
                            //instances.Add(createNewInstance(reader, null, is_set_instance_tostring(expando)));
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        ExpandoObject err_obj = new ExpandoObject();
                        AddProperty(err_obj, "id", Guid.Empty.ToString());
                        AddProperty(err_obj, "text", ex.Message);
                        AddProperty(err_obj, "Error", ex.Message);
                        instances.Add(err_obj);
                        //System.Diagnostics.Debug.Print("-------e"+object_id + " - - " + instances.Count());
                        //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(instance));
                        return instances;
                    }
                    //System.Diagnostics.Debug.Print("-------ne" + object_id+" - - "+instances.Count());
                    //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(instances));
                    return instances;
                }
            }
            else
            {
                ExpandoObject err_obj = new ExpandoObject();
                AddProperty(err_obj, "instance_id", Guid.Empty);
                AddProperty(err_obj, "instance_tostring", "Error reading this instance");
                AddProperty(err_obj, "Error", "Table not valid");
                instances.Add(err_obj);
                //System.Diagnostics.Debug.Print("------->" + object_id + " - - " + instances.Count());
                return instances;
            }
        }



        public  ExpandoObject GetInstance(Guid instance_id, ExpandoObject expando)
        {
            ExpandoObject newinstance = new ExpandoObject();            
            object ListOfProperties = null;
            //try
            //{
                var expandoDict = expando as IDictionary<string, object>;
                string table_name = expandoDict["Table"].ToString();
                expandoDict.TryGetValue("ListOfProperties", out ListOfProperties);
                //Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)item.Where(x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault();

                if (tableisvalid(table_name))
                {
                    string queryString = getQuery(table_name, ListOfProperties, is_set_instance_tostring(expando))+" WHERE instance_id ='"+instance_id+"'";
                    string connectionString = "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;User Id=sa; Password=Gibson1!";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);
                        //try{
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                newinstance = createNewInstance(reader, ListOfProperties, is_set_instance_tostring(expando));
                            }
                            reader.Close();
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                        //}
                        return newinstance;

                    }
                }
                else
                {
                    //System.Diagnostics.Debug.Print("no table:"+table_name+"--**--"+instance_id);
                    /*if there's not any table return the base object, if no inheritance return Empty */
                    object inherit = null;
                    expandoDict.TryGetValue("Inherits", out inherit);
                    if (inherit != null)
                    {
                        return GetInstance(instance_id, Objects.Cast<dynamic>().Where(
                            x => x.ObjectId == (int)inherit).FirstOrDefault());
                    }
                    else return instanceNotValid(instance_id, expando, "Table not valid and no inheritance");
                }
            //}
            //catch(Exception e)
            //{
            //    //System.Diagnostics.Debug.Print("GetInstance Error: "+e.Message);
            //    ExpandoObject err_obj = new ExpandoObject();
            //    AddProperty(err_obj, "instance_id", instance_id);
            //    AddProperty(err_obj, "instance_tostring", "Error reading this instance:"+instance_id);
            //    AddProperty(err_obj, "Error", e.Message);

            //    return err_obj;
            //}

        }
        private ExpandoObject instanceNotValid(Guid instance_id, ExpandoObject expando, string message)
        {
            ExpandoObject err_obj = new ExpandoObject();
            var expandoDict = expando as IDictionary<string, object>;
            int object_id = (int)expandoDict["ObjectId"];

            AddProperty(err_obj, "instance_id", instance_id);
            AddProperty(err_obj, "instance_tostring", "Error reading this instance:" + instance_id);
            AddProperty(err_obj, "Error", message);
            AddProperty(err_obj, "ObjectId", object_id);

            return err_obj;
        }
        private ExpandoObject createNewInstance(SqlDataReader reader, object ListOfProperties, bool is_set_instance_tostring)
        {
            ExpandoObject newinstance = new ExpandoObject();
            AddProperty(newinstance, "instance_id", reader[0]);
            if (ListOfProperties != null)
            {                
                foreach (var item in ListOfProperties as List<ExpandoObject>)
                {

                    if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                    {
                        int idx = reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString());
                        //System.Diagnostics.Debug.Print("Not Object- Value:"+reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()].ToString()+"---column:"+ item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()+"--idx:"+idx);
                        if (idx > 0)
                        {
                            AddProperty(
                                newinstance,
                                item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
                                reader[idx]);
                        }
                    }
                    else
                    {
                        //if its a collection                                
                        if ((bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                        {

                        }
                        else //if its just an object
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id")))
                            {
                                var prop_object_id = item.Where(x => x.Key == "prop_type_id").FirstOrDefault().Value;
                                ExpandoObject asd = Objects.Cast<dynamic>().Where(
                                                x => x.ObjectId == (int)prop_object_id).FirstOrDefault();
                                Guid guid = (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"];
                                System.Diagnostics.Debug.Print("-------Object Fin---------" + reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"]);
                                System.Diagnostics.Debug.Print("-----------"+item.Where(x => x.Key == "prop_type_id").FirstOrDefault().Value.ToString());
                                if (asd != null)
                                {
                                    var r = GetInstance(guid, asd);
                                    AddProperty(
                                        newinstance,
                                        asd.ToString(),
                                        guid
                                    );
                                }
                                

                                //object value3;
                                //var expandoDict3 = r as IDictionary<string, object>;
                                //expandoDict3.TryGetValue("Title", out value3);

                                //if (value3 == null) //in case the Title is not set
                                //{

                                //    expandoDict3.TryGetValue("instance_id", out value3);
                                //    //System.Diagnostics.Debug.Print("Adding Title to Instance" + value2);
                                //    AddProperty(r, "Title", value3.ToString());
                                //}

                            }
                        }
                    }
                }
                checkInstanceToStringProperty(newinstance, reader, is_set_instance_tostring);
                return newinstance;
            }
            else
            {
                checkInstanceToStringProperty(newinstance, reader, is_set_instance_tostring);
                checkInstanceIdProperty(newinstance, reader);
                return newinstance;
            }
           
            
        }




        #region Loaders
        public void LoadObjects()
        {
            string connectionString =
                "Server=localhost;Initial Catalog=DELTA_OrganisatieModel;"
                + "User Id=sa; Password=Gibson1!";

            // Provide the query string with a parameter placeholder.
            string queryString =
                @"SELECT   [object_id]
                          ,[inherits_id]
                          ,[object_code]     
                          ,[code_name]
                          ,[code_namespace]
                          ,[object_is_deprecated]
                          ,[object_modifier]
                          ,[table_name]
                      FROM [DELTA_OrganisatieModel].[dbo].[System.Object]
                        where object_is_deprecated = 0 and table_name is not null";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var new_object = new ExpandoObject();
                        //if ((int)reader[0] == 3200) Console.WriteLine(reader[0]);
                        AddProperty(new_object, "Inherits", reader[1]);
                        AddProperty(new_object, "Title", reader[2]);
                        AddProperty(new_object, "ObjectId", reader[0]);
                        AddProperty(new_object, "Namespace", reader[4]);
                        AddProperty(new_object, "Table", "uvw."+reader[7]);
                        AddProperty(new_object, "ListOfProperties", new List<ExpandoObject>());
                        if ((int)reader[0]==11020)
                        {

                        }
                        if (LoadObjectsAttributes(new_object)) Objects.Add(new_object);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                }
            }


        }
        public bool LoadObjectsAttributes(ExpandoObject expando)
        {
            bool set_instance_tostring = false;
            var expandoDict = expando as IDictionary<string, object>;
            int object_id = (int)expandoDict["ObjectId"];
            string queryString =
                @"SELECT   [attr_value] as [attr_value]
                      FROM [DELTA_OrganisatieModel].[dbo].[System.Object.Attribute]
                        where object_id = "+object_id+" and attr_code='data.tostring.sql' ";

            using (SqlConnection connection =
                 new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        List<ExpandoObject> list_of_attributes = new List<ExpandoObject>();
                        ExpandoObject attr = new ExpandoObject();
                        //if ((int)reader[0] == 3200) Console.WriteLine(reader[0]);
                        AddProperty(attr, "instance_tostring", reader[0]);

                        list_of_attributes.Add(attr);
                        AddProperty(expando, "Attributes", list_of_attributes);
                        set_instance_tostring = true;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                }
            }
            return set_instance_tostring;

        }
        public void LoadProperties()
        {
            string connectionString =
                "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;"
                + "User Id=sa; Password=Gibson1!";

            // Provide the query string with a parameter placeholder.
            string queryString =
                @"SELECT [object_id]
                        ,[prop_code]
                        ,[prop_display]
                        ,[prop_object_id]
                        ,[prop_length]
                        ,[prop_sequence]
                        ,[prop_is_required]
                        ,[prop_is_collection]
                        ,[prop_is_relation]
                        ,[prop_is_active]
                        ,[code_name]
                    FROM [DELTA_OrganisatieModel].[dbo].[System.Object.Property]
                    ";

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)reader[0]).Any())
                        {
                            var obj = new ExpandoObject();
                            AddProperty(obj, "prop_code", reader[1]);
                            AddProperty(obj, "prop_display", reader[2]);
                            AddProperty(obj, "prop_type", getPropertyType(reader[3]));
                            AddProperty(obj, "prop_type_id", reader[3]);
                            if (reader[4] != null) AddProperty(obj, "prop_length", reader[4]);
                            AddProperty(obj, "prop_is_required", reader[6]);
                            AddProperty(obj, "prop_is_collection", reader[7]);
                            //if ((int)reader[0]==2022022)
                            //    Console.WriteLine(reader[3]);
                            object value = null;
                            //expandoDict.TryGetValue("ListOfProperties", out value);
                            //AddProperty(Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)reader[0]).FirstOrDefault(), "ListOfProperties", obj);
                            var a = Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)reader[0]).FirstOrDefault() as IDictionary<string, object>;
                            a.TryGetValue("ListOfProperties", out value);
                            var b = (List<ExpandoObject>)value;
                            b.Add(obj);

                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                }
            }


        }
        public void SetInstance_tostring()
        {
            
            foreach(var obj  in Objects )
            {
                object value = null;
                var a = obj as IDictionary<string, object>;
                a.TryGetValue("ObjectId", out value);
                if (value != null)
                {
                    if ((int)value == 11020)
                    {

                    }
                }


                a.TryGetValue("ListOfProperties", out value);

                

                foreach(var prop in (List<ExpandoObject>)value)
                {
                    if (prop.Where(x => x.Key == "prop_code").FirstOrDefault().Value.Equals("name"))
                    {
                        AddProperty(prop, "instance_tostring", true);
                    }
                    else if (prop.Where(x => x.Key == "prop_code").FirstOrDefault().Value.Equals("code"))
                    {
                        AddProperty(prop, "instance_tostring", true);
                    }
                    else if (prop.Where(x => x.Key == "prop_code").FirstOrDefault().Value.Equals("display"))
                    {
                        AddProperty(prop, "instance_tostring", true);
                    }
                }
            }
           
        }

        #endregion

        #region Helpers
        private object getPropertyType(object v)
        {
            switch (v)
            {
                case 100: return "";
                case 101: return "";
                case 110: return default(Int64);
                case 111: return default(Int32);
                case 112: return default(Int16);
                case 113: return default(byte);
                case 114: return default(Boolean);
                case 115: return default(Decimal);
                case 120: return default(DateTime);
                case 121: return default(DateTime);
                case 122: return default(DateTime);
                case 123: return default(DateTime);
                case 150: return default(Int64);//MOney
                case 151: return default(Int64);//Small money
                case 160: return default(Guid);
                case 190:return default(Byte[]);
                case 199: return "Object";
                default: return "Object";
            }
        }
        private bool tableisvalid(string table_name)
        {
            bool result = false;
            string queryString = "USE [" + Database + "] select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '" + table_name + "'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if ((int)reader[0] > 0)
                            result = true;
                        else result = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
                }
                return result;
            }
        }
        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
        public string getInstanceToString(ExpandoObject expando)
        {
            object value2, value3;
            var expandoDict = expando as IDictionary<string, object>;
            expandoDict.TryGetValue("Attributes", out value2);

            var expandoDict3 = value2 as IDictionary<string, object>;
            expandoDict3.TryGetValue("instance_tostring", out value3);

            return (string)value3;
        }
        public bool is_set_instance_tostring(ExpandoObject expando)
        {
            object value2=null, value3=null;
            var expandoDict = expando as IDictionary<string, object>;
            expandoDict.TryGetValue("Attributes", out value2);
            foreach (dynamic item in value2 as List<ExpandoObject>)
            {
                if (item.instance_tostring.Length > 0) return true;
            }
            return true;
        }
        private void checkInstanceIdProperty(ExpandoObject r, SqlDataReader reader)
        {
            object value3;
            var expandoDict3 = r as IDictionary<string, object>;
            expandoDict3.TryGetValue("instance_id", out value3);

            if (value3 == null) //in case the instance_tostring is not set
            {
                expandoDict3.TryGetValue("instance_id", out value3);

                AddProperty(r, "instance_id", reader[0]);
            }
        }
        private void checkInstanceToStringProperty(ExpandoObject r, SqlDataReader reader, bool is_set_instance_tostring)
        {
            if (is_set_instance_tostring)
            {
                object value3;
                string instance_tostring;
                var expandoDict3 = r as IDictionary<string, object>;
                expandoDict3.TryGetValue("instance_tostring", out value3);
                //System.Diagnostics.Debug.Print(JsonConvert.SerializeObject(reader[0]) + "-->");
                if (value3 == null) //in case the instance_tostring is not set
                {
                    try
                    {
                        var a = reader.GetOrdinal("instance_tostring");
                        //System.Diagnostics.Debug.Print(reader[0].ToString());
                        instance_tostring = reader[a].ToString();
                    }
                    catch
                    {
                        try
                        {
                            instance_tostring = reader["[name]"].ToString();
                        }
                        catch
                        {
                            try
                            {
                                instance_tostring = reader["[code]"].ToString();
                            }
                            catch
                            {
                                try
                                {
                                    instance_tostring = reader["[display]"].ToString();
                                }
                                catch
                                {
                                    instance_tostring = reader[0].ToString();
                                }
                            }
                        }
                    }
                    AddProperty(r, "instance_tostring", instance_tostring);
                }
            }
        }
        private string getQuery(string table_name, object properties, bool is_set_instance_tostring)
        {

            string queryString = @"SELECT instance_id as instance_id";
            if (is_set_instance_tostring)
            {
                queryString += ",[instance_tostring] as [instance_tostring]";
            }

            if (properties != null)
            {
                foreach (var item in properties as List<ExpandoObject>)
                {
                    //for documents there no prop_type
                    if (item.Where(x => x.Key == "prop_type").FirstOrDefault().Value != null)
                    {
                        if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
                            queryString += ",[" + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "] as [" + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "]";
                        else
                        {//i need the id in order to retrieve the whole object
                            if (!(bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
                                queryString += ",[" + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id] as [" + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id]";
                        }
                    }
                }
            }



            queryString += @" FROM [DELTA_OrganisatieModel].[dbo].[" + table_name + "]";
            return queryString;
        }

        #endregion
    }
}

