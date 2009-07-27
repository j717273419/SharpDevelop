// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding.PowerToys
{
	/// <summary>
	/// Description of XamlMenuCommand
	/// </summary>
	public abstract class XamlMenuCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public sealed override void Run()
		{
			try {
				ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				
				if (provider != null) {
					TextReader reader = provider.TextEditor.Document.CreateReader();
					XDocument document = XDocument.Load(reader, LoadOptions.SetLineInfo);
					document.Declaration = null;
					if (Refactor(provider.TextEditor, document)) {
						using (provider.TextEditor.Document.OpenUndoGroup()) {
							StringWriter sWriter = new StringWriter(CultureInfo.InvariantCulture);
							XmlWriter writer = XmlWriter.Create(sWriter, CreateSettings());
							document.WriteTo(writer);
							writer.Flush();
							provider.TextEditor.Document.Text = sWriter.ToString();
						}
					}
				}
			} catch (XmlException e) {
				MessageService.ShowError(e);
			}
		}
		
		static XmlWriterSettings CreateSettings()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.OmitXmlDeclaration = true;
			settings.NewLineOnAttributes = true;
			return settings;
		}
		
		protected abstract bool Refactor(ITextEditor editor, XDocument document);
	}
}
