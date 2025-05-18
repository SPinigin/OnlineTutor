/**
 * Базовая валидация форм
 */

const Validation = {
    init: function () {
        // Инициализация валидации для всех форм
        this.setupFormValidation();
    },

    setupFormValidation: function () {
        // Проверка, загружена ли библиотека jQuery Validation
        if ($.validator) {
            // Настройка дополнительных методов валидации
            this.setupCustomValidationMethods();

            // Настройка стандартных сообщений об ошибках
            this.setupValidationMessages();
        }
    },

    setupCustomValidationMethods: function () {
        // Метод для проверки российского телефона
        $.validator.addMethod('phoneRU', function (value, element) {
            return this.optional(element) || /^(\+7|7|8)?[\s\-]?$$?[489][0-9]{2}$$?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$/.test(value);
        }, 'Пожалуйста, введите корректный номер телефона');

        // Метод для проверки имени (только буквы)
        $.validator.addMethod('nameOnly', function (value, element) {
            return this.optional(element) || /^[а-яА-ЯёЁa-zA-Z\s-]+$/.test(value);
        }, 'Пожалуйста, используйте только буквы');
    },

    setupValidationMessages: function () {
        // Локализация сообщений об ошибках
        $.extend($.validator.messages, {
            required: "Это поле обязательно для заполнения",
            email: "Пожалуйста, введите корректный email",
            url: "Пожалуйста, введите корректный URL",
            date: "Пожалуйста, введите корректную дату",
            number: "Пожалуйста, введите число",
            digits: "Пожалуйста, введите только цифры",
            equalTo: "Пожалуйста, введите то же значение еще раз",
            maxlength: $.validator.format("Пожалуйста, введите не более {0} символов"),
            minlength: $.validator.format("Пожалуйста, введите не менее {0} символов"),
            range: $.validator.format("Пожалуйста, введите значение от {0} до {1}"),
            max: $.validator.format("Пожалуйста, введите значение, меньшее или равное {0}"),
            min: $.validator.format("Пожалуйста, введите значение, большее или равное {0}")
        });
    },

    // Метод для валидации формы
    validateForm: function (formSelector, rules, messages) {
        $(formSelector).validate({
            rules: rules,
            messages: messages,
            errorElement: 'span',
            errorClass: 'field-validation-error',
            highlight: function (element, errorClass) {
                $(element).addClass('input-validation-error');
            },
            unhighlight: function (element, errorClass) {
                $(element).removeClass('input-validation-error');
            }
        });
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    Validation.init();
});
