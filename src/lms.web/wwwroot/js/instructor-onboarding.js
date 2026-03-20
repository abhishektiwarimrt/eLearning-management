$(function () {
    var currentStep = parseInt($('#currentStepInput').val(), 10);
    var totalSteps = parseInt($('#totalSteps').val(), 10);
    var steps = JSON.parse($('#stepsJson').val());

    var stepSelectors = {
        1: '#experience',
        2: '#topics',
        3: '#timePerWeek',
        4: '#goals'
    };

    function showStep(stepNum) {
        // Hide all steps
        $('.step-content')
            .removeClass('active')
            .css({
                opacity: 0,
                transform: 'translateX(50px)'
            });

        // Update progress bar
        $('.step-item').each(function (index) {
            var itemStep = index + 1;
            if (itemStep <= stepNum) {
                $(this).addClass('active');
            } else {
                $(this).removeClass('active');
            }
        });

        // Show current step with small delay for smoother animation
        setTimeout(function () {
            var $activeStep = $('.step-content[data-step="' + stepNum + '"]');
            $activeStep
                .addClass('active')
                .css({
                    opacity: 1,
                    transform: 'translateX(0)'
                });

            var selector = stepSelectors[stepNum];
            if (selector) {
                var $input = $(selector);
                if ($input.length) {
                    $input.focus();
                }
            }
        }, 200);

        // Update header
        $('#stepTitle').text(steps[stepNum - 1]);
        $('#currentStepNum').text(stepNum);
        $('#currentStepInput').val(stepNum);

        // Update buttons
        var $prevBtn = $('#prevBtn');
        var $nextBtn = $('#nextBtn');

        if ($prevBtn.length) {
            if (stepNum === 1) {
                $prevBtn.addClass('hidden');
            } else {
                $prevBtn.removeClass('hidden');
            }
        }

        if (stepNum === totalSteps) {
            $nextBtn.text('✅ Complete Onboarding')
                .addClass('text-lg px-16');
        } else {
            $nextBtn.text('Next Step →')
                .removeClass('text-lg px-16');
        }
    }

    function getAntiForgeryToken() {
        var $tokenInput = $('input[name="__RequestVerificationToken"]');
        return $tokenInput.length ? $tokenInput.val() : '';
    }

    function submitStep(direction) {
        var $form = $('#onboardingForm');
        var formData = new FormData($form[0]);
        var token = getAntiForgeryToken();

        $.ajax({
            url: '/Instructor/Onboarding',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                'RequestVerificationToken': token
            },
            success: function () {
                if (direction === 'next') {
                    if (currentStep < totalSteps) {
                        currentStep++;
                        showStep(currentStep);
                    } else {
                        window.location.href = '/Instructor/Dashboard?success=true';
                    }
                } else {
                    if (currentStep > 1) {
                        currentStep--;
                        showStep(currentStep);
                    }
                }
            },
            error: function () {
                alert('Please fill the required fields or try again.');
            }
        });
    }

    $('#nextBtn').on('click', function () {
        submitStep('next');
    });

    $('#prevBtn').on('click', function () {
        submitStep('prev');
    });

    $(document).on('keydown', function (e) {
        if (e.key === 'Enter' && e.ctrlKey) {
            e.preventDefault();
            submitStep('next');
        }
    });

    $('textarea, input, select').on('focus', function () {
        var $container = $(this).closest('.space-y-6');
        if ($container.length) {
            $container.css('transform', 'scale(1.02)');
        }
    });

    $('textarea, input, select').on('blur', function () {
        var $container = $(this).closest('.space-y-6');
        if ($container.length) {
            $container.css('transform', 'scale(1)');
        }
    });

    showStep(currentStep);
});
