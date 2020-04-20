using System;
using System.Collections.Generic;
using System.Text;

namespace Creator.Models.Objects
{
    public interface IConvertToOctokit<T>
    {
        public T ConvertTo();
    }
}
