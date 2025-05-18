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
            const charset = {
                lowercase: 'abcdefghijklmnopqrstuvwxyz',
                uppercase: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ',
                numbers: '0123456789',
                special: '!@#$%^&*'
            };

            let password = '';

            // Гарантируем наличие всех типов символов
            password += charset.uppercase[Math.floor(Math.random() * charset.uppercase.length)];
            password += charset.lowercase[Math.floor(Math.random() * charset.lowercase.length)];
            password += charset.numbers[Math.floor(Math.random() * charset.numbers.length)];
            password += charset.special[Math.floor(Math.random() * charset.special.length)];

            // Добавляем остальные символы
            const allChars = Object.values(charset).join('');
            for (let i = password.length; i < length; i++) {
                password += allChars[Math.floor(Math.random() * allChars.length)];
            }

            // Перемешиваем символы
            return password.split('').sort(() => Math.random() - 0.5).join('');
        }

        $(document).ready(function () {
            $('#generatePassword').on('click', function () {
                const password = generatePassword(12);
                $('#password').val(password);
                $('#confirmPassword').val(password);

                if ($('#showPassword').is(':checked')) {
                    $('#password, #confirmPassword').attr('type', 'text');
                }
            });

            $('#showPassword').on('change', function () {
                const type = $(this).is(':checked') ? 'text' : 'password';
                $('#password, #confirmPassword').attr('type', type);
            });
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
