﻿using CompleteSQL.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompleteSQL.Merge
{
    public class DataTableCreator
    {
        public DataTable Create<TSource>(IEnumerable<TSource> dataSource, string targetTable) where TSource: class
        {
            
            string tableName = targetTable;

            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = (new SqlTableNameMapper()).GetFullTableName(typeof(TSource)).ToString();
            }

            DataTableSchemaCreator schemaCreator = new DataTableSchemaCreator();
            DataTableSchema schema = schemaCreator.CreateSchema<TSource>(targetTable);


            return Create(dataSource, schema);
        }

        public DataTable Create<TSource>(IEnumerable<TSource> dataSource, DataTableSchema schema) where TSource : class
        {
            DataTable dt = new DataTable(schema.TableName);
            DataColumn dc;
            foreach (DataColumnSchema cSchema in schema.Columns)
            {
                dc = dt.Columns.Add(cSchema.Name, cSchema.Type);
                dc.AllowDBNull = cSchema.AllowDbNull;
            }

            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            DataRow dr;
            foreach (TSource item in dataSource)
            {
                dr = dt.NewRow();

                foreach (PropertyInfo prop in props)
                {
                    dr[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        } 


    }
}
