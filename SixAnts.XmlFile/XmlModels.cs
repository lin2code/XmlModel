using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixAnts.Tools
{
    /// <summary>
    /// Xml标签
    /// XmlTag
    /// </summary>
    public abstract class XmlTag
    {
        /// <summary>
        /// 名称
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 属性
        /// attributes
        /// </summary>
        public Dictionary<string, string> Attrs { get; set; }
    }

    /// <summary>
    /// Xml子标签：子标签内必须有子标签或基础标签
    /// XmlChildTag:At least have a XmlChildTag or XmlBaseTag inner XmlChildTag
    /// </summary>
    public class XmlChildTag : XmlTag
    {
        /// <summary>
        /// 子标签列表
        /// list of XmlChildTag
        /// </summary>
        public List<XmlChildTag> ChildTagList { get; set; }

        /// <summary>
        /// 基础标签列表
        /// list of XmlBaseTag
        /// </summary>
        public List<XmlBaseTag> BaseTagList { get; set; }

        /// <summary>
        /// 构造器
        /// Constructor
        /// </summary>
        /// <param name="name">标签名称/Tag's Name</param>
        public XmlChildTag(string name)
        {
            Name = name;
            Attrs = new Dictionary<string, string>();
            ChildTagList = new List<XmlChildTag>();
            BaseTagList = new List<XmlBaseTag>();
        }

        /// <summary>
        /// 查找此标签内的一个子标签
        /// Get a XmlChildTag from this tag
        /// </summary>
        /// <param name="tagNames">按顺序输入的标签名称/tag's name input by order</param>
        /// <returns></returns>
        public XmlChildTag GetChildTag(params string[] tagNames)
        {
            XmlChildTag result = this as XmlChildTag;
            for (int i = 0; i < tagNames.Length; i++)
            {
                if (result.ChildTagList.Where(t => t.Name == tagNames[i]).Count() == 0)
                {
                    throw new Exception("找不到XmlChildTag类型节点：" + tagNames[i]);
                }
                result = result.ChildTagList.Where(t => t.Name == tagNames[i]).First();
            }
            return result;
        }

        /// <summary>
        /// 查找此标签内的一个基础标签
        /// Get a XmlBaseTag from this tag
        /// </summary>
        /// <param name="tagNames">按顺序输入的节点名称/tag's name input by order</param>
        /// <returns></returns>
        public XmlBaseTag GetBaseTag(params string[] tagNames)
        {
            XmlChildTag ChildTag = this as XmlChildTag;
            for (int i = 0; i < tagNames.Length; i++)
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

    /// <summary>
    /// Xml基础标签：基础标签内不能有子标签，可以有内部文字
    /// XmlBaseTag:XmlBaseTag don't have child tag, but inner text
    /// </summary>
    public class XmlBaseTag : XmlTag
    {
        /// <summary>
        /// 内部文字
        /// InnerText
        /// </summary>
        public string InnerText { get; set; }

        /// <summary>
        /// 构造器
        /// Constructor
        /// </summary>
        /// <param name="name">标签名称/Tag's Name</param>
        public XmlBaseTag(string name)
        {
            Name = name;
            Attrs = new Dictionary<string, string>();
        }
    }
}
