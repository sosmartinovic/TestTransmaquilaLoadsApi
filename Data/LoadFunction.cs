using System;
using System.Data;
using System.Collections;
public class LoadFunction
{
        public enum QueryType{
        Add = 1,
        Read = 2,
        Update = 3,
        Delete = 4
        }

        public static List<T> GetDataCollection<T>(string StoredProcedure){

            List<T> objects = new List<T>();

            DataTable dt = new DataTable();
            LoadData.FillData(StoredProcedure, ref dt);

            if (dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    objects.Add(GetData<T>(ref row));
                }
            }

            return objects;

        }

        public static List<T> GetDataCollection<T>(DataTable table)
        {

            List<T> objects = new List<T>();

            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    objects.Add(GetData<T>(ref row));
                }
            }

            return objects;

        }

        public static DataTable GetSchemaTable<T>(T obj) {

            System.Reflection.PropertyInfo[] props;
            DataTable dt = new DataTable();

            if(obj is not null){
                props = obj.GetType().GetProperties();

                for (int i = 0; i < props.Length; i++)
                {
                    DataColumn dc = new DataColumn();
                    dc.DataType = props[i].PropertyType;
                    dc.ColumnName = props[i].Name;

                    dt.Columns.Add(dc);
                }

            }
            
            return dt;
        }


        public static DataRow GetRow<T>(T obj, ref DataTable table) {

            System.Reflection.PropertyInfo[] props;
            DataRow row = table.NewRow();

            if(obj is not null){
                props= obj.GetType().GetProperties();
                row.BeginEdit();
                foreach (DataColumn c in table.Columns)
                {
                    foreach (var p in props)
                    {
                        if (string.Equals(p.Name.ToLower(), c.ColumnName.ToLower()))
                        {
                        row[c.ColumnName] = p.GetValue(obj);
                        }
                    }
                }
                row.EndEdit();
            }

            return row;
        }

        private static T GetData<T>(ref DataRow row) {
            
            //T o= (T)Activator.CreateInstance(typeof(T));
            T o= Activator.CreateInstance<T>();

            System.Reflection.PropertyInfo[] props;

            if(o is not null){
                props = o.GetType().GetProperties();

                foreach (DataColumn c in row.Table.Columns)
                {
                    foreach (var p in props)
                    {
                        if (string.Equals( p.Name.ToLower(), c.ColumnName.ToLower()))
                        {
                            if (p.CanWrite)
                            {
                                if (!(row[c.ColumnName] == DBNull.Value)) {
                                    p.SetValue(o, row[c.ColumnName]);
                                }
                                    
                            }
                            break;
                        }
                    }
                }
            }

            return o;

        }

        public static bool AddRecord<T>(ref T o,string keyfield,String storedprocedure,ArrayList excludefields) 
	    {
            SortedList values= new SortedList();
            System.Reflection.PropertyInfo[] props;
            int Id=0;
            if(o is not null){
                props = o.GetType().GetProperties();
                
                foreach (var p in props)
                {
                    if (!excludefields.Contains(p.Name)) { //if Field is not in List of Exclusion
                        if (p.GetValue(o,null) is not null){
                            values.Add(p.Name, p.GetValue(o,null));
                        }
                        else{
                            values.Add(p.Name, DBNull.Value);
                        }
                    }
                }

                Id =(int)LoadData.ExecuteStoreProcedure(storedprocedure, values);
                
                T obj= Activator.CreateInstance<T>();
                if(obj is not null){
                    var prop = obj.GetType().GetProperty(keyfield);
                    if(prop !=null){
                        if(prop.CanWrite){
                            prop.SetValue(o, Id,null);
                        }
                    }
                }
                
            }

            return Id >0;

        }

	public static bool UpdateRecord<T>(ref T o,string keyField,string tableName)
	{
		int rowsAffected = 0;
        string query = "";

        query = GenerateQuery(ref o, QueryType.Update, tableName, keyField);

        SortedList values =new SortedList();
        System.Reflection.PropertyInfo[] props;
        
        if(o is not null){
            props= o.GetType().GetProperties();
            foreach (var p in props)
            {
                if (p.GetValue(o,null) is not null){
                    values.Add(p.Name, p.GetValue(o,null));
                }
                else{
                    values.Add(p.Name, DBNull.Value);
                }
            }

            rowsAffected= LoadData.ExecuteQuery(query, values);
        }
        
        return rowsAffected > 0;
    }

    public static T SelectRecord<T>(string keyField,object keyValue, string tableName){
        string query = "";
        T o = Activator.CreateInstance<T>();
        if(o is not null){
            DataRow row;
            //System.Reflection.PropertyInfo p; 
            var p = o.GetType().GetProperty(keyField);
            if(p is not null){
                p.SetValue(o, keyValue);

                query = GenerateQuery<T>(ref o, QueryType.Read, tableName, keyField);

                row = LoadData.GetRow(query, keyField, keyValue);
                if(row is not null) {
                    if (row.Table.Rows.Count==0){
                        p.SetValue(o,0);
                    }
                    else{
                        FillData<T>(ref row,ref o);
                    }
                    
                }
            }

        }
        
        return o;
    }

    public static bool DeleteRecord<T>(string keyField,object keyValue,string tableName)
    {
        string query = "";
        int cuantos=0;
        T o =Activator.CreateInstance<T>();
        if(o is not null){
            var p = o.GetType().GetProperty(keyField);
            if(p is not null){
                p.SetValue(o, keyValue);
                query = GenerateQuery<T>(ref o, QueryType.Delete, tableName, keyField);
                cuantos = LoadData.ExecuteQuery(query);
            }
        }
        
        return (cuantos > 0);
    }        

    private static void FillData<T>(ref DataRow row, ref T o ){
        
        if(o is not null){
            var props  = o.GetType().GetProperties();

            foreach(DataColumn c in row.Table.Columns){
                
                foreach (var p in props){
                    
                    if (p.Name.ToLower() == c.ColumnName.ToLower()) {
                        if (p.CanWrite) {
                            if (row[c.ColumnName]!= DBNull.Value) {
                                p.SetValue(o, row[c.ColumnName]);
                            }
                        }

                        break; //'If we reach the property, go to the next column
                    }
                }
            }
        }

    }

    private static String GenerateQuery<T>(ref T classObject, QueryType queryType, String tableName,String keyField)
	{
		String query = "";
        System.Reflection.PropertyInfo[] props;
        object keyValue="";
        String IdentityColumnName = "";

        if(classObject is not null){
            props = classObject.GetType().GetProperties();
        }
        else{
            return "";
        }
     

        String fields = "";
        String parameters = "";

        if (queryType ==QueryType.Update) {
            IdentityColumnName = LoadData.GetIdentityColumnName(tableName);
        }

        foreach (var p in props){
            if (!(p.Name.ToLower() == keyField.ToLower()) && !(p.Name.ToLower() == IdentityColumnName.ToLower())) {
                
                if (queryType==QueryType.Read){
                    fields = fields + p.Name + ",";
                    parameters = parameters + "@" + p.Name + ",";
                }
                else{
                    var pvalue= p.GetValue(classObject);
                    if(pvalue is not null){
                        fields = fields + p.Name + ",";
                        parameters = parameters + "@" + p.Name + ",";
                    }
                }


            }

            if (p.Name.ToLower() == keyField.ToLower()){
                if (!(queryType == QueryType.Add)) {
                    //' We dont Know the keyValue yet. Until we store the record in the database
                    if( classObject is not null){
                        var pvalue= p.GetValue(classObject);
                        if(pvalue is not null)
                            keyValue=pvalue;
                    }

				}
            }
        }

        if(fields.Length>0 && parameters.Length>0){
            fields = fields.Remove(fields.Length - 1, 1);
            parameters = parameters.Remove(parameters.Length - 1, 1);
        }
        

        switch (queryType){
            case LoadFunction.QueryType.Update:
                string[] c;
                string[] p;
                c = fields.Split(",");
                p = parameters.Split(",");
                String completa = "";
				for(int i=0;i<c.Length;i++){
					if (i == 0) {
                        completa = completa + c[i] + "=" + p[i];
                    }
					else{
                        completa = completa + "," + c[i] + "=" + p[i];
                    }
				}
                
                query = "UPDATE " + tableName + " SET " + completa + " " + GenerateWhere(keyField, keyValue);
				break;
            case LoadFunction.QueryType.Add:
                query = "INSERT INTO " + tableName + "(" + fields + ") VALUES(" + parameters + ")";
				break;
            case LoadFunction.QueryType.Delete:
                query = "DELETE FROM " + tableName + " " + GenerateWhere(keyField, keyValue);
                break;
            case LoadFunction.QueryType.Read:
                query = "SELECT " + fields + " FROM " + tableName + " " + GenerateWhere(keyField, keyValue);
                break;
        }

        return query;
    }

    private static string GenerateWhere(string keyField , object keyValue )
	{
        string where = "";
        string whereValue;

        if(keyValue is not null){
            if ( keyValue.GetType() ==typeof(string) ) {
                whereValue = "'" + keyValue.ToString() + "'";
            }
            else{
                whereValue =((int)keyValue).ToString();
            }

            where = "WHERE " + keyField + "=" + whereValue;
        }
        

        return where;
    }

}

