using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public interface ITemplateStrategy
    {
        string GetTemplatePath(string baseDir);
    }
}
