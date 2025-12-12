using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public class ContractGeneratorFacade
    {
        private readonly NovotekEntities db;


        public ContractGeneratorFacade(NovotekEntities db)
        {
            this.db = db;
        }


        public string GenerateContract(ContractModel model, string templateType)
        {
            
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

            ITemplateStrategy strategy = TemplateStrategyFactory.GetStrategy(templateType);

            string templatePath = strategy.GetTemplatePath(baseDir);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Шаблон не найден: " + templatePath);

            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"Договор_{model.ContractNumber}.docx");

            DocxPlaceholderReplacer.ReplacePlaceholders(templatePath, outputPath, model.ToDictionary());

            return outputPath;
        }

    }
}

