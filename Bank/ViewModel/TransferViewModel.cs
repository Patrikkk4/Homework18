using BankLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static BankLibrary.BankClassLibrary;

namespace Bank.ViewModel
{
    /// <summary>
    /// Клас предоставляет свойства и методы для осуществления переаода денег
    /// </summary>
    public class TransferViewModel : MainVM
    {
        #region Поля
        /// <summary>
        /// Делегат недостаточного колличества средств
        /// </summary>
        /// <param name="msg"></param>
        delegate void NeedMoreMoney(string msg);
        /// <summary>
        /// Событие недостаточности средств
        /// </summary>
        private static event NeedMoreMoney NeedMoreMoneyEvent = msg => MessageBox.Show($"{msg}");
        /// <summary>
        /// Делегат для получения клиентов
        /// </summary>
        /// <returns></returns>
        delegate ObservableCollection<Client> GetTransferClients();
        /// <summary>
        /// Поле выбранной фамилии
        /// </summary>
        private string selectedName;
        /// <summary>
        /// Поле выбранного счета
        /// </summary>
        private int selectedBill;
        /// <summary>
        /// Коллекция счетов клиентов
        /// </summary>
        private List<int> bills = new List<int>();
        /// <summary>
        /// Поле переводимой суммы
        /// </summary>
        private int transferSum;
        /// <summary>
        /// Поле статуса перевода
        /// </summary>
        private string transferMessage;

        private List<string> lastNames = new List<string>();

        #endregion

        #region Свойства

        /// <summary>
        /// Свойство коллекция всех клиентов
        /// </summary>
        public ObservableCollection<Client> AllClients { get; set; } = new ObservableCollection<Client>();

        /// <summary>
        /// Свойство счета с которого производится перевод
        /// </summary>
        public InclusionModel BillOfTransfer { get; set; }

        /// <summary>
        /// Свойство коллекция всех счетов
        /// </summary>
        public List<int> Bills 
        {
            get { return bills; }
            set { bills = value; OnPropertyChange(nameof(Bills)); }
        }

        /// <summary>
        /// Свойство выбранной фамилии
        /// </summary>
        public string SelectedName 
        {
            get { return selectedName; }
            set { selectedName = value; OnPropertyChange(nameof(SelectedName)); Bills = Bills.TakeBills(AllClients, SelectedName); }
        }

        /// <summary>
        /// Свойство выбранного счета, куда будет производится перевод
        /// </summary>
        public int SelectedBill
        {
            get { return selectedBill; }
            set { selectedBill = value; OnPropertyChange(nameof(SelectedBill)); TransferMessage = string.Empty; }
        }

        /// <summary>
        /// Свойство суммы перевода
        /// </summary>
        public int TransferSum
        {
            get { return transferSum; }
            set { transferSum = value; OnPropertyChange(nameof(TransferSum)); }
        }

        /// <summary>
        /// Свойство статуса перевода
        /// </summary>
        public string TransferMessage
        {
            get { return transferMessage; }
            set { transferMessage = value; OnPropertyChange(nameof(TransferMessage)); }
        }

        #endregion

        #region Команды

        /// <summary>
        /// Команда перевода
        /// </summary>
        public ICommand Transfer
        {
            get
            {
                return new RelayCommand<object>(obj =>
                {
                    // Условия, при которых осучествляется перевод
                    if (SelectedBill != 0 && TransferSum != 0)
                    {
                        // Условие проверки наличия достаточного колличества средств
                        if (TransferSum <= BillOfTransfer.Inclusion)
                        {
                            // Вызов метода перевода средств
                            ClientViewModel.Transfer(BillOfTransfer.Bill, SelectedBill, TransferSum);

                            TransferMessage = $"Осуществлен перевод на счет {SelectedBill}";
                        }
                        else
                        {
                            // Возникновение события при недостаточном колличестве средств
                            NeedMoreMoneyEvent?.Invoke("Недостаточно средств для перевода.");

                            TransferMessage = "Недостаточно средств";
                        }
                    }
                    else
                    {
                        TransferMessage = "Перевод не осуществлен";
                    }
                });
            }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор принимает объект выбранного счета
        /// </summary>
        /// <param name="inclusionModel"></param>
        public TransferViewModel(InclusionModel inclusionModel)
        {
            GetTransferClients getTransferClients = ClientViewModel.AllClientsForTransfer;

            AllClients = getTransferClients();

            BillOfTransfer = inclusionModel;
        }

        public TransferViewModel()
        {
        }

        #endregion
    }
}
