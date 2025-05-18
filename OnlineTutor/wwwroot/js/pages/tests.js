/**
 * Скрипты для страниц тестов
 */

const TestsPage = {
    init: function () {
        // Инициализация обработчиков для страниц тестов
        this.setupQuestionType();
        this.setupOptionManagement();
        this.setupDeleteConfirmation();
        this.setupTestAssignment();
    },

    setupQuestionType: function () {
        // Показывать/скрывать варианты ответов в зависимости от типа вопроса
        $('#questionType').change(function () {
            var selectedType = $(this).val();
            if (selectedType === '0' || selectedType === '1') { // SingleChoice или MultipleChoice
                $('#optionsContainer').show();
            } else {
                $('#optionsContainer').hide();
            }
        });

        // Инициализация видимости
        if ($('#questionType').length) {
            $('#questionType').trigger('change');
        }
    },

    setupOptionManagement: function () {
        // Добавление нового варианта ответа
        var optionIndex = $('.option-row').length;

        $('#addOption').click(function () {
            $.ajax({
                url: '/Test/AddOptionRow',
                type: 'POST',
                data: { index: optionIndex },
                success: function (data) {
                    // Заменяем индекс в ответе
                    data = data.replace(/$$0$$/g, '[' + optionIndex + ']');
                    data = data.replace(/option0/g, 'option' + optionIndex);

                    $('#optionsList').append(data);
                    optionIndex++;
                }
            });
        });

        // Удаление варианта ответа
        $(document).on('click', '.remove-option', function () {
            $(this).closest('.option-row').remove();

            // Перенумеруем индексы
            $('.option-row').each(function (index) {
                $(this).find('input[name$=".Text"]').attr('name', 'Options[' + index + '].Text');
                $(this).find('input[name$=".IsCorrect"]').attr('name', 'Options[' + index + '].IsCorrect');
                $(this).find('input[name$=".Id"]').attr('name', 'Options[' + index + '].Id');
                $(this).find('input[type="checkbox"]').attr('id', 'option' + index);
                $(this).find('label').attr('for', 'option' + index);
            });

            optionIndex = $('.option-row').length;
        });
    },

    setupDeleteConfirmation: function () {
        // Скрипт для модального окна подтверждения удаления
        $('#deleteQuestionModal').on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget);
            var questionId = button.data('question-id');
            var questionText = button.data('question-text');

            $('#questionText').text(questionText);
            $('#questionId').val(questionId);
        });
    },

    setupTestAssignment: function () {
        // При выборе класса сбрасываем выбор ученика и наоборот
        $('#classSelect').change(function () {
            if ($(this).val()) {
                $('#studentSelect').val('');
            }
        });

        $('#studentSelect').change(function () {
            if ($(this).val()) {
                $('#classSelect').val('');
            }
        });
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    TestsPage.init();
});
