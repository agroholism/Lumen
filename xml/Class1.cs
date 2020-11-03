using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

public static class Main {
/*	public static void Import(Scope scope, String s) {
		scope.Bind("Xml", XMLModule.Instance);
	}

	public class XMLModule : Module {
		public static Module XmlDocument = new XmlDocumentModule();
		public static Module XmlElement = new XmlElementModule();
		public static Module XmlAttribute = new XmlAttributeModule();
		public static Module XmlName = new XmlNameModule();
		public static Module Instance = new XMLModule();

		public XMLModule() {
			this.SetMember("Document", XmlDocument);
			this.SetMember("Element", XmlElement);
			this.SetMember("Attribute", XmlAttribute);
			this.SetMember("Name", XmlName);

			this.SetMember("read", new LambdaFun((scope, args) => {
				XDocument document = XDocument.Load(scope["fileName"].ToString(scope));
				return new XmlDocument(document);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fileName")
				}
			});

			this.SetMember("elements", new LambdaFun((scope, args) => {
				XDocument document = (scope["document"] as XmlDocument).Document;
				return new Stream(document.Elements().Select(i => new XmlElement(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("document")
				}
			});

			this.SetMember("getElements", new LambdaFun((scope, args) => {
				XDocument document = (scope["document"] as XmlDocument).Document;
				XName name = (scope["name"] as XmlName).Value;

				return new Stream(document.Elements(name).Select(i => new XmlElement(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("document"),
					new NamePattern("name"),
				}
			});
		}
	}

	public class XmlDocumentModule : Module {
		public XmlDocumentModule() {

		}
	}

	public class XmlAttributeModule : Module {
		public XmlAttributeModule() {
			this.SetMember("getName", new LambdaFun((scope, args) => {
				XAttribute element = (scope["element"] as XmlAttribute).Attribute;
				return new Text(element.Name.LocalName);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element")
				}
			});

			this.SetMember("getText", new LambdaFun((scope, args) => {
				XAttribute element = (scope["attribute"] as XmlAttribute).Attribute;
				return new Text(element.Value);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("attribute")
				}
			});

			this.SetMember("setText", new LambdaFun((scope, args) => {
				XAttribute element = (scope["attribute"] as XmlAttribute).Attribute;
				element.Value = scope["text"].ToString(scope);
				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("attribute"),
					new NamePattern("text"),
				}
			});
		}
	}

	public class XmlElementModule : Module {
		public XmlElementModule() {
			this.SetMember("getName", new LambdaFun((scope, args) => {
				XElement element = (scope["element"] as XmlElement).Element;
				return new Text(element.Name.LocalName);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element")
				}
			});

			this.SetMember("getText", new LambdaFun((scope, args) => {
				XElement element = (scope["element"] as XmlElement).Element;
				return new Text(element.Value);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element")
				}
			});

			this.SetMember("setText", new LambdaFun((scope, args) => {
				XElement element = (scope["element"] as XmlElement).Element;
				element.Value = scope["text"].ToString(scope);
				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element"),
					new NamePattern("text"),
				}
			});

			this.SetMember("elements", new LambdaFun((scope, args) => {
				XElement element = (scope["element"] as XmlElement).Element;
				return new Stream(element.Elements().Select(i => new XmlElement(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element")
				}
			});

			this.SetMember("getElements", new LambdaFun((scope, args) => {
				XElement element = (scope["element"] as XmlElement).Element;
				XName name = (scope["name"] as XmlName).Value;

				return new Stream(element.Elements(name).Select(i => new XmlElement(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element"),
					new NamePattern("name")
				}
			});

			this.SetMember("attributes", new LambdaFun((scope, args) => {
				XElement element = (scope["element"] as XmlElement).Element;
				return new Stream(element.Attributes().Select(i => new XmlAttribute(i)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element")
				}
			});
		}
	}

	public class XmlNameModule : Module {
		public XmlNameModule() {
			this.SetMember("defaultConstructor", new LambdaFun((scope, args) => {
				return new XmlName(XName.Get(scope["name"].ToString(scope), scope["namespace"].ToString(scope))) ;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("namespace"),
					new NamePattern("name")
				}
			});
		}
	}

	public class XmlDocument : Value {
		internal XDocument Document { get; set; }

		public XmlDocument(XDocument document) {
			this.Document = document;
		}

		public IType Type => XMLModule.XmlDocument;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return Document.ToString();
		}

		public override String ToString() {
			return Document.ToString();
		}
	}
*/
	/*public class XmlElement : Value {
		internal XElement Element { get; set; }

		public XmlElement(XElement node) {
			this.Element = node;
		}

		public IType Type => XMLModule.XmlElement;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return Element.ToString();
		}

		public override String ToString() {
			return Element.ToString();
		}
	}

	public class XmlAttribute : Value {
		internal XAttribute Attribute { get; set; }

		public XmlAttribute(XAttribute attribute) {
			this.Attribute = attribute;
		}

		public IType Type => XMLModule.XmlAttribute;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return Attribute.ToString();
		}

		public override String ToString() {
			return Attribute.ToString();
		}
	}
	*/
	/*public class XmlName : Value {
		internal XName Value { get; set; }

		public XmlName(XName value) {
			this.Value = value;
		}

		public IType Type => XMLModule.XmlName;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return Value.ToString();
		}

		public override String ToString() {
			return Value.ToString();
		}
	}*/
}
