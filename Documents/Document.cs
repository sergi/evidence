// <copyright file="Document.cs" company="Company">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>Sergi Mansilla</author>
// <email>sergi.mansilla@gmail.com</email>
// <summary>Defines the Document class.</summary>

namespace Documents
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using Db4objects.Db4o;
    using System.Linq;
    using Db4objects.Db4o.Linq;
    using System.Diagnostics;
    using System.IO;

    using org.pdfbox.pdmodel;
    using org.pdfbox.util;

    /// <summary>
    /// Document object class. It makes an abstraction of document files and documents
    /// stored in the database in order to use it in the application.
    /// </summary>
    public class Document
    {
        string title;
        List<Tag> tags;

        /// <summary>
        /// Initializes a new instance of the Document class.
        /// </summary>
        public Document()
        {
            this.Tags = new List<Tag>();
        }

        /// <summary>
        /// Initializes a new instance of the Document class taking a document 
        /// file path as a parameter.
        /// </summary>
        /// <param name="path">Complete filesystem path of the document 
        /// file</param>
        public Document(string path)
        {
            if (System.IO.File.Exists(path))
            {
                PDDocument doc;
                PDDocumentInformation docInfo;
                
                this.Tags = new List<Tag>();
                this.Path = path;
                this.FileName = System.IO.Path.GetFileName(this.Path);

                doc = PDDocument.load(path);
                docInfo = doc.getDocumentInformation();
                this.title = docInfo.getTitle();
                
                // docInfo.getAuthor();
                // docInfo.getCreationDate();
                if (doc != null)
                    doc.close();
            }
            else
                throw new FileNotFoundException("The specified document doesn't exist.", path);
        }

        #region Properties
        public int Id { get; set; }

        public string Title
        {
            get 
            {
                return this.title == null || this.title.Length == 0 ?
                       this.FileName : this.title;
            }
            set { this.title = value; }
        }

        public string FileName { get; set;}
        public string Path { get; set;}

        public string Image 
        { 
            get 
            {
                string imagePath = this.Path + ".png";

                if (!File.Exists(imagePath))
                    imagePath = Document.CreateImageFromPdf(Path);

                return imagePath;
            } 
        }

        public List<Tag> Tags { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public Document Put()
        {
            Document.AddToDatabase(this);
            return this;
        }

        public void AddTag(Tag tag)
        {
            if (!this.ContainsTag(tag))
                this.tags.Add(tag);
        }

        public Tag RemoveTag(Tag tag)
        {
            if (this.tags.Contains(tag))
                this.tags.Remove(tag);

            return tag;
        }

        public bool ContainsTag(Tag tag)
        {
            foreach (Tag localTag in this.tags)
            {
                if (localTag.Name == tag.Name)
                    return true;
            }

            return false;
        }
        #endregion

        #region Static Methods
        public static List<Document> GetDocsFromFolder(string folder)
        {
            if (System.IO.Directory.Exists(folder))
            {
                List<Document> documentList = new List<Document>();

                foreach (string filename in
                        Directory.GetFiles(folder, "*.pdf"))
                {
                    Document document = new Document(filename);
                    documentList.Add(document);
                }

                return documentList;
            }
            else
                throw new DirectoryNotFoundException("The given folder does not exist");
        }

        public static string CreateImageFromPdf(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string command = "C:\\gswin32c.exe -sDEVICE=png16m -r100 -dFirstPage=1 -dLastPage=1 -dGraphicsAlphaBits=4 -dTextAlphaBits=4 -dDOINTERPOLATE -o \"" + path + ".png\" \"" + path + "\" 2>&1";

                    ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    return path + ".png";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Could not generate the image file for the PDF.\n\n" + ex.Message, "ERROR");
            }

            return null;
        }

        /// <summary>
        /// Retrieves a document object from the database given its ID
        /// </summary>
        /// <param name="docID">The document's ID</param>
        /// <returns>The retrieved document from the database</returns>
        /*
        public static Document GetDocumentById(int docId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Env.CS))
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand("select * from Documents where ID=@ID", connection);
                    SQLiteParameter param = cmd.Parameters.Add("@ID", DbType.Int32);
                    param.Value = docId;

                    connection.Open();
                    Document document = LightweightDAL.DBHelper.ReadObject<Document>(cmd);

                    if (document != null)
                        document.GetTagsFromDB();

                    return document;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message, "ERROR");
                    return null;
                }
            }
        }
        * */

        public static IEnumerable<Document> GetAllDocuments()
        {
            return from Document d in Env.db select d;
        }

        public static void AddToDatabase(Document document)
        {
            Env.db.Store(document);
        }
        #endregion
    }
}
