using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Models.Repos.Handler
{
    public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.Value = value.ToTimeSpan(); // Chuyển đổi TimeOnly thành TimeSpan
        }

        public override TimeOnly Parse(object value)
        {
            return TimeOnly.FromTimeSpan((TimeSpan)value); // Chuyển đổi TimeSpan thành TimeOnly
        }
    }
}
