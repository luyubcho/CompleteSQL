﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleteSQL.Merge
{
    public sealed class WhenMatchedQueryPart : QueryPartDecorator
    {
        internal override string GetQueryPart()
        {
            return string.Concat(base.GetQueryPart(), Environment.NewLine, "When Matched");
        }
    }
}
