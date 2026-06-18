function initAi(partyId) {
    const fab     = document.getElementById('ai-fab');
    const panel   = document.getElementById('ai-panel');
    const closeBtn = document.getElementById('ai-close');
    const input   = document.getElementById('ai-input');
    const sendBtn = document.getElementById('ai-send');

    let isOpen = false;

    function openPanel() {
        isOpen = true;
        panel.removeAttribute('hidden');
        panel.offsetHeight;
        panel.classList.add('ai-panel--open');
        fab.classList.add('ai-fab--active');
        input.focus();
    }

    function closePanel() {
        isOpen = false;
        panel.classList.remove('ai-panel--open');
        fab.classList.remove('ai-fab--active');
        panel.addEventListener('transitionend', () => panel.setAttribute('hidden', ''), { once: true });
    }

    fab.addEventListener('click', () => isOpen ? closePanel() : openPanel());
    closeBtn.addEventListener('click', closePanel);

    async function send() {
        const text = input.value.trim();
        if (!text) return;

        sendBtn.disabled = true;
        sendBtn.textContent = '...';

        try {
            const res = await fetch('/api/ai/parse-match', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ text })
            });
            const data = await res.json();
            console.log('AI result:', data);
        } catch (err) {
            console.error('AI error:', err);
        } finally {
            sendBtn.disabled = false;
            sendBtn.innerHTML = '<i class="fa-solid fa-paper-plane"></i>';
        }
    }

    sendBtn.addEventListener('click', send);
    input.addEventListener('keydown', e => { if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); send(); } });
}
