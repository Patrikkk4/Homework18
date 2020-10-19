using Bank.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bank.Model
{
    /// <summary>
    /// Класс предоставляет свойства и методы для добавления нового вклада клиента
    /// </summary>
    class InclusionModel : MainVM
    {
        #region Поля

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

        #endregion

        #region Свойства

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
            set {  dateEndInclusion = value; OnPropertyChange(nameof(DateEndInclusion)); CheckDate(); }
        }

        /// <summary>
        /// Свойство копия даты, введенной при регистрации вклада
        /// </summary>
        public string DateEndInclusionCopy { get; set; } = InclusionViewModel.DateEndInclusionCopy.ToShortDateString();

        /// <summary>
        /// Поле счета вклада
        /// </summary>
        public int Bill { get; set; }

        /// <summary>
        /// Поле статуса клиента
        /// </summary>
        public string ClientStatus { get; set; }

        /// <summary>
        /// Свойство суммы вклада
        /// </summary>
        public double Inclusion 
        {
            get { return inclusion; }
            set { inclusion = value; OnPropertyChange(nameof(Inclusion)); }
        }

        /// <summary>
        /// Свойство процентной ставки
        /// </summary>
        public double Percents { get; set; }

        /// <summary>
        /// Свойство итоговой суммы по окончанию вклада
        /// </summary>
        public double Sum
        {
            get { return sum; }
            set { sum = value; OnPropertyChange(nameof(Sum)); }
        }

        /// <summary>
        /// Свойство указывающее на капитализацию вклада
        /// </summary>
        public bool Capitalize { get; set; }

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
        public InclusionModel(string ClientStatus, double Inclusion, double Percents, bool Capitalize, string DateEndInclusion, string StatusInclusion)
        {
            this.DateInclusion = DateTime.Today.ToString("dd.MM.yyyy");
            this.DateEndInclusion = DateEndInclusion;
            this.Bill = NewBill();
            this.ClientStatus = ClientStatus;
            this.Inclusion = Inclusion;
            this.Percents = Percents;
            this.Capitalize = Capitalize;
            this.Sum = Calculate(DateInclusion, DateEndInclusion, Inclusion, Percents);
            this.statusInclusion = StatusInclusion;
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
        public double Calculate(string dateInclusion, string dateEndInclusion, double inclusion, double percents)
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
            if (Capitalize != true)
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

        /// <summary>
        /// Метод проверяет корректность введенной даты
        /// </summary>
        public void CheckDate()
        {
            // Проверка на корректность ввденных данных
            try
            {
                Convert.ToDateTime(DateEndInclusion);

                //InclusionViewModel.res = false;
            }
            catch
            {
                MessageBox.Show("Данное значение не является датой");

                //InclusionViewModel.res = true;

                // При неудачной проверке присваивается значение по умолчанию даты окончания вклада
                DateEndInclusion = DateEndInclusionCopy;
            }
        }

        #endregion
    }
}
