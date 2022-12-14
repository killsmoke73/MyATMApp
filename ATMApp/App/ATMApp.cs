using ATMApp.Domain.Entitie;
using ATMApp.Domain.Enums;
using ATMApp.Domain.Interfaces;
using ATMApp.UI;
using System;
using System.Web;

namespace ATMApp.App
{

   public class ATMApp : IUserLogin, IUserAccountActions, ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const double minimumKeptAmount = 500;
		public void CheckUserCardNumberAndPassword()
		{
			bool isCorrectLogin = false;
            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach(UserAccount account in userAccountList)
                {
                    selectedAccount= account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if(inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;

                            if(selectedAccount.IsLocked || selectedAccount.TotalLogin >= 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
					if (isCorrectLogin == false)
					{
                        Utility.printMessage("\n Invalid card number or PIN, please try again.", false);
						selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
						if (selectedAccount.IsLocked)
						{
							AppScreen.PrintLockScreen();
						}
					}
				}
            }
            
            Console.Clear();
		}
      

		public void Run()
        {
			AppScreen.Welcome();
			CheckUserCardNumberAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            AppScreen.DisplayAppMenu();
            processMenuoption();
		}

		public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount { Id = 1, FullName = "Emmanuel Chidera", AccountNumber = 068968, CardNumber = 555666, CardPin = 222222, AccountBalance = 100000.00, IsLocked = false},
                new UserAccount { Id = 2, FullName = "Abubakar Ibrahim", AccountNumber = 402088, CardNumber = 657657, CardPin = 111111, AccountBalance = 4000.00, IsLocked = false},
                new UserAccount { Id = 3, FullName = "Amazing Chioma", AccountNumber = 656014, CardNumber = 333555, CardPin = 121212, AccountBalance = 20000.00, IsLocked = true },
            };
            _listOfTransactions = new List<Transaction>();

           
        }


    private void processMenuoption()
        {
            switch(validator.Convert<int>("an option:"))
            {
				case (int)AppMenu.CheckBalance:
					CheckBalance();
					break;
				case (int)AppMenu.PlaceDeposit:
                    placeDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    MakeWithdrawal();
                    break;
                case (int)AppMenu.InternalTransfer:
                    Console.WriteLine("Making internal transfer...");
                    break;
                case (int)AppMenu.ViewTransaction:
                    Console.WriteLine("Viewing transactions...");
                    break;
                case (int)AppMenu.Logout:
                  AppScreen.LogOutProgress();
                    Utility.printMessage("You have successfully logged out. Please collect your ATM Card.", true);
                    Run();
                    break;
                default:
                    Utility.printMessage("Invalid Option.", false);
                    break;

            }
        }

		private static void NewMethod()
		{
			Console.WriteLine("Checking account balance...");
		}

		public void CheckBalance()
		{
            Utility.printMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}", true); 
		}

		public void placeDeposit()
		{
            Console.WriteLine("\nOnly multiples of 500 and 1000 naira allowed.\n");
            var transaction_amt = validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimaton();
            Console.WriteLine("");

            //some guard clause
            if (transaction_amt <= 0)
            {
                Utility.printMessage("Amount needs to be greater than zero. Try again.", false);
                return;
            }
            if (transaction_amt % 500 != 0)
            {
                Utility.printMessage($"Enter deposit amount in multiple of 500 or 1000. Try again.", false);
                return;
            }

            if(PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.printMessage($"You have camcelled your action.", false);
                return;
            }

            //bind transaction detail to tranction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success message
            Utility.printMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successful.", true);



		}

		public void MakeWithdrawal()
		{
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            if (selectedAmount == -1)
            {
                selectedAmount = AppScreen.SelectAmount();
            }
            else if (selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = validator.Convert<int>($"amount {AppScreen.cur}");
            }

            //input validation
            if(transaction_amt <= 0)
            {
                Utility.printMessage("Amount needs to be greater than zero. try again", false);
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.printMessage("You can only withdraw amount in multiples of 500 and 1000 naira . Try agin.", false);
                return;
            }
            //Business logic Validations

            if(transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.printMessage($"Withdrawal failed. Your balance is too low to withdrwa{Utility.FormatAmount(transaction_amt)}", false);
                return;
            }
            if((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.printMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //Bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transaction_amt, "");
            //update account balance
            selectedAccount.AccountBalance -= transaction_amt;
            //success message
            Utility.printMessage($"You have successfully withdrawn{Utility.FormatAmount(transaction_amt)}.", true);


        }

            private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNoteCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("------");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 X {fiveHundredNoteCount} = {500 * fiveHundredNoteCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");


            int opt = validator.Convert<int>("1 to confirm");
            return opt.Equals(1); 
        }

            public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _traAmount, string _desc)
		{
            //create a new transaction object
            var transation = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _traAmount,
                Description = _desc
            };

            //add transaction object to the list
            _listOfTransactions.Add(transation);
		}

		public void ViewTransaction()
		{
			throw new NotImplementedException();
		}
        private void Processinternaltransfer(InternalTransfer internalTransfer)
        {
            if(internalTransfer.TransferAmount <= 0)
            {
                Utility.printMessage("Amount needs to be more than zero. Try again", false);
                return;
            }
            //Check sender's account balance
            if(internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.printMessage($"Transfer failed. You do not have enough balance to tranfer {Utility.FormatAmount(internalTransfer.TransferAmount)}.", false);
                return;
            }
            //check the minimum kept amount
            if((selectedAccount.AccountBalance - minimumKeptAmount) <minimumKeptAmount)
            {
                Utility.printMessage($"Transfer failed. Your account needs to have minimum{Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }

            //check if reciever bank account number is valid
            var selectedBankAccountReciever = (from userAcc in userAccountList
                                               where userAcc.AccountNumber == internalTransfer.reciepientBankAccountNumber
											   select userAcc).FirstOrDefault();

            if(selectedBankAccountReciever == null)
            {
                Utility.printMessage("Transfer failed. Reciever bank account number is invalid.", false);
                return; 
            }

        //check reciever's name
        


		}
	}
}