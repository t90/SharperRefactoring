using System;
using MonoDevelop.Components.Commands;
using System.Diagnostics;
using System.IO;

namespace github.com.t90.Sharper.Refactoring
{
	public class IntroduceVariableHandler : CommandHandler 
	{
		protected override void Run ()
		{
			using(var s = new StreamWriter(@"H:\TEMP\qweasd\run.log")){
				s.WriteLine("{0}",DateTime.Now);
			}
			Debugger.Launch();
			DateTime.Now.ToString();
		}

		protected override void Update (CommandInfo info)
		{
			Debugger.Launch();
			DateTime.Now.ToString();
		}

		public IntroduceVariableHandler ()
		{
		}
	}
}

