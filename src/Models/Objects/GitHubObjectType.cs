using System;
using System.Collections.Generic;
using System.Text;

namespace Creator.Models.Objects
{
    [Flags]
    enum GitHubObjectType
    {
        Milestone = 1,
        Label = 2
    }
}
