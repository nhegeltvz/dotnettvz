const Notify = (() => {
  function show(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `mt-toast mt-toast--${type}`;
    toast.textContent = message;
    document.body.appendChild(toast);

    requestAnimationFrame(() => {
      requestAnimationFrame(() => toast.classList.add('mt-toast--visible'));
    });

    setTimeout(() => {
      toast.classList.remove('mt-toast--visible');
      toast.addEventListener('transitionend', () => toast.remove(), { once: true });
    }, 4500);
  }

  return {
    info:    (m) => show(m, 'info'),
    error:   (m) => show(m, 'error'),
    success: (m) => show(m, 'success'),
  };
})();
