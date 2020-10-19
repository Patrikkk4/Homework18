using Bank.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Model
{
    /// <summary>
    /// Класс предоставляет свойства и методы для добавления нового клиента
    /// </summary>
    class Client : MainVM
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

        #endregion

        #region Свойства

        /// <summary>
        /// Свойство коллекция, содержащая всех добавленных клиентов
        /// </summary>
        ObservableCollection<Client> allClients { get; set; }

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
        public string ClientStatus { get; set; }

        /// <summary>
        /// Свойство даты регистрации клиента
        /// </summary>
        public string RegistrationDate { get; set; }

        /// <summary>
        /// Свойство процентной ставки
        /// </summary>
        public double Percents { get; set; }

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
        public Client(string LastName, string Name, string Patronymic, string ClientStatus)
        {
            RegistrationDate = DateTime.Now.ToString("dd.MM.yyyy");
            this.LastName = LastName;
            this.Name = Name;
            this.Patronymic = Patronymic;
            this.ClientStatus = ClientStatus;
            this.Percents = StatusPercent();
        }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public Client()
        {
        }

        #endregion

        #region Методы

        /// <summary>
        /// Метод указывает процентную ставку в зависимости от статуса клиента
        /// </summary>
        /// <returns></returns>
        public double StatusPercent()
        {
            // Условия установки процентной ставки
            if (ClientStatus == "Обычный")
            {
                Percents = 12;
            }
            else if (ClientStatus == "VIP")
            {
                Percents = 15;
            }
            // Условие установки процентной ставки для корпоративных клиентов
            else
            {
                Percents = 22;
            }

            return Percents;
        }

        #endregion
    }
}
