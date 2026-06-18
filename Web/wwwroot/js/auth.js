(() => {
  const bars = [
    {
      inputId: 'Input_Username',
      serverField: 'Input.Username',
      label: 'Korisničko ime',
      validate: v => v.trim().length >= 3,
    },
    {
      inputId: 'Input_Email',
      serverField: 'Input.Email',
      label: 'Email',
      validate: v => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v.trim()),
    },
    {
      inputId: 'Input_Password',
      serverField: 'Input.Password',
      label: 'Lozinka',
      validate: v => v.length >= 6,
    },
    {
      inputId: 'Input_ConfirmPassword',
      serverField: 'Input.ConfirmPassword',
      label: 'Lozinke se podudaraju',
      validate: (v) => {
        const pw = document.getElementById('Input_Password');
        return pw && v.length > 0 && v === pw.value;
      },
    },
  ];

  function buildTracker() {
    const form = document.getElementById('registerForm');
    if (!form) return;

    const wrapper = document.createElement('div');
    wrapper.className = 'auth-progress';

    bars.forEach((_, i) => {
      const track = document.createElement('div');
      track.className = 'auth-progress__bar';
      track.id = `auth-bar-${i}`;
      wrapper.appendChild(track);
    });

    const title = document.querySelector('.auth-card__title');
    title.insertAdjacentElement('afterend', wrapper);
  }

  function hasServerError(bar) {
    const span = document.querySelector(`[data-valmsg-for="${bar.serverField}"]`);
    return span && span.classList.contains('field-validation-error') && span.textContent.trim() !== '';
  }

  function updateBars() {
    bars.forEach((bar, i) => {
      const input = document.getElementById(bar.inputId);
      const track = document.getElementById(`auth-bar-${i}`);
      if (!input || !track) return;

      const serverError = hasServerError(bar);
      const clientOk = bar.validate(input.value);

      track.classList.toggle('auth-progress__bar--active', clientOk && !serverError);
      track.classList.toggle('auth-progress__bar--server-error', serverError);
    });
  }

  document.addEventListener('DOMContentLoaded', () => {
    buildTracker();
    updateBars(); // apply server-side state on page load after failed POST

    bars.forEach(bar => {
      const input = document.getElementById(bar.inputId);
      if (!input) return;
      input.addEventListener('input', () => {
        // clear server error state once user starts correcting the field
        const span = document.querySelector(`[data-valmsg-for="${bar.serverField}"]`);
        if (span) span.classList.remove('field-validation-error');
        updateBars();
      });
    });
  });
})();
