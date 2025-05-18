/**
 * Скрипты для страниц аккаунта (логин, регистрация, профиль)
 */

const AccountPage = {
    init: function () {
        // Инициализация обработчиков для страницы регистрации
        this.setupRegisterPage();

        // Инициализация обработчиков для страницы профиля
        this.setupProfilePage();
    },

    setupRegisterPage: function () {
        // Обработчик изменения роли при регистрации
        $('#roleSelect').change(function () {
            var selectedRole = $(this).val();
            if (selectedRole === 'Student') {
                $('#studentFields').show();
                $('#teacherFields').hide();
            } else if (selectedRole === 'Teacher') {
                $('#studentFields').hide();
                $('#teacherFields').show();
            } else {
                $('#studentFields').hide();
                $('#teacherFields').hide();
            }
        });

        // Запуск начальной проверки роли
        if ($('#roleSelect').length) {
            $('#roleSelect').trigger('change');
        }

        // Генерация пароля
        $("#generatePassword").click(function () {
            var password = Forms.generatePassword(8);
            $("#password").val(password);
            $("#confirmPassword").val(password);

            // Если пароль сгенерирован и чекбокс "Показать пароль" отмечен, показываем пароль
            if ($("#showPassword").is(":checked")) {
                $("#password").attr("type", "text");
                $("#confirmPassword").attr("type", "text");
            }
        });

        // Обработчик чекбокса "Показать пароль"
        $("#showPassword").change(function () {
            if (this.checked) {
                $("#password").attr("type", "text");
                $("#confirmPassword").attr("type", "text");
            } else {
                $("#password").attr("type", "password");
                $("#confirmPassword").attr("type", "password");
            }
        });
    },

    setupProfilePage: function () {
        // Обработчики для страницы профиля могут быть добавлены здесь
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    AccountPage.init();
});
