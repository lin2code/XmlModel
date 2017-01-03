# XmlModel
  An object mapping to xml, makes xml CURD operation quickly like operate an object.  
## Install  
Download source code generate it yourself or search `XmlModel` in nuget or excute command below.  
```
Install-Package XmlModel
```
## Classes
* `XModel` Class use to create read xml file and find node, represent a xml file.
* `XmlTag` A abstract model of xml node.Has two properties **Name** and **Attrs** as xml node's Name and Attributes.
* `XmlChildTag` Inherit of XmlTag, model of a xml node that have child node.Has Two List **ChildTagList** and **BaseTagList** as xml node's child node.
* `XmlBaseTag` Inherit of XmlTag, model of a xml node that don't have child.Has a property **InnerText** as xml node's inner text.  

## Usage  
**`Using XmlModel;`**  

**`Create`** a xml file must have a root, root is XmlTag Type, so you must create a XmlChildTag or XmlBaseTag as root.  
Use constructor new a XModel then the xml file is created.  
The parameters are **FileDirectory**, **FileName**, **EncodeType**, **RootTag**  
```c#
            XmlChildTag rootTag = new XmlChildTag("Root");
            rootTag.BaseTagList.Add(new XmlBaseTag("BaseTagOne"));
            XModel myXModel = new XModel("D:\\XML\\", "MyXModel.xml", "utf-8", rootTag);
```  
**`Read`** a xml file use two parameters **Directory** and **FileName** constructor a new XModel.Use XmlChildTag's ChildTagList and BaseTagList find a node.  
```c#
            XModel myXml = new XModel("D:\\XML\\", "MyXModel.xml");
            string rootName = myXml.Root.Name;
            string baseTagOneName = (myXml.Root as XmlChildTag).BaseTagList.First().Name;
```  
**`Edit`** a xml file just read it then edit the XModel's root and save.Edit root just some object operations of XmlChildTag and XmlBaseTag.  
```c#
            XModel myXml = new XModel("D:\\XML\\", "MyXModel.xml");
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
```  
**`QuicklyGet`** is an method provide on XModel, XmlChildTag, XmlBaseTag. Input xml node's name in order then it will return the node you want.Because it's all objects You can use lambda or your own code get what else you want.  
```c#
            XModel myXml = new XModel("D:\\XML\\", "MyXModel.xml");
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
```  
**`Delete`** is delete the file this XModel Represent.  
```c#
            XModel myXml = new XModel("D:\\XML\\", "MyXModel.xml");
            myXml.Delete();
```  
  
And you can use XModel.ToString() get xml string. you can create a XModel From only XmlTag or xml string, but need set few properties before save to file.  
Check it out in the test project.
