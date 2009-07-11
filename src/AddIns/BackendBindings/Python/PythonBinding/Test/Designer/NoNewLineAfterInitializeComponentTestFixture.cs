// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Refactoring;
using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the code can be generated if there is no new line after the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class NoNewLineAfterInitializeComponentMethodTestFixture
	{
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (TextEditorControl textEditor = new TextEditorControl()) {
				document = textEditor.Document;
				textEditor.Text = GetTextEditorCode();

				PythonParser parser = new PythonParser();
				ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.TextContent);

				using (DesignSurface designSurface = new DesignSurface(typeof(UserControl))) {
					IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
					UserControl userControl = (UserControl)host.RootComponent;			
					userControl.ClientSize = new Size(489, 389);
					
					PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(userControl);
					PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
					namePropertyDescriptor.SetValue(userControl, "userControl1");
					
					PythonDesignerGenerator.Merge(userControl, new TextEditorDocument(document), compilationUnit, new MockTextEditorProperties(), null);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "from System.Windows.Forms import UserControl\r\n" +
									"\r\n" +
									"class MyUserControl(UserControl):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself.InitializeComponent()\r\n" +
									"\t\r\n" +
									"\tdef InitializeComponent(self):\r\n" +
									"\t\tself.SuspendLayout()\r\n" +
									"\t\t# \r\n" +
									"\t\t# userControl1\r\n" +
									"\t\t# \r\n" + 
									"\t\tself.Name = \"userControl1\"\r\n" +
									"\t\tself.Size = System.Drawing.Size(489, 389)\r\n" +
									"\t\tself.ResumeLayout(False)\r\n" +
									"\t\tself.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, document.TextContent);
		}
		
		/// <summary>
		/// No new line after the pass statement for InitializeComponent method.
		/// </summary>
		string GetTextEditorCode()
		{
			return "from System.Windows.Forms import UserControl\r\n" +
					"\r\n" +
					"class MyUserControl(UserControl):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					 "\tdef InitializeComponent(self):\r\n" +
					"\t\tpass"; 						
		}
	}
}
