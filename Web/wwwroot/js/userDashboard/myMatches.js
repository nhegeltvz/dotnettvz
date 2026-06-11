// ── Stats save ──────────────────────────────────────────────
    async function saveStats(matchPlayerId, btn) {
  const card = btn.closest('.my-match-card');
    const goals   = parseInt(card.querySelector('.mp-goals').value,   10);
    const assists = parseInt(card.querySelector('.mp-assists').value, 10);

    if (isNaN(goals) || isNaN(assists) || goals < 0 || assists < 0) {
        Notify.error('Unesite ispravne vrijednosti.'); return;
  }

    btn.disabled = true;
    try {
    const res = await fetch(`/api/matchplayerapi/${matchPlayerId}/stats`, {
        method: 'PUT',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify({goals, assists}),
    });
    if (res.ok) {
        Notify.success('Statistika je spremljena!');
    } else {
      const err = await res.json().catch(() => null);
    Notify.error(err?.message ?? 'Greška pri spremanju.');
    }
  } catch {
        Notify.error('Greška pri slanju zahtjeva.');
  } finally {
        btn.disabled = false;
  }
}

    // ── Rating ────────────────────────────────────────────────
    let _ratingMatchPlayerId = null;
    let _ratingTargetMatchPlayerId = null;

    function closeRatingModal() {
        document.getElementById('rating-modal-overlay').setAttribute('hidden', '');
    document.body.classList.remove('modal-open');
}

    async function loadRatingTarget(matchPlayerId, btn) {
        btn.disabled = true;
    try {
    const res = await fetch(`/api/matchplayerapi/${matchPlayerId}/rating-target`);
    const data = await res.json();

    if (data.alreadyRated) {Notify.info('Već ste ocijenili igrača za ovaj meč.'); btn.disabled = false; return; }
    if (data.noTarget)     {Notify.info('Nema drugih igrača za ocjenjivanje.'); btn.disabled = false; return; }

    _ratingMatchPlayerId = matchPlayerId;
    _ratingTargetMatchPlayerId = data.targetMatchPlayerId;

    document.getElementById('rating-modal-body').innerHTML = `
    <p style="color:var(--mt-text); margin-bottom:1.25rem;">
        Vaš slučajno odabrani igrač za ocjenu:<br>
            <strong style="font-size:1.15rem; color:var(--mt-highlight)">${data.targetPlayerName}</strong>
    </p>
    <div style="display:flex;flex-direction:column;gap:0.5rem;">
        <label style="font-size:0.8rem;font-weight:700;text-transform:uppercase;letter-spacing:0.06rem;color:var(--mt-text-subtle);">
            Ocjena (1–10)
        </label>
        <input type="range" id="rating-slider" min="1" max="10" value="7"
            style="accent-color:var(--mt-highlight);width:100%;"
            oninput="document.getElementById('rating-value').textContent = this.value" />
        <span id="rating-value"
            style="text-align:center;font-family:var(--font-heading);font-size:2rem;color:var(--mt-highlight);">7</span>
    </div>
    <div class="mt-modal-actions" style="margin-top:1.25rem;">
        <button class="mt-modal-btn mt-modal-btn--cancel" onclick="closeRatingModal()">Odustani</button>
        <button class="mt-modal-btn mt-modal-btn--submit" onclick="submitRating()">
            <i class="fa-solid fa-star"></i> Pošalji ocjenu
        </button>
    </div>`;

    document.getElementById('rating-modal-overlay').removeAttribute('hidden');
    document.body.classList.add('modal-open');
  } catch {
        Notify.error('Greška pri dohvaćanju cilja.');
  } finally {
        btn.disabled = false;
  }
}

    async function submitRating() {
  const rating = parseInt(document.getElementById('rating-slider').value, 10);
    try {
    const res = await fetch(`/api/matchplayerapi/${_ratingMatchPlayerId}/rate`, {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify({targetMatchPlayerId: _ratingTargetMatchPlayerId, rating }),
    });
    if (res.ok) {
        Notify.success('Ocjena je uspješno poslana!');
    closeRatingModal();
      setTimeout(() => location.reload(), 900);
    } else {
      const err = await res.json().catch(() => null);
    Notify.error(err?.message ?? 'Greška pri slanju ocjene.');
    }
  } catch {
        Notify.error('Greška pri slanju zahtjeva.');
  }
}

document.getElementById('rating-modal-overlay').addEventListener('click', e => {
  if (e.target === document.getElementById('rating-modal-overlay')) closeRatingModal();
});