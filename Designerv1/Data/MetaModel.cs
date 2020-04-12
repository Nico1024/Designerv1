using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Designerv1.Data
{
    public class MetaModel
    {
        public string Name { get; set; }
        public string Database;
        public List<ExpandoObject> Objects;
        public string connectionString;

        public MetaModel(string _name)
        {
            Objects = new List<ExpandoObject>();
            Name = _name;
            Database = "DELTA_OrganisatieModel";
            connectionString =
                "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;"
                + "User Id=sa; Password=Gibson1!";
        }

        //public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        //{
        //    // ExpandoObject supports IDictionary so we can extend it like this
        //    var expandoDict = expando as IDictionary<string, object>;
        //    if (expandoDict.ContainsKey(propertyName))
        //        expandoDict[propertyName] = propertyValue;
        //    else
        //        expandoDict.Add(propertyName, propertyValue);
        //}

        //public string ReadObject(ExpandoObject expando, Guid guid)
        //{
        //    var expandoDict = expando as IDictionary<string, object>;
        //    string table_name = expandoDict["Table"].ToString();
        //    object value = null;
        //    expandoDict.TryGetValue("ListOfProperties", out value);

        //    string connectionString =
        //        "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;"
        //        + "User Id=sa; Password=Gibson1!";

        //    // Provide the query string with a parameter placeholder.
        //    string queryString = @"SELECT instance_id as instance_id";

        //    foreach (var item in value as List<ExpandoObject>)
        //    {
        //        if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
        //            queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + " as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value;
        //    }

        //    queryString += @" FROM [DELTA_OrganisatieModel].[dbo].[uvw." + table_name + "] where instance_id = '" + guid + "' ";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        ExpandoObject instance = new ExpandoObject();
        //        SqlCommand command = new SqlCommand(queryString, connection);
        //        try
        //        {
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                foreach (var item in value as List<ExpandoObject>)
        //                {
        //                    if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
        //                        AddProperty(
        //                            instance,
        //                            item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
        //                            reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
        //                }
        //            }
        //            reader.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
        //        }
        //        return JsonConvert.SerializeObject(instance);
        //    }
        //}


        //public string ReadAllInstancesAsJson(ExpandoObject expando)
        //{
        //    /*expando is a meta object definition*/
        //    var expandoDict = expando as IDictionary<string, object>;
        //    string table_name = expandoDict["Table"].ToString();
        //    object value = null;
        //    expandoDict.TryGetValue("ListOfProperties", out value);

        //    // Provide the query string with a parameter placeholder.
        //    string queryString = @"SELECT instance_id as instance_id";

        //    foreach (var item in value as List<ExpandoObject>)
        //    {
        //        if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
        //            queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + " as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value;
        //        else
        //        {//i need the id in order to retrieve the whole object
        //            if (!(bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
        //                queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id";
        //        }

        //    }

        //    queryString += @" FROM [DELTA_OrganisatieModel].[dbo].[" + table_name + "]";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        ExpandoObject instance = new ExpandoObject();
        //        List<ExpandoObject> instances = new List<ExpandoObject>();
        //        SqlCommand command = new SqlCommand(queryString, connection);
        //        try
        //        {
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                instance = new ExpandoObject();
        //                AddProperty(instance, "instance_id", reader[0]);
        //                foreach (var item in value as List<ExpandoObject>)
        //                {

        //                    if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
        //                    {
        //                        //Console.WriteLine(reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
        //                        AddProperty(
        //                            instance,
        //                            item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
        //                            reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
        //                        instances.Add(instance);

        //                    }
        //                    else
        //                    {
        //                        var a = reader[0];
        //                        //if its a collection                                
        //                        if ((bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
        //                        {

        //                        }
        //                        else //if its just an object
        //                        {
        //                            if (!reader.IsDBNull(reader.GetOrdinal(item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id")))
        //                            {
        //                                var new_obj = GetInstance(
        //                                        (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"],
        //                                        Objects.Cast<dynamic>().Where(
        //                                            x => x.ObjectId == (int)item.Where(
        //                                                x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault()
        //                                        );
        //                                AddProperty(
        //                                    instance,
        //                                    item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
        //                                    new_obj
        //                                );

        //                                instances.Add(instance);
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //            reader.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
        //        }
        //        return JsonConvert.SerializeObject(instances, Formatting.Indented);
        //    }
        //}

        //private ExpandoObject GetInstance(Guid instance_id, ExpandoObject expando)
        //{
        //    //Inherits
        //    //Console.WriteLine(JsonConvert.SerializeObject(expando, Formatting.Indented));
        //    ExpandoObject newinstance = new ExpandoObject();
        //    var expandoDict = expando as IDictionary<string, object>;
        //    string table_name = expandoDict["Table"].ToString();
        //    object value = null;
        //    expandoDict.TryGetValue("ListOfProperties", out value);
        //    //Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)item.Where(x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault();

        //    if (tableisvalid(table_name))
        //    {
        //        /*
        //                return a new instance with all properties
        //         */
        //        #region query
        //        string connectionString = "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;User Id=sa; Password=Gibson1!";

        //        // Provide the query string with a parameter placeholder.
        //        string queryString = @"SELECT instance_id as instance_id";

        //        foreach (var item in value as List<ExpandoObject>)
        //        {
        //            if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
        //                queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + " as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value;
        //            else
        //            {//i need the id in order to retrieve the whole object
        //                if (!(bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
        //                    queryString += "," + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id as " + item.Where(x => x.Key == "prop_code").FirstOrDefault().Value + "_id";
        //            }

        //        }

        //        queryString += @" FROM [DELTA_OrganisatieModel].[dbo].[" + table_name + "]";

        //        #endregion
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            SqlCommand command = new SqlCommand(queryString, connection);
        //            try
        //            {
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    AddProperty(newinstance, "instance_id", reader[0]);
        //                    foreach (var item in value as List<ExpandoObject>)
        //                    {
        //                        if (!item.Where(x => x.Key == "prop_type").FirstOrDefault().Value.Equals("Object"))
        //                        {
        //                            AddProperty(
        //                                newinstance,
        //                                item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
        //                                reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString()]);
        //                        }
        //                        else
        //                        {
        //                            var a = reader[0];
        //                            //if its a collection                                
        //                            if ((bool)item.Where(x => x.Key == "prop_is_collection").FirstOrDefault().Value)
        //                            {

        //                            }
        //                            else //if its just an object
        //                            {
        //                                AddProperty(
        //                                    newinstance,
        //                                    item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString(),
        //                                    GetInstance(
        //                                        (Guid)reader[item.Where(x => x.Key == "prop_code").FirstOrDefault().Value.ToString() + "_id"],
        //                                        Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)item.Where(x => x.Key == "prop_type_id").FirstOrDefault().Value).FirstOrDefault()
        //                                        )
        //                                );
        //                            }
        //                        }
        //                    }

        //                }
        //                reader.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
        //            }
        //            return newinstance;

        //        }
        //    }
        //    else
        //    {
        //        /*if there's not any table return the base object */
        //        object inherit = null;
        //        expandoDict.TryGetValue("Inherits", out inherit);
        //        return GetInstance(instance_id, Objects.Cast<dynamic>().Where(
        //            x => x.ObjectId == (int)inherit).FirstOrDefault());
        //    }

        //}

        //public void LoadObjects()
        //{
        //    string connectionString =
        //        "Server=localhost;Initial Catalog=DELTA_OrganisatieModel;"
        //        + "User Id=sa; Password=Gibson1!";

        //    // Provide the query string with a parameter placeholder.
        //    string queryString =
        //        @"SELECT   [object_id]
        //                  ,[inherits_id]
        //                  ,[object_code]     
        //                  ,[code_name]
        //                  ,[code_namespace]
        //                  ,[object_is_deprecated]
        //                  ,[object_modifier]
        //                  ,[table_name]
        //              FROM [DELTA_OrganisatieModel].[dbo].[System.Object] where object_is_deprecated = 0 and table_name is not null";

        //    using (SqlConnection connection =
        //        new SqlConnection(connectionString))
        //    {
        //        // Create the Command and Parameter objects.
        //        SqlCommand command = new SqlCommand(queryString, connection);

        //        try
        //        {
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                var new_object = new ExpandoObject();
        //                //if ((int)reader[0] == 3200) Console.WriteLine(reader[0]);
        //                AddProperty(new_object, "Inherits", reader[1]);
        //                AddProperty(new_object, "Title", reader[2]);
        //                AddProperty(new_object, "ObjectId", reader[0]);
        //                AddProperty(new_object, "Namespace", reader[4]);
        //                AddProperty(new_object, "Table", reader[7]);
        //                AddProperty(new_object, "ListOfProperties", new List<ExpandoObject>());
        //                Objects.Add(new_object);
        //            }
        //            reader.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
        //        }
        //    }


        //}

        //public void LoadProperties()
        //{
        //    string connectionString =
        //        "Data Source=(local);Initial Catalog=DELTA_OrganisatieModel;"
        //        + "User Id=sa; Password=Gibson1!";

        //    // Provide the query string with a parameter placeholder.
        //    string queryString =
        //        @"SELECT [object_id]
        //                ,[prop_code]
        //                ,[prop_display]
        //                ,[prop_object_id]
        //                ,[prop_length]
        //                ,[prop_sequence]
        //                ,[prop_is_required]
        //                ,[prop_is_collection]
        //                ,[prop_is_relation]
        //                ,[prop_is_active]
        //                ,[code_name]
        //            FROM [DELTA_OrganisatieModel].[dbo].[System.Object.Property]
        //            ";

        //    using (SqlConnection connection =
        //        new SqlConnection(connectionString))
        //    {
        //        // Create the Command and Parameter objects.
        //        SqlCommand command = new SqlCommand(queryString, connection);

        //        try
        //        {
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                if (Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)reader[0]).Any())
        //                {
        //                    var obj = new ExpandoObject();
        //                    AddProperty(obj, "prop_code", reader[1]);
        //                    AddProperty(obj, "prop_display", reader[2]);
        //                    AddProperty(obj, "prop_type", getPropertyType(reader[3]));
        //                    AddProperty(obj, "prop_type_id", reader[3]);
        //                    if (reader[4] != null) AddProperty(obj, "prop_length", reader[4]);
        //                    AddProperty(obj, "prop_is_required", reader[6]);
        //                    AddProperty(obj, "prop_is_collection", reader[7]);
        //                    //if ((int)reader[0]==2022022)
        //                    //    Console.WriteLine(reader[3]);
        //                    object value = null;
        //                    //expandoDict.TryGetValue("ListOfProperties", out value);
        //                    //AddProperty(Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)reader[0]).FirstOrDefault(), "ListOfProperties", obj);
        //                    var a = Objects.Cast<dynamic>().Where(x => x.ObjectId == (int)reader[0]).FirstOrDefault() as IDictionary<string, object>;
        //                    a.TryGetValue("ListOfProperties", out value);
        //                    var b = (List<ExpandoObject>)value;
        //                    b.Add(obj);

        //                }
        //            }
        //            reader.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
        //        }
        //    }


        //}

        //private object getPropertyType(object v)
        //{
        //    switch (v)
        //    {
        //        case 100: return "";
        //        case 114: return default(Boolean);
        //        case 115: return default(Decimal);

        //        case 122: return default(DateTime);
        //        case 199: return "Object";
        //        default: return "Object";
        //    }
        //}

        //private bool tableisvalid(string table_name)
        //{
        //    bool result = false;
        //    string queryString = "USE [" + Database + "] select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '" + table_name + "'";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand command = new SqlCommand(queryString, connection);
        //        try
        //        {
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                if ((int)reader[0] > 0)
        //                    result = true;
        //                else result = false;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.TargetSite + "\n\r" + ex.Message);
        //        }
        //        return result;
        //    }
        //}
    }
}
