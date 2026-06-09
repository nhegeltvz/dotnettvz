// ── Join / Leave ─────────────────────────────────────────────────
async function joinParty(partyId, btn) {
  btn.disabled = true;
  try {
    const res = await fetch(`/api/partyapi/${partyId}/join`, { method: 'POST' });
    const data = await res.json().catch(() => null);
    if (res.ok) {
      Notify.success('Uspješno ste se pridružili grupi!');
      setTimeout(() => location.reload(), 1200);
    } else {
      Notify.error(data?.message ?? 'Greška pri pridruživanju.');
      btn.disabled = false;
    }
  } catch {
    Notify.error('Greška pri slanju zahtjeva.');
    btn.disabled = false;
  }
}

async function leaveParty(partyId, btn) {
  if (!confirm('Napustiti ovu grupu?')) return;
  btn.disabled = true;
  try {
    const res = await fetch(`/api/partyapi/${partyId}/leave`, { method: 'DELETE' });
    const data = await res.json().catch(() => null);
    if (res.ok) {
      Notify.success('Napustili ste grupu.');
      setTimeout(() => location.reload(), 1200);
    } else {
      Notify.error(data?.message ?? 'Greška pri napuštanju grupe.');
      btn.disabled = false;
    }
  } catch {
    Notify.error('Greška pri slanju zahtjeva.');
    btn.disabled = false;
  }
}

// ── Party form modal (shared create / edit) ───────────────────────
const partyModalOverlay = document.getElementById('party-modal-overlay');
const userPartyForm     = document.getElementById('user-party-form');
let umPreferredDates    = [];

function _umResetModal() {
  userPartyForm?.reset();
  umPreferredDates = [];
  const list = document.getElementById('um-dates-list');
  if (list) list.innerHTML = '';
  document.getElementById('um-party-id').value = '';
  // Clear the flatpickr altInput if it exists
  const altInput = document.querySelector('#party-modal-overlay .flatpickr-input.flatpickr-input--alt, #party-modal-overlay .flatpickr-input[type="text"]');
  if (altInput) altInput.value = '';
}

function _umOpenModal(title, submitLabel) {
  document.getElementById('party-modal-title').textContent = title;
  document.getElementById('um-submit-btn').textContent     = submitLabel;
  partyModalOverlay.removeAttribute('hidden');
  document.body.classList.add('modal-open');
}

// Called from Parties page button
async function openCreatePartyModal() {
  const btn = document.querySelector('.create-party-container');
  const isSignedIn = btn?.dataset.signedIn === 'true';

  if (!isSignedIn) {
    Notify.info('Morate biti prijavljeni kako biste otvorili meč.');
    return;
  }

  try {
    const res = await fetch('/api/partyapi/can-create');
    if (res.status === 401) { Notify.info('Morate biti prijavljeni kako biste otvorili meč.'); return; }
    const data = await res.json();
    if (!data.canCreate) { Notify.info(data.message); return; }
  } catch {
    Notify.error('Greška pri provjeri statusa. Pokušajte ponovo.');
    return;
  }

  _umResetModal();
  _umOpenModal('Otvori svoj meč', 'Otvori meč');
  if (window.initDatetimePickers) {
    initDatetimePickers(document.getElementById('party-modal-overlay'));
  }
}

// Called from MyParties page Uredi button
function openEditPartyModal(partyId, maxMembers, description, locations, existingDates) {
  _umResetModal();

  document.getElementById('um-party-id').value       = partyId;
  document.getElementById('um-max-members').value    = maxMembers;
  document.getElementById('um-description').value    = description;
  document.getElementById('um-locations').value      = locations;

  const list = document.getElementById('um-dates-list');
  (existingDates || []).forEach(val => _umAddDateChip(val, list));

  _umOpenModal('Uredi grupu', 'Spremi');
  if (window.initDatetimePickers) {
    initDatetimePickers(document.getElementById('party-modal-overlay'));
  }
}

function closePartyModal() {
  partyModalOverlay.setAttribute('hidden', '');
  document.body.classList.remove('modal-open');
  _umResetModal();
}

partyModalOverlay?.addEventListener('click', e => { if (e.target === partyModalOverlay) closePartyModal(); });
document.addEventListener('keydown', e => { if (e.key === 'Escape' && !partyModalOverlay?.hasAttribute('hidden')) closePartyModal(); });

// Date chips
function umAddDate() {
  const input = document.getElementById('um-date-input');
  if (!input?.value) return;
  _umAddDateChip(input.value, document.getElementById('um-dates-list'));
  input.value = '';
}

function _umAddDateChip(val, list) {
  if (umPreferredDates.includes(val)) return;
  umPreferredDates.push(val);

  const chip = document.createElement('div');
  chip.className = 'mt-chip';

  const label = document.createElement('span');
  label.textContent = new Date(val).toLocaleString(undefined, { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

  const remove = document.createElement('button');
  remove.type = 'button';
  remove.className = 'mt-chip__remove';
  remove.textContent = '×';
  remove.onclick = () => { umPreferredDates = umPreferredDates.filter(d => d !== val); chip.remove(); };

  chip.appendChild(label);
  chip.appendChild(remove);
  list.appendChild(chip);
}

// Submit — branches on whether id field is populated
userPartyForm?.addEventListener('submit', async e => {
  e.preventDefault();

  const partyId            = document.getElementById('um-party-id').value;
  const maxMembers         = parseInt(document.getElementById('um-max-members').value, 10);
  const partyDescription   = document.getElementById('um-description').value.trim();
  const preferredLocations = document.getElementById('um-locations').value.trim();

  if (!maxMembers || maxMembers < 2 || maxMembers > 20) { Notify.error('Broj igrača mora biti između 2 i 20.'); return; }
  if (partyDescription.length < 10) { Notify.error('Opis mora imati najmanje 10 znakova.'); return; }
  if (preferredLocations.length < 5) { Notify.error('Lokacija mora imati najmanje 5 znakova.'); return; }

  const isEdit = !!partyId;
  const url    = isEdit ? `/api/partyapi/${partyId}/user-edit` : '/api/partyapi/user-create';
  const method = isEdit ? 'PUT' : 'POST';

  const submitBtn = userPartyForm.querySelector('[type="submit"]');
  submitBtn.disabled = true;

  try {
    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ maxMembers, partyDescription, preferredLocations, preferredPlayingDates: umPreferredDates }),
    });

    if (res.ok) {
      Notify.success(isEdit ? 'Grupa je ažurirana!' : 'Meč je uspješno otvoren!');
      closePartyModal();
      setTimeout(() => location.reload(), 1200);
    } else {
      const err = await res.json().catch(() => null);
      const msg = err?.errors ? Object.values(err.errors).flat().join(' ') : 'Greška pri spremanju grupe.';
      Notify.error(msg);
    }
  } catch {
    Notify.error('Greška pri slanju zahtjeva. Pokušajte ponovo.');
  } finally {
    submitBtn.disabled = false;
  }
});
