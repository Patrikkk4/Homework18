using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static BankLibrary.BankClassLibrary;

namespace BankLibrary
{
    /// <summary>
    /// Класс методов расширения
    /// </summary>
    public static class ExtentionMethods
    {
        /// <summary>
        /// Метод проверяет корректность введенной даты
        /// </summary>
        public static bool CheckDate(this string dateOne)
        {
            // Проверка на корректность ввденных данных
            try
            {
                Convert.ToDateTime(dateOne);

                return true;
            }
            catch
            {
                MessageBox.Show("Данное значение не является датой");

                return false;
            }
        }

        /// <summary>
        /// Метод получает все счета
        /// </summary>
        /// <returns></returns>
        public static List<int> TakeBills(this List<int> Bills, ObservableCollection<Client> AllClients, string SelectedName)
        {
            // Временная коллекция счетов
            List<int> tempInclusions = new List<int>();

            // Выборка счетов выбранного клиента 
            var temp = AllClients.Where(x => x.LastName == SelectedName).ToList();

            foreach (var t in temp)
            {
                foreach (var x in t.Inclusions)
                {
                    tempInclusions.Add(x.Bill);
                }
            }

            return tempInclusions;
        }
    }

    /// <summary>
    /// Библиотека моделей. Содержит классы Клиентов, счетов, выполнения команд, исключений и отслеживания изменений свойств
    /// </summary>
    public class BankClassLibrary
    {
        /// <summary>
        /// Клас предоставляет событие изменения свойства
        /// </summary>
        public class MainVM : INotifyPropertyChanged
        {
            /// <summary>
            /// Событие изменения свойства
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Обработчик события изменения свойства
            /// </summary>
            /// <param name="prop"></param>
            public void OnPropertyChange([CallerMemberName]string prop = "")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        /// <summary>
        /// Класс предоставляет свойства и методы для добавления нового клиента
        /// </summary>
        public class Client : MainVM
        {
            #region Поля

            /// <summary>
            /// Поле счета клиента
            /// </summary>
            private int colBills;
            /// <summary>
            /// Поле коллекции вкладов клиента
            /// </summary>
            private ObservableCollection<InclusionModel> inclusions = new ObservableCollection<InclusionModel>();

            private string clientStatus;

            private double percents;

            #endregion

            #region Свойства

            /// <summary>
            /// Свойство коллекция, содержащая всех добавленных клиентов
            /// </summary>
            ObservableCollection<Client> allClients { get; set; }

            public int Id { get; set; }

            /// <summary>
            /// Свойство счета клиента
            /// </summary>
            public int ColBills
            {
                get { return colBills; }
                set { colBills = value; OnPropertyChange(nameof(ColBills)); }
            }

            /// <summary>
            /// Свойство фамилии клиента
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// свойство имени клиента
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Свойство отчества клиента
            /// </summary>
            public string Patronymic { get; set; }

            /// <summary>
            /// Свойство статуса клиента
            /// </summary>
            public string ClientStatus 
            {
                get { return clientStatus; }
                set 
                { 
                    clientStatus = value;
                    OnPropertyChange(nameof(ClientStatus));
                    switch (value)
                    {
                        case "Обычный":
                            Percents = 12;
                            break;
                        case "VIP":
                            Percents = 15;
                            break;
                        case "Корпоративный":
                            Percents = 22;
                            break;
                    }
                }
            }

            /// <summary>
            /// Свойство даты регистрации клиента
            /// </summary>
            public string RegistrationDate { get; set; }

            /// <summary>
            /// Свойство процентной ставки
            /// </summary>
            public double Percents
            {
                get { return percents; }
                set { percents = value; OnPropertyChange(nameof(Percents)); }
            }

            /// <summary>
            /// Свойство коллекция, содержащая все счета клиента
            /// </summary>
            public ObservableCollection<InclusionModel> Inclusions
            {
                get { return inclusions; }
                set { inclusions = value; OnPropertyChange(nameof(Inclusions)); }
            }

            #endregion

            #region Конструкторы

            /// <summary>
            /// Конструктор добавления нового клиента
            /// </summary>
            /// <param name="LastName"></param>
            /// <param name="Name"></param>
            /// <param name="Patronymic"></param>
            /// <param name="ClientStatus"></param>
            public Client(int id, string LastName, string Name, string Patronymic, string ClientStatus)
            {
                RegistrationDate = DateTime.Now.ToString("dd.MM.yyyy");
                this.Id = id;
                this.LastName = LastName;
                this.Name = Name;
                this.Patronymic = Patronymic;
                this.ClientStatus = ClientStatus;
            }

            /// <summary>
            /// Конструктор выгрузки данных из БД
            /// </summary>
            /// <param name="id"></param>
            /// <param name="LastName"></param>
            /// <param name="Name"></param>
            /// <param name="Patronymic"></param>
            /// <param name="ClientStatus"></param>
            /// <param name="regDate"></param>
            public Client(int id, string LastName, string Name, string Patronymic, string ClientStatus, string regDate)
            {
                RegistrationDate = regDate;
                this.Id = id;
                this.LastName = LastName;
                this.Name = Name;
                this.Patronymic = Patronymic;
                this.ClientStatus = ClientStatus;
            }

            /// <summary>
            /// Пустой конструктор
            /// </summary>
            public Client()
            {
            }

            #endregion
        }

        /// <summary>
        /// Класс предоставляет свойства и методы для добавления нового вклада клиента
        /// </summary>
        public class InclusionModel : MainVM
        {
            #region Поля

            public delegate void SetDate(InclusionModel temp);

            /// <summary>
            /// Событие изменения данных в счете
            /// </summary>
            public static event SetDate ChangeDataEvent;
            /// <summary>
            /// Поле даты окончания вклада
            /// </summary>
            private string dateEndInclusion;
            /// <summary>
            /// Поле статуса вклада
            /// </summary>
            private string statusInclusion;
            /// <summary>
            /// поле суммы вклада
            /// </summary>
            private double inclusion;
            /// <summary>
            /// поле итоговой суммы по окончанию вклада
            /// </summary>
            private double sum;
            /// <summary>
            /// Поле статуса клиента
            /// </summary>
            private string clientStatus;
            /// <summary>
            /// Поле процентной ставки
            /// </summary>
            private double percents;
            /// <summary>
            /// Поле капитализации
            /// </summary>
            private bool capitalize;

            #endregion

            #region Свойства

            /// <summary>
            /// Свойство ID клиента
            /// </summary>
            public int ClientID { get; set; }

            /// <summary>
            /// Свойство статуса вклада
            /// </summary>
            public string StatusInclusion
            {
                get { return statusInclusion; }
                set { statusInclusion = value; OnPropertyChange(nameof(StatusInclusion)); }
            }

            /// <summary>
            /// Поле даты поступления вклада
            /// </summary>
            public string DateInclusion { get; set; }

            /// <summary>
            /// Поле даты окончания вклада
            /// </summary>
            public string DateEndInclusion
            {
                get { return dateEndInclusion; }
                set 
                {
                    dateEndInclusion = value; OnPropertyChange(nameof(DateEndInclusion)); 

                    // Проверка корректности введенной даты при изменении в счете клиента
                    if(!DateEndInclusion.CheckDate())
                    {
                        DateEndInclusion = DateEndInclusionCopy.ToShortDateString();
                    }
                }
            }

            /// <summary>
            /// Свойство копия даты окончания вклада
            /// </summary>
            public static DateTime DateEndInclusionCopy { get; set; }

            /// <summary>
            /// Поле счета вклада
            /// </summary>
            public int Bill { get; set; }

            /// <summary>
            /// Поле статуса клиента
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
                    inclusion = value; 
                    OnPropertyChange(nameof(Inclusion)); 
                    Sum = Calculate(DateInclusion, DateEndInclusion, Inclusion, Percents, Capitalize);
                }
            }

            /// <summary>
            /// Свойство процентной ставки
            /// </summary>
            public double Percents 
            {
                get { return percents; }
                set { percents = value; OnPropertyChange(nameof(Percents)); }
            }

            /// <summary>
            /// Свойство итоговой суммы по окончанию вклада
            /// </summary>
            public double Sum
            {
                get { return sum; }
                set 
                { 
                    sum = value; 
                    OnPropertyChange(nameof(Sum));

                    ChangeDataEvent?.Invoke(this);
                }
            }

            /// <summary>
            /// Свойство указывающее на капитализацию вклада
            /// </summary>
            public bool Capitalize
            {
                get { return capitalize; }
                set 
                { 
                    capitalize = value;
                    OnPropertyChange(nameof(Capitalize));

                    Sum = Calculate(DateInclusion, DateEndInclusion, Inclusion, Percents, Capitalize);

                    ChangeDataEvent?.Invoke(this);
                }
            }

            #endregion

            #region Конструкторы

            /// <summary>
            /// Конструктор внесения вклада клиента
            /// </summary>
            /// <param name="ClientStatus"></param>
            /// <param name="Inclusion"></param>
            /// <param name="Percents"></param>
            /// <param name="Capitalize"></param>
            /// <param name="DateEndInclusion"></param>
            /// <param name="StatusInclusion"></param>
            public InclusionModel
                (string ClientStatus, 
                double Inclusion, 
                double Percents, 
                bool Capitalize, 
                string DateEndInclusion, 
                string StatusInclusion)
            {
                this.DateInclusion = DateTime.Today.ToString("dd.MM.yyyy");
                this.DateEndInclusion = DateEndInclusion;
                this.Bill = NewBill();
                this.ClientStatus = ClientStatus;
                this.Inclusion = Inclusion;
                this.Percents = Percents;
                this.Capitalize = Capitalize;
                this.Sum = Calculate(DateInclusion, DateEndInclusion, Inclusion, Percents, Capitalize);
                this.statusInclusion = StatusInclusion;
                DateEndInclusionCopy = Convert.ToDateTime(DateEndInclusion);
            }

            /// <summary>
            /// Конструктор для выгрузки данных из БД
            /// </summary>
            /// <param name="clientID"></param>
            /// <param name="billNum"></param>
            /// <param name="inclusionDate"></param>
            /// <param name="inclusionDateEnd"></param>
            /// <param name="inclusionSum"></param>
            /// <param name="percents"></param>
            /// <param name="capitalize"></param>
            /// <param name="total"></param>
            /// <param name="statusInclusion"></param>
            public InclusionModel
                (int clientID,
                int billNum,
                string inclusionDate,
                string inclusionDateEnd,
                double inclusionSum,
                double percents,
                bool capitalize,
                double total,
                string statusInclusion)
            {
                this.ClientID = clientID;
                this.DateInclusion = inclusionDate;
                this.DateEndInclusion = inclusionDateEnd;
                this.Bill = billNum;
                this.ClientStatus = ClientStatus;
                this.Inclusion = inclusionSum;
                this.Percents = percents;
                this.Capitalize = capitalize;
                this.Sum = total;
                this.statusInclusion = statusInclusion;
            }

            /// <summary>
            /// Пустой конструктор
            /// </summary>
            public InclusionModel()
            {
            }

            #endregion

            #region Методы

            /// <summary>
            /// Метод задает номер вклада 
            /// </summary>
            /// <returns></returns>
            int NewBill()
            {
                // Инициализация класса случайной генерации чисел
                Random random = new Random();

                // Генерация случайного числа
                return random.Next(10000, 1000001);
            }

            /// <summary>
            /// Метод расчета итоговой суммы по окончании вклада
            /// </summary>
            /// <returns></returns>
            public double Calculate(string dateInclusion, string dateEndInclusion, double inclusion, double percents, bool capitalize)
            {
                // Конвертирование строкового значения даты внесения вклада
                DateTime tempDateInclusion = Convert.ToDateTime(dateInclusion);

                // Конвертирование строкового значения даты окончаня вклада
                DateTime tempDateEndInclusion = Convert.ToDateTime(dateEndInclusion);

                // Расчет разницы дат
                var span = tempDateEndInclusion - tempDateInclusion;

                // Вычисление и запись числового значения разницы дат
                var inclusionTime = Math.Ceiling(span.TotalDays / 30.4);

                // Возвращаемый параметр
                double inc;

                // Условие, при котором производятся расчеты при капитализации вклада и без.
                if (capitalize != true)
                {
                    // растчет итоговой суммы вклада по окончании без капитализации
                    inc = inclusion + inclusion * (percents / 100 / 12 * (inclusionTime - 1));
                }
                else
                {
                    // Вычисление процентной ставки на каждый месяц на протяжении времени вклада
                    double pers = percents / 12 / 100;

                    Sum = inclusion;

                    // Цикл расчета итоговой суммы при учете капитализации
                    for (var i = 0; i < inclusionTime; i++)
                    {
                        // расчет прибыли на каждый месяц
                        double endSum = Sum * pers;

                        // Расчет итоговой суммы на каждый месяц
                        Sum = Sum + endSum;
                    }

                    inc = Sum;
                }

                return inc;
            }

            #endregion
        }

        /// <summary>
        /// Класс принимает не типизрованный входной параметр и наследует интерфейс определения команд
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class RelayCommand<T> : ICommand
        {
            /// <summary>
            /// Поле выполнение логики команды
            /// </summary>
            private readonly Action<T> execute;

            /// <summary>
            /// Поле Определение, может ли команда быть выполнена
            /// </summary>
            private readonly Func<T, bool> canExecute;

            // Событие, вызываемое при изменении условий
            public event EventHandler CanExecuteChanged
            {
                // Определение, может ли условие повлиять на выполнение команды
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Конструктор принимает папараметры команды и определяет может ли она быть выполнена
            /// </summary>
            /// <param name="execute"></param>
            /// <param name="canExecute"></param>
            public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            /// <summary>
            /// Метод определяет возможно ли выполнение команды
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public bool CanExecute(object parameter)
            {
                return this.canExecute == null || this.CanExecute(parameter);
            }

            /// <summary>
            /// Метод выполнения команды
            /// </summary>
            /// <param name="parameter"></param>
            public void Execute(object parameter)
            {
                execute((T)parameter);
            }
        }

        /// <summary>
        /// Клас исключений
        /// </summary>
        public class BankException : Exception
        {
            public int Error { get; set; }
            public string Recommendations { get; set; }

            public BankException(int Error, string mes, string Recommendations) : base(mes)
            {
                this.Error = Error;
                this.Recommendations = Recommendations;
            }
        }
        /// <summary>
        /// Класс содержит информацию по подключению к БД
        /// </summary>
        public class ConnectionInfo
        {
            // Создание строки подключения
            static SqlConnectionStringBuilder connString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = @"TestBankLocalDB",
                IntegratedSecurity = true,
                Pooling = true
            };

            // Подключение к БД
            public static SqlConnection connection = new SqlConnection(connString.ConnectionString);
        }
    }
}
