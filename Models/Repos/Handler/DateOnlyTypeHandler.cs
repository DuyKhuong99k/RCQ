using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Models.Repos.Handler
{
    public class DateOnlyTypeHandler: SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue); // Chuyển đổi DateOnly thành DateTime
        }

        public override DateOnly Parse(object value)
        {
            return DateOnly.FromDateTime((DateTime)value); // Chuyển đổi DateTime thành DateOnly
        }
    }
}
