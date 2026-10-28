using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace UberSy.Domain.Complaints.Enums;

public enum ComplaintStatus : byte
{
    [Description("Open")]
    Open = 1,

    [Description("In Progress")]
    InProgress = 2,

    [Description("Resolved")]
    Resolved = 3,

    [Description("Closed")]
    Closed = 4
}
