using Dapper;
using Microsoft.Data.SqlClient;

namespace Dao.Repos.HQ;
public partial class XiNghiep
{
    private readonly string connectionString;
    private string tableName = @"XiNghiep";
    private readonly string qrDelete = @"DELETE FROM [dbo].[XiNghiep]
      WHERE [Ma] = @Ma
";

    private readonly string qrInsert = @"
INSERT INTO [dbo].[XiNghiep]
           ([Ma]
           ,[Ten]
           ,[SuDung]
           ,[CodeId])
     VALUES
           (@Ma
           ,@Ten
           ,@SuDung
           ,@CodeId)
";

    private readonly string qrUpdate = @"
UPDATE [dbo].[XiNghiep]
   SET [Ten] = @Ten
      ,[SuDung] = @SuDung
      ,[CodeId] = @CodeId
 WHERE [Ma] = @Ma
";

    private readonly string qrGetAll = "Select * from XiNghiep";

    public XiNghiep()
    {
        connectionString = AppViewModels.Base.Ins.ConnectionString;

    }

    public int Delete<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Execute(qrDelete, item);
        return rows;
    }

    public List<T> Gets<T>()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Query<T>(qrGetAll).ToList();
        return rows;
    }

    public int Insert<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Execute(qrInsert, item);
        return rows;
    }

    public int Update<T>(T item)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var rows = connection.Execute(qrUpdate, item);
        return rows;
    }
}
