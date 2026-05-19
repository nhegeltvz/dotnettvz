window.initDatetimePickers = function (container) {
  const locale = navigator.language.startsWith("hr") ? "hr" : "default";

  container.querySelectorAll("[data-custom-date-picker]").forEach((input) => {
    const fieldName = input.dataset.fieldName;
    const hiddenInput = document.getElementById(fieldName);

    const options = {
      enableTime: true,
      dateFormat: "Y-m-d\\TH:i",
      altInput: true,
      altFormat: "j.m.Y H:i",
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
