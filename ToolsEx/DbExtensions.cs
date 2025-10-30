using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ToolsEx;

public class DbExtensions
{
    public static IList<string> GetSqlsInBatches<T>(IList<T> items, int _batchSize = 500)
    {
        var sqlsToExecute = new List<string>();
        if (items.Any())
        {
            var batchSize = _batchSize;
            var valueToSqls = new List<string>();
            var numberOfBatches = (int)Math.Ceiling((double)items.Count / batchSize);
            var firtItem = items.First();
            var pros = firtItem.GetType().GetProperties();
            var prosLen = pros.Length;
            var tableName =  ((TableAttribute)typeof(T).GetCustomAttribute(typeof(TableAttribute))!)?.Name ?? firtItem.GetType().Name;
            var insertSql = @$"INSERT INTO [dbo].[{tableName}](";
            var numItems = items.Count;
            for (var ibatch = 0; ibatch < numberOfBatches; ibatch++)
            {
                valueToSqls.Clear();
                var nextLenIndex = ibatch * batchSize + batchSize;
                nextLenIndex = nextLenIndex < numItems
                    ? nextLenIndex
                    : ibatch * batchSize + Math.Abs(ibatch * batchSize - numItems);
                for (var ix = ibatch * batchSize; ix < nextLenIndex; ix++)
                {
                    var item = items[ix];
                    var valuesStr = "(";
                    for (var i = 0; i < prosLen; i++)
                    {
                        var pro = pros[i];
                        if (ix == 0) insertSql += $@"[{pro.Name}]";

                        var proType = pro.PropertyType;
                        var typeCode = Type.GetTypeCode(pro.PropertyType);
                        var val = item.GetType().GetProperty(pro.Name).GetValue(item);
                        var valStr = $@"{val}";
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                valStr = $"{((bool)val ? 1 : 0)}";
                                break;
                            case TypeCode.Byte:
                                valStr = $"'{val}'";
                                break;
                            case TypeCode.SByte:
                                valStr = $"'{val}'";
                                break;
                            case TypeCode.String:
                                valStr = $"N'{val}'";
                                break;
                            case TypeCode.Empty:
                            case TypeCode.Object:
                                valStr = $"'{val}'";
                                break;
                            case TypeCode.DBNull:
                            case TypeCode.Char:
                                valStr = $"'{val}'";
                                break;
                            case TypeCode.Int16:
                                break;
                            case TypeCode.UInt16:
                                break;
                            case TypeCode.Int32:
                                break;
                            case TypeCode.UInt32:
                                break;
                            case TypeCode.Int64:
                                break;
                            case TypeCode.UInt64:
                                break;
                            case TypeCode.Single:
                                break;
                            case TypeCode.Double:
                                break;
                            case TypeCode.Decimal:
                                break;
                            case TypeCode.DateTime:

                                try
                                {
                                    if (val == null)
                                        valStr = "NULL";
                                    else
                                        valStr = $"'{(DateTime)val:yyyy-MM-dd HH\\:mm\\:ss.fff}'";
                                }
                                catch (Exception e)
                                {
                                    throw new Exception($@"DateTime: {e.Message}{Environment.NewLine}{val}");
                                }

                                break;
                            default:

                                if (proType == typeof(DateTime?))
                                    try
                                    {
                                        //MessageBox.Show($@"{typeCode} - {proType} - {typeof(DateTime?)} - {val}");
                                        if (val == null)
                                            valStr = "NULL";
                                        else
                                            valStr = $"'{(DateTime)val:yyyy-MM-dd HH\\:mm\\:ss.fff}'";
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception($@"DateTime?: {e.Message}{Environment.NewLine}{val}");
                                    }
                                else if (proType == typeof(TimeSpan))
                                    try
                                    {
                                        if (val == null)
                                            valStr = "NULL";
                                        else
                                            valStr = $"'{(TimeSpan)val:HH\\:mm\\:ss.fff}'";
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception($@"TimeSpan: {e.Message}{Environment.NewLine}{val}");
                                    }
                                else if (proType == typeof(TimeSpan?))
                                    try
                                    {
                                        if (val == null)
                                            valStr = "NULL";
                                        else
                                            valStr = $"'{(TimeSpan)val:HH\\:mm\\:ss.fff}'";
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception($@"TimeSpan?: {e.Message}{Environment.NewLine}{val}");
                                    }
                                else
                                    valStr = "NULL";

                                break;
                        }

                        if (proType == typeof(DateTime?))
                            try
                            {
                                //MessageBox.Show($@"{typeCode} - {proType} - {typeof(DateTime?)} - {val}");
                                if (val == null)
                                    valStr = "NULL";
                                else
                                    valStr = $"'{(DateTime)val:yyyy-MM-dd HH\\:mm\\:ss.fff}'";
                            }
                            catch (Exception e)
                            {
                                throw new Exception($@"DateTime?: {e.Message}{Environment.NewLine}{val}");
                            }
                        else if (proType == typeof(TimeSpan))
                            try
                            {
                                if (val == null)
                                    valStr = "NULL";
                                else
                                    valStr = $"'{(TimeSpan)val:HH\\:mm\\:ss.fff}'";
                            }
                            catch (Exception e)
                            {
                                throw new Exception($@"TimeSpan: {e.Message}{Environment.NewLine}{val}");
                            }
                        else if (proType == typeof(TimeSpan?))
                            try
                            {
                                if (val == null)
                                    valStr = "NULL";
                                else
                                    valStr = $"'{(TimeSpan)val:HH\\:mm\\:ss.fff}'";
                            }
                            catch (Exception e)
                            {
                                throw new Exception($@"TimeSpan?: {e.Message}{Environment.NewLine}{val}");
                            }

                        //MessageBox.Show($@"{typeCode} - {proType} - {typeof(DateTime?)} - {val}");

                        valuesStr += valStr;
                        if (i < prosLen - 1)
                        {
                            if (ix == 0) insertSql += @",";

                            valuesStr += @",";
                        }
                        else
                        {
                            if (ix == 0) insertSql += @") VALUES ";

                            valuesStr += @")";
                        }
                    }

                    valueToSqls.Add(valuesStr);
                }

                sqlsToExecute.Add(insertSql + string.Join(",", valueToSqls));
            }
        }

        return sqlsToExecute;
    }
}