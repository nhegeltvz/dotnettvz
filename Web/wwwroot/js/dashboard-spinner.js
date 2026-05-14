(function () {
  const spinner = document.getElementById("dashboard-spinner");
  if (!spinner) {
    return;
  }

  const setVisible = (visible) => {
    spinner.hidden = !visible;
  };

  window.dashboardSpinner = {
    show: () => setVisible(true),
    hide: () => setVisible(false),
  };
})();
