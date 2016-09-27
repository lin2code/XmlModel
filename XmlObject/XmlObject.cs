using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace CodeFarmer.Tools
{
    /// <summary>
    /// Object mapping of a xml file
    /// </summary>
    public class XmlObject
    {
        /// <summary>
        /// Directory of this xml file 
        /// </summary>
        public string XmlDirectory { get; set; }

        /// <summary>
        /// Xml file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Xml file encode
        /// </summary>
        public string Encode { get; set; }

        /// <summary>
        /// Root tag
        /// </summary>
        public XmlTag Root { get; set; }

        /// <summary>
        /// XmlDocument object of this xml file
        /// </summary>
        private XmlDocument XmlDoc { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public XmlObject()
        {

        }

        /// <summary>
        /// Constructor: Use to create a xml file
        /// </summary>
        /// <param name="directory">directory</param>
        /// <param name="fileName">fileName</param>
        /// <param name="encode">encode type</param>
        /// <param name="root">root tag</param>
        public XmlObject(string directory, string fileName, string encode, XmlTag root)
        {
            Create(directory, fileName, encode, root);
        }

        /// <summary>
        /// Constructor: Use to read a xml file
        /// </summary>
        /// <param name="directory">directory</param>
        /// <param name="fileName">fileName</param>
        public XmlObject(string directory, string fileName)
        {
            Read(directory, fileName);
        }

        /// <summary>
        /// Create a xml file
        /// </summary>
        /// <param name="directory">directory</param>
        /// <param name="fileName">fileName</param>
        /// <param name="encode">encode type</param>
        /// <param name="root">root tag</param>
        public void Create(string directory, string fileName, string encode, XmlTag root)
        {
            XmlDirectory = directory;
            FileName = fileName;
            Encode = encode;
            //初始化
            Root = root;
            XmlDoc = new XmlDocument();
            XmlDoc.AppendChild(XmlDoc.CreateXmlDeclaration("1.0", Encode, ""));
            Save();
        }

        /// <summary>
        /// Read a xml file
        /// </summary>
        /// <param name="directory">directory</param>
        /// <param name="fileName">fileName</param>
        public void Read(string directory, string fileName)
        {
            XmlDirectory = directory;
            FileName = fileName;
            XmlDoc = new XmlDocument();
            XmlDoc.Load(directory + fileName);
            Root = NodeToTag(XmlDoc.DocumentElement);
        }

        /// <summary>
        /// Save Object to file
        /// </summary>
        public void Save()
        {
            if(Root == null)
            {
                throw new Exception("Root can't be null");
            }
            GetXmlDocument();
            if(!Directory.Exists(XmlDirectory))
            {
                Directory.CreateDirectory(XmlDirectory);
            }
            XmlDoc.Save(XmlDirectory + FileName);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        public void Delete()
        {
            if(File.Exists(XmlDirectory + FileName))
            {
                File.Delete(XmlDirectory + FileName);
            }
        }

        /// <summary>
        /// Get current XmlObject's XmlDocument type object
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlDocument()
        {
            //更新前清空原内容
            if (XmlDoc.DocumentElement != null)
            {
                XmlDoc.RemoveChild(XmlDoc.DocumentElement);
            }
            XmlDoc.AppendChild(TagToNode(Root));
            return XmlDoc;
        }

        /// <summary>
        /// Recurtion transfer XmlNode into XmlTag
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <returns></returns>
        public XmlTag NodeToTag(XmlNode node)
        {
            //不转化注释
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Name == ("#comment"))
                {
                    node.RemoveChild(node.ChildNodes[i]);
                }
            }
            //判断当前节点类型返回对应类型节点
            if (node.ChildNodes.Count > 0 && node.FirstChild.NodeType != XmlNodeType.Text)
            {
                XmlChildTag childTag = new XmlChildTag(node.Name);
                //属性不为空添加属性
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        childTag.Attrs.Add(attr.Name, attr.Value);
                    }
                }
                //递归添加子节点
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    XmlTag tag = NodeToTag(childNode);
                    if(tag is XmlChildTag)//根据子节点类型加入对应列表
                    {
                        childTag.ChildTagList.Add(tag as XmlChildTag);
                    }
                    else
                    {
                        childTag.BaseTagList.Add(tag as XmlBaseTag);
                    }
                }
                return childTag;
            }
            else
            {
                XmlBaseTag baseTag = new XmlBaseTag(node.Name);
                //属性不为空添加属性
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        baseTag.Attrs.Add(attr.Name, attr.Value);
                    }
                }
                baseTag.InnerText = node.InnerText;
                return baseTag;
            }
        }

        /// <summary>
        /// Recurtion transfer XmlTag into XmlNode
        /// </summary>
        /// <param name="tag">XmlTag</param>
        /// <returns></returns>
        public XmlNode TagToNode(XmlTag tag)
        {
            XmlNode node = XmlDoc.CreateElement(tag.Name);
            foreach (var attr in tag.Attrs)
            {
                XmlAttribute xmlAttr = XmlDoc.CreateAttribute(attr.Key);
                xmlAttr.Value = attr.Value;
                node.Attributes.Append(xmlAttr);
            }
            if (tag is XmlChildTag)
            {
                XmlChildTag childTag = tag as XmlChildTag;
                if(childTag.BaseTagList.Count == 0 && childTag.ChildTagList.Count == 0)
                {
                    throw new Exception("A XmlChildTag " + childTag.Name + "'s BaseTagList and ChildTagList can't both empty，if this tag don't have any child please use XmlBaseTag type");
                }
                //添加对应的子节点
                foreach (XmlTag innerTag in childTag.BaseTagList)
                {
                    node.AppendChild(TagToNode(innerTag));
                }
                foreach (XmlTag innerTag in childTag.ChildTagList)
                {
                    node.AppendChild(TagToNode(innerTag));
                }
            }
            else
            {
                XmlBaseTag baseTag = tag as XmlBaseTag;
                node.InnerText = baseTag.InnerText;
            }
            return node;
        }

        /// <summary>
        /// Quickly get a XmlChildTag
        /// </summary>
        /// <param name="tagNames">tag's name input by order</param>
        /// <returns></returns>
        public XmlChildTag GetChildTag(params string[] tagNames)
        {
            if(tagNames[0] != Root.Name)
            {
                throw new Exception("Root tag doesn't match");
            }
            if(!(Root is XmlChildTag))
            {
                throw new Exception("Root " + tagNames[0] + " must be XmlChildTag type");
            }
            XmlChildTag result = Root as XmlChildTag;
            for (int i = 1; i < tagNames.Length; i++)
            {
                if(result.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("Can't find XmlChildTag tag " + tagNames[i] + " by the input tag names with this order");
                }
                result = result.ChildTagList.Where(t => t.Name == tagNames[i]).First();
            }
            return result;
        }

        /// <summary>
        /// Quickly get a XmlBaseTag
        /// </summary>
        /// <param name="tagNames">按顺序输入的节点名称/tag's name input by order</param>
        /// <returns></returns>
        public XmlBaseTag GetBaseTag(params string[] tagNames)
        {
            if (tagNames[0] != Root.Name)
            {
                throw new Exception("Root tag doesn't match");
            }
            if (Root is XmlBaseTag)
            {
                if (tagNames.Length == 1)
                {
                    return Root as XmlBaseTag;
                }
                else
                {
                    throw new Exception("Root " + tagNames[0] + " is XmlBaseTag type, don't have any childs");
                }
            }
            XmlChildTag ChildTag = Root as XmlChildTag;
            for (int i = 1; i < tagNames.Length; i++)
            {
                if (ChildTag.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0 && ChildTag.BaseTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("Can't find XmlTag " + tagNames[i] + " by the input tag names with this order");
                }
                //最后一个节点查找BaseTagList，否则查找ChildTagList
                if (i == tagNames.Length - 1)
                {
                    if (ChildTag.BaseTagList.Where(t => t.Name == tagNames[i]).Count() > 0)
                    {
                        return ChildTag.BaseTagList.Where(t => t.Name == tagNames[i]).First();
                    }
                    else
                    {
                        throw new Exception("Tag：" + tagNames[i] + " must be XmlBaseTag type");
                    }
                }
                else
                {
                    //找到对应XmlChildTag节点缩小查找范围
                    if (ChildTag.ChildTagList.Where(t => t.Name == tagNames[i]).Count() > 0)
                    {
                        ChildTag = ChildTag.ChildTagList.Where(t => t.Name == tagNames[i]).First();
                    }
                    else
                    {
                        throw new Exception("Tag：" + tagNames[i] + " must be XmlChildTag type");
                    }
                }
            }
            return null;
        }
    }
}
