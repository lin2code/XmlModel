using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace SixAnts.Tools
{
    /// <summary>
    /// Xml文件
    /// XmlFile
    /// </summary>
    public class XmlFile
    {
        /// <summary>
        /// 文件路径
        /// File Directory
        /// </summary>
        public string XmlDirectory { get; set; }

        /// <summary>
        /// 文件名称
        /// File Name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件编码方式
        /// File Encode
        /// </summary>
        public string Encode { get; set; }

        /// <summary>
        /// 根节点
        /// Root Tag
        /// </summary>
        public XmlTag Root { get; set; }

        /// <summary>
        /// XmlDocument格式内容
        /// XmlDocument type content
        /// </summary>
        private XmlDocument XmlDoc { get; set; }

        /// <summary>
        /// 默认构造
        /// Default Constructor
        /// </summary>
        public XmlFile()
        {

        }

        /// <summary>
        /// 构造：用于创建Xml文件
        /// Constructor:Use to create a xml file
        /// </summary>
        /// <param name="directory">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="encode">编码方式</param>
        /// <param name="root">根节点</param>
        public XmlFile(string directory, string fileName, string encode, XmlTag root)
        {
            Create(directory, fileName, encode, root);
        }

        /// <summary>
        /// 构造：用于读取Xml文件
        /// Constructor:Use to read a xml file
        /// </summary>
        /// <param name="directory">路径</param>
        /// <param name="fileName">文件名</param>
        public XmlFile(string directory, string fileName)
        {
            Read(directory, fileName);
        }

        /// <summary>
        /// 创建Xml文件
        /// Create a xml file
        /// </summary>
        /// <param name="directory">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="encode">编码</param>
        /// <param name="root">根节点</param>
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
        /// 读取Xml文件
        /// Read a xml file
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        public void Read(string directory, string fileName)
        {
            XmlDirectory = directory;
            FileName = fileName;
            XmlDoc = new XmlDocument();
            XmlDoc.Load(directory + fileName);
            Root = NodeToTag(XmlDoc.DocumentElement);
        }

        /// <summary>
        /// 保存
        /// Save
        /// </summary>
        public void Save()
        {
            if(Root == null)
            {
                throw new Exception("Root不能为空");
            }
            GetXmlDocument();
            if(!Directory.Exists(XmlDirectory))
            {
                Directory.CreateDirectory(XmlDirectory);
            }
            XmlDoc.Save(XmlDirectory + FileName);
        }

        /// <summary>
        /// 获取当前XmlFile对应的XmlDocument
        /// Get current XmlFile's XmlDocument
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
        /// 递归转换XmlNode为XmlTag
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
        /// 递归转换XmlTag为XmlNode
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
                    throw new Exception("XmlChildTag" + childTag.Name + "的BaseTagList和ChildTagList不能同时为空，如果当前节点没有子节点请使用XmlBaseTag类型");
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
        /// 快速获取一个XmlChildTag
        /// Quickly get a XmlChildTag
        /// </summary>
        /// <param name="tagNames">按顺序输入的节点名称/tag's name input by order</param>
        /// <returns></returns>
        public XmlChildTag GetChildTag(params string[] tagNames)
        {
            if(tagNames[0] != Root.Name)
            {
                throw new Exception("根节点名称不正确。");
            }
            if(!(Root is XmlChildTag))
            {
                throw new Exception("节点：" + tagNames[0] + " 不是XmlChildTag类型");
            }
            XmlChildTag result = Root as XmlChildTag;
            for (int i = 1; i < tagNames.Length; i++)
            {
                if(result.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("找不到XmlChildTag类型节点：" + tagNames[i]);
                }
                result = result.ChildTagList.Where(t => t.Name == tagNames[i]).First();
            }
            return result;
        }

        /// <summary>
        /// 快速获取一个XmlBaseTag
        /// Quickly get a XmlBaseTag
        /// </summary>
        /// <param name="tagNames">按顺序输入的节点名称/tag's name input by order</param>
        /// <returns></returns>
        public XmlBaseTag GetBaseTag(params string[] tagNames)
        {
            if (tagNames[0] != Root.Name)
            {
                throw new Exception("根节点名称不正确。");
            }
            if (Root is XmlBaseTag)
            {
                if (tagNames.Length == 1)
                {
                    return Root as XmlBaseTag;
                }
                else
                {
                    throw new Exception("节点：" + tagNames[0] + " 已经是XmlBaseTag，没有子节点");
                }
            }
            XmlChildTag ChildTag = Root as XmlChildTag;
            for (int i = 1; i < tagNames.Length; i++)
            {
                if (ChildTag.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0 && ChildTag.BaseTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("找不到子节点" + tagNames[i]);
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
                        throw new Exception("节点：" + tagNames[i] + " 必须是XmlBaseTag类型");
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
                        throw new Exception("节点：" + tagNames[i] + " 必须是XmlChildTag类型");
                    }
                }
            }
            return null;
        }
    }
}
