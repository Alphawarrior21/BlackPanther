using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraderPerformanceComparer.Assets
{
  public struct WinnerHighlightStruct
    {
       public  string winneruserId;
       public   string participant1UserId;
    }
    public enum ORDER_STATE
    {
        //AuthenticationMethod.FORMS.ToDescription()

        [Description("NONE")]
        ORDER_STATE_NONE = 0,
        [Description("NEW")]
        ORDER_STATE_NEW = 1,
        [Description("CONFIRMED")]
        ORDER_STATE_CONFIRMED = 2,
        [Description("REJECTED")]
        ORDER_STATE_REJECTED = 3,
        [Description("MODIFY REQUEST")]
        ORDER_STATE_MODIFY_REQ = 4,
        [Description("MODIFY REJECT")]
        ORDER_STATE_MODIFY_REJECT = 5,
        [Description("MODIFIED")]
        ORDER_STATE_MODIFIED = 6,
        [Description("CANCEL REQUEST")]
        ORDER_STATE_CANCEL_REQ = 7,
        [Description("CANCEL REJECT")]
        ORDER_STATE_CANCEL_REJECT = 8,
        [Description("CANCELLED")]
        ORDER_STATE_CANCELLED = 9,
        [Description("TRADED")]
        ORDER_STATE_TRADED = 10

    }
}
