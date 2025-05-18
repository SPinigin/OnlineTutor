/**
 * Обработка форм
 */

const Forms = {
    init: function () {
        // Инициализация обработчиков форм
        this.setupFormHandlers();

        // Инициализация генераторов паролей
        this.setupPasswordGenerators();

        // Инициализация переключателей отображения пароля
        this.setupPasswordToggles();
    },

    setupFormHandlers: function () {
        // Обработка отправки форм с подтверждением
        $('form[data-confirm]').on('submit', function (e) {
            const confirmMessage = $(this).data('confirm');
            if (!confirm(confirmMessage)) {
                e.preventDefault();
                return false;
            }
        });

        // Обработка AJAX-форм
        $('form[data-ajax="true"]').on('submit', function (e) {
            e.preventDefault();

            const form = $(this);
            const url = form.attr('action');
            const method = form.attr('method') || 'POST';
            const data = form.serialize();

            $.ajax({
                url: url,
                method: method,
                data: data,
                success: function (response) {
                    if (form.data('ajax-success')) {
                        window[form.data('ajax-success')](response);
                    } else {
                        App.showNotification('Операция выполнена успешно', 'success');
                    }
                },
                error: function (xhr) {
                    if (form.data('ajax-error')) {
                        window[form.data('ajax-error')](xhr);
                    } else {
                        App.showNotification('Произошла ошибка при выполнении операции', 'danger');
                    }
                }
            });
        });
    },

    setupPasswordGenerators: function () {
        // Функция для генерации случайного пароля
        function generatePassword(length = 8) {
            const lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const numbers = "0123456789";
            const specialChars = "!$%^&*";

            const allChars = lowerChars + upperChars + numbers + specialChars;
            let password = "";

            // Гарантируем наличие заглавной буквы, строчной буквы, цифры и специального символа
            password += upperChars.charAt(Math.floor(Math.random() * upperChars.length));
            password += lowerChars.charAt(Math.floor(Math.random() * lowerChars.length));
            password += numbers.charAt(Math.floor(Math.random() * numbers.length));
            password += specialChars.charAt(Math.floor(Math.random() * specialChars.length));

            // Дополняем до нужной длины
            for (let i = 4; i < length; i++) {
                password += allChars.charAt(Math.floor(Math.random() * allChars.length));
            }

            // Перемешиваем символы
            password = password.split('').sort(function () {
                return 0.5 - Math.random();
            }).join('');

            return password;
        }

        // Обработчик кнопки генерации пароля
        $(".btn-generate-password").on("click", function () {
            const targetInput = $(this).data('target') || $(this).prev('input[type="password"]');
            const confirmInput = $(this).data('confirm');

            const password = generatePassword(8);
            $(targetInput).val(password);

            if (confirmInput) {
                $(confirmInput).val(password);
            }

            // Если пароль сгенерирован и поле видимо, показываем пароль
            if ($(targetInput).attr("type") === "text") {
                $(targetInput).focus();
            }
        });
    },

    setupPasswordToggles: function () {
        // Обработчик чекбокса "Показать пароль"
        $(".show-password-toggle").on("change", function () {
            const targetInput = $(this).data('target') || $(this).closest('.form-group').find('input[type="password"]');
            const confirmInput = $(this).data('confirm');

            if (this.checked) {
                $(targetInput).attr("type", "text");
                if (confirmInput) {
                    $(confirmInput).attr("type", "text");
                }
            } else {
                $(targetInput).attr("type", "password");
                if (confirmInput) {
                    $(confirmInput).attr("type", "password");
                }
            }
        });
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    Forms.init();
});
