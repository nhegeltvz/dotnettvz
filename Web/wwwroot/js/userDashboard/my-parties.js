// ── Edit party button (data-attribute driven) ─────────────────────
document.addEventListener('click', e => {
  const btn = e.target.closest('.js-edit-party');
  if (!btn) return;
  const { partyId, maxMembers, description, locations, dates } = btn.dataset;
  openEditPartyModal(partyId, maxMembers, description, locations, JSON.parse(dates || '[]'));
});

// ── Delete party ─────────────────────────────────────────────────
async function deleteParty(partyId) {
  if (!confirm('Jeste li sigurni da želite izbrisati ovu grupu? Ova radnja je nepovratna.')) return;

  try {
    const res = await fetch(`/api/parties/${partyId}/close`, { method: 'DELETE' });
    if (res.ok) {
      Notify.success('Grupa je izbrisana.');
      setTimeout(() => location.reload(), 1000);
    } else {
      Notify.error('Greška pri brisanju grupe.');
    }
  } catch {
    Notify.error('Greška pri slanju zahtjeva.');
  }
}

// ── Schedule match ────────────────────────────────────────────────
const scheduleOverlay = document.getElementById('schedule-modal-overlay');
const scheduleForm    = document.getElementById('schedule-match-form');

function openScheduleModal(partyId, matchId, playingFieldId, matchDate) {
  document.getElementById('sm-party-id').value          = partyId;
  document.getElementById('sm-match-id').value          = matchId ?? '';
  document.getElementById('sm-playing-field').value     = playingFieldId ?? '';
  document.getElementById('sm-match-date').value        = matchDate ?? '';

  scheduleOverlay.removeAttribute('hidden');
  document.body.classList.add('modal-open');
}

function closeScheduleModal() {
  scheduleOverlay.setAttribute('hidden', '');
  document.body.classList.remove('modal-open');
  scheduleForm.reset();
}

scheduleOverlay?.addEventListener('click', e => { if (e.target === scheduleOverlay) closeScheduleModal(); });
document.addEventListener('keydown', e => { if (e.key === 'Escape' && !scheduleOverlay?.hasAttribute('hidden')) closeScheduleModal(); });

scheduleForm?.addEventListener('submit', async e => {
  e.preventDefault();

  const partyId       = document.getElementById('sm-party-id').value;
  const matchId       = document.getElementById('sm-match-id').value;
  const playingFieldId = document.getElementById('sm-playing-field').value;
  const matchDate     = document.getElementById('sm-match-date').value;

  if (!playingFieldId) { Notify.error('Odaberite teren.'); return; }
  if (!matchDate)      { Notify.error('Odaberite datum i vrijeme.'); return; }

  const isEdit    = !!matchId;
  const url       = isEdit ? `/api/parties/${partyId}/schedule/${matchId}` : `/api/parties/${partyId}/schedule`;
  const submitBtn = scheduleForm.querySelector('[type="submit"]');
  submitBtn.disabled = true;

  try {
    const res = await fetch(url, {
      method: isEdit ? 'PUT' : 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ playingFieldId, matchDate }),
    });

    if (res.ok) {
      Notify.success(isEdit ? 'Meč je ažuriran!' : 'Meč je zakazan!');
      closeScheduleModal();
      setTimeout(() => location.reload(), 1000);
    } else {
      Notify.error('Greška pri spremanju. Pokušajte ponovo.');
    }
  } catch {
    Notify.error('Greška pri slanju zahtjeva.');
  } finally {
    submitBtn.disabled = false;
  }
});

// ── Kick member ───────────────────────────────────────────────────
async function kickMember(partyId, memberId, btn) {
  if (!confirm('Izbaciti ovog igrača iz grupe?')) return;
  btn.disabled = true;

  try {
    const res = await fetch(`/api/parties/${partyId}/kick/${memberId}`, { method: 'DELETE' });
    if (res.ok) {
      Notify.success('Igrač je izbačen.');
      btn.closest('.my-party-member-row').remove();
    } else {
      Notify.error('Greška pri izbacivanju igrača.');
      btn.disabled = false;
    }
  } catch {
    Notify.error('Greška pri slanju zahtjeva.');
    btn.disabled = false;
  }
}
