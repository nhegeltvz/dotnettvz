window.initDatetimePickers = function (container) {
  container.querySelectorAll("[data-custom-date-picker]").forEach((input) => {
    const fieldName = input.dataset.fieldName;
    const hiddenInput = document.getElementById(fieldName);
    const enableTime = input.dataset.enableTime !== "false";

    const options = {
      enableTime: enableTime,
      dateFormat: enableTime ? "Y-m-d\\TH:i" : "Y-m-d",
      altInput: true,
      altFormat: enableTime ? "j.m.Y H:i" : "j.m.Y",
      clickOpens: true,
      defaultDate: hiddenInput.value || null,
      onChange: function (selectedDates, dateStr) {
        hiddenInput.value = dateStr;
      },
    };

    if (navigator.language.startsWith("hr")) {
      options.locale = "hr";
    }

    const picker = flatpickr(input, options);
    const openPicker = () => picker.open();

    input.addEventListener("click", openPicker);

    if (picker.altInput) {
      picker.altInput.addEventListener("click", openPicker);
    }
  });
};
