using System;
using System.Collections.Generic;
using System.Linq;
using Db4objects.Db4o.Linq;

namespace Documents
{
    public class Tag
    {
        private int _id;
        private string _tag;

        public int Id
        {
            get { return this._id; }
            set
            {
                if (this._id == 0)
                    this._id = value;
            }
        }

        public string Name
        {
            get { return this._tag; }
            set { this._tag = value.Trim(); }
        }

        /*
        public static int? Exists(Tag tag)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Env.CS))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("select id from tags where tag = @Tag", connection))
                {
                    cmd.Parameters.Add("@Tag", DbType.String);
                    cmd.Parameters["@Tag"].Value = tag.Name;
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        return (int)cmd.ExecuteScalar();
                }
            }

            return null;

         * }

        public static List<Tag> GetTagsForDocument(int docId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Env.CS))
            {
                using (SQLiteCommand cmd =
                    new SQLiteCommand(@"SELECT * FROM tags INNER JOIN tag_doc 
                                        ON tags.id=(select tag_doc.tag where 
                                        tag_doc.document = @DocID)", connection))
                {
                    SQLiteParameter param = cmd.Parameters.Add("@DocID", DbType.Int32);
                    param.Value = docId;
                    connection.Open();
                    List<Tag> tags = LightweightDAL.DBHelper.ReadCollection<Tag>(cmd);
                    return tags;
                }
            }
        }

        public static List<Tag> GetTagsForDocument(Document doc)
        {
            return GetTagsForDocument(doc.Id);
        }

         * */
        public static IEnumerable<Tag> GetAll()
        {
            return from Tag t in Env.db select t;
        }

        /// <summary>
        /// Adds a tag to the document. It first checks if the tag already exists
        /// in the database and if it does it links it do this document in particular.
        /// </summary>
        /// <param name="tag">Tag object to be added to the document</param>
        /// <param name="docId">Document ID in the database</param>
        
        /*
         * public static void AddToDocument(Tag tag, int docId)
        {
            int? tagID = Exists(tag);
            if (tagID == null)
            {
                using (SQLiteConnection connection = new SQLiteConnection(Env.CS))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(
                        @"insert into tags (tags) values (@Tag)", connection))
                    {
                        cmd.Parameters.Add("@Tag", DbType.String, 50);
                        connection.Open();
                        tagID = (int)LightweightDAL.DBHelper.SaveObject(tag, cmd, true);
                    }
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(Env.CS))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    @"insert into tag_doc (tag, document) values (@IDTag, @IDDocument)", connection))
                {
                    cmd.Parameters.Add("@Tag", DbType.Int32);
                    cmd.Parameters.Add("@IDDocument", DbType.Int32);
                    cmd.Parameters["@Tag"].Value = tagID;
                    cmd.Parameters["@IDDocument"].Value = docId;

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }*/
    }
}
