# SixAnts.XmlFile
一个快速创建、读取、编辑Xml文件的类XmlFile。使用非常简单。  
再也不用去查XmlDocument、XmlNode、XmlElement类的用法了。  
用法如下：以下代码可以到测试项目中查看  

A quickly Create/Read/Edit Xml file Class XmlFile.Very simple to use.  
No more XmlDocument/XmlNode/XmlElement, never search thoese class usage again.  
how to use：code below can find in the test project  

```
    class Program
    {
        static void Main(string[] args)
        {
            //两个标签类型：XmlChildTag和XmlBaseTag
            //XmlChildTag是子标签内部必须有子标签或基础标签，即BaseTagList或ChildTagList不能同时为空
            //XmlBaseTag是基础标签，内部不能有子标签

            //Two type tag:XmlChildTag and XmlBaseTag
            //XmlChildTag is childtag type must have child tag, means BaseTagList and ChildTagList can't both empty
            //XmlBaseTag is basetag type, can't have child tag insie
            
            Console.Read();
        }

        //创建XML文件
        public static void Create()
        {
            //必须创建一个根节点，可以是XmlChildTag或XmlBaseTag
            //must create a roottag, could be XmlChildTag or XmlBaseTag
            XmlChildTag rootTag = new XmlChildTag("Root");
            rootTag.BaseTagList.Add(new XmlBaseTag("BaseTagOne"));
            //XmlBaseTag rootTag = new XmlBaseTag("BaseRoot");
            XmlFile myXmlFile = new XmlFile("D:\\XML\\", "MyXmlFile.xml", "utf-8", rootTag);
        }

        //读取XML文件
        public static void Read()
        {
            XmlFile myXml = new XmlFile("D:\\XML\\", "MyXmlFile.xml");
            Console.WriteLine(myXml.Root.Name);
            Console.WriteLine((myXml.Root as XmlChildTag).BaseTagList.First().Name);
        }

        //修改XML文件
        public static void Edit()
        {
            XmlFile myXml = new XmlFile("D:\\XML\\", "MyXmlFile.xml");
            //修改root
            //Edit root
            XmlChildTag root = myXml.Root as XmlChildTag;
            root.Name = "NewRoot";
            root.Attrs.Add("attr1", "value1");
            //添加子节点
            //add child tag
            XmlChildTag newChild = new XmlChildTag("NewChild");
            newChild.BaseTagList.Add(new XmlBaseTag("BaseTagTwo"));
            root.ChildTagList.Add(newChild);
            myXml.Save();
        }

        #region 快速获取测试前QuicklyGet.xml文件内容/QuicklyGet.xml file content before QuicklyGet test
        /* 
        <?xml version="1.0" encoding="utf-8"?>
        <Root>
          <BaseTagOne>
          </BaseTagOne>
          <ChildA>
            <BaseTagTwo>
            </BaseTagTwo>
            <BaseTagTwo>
            </BaseTagTwo>
            <BaseTagTwo>
            </BaseTagTwo>
          </ChildA>
          <ChildB>
            <ChildC name="cone">
              <BaseTagThree>
              </BaseTagThree>
            </ChildC>
            <ChildC name="ctwo">
              <BaseTagThree>
              </BaseTagThree>
              <BaseTagTarget>
              </BaseTagTarget>
            </ChildC>
            <ChildC name="cthree">
              <BaseTagThree>
              </BaseTagThree>
            </ChildC>
          </ChildB>
        </Root>
        */
        #endregion

        //快速获取
        public static void QuicklyGet()
        {
            XmlFile myXml = new XmlFile("D:\\XML\\", "QuicklyGet.xml");
            //获取子标签
            //GetChildTag
            XmlChildTag childA = myXml.GetChildTag("Root", "ChildA");
            childA.Attrs.Add("changed", "ture");
            //获取基础标签
            //GetBaseTag
            XmlBaseTag baseTagTwo = myXml.GetBaseTag("Root", "ChildA", "BaseTagTwo");
            baseTagTwo.InnerText = "默认返回第一个被找到的标签/default return first tag be found";
            //在子标签内继续快速获取标签
            //quickly get tag from a XmlChildTag
            XmlChildTag childB = myXml.GetChildTag("Root", "ChildB");
            XmlBaseTag target = childB.ChildTagList.Where(c => c.Attrs["name"] == "ctwo").First().GetBaseTag("BaseTagTarget");
            target.InnerText = "快速获取和lambda组合/quickly get combine with lambda";
            myXml.Save();
        }
    }
