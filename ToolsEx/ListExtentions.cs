using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolEx
{
    public static class ListExtensions
    {
        /// <summary>
        /// with dapper object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            PropertyDescriptorCollection props = null;
            DataTable table = new DataTable();
            if (list != null && list.Count > 0)
            {
                // props = Type.GetType().GetProperties(list[0]);
                props = TypeDescriptor.GetProperties(list[0]);
                for (int i = 0; i < props.Count / 2; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }

            if (props != null)
            {
                object[] values = new object[props.Count / 2];
                foreach (T item in list)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item) ?? DBNull.Value;
                    }

                    table.Rows.Add(values);
                }
            }

            return table;
        }

        /// <summary>
        /// Not for dapper object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTablev2<T>(this List<T> list)
        {
            PropertyDescriptorCollection props = null;
            DataTable table = new DataTable();
            if (list != null && list.Count > 0)
            {
                props = TypeDescriptor.GetProperties(list[0]);
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }

            if (props != null)
            {
                //object[] values = new object[props.Count / 2];
                object[] values = new object[props.Count];
                foreach (T item in list)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item) ?? DBNull.Value;
                    }

                    table.Rows.Add(values);
                }
            }

            return table;
        }
    }
}