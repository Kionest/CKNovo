using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public static class TemplateStrategyFactory
    {
        public static ITemplateStrategy GetStrategy(string type)
        {
            if (type == "Entity")
                return new EntityTemplateStrategy();

            return new IndividualTemplateStrategy();
        }
    }
}
