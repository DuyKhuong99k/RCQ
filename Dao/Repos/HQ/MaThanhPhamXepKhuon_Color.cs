using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dao.Repos.HQ
{
    public class MaThanhPhamXepKhuon_Color
    {
        private readonly string connectionString;
        private string tableName = @"MaThanhPhamXepKhuon_Color";
        private readonly string queryDelete = @"DELETE FROM [dbo].[MaThanhPhamXepKhuon_Color]
        WHERE  [Ngay] = @Ngay and [MaXuong] = @MaXuong";
        private readonly string queryInsert = @"INSERT INTO [dbo].[MaThanhPhamXepKhuon_Color]
           ([MaThanhPham]
           ,[MaSize]
           ,[ColorCode]
           ,[Ngay]
           ,[MaLo]
           ,[MaXuong]
           )
     VALUES
           (@MaThanhPham
           ,@MaSize
           ,@ColorCode
           ,@Ngay
           ,@MaLo
           ,@MaXuong
           )";
        private readonly string queryUpdate = @"UPDATE [dbo].[MaThanhPhamXepKhuon_Color]
        SET [MaThanhPham]=@MaThanhPham
           ,[MaSize] = @MaSize
           ,[ColorCode] = @ColorCode
           ,[Ngay] = @Ngay
           ,[MaLo] = @MaLo
           ,[MaXuong] = @MaXuong
 WHERE [ColorCode] = @ColorCode and [Ngay] = @Ngay and [MaLo] = @MaLo and [MaXuong] = @MaXuong";
        private readonly string queryUpdateDb = @"IF (
    NOT EXISTS (
        SELECT
            *
        FROM
            INFORMATION_SCHEMA.TABLES
        WHERE
            TABLE_SCHEMA = 'dbo'
            AND TABLE_NAME = 'MaThanhPhamXepKhuon_Color'
    )
) BEGIN 
 CREATE TABLE [dbo].[MaThanhPhamXepKhuon_Color] (
    [MaThanhPham] VARCHAR (50) NOT NULL,
    [ColorCode]   VARCHAR (50) NOT NULL,
    [Ngay]        DATE         NOT NULL,
    [MaLo]        VARCHAR (50) NOT NULL,
    [MaXuong]     VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MaThanhPhamXepKhuon_Color] PRIMARY KEY CLUSTERED ([ColorCode] ASC, [Ngay] ASC, [MaLo] ASC, [MaXuong] ASC)
);
end
DECLARE @tb varchar(30) = 'MaThanhPhamXepKhuon_Color' IF COL_LENGTH(@tb, 'MaSize') IS NULL BEGIN
ALTER TABLE
    MaThanhPhamXepKhuon_Color
ADD
    MaSize [varchar](50) NULL
END ;
";
        public MaThanhPhamXepKhuon_Color()
        {
            connectionString = AppViewModels.Base.Ins.ConnectionString;
        }


        public int Delete<T>(T item)
        {
            try
            {
                var query = @"DELETE [dbo].[MaThanhPhamXepKhuon_Color] where [Ngay] =@Ngay and MaXuong = @MaXuong";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<T> Gets<T>(DateTime dateTime, string xuongId)
        {
            try
            {
                var query = "Select * from MaThanhPhamXepKhuon_Color where Ngay =@ngay and MaXuong=@xuongId";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query, new { ngay = dateTime.Date, xuongId = xuongId }).ToList();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> GetsLastDay<T>()
        {
            try
            {
                var query = @"Select
    *
from
    (
        Select
            d.*,
            ROW_NUMBER() OVER (
                PARTITION BY ColorCode
                ORDER BY
                    Ngay DESC
            ) AS [ROW NUMBER]
        from
            MaThanhPhamXepKhuon_Color d
    ) as c
where
   c.[ROW NUMBER] = 1";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsLastDay<T>(string xuongId)
        {
            try
            {
                var query = @"Select
    *
from
    (
        Select
            d.*,
            ROW_NUMBER() OVER (
                PARTITION BY ColorCode
                ORDER BY
                    Ngay DESC
            ) AS [ROW NUMBER]
        from
            MaThanhPhamXepKhuon_Color d where MaXuong = @xuongId
    ) as c
where
   c.[ROW NUMBER] = 1";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var items = connection.Query<T>(query,new {xuongId}).ToList();
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert<T>(List<T> items)
        {
            try
            {
                var query = @"INSERT INTO [dbo].[MaThanhPhamXepKhuon_Color]
           ([MaThanhPham]
           ,[ColorCode]
           ,[Ngay]
           ,[MaLo]
           ,[MaXuong]
,[MaSize])
     VALUES
           (@MaThanhPham 
           ,@ColorCode 
           ,@Ngay 
           ,@MaLo 
           ,@MaXuong
,@MaSize)";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, items);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Insert<T>(T item)
        {
            try
            {
                var query = @"INSERT INTO [dbo].[MaThanhPhamXepKhuon_Color]
           ([MaThanhPham]
           ,[ColorCode]
           ,[Ngay]
           ,[MaLo]
           ,[MaXuong]
,[MaSize])
     VALUES
           (@MaThanhPham 
           ,@ColorCode 
           ,@Ngay 
           ,@MaLo 
           ,@MaXuong
,@MaSize)";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Update<T>(T item)
        {
            try
            {
                var query = @"UPDATE [dbo].[MaThanhPhamXepKhuon_Color]
   SET [MaThanhPham] = @MaThanhPham 
,[MaSize] = @MaSize
 WHERE [ColorCode] = @ColorCode 
      and [Ngay] = @Ngay 
      and [MaLo] = @MaLo 
      and [MaXuong] = @MaXuong ";
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                var rows = connection.Execute(query, item);
                return rows;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
