using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using static BankLibrary.BankClassLibrary;
using System.Data.SqlClient;
using System.Windows.Data;

namespace Bank.ViewModel
{
    class InclusionViewModel : MainVM
    {
        #region Поля

        /// <summary>
        /// Выполняемый запрос SQL
        /// </summary>
        SqlCommand sqlCommand = new SqlCommand();
        /// <summary>
        /// Делегат транзакции
        /// </summary>
        /// <param name="msg"></param>
        delegate void InclusionMoneyDel(string msg);
        /// <summary>
        /// Событие изменение суммы вклада
        /// </summary>
        static event InclusionMoneyDel InclusionMoneyEvent = mes => MessageBox.Show($"{mes}");
        /// <summary>
        /// Поле коллекция статусов клиентов
        /// </summary>
        private List<string> сlientStatusList = new List<string>() { "Обычный", "VIP", "Корпоративный" };
        /// <summary>
        /// Поле статуса клиента
        /// </summary>
        private string clientStatus;
        /// <summary>
        /// Поле Статуса вклада
        /// </summary>
        private string statusInclusion = "Активен";
        /// <summary>
        /// Поле суммы вклада
        /// </summary>
        private double inclusion;
        /// <summary>
        /// Поле процентной ставки
        /// </summary>
        private double percents;
        /// <summary>
        /// Поле итоговой суммы по окончании вклада
        /// </summary>
        private double sum;
        /// <summary>
        /// Поле переданного объекта клиента
        /// </summary>
        private Client incModel;
        /// <summary>
        ///  Поле указывающее на капитализацию вклада
        /// </summary>
        private bool capitalize;
        /// <summary>
        /// Поле окончания вклада
        /// </summary>
        private DateTime dateEndInclusion;
        /// <summary>
        /// Поле коллекция, содержащая все вклады клиента
        /// </summary>
        private ObservableCollection<InclusionModel> inclusions = new ObservableCollection<InclusionModel>();
        /// <summary>
        /// Поле выбранного счета
        /// </summary>
        private InclusionModel selectInclusion;
        /// <summary>
        /// Поле вносимой суммы
        /// </summary>
        private double addMoney;
        /// <summary>
        /// поле внесения денег
        /// </summary>
        private bool radioAdd;
        /// <summary>
        /// поле снятия денег
        /// </summary>
        private bool radioWithdraw;

        #endregion

        #region Свойства

        /// <summary>
        /// Свойство коллекция содержащая все вклады клиента
        /// </summary>
        public ObservableCollection<InclusionModel> Inclusions
        {
            get { return inclusions; }
            set { inclusions = value; OnPropertyChange(nameof(Inclusions)); }
        }

        /// <summary>
        /// Свойство коллекция статусов клиентов
        /// </summary>
        public List<string> ClientStatusList
        {
            get { return сlientStatusList; }
            set { сlientStatusList = value; OnPropertyChange(nameof(ClientStatusList)); }
        }

        /// <summary>
        /// Свойство статуса вклада
        /// </summary>
        public string StatusInclusion
        {
            get { return statusInclusion; }
            set { statusInclusion = value; OnPropertyChange(nameof(StatusInclusion)); InclusionStatus(); }
        }

        /// <summary>
        /// свойство статуса клиента
        /// </summary>
        public string ClientStatus
        {
            get { return clientStatus; }
            set
            {
                clientStatus = value;
                OnPropertyChange(nameof(ClientStatus));
            }
        }

        /// <summary>
        /// Свойство суммы вклада
        /// </summary>
        public double Inclusion
        {
            get { return inclusion; }
            set
            {
                inclusion = value; OnPropertyChange(nameof(Inclusion));
            }
        }

        /// <summary>
        /// свойство процентной ставки
        /// </summary>
        public double Percents
        {
            get { return percents; }
            set { percents = value; OnPropertyChange(nameof(Percents)); }
        }

        /// <summary>
        /// Свойство итоговой суммы по окончании вклада
        /// </summary>
        public double Sum
        {
            get { return sum; }
            set { sum = value; OnPropertyChange(nameof(Sum)); }
        }

        /// <summary>
        /// Свойтсво переданного объекта клиента
        /// </summary>
        public Client IncModel
        {
            get { return incModel; }
            set { incModel = value; OnPropertyChange(nameof(IncModel)); }
        }

        /// <summary>
        /// Свойство, указывающее на капитализацию вклада
        /// </summary>
        public bool Capitalize
        {
            get { return capitalize; }
            set
            {
                capitalize = value; OnPropertyChange(nameof(Capitalize));
            }
        }

        /// <summary>
        /// Свойство даты окончания вклада
        /// </summary>
        public DateTime DateEndInclusion
        {
            get { return dateEndInclusion; }
            set { dateEndInclusion = value; OnPropertyChange(nameof(DateEndInclusion)); }
        }

        /// <summary>
        /// Свойство копия даты окончания вклада
        /// </summary>
        public static string DateEndInclusionCopy { get; set; }

        /// <summary>
        /// Свойство выбранного вклада
        /// </summary>
        public InclusionModel SelectInclusion
        {
            get { return selectInclusion; }
            set { selectInclusion = value; OnPropertyChange(nameof(SelectInclusion)); }
        }

        /// <summary>
        /// Свойство вносимой суммы
        /// </summary>
        public double AddMoney
        {
            get { return addMoney; }
            set { addMoney = value; OnPropertyChange(nameof(AddMoney)); }
        }

        /// <summary>
        /// Свойство внесения денег
        /// </summary>
        public bool RadioAdd
        {
            get { return radioAdd; }
            set { radioAdd = value; OnPropertyChange(nameof(RadioAdd)); }
        }

        /// <summary>
        /// Свойство снятия денег
        /// </summary>
        public bool RadioWithdraw
        {
            get { return radioWithdraw; }
            set { radioWithdraw = value; OnPropertyChange(nameof(RadioWithdraw)); }
        }

        /// <summary>
        /// Свойство представления коллекции
        /// </summary>
        public ListCollectionView TableView { get; set; }

        #endregion

        #region Команды

        /// <summary>
        /// Команда добавления нового вклада
        /// </summary>
        public ICommand AddNewInclusion
        {
            get
            {
                return new RelayCommand<object>(obj =>
                {
                    // Заполнение конструкора вклада
                    InclusionModel inclusionModel = new InclusionModel(IncModel.ClientStatus, Inclusion, incModel.Percents, Capitalize, DateEndInclusion.ToString("dd.MM.yyyy"), StatusInclusion);

                    // добавление в коллекцию вкладов клиента
                    IncModel.Inclusions.Add(inclusionModel);

                    TestBankLocalDBEntities billToSql = new TestBankLocalDBEntities();

                    // Заполнение экземпляра класса для внесения данных в БД
                    var newBill = new Bills
                    {
                        ClientID = IncModel.Id,
                        BillNum = inclusionModel.Bill,
                        InclusionDate = inclusionModel.DateInclusion,
                        InclusionDateEnd = inclusionModel.DateEndInclusion,
                        InclusionSum = inclusionModel.Inclusion,
                        Percents = incModel.Percents,
                        Capitalize = inclusionModel.Capitalize,
                        Total = Convert.ToInt32(inclusionModel.Sum),
                        StatusInclusion = inclusionModel.StatusInclusion
                    };

                    try
                    {
                        #region Запрос на добавление счета в БД

                        //sqlCommand = new SqlCommand($"INSERT INTO Bills (ClientID, BillNum, InclusionDate, InclusionDateEnd, InclusionSum, Percents, Capitalize, Total, StatusInclusion)" +
                        //    $"VALUES ('{IncModel.Id}', '{inclusionModel.Bill}', '{inclusionModel.DateInclusion}', '{inclusionModel.DateEndInclusion}', " +
                        //    $"'{inclusionModel.Inclusion}', '{incModel.Percents}', '{inclusionModel.Capitalize}', '{Convert.ToInt32(inclusionModel.Sum)}', N'{inclusionModel.StatusInclusion}' )", ConnectionInfo.connection);

                        //sqlCommand.ExecuteNonQuery();

                        #endregion

                        billToSql.Bills.Add(newBill);

                        // Сохранение данных в бд
                        billToSql.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    IncModel.ColBills++;
                });
            }
        }

        /// <summary>
        /// Команда внесения суммы
        /// </summary>
        public ICommand AddMoneyCommand
        {
            get
            {
                return new RelayCommand<object>(obj =>
                {
                    ChangeMoneyMethod();
                });
            }
        }

        /// <summary>
        /// Команда открытия окна перевода средств
        /// </summary>
        public ICommand OpenTransfer
        {
            get
            {
                return new RelayCommand<object>(obj =>
                {
                    try
                    {
                        // Условие определяет произошел ли выбор клиента в таблице
                        if (SelectInclusion != null)
                        {
                            // Передача выбранного объекта клиента для использования в окне управления счетами 
                            TransferViewModel transferViewModel = new TransferViewModel(SelectInclusion);

                            // определение контекста данных для использования и редактирования переданного экзепляра объекта
                            TransferWindow transferWindow = new TransferWindow()
                            {
                                DataContext = transferViewModel
                            };

                            // Открытие окна управления счетами
                            transferWindow.Show();
                        }
                        else
                        {
                            throw new BankException(2, "Не выбран счет с которого будет осуществляться перевод.", @"Выберите счет");
                            //MessageBox.Show("Выберите счет с которого будет осуществляться перевод.");
                        }
                    }
                    catch (BankException e) when (e.Error == 2)
                    {
                        MessageBox.Show($"Код ошибки = {e.Error}. {e.Message}. {e.Recommendations}");
                    }
                });
            }
        }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Консруктор, принимающий переданный объект клиента
        /// </summary>
        /// <param name="incModel"></param>
        public InclusionViewModel(Client incModel)
        {
            // Присвоение объекта клиента свойтву
            IncModel = incModel;

            // Предстваление для отображаемой коллекции
            TableView = new ListCollectionView(IncModel.Inclusions);

            // Мобытие изменения данных
            InclusionModel.ChangeDataEvent += UpdateBills;
           
            DateEndInclusion = DateTime.Today.AddMonths(12);

            // Вызов метода определения статуса вклада
            InclusionStatus();
        }

        /// <summary>
        /// пустой конструктор
        /// </summary>
        public InclusionViewModel()
        {
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод определяет статус вклада
        /// </summary>
        public void InclusionStatus()
        {
            // Копия даты окончания вклада
            DateEndInclusionCopy = DateEndInclusion.ToShortDateString();

            // Цикл, проверяющий дату окончания каждого вклада
            foreach (var t in Inclusions)
            {
                var tempEndDate = Convert.ToDateTime(t.DateEndInclusion);

                // Условие устанавливает статус вклада в зависимости от даты окончания вклада (Если текущая дата больше чем дата окончания вклада, то устанавливается статус "Закончен")
                if (tempEndDate < DateTime.Today)
                {
                    t.StatusInclusion = "Закончен";
                }
                else
                {
                    t.StatusInclusion = "Активен";
                }
            }
        }

        /// <summary>
        /// Метод меняет сумму вклада в зависимости от действия совершенного со вкладом
        /// </summary>
        private void ChangeMoneyMethod()
        {
            try
            {
                if (SelectInclusion != null)
                {
                    if (RadioAdd == true)
                    {
                        // Прибавление вносимой суммы в выбранный вклад
                        SelectInclusion.Inclusion = SelectInclusion.Inclusion + AddMoney;

                        InclusionMoneyEvent?.Invoke($"Внесена сумма {AddMoney} на счет {SelectInclusion.Bill}");
                    }
                    if (RadioWithdraw == true && SelectInclusion.Inclusion >= AddMoney)
                    {
                        // Вычитание вносимой суммы в выбранный вклад
                        SelectInclusion.Inclusion = SelectInclusion.Inclusion - AddMoney;

                        InclusionMoneyEvent?.Invoke($"Сумма {AddMoney} снята со счета {SelectInclusion.Bill}");
                    }
                    else if (RadioWithdraw == true && SelectInclusion.Inclusion < AddMoney)
                    {
                        InclusionMoneyEvent?.Invoke($"На вашем счете недостаточно средств. Максимальная сума которую вы можете снять {SelectInclusion.Inclusion}");
                    }

                    InclusionModel inclusionModel = new InclusionModel();

                    // Расчет итоговой суммы внесении дополнительной суммы
                    SelectInclusion.Sum = inclusionModel.Calculate(SelectInclusion.DateInclusion, SelectInclusion.DateEndInclusion,
                        SelectInclusion.Inclusion, SelectInclusion.Percents, SelectInclusion.Capitalize);
                }
                else
                {
                    // Исключение вызывается если отсутствует фокус на один из счетов
                    throw new BankException(2, "Не выбран счет", @"Выберите счет пополнения\списания");
                }
            }
            catch (BankException e) when (e.Error == 2)
            {
                MessageBox.Show($"Код ошибки = {e.Error}. {e.Message}. {e.Recommendations}");
            }
        }

        /// <summary>
        /// Метод загрузки измененных данных в БД
        /// </summary>
        /// <param name="temp"></param>
        public static void UpdateBills(InclusionModel temp)
        {
            TestBankLocalDBEntities billToSql = new TestBankLocalDBEntities();

            // Получение определенного счета
            var newBill = billToSql.Bills.Where(x => x.BillNum == temp.Bill).FirstOrDefault();

            if (newBill != null)
            {
                // Тут пытался заполнить свойства рефлексией, но, ели я правильно понял, то работать это будет только для однотипных объектов.
                #region Попытка изменения свойств рефлексией
                //foreach (var t in temp.GetType().GetProperties())
                //{
                //    t.SetValue(newBill, t.GetValue(temp));
                //}
                #endregion

                // Изменение значений для отдельного счета в БД
                newBill.InclusionDateEnd = temp.DateEndInclusion;
                newBill.InclusionSum = temp.Inclusion;
                newBill.Percents = temp.Percents;
                newBill.Capitalize = temp.Capitalize;
                newBill.Total = Convert.ToInt32(temp.Sum);
                newBill.StatusInclusion = temp.StatusInclusion;
            }

            try
            {
                // Сохранение данных в БД
                billToSql.SaveChanges();
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }

            #region Запрос на изменение данных в БД

            //SqlCommand sqlCommand = new SqlCommand($"UPDATE Bills SET InclusionDateEnd='{temp.DateEndInclusion}', " +
            //    $"InclusionSum='{temp.Inclusion}', " +
            //    $"Percents='{temp.Percents}', " +
            //    $"Capitalize='{temp.Capitalize}', " +
            //    $"Total='{Convert.ToInt32(temp.Sum)}', " +
            //    $"StatusInclusion='{temp.StatusInclusion}' WHERE BillNum={temp.Bill}", ConnectionInfo.connection);

            //sqlCommand.ExecuteNonQuery();

            #endregion
        }

        #endregion
    }
}
