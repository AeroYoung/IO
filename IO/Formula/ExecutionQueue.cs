using System;
using System.Collections;
using ExpertLib;
namespace ExpertLib.Formula
{
	/// <summary>
	/// Summary description for ExecutionQueue.
	/// </summary>
	public class ExecutionQueue : Queue
	{
		public ExecutionQueue()
		{
		}

		private new void Enqueue(object ob)
		{
		}

		public void Enqueue(ExecutionItem ei)
		{
			base.Enqueue(ei);
		}

		public new ExecutionItem Dequeue()
		{
			return (ExecutionItem)base.Dequeue();
		}

		public new ExecutionItem Peek()
		{
			return (ExecutionItem)base.Peek();
		}

		public new ExecutionQueue Clone()
		{
			ExecutionQueue retEQ = new ExecutionQueue();
			IEnumerator ieNum = this.GetEnumerator();
			while(ieNum.MoveNext())
				retEQ.Enqueue((ExecutionItem)ieNum.Current);
			return retEQ;
		}
	}
}
