using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

public class LoadData
{
        private static string mConnectionString="";

        public static string? LastException { get; set; }

        public static void SetConnectionString(string ConnectionString){
            mConnectionString=ConnectionString;
        }

    	private static string GetConnectionString()
		{
            return mConnectionString;
		}

		public static object ExecuteQueryAndReturnValue(string query)
		{
			object column_value="";
			SqlCommand cmd = new SqlCommand(query, new SqlConnection(GetConnectionString()));

			try
			{
				cmd.Connection.Open();
				column_value = cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				LastException= ex.Message;
			}
			finally
			{
				cmd.Connection.Close();
				cmd.Connection.Dispose();
				cmd.Dispose();
			}


			return column_value;
		}

        public static object ExecuteQueryAndReturnValue(string query, SortedList parameters) {
			object column_value="";
            string parameterName;
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(GetConnectionString()));

			try
			{

                for (int index = 0; index < parameters.Count; index++)
                {
                    parameterName = "";
                    parameterName = (string)parameters.GetKey(index);

                    if (!parameterName.Contains("@"))
                    {
                        parameterName = "@" + parameterName;
                    }

                    //parameters[]

                    if (parameters[parameters.GetKey(index)] == null)
                    {
                        parameters[parameters.GetKey(index)] = System.DBNull.Value;
                    }

                    cmd.Parameters.AddWithValue(parameterName, parameters[parameters.GetKey(index)]);
                }
                cmd.Connection.Open();
				column_value = cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				//throw ex;
                LastException= ex.Message;
			}
			finally
			{
				cmd.Connection.Close();
				cmd.Connection.Dispose();
				cmd.Dispose();
			}


			return column_value;

		}

        public static int ExecuteQuery(string query) 
        {
            int rows = 0;
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(GetConnectionString()));
            
            try{
                cmd.Connection.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                LastException= ex.Message;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
            }
            

                return rows;
        }     
        public static int ExecuteQuery(string query, SortedList parameters) 
        {
            int rows = 0;
            string parameterName = "";
            
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(GetConnectionString()));
            
            for (int index = 0; index < parameters.Count; index++)
            {
                parameterName = "";
                parameterName = (string)parameters.GetKey(index);

                if (!parameterName.Contains("@"))
                {
                    parameterName = "@" + parameterName;
                }

                //parameters[]

                if (parameters[parameters.GetKey(index)] == null)
                {
                    parameters[parameters.GetKey(index)] = System.DBNull.Value;
                }

                cmd.Parameters.AddWithValue(parameterName, parameters[parameters.GetKey(index)]);
            }

            try{
                cmd.Connection.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                LastException= ex.Message;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
            }
            

                return rows;
        }        

        public static DataTable GetData(string Query, SortedList parameters) 
        {

            SqlCommand cmd = new SqlCommand();
            SqlDataReader rdr;
            DataTable oTable = new DataTable();

            try
            {

                DataRow row;

                cmd.CommandText = Query;
                cmd.Connection = new SqlConnection(GetConnectionString());
                cmd.Connection.Open();
                rdr = cmd.ExecuteReader();
                
                while (rdr.Read())
                {
                    if (oTable.Columns.Count == 0)
                    {
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            oTable.Columns.Add(rdr.GetName(i), rdr.GetFieldType(i));
                        }
                    }

                    row = oTable.NewRow();
                    row.BeginEdit();

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        row[rdr.GetName(i)] = rdr.GetValue(i);
                    }

                    row.EndEdit();
                    oTable.Rows.Add(row);

                }
                rdr.Close();

            }
            catch (Exception ex)
            {
                LastException= ex.Message;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
                
            }

            return oTable;
        }

    public static void FillData(string StoredProcedure, ref DataTable oTable)
    {
        SqlDataAdapter da;

        try
        {
            da = new SqlDataAdapter();
            da.SelectCommand = new SqlCommand();
            da.SelectCommand.Connection = new SqlConnection(GetConnectionString());
            da.SelectCommand.CommandText = StoredProcedure;
            da.SelectCommand.CommandType = CommandType.StoredProcedure;

            da.Fill(oTable);
            da.Dispose();
        }
        catch (Exception ex)
        {
            LastException= ex.Message;
        }
    }


    public static void FillData(string StoredProcedure, ref DataTable oTable, SortedList parameters)
        {
            SqlDataAdapter da;
            string parameterName;

            try
            {
                da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = new SqlConnection(GetConnectionString());
                da.SelectCommand.CommandText = StoredProcedure;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                for (int index = 0; index < parameters.Count; index++)
                {
                    parameterName = "";
                    parameterName = (string)parameters.GetKey(index);

                    if (!parameterName.Contains("@"))
                    {
                        parameterName = "@" + parameterName;
                    }

                    //parameters[]

                    if (parameters[parameters.GetKey(index)] == null)
                    {
                        parameters[parameters.GetKey(index)] = System.DBNull.Value;
                    }

                    da.SelectCommand.Parameters.AddWithValue(parameterName, parameters[parameters.GetKey(index)]);
                }

                da.Fill(oTable);
                da.Dispose();
            }
            catch (Exception ex)
            {
                LastException= ex.Message;
            }
        }


        public static void ExecuteStoreProcedure(string StoredProcedure, DataTable Table, string TableParameter, SortedList parameters )
        {
            SqlCommand cmd = new SqlCommand();
            string parameterName = "";

            cmd.Connection = new SqlConnection(GetConnectionString());
            cmd.CommandText = StoredProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter TablePar = cmd.Parameters.AddWithValue(TableParameter, Table);
            TablePar.SqlDbType = SqlDbType.Structured;

            for (int index = 0; index < parameters.Count; index++)
            {
                parameterName = "";
                parameterName = (string)parameters.GetKey(index);

                if (!parameterName.Contains("@"))
                {
                    parameterName = "@" + parameterName;
                }

                if (parameters[parameters.GetKey(index)] == null)
                {
                    parameters[parameters.GetKey(index)] = System.DBNull.Value;
                }

                cmd.Parameters.AddWithValue(parameterName, parameters[parameters.GetKey(index)]);
            }

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastException= ex.Message;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
            }

        }

        public static object ExecuteStoreProcedure(string StoredProcedure, SortedList parameters) 
        {
            SqlCommand cmd= new SqlCommand();
            cmd.Connection = new SqlConnection(GetConnectionString());
            cmd.CommandText = StoredProcedure;
            cmd.CommandType = CommandType.StoredProcedure;

            string parameterName = "";
            object value="";


            for (int index = 0; index < parameters.Count; index++)
            {
                parameterName = "";
                parameterName = (string)parameters.GetKey(index);

                if (!parameterName.Contains("@"))
                {
                    parameterName = "@" + parameterName;
                }

                if (parameters[parameters.GetKey(index)] == null)
                {
                    parameters[parameters.GetKey(index)] = System.DBNull.Value;
                }

                cmd.Parameters.AddWithValue(parameterName, parameters[parameters.GetKey(index)]);
            }

            try
            {
                cmd.Connection.Open();
                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                LastException= ex.Message;
            }
            finally {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
                cmd.Dispose();
            }

            return value;
        }



        public static bool BulkCopy(DataTable table, string tablename) {

            bool succeed = false;
            string ColumnName = "";

            try
            {
            using (SqlConnection con = new SqlConnection(GetConnectionString()))
            {
                using (SqlBulkCopy cmd = new SqlBulkCopy(con))
                {
                    con.Open();

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        ColumnName= table.Columns[i].ColumnName;
                        cmd.ColumnMappings.Add(ColumnName, ColumnName);
                    }


                    cmd.DestinationTableName = tablename;
                    cmd.WriteToServer(table);
                    con.Close();
                    succeed = true;
                }

            }

            }
            catch (Exception ex) 
            {
               LastException=ex.Message;
            }

            return succeed;

        }


    public static string GetIdentityColumnName(string tableName) 
    {
		string query= "";
        string columnName= "";

        query = "select column_name from information_schema.columns where table_schema = 'dbo' and columnproperty(object_id(table_name), column_name,'IsIdentity') = 1 and TABLE_NAME ='" + tableName + "' order by table_name";
        columnName =(string) ExecuteQueryAndReturnValue(query);
        if(columnName is null){
			columnName="";
		}
		
        
		return columnName;
    }

    public static DataRow GetRow(string query ,string keyField,object keyValue )
    {
        DataRow row;
        
        SqlDataAdapter da =new SqlDataAdapter();
        DataTable dt =new DataTable();

        da.SelectCommand = new SqlCommand();
        da.SelectCommand.Connection = new SqlConnection(GetConnectionString());
        da.SelectCommand.CommandText = query;

        SqlParameter p =new SqlParameter();
        p.ParameterName = keyField;
        p.Value = keyValue;

        da.SelectCommand.Parameters.Add(p);

        da.Fill(dt);

        if (dt.Rows.Count > 0) {
            row = dt.NewRow();
            row.BeginEdit();
            foreach(DataColumn c in dt.Columns){
                row[c.ColumnName] = dt.Rows[0][c.ColumnName];
            }
            row.EndEdit();
        }
        else{
            row =dt.NewRow();
        }

        da.Dispose();
        

        return row;
    }	            

}
