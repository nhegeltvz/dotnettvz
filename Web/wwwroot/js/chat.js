const chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .withAutomaticReconnect()
    .build();

chatConnection.on("ReceiveMessage", (msg) => {
    appendMessage(msg);
});

async function startChat(partyId) {
    await chatConnection.start();
    await chatConnection.invoke("JoinParty", partyId);
    await loadHistory(partyId);
}

async function loadHistory(partyId) {
    _lastSeparatorDate = null;
    const res = await fetch(`/api/parties/${partyId}/chat`);
    const messages = await res.json();
    messages.forEach(appendMessage);
}

async function sendMessage(partyId, text) {
    await chatConnection.invoke("SendMessage", partyId, text);
}

let _currentUser = '';
let _lastSeparatorDate = null;

function formatSeparator(date) {
    const today = new Date();
    const yesterday = new Date(today);
    yesterday.setDate(today.getDate() - 1);

    const sameDay = (a, b) =>
        a.getFullYear() === b.getFullYear() &&
        a.getMonth() === b.getMonth() &&
        a.getDate() === b.getDate();

    if (sameDay(date, today)) return 'Danas';
    if (sameDay(date, yesterday)) return 'Jučer';

    const d = String(date.getDate()).padStart(2, '0');
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const y = date.getFullYear();
    return `${d}.${m}.${y}`;
}

function insertDateSeparator(container, date) {
    const sep = document.createElement('div');
    sep.className = 'chat-date-separator';
    sep.textContent = formatSeparator(date);
    container.appendChild(sep);
}

function appendMessage(msg) {
    const container = document.getElementById("chat-messages");
    const sender = msg.senderUsername ?? msg.username ?? '';
    const isMine = sender === _currentUser;

    const sentAt = msg.sentAt ? new Date(msg.sentAt) : null;

    // Date separator
    if (sentAt) {
        const dayKey = `${sentAt.getFullYear()}-${sentAt.getMonth()}-${sentAt.getDate()}`;
        if (dayKey !== _lastSeparatorDate) {
            _lastSeparatorDate = dayKey;
            insertDateSeparator(container, sentAt);
        }
    }

    const timeStr = sentAt
        ? `${String(sentAt.getHours()).padStart(2, '0')}:${String(sentAt.getMinutes()).padStart(2, '0')}`
        : '';

    const bubble = document.createElement("div");
    bubble.className = `chat-bubble ${isMine ? "chat-bubble--mine" : "chat-bubble--theirs"}`;

    bubble.innerHTML = `
        ${!isMine ? `<span class="chat-bubble__name">${sender}</span>` : ''}
        <span class="chat-bubble__text">${msg.text}</span>
        ${timeStr ? `<span class="chat-bubble__time">${timeStr}</span>` : ''}
    `;

    container.appendChild(bubble);
    container.scrollTop = container.scrollHeight;
}

function initChat(partyId, currentUsername) {
    _currentUser = currentUsername;

    const fab     = document.getElementById('chat-fab');
    const modal   = document.getElementById('chat-modal');
    const overlay = document.getElementById('chat-overlay');
    const closeBtn = document.getElementById('chat-close');
    const input   = document.getElementById('chat-input');
    const sendBtn = document.getElementById('chat-send-btn');

    let isOpen = false;

    function openChat() {
        isOpen = true;
        modal.removeAttribute('hidden');
        // force reflow so the animation triggers
        modal.offsetHeight;
        modal.classList.add('chat-modal--open');
        overlay.classList.add('chat-overlay--open');
        fab.classList.add('chat-fab--active');
        document.body.classList.add('modal-open');
        input.focus();
        const msgs = document.getElementById('chat-messages');
        msgs.scrollTop = msgs.scrollHeight;
    }

    function closeChat() {
        isOpen = false;
        modal.classList.remove('chat-modal--open');
        overlay.classList.remove('chat-overlay--open');
        fab.classList.remove('chat-fab--active');
        document.body.classList.remove('modal-open');
        modal.addEventListener('transitionend', () => modal.setAttribute('hidden', ''), { once: true });
    }

    fab.addEventListener('click', () => isOpen ? closeChat() : openChat());
    closeBtn.addEventListener('click', closeChat);
    overlay.addEventListener('click', closeChat);

    function submit() {
        const text = input.value.trim();
        if (!text) return;
        sendMessage(partyId, text);
        input.value = '';
    }

    input.addEventListener('keydown', e => { if (e.key === 'Enter') submit(); });
    sendBtn.addEventListener('click', submit);

    startChat(partyId);
}
