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
    const res = await fetch(`/api/parties/${partyId}/chat`);
    const messages = await res.json();
    console.log(messages);
    messages.forEach(appendMessage);
}

async function sendMessage(partyId, text) {
    await chatConnection.invoke("SendMessage", partyId, text);
}

function appendMessage(msg) {
    const container = document.getElementById("chat-messages");
    const div = document.createElement("div");
    div.className = "chat-message";
    div.innerHTML = `<strong>${msg.username}</strong> <span>${msg.text}</span>`;
    container.appendChild(div);
    container.scrollTop = container.scrollHeight;
}