using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public static class DocxPlaceholderReplacer
    {
        public static void ReplacePlaceholders(string templatePath, string outputPath, Dictionary<string, string> values)
        {
            File.Copy(templatePath, outputPath, true);

            using (var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(outputPath, true))
            {
                var allTexts = doc.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();

                foreach (var text in allTexts)
                {
                    string original = text.Text;

                    if (!string.IsNullOrEmpty(original))
                    {
                        string newText = original;

                        foreach (var pair in values)
                        {
                            string placeholder = "{" + pair.Key + "}";
                            if (original.Contains(placeholder))
                            {
                                newText = newText.Replace(placeholder, pair.Value);
                            }
                        }

                        if (original != newText)
                        {
                            text.Text = newText;
                        }
                    }
                }

                doc.MainDocumentPart.Document.Save();
            }
        }
    }

}
