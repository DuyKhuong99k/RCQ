/*
    Chỉ cấp quyền sửa trực tiếp trên DataGrid cho nhóm HQ (RoleId 1002).
    Các nhóm Kế Toán (1003) và Cân Nguyên Liệu (1004) vẫn chỉ có quyền Xem.
*/

DECLARE @Permissions TABLE
(
    RoleId INT NOT NULL,
    Fu NVARCHAR(500) NOT NULL,
    Func NVARCHAR(500) NOT NULL,
    PRIMARY KEY (RoleId, Fu, Func)
);

INSERT INTO @Permissions (RoleId, Fu, Func)
VALUES
    (1002, N'Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ', N'Sửa Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ'),
    (1002, N'Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ', N'Sửa Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ'),
    (1002, N'Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ', N'Sửa Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ');

INSERT INTO dbo.RolePermistion
(
    RoleId,
    Fu,
    Func,
    CreatedDateTime,
    Status
)
SELECT
    permissions.RoleId,
    permissions.Fu,
    permissions.Func,
    GETDATE(),
    1
FROM @Permissions AS permissions
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RolePermistion AS existingPermission
    WHERE existingPermission.RoleId = permissions.RoleId
      AND existingPermission.Fu = permissions.Fu
      AND existingPermission.Func = permissions.Func
);

SELECT
    RoleId,
    Fu,
    Func,
    Status
FROM dbo.RolePermistion
WHERE EXISTS
(
    SELECT 1
    FROM @Permissions AS permissions
    WHERE permissions.RoleId = dbo.RolePermistion.RoleId
      AND permissions.Fu = dbo.RolePermistion.Fu
      AND permissions.Func = dbo.RolePermistion.Func
)
ORDER BY RoleId, Fu, Func;
