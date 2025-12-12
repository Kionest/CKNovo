using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public class EntityTemplateStrategy : ITemplateStrategy
    {
        public string GetTemplatePath(string baseDir)
        {
            return Path.Combine(baseDir, "template_entity.docx");
        }
    }
}
