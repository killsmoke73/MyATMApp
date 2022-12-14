using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.Domain.Entitie
{
	public class InternalTransfer
	{
		public double TransferAmount { get; set; }
		public long reciepientBankAccountNumber { get; set; }
		public string ReciepientBankAccountName { get; set; }
	}
}
