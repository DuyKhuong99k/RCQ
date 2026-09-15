/*
    Cấp quyền sửa trực tiếp trên DataGrid cho một hoặc nhiều nhóm quyền.
    Thay các RoleId mẫu bên dưới bằng Id nhóm quyền cần cấp trước khi chạy.
*/

DECLARE @RoleIds TABLE
(
    RoleId INT NOT NULL PRIMARY KEY
);

INSERT INTO @RoleIds (RoleId)
VALUES
    (0); -- Thay 0 bằng Id nhóm quyền, có thể thêm nhiều dòng: (2), (5)

IF EXISTS (SELECT 1 FROM @RoleIds WHERE RoleId <= 0)
BEGIN
    THROW 50000, N'Vui lòng thay RoleId mẫu bằng Id nhóm quyền cần cấp.', 1;
END;

DECLARE @Permissions TABLE
(
    Fu NVARCHAR(500) NOT NULL,
    Func NVARCHAR(500) NOT NULL
);

INSERT INTO @Permissions (Fu, Func)
VALUES
    (N'Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ', N'Sửa Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ'),
    (N'Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ', N'Sửa Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ'),
    (N'Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ', N'Sửa Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ');

INSERT INTO dbo.RolePermistion
(
    RoleId,
    Fu,
    Func,
    CreatedDateTime,
    Status
)
SELECT
    roleIds.RoleId,
    permissions.Fu,
    permissions.Func,
    GETDATE(),
    1
FROM @RoleIds AS roleIds
CROSS JOIN @Permissions AS permissions
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RolePermistion AS existingPermission
    WHERE existingPermission.RoleId = roleIds.RoleId
      AND existingPermission.Fu = permissions.Fu
      AND existingPermission.Func = permissions.Func
);

SELECT
    RoleId,
    Fu,
    Func,
    Status
FROM dbo.RolePermistion
WHERE (Fu = N'Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ'
       AND Func = N'Sửa Báo Cáo Nguyên Liệu Nhập / Chi Tiết HQ')
   OR (Fu = N'Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ'
       AND Func = N'Sửa Báo Cáo Nguyên Liệu Nhập / Tổng Hợp Sản Phẩm HQ')
   OR (Fu = N'Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ'
       AND Func = N'Sửa Báo Cáo Nguyên Liệu Xuất / Khối Lượng Xuất Xưởng Theo Lô HQ')
ORDER BY RoleId, Fu, Func;
