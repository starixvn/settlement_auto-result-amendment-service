using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoResultAmendment.Constants;

public static class ResultHistoryStatus
{
    public const short Failure = 3;
    public const short Success = 2;
    public const short Error = 99;
    public const short ErrorHandled = 4;
}