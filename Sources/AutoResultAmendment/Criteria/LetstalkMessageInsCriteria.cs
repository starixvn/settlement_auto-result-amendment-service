using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace AutoResultAmendment.Criteria;

public class LetstalkMessageInsCriteria : NonQueryCommand
{
    public int ErrCode { get; set; }
    public string ErrMsg { get; set; }
    public string Message { get; set; } = string.Empty;

    public override string GetSettingKey() => "DBA_Send_Letstalk";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Message);
}