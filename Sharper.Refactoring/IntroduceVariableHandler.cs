using System;
using MonoDevelop.Components.Commands;
using System.Diagnostics;
using System.IO;
using MonoDevelop.Ide;
using Mono.TextEditor;
using System.Text.RegularExpressions;
using System.Linq;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace github.com.t90.Sharper.Refactoring
{
	public class IntroduceVariableHandler : CommandHandler
	{
		protected override void Run ()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			TextEditorData textEditorData = doc.GetContent<ITextEditorDataProvider> ().GetTextEditorData ();
			var text = textEditorData.SelectedText;
				
			var selectionStart = textEditorData.MainSelection.Start;
				
			var currentLineText = textEditorData.GetLineText (selectionStart.Line);

			var selectedText = textEditorData.SelectedText;

			selectedText = Regex.Replace (selectedText, "\\\"", string.Empty);
			selectedText = Regex.Replace (selectedText, "\"[^\"]*\"", string.Empty);
			selectedText = Regex.Replace (selectedText, @"\([^\)]*\)", string.Empty);
				
			var array = selectedText.ToCharArray ();
			Array.Reverse (array);
			var reverseString = new String (array);
			var variableName = Regex.Replace (reverseString, @"[^a-zA-Z]*([a-zA-Z]{3,}).*", "$1");
			array = null;
			array = variableName.ToCharArray ();
			Array.Reverse (array);
			if (array.Length != 0) {
				array [0] = Char.ToLower (array [0]);
			}
			variableName = new String (array);

			if (string.IsNullOrEmpty (variableName)) {
				variableName = "v";
			}

			var variableText = "var " + variableName + " = " + text;

			string textStart = currentLineText.Substring (0, selectionStart.Column);
			string textEnd = currentLineText.Length > textEditorData.MainSelection.End.Column ? currentLineText.Substring (textEditorData.MainSelection.End.Column) : string.Empty;
				
			var noWhitespaceTextEnd = new String (textEnd.ToCharArray ().Where (ch => !Char.IsWhiteSpace (ch) && ch != ';').ToArray ());
			if (!variableText.TrimEnd ().EndsWith (";")) {
				variableText = variableText.TrimEnd () + ";";
			}


			if (textStart.Contains ("(") || textStart.Contains ("=") || !string.IsNullOrEmpty (noWhitespaceTextEnd)) {
				var offset = textEditorData.SelectionRange.Offset - (selectionStart.Column - 1);
				var ident = textEditorData.GetIndentationString (offset);
					
				textEditorData.SelectedText = variableName;
				textEditorData.Caret.Offset = offset;

				var insertedText = ident + variableText + Environment.NewLine;

				textEditorData.InsertAtCaret (insertedText);


				textEditorData.Caret.Offset = textEditorData.Caret.Offset - insertedText.Length + 5;

				textEditorData.SetSelection (textEditorData.Caret.Offset + 4, textEditorData.Caret.Offset + 4 + variableName.Length);

			} else {
					
				textEditorData.SelectedText = variableText;
				var anchorOffset = textEditorData.MainSelection.GetAnchorOffset (textEditorData);
				textEditorData.SetSelection (anchorOffset + 4, anchorOffset + 4 + variableName.Length);
			}

		}

		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			info.Enabled = doc != null && doc.GetContent<ITextEditorDataProvider> () != null;
			if (info.Enabled) {
				var textEditorData = doc.GetContent<ITextEditorDataProvider> ().GetTextEditorData ();
				info.Enabled = !string.IsNullOrEmpty (textEditorData.SelectedText) && textEditorData.MainSelection.SelectionMode == SelectionMode.Normal;
			}
		}

		public IntroduceVariableHandler ()
		{

		}
	}
}

