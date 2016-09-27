using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixAnts.Tools
{
    /// <summary>
    /// XmlTag: A xml tag
    /// </summary>
    public abstract class XmlTag
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attributes
        /// </summary>
        public Dictionary<string, string> Attrs { get; set; }
    }

    /// <summary>
    /// XmlChildTag: A XmlTag that at least have a XmlChildTag or XmlBaseTag inside
    /// </summary>
    public class XmlChildTag : XmlTag
    {
        /// <summary>
        /// XmlChildTags of this XmlChildTag
        /// </summary>
        public List<XmlChildTag> ChildTagList { get; set; }

        /// <summary>
        /// XmlBaseTags of this XmlChildTag
        /// </summary>
        public List<XmlBaseTag> BaseTagList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Tag's Name</param>
        public XmlChildTag(string name)
        {
            Name = name;
            Attrs = new Dictionary<string, string>();
            ChildTagList = new List<XmlChildTag>();
            BaseTagList = new List<XmlBaseTag>();
        }

        /// <summary>
        /// Get a XmlChildTag from this tag
        /// </summary>
        /// <param name="tagNames">tag's name input by order</param>
        /// <returns></returns>
        public XmlChildTag GetChildTag(params string[] tagNames)
        {
            XmlChildTag result = this as XmlChildTag;
            for (int i = 0; i < tagNames.Length; i++)
            {
                if (result.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("Can't find XmlChildTag tag " + tagNames[i] + " by the input tag names with this order");
                }
                result = result.ChildTagList.Where(t => t.Name == tagNames[i]).First();
            }
            return result;
        }

        /// <summary>
        /// Get a XmlBaseTag from this tag
        /// </summary>
        /// <param name="tagNames">tag's name input by order</param>
        /// <returns></returns>
        public XmlBaseTag GetBaseTag(params string[] tagNames)
        {
            XmlChildTag ChildTag = this as XmlChildTag;
            for (int i = 0; i < tagNames.Length; i++)
            {
                if (ChildTag.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0 && ChildTag.BaseTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("Can't find XmlTag tag " + tagNames[i] + " by the input tag names with this order");
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

    /// <summary>
    /// Xml基础标签：基础标签内不能有子标签，可以有内部文字
    /// XmlBaseTag:XmlBaseTag don't have child tag, but inner text
    /// </summary>
    public class XmlBaseTag : XmlTag
    {
        /// <summary>
        /// InnerText
        /// </summary>
        public string InnerText { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Tag's Name</param>
        public XmlBaseTag(string name)
        {
            Name = name;
            Attrs = new Dictionary<string, string>();
        }
    }
}
