using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using org.pdfbox.pdmodel;
using org.pdfbox.util;

namespace DocumentsTest
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    
    [TestFixture]
    public class DocumentTest
    {
        [Test]
        public void CreatefromPath()
        {
            string path = @"C:\example.pdf";

            Documents.Document doc = new Documents.Document(path);

            Assert.AreEqual(doc.FileName, "example.pdf");
            Assert.AreEqual(doc.Path, path);
            Assert.AreEqual(doc.Id, 0);
            
            PDDocument docPDF = PDDocument.load(path);
            PDDocumentInformation docInfo = docPDF.getDocumentInformation();
            
            if (docInfo.getTitle() == String.Empty)
                Assert.AreEqual("example.pdf", doc.Title);
            else
                Assert.AreEqual(docInfo.getTitle(), doc.Title);
        }
    }
}
