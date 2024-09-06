namespace WORLDGAMDEVELOPMENT
{
    internal static class DialogData
    {
        public const string YOUR_MESSAGE_HAS_BEEN_RECEIVED = "Ваше сообщение получено. Ожидайте ответа!";
        /// <summary>
        /// {0} - Username
        /// {1} - UserId
        /// </summary>
        public const string WELCOME_MESSAGE_TEMPLATE = "Приветствую {0}\nВы не авторизованы в системе.\nСообщите Менеджеру ваш id: {1}";

        /// <summary>
        /// {0} - FirstName
        /// {1} - SecondName
        /// {2} - LastName
        /// {3} - Phone from Manager
        /// </summary>
        public const string WELCOME_MESSAGE_DEFAULT = "Добро пожаловать, {0} {1} {2}\n\nВсе доступные действия совершаются путем нажатия на кнопки снизу.\n\nВ случае проблем, обратитесь к Менеджеру по телефону {3}";

        /// <summary>
        /// {0} - UserId
        /// </summary>
        public const string USER_HAS_ID = "Пользователь с Id: {0}";
        public const string USER_HAS_NOT_REGISTER = "не зарегистрирован в системе.";
        public const string USER_HAS_REGISTER = "зарегистрирован в системе.";

        public const string HELP_MSG_DEFAULT = "Скоро появится инфа оп помощи. Пока ничего не придумал.";
        public const string CONTACTS_MSG_DEFAULT = "Вы можете спросить интересующий Вас вопрос по телефону 8 (999) 999-99-99. Или пойти на хуй 😂🐱‍👤";


    }
}
