using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlOject;

namespace XmlObjectTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * XmlObject is an object mapping of a xml file
             * XmlObject has two type tag: XmlChildTag and XmlBaseTag
             * XmlChildTag must have child tag, means BaseTagList and ChildTagList can't both empty
             * XmlBaseTag can't have child tag insie
             */

            while (true)
            {
                Console.WriteLine("Input a command:");
                switch (Console.ReadLine().ToLower())
                {
                    case "new": 
                        Create();
                        Read();
                        Edit();
                        QuicklyGet();
                        PrintXmlObject();
                        break;
                    case "delete":
                        Delete();
                        break;
                    case "nofile":
                        CreateWithOutFile();
                        break;
                    case "string":
                        ReadXmlString();
                        break;
                }
            }
        }

        //Create a xmlfile
        public static void Create()
        {
            XmlChildTag rootTag = new XmlChildTag("Root");
            rootTag.BaseTagList.Add(new XmlBaseTag("BaseTagOne"));
            XmlObject myXmlObject = new XmlObject("D:\\XML\\", "MyXmlObject.xml", "utf-8", rootTag);
            Console.WriteLine("Create success");
        }

        //Read a xml file
        public static void Read()
        {
            XmlObject myXml = new XmlObject("D:\\XML\\", "MyXmlObject.xml");
            string rootName = myXml.Root.Name;
            string baseTagOneName = (myXml.Root as XmlChildTag).BaseTagList.First().Name;
            Console.WriteLine("Read success");
        }

        //Edit a xml file 
        public static void Edit()
        {
            XmlObject myXml = new XmlObject("D:\\XML\\", "MyXmlObject.xml");
            //edit root name and attributes
            XmlChildTag root = myXml.Root as XmlChildTag;
            root.Name = "NewRoot";
            root.Attrs.Add("attr1", "value1");
            //root add a child tag which have three base tag
            XmlChildTag newChild = new XmlChildTag("NewChild");
            newChild.BaseTagList.Add(new XmlBaseTag("BaseTagTwo"));

            XmlBaseTag baseTagThree = new XmlBaseTag("BaseTagThree");
            baseTagThree.Attrs.Add("testa", "testv");
            newChild.BaseTagList.Add(baseTagThree);

            newChild.BaseTagList.Add(new XmlBaseTag("BaseTagFour") { InnerText = "some text" });
            root.ChildTagList.Add(newChild);
            //save to file
            myXml.Save();
            Console.WriteLine("Edit success");
        }

        //Quickly Get and edit
        public static void QuicklyGet()
        {
            XmlObject myXml = new XmlObject("D:\\XML\\", "MyXmlObject.xml");
            //GetChildTag
            XmlChildTag childA = myXml.GetChildTag("NewRoot", "NewChild");
            childA.Attrs.Add("find", "ture");

            //GetBaseTag
            XmlBaseTag baseTagTwo = myXml.GetBaseTag("NewRoot", "NewChild", "BaseTagTwo");
            baseTagTwo.InnerText = "default return first tag be found";

            //quickly get tag from a XmlChildTag
            XmlChildTag childB = myXml.GetChildTag("NewRoot", "NewChild");
            //lambda
            XmlBaseTag target = childB.BaseTagList.Where(b => b.Attrs.Keys.Contains("testa")).First();
            target.InnerText = "quickly get combine with lambda let you read write xml file very quick";
            //continue quickly get
            XmlBaseTag target2 = childB.GetBaseTag("BaseTagTwo");
            target2.InnerText = "quickly get can be use on XmlChildTag too.";

            myXml.Save();
            Console.WriteLine("QuicklyGet success");
        }

        //Delete
        public static void Delete()
        {
            XmlObject myXml = new XmlObject("D:\\XML\\", "MyXmlObject.xml");
            myXml.Delete();
            Console.WriteLine("Delete success");
        }

        //To string
        public static void PrintXmlObject()
        {
            XmlObject myXml = new XmlObject("D:\\XML\\", "MyXmlObject.xml");
            Console.WriteLine(myXml.ToString());
        }

        //Create from tag only
        public static void CreateWithOutFile()
        {
            XmlChildTag root = new XmlChildTag("root");
            root.BaseTagList.Add(new XmlBaseTag("base") { InnerText = "test" });
            XmlObject myXml = new XmlObject(root);
            myXml.XmlDirectory = "d:\\";
            myXml.FileName = "nofile.xml";
            myXml.Encode = "utf-8";
            myXml.Save();
        }

        //Read from xml String
        public static void ReadXmlString()
        {
            XmlObject newxml = new XmlObject(new XmlObject("D:\\XML\\", "MyXmlObject.xml").ToString());
            newxml.Root.Name = "newroot";
            newxml.XmlDirectory = "d:\\";
            newxml.FileName = "newxml.xml";
            newxml.Encode = "gbk";
            newxml.Save();
        }
    }
}
