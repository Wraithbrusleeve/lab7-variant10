// Лабораторная работа № 7. Техническое документирование программного проекта
// Вариант 10: система заказов аптеки «Аптека-Плюс»
// Правило: нет заказов в прошлом месяце - скидка 10 %
// Выполнил: Ульчугачев Глеб Сергеевич, группа И-43
namespace Lab7_Variant10
{
    /// <summary>
    /// Сведения об исполнителе: фамилия, имя, отчество и группа.
    /// </summary>
    public static class Author
    {
        /// <summary>Фамилия, имя и отчество студента.</summary>
        public const string FullName = "Ульчугачев Глеб Сергеевич";

        /// <summary>Учебная группа студента.</summary>
        public const string Group = "И-43";
    }

    /// <summary>
    /// Лекарственный препарат или товар из ассортимента аптеки.
    /// </summary>
    public class Medicine
    {
        /// <summary>
        /// Создаёт позицию ассортимента.
        /// </summary>
        /// <param name="name">Название и форма выпуска.</param>
        /// <param name="price">Розничная цена за упаковку в рублях.</param>
        public Medicine(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        /// <summary>Название и форма выпуска.</summary>
        public string Name { get; }

        /// <summary>Розничная цена за упаковку в рублях.</summary>
        public decimal Price { get; }
    }

    /// <summary>
    /// Покупатель аптеки с дисконтной картой.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Создаёт покупателя.
        /// </summary>
        /// <param name="cardNumber">Номер дисконтной карты.</param>
        /// <param name="ordersLastMonth">Количество заказов за прошлый месяц.</param>
        public Customer(string cardNumber, int ordersLastMonth)
        {
            CardNumber = cardNumber;
            OrdersLastMonth = ordersLastMonth;
        }

        /// <summary>Номер дисконтной карты.</summary>
        public string CardNumber { get; }

        /// <summary>Количество заказов за прошлый месяц.</summary>
        public int OrdersLastMonth { get; }

        /// <summary>Препараты в корзине и количество упаковок.</summary>
        public Dictionary<Medicine, int> Cart { get; } = new();
    }

    /// <summary>
    /// Расчёт стоимости заказа в «Аптека-Плюс» по правилу варианта 10.
    /// </summary>
    public static class OrderCalculator
    {
        /// <summary>Скидка в процентах для покупателей без заказов в прошлом месяце.</summary>
        public const decimal DiscountPercent = 10m;

        /// <summary>
        /// Проверяет, положена ли покупателю скидка.
        /// </summary>
        /// <param name="customer">Покупатель аптеки.</param>
        /// <returns>true, если в прошлом месяце у покупателя не было заказов.</returns>
        public static bool IsDiscountAllowed(Customer customer)
        {
            return customer.OrdersLastMonth == 0;
        }

        /// <summary>
        /// Считает стоимость корзины без скидки.
        /// </summary>
        /// <param name="customer">Покупатель с заполненной корзиной.</param>
        /// <returns>Сумма по розничным ценам в рублях.</returns>
        public static decimal GetSubtotal(Customer customer)
        {
            decimal subtotal = 0m;
            foreach (var (medicine, count) in customer.Cart)
                subtotal += medicine.Price * count;
            return subtotal;
        }

        /// <summary>
        /// Считает итоговую сумму заказа с учётом скидки 10 %.
        /// </summary>
        /// <param name="customer">Покупатель с заполненной корзиной.</param>
        /// <returns>Сумма к оплате в рублях.</returns>
        public static decimal GetTotal(Customer customer)
        {
            decimal subtotal = GetSubtotal(customer);
            if (!IsDiscountAllowed(customer))
                return subtotal;
            return subtotal * (100m - DiscountPercent) / 100m;
        }
    }

    /// <summary>
    /// Точка входа: печатает чеки покупателей аптеки.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Формирует заказы, выводит суммы со скидкой и сведения об авторе.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var paracetamol = new Medicine("Парацетамол 500 мг, таблетки № 20", 65m);
            var ibuprofen = new Medicine("Ибупрофен 200 мг, таблетки № 50", 189m);
            var vitaminD = new Medicine("Витамин D3, капли 10 мл", 420m);
            var bandage = new Medicine("Бинт стерильный 7 м x 14 см", 48m);
            var thermometer = new Medicine("Термометр электронный", 350m);

            var first = new Customer("АП-0451", 0);
            first.Cart.Add(paracetamol, 2);
            first.Cart.Add(vitaminD, 1);
            first.Cart.Add(bandage, 3);

            var second = new Customer("АП-0788", 4);
            second.Cart.Add(ibuprofen, 1);
            second.Cart.Add(thermometer, 1);

            var third = new Customer("АП-1203", 0);
            third.Cart.Add(ibuprofen, 2);
            third.Cart.Add(vitaminD, 2);

            Console.WriteLine("Аптека «Аптека-Плюс» - расчёт заказов");
            Console.WriteLine($"Нет заказов в прошлом месяце - скидка {OrderCalculator.DiscountPercent} %");

            decimal revenue = 0m;
            foreach (var customer in new[] { first, second, third })
            {
                Console.WriteLine();
                Console.WriteLine($"Карта {customer.CardNumber}, заказов в прошлом месяце: {customer.OrdersLastMonth}");
                foreach (var (medicine, count) in customer.Cart)
                    Console.WriteLine($"  {medicine.Name} - {count} уп. x {medicine.Price:N2} руб.");

                decimal total = OrderCalculator.GetTotal(customer);
                revenue += total;
                string note = OrderCalculator.IsDiscountAllowed(customer) ? $"скидка {OrderCalculator.DiscountPercent} % применена" : "без скидки";
                Console.WriteLine($"  Сумма: {OrderCalculator.GetSubtotal(customer):N2} руб., к оплате: {total:N2} руб. ({note})");
            }

            Console.WriteLine();
            Console.WriteLine($"Всего к оплате по трём заказам: {revenue:N2} руб.");
            Console.WriteLine($"Выполнил: {Author.FullName}, группа {Author.Group}");

            if (!Console.IsInputRedirected)
                Console.ReadKey();
        }
    }
}
