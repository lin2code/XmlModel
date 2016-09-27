using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixAnts.Tools;

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

            Create();
            Read();
            Edit();
            QuicklyGet();

            Console.ReadLine();
        }

        //Create a xmlfile
        public static void Create()
        {
            XmlChildTag rootTag = new XmlChildTag("Root");
            rootTag.BaseTagList.Add(new XmlBaseTag("BaseTagOne"));
            XmlObject myXmlObject = new XmlObject("D:\\XML\\", "MyXmlObject.xml", "utf-8", rootTag);
        }

        //Read a xml file
        public static void Read()
        {
            XmlObject myXml = new XmlObject("D:\\XML\\", "MyXmlObject.xml");
            Console.WriteLine(myXml.Root.Name);
            Console.WriteLine((myXml.Root as XmlChildTag).BaseTagList.First().Name);
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
            XmlBaseTag target = childB.BaseTagList.Where(b => b.Attrs.Keys.Contains("testa")).First();
            target.InnerText = "quickly get combine with lambda let you read write xml file very quick";
            myXml.Save();
        }
    }
}
